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
//	import static org.camunda.bpm.engine.query.PeriodUnit.MONTH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.query.PeriodUnit.QUARTER;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceReport = org.camunda.bpm.engine.history.HistoricProcessInstanceReport;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class HistoricProcessInstanceDurationReportTest : PluggableProcessEngineTestCase
	{

	  private Random random = new Random();

	  public virtual void testDurationReportByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testTwoInstancesInSamePeriodByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testDurationReportInDifferentPeriodsByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2015, 10, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 11, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 2, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 4, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 5, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 6, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 7, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 8, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 9, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testSamePeriodDifferentYearByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2015, 1, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testDurationReportByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testTwoInstancesInSamePeriodDurationReportByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 5, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testDurationReportInDifferentPeriodsByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2015, 10, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 11, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 2, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 3, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 5, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 6, 1, 10, 0).startAndCompleteProcessInstance("process", 2015, 7, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testSamePeriodDifferentYearByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2015, 1, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 1, 1, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByInvalidPeriodUnit()
	  {
		HistoricProcessInstanceReport report = historyService.createHistoricProcessInstanceReport();

		try
		{
		  report.duration(null);
		  fail();
		}
		catch (NotValidException)
		{
		}
	  }

	  public virtual void testReportByStartedBeforeByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0).done();

		// start a second process instance
		createReportScenario().startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).done();

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 0, 16, 0, 0, 0);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedBefore(calendar).duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByStartedBeforeByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0).startAndCompleteProcessInstance("process", 2016, 0, 15, 10, 0).done();

		// start a second process instance
		createReportScenario().startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).done();

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 0, 16, 0, 0, 0);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedBefore(calendar).duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByInvalidStartedBefore()
	  {
		HistoricProcessInstanceReport report = historyService.createHistoricProcessInstanceReport();

		try
		{
		  report.startedBefore(null);
		  fail();
		}
		catch (NotValidException)
		{
		}
	  }

	  public virtual void testReportByStartedAfterByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		createReportScenario().startAndCompleteProcessInstance("process", 2015, 11, 15, 10, 0).done();

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).done();

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 0, 1, 0, 0, 0);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedAfter(calendar).duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByStartedAfterByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		createReportScenario().startAndCompleteProcessInstance("process", 2015, 11, 15, 10, 0).done();

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2016, 3, 1, 10, 0).done();

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 0, 1, 0, 0, 0);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedAfter(calendar).duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByInvalidStartedAfter()
	  {
		HistoricProcessInstanceReport report = historyService.createHistoricProcessInstanceReport();

		try
		{
		  report.startedAfter(null);
		  fail();
		}
		catch (NotValidException)
		{
		}
	  }

	  public virtual void testReportByStartedAfterAndStartedBeforeByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process", 2016, 1, 15, 10, 0).startAndCompleteProcessInstance("process", 2016, 2, 1, 10, 0).done();

		createReportScenario().startAndCompleteProcessInstance("process", 2016, 3, 15, 10, 0).done();

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 0, 1, 0, 0, 0);
		DateTime after = calendar;
		calendar = new DateTime(2016, 2, 31, 23, 59, 59);
		DateTime before = calendar;

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedAfter(after).startedBefore(before).duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByStartedAfterAndStartedBeforeByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process", 2016, 1, 15, 10, 0).startAndCompleteProcessInstance("process", 2016, 2, 1, 10, 0).done();

		createReportScenario().startAndCompleteProcessInstance("process", 2016, 3, 15, 10, 0).done();

		DateTime calendar = new DateTime();
		calendar = new DateTime(2016, 0, 1, 0, 0, 0);
		DateTime after = calendar;
		calendar = new DateTime(2016, 2, 31, 23, 59, 59);
		DateTime before = calendar;

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedAfter(after).startedBefore(before).duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportWithExcludingConditions()
	  {
		// given
		deployment(createProcessWithUserTask("process"));

		runtimeService.startProcessInstanceByKey("process");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);

		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().startedAfter(hourFromNow).startedBefore(hourAgo).duration(MONTH);

		// then
		assertEquals(0, result.Count);
	  }

	  public virtual void testReportByProcessDefinitionIdByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		string processDefinitionId1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult().Id;

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).done();

		createReportScenario().startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionId1).duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByProcessDefinitionIdByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		string processDefinitionId1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult().Id;

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).done();

		createReportScenario().startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionId1).duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByMultipleProcessDefinitionIdByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		string processDefinitionId1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult().Id;

		string processDefinitionId2 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").singleResult().Id;

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionId1, processDefinitionId2).duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByMultipleProcessDefinitionIdByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		string processDefinitionId1 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult().Id;

		string processDefinitionId2 = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").singleResult().Id;

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processDefinitionId1, processDefinitionId2).duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByInvalidProcessDefinitionId()
	  {
		HistoricProcessInstanceReport report = historyService.createHistoricProcessInstanceReport();

		try
		{
		  report.processDefinitionIdIn((string) null);
		}
		catch (NotValidException)
		{
		}

		try
		{
		  report.processDefinitionIdIn("abc", (string) null, "def");
		}
		catch (NotValidException)
		{
		}
	  }

	  public virtual void testReportByProcessDefinitionKeyByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).done();

		createReportScenario().startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn("process1").duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByProcessDefinitionKeyByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).done();

		createReportScenario().startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn("process1").duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByMultipleProcessDefinitionKeyByMonth()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(MONTH).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn("process1", "process2").duration(MONTH);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByMultipleProcessDefinitionKeyByQuarter()
	  {
		// given
		deployment(createProcessWithUserTask("process1"), createProcessWithUserTask("process2"));

		DurationReportResultAssertion assertion = createReportScenario().periodUnit(QUARTER).startAndCompleteProcessInstance("process1", 2016, 1, 15, 10, 0).startAndCompleteProcessInstance("process2", 2016, 3, 15, 10, 0).done();

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn("process1", "process2").duration(QUARTER);

		// then
		assertThat(result).matches(assertion);
	  }

	  public virtual void testReportByInvalidProcessDefinitionKey()
	  {
		HistoricProcessInstanceReport report = historyService.createHistoricProcessInstanceReport();

		try
		{
		  report.processDefinitionKeyIn((string) null);
		}
		catch (NotValidException)
		{
		}

		try
		{
		  report.processDefinitionKeyIn("abc", (string) null, "def");
		}
		catch (NotValidException)
		{
		}
	  }

	  protected internal virtual BpmnModelInstance createProcessWithUserTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().userTask().endEvent().done();
	  }

	  protected internal class DurationReportScenarioBuilder
	  {
		  private readonly HistoricProcessInstanceDurationReportTest outerInstance;

		  public DurationReportScenarioBuilder(HistoricProcessInstanceDurationReportTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal PeriodUnit periodUnit_Conflict = MONTH;

		protected internal DurationReportResultAssertion assertion = new DurationReportResultAssertion(outerInstance);

		public virtual DurationReportScenarioBuilder periodUnit(PeriodUnit periodUnit)
		{
		  this.periodUnit_Conflict = periodUnit;
		  assertion.PeriodUnit = periodUnit;
		  return this;
		}

		protected internal virtual void setCurrentTime(int year, int month, int dayOfMonth, int hourOfDay, int minute)
		{
		  DateTime calendar = new DateTime();
		  calendar = new DateTime(year, month, dayOfMonth, hourOfDay, minute, 0);
		  ClockUtil.CurrentTime = calendar;
		}

		protected internal virtual void addToCalendar(int field, int month)
		{
		  DateTime calendar = new DateTime();
		  calendar = new DateTime(ClockUtil.CurrentTime);
		  calendar.add(field, month);
		  ClockUtil.CurrentTime = calendar;
		}

		public virtual DurationReportScenarioBuilder startAndCompleteProcessInstance(string key, int year, int month, int dayOfMonth, int hourOfDay, int minute)
		{
		  setCurrentTime(year, month, dayOfMonth, hourOfDay, minute);

		  ProcessInstance pi = outerInstance.runtimeService.startProcessInstanceByKey(key);

		  int period = month;
		  if (periodUnit_Conflict == QUARTER)
		  {
			period = month / 3;
		  }
		  assertion.addDurationReportResult(period + 1, pi.Id);

		  addToCalendar(DateTime.MONTH, 5);
		  addToCalendar(DateTime.SECOND, outerInstance.random.Next(60));
		  Task task = outerInstance.taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		  outerInstance.taskService.complete(task.Id);

		  return this;
		}

		public virtual DurationReportResultAssertion done()
		{
		  return assertion;
		}

	  }

	  protected internal class DurationReportResultAssertion
	  {
		  private readonly HistoricProcessInstanceDurationReportTest outerInstance;

		  public DurationReportResultAssertion(HistoricProcessInstanceDurationReportTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		protected internal PeriodUnit periodUnit = MONTH;
		protected internal IDictionary<int, ISet<string>> periodToProcessInstancesMap = new Dictionary<int, ISet<string>>();

		public virtual DurationReportResultAssertion addDurationReportResult(int period, string processInstanceId)
		{
		  ISet<string> processInstances = periodToProcessInstancesMap[period];
		  if (processInstances == null)
		  {
			processInstances = new HashSet<string>();
			periodToProcessInstancesMap[period] = processInstances;
		  }
		  processInstances.Add(processInstanceId);
		  return this;
		}

		public virtual DurationReportResultAssertion setPeriodUnit(PeriodUnit periodUnit)
		{
		  this.periodUnit = periodUnit;
		  return this;
		}

		public virtual void assertReportResults(IList<DurationReportResult> actual)
		{
		  assertEquals("Report size", periodToProcessInstancesMap.Count, actual.Count);

		  foreach (DurationReportResult reportResult in actual)
		  {
			assertEquals("Period unit", periodUnit, reportResult.PeriodUnit);

			int period = reportResult.Period;
			ISet<string> processInstancesInPeriod = periodToProcessInstancesMap[period];
			assertNotNull("Unexpected report for period " + period, processInstancesInPeriod);

			IList<HistoricProcessInstance> historicProcessInstances = outerInstance.historyService.createHistoricProcessInstanceQuery().processInstanceIds(processInstancesInPeriod).finished().list();

			long max = 0;
			long min = 0;
			long sum = 0;

			for (int i = 0; i < historicProcessInstances.Count; i++)
			{
			  HistoricProcessInstance historicProcessInstance = historicProcessInstances[i];
			  long? duration = historicProcessInstance.DurationInMillis;
			  sum = sum + duration;
			  max = i > 0 ? Math.Max(max, duration) : duration.Value;
			  min = i > 0 ? Math.Min(min, duration) : duration.Value;
			}

			long avg = sum / historicProcessInstances.Count;

			assertEquals("maximum", max, reportResult.Maximum);
			assertEquals("minimum", min, reportResult.Minimum);
			assertEquals("average", avg, reportResult.Average, 1);
		  }
		}

	  }

	  protected internal class DurationReportResultAssert
	  {
		  private readonly HistoricProcessInstanceDurationReportTest outerInstance;


		protected internal IList<DurationReportResult> actual;

		public DurationReportResultAssert(HistoricProcessInstanceDurationReportTest outerInstance, IList<DurationReportResult> actual)
		{
			this.outerInstance = outerInstance;
		  this.actual = actual;
		}

		public virtual void matches(DurationReportResultAssertion assertion)
		{
		  assertion.assertReportResults(actual);
		}

	  }

	  protected internal virtual DurationReportScenarioBuilder createReportScenario()
	  {
		return new DurationReportScenarioBuilder(this);
	  }

	  protected internal virtual DurationReportResultAssert assertThat(IList<DurationReportResult> actual)
	  {
		return new DurationReportResultAssert(this, actual);
	  }

	}

}