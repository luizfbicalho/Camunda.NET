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
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class ProcessInstanceModificationHistoryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string EXCLUSIVE_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGateway.bpmn20.xml";
	  protected internal const string EXCLUSIVE_GATEWAY_ASYNC_TASK_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.exclusiveGatewayAsyncTask.bpmn20.xml";
	  protected internal const string SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocess.bpmn20.xml";

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartBeforeWithVariablesInHistory()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").setVariable("procInstVar", "procInstValue").setVariableLocal("localVar", "localValue").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);

		HistoricVariableInstance procInstVariable = historyService.createHistoricVariableInstanceQuery().variableName("procInstVar").singleResult();

		assertNotNull(procInstVariable);
		assertEquals(updatedTree.Id, procInstVariable.ActivityInstanceId);
		assertEquals("procInstVar", procInstVariable.Name);
		assertEquals("procInstValue", procInstVariable.Value);

		HistoricDetail procInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(procInstVariable.Id).singleResult();
		assertNotNull(procInstanceVarDetail);
		// when starting before/after an activity instance, the activity instance id of the
		// execution is null and so is the activity instance id of the historic detail
		assertNull(procInstanceVarDetail.ActivityInstanceId);

		HistoricVariableInstance localVariable = historyService.createHistoricVariableInstanceQuery().variableName("localVar").singleResult();

		assertNotNull(localVariable);
		assertNull(localVariable.ActivityInstanceId);
		assertEquals("localVar", localVariable.Name);
		assertEquals("localValue", localVariable.Value);

		HistoricDetail localInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(localVariable.Id).singleResult();
		assertNotNull(localInstanceVarDetail);
		assertNull(localInstanceVarDetail.ActivityInstanceId);

		completeTasksInOrder("task1", "task2");
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_ASYNC_TASK_PROCESS)]
	  public virtual void testStartBeforeAsyncWithVariablesInHistory()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("task2").setVariable("procInstVar", "procInstValue").setVariableLocal("localVar", "localValue").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);

		HistoricVariableInstance procInstVariable = historyService.createHistoricVariableInstanceQuery().variableName("procInstVar").singleResult();

		assertNotNull(procInstVariable);
		assertEquals(updatedTree.Id, procInstVariable.ActivityInstanceId);
		assertEquals("procInstVar", procInstVariable.Name);
		assertEquals("procInstValue", procInstVariable.Value);

		HistoricDetail procInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(procInstVariable.Id).singleResult();
		assertNotNull(procInstanceVarDetail);
		// when starting before/after an activity instance, the activity instance id of the
		// execution is null and so is the activity instance id of the historic detail
		assertNull(procInstanceVarDetail.ActivityInstanceId);

		HistoricVariableInstance localVariable = historyService.createHistoricVariableInstanceQuery().variableName("localVar").singleResult();

		assertNotNull(localVariable);
		// the following is null because localVariable is local on a concurrent execution
		// but the concurrent execution does not execute an activity at the time the variable is set
		assertNull(localVariable.ActivityInstanceId);
		assertEquals("localVar", localVariable.Name);
		assertEquals("localValue", localVariable.Value);

		HistoricDetail localInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(localVariable.Id).singleResult();
		assertNotNull(localInstanceVarDetail);
		assertNull(localInstanceVarDetail.ActivityInstanceId);

		// end process instance
		completeTasksInOrder("task1");

		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		completeTasksInOrder("task2");
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testStartBeforeScopeWithVariablesInHistory()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subprocess");

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("innerTask").setVariable("procInstVar", "procInstValue").setVariableLocal("localVar", "localValue").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);

		HistoricVariableInstance procInstVariable = historyService.createHistoricVariableInstanceQuery().variableName("procInstVar").singleResult();

		assertNotNull(procInstVariable);
		assertEquals(updatedTree.Id, procInstVariable.ActivityInstanceId);
		assertEquals("procInstVar", procInstVariable.Name);
		assertEquals("procInstValue", procInstVariable.Value);

		HistoricDetail procInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(procInstVariable.Id).singleResult();
		assertNotNull(procInstanceVarDetail);
		// when starting before/after an activity instance, the activity instance id of the
		// execution is null and so is the activity instance id of the historic detail
		assertNull(procInstanceVarDetail.ActivityInstanceId);

		HistoricVariableInstance localVariable = historyService.createHistoricVariableInstanceQuery().variableName("localVar").singleResult();

		assertNotNull(localVariable);
		assertEquals(updatedTree.getActivityInstances("subProcess")[0].Id, localVariable.ActivityInstanceId);
		assertEquals("localVar", localVariable.Name);
		assertEquals("localValue", localVariable.Value);

		HistoricDetail localInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(localVariable.Id).singleResult();
		assertNotNull(localInstanceVarDetail);
		assertNull(localInstanceVarDetail.ActivityInstanceId);

	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testStartTransitionWithVariablesInHistory()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exclusiveGateway");

		runtimeService.createProcessInstanceModification(processInstance.Id).startTransition("flow2").setVariable("procInstVar", "procInstValue").setVariableLocal("localVar", "localValue").execute();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);

		HistoricVariableInstance procInstVariable = historyService.createHistoricVariableInstanceQuery().variableName("procInstVar").singleResult();

		assertNotNull(procInstVariable);
		assertEquals(updatedTree.Id, procInstVariable.ActivityInstanceId);
		assertEquals("procInstVar", procInstVariable.Name);
		assertEquals("procInstValue", procInstVariable.Value);

		HistoricDetail procInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(procInstVariable.Id).singleResult();
		assertNotNull(procInstanceVarDetail);
		assertEquals(updatedTree.Id, procInstVariable.ActivityInstanceId);

		HistoricVariableInstance localVariable = historyService.createHistoricVariableInstanceQuery().variableName("localVar").singleResult();

		assertNotNull(localVariable);
		assertEquals(updatedTree.Id, procInstVariable.ActivityInstanceId);
		assertEquals("localVar", localVariable.Name);
		assertEquals("localValue", localVariable.Value);

		HistoricDetail localInstanceVarDetail = historyService.createHistoricDetailQuery().variableInstanceId(localVariable.Id).singleResult();
		assertNotNull(localInstanceVarDetail);
		// when starting before/after an activity instance, the activity instance id of the
		// execution is null and so is the activity instance id of the historic detail
		assertNull(localInstanceVarDetail.ActivityInstanceId);

		completeTasksInOrder("task1", "task1");
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testCancelTaskShouldCancelProcessInstance()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		// when
		runtimeService.createProcessInstanceModification(processInstanceId).cancelAllForActivity("theTask").execute(true, false);

		// then
		HistoricProcessInstance instance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(instance);

		assertEquals(processInstanceId, instance.Id);
		assertNotNull(instance.EndTime);
	  }

	  [Deployment(resources : EXCLUSIVE_GATEWAY_PROCESS)]
	  public virtual void testSkipCustomListenerEnsureHistoryWritten()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("exclusiveGateway").Id;

		// when creating the task skipping custom listeners
		runtimeService.createProcessInstanceModification(processInstanceId).startBeforeActivity("task2").execute(true, false);

		// then the task assignment history (which uses a task listener) is written
		Task task = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult();

		HistoricActivityInstance instance = historyService.createHistoricActivityInstanceQuery().activityId("task2").singleResult();
		assertNotNull(instance);
		assertEquals(task.Id, instance.TaskId);
		assertEquals("kermit", instance.Assignee);
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