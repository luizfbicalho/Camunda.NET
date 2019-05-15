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
namespace org.camunda.bpm.engine.test.bpmn.gateway
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class EventBasedGatewayTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/gateway/EventBasedGatewayTest.testCatchAlertAndTimer.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/gateway/EventBasedGatewayTest.throwAlertSignal.bpmn20.xml"})]
	  public virtual void testCatchSignalCancelsTimer()
	  {

		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(1, managementService.createJobQuery().count());

		runtimeService.startProcessInstanceByKey("throwSignal");

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, managementService.createJobQuery().count());

		Task task = taskService.createTaskQuery().taskName("afterSignal").singleResult();

		assertNotNull(task);

		taskService.complete(task.Id);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/gateway/EventBasedGatewayTest.testCatchAlertAndTimer.bpmn20.xml" })]
	  public virtual void testCatchTimerCancelsSignal()
	  {

		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(1, managementService.createJobQuery().count());

		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 10000);
		try
		{
		  // wait for timer to fire
		  waitForJobExecutorToProcessAllJobs(10000);

		  assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
		  assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		  assertEquals(0, managementService.createJobQuery().count());

		  Task task = taskService.createTaskQuery().taskName("afterTimer").singleResult();

		  assertNotNull(task);

		  taskService.complete(task.Id);
		}
		finally
		{
		  ClockUtil.CurrentTime = DateTime.Now;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchSignalAndMessageAndTimer()
	  public virtual void testCatchSignalAndMessageAndTimer()
	  {

		runtimeService.startProcessInstanceByKey("catchSignal");

		assertEquals(2, runtimeService.createEventSubscriptionQuery().count());
		EventSubscriptionQuery messageEventSubscriptionQuery = runtimeService.createEventSubscriptionQuery().eventType("message");
		assertEquals(1, messageEventSubscriptionQuery.count());
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType("signal").count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(1, managementService.createJobQuery().count());

		// we can query for an execution with has both a signal AND message subscription
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoice").signalEventSubscriptionName("alert").singleResult();
		assertNotNull(execution);

		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 10000);
		try
		{

		  EventSubscription messageEventSubscription = messageEventSubscriptionQuery.singleResult();
		  runtimeService.messageEventReceived(messageEventSubscription.EventName, messageEventSubscription.ExecutionId);

		  assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
		  assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		  assertEquals(0, managementService.createJobQuery().count());

		  Task task = taskService.createTaskQuery().taskName("afterMessage").singleResult();

		  assertNotNull(task);

		  taskService.complete(task.Id);
		}
		finally
		{
		  ClockUtil.CurrentTime = DateTime.Now;
		}
	  }

	  public virtual void testConnectedToActitiy()
	  {

		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/gateway/EventBasedGatewayTest.testConnectedToActivity.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  if (!e.Message.contains("Event based gateway can only be connected to elements of type intermediateCatchEvent"))
		  {
			fail("different exception expected");
		  }
		}

	  }

	  public virtual void testInvalidSequenceFlow()
	  {

		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/gateway/EventBasedGatewayTest.testEventInvalidSequenceFlow.bpmn20.xml").deploy();
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  if (!e.Message.contains("Invalid incoming sequenceflow for intermediateCatchEvent"))
		  {
			fail("different exception expected");
		  }
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimeCycle()
	  public virtual void testTimeCycle()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		string jobId = jobQuery.singleResult().Id;
		managementService.executeJob(jobId);

		assertEquals(0, jobQuery.count());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		assertProcessEnded(processInstanceId);
	  }

	}

}