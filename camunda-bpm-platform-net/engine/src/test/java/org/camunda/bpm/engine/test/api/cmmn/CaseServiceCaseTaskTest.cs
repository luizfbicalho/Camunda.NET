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
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceCaseTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal readonly string DEFINITION_KEY = "oneCaseTaskCase";
	  protected internal readonly string DEFINITION_KEY_2 = "oneTaskCase";
	  protected internal readonly string CASE_TASK_KEY = "PI_CaseTask_1";


	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testManualStart()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		// when
		caseService.withCaseExecution(caseTaskId).manualStart();

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testManualStartWithVariable()
	  {
		// given
		string superCaseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		// when
		caseService.withCaseExecution(caseTaskId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).manualStart();

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		verifyVariables(superCaseInstanceId, result);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testManualStartWithVariables()
	  {
		// given
		string superCaseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		caseService.withCaseExecution(caseTaskId).setVariables(variables).manualStart();

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		verifyVariables(superCaseInstanceId, result);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testStart()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance;

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testStartWithVariable()
	  {
		// given
		string superCaseInstanceId = createCaseInstance(DEFINITION_KEY, Variables.createVariables().putValue("aVariableName", "abc").putValue("anotherVariableName", 999)).Id;

		CaseInstance subCaseInstance;

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		verifyVariables(superCaseInstanceId, result);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testStartWithVariables()
	  {
		// given
		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		string superCaseInstanceId = createCaseInstance(DEFINITION_KEY, variables).Id;

		CaseInstance subCaseInstance;

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		verifyVariables(superCaseInstanceId, result);

	  }

	  protected internal virtual void verifyVariables(string superCaseInstanceId, IList<VariableInstance> result)
	  {
		foreach (VariableInstance variable in result)
		{

		  assertEquals(superCaseInstanceId, variable.CaseExecutionId);
		  assertEquals(superCaseInstanceId, variable.CaseInstanceId);

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

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testManualStartWithLocalVariable()
	  {
		// given
		string superCaseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		// when
		caseService.withCaseExecution(caseTaskId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).manualStart();

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseTaskId, variable.CaseExecutionId);
		  assertEquals(superCaseInstanceId, variable.CaseInstanceId);

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

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testManualStartWithLocalVariables()
	  {
		// given
		string superCaseInstanceId = createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		// variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariableName"] = "abc";
		variables["anotherVariableName"] = 999;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseTaskId).setVariablesLocal(variables).manualStart();

		// then
		subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNotNull(subCaseInstance);
		assertTrue(subCaseInstance.Active);

		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Active);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variable in result)
		{

		  assertEquals(caseTaskId, variable.CaseExecutionId);
		  assertEquals(superCaseInstanceId, variable.CaseInstanceId);

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

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn" })]
	  public virtual void testReenableAnEnabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).reenable();
		  fail("It should not be possible to re-enable an enabled case task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskAndOneHumanTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testReenableADisabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		caseService.withCaseExecution(caseTaskId).disable();

		// when
		caseService.withCaseExecution(caseTaskId).reenable();

		// then
		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Enabled);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testReenableAnActiveCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).reenable();
		  fail("It should not be possible to re-enable an active case task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskAndOneHumanTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDisableAnEnabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertNull(subCaseInstance);

		// when
		caseService.withCaseExecution(caseTaskId).disable();

		// then
		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertTrue(caseTask.Disabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskAndOneHumanTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDisableADisabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		caseService.withCaseExecution(caseTaskId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).disable();
		  fail("It should not be possible to disable a already disabled case task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testDisableAnActiveCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		// when
		try
		{
		  caseService.withCaseExecution(caseTaskId).disable();
		  fail("It should not be possible to disable an active case task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskAndOneHumanTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testManualStartOfADisabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		caseService.withCaseExecution(caseTaskId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).manualStart();
		  fail("It should not be possible to start a disabled case task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testManualStartOfAnActiveCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).manualStart();
		  fail("It should not be possible to start an already active case task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testComplete()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).complete();
		  fail("It should not be possible to complete a case task, while the process instance is still running.");
		}
		catch (NotAllowedException)
		{
		}

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCloseCaseInstanceShouldCompleteCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		string humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		caseService.withCaseExecution(humanTaskId).complete();

		CaseInstance subCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY_2);
		assertTrue(subCaseInstance.Completed);

		// when
		caseService.withCaseExecution(subCaseInstance.Id).close();

		// then
		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertNull(caseTask);

		CaseInstance superCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY);
		assertNotNull(superCaseInstance);
		assertTrue(superCaseInstance.Completed);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testDisableShouldCompleteCaseInstance()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		// when

		caseService.withCaseExecution(caseTaskId).disable();

		// then
		CaseExecution caseTask = queryCaseExecutionByActivityId(CASE_TASK_KEY);

		// the case instance has been completed
		CaseInstance superCaseInstance = queryCaseInstanceByKey(DEFINITION_KEY);
		assertNotNull(superCaseInstance);
		assertTrue(superCaseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCompleteAnEnabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).complete();
		  fail("Should not be able to complete an enabled case task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskAndOneHumanTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCompleteADisabledCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		caseService.withCaseExecution(caseTaskId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).complete();
		  fail("Should not be able to complete a disabled case task.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testClose()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		string caseTaskId = queryCaseExecutionByActivityId(CASE_TASK_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).close();
		  fail("It should not be possible to close a case task.");
		}
		catch (NotAllowedException)
		{

		}

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testTerminate()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		CaseExecution caseTaskExecution = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		// when
		caseService.withCaseExecution(caseTaskExecution.Id).terminate();

		caseTaskExecution = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertNull(caseTaskExecution);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testTerminateNonFluent()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		CaseExecution caseTaskExecution = queryCaseExecutionByActivityId(CASE_TASK_KEY);

		// when
		caseService.terminateCaseExecution(caseTaskExecution.Id);

		caseTaskExecution = queryCaseExecutionByActivityId(CASE_TASK_KEY);
		assertNull(caseTaskExecution);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCaseWithManualActivation.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testTerminateNonActiveCaseTask()
	  {
		// given
		createCaseInstance(DEFINITION_KEY);
		CaseExecution caseTaskExecution = queryCaseExecutionByActivityId(CASE_TASK_KEY);

		try
		{
		  // when
		  caseService.terminateCaseExecution(caseTaskExecution.Id);
		  fail("It should not be possible to terminate a case task.");
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

	  protected internal virtual CaseInstance createCaseInstance(string caseDefinitionKey, IDictionary<string, object> variables)
	  {
		return caseService.withCaseDefinitionByKey(caseDefinitionKey).setVariables(variables).create();
	  }

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }

	  protected internal virtual CaseInstance queryCaseInstanceByKey(string caseDefinitionKey)
	  {
		return caseService.createCaseInstanceQuery().caseDefinitionKey(caseDefinitionKey).singleResult();
	  }

	  protected internal virtual Task queryTask()
	  {
		return taskService.createTaskQuery().singleResult();
	  }

	}

}