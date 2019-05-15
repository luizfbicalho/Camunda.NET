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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ASSIGN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CLAIM;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELEGATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RESOLVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_OWNER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.ASSIGNEE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.DELEGATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.OWNER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.PRIORITY;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogTaskTest : AbstractUserOperationLogTest
	{

	  protected internal ProcessDefinition processDefinition;
	  protected internal ProcessInstance process;
	  protected internal Task task;

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testCreateAndCompleteTask()
	  {
		startTestProcess();

		// expect: one entry for process instance creation,
		//         no entry for the task creation by process engine
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(1, query.count());

		completeTestProcess();

		// expect: one entry for the task completion
		query = queryOperationDetails(OPERATION_TYPE_COMPLETE);
		assertEquals(1, query.count());
		UserOperationLogEntry complete = query.singleResult();
		assertEquals(DELETE, complete.Property);
		assertTrue(bool.Parse(complete.NewValue));
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, complete.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testAssignTask()
	  {
		startTestProcess();

		// then: assign the task
		taskService.setAssignee(task.Id, "icke");

		// expect: one entry for the task assignment
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_ASSIGN);
		assertEquals(1, query.count());

		// assert: details
		UserOperationLogEntry assign = query.singleResult();
		assertEquals(ASSIGNEE, assign.Property);
		assertEquals("icke", assign.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, assign.Category);

		completeTestProcess();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testChangeTaskOwner()
	  {
		startTestProcess();

		// then: change the task owner
		taskService.setOwner(task.Id, "icke");

		// expect: one entry for the owner change
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_SET_OWNER);
		assertEquals(1, query.count());

		// assert: details
		UserOperationLogEntry change = query.singleResult();
		assertEquals(OWNER, change.Property);
		assertEquals("icke", change.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, change.Category);

		completeTestProcess();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSetPriority()
	  {
		startTestProcess();

		// then: set the priority of the task to 10
		taskService.setPriority(task.Id, 10);

		// expect: one entry for the priority update
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_SET_PRIORITY);
		assertEquals(1, query.count());

		// assert: correct priority set
		UserOperationLogEntry userOperationLogEntry = query.singleResult();
		assertEquals(PRIORITY, userOperationLogEntry.Property);
		// note: 50 is the default task priority
		assertEquals(50, int.Parse(userOperationLogEntry.OrgValue));
		assertEquals(10, int.Parse(userOperationLogEntry.NewValue));
		// assert: correct category set
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, userOperationLogEntry.Category);

		// move clock by 5 minutes
		DateTime date = DateTimeUtil.now().plusMinutes(5).toDate();
		ClockUtil.CurrentTime = date;

		// then: set priority again
		taskService.setPriority(task.Id, 75);

		// expect: one entry for the priority update
		query = queryOperationDetails(OPERATION_TYPE_SET_PRIORITY);
		assertEquals(2, query.count());

		// assert: correct priority set
		userOperationLogEntry = query.orderByTimestamp().asc().list().get(1);
		assertEquals(PRIORITY, userOperationLogEntry.Property);
		assertEquals(10, int.Parse(userOperationLogEntry.OrgValue));
		assertEquals(75, int.Parse(userOperationLogEntry.NewValue));
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, userOperationLogEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testClaimTask()
	  {
		startTestProcess();

		// then: claim a new the task
		taskService.claim(task.Id, "icke");

		// expect: one entry for the claim
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_CLAIM);
		assertEquals(1, query.count());

		// assert: details
		UserOperationLogEntry claim = query.singleResult();
		assertEquals(ASSIGNEE, claim.Property);
		assertEquals("icke", claim.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, claim.Category);

		completeTestProcess();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDelegateTask()
	  {
		startTestProcess();

		// then: delegate the assigned task
		taskService.claim(task.Id, "icke");
		taskService.delegateTask(task.Id, "er");

		// expect: three entries for the delegation
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_DELEGATE);
		assertEquals(3, query.count());

		// assert: details
		assertEquals("icke", queryOperationDetails(OPERATION_TYPE_DELEGATE, OWNER).singleResult().NewValue);
		assertEquals("er", queryOperationDetails(OPERATION_TYPE_DELEGATE, ASSIGNEE).singleResult().NewValue);
		assertEquals(DelegationState.PENDING.ToString(), queryOperationDetails(OPERATION_TYPE_DELEGATE, DELEGATION).singleResult().NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, queryOperationDetails(OPERATION_TYPE_DELEGATE, DELEGATION).singleResult().Category);

		completeTestProcess();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testResolveTask()
	  {
		startTestProcess();

		// then: resolve the task
		taskService.resolveTask(task.Id);

		// expect: one entry for the resolving
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_RESOLVE);
		assertEquals(1, query.count());

		// assert: details
		UserOperationLogEntry log = query.singleResult();
		assertEquals(DelegationState.RESOLVED.ToString(), log.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, log.Category);

		completeTestProcess();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskForm_Complete()
	  {
		startTestProcess();

		formService.submitTaskForm(task.Id, new Dictionary<string, object>());

		// expect: one entry for the completion
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_COMPLETE);
		assertEquals(1, query.count());

		// assert: delete
		UserOperationLogEntry log = query.property("delete").singleResult();
		assertFalse(bool.Parse(log.OrgValue));
		assertTrue(bool.Parse(log.NewValue));
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, log.Category);

		assertProcessEnded(process.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskForm_Resolve()
	  {
		startTestProcess();

		taskService.delegateTask(task.Id, "demo");

		formService.submitTaskForm(task.Id, new Dictionary<string, object>());

		// expect: two entries for the resolving (delegation and assignee changed)
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_RESOLVE);
		assertEquals(2, query.count());

		// assert: delegation
		UserOperationLogEntry log = query.property("delegation").singleResult();
		assertEquals(DelegationState.PENDING.ToString(), log.OrgValue);
		assertEquals(DelegationState.RESOLVED.ToString(), log.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, log.Category);

		// assert: assignee
		log = query.property("assignee").singleResult();
		assertEquals("demo", log.OrgValue);
		assertEquals(null, log.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, log.Category);

		completeTestProcess();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCompleteCaseExecution()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_COMPLETE);

		assertEquals(1, query.count());

		UserOperationLogEntry entry = query.singleResult();
		assertNotNull(entry);

		assertEquals(caseDefinitionId, entry.CaseDefinitionId);
		assertEquals(caseInstanceId, entry.CaseInstanceId);
		assertEquals(humanTaskId, entry.CaseExecutionId);
		assertEquals(deploymentId, entry.DeploymentId);

		assertFalse(Convert.ToBoolean(entry.OrgValue));
		assertTrue(Convert.ToBoolean(entry.NewValue));
		assertEquals(DELETE, entry.Property);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, entry.Category);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testKeepOpLogEntriesOnUndeployment()
	  {
		// given
		startTestProcess();
		// an op log entry directly related to the process instance is created
		taskService.resolveTask(task.Id);

		// and an op log entry with indirect reference to the process instance is created
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		// when
		// the deployment is deleted with cascade
		repositoryService.deleteDeployment(deploymentId, true);

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(4, query.count());
		assertEquals(1, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE).count());
		assertEquals(1, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND).count());
		assertEquals(1, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RESOLVE).count());
		assertEquals(1, query.operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteOpLogEntry()
	  {
		// given
		startTestProcess();

		// an op log instance is created
		taskService.resolveTask(task.Id);
		UserOperationLogEntry opLogEntry = historyService.createUserOperationLogQuery().entityType(EntityTypes.TASK).singleResult();

		// when the op log instance is deleted
		historyService.deleteUserOperationLogEntry(opLogEntry.Id);

		// then it should be removed from the database
		assertEquals(0, historyService.createUserOperationLogQuery().entityType(EntityTypes.TASK).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteOpLogEntryWithNullArgument()
	  {
		// given
		startTestProcess();

		// an op log instance is created
		taskService.resolveTask(task.Id);

		// when null is used as deletion parameter
		try
		{
		  historyService.deleteUserOperationLogEntry(null);
		  fail("exeception expected");
		}
		catch (NotValidException)
		{
		  // then there should be an exception that signals an illegal input
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDeleteOpLogNonExstingEntry()
	  {
		// given
		startTestProcess();

		// an op log instance is created
		taskService.resolveTask(task.Id);
		assertEquals(2, historyService.createUserOperationLogQuery().count());

		// when a non-existing id is used
		historyService.deleteUserOperationLogEntry("a non existing id");

		// then no op log entry should have been deleted (process instance creation+ resolve task)
		assertEquals(2, historyService.createUserOperationLogQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOnlyTaskCompletionIsLogged()
	  public virtual void testOnlyTaskCompletionIsLogged()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		// then
		assertTrue((bool?) runtimeService.getVariable(processInstanceId, "taskListenerCalled"));
		assertTrue((bool?) runtimeService.getVariable(processInstanceId, "serviceTaskCalled"));

		// Filter only task entities, as the process start is also recorded
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().entityType(EntityTypes.TASK);

		assertEquals(1, query.count());

		UserOperationLogEntry log = query.singleResult();
		assertEquals("process", log.ProcessDefinitionKey);
		assertEquals(processInstanceId, log.ProcessInstanceId);
		assertEquals(deploymentId, log.DeploymentId);
		assertEquals(taskId, log.TaskId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE, log.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, log.Category);
	  }

	  protected internal virtual void startTestProcess()
	  {
		processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult();

		process = runtimeService.startProcessInstanceById(processDefinition.Id);
		task = taskService.createTaskQuery().singleResult();
	  }

	  protected internal virtual UserOperationLogQuery queryOperationDetails(string type)
	  {
		return historyService.createUserOperationLogQuery().operationType(type);
	  }

	  protected internal virtual UserOperationLogQuery queryOperationDetails(string type, string property)
	  {
		return historyService.createUserOperationLogQuery().operationType(type).property(property);
	  }

	  protected internal virtual void completeTestProcess()
	  {
		taskService.complete(task.Id);
		assertProcessEnded(process.Id);
	  }

	}

}