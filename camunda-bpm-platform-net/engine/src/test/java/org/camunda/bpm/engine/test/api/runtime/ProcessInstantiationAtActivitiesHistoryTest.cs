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

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class ProcessInstantiationAtActivitiesHistoryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PARALLEL_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGateway.bpmn20.xml";
	  protected internal const string EXCLUSIVE_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGateway.bpmn20.xml";
	  protected internal const string SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocess.bpmn20.xml";
	  protected internal const string ASYNC_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGatewayAsyncTask.bpmn20.xml";

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testHistoricProcessInstanceForSingleActivityInstantiation()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task1").execute();

		// then
		HistoricProcessInstance historicInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicInstance);
		assertEquals(instance.Id, historicInstance.Id);
		assertNotNull(historicInstance.StartTime);
		assertNull(historicInstance.EndTime);

		// should be the first activity started
		assertEquals("task1", historicInstance.StartActivityId);

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().singleResult();
		assertNotNull(historicActivityInstance);
		assertEquals("task1", historicActivityInstance.ActivityId);
		assertNotNull(historicActivityInstance.Id);
		assertFalse(instance.Id.Equals(historicActivityInstance.Id));
		assertNotNull(historicActivityInstance.StartTime);
		assertNull(historicActivityInstance.EndTime);
	  }

	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testHistoricActivityInstancesForSubprocess()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("subprocess").startBeforeActivity("innerTask").startBeforeActivity("theSubProcessStart").execute();

		// then
		HistoricProcessInstance historicInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicInstance);
		assertEquals(instance.Id, historicInstance.Id);
		assertNotNull(historicInstance.StartTime);
		assertNull(historicInstance.EndTime);

		// should be the first activity started
		assertEquals("innerTask", historicInstance.StartActivityId);

		// subprocess, subprocess start event, two innerTasks
		assertEquals(4, historyService.createHistoricActivityInstanceQuery().count());

		HistoricActivityInstance subProcessInstance = historyService.createHistoricActivityInstanceQuery().activityId("subProcess").singleResult();
		assertNotNull(subProcessInstance);
		assertEquals("subProcess", subProcessInstance.ActivityId);
		assertNotNull(subProcessInstance.Id);
		assertFalse(instance.Id.Equals(subProcessInstance.Id));
		assertNotNull(subProcessInstance.StartTime);
		assertNull(subProcessInstance.EndTime);

		HistoricActivityInstance startEventInstance = historyService.createHistoricActivityInstanceQuery().activityId("theSubProcessStart").singleResult();
		assertNotNull(startEventInstance);
		assertEquals("theSubProcessStart", startEventInstance.ActivityId);
		assertNotNull(startEventInstance.Id);
		assertFalse(instance.Id.Equals(startEventInstance.Id));
		assertNotNull(startEventInstance.StartTime);
		assertNotNull(startEventInstance.EndTime);

		IList<HistoricActivityInstance> innerTaskInstances = historyService.createHistoricActivityInstanceQuery().activityId("innerTask").list();

		assertEquals(2, innerTaskInstances.Count);

		foreach (HistoricActivityInstance innerTaskInstance in innerTaskInstances)
		{
		  assertNotNull(innerTaskInstance);
		  assertEquals("innerTask", innerTaskInstance.ActivityId);
		  assertNotNull(innerTaskInstance.Id);
		  assertFalse(instance.Id.Equals(innerTaskInstance.Id));
		  assertNotNull(innerTaskInstance.StartTime);
		  assertNull(innerTaskInstance.EndTime);
		}
	  }

	  [Deployment(resources : ASYNC_PROCESS)]
	  public virtual void testHistoricProcessInstanceAsyncStartEvent()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task2").setVariable("aVar", "aValue").execute();

		// then
		HistoricProcessInstance historicInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicInstance);
		assertEquals(instance.Id, historicInstance.Id);
		assertNotNull(historicInstance.StartTime);
		assertNull(historicInstance.EndTime);

		// should be the first activity started
		assertEquals("task2", historicInstance.StartActivityId);

		// task2 wasn't entered yet
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().count());

		// history events for variables exist already
		ActivityInstance activityInstance = runtimeService.getActivityInstance(instance.Id);

		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().variableName("aVar").singleResult();

		assertNotNull(historicVariable);
		assertEquals(instance.Id, historicVariable.ProcessInstanceId);
		assertEquals(activityInstance.Id, historicVariable.ActivityInstanceId);
		assertEquals("aVar", historicVariable.Name);
		assertEquals("aValue", historicVariable.Value);

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().variableInstanceId(historicVariable.Id).singleResult();
		assertEquals(instance.Id, historicDetail.ProcessInstanceId);
		assertNotNull(historicDetail);
		// TODO: fix if this is not ok due to CAM-3886
		assertNull(historicDetail.ActivityInstanceId);
		assertTrue(historicDetail is HistoricVariableUpdate);
		assertEquals("aVar", ((HistoricVariableUpdate) historicDetail).VariableName);
		assertEquals("aValue", ((HistoricVariableUpdate) historicDetail).Value);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testHistoricVariableInstanceForSingleActivityInstantiation()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task1").setVariable("aVar", "aValue").execute();

		ActivityInstance activityInstance = runtimeService.getActivityInstance(instance.Id);

		// then
		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().variableName("aVar").singleResult();

		assertNotNull(historicVariable);
		assertEquals(instance.Id, historicVariable.ProcessInstanceId);
		assertEquals(activityInstance.Id, historicVariable.ActivityInstanceId);
		assertEquals("aVar", historicVariable.Name);
		assertEquals("aValue", historicVariable.Value);

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().variableInstanceId(historicVariable.Id).singleResult();
		assertEquals(instance.Id, historicDetail.ProcessInstanceId);
		assertNotNull(historicDetail);
		// TODO: fix if this is not ok due to CAM-3886
		assertNull(historicDetail.ActivityInstanceId);
		assertTrue(historicDetail is HistoricVariableUpdate);
		assertEquals("aVar", ((HistoricVariableUpdate) historicDetail).VariableName);
		assertEquals("aValue", ((HistoricVariableUpdate) historicDetail).Value);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testHistoricVariableInstanceSetOnProcessInstance()
	  {
		// when
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").setVariable("aVar", "aValue").startBeforeActivity("task1").execute();

		ActivityInstance activityInstance = runtimeService.getActivityInstance(instance.Id);

		// then
		HistoricVariableInstance historicVariable = historyService.createHistoricVariableInstanceQuery().variableName("aVar").singleResult();

		assertNotNull(historicVariable);
		assertEquals(instance.Id, historicVariable.ProcessInstanceId);
		assertEquals(activityInstance.Id, historicVariable.ActivityInstanceId);
		assertEquals("aVar", historicVariable.Name);
		assertEquals("aValue", historicVariable.Value);

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().variableInstanceId(historicVariable.Id).singleResult();
		assertEquals(instance.Id, historicDetail.ProcessInstanceId);
		assertNotNull(historicDetail);
		// TODO: fix if this is not ok due to CAM-3886
		assertEquals(instance.Id, historicDetail.ActivityInstanceId);
		assertTrue(historicDetail is HistoricVariableUpdate);
		assertEquals("aVar", ((HistoricVariableUpdate) historicDetail).VariableName);
		assertEquals("aValue", ((HistoricVariableUpdate) historicDetail).Value);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testHistoricProcessInstanceForSynchronousCompletion()
	  {
		// when the process instance ends immediately
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("exclusiveGateway").startAfterActivity("task1").execute();

		// then
		HistoricProcessInstance historicInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicInstance);
		assertEquals(instance.Id, historicInstance.Id);
		assertNotNull(historicInstance.StartTime);
		assertNotNull(historicInstance.EndTime);

		assertEquals("join", historicInstance.StartActivityId);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testSkipCustomListenerEnsureHistoryWritten()
	  {
		// when creating the task skipping custom listeners
		runtimeService.createProcessInstanceByKey("exclusiveGateway").startBeforeActivity("task2").execute(true, false);

		// then the task assignment history (which uses a task listener) is written
		Task task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();

		HistoricActivityInstance instance = historyService.createHistoricActivityInstanceQuery().activityId("task2").singleResult();
		assertNotNull(instance);
		assertEquals(task.Id, instance.TaskId);
		assertEquals("kermit", instance.Assignee);
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