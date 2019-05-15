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


	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessInstanceModificationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PARALLEL_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGateway.bpmn20.xml";
	  protected internal const string EXCLUSIVE_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGateway.bpmn20.xml";
	  protected internal const string SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocess.bpmn20.xml";
	  protected internal const string SUBPROCESS_LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocessListeners.bpmn20.xml";
	  protected internal const string SUBPROCESS_BOUNDARY_EVENTS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocessBoundaryEvents.bpmn20.xml";
	  protected internal const string ONE_SCOPE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.oneScopeTaskProcess.bpmn20.xml";
	  protected internal const string TRANSITION_LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.transitionListeners.bpmn20.xml";
	  protected internal const string TASK_LISTENER_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.taskListeners.bpmn20.xml";
	  protected internal const string IO_MAPPING_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.ioMapping.bpmn20.xml";
	  protected internal const string IO_MAPPING_ON_SUB_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.ioMappingOnSubProcess.bpmn20.xml";
	  protected internal const string IO_MAPPING_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.ioMappingOnSubProcessNested.bpmn20.xml";
	  protected internal const string LISTENERS_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.listenersOnSubProcessNested.bpmn20.xml";
	  protected internal const string DOUBLE_NESTED_SUB_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.doubleNestedSubprocess.bpmn20.xml";
	  protected internal const string TRANSACTION_WITH_COMPENSATION_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.testTransactionWithCompensation.bpmn20.xml";
	  protected internal const string CALL_ACTIVITY_PARENT_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.testCancelCallActivityParentProcess.bpmn";
	  protected internal const string CALL_ACTIVITY_CHILD_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.testCancelCallActivityChildProcess.bpmn";

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).execute();

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

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).cancelActivityInstance(getInstanceIdForActivity(tree, "task2")).execute();

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testCancellationWithWrongProcessInstanceId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		try
		{
		  runtimeService.createProcessInstanceModification("foo").cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).cancelActivityInstance(getInstanceIdForActivity(tree, "task2")).execute();
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

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").execute();

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

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2", tree.Id).execute();

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

	  [Deployment(resources : DOUBLE_NESTED_SUB_PROCESS)]
	  public virtual void testStartBeforeWithAncestorInstanceIdTwoScopesUp()
	  {
		// given two instances of the outer subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("doubleNestedSubprocess");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess").execute();

		// when I start the inner subprocess task without explicit ancestor
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerSubProcessTask").execute();
		  // then the command fails
		  fail("should not succeed because the ancestors are ambiguous");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		// when I start the inner subprocess task with an explicit ancestor activity
		// instance id
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		ActivityInstance randomSubProcessInstance = getChildInstanceForActivity(updatedTree, "subProcess");

		// then the command suceeds
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("innerSubProcessTask", randomSubProcessInstance.Id).execute();

		// and the trees are correct
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("subProcessTask").endScope().beginScope("subProcess").activity("subProcessTask").beginScope("innerSubProcess").activity("innerSubProcessTask").done());

		ActivityInstance innerSubProcessInstance = getChildInstanceForActivity(updatedTree, "innerSubProcess");
		assertEquals(randomSubProcessInstance.Id, innerSubProcessInstance.ParentActivityInstanceId);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerSubProcessTask").scope().done());

		assertEquals(3, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("subProcessTask", "subProcessTask", "innerSubProcessTask", "innerSubProcessTask", "innerSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : DOUBLE_NESTED_SUB_PROCESS)]
	  public virtual void testStartBeforeWithInvalidAncestorInstanceId()
	  {
		// given two instances of the outer subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("doubleNestedSubprocess");
		string processInstanceId = processInstance.Id;

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess", "noValidActivityInstanceId").execute();
		  fail();
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start before activity 'subProcess' with ancestor activity instance 'noValidActivityInstanceId'; " + "Ancestor activity instance 'noValidActivityInstanceId' does not exist", e.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess", null).execute();
		  fail();
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("ancestorActivityInstanceId is null", e.Message);
		}

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		string subProcessTaskId = getInstanceIdForActivity(tree, "subProcessTask");

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess", subProcessTaskId).execute();
		  fail("should not succeed because subProcessTask is a child of subProcess");
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start before activity 'subProcess' with ancestor activity instance '" + subProcessTaskId + "'; " + "Scope execution for '" + subProcessTaskId + "' cannot be found in parent hierarchy of flow element 'subProcess'", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartBeforeNonExistingActivity()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		try
		{
		  // when
		  runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("someNonExistingActivity").execute();
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

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startAfterActivity("task1").startBeforeActivity("task1").execute();

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

		runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow4").execute();

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

		runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow4", tree.Id).execute();

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

	  [Deployment(resources : DOUBLE_NESTED_SUB_PROCESS)]
	  public virtual void testStartTransitionWithAncestorInstanceIdTwoScopesUp()
	  {
		// given two instances of the outer subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("doubleNestedSubprocess");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess").execute();

		// when I start the inner subprocess task without explicit ancestor
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow5").execute();
		  // then the command fails
		  fail("should not succeed because the ancestors are ambiguous");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		// when I start the inner subprocess task with an explicit ancestor activity
		// instance id
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		ActivityInstance randomSubProcessInstance = getChildInstanceForActivity(updatedTree, "subProcess");

		// then the command suceeds
		runtimeService.createProcessInstanceModification(processInstanceId).startTransition("flow5", randomSubProcessInstance.Id).execute();

		// and the trees are correct
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("subProcessTask").endScope().beginScope("subProcess").activity("subProcessTask").beginScope("innerSubProcess").activity("innerSubProcessTask").done());

		ActivityInstance innerSubProcessInstance = getChildInstanceForActivity(updatedTree, "innerSubProcess");
		assertEquals(randomSubProcessInstance.Id, innerSubProcessInstance.ParentActivityInstanceId);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerSubProcessTask").scope().done());

		assertEquals(3, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("subProcessTask", "subProcessTask", "innerSubProcessTask", "innerSubProcessTask", "innerSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : DOUBLE_NESTED_SUB_PROCESS)]
	  public virtual void testStartTransitionWithInvalidAncestorInstanceId()
	  {
		// given two instances of the outer subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("doubleNestedSubprocess");
		string processInstanceId = processInstance.Id;

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow5", "noValidActivityInstanceId").execute();
		  fail();
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start transition 'flow5' with ancestor activity instance 'noValidActivityInstanceId'; " + "Ancestor activity instance 'noValidActivityInstanceId' does not exist", e.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow5", null).execute();
		  fail();
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("ancestorActivityInstanceId is null", e.Message);
		}

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		string subProcessTaskId = getInstanceIdForActivity(tree, "subProcessTask");

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow5", subProcessTaskId).execute();
		  fail("should not succeed because subProcessTask is a child of subProcess");
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start transition 'flow5' with ancestor activity instance '" + subProcessTaskId + "'; " + "Scope execution for '" + subProcessTaskId + "' cannot be found in parent hierarchy of flow element 'flow5'", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartTransitionCase2()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow2").execute();

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
	  public virtual void testStartTransitionInvalidTransitionId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		try
		{
		  runtimeService.createProcessInstanceModification(processInstanceId).startTransition("invalidFlowId").execute();

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

		runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theStart").execute();

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

		runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theStart", tree.Id).execute();

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

	  [Deployment(resources : DOUBLE_NESTED_SUB_PROCESS)]
	  public virtual void testStartAfterWithAncestorInstanceIdTwoScopesUp()
	  {
		// given two instances of the outer subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("doubleNestedSubprocess");
		string processInstanceId = processInstance.Id;

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess").execute();

		// when I start the inner subprocess task without explicit ancestor
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("innerSubProcessStart").execute();
		  // then the command fails
		  fail("should not succeed because the ancestors are ambiguous");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		// when I start the inner subprocess task with an explicit ancestor activity
		// instance id
		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		ActivityInstance randomSubProcessInstance = getChildInstanceForActivity(updatedTree, "subProcess");

		// then the command suceeds
		runtimeService.createProcessInstanceModification(processInstanceId).startAfterActivity("innerSubProcessStart", randomSubProcessInstance.Id).execute();

		// and the trees are correct
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("subProcessTask").endScope().beginScope("subProcess").activity("subProcessTask").beginScope("innerSubProcess").activity("innerSubProcessTask").done());

		ActivityInstance innerSubProcessInstance = getChildInstanceForActivity(updatedTree, "innerSubProcess");
		assertEquals(randomSubProcessInstance.Id, innerSubProcessInstance.ParentActivityInstanceId);

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("subProcessTask").scope().up().up().child(null).concurrent().noScope().child(null).scope().child("subProcessTask").concurrent().noScope().up().child(null).concurrent().noScope().child("innerSubProcessTask").scope().done());

		assertEquals(3, taskService.createTaskQuery().count());

		// complete the process
		completeTasksInOrder("subProcessTask", "subProcessTask", "innerSubProcessTask", "innerSubProcessTask", "innerSubProcessTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : DOUBLE_NESTED_SUB_PROCESS)]
	  public virtual void testStartAfterWithInvalidAncestorInstanceId()
	  {
		// given two instances of the outer subprocess
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("doubleNestedSubprocess");
		string processInstanceId = processInstance.Id;

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("innerSubProcessStart", "noValidActivityInstanceId").execute();
		  fail();
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start after activity 'innerSubProcessStart' with ancestor activity instance 'noValidActivityInstanceId'; " + "Ancestor activity instance 'noValidActivityInstanceId' does not exist", e.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("innerSubProcessStart", null).execute();
		  fail();
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("ancestorActivityInstanceId is null", e.Message);
		}

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		string subProcessTaskId = getInstanceIdForActivity(tree, "subProcessTask");

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("innerSubProcessStart", subProcessTaskId).execute();
		  fail("should not succeed because subProcessTask is a child of subProcess");
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresent("Cannot perform instruction: " + "Start after activity 'innerSubProcessStart' with ancestor activity instance '" + subProcessTaskId + "'; " + "Scope execution for '" + subProcessTaskId + "' cannot be found in parent hierarchy of flow element 'flow5'", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartAfterActivityAmbiguousTransitions()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		try
		{
		  runtimeService.createProcessInstanceModification(processInstanceId).startAfterActivity("fork").execute();

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
		  runtimeService.createProcessInstanceModification(processInstanceId).startAfterActivity("theEnd").execute();

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
		  runtimeService.createProcessInstanceModification(instance.Id).startAfterActivity("someNonExistingActivity").execute();
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

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();

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
		runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theTask").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("theTask").scope().done());

		// when starting after the start event, regular concurrency happens
		runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theStart").execute();

		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("theTask").activity("theTask").done());

		executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree(null).scope().child(null).concurrent().noScope().child("theTask").scope().up().up().child(null).concurrent().noScope().child("theTask").scope().done());

		completeTasksInOrder("theTask", "theTask");
		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : SUBPROCESS_BOUNDARY_EVENTS_PROCESS)]
	  public virtual void testStartBeforeEventSubscription()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subprocess");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask").execute();

		// then two timer jobs should have been created
		assertEquals(2, managementService.createJobQuery().count());
		Job innerJob = managementService.createJobQuery().activityId("innerTimer").singleResult();
		assertNotNull(innerJob);
		assertEquals(runtimeService.createExecutionQuery().activityId("innerTask").singleResult().Id, innerJob.ExecutionId);

		Job outerJob = managementService.createJobQuery().activityId("outerTimer").singleResult();
		assertNotNull(outerJob);

		// when executing the jobs
		managementService.executeJob(innerJob.Id);

		Task innerBoundaryTask = taskService.createTaskQuery().taskDefinitionKey("innerAfterBoundaryTask").singleResult();
		assertNotNull(innerBoundaryTask);

		managementService.executeJob(outerJob.Id);

		Task outerBoundaryTask = taskService.createTaskQuery().taskDefinitionKey("outerAfterBoundaryTask").singleResult();
		assertNotNull(outerBoundaryTask);

	  }

	  [Deployment(resources : SUBPROCESS_LISTENER_PROCESS)]
	  public virtual void testActivityExecutionListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subprocess", Collections.singletonMap<string, object> ("listener", new RecorderExecutionListener()));

		string processInstanceId = processInstance.Id;

		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask").execute();

		// assert activity instance tree
		ActivityInstance activityInstanceTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(activityInstanceTree);
		assertEquals(processInstanceId, activityInstanceTree.ProcessInstanceId);

		assertThat(activityInstanceTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").done());

		// assert listener invocations
		IList<RecorderExecutionListener.RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(2, recordedEvents.Count);

		ActivityInstance subprocessInstance = getChildInstanceForActivity(activityInstanceTree, "subProcess");
		ActivityInstance innerTaskInstance = getChildInstanceForActivity(subprocessInstance, "innerTask");

		RecorderExecutionListener.RecordedEvent firstEvent = recordedEvents[0];
		RecorderExecutionListener.RecordedEvent secondEvent = recordedEvents[1];

		assertEquals("subProcess", firstEvent.ActivityId);
		assertEquals(subprocessInstance.Id, firstEvent.ActivityInstanceId);
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, secondEvent.EventName);

		assertEquals("innerTask", secondEvent.ActivityId);
		assertEquals(innerTaskInstance.Id, secondEvent.ActivityInstanceId);
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, secondEvent.EventName);

		RecorderExecutionListener.clear();

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(innerTaskInstance.Id).execute();

		assertEquals(2, RecorderExecutionListener.RecordedEvents.Count);
	  }

	  [Deployment(resources : SUBPROCESS_LISTENER_PROCESS)]
	  public virtual void testSkipListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subprocess", Collections.singletonMap<string, object> ("listener", new RecorderExecutionListener()));

		string processInstanceId = processInstance.Id;

		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);

		// when I start an activity with "skip listeners" setting
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("innerTask").execute(true, false);

		// then no listeners are invoked
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);

		// when I cancel an activity with "skip listeners" setting
		ActivityInstance activityInstanceTree = runtimeService.getActivityInstance(processInstanceId);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "innerTask").Id).execute(true, false);

		// then no listeners are invoked
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);

		// when I cancel an activity that ends the process instance
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "outerTask").Id).execute(true, false);

		// then no listeners are invoked
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);
	  }

	  [Deployment(resources : TASK_LISTENER_PROCESS)]
	  public virtual void testSkipTaskListenerInvocation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskListenerProcess", Collections.singletonMap<string, object> ("listener", new RecorderTaskListener()));

		string processInstanceId = processInstance.Id;

		RecorderTaskListener.clear();

		// when I start an activity with "skip listeners" setting
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("task").execute(true, false);

		// then no listeners are invoked
		assertTrue(RecorderTaskListener.RecordedEvents.Count == 0);

		// when I cancel an activity with "skip listeners" setting
		ActivityInstance activityInstanceTree = runtimeService.getActivityInstance(processInstanceId);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "task").Id).execute(true, false);

		// then no listeners are invoked
		assertTrue(RecorderTaskListener.RecordedEvents.Count == 0);
	  }

	  [Deployment(resources : IO_MAPPING_PROCESS)]
	  public virtual void testSkipIoMappings()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("ioMappingProcess");

		// when I start task2
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").execute(false, true);

		// then the input mapping should not have executed
		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		assertNotNull(task2Execution);

		assertNull(runtimeService.getVariable(task2Execution.Id, "inputMappingExecuted"));

		// when I cancel task2
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelAllForActivity("task2").execute(false, true);

		// then the output mapping should not have executed
		assertNull(runtimeService.getVariable(processInstance.Id, "outputMappingExecuted"));
	  }

	  [Deployment(resources : IO_MAPPING_ON_SUB_PROCESS)]
	  public virtual void testSkipIoMappingsOnSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("boundaryEvent").execute(false, true);

		// then the output mapping should not have executed
		assertNull(runtimeService.getVariable(processInstance.Id, "outputMappingExecuted"));
	  }

	  /// <summary>
	  /// should also skip io mappings that are defined on already instantiated
	  /// ancestor scopes and that may be executed due to the ancestor scope
	  /// completing within the modification command.
	  /// </summary>
	  [Deployment(resources : IO_MAPPING_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS)]
	  public virtual void testSkipIoMappingsOnSubProcessNested()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("boundaryEvent").execute(false, true);

		// then the output mapping should not have executed
		assertNull(runtimeService.getVariable(processInstance.Id, "outputMappingExecuted"));
	  }

	  [Deployment(resources : LISTENERS_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS)]
	  public virtual void testSkipListenersOnSubProcessNested()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("boundaryEvent").execute(true, false);

		assertProcessEnded(processInstance.Id);

		// then the output mapping should not have executed
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);
	  }

	  [Deployment(resources : TRANSITION_LISTENER_PROCESS)]
	  public virtual void testStartTransitionListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("transitionListenerProcess", Variables.createVariables().putValue("listener", new RecorderExecutionListener()));

		runtimeService.createProcessInstanceModification(instance.Id).startTransition("flow2").execute();

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

		runtimeService.createProcessInstanceModification(instance.Id).startTransition("flow2").execute();

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
	  public virtual void testStartBeforeWithVariables()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").setVariable("procInstVar", "procInstValue").setVariableLocal("localVar", "localValue").setVariables(Variables.createVariables().putValue("procInstMapVar", "procInstMapValue")).setVariablesLocal(Variables.createVariables().putValue("localMapVar", "localMapValue")).execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);
		assertNotNull(updatedTree);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task1").activity("task2").done());

		ActivityInstance task2Instance = getChildInstanceForActivity(updatedTree, "task2");
		assertNotNull(task2Instance);
		assertEquals(1, task2Instance.ExecutionIds.Length);
		string task2ExecutionId = task2Instance.ExecutionIds[0];

		assertEquals(4, runtimeService.createVariableInstanceQuery().count());
		assertEquals("procInstValue", runtimeService.getVariableLocal(processInstance.Id, "procInstVar"));
		assertEquals("localValue", runtimeService.getVariableLocal(task2ExecutionId, "localVar"));
		assertEquals("procInstMapValue", runtimeService.getVariableLocal(processInstance.Id, "procInstMapVar"));
		assertEquals("localMapValue", runtimeService.getVariableLocal(task2ExecutionId, "localMapVar"));

		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testCancellationAndStartBefore()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");
		string processInstanceId = processInstance.Id;

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).startBeforeActivity("task2").execute();

		ActivityInstance activityInstanceTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(activityInstanceTree);
		assertEquals(processInstanceId, activityInstanceTree.ProcessInstanceId);

		assertThat(activityInstanceTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task2").scope().done());

		completeTasksInOrder("task2");
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationRemovalOnCancellation()
	  public virtual void testCompensationRemovalOnCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensationProcess");

		Execution taskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();
		Task task = taskService.createTaskQuery().executionId(taskExecution.Id).singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);
		// there should be a compensation event subscription for innerTask now
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());

		// when innerTask2 is cancelled
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask2")).execute();

		// then the innerTask compensation should be removed
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationCreation()
	  public virtual void testCompensationCreation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensationProcess");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask").execute();

		Execution task2Execution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();
		Task task = taskService.createTaskQuery().executionId(task2Execution.Id).singleResult();
		assertNotNull(task);

		taskService.complete(task.Id);
		assertEquals(3, runtimeService.createEventSubscriptionQuery().count());

		// trigger compensation
		Task outerTask = taskService.createTaskQuery().taskDefinitionKey("outerTask").singleResult();
		assertNotNull(outerTask);
		taskService.complete(outerTask.Id);

		// then there are two compensation tasks and the afterSubprocessTask:
		assertEquals(3, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("innerAfterBoundaryTask").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("outerAfterBoundaryTask").count());
		assertEquals(1, taskService.createTaskQuery().taskDefinitionKey("taskAfterSubprocess").count());

		// complete process
		completeTasksInOrder("taskAfterSubprocess", "innerAfterBoundaryTask", "outerAfterBoundaryTask");

		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoCompensationCreatedOnCancellation()
	  public virtual void testNoCompensationCreatedOnCancellation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensationProcess");
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		// one on outerTask, one on innerTask
		assertEquals(2, taskService.createTaskQuery().count());

		// when inner task is cancelled
		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "innerTask")).execute();

		// then no compensation event subscription exists
		assertEquals(0, runtimeService.createEventSubscriptionQuery().count());

		// and the compensation throw event does not trigger compensation handlers
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("outerTask", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartActivityInTransactionWithCompensation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		completeTasksInOrder("userTask");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("undoTask", task.TaskDefinitionKey);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").done());

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("userTask").execute();

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").activity("userTask").done());

		completeTasksInOrder("userTask");

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").done());

		Task newTask = taskService.createTaskQuery().singleResult();
		assertNotSame(task.Id, newTask.Id);

		completeTasksInOrder("undoTask", "afterCancel");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartActivityWithAncestorInTransactionWithCompensation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		completeTasksInOrder("userTask");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("undoTask", task.TaskDefinitionKey);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").done());

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("userTask", processInstance.Id).execute();

		completeTasksInOrder("userTask");

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").endScope().beginScope("tx").activity("txEnd").activity("undoTask").done());

		completeTasksInOrder("undoTask", "undoTask", "afterCancel", "afterCancel");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartAfterActivityDuringCompensation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		completeTasksInOrder("userTask");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("undoTask", task.TaskDefinitionKey);

		runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("userTask").execute();

		task = taskService.createTaskQuery().singleResult();
		assertEquals("afterCancel", task.TaskDefinitionKey);

		completeTasksInOrder("afterCancel");
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testCancelCompensatingTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "undoTask")).execute();

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testCancelCompensatingTaskAndStartActivity()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "undoTask")).startBeforeActivity("userTask").execute();

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("userTask").done());

		completeTasksInOrder("userTask", "undoTask", "afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testCancelCompensatingTaskAndStartActivityWithAncestor()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "undoTask")).startBeforeActivity("userTask", processInstance.Id).execute();

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("userTask").done());

		completeTasksInOrder("userTask", "undoTask", "afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartActivityAndCancelCompensatingTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("userTask").cancelActivityInstance(getInstanceIdForActivity(tree, "undoTask")).execute();

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("userTask").done());

		completeTasksInOrder("userTask", "undoTask", "afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartCompensatingTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("undoTask").execute();

		completeTasksInOrder("undoTask");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("userTask", task.TaskDefinitionKey);

		completeTasksInOrder("userTask", "undoTask", "afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartAdditionalCompensatingTaskAndCompleteOldCompensationTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		Task firstUndoTask = taskService.createTaskQuery().singleResult();

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("undoTask").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").activity("undoTask").done());

		taskService.complete(firstUndoTask.Id);

		Task secondUndoTask = taskService.createTaskQuery().taskDefinitionKey("undoTask").singleResult();
		assertNull(secondUndoTask);

		completeTasksInOrder("afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartAdditionalCompensatingTaskAndCompleteNewCompensatingTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		Task firstUndoTask = taskService.createTaskQuery().taskDefinitionKey("undoTask").singleResult();

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("undoTask").setVariableLocal("new", true).execute();

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").activity("undoTask").done());

		string taskExecutionId = runtimeService.createExecutionQuery().variableValueEquals("new", true).singleResult().Id;
		Task secondUndoTask = taskService.createTaskQuery().executionId(taskExecutionId).singleResult();

		assertNotNull(secondUndoTask);
		assertNotSame(firstUndoTask.Id, secondUndoTask.Id);
		taskService.complete(secondUndoTask.Id);

		tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").done());

		completeTasksInOrder("undoTask", "afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartCompensationBoundary()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("compensateBoundaryEvent").execute();

		  fail("should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("compensation boundary event", e.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("compensateBoundaryEvent").execute();

		  fail("should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no outgoing sequence flow", e.Message);
		}
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartCancelEndEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("txEnd").execute();

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("afterCancel", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartCancelBoundaryEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("catchCancelTx").execute();

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("afterCancel", task.TaskDefinitionKey);

		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : TRANSACTION_WITH_COMPENSATION_PROCESS)]
	  public virtual void testStartTaskAfterCancelBoundaryEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");
		completeTasksInOrder("userTask");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("afterCancel").execute();

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").endScope().activity("afterCancel").done());

		completeTasksInOrder("afterCancel", "undoTask", "afterCancel");

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testCancelNonExistingActivityInstance()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		// when - then throw exception
		try
		{
		  runtimeService.createProcessInstanceModification(instance.Id).cancelActivityInstance("nonExistingActivityInstance").execute();
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
		  runtimeService.createProcessInstanceModification(instance.Id).cancelTransitionInstance("nonExistingActivityInstance").execute();
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  assertTextPresent("Cannot perform instruction: Cancel transition instance 'nonExistingActivityInstance'; " + "Transition instance 'nonExistingActivityInstance' does not exist", e.Message);
		}

	  }

	  [Deployment(resources : { CALL_ACTIVITY_PARENT_PROCESS, CALL_ACTIVITY_CHILD_PROCESS })]
	  public virtual void FAILING_testCancelCallActivityInstance()
	  {
		// given
		ProcessInstance parentprocess = runtimeService.startProcessInstanceByKey("parentprocess");
		ProcessInstance subProcess = runtimeService.createProcessInstanceQuery().processDefinitionKey("subprocess").singleResult();

		ActivityInstance subProcessActivityInst = runtimeService.getActivityInstance(subProcess.Id);

		// when
		runtimeService.createProcessInstanceModification(subProcess.Id).startBeforeActivity("childEnd", subProcess.Id).cancelActivityInstance(getInstanceIdForActivity(subProcessActivityInst, "innerTask")).execute();

		// then
		assertProcessEnded(parentprocess.Id);
	  }

	  public virtual void testModifyNullProcessInstance()
	  {
		try
		{
		  runtimeService.createProcessInstanceModification(null).startBeforeActivity("someActivity").execute();
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  assertTextPresent("processInstanceId is null", e.Message);
		}
	  }

	  // TODO: check if starting with a non-existing activity/transition id is
	  // handled properly

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