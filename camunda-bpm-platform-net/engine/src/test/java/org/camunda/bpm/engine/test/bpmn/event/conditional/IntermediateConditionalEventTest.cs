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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class IntermediateConditionalEventTest : AbstractConditionalEventTestCase
	{

	  protected internal const string EVENT_BASED_GATEWAY_ID = "egw";
	  protected internal const string PARALLEL_GATEWAY_ID = "parallelGateway";
	  protected internal const string TASK_BEFORE_SERVICE_TASK_ID = "taskBeforeServiceTask";
	  protected internal const string TASK_BEFORE_EVENT_BASED_GW_ID = "taskBeforeEGW";

	  public override void checkIfProcessCanBeFinished()
	  {
		//override since check is not needed in intermediate test suite
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testFalseCondition()
	  public virtual void testFalseCondition()
	  {
		//given process with intermediate conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery();
		Task task = taskQuery.processInstanceId(procInst.Id).singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before condition is completed
		taskService.complete(task.Id);

		//then next wait state is on conditional event, since condition is false
		//and a condition event subscription is create
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testTrueCondition()
	  public virtual void testTrueCondition()
	  {
		//given process with intermediate conditional event
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before condition is completed
		taskService.complete(task.Id);

		//then next wait state is on user task after conditional event, since condition was true
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNull(execution);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION, task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testVariableValue()
	  public virtual void testVariableValue()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);

		//wait state is on conditional event, since condition is false
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable is set to correct value
		runtimeService.setVariable(execution.Id, VARIABLE_NAME, 1);

		//then process instance is completed, since condition was true
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNull(execution);

		procInst = runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS_KEY).singleResult();
		assertNull(procInst);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testParallelVariableValue()
	  public virtual void testParallelVariableValue()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);
		Execution execution1 = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT + 1).singleResult();

		Execution execution2 = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT + 2).singleResult();
		assertEquals(2, conditionEventSubscriptionQuery.list().Count);

		//when variable is set to correct value
		runtimeService.setVariable(execution1.Id, VARIABLE_NAME, 1);

		//then execution of first conditional event is completed
		execution1 = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT + 1).singleResult();
		assertNull(execution1);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when second variable is set to correct value
		runtimeService.setVariable(execution2.Id, VARIABLE_NAME, 2);

		//then execution and process instance is ended, since both conditions was true
		execution2 = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT + 2).singleResult();
		assertNull(execution2);
		procInst = runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS_KEY).singleResult();
		assertNull(procInst);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testParallelVariableValueEqualConditions()
	  public virtual void testParallelVariableValueEqualConditions()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);

		//when variable is set to correct value
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then process instance is ended, since both conditions are true
		procInst = runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS_KEY).singleResult();
		assertNull(procInst);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/IntermediateConditionalEventTest.testParallelVariableValue.bpmn20.xml"}) public void testParallelVariableSetValueOnParent()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/IntermediateConditionalEventTest.testParallelVariableValue.bpmn20.xml"})]
	  public virtual void testParallelVariableSetValueOnParent()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);

		//when variable is set to correct value
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then execution of conditional event is completed
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT + 1).singleResult();
		assertNull(execution);

		//when second variable is set to correct value
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 2);

		//then execution and process instance is ended, since both conditions was true
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT + 2).singleResult();
		assertNull(execution);
		procInst = runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS_KEY).singleResult();
		assertNull(procInst);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testSubProcessVariableValue()
	  public virtual void testSubProcessVariableValue()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);

		//when variable is set to correct value
		runtimeService.setVariableLocal(execution.Id, VARIABLE_NAME, 1);

		//then execution and process instance is ended, since condition was true
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNull(execution);
		procInst = runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS_KEY).singleResult();
		assertNull(procInst);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/conditional/IntermediateConditionalEventTest.testSubProcessVariableValue.bpmn20.xml"}) public void testSubProcessVariableSetValueOnParent()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/conditional/IntermediateConditionalEventTest.testSubProcessVariableValue.bpmn20.xml"})]
	  public virtual void testSubProcessVariableSetValueOnParent()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);

		//when variable is set to correct value
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then process instance is ended, since condition was true
		procInst = runtimeService.createProcessInstanceQuery().processDefinitionKey(CONDITIONAL_EVENT_PROCESS_KEY).singleResult();
		assertNull(procInst);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testCleanUpConditionalEventSubscriptions()
	  public virtual void testCleanUpConditionalEventSubscriptions()
	  {
		//given process with intermediate conditional event and variable with wrong value
		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);

		//wait state is on conditional event, since condition is false
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);

		//condition subscription is created
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable is set to correct value
		runtimeService.setVariable(execution.Id, VARIABLE_NAME, 1);

		//then execution is on next user task and the subscription is deleted
		Task task = taskService.createTaskQuery().processInstanceId(procInst.Id).singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);

		//and task can be completed which ends process instance
		taskService.complete(task.Id);
		assertNull(taskService.createTaskQuery().singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableConditionWithVariableName()
	  public virtual void testVariableConditionWithVariableName()
	  {

		//given process with boundary conditional event and defined variable name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).camundaVariableName(VARIABLE_NAME).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).camundaVariableName(VARIABLE_NAME).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable with name `variable1` is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME+1, 1);

		//then nothing happens
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable with name `variable` is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional intermediate event
		Task task = taskQuery.singleResult();
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);

		//and task can be completed which ends process instance
		taskService.complete(task.Id);
		assertNull(taskService.createTaskQuery().singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableConditionWithVariableEvent()
	  public virtual void testVariableConditionWithVariableEvent()
	  {

		//given process with boundary conditional event and defined variable name and event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).camundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).camundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		IDictionary<string, object> variables = Variables.createVariables();
		variables[VARIABLE_NAME+1] = 0;
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY, variables);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);


		//when variable with name `variable` is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then nothing happens
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable with name `variable1` is updated
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME+1, 1);

		//then execution is at user task after conditional intermediate event
		Task task = taskQuery.singleResult();
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);

		//and task can be completed which ends process instance
		taskService.complete(task.Id);
		assertNull(taskService.createTaskQuery().singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableConditionWithVariableNameAndEvent()
	  public virtual void testVariableConditionWithVariableNameAndEvent()
	  {

		//given process with boundary conditional event and defined variable name and event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).camundaVariableName(VARIABLE_NAME).camundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).camundaVariableName(VARIABLE_NAME).camundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);


		//when variable with name `variable` is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then nothing happens
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(CONDITIONAL_EVENT).singleResult();
		assertNotNull(execution);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);

		//when variable with name `variable` is updated
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then execution is at user task after conditional intermediate event
		Task task = taskQuery.singleResult();
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);

		//and task can be completed which ends process instance
		taskService.complete(task.Id);
		assertNull(taskService.createTaskQuery().singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendedProcess()
	  public virtual void testSuspendedProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
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
//ORIGINAL LINE: @Test public void testEventBasedGateway()
	  public virtual void testEventBasedGateway()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().eventBasedGateway().id(EVENT_BASED_GATEWAY_ID).intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		assertEquals(1, conditionEventSubscriptionQuery.list().Count);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(EVENT_BASED_GATEWAY_ID).singleResult();
		assertNotNull(execution);

		//when variable is set on execution
		runtimeService.setVariable(procInst.Id, VARIABLE_NAME, 1);

		//then execution is at user task after intermediate conditional event
		Task task = taskQuery.singleResult();
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		assertEquals(0, conditionEventSubscriptionQuery.list().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventBasedGatewayTrueCondition()
	  public virtual void testEventBasedGatewayTrueCondition()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).eventBasedGateway().id(EVENT_BASED_GATEWAY_ID).intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(TRUE_CONDITION).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before condition is completed
		taskService.complete(task.Id);

		//then next wait state is on user task after conditional event, since condition was true
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(EVENT_BASED_GATEWAY_ID).singleResult();
		assertNull(execution);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION, task.Name);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventBasedGatewayWith2ConditionsOneIsTrue()
	  public virtual void testEventBasedGatewayWith2ConditionsOneIsTrue()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).eventBasedGateway().id(EVENT_BASED_GATEWAY_ID).intermediateCatchEvent().conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION + 1).endEvent().moveToLastGateway().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition(TRUE_CONDITION).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION + 2).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before condition is completed
		taskService.complete(task.Id);

		//then next wait state is on user task after true conditional event
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(EVENT_BASED_GATEWAY_ID).singleResult();
		assertNull(execution);

		task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION + 2, task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventBasedGatewayWith2VarConditions()
	  public virtual void testEventBasedGatewayWith2VarConditions()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().eventBasedGateway().id(EVENT_BASED_GATEWAY_ID).intermediateCatchEvent().conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION + 1).endEvent().moveToLastGateway().intermediateCatchEvent(CONDITIONAL_EVENT).conditionalEventDefinition().condition("${var==2}").conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION + 2).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(EVENT_BASED_GATEWAY_ID).singleResult();
		assertNotNull(execution);

		//when wrong value of variable `var` is set
		runtimeService.setVariable(procInst.Id, "var", 1);

		//then nothing happens
		execution = runtimeService.createExecutionQuery().processInstanceId(procInst.Id).activityId(EVENT_BASED_GATEWAY_ID).singleResult();
		assertNotNull(execution);
		assertEquals(0, taskQuery.count());

		//when right value is set
		runtimeService.setVariable(procInst.Id, "var", 2);

		//then next wait state is on user task after second conditional event
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION + 2, task.Name);
	  }

	  protected internal virtual void deployParallelProcessWithEventBasedGateway()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().parallelGateway().id(PARALLEL_GATEWAY_ID).userTask(TASK_BEFORE_EVENT_BASED_GW_ID).eventBasedGateway().id(EVENT_BASED_GATEWAY_ID).intermediateCatchEvent().conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().moveToNode(PARALLEL_GATEWAY_ID).userTask(TASK_BEFORE_SERVICE_TASK_ID).serviceTask().camundaClass(typeof(SetVariableDelegate).FullName).endEvent().done();

		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelProcessWithSetVariableBeforeReachingEventBasedGW()
	  public virtual void testParallelProcessWithSetVariableBeforeReachingEventBasedGW()
	  {
		deployParallelProcessWithEventBasedGateway();
		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task taskBeforeEGW = taskService.createTaskQuery().taskDefinitionKey(TASK_BEFORE_EVENT_BASED_GW_ID).singleResult();
		Task taskBeforeServiceTask = taskService.createTaskQuery().taskDefinitionKey(TASK_BEFORE_SERVICE_TASK_ID).singleResult();

		//when task before service task is completed and after that task before event based gateway
		taskService.complete(taskBeforeServiceTask.Id);
		taskService.complete(taskBeforeEGW.Id);

		//then variable is set before event based gateway is reached
		//on reaching event based gateway condition of conditional event is also evaluated to true
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		//completing this task ends process instance
		taskService.complete(task.Id);
		assertNull(taskQuery.singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelProcessWithSetVariableAfterReachingEventBasedGW()
	  public virtual void testParallelProcessWithSetVariableAfterReachingEventBasedGW()
	  {
		deployParallelProcessWithEventBasedGateway();
		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task taskBeforeEGW = taskService.createTaskQuery().taskDefinitionKey(TASK_BEFORE_EVENT_BASED_GW_ID).singleResult();
		Task taskBeforeServiceTask = taskService.createTaskQuery().taskDefinitionKey(TASK_BEFORE_SERVICE_TASK_ID).singleResult();

		//when task before event based gateway is completed and after that task before service task
		taskService.complete(taskBeforeEGW.Id);
		taskService.complete(taskBeforeServiceTask.Id);

		//then event based gateway is reached and executions stays there
		//variable is set after reaching event based gateway
		//after setting variable the conditional event is triggered and evaluated to true
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		//completing this task ends process instance
		taskService.complete(task.Id);
		assertNull(taskQuery.singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }
	}

}