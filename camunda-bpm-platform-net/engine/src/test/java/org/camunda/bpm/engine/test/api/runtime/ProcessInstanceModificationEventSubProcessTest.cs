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

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessInstanceModificationEventSubProcessTest : PluggableProcessEngineTestCase
	{

	  protected internal const string INTERRUPTING_EVENT_SUBPROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.interruptingEventSubProcess.bpmn20.xml";
	  protected internal const string NON_INTERRUPTING_EVENT_SUBPROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nonInterruptingEventSubProcess.bpmn20.xml";
	  protected internal const string INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.interruptingEventSubProcessInsideSubProcess.bpmn20.xml";
	  protected internal const string NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nonInterruptingEventSubProcessInsideSubProcess.bpmn20.xml";
	  protected internal const string CANCEL_AND_RESTART = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationEventSubProcessTest.testCancelAndRestart.bpmn20.xml";

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideEventSubProcessAndCancelTaskOutsideEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstanceId).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask");
		assertProcessEnded(processInstanceId);

	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeStartEventInsideEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventProcessStart").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcess").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "eventSubProcessTask", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcessAndCancelTaskOutsideEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstanceId).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeStartEventInsideNonInterruptingEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventProcessStart").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testStartBeforeNonInterruptingEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcess").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("eventSubProcess").activity("eventSubProcessTask").endScope().done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "eventSubProcessTask", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideEventSubProcessInsideSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "eventSubProcessTask", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeStartEventInsideEventSubProcessInsideSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventProcessStart").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask", "task1", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeEventSubProcessInsideSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcess").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "eventSubProcessTask", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideEventSubProcessInsideSubProcessTask2ShouldStay()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("task2").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("task2").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeStartEventInsideEventSubProcessInsideSubProcessTask2ShouldBeCancelled()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventProcessStart").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeEventSubProcessInsideSubProcessTask2ShouldBeCancelled()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcess").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcessInsideSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcessTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "eventSubProcessTask", "task2");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeStartEventInsideNonInterruptingEventSubProcessInsideSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventProcessStart").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeNonInterruptingEventSubProcessInsideSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcess").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task1", "task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);

	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcessInsideSubProcessTask2ShouldStay()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcessTask").execute();


		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("task2").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("task2").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeStartEventInsideNonInterruptingEventSubProcessInsideSubProcessTask2ShouldStay()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventProcessStart").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("task2").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("task2").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
	  public virtual void testStartBeforeNonInterruptingEventSubProcessInsideSubProcessTask2ShouldStay()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = processInstance.Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("eventSubProcess").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("task2").beginScope("eventSubProcess").activity("eventSubProcessTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).scope().child("task2").concurrent().noScope().up().child(null).concurrent().noScope().child("eventSubProcessTask").scope().done());

		completeTasksInOrder("task2", "eventSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerJobPreservationOnCancellationAndStart()
	  public virtual void testTimerJobPreservationOnCancellationAndStart()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("timerEventSubProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		Job timerJob = managementService.createJobQuery().singleResult();
		assertNotNull(timerJob);

		// when the process instance is bare intermediately due to cancellation
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task")).startBeforeActivity("task").execute();

		// then it is still the same job

		Job remainingTimerJob = managementService.createJobQuery().singleResult();
		assertNotNull(remainingTimerJob);

		assertEquals(timerJob.Id, remainingTimerJob.Id);
		assertEquals(timerJob.Duedate, remainingTimerJob.Duedate);

	  }


	  [Deployment(resources : CANCEL_AND_RESTART)]
	  public virtual void testProcessInstanceModificationInEventSubProcessCancellationAndRestart()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("ProcessWithEventSubProcess");

		// assume
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).taskDefinitionKey("UserTaskEventSubProcess").singleResult();
		assertNotNull(task);

		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("UserTaskEventSubProcess").startAfterActivity("UserTaskEventSubProcess").execute();

		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
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

	  /// <summary>
	  /// Important that only the direct children are considered here. If you change this,
	  /// the test assertions are not as tight anymore.
	  /// </summary>
	  protected internal virtual ActivityInstance getChildInstanceForActivity(ActivityInstance activityInstance, string activityId)
	  {
		foreach (ActivityInstance childInstance in activityInstance.ChildActivityInstances)
		{
		  if (childInstance.ActivityId.Equals(activityId))
		  {
			return childInstance;
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