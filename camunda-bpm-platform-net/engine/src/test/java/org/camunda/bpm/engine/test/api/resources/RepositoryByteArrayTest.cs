using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.test.api.resources
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.repository.ResourceTypes.REPOSITORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using Picture = org.camunda.bpm.engine.identity.Picture;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Resource = org.camunda.bpm.engine.repository.Resource;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class RepositoryByteArrayTest
	{
		private bool InstanceFieldsInitialized = false;

		public RepositoryByteArrayTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}

	  protected internal const string USER_ID = "johndoe";
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal TaskService taskService;
	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		configuration = engineRule.ProcessEngineConfiguration;
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		identityService.deleteUser(USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResourceBinary()
	  public virtual void testResourceBinary()
	  {
		DateTime fixedDate = DateTime.Now;
		ClockUtil.CurrentTime = fixedDate;

		string bpmnDeploymentId = testRule.deploy("org/camunda/bpm/engine/test/repository/one.bpmn20.xml").Id;
		string dmnDeploymentId = testRule.deploy("org/camunda/bpm/engine/test/repository/one.dmn").Id;
		string cmmnDeplymentId = testRule.deploy("org/camunda/bpm/engine/test/repository/one.cmmn").Id;

		checkResource(fixedDate, bpmnDeploymentId);
		checkResource(fixedDate, dmnDeploymentId);
		checkResource(fixedDate, cmmnDeplymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormsBinaries()
	  public virtual void testFormsBinaries()
	  {
		DateTime fixedDate = DateTime.Now;
		ClockUtil.CurrentTime = fixedDate;

		string deploymentId = testRule.deploy("org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form", "org/camunda/bpm/engine/test/api/authorization/renderedFormProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn").Id;

		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deploymentId);
		assertEquals(5, deploymentResources.Count);
		foreach (Resource resource in deploymentResources)
		{
		  ResourceEntity entity = (ResourceEntity) resource;
		  checkEntity(fixedDate, entity);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserPictureBinary()
	  public virtual void testUserPictureBinary()
	  {
		// when
		DateTime fixedDate = DateTime.Now;
		ClockUtil.CurrentTime = fixedDate;
		User user = identityService.newUser(USER_ID);
		identityService.saveUser(user);
		string userId = user.Id;

		Picture picture = new Picture("niceface".GetBytes(), "image/string");
		identityService.setUserPicture(userId, picture);
		string userInfo = identityService.getUserInfo(USER_ID, "picture");

		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(userInfo));

		// then
		assertNotNull(byteArrayEntity);
		assertEquals(fixedDate.ToString(), byteArrayEntity.CreateTime.ToString());
		assertEquals(REPOSITORY.Value, byteArrayEntity.Type);
	  }


	  protected internal virtual void checkResource(DateTime expectedDate, string deploymentId)
	  {
		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deploymentId);
		assertEquals(1, deploymentResources.Count);
		ResourceEntity resource = (ResourceEntity) deploymentResources[0];
		checkEntity(expectedDate, resource);
	  }

	  protected internal virtual void checkEntity(DateTime expectedDate, ResourceEntity entity)
	  {
		assertNotNull(entity);
		assertEquals(expectedDate.ToString(), entity.CreateTime.ToString());
		assertEquals(REPOSITORY.Value, entity.Type);
	  }
	}

}