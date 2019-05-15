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
namespace org.camunda.bpm.engine.test.bpmn.@event.signal
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Johannes Heinemann
	/// </summary>
	public class SignalEventExpressionNameTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSignalCatchIntermediate()
	  public virtual void testSignalCatchIntermediate()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "TestVar";

		// when
		runtimeService.startProcessInstanceByKey("catchSignal", variables);

		// then
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml"})]
	  public virtual void testSignalCatchIntermediateActsOnEventReceive()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "TestVar";

		// when
		runtimeService.startProcessInstanceByKey("catchSignal", variables);
		runtimeService.signalEventReceived("alert-TestVar");

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalThrowIntermediate.bpmn20.xml"})]
	  public virtual void testSignalThrowCatchIntermediate()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "TestVar";

		// when
		runtimeService.startProcessInstanceByKey("catchSignal", variables);
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
		runtimeService.startProcessInstanceByKey("throwSignal", variables);

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-${var}").count());
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalThrowEnd.bpmn20.xml"})]
	  public virtual void testSignalThrowEndCatchIntermediate()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "TestVar";

		// when
		runtimeService.startProcessInstanceByKey("catchSignal", variables);
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
		runtimeService.startProcessInstanceByKey("throwEndSignal", variables);

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-${var}").count());
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }


	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchBoundary.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalThrowIntermediate.bpmn20.xml"})]
	  public virtual void testSignalCatchBoundary()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "TestVar";
		runtimeService.startProcessInstanceByKey("catchSignal", variables);
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// when
		runtimeService.startProcessInstanceByKey("throwSignal", variables);

		// then
		assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-TestVar").count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalStartEvent.bpmn20.xml"})]
	  public virtual void testSignalStartEvent()
	  {

		// given
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").eventName("alert-foo").count());
		assertEquals(0, taskService.createTaskQuery().count());

		// when
		runtimeService.signalEventReceived("alert-foo");

		// then
		// the signal should start a new process instance
		assertEquals(1, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSignalStartEventInEventSubProcess()
	  public virtual void testSignalStartEventInEventSubProcess()
	  {

		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("signalStartEventInEventSubProcess");
		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());
		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		// when
		runtimeService.signalEventReceived("alert-foo");

		// then
		assertEquals(true, DummyServiceTask.wasExecuted);
		// check if user task doesn't exist because signal start event is interrupting
		taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(0, taskQuery.count());
		// check if execution doesn't exist because signal start event is interrupting
		executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(0, executionQuery.count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.throwAlertSignalAsync.bpmn20.xml"})]
	  public virtual void testAsyncSignalStartEvent()
	  {
		ProcessDefinition catchingProcessDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("startBySignal").singleResult();

		// given a process instance that throws a signal asynchronously
		runtimeService.startProcessInstanceByKey("throwSignalAsync");
		// with an async job to trigger the signal event
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// when the job is executed
		managementService.executeJob(job.Id);

		// then there is a process instance
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertNotNull(processInstance);
		assertEquals(catchingProcessDefinition.Id, processInstance.ProcessDefinitionId);

		// and a task
		assertEquals(1, taskService.createTaskQuery().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml"})]
	  public virtual void testSignalExpressionErrorHandling()
	  {

		string expectedErrorMessage = "Unknown property used in expression: alert-${var}. Cannot resolve identifier 'var'";

		// given an empty variable mapping
		Dictionary<string, object> variables = new Dictionary<string, object>();

		try
		{
		  // when starting the process
		  runtimeService.startProcessInstanceByKey("catchSignal", variables);

		  fail("exception expected: " + expectedErrorMessage);
		}
		catch (ProcessEngineException)
		{
		  // then the expression cannot be resolved and no signal should be available
		  assertEquals(0, runtimeService.createEventSubscriptionQuery().eventType("signal").count());
		}
	  }

	}

}