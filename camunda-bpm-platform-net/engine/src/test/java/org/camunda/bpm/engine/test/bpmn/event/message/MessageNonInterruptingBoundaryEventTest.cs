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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;

	/// 
	/// <summary>
	/// @author Kristin Polenz
	/// </summary>
	public class MessageNonInterruptingBoundaryEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleNonInterruptingBoundaryMessageEvent()
	  public virtual void testSingleNonInterruptingBoundaryMessageEvent()
	  {
		runtimeService.startProcessInstanceByKey("process");

		assertEquals(2, runtimeService.createExecutionQuery().count());

		Task userTask = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		assertNotNull(userTask);

		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNotNull(execution);

		// 1. case: message received before completing the task

		runtimeService.messageEventReceived("messageName", execution.Id);
		// event subscription not removed
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNotNull(execution);

		assertEquals(2, taskService.createTaskQuery().count());

		userTask = taskService.createTaskQuery().taskDefinitionKey("taskAfterMessage").singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// send a message a second time
		runtimeService.messageEventReceived("messageName", execution.Id);
		// event subscription not removed
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNotNull(execution);

		assertEquals(2, taskService.createTaskQuery().count());

		userTask = taskService.createTaskQuery().taskDefinitionKey("taskAfterMessage").singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterMessage", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// now complete the user task with the message boundary event
		userTask = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		assertNotNull(userTask);

		taskService.complete(userTask.Id);

		// event subscription removed
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		userTask = taskService.createTaskQuery().taskDefinitionKey("taskAfterTask").singleResult();
		assertNotNull(userTask);

		taskService.complete(userTask.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// 2nd. case: complete the user task cancels the message subscription

		runtimeService.startProcessInstanceByKey("process");

		userTask = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		assertNotNull(userTask);
		taskService.complete(userTask.Id);

		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName").singleResult();
		assertNull(execution);

		userTask = taskService.createTaskQuery().taskDefinitionKey("taskAfterTask").singleResult();
		assertNotNull(userTask);
		assertEquals("taskAfterTask", userTask.TaskDefinitionKey);
		taskService.complete(userTask.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEventInCombinationWithReceiveTask()
	  public virtual void testNonInterruptingEventInCombinationWithReceiveTask()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(1, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task2Execution).ParentId);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEventInCombinationWithReceiveTaskInConcurrentSubprocess()
	  public virtual void testNonInterruptingEventInCombinationWithReceiveTaskInConcurrentSubprocess()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(2, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);


		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(3, taskService.createTaskQuery().count());
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		Task afterFork = taskService.createTaskQuery().taskDefinitionKey("afterFork").singleResult();
		taskService.complete(afterFork.Id);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task2Execution).ParentId);

		taskService.complete(task2.Id);
		taskService.complete(task1.Id);

		assertProcessEnded(processInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEventInCombinationWithReceiveTaskInsideSubProcess()
	  public virtual void testNonInterruptingEventInCombinationWithReceiveTaskInsideSubProcess()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = instance.Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		ActivityInstance activityInstance = runtimeService.getActivityInstance(instance.Id);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").activity("task1").beginScope("innerSubProcess").activity("receiveTask").done());

		assertEquals(1, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task1Execution).ParentId));

		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task1Execution).ParentId));

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task2Execution).ParentId));

		assertTrue(((ExecutionEntity) task1Execution).ParentId.Equals(((ExecutionEntity) task2Execution).ParentId));

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEventInCombinationWithUserTaskInsideSubProcess()
	  public virtual void testNonInterruptingEventInCombinationWithUserTaskInsideSubProcess()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(2, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Task innerTask = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(innerTask);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("task1").noScope().concurrent().up().child(null).noScope().concurrent().child("innerTask").scope().done());

		// when (2)
		taskService.complete(innerTask.Id);

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);


		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("task1").noScope().concurrent().up().child("task2").noScope().concurrent().done());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEventInCombinationWithUserTask()
	  public virtual void testNonInterruptingEventInCombinationWithUserTask()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(2, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Task innerTask = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(innerTask);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").noScope().concurrent().up().child(null).noScope().concurrent().child("innerTask").scope().done());

		// when (2)
		taskService.complete(innerTask.Id);

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").noScope().concurrent().up().child("task2").noScope().concurrent().done());

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithUserTaskAndBoundaryEvent()
	  public virtual void testNonInterruptingWithUserTaskAndBoundaryEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(2, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task2Execution).ParentId));

		// when (2)
		taskService.complete(task2.Id);

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		task2Execution = runtimeService.createExecutionQuery().activityId("tasks").singleResult();

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedEvents()
	  public virtual void testNestedEvents()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(1, taskService.createTaskQuery().count());

		Task innerTask = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(innerTask);

		Execution innerTaskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) innerTaskExecution).ParentId));

		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		innerTask = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(innerTask);

		innerTaskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) innerTaskExecution).ParentId));

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		assertNotNull(task1Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		// when (3)
		runtimeService.correlateMessage("thirdMessage");

		// then (3)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		assertNotNull(task1Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		assertNotNull(task2Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task2Execution).ParentId);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/message/MessageNonInterruptingBoundaryEventTest.testNestedEvents.bpmn20.xml"})]
	  public virtual void testNestedEventsAnotherExecutionOrder()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("secondMessage");

		// then (1)
		assertEquals(1, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		assertNotNull(task1Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		// when (2)
		runtimeService.correlateMessage("firstMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		Task innerTask = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(innerTask);

		Execution innerTaskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) innerTaskExecution).ParentId));

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		assertNotNull(task1Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		// when (3)
		runtimeService.correlateMessage("thirdMessage");

		// then (3)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		assertNotNull(task1Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task1Execution).ParentId);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		assertNotNull(task2Execution);

		assertEquals(processInstanceId, ((ExecutionEntity) task2Execution).ParentId);

		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

	}

}