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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using EndEvent = org.camunda.bpm.model.bpmn.instance.EndEvent;
	using TerminateEventDefinition = org.camunda.bpm.model.bpmn.instance.TerminateEventDefinition;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;

	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// Tests for when delegate code synchronously cancels the activity instance it belongs to.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class SelfCancellationTest
	{
		private bool InstanceFieldsInitialized = false;

		public SelfCancellationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineRule).around(testHelper);
		}


	  protected internal const string MESSAGE = "Message";

	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineRule).around(testHelper);
	  public RuleChain ruleChain;

	  //========================================================================================================================
	  //=======================================================MODELS===========================================================
	  //========================================================================================================================

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK = Bpmn.createExecutableProcess("process").startEvent().parallelGateway("fork").userTask().sendTask("sendTask").camundaClass(typeof(SendMessageDelegate).FullName).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).moveToLastGateway().receiveTask("receiveTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).message(MESSAGE).endEvent("terminateEnd").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_AND_USER_TASK_AFTER_SEND = modify(PROCESS_WITH_CANCELING_RECEIVE_TASK).removeFlowNode("endEvent").activityBuilder("sendTask").userTask("userTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).endEvent().done();

	  public static readonly BpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_WITHOUT_END_AFTER_SEND = modify(PROCESS_WITH_CANCELING_RECEIVE_TASK).removeFlowNode("endEvent");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE = modify(PROCESS_WITH_CANCELING_RECEIVE_TASK).activityBuilder("sendTask").boundaryEvent("boundary").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).timerWithDuration("PT5S").endEvent("endEventBoundary").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done();


	  public static readonly BpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE_WITHOUT_END = modify(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE).removeFlowNode("endEvent");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_WITH_SUBPROCESS_AND_DELEGATE_MSG_SEND = modify(Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().userTask().serviceTask("sendTask").camundaClass(typeof(SendMessageDelegate).FullName).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).endEvent("endEventSubProc").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).subProcessDone().endEvent().camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done()).addSubProcessTo("process").triggerByEvent().embeddedSubProcess().startEvent("startSubEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).message(MESSAGE).endEvent("endEventSubEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_WITH_PARALLEL_SEND_TASK_AND_BOUNDARY_EVENT = Bpmn.createExecutableProcess("process").startEvent().parallelGateway("fork").userTask().endEvent().camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).moveToLastGateway().sendTask("sendTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).camundaClass(typeof(SignalDelegate).FullName).boundaryEvent("boundary").message(MESSAGE).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).endEvent("endEventBoundary").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).moveToNode("sendTask").endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done();


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_WITH_SEND_TASK_AND_BOUNDARY_EVENT = Bpmn.createExecutableProcess("process").startEvent().sendTask("sendTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).camundaClass(typeof(SignalDelegate).FullName).boundaryEvent("boundary").message(MESSAGE).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).endEvent("endEventBoundary").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).moveToNode("sendTask").endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done();


	  //========================================================================================================================
	  //=========================================================INIT===========================================================
	  //========================================================================================================================

	  static SelfCancellationTest()
	  {
		initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK, "terminateEnd");
		initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_AND_USER_TASK_AFTER_SEND, "terminateEnd");
		initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE, "terminateEnd");
		initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITHOUT_END_AFTER_SEND, "terminateEnd");
		initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE_WITHOUT_END, "terminateEnd");
	  }

	  public static void initEndEvent(BpmnModelInstance modelInstance, string endEventId)
	  {
		EndEvent endEvent = modelInstance.getModelElementById(endEventId);
		TerminateEventDefinition terminateDefinition = modelInstance.newInstance(typeof(TerminateEventDefinition));
		endEvent.addChildElement(terminateDefinition);
	  }

	  //========================================================================================================================
	  //=======================================================TESTS============================================================
	  //========================================================================================================================


	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void clearRecorderListener()
	  public virtual void clearRecorderListener()
	  {
		RecorderExecutionListener.clear();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = processEngineRule.RuntimeService;
		taskService = processEngineRule.TaskService;
	  }

	  private void checkRecordedEvents(params string[] activityIds)
	  {
		IList<RecorderExecutionListener.RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(activityIds.Length, recordedEvents.Count);

		for (int i = 0; i < activityIds.Length; i++)
		{
		  assertEquals(activityIds[i], recordedEvents[i].ActivityId);
		}
	  }

	  private void testParallelTerminationWithSend(BpmnModelInstance modelInstance)
	  {
		// given
		testHelper.deploy(modelInstance);
		runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();

		// when
		taskService.complete(task.Id);

		// then
		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		checkRecordedEvents("receiveTask", "sendTask", "terminateEnd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerParallelTerminateEndEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTriggerParallelTerminateEndEvent()
	  {
		testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerParallelTerminateEndEventWithUserTask() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTriggerParallelTerminateEndEventWithUserTask()
	  {
		testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_AND_USER_TASK_AFTER_SEND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerParallelTerminateEndEventWithoutEndAfterSend() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTriggerParallelTerminateEndEventWithoutEndAfterSend()
	  {
		testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITHOUT_END_AFTER_SEND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerParallelTerminateEndEventWithSendAsScope() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTriggerParallelTerminateEndEventWithSendAsScope()
	  {
		testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTriggerParallelTerminateEndEventWithSendAsScopeWithoutEnd() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTriggerParallelTerminateEndEventWithSendAsScopeWithoutEnd()
	  {
		testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE_WITHOUT_END);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSendMessageInSubProcess() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSendMessageInSubProcess()
	  {
		// given
		testHelper.deploy(PROCESS_WITH_SUBPROCESS_AND_DELEGATE_MSG_SEND);
		runtimeService.startProcessInstanceByKey("process");

		Task task = taskService.createTaskQuery().singleResult();

		// when
		taskService.complete(task.Id);

		// then
		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		checkRecordedEvents("sendTask", "startSubEvent", "endEventSubEvent");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelSendTaskWithBoundaryRecieveTask() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParallelSendTaskWithBoundaryRecieveTask()
	  {
		// given
		testHelper.deploy(PROCESS_WITH_PARALLEL_SEND_TASK_AND_BOUNDARY_EVENT);
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("process");

		Execution activity = runtimeService.createExecutionQuery().activityId("sendTask").singleResult();
		runtimeService.signal(activity.Id);

		// then
		IList<string> activities = runtimeService.getActiveActivityIds(procInst.Id);
		Assert.assertNotNull(activities);
		Assert.assertEquals(1, activities.Count);
		checkRecordedEvents("sendTask", "boundary", "endEventBoundary");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSendTaskWithBoundaryEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSendTaskWithBoundaryEvent()
	  {
		// given
		testHelper.deploy(PROCESS_WITH_SEND_TASK_AND_BOUNDARY_EVENT);
		runtimeService.startProcessInstanceByKey("process");

		Execution activity = runtimeService.createExecutionQuery().activityId("sendTask").singleResult();
		runtimeService.signal(activity.Id);

		// then
		checkRecordedEvents("sendTask", "boundary", "endEventBoundary");
	  }

	  //========================================================================================================================
	  //===================================================STATIC CLASSES=======================================================
	  //========================================================================================================================
	  public class SendMessageDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  RuntimeService runtimeService = execution.ProcessEngineServices.RuntimeService;
		  runtimeService.correlateMessage(MESSAGE);
		}
	  }

	  public class SignalDelegate : SignallableActivityBehavior
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
		public virtual void execute(ActivityExecution execution)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalEvent, Object signalData) throws Exception
		public virtual void signal(ActivityExecution execution, string signalEvent, object signalData)
		{
		  RuntimeService runtimeService = execution.ProcessEngineServices.RuntimeService;
		  runtimeService.correlateMessage(MESSAGE);
		}
	  }
	}

}