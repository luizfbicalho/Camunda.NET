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
	using EventSubscriptionQueryImpl = org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using org.camunda.bpm.engine.runtime;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using TestExecutionListener = org.camunda.bpm.engine.test.util.TestExecutionListener;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// @author Danny Gräf
	/// </summary>
	public class MessageEventSubprocessTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		try
		{
		  base.tearDown();
		}
		finally
		{
		  TestExecutionListener.reset();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingUnderProcessDefinition()
	  public virtual void testInterruptingUnderProcessDefinition()
	  {
		testInterruptingUnderProcessDefinition(1);
	  }

	  /// <summary>
	  /// Checks if unused event subscriptions are properly deleted.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoInterruptingUnderProcessDefinition()
	  public virtual void testTwoInterruptingUnderProcessDefinition()
	  {
		testInterruptingUnderProcessDefinition(2);
	  }

	  private void testInterruptingUnderProcessDefinition(int expectedNumberOfEventSubscriptions)
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// the process instance must have a message event subscription:
		Execution execution = runtimeService.createExecutionQuery().executionId(processInstance.Id).messageEventSubscriptionName("newMessage").singleResult();
		assertNotNull(execution);
		assertEquals(expectedNumberOfEventSubscriptions, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createExecutionQuery().count());

		// if we trigger the usertask, the process terminates and the event subscription is removed:
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task", task.TaskDefinitionKey);
		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// now we start a new instance but this time we trigger the event subprocess:
		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.messageEventReceived("newMessage", processInstance.Id);

		task = taskService.createTaskQuery().singleResult();
		assertEquals("eventSubProcessTask", task.TaskDefinitionKey);
		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventSubprocessListenersInvoked()
	  public virtual void testEventSubprocessListenersInvoked()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.correlateMessage("message");

		Task taskInEventSubProcess = taskService.createTaskQuery().singleResult();
		assertEquals("taskInEventSubProcess", taskInEventSubProcess.TaskDefinitionKey);

		taskService.complete(taskInEventSubProcess.Id);

		IList<string> collectedEvents = TestExecutionListener.collectedEvents;

		assertEquals("taskInMainFlow-start", collectedEvents[0]);
		assertEquals("taskInMainFlow-end", collectedEvents[1]);
		assertEquals("eventSubProcess-start", collectedEvents[2]);
		assertEquals("startEventInSubProcess-start", collectedEvents[3]);
		assertEquals("startEventInSubProcess-end", collectedEvents[4]);
		assertEquals("taskInEventSubProcess-start", collectedEvents[5]);
		assertEquals("taskInEventSubProcess-end", collectedEvents[6]);
		assertEquals("eventSubProcess-end", collectedEvents[7]);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInMainFlow").canceled().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("startEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInEventSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("endEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("eventSubProcess").finished().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEventSubprocessListenersInvoked()
	  public virtual void testNonInterruptingEventSubprocessListenersInvoked()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.correlateMessage("message");

		Task taskInMainFlow = taskService.createTaskQuery().taskDefinitionKey("taskInMainFlow").singleResult();
		assertNotNull(taskInMainFlow);

		Task taskInEventSubProcess = taskService.createTaskQuery().taskDefinitionKey("taskInEventSubProcess").singleResult();
		assertNotNull(taskInEventSubProcess);

		taskService.complete(taskInMainFlow.Id);
		taskService.complete(taskInEventSubProcess.Id);

		IList<string> collectedEvents = TestExecutionListener.collectedEvents;

		assertEquals("taskInMainFlow-start", collectedEvents[0]);
		assertEquals("eventSubProcess-start", collectedEvents[1]);
		assertEquals("startEventInSubProcess-start", collectedEvents[2]);
		assertEquals("startEventInSubProcess-end", collectedEvents[3]);
		assertEquals("taskInEventSubProcess-start", collectedEvents[4]);
		assertEquals("taskInMainFlow-end", collectedEvents[5]);
		assertEquals("taskInEventSubProcess-end", collectedEvents[6]);
		assertEquals("eventSubProcess-end", collectedEvents[7]);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("startEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInMainFlow").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInEventSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("endEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("eventSubProcess").finished().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedEventSubprocessListenersInvoked()
	  public virtual void testNestedEventSubprocessListenersInvoked()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.correlateMessage("message");

		Task taskInEventSubProcess = taskService.createTaskQuery().singleResult();
		assertEquals("taskInEventSubProcess", taskInEventSubProcess.TaskDefinitionKey);

		taskService.complete(taskInEventSubProcess.Id);

		IList<string> collectedEvents = TestExecutionListener.collectedEvents;

		assertEquals("taskInMainFlow-start", collectedEvents[0]);
		assertEquals("taskInMainFlow-end", collectedEvents[1]);
		assertEquals("eventSubProcess-start", collectedEvents[2]);
		assertEquals("startEventInSubProcess-start", collectedEvents[3]);
		assertEquals("startEventInSubProcess-end", collectedEvents[4]);
		assertEquals("taskInEventSubProcess-start", collectedEvents[5]);
		assertEquals("taskInEventSubProcess-end", collectedEvents[6]);
		assertEquals("eventSubProcess-end", collectedEvents[7]);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInMainFlow").canceled().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("startEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInEventSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("endEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("eventSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("subProcess").finished().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedNonInterruptingEventSubprocessListenersInvoked()
	  public virtual void testNestedNonInterruptingEventSubprocessListenersInvoked()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.correlateMessage("message");

		Task taskInMainFlow = taskService.createTaskQuery().taskDefinitionKey("taskInMainFlow").singleResult();
		assertNotNull(taskInMainFlow);

		Task taskInEventSubProcess = taskService.createTaskQuery().taskDefinitionKey("taskInEventSubProcess").singleResult();
		assertNotNull(taskInEventSubProcess);

		taskService.complete(taskInMainFlow.Id);
		taskService.complete(taskInEventSubProcess.Id);

		IList<string> collectedEvents = TestExecutionListener.collectedEvents;

		assertEquals("taskInMainFlow-start", collectedEvents[0]);
		assertEquals("eventSubProcess-start", collectedEvents[1]);
		assertEquals("startEventInSubProcess-start", collectedEvents[2]);
		assertEquals("startEventInSubProcess-end", collectedEvents[3]);
		assertEquals("taskInEventSubProcess-start", collectedEvents[4]);
		assertEquals("taskInMainFlow-end", collectedEvents[5]);
		assertEquals("taskInEventSubProcess-end", collectedEvents[6]);
		assertEquals("eventSubProcess-end", collectedEvents[7]);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInMainFlow").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("startEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInEventSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("endEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("eventSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("subProcess").finished().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventSubprocessBoundaryListenersInvoked()
	  public virtual void testEventSubprocessBoundaryListenersInvoked()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.correlateMessage("message");

		Task taskInEventSubProcess = taskService.createTaskQuery().singleResult();
		assertEquals("taskInEventSubProcess", taskInEventSubProcess.TaskDefinitionKey);

		runtimeService.correlateMessage("message2");

		IList<string> collectedEvents = TestExecutionListener.collectedEvents;


		assertEquals("taskInMainFlow-start", collectedEvents[0]);
		assertEquals("taskInMainFlow-end", collectedEvents[1]);
		assertEquals("eventSubProcess-start", collectedEvents[2]);
		assertEquals("startEventInSubProcess-start", collectedEvents[3]);
		assertEquals("startEventInSubProcess-end", collectedEvents[4]);
		assertEquals("taskInEventSubProcess-start", collectedEvents[5]);
		assertEquals("taskInEventSubProcess-end", collectedEvents[6]);
		assertEquals("eventSubProcess-end", collectedEvents[7]);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInMainFlow").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInMainFlow").canceled().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("startEventInSubProcess").finished().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("taskInEventSubProcess").canceled().count());
		  assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("eventSubProcess").finished().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingUnderProcessDefinition()
	  public virtual void testNonInterruptingUnderProcessDefinition()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// the process instance must have a message event subscription:
		Execution execution = runtimeService.createExecutionQuery().executionId(processInstance.Id).messageEventSubscriptionName("newMessage").singleResult();
		assertNotNull(execution);
		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(1, runtimeService.createExecutionQuery().count());

		// if we trigger the usertask, the process terminates and the event subscription is removed:
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task", task.TaskDefinitionKey);
		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// ###################### now we start a new instance but this time we trigger the event subprocess:
		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.messageEventReceived("newMessage", processInstance.Id);

		assertEquals(2, taskService.createTaskQuery().count());

		// now let's first complete the task in the main flow:
		task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);
		// we still have 2 executions (one for process instance, one for event subprocess):
		assertEquals(2, runtimeService.createExecutionQuery().count());

		// now let's complete the task in the event subprocess
		task = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		taskService.complete(task.Id);
		// done!
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// #################### again, the other way around:

		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.messageEventReceived("newMessage", processInstance.Id);

		assertEquals(2, taskService.createTaskQuery().count());

		task = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		taskService.complete(task.Id);
		// we still have 1 execution:
		assertEquals(1, runtimeService.createExecutionQuery().count());

		task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);
		// done!
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingUnderProcessDefinitionScope()
	  public virtual void testNonInterruptingUnderProcessDefinitionScope()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// the process instance must have a message event subscription:
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("newMessage").singleResult();
		assertNotNull(execution);
		assertEquals(1, createEventSubscriptionQuery().count());
		assertEquals(2, runtimeService.createExecutionQuery().count());

		// if we trigger the usertask, the process terminates and the event subscription is removed:
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task", task.TaskDefinitionKey);
		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// ###################### now we start a new instance but this time we trigger the event subprocess:
		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.correlateMessage("newMessage");

		assertEquals(2, taskService.createTaskQuery().count());
		assertEquals(1, createEventSubscriptionQuery().count());

		// now let's first complete the task in the main flow:
		task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);
		// we still have 2 executions (one for process instance, one for subprocess scope):
		assertEquals(2, runtimeService.createExecutionQuery().count());

		// now let's complete the task in the event subprocess
		task = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		taskService.complete(task.Id);
		// done!
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// #################### again, the other way around:

		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.correlateMessage("newMessage");

		assertEquals(2, taskService.createTaskQuery().count());

		task = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		taskService.complete(task.Id);
		// we still have 2 executions (usertask in main flow is scope):
		assertEquals(2, runtimeService.createExecutionQuery().count());

		task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);
		// done!
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingInEmbeddedSubprocess()
	  public virtual void testNonInterruptingInEmbeddedSubprocess()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// the process instance must have a message event subscription:
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("newMessage").singleResult();
		assertNotNull(execution);
		assertEquals(1, createEventSubscriptionQuery().count());

		// if we trigger the usertask, the process terminates and the event subscription is removed:
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task", task.TaskDefinitionKey);
		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
		assertEquals(0, createEventSubscriptionQuery().count());
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// ###################### now we start a new instance but this time we trigger the event subprocess:
		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.correlateMessage("newMessage");

		assertEquals(2, taskService.createTaskQuery().count());

		// now let's first complete the task in the main flow:
		task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);
		// we still have 3 executions:
		assertEquals(3, runtimeService.createExecutionQuery().count());

		// now let's complete the task in the event subprocess
		task = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		taskService.complete(task.Id);
		// done!
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// #################### again, the other way around:

		processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.correlateMessage("newMessage");

		assertEquals(2, taskService.createTaskQuery().count());

		task = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		taskService.complete(task.Id);
		// we still have 2 executions:
		assertEquals(2, runtimeService.createExecutionQuery().count());

		task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);
		// done!
		assertProcessEnded(processInstance.Id);
		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleNonInterruptingInEmbeddedSubprocess()
	  public virtual void testMultipleNonInterruptingInEmbeddedSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// the process instance must have a message event subscription:
		Execution subProcess = runtimeService.createExecutionQuery().messageEventSubscriptionName("newMessage").singleResult();
		assertNotNull(subProcess);
		assertEquals(1, createEventSubscriptionQuery().count());

		Task subProcessTask = taskService.createTaskQuery().taskDefinitionKey("subProcessTask").singleResult();
		assertNotNull(subProcessTask);

		// start event sub process multiple times
		for (int i = 1; i < 3; i++)
		{
		  runtimeService.messageEventReceived("newMessage", subProcess.Id);

		  // check that now i event sub process tasks exist
		  IList<Task> eventSubProcessTasks = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").list();
		  assertEquals(i, eventSubProcessTasks.Count);
		}

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		// check that the parent execution of the event sub process task execution is the event
		// sub process execution
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("subProcessTask").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().up().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		// complete sub process task
		taskService.complete(subProcessTask.Id);

		// after complete the sub process task all task should be deleted because of the terminating end event
		assertEquals(0, taskService.createTaskQuery().count());

		// and the process instance should be ended
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  private EventSubscriptionQueryImpl createEventSubscriptionQuery()
	  {
		return new EventSubscriptionQueryImpl(processEngineConfiguration.CommandExecutorTxRequired);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingInMultiParallelEmbeddedSubprocess()
	  public virtual void testNonInterruptingInMultiParallelEmbeddedSubprocess()
	  {
		// #################### I. start process and only complete the tasks
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// assert execution tree: scope (process) > scope (subprocess) > 2 x subprocess + usertask
		assertEquals(6, runtimeService.createExecutionQuery().count());

		// expect: two subscriptions, one for each instance
		assertEquals(2, runtimeService.createEventSubscriptionQuery().count());

		// expect: two subprocess instances, i.e. two tasks created
		IList<Task> tasks = taskService.createTaskQuery().list();
		// then: complete both tasks
		foreach (Task task in tasks)
		{
		  assertEquals("subUserTask", task.TaskDefinitionKey);
		  taskService.complete(task.Id);
		}

		// expect: the event subscriptions are removed
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		// then: complete the last task of the main process
		taskService.complete(taskService.createTaskQuery().singleResult().Id);
		assertProcessEnded(processInstance.Id);

		// #################### II. start process and correlate messages to trigger subprocesses instantiation
		processInstance = runtimeService.startProcessInstanceByKey("process");
		foreach (EventSubscription es in runtimeService.createEventSubscriptionQuery().list())
		{
		  runtimeService.messageEventReceived("message", es.ExecutionId); // trigger
		}

		// expect: both subscriptions are remaining and they can be re-triggered as long as the subprocesses are active
		assertEquals(2, runtimeService.createEventSubscriptionQuery().count());

		// expect: two additional task, one for each triggered process
		tasks = taskService.createTaskQuery().taskName("Message User Task").list();
		assertEquals(2, tasks.Count);
		foreach (Task task in tasks)
		{ // complete both tasks
		  taskService.complete(task.Id);
		}

		// then: complete one subprocess
		taskService.complete(taskService.createTaskQuery().taskName("Sub User Task").list().get(0).Id);

		// expect: only the subscription of the second subprocess instance is left
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		// then: trigger the second subprocess again
		runtimeService.messageEventReceived("message", runtimeService.createEventSubscriptionQuery().singleResult().ExecutionId);

		// expect: one message subprocess task exist
		assertEquals(1, taskService.createTaskQuery().taskName("Message User Task").list().size());

		// then: complete all inner subprocess tasks
		tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		// expect: no subscription is left
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		// then: complete the last task of the main process
		taskService.complete(taskService.createTaskQuery().singleResult().Id);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingInMultiSequentialEmbeddedSubprocess()
	  public virtual void testNonInterruptingInMultiSequentialEmbeddedSubprocess()
	  {
		// start process and trigger the first message sub process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.messageEventReceived("message", runtimeService.createEventSubscriptionQuery().singleResult().ExecutionId);

		// expect: one subscription is remaining for the first instance
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		// then: complete both tasks (subprocess and message subprocess)
		taskService.complete(taskService.createTaskQuery().taskName("Message User Task").singleResult().Id);
		taskService.complete(taskService.createTaskQuery().taskName("Sub User Task").list().get(0).Id);

		// expect: the second instance is started
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		// then: just complete this
		taskService.complete(taskService.createTaskQuery().taskName("Sub User Task").list().get(0).Id);

		// expect: no subscription is left
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		// then: complete the last task of the main process
		taskService.complete(taskService.createTaskQuery().singleResult().Id);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithParallelForkInsideEmbeddedSubProcess()
	  public virtual void testNonInterruptingWithParallelForkInsideEmbeddedSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		runtimeService.messageEventReceived("newMessage", runtimeService.createEventSubscriptionQuery().singleResult().ExecutionId);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("firstUserTask").concurrent().noScope().up().child("secondUserTask").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").done());

		IList<Task> tasks = taskService.createTaskQuery().list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithReceiveTask()
	  public virtual void testNonInterruptingWithReceiveTask()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(1, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		// check that the parent execution of the event sub process task execution is the event
		// sub process execution
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("receiveTask").scope().up().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("userTask").singleResult();
		assertNotNull(task2);

		executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		// check that the parent execution of the event sub process task execution is the event
		// sub process execution
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("userTask").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

	  /// <summary>
	  /// CAM-3655
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithAsyncConcurrentTask()
	  public virtual void testNonInterruptingWithAsyncConcurrentTask()
	  {
		// given a process instance with an asyncBefore user task
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// and a triggered non-interrupting subprocess with a user task
		runtimeService.correlateMessage("message");

		// then triggering the async job should be successful
		Job asyncJob = managementService.createJobQuery().singleResult();
		assertNotNull(asyncJob);
		managementService.executeJob(asyncJob.Id);

		// and there should be two tasks now that can be completed successfully
		assertEquals(2, taskService.createTaskQuery().count());
		Task processTask = taskService.createTaskQuery().taskDefinitionKey("userTask").singleResult();
		Task eventSubprocessTask = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();

		assertNotNull(processTask);
		assertNotNull(eventSubprocessTask);

		taskService.complete(processTask.Id);
		taskService.complete(eventSubprocessTask.Id);


		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithReceiveTaskInsideEmbeddedSubProcess()
	  public virtual void testNonInterruptingWithReceiveTaskInsideEmbeddedSubProcess()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(1, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("eventSubProcessTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task1Execution).ParentId));

		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		task1Execution = runtimeService.createExecutionQuery().activityId("eventSubProcessTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task1Execution).ParentId));

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("userTask").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("eventSubProcessTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task2Execution).ParentId));

		// both have the same parent (but it is not the process instance)
		assertTrue(((ExecutionEntity) task1Execution).ParentId.Equals(((ExecutionEntity) task2Execution).ParentId));

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithUserTaskAndBoundaryEventInsideEmbeddedSubProcess()
	  public virtual void testNonInterruptingWithUserTaskAndBoundaryEventInsideEmbeddedSubProcess()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when
		runtimeService.correlateMessage("newMessage");

		// then
		assertEquals(2, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		Execution task1Execution = runtimeService.createExecutionQuery().activityId("eventSubProcessTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task1Execution).ParentId));

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		assertNotNull(task2);

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("eventSubProcessTask").singleResult();

		assertFalse(processInstanceId.Equals(((ExecutionEntity) task2Execution).ParentId));

		// both have the same parent (but it is not the process instance)
		assertTrue(((ExecutionEntity) task1Execution).ParentId.Equals(((ExecutionEntity) task2Execution).ParentId));

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingOutsideEmbeddedSubProcessWithReceiveTaskInsideEmbeddedSubProcess()
	  public virtual void testNonInterruptingOutsideEmbeddedSubProcessWithReceiveTaskInsideEmbeddedSubProcess()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)
		runtimeService.correlateMessage("firstMessage");

		// then (1)
		assertEquals(1, taskService.createTaskQuery().count());

		Task task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		// when (2)
		runtimeService.correlateMessage("secondMessage");

		// then (2)
		assertEquals(2, taskService.createTaskQuery().count());

		task1 = taskService.createTaskQuery().taskDefinitionKey("eventSubProcessTask").singleResult();
		assertNotNull(task1);

		Task task2 = taskService.createTaskQuery().taskDefinitionKey("userTask").singleResult();
		assertNotNull(task2);

		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingActivityInstanceTree()
	  public virtual void testInterruptingActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = instance.Id;

		// when
		runtimeService.correlateMessage("newMessage");

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().endScope().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingActivityInstanceTree()
	  public virtual void testNonInterruptingActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = instance.Id;

		// when
		runtimeService.correlateMessage("newMessage");

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").activity("innerTask").beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().endScope().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingWithTerminatingEndEvent()
	  public virtual void testNonInterruptingWithTerminatingEndEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task.Name, @is("Inner User Task"));
		runtimeService.correlateMessage("message");

		Task eventSubprocessTask = taskService.createTaskQuery().taskName("Event User Task").singleResult();
		assertThat(eventSubprocessTask, @is(notNullValue()));
		taskService.complete(eventSubprocessTask.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("SubProcess_1").activity("UserTask_1").endScope().endScope().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpressionInMessageNameInInterruptingSubProcessDefinition()
	  public virtual void testExpressionInMessageNameInInterruptingSubProcessDefinition()
	  {
		// given an process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// when receiving the message
		runtimeService.messageEventReceived("newMessage-foo", processInstance.Id);

		// the the subprocess is triggered and we can complete the task
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("eventSubProcessTask", task.TaskDefinitionKey);
		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
	  }

	}

}