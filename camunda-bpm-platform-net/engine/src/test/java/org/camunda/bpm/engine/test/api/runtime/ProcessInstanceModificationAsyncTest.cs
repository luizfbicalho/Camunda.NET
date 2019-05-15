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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessInstanceModificationAsyncTest : PluggableProcessEngineTestCase
	{

	  protected internal const string EXCLUSIVE_GATEWAY_ASYNC_BEFORE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGatewayAsyncTask.bpmn20.xml";

	  protected internal const string ASYNC_BEFORE_ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.asyncBeforeOneTaskProcess.bpmn20.xml";
	  protected internal const string ASYNC_BEFORE_ONE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.asyncBeforeOneScopeTaskProcess.bpmn20.xml";

	  protected internal const string NESTED_ASYNC_BEFORE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelAsyncBeforeOneTaskProcess.bpmn20.xml";
	  protected internal const string NESTED_ASYNC_BEFORE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelAsyncBeforeOneScopeTaskProcess.bpmn20.xml";
	  protected internal const string NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelAsyncBeforeConcurrentScopeTaskProcess.bpmn20.xml";
	  protected internal const string NESTED_ASYNC_BEFORE_IO_LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelAsyncBeforeOneTaskProcessIoAndListeners.bpmn20.xml";

	  protected internal const string ASYNC_AFTER_ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.asyncAfterOneTaskProcess.bpmn20.xml";

	  protected internal const string NESTED_ASYNC_AFTER_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelAsyncAfterOneTaskProcess.bpmn20.xml";
	  protected internal const string NESTED_ASYNC_AFTER_END_EVENT_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelAsyncAfterEndEventProcess.bpmn20.xml";

	  protected internal const string ASYNC_AFTER_FAILING_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.asyncAfterFailingTaskProcess.bpmn20.xml";
	  protected internal const string ASYNC_BEFORE_FAILING_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.asyncBeforeFailingTaskProcess.bpmn20.xml";

	  [Deployment(resources : EXCLUSIVE_GATEWAY_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testStartBeforeAsync()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").execute();

		// the task does not yet exist because it is started asynchronously
		Task task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNull(task);

		// and there is no activity instance for task2 yet
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").transition("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().done());

		// when the async job is executed
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		executeAvailableJobs();

		// then there is the task
		task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();
		assertNotNull(task);

		// and there is an activity instance for task2
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  /// <summary>
	  /// starting after a task should not respect that tasks asyncAfter setting
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartAfterAsync()
	  public virtual void testStartAfterAsync()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("task2").execute();

		// there is now a job for the end event after task2
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		Execution jobExecution = runtimeService.createExecutionQuery().activityId("end2").executionId(job.ExecutionId).singleResult();
		assertNotNull(jobExecution);

		// end process
		completeTasksInOrder("task1");
		managementService.executeJob(job.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelParentScopeOfAsyncBeforeActivity()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// when I cancel the subprocess
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		// then the process instance is in a valid state
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : NESTED_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelParentScopeOfAsyncBeforeScopeActivity()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// when I cancel the subprocess
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		// then the process instance is in a valid state
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelParentScopeOfParallelAsyncBeforeScopeActivity()
	  {
		// given a process instance with two concurrent async scope tasks in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedConcurrentTasksProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// when I cancel the subprocess
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		// then the process instance is in a valid state
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelAsyncActivityInstanceFails()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// the the async task is not an activity instance so it cannot be cancelled as follows
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask").Id).execute();
		  fail("should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("activityInstance is null", e.Message);
		}
	  }

	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelAsyncBeforeTransitionInstance()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		assertEquals(1, managementService.createJobQuery().count());

		// when the async task is cancelled via cancelTransitionInstance
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask").Id).execute();

		// then the job has been removed
		assertEquals(0, managementService.createJobQuery().count());

		// and the activity instance and execution trees match
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// and the process can be completed successfully
		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);
	  }


	  [Deployment(resources : ASYNC_BEFORE_ONE_TASK_PROCESS)]
	  public virtual void testCancelAsyncBeforeTransitionInstanceEndsProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstanceId).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask").Id).execute();

		// then the process instance has ended
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ASYNC_BEFORE_ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelAsyncBeforeScopeTransitionInstanceEndsProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstanceId).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask").Id).execute();

		// then the process instance has ended
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ASYNC_BEFORE_ONE_TASK_PROCESS)]
	  public virtual void testCancelAndStartAsyncBeforeTransitionInstance()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		Job asyncJob = managementService.createJobQuery().singleResult();

		// when cancelling the only transition instance in the process and immediately starting it again
		runtimeService.createProcessInstanceModification(processInstanceId).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask").Id).startBeforeActivity("theTask").execute();

		// then the activity instance tree should be as before
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("theTask").done());

		// and the async job should be a new one
		Job newAsyncJob = managementService.createJobQuery().singleResult();
		assertFalse(asyncJob.Id.Equals(newAsyncJob.Id));

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("theTask").scope().done());

		// and the process can be completed successfully
		executeAvailableJobs();
		completeTasksInOrder("theTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelNestedConcurrentTransitionInstance()
	  {
		// given a process instance with an instance of outerTask and two asynchronous tasks nested
		// in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedConcurrentTasksProcess");
		string processInstanceId = processInstance.Id;

		// when one of the inner transition instances is cancelled
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask1").Id).execute();

		// then the activity instance and execution trees should match
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").transition("innerTask2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerTask2").scope().done());

		// and the job for innerTask2 should still be there and assigned to the correct execution
		Job innerTask2Job = managementService.createJobQuery().singleResult();
		assertNotNull(innerTask2Job);

		Execution innerTask2Execution = runtimeService.createExecutionQuery().activityId("innerTask2").singleResult();
		assertNotNull(innerTask2Execution);

		assertEquals(innerTask2Job.ExecutionId, innerTask2Execution.Id);

		// and completing the process should succeed
		completeTasksInOrder("outerTask");
		managementService.executeJob(innerTask2Job.Id);
		completeTasksInOrder("innerTask2");

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelNestedConcurrentTransitionInstanceWithConcurrentScopeTask()
	  {
		// given a process instance where the job for innerTask2 is already executed
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedConcurrentTasksProcess");
		string processInstanceId = processInstance.Id;

		Job innerTask2Job = managementService.createJobQuery().activityId("innerTask2").singleResult();
		assertNotNull(innerTask2Job);
		managementService.executeJob(innerTask2Job.Id);

		// when the transition instance to innerTask1 is cancelled
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask1").Id).execute();

		// then the activity instance and execution tree should match
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("innerTask2").scope().done());

		// and there should be no job for innerTask1 anymore
		assertEquals(0, managementService.createJobQuery().activityId("innerTask1").count());

		// and completing the process should succeed
		completeTasksInOrder("innerTask2", "outerTask");

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_ASYNC_BEFORE_IO_LISTENER_PROCESS)]
	  public virtual void testCancelTransitionInstanceShouldNotInvokeIoMappingAndListenersOfTargetActivity()
	  {
		RecorderExecutionListener.clear();

		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		assertEquals(1, managementService.createJobQuery().count());

		// when the async task is cancelled via cancelTransitionInstance
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask").Id).execute();

		// then no io mapping is executed and no end listener is executed
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);
		assertEquals(0, runtimeService.createVariableInstanceQuery().variableName("outputMappingExecuted").count());

		// and the process can be completed successfully
		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : NESTED_ASYNC_AFTER_TASK_PROCESS)]
	  public virtual void testCancelAsyncAfterTransitionInstance()
	  {
		// given a process instance with an asyncAfter task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");

		Task innerTask = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult();
		assertNotNull(innerTask);
		taskService.complete(innerTask.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		assertEquals(1, managementService.createJobQuery().count());

		// when the async task is cancelled via cancelTransitionInstance
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "innerTask").Id).execute();

		// then the job has been removed
		assertEquals(0, managementService.createJobQuery().count());

		// and the activity instance and execution trees match
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// and the process can be completed successfully
		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : NESTED_ASYNC_AFTER_END_EVENT_PROCESS)]
	  public virtual void testCancelAsyncAfterEndEventTransitionInstance()
	  {
		// given a process instance with an asyncAfter end event in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedAsyncEndEventProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		assertEquals(1, managementService.createJobQuery().count());

		// when the async task is cancelled via cancelTransitionInstance
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "subProcessEnd").Id).execute();

		// then the job has been removed
		assertEquals(0, managementService.createJobQuery().count());

		// and the activity instance and execution trees match
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// and the process can be completed successfully
		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : ASYNC_AFTER_ONE_TASK_PROCESS)]
	  public virtual void testCancelAsyncAfterTransitionInstanceEndsProcessInstance()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstanceId).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "theTask").Id).execute();

		// then the process instance has ended
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelAsyncAfterTransitionInstanceInvokesParentListeners()
	  public virtual void testCancelAsyncAfterTransitionInstanceInvokesParentListeners()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstanceId).cancelTransitionInstance(getChildTransitionInstanceForTargetActivity(tree, "subProcessEnd").Id).execute();

		assertEquals(1, RecorderExecutionListener.RecordedEvents.Count);
		RecorderExecutionListener.RecordedEvent @event = RecorderExecutionListener.RecordedEvents[0];
		assertEquals("subProcess", @event.ActivityId);

		RecorderExecutionListener.clear();
	  }

	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelAllCancelsTransitionInstances()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");

		assertEquals(1, managementService.createJobQuery().count());

		// when the async task is cancelled via cancelAll
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("innerTask").execute();

		// then the job has been removed
		assertEquals(0, managementService.createJobQuery().count());

		// and the activity instance and execution trees match
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// and the process can be completed successfully
		completeTasksInOrder("outerTask");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : ASYNC_AFTER_FAILING_TASK_PROCESS)]
	  public virtual void testStartBeforeAsyncAfterTask()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingAfterAsyncTask");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task1").execute();

		// then there are two transition instances of task1
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("task1").transition("task1").done());

		// when all jobs are executed
		executeAvailableJobs();

		// then the tree is still the same, since the jobs failed
		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).transition("task1").transition("task1").done());
	  }

	  [Deployment(resources : ASYNC_AFTER_FAILING_TASK_PROCESS)]
	  public virtual void testStartBeforeAsyncAfterTaskActivityStatistics()
	  {
		// given a process instance with an async task in a subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingAfterAsyncTask");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		// there is one statistics instance
		IList<ActivityStatistics> statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).includeFailedJobs().includeIncidents().list();

		assertEquals(1, statistics.Count);
		assertEquals("task1", statistics[0].Id);
		assertEquals(0, statistics[0].FailedJobs);
		assertEquals(0, statistics[0].IncidentStatistics.Count);
		assertEquals(1, statistics[0].Instances);

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task1").execute();

		// then there are statistics instances of task1
		statistics = managementService.createActivityStatisticsQuery(processInstance.ProcessDefinitionId).includeFailedJobs().includeIncidents().list();

		assertEquals(1, statistics.Count);
		assertEquals("task1", statistics[0].Id);
		assertEquals(0, statistics[0].FailedJobs);
		assertEquals(0, statistics[0].IncidentStatistics.Count);
		assertEquals(2, statistics[0].Instances);


		// when all jobs are executed
		executeAvailableJobs();

	  }


	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelAllTransitionInstanceInScope()
	  {
		// given there are two transition instances in an inner scope
		// and an active activity instance in an outer scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedOneTaskProcess").startBeforeActivity("innerTask").startBeforeActivity("innerTask").startBeforeActivity("outerTask").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		// when i cancel both transition instances
		TransitionInstance[] transitionInstances = tree.getTransitionInstances("innerTask");

		runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance(transitionInstances[0].Id).cancelTransitionInstance(transitionInstances[1].Id).execute();

		// then the outer activity instance is the only one remaining
		tree = runtimeService.getActivityInstance(instance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelTransitionInstanceTwiceFails()
	  {
		// given there are two transition instances in an inner scope
		// and an active activity instance in an outer scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedOneTaskProcess").startBeforeActivity("innerTask").startBeforeActivity("innerTask").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		// when i cancel both transition instances
		TransitionInstance[] transitionInstances = tree.getTransitionInstances("innerTask");

		// this test ensures that the replacedBy link of executions is not followed
		// in case the original execution was actually removed/cancelled
		string transitionInstanceId = transitionInstances[0].Id;
		try
		{
		  runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance(transitionInstanceId).cancelTransitionInstance(transitionInstanceId).execute();
		  fail("should not be possible to cancel the first instance twice");
		}
		catch (NotValidException e)
		{
		  assertTextPresentIgnoreCase("Cannot perform instruction: Cancel transition instance '" + transitionInstanceId + "'; Transition instance '" + transitionInstanceId + "' does not exist: transitionInstance is null", e.Message);
		}
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_ASYNC_BEFORE_TASK_PROCESS)]
	  public virtual void testCancelTransitionInstanceTwiceFailsCase2()
	  {
		// given there are two transition instances in an inner scope
		// and an active activity instance in an outer scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedOneTaskProcess").startBeforeActivity("innerTask").startBeforeActivity("innerTask").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		// when i cancel both transition instances
		TransitionInstance[] transitionInstances = tree.getTransitionInstances("innerTask");

		// this test ensures that the replacedBy link of executions is not followed
		// in case the original execution was actually removed/cancelled

		try
		{
		  runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance(transitionInstances[0].Id).startBeforeActivity("innerTask").startBeforeActivity("innerTask").cancelTransitionInstance(transitionInstances[1].Id).cancelTransitionInstance(transitionInstances[1].Id).execute();
		  fail("should not be possible to cancel the first instance twice");
		}
		catch (NotValidException e)
		{
		  string transitionInstanceId = transitionInstances[1].Id;
		  assertTextPresentIgnoreCase("Cannot perform instruction: Cancel transition instance '" + transitionInstanceId + "'; Transition instance '" + transitionInstanceId + "' does not exist: transitionInstance is null", e.Message);
		}
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelStartCancelInScope()
	  {
		// given there are two transition instances in an inner scope
		// and an active activity instance in an outer scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedConcurrentTasksProcess").startBeforeActivity("innerTask1").startBeforeActivity("innerTask1").startBeforeActivity("outerTask").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		// when i cancel both transition instances
		TransitionInstance[] transitionInstances = tree.getTransitionInstances("innerTask1");

		runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance(transitionInstances[0].Id).startBeforeActivity("innerTask2").cancelTransitionInstance(transitionInstances[1].Id).execute();

		// then the outer activity instance is the only one remaining
		tree = runtimeService.getActivityInstance(instance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").transition("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerTask2").scope().done());
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testStartAndCancelAllForTransitionInstance()
	  {
		// given there is one transition instance in a scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedConcurrentTasksProcess").startBeforeActivity("innerTask1").startBeforeActivity("innerTask1").startBeforeActivity("innerTask1").execute();

		// when I start an activity in the same scope
		// and cancel the first transition instance
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("innerTask2").cancelAllForActivity("innerTask1").execute();

		// then the activity was successfully instantiated
		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").transition("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("innerTask2").scope().done());
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testRepeatedStartAndCancellationForTransitionInstance()
	  {
		// given there is one transition instance in a scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedConcurrentTasksProcess").startBeforeActivity("innerTask1").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		TransitionInstance transitionInstance = tree.getTransitionInstances("innerTask1")[0];

		// when I start an activity in the same scope
		// and cancel the first transition instance
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("innerTask2").cancelAllForActivity("innerTask2").startBeforeActivity("innerTask2").cancelAllForActivity("innerTask2").startBeforeActivity("innerTask2").cancelAllForActivity("innerTask2").cancelTransitionInstance(transitionInstance.Id).execute();

		// then the process has ended
		assertProcessEnded(instance.Id);
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testRepeatedCancellationAndStartForTransitionInstance()
	  {
		// given there is one transition instance in a scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedConcurrentTasksProcess").startBeforeActivity("innerTask1").startBeforeActivity("innerTask1").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		TransitionInstance[] transitionInstances = tree.getTransitionInstances("innerTask1");

		// when I start an activity in the same scope
		// and cancel the first transition instance
		runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance(transitionInstances[0].Id).startBeforeActivity("innerTask2").cancelAllForActivity("innerTask2").startBeforeActivity("innerTask2").cancelAllForActivity("innerTask2").startBeforeActivity("innerTask2").cancelTransitionInstance(transitionInstances[1].Id).execute();

		// then there is only an activity instance for innerTask2
		tree = runtimeService.getActivityInstance(instance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").transition("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("innerTask2").scope().done());
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testStartBeforeAndCancelSingleTransitionInstance()
	  {
		// given there is one transition instance in a scope
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedConcurrentTasksProcess").startBeforeActivity("innerTask1").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		TransitionInstance transitionInstance = tree.getTransitionInstances("innerTask1")[0];

		// when I start an activity in the same scope
		// and cancel the first transition instance
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("innerTask2").cancelTransitionInstance(transitionInstance.Id).execute();

		// then the activity was successfully instantiated
		tree = runtimeService.getActivityInstance(instance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").transition("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("innerTask2").scope().done());
	  }

	  /// <summary>
	  /// CAM-4090
	  /// </summary>
	  [Deployment(resources : NESTED_PARALLEL_ASYNC_BEFORE_SCOPE_TASK_PROCESS)]
	  public virtual void testStartBeforeSyncEndAndCancelSingleTransitionInstance()
	  {
		// given there is one transition instance in a scope and an outer activity instance
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("nestedConcurrentTasksProcess").startBeforeActivity("outerTask").startBeforeActivity("innerTask1").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		TransitionInstance transitionInstance = tree.getTransitionInstances("innerTask1")[0];

		// when I start an activity in the same scope that ends immediately
		// and cancel the first transition instance
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("subProcessEnd2").cancelTransitionInstance(transitionInstance.Id).execute();

		// then only the outer activity instance is left
		tree = runtimeService.getActivityInstance(instance.Id);

		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(instance.Id, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());
	  }

	  [Deployment(resources : ASYNC_BEFORE_FAILING_TASK_PROCESS)]
	  public virtual void testRestartAFailedServiceTask()
	  {
		// given a failed job
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("failingAfterBeforeTask").startBeforeActivity("task2").execute();

		executeAvailableJobs();
		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);

		// when the service task is restarted
		ActivityInstance tree = runtimeService.getActivityInstance(instance.Id);
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("task2").cancelTransitionInstance(tree.getTransitionInstances("task2")[0].Id).execute();

		executeAvailableJobs();

		// then executing the task has failed again and there is a new incident
		Incident newIncident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(newIncident);

		assertNotSame(incident.Id, newIncident.Id);
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

	  protected internal virtual TransitionInstance getChildTransitionInstanceForTargetActivity(ActivityInstance activityInstance, string targetActivityId)
	  {
		foreach (TransitionInstance childTransitionInstance in activityInstance.ChildTransitionInstances)
		{
		  if (targetActivityId.Equals(childTransitionInstance.ActivityId))
		  {
			return childTransitionInstance;
		  }
		}

		foreach (ActivityInstance childInstance in activityInstance.ChildActivityInstances)
		{
		  TransitionInstance instance = getChildTransitionInstanceForTargetActivity(childInstance, targetActivityId);
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