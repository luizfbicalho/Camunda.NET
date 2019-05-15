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
namespace org.camunda.bpm.engine.test.bpmn.@event.conditional
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using AbstractActivityBuilder = org.camunda.bpm.model.bpmn.builder.AbstractActivityBuilder;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class BoundaryConditionalEventTest : AbstractConditionalEventTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testTrueCondition()
	  public virtual void testTrueCondition()
	  {
		//given process with boundary conditional event

		//when process is started and execution arrives user task with boundary event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then default evaluation behavior triggers boundary event
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testNonInterruptingTrueCondition()
	  public virtual void testNonInterruptingTrueCondition()
	  {
		//given process with boundary conditional event

		//when process is started and execution arrives activity with boundary event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then default evaluation behavior triggers conditional event
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testFalseCondition()
	  public virtual void testFalseCondition()
	  {
		//given process with boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery();
		Task task = taskQuery.processInstanceId(procInst.Id).singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when variable is set on task execution
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution stays in task with boundary condition
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_WITH_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testVariableCondition()
	  public virtual void testVariableCondition()
	  {
		//given process with boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when local variable is set on task with condition
		taskService.setVariableLocal(task.Id, VARIABLE_NAME, 1);

		//then execution should remain on task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_WITH_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testVariableCondition.bpmn20.xml"}) public void testVariableSetOnExecutionCondition()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testVariableCondition.bpmn20.xml"})]
	  public virtual void testVariableSetOnExecutionCondition()
	  {
		//given process with boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when variable is set on task execution
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution ends
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(TASK_WITH_CONDITION_ID).singleResult();
		assertNull(execution);

		//and execution is at user task after boundary event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testNonInterruptingVariableCondition()
	  public virtual void testNonInterruptingVariableCondition()
	  {
		//given process with boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when variable is set on task with condition
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is at user task after boundary event and in the task with the boundary event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testVariableCondition.bpmn20.xml"}) public void testWrongVariableCondition()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testVariableCondition.bpmn20.xml"})]
	  public virtual void testWrongVariableCondition()
	  {
		//given process with boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when wrong variable is set on task execution
		taskService.setVariable(task.Id, VARIABLE_NAME + 1, 1);

		//then execution stays at user task with condition
		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when correct variable is set
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is on user task after condition
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testParallelVariableCondition()
	  public virtual void testParallelVariableCondition()
	  {
		//given process with parallel user tasks and boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		IList<Task> tasks = taskQuery.list();
		assertEquals(2, tasks.Count);
		assertEquals(2, conditionEventSubscriptionQuery.list().Count);

		Task task = tasks[0];

		//when local variable is set on task
		taskService.setVariableLocal(task.Id, VARIABLE_NAME, 1);

		//then nothing happens
		tasks = taskQuery.list();
		assertEquals(2, tasks.Count);

		//when local variable is set on task execution
		runtimeService.setVariableLocal(task.ExecutionId, VARIABLE_NAME, 1);

		//then boundary event is triggered of this task and task ends (subscription is deleted)
		//other execution stays in other task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testParallelVariableCondition.bpmn20.xml"}) public void testParallelSetVariableOnTaskCondition()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testParallelVariableCondition.bpmn20.xml"})]
	  public virtual void testParallelSetVariableOnTaskCondition()
	  {
		//given process with parallel user tasks and boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		IList<Task> tasks = taskQuery.list();
		assertEquals(2, tasks.Count);

		Task task = tasks[0];

		//when variable is set on execution
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then both boundary event are triggered and process instance ends
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).list();
		assertEquals(0, executions.Count);

		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testParallelVariableCondition.bpmn20.xml"}) public void testParallelSetVariableOnExecutionCondition()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testParallelVariableCondition.bpmn20.xml"})]
	  public virtual void testParallelSetVariableOnExecutionCondition()
	  {
		//given process with parallel user tasks and boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		IList<Task> tasks = taskQuery.list();
		assertEquals(2, tasks.Count);

		//when variable is set on execution
		//taskService.setVariable(task.getId(), VARIABLE_NAME, 1);
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then both boundary events are triggered and process instance ends
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).list();
		assertEquals(0, executions.Count);

		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSubProcessVariableCondition()
	  public virtual void testSubProcessVariableCondition()
	  {
		//given process with boundary conditional event on sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_IN_SUB_PROCESS, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when local variable is set on task with condition
		taskService.setVariableLocal(task.Id, VARIABLE_NAME, 1);

		//then execution stays on user task
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).list();
		assertEquals(2, executions.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when local variable is set on task execution
		runtimeService.setVariableLocal(task.ExecutionId, VARIABLE_NAME, 1);

		//then process instance ends
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"}) public void testSubProcessSetVariableOnTaskCondition()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"})]
	  public virtual void testSubProcessSetVariableOnTaskCondition()
	  {
		//given process with boundary conditional event on sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_IN_SUB_PROCESS, task.Name);

		//when variable is set on task execution with condition
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then process instance ends
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"}) public void testSubProcessSetVariableOnExecutionCondition()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/BoundaryConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"})]
	  public virtual void testSubProcessSetVariableOnExecutionCondition()
	  {
		//given process with boundary conditional event on sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_IN_SUB_PROCESS, task.Name);

		//when variable is set on task execution with condition
		runtimeService.setVariable(task.ExecutionId, VARIABLE_NAME, 1);

		//then process instance ends
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testNonInterruptingSubProcessVariableCondition()
	  public virtual void testNonInterruptingSubProcessVariableCondition()
	  {
		//given process with boundary conditional event on sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_IN_SUB_PROCESS, task.Name);

		//when variable is set on task with condition
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution stays on user task and at task after condition
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testCleanUpConditionalEventSubscriptions()
	  public virtual void testCleanUpConditionalEventSubscriptions()
	  {
		//given process with boundary conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when task is completed
		taskService.complete(task.Id);

		//then conditional subscription should be deleted
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

	  protected internal virtual void deployBoundaryEventProcess(AbstractActivityBuilder builder, bool isInterrupting)
	  {
		deployBoundaryEventProcess(builder, CONDITION_EXPR, isInterrupting);
	  }

	  protected internal virtual void deployBoundaryEventProcess(AbstractActivityBuilder builder, string conditionExpr, bool isInterrupting)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = builder.boundaryEvent().cancelActivity(isInterrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(conditionExpr).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = builder.boundaryEvent().cancelActivity(isInterrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(conditionExpr).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInDelegate()
	  public virtual void testSetVariableInDelegate()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaClass(SetVariableDelegate.class.getName()).endEvent().done();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaClass(typeof(SetVariableDelegate).FullName).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task with delegated code is called and variable is set
		//-> conditional event is triggered and execution stays at user task after condition
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInDelegate()
	  public virtual void testNonInterruptingSetVariableInDelegate()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaClass(SetVariableDelegate.class.getName()).userTask().endEvent().done();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaClass(typeof(SetVariableDelegate).FullName).userTask().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then service task with delegated code is called and variable is set
		//-> non interrupting conditional event is triggered
		//execution stays at user task after condition and after service task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInDelegateWithSynchronousEvent()
	  public virtual void testSetVariableInDelegateWithSynchronousEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaClass(typeof(SetVariableDelegate).FullName).endEvent().done();

		modelInstance = modify(modelInstance).serviceTaskBuilder(TASK_WITH_CONDITION_ID).boundaryEvent().cancelActivity(true).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then service task with delegated code is called and variable is set
		//-> conditional event is triggered and process instance ends
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.Count);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInDelegateWithSynchronousEvent()
	  public virtual void testNonInterruptingSetVariableInDelegateWithSynchronousEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaClass(typeof(SetVariableDelegate).FullName).userTask().endEvent().done();

		modelInstance = modify(modelInstance).serviceTaskBuilder(TASK_WITH_CONDITION_ID).boundaryEvent().cancelActivity(false).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given process with event sub process conditional start event and service task with delegate class which sets a variable
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before service task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then service task with delegated code is called and variable is set
		//-> non interrupting conditional event is triggered
		//execution stays at user task after service task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInInputMapping()
	  public virtual void testSetVariableInInputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		// input mapping does trigger boundary event with help of default evaluation behavior and process ends regularly
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInInputMapping()
	  public virtual void testNonInterruptingSetVariableInInputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		// then the variable is set in an input mapping
		// -> non interrupting conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_SERVICE_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInExpression()
	  public virtual void testSetVariableInExpression()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression(EXPR_SET_VARIABLE).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression(EXPR_SET_VARIABLE).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task with expression is called and variable is set
		//-> interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInExpression()
	  public virtual void testNonInterruptingSetVariableInExpression()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then service task with expression is called and variable is set
		//->non interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInInputMappingOfSubProcess()
	  public virtual void testSetVariableInInputMappingOfSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		// Then input mapping from sub process sets variable,
		// interrupting conditional event is triggered by default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInInputMappingOfSubProcess()
	  public virtual void testNonInterruptingSetVariableInInputMappingOfSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before is completed
		taskService.complete(task.Id);

		// Then input mapping from sub process sets variable, but
		// non interrupting conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_IN_SUB_PROCESS_ID, tasksAfterVariableIsSet[0].Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInStartListenerOfSubProcess()
	  public virtual void testSetVariableInStartListenerOfSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		// Then start listener from sub process sets variable,
		// interrupting conditional event is triggered by default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInStartListenerOfSubProcess()
	  public virtual void testNonInterruptingSetVariableInStartListenerOfSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before is completed
		taskService.complete(task.Id);

		// Then start listener from sub process sets variable,
		// non interrupting conditional event is triggered by default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutputMapping()
	  public virtual void testSetVariableInOutputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_BEFORE_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping sets variable
		//boundary event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_OUTPUT_MAPPING, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutputMappingWithBoundary()
	  public virtual void testSetVariableInOutputMappingWithBoundary()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping sets variable
		//boundary event is triggered by default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutputMapping()
	  public virtual void testNonInterruptingSetVariableInOutputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task with output mapping is completed
		taskService.complete(task.Id);

		//then output mapping sets variable
		//boundary event is triggered from default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutputMappingWithBoundary()
	  public virtual void testNonInterruptingSetVariableInOutputMappingWithBoundary()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when task with output mapping is completed
		taskService.complete(task.Id);

		//then output mapping sets variable
		//boundary event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_OUTPUT_MAPPING, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutputMappingOfCallActivity()
	  public virtual void testSetVariableInOutputMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping from call activity sets variable
		//-> interrupting conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_OUTPUT_MAPPING, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutputMappingOfCallActivity()
	  public virtual void testNonInterruptingSetVariableInOutputMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then out mapping of call activity sets a variable
		//-> non interrupting conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_OUTPUT_MAPPING, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutMappingOfCallActivity()
	  public virtual void testSetVariableInOutMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then out mapping from call activity sets variable
		//-> interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutMappingOfCallActivity()
	  public virtual void testNonInterruptingSetVariableInOutMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then out mapping of call activity sets a variable
		//-> non interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(0, conditionEventSubscriptionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInInMappingOfCallActivity()
	  public virtual void testSetVariableInInMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaIn(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaIn(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then in mapping from call activity sets variable
		//-> interrupting conditional event is not triggered, since variable is only locally
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_OUTPUT_MAPPING, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInInMappingOfCallActivity()
	  public virtual void testNonInterruptingSetVariableInInMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaIn(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaIn(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then in mapping from call activity sets variable
		//-> interrupting conditional event is not triggered, since variable is only locally
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_OUTPUT_MAPPING, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInStartListener()
	  public virtual void testSetVariableInStartListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//boundary event is triggered by default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInStartListener()
	  public virtual void testNonInterruptingSetVariableInStartListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//non interrupting boundary event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_WITH_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInTakeListener()
	  public virtual void testSetVariableInTakeListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then take listener sets variable
		//non interrupting boundary event is triggered with default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInTakeListener()
	  public virtual void testNonInterruptingSetVariableInTakeListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then take listener sets variable
		//non interrupting boundary event is triggered with default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInEndListener()
	  public virtual void testSetVariableInEndListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).userTask().name(AFTER_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).userTask().name(AFTER_TASK).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then end listener sets variable
		//conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(AFTER_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInEndListener()
	  public virtual void testNonInterruptingSetVariableInEndListener()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).userTask().name(AFTER_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).userTask().name(AFTER_TASK).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then end listener sets variable
		//non interrupting boundary event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(AFTER_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInMultiInstance()
	  public virtual void testSetVariableInMultiInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").parallel().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").parallel().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, "${nrOfInstances == 3}", true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then multi instance is created
		//and boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInMultiInstance()
	  public virtual void testNonInterruptingSetVariableInMultiInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").parallel().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").parallel().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, "${nrOfInstances == 3}", false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then multi instance is created
		//and boundary event is triggered for each multi instance creation
		IList<Task> multiInstanceTasks = taskQuery.taskDefinitionKey(TASK_WITH_CONDITION_ID).list();
		assertEquals(3, multiInstanceTasks.Count);
		assertEquals(3, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());

		//when multi instances are completed
		foreach (Task multiInstanceTask in multiInstanceTasks)
		{
		  taskService.complete(multiInstanceTask.Id);
		}

		//then non boundary events are triggered
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(9, tasksAfterVariableIsSet.Count);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInSeqMultiInstance()
	  public virtual void testSetVariableInSeqMultiInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").sequential().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").sequential().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, "${true}", true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then multi instance is created
		//and boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInSeqMultiInstance()
	  public virtual void testNonInterruptingSetVariableInSeqMultiInstance()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").sequential().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).multiInstance().cardinality("3").sequential().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, "${true}", false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then multi instance is created
		//and boundary event is triggered for each multi instance creation and also from the default evaluation behavior
		//since the condition is true. That means one time from the default behavior and 4 times for the variables which are set:
		//nrOfInstances, nrOfCompletedInstances, nrOfActiveInstances, loopCounter
		for (int i = 0; i < 3; i++)
		{
		  Task multiInstanceTask = taskQuery.taskDefinitionKey(TASK_WITH_CONDITION_ID).singleResult();
		  assertNotNull(multiInstanceTask);
		  assertEquals(i == 0 ? 5 : 5 + i * 2, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());
		  taskService.complete(multiInstanceTask.Id);
		}

		//then non boundary events are triggered 9 times
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(9, tasksAfterVariableIsSet.Count);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInCallActivity()
	  public virtual void testSetVariableInCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task in call activity sets variable
		//conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_SERVICE_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInCallActivity()
	  public virtual void testNonInterruptingSetVariableInCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task in call activity sets variable
		//conditional event is not triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_SERVICE_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInSubProcessInDelegatedCode()
	  public virtual void testSetVariableInSubProcessInDelegatedCode()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task in sub process sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInSubProcessInDelegatedCode()
	  public virtual void testNonInterruptingSetVariableInSubProcessInDelegatedCode()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		deployConditionalBoundaryEventProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task in sub process sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetMultipleVariables()
	  public virtual void testSetMultipleVariables()
	  {

		// given
		BpmnModelInstance modelInstance = modify(TASK_MODEL).userTaskBuilder(TASK_BEFORE_CONDITION_ID).boundaryEvent().cancelActivity(true).conditionalEventDefinition("event1").condition("${variable1 == 1}").conditionalEventDefinitionDone().userTask("afterBoundary1").endEvent().moveToActivity(TASK_BEFORE_CONDITION_ID).boundaryEvent().cancelActivity(true).conditionalEventDefinition("event2").condition("${variable2 == 1}").conditionalEventDefinitionDone().userTask("afterBoundary2").endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
		runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, Variables.createVariables().putValue("variable1", "44").putValue("variable2", "44"));
		Task task = taskService.createTaskQuery().singleResult();

		// when
		taskService.setVariables(task.Id, Variables.createVariables().putValue("variable1", 1).putValue("variable2", 1));

		// then
		assertEquals(1, taskService.createTaskQuery().count());
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		string taskDefinitionKey = tasksAfterVariableIsSet[0].TaskDefinitionKey;
		Assert.assertTrue("afterBoundary1".Equals(taskDefinitionKey) || "afterBoundary2".Equals(taskDefinitionKey));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testTrueConditionWithExecutionListener()
	  public virtual void testTrueConditionWithExecutionListener()
	  {
		//given process with boundary conditional event

		//when process is started and execution arrives activity with boundary event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then default evaluation behavior triggers conditional event
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendedProcess()
	  public virtual void testSuspendedProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).endEvent().done();

		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, true);

		// given suspended process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		runtimeService.suspendProcessInstanceById(procInst.Id);

		//when wrong variable is set
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME+1, 1);

		//then nothing happens
		assertTrue(runtimeService.createProcessInstanceQuery().singleResult().Suspended);

		//when variable which triggers condition is set
		//then exception is expected
		try
		{
		  runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);
		  fail("Should fail!");
		}
		catch (SuspendedEntityInteractionException)
		{
		  //expected
		}
		runtimeService.activateProcessInstanceById(procInst.Id);
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingConditionalSuspendedProcess()
	  public virtual void testNonInterruptingConditionalSuspendedProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).endEvent().done();

		deployConditionalBoundaryEventProcess(modelInstance, TASK_WITH_CONDITION_ID, false);

		// given suspended process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		runtimeService.suspendProcessInstanceById(procInst.Id);

		//when wrong variable is set
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME+1, 1);

		//then nothing happens
		assertTrue(runtimeService.createProcessInstanceQuery().singleResult().Suspended);

		//when variable which triggers condition is set
		//then exception is expected
		try
		{
		  runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);
		  fail("Should fail!");
		}
		catch (SuspendedEntityInteractionException)
		{
		  //expected
		}
		runtimeService.activateProcessInstanceById(procInst.Id);
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
	  }
	}

}