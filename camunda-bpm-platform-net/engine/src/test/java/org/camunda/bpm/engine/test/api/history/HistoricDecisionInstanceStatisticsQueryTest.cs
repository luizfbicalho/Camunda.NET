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
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDecisionInstanceStatisticsQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricDecisionInstanceStatisticsQueryTest()
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


	  protected internal const string DISH_DRG_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";
	  protected internal const string SCORE_DRG_DMN = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";

	  protected internal const string NON_EXISTING = "fake";
	  protected internal const string DISH_DECISION = "dish-decision";
	  protected internal const string TEMPERATURE = "temperature";
	  protected internal const string DAY_TYPE = "dayType";
	  protected internal const string WEEKEND = "Weekend";

	  protected internal DecisionService decisionService;
	  protected internal RepositoryService repositoryService;
	  protected internal HistoryService historyService;

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		decisionService = engineRule.DecisionService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		testRule.deploy(DISH_DRG_DMN);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticForRootDecisionEvaluation() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticForRootDecisionEvaluation()
	  {
		//when
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue(TEMPERATURE, 21).putValue(DAY_TYPE, WEEKEND)).evaluate();

		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue(TEMPERATURE, 11).putValue(DAY_TYPE, WEEKEND)).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();

		HistoricDecisionInstanceStatisticsQuery statisticsQuery = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		//then
		assertThat(statisticsQuery.count(), @is(3L));
		assertThat(statisticsQuery.list().size(), @is(3));
		assertThat(statisticsQuery.list().get(0).Evaluations, @is(2));
		assertThat(statisticsQuery.list().get(0).DecisionDefinitionKey, @is(notNullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticForRootDecisionWithInstanceConstraintEvaluation() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticForRootDecisionWithInstanceConstraintEvaluation()
	  {
		//when
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue(TEMPERATURE, 21).putValue(DAY_TYPE, WEEKEND)).evaluate();

		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue(TEMPERATURE, 11).putValue(DAY_TYPE, WEEKEND)).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();


		string decisionInstanceId = engineRule.HistoryService.createHistoricDecisionInstanceQuery().decisionRequirementsDefinitionId(decisionRequirementsDefinition.Id).rootDecisionInstancesOnly().list().get(0).Id;

		HistoricDecisionInstanceStatisticsQuery query = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id).decisionInstanceId(decisionInstanceId);

		//then
		assertThat(query.count(), @is(3L));
		assertThat(query.list().size(), @is(3));
		assertThat(query.list().get(0).Evaluations, @is(1));
		assertThat(query.list().get(0).DecisionDefinitionKey, @is(notNullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticForRootDecisionWithFakeInstanceConstraintEvaluation() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticForRootDecisionWithFakeInstanceConstraintEvaluation()
	  {
		//when
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue(TEMPERATURE, 21).putValue(DAY_TYPE, WEEKEND)).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();

		HistoricDecisionInstanceStatisticsQuery query = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id).decisionInstanceId(NON_EXISTING);

		//then
		assertThat(query.count(), @is(0L));
		assertThat(query.list().size(), @is(0));


	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticForRootDecisionWithNullInstanceConstraintEvaluation() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticForRootDecisionWithNullInstanceConstraintEvaluation()
	  {
		//when
		decisionService.evaluateDecisionTableByKey(DISH_DECISION).variables(Variables.createVariables().putValue(TEMPERATURE, 21).putValue(DAY_TYPE, WEEKEND)).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();
		//when
		HistoricDecisionInstanceStatisticsQuery query = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id).decisionInstanceId(null);

		//then
		try
		{
		  query.count();
		}
		catch (NullValueException)
		{
		  //expected
		}

		try
		{
		  query.list();
		}
		catch (NullValueException)
		{
		  //expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticForChildDecisionEvaluation() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticForChildDecisionEvaluation()
	  {
		//when
		decisionService.evaluateDecisionTableByKey("season").variables(Variables.createVariables().putValue(TEMPERATURE, 21)).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();

		HistoricDecisionInstanceStatisticsQuery statisticsQuery = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		//then
		assertThat(statisticsQuery.count(), @is(1L));
		assertThat(statisticsQuery.list().size(), @is(1));
		assertThat(statisticsQuery.list().get(0).Evaluations, @is(1));
		assertThat(statisticsQuery.list().get(0).DecisionDefinitionKey, @is(notNullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticConstrainedToOneDRD() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticConstrainedToOneDRD()
	  {
		//given
		testRule.deploy(SCORE_DRG_DMN);

		//when
		decisionService.evaluateDecisionTableByKey("score-decision").variables(Variables.createVariables().putValue("input", "john")).evaluate();

		decisionService.evaluateDecisionTableByKey("season").variables(Variables.createVariables().putValue(TEMPERATURE, 21)).evaluate();

		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionName("Score").singleResult();

		HistoricDecisionInstanceStatisticsQuery statisticsQuery = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		//then
		assertThat(statisticsQuery.count(), @is(1L));
		assertThat(statisticsQuery.list().size(), @is(1));
		assertThat(statisticsQuery.list().get(0).Evaluations, @is(1));
		assertThat(statisticsQuery.list().get(0).DecisionDefinitionKey, @is(notNullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticDoesNotExistForFakeId() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticDoesNotExistForFakeId()
	  {
		assertThat("available statistics count of fake", historyService.createHistoricDecisionInstanceStatisticsQuery(NON_EXISTING).count(), @is(0L));

		assertThat("available statistics elements of fake", historyService.createHistoricDecisionInstanceStatisticsQuery(NON_EXISTING).list().size(), @is(0));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticThrowsExceptionOnNullConstraintsCount() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticThrowsExceptionOnNullConstraintsCount()
	  {
		//expect
		thrown.expect(typeof(NullValueException));
		historyService.createHistoricDecisionInstanceStatisticsQuery(null).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticThrowsExceptionOnNullConstraintsList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticThrowsExceptionOnNullConstraintsList()
	  {
		//expect
		thrown.expect(typeof(NullValueException));
		historyService.createHistoricDecisionInstanceStatisticsQuery(null).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticForNotEvaluatedDRD() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStatisticForNotEvaluatedDRD()
	  {
		//when
		DecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.createDecisionRequirementsDefinitionQuery().singleResult();

		HistoricDecisionInstanceStatisticsQuery statisticsQuery = historyService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

		//then
		assertThat("available statistics count", statisticsQuery.count(), @is(0L));
		assertThat("available statistics elements", statisticsQuery.list().size(), @is(0));
	  }
	}
}