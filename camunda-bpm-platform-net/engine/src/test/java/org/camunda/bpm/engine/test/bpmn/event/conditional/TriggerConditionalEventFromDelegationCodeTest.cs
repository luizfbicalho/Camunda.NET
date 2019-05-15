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
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class TriggerConditionalEventFromDelegationCodeTest extends AbstractConditionalEventTestCase
	public class TriggerConditionalEventFromDelegationCodeTest : AbstractConditionalEventTestCase
	{

	  private interface ConditionalEventProcessSpecifier
	  {
		Type DelegateClass {get;}
		int ExpectedInterruptingCount {get;}
		int ExpectedNonInterruptingCount {get;}
		string Condition {get;}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "{index}: {0}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
//JAVA TO C# CONVERTER TODO TASK: The following anonymous inner class could not be converted:
//		return java.util.Arrays.asList(new Object[][]{ {new ConditionalEventProcessSpecifier()
	//	{
	//		@@Override public Class getDelegateClass()
	//		{
	//		  return SetVariableDelegate.class;
	//		}
	//
	//		@@Override public int getExpectedInterruptingCount()
	//		{
	//		  return 1;
	//		}
	//
	//		@@Override public int getExpectedNonInterruptingCount()
	//		{
	//		  return 1;
	//		}
	//
	//		@@Override public String getCondition()
	//		{
	//		  return CONDITION_EXPR;
	//		}
	//
	//		@@Override public String toString()
	//		{
	//		  return "SetSingleVariableInDelegate";
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
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, specifier.DelegateClass.Name).endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, true);

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
		assertEquals(specifier.ExpectedInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInStartListener()
	  public void testNonInterruptingSetVariableInStartListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).userTask(TASK_WITH_CONDITION_ID).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, specifier.DelegateClass.Name).name(TASK_WITH_CONDITION).endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then start listener sets variable
		//non interrupting boundary event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1 + specifier.ExpectedNonInterruptingCount, tasksAfterVariableIsSet.size());
		assertEquals(specifier.ExpectedNonInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInTakeListener()
	  public void testSetVariableInTakeListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaClass = specifier.DelegateClass.Name;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, true);

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
		assertEquals(specifier.ExpectedInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInTakeListener()
	  public void testNonInterruptingSetVariableInTakeListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaClass = specifier.DelegateClass.Name;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, false);

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
		assertEquals(1 + specifier.ExpectedNonInterruptingCount, tasksAfterVariableIsSet.size());
		assertEquals(specifier.ExpectedNonInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInTakeListenerWithAsyncBefore()
	  public void testSetVariableInTakeListenerWithAsyncBefore()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).camundaAsyncBefore().endEvent().done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaClass = specifier.DelegateClass.Name;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, true);

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
		assertEquals(specifier.ExpectedInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInTakeListenerWithAsyncBefore()
	  public void testNonInterruptingSetVariableInTakeListenerWithAsyncBefore()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).sequenceFlowId(FLOW_ID).userTask(TASK_WITH_CONDITION_ID).camundaAsyncBefore().endEvent().done();
		CamundaExecutionListener listener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		listener.CamundaClass = specifier.DelegateClass.Name;
		modelInstance.getModelElementById<SequenceFlow>(FLOW_ID).builder().addExtensionElement(listener);
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then take listener sets variable
		//non interrupting boundary event is triggered
		assertEquals(specifier.ExpectedNonInterruptingCount, taskService.createTaskQuery().taskName(TASK_AFTER_CONDITION).count());

		//and job was created
		Job job = engine.ManagementService.createJobQuery().singleResult();
		assertNotNull(job);


		//when job is executed task is created
		engine.ManagementService.executeJob(job.Id);
		//when all tasks are completed
		assertEquals(specifier.ExpectedNonInterruptingCount + 1, taskQuery.count());
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
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, specifier.DelegateClass.Name).userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, true);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();

		//when task is completed
		taskService.complete(task.Id);

		//then end listener sets variable
		//conditional event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(specifier.ExpectedInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonInterruptingSetVariableInEndListener()
	  public void testNonInterruptingSetVariableInEndListener()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, specifier.DelegateClass.Name).userTask(TASK_WITH_CONDITION_ID).name(TASK_WITH_CONDITION).endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, false);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then end listener sets variable
		//non interrupting event is triggered
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(1 + specifier.ExpectedNonInterruptingCount, tasksAfterVariableIsSet.size());
		assertEquals(specifier.ExpectedNonInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetVariableInStartAndEndListener()
	  public void testSetVariableInStartAndEndListener()
	  {
		//given process with start and end listener on user task
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().userTask(TASK_BEFORE_CONDITION_ID).name(TASK_BEFORE_CONDITION).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, specifier.DelegateClass.Name).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, specifier.DelegateClass.Name).userTask(TASK_WITH_CONDITION_ID).endEvent().done();
		deployConditionalEventSubProcess(modelInstance, CONDITIONAL_EVENT_PROCESS_KEY, specifier.Condition, true);

		//when process is started
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then start listener sets variable and
		//execution stays in task after conditional event in event sub process
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task task = taskQuery.singleResult();
		assertEquals(TASK_AFTER_CONDITION, task.Name);
		tasksAfterVariableIsSet = taskQuery.list();
		assertEquals(specifier.ExpectedInterruptingCount, taskQuery.taskName(TASK_AFTER_CONDITION).count());
	  }
	}

}