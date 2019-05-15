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
namespace org.camunda.bpm.engine.test.api.history
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
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
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class BulkHistoryDeleteDecisionInstancesAuthorizationTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BulkHistoryDeleteDecisionInstancesAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BulkHistoryDeleteDecisionInstancesAuthorizationTest()
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


	  public const string DECISION = "decision";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain chain;

	  private HistoryService historyService;
	  private DecisionService decisionService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		historyService = engineRule.HistoryService;
		decisionService = engineRule.DecisionService;

		authRule.createUserAndGroup("demo", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().failsDueToRequired(grant(Resources.DECISION_DEFINITION, "*", "demo", Permissions.DELETE_HISTORY)), scenario().withAuthorizations(grant(Resources.DECISION_DEFINITION, "someId", "demo", Permissions.DELETE_HISTORY)).failsDueToRequired(grant(Resources.DECISION_DEFINITION, "*", "demo", Permissions.DELETE_HISTORY)), scenario().withAuthorizations(grant(Resources.DECISION_DEFINITION, "*", "demo", Permissions.DELETE_HISTORY)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/dmn/Example.dmn"}) public void testCleanupHistory()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/dmn/Example.dmn"})]
	  public virtual void testCleanupHistory()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricDecisions();
		IList<string> ids = prepareHistoricDecisions();

		// when
		authRule.init(scenario).withUser("demo").start();

		historyService.deleteHistoricDecisionInstancesBulk(ids);

		//then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals(0, historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION).count());
		}

	  }

	  private IList<string> prepareHistoricDecisions()
	  {
		for (int i = 0; i < 5; i++)
		{
		  decisionService.evaluateDecisionByKey(DECISION).variables(createVariables()).evaluate();
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricDecisionInstance> decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		IList<HistoricDecisionInstance> decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> decisionInstanceIds = new java.util.ArrayList<String>();
		IList<string> decisionInstanceIds = new List<string>();
		foreach (HistoricDecisionInstance decisionInstance in decisionInstances)
		{
		  decisionInstanceIds.Add(decisionInstance.Id);
		}
		return decisionInstanceIds;
	  }

	  protected internal virtual VariableMap createVariables()
	  {
		return Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
	  }

	}

}