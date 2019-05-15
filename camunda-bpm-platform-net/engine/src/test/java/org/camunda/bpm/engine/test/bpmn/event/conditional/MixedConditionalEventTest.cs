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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using UserTaskBuilder = org.camunda.bpm.model.bpmn.builder.UserTaskBuilder;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MixedConditionalEventTest : AbstractConditionalEventTestCase
	{

	  protected internal const string TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT = "Task after conditional boundary event";
	  protected internal const string TASK_AFTER_CONDITIONAL_START_EVENT = "Task after conditional start event";
	  protected internal const string TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS = "Task after cond start event in sub process";
	  protected internal const string TASK_AFTER_COND_BOUN_EVENT_IN_SUB_PROCESS = "Task after cond bound event in sub process";

	  protected internal virtual BpmnModelInstance addBoundaryEvent(BpmnModelInstance modelInstance, string activityId, string userTaskName, bool isInterrupting)
	  {
		return modify(modelInstance).activityBuilder(activityId).boundaryEvent().cancelActivity(isInterrupting).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(userTaskName).endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance addEventSubProcess(BpmnModelInstance model, string parentId, string userTaskName, bool isInterrupting)
	  {
		return modify(model).addSubProcessTo(parentId).triggerByEvent().embeddedSubProcess().startEvent().interrupting(isInterrupting).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(userTaskName).endEvent().done();
	  }

	  protected internal virtual void deployMixedProcess(BpmnModelInstance model, string parentId, bool isInterrupting)
	  {
		deployMixedProcess(model, parentId, TASK_WITH_CONDITION_ID, isInterrupting);
	  }

	  protected internal virtual void deployMixedProcess(BpmnModelInstance model, string parentId, string activityId, bool isInterrupting)
	  {
		BpmnModelInstance modelInstance = addEventSubProcess(model, parentId, TASK_AFTER_CONDITIONAL_START_EVENT, isInterrupting);
		modelInstance = addBoundaryEvent(modelInstance, activityId, TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, isInterrupting);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
	  }

	  // io mapping ////////////////////////////////////////////////////////////////////////////////////////////////////////


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnInputMapping()
	  public virtual void testSetVariableOnInputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().done();

		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary event should triggered with the default evaluation behavior
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnOutputMapping()
	  public virtual void testSetVariableOnOutputMapping()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().done();

		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnInputMapping()
	  public virtual void testNonInterruptingSetVariableOnInputMapping()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().done();

		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then conditional boundary event should not triggered also not conditional start event
		//since variable is only local
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_WITH_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnOutputMapping()
	  public virtual void testNonInterruptingSetVariableOnOutputMapping()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().done();

		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_CONDITIONAL_START_EVENT, TASK_AFTER_OUTPUT_MAPPING);
	  }

	  // sub process testing with event sub process and conditional start event and boundary event on user task
	  // execution listener in sub process //////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnStartExecutionListenerInSubProcess()
	  public virtual void testSetVariableOnStartExecutionListenerInSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnEndExecutionListenerInSubProcess()
	  public virtual void testSetVariableOnEndExecutionListenerInSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnStartExecutionListenerInSubProcess()
	  public virtual void testNonInterruptingSetVariableOnStartExecutionListenerInSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then conditional boundary should triggered via default evaluation behavior
		//and conditional start event via delayed events
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(3, tasksAfterVariableIsSet.Count);
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_CONDITIONAL_START_EVENT, TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, TASK_WITH_CONDITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnEndExecutionListenerInSubProcess()
	  public virtual void testNonInterruptingSetVariableOnEndExecutionListenerInSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_CONDITIONAL_START_EVENT, TASK_AFTER_OUTPUT_MAPPING);
	  }

	  // io mapping in sub process /////////////////////////////////////////////////////////////////////////////////////////


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnInputMappingInSubProcess()
	  public virtual void testSetVariableOnInputMappingInSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary event should triggered via default evaluation behavior
		//but conditional start event should not
		//since variable is only local
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnOutputMappingInSubProcess()
	  public virtual void testSetVariableOnOutputMappingInSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnInputMappingInSubProcess()
	  public virtual void testNonInterruptingSetVariableOnInputMappingInSubProcess()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaInputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then conditional boundary event should not triggered also not conditional start event
		//since variable is only local
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_WITH_CONDITION, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnOutputMappingInSubProcess()
	  public virtual void testNonInterruptingSetVariableOnOutputMappingInSubProcess()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();

		deployMixedProcess(modelInstance, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(2, tasksAfterVariableIsSet.Count);
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_CONDITIONAL_START_EVENT, TASK_AFTER_OUTPUT_MAPPING);
	  }


	  // sub process testing with event sub process on process instance and in sub process /////////////////////////////////
	  // and conditional start event and boundary event on sub process /////////////////////////////////////////////////////
	  // execution listener in sub process /////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnStartExecutionListenerInSubProcessWithBoundary()
	  public virtual void testSetVariableOnStartExecutionListenerInSubProcessWithBoundary()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, true);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when start listener sets variable

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnEndExecutionListenerInSubProcessWithBoundary()
	  public virtual void testSetVariableOnEndExecutionListenerInSubProcessWithBoundary()
	  {

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, true);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnStartExecutionListenerInSubProcessWithBoundary()
	  public virtual void testNonInterruptingSetVariableOnStartExecutionListenerInSubProcessWithBoundary()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, false);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when start listener sets variable

		//then conditional boundary and event sub process inside the sub process should triggered via default evaluation behavior
		//and global conditional start event via delayed events
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(4, tasksAfterVariableIsSet.Count);
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_CONDITIONAL_START_EVENT, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, TASK_WITH_CONDITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnEndExecutionListenerInSubProcessWithBoundary()
	  public virtual void testNonInterruptingSetVariableOnEndExecutionListenerInSubProcessWithBoundary()
	  {

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, false);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then all conditional events are triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(4, tasksAfterVariableIsSet.Count);
	  }

	  // io mapping in sub process /////////////////////////////////////////////////////////////////////////////////////////


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnInputMappingInSubProcessWithBoundary()
	  public virtual void testSetVariableOnInputMappingInSubProcessWithBoundary()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, true);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when input mapping sets variable

		//then conditional boundary event should triggered from the default evaluation behavior
		// The event sub process inside the sub process should not since the scope is lower than from the boundary.
		// The global event sub process should not since the variable is only locally.
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnOutputMappingInSubProcessWithBoundary()
	  public virtual void testSetVariableOnOutputMappingInSubProcessWithBoundary()
	  {

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, true);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task before is completed
		taskService.complete(task.Id);

		//then conditional boundary should not triggered but conditional start event
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnInputMappingInSubProcessWithBoundary()
	  public virtual void testNonInterruptingSetVariableOnInputMappingInSubProcessWithBoundary()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).camundaInputParameter(VARIABLE_NAME, "1").embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, false);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when input mapping sets variable

		//then conditional boundary event should triggered and also conditional start event in sub process
		//via the default evaluation behavior but not the global event sub process
		//since variable is only local
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(3, tasksAfterVariableIsSet.Count);
		assertTaskNames(tasksAfterVariableIsSet, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT, TASK_WITH_CONDITION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnOutputMappingInSubProcessWithBoundary()
	  public virtual void testNonInterruptingSetVariableOnOutputMappingInSubProcessWithBoundary()
	  {

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().userTask(TASK_WITH_CONDITION_ID).camundaOutputParameter(VARIABLE_NAME, "1").name(TASK_WITH_CONDITION).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, false);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task before is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then all conditional events are triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(4, tasksAfterVariableIsSet.Count);
	  }


	  //sub process with call activity and out mapping /////////////////////////////////////////////////////////////////////
	  //conditional boundary on sub process and call activity //////////////////////////////////////////////////////////////
	  //conditional start event event sub process on process instance level and on sub process /////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInOutMappingOfCallActivity()
	  public virtual void testSetVariableInOutMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, true);
		modelInstance = addBoundaryEvent(modelInstance, TASK_WITH_CONDITION_ID, TASK_AFTER_COND_BOUN_EVENT_IN_SUB_PROCESS, true);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then out mapping from call activity sets variable
		//-> interrupting conditional start event on process instance level is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInOutMappingOfCallActivity()
	  public virtual void testNonInterruptingSetVariableInOutMappingOfCallActivity()
	  {
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, DELEGATED_PROCESS).deploy());

		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent().callActivity(TASK_WITH_CONDITION_ID).calledElement(DELEGATED_PROCESS_KEY).camundaOut(VARIABLE_NAME, VARIABLE_NAME).userTask().name(TASK_AFTER_OUTPUT_MAPPING).endEvent().subProcessDone().endEvent().done();

		modelInstance = addEventSubProcess(modelInstance, SUB_PROCESS_ID, TASK_AFTER_COND_START_EVENT_IN_SUB_PROCESS, false);
		modelInstance = addBoundaryEvent(modelInstance, TASK_WITH_CONDITION_ID, TASK_AFTER_COND_BOUN_EVENT_IN_SUB_PROCESS, false);
		deployMixedProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, SUB_PROCESS_ID, false);


		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task before service task is completed
		taskService.complete(task.Id);

		//then out mapping of call activity sets a variable
		//-> all non interrupting conditional events are triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(5, tasksAfterVariableIsSet.Count);
		//three subscriptions: event sub process in sub process and on process instance level and boundary event of sub process
		assertEquals(3, conditionEventSubscriptionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Deployment public void testCompensationWithConditionalEvents()
	  public virtual void testCompensationWithConditionalEvents()
	  {
		//given process with compensation and conditional events
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		assertEquals("Before Cancel", task.Name);

		//when task before cancel is completed
		taskService.complete(task.Id);

		//then compensation is triggered -> which triggers conditional events
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(4, tasksAfterVariableIsSet.Count);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testCompactedExecutionTree()
	  public virtual void testCompactedExecutionTree()
	  {
		//given process with concurrent execution and conditional events
		runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//when task before cancel is completed
		taskService.complete(taskService.createTaskQuery().taskName(TASK_BEFORE_CONDITION).singleResult().Id);

		//then conditional events are triggered
		tasksAfterVariableIsSet = taskService.createTaskQuery().list();
		assertEquals(1, tasksAfterVariableIsSet.Count);
		assertEquals(TASK_AFTER_CONDITIONAL_START_EVENT, tasksAfterVariableIsSet[0].Name);
	  }
	}

}