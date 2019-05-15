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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoricActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricActivityInstanceEventEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Marcel Wieczorek
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricActivityInstanceTest : PluggableProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceNoop()
		public virtual void testHistoricActivityInstanceNoop()
		{
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("noopProcess");

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("noop").singleResult();

		assertEquals("noop", historicActivityInstance.ActivityId);
		assertEquals("serviceTask", historicActivityInstance.ActivityType);
		assertNotNull(historicActivityInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, historicActivityInstance.ProcessInstanceId);
		assertEquals(processInstance.Id, historicActivityInstance.ExecutionId);
		assertNotNull(historicActivityInstance.StartTime);
		assertNotNull(historicActivityInstance.EndTime);
		assertTrue(historicActivityInstance.DurationInMillis >= 0);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceReceive()
	  public virtual void testHistoricActivityInstanceReceive()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("receiveProcess");

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("receive").singleResult();

		assertEquals("receive", historicActivityInstance.ActivityId);
		assertEquals("receiveTask", historicActivityInstance.ActivityType);
		assertNull(historicActivityInstance.EndTime);
		assertNull(historicActivityInstance.DurationInMillis);
		assertNotNull(historicActivityInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, historicActivityInstance.ProcessInstanceId);
		assertEquals(processInstance.Id, historicActivityInstance.ExecutionId);
		assertNotNull(historicActivityInstance.StartTime);

		// move clock by 1 second
		DateTime now = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = new DateTime(now.Ticks + 1000);

		runtimeService.signal(processInstance.Id);

		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("receive").singleResult();

		assertEquals("receive", historicActivityInstance.ActivityId);
		assertEquals("receiveTask", historicActivityInstance.ActivityType);
		assertNotNull(historicActivityInstance.EndTime);
		assertNotNull(historicActivityInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, historicActivityInstance.ProcessInstanceId);
		assertEquals(processInstance.Id, historicActivityInstance.ExecutionId);
		assertNotNull(historicActivityInstance.StartTime);
		assertTrue(historicActivityInstance.DurationInMillis >= 1000);
		assertTrue(((HistoricActivityInstanceEventEntity)historicActivityInstance).DurationRaw >= 1000);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testHistoricActivityInstanceReceive.bpmn20.xml" })]
	  public virtual void testLongRunningHistoricActivityInstanceReceive()
	  {
		const long ONE_YEAR = 1000 * 60 * 60 * 24 * 365;

		DateTime cal = new DateTime();
		cal.set(DateTime.SECOND, 0);
		cal.set(DateTime.MILLISECOND, 0);

		ClockUtil.CurrentTime = cal;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("receiveProcess");

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("receive").singleResult();

		assertEquals("receive", historicActivityInstance.ActivityId);
		assertEquals("receiveTask", historicActivityInstance.ActivityType);
		assertNull(historicActivityInstance.EndTime);
		assertNull(historicActivityInstance.DurationInMillis);
		assertNotNull(historicActivityInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, historicActivityInstance.ProcessInstanceId);
		assertEquals(processInstance.Id, historicActivityInstance.ExecutionId);
		assertNotNull(historicActivityInstance.StartTime);

		// move clock by 1 year
		cal.AddYears(1);
		ClockUtil.CurrentTime = cal;

		runtimeService.signal(processInstance.Id);

		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("receive").singleResult();

		assertEquals("receive", historicActivityInstance.ActivityId);
		assertEquals("receiveTask", historicActivityInstance.ActivityType);
		assertNotNull(historicActivityInstance.EndTime);
		assertNotNull(historicActivityInstance.ProcessDefinitionId);
		assertEquals(processInstance.Id, historicActivityInstance.ProcessInstanceId);
		assertEquals(processInstance.Id, historicActivityInstance.ExecutionId);
		assertNotNull(historicActivityInstance.StartTime);
		assertTrue(historicActivityInstance.DurationInMillis.Value >= ONE_YEAR);
		assertTrue(((HistoricActivityInstanceEventEntity)historicActivityInstance).DurationRaw.Value >= ONE_YEAR);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceQuery()
	  public virtual void testHistoricActivityInstanceQuery()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("noopProcess");

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("nonExistingActivityId").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("noop").list().size());

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityType("nonExistingActivityType").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityType("serviceTask").list().size());

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityName("nonExistingActivityName").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityName("No operation").list().size());

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().taskAssignee("nonExistingAssignee").list().size());

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().executionId("nonExistingExecutionId").list().size());

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  assertEquals(3, historyService.createHistoricActivityInstanceQuery().executionId(processInstance.Id).list().size());
		}
		else
		{
		  assertEquals(0, historyService.createHistoricActivityInstanceQuery().executionId(processInstance.Id).list().size());
		}

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().processInstanceId("nonExistingProcessInstanceId").list().size());

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  assertEquals(3, historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).list().size());
		}
		else
		{
		  assertEquals(0, historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).list().size());
		}

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().processDefinitionId("nonExistingProcessDefinitionId").list().size());

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  assertEquals(3, historyService.createHistoricActivityInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId).list().size());
		}
		else
		{
		  assertEquals(0, historyService.createHistoricActivityInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId).list().size());
		}

		assertEquals(0, historyService.createHistoricActivityInstanceQuery().unfinished().list().size());

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  assertEquals(3, historyService.createHistoricActivityInstanceQuery().finished().list().size());
		}
		else
		{
		  assertEquals(0, historyService.createHistoricActivityInstanceQuery().finished().list().size());
		}

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().list().get(0);
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityInstanceId(historicActivityInstance.Id).list().size());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceForEventsQuery()
	  public virtual void testHistoricActivityInstanceForEventsQuery()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("eventProcess");
		assertEquals(1, taskService.createTaskQuery().count());
		runtimeService.signalEventReceived("signal");
		assertProcessEnded(pi.Id);

		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("noop").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("userTask").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("intermediate-event").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("start").list().size());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("end").list().size());

		 assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("boundaryEvent").list().size());

		HistoricActivityInstance intermediateEvent = historyService.createHistoricActivityInstanceQuery().activityId("intermediate-event").singleResult();
		assertNotNull(intermediateEvent.StartTime);
		assertNotNull(intermediateEvent.EndTime);

		HistoricActivityInstance startEvent = historyService.createHistoricActivityInstanceQuery().activityId("start").singleResult();
		assertNotNull(startEvent.StartTime);
		assertNotNull(startEvent.EndTime);

		HistoricActivityInstance endEvent = historyService.createHistoricActivityInstanceQuery().activityId("end").singleResult();
		assertNotNull(endEvent.StartTime);
		assertNotNull(endEvent.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceProperties()
	  public virtual void testHistoricActivityInstanceProperties()
	  {
		// Start process instance
		runtimeService.startProcessInstanceByKey("taskAssigneeProcess");

		// Get task list
		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("theTask").singleResult();

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals(task.Id, historicActivityInstance.TaskId);
		assertEquals("kermit", historicActivityInstance.Assignee);

		// change assignee of the task
		taskService.setAssignee(task.Id, "gonzo");
		task = taskService.createTaskQuery().singleResult();

		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("theTask").singleResult();
		assertEquals("gonzo", historicActivityInstance.Assignee);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/calledProcess.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testCallSimpleSubProcess.bpmn20.xml" })]
	  public virtual void testHistoricActivityInstanceCalledProcessId()
	  {
		runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("callSubProcess").singleResult();

		HistoricProcessInstance oldInstance = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("calledProcess").singleResult();

		assertEquals(oldInstance.Id, historicActivityInstance.CalledProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSorting()
	  public virtual void testSorting()
	  {
		runtimeService.startProcessInstanceByKey("process");

		int expectedActivityInstances = -1;
		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  expectedActivityInstances = 2;
		}
		else
		{
		  expectedActivityInstances = 0;
		}

		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceId().asc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceStartTime().asc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceEndTime().asc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceDuration().asc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByExecutionId().asc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessDefinitionId().asc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessInstanceId().asc().list().size());

		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceId().desc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceStartTime().desc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceEndTime().desc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceDuration().desc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByExecutionId().desc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessDefinitionId().desc().list().size());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessInstanceId().desc().list().size());

		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceId().asc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceStartTime().asc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceEndTime().asc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceDuration().asc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByExecutionId().asc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessDefinitionId().asc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessInstanceId().asc().count());

		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceId().desc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceStartTime().desc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceEndTime().desc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceDuration().desc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByExecutionId().desc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessDefinitionId().desc().count());
		assertEquals(expectedActivityInstances, historyService.createHistoricActivityInstanceQuery().orderByProcessInstanceId().desc().count());
	  }

	  public virtual void testInvalidSorting()
	  {
		try
		{
		  historyService.createHistoricActivityInstanceQuery().asc().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricActivityInstanceQuery().desc().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceDuration().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }
	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricActivityInstanceQueryStartFinishAfterBefore()
	  {
		DateTime startTime = new DateTime();

		ClockUtil.CurrentTime = startTime;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "businessKey123");

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		// Start/end dates
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedBefore(hourAgo).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedBefore(hourFromNow).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedAfter(hourFromNow).count());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("theTask").startedBefore(hourFromNow).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").startedBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("theTask").startedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").startedAfter(hourFromNow).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").startedAfter(hourFromNow).startedBefore(hourAgo).count());

		// After finishing process
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult().Id);
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finished().count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedBefore(hourFromNow).count());
		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedAfter(hourFromNow).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("theTask").finishedBefore(hourAgo).finishedAfter(hourFromNow).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceQueryByCompleteScope()
	  public virtual void testHistoricActivityInstanceQueryByCompleteScope()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		IList<Task> tasks = taskService.createTaskQuery().list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().completeScope();

		assertEquals(3, query.count());

		IList<HistoricActivityInstance> instances = query.list();

		foreach (HistoricActivityInstance instance in instances)
		{
		  if (!instance.ActivityId.Equals("innerEnd") && !instance.ActivityId.Equals("end1") && !instance.ActivityId.Equals("end2"))
		  {
			fail("Unexpected instance with activity id " + instance.ActivityId + " found.");
		  }
		}

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testHistoricActivityInstanceQueryByCompleteScope.bpmn")]
	  public virtual void testHistoricActivityInstanceQueryByCanceled()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().canceled();

		assertEquals(3, query.count());

		IList<HistoricActivityInstance> instances = query.list();

		foreach (HistoricActivityInstance instance in instances)
		{
		  if (!instance.ActivityId.Equals("subprocess") && !instance.ActivityId.Equals("userTask1") && !instance.ActivityId.Equals("userTask2"))
		  {
			fail("Unexpected instance with activity id " + instance.ActivityId + " found.");
		  }
		}

		assertProcessEnded(processInstance.Id);
	  }

	  public virtual void testHistoricActivityInstanceQueryByCompleteScopeAndCanceled()
	  {
		try
		{
		  historyService.createHistoricActivityInstanceQuery().completeScope().canceled().list();
		  fail("It should not be possible to query by completeScope and canceled.");
		}
		catch (ProcessEngineException)
		{
		  // exception expected
		}
	  }

	  /// <summary>
	  /// https://app.camunda.com/jira/browse/CAM-1537
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceGatewayEndTimes()
	  public virtual void testHistoricActivityInstanceGatewayEndTimes()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("gatewayEndTimes");

		TaskQuery query = taskService.createTaskQuery().orderByTaskName().asc();
		IList<Task> tasks = query.list();
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// process instance should have finished
		assertNotNull(historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult().EndTime);
		// gateways should have end timestamps
		assertNotNull(historyService.createHistoricActivityInstanceQuery().activityId("Gateway_0").singleResult().EndTime);

		// there exists two historic activity instances for "Gateway_1" (parallel join)
		HistoricActivityInstanceQuery historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("Gateway_1");

		assertEquals(2, historicActivityInstanceQuery.count());
		// they should have an end timestamp
		assertNotNull(historicActivityInstanceQuery.list().get(0).EndTime);
		assertNotNull(historicActivityInstanceQuery.list().get(1).EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricActivityInstanceTimerEvent()
	  public virtual void testHistoricActivityInstanceTimerEvent()
	  {
		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		Job timer = jobQuery.singleResult();
		managementService.executeJob(timer.Id);

		TaskQuery taskQuery = taskService.createTaskQuery();
		Task task = taskQuery.singleResult();

		assertEquals("afterTimer", task.Name);

		HistoricActivityInstanceQuery historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("gw1");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNotNull(historicActivityInstanceQuery.singleResult().EndTime);

		historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("timerEvent");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNotNull(historicActivityInstanceQuery.singleResult().EndTime);
		assertEquals("intermediateTimer", historicActivityInstanceQuery.singleResult().ActivityType);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testHistoricActivityInstanceTimerEvent.bpmn20.xml"})]
	  public virtual void testHistoricActivityInstanceMessageEvent()
	  {
		runtimeService.startProcessInstanceByKey("catchSignal");

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		EventSubscriptionQuery eventSubscriptionQuery = runtimeService.createEventSubscriptionQuery();
		assertEquals(1, eventSubscriptionQuery.count());

		runtimeService.correlateMessage("newInvoice");

		TaskQuery taskQuery = taskService.createTaskQuery();
		Task task = taskQuery.singleResult();

		assertEquals("afterMessage", task.Name);

		HistoricActivityInstanceQuery historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("gw1");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNotNull(historicActivityInstanceQuery.singleResult().EndTime);

		historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("messageEvent");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNotNull(historicActivityInstanceQuery.singleResult().EndTime);
		assertEquals("intermediateMessageCatch", historicActivityInstanceQuery.singleResult().ActivityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUserTaskStillRunning()
	  public virtual void testUserTaskStillRunning()
	  {
		runtimeService.startProcessInstanceByKey("nonInterruptingEvent");

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		managementService.executeJob(jobQuery.singleResult().Id);

		HistoricActivityInstanceQuery historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("userTask");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNull(historicActivityInstanceQuery.singleResult().EndTime);

		historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("end1");
		assertEquals(0, historicActivityInstanceQuery.count());

		historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("timer");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNotNull(historicActivityInstanceQuery.singleResult().EndTime);

		historicActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().activityId("end2");
		assertEquals(1, historicActivityInstanceQuery.count());
		assertNotNull(historicActivityInstanceQuery.singleResult().EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingBoundaryMessageEvent()
	  public virtual void testInterruptingBoundaryMessageEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("newMessage").singleResult();

		runtimeService.messageEventReceived("newMessage", execution.Id);

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("message");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundaryMessage", query.singleResult().ActivityType);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingBoundaryMessageEvent()
	  public virtual void testNonInterruptingBoundaryMessageEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("newMessage").singleResult();

		runtimeService.messageEventReceived("newMessage", execution.Id);

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("message");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundaryMessage", query.singleResult().ActivityType);

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingBoundarySignalEvent()
	  public virtual void testInterruptingBoundarySignalEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Execution execution = runtimeService.createExecutionQuery().signalEventSubscriptionName("newSignal").singleResult();

		runtimeService.signalEventReceived("newSignal", execution.Id);

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("signal");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundarySignal", query.singleResult().ActivityType);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingBoundarySignalEvent()
	  public virtual void testNonInterruptingBoundarySignalEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Execution execution = runtimeService.createExecutionQuery().signalEventSubscriptionName("newSignal").singleResult();

		runtimeService.signalEventReceived("newSignal", execution.Id);

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("signal");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundarySignal", query.singleResult().ActivityType);

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingBoundaryTimerEvent()
	  public virtual void testInterruptingBoundaryTimerEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("timer");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundaryTimer", query.singleResult().ActivityType);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingBoundaryTimerEvent()
	  public virtual void testNonInterruptingBoundaryTimerEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("timer");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundaryTimer", query.singleResult().ActivityType);

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryErrorEvent()
	  public virtual void testBoundaryErrorEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("error");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("boundaryError", query.singleResult().ActivityType);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryCancelEvent()
	  public virtual void testBoundaryCancelEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("catchCancel");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);
		assertEquals("cancelBoundaryCatch", query.singleResult().ActivityType);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryCompensateEvent()
	  public virtual void testBoundaryCompensateEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// the compensation boundary event should not appear in history!
		query.activityId("compensate");
		assertEquals(0, query.count());

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testBoundaryCompensateEvent.bpmn20.xml")]
	  public virtual void testCompensationServiceTaskHasEndTime()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("compensationServiceTask");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testBoundaryCancelEvent.bpmn20.xml")]
	  public virtual void testTransaction()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("transaction");
		assertEquals(1, query.count());
		assertNotNull(query.singleResult().EndTime);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScopeActivity()
	  public virtual void testScopeActivity()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		query.activityId("userTask");
		assertEquals(1, query.count());

		HistoricActivityInstance historicActivityInstance = query.singleResult();

		assertEquals(pi.Id, historicActivityInstance.ParentActivityInstanceId);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultiInstanceScopeActivity()
	  public virtual void testMultiInstanceScopeActivity()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		HistoricActivityInstance miBodyInstance = query.activityId("userTask#multiInstanceBody").singleResult();

		query.activityId("userTask");
		assertEquals(5, query.count());


		IList<HistoricActivityInstance> result = query.list();

		foreach (HistoricActivityInstance instance in result)
		{
		  assertEquals(miBodyInstance.Id, instance.ParentActivityInstanceId);
		}

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultiInstanceReceiveActivity()
	  public virtual void testMultiInstanceReceiveActivity()
	  {
		runtimeService.startProcessInstanceByKey("process");

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();
		HistoricActivityInstance miBodyInstance = query.activityId("receiveTask#multiInstanceBody").singleResult();

		query.activityId("receiveTask");
		assertEquals(5, query.count());

		IList<HistoricActivityInstance> result = query.list();

		foreach (HistoricActivityInstance instance in result)
		{
		  assertEquals(miBodyInstance.Id, instance.ParentActivityInstanceId);
		}

	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testEvents.bpmn")]
	  public virtual void testIntermediateCatchEventTypes()
	  {
		HistoricActivityInstanceQuery query = startEventTestProcess("");

		query.activityId("intermediateSignalCatchEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateSignalCatch", query.singleResult().ActivityType);

		query.activityId("intermediateMessageCatchEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateMessageCatch", query.singleResult().ActivityType);

		query.activityId("intermediateTimerCatchEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateTimer", query.singleResult().ActivityType);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testEvents.bpmn")]
	  public virtual void testIntermediateThrowEventTypes()
	  {
		HistoricActivityInstanceQuery query = startEventTestProcess("");

		query.activityId("intermediateSignalThrowEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateSignalThrow", query.singleResult().ActivityType);

		query.activityId("intermediateMessageThrowEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateMessageThrowEvent", query.singleResult().ActivityType);

		query.activityId("intermediateNoneThrowEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateNoneThrowEvent", query.singleResult().ActivityType);

		query.activityId("intermediateCompensationThrowEvent");
		assertEquals(1, query.count());
		assertEquals("intermediateCompensationThrowEvent", query.singleResult().ActivityType);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testEvents.bpmn")]
	  public virtual void testStartEventTypes()
	  {
		HistoricActivityInstanceQuery query = startEventTestProcess("");

		query.activityId("timerStartEvent");
		assertEquals(1, query.count());
		assertEquals("startTimerEvent", query.singleResult().ActivityType);

		query.activityId("noneStartEvent");
		assertEquals(1, query.count());
		assertEquals("startEvent", query.singleResult().ActivityType);

		query = startEventTestProcess("CAM-2365");
		query.activityId("messageStartEvent");
		assertEquals(1, query.count());
		assertEquals("messageStartEvent", query.singleResult().ActivityType);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testEvents.bpmn")]
	  public virtual void testEndEventTypes()
	  {
		HistoricActivityInstanceQuery query = startEventTestProcess("");

		query.activityId("cancellationEndEvent");
		assertEquals(1, query.count());
		assertEquals("cancelEndEvent", query.singleResult().ActivityType);

		query.activityId("messageEndEvent");
		assertEquals(1, query.count());
		assertEquals("messageEndEvent", query.singleResult().ActivityType);

		query.activityId("errorEndEvent");
		assertEquals(1, query.count());
		assertEquals("errorEndEvent", query.singleResult().ActivityType);

		query.activityId("signalEndEvent");
		assertEquals(1, query.count());
		assertEquals("signalEndEvent", query.singleResult().ActivityType);

		query.activityId("terminationEndEvent");
		assertEquals(1, query.count());
		assertEquals("terminateEndEvent", query.singleResult().ActivityType);

		query.activityId("noneEndEvent");
		assertEquals(1, query.count());
		assertEquals("noneEndEvent", query.singleResult().ActivityType);
	  }

	  private HistoricActivityInstanceQuery startEventTestProcess(string message)
	  {
		if (message.Equals(""))
		{
		  runtimeService.startProcessInstanceByKey("testEvents");
		}
		else
		{
		  runtimeService.startProcessInstanceByMessage("CAM-2365");
		}

		return historyService.createHistoricActivityInstanceQuery();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.startEventTypesForEventSubprocess.bpmn20.xml")]
	  public virtual void testMessageEventSubprocess()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["shouldThrowError"] = false;
		runtimeService.startProcessInstanceByKey("process", vars);

		runtimeService.correlateMessage("newMessage");

		HistoricActivityInstance historicActivity = historyService.createHistoricActivityInstanceQuery().activityId("messageStartEvent").singleResult();

		assertEquals("messageStartEvent", historicActivity.ActivityType);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.startEventTypesForEventSubprocess.bpmn20.xml")]
	  public virtual void testSignalEventSubprocess()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["shouldThrowError"] = false;
		runtimeService.startProcessInstanceByKey("process", vars);

		runtimeService.signalEventReceived("newSignal");

		HistoricActivityInstance historicActivity = historyService.createHistoricActivityInstanceQuery().activityId("signalStartEvent").singleResult();

		assertEquals("signalStartEvent", historicActivity.ActivityType);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.startEventTypesForEventSubprocess.bpmn20.xml")]
	  public virtual void testTimerEventSubprocess()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["shouldThrowError"] = false;
		runtimeService.startProcessInstanceByKey("process", vars);

		Job timerJob = managementService.createJobQuery().singleResult();
		managementService.executeJob(timerJob.Id);

		HistoricActivityInstance historicActivity = historyService.createHistoricActivityInstanceQuery().activityId("timerStartEvent").singleResult();

		assertEquals("startTimerEvent", historicActivity.ActivityType);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.startEventTypesForEventSubprocess.bpmn20.xml")]
	  public virtual void testErrorEventSubprocess()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["shouldThrowError"] = true;
		runtimeService.startProcessInstanceByKey("process", vars);

		HistoricActivityInstance historicActivity = historyService.createHistoricActivityInstanceQuery().activityId("errorStartEvent").singleResult();

		assertEquals("errorStartEvent", historicActivity.ActivityType);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testCaseCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCaseCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("process");

		string subCaseInstanceId = caseService.createCaseInstanceQuery().singleResult().Id;


		HistoricActivityInstance historicCallActivity = historyService.createHistoricActivityInstanceQuery().activityId("callActivity").singleResult();

		assertEquals(subCaseInstanceId, historicCallActivity.CalledCaseInstanceId);
		assertNull(historicCallActivity.EndTime);

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.completeCaseExecution(humanTaskId);

		historicCallActivity = historyService.createHistoricActivityInstanceQuery().activityId("callActivity").singleResult();

		assertEquals(subCaseInstanceId, historicCallActivity.CalledCaseInstanceId);
		assertNotNull(historicCallActivity.EndTime);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessDefinitionKeyProperty()
	  {
		// given
		string key = "oneTaskProcess";
		string processInstanceId = runtimeService.startProcessInstanceByKey(key).Id;

		// when
		HistoricActivityInstance activityInstance = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstanceId).activityId("theTask").singleResult();

		// then
		assertNotNull(activityInstance.ProcessDefinitionKey);
		assertEquals(key, activityInstance.ProcessDefinitionKey);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEndParallelJoin()
	  public virtual void testEndParallelJoin()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		IList<HistoricActivityInstance> activityInstance = historyService.createHistoricActivityInstanceQuery().processInstanceId(pi.Id).activityId("parallelJoinEnd").list();

		assertThat(activityInstance.Count, @is(2));
		assertThat(pi.Ended, @is(true));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testHistoricActivityInstanceProperties.bpmn20.xml"})]
	  public virtual void testAssigneeSavedWhenTaskSaved()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().activityId("theTask");

		runtimeService.startProcessInstanceByKey("taskAssigneeProcess");
		HistoricActivityInstance historicActivityInstance = query.singleResult();

		Task task = taskService.createTaskQuery().singleResult();

		// assume
		assertEquals("kermit", historicActivityInstance.Assignee);

		// when
		task.Assignee = "gonzo";
		taskService.saveTask(task);

		// then
		historicActivityInstance = query.singleResult();
		assertEquals("gonzo", historicActivityInstance.Assignee);
	  }

	}

}