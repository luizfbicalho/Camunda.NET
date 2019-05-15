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
	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class CleanableHistoricCaseInstanceReportTest
	{
		private bool InstanceFieldsInitialized = false;

		public CleanableHistoricCaseInstanceReportTest()
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

	  private const string FORTH_CASE_DEFINITION_KEY = "case";
	  private const string THIRD_CASE_DEFINITION_KEY = "oneTaskCase";
	  private const string SECOND_CASE_DEFINITION_KEY = "oneCaseTaskCase";
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(testRule).around(engineRule);
	  public RuleChain ruleChain;

	  protected internal HistoryService historyService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal CaseService caseService;
	  protected internal TaskService taskService;

	  protected internal const string CASE_DEFINITION_KEY = "one";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
		caseService = engineRule.CaseService;
		taskService = engineRule.TaskService;

		testRule.deploy("org/camunda/bpm/engine/test/repository/one.cmmn");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		IList<HistoricCaseInstance> instanceList = historyService.createHistoricCaseInstanceQuery().active().list();
		if (instanceList.Count > 0)
		{
		  foreach (HistoricCaseInstance instance in instanceList)
		  {

			caseService.terminateCaseExecution(instance.Id);
			caseService.closeCaseInstance(instance.Id);
		  }
		}
		IList<HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().list();
		foreach (HistoricCaseInstance historicCaseInstance in historicCaseInstances)
		{
		  historyService.deleteHistoricCaseInstance(historicCaseInstance.Id);
		}
	  }

	  private void prepareCaseInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount)
	  {
		// update time to live
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(key).list();
		assertEquals(1, caseDefinitions.Count);
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitions[0].Id, historyTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, daysInThePast);

		for (int i = 0; i < instanceCount; i++)
		{
		  CaseInstance caseInstance = caseService.createCaseInstanceByKey(key);
		  caseService.terminateCaseExecution(caseInstance.Id);
		  caseService.closeCaseInstance(caseInstance.Id);
		}

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	  private void checkResultNumbers(CleanableHistoricCaseInstanceReportResult result, int expectedCleanable, int expectedFinished)
	  {
		assertEquals(expectedCleanable, result.CleanableCaseInstanceCount);
		assertEquals(expectedFinished, result.FinishedCaseInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithAllCleanableInstances()
	  public virtual void testReportWithAllCleanableInstances()
	  {
		// given
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResults = historyService.createCleanableHistoricCaseInstanceReport().list();
		long count = historyService.createCleanableHistoricCaseInstanceReport().count();

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
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, 5, 5);
		prepareCaseInstances(CASE_DEFINITION_KEY, 0, 5, 5);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResults = historyService.createCleanableHistoricCaseInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		checkResultNumbers(reportResults[0], 5, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithZeroHistoryTTL()
	  public virtual void testReportWithZeroHistoryTTL()
	  {
		// given
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, 0, 5);
		prepareCaseInstances(CASE_DEFINITION_KEY, 0, 0, 5);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResults = historyService.createCleanableHistoricCaseInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		checkResultNumbers(reportResults[0], 10, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithNullHistoryTTL()
	  public virtual void testReportWithNullHistoryTTL()
	  {
		// given
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, null, 5);
		prepareCaseInstances(CASE_DEFINITION_KEY, 0, null, 5);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResults = historyService.createCleanableHistoricCaseInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		checkResultNumbers(reportResults[0], 0, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportComplex()
	  public virtual void testReportComplex()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithHistoryTimeToLive.cmmn");
		prepareCaseInstances(CASE_DEFINITION_KEY, 0, 5, 10);
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, 5, 10);
		prepareCaseInstances(SECOND_CASE_DEFINITION_KEY, -6, null, 10);
		prepareCaseInstances(THIRD_CASE_DEFINITION_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResults = historyService.createCleanableHistoricCaseInstanceReport().list();
		string id = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(SECOND_CASE_DEFINITION_KEY).singleResult().Id;
		CleanableHistoricCaseInstanceReportResult secondReportResult = historyService.createCleanableHistoricCaseInstanceReport().caseDefinitionIdIn(id).singleResult();
		CleanableHistoricCaseInstanceReportResult thirdReportResult = historyService.createCleanableHistoricCaseInstanceReport().caseDefinitionKeyIn(THIRD_CASE_DEFINITION_KEY).singleResult();

		// then
		assertEquals(4, reportResults.Count);
		foreach (CleanableHistoricCaseInstanceReportResult result in reportResults)
		{
		  if (result.CaseDefinitionKey.Equals(CASE_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 10, 20);
		  }
		  else if (result.CaseDefinitionKey.Equals(SECOND_CASE_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 0, 10);
		  }
		  else if (result.CaseDefinitionKey.Equals(THIRD_CASE_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 10, 10);
		  }
		  else if (result.CaseDefinitionKey.Equals(FORTH_CASE_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 0, 0);
		  }
		}
		checkResultNumbers(secondReportResult, 0, 10);
		checkResultNumbers(thirdReportResult, 10, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportByInvalidCaseDefinitionId()
	  public virtual void testReportByInvalidCaseDefinitionId()
	  {
		CleanableHistoricCaseInstanceReport report = historyService.createCleanableHistoricCaseInstanceReport();

		try
		{
		  report.caseDefinitionIdIn(null);
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  report.caseDefinitionIdIn("abc", null, "def");
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportByInvalidCaseDefinitionKey()
	  public virtual void testReportByInvalidCaseDefinitionKey()
	  {
		CleanableHistoricCaseInstanceReport report = historyService.createCleanableHistoricCaseInstanceReport();

		try
		{
		  report.caseDefinitionKeyIn(null);
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  report.caseDefinitionKeyIn("abc", null, "def");
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
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).list();
		assertEquals(1, caseDefinitions.Count);

		IList<CleanableHistoricCaseInstanceReportResult> resultWithZeros = historyService.createCleanableHistoricCaseInstanceReport().list();
		assertEquals(1, resultWithZeros.Count);
		assertEquals(0, resultWithZeros[0].FinishedCaseInstanceCount);

		// when
		long resultCountWithoutZeros = historyService.createCleanableHistoricCaseInstanceReport().compact().count();

		// then
		assertEquals(0, resultCountWithoutZeros);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedAsc()
	  public virtual void testReportOrderByFinishedAsc()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn");
		prepareCaseInstances(THIRD_CASE_DEFINITION_KEY, -6, 5, 8);
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, 5, 4);
		prepareCaseInstances(SECOND_CASE_DEFINITION_KEY, -6, 5, 6);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResult = historyService.createCleanableHistoricCaseInstanceReport().orderByFinished().asc().list();

		// then
		assertEquals(3, reportResult.Count);
		assertEquals(CASE_DEFINITION_KEY, reportResult[0].CaseDefinitionKey);
		assertEquals(SECOND_CASE_DEFINITION_KEY, reportResult[1].CaseDefinitionKey);
		assertEquals(THIRD_CASE_DEFINITION_KEY, reportResult[2].CaseDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedDesc()
	  public virtual void testReportOrderByFinishedDesc()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn");
		prepareCaseInstances(THIRD_CASE_DEFINITION_KEY, -6, 5, 8);
		prepareCaseInstances(CASE_DEFINITION_KEY, -6, 5, 4);
		prepareCaseInstances(SECOND_CASE_DEFINITION_KEY, -6, 5, 6);

		// when
		IList<CleanableHistoricCaseInstanceReportResult> reportResult = historyService.createCleanableHistoricCaseInstanceReport().orderByFinished().desc().list();

		// then
		assertEquals(3, reportResult.Count);
		assertEquals(THIRD_CASE_DEFINITION_KEY, reportResult[0].CaseDefinitionKey);
		assertEquals(SECOND_CASE_DEFINITION_KEY, reportResult[1].CaseDefinitionKey);
		assertEquals(CASE_DEFINITION_KEY, reportResult[2].CaseDefinitionKey);
	  }

	}

}