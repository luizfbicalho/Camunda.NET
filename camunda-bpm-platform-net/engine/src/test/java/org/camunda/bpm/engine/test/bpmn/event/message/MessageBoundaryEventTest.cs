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
namespace org.camunda.bpm.engine.test.bpmn.@event.message
{
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;



	/// <summary>
	/// @author Daniel Meyer (camunda)
	/// @author Kristin Polenz (camunda)
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	public class MessageBoundaryEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleBoundaryMessageEvent()
	  public virtual void testSingleBoundaryMessageEvent()
	  {
		runtimeService.startProcessInstanceByKey("process");

		assertEquals(2, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);

		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNotNull(execution);

		// 1. case: message received cancels the task

		runtimeService.messageEventReceived("messageName", execution.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// 2nd. case: complete the user task cancels the message subscription

		runtimeService.startProcessInstanceByKey("process");

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		taskService.complete(userTask.Id);

		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterTask", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

	  public virtual void testDoubleBoundaryMessageEventSameMessageId()
	  {
		// deployment fails when two boundary message events have the same messageId
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/message/MessageBoundaryEventTest.testDoubleBoundaryMessageEventSameMessageId.bpmn20.xml").deploy();
		  fail("Deployment should fail because Activiti cannot handle two boundary message events with same messageId.");
		}
		catch (Exception e)
		{
		  assertTextPresent("Cannot have more than one message event subscription with name 'messageName' for scope 'task'", e.Message);
		  assertEquals(0, repositoryService.createDeploymentQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDoubleBoundaryMessageEvent()
	  public virtual void testDoubleBoundaryMessageEvent()
	  {
		runtimeService.startProcessInstanceByKey("process");

		assertEquals(2, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);

		// the executions for both messageEventSubscriptionNames are the same
		Execution execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();
		assertNotNull(execution1);

		Execution execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();
		assertNotNull(execution2);

		assertEquals(execution1.Id, execution2.Id);

		///////////////////////////////////////////////////////////////////////////////////
		// 1. first message received cancels the task and the execution and both subscriptions
		runtimeService.messageEventReceived("messageName_1", execution1.Id);

		// this should then throw an exception because execution2 no longer exists
		try
		{
		  runtimeService.messageEventReceived("messageName_2", execution2.Id);
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("does not have a subscription to a message event with name 'messageName_2'", e.Message);
		}

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage_1", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		/////////////////////////////////////////////////////////////////////
		// 2. complete the user task cancels the message subscriptions

		runtimeService.startProcessInstanceByKey("process");

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		taskService.complete(userTask.Id);

		execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();
		assertNull(execution1);
		execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();
		assertNull(execution2);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterTask", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDoubleBoundaryMessageEventMultiInstance()
	  public virtual void testDoubleBoundaryMessageEventMultiInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		// assume we have 7 executions
		// one process instance
		// one execution for scope created for boundary message event
		// five execution because we have loop cardinality 5
		assertEquals(7, runtimeService.createExecutionQuery().count());

		assertEquals(5, taskService.createTaskQuery().count());

		Execution execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();
		Execution execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();
		// both executions are the same
		assertEquals(execution1.Id, execution2.Id);

		///////////////////////////////////////////////////////////////////////////////////
		// 1. first message received cancels all tasks and the executions and both subscriptions
		runtimeService.messageEventReceived("messageName_1", execution1.Id);

		// this should then throw an exception because execution2 no longer exists
		try
		{
		  runtimeService.messageEventReceived("messageName_2", execution2.Id);
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("does not have a subscription to a message event with name 'messageName_2'", e.Message);
		}

		// only process instance left
		assertEquals(1, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage_1", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertProcessEnded(processInstance.Id);


		///////////////////////////////////////////////////////////////////////////////////
		// 2. complete the user task cancels the message subscriptions

		processInstance = runtimeService.startProcessInstanceByKey("process");
		// assume we have 7 executions
		// one process instance
		// one execution for scope created for boundary message event
		// five execution because we have loop cardinality 5
		assertEquals(7, runtimeService.createExecutionQuery().count());

		assertEquals(5, taskService.createTaskQuery().count());

		execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();
		execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();
		// both executions are the same
		assertEquals(execution1.Id, execution2.Id);

		IList<Task> userTasks = taskService.createTaskQuery().list();
		assertNotNull(userTasks);
		assertEquals(5, userTasks.Count);

		// as long as tasks exists, the message subscriptions exist
		for (int i = 0; i < userTasks.Count - 1; i++)
		{
		  Task task = userTasks[i];
		  taskService.complete(task.Id);

		  execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();
		  assertNotNull(execution1);
		  execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();
		  assertNotNull(execution2);
		}

		// only one task left
		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		taskService.complete(userTask.Id);

		// after last task is completed, no message subscriptions left
		execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();
		assertNull(execution1);
		execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();
		assertNull(execution2);

		// complete last task to end process
		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterTask", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryMessageEventInsideSubprocess()
	  public virtual void testBoundaryMessageEventInsideSubprocess()
	  {

		// this time the boundary events are placed on a user task that is contained inside a sub process

		runtimeService.startProcessInstanceByKey("process");

		assertEquals(3, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);

		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNotNull(execution);

		///////////////////////////////////////////////////
		// 1. case: message received cancels the task

		runtimeService.messageEventReceived("messageName", execution.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		///////////////////////////////////////////////////
		// 2nd. case: complete the user task cancels the message subscription

		runtimeService.startProcessInstanceByKey("process");

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		taskService.complete(userTask.Id);

		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterTask", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryMessageEventOnSubprocessAndInsideSubprocess()
	  public virtual void testBoundaryMessageEventOnSubprocessAndInsideSubprocess()
	  {

		// this time the boundary events are placed on a user task that is contained inside a sub process
		// and on the subprocess itself

		runtimeService.startProcessInstanceByKey("process");

		assertEquals(3, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);

		Execution execution1 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNotNull(execution1);

		Execution execution2 = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		assertNotNull(execution2);

		assertNotSame(execution1.Id, execution2.Id);

		/////////////////////////////////////////////////////////////
		// first case: we complete the inner usertask.

		taskService.complete(userTask.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterTask", userTask.TaskDefinitionKey);

		// the inner subscription is cancelled
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		// the outer subscription still exists
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		assertNotNull(execution);

		// now complete the second usertask
		taskService.complete(userTask.Id);

		// now the outer event subscription is cancelled as well
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		assertNull(execution);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterSubprocess", userTask.TaskDefinitionKey);

		// now complete the outer usertask
		taskService.complete(userTask.Id);

		/////////////////////////////////////////////////////////////
		// second case: we signal the inner message event

		runtimeService.startProcessInstanceByKey("process");

		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		runtimeService.messageEventReceived("messageName", execution.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage", userTask.TaskDefinitionKey);

		// the inner subscription is removed
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		// the outer subscription still exists
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		assertNotNull(execution);

		// now complete the second usertask
		taskService.complete(userTask.Id);

		// now the outer event subscription is cancelled as well
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		assertNull(execution);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterSubprocess", userTask.TaskDefinitionKey);

		// now complete the outer usertask
		taskService.complete(userTask.Id);

		/////////////////////////////////////////////////////////////
		// third case: we signal the outer message event

		runtimeService.startProcessInstanceByKey("process");

		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		runtimeService.messageEventReceived("messageName2", execution.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterOuterMessageBoundary", userTask.TaskDefinitionKey);

		// the inner subscription is removed
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		// the outer subscription is removed
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").singleResult();
		assertNull(execution);

		// now complete the second usertask
		taskService.complete(userTask.Id);

		// and we are done

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryMessageEventOnSubprocess()
	  public virtual void testBoundaryMessageEventOnSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("process");

		assertEquals(2, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);

		// 1. case: message one received cancels the task

		Execution executionMessageOne = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_one").singleResult();
		assertNotNull(executionMessageOne);

		runtimeService.messageEventReceived("messageName_one", executionMessageOne.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage_one", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// 2nd. case: message two received cancels the task

		runtimeService.startProcessInstanceByKey("process");

		Execution executionMessageTwo = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_two").singleResult();
		assertNotNull(executionMessageTwo);

		runtimeService.messageEventReceived("messageName_two", executionMessageTwo.Id);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage_two", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());


		// 3rd. case: complete the user task cancels the message subscription

		runtimeService.startProcessInstanceByKey("process");

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		taskService.complete(userTask.Id);

		executionMessageOne = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_one").singleResult();
		assertNull(executionMessageOne);

		executionMessageTwo = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_two").singleResult();
		assertNull(executionMessageTwo);

		userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterSubProcess", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryMessageEventOnSubprocessWithIntermediateMessageCatch()
	  public virtual void testBoundaryMessageEventOnSubprocessWithIntermediateMessageCatch()
	  {

		// given
		// a process instance waiting inside the intermediate message catch inside the subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// when
		// I cancel the subprocess
		runtimeService.correlateMessage("cancelMessage");

		// then
		// the process instance is ended
		assertProcessEnded(processInstance.Id);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // and all activity instances in history have an end time set
		  IList<HistoricActivityInstance> hais = historyService.createHistoricActivityInstanceQuery().list();
		  foreach (HistoricActivityInstance historicActivityInstance in hais)
		  {
			assertNotNull(historicActivityInstance.EndTime);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryMessageEventOnSubprocessAndInsideSubprocessMultiInstance()
	  public virtual void testBoundaryMessageEventOnSubprocessAndInsideSubprocessMultiInstance()
	  {

		// this time the boundary events are placed on a user task that is contained inside a sub process
		// and on the subprocess itself

		runtimeService.startProcessInstanceByKey("process");

		assertEquals(17, runtimeService.createExecutionQuery().count());

		// 5 user tasks
		IList<Task> userTasks = taskService.createTaskQuery().list();
		assertNotNull(userTasks);
		assertEquals(5, userTasks.Count);

		// there are 5 event subscriptions to the event on the inner user task
		IList<Execution> executions = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").list();
		assertNotNull(executions);
		assertEquals(5, executions.Count);

		// there is a single event subscription for the event on the subprocess
		executions = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName2").list();
		assertNotNull(executions);
		assertEquals(1, executions.Count);

		// if we complete the outer message event, all inner executions are removed
		Execution outerScopeExecution = executions[0];
		runtimeService.messageEventReceived("messageName2", outerScopeExecution.Id);

		executions = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").list();
		assertEquals(0, executions.Count);

		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterOuterMessageBoundary", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);

		// and we are done

	  }

	  /// <summary>
	  /// Triggering one boundary event should not remove the event subscription
	  /// of a boundary event for a concurrent task
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryMessageEventConcurrent()
	  public virtual void testBoundaryMessageEventConcurrent()
	  {
		runtimeService.startProcessInstanceByKey("boundaryEvent");

		EventSubscription eventSubscriptionTask1 = runtimeService.createEventSubscriptionQuery().activityId("messageBoundary1").singleResult();
		assertNotNull(eventSubscriptionTask1);

		EventSubscription eventSubscriptionTask2 = runtimeService.createEventSubscriptionQuery().activityId("messageBoundary2").singleResult();
		assertNotNull(eventSubscriptionTask2);

		// when I trigger the boundary event for task1
		runtimeService.correlateMessage("task1Message");

		// then the event subscription for task2 still exists
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertNotNull(runtimeService.createEventSubscriptionQuery().activityId("messageBoundary2").singleResult());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpressionInBoundaryMessageEventName()
	  public virtual void testExpressionInBoundaryMessageEventName()
	  {

		// given a process instance with its variables
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "bar";
		runtimeService.startProcessInstanceByKey("process", variables);


		// when message is received
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName-bar").singleResult();
		assertNotNull(execution);
		runtimeService.messageEventReceived("messageName-bar", execution.Id);

		// then then a task should be completed
		Task userTask = taskService.createTaskQuery().singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

	  }

	}

}