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

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceStageTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testStartAutomated()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists two new case execution:
		verifyTasksState(caseExecutionQuery);


	  }

	  protected internal virtual void verifyTasksState(CaseExecutionQuery caseExecutionQuery)
	  {
		// (1) one case case execution representing "PI_HumanTask_1"
		CaseExecution firstHumanTask = caseExecutionQuery.activityId("PI_HumanTask_1").singleResult();

		assertNotNull(firstHumanTask);
		assertTrue(firstHumanTask.Active);
		assertFalse(firstHumanTask.Enabled);

		// (2) one case case execution representing "PI_HumanTask_2"
		CaseExecution secondHumanTask = caseExecutionQuery.activityId("PI_HumanTask_2").singleResult();

		assertNotNull(secondHumanTask);
		assertTrue(secondHumanTask.Active);
		assertFalse(secondHumanTask.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
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
		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// there exists two new case execution:

		// (1) one case case execution representing "PI_HumanTask_1"
		verifyTasksState(caseExecutionQuery);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
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
		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).setVariable("aVariableName", "abc").setVariable("anotherVariableName", 999).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// (1) one case case execution representing "PI_HumanTask_1"
		verifyTasksState(caseExecutionQuery);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		verifyVariables(caseInstanceId, caseInstanceId, result);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
	  public virtual void testManualWithVariables()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		// an enabled child case execution of
		// the case instance
		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

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

		// (1) one case case execution representing "PI_HumanTask_1"
		verifyTasksState(caseExecutionQuery);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		verifyVariables(caseInstanceId, caseInstanceId, result);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
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
		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

		// when
		// activate child case execution
		caseService.withCaseExecution(caseExecutionId).setVariableLocal("aVariableName", "abc").setVariableLocal("anotherVariableName", 999).manualStart();

		// then

		// the child case execution is active...
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		assertTrue(caseExecution.Active);
		// ... and not enabled
		assertFalse(caseExecution.Enabled);

		// (1) one case case execution representing "PI_HumanTask_1"
		verifyTasksState(caseExecutionQuery);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		verifyVariables(caseInstanceId, caseExecutionId, result);

	  }

	  protected internal virtual void verifyVariables(string caseInstanceId, string caseExecutionId, IList<VariableInstance> result)
	  {
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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
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
		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

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

		// (1) one case case execution representing "PI_HumanTask_1"
		verifyTasksState(caseExecutionQuery);

		// the case instance has two variables:
		// - aVariableName
		// - anotherVariableName
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();

		verifyVariables(caseInstanceId, caseExecutionId, result);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testReenableAnEnabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).reenable();
		  fail("It should not be possible to re-enable an enabled stage.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"})]
	  public virtual void testReenableAnDisabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testReenableAnActiveStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"})]
	  public virtual void testDisableAnEnabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance and the containing
		// human task is enabled
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

		// when
		caseService.withCaseExecution(caseExecutionId).disable();

		// then
		CaseExecution caseExecution = caseExecutionQuery.singleResult();
		// the human task is disabled
		assertTrue(caseExecution.Disabled);
		assertFalse(caseExecution.Active);
		assertFalse(caseExecution.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"})]
	  public virtual void testDisableADisabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecutionQuery caseExecutionQuery = caseService.createCaseExecutionQuery();

		string caseExecutionId = caseExecutionQuery.activityId("PI_Stage_1").singleResult().Id;

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testDisableAnActiveStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"})]
	  public virtual void testManualStartOfADisabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testManualStartOfAnActiveStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
	  public virtual void testDisableShouldCompleteCaseInstance()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		// when

		caseService.withCaseExecution(caseExecutionId).disable();

		// then

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();

		assertNull(caseExecution);

		// the case instance has been completed
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testCompleteShouldCompleteCaseInstance()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		// when

		caseService.withCaseExecution(queryCaseExecutionByActivityId("PI_HumanTask_1").Id).complete();
		caseService.withCaseExecution(queryCaseExecutionByActivityId("PI_HumanTask_2").Id).complete();

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

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageCase.cmmn"})]
	  public virtual void testComplete()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		// when
		caseService.withCaseExecution(queryCaseExecutionByActivityId("PI_HumanTask_11").Id).complete();

		caseService.withCaseExecution(queryCaseExecutionByActivityId("PI_HumanTask_2").Id).complete();

		// then

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();

		assertNull(caseExecution);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().active().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testCompleteEnabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).complete();
		  fail("Should not be able to complete stage.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskAndOneStageWithManualActivationCase.cmmn"})]
	  public virtual void testCompleteDisabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).disable();

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).complete();
		  fail("Should not be able to complete stage.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageCase.cmmn"})]
	  public virtual void testAutoCompletionOfEmptyStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create();

		// then

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();

		assertNull(caseExecution);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testClose()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseExecutionId).close();
		  fail("It should not be possible to close a stage.");
		}
		catch (NotAllowedException)
		{

		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testTerminate()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecution stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");

		// when
		CaseExecution humanTaskExecution1 = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(humanTaskExecution1.Active);

		CaseExecution humanTaskExecution2 = queryCaseExecutionByActivityId("PI_HumanTask_2");
		assertTrue(humanTaskExecution2.Active);

		caseService.withCaseExecution(stageExecution.Id).terminate();

		// then
		stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");
		assertNull(stageExecution);

		humanTaskExecution1 = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertNull(humanTaskExecution1);

		humanTaskExecution2 = queryCaseExecutionByActivityId("PI_HumanTask_2");
		assertNull(humanTaskExecution2);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testTerminateNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecution stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");

		// when
		caseService.terminateCaseExecution(stageExecution.Id);

		// then
		stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");
		assertNull(stageExecution);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCaseWithManualActivation.cmmn"})]
	  public virtual void testTerminateWithNonActiveState()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		caseService.withCaseDefinition(caseDefinitionId).create().Id;

		CaseExecution stageExecution = queryCaseExecutionByActivityId("PI_Stage_1");

		// when
		try
		{
		  // when
		  caseService.terminateCaseExecution(stageExecution.Id);
		  fail("It should not be possible to terminate a task.");
		}
		catch (NotAllowedException e)
		{
		  bool result = e.Message.contains("The case execution must be in state 'active' to terminate");
		  assertTrue(result);
		}

	  }

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }
	}

}