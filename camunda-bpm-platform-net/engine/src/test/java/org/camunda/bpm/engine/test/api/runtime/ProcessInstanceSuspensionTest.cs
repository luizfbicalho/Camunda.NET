using System;
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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using org.camunda.bpm.engine.runtime;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class ProcessInstanceSuspensionTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessInstanceActiveByDefault()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendActivateProcessInstance()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

		//suspend
		runtimeService.suspendProcessInstanceById(processInstance.Id);
		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertTrue(processInstance.Suspended);

		//activate
		runtimeService.activateProcessInstanceById(processInstance.Id);
		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendActivateProcessInstanceByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

		//suspend
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertTrue(processInstance.Suspended);

		//activate
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinition.Id);
		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendActivateProcessInstanceByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

		//suspend
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertTrue(processInstance.Suspended);

		//activate
		runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testActivateAlreadyActiveProcessInstance()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

		try
		{
		  //activate
		  runtimeService.activateProcessInstanceById(processInstance.Id);
		  processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		  assertFalse(processInstance.Suspended);
		}
		catch (ProcessEngineException)
		{
		  fail("Should not fail");
		}

		try
		{
		  //activate
		  runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinition.Id);
		  processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		  assertFalse(processInstance.Suspended);
		}
		catch (ProcessEngineException)
		{
		  fail("Should not fail");
		}

		try
		{
		  //activate
		  runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		  processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		  assertFalse(processInstance.Suspended);
		}
		catch (ProcessEngineException)
		{
		  fail("Should not fail");
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendAlreadySuspendedProcessInstance()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

		runtimeService.suspendProcessInstanceById(processInstance.Id);

		try
		{
		  runtimeService.suspendProcessInstanceById(processInstance.Id);
		  processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		  assertTrue(processInstance.Suspended);
		}
		catch (ProcessEngineException)
		{
		  fail("Should not fail");
		}

		try
		{
		  runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
		  processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		  assertTrue(processInstance.Suspended);
		}
		catch (ProcessEngineException)
		{
		  fail("Should not fail");
		}

		try
		{
		  runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		  processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		  assertTrue(processInstance.Suspended);
		}
		catch (ProcessEngineException)
		{
		  fail("Should not fail");
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/superProcessWithMultipleNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryForActiveAndSuspendedProcessInstances()
	  {
		runtimeService.startProcessInstanceByKey("nestedSubProcessQueryTest");

		assertEquals(5, runtimeService.createProcessInstanceQuery().count());
		assertEquals(5, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());

		ProcessInstance piToSuspend = runtimeService.createProcessInstanceQuery().processDefinitionKey("nestedSubProcessQueryTest").singleResult();
		runtimeService.suspendProcessInstanceById(piToSuspend.Id);

		assertEquals(5, runtimeService.createProcessInstanceQuery().count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().suspended().count());

		assertEquals(piToSuspend.Id, runtimeService.createProcessInstanceQuery().suspended().singleResult().Id);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/superProcessWithMultipleNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryForActiveAndSuspendedProcessInstancesByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("nestedSubProcessQueryTest").singleResult();

		runtimeService.startProcessInstanceByKey("nestedSubProcessQueryTest");

		assertEquals(5, runtimeService.createProcessInstanceQuery().count());
		assertEquals(5, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());

		ProcessInstance piToSuspend = runtimeService.createProcessInstanceQuery().processDefinitionKey("nestedSubProcessQueryTest").singleResult();
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		assertEquals(5, runtimeService.createProcessInstanceQuery().count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().suspended().count());

		assertEquals(piToSuspend.Id, runtimeService.createProcessInstanceQuery().suspended().singleResult().Id);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/runtime/superProcessWithMultipleNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryForActiveAndSuspendedProcessInstancesByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("nestedSubProcessQueryTest").singleResult();

		runtimeService.startProcessInstanceByKey("nestedSubProcessQueryTest");

		assertEquals(5, runtimeService.createProcessInstanceQuery().count());
		assertEquals(5, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());

		ProcessInstance piToSuspend = runtimeService.createProcessInstanceQuery().processDefinitionKey("nestedSubProcessQueryTest").singleResult();
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		assertEquals(5, runtimeService.createProcessInstanceQuery().count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().suspended().count());

		assertEquals(piToSuspend.Id, runtimeService.createProcessInstanceQuery().suspended().singleResult().Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskSuspendedAfterProcessInstanceSuspension()
	  {

		// Start Process Instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspense process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// Assert that the task is now also suspended
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		foreach (Task task in tasks)
		{
		  assertTrue(task.Suspended);
		}

		// Activate process instance again
		runtimeService.activateProcessInstanceById(processInstance.Id);
		tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		foreach (Task task in tasks)
		{
		  assertFalse(task.Suspended);
		}
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-9505
	  /// </summary>
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testPreserveCreateTimeOnUpdatedTask()
	  {
		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		Task taskBeforeSuspension = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		DateTime createTime = taskBeforeSuspension.CreateTime;

		// when
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// assume
		assertTrue(task.Suspended);

		// then
		assertEquals(createTime, task.CreateTime);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskSuspendedAfterProcessInstanceSuspensionByProcessDefinitionId()
	  {

		// Start Process Instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspense process instance
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		// Assert that the task is now also suspended
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		foreach (Task task in tasks)
		{
		  assertTrue(task.Suspended);
		}

		// Activate process instance again
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinition.Id);
		tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		foreach (Task task in tasks)
		{
		  assertFalse(task.Suspended);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskSuspendedAfterProcessInstanceSuspensionByProcessDefinitionKey()
	  {

		// Start Process Instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspense process instance
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		// Assert that the task is now also suspended
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		foreach (Task task in tasks)
		{
		  assertTrue(task.Suspended);
		}

		// Activate process instance again
		runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		foreach (Task task in tasks)
		{
		  assertFalse(task.Suspended);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskQueryAfterProcessInstanceSuspend()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		task = taskService.createTaskQuery().active().singleResult();
		assertNotNull(task);

		// Suspend
		runtimeService.suspendProcessInstanceById(processInstance.Id);
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().suspended().count());
		assertEquals(0, taskService.createTaskQuery().active().count());

		// Activate
		runtimeService.activateProcessInstanceById(processInstance.Id);
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(0, taskService.createTaskQuery().suspended().count());
		assertEquals(1, taskService.createTaskQuery().active().count());

		// Completing should end the process instance
		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskQueryAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		task = taskService.createTaskQuery().active().singleResult();
		assertNotNull(task);

		// Suspend
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().suspended().count());
		assertEquals(0, taskService.createTaskQuery().active().count());

		// Activate
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinition.Id);
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(0, taskService.createTaskQuery().suspended().count());
		assertEquals(1, taskService.createTaskQuery().active().count());

		// Completing should end the process instance
		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskQueryAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		task = taskService.createTaskQuery().active().singleResult();
		assertNotNull(task);

		// Suspend
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().suspended().count());
		assertEquals(0, taskService.createTaskQuery().active().count());

		// Activate
		runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(0, taskService.createTaskQuery().suspended().count());
		assertEquals(1, taskService.createTaskQuery().active().count());

		// Completing should end the process instance
		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testChildExecutionsSuspendedAfterProcessInstanceSuspend()
	  public virtual void testChildExecutionsSuspendedAfterProcessInstanceSuspend()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testChildExecutionsSuspended");
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution execution in executions)
		{
		  assertTrue(execution.Suspended);
		}

		// Activate again
		runtimeService.activateProcessInstanceById(processInstance.Id);
		executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution execution in executions)
		{
		  assertFalse(execution.Suspended);
		}

		// Finish process
		while (taskService.createTaskQuery().count() > 0)
		{
		  foreach (Task task in taskService.createTaskQuery().list())
		  {
			taskService.complete(task.Id);
		  }
		}
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testChildExecutionsSuspendedAfterProcessInstanceSuspend.bpmn20.xml"})]
	  public virtual void testChildExecutionsSuspendedAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testChildExecutionsSuspended");
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);

		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution execution in executions)
		{
		  assertTrue(execution.Suspended);
		}

		// Activate again
		runtimeService.activateProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);
		executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution execution in executions)
		{
		  assertFalse(execution.Suspended);
		}

		// Finish process
		while (taskService.createTaskQuery().count() > 0)
		{
		  foreach (Task task in taskService.createTaskQuery().list())
		  {
			taskService.complete(task.Id);
		  }
		}
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testChildExecutionsSuspendedAfterProcessInstanceSuspend.bpmn20.xml"})]
	  public virtual void testChildExecutionsSuspendedAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testChildExecutionsSuspended");
		runtimeService.suspendProcessInstanceByProcessDefinitionKey("testChildExecutionsSuspended");

		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution execution in executions)
		{
		  assertTrue(execution.Suspended);
		}

		// Activate again
		runtimeService.activateProcessInstanceByProcessDefinitionKey("testChildExecutionsSuspended");
		executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution execution in executions)
		{
		  assertFalse(execution.Suspended);
		}

		// Finish process
		while (taskService.createTaskQuery().count() > 0)
		{
		  foreach (Task task in taskService.createTaskQuery().list())
		  {
			taskService.complete(task.Id);
		  }
		}
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testChangeVariablesAfterProcessInstanceSuspend()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		try
		{
		  runtimeService.removeVariable(processInstance.Id, "someVariable");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariableLocal(processInstance.Id, "someVariable");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariables(processInstance.Id, Arrays.asList("one", "two", "three"));
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}


		try
		{
		  runtimeService.removeVariablesLocal(processInstance.Id, Arrays.asList("one", "two", "three"));
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariable(processInstance.Id, "someVariable", "someValue");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariableLocal(processInstance.Id, "someVariable", "someValue");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariables(processInstance.Id, new Dictionary<string, object>());
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariablesLocal(processInstance.Id, new Dictionary<string, object>());
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testChangeVariablesAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);

		try
		{
		  runtimeService.removeVariable(processInstance.Id, "someVariable");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariableLocal(processInstance.Id, "someVariable");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariables(processInstance.Id, Arrays.asList("one", "two", "three"));
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}


		try
		{
		  runtimeService.removeVariablesLocal(processInstance.Id, Arrays.asList("one", "two", "three"));
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariable(processInstance.Id, "someVariable", "someValue");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariableLocal(processInstance.Id, "someVariable", "someValue");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariables(processInstance.Id, new Dictionary<string, object>());
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariablesLocal(processInstance.Id, new Dictionary<string, object>());
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testChangeVariablesAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		try
		{
		  runtimeService.removeVariable(processInstance.Id, "someVariable");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariableLocal(processInstance.Id, "someVariable");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariables(processInstance.Id, Arrays.asList("one", "two", "three"));
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.removeVariablesLocal(processInstance.Id, Arrays.asList("one", "two", "three"));
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariable(processInstance.Id, "someVariable", "someValue");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariableLocal(processInstance.Id, "someVariable", "someValue");
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariables(processInstance.Id, new Dictionary<string, object>());
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}

		try
		{
		  runtimeService.setVariablesLocal(processInstance.Id, new Dictionary<string, object>());
		}
		catch (ProcessEngineException)
		{
		  fail("This should be possible");
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskFormFailAfterProcessInstanceSuspend()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		try
		{
		  formService.submitTaskFormData(taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult().Id, new Dictionary<string, string>());
		  fail();
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskFormFailAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		try
		{
		  formService.submitTaskFormData(taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult().Id, new Dictionary<string, string>());
		  fail();
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskFormFailAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		try
		{
		  formService.submitTaskFormData(taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult().Id, new Dictionary<string, string>());
		  fail();
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessInstanceSignalFailAfterSuspend()
	  {

		// Suspend process instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		try
		{
		  runtimeService.signal(processInstance.Id);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		  assertTrue(e is BadUserRequestException);
		}

		try
		{
		  runtimeService.signal(processInstance.Id, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		  assertTrue(e is BadUserRequestException);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessInstanceSignalFailAfterSuspendByProcessDefinitionId()
	  {

		// Suspend process instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		try
		{
		  runtimeService.signal(processInstance.Id);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		  assertTrue(e is BadUserRequestException);
		}

		try
		{
		  runtimeService.signal(processInstance.Id, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		  assertTrue(e is BadUserRequestException);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessInstanceSignalFailAfterSuspendByProcessDefinitionKey()
	  {

		// Suspend process instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		try
		{
		  runtimeService.signal(processInstance.Id);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		  assertTrue(e is BadUserRequestException);
		}

		try
		{
		  runtimeService.signal(processInstance.Id, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		  assertTrue(e is BadUserRequestException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageEventReceiveFailAfterSuspend()
	  public virtual void testMessageEventReceiveFailAfterSuspend()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceById(processInstance.Id);
		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();

		try
		{
		  runtimeService.messageEventReceived("someMessage", subscription.ExecutionId);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		try
		{
		  runtimeService.messageEventReceived("someMessage", subscription.ExecutionId, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testMessageEventReceiveFailAfterSuspend.bpmn20.xml"})]
	  public virtual void testMessageEventReceiveFailAfterSuspendByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();

		try
		{
		  runtimeService.messageEventReceived("someMessage", subscription.ExecutionId);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		try
		{
		  runtimeService.messageEventReceived("someMessage", subscription.ExecutionId, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testMessageEventReceiveFailAfterSuspend.bpmn20.xml"})]
	  public virtual void testMessageEventReceiveFailAfterSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();

		try
		{
		  runtimeService.messageEventReceived("someMessage", subscription.ExecutionId);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		try
		{
		  runtimeService.messageEventReceived("someMessage", subscription.ExecutionId, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSignalEventReceivedAfterProcessInstanceSuspended()
	  public virtual void testSignalEventReceivedAfterProcessInstanceSuspended()
	  {

		const string signal = "Some Signal";

		// Test if process instance can be completed using the signal
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("signalSuspendedProcessInstance");
		runtimeService.signalEventReceived(signal);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// Now test when suspending the process instance: the process instance shouldn't be continued
		processInstance = runtimeService.startProcessInstanceByKey("signalSuspendedProcessInstance");
		runtimeService.suspendProcessInstanceById(processInstance.Id);
		runtimeService.signalEventReceived(signal);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.signalEventReceived(signal, new Dictionary<string, object>());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();
		try
		{
		  runtimeService.signalEventReceived(signal, subscription.ExecutionId);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		try
		{
		  runtimeService.signalEventReceived(signal, subscription.ExecutionId, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		// Activate and try again
		runtimeService.activateProcessInstanceById(processInstance.Id);
		runtimeService.signalEventReceived(signal);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testSignalEventReceivedAfterProcessInstanceSuspended.bpmn20.xml"})]
	  public virtual void testSignalEventReceivedAfterProcessInstanceSuspendedByProcessDefinitionId()
	  {

		const string signal = "Some Signal";

		// Test if process instance can be completed using the signal
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("signalSuspendedProcessInstance");
		runtimeService.signalEventReceived(signal);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// Now test when suspending the process instance: the process instance shouldn't be continued
		processInstance = runtimeService.startProcessInstanceByKey("signalSuspendedProcessInstance");
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);
		runtimeService.signalEventReceived(signal);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.signalEventReceived(signal, new Dictionary<string, object>());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();
		try
		{
		  runtimeService.signalEventReceived(signal, subscription.ExecutionId);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		try
		{
		  runtimeService.signalEventReceived(signal, subscription.ExecutionId, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		// Activate and try again
		runtimeService.activateProcessInstanceById(processInstance.Id);
		runtimeService.signalEventReceived(signal);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testSignalEventReceivedAfterProcessInstanceSuspended.bpmn20.xml"})]
	  public virtual void testSignalEventReceivedAfterProcessInstanceSuspendedByProcessDefinitionKey()
	  {

		const string signal = "Some Signal";

		// Test if process instance can be completed using the signal
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("signalSuspendedProcessInstance");
		runtimeService.signalEventReceived(signal);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		// Now test when suspending the process instance: the process instance shouldn't be continued
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("signalSuspendedProcessInstance").singleResult();

		processInstance = runtimeService.startProcessInstanceByKey("signalSuspendedProcessInstance");
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		runtimeService.signalEventReceived(signal);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		runtimeService.signalEventReceived(signal, new Dictionary<string, object>());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		EventSubscription subscription = runtimeService.createEventSubscriptionQuery().singleResult();
		try
		{
		  runtimeService.signalEventReceived(signal, subscription.ExecutionId);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		try
		{
		  runtimeService.signalEventReceived(signal, subscription.ExecutionId, new Dictionary<string, object>());
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  // This is expected
		  assertTextPresent("is suspended", e.Message);
		}

		// Activate and try again
		runtimeService.activateProcessInstanceById(processInstance.Id);
		runtimeService.signalEventReceived(signal);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskLifecycleOperationsFailAfterProcessInstanceSuspend()
	  {

		// Start a new process instance with one task
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);

		// Suspend the process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// Completing the task should fail
		try
		{
		  taskService.complete(task.Id);
		  fail("It is not allowed to complete a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Claiming the task should fail
		try
		{
		  taskService.claim(task.Id, "jos");
		  fail("It is not allowed to claim a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}



		// Adding candidate groups on the task should fail
		try
		{
		  taskService.addCandidateGroup(task.Id, "blahGroup");
		  fail("It is not allowed to add a candidate group on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding candidate users on the task should fail
		try
		{
		  taskService.addCandidateUser(task.Id, "blahUser");
		  fail("It is not allowed to add a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding group identity links on the task should fail
		try
		{
		  taskService.addGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.CANDIDATE);
		  fail("It is not allowed to add a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding an identity link on the task should fail
		try
		{
		  taskService.addUserIdentityLink(task.Id, "blahUser", IdentityLinkType.OWNER);
		  fail("It is not allowed to add an identityLink on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}


		// Set an assignee on the task should fail
		try
		{
		  taskService.setAssignee(task.Id, "mispiggy");
		  fail("It is not allowed to set an assignee on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Set an owner on the task should fail
		try
		{
		  taskService.setOwner(task.Id, "kermit");
		  fail("It is not allowed to set an owner on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing candidate groups on the task should fail
		try
		{
		  taskService.deleteCandidateGroup(task.Id, "blahGroup");
		  fail("It is not allowed to remove a candidate group on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing candidate users on the task should fail
		try
		{
		  taskService.deleteCandidateUser(task.Id, "blahUser");
		  fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing group identity links on the task should fail
		try
		{
		  taskService.deleteGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.CANDIDATE);
		  fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing an identity link on the task should fail
		try
		{
		  taskService.deleteUserIdentityLink(task.Id, "blahUser", IdentityLinkType.OWNER);
		  fail("It is not allowed to remove an identityLink on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskLifecycleOperationsFailAfterProcessInstanceSuspendByProcessDefinitionId()
	  {

		// Start a new process instance with one task
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);

		// Suspend the process instance
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		// Completing the task should fail
		try
		{
		  taskService.complete(task.Id);
		  fail("It is not allowed to complete a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Claiming the task should fail
		try
		{
		  taskService.claim(task.Id, "jos");
		  fail("It is not allowed to claim a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}



		// Adding candidate groups on the task should fail
		try
		{
		  taskService.addCandidateGroup(task.Id, "blahGroup");
		  fail("It is not allowed to add a candidate group on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding candidate users on the task should fail
		try
		{
		  taskService.addCandidateUser(task.Id, "blahUser");
		  fail("It is not allowed to add a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding group identity links on the task should fail
		try
		{
		  taskService.addGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.CANDIDATE);
		  fail("It is not allowed to add a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding an identity link on the task should fail
		try
		{
		  taskService.addUserIdentityLink(task.Id, "blahUser", IdentityLinkType.OWNER);
		  fail("It is not allowed to add an identityLink on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}


		// Set an assignee on the task should fail
		try
		{
		  taskService.setAssignee(task.Id, "mispiggy");
		  fail("It is not allowed to set an assignee on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Set an owner on the task should fail
		try
		{
		  taskService.setOwner(task.Id, "kermit");
		  fail("It is not allowed to set an owner on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing candidate groups on the task should fail
		try
		{
		  taskService.deleteCandidateGroup(task.Id, "blahGroup");
		  fail("It is not allowed to remove a candidate group on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing candidate users on the task should fail
		try
		{
		  taskService.deleteCandidateUser(task.Id, "blahUser");
		  fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing group identity links on the task should fail
		try
		{
		  taskService.deleteGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.CANDIDATE);
		  fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing an identity link on the task should fail
		try
		{
		  taskService.deleteUserIdentityLink(task.Id, "blahUser", IdentityLinkType.OWNER);
		  fail("It is not allowed to remove an identityLink on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskLifecycleOperationsFailAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {

		// Start a new process instance with one task
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);

		// Suspend the process instance
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		// Completing the task should fail
		try
		{
		  taskService.complete(task.Id);
		  fail("It is not allowed to complete a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Claiming the task should fail
		try
		{
		  taskService.claim(task.Id, "jos");
		  fail("It is not allowed to claim a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}



		// Adding candidate groups on the task should fail
		try
		{
		  taskService.addCandidateGroup(task.Id, "blahGroup");
		  fail("It is not allowed to add a candidate group on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding candidate users on the task should fail
		try
		{
		  taskService.addCandidateUser(task.Id, "blahUser");
		  fail("It is not allowed to add a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding group identity links on the task should fail
		try
		{
		  taskService.addGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.CANDIDATE);
		  fail("It is not allowed to add a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Adding an identity link on the task should fail
		try
		{
		  taskService.addUserIdentityLink(task.Id, "blahUser", IdentityLinkType.OWNER);
		  fail("It is not allowed to add an identityLink on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}


		// Set an assignee on the task should fail
		try
		{
		  taskService.setAssignee(task.Id, "mispiggy");
		  fail("It is not allowed to set an assignee on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Set an owner on the task should fail
		try
		{
		  taskService.setOwner(task.Id, "kermit");
		  fail("It is not allowed to set an owner on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing candidate groups on the task should fail
		try
		{
		  taskService.deleteCandidateGroup(task.Id, "blahGroup");
		  fail("It is not allowed to remove a candidate group on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing candidate users on the task should fail
		try
		{
		  taskService.deleteCandidateUser(task.Id, "blahUser");
		  fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing group identity links on the task should fail
		try
		{
		  taskService.deleteGroupIdentityLink(task.Id, "blahGroup", IdentityLinkType.CANDIDATE);
		  fail("It is not allowed to remove a candidate user on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}

		// Removing an identity link on the task should fail
		try
		{
		  taskService.deleteUserIdentityLink(task.Id, "blahUser", IdentityLinkType.OWNER);
		  fail("It is not allowed to remove an identityLink on a task of a suspended process instance");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // This is good
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubTaskCreationFailAfterProcessInstanceSuspend()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		Task subTask = taskService.newTask("someTaskId");
		subTask.ParentTaskId = task.Id;

		try
		{
		  taskService.saveTask(subTask);
		  fail("Creating sub tasks for suspended task should not be possible");
		}
		catch (SuspendedEntityInteractionException)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubTaskCreationFailAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);

		Task subTask = taskService.newTask("someTaskId");
		subTask.ParentTaskId = task.Id;

		try
		{
		  taskService.saveTask(subTask);
		  fail("Creating sub tasks for suspended task should not be possible");
		}
		catch (SuspendedEntityInteractionException)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubTaskCreationFailAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		Task subTask = taskService.newTask("someTaskId");
		subTask.ParentTaskId = task.Id;

		try
		{
		  taskService.saveTask(subTask);
		  fail("Creating sub tasks for suspended task should not be possible");
		}
		catch (SuspendedEntityInteractionException)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskNonLifecycleOperationsSucceedAfterProcessInstanceSuspend()
	  {

		// Start a new process instance with one task
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		runtimeService.suspendProcessInstanceById(processInstance.Id);
		assertNotNull(task);

		try
		{
		  taskService.setVariable(task.Id, "someVar", "someValue");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.setVariableLocal(task.Id, "someVar", "someValue");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  Dictionary<string, string> variables = new Dictionary<string, string>();
		  variables["varOne"] = "one";
		  variables["varTwo"] = "two";
		  taskService.setVariables(task.Id, variables);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  Dictionary<string, string> variables = new Dictionary<string, string>();
		  variables["varOne"] = "one";
		  variables["varTwo"] = "two";
		  taskService.setVariablesLocal(task.Id, variables);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariable(task.Id, "someVar");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariableLocal(task.Id, "someVar");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariables(task.Id, Arrays.asList("one", "two"));
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariablesLocal(task.Id, Arrays.asList("one", "two"));
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		if (processEngineConfiguration.HistoryLevel.Id > org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		{

		  try
		  {
			taskService.createComment(task.Id, processInstance.Id, "test comment");
		  }
		  catch (SuspendedEntityInteractionException)
		  {
			fail("should be allowed");
		  }

		  try
		  {
			taskService.createAttachment("text", task.Id, processInstance.Id, "tesTastName", "testDescription", "http://test.com");
		  }
		  catch (SuspendedEntityInteractionException)
		  {
			fail("should be allowed");
		  }

		}


		try
		{
		  taskService.setPriority(task.Id, 99);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskNonLifecycleOperationsSucceedAfterProcessInstanceSuspendByProcessDefinitionId()
	  {

		// Start a new process instance with one task
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processInstance.ProcessDefinitionId);
		assertNotNull(task);

		try
		{
		  taskService.setVariable(task.Id, "someVar", "someValue");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.setVariableLocal(task.Id, "someVar", "someValue");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  Dictionary<string, string> variables = new Dictionary<string, string>();
		  variables["varOne"] = "one";
		  variables["varTwo"] = "two";
		  taskService.setVariables(task.Id, variables);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  Dictionary<string, string> variables = new Dictionary<string, string>();
		  variables["varOne"] = "one";
		  variables["varTwo"] = "two";
		  taskService.setVariablesLocal(task.Id, variables);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariable(task.Id, "someVar");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariableLocal(task.Id, "someVar");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariables(task.Id, Arrays.asList("one", "two"));
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariablesLocal(task.Id, Arrays.asList("one", "two"));
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		if (processEngineConfiguration.HistoryLevel.Id > org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		{

		  try
		  {
			taskService.createComment(task.Id, processInstance.Id, "test comment");
		  }
		  catch (SuspendedEntityInteractionException)
		  {
			fail("should be allowed");
		  }

		  try
		  {
			taskService.createAttachment("text", task.Id, processInstance.Id, "tesTastName", "testDescription", "http://test.com");
		  }
		  catch (SuspendedEntityInteractionException)
		  {
			fail("should be allowed");
		  }

		}


		try
		{
		  taskService.setPriority(task.Id, 99);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testTaskNonLifecycleOperationsSucceedAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {

		// Start a new process instance with one task
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		assertNotNull(task);

		try
		{
		  taskService.setVariable(task.Id, "someVar", "someValue");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.setVariableLocal(task.Id, "someVar", "someValue");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  Dictionary<string, string> variables = new Dictionary<string, string>();
		  variables["varOne"] = "one";
		  variables["varTwo"] = "two";
		  taskService.setVariables(task.Id, variables);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  Dictionary<string, string> variables = new Dictionary<string, string>();
		  variables["varOne"] = "one";
		  variables["varTwo"] = "two";
		  taskService.setVariablesLocal(task.Id, variables);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariable(task.Id, "someVar");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariableLocal(task.Id, "someVar");
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariables(task.Id, Arrays.asList("one", "two"));
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		try
		{
		  taskService.removeVariablesLocal(task.Id, Arrays.asList("one", "two"));
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}

		if (processEngineConfiguration.HistoryLevel.Id > org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		{

		  try
		  {
			taskService.createComment(task.Id, processInstance.Id, "test comment");
		  }
		  catch (SuspendedEntityInteractionException)
		  {
			fail("should be allowed");
		  }

		  try
		  {
			taskService.createAttachment("text", task.Id, processInstance.Id, "tesTastName", "testDescription", "http://test.com");
		  }
		  catch (SuspendedEntityInteractionException)
		  {
			fail("should be allowed");
		  }

		}


		try
		{
		  taskService.setPriority(task.Id, 99);
		}
		catch (SuspendedEntityInteractionException)
		{
		  fail("should be allowed");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJobNotExecutedAfterProcessInstanceSuspend()
	  public virtual void testJobNotExecutedAfterProcessInstanceSuspend()
	  {

		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;

		// Suspending the process instance should also stop the execution of jobs for that process instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(1, managementService.createJobQuery().count());
		runtimeService.suspendProcessInstanceById(processInstance.Id);
		assertEquals(1, managementService.createJobQuery().count());

		// The jobs should not be executed now
		ClockUtil.CurrentTime = new DateTime(now.Ticks + (60 * 60 * 1000)); // Timer is set to fire on 5 minutes
		assertEquals(0, managementService.createJobQuery().executable().count());

		// Activation of the process instance should now allow for job execution
		runtimeService.activateProcessInstanceById(processInstance.Id);
		assertEquals(1, managementService.createJobQuery().executable().count());
		managementService.executeJob(managementService.createJobQuery().singleResult().Id);
		assertEquals(0, managementService.createJobQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testJobNotExecutedAfterProcessInstanceSuspend.bpmn20.xml"})]
	  public virtual void testJobNotExecutedAfterProcessInstanceSuspendByProcessDefinitionId()
	  {

		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;

		// Suspending the process instance should also stop the execution of jobs for that process instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(1, managementService.createJobQuery().count());
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processDefinition.Id);
		assertEquals(1, managementService.createJobQuery().count());

		// The jobs should not be executed now
		ClockUtil.CurrentTime = new DateTime(now.Ticks + (60 * 60 * 1000)); // Timer is set to fire on 5 minutes
		assertEquals(0, managementService.createJobQuery().executable().count());

		// Activation of the process instance should now allow for job execution
		runtimeService.activateProcessInstanceByProcessDefinitionId(processDefinition.Id);
		assertEquals(1, managementService.createJobQuery().executable().count());
		managementService.executeJob(managementService.createJobQuery().singleResult().Id);
		assertEquals(0, managementService.createJobQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.testJobNotExecutedAfterProcessInstanceSuspend.bpmn20.xml"})]
	  public virtual void testJobNotExecutedAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {

		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;

		// Suspending the process instance should also stop the execution of jobs for that process instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(1, managementService.createJobQuery().count());
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		assertEquals(1, managementService.createJobQuery().count());

		// The jobs should not be executed now
		ClockUtil.CurrentTime = new DateTime(now.Ticks + (60 * 60 * 1000)); // Timer is set to fire on 5 minutes
		assertEquals(0, managementService.createJobQuery().executable().count());

		// Activation of the process instance should now allow for job execution
		runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		assertEquals(1, managementService.createJobQuery().executable().count());
		managementService.executeJob(managementService.createJobQuery().singleResult().Id);
		assertEquals(0, managementService.createJobQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.callSimpleProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testCallActivityReturnAfterProcessInstanceSuspend()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callSimpleProcess");
		runtimeService.suspendProcessInstanceById(instance.Id);

		Task task = taskService.createTaskQuery().singleResult();

		try
		{
		  taskService.complete(task.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		// should be successful after reactivation
		runtimeService.activateProcessInstanceById(instance.Id);
		taskService.complete(task.Id);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.callSimpleProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testCallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callSimpleProcess");
		runtimeService.suspendProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);

		Task task = taskService.createTaskQuery().singleResult();

		try
		{
		  taskService.complete(task.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		// should be successful after reactivation
		runtimeService.activateProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);
		taskService.complete(task.Id);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.callSimpleProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testCallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("callSimpleProcess").singleResult();

		runtimeService.startProcessInstanceByKey("callSimpleProcess");
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		Task task = taskService.createTaskQuery().singleResult();

		try
		{
		  taskService.complete(task.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		// should be successful after reactivation
		runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		taskService.complete(task.Id);

		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.callMISimpleProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testMICallActivityReturnAfterProcessInstanceSuspend()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callMISimpleProcess");
		runtimeService.suspendProcessInstanceById(instance.Id);

		IList<Task> tasks = taskService.createTaskQuery().list();
		Task task1 = tasks[0];
		Task task2 = tasks[1];

		try
		{
		  taskService.complete(task1.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		try
		{
		  taskService.complete(task2.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		// should be successful after reactivation
		runtimeService.activateProcessInstanceById(instance.Id);
		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.callMISimpleProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testMICallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionId()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("callMISimpleProcess");
		runtimeService.suspendProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);

		IList<Task> tasks = taskService.createTaskQuery().list();
		Task task1 = tasks[0];
		Task task2 = tasks[1];

		try
		{
		  taskService.complete(task1.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		try
		{
		  taskService.complete(task2.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		// should be successful after reactivation
		runtimeService.activateProcessInstanceByProcessDefinitionId(instance.ProcessDefinitionId);
		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/ProcessInstanceSuspensionTest.callMISimpleProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testMICallActivityReturnAfterProcessInstanceSuspendByProcessDefinitionKey()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("callMISimpleProcess").singleResult();
		runtimeService.startProcessInstanceByKey("callMISimpleProcess");
		runtimeService.suspendProcessInstanceByProcessDefinitionKey(processDefinition.Key);

		IList<Task> tasks = taskService.createTaskQuery().list();
		Task task1 = tasks[0];
		Task task2 = tasks[1];

		try
		{
		  taskService.complete(task1.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		try
		{
		  taskService.complete(task2.Id);
		  fail("this should not be successful, as the execution of a suspended instance is resumed");
		}
		catch (SuspendedEntityInteractionException)
		{
		  // this is expected to fail
		}

		// should be successful after reactivation
		runtimeService.activateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		taskService.complete(task1.Id);
		taskService.complete(task2.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartBeforeActivityForSuspendProcessInstance()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		//start process instance
		runtimeService.startProcessInstanceById(processDefinition.Id);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspend process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// try to start before activity for suspended processDefinition
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();
		  fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartAfterActivityForSuspendProcessInstance()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		//start process instance
		runtimeService.startProcessInstanceById(processDefinition.Id);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspend process instance
		runtimeService.suspendProcessInstanceById(processInstance.Id);

		// try to start after activity for suspended processDefinition
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theTask").execute();
		  fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspensionByIdCascadesToExternalTasks()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		ExternalTask task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(task1.Suspended);

		// when the process instance is suspended
		runtimeService.suspendProcessInstanceById(processInstance1.Id);

		// then the task is suspended
		task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(task1.Suspended);

		// the other task is not
		ExternalTask task2 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(task2.Suspended);

		// when it is activated again
		runtimeService.activateProcessInstanceById(processInstance1.Id);

		// then the task is activated too
		task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(task1.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspensionByProcessDefinitionIdCascadesToExternalTasks()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		ExternalTask task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(task1.Suspended);

		// when the process instance is suspended
		runtimeService.suspendProcessInstanceByProcessDefinitionId(processInstance1.ProcessDefinitionId);

		// then the task is suspended
		task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(task1.Suspended);

		// the other task is not
		ExternalTask task2 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(task2.Suspended);

		// when it is activated again
		runtimeService.activateProcessInstanceByProcessDefinitionId(processInstance1.ProcessDefinitionId);

		// then the task is activated too
		task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(task1.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspensionByProcessDefinitionKeyCascadesToExternalTasks()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		ExternalTask task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(task1.Suspended);

		// when the process instance is suspended
		runtimeService.suspendProcessInstanceByProcessDefinitionKey("oneExternalTaskProcess");

		// then the task is suspended
		task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(task1.Suspended);

		// the other task is not
		ExternalTask task2 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(task2.Suspended);

		// when it is activated again
		runtimeService.activateProcessInstanceByProcessDefinitionKey("oneExternalTaskProcess");

		// then the task is activated too
		task1 = externalTaskService.createExternalTaskQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(task1.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendAndActivateProcessInstanceByIdUsingBuilder()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);

		//suspend
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceId(processInstance.Id).suspend();

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		//activate
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceId(processInstance.Id).activate();

		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendAndActivateProcessInstanceByProcessDefinitionIdUsingBuilder()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());

		//suspend
		runtimeService.updateProcessInstanceSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();

		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		//activate
		runtimeService.updateProcessInstanceSuspensionState().byProcessDefinitionId(processDefinition.Id).activate();

		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendAndActivateProcessInstanceByProcessDefinitionKeyUsingBuilder()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());

		//suspend
		runtimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey("oneTaskProcess").suspend();

		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		//activate
		runtimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey("oneTaskProcess").activate();

		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJobSuspensionStateUpdate()
	  public virtual void testJobSuspensionStateUpdate()
	  {

		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");
		string id = instance.ProcessInstanceId;

		//when
		runtimeService.suspendProcessInstanceById(id);
		Job job = managementService.createJobQuery().processInstanceId(id).singleResult();

		// then
		assertTrue(job.Suspended);
	  }

	}

}