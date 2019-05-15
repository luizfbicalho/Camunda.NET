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
	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class CleanableHistoricProcessInstanceReportTest
	{
		private bool InstanceFieldsInitialized = false;

		public CleanableHistoricProcessInstanceReportTest()
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
	  protected internal TaskService taskService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;

	  protected internal const string PROCESS_DEFINITION_KEY = "HISTORIC_INST";
	  protected internal const string SECOND_PROCESS_DEFINITION_KEY = "SECOND_HISTORIC_INST";
	  protected internal const string THIRD_PROCESS_DEFINITION_KEY = "THIRD_HISTORIC_INST";
	  protected internal const string FOURTH_PROCESS_DEFINITION_KEY = "FOURTH_HISTORIC_INST";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;

		testRule.deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, null, true, true);
		}

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.deleteTask(task.Id, true);
		}

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		{
		  historyService.deleteHistoricProcessInstance(historicProcessInstance.Id);
		}
	  }

	  protected internal virtual BpmnModelInstance createProcessWithUserTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().userTask(key + "_task1").name(key + " Task 1").endEvent().done();
	  }

	  protected internal virtual void prepareProcessInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount)
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(key).list();
		assertEquals(1, processDefinitions.Count);
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, historyTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(oldCurrentTime, daysInThePast);

		IList<string> processInstanceIds = new List<string>();
		for (int i = 0; i < instanceCount; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(key);
		  processInstanceIds.Add(processInstance.Id);
		}
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportComplex()
	  public virtual void testReportComplex()
	  {
		testRule.deploy(createProcessWithUserTask(SECOND_PROCESS_DEFINITION_KEY));
		testRule.deploy(createProcessWithUserTask(THIRD_PROCESS_DEFINITION_KEY));
		testRule.deploy(createProcessWithUserTask(FOURTH_PROCESS_DEFINITION_KEY));
		// given
		prepareProcessInstances(PROCESS_DEFINITION_KEY, 0, 5, 10);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10);
		prepareProcessInstances(SECOND_PROCESS_DEFINITION_KEY, -6, 5, 10);
		prepareProcessInstances(THIRD_PROCESS_DEFINITION_KEY, -6, null, 10);
		prepareProcessInstances(FOURTH_PROCESS_DEFINITION_KEY, -6, 0, 10);

		repositoryService.deleteProcessDefinition(repositoryService.createProcessDefinitionQuery().processDefinitionKey(SECOND_PROCESS_DEFINITION_KEY).singleResult().Id, false);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();
		CleanableHistoricProcessInstanceReportResult secondReportResult = historyService.createCleanableHistoricProcessInstanceReport().processDefinitionIdIn(repositoryService.createProcessDefinitionQuery().processDefinitionKey(THIRD_PROCESS_DEFINITION_KEY).singleResult().Id).singleResult();
		CleanableHistoricProcessInstanceReportResult thirdReportResult = historyService.createCleanableHistoricProcessInstanceReport().processDefinitionKeyIn(FOURTH_PROCESS_DEFINITION_KEY).singleResult();

		// then
		assertEquals(3, reportResults.Count);
		foreach (CleanableHistoricProcessInstanceReportResult result in reportResults)
		{
		  if (result.ProcessDefinitionKey.Equals(PROCESS_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 10, 20);
		  }
		  else if (result.ProcessDefinitionKey.Equals(THIRD_PROCESS_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 0, 10);
		  }
		  else if (result.ProcessDefinitionKey.Equals(FOURTH_PROCESS_DEFINITION_KEY))
		  {
			checkResultNumbers(result, 10, 10);
		  }
		}
		checkResultNumbers(secondReportResult, 0, 10);
		checkResultNumbers(thirdReportResult, 10, 10);
	  }

	  private void checkResultNumbers(CleanableHistoricProcessInstanceReportResult result, int expectedCleanable, int expectedFinished)
	  {
		assertEquals(expectedCleanable, result.CleanableProcessInstanceCount);
		assertEquals(expectedFinished, result.FinishedProcessInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithAllCleanableInstances()
	  public virtual void testReportWithAllCleanableInstances()
	  {
		// given
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();
		long count = historyService.createCleanableHistoricProcessInstanceReport().count();

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
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 5);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, 0, 5, 5);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);

		checkResultNumbers(reportResults[0], 5, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithZeroHistoryTTL()
	  public virtual void testReportWithZeroHistoryTTL()
	  {
		// given
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 0, 5);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, 0, 0, 5);

		// when
		CleanableHistoricProcessInstanceReportResult result = historyService.createCleanableHistoricProcessInstanceReport().singleResult();

		// then
		checkResultNumbers(result, 10, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithNullHistoryTTL()
	  public virtual void testReportWithNullHistoryTTL()
	  {
		// given
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, null, 5);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, 0, null, 5);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);

		checkResultNumbers(reportResults[0], 0, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportByInvalidProcessDefinitionId()
	  public virtual void testReportByInvalidProcessDefinitionId()
	  {
		CleanableHistoricProcessInstanceReport report = historyService.createCleanableHistoricProcessInstanceReport();

		try
		{
		  report.processDefinitionIdIn(null);
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  report.processDefinitionIdIn("abc", null, "def");
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportByInvalidProcessDefinitionKey()
	  public virtual void testReportByInvalidProcessDefinitionKey()
	  {
		CleanableHistoricProcessInstanceReport report = historyService.createCleanableHistoricProcessInstanceReport();

		try
		{
		  report.processDefinitionKeyIn(null);
		  fail("Expected NotValidException");
		}
		catch (NotValidException)
		{
		  // expected
		}

		try
		{
		  report.processDefinitionKeyIn("abc", null, "def");
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
		IList<ProcessDefinition> pdList = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).list();
		assertEquals(1, pdList.Count);
		runtimeService.startProcessInstanceById(pdList[0].Id);

		IList<CleanableHistoricProcessInstanceReportResult> resultWithZeros = historyService.createCleanableHistoricProcessInstanceReport().list();
		assertEquals(1, resultWithZeros.Count);
		assertEquals(0, resultWithZeros[0].FinishedProcessInstanceCount);

		// when
		long resultCountWithoutZeros = historyService.createCleanableHistoricProcessInstanceReport().compact().count();

		// then
		assertEquals(0, resultCountWithoutZeros);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedAsc()
	  public virtual void testReportOrderByFinishedAsc()
	  {
		testRule.deploy(createProcessWithUserTask(SECOND_PROCESS_DEFINITION_KEY));
		testRule.deploy(createProcessWithUserTask(THIRD_PROCESS_DEFINITION_KEY));
		// given
		prepareProcessInstances(SECOND_PROCESS_DEFINITION_KEY, -6, 5, 6);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 4);
		prepareProcessInstances(THIRD_PROCESS_DEFINITION_KEY, -6, 5, 8);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResult = historyService.createCleanableHistoricProcessInstanceReport().orderByFinished().asc().list();

		// then
		assertEquals(3, reportResult.Count);
		assertEquals(PROCESS_DEFINITION_KEY, reportResult[0].ProcessDefinitionKey);
		assertEquals(SECOND_PROCESS_DEFINITION_KEY, reportResult[1].ProcessDefinitionKey);
		assertEquals(THIRD_PROCESS_DEFINITION_KEY, reportResult[2].ProcessDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedDesc()
	  public virtual void testReportOrderByFinishedDesc()
	  {
		testRule.deploy(createProcessWithUserTask(SECOND_PROCESS_DEFINITION_KEY));
		testRule.deploy(createProcessWithUserTask(THIRD_PROCESS_DEFINITION_KEY));
		// given
		prepareProcessInstances(SECOND_PROCESS_DEFINITION_KEY, -6, 5, 6);
		prepareProcessInstances(PROCESS_DEFINITION_KEY, -6, 5, 4);
		prepareProcessInstances(THIRD_PROCESS_DEFINITION_KEY, -6, 5, 8);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResult = historyService.createCleanableHistoricProcessInstanceReport().orderByFinished().desc().list();

		// then
		assertEquals(3, reportResult.Count);
		assertEquals(THIRD_PROCESS_DEFINITION_KEY, reportResult[0].ProcessDefinitionKey);
		assertEquals(SECOND_PROCESS_DEFINITION_KEY, reportResult[1].ProcessDefinitionKey);
		assertEquals(PROCESS_DEFINITION_KEY, reportResult[2].ProcessDefinitionKey);
	  }
	}

}