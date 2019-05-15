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
namespace org.camunda.bpm.engine.test.api.authorization
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class DeleteProcessDefinitionAuthorizationTest
	public class DeleteProcessDefinitionAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public DeleteProcessDefinitionAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  public const string PROCESS_DEFINITION_KEY = "one";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;
	  public ProcessEngineTestRule testHelper;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.PROCESS_DEFINITION, PROCESS_DEFINITION_KEY, "userId", Permissions.DELETE)), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, PROCESS_DEFINITION_KEY, "userId", Permissions.DELETE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.DELETE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
		repositoryService = null;
		runtimeService = null;
		processEngineConfiguration = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinition()
	  public virtual void testDeleteProcessDefinition()
	  {
		testHelper.deploy("org/camunda/bpm/engine/test/repository/twoProcesses.bpmn20.xml");
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		authRule.init(scenario).withUser("userId").start();

		//when a process definition is been deleted
		repositoryService.deleteProcessDefinition(processDefinitions[0].Id);

		//then only one process definition should remain
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(1, repositoryService.createProcessDefinitionQuery().count());
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionCascade()
	  public virtual void testDeleteProcessDefinitionCascade()
	  {
		// given process definition and a process instance
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done();
		testHelper.deploy(bpmnModel);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).executeWithVariablesInReturn();

		authRule.init(scenario).withUser("userId").start();

		//when the corresponding process definition is cascading deleted from the deployment
		repositoryService.deleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and no definition
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		  assertEquals(0, repositoryService.createProcessDefinitionQuery().count());
		  if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		  {
			assertEquals(0, engineRule.HistoryService.createHistoricActivityInstanceQuery().count());
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKey()
	  public virtual void testDeleteProcessDefinitionsByKey()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinition();
		}

		authRule.init(scenario).withUser("userId").start();

		// when
		repositoryService.deleteProcessDefinitions().byKey(PROCESS_DEFINITION_KEY).withoutTenantId().delete();

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, repositoryService.createProcessDefinitionQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyCascade()
	  public virtual void testDeleteProcessDefinitionsByKeyCascade()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinition();
		}

		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		authRule.init(scenario).withUser("userId").start();

		// when
		repositoryService.deleteProcessDefinitions().byKey(PROCESS_DEFINITION_KEY).withoutTenantId().cascade().delete();

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		  assertEquals(0, repositoryService.createProcessDefinitionQuery().count());

		  if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		  {
			assertEquals(0, historyService.createHistoricActivityInstanceQuery().count());
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIds()
	  public virtual void testDeleteProcessDefinitionsByIds()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinition();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey(PROCESS_DEFINITION_KEY);

		authRule.init(scenario).withUser("userId").start();

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).delete();

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, repositoryService.createProcessDefinitionQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsCascade()
	  public virtual void testDeleteProcessDefinitionsByIdsCascade()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployProcessDefinition();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey(PROCESS_DEFINITION_KEY);

		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		authRule.init(scenario).withUser("userId").start();

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).cascade().delete();

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		  assertEquals(0, repositoryService.createProcessDefinitionQuery().count());

		  if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		  {
			assertEquals(0, historyService.createHistoricActivityInstanceQuery().count());
		  }
		}
	  }

	  private void deployProcessDefinition()
	  {
		testHelper.deploy(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done());
	  }

	  private string[] findProcessDefinitionIdsByKey(string processDefinitionKey)
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).list();
		IList<string> processDefinitionIds = new List<string>();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  processDefinitionIds.Add(processDefinition.Id);
		}

		return processDefinitionIds.ToArray();
	  }

	}

}