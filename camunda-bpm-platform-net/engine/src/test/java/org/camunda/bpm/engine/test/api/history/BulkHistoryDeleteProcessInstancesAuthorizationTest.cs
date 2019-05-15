﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.test.api.history
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class BulkHistoryDeleteProcessInstancesAuthorizationTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BulkHistoryDeleteProcessInstancesAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BulkHistoryDeleteProcessInstancesAuthorizationTest()
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


	  protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain chain;

	  private HistoryService historyService;
	  private RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;

		authRule.createUserAndGroup("demo", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().failsDueToRequired(grant(Resources.PROCESS_DEFINITION, "processDefinition", "demo", Permissions.DELETE_HISTORY)), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinition", "demo", Permissions.DELETE_HISTORY)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "*", "demo", Permissions.DELETE_HISTORY)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testCleanupHistory()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testCleanupHistory()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();
		runtimeService.deleteProcessInstances(ids, null, true, true);

		// when
		authRule.init(scenario).bindResource("processDefinition", "oneTaskProcess").withUser("demo").start();

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		}

	  }

	  private IList<string> prepareHistoricProcesses()
	  {
		return prepareHistoricProcesses(ONE_TASK_PROCESS);
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey)
	  {
		return prepareHistoricProcesses(businessKey, null);
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey, VariableMap variables)
	  {
		IList<string> processInstanceIds = new List<string>();

		for (int i = 0; i < 5; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(businessKey, variables);
		  processInstanceIds.Add(processInstance.Id);
		}

		return processInstanceIds;
	  }

	}

}