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

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessInstantiationAtActivitiesTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PARALLEL_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGateway.bpmn20.xml";
	  protected internal const string EXCLUSIVE_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGateway.bpmn20.xml";
	  protected internal const string SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocess.bpmn20.xml";
	  protected internal const string LISTENERS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstantiationAtActivitiesTest.listeners.bpmn20.xml";
	  protected internal const string IO_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstantiationAtActivitiesTest.ioMappings.bpmn20.xml";
	  protected internal const string ASYNC_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGatewayAsyncTask.bpmn20.xml";
	  protected internal const string SYNC_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstantiationAtActivitiesTest.synchronous.bpmn20.xml";

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testSingleActivityInstantiation()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task1").execute();

		// then
		assertNotNull(instance);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").done());

		// and it is possible to end the process
		completeTasksInOrder("task1");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testSingleActivityInstantiationById()
	  {
		// given
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		// when
		ProcessInstance instance = runtimeService.createProcessInstanceById(processDefinitionId).startBeforeActivity("task1").execute();

		// then
		assertNotNull(instance);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").done());

		// and it is possible to end the process
		completeTasksInOrder("task1");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testSingleActivityInstantiationSetBusinessKey()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").businessKey("businessKey").startBeforeActivity("task1").execute();

		// then
		assertNotNull(instance);
		assertEquals("businessKey", instance.BusinessKey);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testSingleActivityInstantiationSetCaseInstanceId()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").caseInstanceId("caseInstanceId").startBeforeActivity("task1").execute();

		// then
		assertNotNull(instance);
		assertEquals("caseInstanceId", instance.CaseInstanceId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartEventInstantiation()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("theStart").execute();

		// then
		assertNotNull(instance);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").done());

		// and it is possible to end the process
		completeTasksInOrder("task1");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartEventInstantiationWithVariables()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("theStart").setVariable("aVariable", "aValue").execute();

		// then
		assertNotNull(instance);

		assertEquals("aValue", runtimeService.getVariable(instance.Id, "aVariable"));
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartWithInvalidInitialActivity()
	  {
		try
		{
		  // when
		  runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("someNonExistingActivity").execute();
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  // then
		  assertTextPresentIgnoreCase("element 'someNonExistingActivity' does not exist in process ", e.Message);
		}
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testMultipleActivitiesInstantiation()
	  {

		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task1").startBeforeActivity("task2").startBeforeActivity("task1").execute();

		// then
		assertNotNull(instance);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").activity("task2").activity("task1").done());

		// and it is possible to end the process
		completeTasksInOrder("task1", "task2", "task1");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testMultipleActivitiesInstantiationWithVariables()
	  {
		// when
		runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task1").setVariableLocal("aVar1", "aValue1").startBeforeActivity("task2").setVariableLocal("aVar2", "aValue2").execute();

		// then
		// variables for task2's execution
		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		assertNotNull(task2Execution);
		assertNull(runtimeService.getVariableLocal(task2Execution.Id, "aVar1"));
		assertEquals("aValue2", runtimeService.getVariableLocal(task2Execution.Id, "aVar2"));

		// variables for task1's execution
		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		assertNotNull(task1Execution);

		assertNull(runtimeService.getVariableLocal(task1Execution.Id, "aVar2"));

		// this variable is not a local variable on execution1 due to tree expansion
		assertNull(runtimeService.getVariableLocal(task1Execution.Id, "aVar1"));
		assertEquals("aValue1", runtimeService.getVariable(task1Execution.Id, "aVar1"));

	  }

	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testNestedActivitiesInstantiation()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("subprocess").startBeforeActivity("innerTask").startBeforeActivity("outerTask").startBeforeActivity("innerTask").execute();

		// then
		assertNotNull(instance);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("outerTask").beginScope("subProcess").activity("innerTask").activity("innerTask").done());

		// and it is possible to end the process
		completeTasksInOrder("innerTask", "innerTask", "outerTask", "innerTask");
		assertProcessEnded(instance.Id);
	  }

	  public virtual void testStartNonExistingProcessDefinition()
	  {
		try
		{
		  runtimeService.createProcessInstanceById("I don't exist").startBeforeActivity("start").execute();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no deployed process definition found with id", e.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceByKey("I don't exist either").startBeforeActivity("start").execute();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("no processes deployed with key", e.Message);
		}
	  }

	  public virtual void testStartNullProcessDefinition()
	  {
		try
		{
		  runtimeService.createProcessInstanceById(null).startBeforeActivity("start").execute();
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		try
		{
		  runtimeService.createProcessInstanceByKey(null).startBeforeActivity("start").execute();
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }

	  [Deployment(resources : LISTENERS_PROCESS)]
	  public virtual void testListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("listenerProcess").startBeforeActivity("innerTask").execute();

		// then
		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").activity("innerTask").done());

		IList<RecorderExecutionListener.RecordedEvent> events = RecorderExecutionListener.RecordedEvents;
		assertEquals(3, events.Count);

		RecorderExecutionListener.RecordedEvent processStartEvent = events[0];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, processStartEvent.EventName);
		assertEquals("innerTask", processStartEvent.ActivityId);

		RecorderExecutionListener.RecordedEvent subProcessStartEvent = events[1];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, subProcessStartEvent.EventName);
		assertEquals("subProcess", subProcessStartEvent.ActivityId);

		RecorderExecutionListener.RecordedEvent innerTaskStartEvent = events[2];
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, innerTaskStartEvent.EventName);
		assertEquals("innerTask", innerTaskStartEvent.ActivityId);

	  }

	  [Deployment(resources : LISTENERS_PROCESS)]
	  public virtual void testSkipListenerInvocation()
	  {
		RecorderExecutionListener.clear();

		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("listenerProcess").startBeforeActivity("innerTask").execute(true, true);

		// then
		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("subProcess").activity("innerTask").done());

		IList<RecorderExecutionListener.RecordedEvent> events = RecorderExecutionListener.RecordedEvents;
		assertEquals(0, events.Count);
	  }

	  [Deployment(resources : IO_PROCESS)]
	  public virtual void testIoMappingInvocation()
	  {
		// when
		runtimeService.createProcessInstanceByKey("ioProcess").startBeforeActivity("innerTask").execute();

		// then no io mappings have been executed
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().orderByVariableName().asc().list();
		assertEquals(2, variables.Count);

		Execution innerTaskExecution = runtimeService.createExecutionQuery().activityId("innerTask").singleResult();
		VariableInstance innerTaskVariable = variables[0];
		assertEquals("innerTaskVariable", innerTaskVariable.Name);
		assertEquals("innerTaskValue", innerTaskVariable.Value);
		assertEquals(innerTaskExecution.Id, innerTaskVariable.ExecutionId);

		VariableInstance subProcessVariable = variables[1];
		assertEquals("subProcessVariable", subProcessVariable.Name);
		assertEquals("subProcessValue", subProcessVariable.Value);
		assertEquals(((ExecutionEntity) innerTaskExecution).ParentId, subProcessVariable.ExecutionId);
	  }

	  [Deployment(resources : IO_PROCESS)]
	  public virtual void testSkipIoMappingInvocation()
	  {
		// when
		runtimeService.createProcessInstanceByKey("ioProcess").startBeforeActivity("innerTask").execute(true, true);

		// then no io mappings have been executed
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testSetProcessInstanceVariable()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("subprocess").setVariable("aVariable1", "aValue1").setVariableLocal("aVariable2", "aValue2").setVariables(Variables.createVariables().putValue("aVariable3", "aValue3")).setVariablesLocal(Variables.createVariables().putValue("aVariable4", "aValue4")).startBeforeActivity("innerTask").execute();

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().orderByVariableName().asc().list();

		assertEquals(4, variables.Count);
		assertEquals("aVariable1", variables[0].Name);
		assertEquals("aValue1", variables[0].Value);
		assertEquals(instance.Id, variables[0].ExecutionId);

		assertEquals("aVariable2", variables[1].Name);
		assertEquals("aValue2", variables[1].Value);
		assertEquals(instance.Id, variables[1].ExecutionId);

		assertEquals("aVariable3", variables[2].Name);
		assertEquals("aValue3", variables[2].Value);
		assertEquals(instance.Id, variables[2].ExecutionId);

		assertEquals("aVariable4", variables[3].Name);
		assertEquals("aValue4", variables[3].Value);
		assertEquals(instance.Id, variables[3].ExecutionId);

	  }

	  [Deployment(resources : ASYNC_PROCESS)]
	  public virtual void testStartAsyncTask()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task2").execute();

		// then
		assertNotNull(instance);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(instance.Id);
		assertNotNull(updatedTree);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).transition("task2").done());

		// and it is possible to end the process
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		completeTasksInOrder("task2");
		assertProcessEnded(instance.Id);
	  }

	  [Deployment(resources : SYNC_PROCESS)]
	  public virtual void testStartMultipleTasksInSyncProcess()
	  {
		RecorderExecutionListener.clear();

		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("syncProcess").startBeforeActivity("syncTask").startBeforeActivity("syncTask").startBeforeActivity("syncTask").execute();

		// then the request was successful even though the process instance has already ended
		assertNotNull(instance);
		assertProcessEnded(instance.Id);

		// and the execution listener was invoked correctly
		IList<RecorderExecutionListener.RecordedEvent> events = RecorderExecutionListener.RecordedEvents;
		assertEquals(8, events.Count);

		// process start event
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, events[0].EventName);
		assertEquals("syncTask", events[0].ActivityId);

		// start instruction 1
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, events[1].EventName);
		assertEquals("syncTask", events[1].ActivityId);
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, events[2].EventName);
		assertEquals("syncTask", events[2].ActivityId);

		// start instruction 2
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, events[3].EventName);
		assertEquals("syncTask", events[3].ActivityId);
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, events[4].EventName);
		assertEquals("syncTask", events[4].ActivityId);

		// start instruction 3
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, events[5].EventName);
		assertEquals("syncTask", events[5].ActivityId);
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, events[6].EventName);
		assertEquals("syncTask", events[6].ActivityId);

		// process end event
		assertEquals(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, events[7].EventName);
		assertEquals("end", events[7].ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInitiatorVariable()
	  public virtual void testInitiatorVariable()
	  {
		// given
		identityService.AuthenticatedUserId = "kermit";

		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("initiatorProcess").startBeforeActivity("task").execute();

		// then
		string initiator = (string) runtimeService.getVariable(instance.Id, "initiator");
		assertEquals("kermit", initiator);

		identityService.clearAuthentication();
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