using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.bpmn.@event.signal
{
	using EventSubscriptionQueryImpl = org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using FailingJavaSerializable = org.camunda.bpm.engine.test.api.variables.FailingJavaSerializable;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class SignalEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public SignalEventTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;
	  private RepositoryService repositoryService;
	  private ManagementService managementService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal bool defaultEnsureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		defaultEnsureJobDueDateSet = processEngineConfiguration.EnsureJobDueDateNotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetConfiguration()
	  public virtual void resetConfiguration()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"}) @Test public void testSignalCatchIntermediate()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testSignalCatchIntermediate()
	  {

		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.startProcessInstanceByKey("throwSignal");

		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundary.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"}) @Test public void testSignalCatchBoundary()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundary.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testSignalCatchBoundary()
	  {
		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.startProcessInstanceByKey("throwSignal");

		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundaryWithReceiveTask.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"}) @Test public void testSignalCatchBoundaryWithVariables()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundaryWithReceiveTask.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testSignalCatchBoundaryWithVariables()
	  {
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["processName"] = "catchSignal";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("catchSignal", variables1);

		Dictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["processName"] = "throwSignal";
		runtimeService.startProcessInstanceByKey("throwSignal", variables2);

		assertEquals("catchSignal", runtimeService.getVariable(pi.Id, "processName"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsynch.bpmn20.xml"}) @Test public void testSignalCatchIntermediateAsynch()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsynch.bpmn20.xml"})]
	  public virtual void testSignalCatchIntermediateAsynch()
	  {

		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.startProcessInstanceByKey("throwSignal");

		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// there is a job:
		assertEquals(1, managementService.createJobQuery().count());

		try
		{
		  ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis() + 1000);
		  testRule.waitForJobExecutorToProcessAllJobs(10000);

		  assertEquals(0, createEventSubscriptionQuery().count());
		  assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		  assertEquals(0, managementService.createJobQuery().count());
		}
		finally
		{
		  ClockUtil.CurrentTime = DateTime.Now;
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchMultipleSignals.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAbortSignal.bpmn20.xml"}) @Test public void testSignalCatchDifferentSignals()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchMultipleSignals.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAbortSignal.bpmn20.xml"})]
	  public virtual void testSignalCatchDifferentSignals()
	  {

		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(2, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.startProcessInstanceByKey("throwAbort");

		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		Task taskAfterAbort = taskService.createTaskQuery().taskAssignee("gonzo").singleResult();
		assertNotNull(taskAfterAbort);
		taskService.complete(taskAfterAbort.Id);

		runtimeService.startProcessInstanceByKey("throwSignal");

		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  /// <summary>
	  /// Verifies the solution of https://jira.codehaus.org/browse/ACT-1309
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSignalBoundaryOnSubProcess()
	  public virtual void testSignalBoundaryOnSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("signalEventOnSubprocess");
		runtimeService.signalEventReceived("stopSignal");
		testRule.assertProcessEnded(pi.ProcessInstanceId);
	  }

	  private EventSubscriptionQueryImpl createEventSubscriptionQuery()
	  {
		return new EventSubscriptionQueryImpl(processEngineConfiguration.CommandExecutorTxRequired);
	  }

	  /// <summary>
	  /// TestCase to reproduce Issue ACT-1344
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testNonInterruptingSignal()
	  public virtual void testNonInterruptingSignal()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingSignalEvent");

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(1, tasks.Count);
		Task currentTask = tasks[0];
		assertEquals("My User Task", currentTask.Name);

		runtimeService.signalEventReceived("alert");

		tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  if (!task.Name.Equals("My User Task") && !task.Name.Equals("My Second User Task"))
		  {
			fail("Expected: <My User Task> or <My Second User Task> but was <" + task.Name + ">.");
		  }
		}

		taskService.complete(taskService.createTaskQuery().taskName("My User Task").singleResult().Id);

		tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(1, tasks.Count);
		currentTask = tasks[0];
		assertEquals("My Second User Task", currentTask.Name);
	  }


	  /// <summary>
	  /// TestCase to reproduce Issue ACT-1344
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testNonInterruptingSignalWithSubProcess()
	  public virtual void testNonInterruptingSignalWithSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingSignalWithSubProcess");
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(1, tasks.Count);

		Task currentTask = tasks[0];
		assertEquals("Approve", currentTask.Name);

		runtimeService.signalEventReceived("alert");

		tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  if (!task.Name.Equals("Approve") && !task.Name.Equals("Review"))
		  {
			fail("Expected: <Approve> or <Review> but was <" + task.Name + ">.");
		  }
		}

		taskService.complete(taskService.createTaskQuery().taskName("Approve").singleResult().Id);

		tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(1, tasks.Count);

		currentTask = tasks[0];
		assertEquals("Review", currentTask.Name);

		taskService.complete(taskService.createTaskQuery().taskName("Review").singleResult().Id);

		tasks = taskService.createTaskQuery().processInstanceId(pi.ProcessInstanceId).list();
		assertEquals(1, tasks.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSignalStartEventInEventSubProcess()
	  public virtual void testSignalStartEventInEventSubProcess()
	  {
		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("signalStartEventInEventSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		// send interrupting signal to event sub process
		runtimeService.signalEventReceived("alert");

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task doesn't exist because signal start event is interrupting
		taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(0, taskQuery.count());

		// check if execution doesn't exist because signal start event is interrupting
		executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(0, executionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testNonInterruptingSignalStartEventInEventSubProcess()
	  public virtual void testNonInterruptingSignalStartEventInEventSubProcess()
	  {
		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingSignalStartEventInEventSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		// send non interrupting signal to event sub process
		runtimeService.signalEventReceived("alert");

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task still exists because signal start event is non interrupting
		taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		// check if execution still exists because signal start event is non interrupting
		executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml"}) @Test public void testSignalStartEvent()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml"})]
	  public virtual void testSignalStartEvent()
	  {
		// event subscription for signal start event
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert").count());

		runtimeService.signalEventReceived("alert");
		// the signal should start a new process instance
		assertEquals(1, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml"}) @Test public void testSuspendedProcessWithSignalStartEvent()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml"})]
	  public virtual void testSuspendedProcessWithSignalStartEvent()
	  {
		// event subscription for signal start event
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert").count());

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		runtimeService.signalEventReceived("alert");
		// the signal should not start a process instance for the suspended process definition
		assertEquals(0, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.testOtherSignalStartEvent.bpmn20.xml"}) @Test public void testMultipleProcessesWithSameSignalStartEvent()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.testOtherSignalStartEvent.bpmn20.xml"})]
	  public virtual void testMultipleProcessesWithSameSignalStartEvent()
	  {
		// event subscriptions for signal start event
		assertEquals(2, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert").count());

		runtimeService.signalEventReceived("alert");
		// the signal should start new process instances for both process definitions
		assertEquals(2, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"}) @Test public void testStartProcessInstanceBySignalFromIntermediateThrowingSignalEvent()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testStartProcessInstanceBySignalFromIntermediateThrowingSignalEvent()
	  {
		// start a process instance to throw a signal
		runtimeService.startProcessInstanceByKey("throwSignal");
		// the signal should start a new process instance
		assertEquals(1, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"}) @Test public void testIntermediateThrowingSignalEventWithSuspendedSignalStartEvent()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testIntermediateThrowingSignalEventWithSuspendedSignalStartEvent()
	  {
		// event subscription for signal start event
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert").count());

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("startBySignal").singleResult();
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		// start a process instance to throw a signal
		runtimeService.startProcessInstanceByKey("throwSignal");
		// the signal should not start a new process instance of the suspended process definition
		assertEquals(0, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testProcessesWithMultipleSignalStartEvents()
	  public virtual void testProcessesWithMultipleSignalStartEvents()
	  {
		// event subscriptions for signal start event
		assertEquals(2, runtimeService.createEventSubscriptionQuery().eventType("signal").count());

		runtimeService.signalEventReceived("alert");
		// the signal should start new process instances for both process definitions
		assertEquals(1, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertTwiceAndTerminate.bpmn20.xml"}) @Test public void testThrowSignalMultipleCancellingReceivers()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertTwiceAndTerminate.bpmn20.xml"})]
	  public virtual void testThrowSignalMultipleCancellingReceivers()
	  {
		RecorderExecutionListener.clear();

		runtimeService.startProcessInstanceByKey("catchAlertTwiceAndTerminate");

		// event subscription for intermediate signal events
		assertEquals(2, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert").count());

		// try to send 'alert' signal to both executions
		runtimeService.signalEventReceived("alert");

		// then only one terminate end event was executed
		assertEquals(1, RecorderExecutionListener.RecordedEvents.Count);

		// and instances ended successfully
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertTwiceAndTerminate.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"}) @Test public void testIntermediateThrowSignalMultipleCancellingReceivers()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertTwiceAndTerminate.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testIntermediateThrowSignalMultipleCancellingReceivers()
	  {
		RecorderExecutionListener.clear();

		runtimeService.startProcessInstanceByKey("catchAlertTwiceAndTerminate");

		// event subscriptions for intermediate events
		assertEquals(2, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert").count());

		// started process instance try to send 'alert' signal to both executions
		runtimeService.startProcessInstanceByKey("throwSignal");

		// then only one terminate end event was executed
		assertEquals(1, RecorderExecutionListener.RecordedEvents.Count);

		// and both instances ended successfully
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"}) @Test public void testAsyncSignalStartEventJobProperties()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"})]
	  public virtual void testAsyncSignalStartEventJobProperties()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = false;

		ProcessDefinition catchingProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("startBySignal").singleResult();

		// given a process instance that throws a signal asynchronously
		runtimeService.startProcessInstanceByKey("throwSignalAsync");
		// where the throwing instance ends immediately

		// then there is not yet a catching process instance
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// but there is a job for the asynchronous continuation
		Job asyncJob = managementService.createJobQuery().singleResult();
		assertEquals(catchingProcessDefinition.Id, asyncJob.ProcessDefinitionId);
		assertEquals(catchingProcessDefinition.Key, asyncJob.ProcessDefinitionKey);
		assertNull(asyncJob.ExceptionMessage);
		assertNull(asyncJob.ExecutionId);
		assertNull(asyncJob.JobDefinitionId);
		assertEquals(0, asyncJob.Priority);
		assertNull(asyncJob.ProcessInstanceId);
		assertEquals(3, asyncJob.Retries);
		assertNull(asyncJob.Duedate);
		assertNull(asyncJob.DeploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"}) @Test public void testAsyncSignalStartEventJobPropertiesDueDateSet()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"})]
	  public virtual void testAsyncSignalStartEventJobPropertiesDueDateSet()
	  {
		DateTime testTime = new DateTime(1457326800000L);
		ClockUtil.CurrentTime = testTime;
		processEngineConfiguration.EnsureJobDueDateNotNull = true;

		ProcessDefinition catchingProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("startBySignal").singleResult();

		// given a process instance that throws a signal asynchronously
		runtimeService.startProcessInstanceByKey("throwSignalAsync");
		// where the throwing instance ends immediately

		// then there is not yet a catching process instance
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// but there is a job for the asynchronous continuation
		Job asyncJob = managementService.createJobQuery().singleResult();
		assertEquals(catchingProcessDefinition.Id, asyncJob.ProcessDefinitionId);
		assertEquals(catchingProcessDefinition.Key, asyncJob.ProcessDefinitionKey);
		assertNull(asyncJob.ExceptionMessage);
		assertNull(asyncJob.ExecutionId);
		assertNull(asyncJob.JobDefinitionId);
		assertEquals(0, asyncJob.Priority);
		assertNull(asyncJob.ProcessInstanceId);
		assertEquals(3, asyncJob.Retries);
		assertEquals(testTime, asyncJob.Duedate);
		assertNull(asyncJob.DeploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"}) @Test public void testAsyncSignalStartEvent()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"})]
	  public virtual void testAsyncSignalStartEvent()
	  {
		ProcessDefinition catchingProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("startBySignal").singleResult();

		// given a process instance that throws a signal asynchronously
		runtimeService.startProcessInstanceByKey("throwSignalAsync");

		// with an async job to trigger the signal event
		Job job = managementService.createJobQuery().singleResult();

		// when the job is executed
		managementService.executeJob(job.Id);

		// then there is a process instance
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
		assertEquals(catchingProcessDefinition.Id, processInstance.ProcessDefinitionId);

		// and a task
		assertEquals(1, taskService.createTaskQuery().count());
	  }

	  /// <summary>
	  /// CAM-4527
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testNoContinuationWhenSignalInterruptsThrowingActivity()
	  public virtual void testNoContinuationWhenSignalInterruptsThrowingActivity()
	  {

		// given a process instance
		runtimeService.startProcessInstanceByKey("signalEventSubProcess");

		// when throwing a signal in the sub process that interrupts the subprocess
		Task subProcessTask = taskService.createTaskQuery().singleResult();
		taskService.complete(subProcessTask.Id);

		// then execution should not have been continued after the subprocess
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(0, taskService.createTaskQuery().taskDefinitionKey("afterSubProcessTask").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml") @Test public void testSetSerializedVariableValues() throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml")]
	  public virtual void testSetSerializedVariableValues()
	  {

		// when
		FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		// then it is not possible to deserialize the object
		try
		{
		  (new ObjectInputStream(new MemoryStream(baos.toByteArray()))).readObject();
		}
		catch (Exception e)
		{
		  testRule.assertTextPresent("Exception while deserializing object.", e.Message);
		}

		// but it can be set as a variable when delivering a message:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		runtimeService.signalEventReceived("alert", Variables.createVariables().putValueTyped("var", Variables.serializedObjectValue(serializedObject).objectTypeName(typeof(FailingJavaSerializable).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()));

		// then
		ProcessInstance startedInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(startedInstance);

		ObjectValue variableTyped = runtimeService.getVariableTyped(startedInstance.Id, "var", false);
		assertNotNull(variableTyped);
		assertFalse(variableTyped.Deserialized);
		assertEquals(serializedObject, variableTyped.ValueSerialized);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
	  }

	  /// <summary>
	  /// CAM-6807
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundary.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"}) @Test @Ignore public void FAILING_testAsyncSignalBoundary()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundary.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml"})]
	  public virtual void FAILING_testAsyncSignalBoundary()
	  {
		runtimeService.startProcessInstanceByKey("catchSignal");

		// given a process instance that throws a signal asynchronously
		runtimeService.startProcessInstanceByKey("throwSignalAsync");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job); // Throws Exception!

		// when the job is executed
		managementService.executeJob(job.Id);

		// then there is a process instance
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
	//    assertEquals(catchingProcessDefinition.getId(), processInstance.getProcessDefinitionId());

		// and a task
		assertEquals(1, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testThrownSignalInEventSubprocessInSubprocess()
	  public virtual void testThrownSignalInEventSubprocessInSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("embeddedEventSubprocess");

		Task taskBefore = taskService.createTaskQuery().singleResult();
		assertNotNull(taskBefore);
		assertEquals("task in subprocess", taskBefore.Name);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		//when job is executed task is created
		managementService.executeJob(job.Id);

		Task taskAfter = taskService.createTaskQuery().singleResult();
		assertNotNull(taskAfter);
		assertEquals("after catch", taskAfter.Name);

		Job jobAfter = managementService.createJobQuery().singleResult();
		assertNull(jobAfter);
	  }

	}

}