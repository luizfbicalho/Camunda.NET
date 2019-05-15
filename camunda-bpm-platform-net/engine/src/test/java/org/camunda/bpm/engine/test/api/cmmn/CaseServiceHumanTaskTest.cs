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
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceHumanTaskTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testManualStart()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testManualStartWithVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testManualStartWithVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).setVariables(variables).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testStart()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// activate child case execution

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testStartWithVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		VariableMap variableMap = Variables.createVariables().putValue("aVariableName", "abc").putValue("anotherVariableName", 999);
		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariables(variableMap).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// activate child case execution

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testStartWithVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariables(variables).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testManualStartWithLocalVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testManualStartWithLocalVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).setVariablesLocal(variables).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseExecutionId, variable.CaseExecutionId);
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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testReenableAnEnabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance

		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).reenable();
		  fail("It should not be possible to re-enable an enabled human task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testReenableAnDisabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// the human task is disabled
		caseService.withCaseExecution(caseExecutionId).disable();

		// when
		caseService.withCaseExecution(caseExecutionId).reenable();

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertFalse(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertTrue(caseExecution.Enabled);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testReenableAnActiveHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		try
		{
		  caseService.withCaseExecution(caseExecutionId).reenable();
		  fail("It should not be possible to re-enable an active human task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testDisableAnEnabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance and the containing
		// human task is enabled
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(caseExecutionId).disable();

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertTrue(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertFalse(caseExecution.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testDisableADisabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// the human task is disabled
		caseService.withCaseExecution(caseExecutionId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).disable();
		  fail("It should not be possible to disable a already disabled human task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDisableAnActiveHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		try
		{
		  caseService.withCaseExecution(caseExecutionId).disable();
		  fail("It should not be possible to disable an active human task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testManualStartOfADisabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).manualStart();
		  fail("It should not be possible to start a disabled human task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testManualStartOfAnActiveHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).manualStart();
		  fail("It should not be possible to start an already active human task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testComplete()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.withCaseExecution(caseExecutionId).complete();

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCompleteShouldCompleteCaseInstance()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.withCaseExecution(caseExecutionId).complete();

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance has been completed
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCompleteShouldCompleteCaseInstanceViaTaskService()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		taskService.complete(task.Id);

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance has been completed
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDisableShouldCompleteCaseInstance()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		caseService.withCaseExecution(caseExecutionId).disable();

		// then

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance has been completed
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCompleteAnEnabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).complete();
		  fail("Should not be able to complete task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteADisabledHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).complete();
		  fail("Should not be able to complete task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteWithSetVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).complete();

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteWithSetVariableLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).complete();

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		// the variables has been set and due to the completion
		// also removed in one command
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteWithRemoveVariable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.withCaseExecution(caseExecutionId).removeVariable("aVariableName").removeVariable("anotherVariableName").complete();

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteWithRemoveVariableLocal()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.withCaseExecution(caseExecutionId).removeVariableLocal("aVariableName").removeVariableLocal("anotherVariableName").complete();

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertTrue(result.Count == 0);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testClose()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).close();
		  fail("It should not be possible to close a task.");
		}
		catch (NotAllowedException)
		{

		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testTerminate()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// when
		caseService.withCaseExecution(taskExecution.Id).terminate();

		// then
		taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertNull(taskExecution);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testTerminateNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// when
		caseService.terminateCaseExecution(taskExecution.Id);

		// then
		taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertNull(taskExecution);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testTerminateNonActiveHumanTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		try
		{
		  // when
		  caseService.terminateCaseExecution(taskExecution.Id);
		  fail("It should not be possible to terminate a task.");
		}
		catch (NotAllowedException e)
		{
		  bool result = e.Message.contains("The case execution must be in state 'active' to terminate");
		  assertTrue(result);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testManualStartNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);
	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testManualStartWithVariablesNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// activate child case execution
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		caseService.manuallyStartCaseExecution(caseExecutionId, variables);

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists a task
		Task task = taskService.createTaskQuery().caseExecutionId(caseExecutionId).singleResult();

		assertNotNull(task);

		// there is a variable set on the case instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertNotNull(variable);
		assertEquals(caseInstanceId, variable.CaseExecutionId);
		assertEquals(caseInstanceId, variable.CaseInstanceId);
		assertEquals("aVariable", variable.Name);
		assertEquals("aValue", variable.Value);

	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testDisableNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance and the containing
		// human task is enabled
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.disableCaseExecution(caseExecutionId);

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertTrue(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertFalse(caseExecution.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testDisableNonFluentWithVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance and the containing
		// human task is enabled
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		caseService.disableCaseExecution(caseExecutionId, variables);

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertTrue(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertFalse(caseExecution.Enabled);

		// there is a variable set on the case instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertNotNull(variable);
		assertEquals(caseInstanceId, variable.CaseExecutionId);
		assertEquals(caseInstanceId, variable.CaseInstanceId);
		assertEquals("aVariable", variable.Name);
		assertEquals("aValue", variable.Value);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testReenableNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// the human task is disabled
		caseService.withCaseExecution(caseExecutionId).disable();

		// when
		caseService.reenableCaseExecution(caseExecutionId);

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertFalse(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertTrue(caseExecution.Enabled);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testReenableNonFluentWithVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// the human task is disabled
		caseService.withCaseExecution(caseExecutionId).disable();

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		caseService.reenableCaseExecution(caseExecutionId, variables);

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertFalse(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertTrue(caseExecution.Enabled);

		// there is a variable set on the case instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertNotNull(variable);
		assertEquals(caseInstanceId, variable.CaseExecutionId);
		assertEquals(caseInstanceId, variable.CaseInstanceId);
		assertEquals("aVariable", variable.Name);
		assertEquals("aValue", variable.Value);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when

		caseService.completeCaseExecution(caseExecutionId);

		// then

		// the task has been completed and has been deleted
		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn"})]
	  public virtual void testCompleteWithVariablesNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).manualStart();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";

		caseService.completeCaseExecution(caseExecutionId, variables);

		// then

		// the task has been completed and has been deleted
		assertNull(taskService.createTaskQuery().singleResult());

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		// there is a variable set on the case instance
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertNotNull(variable);
		assertEquals(caseInstanceId, variable.CaseExecutionId);
		assertEquals(caseInstanceId, variable.CaseInstanceId);
		assertEquals("aVariable", variable.Name);
		assertEquals("aValue", variable.Value);

	  }

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }
	}

}