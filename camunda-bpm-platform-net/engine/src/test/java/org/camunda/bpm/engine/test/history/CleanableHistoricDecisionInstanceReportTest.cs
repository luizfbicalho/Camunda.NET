using System;
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
namespace org.camunda.bpm.engine.test.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.fail;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CleanableHistoricDecisionInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReport;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class CleanableHistoricDecisionInstanceReportTest
	{
		private bool InstanceFieldsInitialized = false;

		public CleanableHistoricDecisionInstanceReportTest()
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
			ruleChain = RuleChain.outerRule(testRule).around(engineRule);
		}

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(testRule).around(engineRule);
	  public RuleChain ruleChain;

	  protected internal HistoryService historyService;
	  protected internal RepositoryService repositoryService;

	  protected internal const string DECISION_DEFINITION_KEY = "one";
	  protected internal const string SECOND_DECISION_DEFINITION_KEY = "two";
	  protected internal const string THIRD_DECISION_DEFINITION_KEY = "anotherDecision";
	  protected internal const string FOURTH_DECISION_DEFINITION_KEY = "decision";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		repositoryService = engineRule.RepositoryService;

		testRule.deploy("org/camunda/bpm/engine/test/repository/one.dmn");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
		}
	  }

	  protected internal virtual void prepareDecisionInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount)
	  {
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(key).list();
		assertEquals(1, decisionDefinitions.Count);
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, historyTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, daysInThePast);

		IDictionary<string, object> variables = Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
		for (int i = 0; i < instanceCount; i++)
		{
		  engineRule.DecisionService.evaluateDecisionByKey(key).variables(variables).evaluate();
		}

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportComplex()
	  public virtual void testReportComplex()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/repository/two.dmn", "org/camunda/bpm/engine/test/api/dmn/Another_Example.dmn", "org/camunda/bpm/engine/test/api/dmn/Example.dmn");
		prepareDecisionInstances(DECISION_DEFINITION_KEY, 0, 5, 10);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10);
		prepareDecisionInstances(SECOND_DECISION_DEFINITION_KEY, -6, null, 10);
		prepareDecisionInstances(THIRD_DECISION_DEFINITION_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();
		string secondDecisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(SECOND_DECISION_DEFINITION_KEY).singleResult().Id;
		CleanableHistoricDecisionInstanceReportResult secondReportResult = historyService.createCleanableHistoricDecisionInstanceReport().decisionDefinitionIdIn(secondDecisionDefinitionId).singleResult();
		CleanableHistoricDecisionInstanceReportResult thirdReportResult = historyService.createCleanableHistoricDecisionInstanceReport().decisionDefinitionKeyIn(THIRD_DECISION_DEFINITION_KEY).singleResult();

		// then
		assertEquals(4, reportResults.Count);
		foreach (CleanableHistoricDecisionInstanceReportResult result in reportResults)
		{
		  if (result.DecisionDefinitionKey.Equals(DECISION_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 10, 20);
		  }
		  else if (result.DecisionDefinitionKey.Equals(SECOND_DECISION_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 0, 10);
		  }
		  else if (result.DecisionDefinitionKey.Equals(THIRD_DECISION_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 10, 10);
		  }
		  else if (result.DecisionDefinitionKey.Equals(FOURTH_DECISION_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 0, 0);
		  }
		}
		checkResultNumbers(secondReportResult, 0, 10);
		checkResultNumbers(thirdReportResult, 10, 10);

	  }

	  private void checkResultNumbers(CleanableHistoricDecisionInstanceReportResult result, int expectedCleanable, int expectedFinished)
	  {
		assertEquals(expectedCleanable, result.CleanableDecisionInstanceCount);
		assertEquals(expectedFinished, result.FinishedDecisionInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithAllCleanableInstances()
	  public virtual void testReportWithAllCleanableInstances()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();
		long count = historyService.createCleanableHistoricDecisionInstanceReport().count();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(1, count);

		checkResultNumbers(reportResults[0], 10, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithPartiallyCleanableInstances()
	  public virtual void testReportWithPartiallyCleanableInstances()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 5);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, 0, 5, 5);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		checkResultNumbers(reportResults[0], 5, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithZeroHistoryTTL()
	  public virtual void testReportWithZeroHistoryTTL()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 0, 5);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, 0, 0, 5);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		checkResultNumbers(reportResults[0], 10, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithNullHistoryTTL()
	  public virtual void testReportWithNullHistoryTTL()
	  {
		// given
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, null, 5);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, 0, null, 5);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResults = historyService.createCleanableHistoricDecisionInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		checkResultNumbers(reportResults[0], 0, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportByInvalidDecisionDefinitionId()
	  public virtual void testReportByInvalidDecisionDefinitionId()
	  {
		CleanableHistoricDecisionInstanceReport report = historyService.createCleanableHistoricDecisionInstanceReport();

		try
		{
		  report.decisionDefinitionIdIn(null);
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  report.decisionDefinitionIdIn("abc", null, "def");
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportByInvalidDecisionDefinitionKey()
	  public virtual void testReportByInvalidDecisionDefinitionKey()
	  {
		CleanableHistoricDecisionInstanceReport report = historyService.createCleanableHistoricDecisionInstanceReport();

		try
		{
		  report.decisionDefinitionKeyIn(null);
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  report.decisionDefinitionKeyIn("abc", null, "def");
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportCompact()
	  public virtual void testReportCompact()
	  {
		// given
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).list();
		assertEquals(1, decisionDefinitions.Count);

		// assume
		IList<CleanableHistoricDecisionInstanceReportResult> resultWithZeros = historyService.createCleanableHistoricDecisionInstanceReport().list();
		assertEquals(1, resultWithZeros.Count);
		assertEquals(0, resultWithZeros[0].FinishedDecisionInstanceCount);

		// when
		long resultCountWithoutZeros = historyService.createCleanableHistoricDecisionInstanceReport().compact().count();

		// then
		assertEquals(0, resultCountWithoutZeros);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedAsc()
	  public virtual void testReportOrderByFinishedAsc()
	  {
		// give
		testRule.deploy("org/camunda/bpm/engine/test/repository/two.dmn", "org/camunda/bpm/engine/test/api/dmn/Another_Example.dmn");
		prepareDecisionInstances(SECOND_DECISION_DEFINITION_KEY, -6, 5, 6);
		prepareDecisionInstances(THIRD_DECISION_DEFINITION_KEY, -6, 5, 8);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 4);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResult = historyService.createCleanableHistoricDecisionInstanceReport().orderByFinished().asc().list();

		// then
		assertEquals(3, reportResult.Count);
		assertEquals(DECISION_DEFINITION_KEY, reportResult[0].DecisionDefinitionKey);
		assertEquals(SECOND_DECISION_DEFINITION_KEY, reportResult[1].DecisionDefinitionKey);
		assertEquals(THIRD_DECISION_DEFINITION_KEY, reportResult[2].DecisionDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedDesc()
	  public virtual void testReportOrderByFinishedDesc()
	  {
		// give
		testRule.deploy("org/camunda/bpm/engine/test/repository/two.dmn", "org/camunda/bpm/engine/test/api/dmn/Another_Example.dmn");
		prepareDecisionInstances(SECOND_DECISION_DEFINITION_KEY, -6, 5, 6);
		prepareDecisionInstances(THIRD_DECISION_DEFINITION_KEY, -6, 5, 8);
		prepareDecisionInstances(DECISION_DEFINITION_KEY, -6, 5, 4);

		// when
		IList<CleanableHistoricDecisionInstanceReportResult> reportResult = historyService.createCleanableHistoricDecisionInstanceReport().orderByFinished().desc().list();

		// then
		assertEquals(3, reportResult.Count);
		assertEquals(THIRD_DECISION_DEFINITION_KEY, reportResult[0].DecisionDefinitionKey);
		assertEquals(SECOND_DECISION_DEFINITION_KEY, reportResult[1].DecisionDefinitionKey);
		assertEquals(DECISION_DEFINITION_KEY, reportResult[2].DecisionDefinitionKey);
	  }
	}

}