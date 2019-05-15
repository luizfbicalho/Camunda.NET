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
namespace org.camunda.bpm.engine.test.standalone.history
{

	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricTaskInstanceQueryTest : PluggableProcessEngineTestCase
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueEqualsNumber() throws Exception
		[Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
		public virtual void testProcessVariableValueEqualsNumber()
		{
		// long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123L));

		// non-matching long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 12345L));

		// short
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (short) 123));

		// double
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123.0d));

		// integer
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123));

		// untyped null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", null));

		// typed null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueEquals("var", Variables.numberValue(null)).count());

		assertEquals(8, historyService.createHistoricTaskInstanceQuery().processVariableValueNotEquals("var", 999L).count());
		assertEquals(8, historyService.createHistoricTaskInstanceQuery().processVariableValueNotEquals("var", (short) 999).count());
		assertEquals(8, historyService.createHistoricTaskInstanceQuery().processVariableValueNotEquals("var", 999).count());
		assertEquals(8, historyService.createHistoricTaskInstanceQuery().processVariableValueNotEquals("var", "999").count());
		assertEquals(8, historyService.createHistoricTaskInstanceQuery().processVariableValueNotEquals("var", false).count());

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueLike() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueLike()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("requester", "vahid alizadeh"));

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", "vahid%").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", "%alizadeh").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", "%ali%").count());

		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", "requester%").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", "%ali").count());

		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", "vahid").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().processVariableValueLike("nonExistingVar", "string%").count());

		// test with null value
		try
		{
		  historyService.createHistoricTaskInstanceQuery().processVariableValueLike("requester", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueGreaterThan() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueGreaterThan()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("requestNumber", 123));

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueGreaterThan("requestNumber", 122).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueGreaterThanOrEqual() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueGreaterThanOrEqual()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("requestNumber", 123));

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueGreaterThanOrEquals("requestNumber", 122).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueGreaterThanOrEquals("requestNumber", 123).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueLessThan() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueLessThan()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("requestNumber", 123));

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueLessThan("requestNumber", 124).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueLessThanOrEqual() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueLessThanOrEqual()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("requestNumber", 123));

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueLessThanOrEquals("requestNumber", 123).count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().processVariableValueLessThanOrEquals("requestNumber", 124).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testTaskVariableValueEqualsNumber() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testTaskVariableValueEqualsNumber()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").list();
		assertEquals(8, tasks.Count);
		taskService.setVariableLocal(tasks[0].Id, "var", 123L);
		taskService.setVariableLocal(tasks[1].Id, "var", 12345L);
		taskService.setVariableLocal(tasks[2].Id, "var", (short) 123);
		taskService.setVariableLocal(tasks[3].Id, "var", 123.0d);
		taskService.setVariableLocal(tasks[4].Id, "var", 123);
		taskService.setVariableLocal(tasks[5].Id, "var", null);
		taskService.setVariableLocal(tasks[6].Id, "var", Variables.longValue(null));
		taskService.setVariableLocal(tasks[7].Id, "var", "123");

		assertEquals(4, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskVariableValueEquals("var", Variables.numberValue(null)).count());
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskInvolvedUser()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateUser(taskId, "aUserId");
		taskService.addCandidateUser(taskId, "bUserId");
		taskService.deleteCandidateUser(taskId, "aUserId");
		taskService.deleteCandidateUser(taskId, "bUserId");
		Task taskAssignee = taskService.newTask("newTask");
		taskAssignee.Assignee = "aUserId";
		taskService.saveTask(taskAssignee);
		// query test
		assertEquals(2, historyService.createHistoricTaskInstanceQuery().taskInvolvedUser("aUserId").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskInvolvedUser("bUserId").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskInvolvedUser("invalidUserId").count());
		taskService.deleteTask("newTask",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskInvolvedGroup()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateGroup(taskId, "aGroupId");
		taskService.addCandidateGroup(taskId, "bGroupId");
		taskService.deleteCandidateGroup(taskId, "aGroupId");
		taskService.deleteCandidateGroup(taskId, "bGroupId");
		// query test
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskInvolvedGroup("aGroupId").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskInvolvedGroup("bGroupId").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskInvolvedGroup("invalidGroupId").count());

		taskService.deleteTask("newTask",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskHadCandidateUser()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateUser(taskId, "aUserId");
		taskService.addCandidateUser(taskId, "bUserId");
		taskService.deleteCandidateUser(taskId, "bUserId");
		Task taskAssignee = taskService.newTask("newTask");
		taskAssignee.Assignee = "aUserId";
		taskService.saveTask(taskAssignee);
		// query test
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskHadCandidateUser("aUserId").count());
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskHadCandidateUser("bUserId").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskHadCandidateUser("invalidUserId").count());
		// delete test
		taskService.deleteTask("newTask",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskHadCandidateGroup()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateGroup(taskId, "bGroupId");
		taskService.deleteCandidateGroup(taskId, "bGroupId");
		// query test
		assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskHadCandidateGroup("bGroupId").count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().taskHadCandidateGroup("invalidGroupId").count());
		// delete test
		taskService.deleteTask("newTask",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testWithCandidateGroups()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateGroup(taskId, "aGroupId");

		// then
		assertEquals(historyService.createHistoricTaskInstanceQuery().withCandidateGroups().count(), 1);

		// cleanup
		taskService.deleteTask("newTask", true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testWithoutCandidateGroups()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateGroup(taskId, "aGroupId");

		// when
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// then
		assertEquals(historyService.createHistoricTaskInstanceQuery().count(), 2);
		assertEquals(historyService.createHistoricTaskInstanceQuery().withoutCandidateGroups().count(), 1);

		// cleanup
		taskService.deleteTask("newTask", true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testGroupTaskQuery()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateUser(taskId, "aUserId");
		taskService.addCandidateGroup(taskId, "aGroupId");
		taskService.addCandidateGroup(taskId, "bGroupId");
		Task taskOne = taskService.newTask("taskOne");
		taskOne.Assignee = "aUserId";
		taskService.saveTask(taskOne);
		Task taskTwo = taskService.newTask("taskTwo");
		taskTwo.Assignee = "aUserId";
		taskService.saveTask(taskTwo);
		Task taskThree = taskService.newTask("taskThree");
		taskThree.Owner = "aUserId";
		taskService.saveTask(taskThree);
		taskService.deleteCandidateGroup(taskId, "aGroupId");
		taskService.deleteCandidateGroup(taskId, "bGroupId");
		historyService.createHistoricTaskInstanceQuery();

		// Query test
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();
		assertEquals(4, query.taskInvolvedUser("aUserId").count());
		query = historyService.createHistoricTaskInstanceQuery();
		assertEquals(1, query.taskHadCandidateUser("aUserId").count());
		query = historyService.createHistoricTaskInstanceQuery();
		assertEquals(1, query.taskHadCandidateGroup("aGroupId").count());
		assertEquals(1, query.taskHadCandidateGroup("bGroupId").count());
		assertEquals(0, query.taskInvolvedUser("aUserId").count());
		query = historyService.createHistoricTaskInstanceQuery();
		assertEquals(4, query.taskInvolvedUser("aUserId").count());
		assertEquals(1, query.taskHadCandidateUser("aUserId").count());
		assertEquals(1, query.taskInvolvedUser("aUserId").count());
		// delete task
		taskService.deleteTask("taskOne",true);
		taskService.deleteTask("taskTwo",true);
		taskService.deleteTask("taskThree",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskWasAssigned()
	  {
		// given
		Task taskOne = taskService.newTask("taskOne");
		Task taskTwo = taskService.newTask("taskTwo");
		Task taskThree = taskService.newTask("taskThree");

		// when
		taskOne.Assignee = "aUserId";
		taskService.saveTask(taskOne);

		taskTwo.Assignee = "anotherUserId";
		taskService.saveTask(taskTwo);

		taskService.saveTask(taskThree);

		IList<HistoricTaskInstance> list = historyService.createHistoricTaskInstanceQuery().taskAssigned().list();

		// then
		assertEquals(list.Count, 2);

		// cleanup
		taskService.deleteTask("taskOne",true);
		taskService.deleteTask("taskTwo",true);
		taskService.deleteTask("taskThree",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskWasUnassigned()
	  {
		// given
		Task taskOne = taskService.newTask("taskOne");
		Task taskTwo = taskService.newTask("taskTwo");
		Task taskThree = taskService.newTask("taskThree");

		// when
		taskOne.Assignee = "aUserId";
		taskService.saveTask(taskOne);

		taskTwo.Assignee = "anotherUserId";
		taskService.saveTask(taskTwo);

		taskService.saveTask(taskThree);

		IList<HistoricTaskInstance> list = historyService.createHistoricTaskInstanceQuery().taskUnassigned().list();

		// then
		assertEquals(list.Count, 1);

		// cleanup
		taskService.deleteTask("taskOne",true);
		taskService.deleteTask("taskTwo",true);
		taskService.deleteTask("taskThree",true);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskReturnedBeforeEndTime()
	  {
		// given
		Task taskOne = taskService.newTask("taskOne");

		// when
		taskOne.Assignee = "aUserId";
		taskService.saveTask(taskOne);

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		ClockUtil.CurrentTime = hourAgo;

		taskService.complete(taskOne.Id);

		IList<HistoricTaskInstance> list = historyService.createHistoricTaskInstanceQuery().finishedBefore(hourAgo).list();

		// then
		assertEquals(1, list.Count);

		// cleanup
		taskService.deleteTask("taskOne",true);
		ClockUtil.reset();
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTaskNotReturnedAfterEndTime()
	  {
		// given
		Task taskOne = taskService.newTask("taskOne");

		// when
		taskOne.Assignee = "aUserId";
		taskService.saveTask(taskOne);

		DateTime hourAgo = new DateTime();
		hourAgo.AddHours(-1);
		ClockUtil.CurrentTime = hourAgo;

		taskService.complete(taskOne.Id);

		IList<HistoricTaskInstance> list = historyService.createHistoricTaskInstanceQuery().finishedAfter(new DateTime()).list();

		// then
		assertEquals(0, list.Count);

		// cleanup
		taskService.deleteTask("taskOne",true);

		ClockUtil.reset();
	  }

	}

}