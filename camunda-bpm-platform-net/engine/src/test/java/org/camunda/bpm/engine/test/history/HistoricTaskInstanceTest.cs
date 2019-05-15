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
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoricTaskInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Frederik Heremans
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricTaskInstanceTest : PluggableProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricTaskInstance() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testHistoricTaskInstance()
		{
		string processInstanceId = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest").Id;

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss");

		// Set priority to non-default value
		Task runtimeTask = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult();
		runtimeTask.Priority = 1234;

		// Set due-date
		DateTime dueDate = sdf.parse("01/02/2003 04:05:06");
		runtimeTask.DueDate = dueDate;
		taskService.saveTask(runtimeTask);

		string taskId = runtimeTask.Id;
		string taskDefinitionKey = runtimeTask.TaskDefinitionKey;

		HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals(taskId, historicTaskInstance.Id);
		assertEquals(1234, historicTaskInstance.Priority);
		assertEquals("Clean up", historicTaskInstance.Name);
		assertEquals("Schedule an engineering meeting for next week with the new hire.", historicTaskInstance.Description);
		assertEquals(dueDate, historicTaskInstance.DueDate);
		assertEquals("kermit", historicTaskInstance.Assignee);
		assertEquals(taskDefinitionKey, historicTaskInstance.TaskDefinitionKey);
		assertNull(historicTaskInstance.EndTime);
		assertNull(historicTaskInstance.DurationInMillis);

		assertNull(historicTaskInstance.CaseDefinitionId);
		assertNull(historicTaskInstance.CaseInstanceId);
		assertNull(historicTaskInstance.CaseExecutionId);

		// the activity instance id is set
		assertEquals(((TaskEntity)runtimeTask).getExecution().ActivityInstanceId, historicTaskInstance.ActivityInstanceId);

		runtimeService.setVariable(processInstanceId, "deadline", "yesterday");

		// move clock by 1 second
		DateTime now = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = new DateTime(now.Ticks + 1000);

		taskService.complete(taskId);

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().count());

		historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals(taskId, historicTaskInstance.Id);
		assertEquals(1234, historicTaskInstance.Priority);
		assertEquals("Clean up", historicTaskInstance.Name);
		assertEquals("Schedule an engineering meeting for next week with the new hire.", historicTaskInstance.Description);
		assertEquals(dueDate, historicTaskInstance.DueDate);
		assertEquals("kermit", historicTaskInstance.Assignee);
		assertEquals(TaskEntity.DELETE_REASON_COMPLETED, historicTaskInstance.DeleteReason);
		assertEquals(taskDefinitionKey, historicTaskInstance.TaskDefinitionKey);
		assertNotNull(historicTaskInstance.EndTime);
		assertNotNull(historicTaskInstance.DurationInMillis);
		assertTrue(historicTaskInstance.DurationInMillis >= 1000);
		assertTrue(((HistoricTaskInstanceEntity)historicTaskInstance).DurationRaw >= 1000);

		assertNull(historicTaskInstance.CaseDefinitionId);
		assertNull(historicTaskInstance.CaseInstanceId);
		assertNull(historicTaskInstance.CaseExecutionId);

		historyService.deleteHistoricTaskInstance(taskId);

		assertEquals(0, historyService.createHistoricTaskInstanceQuery().count());
		}

	  public virtual void testDeleteHistoricTaskInstance()
	  {
		// deleting unexisting historic task instance should be silently ignored
		historyService.deleteHistoricTaskInstance("unexistingId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricTaskInstanceQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricTaskInstanceQuery()
	  {
		// First instance is finished
		ProcessInstance finishedInstance = runtimeService.startProcessInstanceByKey("HistoricTaskQueryTest");

		// Set priority to non-default value
		Task task = taskService.createTaskQuery().processInstanceId(finishedInstance.Id).singleResult();
		task.Priority = 1234;
		DateTime dueDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/02/2003 04:05:06");
		task.DueDate = dueDate;

		taskService.saveTask(task);

		// Complete the task
		string taskId = task.Id;
		taskService.complete(taskId);

		// Task id
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskId(taskId).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskId("unexistingtaskid").count());

		// Name
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskName("Clean_up").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskName("unexistingname").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskNameLike("Clean\\_u%").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskNameLike("%lean\\_up").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskNameLike("%lean\\_u%").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskNameLike("%unexistingname%").count());


		// Description
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDescription("Historic task_description").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDescription("unexistingdescription").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDescriptionLike("%task\\_description").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDescriptionLike("Historic task%").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDescriptionLike("%task%").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDescriptionLike("%unexistingdescripton%").count());

		// Execution id
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().executionId(finishedInstance.Id).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().executionId("unexistingexecution").count());

		// Process instance id
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processInstanceId(finishedInstance.Id).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processInstanceId("unexistingid").count());

		// Process definition id
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processDefinitionId(finishedInstance.ProcessDefinitionId).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processDefinitionId("unexistingdefinitionid").count());

		// Process definition name
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processDefinitionName("Historic task query test process").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processDefinitionName("unexistingdefinitionname").count());

		// Process definition key
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processDefinitionKey("HistoricTaskQueryTest").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processDefinitionKey("unexistingdefinitionkey").count());


		// Assignee
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskAssignee("ker_mit").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskAssignee("johndoe").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskAssigneeLike("%er\\_mit").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskAssigneeLike("ker\\_mi%").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskAssigneeLike("%er\\_mi%").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskAssigneeLike("%johndoe%").count());

		// Delete reason
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDeleteReason(TaskEntity.DELETE_REASON_COMPLETED).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDeleteReason("deleted").count());

		// Task definition ID
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDefinitionKey("task").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDefinitionKey("unexistingkey").count());

		// Task priority
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskPriority(1234).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskPriority(5678).count());


		// Due date
		DateTime anHourAgo = new DateTime();
		anHourAgo = new DateTime(dueDate);
		anHourAgo.add(DateTime.HOUR, -1);

		DateTime anHourLater = new DateTime();
		anHourLater = new DateTime(dueDate);
		anHourLater.add(DateTime.HOUR, 1);

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDueDate(dueDate).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueDate(anHourAgo).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueDate(anHourLater).count());

		// Due date before
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDueBefore(anHourLater).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueBefore(anHourAgo).count());

		// Due date after
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDueAfter(anHourAgo).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueAfter(anHourLater).count());

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDueDate(dueDate).taskDueBefore(anHourLater).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueDate(dueDate).taskDueBefore(anHourAgo).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskDueDate(dueDate).taskDueAfter(anHourAgo).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueDate(dueDate).taskDueAfter(anHourLater).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskDueBefore(anHourAgo).taskDueAfter(anHourLater).count());

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		DateTime hourFromNow = new DateTime();
		hourFromNow.AddHours(1);

		// Start/end dates
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().finishedBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().finishedBefore(hourFromNow).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().finishedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().finishedAfter(hourFromNow).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().startedBefore(hourFromNow).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().startedBefore(hourAgo).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().startedAfter(hourAgo).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().startedAfter(hourFromNow).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().startedAfter(hourFromNow).startedBefore(hourAgo).count());

		// Finished and Unfinished - Add anther other instance that has a running task (unfinished)
		runtimeService.startProcessInstanceByKey("HistoricTaskQueryTest");

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().finished().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().unfinished().count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().unfinished().finished().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricTaskInstanceQueryByProcessVariableValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoricTaskInstanceQueryByProcessVariableValue()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  IDictionary<string, object> variables = new Dictionary<string, object>();
		  variables["hallo"] = "steffen";

		  string processInstanceId = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest", variables).Id;

		  Task runtimeTask = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult();
		  string taskId = runtimeTask.Id;

		  HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("hallo", "steffen").singleResult();

		  assertNotNull(historicTaskInstance);
		  assertEquals(taskId, historicTaskInstance.Id);

		  taskService.complete(taskId);
		  assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskId(taskId).count());

		  historyService.deleteHistoricTaskInstance(taskId);
		  assertEquals(0, historyService.createHistoricTaskInstanceQuery().count());
		}
	  }

	  public virtual void testHistoricTaskInstanceAssignment()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);

		// task exists & has no assignee:
		HistoricTaskInstance hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertNull(hti.Assignee);

		// assign task to jonny:
		taskService.setAssignee(task.Id, "jonny");

		// should be reflected in history
		hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals("jonny", hti.Assignee);
		assertNull(hti.Owner);

		taskService.deleteTask(task.Id);
		historyService.deleteHistoricTaskInstance(hti.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricTaskInstanceAssignmentListener()
	  public virtual void testHistoricTaskInstanceAssignmentListener()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["assignee"] = "jonny";
		runtimeService.startProcessInstanceByKey("testProcess", variables);

		HistoricActivityInstance hai = historyService.createHistoricActivityInstanceQuery().activityId("task").singleResult();
		assertEquals("jonny", hai.Assignee);

		HistoricTaskInstance hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals("jonny", hti.Assignee);
		assertNull(hti.Owner);

	  }

	  public virtual void testHistoricTaskInstanceOwner()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);

		// task exists & has no owner:
		HistoricTaskInstance hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertNull(hti.Owner);

		// set owner to jonny:
		taskService.setOwner(task.Id, "jonny");

		// should be reflected in history
		hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals("jonny", hti.Owner);

		taskService.deleteTask(task.Id);
		historyService.deleteHistoricTaskInstance(hti.Id);
	  }

	  public virtual void testHistoricTaskInstancePriority()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);

		// task exists & has normal priority:
		HistoricTaskInstance hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals(org.camunda.bpm.engine.task.Task_Fields.PRIORITY_NORMAL, hti.Priority);

		// set priority to maximum value:
		taskService.setPriority(task.Id, org.camunda.bpm.engine.task.Task_Fields.PRIORITY_MAXIMUM);

		// should be reflected in history
		hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		assertEquals(org.camunda.bpm.engine.task.Task_Fields.PRIORITY_MAXIMUM, hti.Priority);

		taskService.deleteTask(task.Id);
		historyService.deleteHistoricTaskInstance(hti.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricTaskInstanceQueryProcessFinished()
	  public virtual void testHistoricTaskInstanceQueryProcessFinished()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("TwoTaskHistoricTaskQueryTest");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Running task on running process should be available
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processUnfinished().count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processFinished().count());

		// Finished and running task on running process should be available
		taskService.complete(task.Id);
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().processUnfinished().count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processFinished().count());

		// 2 finished tasks are found for finished process after completing last task of process
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.complete(task.Id);
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processUnfinished().count());
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().processFinished().count());

		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processUnfinished().processFinished().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricTaskInstanceQuerySorting()
	  public virtual void testHistoricTaskInstanceQuerySorting()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("HistoricTaskQueryTest");

		string taskId = taskService.createTaskQuery().processInstanceId(instance.Id).singleResult().Id;
		taskService.complete(taskId);

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByDeleteReason().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByExecutionId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByHistoricActivityInstanceId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByHistoricActivityInstanceStartTime().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByProcessDefinitionId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByProcessInstanceId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskDescription().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskName().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskDefinitionKey().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskPriority().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskAssignee().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskDueDate().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskFollowUpDate().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByCaseDefinitionId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByCaseInstanceId().asc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByCaseExecutionId().asc().count());

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByDeleteReason().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByExecutionId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByHistoricActivityInstanceId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByHistoricActivityInstanceStartTime().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByProcessDefinitionId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByProcessInstanceId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskDescription().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskName().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskDefinitionKey().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskPriority().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskAssignee().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskDueDate().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByTaskFollowUpDate().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByCaseDefinitionId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByCaseInstanceId().desc().count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().orderByCaseExecutionId().desc().count());
	  }

	  public virtual void testInvalidSorting()
	  {
		try
		{
		  historyService.createHistoricTaskInstanceQuery().asc();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricTaskInstanceQuery().desc();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}

		try
		{
		  historyService.createHistoricTaskInstanceQuery().orderByProcessInstanceId().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"}) public void testHistoricTaskInstanceQueryByFollowUpDate() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"})]
	  public virtual void testHistoricTaskInstanceQueryByFollowUpDate()
	  {
		DateTime otherDate = new DateTime();

		runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest");

		// do not find any task instances with follow up date
		assertEquals(0, taskService.createTaskQuery().followUpDate(otherDate).count());

		Task task = taskService.createTaskQuery().singleResult();

		// set follow-up date on task
		DateTime followUpDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/02/2003 01:12:13");
		task.FollowUpDate = followUpDate;
		taskService.saveTask(task);

		// test that follow-up date was written to historic database
		assertEquals(followUpDate, historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult().FollowUpDate);
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskFollowUpDate(followUpDate).count());

		otherDate = new DateTime(followUpDate);

		otherDate.AddYears(1);
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskFollowUpDate(otherDate).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskFollowUpBefore(otherDate).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskFollowUpAfter(otherDate).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskFollowUpAfter(otherDate).taskFollowUpDate(followUpDate).count());

		otherDate.AddYears(-2);
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskFollowUpAfter(otherDate).count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskFollowUpBefore(otherDate).count());
		assertEquals(followUpDate, historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult().FollowUpDate);
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskFollowUpBefore(otherDate).taskFollowUpDate(followUpDate).count());

		taskService.complete(task.Id);

		assertEquals(followUpDate, historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult().FollowUpDate);
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskFollowUpDate(followUpDate).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"}) public void testHistoricTaskInstanceQueryByActivityInstanceId() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"})]
	  public virtual void testHistoricTaskInstanceQueryByActivityInstanceId()
	  {
		runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest");

		string activityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId("task").singleResult().Id;

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().activityInstanceIdIn(activityInstanceId);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"}) public void testHistoricTaskInstanceQueryByActivityInstanceIds() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"})]
	  public virtual void testHistoricTaskInstanceQueryByActivityInstanceIds()
	  {
		ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest");
		ProcessInstance pi2 = runtimeService.startProcessInstanceByKey("HistoricTaskInstanceTest");

		string activityInstanceId1 = historyService.createHistoricActivityInstanceQuery().processInstanceId(pi1.Id).activityId("task").singleResult().Id;

		string activityInstanceId2 = historyService.createHistoricActivityInstanceQuery().processInstanceId(pi2.Id).activityId("task").singleResult().Id;

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().activityInstanceIdIn(activityInstanceId1, activityInstanceId2);

		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testHistoricTaskInstance.bpmn20.xml"})]
	  public virtual void testHistoricTaskInstanceQueryByInvalidActivityInstanceId()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.activityInstanceIdIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  query.activityInstanceIdIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  query.activityInstanceIdIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  string[] values = new string[] {"a", null, "b"};
		  query.activityInstanceIdIn(values);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseDefinitionId()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseDefinitionId(caseDefinitionId);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		HistoricTaskInstance task = query.singleResult();
		assertNotNull(task);

		assertEquals(caseDefinitionId, task.CaseDefinitionId);
		assertEquals(caseInstanceId, task.CaseInstanceId);
		assertEquals(humanTaskId, task.CaseExecutionId);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionId()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseDefinitionId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

		query.caseDefinitionId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseDefinitionKey()
	  {
		// given
		string key = "oneTaskCase";

		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(key).singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinitionByKey(key).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseDefinitionKey(key);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		HistoricTaskInstance task = query.singleResult();
		assertNotNull(task);

		assertEquals(caseDefinitionId, task.CaseDefinitionId);
		assertEquals(caseInstanceId, task.CaseInstanceId);
		assertEquals(humanTaskId, task.CaseExecutionId);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionKey()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseDefinitionKey("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

		query.caseDefinitionKey(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseDefinitionName()
	  {
		// given
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		string caseDefinitionName = caseDefinition.Name;
		string caseDefinitionId = caseDefinition.Id;

		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseDefinitionName(caseDefinitionName);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		HistoricTaskInstance task = query.singleResult();
		assertNotNull(task);

		assertEquals(caseDefinitionId, task.CaseDefinitionId);
		assertEquals(caseInstanceId, task.CaseInstanceId);
		assertEquals(humanTaskId, task.CaseExecutionId);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionName()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseDefinitionName("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

		query.caseDefinitionName(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceId()
	  {
		// given
		string key = "oneTaskCase";

		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(key).singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinitionByKey(key).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseInstanceId(caseInstanceId);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		HistoricTaskInstance task = query.singleResult();
		assertNotNull(task);

		assertEquals(caseDefinitionId, task.CaseDefinitionId);
		assertEquals(caseInstanceId, task.CaseInstanceId);
		assertEquals(humanTaskId, task.CaseExecutionId);
	  }



	  [Deployment(resources: { "org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testQueryByCaseInstanceIdHierarchy.cmmn", "org/camunda/bpm/engine/test/history/HistoricTaskInstanceTest.testQueryByCaseInstanceIdHierarchy.bpmn20.xml" })]
	  public virtual void testQueryByCaseInstanceIdHierarchy()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult().Id;

		// then
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseInstanceId(caseInstanceId);

		assertEquals(2, query.count());
		assertEquals(2, query.list().size());

		foreach (HistoricTaskInstance task in query.list())
		{
		  assertEquals(caseInstanceId, task.CaseInstanceId);

		  assertNull(task.CaseDefinitionId);
		  assertNull(task.CaseExecutionId);

		  taskService.complete(task.Id);
		}

		assertEquals(3, query.count());
		assertEquals(3, query.list().size());

		foreach (HistoricTaskInstance task in query.list())
		{
		  assertEquals(caseInstanceId, task.CaseInstanceId);

		  assertNull(task.CaseDefinitionId);
		  assertNull(task.CaseExecutionId);
		}

	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseExecutionId()
	  {
		// given
		string key = "oneTaskCase";

		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(key).singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinitionByKey(key).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseExecutionId(humanTaskId);

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		HistoricTaskInstance task = query.singleResult();
		assertNotNull(task);

		assertEquals(caseDefinitionId, task.CaseDefinitionId);
		assertEquals(caseInstanceId, task.CaseInstanceId);
		assertEquals(humanTaskId, task.CaseExecutionId);
	  }

	  public virtual void testQueryByInvalidCaseExecutionId()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.caseExecutionId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

		query.caseExecutionId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());

	  }

	  public virtual void testHistoricTaskInstanceCaseInstanceId()
	  {
		Task task = taskService.newTask();
		task.CaseInstanceId = "aCaseInstanceId";
		taskService.saveTask(task);

		HistoricTaskInstance hti = historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult();

		assertEquals("aCaseInstanceId", hti.CaseInstanceId);

		task.CaseInstanceId = "anotherCaseInstanceId";
		taskService.saveTask(task);

		hti = historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult();

		assertEquals("anotherCaseInstanceId", hti.CaseInstanceId);

		// Finally, delete task
		taskService.deleteTask(task.Id, true);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessDefinitionKeyProperty()
	  {
		// given
		string key = "oneTaskProcess";
		string processInstanceId = runtimeService.startProcessInstanceByKey(key).Id;

		// when
		HistoricTaskInstance task = historyService.createHistoricTaskInstanceQuery().processInstanceId(processInstanceId).taskDefinitionKey("theTask").singleResult();

		// then
		assertNotNull(task.ProcessDefinitionKey);
		assertEquals(key, task.ProcessDefinitionKey);

		assertNull(task.CaseDefinitionKey);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testCaseDefinitionKeyProperty()
	  {
		// given
		string key = "oneTaskCase";
		string caseInstanceId = caseService.createCaseInstanceByKey(key).Id;

		// when
		HistoricTaskInstance task = historyService.createHistoricTaskInstanceQuery().caseInstanceId(caseInstanceId).taskDefinitionKey("PI_HumanTask_1").singleResult();

		// then
		assertNotNull(task.CaseDefinitionKey);
		assertEquals(key, task.CaseDefinitionKey);

		assertNull(task.ProcessDefinitionKey);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testQueryByTaskDefinitionKey()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		HistoricTaskInstanceQuery query1 = historyService.createHistoricTaskInstanceQuery().taskDefinitionKey("theTask");

		HistoricTaskInstanceQuery query2 = historyService.createHistoricTaskInstanceQuery().taskDefinitionKeyIn("theTask");

		// then
		assertEquals(1, query1.count());
		assertEquals(1, query2.count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryByTaskDefinitionKeys()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		caseService.createCaseInstanceByKey("oneTaskCase");

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().taskDefinitionKeyIn("theTask", "PI_HumanTask_1");

		// then
		assertEquals(2, query.count());
	  }

	  public virtual void testQueryByInvalidTaskDefinitionKeys()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.taskDefinitionKeyIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  query.taskDefinitionKeyIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (NotValidException)
		{
		}

		try
		{
		  query.taskDefinitionKeyIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (NotValidException)
		{
		}

		try
		{
		  string[] values = new string[] {"a", null, "b"};
		  query.taskDefinitionKeyIn(values);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (NotValidException)
		{
		}

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryByProcessInstanceBusinessKey()
	  {
		// given
		ProcessInstance piBusinessKey1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		assertEquals(1, query.processInstanceBusinessKey(piBusinessKey1.BusinessKey).count());
		assertEquals(0, query.processInstanceBusinessKey("unexistingBusinessKey").count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryByProcessInstanceBusinessKeyIn()
	  {
		// given
		string businessKey1 = "BUSINESS-KEY-1";
		string businessKey2 = "BUSINESS-KEY-2";
		string businessKey3 = "BUSINESS-KEY-3";
		string unexistingBusinessKey = "unexistingBusinessKey";

		runtimeService.startProcessInstanceByKey("oneTaskProcess", businessKey1);
		runtimeService.startProcessInstanceByKey("oneTaskProcess", businessKey2);
		runtimeService.startProcessInstanceByKey("oneTaskProcess", businessKey3);

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		assertEquals(3, query.processInstanceBusinessKeyIn(businessKey1, businessKey2, businessKey3).list().size());
		assertEquals(1, query.processInstanceBusinessKeyIn(businessKey2, unexistingBusinessKey).count());
	  }

	  public virtual void testQueryByInvalidProcessInstanceBusinessKeyIn()
	  {
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		query.processInstanceBusinessKeyIn("invalid");
		assertEquals(0, query.count());

		try
		{
		  query.processInstanceBusinessKeyIn(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  query.processInstanceBusinessKeyIn((string)null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  string[] values = new string[] {"a", null, "b"};
		  query.processInstanceBusinessKeyIn(values);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryByProcessInstanceBusinessKeyLike()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		assertEquals(1, query.processInstanceBusinessKeyLike("BUSINESS-KEY-1").list().size());
		assertEquals(1, query.processInstanceBusinessKeyLike("BUSINESS-KEY%").count());
		assertEquals(1, query.processInstanceBusinessKeyLike("%KEY-1").count());
		assertEquals(1, query.processInstanceBusinessKeyLike("%KEY%").count());
		assertEquals(0, query.processInstanceBusinessKeyLike("BUZINESS-KEY%").count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryByProcessInstanceBusinessKeyAndArray()
	  {
		// given
		string businessKey1 = "BUSINESS-KEY-1";
		string businessKey2 = "BUSINESS-KEY-2";
		string businessKey3 = "BUSINESS-KEY-3";
		string unexistingBusinessKey = "unexistingBusinessKey";

		runtimeService.startProcessInstanceByKey("oneTaskProcess", businessKey1);
		runtimeService.startProcessInstanceByKey("oneTaskProcess", businessKey2);
		runtimeService.startProcessInstanceByKey("oneTaskProcess", businessKey3);

		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		assertEquals(0, query.processInstanceBusinessKeyIn(businessKey1, businessKey2).processInstanceBusinessKey(unexistingBusinessKey).count());
		assertEquals(1, query.processInstanceBusinessKeyIn(businessKey2, businessKey3).processInstanceBusinessKey(businessKey2).count());
	  }
	}

}