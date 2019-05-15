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
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
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


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricTaskReportTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricTaskReportTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}


	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfiguration processEngineConfiguration;
	  protected internal HistoryService historyService;

	  protected internal const string PROCESS_DEFINITION_KEY = "HISTORIC_TASK_INST_REPORT";
	  protected internal const string ANOTHER_PROCESS_DEFINITION_KEY = "ANOTHER_HISTORIC_TASK_INST_REPORT";


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = processEngineRule.HistoryService;
		processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;

		processEngineTestRule.deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
		processEngineTestRule.deploy(createProcessWithUserTask(ANOTHER_PROCESS_DEFINITION_KEY));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		IList<Task> list = processEngineRule.TaskService.createTaskQuery().list();
		foreach (Task task in list)
		{
		  processEngineRule.TaskService.deleteTask(task.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricTaskInstanceReportQuery()
	  public virtual void testHistoricTaskInstanceReportQuery()
	  {
		// given
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		processEngineTestRule.deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		// when
		IList<HistoricTaskInstanceReportResult> historicTaskInstanceReportResults = historyService.createHistoricTaskInstanceReport().countByTaskName();

		// then
		assertEquals(2, historicTaskInstanceReportResults.Count);
		assertEquals(2, historicTaskInstanceReportResults[0].Count, 0);
		assertEquals(ANOTHER_PROCESS_DEFINITION_KEY, historicTaskInstanceReportResults[0].ProcessDefinitionKey);
		assertEquals("name_" + ANOTHER_PROCESS_DEFINITION_KEY, historicTaskInstanceReportResults[0].ProcessDefinitionName);
		assertEquals(ANOTHER_PROCESS_DEFINITION_KEY + " Task 1", historicTaskInstanceReportResults[0].TaskName);

		assertTrue(historicTaskInstanceReportResults[1].ProcessDefinitionId.Contains(":2:"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricTaskInstanceReportGroupedByProcessDefinitionKey()
	  public virtual void testHistoricTaskInstanceReportGroupedByProcessDefinitionKey()
	  {
		// given
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		processEngineTestRule.deploy(createProcessWithUserTask(PROCESS_DEFINITION_KEY));
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		// when
		IList<HistoricTaskInstanceReportResult> historicTaskInstanceReportResults = historyService.createHistoricTaskInstanceReport().countByProcessDefinitionKey();

		// then
		assertEquals(2, historicTaskInstanceReportResults.Count);
		assertTrue(historicTaskInstanceReportResults[0].ProcessDefinitionId.Contains(":1:"));
		assertEquals("name_" + ANOTHER_PROCESS_DEFINITION_KEY, historicTaskInstanceReportResults[0].ProcessDefinitionName);

		assertEquals(ANOTHER_PROCESS_DEFINITION_KEY, historicTaskInstanceReportResults[0].ProcessDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricTaskInstanceReportWithCompletedAfterDate()
	  public virtual void testHistoricTaskInstanceReportWithCompletedAfterDate()
	  {
		// given
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 8, 14, 12, 1);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		// when
		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 11, 14, 12, 5, 0);

		IList<HistoricTaskInstanceReportResult> historicTaskInstanceReportResults = historyService.createHistoricTaskInstanceReport().completedAfter(calendar).countByProcessDefinitionKey();

		// then
		assertEquals(1, historicTaskInstanceReportResults.Count);
		assertEquals(1, historicTaskInstanceReportResults[0].Count, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricTaskInstanceReportWithCompletedBeforeDate()
	  public virtual void testHistoricTaskInstanceReportWithCompletedBeforeDate()
	  {
		// given
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 8, 14, 12, 1);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		// when
		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 11, 14, 12, 5, 0);

		IList<HistoricTaskInstanceReportResult> historicTaskInstanceReportResults = historyService.createHistoricTaskInstanceReport().completedBefore(calendar).countByProcessDefinitionKey();

		// then
		assertEquals(2, historicTaskInstanceReportResults.Count);
		assertEquals(1, historicTaskInstanceReportResults[0].Count, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletedAfterWithNullValue()
	  public virtual void testCompletedAfterWithNullValue()
	  {
		try
		{
		  historyService.createHistoricTaskInstanceReport().completedAfter(null).countByProcessDefinitionKey();

		  fail("Expected NotValidException");
		}
		catch (NotValidException nve)
		{
		  assertTrue(nve.Message.contains("completedAfter"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletedBeforeWithNullValue()
	  public virtual void testCompletedBeforeWithNullValue()
	  {
		try
		{
		  historyService.createHistoricTaskInstanceReport().completedBefore(null).countByProcessDefinitionKey();

		  fail("Expected NotValidException");
		}
		catch (NotValidException nve)
		{
		  assertTrue(nve.Message.contains("completedBefore"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithNullTaskName()
	  public virtual void testReportWithNullTaskName()
	  {
		// given
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		// when
		BpmnModelInstance instance = Bpmn.createExecutableProcess(ANOTHER_PROCESS_DEFINITION_KEY).name("name_" + ANOTHER_PROCESS_DEFINITION_KEY).startEvent().userTask("task1_" + ANOTHER_PROCESS_DEFINITION_KEY).name(null).endEvent().done();

		processEngineTestRule.deploy(instance);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 11, 14, 12, 5, 0);

		IList<HistoricTaskInstanceReportResult> historicTaskInstanceReportResults = historyService.createHistoricTaskInstanceReport().completedBefore(calendar).countByTaskName();

		assertEquals(1, historicTaskInstanceReportResults.Count);
		assertEquals(1, historicTaskInstanceReportResults[0].Count, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportWithEmptyTaskName()
	  public virtual void testReportWithEmptyTaskName()
	  {
		// given
		startAndCompleteProcessInstance(PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		// when
		BpmnModelInstance instance = Bpmn.createExecutableProcess(ANOTHER_PROCESS_DEFINITION_KEY).name("name_" + ANOTHER_PROCESS_DEFINITION_KEY).startEvent().userTask("task1_" + ANOTHER_PROCESS_DEFINITION_KEY).name("").endEvent().done();

		processEngineTestRule.deploy(instance);
		startAndCompleteProcessInstance(ANOTHER_PROCESS_DEFINITION_KEY, 2016, 7, 14, 12, 1);

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 11, 14, 12, 5, 0);

		IList<HistoricTaskInstanceReportResult> historicTaskInstanceReportResults = historyService.createHistoricTaskInstanceReport().completedBefore(calendar).countByTaskName();

		assertEquals(1, historicTaskInstanceReportResults.Count);
		assertEquals(1, historicTaskInstanceReportResults[0].Count, 0);
	  }

	  protected internal virtual BpmnModelInstance createProcessWithUserTask(string key)
	  {
		double random = GlobalRandom.NextDouble;
		return Bpmn.createExecutableProcess(key).name("name_" + key).startEvent().userTask(key + "_" + random + "_task1").name(key + " Task 1").endEvent().done();
	  }

	  protected internal virtual void completeTask(string pid)
	  {
		Task task = processEngineRule.TaskService.createTaskQuery().processInstanceId(pid).singleResult();
		processEngineRule.TaskService.complete(task.Id);
	  }

	  protected internal virtual void setCurrentTime(int year, int month, int dayOfMonth, int hourOfDay, int minute)
	  {
		DateTime calendar = new DateTime();
		// Calendars month start with 0 = January
		calendar = new DateTime(year, month - 1, dayOfMonth, hourOfDay, minute, 0);
		ClockUtil.CurrentTime = calendar;
	  }

	  protected internal virtual void addToCalendar(int field, int month)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(ClockUtil.CurrentTime);
		calendar.add(field, month);
		ClockUtil.CurrentTime = calendar;
	  }

	  protected internal virtual void startAndCompleteProcessInstance(string key, int year, int month, int dayOfMonth, int hourOfDay, int minute)
	  {
		setCurrentTime(year, month, dayOfMonth, hourOfDay, minute);

		ProcessInstance pi = processEngineRule.RuntimeService.startProcessInstanceByKey(key);

		addToCalendar(DateTime.MONTH, 5);
		completeTask(pi.Id);

		ClockUtil.reset();
	  }
	}

}