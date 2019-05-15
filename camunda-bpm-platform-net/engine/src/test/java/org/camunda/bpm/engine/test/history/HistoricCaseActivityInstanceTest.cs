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
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.DISABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasProperty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.collection.IsIterableContainingInOrder.contains;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Direction = org.camunda.bpm.engine.impl.Direction;
	using QueryOrderingProperty = org.camunda.bpm.engine.impl.QueryOrderingProperty;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using HistoricCaseActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseActivityInstanceEventEntity;
	using HistoricCaseActivityInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseActivityInstanceEntity;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Query = org.camunda.bpm.engine.query.Query;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Matcher = org.hamcrest.Matcher;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricCaseActivityInstanceTest : CmmnProcessEngineTestCase
	{
		[Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageWithManualActivationCase.cmmn"})]
		public virtual void testHistoricCaseActivityInstanceProperties()
		{
		string activityId = "PI_Stage_1";

		createCaseInstance();
		CaseExecution stage = queryCaseExecutionByActivityId(activityId);
		HistoricCaseActivityInstance historicStage = queryHistoricActivityCaseInstance(activityId);

		assertEquals(stage.Id, historicStage.Id);
		assertEquals(stage.ParentId, historicStage.ParentCaseActivityInstanceId);
		assertEquals(stage.CaseDefinitionId, historicStage.CaseDefinitionId);
		assertEquals(stage.CaseInstanceId, historicStage.CaseInstanceId);
		assertEquals(stage.ActivityId, historicStage.CaseActivityId);
		assertEquals(stage.ActivityName, historicStage.CaseActivityName);
		assertEquals(stage.ActivityType, historicStage.CaseActivityType);

		manualStart(stage.Id);

		historicStage = queryHistoricActivityCaseInstance(activityId);
		assertNotNull(historicStage.EndTime);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricCaseActivityTaskStates()
	  public virtual void testHistoricCaseActivityTaskStates()
	  {
		string humanTaskId1 = "PI_HumanTask_1";
		string humanTaskId2 = "PI_HumanTask_2";
		string humanTaskId3 = "PI_HumanTask_3";

		// given
		string caseInstanceId = createCaseInstance().Id;
		string taskInstanceId1 = queryCaseExecutionByActivityId(humanTaskId1).Id;
		string taskInstanceId2 = queryCaseExecutionByActivityId(humanTaskId2).Id;
		string taskInstanceId3 = queryCaseExecutionByActivityId(humanTaskId3).Id;

		// human task 1 should enabled and human task 2 and 3 will be available cause the sentry is not fulfilled
		assertHistoricState(humanTaskId1, ENABLED);
		assertHistoricState(humanTaskId2, AVAILABLE);
		assertHistoricState(humanTaskId3, AVAILABLE);
		assertStateQuery(ENABLED, AVAILABLE, AVAILABLE);

		// when human task 1 is started
		manualStart(taskInstanceId1);

		// then human task 1 is active and human task 2 and 3 are still available
		assertHistoricState(humanTaskId1, ACTIVE);
		assertHistoricState(humanTaskId2, AVAILABLE);
		assertHistoricState(humanTaskId3, AVAILABLE);
		assertStateQuery(ACTIVE, AVAILABLE, AVAILABLE);

		// when human task 1 is completed
		complete(taskInstanceId1);

		// then human task 1 is completed and human task 2 is enabled and human task 3 is active
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertHistoricState(humanTaskId3, ACTIVE);
		assertStateQuery(COMPLETED, ENABLED, ACTIVE);

		// disable human task 2
		disable(taskInstanceId2);
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, DISABLED);
		assertHistoricState(humanTaskId3, ACTIVE);
		assertStateQuery(COMPLETED, DISABLED, ACTIVE);

		// re-enable human task 2
		reenable(taskInstanceId2);
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertHistoricState(humanTaskId3, ACTIVE);
		assertStateQuery(COMPLETED, ENABLED, ACTIVE);

		// suspend human task 3
		suspend(taskInstanceId3);
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertHistoricState(humanTaskId3, SUSPENDED);
		assertStateQuery(COMPLETED, ENABLED, SUSPENDED);

		// resume human task 3
		resume(taskInstanceId3);
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertHistoricState(humanTaskId3, ACTIVE);
		assertStateQuery(COMPLETED, ENABLED, ACTIVE);

		// when the case instance is suspended
		suspend(caseInstanceId);

		// then human task 2 and 3 are suspended
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, SUSPENDED);
		assertHistoricState(humanTaskId3, SUSPENDED);
		assertStateQuery(COMPLETED, SUSPENDED, SUSPENDED);

		// when case instance is re-activated
		reactivate(caseInstanceId);

		// then human task 2 is enabled and human task is active
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertHistoricState(humanTaskId3, ACTIVE);
		assertStateQuery(COMPLETED, ENABLED, ACTIVE);

		manualStart(taskInstanceId2);
		// when human task 3 is terminated
		terminate(taskInstanceId3);

		// then human task 2 and 3 are terminated caused by the exitCriteria of human task 2
		assertHistoricState(humanTaskId1, COMPLETED);
		assertHistoricState(humanTaskId2, TERMINATED);
		assertHistoricState(humanTaskId3, TERMINATED);
		assertStateQuery(COMPLETED, TERMINATED, TERMINATED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricCaseActivityMilestoneStates()
	  public virtual void testHistoricCaseActivityMilestoneStates()
	  {
		string milestoneId1 = "PI_Milestone_1";
		string milestoneId2 = "PI_Milestone_2";
		string humanTaskId1 = "PI_HumanTask_1";
		string humanTaskId2 = "PI_HumanTask_2";

		// given
		string caseInstanceId = createCaseInstance().Id;
		string milestoneInstance1 = queryCaseExecutionByActivityId(milestoneId1).Id;
		string milestoneInstance2 = queryCaseExecutionByActivityId(milestoneId2).Id;
		string humanTaskInstance1 = queryCaseExecutionByActivityId(humanTaskId1).Id;

		// then milestone 1 and 2 are available and
		// humanTask 1 and 2 are enabled
		assertHistoricState(milestoneId1, AVAILABLE);
		assertHistoricState(milestoneId2, AVAILABLE);
		assertHistoricState(humanTaskId1, ENABLED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertStateQuery(AVAILABLE, AVAILABLE, ENABLED, ENABLED);

		// suspend event milestone 1 and 2
		suspend(milestoneInstance1);
		suspend(milestoneInstance2);
		assertHistoricState(milestoneId1, SUSPENDED);
		assertHistoricState(milestoneId2, SUSPENDED);
		assertHistoricState(humanTaskId1, ENABLED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertStateQuery(SUSPENDED, SUSPENDED, ENABLED, ENABLED);

		// resume user milestone 1
		resume(milestoneInstance1);
		assertHistoricState(milestoneId1, AVAILABLE);
		assertHistoricState(milestoneId2, SUSPENDED);
		assertHistoricState(humanTaskId1, ENABLED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertStateQuery(AVAILABLE, SUSPENDED, ENABLED, ENABLED);

		// when humanTask 1 is terminated
		manualStart(humanTaskInstance1);
		terminate(humanTaskInstance1);

		// then humanTask 1 is terminated and milestone 1 is completed caused by its entryCriteria
		assertHistoricState(milestoneId1, COMPLETED);
		assertHistoricState(milestoneId2, SUSPENDED);
		assertHistoricState(humanTaskId1, TERMINATED);
		assertHistoricState(humanTaskId2, ENABLED);
		assertStateQuery(COMPLETED, SUSPENDED, TERMINATED, ENABLED);

		// when the case instance is terminated
		terminate(caseInstanceId);

		// then milestone 2 is terminated
		assertHistoricState(milestoneId1, COMPLETED);
		assertHistoricState(milestoneId2, TERMINATED);
		assertHistoricState(humanTaskId1, TERMINATED);
		assertHistoricState(humanTaskId2, TERMINATED);
		assertStateQuery(COMPLETED, TERMINATED, TERMINATED, TERMINATED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHistoricCaseActivityInstanceDates()
	  public virtual void testHistoricCaseActivityInstanceDates()
	  {
		string taskId1 = "PI_HumanTask_1";
		string taskId2 = "PI_HumanTask_2";
		string taskId3 = "PI_HumanTask_3";
		string milestoneId1 = "PI_Milestone_1";
		string milestoneId2 = "PI_Milestone_2";
		string milestoneId3 = "PI_Milestone_3";

		// create test dates
		long duration = 72 * 3600 * 1000;
		DateTime created = ClockUtil.CurrentTime;
		DateTime ended = new DateTime(created.Ticks + duration);

		ClockUtil.CurrentTime = created;
		string caseInstanceId = createCaseInstance().Id;
		string taskInstance1 = queryCaseExecutionByActivityId(taskId1).Id;
		string taskInstance2 = queryCaseExecutionByActivityId(taskId2).Id;
		string taskInstance3 = queryCaseExecutionByActivityId(taskId3).Id;
		string milestoneInstance1 = queryCaseExecutionByActivityId(milestoneId1).Id;
		string milestoneInstance2 = queryCaseExecutionByActivityId(milestoneId2).Id;
		string milestoneInstance3 = queryCaseExecutionByActivityId(milestoneId3).Id;

		// assert create time of all historic instances
		assertHistoricCreateTime(taskId1, created);
		assertHistoricCreateTime(taskId2, created);
		assertHistoricCreateTime(milestoneId1, created);
		assertHistoricCreateTime(milestoneId2, created);

		// complete human task 1
		ClockUtil.CurrentTime = ended;
		complete(taskInstance1);

		// assert end time of human task 1
		assertHistoricEndTime(taskId1, ended);
		assertHistoricDuration(taskId1, duration);

		// complete milestone 1
		ClockUtil.CurrentTime = ended;
		occur(milestoneInstance1);

		// assert end time of milestone 1
		assertHistoricEndTime(milestoneId1, ended);
		assertHistoricDuration(milestoneId1, duration);

		// terminate human task 2
		ClockUtil.CurrentTime = ended;
		terminate(taskInstance2);

		// assert end time of human task 2
		assertHistoricEndTime(taskId2, ended);
		assertHistoricDuration(taskId2, duration);

		// terminate milestone 2
		ClockUtil.CurrentTime = ended;
		terminate(milestoneInstance2);

		// assert end time of user event 2
		assertHistoricEndTime(milestoneId2, ended);
		assertHistoricDuration(milestoneId2, duration);

		// disable human task 3 and suspend milestone 3
		disable(taskInstance3);
		suspend(milestoneInstance3);

		// when terminate case instance
		ClockUtil.CurrentTime = ended;
		terminate(caseInstanceId);

		// then human task 3 and milestone 3 should be terminated and a end time is set
		assertHistoricEndTime(taskId3, ended);
		assertHistoricEndTime(milestoneId3, ended);
		assertHistoricDuration(taskId3, duration);
		assertHistoricDuration(milestoneId3, duration);

		// test queries
		DateTime beforeCreate = new DateTime(created.Ticks - 3600 * 1000);
		DateTime afterEnd = new DateTime(ended.Ticks + 3600 * 1000);

		assertCount(6, historicQuery().createdAfter(beforeCreate));
		assertCount(0, historicQuery().createdAfter(ended));

		assertCount(0, historicQuery().createdBefore(beforeCreate));
		assertCount(6, historicQuery().createdBefore(ended));

		assertCount(0, historicQuery().createdBefore(beforeCreate).createdAfter(ended));

		assertCount(6, historicQuery().endedAfter(created));
		assertCount(0, historicQuery().endedAfter(afterEnd));

		assertCount(0, historicQuery().endedBefore(created));
		assertCount(6, historicQuery().endedBefore(afterEnd));

		assertCount(0, historicQuery().endedBefore(created).endedAfter(afterEnd));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testHistoricCaseActivityTaskId()
	  {
		string taskId = "PI_HumanTask_1";

		createCaseInstance();

		// as long as the human task was not started there should be no task id set
		assertCount(0, taskService.createTaskQuery());
		HistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertNull(historicInstance.TaskId);

		// start human task manually to create task instance
		CaseExecution humanTask = queryCaseExecutionByActivityId(taskId);
		manualStart(humanTask.Id);

		// there should exist a single task
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// check that the task id was correctly set
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(task.Id, historicInstance.TaskId);

		// complete task
		taskService.complete(task.Id);

		// check that the task id is still set
		assertCount(0, taskService.createTaskQuery());
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(task.Id, historicInstance.TaskId);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testHistoricCaseActivityCalledProcessInstanceId()
	  {
		string taskId = "PI_ProcessTask_1";

		createCaseInstanceByKey("oneProcessTaskCase").Id;

		// as long as the process task is not activated there should be no process instance
		assertCount(0, runtimeService.createProcessInstanceQuery());

		HistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertNull(historicInstance.CalledProcessInstanceId);

		// start process task manually to create case instance
		CaseExecution processTask = queryCaseExecutionByActivityId(taskId);
		manualStart(processTask.Id);

		// there should exist a new process instance
		ProcessInstance calledProcessInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(calledProcessInstance);

		// check that the called process instance id was correctly set
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(calledProcessInstance.Id, historicInstance.CalledProcessInstanceId);

		// complete task and thereby the process instance
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// check that the task id is still set
		assertCount(0, runtimeService.createProcessInstanceQuery());
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(calledProcessInstance.Id, historicInstance.CalledProcessInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testHistoricCaseActivityCalledCaseInstanceId()
	  {
		string taskId = "PI_CaseTask_1";

		string calledCaseId = "oneTaskCase";
		string calledTaskId = "PI_HumanTask_1";

		createCaseInstanceByKey("oneCaseTaskCase").Id;

		// as long as the case task is not activated there should be no other case instance
		assertCount(0, caseService.createCaseInstanceQuery().caseDefinitionKey(calledCaseId));

		HistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertNull(historicInstance.CalledCaseInstanceId);

		// start case task manually to create case instance
		CaseExecution caseTask = queryCaseExecutionByActivityId(taskId);
		manualStart(caseTask.Id);

		// there should exist a new case instance
		CaseInstance calledCaseInstance = caseService.createCaseInstanceQuery().caseDefinitionKey(calledCaseId).singleResult();
		assertNotNull(calledCaseInstance);

		// check that the called case instance id was correctly set
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(calledCaseInstance.Id, historicInstance.CalledCaseInstanceId);

		// disable task to complete called case instance and close it
		CaseExecution calledTask = queryCaseExecutionByActivityId(calledTaskId);
		disable(calledTask.Id);
		close(calledCaseInstance.Id);

		// check that the called case instance id is still set
		assertCount(0, caseService.createCaseInstanceQuery().caseDefinitionKey(calledCaseId));
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(calledCaseInstance.Id, historicInstance.CalledCaseInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"})]
	  public virtual void testHistoricCaseActivityQuery()
	  {
		string stageId = "PI_Stage_1";
		string stageName = "A HumanTask";
		string taskId = "PI_HumanTask_1";
		string taskName = "A HumanTask";

		string caseInstanceId = createCaseInstance().Id;

		CaseExecution stageExecution = queryCaseExecutionByActivityId(stageId);
		CaseExecution taskExecution = queryCaseExecutionByActivityId(taskId);

		assertCount(1, historicQuery().caseActivityInstanceId(stageExecution.Id));
		assertCount(1, historicQuery().caseActivityInstanceId(taskExecution.Id));

		assertCount(2, historicQuery().caseInstanceId(caseInstanceId));
		assertCount(2, historicQuery().caseDefinitionId(stageExecution.CaseDefinitionId));

		assertCount(1, historicQuery().caseExecutionId(stageExecution.Id));
		assertCount(1, historicQuery().caseExecutionId(taskExecution.Id));

		assertCount(1, historicQuery().caseActivityId(stageId));
		assertCount(1, historicQuery().caseActivityId(taskId));

		assertCount(1, historicQuery().caseActivityName(stageName));
		assertCount(1, historicQuery().caseActivityName(taskName));

		assertCount(1, historicQuery().caseActivityType("stage"));
		assertCount(1, historicQuery().caseActivityType("humanTask"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryPaging()
	  {
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();

		assertEquals(3, historicQuery().listPage(0, 3).size());
		assertEquals(2, historicQuery().listPage(2, 2).size());
		assertEquals(1, historicQuery().listPage(3, 2).size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" })]
	  public virtual void testQuerySorting()
	  {
		string taskId1 = "PI_HumanTask_1";
		string taskId2 = "PI_HumanTask_2";

		string oneTaskCaseId = createCaseInstanceByKey("oneTaskCase").Id;
		string twoTaskCaseId = createCaseInstanceByKey("twoTaskCase").Id;

		CaseExecution task1 = caseService.createCaseExecutionQuery().caseInstanceId(oneTaskCaseId).activityId(taskId1).singleResult();
		CaseExecution task2 = caseService.createCaseExecutionQuery().caseInstanceId(twoTaskCaseId).activityId(taskId1).singleResult();
		CaseExecution task3 = caseService.createCaseExecutionQuery().caseInstanceId(twoTaskCaseId).activityId(taskId2).singleResult();

		// sort by historic case activity instance ids
		assertQuerySorting("id", historicQuery().orderByHistoricCaseActivityInstanceId(), task1.Id, task2.Id, task3.Id);

		// sort by case instance ids
		assertQuerySorting("caseInstanceId", historicQuery().orderByCaseInstanceId(), oneTaskCaseId, twoTaskCaseId, twoTaskCaseId);

		// sort by case execution ids
		assertQuerySorting("caseExecutionId", historicQuery().orderByCaseExecutionId(), task1.Id, task2.Id, task3.Id);

		// sort by case activity ids
		assertQuerySorting("caseActivityId", historicQuery().orderByCaseActivityId(), taskId1, taskId1, taskId2);

		// sort by case activity names
		assertQuerySorting("caseActivityName", historicQuery().orderByCaseActivityName(), "A HumanTask", "A HumanTask", "Another HumanTask");

		// sort by case definition ids
		assertQuerySorting("caseDefinitionId", historicQuery().orderByCaseDefinitionId(), task1.CaseDefinitionId, task2.CaseDefinitionId, task3.CaseDefinitionId);

		// manually start tasks to be able to complete them
		manualStart(task2.Id);
		manualStart(task3.Id);

		// complete tasks to set end time and duration
		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.complete(task.Id);
		}

		HistoricCaseActivityInstanceQuery query = historyService.createHistoricCaseActivityInstanceQuery();
		HistoricCaseActivityInstance historicTask1 = query.caseInstanceId(oneTaskCaseId).caseActivityId(taskId1).singleResult();
		HistoricCaseActivityInstance historicTask2 = query.caseInstanceId(twoTaskCaseId).caseActivityId(taskId1).singleResult();
		HistoricCaseActivityInstance historicTask3 = query.caseInstanceId(twoTaskCaseId).caseActivityId(taskId2).singleResult();

		// sort by create times
		assertQuerySorting("createTime", historicQuery().orderByHistoricCaseActivityInstanceCreateTime(), historicTask1.CreateTime, historicTask2.CreateTime, historicTask3.CreateTime);

		// sort by end times
		assertQuerySorting("endTime", historicQuery().orderByHistoricCaseActivityInstanceEndTime(), historicTask1.EndTime, historicTask2.EndTime, historicTask3.EndTime);

		// sort by durations times
		assertQuerySorting("durationInMillis", historicQuery().orderByHistoricCaseActivityInstanceDuration(), historicTask1.DurationInMillis, historicTask2.DurationInMillis, historicTask3.DurationInMillis);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQuerySortingCaseActivityType()
	  public virtual void testQuerySortingCaseActivityType()
	  {
		createCaseInstance().Id;

		// sort by case activity type
		assertQuerySorting("caseActivityType", historicQuery().orderByCaseActivityType(), "milestone", "processTask", "humanTask");
	  }

	  public virtual void testInvalidSorting()
	  {
		try
		{
		  historicQuery().asc();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  historicQuery().desc();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  historicQuery().orderByHistoricCaseActivityInstanceId().count();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testNativeQuery()
	  {
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();

		string instanceId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").list().get(0).Id;

		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		string tableName = managementService.getTableName(typeof(HistoricCaseActivityInstance));

		assertEquals(tablePrefix + "ACT_HI_CASEACTINST", tableName);
		assertEquals(tableName, managementService.getTableName(typeof(HistoricCaseActivityInstanceEntity)));

		assertEquals(4, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT * FROM " + tableName).list().size());
		assertEquals(4, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT count(*) FROM " + tableName).count());

		assertEquals(16, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT count(*) FROM " + tableName + " H1, " + tableName + " H2").count());

		// select with distinct
		assertEquals(4, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT DISTINCT * FROM " + tableName).list().size());

		assertEquals(1, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT count(*) FROM " + tableName + " H WHERE H.ID_ = '" + instanceId + "'").count());
		assertEquals(1, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT * FROM " + tableName + " H WHERE H.ID_ = '" + instanceId + "'").list().size());

		// use parameters
		assertEquals(1, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT count(*) FROM " + tableName + " H WHERE H.ID_ = #{caseActivityInstanceId}").parameter("caseActivityInstanceId", instanceId).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testNativeQueryPaging()
	  {
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();

		string tableName = managementService.getTableName(typeof(HistoricCaseActivityInstance));
		assertEquals(3, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT * FROM " + tableName).listPage(0, 3).size());
		assertEquals(2, historyService.createNativeHistoricCaseActivityInstanceQuery().sql("SELECT * FROM " + tableName).listPage(2, 2).size());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDeleteHistoricCaseActivityInstance()
	  {
		CaseInstance caseInstance = createCaseInstance();

		HistoricCaseActivityInstance historicInstance = historicQuery().singleResult();
		assertNotNull(historicInstance);

		// disable human task to complete case
		disable(historicInstance.Id);
		// close case to be able to delete historic case instance
		close(caseInstance.Id);
		// delete historic case instance
		historyService.deleteHistoricCaseInstance(caseInstance.Id);

		assertCount(0, historicQuery());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonBlockingHumanTask()
	  public virtual void testNonBlockingHumanTask()
	  {
		CaseInstance caseInstance = createCaseInstance();
		assertNotNull(caseInstance);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testRequiredRuleEvaluatesToTrue()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", true));

		HistoricCaseActivityInstance task = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_1").singleResult();

		assertNotNull(task);
		assertTrue(task.Required);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testRequiredRuleEvaluatesToFalse()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", false));

		HistoricCaseActivityInstance task = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_1").singleResult();

		assertNotNull(task);
		assertFalse(task.Required);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testQueryByRequired()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", true));

		HistoricCaseActivityInstanceQuery query = historyService.createHistoricCaseActivityInstanceQuery().required();

		assertEquals(1, query.count());
		assertEquals(1, query.list().size());

		HistoricCaseActivityInstance activityInstance = query.singleResult();
		assertNotNull(activityInstance);
		assertTrue(activityInstance.Required);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/stage/AutoCompleteTest.testCasePlanModel.cmmn"})]
	  public virtual void testAutoCompleteEnabled()
	  {
		string caseInstanceId = createCaseInstanceByKey("case").Id;

		HistoricCaseInstance caseInstance = historyService.createHistoricCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		HistoricCaseActivityInstanceQuery query = historyService.createHistoricCaseActivityInstanceQuery();

		HistoricCaseActivityInstance humanTask1 = query.caseActivityId("PI_HumanTask_1").singleResult();
		assertNotNull(humanTask1);
		assertTrue(humanTask1.Terminated);
		assertNotNull(humanTask1.EndTime);
		assertNotNull(humanTask1.DurationInMillis);


		HistoricCaseActivityInstance humanTask2 = query.caseActivityId("PI_HumanTask_2").singleResult();
		assertNotNull(humanTask2);
		assertTrue(humanTask2.Terminated);
		assertNotNull(humanTask2.EndTime);
		assertNotNull(humanTask2.DurationInMillis);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatTask.cmmn"})]
	  public virtual void testRepeatTask()
	  {
		// given
		createCaseInstance();
		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityId("PI_HumanTask_2");
		assertEquals(2, query.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatStage.cmmn"})]
	  public virtual void testRepeatStage()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityId("PI_Stage_1");
		assertEquals(2, query.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatMilestone.cmmn"})]
	  public virtual void testRepeatMilestone()
	  {
		// given
		createCaseInstance();
		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityId("PI_Milestone_1");
		assertEquals(2, query.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testAutoCompleteStage.cmmn"})]
	  public virtual void testAutoCompleteStage()
	  {
		// given
		createCaseInstance();
		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		// then
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityId("PI_Stage_1");
		assertEquals(1, query.count());

		query = historicQuery().caseActivityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		query = historicQuery().caseActivityId("PI_HumanTask_2");
		assertEquals(2, query.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testAutoCompleteStageWithoutEntryCriteria.cmmn"})]
	  public virtual void testAutoCompleteStageWithRepeatableTaskWithoutEntryCriteria()
	  {
		// given
		createCaseInstanceByKey("case", Variables.createVariables().putValue("manualActivation", false));
		queryCaseExecutionByActivityId("PI_Stage_1");

		// when
		string humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;
		complete(humanTask);

		// then
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityId("PI_HumanTask_1");
		assertEquals(2, query.count());

		query = historicQuery().caseActivityId("PI_Stage_1");
		assertEquals(1, query.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDecisionTask()
	  public virtual void testDecisionTask()
	  {
		createCaseInstance();

		HistoricCaseActivityInstance decisionTask = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_DecisionTask_1").singleResult();

		assertNotNull(decisionTask);
		assertEquals("decisionTask", decisionTask.CaseActivityType);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceId()
	  {
		// given
		createCaseInstance();

		string taskInstanceId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityInstanceIdIn(taskInstanceId);

		// then
		assertCount(1, query);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceIds()
	  {
		// given
		CaseInstance instance1 = createCaseInstance();
		CaseInstance instance2 = createCaseInstance();

		string taskInstanceId1 = caseService.createCaseExecutionQuery().caseInstanceId(instance1.Id).activityId("PI_HumanTask_1").singleResult().Id;

		string taskInstanceId2 = caseService.createCaseExecutionQuery().caseInstanceId(instance2.Id).activityId("PI_HumanTask_1").singleResult().Id;

		// when
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityInstanceIdIn(taskInstanceId1, taskInstanceId2);

		// then
		assertCount(2, query);
	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {

		// when
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityInstanceIdIn("invalid");

		// then
		assertCount(0, query);

		try
		{
		  historicQuery().caseActivityInstanceIdIn((string[])null);
		  fail("A NotValidException was expected.");
		}
		catch (NotValidException)
		{
		}

		try
		{
		  historicQuery().caseActivityInstanceIdIn((string)null);
		  fail("A NotValidException was expected.");
		}
		catch (NotValidException)
		{
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" })]
	  public virtual void testQueryByCaseActivityIds()
	  {
		// given
		createCaseInstanceByKey("oneTaskCase");
		createCaseInstanceByKey("twoTaskCase");

		// when
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityIdIn("PI_HumanTask_1", "PI_HumanTask_2");

		// then
		assertCount(3, query);
	  }

	  public virtual void testQueryByInvalidCaseActivityId()
	  {

		// when
		HistoricCaseActivityInstanceQuery query = historicQuery().caseActivityIdIn("invalid");

		// then
		assertCount(0, query);

		try
		{
		  historicQuery().caseActivityIdIn((string[])null);
		  fail("A NotValidException was expected.");
		}
		catch (NotValidException)
		{
		}

		try
		{
		  historicQuery().caseActivityIdIn((string)null);
		  fail("A NotValidException was expected.");
		}
		catch (NotValidException)
		{
		}
	  }

	  protected internal virtual HistoricCaseActivityInstanceQuery historicQuery()
	  {
		return historyService.createHistoricCaseActivityInstanceQuery();
	  }

	  protected internal virtual HistoricCaseActivityInstance queryHistoricActivityCaseInstance(string activityId)
	  {
		HistoricCaseActivityInstance historicActivityInstance = historicQuery().caseActivityId(activityId).singleResult();
		assertNotNull("No historic activity instance found for activity id: " + activityId, historicActivityInstance);
		return historicActivityInstance;
	  }

	  protected internal virtual void assertHistoricState(string activityId, CaseExecutionState expectedState)
	  {
		HistoricCaseActivityInstanceEventEntity historicActivityInstance = (HistoricCaseActivityInstanceEventEntity) queryHistoricActivityCaseInstance(activityId);
		int actualStateCode = historicActivityInstance.CaseActivityInstanceState;
		CaseExecutionState actualState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_CaseExecutionStateImpl.getStateForCode(actualStateCode);
		assertEquals("The state of historic case activity '" + activityId + "' wasn't as expected", expectedState, actualState);
	  }

	  protected internal virtual void assertHistoricCreateTime(string activityId, DateTime expectedCreateTime)
	  {
		HistoricCaseActivityInstance historicActivityInstance = queryHistoricActivityCaseInstance(activityId);
		DateTime actualCreateTime = historicActivityInstance.CreateTime;
		assertSimilarDate(expectedCreateTime, actualCreateTime);
	  }

	  protected internal virtual void assertHistoricEndTime(string activityId, DateTime expectedEndTime)
	  {
		HistoricCaseActivityInstance historicActivityInstance = queryHistoricActivityCaseInstance(activityId);
		DateTime actualEndTime = historicActivityInstance.EndTime;
		assertSimilarDate(expectedEndTime, actualEndTime);
	  }

	  protected internal virtual void assertSimilarDate(DateTime expectedDate, DateTime actualDate)
	  {
		long difference = Math.Abs(expectedDate.Ticks - actualDate.Ticks);
		// assert that the dates don't differ more than a second
		assertTrue(difference < 1000);
	  }

	  protected internal virtual void assertHistoricDuration(string activityId, long expectedDuration)
	  {
		long? actualDuration = queryHistoricActivityCaseInstance(activityId).DurationInMillis;
		assertNotNull(actualDuration);
		// test that duration is as expected with a maximal difference of one second
		assertTrue(actualDuration.Value >= expectedDuration);
		assertTrue(actualDuration.Value < expectedDuration + 1000);
	  }

	  protected internal virtual void assertCount<T1>(long count, Query<T1> historicQuery)
	  {
		assertEquals(count, historicQuery.count());
	  }

	  protected internal virtual void assertStateQuery(params CaseExecutionState[] states)
	  {
		CaseExecutionStateCountMap stateCounts = new CaseExecutionStateCountMap(this);

		if (states != null)
		{
		  foreach (CaseExecutionState state in states)
		  {
			stateCounts[state] = stateCounts[state] + 1;
		  }
		}

		assertCount(stateCounts.count(), historicQuery());
		assertCount(stateCounts.unfinished().Value, historicQuery().notEnded());
		assertCount(stateCounts.finished().Value, historicQuery().ended());

		assertCount(stateCounts[ACTIVE], historicQuery().active());
		assertCount(stateCounts[AVAILABLE], historicQuery().available());
		assertCount(stateCounts[COMPLETED], historicQuery().completed());
		assertCount(stateCounts[DISABLED], historicQuery().disabled());
		assertCount(stateCounts[ENABLED], historicQuery().enabled());
		assertCount(stateCounts[TERMINATED], historicQuery().terminated());
	  }

	  protected internal class CaseExecutionStateCountMap : Dictionary<CaseExecutionState, long>
	  {
		  private readonly HistoricCaseActivityInstanceTest outerInstance;


		internal const long serialVersionUID = 1L;

		public readonly IDictionary<int, CaseExecutionState>.ValueCollection ALL_STATES = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CASE_EXECUTION_STATES.Values;
		public readonly ICollection<CaseExecutionState> ENDED_STATES = Arrays.asList(COMPLETED, TERMINATED);
		public readonly ICollection<CaseExecutionState> NOT_ENDED_STATES;

		public CaseExecutionStateCountMap(HistoricCaseActivityInstanceTest outerInstance)
		{
			this.outerInstance = outerInstance;
		  NOT_ENDED_STATES = new List<CaseExecutionState>(ALL_STATES);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		  NOT_ENDED_STATES.removeAll(ENDED_STATES);
		}

		public virtual long? get(CaseExecutionState state)
		{
		  return state != null && this.ContainsKey(state) ? base[state] : 0;
		}

		public virtual long? count()
		{
		  return count(ALL_STATES);
		}

		public virtual long? finished()
		{
		  return count(ENDED_STATES);
		}

		public virtual long? unfinished()
		{
		  return count(NOT_ENDED_STATES);
		}

		public virtual long? count(ICollection<CaseExecutionState> states)
		{
		  long count = 0;
		  foreach (CaseExecutionState state in states)
		  {
			count += this[state];
		  }
		  return count;
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) protected void assertQuerySorting(String property, org.camunda.bpm.engine.query.Query<?, ?> query, Comparable... items)
	  protected internal virtual void assertQuerySorting<T1>(string property, Query<T1> query, params IComparable[] items)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.AbstractQuery<?, ?> queryImpl = (org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query;
		AbstractQuery<object, ?> queryImpl = (AbstractQuery<object, ?>) query;

		// save order properties to later reverse ordering
		IList<QueryOrderingProperty> orderProperties = queryImpl.OrderingProperties;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends Comparable> sortedList = java.util.Arrays.asList(items);
		IList<IComparable> sortedList = Arrays.asList(items);
		sortedList.Sort();

		IList<Matcher<object>> matchers = new List<Matcher<object>>();
		foreach (IComparable comparable in sortedList)
		{
		  matchers.Add(hasProperty(property, equalTo(comparable)));
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<?> instances = query.asc().list();
		IList<object> instances = query.asc().list();
		assertEquals(sortedList.Count, instances.Count);
		assertThat(instances, contains(matchers.ToArray()));

		// reverse ordering
		foreach (QueryOrderingProperty orderingProperty in orderProperties)
		{
		  orderingProperty.Direction = Direction.DESCENDING;
		}

		// reverse matchers
		matchers.Reverse();

		instances = query.list();
		assertEquals(sortedList.Count, instances.Count);
		assertThat(instances, contains(matchers.ToArray()));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricCaseActivityInstanceTest.oneStageAndOneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testHistoricActivityInstanceWithinStageIsMarkedTerminatedOnComplete()
	  {

		// given
		createCaseInstance();

		string stageExecutionId = queryCaseExecutionByActivityId("PI_Stage_1").Id;
		manualStart(stageExecutionId);
		string activeStageTaskExecutionId = queryCaseExecutionByActivityId("PI_HumanTask_Stage_2").Id;
		complete(activeStageTaskExecutionId);
		CaseExecution enabledStageTaskExecutionId = queryCaseExecutionByActivityId("PI_HumanTask_Stage_1");
		assertTrue(enabledStageTaskExecutionId.Enabled);

		// when
		complete(stageExecutionId);

		// then the remaining stage task that was enabled is set to terminated in history
		HistoricCaseActivityInstance manualActivationTask = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_Stage_1").singleResult();
		HistoricCaseActivityInstance completedTask = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_Stage_2").singleResult();

		assertTrue(manualActivationTask.Terminated);
		assertTrue(completedTask.Completed);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricCaseActivityInstanceTest.oneStageAndOneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testHistoricActivityInstancesAreMarkedTerminatedOnComplete()
	  {

		// given
		createCaseInstance();

		CaseExecution humanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		assertTrue(humanTask.Enabled);
		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		assertTrue(stage.Enabled);

		// when
		CaseExecution casePlanExecution = queryCaseExecutionByActivityId("CasePlanModel_1");
		complete(casePlanExecution.Id);

		// then make sure all cases in the lower scope are marked as terminated in history
		HistoricCaseActivityInstance stageInstance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_Stage_1").singleResult();
		HistoricCaseActivityInstance taskInstance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_3").singleResult();

		assertTrue(stageInstance.Terminated);
		assertTrue(taskInstance.Terminated);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricCaseActivityInstanceTest.oneStageAndOneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDisabledHistoricActivityInstancesStayDisabledOnComplete()
	  {

		// given
		createCaseInstance();

		CaseExecution humanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		assertTrue(humanTask.Enabled);
		CaseExecution stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");
		disable(stageExecution.Id);
		stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");
		assertTrue(stageExecution.Disabled);

		// when
		CaseExecution casePlanExecution = queryCaseExecutionByActivityId("CasePlanModel_1");
		complete(casePlanExecution.Id);

		// then make sure disabled executions stay disabled
		HistoricCaseActivityInstance stageInstance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_Stage_1").singleResult();
		HistoricCaseActivityInstance taskInstance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_3").singleResult();

		assertTrue(stageInstance.Disabled);
		assertTrue(taskInstance.Terminated);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMilestoneHistoricActivityInstanceIsTerminatedOnComplete()
	  public virtual void testMilestoneHistoricActivityInstanceIsTerminatedOnComplete()
	  {

		// given
		createCaseInstance();
		const string milestoneId = "PI_Milestone_1";
		CaseExecution caseMilestone = queryCaseExecutionByActivityId(milestoneId);
		assertTrue(caseMilestone.Available);

		// when
		CaseExecution casePlanExecution = queryCaseExecutionByActivityId("CasePlanModel_1");
		complete(casePlanExecution.Id);

		// then make sure that the milestone is terminated
		HistoricCaseActivityInstance milestoneInstance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId(milestoneId).singleResult();

		assertTrue(milestoneInstance.Terminated);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricCaseActivityInstanceTest.oneStageWithSentryAsEntryPointCase.cmmn"})]
	  public virtual void testHistoricTaskWithSentryIsMarkedTerminatedOnComplete()
	  {

		// given
		createCaseInstance();

		// when
		CaseExecution casePlanExecution = queryCaseExecutionByActivityId("PI_Stage_1");
		complete(casePlanExecution.Id);

		// then both tasks are terminated
		HistoricCaseActivityInstance taskInstance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_1").singleResult();

		HistoricCaseActivityInstance taskInstance2 = historyService.createHistoricCaseActivityInstanceQuery().caseActivityId("PI_HumanTask_2").singleResult();

		assertTrue(taskInstance.Terminated);
		assertTrue(taskInstance2.Terminated);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricCaseActivityInstanceTest.oneStageWithSentryAsEntryPointCase.cmmn"})]
	  public virtual void testHistoricTaskWithSentryDoesNotReachStateActiveOnComplete()
	  {

		// given
		createCaseInstance();

		// when
		CaseExecution casePlanExecution = queryCaseExecutionByActivityId("PI_Stage_1");
		complete(casePlanExecution.Id);

		// then task 2 was never in state 'active'
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseExecutionIdIn(casePlanExecution.Id);

		assertEquals(0, query.count());
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/history/HistoricCaseActivityInstanceTest.oneTaskProcess.bpmn20.xml" })]
	  public virtual void testHistoricCalledProcessInstanceId()
	  {
		string taskId = "PI_ProcessTask_1";

		createCaseInstanceByKey("oneProcessTaskCase").Id;

		// as long as the process task is not activated there should be no process instance
		assertCount(0, historyService.createHistoricProcessInstanceQuery());

		HistoricCaseActivityInstance historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertNull(historicInstance.CalledProcessInstanceId);

		// start process task manually to create case instance
		CaseExecution processTask = queryCaseExecutionByActivityId(taskId);
		manualStart(processTask.Id);

		// there should exist a new process instance
		HistoricProcessInstance calledProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(calledProcessInstance);
		assertNotNull(calledProcessInstance.EndTime);

		// check that the called process instance id was correctly set
		historicInstance = queryHistoricActivityCaseInstance(taskId);
		assertEquals(calledProcessInstance.Id, historicInstance.CalledProcessInstanceId);
	  }

	}

}