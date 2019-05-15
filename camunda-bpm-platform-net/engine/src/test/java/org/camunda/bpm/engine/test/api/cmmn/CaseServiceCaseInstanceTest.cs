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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceCaseInstanceTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKey()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("oneTaskCase").create();

		// then
		assertNotNull(caseInstance);

		// check properties
		assertNull(caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persisted case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);
	  }

	  public virtual void testCreateByInvalidKey()
	  {
		try
		{
		  caseService.withCaseDefinitionByKey("invalid").create();
		  fail();
		}
		catch (NotFoundException)
		{
		}

		try
		{
		  caseService.withCaseDefinitionByKey(null).create();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateById()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinition(caseDefinitionId).create();

		// then
		assertNotNull(caseInstance);

		// check properties
		assertNull(caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persistent case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);

	  }

	  public virtual void testCreateByInvalidId()
	  {
		try
		{
		  caseService.withCaseDefinition("invalid").create();
		  fail();
		}
		catch (NotFoundException)
		{
		}

		try
		{
		  caseService.withCaseDefinition(null).create();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyWithBusinessKey()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("oneTaskCase").businessKey("aBusinessKey").create();

		// then
		assertNotNull(caseInstance);

		// check properties
		assertEquals("aBusinessKey", caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persistend case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdWithBusinessKey()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinition(caseDefinitionId).businessKey("aBusinessKey").create();

		// then
		assertNotNull(caseInstance);

		// check properties
		assertEquals("aBusinessKey", caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persistend case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyWithVariable()
	  {
		// given a deployed case definition

		// when
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("aVariableName", "aVariableValue").setVariable("anotherVariableName", 999).create();

		// then
		assertNotNull(caseInstance);

		// there should exist two variables
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		IList<VariableInstance> result = query.caseInstanceIdIn(caseInstance.Id).orderByVariableName().asc().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("aVariableValue", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals(999, variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyWithVariables()
	  {
		// given a deployed case definition
		IDictionary<string, object> variables = new Dictionary<string, object>();

		variables["aVariableName"] = "aVariableValue";
		variables["anotherVariableName"] = 999;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("oneTaskCase").setVariables(variables).create();

		// then
		assertNotNull(caseInstance);

		// there should exist two variables
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		IList<VariableInstance> result = query.caseInstanceIdIn(caseInstance.Id).orderByVariableName().asc().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("aVariableValue", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals(999, variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdWithVariable()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinition(caseDefinitionId).setVariable("aVariableName", "aVariableValue").setVariable("anotherVariableName", 999).create();

		// then
		assertNotNull(caseInstance);

		// there should exist two variables
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		IList<VariableInstance> result = query.caseInstanceIdIn(caseInstance.Id).orderByVariableName().asc().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("aVariableValue", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals(999, variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdWithVariables()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		IDictionary<string, object> variables = new Dictionary<string, object>();

		variables["aVariableName"] = "aVariableValue";
		variables["anotherVariableName"] = 999;

		// when
		CaseInstance caseInstance = caseService.withCaseDefinition(caseDefinitionId).setVariables(variables).create();

		// then
		assertNotNull(caseInstance);

		// there should exist two variables
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		IList<VariableInstance> result = query.caseInstanceIdIn(caseInstance.Id).orderByVariableName().asc().list();

		assertFalse(result.Count == 0);
		assertEquals(2, result.Count);

		foreach (VariableInstance variableInstance in result)
		{
		  if (variableInstance.Name.Equals("aVariableName"))
		  {
			assertEquals("aVariableName", variableInstance.Name);
			assertEquals("aVariableValue", variableInstance.Value);
		  }
		  else if (variableInstance.Name.Equals("anotherVariableName"))
		  {
			assertEquals("anotherVariableName", variableInstance.Name);
			assertEquals(999, variableInstance.Value);
		  }
		  else
		  {
			fail("Unexpected variable: " + variableInstance.Name);
		  }

		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testManualStart()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// when
		try
		{
		  caseService.withCaseExecution(caseInstanceId).manualStart();
		  fail("It should not be possible to start a case instance manually.");
		}
		catch (NotAllowedException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDisable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// when
		try
		{
		  caseService.withCaseExecution(caseInstanceId).disable();
		  fail("It should not be possible to disable a case instance.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testReenable()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// when
		try
		{
		  caseService.withCaseExecution(caseInstanceId).reenable();
		  fail("It should not be possible to re-enable a case instance.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCompleteWithEnabledTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// when

		caseService.withCaseExecution(caseInstanceId).complete();

		// then

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testCompleteWithEnabledStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();


		CaseExecution caseExecution2 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2").singleResult();

		// when

		caseService.withCaseExecution(caseExecution.Id).complete();

		caseService.withCaseExecution(caseExecution2.Id).complete();

		// then

		// the corresponding case execution has been also
		// deleted and completed
		CaseExecution caseExecution3 = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();

		assertNull(caseExecution3);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCompleteWithActiveTask()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// when

		try
		{
		  caseService.withCaseExecution(caseInstanceId).complete();
		  fail("It should not be possible to complete a case instance containing an active task.");
		}
		catch (ProcessEngineException)
		{
		}

		// then

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNotNull(caseExecution);
		assertTrue(caseExecution.Active);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneStageCase.cmmn"})]
	  public virtual void testCompleteWithActiveStage()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		// when

		try
		{
		  caseService.withCaseExecution(caseInstanceId).complete();
		  fail("It should not be possible to complete a case instance containing an active stage.");
		}
		catch (ProcessEngineException)
		{
		}

		// then

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();

		assertNotNull(caseExecution);
		assertTrue(caseExecution.Active);

		// the case instance is still active
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyCasePlanModelCase.cmmn"})]
	  public virtual void testAutoCompletionOfEmptyCase()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		caseService.withCaseDefinition(caseDefinitionId).create();

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().completed().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCloseAnActiveCaseInstance()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseInstanceId).close();
		  fail("It should not be possible to close an active case instance.");
		}
		catch (ProcessEngineException)
		{
		}

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCloseACompletedCaseInstance()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// disable human task -> case instance is completed
		caseService.withCaseExecution(caseExecutionId).disable();

		// when
		caseService.withCaseExecution(caseInstanceId).close();

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNull(caseInstance);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testTerminateActiveCaseInstance()
	  {
		// given:
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		assertNotNull(queryCaseExecutionByActivityId("CasePlanModel_1"));

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(taskExecution.Active);

		caseService.withCaseExecution(caseInstanceId).terminate();

		CaseExecution caseInstance = queryCaseExecutionByActivityId("CasePlanModel_1");
		assertTrue(caseInstance.Terminated);

		assertNull(queryCaseExecutionByActivityId("PI_HumanTask_1"));
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testTerminateNonActiveCaseInstance()
	  {
		// given:
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		assertNotNull(queryCaseExecutionByActivityId("CasePlanModel_1"));

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(taskExecution.Enabled);

		caseService.completeCaseExecution(caseInstanceId);

		try
		{
		  // when
		  caseService.terminateCaseExecution(caseInstanceId);
		  fail("It should not be possible to terminate a task.");
		}
		catch (NotAllowedException e)
		{
		  bool result = e.Message.contains("The case execution must be in state 'active' to terminate");
		  assertTrue(result);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testTerminateActiveCaseInstanceNonFluent()
	  {
		// given:
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		assertNotNull(queryCaseExecutionByActivityId("CasePlanModel_1"));

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(taskExecution.Active);

		caseService.terminateCaseExecution(caseInstanceId);

		CaseExecution caseInstance = queryCaseExecutionByActivityId("CasePlanModel_1");
		assertTrue(caseInstance.Terminated);

		assertNull(queryCaseExecutionByActivityId("PI_HumanTask_1"));
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyNonFluent()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");

		// then
		assertNotNull(caseInstance);

		// check properties
		assertNull(caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persisted case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdNonFluent()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.createCaseInstanceById(caseDefinitionId);

		// then
		assertNotNull(caseInstance);

		// check properties
		assertNull(caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persisted case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyWithBusinessKeyNonFluent()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", "aBusinessKey");

		// then
		assertNotNull(caseInstance);

		// check properties
		assertEquals("aBusinessKey", caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persisted case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdWithBusinessKeyNonFluent()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		CaseInstance caseInstance = caseService.createCaseInstanceById(caseDefinitionId, "aBusinessKey");

		// then
		assertNotNull(caseInstance);

		// check properties
		assertEquals("aBusinessKey", caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

		// get persisted case instance
		CaseInstance instance = caseService.createCaseInstanceQuery().singleResult();

		// should have the same properties
		assertEquals(caseInstance.Id, instance.Id);
		assertEquals(caseInstance.BusinessKey, instance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, instance.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, instance.CaseInstanceId);
		assertEquals(caseInstance.Active, instance.Active);
		assertEquals(caseInstance.Enabled, instance.Enabled);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyWithVariablesNonFluent()
	  {
		// given a deployed case definition

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", variables);

		// then
		assertNotNull(caseInstance);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstance.Id).singleResult();

		assertNotNull(variable);
		assertEquals("aVariable", variable.Name);
		assertEquals("aValue", variable.Value);
		assertEquals(caseInstance.Id, variable.CaseInstanceId);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdWithVariablesNonFluent()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		CaseInstance caseInstance = caseService.createCaseInstanceById(caseDefinitionId, variables);

		// then
		assertNotNull(caseInstance);

		VariableInstance variable = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstance.Id).singleResult();

		assertNotNull(variable);
		assertEquals("aVariable", variable.Name);
		assertEquals("aValue", variable.Value);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByKeyWithVariablesAndBusinessKeyNonFluent()
	  {
		// given a deployed case definition

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", "aBusinessKey", variables);

		// then
		assertNotNull(caseInstance);

		assertEquals("aBusinessKey", caseInstance.BusinessKey);
		assertEquals(1, runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstance.Id).variableValueEquals("aVariable", "aValue").count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCreateByIdWithVariablesAndBusinessKeyNonFluent()
	  {
		// given a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["aVariable"] = "aValue";
		CaseInstance caseInstance = caseService.createCaseInstanceById(caseDefinitionId, "aBusinessKey", variables);

		// then
		assertNotNull(caseInstance);

		assertEquals("aBusinessKey", caseInstance.BusinessKey);
		assertEquals(1, runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstance.Id).variableValueEquals("aVariable", "aValue").count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testCloseNonFluent()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// disable human task -> case instance is completed
		caseService.withCaseExecution(caseExecutionId).disable();

		// when
		caseService.closeCaseInstance(caseInstanceId);

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNull(caseInstance);
	  }

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }
	}

}