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
namespace org.camunda.bpm.engine.test.cmmn.stage
{
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class AutoCompleteTest : CmmnProcessEngineTestCase
	{

	  protected internal const string CASE_DEFINITION_KEY = "case";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCasePlanModel()
	  public virtual void testCasePlanModel()
	  {
		// given
		// a deployed process

		// when
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		// humanTask1 and humanTask2 are not available
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();
		assertNull(query.activityId("PI_HumanTask_1").singleResult());
		assertNull(query.activityId("PI_HumanTask_2").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStage()
	  public virtual void testStage()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");

		string stageId = query.singleResult().Id;

		// when
		caseService.manuallyStartCaseExecution(stageId);

		// then

		// the instance is still active (contains
		// a further human task)
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		// humanTask1 is still available
		assertNotNull(query.activityId("PI_HumanTask_1").singleResult());

		// stage, humanTask2, humanTask3 are not available
		assertNull(query.activityId("PI_Stage_1").singleResult());
		assertNull(query.activityId("PI_HumanTask_2").singleResult());
		assertNull(query.activityId("PI_HumanTask_3").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testManualActivationDisabled()
	  public virtual void testManualActivationDisabled()
	  {
		// given
		// a deployed case definition

		// when (1)
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		// then (1)
		CaseInstanceQuery instanceQuery = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId);

		CaseInstance caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();

		string humanTask2Id = executionQuery.activityId("PI_HumanTask_2").singleResult().Id;

		// when (2)
		caseService.completeCaseExecution(humanTask2Id);

		// then (2)
		caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		// humanTask1 and humanTask2 are not available
		assertNull(executionQuery.activityId("PI_HumanTask_1").singleResult());
		assertNull(executionQuery.activityId("PI_HumanTask_2").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testManualActivationDisabledInsideStage()
	  public virtual void testManualActivationDisabledInsideStage()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();

		string stageId = executionQuery.activityId("PI_Stage_1").singleResult().Id;

		// then (1)
		CaseExecution stage = executionQuery.activityId("PI_Stage_1").singleResult();
		assertNotNull(stage);
		assertTrue(stage.Active);

		string humanTask2Id = executionQuery.activityId("PI_HumanTask_2").singleResult().Id;

		// when (2)
		complete(humanTask2Id);

		// then (2)
		// the instance is still active (contains
		// a further human task)
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		// humanTask1 is still available
		assertNotNull(executionQuery.activityId("PI_HumanTask_1").singleResult());

		// stage, humanTask2, humanTask3 are not available
		assertNull(executionQuery.activityId("PI_Stage_1").singleResult());
		assertNull(executionQuery.activityId("PI_HumanTask_2").singleResult());
		assertNull(executionQuery.activityId("PI_HumanTask_3").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNested()
	  public virtual void testNested()
	  {
		// given
		// a deployed case definition

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();

		// when
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		// stage, humanTask1, humanTask2, humanTask3 are not available
		assertNull(executionQuery.activityId("PI_Stage_1").singleResult());
		assertNull(executionQuery.activityId("PI_HumanTask_1").singleResult());
		assertNull(executionQuery.activityId("PI_HumanTask_2").singleResult());
		assertNull(executionQuery.activityId("PI_HumanTask_3").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRequiredEnabled()
	  public virtual void testRequiredEnabled()
	  {
		// given
		// a deployed case definition

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();
		CaseInstanceQuery instanceQuery = caseService.createCaseInstanceQuery();

		// when (1)
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		// then (1)
		CaseInstance caseInstance = instanceQuery.caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		string humanTask1Id = executionQuery.activityId("PI_HumanTask_1").singleResult().Id;
		manualStart(humanTask1Id);

		// when (2)
		complete(humanTask1Id);

		// then (2)
		caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		string humanTask2Id = executionQuery.activityId("PI_HumanTask_2").singleResult().Id;
		manualStart(humanTask2Id);

		// when (3)
		complete(humanTask2Id);

		// then (3)
		caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRequiredEnabledInsideStage()
	  public virtual void testRequiredEnabledInsideStage()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();

		string humanTask3Id = executionQuery.activityId("PI_HumanTask_3").singleResult().Id;

		// when (1)
		complete(humanTask3Id);

		// then (1)
		CaseExecution stage = executionQuery.activityId("PI_Stage_1").singleResult();
		assertNotNull(stage);
		assertTrue(stage.Active);

		string humanTask2Id = executionQuery.activityId("PI_HumanTask_2").singleResult().Id;

		// when (2)
		complete(humanTask2Id);

		// then (2)
		assertNull(executionQuery.activityId("PI_Stage_1").singleResult());

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEntryCriteriaAndManualActivationDisabled()
	  public virtual void testEntryCriteriaAndManualActivationDisabled()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();

		string humanTask1Id = executionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		// when (1)
		complete(humanTask1Id);

		// then (1)
		CaseInstanceQuery instanceQuery = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId);

		CaseInstance caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Active);

		string humanTask2Id = executionQuery.activityId("PI_HumanTask_2").singleResult().Id;

		// when (2)
		complete(humanTask2Id);

		// then (2)
		caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExitCriteriaAndRequiredEnabled()
	  public virtual void testExitCriteriaAndRequiredEnabled()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();

		string humanTask1Id = executionQuery.activityId("PI_HumanTask_1").singleResult().Id;

		CaseExecution humanTask2 = executionQuery.activityId("PI_HumanTask_2").singleResult();

		manualStart(humanTask2.Id);

		// when
		complete(humanTask1Id);

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/stage/AutoCompleteTest.testRequiredEnabled.cmmn"})]
	  public virtual void testTerminate()
	  {
		// given
		// a deployed case definition

		string caseInstanceId = createCaseInstanceByKey(CASE_DEFINITION_KEY).Id;

		CaseExecutionQuery executionQuery = caseService.createCaseExecutionQuery();
		CaseInstanceQuery instanceQuery = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId);

		string humanTask2Id = executionQuery.activityId("PI_HumanTask_2").singleResult().Id;
		manualStart(humanTask2Id);

		// when
		terminate(humanTask2Id);

		// then
		CaseInstance caseInstance = instanceQuery.singleResult();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/stage/AutoCompleteTest.testProcessTasksOnStage.cmmn", "org/camunda/bpm/engine/test/cmmn/stage/AutoCompleteTest.testProcessTasksOnStage.bpmn" }), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testProcessTasksOnStage()
	  {
		// given

		// when
		createCaseInstanceByKey(CASE_DEFINITION_KEY);

		IList<HistoricCaseActivityInstance> historicCaseActivityInstances = historyService.createHistoricCaseActivityInstanceQuery().caseActivityType("processTask").list();

		// then
		assertThat(historicCaseActivityInstances.Count, @is(2));
	  }

	}

}