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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class EventSubProcessStartConditionalEventTest : AbstractConditionalEventTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testTrueCondition()
	  public virtual void testTrueCondition()
	  {
		//given process with event sub process conditional start event

		//when process instance is started with true condition
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then event sub process is triggered via default evaluation behavior
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testFalseCondition()
	  public virtual void testFalseCondition()
	  {
		//given process with event sub process conditional start event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when variable is set on task with condition
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution stays at user task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_BEFORE_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testVariableCondition()
	  public virtual void testVariableCondition()
	  {
		//given process with event sub process conditional start event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when variable is set on task with condition
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources ={ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testVariableCondition.bpmn20.xml"}) public void testVariableConditionAndStartingWithVar()
	  [Deployment(resources :{ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testVariableCondition.bpmn20.xml"})]
	  public virtual void testVariableConditionAndStartingWithVar()
	  {
		//given process with event sub process conditional start event
		IDictionary<string, object> vars = Variables.createVariables();
		vars[VARIABLE_NAME] = 1;

		//when starting process with variable
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, vars);

		//then event sub process is triggered via default evaluation behavior
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources ={ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testVariableCondition.bpmn20.xml"}) public void testWrongVariableCondition()
	  [Deployment(resources :{ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testVariableCondition.bpmn20.xml"})]
	  public virtual void testWrongVariableCondition()
	  {
		//given process with event sub process conditional start event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable is set on task with condition
		taskService.setVariable(task.Id, VARIABLE_NAME+1, 1);

		//then execution stays at user task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_BEFORE_CONDITION, tasksAfterVariableIsSet[0].Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testNonInterruptingVariableCondition()
	  public virtual void testNonInterruptingVariableCondition()
	  {
		//given process with event sub process conditional start event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable is set on task with condition
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSubProcessVariableCondition()
	  public virtual void testSubProcessVariableCondition()
	  {
		//given process with event sub process conditional start event and user task in sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when local variable is set on task with condition
		taskService.setVariableLocal(task.Id, VARIABLE_NAME, 1);

		//then execution stays at user task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_BEFORE_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources ={ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"}) public void testSubProcessSetVariableOnTaskCondition()
	  [Deployment(resources :{ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"})]
	  public virtual void testSubProcessSetVariableOnTaskCondition()
	  {
		//given process with event sub process conditional start event and user task in sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when variable is set on task, variable is propagated to process instance
		taskService.setVariable(task.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources ={ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"}) public void testSubProcessSetVariableOnExecutionCondition()
	  [Deployment(resources :{ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"})]
	  public virtual void testSubProcessSetVariableOnExecutionCondition()
	  {
		//given process with event sub process conditional start event and user task in sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when local variable is set on task execution
		runtimeService.setVariableLocal(task.ExecutionId, VARIABLE_NAME, 1);

		//then execution stays at user task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_BEFORE_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }


	  protected internal virtual void deployConditionalEventSubProcess(BpmnModelInstance model, bool isInterrupting)
	  {
		deployConditionalEventSubProcess(model, CONDITIONAL_EVENT_PROCESS_KEY, isInterrupting);
	  }

	  protected internal override void deployConditionalEventSubProcess(BpmnModelInstance model, string parentId, bool isInterrupting)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = modify(model).addSubProcessTo(parentId).id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent().interrupting(isInterrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask("taskAfterCond").name(TASK_AFTER_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = modify(model).addSubProcessTo(parentId).id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent().interrupting(isInterrupting).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask("taskAfterCond").name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInDelegate()
	  public virtual void testSetVariableInDelegate()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask().camundaClass(SetVariableDelegate.class.getName()).endEvent().done();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask().camundaClass(typeof(SetVariableDelegate).FullName).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		// given process with event sub process conditional start event and service task with delegate class which sets a variable
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

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
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask().camundaClass(SetVariableDelegate.class.getName()).userTask().endEvent().done();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask().camundaClass(typeof(SetVariableDelegate).FullName).userTask().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		// given process with event sub process conditional start event and service task with delegate class which sets a variable
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then service task with delegated code is called and variable is set
		//-> non interrupting conditional event is triggered
		//execution stays at user task after condition and after service task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInDelegateWithSynchronousEvent()
	  public virtual void testSetVariableInDelegateWithSynchronousEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask().camundaClass(typeof(SetVariableDelegate).FullName).endEvent().done();

		modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(true).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task is completed
		taskService.complete(task.Id);

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
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask().camundaClass(typeof(SetVariableDelegate).FullName).userTask().endEvent().done();

		modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(false).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given process with event sub process conditional start event and service task with delegate class which sets a variable
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then service task with delegated code is called and variable is set
		//-> non interrupting conditional event is triggered
		//execution stays at user task after service task
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInInputMapping()
	  public virtual void testSetVariableInInputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then service task with input mapping is called and variable is set
		//-> interrupting conditional event is not triggered
		//since variable is only locally
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_SERVICE_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInInputMapping()
	  public virtual void testNonInterruptingSetVariableInInputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").camundaExpression(TRUE_CONDITION).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then service task with input mapping is called and variable is set
		//-> non interrupting conditional event is not triggered
		//since variable is only locally
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_SERVICE_TASK, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInExpression()
	  public virtual void testSetVariableInExpression()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression("${execution.setVariable(\"variable\", 1)}").userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression("${execution.setVariable(\"variable\", 1)}").userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

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
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression("${execution.setVariable(\"variable\", 1)}").userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).serviceTask(TASK_WITH_CONDITION_ID).camundaExpression("${execution.setVariable(\"variable\", 1)}").userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then service task with expression is called and variable is set
		//-> non interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInInputMappingOfSubProcess()
	  public virtual void testSetVariableInInputMappingOfSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent("startSubProcess").userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent("startSubProcess").userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then input mapping from sub process sets variable
		//-> interrupting conditional event is triggered by default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInInputMappingOfSubProcess()
	  public virtual void testNonInterruptingSetVariableInInputMappingOfSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask().name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask().name(TASK_IN_SUB_PROCESS_ID).endEvent().subProcessDone().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then input mapping from sub process sets variable
		//-> non interrupting conditional event is triggered via default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutputMapping()
	  public virtual void testSetVariableInOutputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping from user task sets variable
		//-> interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutputMapping()
	  public virtual void testNonInterruptingSetVariableInOutputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping from user task sets variable
		//-> non interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutputMappingOfCallActivity()
	  public virtual void testSetVariableInOutputMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping from call activity sets variable
		//-> interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutputMappingOfCallActivity()
	  public virtual void testNonInterruptingSetVariableInOutputMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOutputParameter(VARIABLE_NAME, "1").userTask().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);


		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then output mapping from call activity sets variable
		//-> non interrupting conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutMappingOfCallActivity()
	  public virtual void testSetVariableInOutMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

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
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);


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
		assertEquals(1, conditionEventSubscriptionQuery.count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInInMappingOfCallActivity()
	  public virtual void testSetVariableInInMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaIn(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaIn(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

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
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

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
//ORIGINAL LINE: @Test public void testSetVariableInCallActivity()
	  public virtual void testSetVariableInCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

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
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

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
		 deployConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, true);

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
		 deployConditionalEventSubProcess(modelInstance, SUB_PROCESS_ID, false);

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
//ORIGINAL LINE: @Test public void testSetVariableInSubProcessInDelegatedCodeConditionOnPI()
	  public virtual void testSetVariableInSubProcessInDelegatedCodeConditionOnPI()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

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
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInSubProcessInDelegatedCodeConditionOnPI()
	  public virtual void testNonInterruptingSetVariableInSubProcessInDelegatedCodeConditionOnPI()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().serviceTask().camundaExpression(EXPR_SET_VARIABLE).userTask().name(TASK_AFTER_SERVICE_TASK).endEvent().subProcessDone().endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

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
//ORIGINAL LINE: @Test @Deployment(resources ={ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"}) public void testSubProcessSetVariableOnProcessInstanceCondition()
	  [Deployment(resources :{ "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testSubProcessVariableCondition.bpmn20.xml"})]
	  public virtual void testSubProcessSetVariableOnProcessInstanceCondition()
	  {
		//given process with event sub process conditional start event and user task in sub process
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when variable is set on process instance
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendedProcess()
	  public virtual void testSuspendedProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).endEvent().done();

		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

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


		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

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
//ORIGINAL LINE: @Test public void testNonInterruptingSetMultipleVariables()
	  public virtual void testNonInterruptingSetMultipleVariables()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		//given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		Task task = taskQuery.singleResult();

		//when multiple variable are set on task execution
		VariableMap variables = Variables.createVariables();
		variables.put("variable", 1);
		variables.put("variable1", 1);
		runtimeService.setVariables(task.ExecutionId, variables);

		//then event sub process should be triggered more than once
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(3, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testLoop()
	  public virtual void testLoop()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("Task_1").singleResult();

		// when
		taskService.complete(task.Id);

		//then process instance will be in endless loop
		//to end the instance we have a conditional branch in the java delegate
		//after 3 instantiations the variable will be set to the instantiation count
		//execution stays in task 2
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals("Task_2", tasksAfterVariableIsSet[0].TaskDefinitionKey);
		assertEquals(3, runtimeService.getVariable(processInstance.Id, VARIABLE_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerAnotherEventSubprocess()
	  public virtual void testTriggerAnotherEventSubprocess()
	  {
		//given process with user task
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();

		//and event sub process with true condition
		modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(true).conditionalEventDefinition().condition(TRUE_CONDITION).conditionalEventDefinitionDone().userTask(TASK_AFTER_CONDITION_ID + 1).name(TASK_AFTER_CONDITION + 1).endEvent().done();
		//a second event sub process
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		//when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);

		//then first event sub process is on starting of process instance triggered
		Task task = taskQuery.singleResult();
		assertEquals(TASK_AFTER_CONDITION + 1, task.Name);

		//when variable is set, second condition becomes true -> but since first event sub process has
		// interrupt the process instance the second event sub process can't be triggered
		runtimeService.setVariable(processInstance.Id, VARIABLE_NAME, 1);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION + 1, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingTriggerAnotherEventSubprocess()
	  public virtual void testNonInterruptingTriggerAnotherEventSubprocess()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();

		//first event sub process
		modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).id("eventSubProcess1").triggerByEvent().embeddedSubProcess().startEvent().interrupting(false).conditionalEventDefinition().condition(TRUE_CONDITION).conditionalEventDefinitionDone().userTask("taskAfterCond1").name(TASK_AFTER_CONDITION + 1).endEvent().done();

		 deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		//given process with two event sub processes

		//when process is started
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);

		//then first event sub process is triggered because condition is true
		Task task = taskQuery.taskName(TASK_AFTER_CONDITION + 1).singleResult();
		assertNotNull(task);
		assertEquals(2, taskService.createTaskQuery().count());

		//when variable is set, second condition becomes true -> second event sub process is triggered
		runtimeService.setVariable(processInstance.Id, "variable", 1);
		task = taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).singleResult();
		assertNotNull(task);
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(4, tasksAfterVariableIsSet.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testNonInterruptingSetMultipleVariableInDelegate()
	  public virtual void testNonInterruptingSetMultipleVariableInDelegate()
	  {
		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(5, tasksAfterVariableIsSet.Count);
		assertEquals(3, taskService.createTaskQuery().taskDefinitionKey("Task_3").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInTriggeredEventSubProcess()
	  public virtual void testSetVariableInTriggeredEventSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).serviceTask().camundaClass(typeof(SetVariableDelegate).FullName).endEvent().done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(true).conditionalEventDefinition(CONDITIONAL_EVENT).condition(CONDITION_EXPR).conditionalEventDefinitionDone().serviceTask().camundaClass(typeof(LoopDelegate).FullName).userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());


		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		Task task = taskQuery.singleResult();
		assertEquals(TASK_WITH_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then variable is set
		//event sub process is triggered
		//and service task in event sub process triggers again sub process
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testVariableCondition.bpmn20.xml") public void testVariableConditionWithHistory()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.testVariableCondition.bpmn20.xml")]
	  public virtual void testVariableConditionWithHistory()
	  {
		// given process with event sub process conditional start event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, Variables.createVariables().putValue(VARIABLE_NAME, 1).putValue("donotloseme", "here"));

		// assume
		tasksAfterVariableIsSet = taskService.createTaskQuery().processInstanceId(procInst.Id).list();
		assertEquals(1, tasksAfterVariableIsSet.Count);

		// then
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName(VARIABLE_NAME).count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("donotloseme").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Deployment public void testNonInterruptingVariableConditionWithHistory()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testNonInterruptingVariableConditionWithHistory()
	  {
		// given process with event sub process conditional start event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, Variables.createVariables().putValue(VARIABLE_NAME, 1).putValue("donotloseme", "here"));

		// assume
		tasksAfterVariableIsSet = taskService.createTaskQuery().processInstanceId(procInst.Id).list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		// then
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName(VARIABLE_NAME).count());
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().variableName("donotloseme").count());
	  }
	}

}