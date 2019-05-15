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
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using CallActivityModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CallActivityModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicProcessInstanceByProcessDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicProcessInstanceByProcessDefinitionKey;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicProcessInstanceByProcessDefinitionName;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicProcessInstanceByProcessDefinitionVersion;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicProcessInstanceByProcessInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricProcessInstanceTest : PluggableProcessEngineTestCase
	{
		[Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
		public virtual void testHistoricDataCreatedForProcessExecution()
		{

		DateTime calendar = new GregorianCalendar();
		calendar.set(DateTime.YEAR, 2010);
		calendar.set(DateTime.MONTH, 8);
		calendar.set(DateTime.DAY_OF_MONTH, 30);
		calendar.set(DateTime.HOUR_OF_DAY, 12);
		calendar.set(DateTime.MINUTE, 0);
		calendar.set(DateTime.SECOND, 0);
		calendar.set(DateTime.MILLISECOND, 0);
		DateTime noon = calendar;

		ClockUtil.CurrentTime = noon;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "myBusinessKey");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "myBusinessKey");

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().unfinished().count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finished().count());
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(historicProcessInstance);
		assertEquals(processInstance.Id, historicProcessInstance.Id);
		assertEquals(processInstance.BusinessKey, historicProcessInstance.BusinessKey);
		assertEquals(processInstance.ProcessDefinitionId, historicProcessInstance.ProcessDefinitionId);
		assertEquals(noon, historicProcessInstance.StartTime);
		assertNull(historicProcessInstance.EndTime);
		assertNull(historicProcessInstance.DurationInMillis);
		assertNull(historicProcessInstance.CaseInstanceId);

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();

		assertEquals(1, tasks.Count);

		// in this test scenario we assume that 25 seconds after the process start, the
		// user completes the task (yes! he must be almost as fast as me)
		DateTime twentyFiveSecsAfterNoon = new DateTime(noon.Ticks + 25 * 1000);
		ClockUtil.CurrentTime = twentyFiveSecsAfterNoon;
		taskService.complete(tasks[0].Id);

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(historicProcessInstance);
		assertEquals(processInstance.Id, historicProcessInstance.Id);
		assertEquals(processInstance.ProcessDefinitionId, historicProcessInstance.ProcessDefinitionId);
		assertEquals(noon, historicProcessInstance.StartTime);
		assertEquals(twentyFiveSecsAfterNoon, historicProcessInstance.EndTime);
		assertEquals(new long?(25 * 1000), historicProcessInstance.DurationInMillis);
		assertTrue(((HistoricProcessInstanceEventEntity) historicProcessInstance).DurationRaw >= 25000);
		assertNull(historicProcessInstance.CaseInstanceId);

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().unfinished().count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finished().count());

		runtimeService.startProcessInstanceByKey("oneTaskProcess", "myBusinessKey");
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finished().count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().unfinished().count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finished().unfinished().count());
		}

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testLongRunningHistoricDataCreatedForProcessExecution()
	  {
		const long ONE_YEAR = 1000 * 60 * 60 * 24 * 365;

		DateTime cal = new DateTime();
		cal.set(DateTime.SECOND, 0);
		cal.set(DateTime.MILLISECOND, 0);

		DateTime now = cal;
		ClockUtil.CurrentTime = now;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "myBusinessKey");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "myBusinessKey");

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().unfinished().count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finished().count());
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		assertEquals(now, historicProcessInstance.StartTime);

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		assertEquals(1, tasks.Count);

		// in this test scenario we assume that one year after the process start, the
		// user completes the task (incredible speedy!)
		cal.AddYears(1);
		DateTime oneYearLater = cal;
		ClockUtil.CurrentTime = oneYearLater;

		taskService.complete(tasks[0].Id);

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		assertEquals(now, historicProcessInstance.StartTime);
		assertEquals(oneYearLater, historicProcessInstance.EndTime);
		assertTrue(historicProcessInstance.DurationInMillis.Value >= ONE_YEAR);
		assertTrue(((HistoricProcessInstanceEventEntity)historicProcessInstance).DurationRaw.Value >= ONE_YEAR);

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().unfinished().count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finished().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteProcessInstanceHistoryCreated()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertNotNull(processInstance);

		// delete process instance should not delete the history
		runtimeService.deleteProcessInstance(processInstance.Id, "cancel");
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(historicProcessInstance.EndTime);
	  }

	  public virtual void testDeleteProcessInstanceWithoutSubprocessInstances()
	  {
		// given a process instance with subprocesses
		BpmnModelInstance calling = Bpmn.createExecutableProcess("calling").startEvent().callActivity().calledElement("called").endEvent("endA").done();

		BpmnModelInstance called = Bpmn.createExecutableProcess("called").startEvent().userTask("Task1").endEvent().done();

		deployment(calling, called);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("calling");

		// when the process instance is deleted and we do skip sub processes
		string id = instance.Id;
		runtimeService.deleteProcessInstance(id, "test_purposes", false, true, false, true);

		// then
		IList<HistoricProcessInstance> historicSubprocessList = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("called").list();
		foreach (HistoricProcessInstance historicProcessInstance in historicSubprocessList)
		{
		  assertNull(historicProcessInstance.SuperProcessInstanceId);
		}
	  }

	  public virtual void testDeleteProcessInstanceWithSubprocessInstances()
	  {
		// given a process instance with subprocesses
		BpmnModelInstance calling = Bpmn.createExecutableProcess("calling").startEvent().callActivity().calledElement("called").endEvent("endA").done();

		BpmnModelInstance called = Bpmn.createExecutableProcess("called").startEvent().userTask("Task1").endEvent().done();

		deployment(calling, called);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("calling");

		// when the process instance is deleted and we do not skip sub processes
		string id = instance.Id;
		runtimeService.deleteProcessInstance(id, "test_purposes", false, true, false, false);

		// then
		IList<HistoricProcessInstance> historicSubprocessList = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("called").list();
		foreach (HistoricProcessInstance historicProcessInstance in historicSubprocessList)
		{
		  assertNotNull(historicProcessInstance.SuperProcessInstanceId);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceStartDate()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		DateTime date = DateTime.Now;

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().startDateOn(date).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().startDateBy(date).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().startDateBy(DateUtils.addDays(date, -1)).count());

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().startDateBy(DateUtils.addDays(date, 1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().startDateOn(DateUtils.addDays(date, -1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().startDateOn(DateUtils.addDays(date, 1)).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceFinishDateUnfinished()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		DateTime date = DateTime.Now;

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateOn(date).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateBy(date).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateBy(DateUtils.addDays(date, 1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateBy(DateUtils.addDays(date, -1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateOn(DateUtils.addDays(date, -1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateOn(DateUtils.addDays(date, 1)).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceFinishDateFinished()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		DateTime date = DateTime.Now;

		runtimeService.deleteProcessInstance(pi.Id, "cancel");

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finishDateOn(date).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finishDateBy(date).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finishDateBy(DateUtils.addDays(date, 1)).count());

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateBy(DateUtils.addDays(date, -1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateOn(DateUtils.addDays(date, -1)).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishDateOn(DateUtils.addDays(date, 1)).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceDelete()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		runtimeService.deleteProcessInstance(pi.Id, "cancel");

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicProcessInstance.DeleteReason);
		assertEquals("cancel", historicProcessInstance.DeleteReason);

		assertNotNull(historicProcessInstance.EndTime);
	  }

	  /// <summary>
	  /// See: https://app.camunda.com/jira/browse/CAM-1324 </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricProcessInstanceDeleteAsync()
	  public virtual void testHistoricProcessInstanceDeleteAsync()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failing");

		runtimeService.deleteProcessInstance(pi.Id, "cancel");

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicProcessInstance.DeleteReason);
		assertEquals("cancel", historicProcessInstance.DeleteReason);

		assertNotNull(historicProcessInstance.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcessInstanceQueryWithIncidents()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcessInstanceQueryWithIncidents()
	  {
		// start instance with incidents
		runtimeService.startProcessInstanceByKey("Process_1");
		executeAvailableJobs();

		// start instance without incidents
		runtimeService.startProcessInstanceByKey("Process_1");

		assertEquals(2, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(2, historyService.createHistoricProcessInstanceQuery().list().size());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().withIncidents().count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().withIncidents().list().size());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentMessageLike("Unknown property used%\\_Tr%").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentMessageLike("Unknown property used%\\_Tr%").list().size());

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().incidentMessageLike("Unknown message%").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().incidentMessageLike("Unknown message%").list().size());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown property used in expression: ${incidentTrigger1}. Cause: Cannot resolve identifier 'incidentTrigger1'").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown property used in expression: ${incidentTrigger1}. Cause: Cannot resolve identifier 'incidentTrigger1'").list().size());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown property used in expression: ${incident_Trigger2}. Cause: Cannot resolve identifier 'incident_Trigger2'").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown property used in expression: ${incident_Trigger2}. Cause: Cannot resolve identifier 'incident_Trigger2'").list().size());

		assertEquals(0, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown message").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown message").list().size());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentType("failedJob").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentType("failedJob").list().size());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().withRootIncidents().count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().withRootIncidents().list().size());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldDeleteIncidentAfterJobWasSuccessfully.bpmn"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcessInstanceQueryIncidentStatusOpen()
	  {
		//given a processes instance, which will fail
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		runtimeService.startProcessInstanceByKey("failingProcessWithUserTask", parameters);

		//when jobs are executed till retry count is zero
		executeAvailableJobs();

		//then query for historic process instance with open incidents will return one
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentStatus("open").count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldDeleteIncidentAfterJobWasSuccessfully.bpmn"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcessInstanceQueryIncidentStatusResolved()
	  {
		//given a incident processes instance
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("failingProcessWithUserTask", parameters);
		executeAvailableJobs();

		//when `fail` variable is set to true and job retry count is set to one and executed again
		runtimeService.setVariable(pi1.Id, "fail", false);
		Job jobToResolve = managementService.createJobQuery().processInstanceId(pi1.Id).singleResult();
		managementService.setJobRetries(jobToResolve.Id, 1);
		executeAvailableJobs();

		//then query for historic process instance with resolved incidents will return one
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentStatus("resolved").count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldDeleteIncidentAfterJobWasSuccessfully.bpmn"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcessInstanceQueryIncidentStatusOpenWithTwoProcesses()
	  {
		//given two processes, which will fail, are started
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("failingProcessWithUserTask", parameters);
		runtimeService.startProcessInstanceByKey("failingProcessWithUserTask", parameters);
		executeAvailableJobs();
		assertEquals(2, historyService.createHistoricProcessInstanceQuery().incidentStatus("open").count());

		//when 'fail' variable is set to false, job retry count is set to one
		//and available jobs are executed
		runtimeService.setVariable(pi1.Id, "fail", false);
		Job jobToResolve = managementService.createJobQuery().processInstanceId(pi1.Id).singleResult();
		managementService.setJobRetries(jobToResolve.Id, 1);
		executeAvailableJobs();

		//then query with open and with resolved incidents returns one
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentStatus("open").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().incidentStatus("resolved").count());
	  }

	  public virtual void testHistoricProcessInstanceQueryWithIncidentMessageNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().incidentMessage(null).count();
		  fail("incidentMessage with null value is not allowed");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

	  public virtual void testHistoricProcessInstanceQueryWithIncidentMessageLikeNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().incidentMessageLike(null).count();
		  fail("incidentMessageLike with null value is not allowed");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneAsyncTaskProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceQuery()
	  {
		DateTime startTime = new DateTime();

		ClockUtil.CurrentTime = startTime;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", "businessKey_123");
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		// Start/end dates
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedBefore(hourAgo).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedBefore(hourFromNow).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedAfter(hourFromNow).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().startedBefore(hourFromNow).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().startedBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().startedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().startedAfter(hourFromNow).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().startedAfter(hourFromNow).startedBefore(hourAgo).count());

		// General fields
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finished().count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionId(processInstance.ProcessDefinitionId).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKey("businessKey_123").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKeyLike("business%").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKeyLike("%sinessKey\\_123").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKeyLike("%siness%").count());

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionName("The One Task_Process").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionNameLike("The One Task%").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionNameLike("%One Task\\_Process").count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionNameLike("%One Task%").count());

		IList<string> exludeIds = new List<string>();
		exludeIds.Add("unexistingProcessDefinition");

		assertEquals(1, historyService.createHistoricProcessInstanceQuery().processDefinitionKeyNotIn(exludeIds).count());

		exludeIds.Add("oneTaskProcess");
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey("oneTaskProcess").processDefinitionKeyNotIn(exludeIds).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKeyNotIn(exludeIds).count());

		try
		{
		  // oracle handles empty string like null which seems to lead to undefined behavior of the LIKE comparison
		  historyService.createHistoricProcessInstanceQuery().processDefinitionKeyNotIn(Arrays.asList(""));
		  fail("Exception expected");
		}
		catch (NotValidException)
		{
		  // expected
		}

		// After finishing process
		taskService.complete(taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult().Id);
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finished().count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finishedBefore(hourFromNow).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().finishedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedAfter(hourFromNow).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().finishedAfter(hourFromNow).finishedBefore(hourAgo).count());

		// No incidents should are created
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().withIncidents().count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().incidentMessageLike("Unknown property used%").count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().incidentMessage("Unknown property used in expression: #{failing}. Cause: Cannot resolve identifier 'failing'").count());

		// execute activities
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().executedActivityAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().executedActivityBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricProcessInstanceQuery().executedActivityBefore(hourFromNow).count());
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().executedActivityAfter(hourFromNow).count());

		// execute jobs
		if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  assertEquals(1, historyService.createHistoricProcessInstanceQuery().executedJobAfter(hourAgo).count());
		  assertEquals(0, historyService.createHistoricProcessInstanceQuery().executedActivityBefore(hourAgo).count());
		  assertEquals(1, historyService.createHistoricProcessInstanceQuery().executedActivityBefore(hourFromNow).count());
		  assertEquals(0, historyService.createHistoricProcessInstanceQuery().executedActivityAfter(hourFromNow).count());
		}
	  }

	  public virtual void testHistoricProcessInstanceSorting()
	  {

		deployment("org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml");
		deployment("org/camunda/bpm/engine/test/history/HistoricActivityInstanceTest.testSorting.bpmn20.xml");

		//deploy second version of the same process definition
		deployment("org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml");

		IList<ProcessDefinition> processDefinitions = processEngine.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").list();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  runtimeService.startProcessInstanceById(processDefinition.Id);
		}
		runtimeService.startProcessInstanceByKey("process");

		IList<HistoricProcessInstance> processInstances = historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceId().asc().list();
		assertEquals(3, processInstances.Count);
		verifySorting(processInstances, historicProcessInstanceByProcessInstanceId());

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceStartTime().asc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceEndTime().asc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceDuration().asc().list().size());

		processInstances = historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionId().asc().list();
		assertEquals(3, processInstances.Count);
		verifySorting(processInstances, historicProcessInstanceByProcessDefinitionId());

		processInstances = historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionKey().asc().list();
		assertEquals(3, processInstances.Count);
		verifySorting(processInstances, historicProcessInstanceByProcessDefinitionKey());

		processInstances = historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionName().asc().list();
		assertEquals(3, processInstances.Count);
		verifySorting(processInstances, historicProcessInstanceByProcessDefinitionName());

		processInstances = historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionVersion().asc().list();
		assertEquals(3, processInstances.Count);
		verifySorting(processInstances, historicProcessInstanceByProcessDefinitionVersion());

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceBusinessKey().asc().list().size());

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceId().desc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceStartTime().desc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceEndTime().desc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceDuration().desc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionId().desc().list().size());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceBusinessKey().desc().list().size());

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceId().asc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceStartTime().asc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceEndTime().asc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceDuration().asc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionId().asc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceBusinessKey().asc().count());

		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceId().desc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceStartTime().desc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceEndTime().desc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceDuration().desc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessDefinitionId().desc().count());
		assertEquals(3, historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceBusinessKey().desc().count());

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testHistoricProcessInstanceSubProcess()
	  {
		ProcessInstance superPi = runtimeService.startProcessInstanceByKey("subProcessQueryTest");
		ProcessInstance subPi = runtimeService.createProcessInstanceQuery().superProcessInstanceId(superPi.ProcessInstanceId).singleResult();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().subProcessInstanceId(subPi.ProcessInstanceId).singleResult();
		assertNotNull(historicProcessInstance);
		assertEquals(historicProcessInstance.Id, superPi.Id);
	  }

	  public virtual void testInvalidSorting()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().asc();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricProcessInstanceQuery().desc();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricProcessInstanceQuery().orderByProcessInstanceId().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteReason()
	  {
		if (!ProcessEngineConfiguration.HISTORY_NONE.Equals(processEngineConfiguration.History))
		{
		  const string deleteReason = "some delete reason";
		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		  runtimeService.deleteProcessInstance(pi.Id, deleteReason);
		  HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().processInstanceId(pi.Id).singleResult();
		  assertEquals(deleteReason, hpi.DeleteReason);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLongProcessDefinitionKey()
	  public virtual void testLongProcessDefinitionKey()
	  {
		// must be equals to attribute id of element process in process model
		const string PROCESS_DEFINITION_KEY = "myrealrealrealrealrealrealrealrealrealrealreallongprocessdefinitionkeyawesome";

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		// get HPI by process instance id
		HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(hpi);
		assertProcessEnded(hpi.Id);

		// get HPI by process definition key
		HistoricProcessInstance hpi2 = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertNotNull(hpi2);
		assertProcessEnded(hpi2.Id);

		// check we got the same HPIs
		assertEquals(hpi.Id, hpi2.Id);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testQueryByCaseInstanceId.cmmn", "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testQueryByCaseInstanceId.bpmn20.xml" })]
	  public virtual void testQueryByCaseInstanceId()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		query.caseInstanceId(caseInstanceId);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());

		HistoricProcessInstance historicProcessInstance = query.singleResult();
		assertNotNull(historicProcessInstance);
		assertNull(historicProcessInstance.EndTime);

		assertEquals(caseInstanceId, historicProcessInstance.CaseInstanceId);

		// complete existing user task -> completes the process instance
		string taskId = taskService.createTaskQuery().caseInstanceId(caseInstanceId).singleResult().Id;
		taskService.complete(taskId);

		// the completed historic process instance is still associated with the
		// case instance id
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());

		historicProcessInstance = query.singleResult();
		assertNotNull(historicProcessInstance);
		assertNotNull(historicProcessInstance.EndTime);

		assertEquals(caseInstanceId, historicProcessInstance.CaseInstanceId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testQueryByCaseInstanceId.cmmn", "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testQueryByCaseInstanceIdHierarchy-super.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testQueryByCaseInstanceIdHierarchy-sub.bpmn20.xml" })]
	  public virtual void testQueryByCaseInstanceIdHierarchy()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		query.caseInstanceId(caseInstanceId);

		assertEquals(2, query.count());
		assertEquals(2, query.list().size());

		foreach (HistoricProcessInstance hpi in query.list())
		{
		  assertEquals(caseInstanceId, hpi.CaseInstanceId);
		}

		// complete existing user task -> completes the process instance(s)
		string taskId = taskService.createTaskQuery().caseInstanceId(caseInstanceId).singleResult().Id;
		taskService.complete(taskId);

		// the completed historic process instance is still associated with the
		// case instance id
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());

		foreach (HistoricProcessInstance hpi in query.list())
		{
		  assertEquals(caseInstanceId, hpi.CaseInstanceId);
		}

	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		query.caseInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testBusinessKey.cmmn", "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testBusinessKey.bpmn20.xml" })]
	  public virtual void testBusinessKey()
	  {
		// given
		string businessKey = "aBusinessKey";

		caseService.withCaseDefinitionByKey("case").businessKey(businessKey).create().Id;

		// then
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		query.processInstanceBusinessKey(businessKey);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());

		HistoricProcessInstance historicProcessInstance = query.singleResult();
		assertNotNull(historicProcessInstance);

		assertEquals(businessKey, historicProcessInstance.BusinessKey);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testStartActivityId-super.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testStartActivityId-sub.bpmn20.xml" })]
	  public virtual void testStartActivityId()
	  {
		// given

		// when
		runtimeService.startProcessInstanceByKey("super");

		// then
		HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("sub").singleResult();

		assertEquals("theSubStart", hpi.StartActivityId);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testStartActivityId-super.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testAsyncStartActivityId-sub.bpmn20.xml" })]
	  public virtual void testAsyncStartActivityId()
	  {
		// given
		runtimeService.startProcessInstanceByKey("super");

		// when
		executeAvailableJobs();

		// then
		HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("sub").singleResult();

		assertEquals("theSubStart", hpi.StartActivityId);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testStartByKeyWithCaseInstanceId()
	  {
		string caseInstanceId = "aCaseInstanceId";

		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess", null, caseInstanceId).Id;

		HistoricProcessInstance firstInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertNotNull(firstInstance);

		assertEquals(caseInstanceId, firstInstance.CaseInstanceId);

		// the second possibility to start a process instance /////////////////////////////////////////////

		processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess", null, caseInstanceId, null).Id;

		HistoricProcessInstance secondInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertNotNull(secondInstance);

		assertEquals(caseInstanceId, secondInstance.CaseInstanceId);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testStartByIdWithCaseInstanceId()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult().Id;

		string caseInstanceId = "aCaseInstanceId";
		string processInstanceId = runtimeService.startProcessInstanceById(processDefinitionId, null, caseInstanceId).Id;

		HistoricProcessInstance firstInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertNotNull(firstInstance);

		assertEquals(caseInstanceId, firstInstance.CaseInstanceId);

		// the second possibility to start a process instance /////////////////////////////////////////////

		processInstanceId = runtimeService.startProcessInstanceById(processDefinitionId, null, caseInstanceId, null).Id;

		HistoricProcessInstance secondInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertNotNull(secondInstance);

		assertEquals(caseInstanceId, secondInstance.CaseInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEndTimeAndEndActivity()
	  public virtual void testEndTimeAndEndActivity()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string taskId = taskService.createTaskQuery().taskDefinitionKey("userTask2").singleResult().Id;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when (1)
		taskService.complete(taskId);

		// then (1)
		HistoricProcessInstance historicProcessInstance = query.singleResult();

		assertNull(historicProcessInstance.EndActivityId);
		assertNull(historicProcessInstance.EndTime);

		// when (2)
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then (2)
		historicProcessInstance = query.singleResult();

		assertNull(historicProcessInstance.EndActivityId);
		assertNotNull(historicProcessInstance.EndTime);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryBySuperCaseInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneProcessTaskCase").Id;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().superCaseInstanceId(superCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		HistoricProcessInstance subProcessInstance = query.singleResult();
		assertNotNull(subProcessInstance);
		assertEquals(superCaseInstanceId, subProcessInstance.SuperCaseInstanceId);
		assertNull(subProcessInstance.SuperProcessInstanceId);
	  }

	  public virtual void testQueryByInvalidSuperCaseInstanceId()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		query.superCaseInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryBySubCaseInstanceId()
	  {
		string superProcessInstanceId = runtimeService.startProcessInstanceByKey("subProcessQueryTest").Id;

		string subCaseInstanceId = caseService.createCaseInstanceQuery().superProcessInstanceId(superProcessInstanceId).singleResult().Id;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().subCaseInstanceId(subCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		HistoricProcessInstance superProcessInstance = query.singleResult();
		assertNotNull(superProcessInstance);
		assertEquals(superProcessInstanceId, superProcessInstance.Id);
		assertNull(superProcessInstance.SuperCaseInstanceId);
		assertNull(superProcessInstance.SuperProcessInstanceId);
	  }

	  public virtual void testQueryByInvalidSubCaseInstanceId()
	  {
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		query.subCaseInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testSuperCaseInstanceIdProperty()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneProcessTaskCase").Id;

		caseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult().Id;

		HistoricProcessInstance instance = historyService.createHistoricProcessInstanceQuery().singleResult();

		assertNotNull(instance);
		assertEquals(superCaseInstanceId, instance.SuperCaseInstanceId);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		instance = historyService.createHistoricProcessInstanceQuery().singleResult();

		assertNotNull(instance);
		assertEquals(superCaseInstanceId, instance.SuperCaseInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessDefinitionKeyProperty()
	  {
		// given
		string key = "oneTaskProcess";
		string processInstanceId = runtimeService.startProcessInstanceByKey(key).Id;

		// when
		HistoricProcessInstance instance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		// then
		assertNotNull(instance.ProcessDefinitionKey);
		assertEquals(key, instance.ProcessDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessInstanceShouldBeActive()
	  public virtual void testProcessInstanceShouldBeActive()
	  {
		// given

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// then
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertNull(historicProcessInstance.EndTime);
		assertNull(historicProcessInstance.DurationInMillis);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testRetrieveProcessDefinitionName()
	  {

		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		// then
		assertEquals("The One Task Process", historicProcessInstance.ProcessDefinitionName);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testRetrieveProcessDefinitionVersion()
	  {

		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		// then
		assertEquals(1, historicProcessInstance.ProcessDefinitionVersion.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstExecutedActivityInInterval()
	  public virtual void testHistoricProcInstExecutedActivityInInterval()
	  {
		// given proc instance with wait state
		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().userTask().endEvent().done();
		deployment(model);

		DateTime hourFromNow = (DateTime) now.clone();
		hourFromNow.AddHours(1);

		runtimeService.startProcessInstanceByKey("proc");

		//when query historic process instance which has executed an activity after the start time
		// and before a hour after start time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityAfter(now).executedActivityBefore(hourFromNow).singleResult();


		//then query returns result
		assertNotNull(historicProcessInstance);


		// when proc inst is not in interval
		DateTime sixHoursFromNow = (DateTime) now.clone();
		sixHoursFromNow.AddHours(6);


		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityAfter(hourFromNow).executedActivityBefore(sixHoursFromNow).singleResult();

		//then query should return NO result
		assertNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstExecutedActivityAfter()
	  public virtual void testHistoricProcInstExecutedActivityAfter()
	  {
		// given
		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime hourFromNow = (DateTime) now.clone();
		hourFromNow.AddHours(1);

		runtimeService.startProcessInstanceByKey("proc");

		//when query historic process instance which has executed an activity after the start time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityAfter(now).singleResult();

		//then query returns result
		assertNotNull(historicProcessInstance);

		//when query historic proc inst with execute activity after a hour of the starting time
		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityAfter(hourFromNow).singleResult();

		//then query returns no result
		assertNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstExecutedActivityBefore()
	  public virtual void testHistoricProcInstExecutedActivityBefore()
	  {
		// given
		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime hourBeforeNow = (DateTime) now.clone();
		hourBeforeNow.add(DateTime.HOUR, -1);

		runtimeService.startProcessInstanceByKey("proc");

		//when query historic process instance which has executed an activity before the start time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityBefore(now).singleResult();

		//then query returns result, since the query is less-then-equal
		assertNotNull(historicProcessInstance);

		//when query historic proc inst which executes an activity an hour before the starting time
		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityBefore(hourBeforeNow).singleResult();

		//then query returns no result
		assertNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstExecutedActivityWithTwoProcInsts()
	  public virtual void testHistoricProcInstExecutedActivityWithTwoProcInsts()
	  {
		// given
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime now = new DateTime();
		DateTime hourBeforeNow = (DateTime) now.clone();
		hourBeforeNow.add(DateTime.HOUR, -1);

		ClockUtil.CurrentTime = hourBeforeNow;
		runtimeService.startProcessInstanceByKey("proc");

		ClockUtil.CurrentTime = now;
		runtimeService.startProcessInstanceByKey("proc");

		//when query execute activity between now and an hour ago
		IList<HistoricProcessInstance> list = historyService.createHistoricProcessInstanceQuery().executedActivityAfter(hourBeforeNow).executedActivityBefore(now).list();

		//then two historic process instance have to be returned
		assertEquals(2, list.Count);

		//when query execute activity after an half hour before now
		DateTime halfHour = (DateTime) now.clone();
		halfHour.AddMinutes(-30);
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityAfter(halfHour).singleResult();

		//then only the latest historic process instance is returned
		assertNotNull(historicProcessInstance);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstExecutedActivityWithEmptyInterval()
	  public virtual void testHistoricProcInstExecutedActivityWithEmptyInterval()
	  {
		// given
		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime hourBeforeNow = (DateTime) now.clone();
		hourBeforeNow.add(DateTime.HOUR, -1);

		runtimeService.startProcessInstanceByKey("proc");

		//when query historic proc inst which executes an activity an hour before and after the starting time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedActivityBefore(hourBeforeNow).executedActivityAfter(hourBeforeNow).singleResult();

		//then query returns no result
		assertNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcInstExecutedJobAfter()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcInstExecutedJobAfter()
	  {
		// given
		BpmnModelInstance asyncModel = Bpmn.createExecutableProcess("async").startEvent().camundaAsyncBefore().endEvent().done();
		deployment(asyncModel);
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		DateTime hourFromNow = (DateTime) now.clone();
		hourFromNow.AddHours(1);

		runtimeService.startProcessInstanceByKey("async");
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);
		runtimeService.startProcessInstanceByKey("proc");

		//when query historic process instance which has executed an job after the start time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedJobAfter(now).singleResult();

		//then query returns only a single process instance
		assertNotNull(historicProcessInstance);

		//when query historic proc inst with execute job after a hour of the starting time
		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedJobAfter(hourFromNow).singleResult();

		//then query returns no result
		assertNull(historicProcessInstance);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcInstExecutedJobBefore()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcInstExecutedJobBefore()
	  {
		// given
		BpmnModelInstance asyncModel = Bpmn.createExecutableProcess("async").startEvent().camundaAsyncBefore().endEvent().done();
		deployment(asyncModel);
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		DateTime hourBeforeNow = (DateTime) now.clone();
		hourBeforeNow.AddHours(-1);

		runtimeService.startProcessInstanceByKey("async");
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);
		runtimeService.startProcessInstanceByKey("proc");

		//when query historic process instance which has executed an job before the start time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedJobBefore(now).singleResult();

		//then query returns only a single process instance since before is less-then-equal
		assertNotNull(historicProcessInstance);

		//when query historic proc inst with executed job before an hour of the starting time
		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedJobBefore(hourBeforeNow).singleResult();

		//then query returns no result
		assertNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcInstExecutedJobWithTwoProcInsts()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcInstExecutedJobWithTwoProcInsts()
	  {
		// given
		BpmnModelInstance asyncModel = Bpmn.createExecutableProcess("async").startEvent().camundaAsyncBefore().endEvent().done();
		deployment(asyncModel);

		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		DateTime hourBeforeNow = (DateTime) now.clone();
		hourBeforeNow.AddHours(-1);

		ClockUtil.CurrentTime = hourBeforeNow;
		runtimeService.startProcessInstanceByKey("async");
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		ClockUtil.CurrentTime = now;
		runtimeService.startProcessInstanceByKey("async");
		runtimeService.startProcessInstanceByKey("proc");

		//when query executed job between now and an hour ago
		IList<HistoricProcessInstance> list = historyService.createHistoricProcessInstanceQuery().executedJobAfter(hourBeforeNow).executedJobBefore(now).list();

		//then the two async historic process instance have to be returned
		assertEquals(2, list.Count);

		//when query execute activity after an half hour before now
		DateTime halfHour = (DateTime) now.clone();
		halfHour.AddMinutes(-30);
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedJobAfter(halfHour).singleResult();

		//then only the latest async historic process instance is returned
		assertNotNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcInstExecutedJobWithEmptyInterval()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcInstExecutedJobWithEmptyInterval()
	  {
		// given
		BpmnModelInstance asyncModel = Bpmn.createExecutableProcess("async").startEvent().camundaAsyncBefore().endEvent().done();
		deployment(asyncModel);
		BpmnModelInstance model = Bpmn.createExecutableProcess("proc").startEvent().endEvent().done();
		deployment(model);

		DateTime now = new DateTime();
		ClockUtil.CurrentTime = now;
		DateTime hourBeforeNow = (DateTime) now.clone();
		hourBeforeNow.AddHours(-1);

		runtimeService.startProcessInstanceByKey("async");
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);
		runtimeService.startProcessInstanceByKey("proc");

		//when query historic proc inst with executed job before and after an hour before the starting time
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().executedJobBefore(hourBeforeNow).executedJobAfter(hourBeforeNow).singleResult();

		//then query returns no result
		assertNull(historicProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcInstQueryWithExecutedActivityIds()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcInstQueryWithExecutedActivityIds()
	  {
		// given
		deployment(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		Task task = taskService.createTaskQuery().active().singleResult();
		taskService.complete(task.Id);

		// assume
		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).activityId("userTask1").singleResult();
		assertNotNull(historicActivityInstance);

		// when
		IList<HistoricProcessInstance> result = historyService.createHistoricProcessInstanceQuery().executedActivityIdIn(historicActivityInstance.ActivityId).list();

		// then
		assertNotNull(result);
		assertEquals(1, result.Count);
		assertEquals(result[0].Id, processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstQueryWithExecutedActivityIdsNull()
	  public virtual void testHistoricProcInstQueryWithExecutedActivityIdsNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().executedActivityIdIn(null).list();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("activity ids is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstQueryWithExecutedActivityIdsContainNull()
	  public virtual void testHistoricProcInstQueryWithExecutedActivityIdsContainNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().executedActivityIdIn(null, "1").list();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("activity ids contains null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricProcInstQueryWithActiveActivityIds()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricProcInstQueryWithActiveActivityIds()
	  {
		// given
		deployment(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		// assume
		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityId("userTask1").singleResult();
		assertNotNull(historicActivityInstance);

		// when
		IList<HistoricProcessInstance> result = historyService.createHistoricProcessInstanceQuery().activeActivityIdIn(historicActivityInstance.ActivityId).list();

		// then
		assertNotNull(result);
		assertEquals(1, result.Count);
		assertEquals(result[0].Id, processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstQueryWithActiveActivityIdsNull()
	  public virtual void testHistoricProcInstQueryWithActiveActivityIdsNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().activeActivityIdIn(null).list();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("activity ids is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricProcInstQueryWithActiveActivityIdsContainNull()
	  public virtual void testHistoricProcInstQueryWithActiveActivityIdsContainNull()
	  {
		try
		{
		  historyService.createHistoricProcessInstanceQuery().activeActivityIdIn(null, "1").list();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("activity ids contains null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActiveActivityIdInAndProcessDefinitionKey()
	  public virtual void testQueryByActiveActivityIdInAndProcessDefinitionKey()
	  {
		// given
		deployment(ProcessModels.ONE_TASK_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		// when
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("Process").activeActivityIdIn("userTask").singleResult();

		// then
		assertNotNull(historicProcessInstance);
		assertEquals(processInstance.Id, historicProcessInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutedActivityIdInAndProcessDefinitionKey()
	  public virtual void testQueryByExecutedActivityIdInAndProcessDefinitionKey()
	  {
		// given
		deployment(ProcessModels.ONE_TASK_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("Process").executedActivityIdIn("userTask").singleResult();

		// then
		assertNotNull(historicProcessInstance);
		assertEquals(processInstance.Id, historicProcessInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testQueryWithRootIncidents()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testQueryWithRootIncidents()
	  {
		// given
		deployment("org/camunda/bpm/engine/test/history/HistoricProcessInstanceTest.testQueryWithRootIncidents.bpmn20.xml");
		deployment(CallActivityModels.oneBpmnCallActivityProcess("Process_1"));

		runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance calledProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("Process_1").singleResult();
		executeAvailableJobs();

		// when
		IList<HistoricProcessInstance> historicProcInstances = historyService.createHistoricProcessInstanceQuery().withRootIncidents().list();

		// then
		assertNotNull(calledProcessInstance);
		assertEquals(1, historicProcInstances.Count);
		assertEquals(calledProcessInstance.Id, historicProcInstances[0].Id);
	  }
	}

}