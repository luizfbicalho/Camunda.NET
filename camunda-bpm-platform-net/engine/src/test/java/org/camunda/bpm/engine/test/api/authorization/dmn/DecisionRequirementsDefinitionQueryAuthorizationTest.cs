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
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_REQUIREMENTS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class DecisionRequirementsDefinitionQueryAuthorizationTest
	public class DecisionRequirementsDefinitionQueryAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionRequirementsDefinitionQueryAuthorizationTest()
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


	  protected internal const string DMN_FILE = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";
	  protected internal const string ANOTHER_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal const string DEFINITION_KEY = "score";
	  protected internal const string ANOTHER_DEFINITION_KEY = "dish";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;

	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public String[] expectedDefinitionKeys;
	  public string[] expectedDefinitionKeys;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "scenario {index}") public static java.util.Collection<Object[]> scenarios()
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {scenario().withoutAuthorizations().succeeds(), expectedDefinitions()},
			new object[] {scenario().withAuthorizations(grant(DECISION_REQUIREMENTS_DEFINITION, DEFINITION_KEY, "userId", Permissions.READ)).succeeds(), expectedDefinitions(DEFINITION_KEY)},
			new object[] {scenario().withAuthorizations(grant(DECISION_REQUIREMENTS_DEFINITION, ANY, "userId", Permissions.READ)).succeeds(), expectedDefinitions(DEFINITION_KEY, ANOTHER_DEFINITION_KEY)},
			new object[] {scenario().withAuthorizations(grant(DECISION_REQUIREMENTS_DEFINITION, DEFINITION_KEY, "userId", Permissions.READ), grant(DECISION_REQUIREMENTS_DEFINITION, ANY, "userId", Permissions.READ)).succeeds(), expectedDefinitions(DEFINITION_KEY, ANOTHER_DEFINITION_KEY)}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { DMN_FILE, ANOTHER_DMN }) public void queryDecisionRequirementsDefinitions()
	  [Deployment(resources : { DMN_FILE, ANOTHER_DMN })]
	  public virtual void queryDecisionRequirementsDefinitions()
	  {

		// when
		authRule.init(scenario).withUser("userId").bindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).start();

		DecisionRequirementsDefinitionQuery query = engineRule.RepositoryService.createDecisionRequirementsDefinitionQuery();
		long count = query.count();

		// then
		if (authRule.assertScenario(scenario))
		{
		  assertThat(count, @is((long) expectedDefinitionKeys.Length));

		  IList<string> definitionKeys = getDefinitionKeys(query.list());
		  assertThat(definitionKeys, hasItems(expectedDefinitionKeys));
		}
	  }

	  protected internal virtual IList<string> getDefinitionKeys(IList<DecisionRequirementsDefinition> definitions)
	  {
		IList<string> definitionKeys = new List<string>();
		foreach (DecisionRequirementsDefinition definition in definitions)
		{
		  definitionKeys.Add(definition.Key);
		}
		return definitionKeys;
	  }

	  protected internal static string[] expectedDefinitions(params string[] keys)
	  {
		return keys;
	  }

	}

}