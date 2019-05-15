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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.startsWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using RecorderTaskListener = org.camunda.bpm.engine.test.bpmn.tasklistener.util.RecorderTaskListener;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;

	/// <summary>
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public class SingleProcessInstanceModificationAsyncTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PARALLEL_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGateway.bpmn20.xml";
	  protected internal const string EXCLUSIVE_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGateway.bpmn20.xml";
	  protected internal const string SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocess.bpmn20.xml";
	  protected internal const string ONE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.oneScopeTaskProcess.bpmn20.xml";
	  protected internal const string TRANSITION_LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.transitionListeners.bpmn20.xml";
	  protected internal const string TASK_LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.taskListeners.bpmn20.xml";
	  protected internal const string IO_MAPPING_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.ioMapping.bpmn20.xml";
	  protected internal const string CALL_ACTIVITY_PARENT_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.testCancelCallActivityParentProcess.bpmn";
	  protected internal const string CALL_ACTIVITY_CHILD_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.testCancelCallActivityChildProcess.bpmn";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		IList<Batch> batches = managementService.createBatchQuery().list();
		foreach (Batch batch in batches)
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  managementService.deleteJob(job.Id);
		}
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testTheDeploymentIdIsSet()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processDefinitionId = processInstance.ProcessDefinitionId;
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionId(processDefinitionId).singleResult();

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).executeAsync();
		assertNotNull(modificationBatch);
		Job job = managementService.createJobQuery().jobDefinitionId(modificationBatch.SeedJobDefinitionId).singleResult();
		// seed job
		managementService.executeJob(job.Id);

		foreach (Job pending in managementService.createJobQuery().jobDefinitionId(modificationBatch.BatchJobDefinitionId).list())
		{
		  managementService.executeJob(pending.Id);
		  assertEquals(processDefinition.DeploymentId, pending.DeploymentId);
		}
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task2").scope().done());

		// complete the process
		completeTasksInOrder("task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testCancellationThatEndsProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).cancelActivityInstance(getInstanceIdForActivity(tree, "task2")).executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testCancellationWithWrongProcessInstanceId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification("foo").cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).cancelActivityInstance(getInstanceIdForActivity(tree, "task2")).executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  assertProcessEnded(processInstance.Id);

		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, startsWith("ENGINE-13036"));
		  assertThat(e.Message, containsString("Process instance '" + "foo" + "' cannot be modified"));
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartBefore()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		assertEquals(2, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartBeforeWithAncestorInstanceId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2", tree.Id).executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		assertEquals(2, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstanceId);
	  }


	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartBeforeNonExistingActivity()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		try
		{
		  // when
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("someNonExistingActivity").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  // then
		  assertTextPresentIgnoreCase("element 'someNonExistingActivity' does not exist in process ", e.Message);
		}
	  }

	  /// <summary>
	  /// CAM-3718
	  /// </summary>
	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testEndProcessInstanceIntermediately()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startAfterActivity("task1").startBeforeActivity("task1").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task1").scope().done());

		assertEquals(1, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartTransition()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow4").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		assertEquals(2, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartTransitionWithAncestorInstanceId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow4", tree.Id).executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		assertEquals(2, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartTransitionInvalidTransitionId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstanceId).startTransition("invalidFlowId").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);

		  fail("should not suceed");

		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start transition 'invalidFlowId'; " + "Element 'invalidFlowId' does not exist in process '" + processInstance.ProcessDefinitionId + "'", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartAfter()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theStart").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task1").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task1").concurrent().noScope().done());

		assertEquals(2, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1", "task1");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartAfterWithAncestorInstanceId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theStart", tree.Id).executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task1").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task1").concurrent().noScope().done());

		assertEquals(2, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("task1", "task1");
		assertProcessEnded(processInstanceId);
	  }


	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartAfterActivityAmbiguousTransitions()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstanceId).startAfterActivity("fork").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not suceed since 'fork' has more than one outgoing sequence flow");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("activity has more than one outgoing sequence flow", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartAfterActivityNoOutgoingTransitions()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstanceId).startAfterActivity("theEnd").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not suceed since 'theEnd' has no outgoing sequence flow");

		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("activity has no outgoing sequence flow to take", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartAfterNonExistingActivity()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		try
		{
		  // when
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(instance.Id).startAfterActivity("someNonExistingActivity").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  // then
		  assertTextPresentIgnoreCase("Cannot perform instruction: " + "Start after activity 'someNonExistingActivity'; " + "Activity 'someNonExistingActivity' does not exist: activity is null", e.Message);
		}
	  }

	  [Deployment(resources : ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testScopeTaskStartBefore()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").activity("theTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("theTask").scope().up().up().child(null).concurrent().noScope().child("theTask").scope().done());

		assertEquals(2, taskService.createTaskQuery().count());
		completeTasksInOrder("theTask", "theTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testScopeTaskStartAfter()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		// when starting after the task, essentially nothing changes in the process
		// instance
		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theTask").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("theTask").scope().done());

		// when starting after the start event, regular concurrency happens
		Batch modificationBatch2 = runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theStart").executeAsync();
		assertNotNull(modificationBatch2);
		executeSeedAndBatchJobs(modificationBatch2);

		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").activity("theTask").done());

		executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("theTask").scope().up().up().child(null).concurrent().noScope().child("theTask").scope().done());

		completeTasksInOrder("theTask", "theTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : TASK_LISTENER_PROCESS)]
	  public virtual void testSkipTaskListenerInvocation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskListenerProcess", "brum", Collections.singletonMap<string, object> ("listener", new RecorderTaskListener()));

		string processInstanceId = processInstance.Id;

		RecorderTaskListener.clear();

		// when I start an activity with "skip listeners" setting
		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("task").executeAsync(true, false);
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		// then no listeners are invoked
		assertTrue(RecorderTaskListener.RecordedEvents.Count == 0);

		// when I cancel an activity with "skip listeners" setting
		ActivityInstance activityInstanceTree = runtimeService.getActivityInstance(processInstanceId);

		Batch batch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "task").Id).executeAsync(true, false);
		assertNotNull(batch);
		executeSeedAndBatchJobs(batch);

		// then no listeners are invoked
		assertTrue(RecorderTaskListener.RecordedEvents.Count == 0);
	  }

	  [Deployment(resources : IO_MAPPING_PROCESS)]
	  public virtual void testSkipIoMappings()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("ioMappingProcess");

		// when I start task2
		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").executeAsync(false, true);
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		// then the input mapping should not have executed
		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		assertNotNull(task2Execution);

		assertNull(runtimeService.getVariable(task2Execution.Id, "inputMappingExecuted"));

		// when I cancel task2
		Batch modificationBatch2 = runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("task2").executeAsync(false, true);
		assertNotNull(modificationBatch2);
		executeSeedAndBatchJobs(modificationBatch2);

		// then the output mapping should not have executed
		assertNull(runtimeService.getVariable(processInstance.Id, "outputMappingExecuted"));
	  }

	  [Deployment(resources : TRANSITION_LISTENER_PROCESS)]
	  public virtual void testStartTransitionListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("transitionListenerProcess", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));

		Batch modificationBatch = runtimeService.createProcessInstanceModification(instance.Id).startTransition("flow2").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		// transition listener should have been invoked
		IList<RecorderExecutionListener.RecordedEvent> events = RecorderExecutionListener.RecordedEvents;
		assertEquals(1, events.Count);

		RecorderExecutionListener.RecordedEvent @event = events[0];
		assertEquals("flow2", @event.TransitionId);

		RecorderExecutionListener.clear();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);
		assertEquals(instance.Id, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		completeTasksInOrder("task1", "task2", "task2");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : TRANSITION_LISTENER_PROCESS)]
	  public virtual void testStartAfterActivityListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("transitionListenerProcess", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));

		Batch modificationBatch = runtimeService.createProcessInstanceModification(instance.Id).startTransition("flow2").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		// transition listener should have been invoked
		IList<RecorderExecutionListener.RecordedEvent> events = RecorderExecutionListener.RecordedEvents;
		assertEquals(1, events.Count);

		RecorderExecutionListener.RecordedEvent @event = events[0];
		assertEquals("flow2", @event.TransitionId);

		RecorderExecutionListener.clear();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);
		assertEquals(instance.Id, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		completeTasksInOrder("task1", "task2", "task2");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testCancellationAndStartBefore()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("task2").executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		ActivityInstance activityInstanceTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(activityInstanceTree);
		assertEquals(processInstanceId, activityInstanceTree.ProcessInstanceId);

		assertThat(activityInstanceTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task2").scope().done());

		completeTasksInOrder("task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testCancelNonExistingActivityInstance()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		// when - then throw exception
		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(instance.Id).cancelActivityInstance("nonExistingActivityInstance").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  assertTextPresent("Cannot perform instruction: Cancel activity instance 'nonExistingActivityInstance'; " + "Activity instance 'nonExistingActivityInstance' does not exist", e.Message);
		}

	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testCancelNonExistingTranisitionInstance()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		// when - then throw exception
		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance("nonExistingActivityInstance").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  assertTextPresent("Cannot perform instruction: Cancel transition instance 'nonExistingActivityInstance'; " + "Transition instance 'nonExistingActivityInstance' does not exist", e.Message);
		}

	  }

	  [Deployment(resources : { CALL_ACTIVITY_PARENT_PROCESS, CALL_ACTIVITY_CHILD_PROCESS })]
	  public virtual void testCancelCallActivityInstance()
	  {
		// given
		ProcessInstance parentprocess = runtimeService.startProcessInstanceByKey("parentprocess");
		ProcessInstance subProcess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();

		ActivityInstance subProcessActivityInst = runtimeService.getActivityInstance(subProcess.Id);

		// when
		Batch modificationBatch = runtimeService.createProcessInstanceModification(subProcess.Id).startBeforeActivity("childEnd", subProcess.Id).cancelActivityInstance(getInstanceIdForActivity(subProcessActivityInst, "innerTask")).executeAsync();
		assertNotNull(modificationBatch);
		executeSeedAndBatchJobs(modificationBatch);

		// then
		assertProcessEnded(parentprocess.Id);
	  }

	  public virtual void testModifyNullProcessInstance()
	  {
		try
		{
		  Batch modificationBatch = runtimeService.createProcessInstanceModification(null).startBeforeActivity("someActivity").executeAsync();
		  assertNotNull(modificationBatch);
		  executeSeedAndBatchJobs(modificationBatch);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  assertTextPresent("processInstanceId is null", e.Message);
		}
	  }

	  protected internal virtual void executeSeedAndBatchJobs(Batch batch)
	  {
		Job job = managementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();
		// seed job
		managementService.executeJob(job.Id);


		foreach (Job pending in managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		{
		  managementService.executeJob(pending.Id);
		}
	  }

	  protected internal virtual string getInstanceIdForActivity(ActivityInstance activityInstance, string activityId)
	  {
		ActivityInstance instance = getChildInstanceForActivity(activityInstance, activityId);
		if (instance != null)
		{
		  return instance.Id;
		}
		return null;
	  }

	  protected internal virtual ActivityInstance getChildInstanceForActivity(ActivityInstance activityInstance, string activityId)
	  {
		if (activityId.Equals(activityInstance.ActivityId))
		{
		  return activityInstance;
		}

		foreach (ActivityInstance childInstance in activityInstance.ChildActivityInstances)
		{
		  ActivityInstance instance = getChildInstanceForActivity(childInstance, activityId);
		  if (instance != null)
		  {
			return instance;
		  }
		}

		return null;
	  }

	  protected internal virtual void completeTasksInOrder(params string[] taskNames)
	  {
		foreach (string taskName in taskNames)
		{
		  // complete any task with that name
		  IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey(taskName).listPage(0, 1);
		  assertTrue("task for activity " + taskName + " does not exist", tasks.Count > 0);
		  taskService.complete(tasks[0].Id);
		}
	  }
	}

}