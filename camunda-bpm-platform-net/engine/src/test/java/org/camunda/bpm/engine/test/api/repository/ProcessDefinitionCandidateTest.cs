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
namespace org.camunda.bpm.engine.test.api.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class ProcessDefinitionCandidateTest
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessDefinitionCandidateTest()
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


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string CANDIDATE_STARTER_USER = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterUser.bpmn20.xml";
	  protected internal const string CANDIDATE_STARTER_USERS = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterUsers.bpmn20.xml";

	  protected internal const string CANDIDATE_STARTER_GROUP = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterGroup.bpmn20.xml";
	  protected internal const string CANDIDATE_STARTER_GROUPS = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterGroups.bpmn20.xml";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPropagateTenantIdToCandidateStarterUser()
	  public virtual void shouldPropagateTenantIdToCandidateStarterUser()
	  {
		// when
		DeploymentBuilder builder = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_USER).tenantId(TENANT_ONE);
		testRule.deploy(builder);

		// then
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(processDefinition.Id);
		assertEquals(1, links.Count);

		IdentityLink link = links[0];
		assertNotNull(link.TenantId);
		assertEquals(TENANT_ONE, link.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPropagateTenantIdToCandidateStarterUsers()
	  public virtual void shouldPropagateTenantIdToCandidateStarterUsers()
	  {
		// when
		DeploymentBuilder builder = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_USERS).tenantId(TENANT_ONE);
		testRule.deploy(builder);

		// then
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(processDefinition.Id);
		assertEquals(3, links.Count);

		foreach (IdentityLink link in links)
		{
		  assertNotNull(link.TenantId);
		  assertEquals(TENANT_ONE, link.TenantId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPropagateTenantIdToCandidateStarterGroup()
	  public virtual void shouldPropagateTenantIdToCandidateStarterGroup()
	  {
		// when
		DeploymentBuilder builder = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_GROUP).tenantId(TENANT_ONE);
		testRule.deploy(builder);

		// then
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(processDefinition.Id);
		assertEquals(1, links.Count);

		IdentityLink link = links[0];
		assertNotNull(link.TenantId);
		assertEquals(TENANT_ONE, link.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPropagateTenantIdToCandidateStarterGroups()
	  public virtual void shouldPropagateTenantIdToCandidateStarterGroups()
	  {
		// when
		DeploymentBuilder builder = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_GROUPS).tenantId(TENANT_ONE);
		testRule.deploy(builder);

		// then
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(processDefinition.Id);
		assertEquals(3, links.Count);

		foreach (IdentityLink link in links)
		{
		  assertNotNull(link.TenantId);
		  assertEquals(TENANT_ONE, link.TenantId);
		}
	  }
	}

}