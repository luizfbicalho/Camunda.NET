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
namespace org.camunda.bpm.engine.test.api.authorization.dmn
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
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
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class EvaluateDecisionAuthorizationTest
	public class EvaluateDecisionAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public EvaluateDecisionAuthorizationTest()
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
			chain = RuleChain.outerRule(engineRule).around(authRule);
		}


	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/api/dmn/Example.dmn";
	  protected internal const string DECISION_DEFINITION_KEY = "decision";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.DECISION_DEFINITION, DECISION_DEFINITION_KEY, "userId", Permissions.CREATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.DECISION_DEFINITION, DECISION_DEFINITION_KEY, "userId", Permissions.CREATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.DECISION_DEFINITION, "*", "userId", Permissions.CREATE_INSTANCE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = DMN_FILE) public void evaluateDecisionById()
	  [Deployment(resources : DMN_FILE)]
	  public virtual void evaluateDecisionById()
	  {

		// given
		DecisionDefinition decisionDefinition = engineRule.RepositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionDefinitionKey", DECISION_DEFINITION_KEY).start();

		DmnDecisionTableResult decisionResult = engineRule.DecisionService.evaluateDecisionTableById(decisionDefinition.Id, createVariables());

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertThatDecisionHasExpectedResult(decisionResult);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = DMN_FILE) public void evaluateDecisionByKey()
	  [Deployment(resources : DMN_FILE)]
	  public virtual void evaluateDecisionByKey()
	  {

		// given
		DecisionDefinition decisionDefinition = engineRule.RepositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionDefinitionKey", DECISION_DEFINITION_KEY).start();

		DmnDecisionTableResult decisionResult = engineRule.DecisionService.evaluateDecisionTableByKey(decisionDefinition.Key, createVariables());

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertThatDecisionHasExpectedResult(decisionResult);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = DMN_FILE) public void evaluateDecisionByKeyAndVersion()
	  [Deployment(resources : DMN_FILE)]
	  public virtual void evaluateDecisionByKeyAndVersion()
	  {

		// given
		DecisionDefinition decisionDefinition = engineRule.RepositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionDefinitionKey", DECISION_DEFINITION_KEY).start();

		DmnDecisionTableResult decisionResult = engineRule.DecisionService.evaluateDecisionTableByKeyAndVersion(decisionDefinition.Key, decisionDefinition.Version, createVariables());

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertThatDecisionHasExpectedResult(decisionResult);
		}
	  }

	  protected internal virtual VariableMap createVariables()
	  {
		return Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
	  }

	  protected internal virtual void assertThatDecisionHasExpectedResult(DmnDecisionTableResult decisionResult)
	  {
		assertThat(decisionResult, @is(notNullValue()));
		assertThat(decisionResult.size(), @is(1));
		string value = decisionResult.SingleResult.FirstEntry;
		assertThat(value, @is("ok"));
	  }

	}

}