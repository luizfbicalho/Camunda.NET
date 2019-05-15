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
namespace org.camunda.bpm.integrationtest.functional.cdi
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExampleBean = org.camunda.bpm.integrationtest.functional.cdi.beans.ExampleBean;
	using PriorityBean = org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using ByteArrayAsset = org.jboss.shrinkwrap.api.asset.ByteArrayAsset;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.application.ProcessApplicationContext.withProcessApplicationContext;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CdiBeanResolutionTwoEnginesTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CdiBeanResolutionTwoEnginesTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name: "engine1", order : 1)]
		public static WebArchive createDeployment()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.shrinkwrap.api.spec.WebArchive webArchive = initWebArchiveDeployment("paEngine1.war", "org/camunda/bpm/integrationtest/paOnEngine1.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanResolutionTwoEnginesTest.testResolveBean.bpmn20.xml").addAsLibraries(org.camunda.bpm.integrationtest.util.DeploymentHelper.getEngineCdi());
		WebArchive webArchive = initWebArchiveDeployment("paEngine1.war", "org/camunda/bpm/integrationtest/paOnEngine1.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanResolutionTwoEnginesTest.testResolveBean.bpmn20.xml").addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificProcessEngineConfigurationClass(webArchive);
		return webArchive;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("engine1") public void testResolveBean() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testResolveBean()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.ProcessEngine processEngine1 = processEngineService.getProcessEngine("engine1");
		ProcessEngine processEngine1 = processEngineService.getProcessEngine("engine1");
		Assert.assertEquals("engine1", processEngine1.Name);
		createAuthorizations(processEngine1);

		//when we operate the process under authenticated user
		processEngine1.IdentityService.setAuthentication("user1", Arrays.asList("group1"));

		processEngine1.RuntimeService.startProcessInstanceByKey("testProcess");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.task.Task> tasks = processEngine1.getTaskService().createTaskQuery().list();
		IList<Task> tasks = processEngine1.TaskService.createTaskQuery().list();
		Assert.assertEquals(1, tasks.Count);
		processEngine1.TaskService.complete(tasks[0].Id);

		//then
		//identityService resolution respects the engine, on which the process is being executed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.VariableInstance> variableInstances = processEngine1.getRuntimeService().createVariableInstanceQuery().variableName("changeInitiatorUsername").list();
		IList<VariableInstance> variableInstances = processEngine1.RuntimeService.createVariableInstanceQuery().variableName("changeInitiatorUsername").list();
		Assert.assertEquals(1, variableInstances.Count);
		Assert.assertEquals("user1", variableInstances[0].Value);
	  }

	  private void createAuthorizations(ProcessEngine processEngine1)
	  {
		Authorization newAuthorization = processEngine1.AuthorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		newAuthorization.Resource = Resources.PROCESS_INSTANCE;
		newAuthorization.ResourceId = "*";
		newAuthorization.Permissions = new Permission[] {Permissions.CREATE};
		processEngine1.AuthorizationService.saveAuthorization(newAuthorization);

		newAuthorization = processEngine1.AuthorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		newAuthorization.Resource = Resources.PROCESS_DEFINITION;
		newAuthorization.ResourceId = "*";
		newAuthorization.Permissions = new Permission[] {Permissions.CREATE_INSTANCE};
		processEngine1.AuthorizationService.saveAuthorization(newAuthorization);

		newAuthorization = processEngine1.AuthorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		newAuthorization.Resource = Resources.TASK;
		newAuthorization.ResourceId = "*";
		newAuthorization.Permissions = new Permission[] {Permissions.READ, Permissions.TASK_WORK};
		processEngine1.AuthorizationService.saveAuthorization(newAuthorization);
	  }

	}

}