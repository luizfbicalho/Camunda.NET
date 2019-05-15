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


	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// Tests cancellation of four basic patterns of active activities in a scope:
	/// <ul>
	///  <li>single, non-scope activity
	///  <li>single, scope activity
	///  <li>two concurrent non-scope activities
	///  <li>two concurrent scope activities
	/// </ul>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class ProcessInstanceModificationCancellationTest : PluggableProcessEngineTestCase
	{

	  // the four patterns as described above
	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml";
	  protected internal const string ONE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.oneScopeTaskProcess.bpmn20.xml";
	  protected internal const string CONCURRENT_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGateway.bpmn20.xml";
	  protected internal const string CONCURRENT_SCOPE_TASKS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGatewayScopeTasks.bpmn20.xml";

	  // the four patterns nested in a subprocess and with an outer parallel task
	  protected internal const string NESTED_PARALLEL_ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelOneTaskProcess.bpmn20.xml";
	  protected internal const string NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelOneScopeTaskProcess.bpmn20.xml";
	  protected internal const string NESTED_PARALLEL_CONCURRENT_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelGateway.bpmn20.xml";
	  protected internal const string NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.nestedParallelGatewayScopeTasks.bpmn20.xml";

	  protected internal const string LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.listenerProcess.bpmn20.xml";
	  protected internal const string FAILING_OUTPUT_MAPPINGS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.failingOutputMappingProcess.bpmn20.xml";

	  protected internal const string INTERRUPTING_EVENT_SUBPROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.interruptingEventSubProcess.bpmn20.xml";

	  protected internal const string CALL_ACTIVITY_PROCESS = "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml";
	  protected internal const string SIMPLE_SUBPROCESS = "org/camunda/bpm/engine/test/bpmn/callactivity/simpleSubProcess.bpmn20.xml";
	  protected internal const string TWO_SUBPROCESSES = "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testTwoSubProcesses.bpmn20.xml";
	  protected internal const string NESTED_CALL_ACTIVITY = "org/camunda/bpm/engine/test/bpmn/callactivity/CallActivity.testNestedCallActivity.bpmn20.xml";




	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testCancellationInOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).execute();

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testCancelAllInOneTaskProcess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		// two instance of theTask
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("theTask").execute();

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).cancelAllForActivity("theTask").execute();

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testCancellationAndCreationInOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).startBeforeActivity("theTask").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertEquals(tree.Id, updatedTree.Id);
		assertTrue(!getInstanceIdForActivity(tree, "theTask").Equals(getInstanceIdForActivity(updatedTree, "theTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("theTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testCreationAndCancellationInOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "theTask").Equals(getInstanceIdForActivity(updatedTree, "theTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("theTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancellationInOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).execute();

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancelAllInOneScopeTaskProcess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		// two instances of theTask
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();

		// then
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("theTask").execute();

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancellationAndCreationInOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).startBeforeActivity("theTask").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "theTask").Equals(getInstanceIdForActivity(updatedTree, "theTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("theTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCreationAndCancellationInOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "theTask").Equals(getInstanceIdForActivity(updatedTree, "theTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("theTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_PROCESS)]
	  public virtual void testCancellationInConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task2").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_PROCESS)]
	  public virtual void testCancelAllInConcurrentProcess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		// two instances in task1
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("task1").execute();

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("task1").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task2").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }


	  [Deployment(resources : CONCURRENT_PROCESS)]
	  public virtual void testCancellationAndCreationInConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("task1").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "task1").Equals(getInstanceIdForActivity(updatedTree, "task1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").noScope().concurrent().up().child("task2").noScope().concurrent().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_PROCESS)]
	  public virtual void testCreationAndCancellationInConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task1").cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "task1").Equals(getInstanceIdForActivity(updatedTree, "task1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").noScope().concurrent().up().child("task2").noScope().concurrent().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCancellationInConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "task1").Equals(getInstanceIdForActivity(updatedTree, "task1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task2").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCancelAllInConcurrentScopeTasksProcess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		// two instances of task1
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task1").execute();


		// when
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("task1").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task2").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCancellationAndCreationInConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("task1").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "task1").Equals(getInstanceIdForActivity(updatedTree, "task1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).noScope().concurrent().child("task1").scope().up().up().child(null).noScope().concurrent().child("task2").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCreationAndCancellationInConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task1").cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "task1").Equals(getInstanceIdForActivity(updatedTree, "task1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).noScope().concurrent().child("task1").scope().up().up().child(null).noScope().concurrent().child("task2").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_TASK_PROCESS)]
	  public virtual void testCancellationInNestedOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_TASK_PROCESS)]
	  public virtual void testScopeCancellationInNestedOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_TASK_PROCESS)]
	  public virtual void testCancellationAndCreationInNestedOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).startBeforeActivity("innerTask").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask").Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerTask").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_TASK_PROCESS)]
	  public virtual void testCreationAndCancellationInNestedOneTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask").cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask").Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerTask").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancellationInNestedOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneScopeTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testScopeCancellationInNestedOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneScopeTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCancellationAndCreationInNestedOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneScopeTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).startBeforeActivity("innerTask").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask").Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("innerTask").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
	  public virtual void testCreationAndCancellationInNestedOneScopeTaskProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedOneScopeTaskProcess");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask").cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask").Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("innerTask").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_PROCESS)]
	  public virtual void testCancellationInNestedConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerTask2").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_PROCESS)]
	  public virtual void testScopeCancellationInNestedConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_PROCESS)]
	  public virtual void testCancellationAndCreationInNestedConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).startBeforeActivity("innerTask1").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask1").Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask1").activity("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").noScope().concurrent().up().child(null).noScope().concurrent().child(null).scope().child("innerTask1").noScope().concurrent().up().child("innerTask2").noScope().concurrent().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(3, tasks.Count);
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_PROCESS)]
	  public virtual void testCreationAndCancellationInNestedConcurrentProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask1").cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask1").Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask1").activity("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").noScope().concurrent().up().child(null).noScope().concurrent().child(null).scope().child("innerTask1").noScope().concurrent().up().child("innerTask2").noScope().concurrent().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(3, tasks.Count);
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCancellationInNestedConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGatewayScopeTasks");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child("innerTask2").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(2, tasks.Count);
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testScopeCancellationInNestedConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGatewayScopeTasks");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "subProcess")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("outerTask").scope().done());

		// assert successful completion of process
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCancellationAndCreationInNestedConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGatewayScopeTasks");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).startBeforeActivity("innerTask1").execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask1").Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask1").activity("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).concurrent().noScope().child(null).scope().child(null).concurrent().noScope().child("innerTask1").scope().up().up().child(null).concurrent().noScope().child("innerTask2").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(3, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
	  public virtual void testCreationAndCancellationInNestedConcurrentScopeTasksProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedParallelGatewayScopeTasks");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask1").cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).execute();

		assertProcessNotEnded(processInstanceId);

		// assert activity instance
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertTrue(!getInstanceIdForActivity(tree, "innerTask1").Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask1").activity("innerTask2").done());

		// assert executions
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("outerTask").concurrent().noScope().up().child(null).noScope().concurrent().child(null).scope().child(null).concurrent().noScope().child("innerTask1").scope().up().up().child(null).concurrent().noScope().child("innerTask2").scope().done());

		// assert successful completion of process
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(3, tasks.Count);

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : LISTENER_PROCESS)]
	  public virtual void testEndListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("listenerProcess", Collections.singletonMap<string, object>("listener", new RecorderExecutionListener()));

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// when one inner task is cancelled
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1")).execute();

		assertEquals(1, RecorderExecutionListener.RecordedEvents.Count);
		RecorderExecutionListener.RecordedEvent innerTask1EndEvent = RecorderExecutionListener.RecordedEvents[0];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, innerTask1EndEvent.EventName);
		assertEquals("innerTask1", innerTask1EndEvent.ActivityId);
		assertEquals(getInstanceIdForActivity(tree, "innerTask1"), innerTask1EndEvent.ActivityInstanceId);

		// when the second inner task is cancelled
		RecorderExecutionListener.clear();
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask2")).execute();

		assertEquals(2, RecorderExecutionListener.RecordedEvents.Count);
		RecorderExecutionListener.RecordedEvent innerTask2EndEvent = RecorderExecutionListener.RecordedEvents[0];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, innerTask2EndEvent.EventName);
		assertEquals("innerTask2", innerTask2EndEvent.ActivityId);
		assertEquals(getInstanceIdForActivity(tree, "innerTask2"), innerTask2EndEvent.ActivityInstanceId);

		RecorderExecutionListener.RecordedEvent subProcessEndEvent = RecorderExecutionListener.RecordedEvents[1];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, subProcessEndEvent.EventName);
		assertEquals("subProcess", subProcessEndEvent.ActivityId);
		assertEquals(getInstanceIdForActivity(tree, "subProcess"), subProcessEndEvent.ActivityInstanceId);

		// when the outer task is cancelled (and so the entire process)
		RecorderExecutionListener.clear();
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "outerTask")).execute();

		assertEquals(2, RecorderExecutionListener.RecordedEvents.Count);
		RecorderExecutionListener.RecordedEvent outerTaskEndEvent = RecorderExecutionListener.RecordedEvents[0];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, outerTaskEndEvent.EventName);
		assertEquals("outerTask", outerTaskEndEvent.ActivityId);
		assertEquals(getInstanceIdForActivity(tree, "outerTask"), outerTaskEndEvent.ActivityInstanceId);

		RecorderExecutionListener.RecordedEvent processEndEvent = RecorderExecutionListener.RecordedEvents[1];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, processEndEvent.EventName);
		assertNull(processEndEvent.ActivityId);
		assertEquals(tree.Id, processEndEvent.ActivityInstanceId);

		RecorderExecutionListener.clear();
	  }

	  /// <summary>
	  /// Tests the case that an output mapping exists that expects variables
	  /// that do not exist yet when the activities are cancelled
	  /// </summary>
	  [Deployment(resources : FAILING_OUTPUT_MAPPINGS_PROCESS)]
	  public virtual void testSkipOutputMappingsOnCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingOutputMappingProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// then executing the following cancellations should not fail because
		// it skips the output mapping
		// cancel inner task
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).execute(false, true);

		// cancel outer task
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "outerTask")).execute(false, true);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : INTERRUPTING_EVENT_SUBPROCESS)]
	  public virtual void testProcessInstanceEventSubscriptionsPreservedOnIntermediateCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// event subscription for the event subprocess
		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull(subscription);
		assertEquals(processInstance.Id, subscription.ProcessInstanceId);

		// when I execute cancellation and then start, such that the intermediate state of the process instance
		// has no activities
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("task1").execute();

		// then the message event subscription remains (i.e. it is not deleted and later re-created)
		EventSubscription updatedSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull(updatedSubscription);
		assertEquals(subscription.Id, updatedSubscription.Id);
		assertEquals(subscription.ProcessInstanceId, updatedSubscription.ProcessInstanceId);
	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testProcessInstanceVariablesPreservedOnIntermediateCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "value"));

		// when I execute cancellation and then start, such that the intermediate state of the process instance
		// has no activities
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "theTask")).startBeforeActivity("theTask").execute();

		// then the process instance variables remain
		object variable = runtimeService.getVariable(processInstance.Id, "var");
		assertNotNull(variable);
		assertEquals("value", variable);
	  }

	  public virtual string getInstanceIdForActivity(ActivityInstance activityInstance, string activityId)
	  {
		ActivityInstance instance = getChildInstanceForActivity(activityInstance, activityId);
		if (instance != null)
		{
		  return instance.Id;
		}
		return null;
	  }

	  public virtual ActivityInstance getChildInstanceForActivity(ActivityInstance activityInstance, string activityId)
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

	  /// <summary>
	  /// Test case for checking cancellation of process instances in call activity subprocesses
	  /// 
	  /// Test should propagate upward and destroy all process instances
	  /// 
	  /// </summary>
	  [Deployment(resources : { SIMPLE_SUBPROCESS, CALL_ACTIVITY_PROCESS })]
	  public virtual void testCancellationInCallActivitySubProcess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callSimpleSubProcess");
		string processInstanceId = processInstance.Id;

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();


		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(2, instanceList.Count);

		ActivityInstance tree = runtimeService.getActivityInstance(taskInSubProcess.ProcessInstanceId);
		// when
		runtimeService.createProcessInstanceModification(taskInSubProcess.ProcessInstanceId).cancelActivityInstance(getInstanceIdForActivity(tree, "task")).execute();


		// then
		assertProcessEnded(processInstanceId);

		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(0, instanceList.Count);
	  }

	  [Deployment(resources : { SIMPLE_SUBPROCESS, CALL_ACTIVITY_PROCESS })]
	  public virtual void testCancellationAndRestartInCallActivitySubProcess()
	  {
		// given
		runtimeService.startProcessInstanceByKey("callSimpleSubProcess");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();


		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(2, instanceList.Count);

		ActivityInstance tree = runtimeService.getActivityInstance(taskInSubProcess.ProcessInstanceId);
		// when
		runtimeService.createProcessInstanceModification(taskInSubProcess.ProcessInstanceId).cancelActivityInstance(getInstanceIdForActivity(tree, "task")).startBeforeActivity("task").execute();

		// then
		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertEquals(2, instanceList.Count);
	  }

	  /// <summary>
	  /// Test case for checking cancellation of process instances in call activity subprocesses
	  /// 
	  /// Test that upward cancellation respects other process instances
	  /// 
	  /// </summary>
	  [Deployment(resources : { SIMPLE_SUBPROCESS, TWO_SUBPROCESSES })]
	  public virtual void testSingleCancellationWithTwoSubProcess()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callTwoSubProcesses");
		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(3, instanceList.Count);

		IList<Task> taskList = taskService.createTaskQuery().list();
		assertNotNull(taskList);
		assertEquals(2, taskList.Count);

		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.ProcessInstanceId);
		assertNotNull(activeActivityIds);
		assertEquals(2, activeActivityIds.Count);

		ActivityInstance tree = runtimeService.getActivityInstance(taskList[0].ProcessInstanceId);

		// when
		runtimeService.createProcessInstanceModification(taskList[0].ProcessInstanceId).cancelActivityInstance(getInstanceIdForActivity(tree, "task")).execute();

		// then

		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(2, instanceList.Count);

		// How man call activities
		activeActivityIds = runtimeService.getActiveActivityIds(processInstance.ProcessInstanceId);
		assertNotNull(activeActivityIds);
		assertEquals(1, activeActivityIds.Count);
	  }

	  /// <summary>
	  /// Test case for checking deletion of process instances in nested call activity subprocesses
	  /// 
	  /// Checking that nested call activities will propagate upward over multiple nested levels
	  /// 
	  /// </summary>
	  [Deployment(resources : { SIMPLE_SUBPROCESS, NESTED_CALL_ACTIVITY, CALL_ACTIVITY_PROCESS })]
	  public virtual void testCancellationMultilevelProcessInstanceInCallActivity()
	  {
		// given
		runtimeService.startProcessInstanceByKey("nestedCallActivity");

		// one task in the subprocess should be active after starting the process instance
		TaskQuery taskQuery = taskService.createTaskQuery();
		Task taskBeforeSubProcess = taskQuery.singleResult();

		// Completing the task continues the process which leads to calling the subprocess
		taskService.complete(taskBeforeSubProcess.Id);
		Task taskInSubProcess = taskQuery.singleResult();

		// Completing the task continues the sub process which leads to calling the deeper subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskInNestedSubProcess = taskQuery.singleResult();

		IList<ProcessInstance> instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(3, instanceList.Count);

		ActivityInstance tree = runtimeService.getActivityInstance(taskInNestedSubProcess.ProcessInstanceId);

		// when
		runtimeService.createProcessInstanceModification(taskInNestedSubProcess.ProcessInstanceId).cancelActivityInstance(getInstanceIdForActivity(tree, "task")).execute();

		// then
		// How many process Instances
		instanceList = runtimeService.createProcessInstanceQuery().list();
		assertNotNull(instanceList);
		assertEquals(0, instanceList.Count);
	  }

	}

}