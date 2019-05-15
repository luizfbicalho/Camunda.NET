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

	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Ignore = org.junit.Ignore;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricActivityStatisticsQueryTest : PluggableProcessEngineTestCase
	{

	  private SimpleDateFormat sdf = new SimpleDateFormat("dd.MM.yyyy HH:mm:ss");

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testNoRunningProcessInstances()
	  {
		string processDefinitionId = ProcessDefinitionId;

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(0, query.count());
		assertEquals(0, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleTask()
	  public virtual void testSingleTask()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		HistoricActivityStatistics statistic = statistics[0];

		assertEquals("task", statistic.Id);
		assertEquals(5, statistic.Instances);

		completeProcessInstances();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testFinishedProcessInstances()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		completeProcessInstances();

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(0, query.count());
		assertEquals(0, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleRunningTasks()
	  public virtual void testMultipleRunningTasks()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).orderByActivityId().asc();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(4, query.count());
		assertEquals(4, statistics.Count);

		// innerTask
		HistoricActivityStatistics innerTask = statistics[0];

		assertEquals("innerTask", innerTask.Id);
		assertEquals(25, innerTask.Instances);

		// subprocess
		HistoricActivityStatistics subProcess = statistics[1];

		assertEquals("subprocess", subProcess.Id);
		assertEquals(25, subProcess.Instances);

		// subprocess multi instance body
		HistoricActivityStatistics subProcessMiBody = statistics[2];

		assertEquals("subprocess#multiInstanceBody", subProcessMiBody.Id);
		assertEquals(5, subProcessMiBody.Instances);

		// task
		HistoricActivityStatistics task = statistics[3];

		assertEquals("task", task.Id);
		assertEquals(5, task.Instances);

		completeProcessInstances();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testWithCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.calledProcess.bpmn20.xml" })]
	  public virtual void testMultipleProcessDefinitions()
	  {
		string processId = ProcessDefinitionId;
		string calledProcessId = getProcessDefinitionIdByKey("calledProcess");

		startProcesses(5);

		startProcessesByKey(10, "calledProcess");

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processId).orderByActivityId().asc();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		// callActivity
		HistoricActivityStatistics calledActivity = statistics[0];

		assertEquals("callActivity", calledActivity.Id);
		assertEquals(5, calledActivity.Instances);

		query = historyService.createHistoricActivityStatisticsQuery(calledProcessId).orderByActivityId().asc();

		statistics = query.list();

		assertEquals(2, query.count());
		assertEquals(2, statistics.Count);

		// task1
		HistoricActivityStatistics task1 = statistics[0];

		assertEquals("task1", task1.Id);
		assertEquals(15, task1.Instances);

		// task2
		HistoricActivityStatistics task2 = statistics[1];

		assertEquals("task2", task2.Id);
		assertEquals(15, task2.Instances);

		completeProcessInstances();
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByFinished()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().orderByActivityId().asc();
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(2, query.count());
		assertEquals(2, statistics.Count);

		// start
		HistoricActivityStatistics start = statistics[0];

		assertEquals("start", start.Id);
		assertEquals(0, start.Instances);
		assertEquals(5, start.Finished);

		// task
		HistoricActivityStatistics task = statistics[1];

		assertEquals("task", task.Id);
		assertEquals(5, task.Instances);
		assertEquals(0, task.Finished);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByFinishedAfterFinishingSomeInstances()
	  {
		string processDefinitionId = ProcessDefinitionId;

		// start five instances
		startProcesses(5);

		// complete two task, so that two process instances are finished
		IList<Task> tasks = taskService.createTaskQuery().list();
		for (int i = 0; i < 2; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().orderByActivityId().asc();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(3, query.count());
		assertEquals(3, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(2, end.Finished);

		// start
		HistoricActivityStatistics start = statistics[1];

		assertEquals("start", start.Id);
		assertEquals(0, start.Instances);
		assertEquals(5, start.Finished);

		// task
		HistoricActivityStatistics task = statistics[2];

		assertEquals("task", task.Id);
		assertEquals(3, task.Instances);
		assertEquals(2, task.Finished);

		completeProcessInstances();
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testMultipleRunningTasks.bpmn20.xml")]
	  public virtual void testQueryByFinishedMultipleRunningTasks()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey("innerTask").list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().orderByActivityId().asc();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(9, query.count());
		assertEquals(9, statistics.Count);

		// end1
		HistoricActivityStatistics end1 = statistics[0];

		assertEquals("end1", end1.Id);
		assertEquals(0, end1.Instances);
		assertEquals(5, end1.Finished);

		// gtw
		HistoricActivityStatistics gtw = statistics[1];

		assertEquals("gtw", gtw.Id);
		assertEquals(0, gtw.Instances);
		assertEquals(5, gtw.Finished);

		// innerEnd
		HistoricActivityStatistics innerEnd = statistics[2];

		assertEquals("innerEnd", innerEnd.Id);
		assertEquals(0, innerEnd.Instances);
		assertEquals(25, innerEnd.Finished);

		// innerStart
		HistoricActivityStatistics innerStart = statistics[3];

		assertEquals("innerStart", innerStart.Id);
		assertEquals(0, innerStart.Instances);
		assertEquals(25, innerStart.Finished);

		// innerTask
		HistoricActivityStatistics innerTask = statistics[4];

		assertEquals("innerTask", innerTask.Id);
		assertEquals(0, innerTask.Instances);
		assertEquals(25, innerTask.Finished);

		// innerStart
		HistoricActivityStatistics start = statistics[5];

		assertEquals("start", start.Id);
		assertEquals(0, start.Instances);
		assertEquals(5, start.Finished);

		// subprocess
		HistoricActivityStatistics subProcess = statistics[6];

		assertEquals("subprocess", subProcess.Id);
		assertEquals(0, subProcess.Instances);
		assertEquals(25, subProcess.Finished);

		// subprocess - multi-instance body
		HistoricActivityStatistics subProcessMiBody = statistics[7];

		assertEquals("subprocess#multiInstanceBody", subProcessMiBody.Id);
		assertEquals(0, subProcessMiBody.Instances);
		assertEquals(5, subProcessMiBody.Finished);

		// task
		HistoricActivityStatistics task = statistics[8];

		assertEquals("task", task.Id);
		assertEquals(5, task.Instances);
		assertEquals(0, task.Finished);

		completeProcessInstances();
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCompleteScope()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		completeProcessInstances();

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCompleteScope();
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(5, end.CompleteScope);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCompleteScopeAfterFinishingSomeInstances()
	  {
		string processDefinitionId = ProcessDefinitionId;

		// start five instances
		startProcesses(5);

		// complete two task, so that two process instances are finished
		IList<Task> tasks = taskService.createTaskQuery().list();
		for (int i = 0; i < 2; i++)
		{
		  taskService.complete(tasks[i].Id);
		}

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCompleteScope().orderByActivityId().asc();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(2, query.count());
		assertEquals(2, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(2, end.CompleteScope);

		// task
		HistoricActivityStatistics task = statistics[1];

		assertEquals("task", task.Id);
		assertEquals(3, task.Instances);
		assertEquals(0, task.CompleteScope);

		completeProcessInstances();
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testMultipleRunningTasks.bpmn20.xml")]
	  public virtual void testQueryByCompleteScopeMultipleRunningTasks()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey("innerTask").list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCompleteScope().orderByActivityId().asc();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(4, query.count());
		assertEquals(4, statistics.Count);

		// end1
		HistoricActivityStatistics end1 = statistics[0];

		assertEquals("end1", end1.Id);
		assertEquals(0, end1.Instances);
		assertEquals(5, end1.CompleteScope);

		// innerEnd
		HistoricActivityStatistics innerEnd = statistics[1];

		assertEquals("innerEnd", innerEnd.Id);
		assertEquals(0, innerEnd.Instances);
		assertEquals(25, innerEnd.CompleteScope);

		// subprocess (completes the multi-instances body scope, see BPMN spec)
		HistoricActivityStatistics subprocess = statistics[2];

		assertEquals("subprocess", subprocess.Id);
		assertEquals(0, subprocess.Instances);
		assertEquals(25, subprocess.CompleteScope);

		// task
		HistoricActivityStatistics task = statistics[3];

		assertEquals("task", task.Id);
		assertEquals(5, task.Instances);
		assertEquals(0, task.CompleteScope);

		completeProcessInstances();
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCanceled()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		cancelProcessInstances();

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		// task
		HistoricActivityStatistics task = statistics[0];

		assertEquals("task", task.Id);
		assertEquals(0, task.Instances);
		assertEquals(5, task.Canceled);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCanceledAfterCancelingSomeInstances()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(3);

		// cancel running process instances
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "test");
		}

		startProcesses(2);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled();

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		// task
		HistoricActivityStatistics task = statistics[0];

		assertEquals("task", task.Id);
		assertEquals(2, task.Instances);
		assertEquals(3, task.Canceled);

		completeProcessInstances();
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCanceledAndFinished()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(2);

		// cancel running process instances
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "test");
		}

		startProcesses(2);

		// complete running tasks
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		startProcesses(2);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().orderByActivityId().asc();
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(3, query.count());
		assertEquals(3, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(0, end.Canceled);
		assertEquals(2, end.Finished);

		// start
		HistoricActivityStatistics start = statistics[1];

		assertEquals("start", start.Id);
		assertEquals(0, start.Instances);
		assertEquals(0, start.Canceled);
		assertEquals(6, start.Finished);

		// task
		HistoricActivityStatistics task = statistics[2];

		assertEquals("task", task.Id);
		assertEquals(2, task.Instances);
		assertEquals(2, task.Canceled);
		assertEquals(4, task.Finished);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml") public void testQueryByCanceledAndFinishedByPeriods() throws java.text.ParseException
	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCanceledAndFinishedByPeriods()
	  {
		try
		{

		  //start two process instances
		  ClockUtil.CurrentTime = sdf.parse("15.01.2016 12:00:00");
		  startProcesses(2);

		  // cancel running process instances
		  ClockUtil.CurrentTime = sdf.parse("15.02.2016 12:00:00");
		  IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		  foreach (ProcessInstance processInstance in processInstances)
		  {
			runtimeService.deleteProcessInstance(processInstance.Id, "test");
		  }

		  //start two process instances
		  ClockUtil.CurrentTime = sdf.parse("01.02.2016 12:00:00");
		  startProcesses(2);

		  // complete running tasks
		  ClockUtil.CurrentTime = sdf.parse("25.02.2016 12:00:00");
		  IList<Task> tasks = taskService.createTaskQuery().list();
		  foreach (Task task in tasks)
		  {
			taskService.complete(task.Id);
		  }

		  //starte two more process instances
		  ClockUtil.CurrentTime = sdf.parse("15.03.2016 12:00:00");
		  startProcesses(2);

		  //NOW
		  ClockUtil.CurrentTime = sdf.parse("25.03.2016 12:00:00");

		  string processDefinitionId = ProcessDefinitionId;
		  //check January by started dates
		  HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().startedAfter(sdf.parse("01.01.2016 00:00:00")).startedBefore(sdf.parse("31.01.2016 23:59:59")).orderByActivityId().asc();
		  IList<HistoricActivityStatistics> statistics = query.list();

		  assertEquals(2, query.count());
		  assertEquals(2, statistics.Count);

		  // start
		  assertActivityStatistics(statistics[0], "start", 0, 0, 2);

		  // task
		  assertActivityStatistics(statistics[1], "task", 0, 2, 2);

		  //check January by finished dates
		  query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().finishedAfter(sdf.parse("01.01.2016 00:00:00")).finishedBefore(sdf.parse("31.01.2016 23:59:59")).orderByActivityId().asc();
		  statistics = query.list();

		  assertEquals(0, query.count());
		  assertEquals(0, statistics.Count);

		  //check February by started dates
		  query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().startedAfter(sdf.parse("01.02.2016 00:00:00")).startedBefore(sdf.parse("28.02.2016 23:59:59")).orderByActivityId().asc();
		  statistics = query.list();

		  assertEquals(3, query.count());
		  assertEquals(3, statistics.Count);

		  // end
		  assertActivityStatistics(statistics[0], "end", 0, 0, 2);

		  // start
		  assertActivityStatistics(statistics[1], "start", 0, 0, 2);

		  // task
		  assertActivityStatistics(statistics[2], "task", 0, 0, 2);

		  //check February by finished dates
		  query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().finishedAfter(sdf.parse("01.02.2016 00:00:00")).finishedBefore(sdf.parse("28.02.2016 23:59:59")).orderByActivityId().asc();
		  statistics = query.list();

		  assertEquals(3, query.count());
		  assertEquals(3, statistics.Count);

		  // end
		  assertActivityStatistics(statistics[0], "end", 0, 0, 2);

		  // start
		  assertActivityStatistics(statistics[1], "start", 0, 0, 4);

		  // task
		  assertActivityStatistics(statistics[2], "task", 0, 2, 4);

		  //check March by started dates
		  query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().startedAfter(sdf.parse("01.03.2016 00:00:00")).orderByActivityId().asc();
		  statistics = query.list();

		  assertEquals(2, query.count());
		  assertEquals(2, statistics.Count);

		  // start
		  assertActivityStatistics(statistics[0], "start", 0, 0, 2);

		  // task
		  assertActivityStatistics(statistics[1], "task", 2, 0, 0);

		  //check March by finished dates
		  query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().finishedAfter(sdf.parse("01.03.2016 00:00:00")).orderByActivityId().asc();
		  statistics = query.list();

		  assertEquals(0, query.count());
		  assertEquals(0, statistics.Count);

		  //check whole period by started date
		  query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeFinished().startedAfter(sdf.parse("01.01.2016 00:00:00")).orderByActivityId().asc();
		  statistics = query.list();

		  assertEquals(3, query.count());
		  assertEquals(3, statistics.Count);

		  // end
		  assertActivityStatistics(statistics[0], "end", 0, 0, 2);

		  // start
		  assertActivityStatistics(statistics[1], "start", 0, 0, 6);

		  // task
		  assertActivityStatistics(statistics[2], "task", 2, 2, 4);

		}
		finally
		{
		  ClockUtil.reset();
		}

	  }

	  protected internal virtual void assertActivityStatistics(HistoricActivityStatistics activity, string activityName, long instances, long canceled, long finished)
	  {
		assertEquals(activityName, activity.Id);
		assertEquals(instances, activity.Instances);
		assertEquals(canceled, activity.Canceled);
		assertEquals(finished, activity.Finished);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByCanceledAndCompleteScope()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(2);

		// cancel running process instances
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "test");
		}

		startProcesses(2);

		// complete running tasks
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		startProcesses(2);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled().includeCompleteScope().orderByActivityId().asc();
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(2, query.count());
		assertEquals(2, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(0, end.Canceled);
		assertEquals(2, end.CompleteScope);

		// task
		HistoricActivityStatistics task = statistics[1];

		assertEquals("task", task.Id);
		assertEquals(2, task.Instances);
		assertEquals(2, task.Canceled);
		assertEquals(0, task.CompleteScope);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByFinishedAndCompleteScope()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(2);

		// cancel running process instances
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "test");
		}

		startProcesses(2);

		// complete running tasks
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		startProcesses(2);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().includeCompleteScope().orderByActivityId().asc();
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(3, query.count());
		assertEquals(3, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(2, end.Finished);
		assertEquals(2, end.CompleteScope);

		// start
		HistoricActivityStatistics start = statistics[1];

		assertEquals("start", start.Id);
		assertEquals(0, start.Instances);
		assertEquals(6, start.Finished);
		assertEquals(0, start.CompleteScope);

		// task
		HistoricActivityStatistics task = statistics[2];

		assertEquals("task", task.Id);
		assertEquals(2, task.Instances);
		assertEquals(4, task.Finished);
		assertEquals(0, task.CompleteScope);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testQueryByFinishedAndCompleteScopeAndCanceled()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(2);

		// cancel running process instances
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance processInstance in processInstances)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "test");
		}

		startProcesses(2);

		// complete running tasks
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		startProcesses(2);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().includeCompleteScope().includeCanceled().orderByActivityId().asc();
		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(3, query.count());
		assertEquals(3, statistics.Count);

		// end
		HistoricActivityStatistics end = statistics[0];

		assertEquals("end", end.Id);
		assertEquals(0, end.Instances);
		assertEquals(0, end.Canceled);
		assertEquals(2, end.Finished);
		assertEquals(2, end.CompleteScope);

		// start
		HistoricActivityStatistics start = statistics[1];

		assertEquals("start", start.Id);
		assertEquals(0, start.Instances);
		assertEquals(0, start.Canceled);
		assertEquals(6, start.Finished);
		assertEquals(0, start.CompleteScope);

		// task
		HistoricActivityStatistics task = statistics[2];

		assertEquals("task", task.Id);
		assertEquals(2, task.Instances);
		assertEquals(2, task.Canceled);
		assertEquals(4, task.Finished);
		assertEquals(0, task.CompleteScope);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml")]
	  public virtual void testSorting()
	  {
		string processDefinitionId = ProcessDefinitionId;

		startProcesses(5);

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		assertEquals(1, query.orderByActivityId().asc().list().size());
		assertEquals(1, query.orderByActivityId().desc().list().size());

		assertEquals(1, query.orderByActivityId().asc().count());
		assertEquals(1, query.orderByActivityId().desc().count());
	  }

	  [Deployment(resources: {"org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testSingleTask.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricActivityStatisticsQueryTest.testAnotherSingleTask.bpmn20.xml"})]
	  public virtual void testDifferentProcessesWithSameActivityId()
	  {
		string processDefinitionId = ProcessDefinitionId;
		string anotherProcessDefinitionId = getProcessDefinitionIdByKey("anotherProcess");

		startProcesses(5);

		startProcessesByKey(10, "anotherProcess");

		// first processDefinition
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		IList<HistoricActivityStatistics> statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		HistoricActivityStatistics task = statistics[0];
		assertEquals(5, task.Instances);

		// second processDefinition
		query = historyService.createHistoricActivityStatisticsQuery(anotherProcessDefinitionId);

		statistics = query.list();

		assertEquals(1, query.count());
		assertEquals(1, statistics.Count);

		task = statistics[0];
		assertEquals(10, task.Instances);

	  }

	  protected internal virtual void completeProcessInstances()
	  {
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
	  }

	  protected internal virtual void cancelProcessInstances()
	  {
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		foreach (ProcessInstance pi in processInstances)
		{
		  runtimeService.deleteProcessInstance(pi.Id, "test");
		}
	  }

	  protected internal virtual void startProcesses(int numberOfInstances)
	  {
		startProcessesByKey(numberOfInstances, "process");
	  }

	  protected internal virtual void startProcessesByKey(int numberOfInstances, string key)
	  {
		for (int i = 0; i < numberOfInstances; i++)
		{
		  runtimeService.startProcessInstanceByKey(key);
		}
	  }

	  protected internal virtual string getProcessDefinitionIdByKey(string key)
	  {
		return repositoryService.createProcessDefinitionQuery().processDefinitionKey(key).singleResult().Id;
	  }

	  protected internal virtual string ProcessDefinitionId
	  {
		  get
		  {
			return getProcessDefinitionIdByKey("process");
		  }
	  }

	}

}