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
namespace org.camunda.bpm.engine.test.api.dmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Tobias Metzke
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class DecisionServiceUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionServiceUserOperationLogTest()
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


	  protected internal const string DMN_DECISION_TABLE = "org/camunda/bpm/engine/test/api/dmn/Example.dmn";
	  protected internal const string DMN_DECISION_TABLE_V2 = "org/camunda/bpm/engine/test/api/dmn/Example_v2.dmn";

	  protected internal const string DMN_DECISION_LITERAL_EXPRESSION = "org/camunda/bpm/engine/test/api/dmn/DecisionWithLiteralExpression.dmn";
	  protected internal const string DMN_DECISION_LITERAL_EXPRESSION_V2 = "org/camunda/bpm/engine/test/api/dmn/DecisionWithLiteralExpression_v2.dmn";

	  protected internal const string DECISION_DEFINITION_KEY = "decision";

	  protected internal const string USER_ID = "userId";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal DecisionService decisionService;
	  protected internal RepositoryService repositoryService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		decisionService = engineRule.DecisionService;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		historyService = engineRule.HistoryService;
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void logCreationOnEvaluateDecisionTableById()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void logCreationOnEvaluateDecisionTableById()
	  {
		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionTableById(decisionDefinition.Id, createVariables());
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void logCreationOnEvaluateDecisionTableByKey()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void logCreationOnEvaluateDecisionTableByKey()
	  {
		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, createVariables());
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void logCreationOnEvaluateDecisionTableByKeyAndLatestVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void logCreationOnEvaluateDecisionTableByKeyAndLatestVersion()
	  {
		testRule.deploy(DMN_DECISION_TABLE_V2);

		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().latestVersion().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, createVariables());
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void logCreationOnEvaluateDecisionTableByKeyAndVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void logCreationOnEvaluateDecisionTableByKeyAndVersion()
	  {
		testRule.deploy(DMN_DECISION_TABLE_V2);

		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionVersion(1).singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionTableByKeyAndVersion(DECISION_DEFINITION_KEY, 1, createVariables());
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void logCreationOnEvaluateDecisionTableByKeyAndNullVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void logCreationOnEvaluateDecisionTableByKeyAndNullVersion()
	  {
		testRule.deploy(DMN_DECISION_TABLE_V2);

		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().latestVersion().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionTableByKeyAndVersion(DECISION_DEFINITION_KEY, null, createVariables());
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void logCreationOnEvaluateDecisionById()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void logCreationOnEvaluateDecisionById()
	  {
		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionById(decisionDefinition.Id).variables(createVariables()).evaluate();
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void logCreationOnEvaluateDecisionByKey()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void logCreationOnEvaluateDecisionByKey()
	  {
		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void logCreationOnEvaluateDecisionByKeyAndLatestVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void logCreationOnEvaluateDecisionByKeyAndLatestVersion()
	  {
		testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().latestVersion().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void logCreationOnEvaluateDecisionByKeyAndVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void logCreationOnEvaluateDecisionByKeyAndVersion()
	  {
		testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionVersion(1).singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).version(1).variables(createVariables()).evaluate();
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void logCreationOnEvaluateDecisionByKeyAndNullVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void logCreationOnEvaluateDecisionByKeyAndNullVersion()
	  {
		testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

		// given
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().latestVersion().singleResult();

		// when
		identityService.AuthenticatedUserId = USER_ID;
		decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).version(null).variables(createVariables()).evaluate();
		identityService.clearAuthentication();

		// then
		assertOperationLog(decisionDefinition);
	  }

	  protected internal virtual VariableMap createVariables()
	  {
		return Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
	  }

	  protected internal virtual void assertOperationLog(DecisionDefinition definition)
	  {
		assertThat(historyService.createUserOperationLogQuery().count(), @is(2L));
		assertLogEntry("decisionDefinitionId", definition.Id);
		assertLogEntry("decisionDefinitionKey", definition.Key);
	  }

	  protected internal virtual void assertLogEntry(string property, object newValue)
	  {
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().property(property).singleResult();
		assertThat(entry, notNullValue());
		assertThat(entry.OrgValue, nullValue());
		assertThat(entry.NewValue, @is(newValue.ToString()));
		assertThat(entry.Category, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR));
		assertThat(entry.EntityType, @is(EntityTypes.DECISION_DEFINITION));
		assertThat(entry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_EVALUATE));
	  }

	}

}