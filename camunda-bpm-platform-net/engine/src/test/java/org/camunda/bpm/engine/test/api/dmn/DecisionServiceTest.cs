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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
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
	/// @author Philipp Ossler
	/// </summary>
	public class DecisionServiceTest
	{
		private bool InstanceFieldsInitialized = false;

		public DecisionServiceTest()
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

	  protected internal const string DRD_DISH_DECISION_TABLE = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal const string DECISION_DEFINITION_KEY = "decision";

	  protected internal const string RESULT_OF_FIRST_VERSION = "ok";
	  protected internal const string RESULT_OF_SECOND_VERSION = "notok";

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		decisionService = engineRule.DecisionService;
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void evaluateDecisionTableById()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void evaluateDecisionTableById()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableById(decisionDefinition.Id, createVariables());

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void evaluateDecisionTableByKey()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void evaluateDecisionTableByKey()
	  {
		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, createVariables());

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void evaluateDecisionTableByKeyAndLatestVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void evaluateDecisionTableByKeyAndLatestVersion()
	  {
		testRule.deploy(DMN_DECISION_TABLE_V2);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, createVariables());

		assertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void evaluateDecisionTableByKeyAndVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void evaluateDecisionTableByKeyAndVersion()
	  {
		testRule.deploy(DMN_DECISION_TABLE_V2);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKeyAndVersion(DECISION_DEFINITION_KEY, 1, createVariables());

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void evaluateDecisionTableByKeyAndNullVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void evaluateDecisionTableByKeyAndNullVersion()
	  {
		testRule.deploy(DMN_DECISION_TABLE_V2);

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKeyAndVersion(DECISION_DEFINITION_KEY, null, createVariables());

		assertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionTableByNullId()
	  public virtual void evaluateDecisionTableByNullId()
	  {
		thrown.expect(typeof(NotValidException));
		thrown.expectMessage("either decision definition id or key must be set");

		decisionService.evaluateDecisionTableById(null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionTableByNonExistingId()
	  public virtual void evaluateDecisionTableByNonExistingId()
	  {
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("no deployed decision definition found with id 'unknown'");

		decisionService.evaluateDecisionTableById("unknown", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionTableByNullKey()
	  public virtual void evaluateDecisionTableByNullKey()
	  {
		thrown.expect(typeof(NotValidException));
		thrown.expectMessage("either decision definition id or key must be set");

		decisionService.evaluateDecisionTableByKey(null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionTableByNonExistingKey()
	  public virtual void evaluateDecisionTableByNonExistingKey()
	  {
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("no decision definition deployed with key 'unknown'");

		decisionService.evaluateDecisionTableByKey("unknown", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_TABLE) @Test public void evaluateDecisionTableByKeyWithNonExistingVersion()
	  [Deployment(resources : DMN_DECISION_TABLE)]
	  public virtual void evaluateDecisionTableByKeyWithNonExistingVersion()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("no decision definition deployed with key = 'decision' and version = '42'");

		decisionService.evaluateDecisionTableByKeyAndVersion(decisionDefinition.Key, 42, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void evaluateDecisionById()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void evaluateDecisionById()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		DmnDecisionResult decisionResult = decisionService.evaluateDecisionById(decisionDefinition.Id).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void evaluateDecisionByKey()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void evaluateDecisionByKey()
	  {
		DmnDecisionResult decisionResult = decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void evaluateDecisionByKeyAndLatestVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void evaluateDecisionByKeyAndLatestVersion()
	  {
		testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

		DmnDecisionResult decisionResult = decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void evaluateDecisionByKeyAndVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void evaluateDecisionByKeyAndVersion()
	  {
		testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

		DmnDecisionResult decisionResult = decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).version(1).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void evaluateDecisionByKeyAndNullVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void evaluateDecisionByKeyAndNullVersion()
	  {
		testRule.deploy(DMN_DECISION_LITERAL_EXPRESSION_V2);

		DmnDecisionResult decisionResult = decisionService.evaluateDecisionByKey(DECISION_DEFINITION_KEY).version(null).variables(createVariables()).evaluate();

		assertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionByNullId()
	  public virtual void evaluateDecisionByNullId()
	  {
		thrown.expect(typeof(NotValidException));
		thrown.expectMessage("either decision definition id or key must be set");

		decisionService.evaluateDecisionById(null).evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionByNonExistingId()
	  public virtual void evaluateDecisionByNonExistingId()
	  {
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("no deployed decision definition found with id 'unknown'");

		decisionService.evaluateDecisionById("unknown").evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionByNullKey()
	  public virtual void evaluateDecisionByNullKey()
	  {
		thrown.expect(typeof(NotValidException));
		thrown.expectMessage("either decision definition id or key must be set");

		decisionService.evaluateDecisionByKey(null).evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void evaluateDecisionByNonExistingKey()
	  public virtual void evaluateDecisionByNonExistingKey()
	  {
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("no decision definition deployed with key 'unknown'");

		decisionService.evaluateDecisionByKey("unknown").evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DMN_DECISION_LITERAL_EXPRESSION) @Test public void evaluateDecisionByKeyWithNonExistingVersion()
	  [Deployment(resources : DMN_DECISION_LITERAL_EXPRESSION)]
	  public virtual void evaluateDecisionByKeyWithNonExistingVersion()
	  {
		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().singleResult();

		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("no decision definition deployed with key = 'decision' and version = '42'");

		decisionService.evaluateDecisionByKey(decisionDefinition.Key).version(42).evaluate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = DRD_DISH_DECISION_TABLE) @Test public void evaluateDecisionWithRequiredDecisions()
	  [Deployment(resources : DRD_DISH_DECISION_TABLE)]
	  public virtual void evaluateDecisionWithRequiredDecisions()
	  {

		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey("dish-decision", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		assertThatDecisionHasResult(decisionResult, "Light salad");
	  }

	  protected internal virtual VariableMap createVariables()
	  {
		return Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
	  }

	  protected internal virtual void assertThatDecisionHasResult(DmnDecisionTableResult decisionResult, object expectedValue)
	  {
		assertThat(decisionResult, @is(notNullValue()));
		assertThat(decisionResult.size(), @is(1));
		string value = decisionResult.SingleResult.FirstEntry;
		assertThat(value, @is(expectedValue));
	  }

	  protected internal virtual void assertThatDecisionHasResult(DmnDecisionResult decisionResult, object expectedValue)
	  {
		assertThat(decisionResult, @is(notNullValue()));
		assertThat(decisionResult.size(), @is(1));
		string value = decisionResult.SingleResult.FirstEntry;
		assertThat(value, @is(expectedValue));
	  }

	}

}