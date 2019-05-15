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
namespace org.camunda.bpm.engine.test.api.cmmn
{

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceProcessTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal readonly string DEFINITION_KEY = "oneProcessTaskCase";
	  protected internal readonly string PROCESS_TASK_KEY = "PI_ProcessTask_1";

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testStart()
	  {
		// given
		string caseInstanceId = createCaseInstance(DEFINITION_KEY).Id;

		ProcessInstance processInstance;

		// then
		processInstance = queryProcessInstance();

		assertNotNull(processInstance);
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Active);
	  }

	  protected internal virtual void verifyVariables(string caseInstanceId, IList<VariableInstance> result)
	  {
		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseInstanceId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testManualStart()
	  {
		// given
		string caseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		// when
		caseService.withCaseExecution(processTaskId).manualStart();

		// then
		processInstance = queryProcessInstance();

		assertNotNull(processInstance);
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Active);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testManualStartWithVariables()
	  {
		// given
		string caseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(processTaskId).setVariables(variables).manualStart();

		// then
		processInstance = queryProcessInstance();

		assertNotNull(processInstance);
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		verifyVariables(caseInstanceId, result);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testManualStartWithLocalVariable()
	  {
		// given
		string caseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		// when
		caseService.withCaseExecution(processTaskId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).manualStart();

		// then
		processInstance = queryProcessInstance();

		assertNotNull(processInstance);
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);

		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(processTaskId, variable.CaseExecutionId);
		  assertEquals(caseInstanceId, variable.CaseInstanceId);

		  if (variable.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variable.Name);
			assertEquals("abc", variable.Value);
		  }
		  else if (variable.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variable.Name);
			assertEquals(999, variable.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variable.Name);
		  }
		}

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testReenableAnEnabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).reenable();
		  fail("It should not be possible to re-enable an enabled process task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskWithManualActivationAndOneHumanTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testReenableADisabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		caseService.withCaseExecution(processTaskId).disable();

		// when
		caseService.withCaseExecution(processTaskId).reenable();

		// then
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Enabled);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testReenableAnActiveProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).reenable();
		  fail("It should not be possible to re-enable an active process task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskWithManualActivationAndOneHumanTaskCase.cmmn"})]
	  public virtual void testDisableAnEnabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		ProcessInstance processInstance = queryProcessInstance();
		assertNull(processInstance);

		// when
		caseService.withCaseExecution(processTaskId).disable();

		// then
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Disabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskWithManualActivationAndOneHumanTaskCase.cmmn"})]
	  public virtual void testDisableADisabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		caseService.withCaseExecution(processTaskId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).disable();
		  fail("It should not be possible to disable a already disabled process task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDisableAnActiveProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		// when
		try
		{
		  caseService.withCaseExecution(processTaskId).disable();
		  fail("It should not be possible to disable an active process task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskWithManualActivationAndOneHumanTaskCase.cmmn"})]
	  public virtual void testManualStartOfADisabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		caseService.withCaseExecution(processTaskId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).manualStart();
		  fail("It should not be possible to start a disabled process task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testManualStartOfAnActiveProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).manualStart();
		  fail("It should not be possible to start an already active process task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testComplete()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).complete();
		  fail("It should not be possible to complete a process task, while the process instance is still running.");
		}
		catch (NotAllowedException)
		{
		}

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCompleteProcessInstanceShouldCompleteProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		string taskId = queryTask().Id;

		// when
		taskService.complete(taskId);

		// then
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertNull(processTask);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDisableShouldCompleteCaseInstance()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		// when

		caseService.withCaseExecution(processTaskId).disable();

		// then
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertNull(processTask);

		// the case instance has been completed
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCompleteAnEnabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).complete();
		  fail("Should not be able to complete an enabled process task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskWithManualActivationAndOneHumanTaskCase.cmmn"})]
	  public virtual void testCompleteADisabledProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		caseService.withCaseExecution(processTaskId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).complete();
		  fail("Should not be able to complete a disabled process task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testClose()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string processTaskId = queryCaseExecutionByActivityId(PROCESS_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(processTaskId).close();
		  fail("It should not be possible to close a process task.");
		}
		catch (NotAllowedException)
		{

		}

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTerminate()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Active);
		// when
		caseService.withCaseExecution(processTask.Id).terminate();

		processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertNull(processTask);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTerminateNonFluent()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Active);

		// when
		caseService.terminateCaseExecution(processTask.Id);

		processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertNull(processTask);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testTerminateNonActiveProcessTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		CaseExecution processTask = queryCaseExecutionByActivityId(PROCESS_TASK_KEY);
		assertTrue(processTask.Enabled);

		try
		{
		  // when
		  caseService.terminateCaseExecution(processTask.Id);
		  fail("It should not be possible to terminate a task.");
		}
		catch (NotAllowedException e)
		{
		  bool result = e.Message.contains("The case execution must be in state 'active' to terminate");
		  assertTrue(result);
		}
	  }

	  protected internal virtual CaseInstance createCaseInstance(string caseDefinitionKey)
	  {
		return caseService.withCaseDefinitionByKey(caseDefinitionKey).create();
	  }

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }

	  protected internal virtual ProcessInstance queryProcessInstance()
	  {
		return runtimeService.createProcessInstanceQuery().singleResult();
	  }

	  protected internal virtual Task queryTask()
	  {
		return taskService.createTaskQuery().singleResult();
	  }

	}

}