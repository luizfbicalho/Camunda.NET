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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
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

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class HistoricDecisionInstanceStatisticsAuthorizationTest
	public class HistoricDecisionInstanceStatisticsAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricDecisionInstanceStatisticsAuthorizationTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const string DISH_DRG_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;
	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal RepositoryService repositoryService;

	  protected internal DecisionRequirementsDefinition decisionRequirementsDefinition;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.DECISION_REQUIREMENTS_DEFINITION, "dish", "userId", Permissions.READ)), scenario().withAuthorizations(grant(Resources.DECISION_REQUIREMENTS_DEFINITION, "drd", "userId", Permissions.READ)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		testHelper.deploy(DISH_DRG_DMN);
		decisionService = engineRule.DecisionService;
		historyService = engineRule.HistoryService;
		repositoryService = engineRule.RepositoryService;

		authRule.createUserAndGroup("userId", "groupId");

		decisionService.evaluateDecisionTableByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend")).evaluate();

		decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateStatistics()
	  public virtual void testCreateStatistics()
	  {
		//given
		authRule.init(scenario).withUser("userId").bindResource("drd", "*").start();

		// when
		historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id).list();

		// then
		authRule.assertScenario(scenario);
	  }

	}

}