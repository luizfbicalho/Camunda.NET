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
namespace org.camunda.bpm.engine.test.api.task
{
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TaskQueryOrTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule processEngineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule processEngineRule = new ProcessEngineRule(true);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal CaseService caseService;
	  protected internal RepositoryService repositoryService;
	  protected internal FilterService filterService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = processEngineRule.RuntimeService;
		taskService = processEngineRule.TaskService;
		caseService = processEngineRule.CaseService;
		repositoryService = processEngineRule.RepositoryService;
		filterService = processEngineRule.FilterService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.deleteTask(task.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByMissingStartOr()
	  public virtual void shouldThrowExceptionByMissingStartOr()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set endOr() before or()");

		taskService.createTaskQuery().or().endOr().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByNesting()
	  public virtual void shouldThrowExceptionByNesting()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set or() within 'or' query");

		taskService.createTaskQuery().or().or().endOr().endOr().or().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByWithCandidateGroupsApplied()
	  public virtual void shouldThrowExceptionByWithCandidateGroupsApplied()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set withCandidateGroups() within 'or' query");

		taskService.createTaskQuery().or().withCandidateGroups().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByWithoutCandidateGroupsApplied()
	  public virtual void shouldThrowExceptionByWithoutCandidateGroupsApplied()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set withoutCandidateGroups() within 'or' query");

		taskService.createTaskQuery().or().withoutCandidateGroups().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByWithCandidateUsersApplied()
	  public virtual void shouldThrowExceptionByWithCandidateUsersApplied()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set withCandidateUsers() within 'or' query");

		taskService.createTaskQuery().or().withCandidateUsers().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByWithoutCandidateUsersApplied()
	  public virtual void shouldThrowExceptionByWithoutCandidateUsersApplied()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set withoutCandidateUsers() within 'or' query");

		taskService.createTaskQuery().or().withoutCandidateUsers().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByOrderingApplied()
	  public virtual void shouldThrowExceptionByOrderingApplied()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set orderByCaseExecutionId() within 'or' query");

		taskService.createTaskQuery().or().orderByCaseExecutionId().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionByInitializeFormKeysInOrQuery()
	  public virtual void shouldThrowExceptionByInitializeFormKeysInOrQuery()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid query usage: cannot set initializeFormKeys() within 'or' query");

		taskService.createTaskQuery().or().initializeFormKeys().endOr();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnNoTasksWithTaskCandidateUserAndOrTaskCandidateGroup()
	  public virtual void shouldReturnNoTasksWithTaskCandidateUserAndOrTaskCandidateGroup()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.addCandidateUser(task1.Id, "aCandidateUser");

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.addCandidateGroup(task2.Id, "aCandidateGroup");

		// when
		IList<Task> tasks = taskService.createTaskQuery().taskCandidateUser("aCandidateUser").or().taskCandidateGroup("aCandidateGroup").endOr().list();

		// then
		assertEquals(0, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithEmptyOrQuery()
	  public virtual void shouldReturnTasksWithEmptyOrQuery()
	  {
		// given
		taskService.saveTask(taskService.newTask());
		taskService.saveTask(taskService.newTask());

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithTaskCandidateUserOrTaskCandidateGroup()
	  public virtual void shouldReturnTasksWithTaskCandidateUserOrTaskCandidateGroup()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.addCandidateUser(task1.Id, "John Doe");

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.addCandidateGroup(task2.Id, "Controlling");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskCandidateUser("John Doe").taskCandidateGroup("Controlling").endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithTaskCandidateUserOrTaskCandidateGroupWithIncludeAssignedTasks()
	  public virtual void shouldReturnTasksWithTaskCandidateUserOrTaskCandidateGroupWithIncludeAssignedTasks()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.addCandidateUser(task1.Id, "John Doe");
		taskService.setAssignee(task1.Id, "John Doe");

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.addCandidateGroup(task2.Id, "Controlling");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskCandidateUser("John Doe").taskCandidateGroup("Controlling").includeAssignedTasks().endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithTaskCandidateUserOrTaskCandidateGroupIn()
	  public virtual void shouldReturnTasksWithTaskCandidateUserOrTaskCandidateGroupIn()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.addCandidateUser(task1.Id, "John Doe");

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.addCandidateGroup(task2.Id, "Controlling");

		Task task3 = taskService.newTask();
		taskService.saveTask(task3);
		taskService.addCandidateGroup(task3.Id, "Sales");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskCandidateUser("John Doe").taskCandidateGroupIn(Arrays.asList("Controlling", "Sales")).endOr().list();

		// then
		assertEquals(3, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithTaskCandidateGroupOrTaskCandidateGroupIn()
	  public virtual void shouldReturnTasksWithTaskCandidateGroupOrTaskCandidateGroupIn()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.addCandidateGroup(task1.Id, "Accounting");

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.addCandidateGroup(task2.Id, "Controlling");

		Task task3 = taskService.newTask();
		taskService.saveTask(task3);
		taskService.addCandidateGroup(task3.Id, "Sales");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskCandidateGroup("Accounting").taskCandidateGroupIn(Arrays.asList("Controlling", "Sales")).endOr().list();

		// then
		assertEquals(3, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithTaskNameOrTaskDescription()
	  public virtual void shouldReturnTasksWithTaskNameOrTaskDescription()
	  {
		// given
		Task task1 = taskService.newTask();
		task1.Name = "aTaskName";
		taskService.saveTask(task1);

		Task task2 = taskService.newTask();
		task2.Description = "aTaskDescription";
		taskService.saveTask(task2);

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskName("aTaskName").taskDescription("aTaskDescription").endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithMultipleOrCriteria()
	  public virtual void shouldReturnTasksWithMultipleOrCriteria()
	  {
		// given
		Task task1 = taskService.newTask();
		task1.Name = "aTaskName";
		taskService.saveTask(task1);

		Task task2 = taskService.newTask();
		task2.Description = "aTaskDescription";
		taskService.saveTask(task2);

		Task task3 = taskService.newTask();
		taskService.saveTask(task3);

		Task task4 = taskService.newTask();
		task4.Priority = 5;
		taskService.saveTask(task4);

		Task task5 = taskService.newTask();
		task5.Owner = "aTaskOwner";
		taskService.saveTask(task5);

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskName("aTaskName").taskDescription("aTaskDescription").taskId(task3.Id).taskPriority(5).taskOwner("aTaskOwner").endOr().list();

		// then
		assertEquals(5, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksFilteredByMultipleOrAndCriteria()
	  public virtual void shouldReturnTasksFilteredByMultipleOrAndCriteria()
	  {
		// given
		Task task1 = taskService.newTask();
		task1.Priority = 4;
		taskService.saveTask(task1);

		Task task2 = taskService.newTask();
		task2.Name = "aTaskName";
		task2.Owner = "aTaskOwner";
		task2.Assignee = "aTaskAssignee";
		task2.Priority = 4;
		taskService.saveTask(task2);

		Task task3 = taskService.newTask();
		task3.Name = "aTaskName";
		task3.Owner = "aTaskOwner";
		task3.Assignee = "aTaskAssignee";
		task3.Priority = 4;
		task3.Description = "aTaskDescription";
		taskService.saveTask(task3);

		Task task4 = taskService.newTask();
		task4.Owner = "aTaskOwner";
		task4.Assignee = "aTaskAssignee";
		task4.Priority = 4;
		task4.Description = "aTaskDescription";
		taskService.saveTask(task4);

		Task task5 = taskService.newTask();
		task5.Description = "aTaskDescription";
		task5.Owner = "aTaskOwner";
		taskService.saveTask(task5);

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskName("aTaskName").taskDescription("aTaskDescription").taskId(task3.Id).endOr().taskOwner("aTaskOwner").taskPriority(4).taskAssignee("aTaskAssignee").list();

		// then
		assertEquals(3, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksFilteredByMultipleOrQueries()
	  public virtual void shouldReturnTasksFilteredByMultipleOrQueries()
	  {
		// given
		Task task1 = taskService.newTask();
		task1.Name = "aTaskName";
		taskService.saveTask(task1);

		Task task2 = taskService.newTask();
		task2.Name = "aTaskName";
		task2.Description = "aTaskDescription";
		taskService.saveTask(task2);

		Task task3 = taskService.newTask();
		task3.Name = "aTaskName";
		task3.Description = "aTaskDescription";
		task3.Owner = "aTaskOwner";
		taskService.saveTask(task3);

		Task task4 = taskService.newTask();
		task4.Name = "aTaskName";
		task4.Description = "aTaskDescription";
		task4.Owner = "aTaskOwner";
		task4.Assignee = "aTaskAssignee";
		taskService.saveTask(task4);

		Task task5 = taskService.newTask();
		task5.Name = "aTaskName";
		task5.Description = "aTaskDescription";
		task5.Owner = "aTaskOwner";
		task5.Assignee = "aTaskAssignee";
		task5.Priority = 4;
		taskService.saveTask(task5);

		Task task6 = taskService.newTask();
		task6.Name = "aTaskName";
		task6.Description = "aTaskDescription";
		task6.Owner = "aTaskOwner";
		task6.Assignee = "aTaskAssignee";
		task6.Priority = 4;
		taskService.saveTask(task6);

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskName("aTaskName").taskDescription("aTaskDescription").endOr().or().taskName("aTaskName").taskDescription("aTaskDescription").taskAssignee("aTaskAssignee").endOr().or().taskName("aTaskName").taskDescription("aTaskDescription").taskOwner("aTaskOwner").taskAssignee("aTaskAssignee").endOr().or().taskAssignee("aTaskAssignee").taskPriority(4).endOr().list();

		// then
		assertEquals(3, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWhereSameCriterionWasAppliedThreeTimesInOneQuery()
	  public virtual void shouldReturnTasksWhereSameCriterionWasAppliedThreeTimesInOneQuery()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.addCandidateGroup(task1.Id, "Accounting");

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.addCandidateGroup(task2.Id, "Controlling");

		Task task3 = taskService.newTask();
		taskService.saveTask(task3);
		taskService.addCandidateGroup(task3.Id, "Sales");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().taskCandidateGroup("Accounting").taskCandidateGroup("Controlling").taskCandidateGroup("Sales").endOr().list();

		// then
		assertEquals(1, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithTaskVariableValueEqualsOrTaskVariableValueGreaterThan()
	  public virtual void shouldReturnTasksWithTaskVariableValueEqualsOrTaskVariableValueGreaterThan()
	  {
		// given
		Task task1 = taskService.newTask();
		taskService.saveTask(task1);
		taskService.setVariable(task1.Id,"aLongValue", 789L);

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);
		taskService.setVariable(task2.Id,"anEvenLongerValue", 1000L);

		// when
		TaskQuery query = taskService.createTaskQuery().or().taskVariableValueEquals("aLongValue", 789L).taskVariableValueGreaterThan("anEvenLongerValue", 999L).endOr();

		// then
		assertEquals(2, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldInitializeFormKeys()
	  public virtual void shouldInitializeFormKeys()
	  {
		// given
		BpmnModelInstance aProcessDefinition = Bpmn.createExecutableProcess("aProcessDefinition").startEvent().userTask().camundaFormKey("aFormKey").endEvent().done();

		repositoryService.createDeployment().addModelInstance("foo.bpmn", aProcessDefinition).deploy();

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("aProcessDefinition");

		BpmnModelInstance anotherProcessDefinition = Bpmn.createExecutableProcess("anotherProcessDefinition").startEvent().userTask().camundaFormKey("anotherFormKey").endEvent().done();

		repositoryService.createDeployment().addModelInstance("foo.bpmn", anotherProcessDefinition).deploy();

		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("anotherProcessDefinition");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().processDefinitionId(processInstance1.ProcessDefinitionId).processInstanceId(processInstance2.Id).endOr().initializeFormKeys().list();

		// then
		assertEquals(2, tasks.Count);
		assertEquals("aFormKey", tasks[0].FormKey);
		assertEquals("anotherFormKey", tasks[1].FormKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithProcessDefinitionNameOrProcessDefinitionKey()
	  public virtual void shouldReturnTasksWithProcessDefinitionNameOrProcessDefinitionKey()
	  {
		// given
		BpmnModelInstance aProcessDefinition = Bpmn.createExecutableProcess("aProcessDefinition").name("process1").startEvent().userTask().endEvent().done();

		repositoryService.createDeployment().addModelInstance("foo.bpmn", aProcessDefinition).deploy();

		runtimeService.startProcessInstanceByKey("aProcessDefinition");

		BpmnModelInstance anotherProcessDefinition = Bpmn.createExecutableProcess("anotherProcessDefinition").startEvent().userTask().endEvent().done();

		 repositoryService.createDeployment().addModelInstance("foo.bpmn", anotherProcessDefinition).deploy();

		runtimeService.startProcessInstanceByKey("anotherProcessDefinition");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().processDefinitionName("process1").processDefinitionKey("anotherProcessDefinition").endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithProcessInstanceBusinessKeyOrProcessInstanceBusinessKeyLike()
	  public virtual void shouldReturnTasksWithProcessInstanceBusinessKeyOrProcessInstanceBusinessKeyLike()
	  {
		// given
		BpmnModelInstance aProcessDefinition = Bpmn.createExecutableProcess("aProcessDefinition").startEvent().userTask().endEvent().done();

		repositoryService.createDeployment().addModelInstance("foo.bpmn", aProcessDefinition).deploy();

		runtimeService.startProcessInstanceByKey("aProcessDefinition", "aBusinessKey");

		BpmnModelInstance anotherProcessDefinition = Bpmn.createExecutableProcess("anotherProcessDefinition").startEvent().userTask().endEvent().done();

		 repositoryService.createDeployment().addModelInstance("foo.bpmn", anotherProcessDefinition).deploy();

		runtimeService.startProcessInstanceByKey("anotherProcessDefinition", "anotherBusinessKey");

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().processInstanceBusinessKey("aBusinessKey").processInstanceBusinessKeyLike("anotherBusinessKey").endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={ "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase2.cmmn"}) public void shouldReturnTasksWithCaseDefinitionKeyCaseDefinitionName()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase2.cmmn"})]
	  public virtual void shouldReturnTasksWithCaseDefinitionKeyCaseDefinitionName()
	  {
		// given
		string caseDefinitionId1 = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId1).create();

		string caseDefinitionId2 = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase2").singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId2).create();

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().caseDefinitionKey("oneTaskCase").caseDefinitionName("One").endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={ "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase2.cmmn"}) public void shouldReturnTasksWithCaseInstanceBusinessKeyOrCaseInstanceBusinessKeyLike()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase2.cmmn"})]
	  public virtual void shouldReturnTasksWithCaseInstanceBusinessKeyOrCaseInstanceBusinessKeyLike()
	  {
		// given
		string caseDefinitionId1 = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").singleResult().Id;

		CaseInstance caseInstance1 = caseService.withCaseDefinition(caseDefinitionId1).businessKey("aBusinessKey").create();

		string caseDefinitionId2 = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase2").singleResult().Id;

		CaseInstance caseInstance2 = caseService.withCaseDefinition(caseDefinitionId2).businessKey("anotherBusinessKey").create();

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().caseInstanceBusinessKey(caseInstance1.BusinessKey).caseInstanceBusinessKeyLike(caseInstance2.BusinessKey).endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore("CAM-9114") @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) public void shouldReturnTasksWithCaseInstanceBusinessKeyOrProcessInstanceBusinessKey()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void shouldReturnTasksWithCaseInstanceBusinessKeyOrProcessInstanceBusinessKey()
	  {
		string businessKey = "aBusinessKey";

		BpmnModelInstance aProcessDefinition = Bpmn.createExecutableProcess("aProcessDefinition").startEvent().userTask().endEvent().done();

		repositoryService.createDeployment().addModelInstance("foo.bpmn", aProcessDefinition).deploy();

		runtimeService.startProcessInstanceByKey("aProcessDefinition", businessKey);

		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).businessKey(businessKey).create();

		TaskQuery query = taskService.createTaskQuery();

		query.processInstanceBusinessKey(businessKey).or().caseInstanceBusinessKey(businessKey).processInstanceBusinessKey(businessKey).endOr();

		assertEquals(2, query.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksWithActivityInstanceIdInOrTaskId()
	  public virtual void shouldReturnTasksWithActivityInstanceIdInOrTaskId()
	  {
		// given
		BpmnModelInstance aProcessDefinition = Bpmn.createExecutableProcess("aProcessDefinition").startEvent().userTask().endEvent().done();

		repositoryService.createDeployment().addModelInstance("foo.bpmn", aProcessDefinition).deploy();

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("aProcessDefinition");

		string activityInstanceId = runtimeService.getActivityInstance(processInstance1.Id).ChildActivityInstances[0].Id;

		Task task2 = taskService.newTask();
		taskService.saveTask(task2);

		// when
		IList<Task> tasks = taskService.createTaskQuery().or().activityInstanceIdIn(activityInstanceId).taskId(task2.Id).endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksByExtendingQuery_OrInExtendingQuery()
	  public virtual void shouldReturnTasksByExtendingQuery_OrInExtendingQuery()
	  {
		// given
		TaskQuery extendedQuery = taskService.createTaskQuery().taskCandidateGroup("sales");

		TaskQuery extendingQuery = taskService.createTaskQuery().or().taskName("aTaskName").endOr().or().taskNameLike("anotherTaskName").endOr();

		// when
		TaskQueryImpl result = (TaskQueryImpl)((TaskQueryImpl)extendedQuery).extend(extendingQuery);

		// then
		assertEquals("sales", result.CandidateGroup);
		assertEquals("aTaskName", result.Queries[1].Name);
		assertEquals("anotherTaskName", result.Queries[2].NameLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksByExtendingQuery_OrInExtendedQuery()
	  public virtual void shouldReturnTasksByExtendingQuery_OrInExtendedQuery()
	  {
		// given
		TaskQuery extendedQuery = taskService.createTaskQuery().or().taskName("aTaskName").endOr().or().taskNameLike("anotherTaskName").endOr();

		TaskQuery extendingQuery = taskService.createTaskQuery().taskCandidateGroup("aCandidateGroup");

		// when
		TaskQueryImpl result = (TaskQueryImpl)((TaskQueryImpl)extendedQuery).extend(extendingQuery);

		// then
		assertEquals("aTaskName", result.Queries[1].Name);
		assertEquals("anotherTaskName", result.Queries[2].NameLike);
		assertEquals("aCandidateGroup", result.CandidateGroup);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksByExtendingQuery_OrInBothExtendedAndExtendingQuery()
	  public virtual void shouldReturnTasksByExtendingQuery_OrInBothExtendedAndExtendingQuery()
	  {
		// given
		TaskQuery extendedQuery = taskService.createTaskQuery().or().taskName("aTaskName").endOr().or().taskNameLike("anotherTaskName").endOr();

		TaskQuery extendingQuery = taskService.createTaskQuery().or().taskCandidateGroup("aCandidateGroup").endOr().or().taskCandidateUser("aCandidateUser").endOr();

		// when
		TaskQueryImpl result = (TaskQueryImpl)((TaskQueryImpl)extendedQuery).extend(extendingQuery);

		// then
		assertEquals("aTaskName", result.Queries[1].Name);
		assertEquals("anotherTaskName", result.Queries[2].NameLike);
		assertEquals("aCandidateGroup", result.Queries[3].CandidateGroup);
		assertEquals("aCandidateUser", result.Queries[4].CandidateUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldTestDueDateCombinations() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldTestDueDateCombinations()
	  {
		Dictionary<string, DateTime> dates = createFollowUpAndDueDateTasks();

		assertEquals(2, taskService.createTaskQuery().or().dueDate(dates["date"]).dueBefore(dates["oneHourAgo"]).endOr().count());

		assertEquals(2, taskService.createTaskQuery().or().dueDate(dates["date"]).dueAfter(dates["oneHourLater"]).endOr().count());

		assertEquals(2, taskService.createTaskQuery().or().dueBefore(dates["oneHourAgo"]).dueAfter(dates["oneHourLater"]).endOr().count());

		assertEquals(3, taskService.createTaskQuery().or().dueBefore(dates["oneHourLater"]).dueAfter(dates["oneHourAgo"]).endOr().count());

		assertEquals(3, taskService.createTaskQuery().or().dueDate(dates["date"]).dueBefore(dates["oneHourAgo"]).dueAfter(dates["oneHourLater"]).endOr().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldTestFollowUpDateCombinations() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldTestFollowUpDateCombinations()
	  {
		Dictionary<string, DateTime> dates = createFollowUpAndDueDateTasks();

		assertEquals(2, taskService.createTaskQuery().or().followUpDate(dates["date"]).followUpBefore(dates["oneHourAgo"]).endOr().count());

		assertEquals(2, taskService.createTaskQuery().or().followUpDate(dates["date"]).followUpAfter(dates["oneHourLater"]).endOr().count());

		assertEquals(2, taskService.createTaskQuery().or().followUpBefore(dates["oneHourAgo"]).followUpAfter(dates["oneHourLater"]).endOr().count());

		assertEquals(3, taskService.createTaskQuery().or().followUpBefore(dates["oneHourLater"]).followUpAfter(dates["oneHourAgo"]).endOr().count());

		assertEquals(3, taskService.createTaskQuery().or().followUpDate(dates["date"]).followUpBefore(dates["oneHourAgo"]).followUpAfter(dates["oneHourLater"]).endOr().count());

		// followUp before or null
		taskService.saveTask(taskService.newTask());

		assertEquals(4, taskService.createTaskQuery().count());

		assertEquals(3, taskService.createTaskQuery().or().followUpDate(dates["date"]).followUpBeforeOrNotExistent(dates["oneHourAgo"]).endOr().count());

		assertEquals(3, taskService.createTaskQuery().or().followUpBeforeOrNotExistent(dates["oneHourAgo"]).followUpAfter(dates["oneHourLater"]).endOr().count());

		assertEquals(4, taskService.createTaskQuery().or().followUpBeforeOrNotExistent(dates["oneHourLater"]).followUpAfter(dates["oneHourAgo"]).endOr().count());

		assertEquals(4, taskService.createTaskQuery().or().followUpDate(dates["date"]).followUpBeforeOrNotExistent(dates["oneHourAgo"]).followUpAfter(dates["oneHourLater"]).endOr().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTasksByVariableAndActiveProcesses() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldReturnTasksByVariableAndActiveProcesses()
	  {
		// given
		BpmnModelInstance aProcessDefinition = Bpmn.createExecutableProcess("oneTaskProcess").startEvent().userTask("testQuerySuspensionStateTask").endEvent().done();

		  repositoryService.createDeployment().addModelInstance("foo.bpmn", aProcessDefinition).deploy();

		// start two process instance and leave them active
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// start one process instance and suspend it
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = 0;
		ProcessInstance suspendedProcessInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		runtimeService.suspendProcessInstanceById(suspendedProcessInstance.ProcessInstanceId);

		// assume
		assertEquals(2, taskService.createTaskQuery().taskDefinitionKey("testQuerySuspensionStateTask").active().count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("testQuerySuspensionStateTask").suspended().count());

		// then
		assertEquals(3, taskService.createTaskQuery().or().active().processVariableValueEquals("foo", 0).endOr().list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.HashMap<String, java.util.Date> createFollowUpAndDueDateTasks() throws java.text.ParseException
	  public virtual Dictionary<string, DateTime> createFollowUpAndDueDateTasks()
	  {
		const DateTime date = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("27/07/2017 01:12:13"), oneHourAgo = new DateTime(date.Ticks - 60 * 60 * 1000), oneHourLater = new DateTime(date.Ticks + 60 * 60 * 1000);

		Task taskDueBefore = taskService.newTask();
		taskDueBefore.FollowUpDate = new DateTime(oneHourAgo.Ticks - 1000);
		taskDueBefore.DueDate = new DateTime(oneHourAgo.Ticks - 1000);
		taskService.saveTask(taskDueBefore);

		Task taskDueDate = taskService.newTask();
		taskDueDate.FollowUpDate = date;
		taskDueDate.DueDate = date;
		taskService.saveTask(taskDueDate);

		Task taskDueAfter = taskService.newTask();
		taskDueAfter.FollowUpDate = new DateTime(oneHourLater.Ticks + 1000);
		taskDueAfter.DueDate = new DateTime(oneHourLater.Ticks + 1000);
		taskService.saveTask(taskDueAfter);

		assertEquals(3, taskService.createTaskQuery().count());

		return new HashMapAnonymousInnerClass(this, date, oneHourAgo, oneHourLater);
	  }

	  private class HashMapAnonymousInnerClass : Dictionary<string, DateTime>
	  {
		  private readonly TaskQueryOrTest outerInstance;

		  private DateTime date;
		  private DateTime oneHourAgo;
		  private DateTime oneHourLater;

		  public HashMapAnonymousInnerClass(TaskQueryOrTest outerInstance, DateTime date, DateTime oneHourAgo, DateTime oneHourLater)
		  {
			  this.outerInstance = outerInstance;
			  this.date = date;
			  this.oneHourAgo = oneHourAgo;
			  this.oneHourLater = oneHourLater;

			  this.put("date", date);
			  this.put("oneHourAgo", oneHourAgo);
			  this.put("oneHourLater", oneHourLater);
		  }

	  }

	}

}