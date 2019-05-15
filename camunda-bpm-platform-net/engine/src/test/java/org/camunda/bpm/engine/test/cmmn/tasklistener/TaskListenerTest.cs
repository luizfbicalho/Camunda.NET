using System;

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
namespace org.camunda.bpm.engine.test.cmmn.tasklistener
{
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using FieldInjectionTaskListener = org.camunda.bpm.engine.test.cmmn.tasklistener.util.FieldInjectionTaskListener;
	using MySpecialTaskListener = org.camunda.bpm.engine.test.cmmn.tasklistener.util.MySpecialTaskListener;
	using MyTaskListener = org.camunda.bpm.engine.test.cmmn.tasklistener.util.MyTaskListener;
	using NotTaskListener = org.camunda.bpm.engine.test.cmmn.tasklistener.util.NotTaskListener;
	using TaskDeleteListener = org.camunda.bpm.engine.test.cmmn.tasklistener.util.TaskDeleteListener;
	using Rule = org.junit.Rule;
	using ExpectedException = org.junit.rules.ExpectedException;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.contains;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskListenerTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCreateListenerByClass.cmmn"})]
	  public virtual void testCreateListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(3, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCreateListenerByExpression.cmmn"})]
	  public virtual void testCreateListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MyTaskListener()).create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCreateListenerByDelegateExpression.cmmn"})]
	  public virtual void testCreateListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MySpecialTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCreateListenerByScript.cmmn"})]
	  public virtual void testCreateListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(2, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCompleteListenerByClass.cmmn"})]
	  public virtual void testCompleteListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(3, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCompleteListenerByExpression.cmmn"})]
	  public virtual void testCompleteListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MyTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCompleteListenerByDelegateExpression.cmmn"})]
	  public virtual void testCompleteListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MySpecialTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testCompleteListenerByScript.cmmn"})]
	  public virtual void testCompleteListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(2, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDeleteListenerByClass.cmmn"})]
	  public virtual void testDeleteListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(3, query.count());

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDeleteListenerByExpression.cmmn"})]
	  public virtual void testDeleteListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MyTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDeleteListenerByDelegateExpression.cmmn"})]
	  public virtual void testDeleteListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MySpecialTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDeleteListenerByScript.cmmn"})]
	  public virtual void testDeleteListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(2, query.count());

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDeleteListenerByCaseInstanceDeletion.cmmn"})]
	  public virtual void testDeleteListenerByCaseInstanceDeletion()
	  {
		TaskDeleteListener.clear();

		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String caseInstanceId = caseService.withCaseDefinitionByKey("case").create().getId();
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, caseInstanceId));

		// then
		assertEquals(1, TaskDeleteListener.eventCounter);

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly TaskListenerTest outerInstance;

		  private string caseInstanceId;

		  public CommandAnonymousInnerClass(TaskListenerTest outerInstance, string caseInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.caseInstanceId = caseInstanceId;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseExecutionManager.deleteCaseInstance(caseInstanceId, null);
			return null;
		  }

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAssignmentListenerByClass.cmmn"})]
	  public virtual void testAssignmentListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		// when
		taskService.setAssignee(taskId, "jonny");

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(3, query.count());

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAssignmentListenerByExpression.cmmn"})]
	  public virtual void testAssignmentListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MyTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		// when
		taskService.setAssignee(taskId, "jonny");

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAssignmentListenerByDelegateExpression.cmmn"})]
	  public virtual void testAssignmentListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MySpecialTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		// when
		taskService.setAssignee(taskId, "jonny");

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAssignmentListenerByScript.cmmn"})]
	  public virtual void testAssignmentListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		// when
		taskService.setAssignee(taskId, "jonny");

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(2, query.count());

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAssignmentListenerByInitialInstantiation.cmmn"})]
	  public virtual void testAssignmentListenerByInitialInstantiation()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(3, query.count());

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByClass.cmmn"})]
	  public virtual void testAllListenerByClassExcludingDeletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(7, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByClass.cmmn"})]
	  public virtual void testAllListenerByClassExcludingCompletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(7, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByExpression.cmmn"})]
	  public virtual void testAllListenerByExpressionExcludingDeletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MyTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(8, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByExpression.cmmn"})]
	  public virtual void testAllListenerByExpressionExcludingCompletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MyTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(8, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByDelegateExpression.cmmn"})]
	  public virtual void testAllListenerByDelegateExpressionExcludingDeletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MySpecialTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(8, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByDelegateExpression.cmmn"})]
	  public virtual void testAllListenerByDelegateExpressionExcludingCompletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new MySpecialTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(8, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByScript.cmmn"})]
	  public virtual void testAllListenerByScriptExcludingDeletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		//when
		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(7, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testAllListenerByScript.cmmn"})]
	  public virtual void testAllListenerByScriptExcludingCompletion()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(7, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("delete").singleResult().Value);
		assertEquals(1, query.variableName("deleteEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testFieldInjectionByClass.cmmn"})]
	  public virtual void testFieldInjectionByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertEquals("Hello from The Case", query.variableName("greeting").singleResult().Value);
		assertEquals("Hello World", query.variableName("helloWorld").singleResult().Value);
		assertEquals("cam", query.variableName("prefix").singleResult().Value);
		assertEquals("unda", query.variableName("suffix").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testFieldInjectionByDelegateExpression.cmmn"})]
	  public virtual void testFieldInjectionByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new FieldInjectionTaskListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertEquals("Hello from The Case", query.variableName("greeting").singleResult().Value);
		assertEquals("Hello World", query.variableName("helloWorld").singleResult().Value);
		assertEquals("cam", query.variableName("prefix").singleResult().Value);
		assertEquals("unda", query.variableName("suffix").singleResult().Value);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testListenerByScriptResource.cmmn", "org/camunda/bpm/engine/test/cmmn/tasklistener/taskListener.groovy" })]
	  public virtual void testListenerByScriptResource()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		string taskId = taskService.createTaskQuery().caseExecutionId(humanTaskId).singleResult().Id;

		taskService.setAssignee(taskId, "jonny");

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(7, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("assignment").singleResult().Value);
		assertEquals(1, query.variableName("assignmentEventCounter").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDoesNotImplementTaskListenerInterfaceByClass.cmmn"})]
	  public virtual void testDoesNotImplementTaskListenerInterfaceByClass()
	  {
		try
		{
		  caseService.withCaseDefinitionByKey("case").create().Id;
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  // then
		  Exception cause = e.InnerException;
		  string message = cause.Message;
		  assertTextPresent("NotTaskListener doesn't implement " + typeof(TaskListener), message);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testDoesNotImplementTaskListenerInterfaceByDelegateExpression.cmmn"})]
	  public virtual void testDoesNotImplementTaskListenerInterfaceByDelegateExpression()
	  {
		try
		{
		  caseService.withCaseDefinitionByKey("case").setVariable("myTaskListener", new NotTaskListener()).create().Id;
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  // then
		  Exception cause = e.InnerException;
		  string message = cause.Message;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertTextPresent("Delegate expression ${myTaskListener} did not resolve to an implementation of interface " + typeof(TaskListener).FullName, message);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/tasklistener/TaskListenerTest.testTaskListenerDoesNotExist.cmmn"})]
	  public virtual void testTaskListenerDoesNotExist()
	  {

		try
		{
		  caseService.withCaseDefinitionByKey("case").create().Id;
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  // then
		  Exception cause = e.InnerException;
		  string message = cause.Message;
		  assertTextPresent("Exception while instantiating class 'org.camunda.bpm.engine.test.cmmn.tasklistener.util.NotExistingTaskListener'", message);
		}

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void terminate(final String caseExecutionId)
	  protected internal virtual void terminate(string caseExecutionId)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, caseExecutionId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly TaskListenerTest outerInstance;

		  private string caseExecutionId;

		  public CommandAnonymousInnerClass2(TaskListenerTest outerInstance, string caseExecutionId)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			CmmnExecution caseTask = (CmmnExecution) outerInstance.caseService.createCaseExecutionQuery().caseExecutionId(caseExecutionId).singleResult();
			caseTask.terminate();
			return null;
		  }

	  }

	}

}