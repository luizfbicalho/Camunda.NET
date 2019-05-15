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
namespace org.camunda.bpm.engine.test.bpmn.receivetask
{

	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// see https://app.camunda.com/jira/browse/CAM-1612
	/// 
	/// @author Daniel Meyer
	/// @author Danny Gräf
	/// @author Falko Menge
	/// </summary>
	public class ReceiveTaskTest : PluggableProcessEngineTestCase
	{

	  private IList<EventSubscription> EventSubscriptionList
	  {
		  get
		  {
			return runtimeService.createEventSubscriptionQuery().eventType(EventType.MESSAGE.name()).list();
		  }
	  }

	  private IList<EventSubscription> getEventSubscriptionList(string activityId)
	  {
		return runtimeService.createEventSubscriptionQuery().eventType(EventType.MESSAGE.name()).activityId(activityId).list();
	  }

	  private string getExecutionId(string processInstanceId, string activityId)
	  {
		return runtimeService.createExecutionQuery().processInstanceId(processInstanceId).activityId(activityId).singleResult().Id;
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.simpleReceiveTask.bpmn20.xml")]
	  public virtual void testReceiveTaskWithoutMessageReference()
	  {

		// given: a process instance waiting in the receive task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is no message event subscription created for a receive task without a message reference
		assertEquals(0, EventSubscriptionList.Count);

		// then: we can signal the waiting receive task
		runtimeService.signal(processInstance.Id);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsLegacySignalingOnSingleReceiveTask()
	  {

		// given: a process instance waiting in the receive task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the task
		assertEquals(1, EventSubscriptionList.Count);

		// then: we can signal the waiting receive task
		runtimeService.signal(getExecutionId(processInstance.Id, "waitState"));

		// expect: subscription is removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnSingleReceiveTask()
	  {

		// given: a process instance waiting in the receive task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the task
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		EventSubscription subscription = subscriptionList[0];

		// then: we can trigger the event subscription
		runtimeService.messageEventReceived(subscription.EventName, subscription.ExecutionId);

		// expect: subscription is removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsCorrelateMessageOnSingleReceiveTask()
	  {

		// given: a process instance waiting in the receive task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the task
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		EventSubscription subscription = subscriptionList[0];

		// then: we can correlate the event subscription
		runtimeService.correlateMessage(subscription.EventName);

		// expect: subscription is removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsCorrelateMessageByBusinessKeyOnSingleReceiveTask()
	  {

		// given: a process instance with business key 23 waiting in the receive task
		ProcessInstance processInstance23 = runtimeService.startProcessInstanceByKey("testProcess", "23");

		// given: a 2nd process instance with business key 42 waiting in the receive task
		ProcessInstance processInstance42 = runtimeService.startProcessInstanceByKey("testProcess", "42");

		// expect: there is two message event subscriptions for the tasks
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(2, subscriptionList.Count);

		// then: we can correlate the event subscription to one of the process instances
		runtimeService.correlateMessage("newInvoiceMessage", "23");

		// expect: one subscription is removed
		assertEquals(1, EventSubscriptionList.Count);

		// expect: this ends the process instance with business key 23
		assertProcessEnded(processInstance23.Id);

		// expect: other process instance is still running
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceId(processInstance42.Id).count());

		// then: we can correlate the event subscription to the other process instance
		runtimeService.correlateMessage("newInvoiceMessage", "42");

		// expect: subscription is removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance42.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsLegacySignalingOnSequentialMultiReceiveTask()
	  {

		// given: a process instance waiting in the first receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the first task
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		EventSubscription subscription = subscriptionList[0];
		string firstSubscriptionId = subscription.Id;

		// then: we can signal the waiting receive task
		runtimeService.signal(getExecutionId(processInstance.Id, "waitState"));

		// expect: there is a new subscription created for the second receive task instance
		subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		subscription = subscriptionList[0];
		assertFalse(firstSubscriptionId.Equals(subscription.Id));

		// then: we can signal the second waiting receive task
		runtimeService.signal(getExecutionId(processInstance.Id, "waitState"));

		// expect: no event subscription left
		assertEquals(0, EventSubscriptionList.Count);

		// expect: one user task is created
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnSequentialMultiReceiveTask()
	  {

		// given: a process instance waiting in the first receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the first task
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		EventSubscription subscription = subscriptionList[0];
		string firstSubscriptionId = subscription.Id;

		// then: we can trigger the event subscription
		runtimeService.messageEventReceived(subscription.EventName, subscription.ExecutionId);

		// expect: there is a new subscription created for the second receive task instance
		subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		subscription = subscriptionList[0];
		assertFalse(firstSubscriptionId.Equals(subscription.Id));

		// then: we can trigger the second event subscription
		runtimeService.messageEventReceived(subscription.EventName, subscription.ExecutionId);

		// expect: no event subscription left
		assertEquals(0, EventSubscriptionList.Count);

		// expect: one user task is created
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsCorrelateMessageOnSequentialMultiReceiveTask()
	  {

		// given: a process instance waiting in the first receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the first task
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		EventSubscription subscription = subscriptionList[0];
		string firstSubscriptionId = subscription.Id;

		// then: we can trigger the event subscription
		runtimeService.correlateMessage(subscription.EventName);

		// expect: there is a new subscription created for the second receive task instance
		subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		subscription = subscriptionList[0];
		assertFalse(firstSubscriptionId.Equals(subscription.Id));

		// then: we can trigger the second event subscription
		runtimeService.correlateMessage(subscription.EventName);

		// expect: no event subscription left
		assertEquals(0, EventSubscriptionList.Count);

		// expect: one user task is created
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiParallelReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsLegacySignalingOnParallelMultiReceiveTask()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// expect: there are two executions
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).activityId("waitState").messageEventSubscriptionName("newInvoiceMessage").list();
		assertEquals(2, executions.Count);

		// then: we can signal both waiting receive task
		runtimeService.signal(executions[0].Id);
		runtimeService.signal(executions[1].Id);

		// expect: both event subscriptions are removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiParallelReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnParallelMultiReceiveTask()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// then: we can trigger both event subscriptions
		runtimeService.messageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
		runtimeService.messageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

		// expect: both event subscriptions are removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiParallelReceiveTask.bpmn20.xml")]
	  public virtual void testNotSupportsCorrelateMessageOnParallelMultiReceiveTask()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// then: we can not correlate an event
		try
		{
		  runtimeService.correlateMessage(subscriptions[0].EventName);
		  fail("should throw a mismatch");
		}
		catch (MismatchingMessageCorrelationException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiParallelReceiveTaskCompensate.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnParallelMultiReceiveTaskWithCompensation()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// then: we can trigger the first event subscription
		runtimeService.messageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);

		// expect: after completing the first receive task there is one event subscription for compensation
		assertEquals(1, runtimeService.createEventSubscriptionQuery().eventType(EventType.COMPENSATE.name()).count());

		// then: we can trigger the second event subscription
		runtimeService.messageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

		// expect: there are three event subscriptions for compensation (two subscriptions for tasks and one for miBody)
		assertEquals(3, runtimeService.createEventSubscriptionQuery().eventType(EventType.COMPENSATE.name()).count());

		// expect: one user task is created
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiParallelReceiveTaskBoundary.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnParallelMultiInstanceWithBoundary()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are three message event subscriptions
		assertEquals(3, EventSubscriptionList.Count);

		// expect: there are two message event subscriptions for the receive tasks
		IList<EventSubscription> subscriptions = getEventSubscriptionList("waitState");
		assertEquals(2, subscriptions.Count);

		// then: we can trigger both receive task event subscriptions
		runtimeService.messageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
		runtimeService.messageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

		// expect: all subscriptions are removed (boundary subscription is removed too)
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiParallelReceiveTaskBoundary.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnParallelMultiInstanceWithBoundaryEventReceived()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are three message event subscriptions
		assertEquals(3, EventSubscriptionList.Count);

		// expect: there is one message event subscription for the boundary event
		IList<EventSubscription> subscriptions = getEventSubscriptionList("cancel");
		assertEquals(1, subscriptions.Count);
		EventSubscription subscription = subscriptions[0];

		// then: we can trigger the boundary subscription to cancel both tasks
		runtimeService.messageEventReceived(subscription.EventName, subscription.ExecutionId);

		// expect: all subscriptions are removed (receive task subscriptions too)
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.subProcessReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnSubProcessReceiveTask()
	  {

		// given: a process instance waiting in the sub-process receive task
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there is a message event subscription for the task
		IList<EventSubscription> subscriptionList = EventSubscriptionList;
		assertEquals(1, subscriptionList.Count);
		EventSubscription subscription = subscriptionList[0];

		// then: we can trigger the event subscription
		runtimeService.messageEventReceived(subscription.EventName, subscription.ExecutionId);

		// expect: subscription is removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.multiSubProcessReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnMultiSubProcessReceiveTask()
	  {

		// given: a process instance waiting in two parallel sub-process receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// then: we can trigger both receive task event subscriptions
		runtimeService.messageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
		runtimeService.messageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

		// expect: subscriptions are removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.parallelGatewayReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsMessageEventReceivedOnReceiveTaskBehindParallelGateway()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// then: we can trigger both receive task event subscriptions
		runtimeService.messageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
		runtimeService.messageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

		// expect: subscriptions are removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/receivetask/ReceiveTaskTest.parallelGatewayReceiveTask.bpmn20.xml")]
	  public virtual void testSupportsCorrelateMessageOnReceiveTaskBehindParallelGateway()
	  {

		// given: a process instance waiting in two receive tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// expect: there are two message event subscriptions
		IList<EventSubscription> subscriptions = EventSubscriptionList;
		assertEquals(2, subscriptions.Count);

		// then: we can trigger both receive task event subscriptions
		runtimeService.correlateMessage(subscriptions[0].EventName);
		runtimeService.correlateMessage(subscriptions[1].EventName);

		// expect: subscriptions are removed
		assertEquals(0, EventSubscriptionList.Count);

		// expect: this ends the process instance
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testWaitStateBehavior()
	  public virtual void testWaitStateBehavior()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("receiveTask");
		Execution execution = runtimeService.createExecutionQuery().processInstanceId(pi.Id).activityId("waitState").singleResult();
		assertNotNull(execution);

		runtimeService.signal(execution.Id);
		assertProcessEnded(pi.Id);
	  }
	}

}