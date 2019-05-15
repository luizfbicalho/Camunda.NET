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
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ConditionalEventTriggeredByExecutionListenerTest extends AbstractConditionalEventTestCase
	public class ConditionalEventTriggeredByExecutionListenerTest : AbstractConditionalEventTestCase
	{

	  protected internal const string TASK_AFTER_CONDITIONAL_BOUNDARY_EVENT = "Task after conditional boundary event";
	  protected internal const string TASK_AFTER_CONDITIONAL_START_EVENT = "Task after conditional start event";
	  protected internal const string START_EVENT_ID = "startEventId";
	  protected internal const string END_EVENT_ID = "endEventId";

	  private interface ConditionalEventProcessSpecifier
	  {
		BpmnModelInstance specifyConditionalProcess(BpmnModelInstance modelInstance, bool isInterrupting);
		void assertTaskNames(IList<Task> tasks, bool isInterrupting, bool isAyncBefore);
		int expectedSubscriptions();
		int expectedTaskCount();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "{index}: {0}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
//JAVA TO C# CONVERTER TODO TASK: The following anonymous inner class could not be converted:
//		return java.util.Arrays.asList(new Object[][]{ {new ConditionalEventProcessSpecifier()
	//	{
	//		@@Override public BpmnModelInstance specifyConditionalProcess(BpmnModelInstance modelInstance, boolean isInterrupting)
	//		{
	//		  return modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().interrupting(isInterrupting).conditionalEventDefinition().condition(CONDITION_EXPR).conditionalEventDefinitionDone().userTask().name(TASK_AFTER_CONDITION).endEvent().done();
	//		}
	//
	//		@@Override public void assertTaskNames(List<Task> tasks, boolean isInterrupting, boolean isAyncBefore)
	//		{
	//			if (isInterrupting || isAyncBefore)
	//			{
	//			  ConditionalEventTriggeredByExecutionListenerTest.assertTaskNames(tasks, TASK_AFTER_CONDITION);
	//			}
	//			else
	//			{
	//			  ConditionalEventTriggeredByExecutionListenerTest.assertTaskNames(tasks, TASK_WITH_CONDITION, TASK_AFTER_CONDITION);
	//			}
	//		}
	//
	//		@@Override public int expectedSubscriptions()
	//		{
	//		  return 1;
	//		}
	//
	//		@@Override public int expectedTaskCount()
	//		{
	//		  return 2;
	//		}
	//
	//		@@Override public String toString()
	//		{
	//		  return "ConditionalEventSubProcess";
	//		}
	//	  }
	  }
	 ,
	 {
		  new ConditionalEventProcessSpecifierAnonymousInnerClass2(this)
	 }
	}
	);
}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public ConditionalEventProcessSpecifier specifier;
	  public ConditionalEventProcessSpecifier specifier;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInStartListener()
	  public void testSetVariableInStartListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInStartListener()
	  public void testNonInterruptingSetVariableInStartListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE).name(TASK_WITH_CONDITION).endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then start listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(specifier.expectedTaskCount(), tasksAfterVariableIsSet.size());
		assertEquals(specifier.expectedSubscriptions(), conditionEventSubscriptionQuery.list().size());
		specifier.assertTaskNames(tasksAfterVariableIsSet, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInTakeListener()
	  public void testSetVariableInTakeListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent(END_EVENT_ID).done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then take listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInTakeListener()
	  public void testNonInterruptingSetVariableInTakeListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent(END_EVENT_ID).done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then take listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(specifier.expectedTaskCount(), tasksAfterVariableIsSet.size());
		assertEquals(specifier.expectedSubscriptions(), conditionEventSubscriptionQuery.list().size());
		specifier.assertTaskNames(tasksAfterVariableIsSet, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInTakeListenerWithAsyncBefore()
	  public void testSetVariableInTakeListenerWithAsyncBefore()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaAsyncBefore().endEvent(END_EVENT_ID).done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then take listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInTakeListenerWithAsyncBefore()
	  public void testNonInterruptingSetVariableInTakeListenerWithAsyncBefore()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaAsyncBefore().endEvent(END_EVENT_ID).done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then take listener sets variable
		//non interrupting boundary event is triggered
		specifier.assertTaskNames(taskQuery.list(), false, true);

		//and job was created
		Job job = engine.ManagementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(1, conditionEventSubscriptionQuery.list().size());

		//when job is executed task is created
		engine.ManagementService.executeJob(job.Id);
		//when tasks are completed
		foreach (Task task in taskQuery.list())
		{
		  taskService.complete(task.Id);
		}

		//then no task exist and process instance is ended
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(0, tasksAfterVariableIsSet.size());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInEndListener()
	  public void testSetVariableInEndListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task is completed
		taskService.complete(task.Id);

		//then end listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInEndListener()
	  public void testNonInterruptingSetVariableInEndListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then end listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(specifier.expectedTaskCount(), tasksAfterVariableIsSet.size());
		specifier.assertTaskNames(tasksAfterVariableIsSet, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnParentScopeInTakeListener()
	  public void testSetVariableOnParentScopeInTakeListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).subProcess().embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent(END_EVENT_ID).done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE_ON_PARENT;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnParentScopeInTakeListener()
	  public void testNonInterruptingSetVariableOnParentScopeInTakeListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).subProcess().embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent(END_EVENT_ID).done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaExpression = EXPR_SET_VARIABLE_ON_PARENT;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnParentScopeInStartListener()
	  public void testSetVariableOnParentScopeInStartListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).subProcess().embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE_ON_PARENT).endEvent().subProcessDone().endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnParentScopeInStartListener()
	  public void testNonInterruptingSetVariableOnParentScopeInStartListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).subProcess().embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, EXPR_SET_VARIABLE_ON_PARENT).endEvent().subProcessDone().endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then start listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableOnParentScopeInEndListener()
	  public void testSetVariableOnParentScopeInEndListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).subProcess().embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE_ON_PARENT).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, true);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then end listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableOnParentScopeInEndListener()
	  public void testNonInterruptingSetVariableOnParentScopeInEndListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent(START_EVENT_ID).subProcess().embeddedSubProcess().startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, EXPR_SET_VARIABLE_ON_PARENT).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().subProcessDone().endEvent(END_EVENT_ID).done();
		modelInstance = specifier.specifyConditionalProcess(modelInstance, false);
		engine.manageDeployment(repositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals(TASK_BEFORE_CONDITION, task.Name);

		//when task is completed
		taskService.complete(task.Id);

		//then end listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		specifier.assertTaskNames(tasksAfterVariableIsSet, false, false);
	  }

	}

}