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
namespace org.camunda.bpm.engine.test.cmmn.sentry
{
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class SentryVariableOnPartEntryCriteriaTest : CmmnProcessEngineTestCase
	{

	  // Basic tests - create, update, delete variable
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSimpleVariableOnPart.cmmn"})]
	  public virtual void testVariableCreate()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertFalse(firstHumanTask.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(firstHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSimpleVariableOnPart.cmmn"})]
	  public virtual void testUnknownVariableCreate()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.setVariable(caseInstanceId, "unknown", "aVariable");
		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertFalse(firstHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testVariableUpdate.cmmn"})]
	  public virtual void testVariableUpdate()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		// HumanTask not enabled on variable create
		assertFalse(firstHumanTask.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "bVariable");
		firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(firstHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testVariableDelete.cmmn"})]
	  public virtual void testVariableDelete()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.removeVariable(caseInstanceId, "variable_1");
		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		// removing unknown variable would not enable human task
		assertFalse(firstHumanTask.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertFalse(firstHumanTask.Enabled);

		caseService.removeVariable(caseInstanceId, "variable_1");
		firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(firstHumanTask.Enabled);
	  }

	  // different variable name and variable event test
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testDifferentVariableName.cmmn"})]
	  public virtual void testDifferentVariableName()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		CaseExecution firstHumanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		CaseExecution firstHumanTask2 = queryCaseExecutionByActivityId("HumanTask_2");

		assertFalse(firstHumanTask1.Enabled);
		assertFalse(firstHumanTask2.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		firstHumanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(firstHumanTask1.Enabled);

		firstHumanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		// variable_2 is not set 
		assertFalse(firstHumanTask2.Enabled);

		caseService.setVariable(caseInstanceId, "variable_2", "aVariable");
		firstHumanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(firstHumanTask2.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testDifferentVariableEvents.cmmn"})]
	  public virtual void testDifferentVariableEventsButSameName()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		CaseExecution firstHumanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		CaseExecution firstHumanTask2 = queryCaseExecutionByActivityId("HumanTask_2");

		assertFalse(firstHumanTask1.Enabled);
		assertFalse(firstHumanTask2.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		firstHumanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(firstHumanTask1.Enabled);

		firstHumanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		// variable_1 is not updated 
		assertFalse(firstHumanTask2.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "bVariable");
		firstHumanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(firstHumanTask2.Enabled);
	  }

	  // Multiple variableOnParts test
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testMoreVariableOnPart.cmmn"})]
	  public virtual void testMultipleVariableOnParts()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		// sentry would not be satisfied as the variable has to updated and deleted as well
		assertFalse(firstHumanTask.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "bVariable");
		firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		// sentry would not be satisfied as the variable has to deleted
		assertFalse(firstHumanTask.Enabled);

		caseService.removeVariable(caseInstanceId, "variable_1");
		firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(firstHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testMultipleSentryMultipleVariableOnPart.cmmn"})]
	  public virtual void testMultipleSentryMultipleVariableOnParts()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.setVariable(caseInstanceId, "value", 99);
		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("HumanTask_1");
		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		// Sentry1 would not be satisfied as the value has to be > 100
		// Sentry2 would not be satisfied as the humanTask 1 has to completed
		assertFalse(secondHumanTask.Enabled);

		manualStart(firstHumanTask.Id);
		complete(firstHumanTask.Id);

		secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		// Sentry1 would not be satisfied as the value has to be > 100
		// But, Sentry 2 would be satisfied and enables HumanTask2
		assertTrue(secondHumanTask.Enabled);

	  }

	  // IfPart, OnPart and VariableOnPart combination test
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testOnPartIfPartAndVariableOnPart.cmmn"})]
	  public virtual void testOnPartIfPartAndVariableOnPart()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		string firstHumanTaskId = queryCaseExecutionByActivityId("HumanTask_1").Id;

		complete(firstHumanTaskId);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		// Sentry would not be satisfied as variable_1 is not created and IfPart is not true
		assertFalse(secondHumanTask.Enabled);

		caseService.setVariable(caseInstanceId, "value", 101);
		secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		// Sentry would not be satisfied as variable_1 is not created
		assertFalse(secondHumanTask.Enabled);

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");
		secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(secondHumanTask.Enabled);

	  }


	  // Variable scope tests
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSimpleVariableScope.cmmn"})]
	  public virtual void testVariableCreateScope()
	  {

		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		string firstHumanTaskId = queryCaseExecutionByActivityId("HumanTask_1").Id;

		manualStart(firstHumanTaskId);

		caseService.setVariableLocal(firstHumanTaskId, "variable_1", "aVariable");

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		// Sentry would not be triggered as the scope of the sentry and humanTask1 is different
		assertFalse(secondHumanTask.Enabled);

		caseService.setVariableLocal(secondHumanTask.Id, "variable_1", "aVariable");
		secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		// Still Sentry would not be triggered as the scope of sentry and the humantask2 is different
		assertFalse(secondHumanTask.Enabled);

		caseService.setVariableLocal(caseInstanceId, "variable_1", "aVariable");
		secondHumanTask = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(secondHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testStageScope.cmmn"})]
	  public virtual void testStageScope()
	  {

		caseService.createCaseInstanceByKey("Case_1");

		CaseExecution caseModelHumanTask = queryCaseExecutionByActivityId("CaseModel_HumanTask");
		assertFalse(caseModelHumanTask.Enabled);

		string stageExecutionId = queryCaseExecutionByActivityId("Stage_1").Id;
		// set the variable in the scope of stage such that sentry in the scope of case model does not gets evaluated.
		caseService.setVariableLocal(stageExecutionId, "variable_1", "aVariable");

		CaseExecution stageHumanTask = queryCaseExecutionByActivityId("Stage_HumanTask");
		caseModelHumanTask = queryCaseExecutionByActivityId("CaseModel_HumanTask");
		assertFalse(caseModelHumanTask.Enabled);
		assertTrue(stageHumanTask.Enabled);

		caseService.removeVariable(stageExecutionId, "variable_1");
		// set the variable in the scope of case model that would trigger the sentry outside the scope of the stage
		caseService.setVariable(stageHumanTask.Id, "variable_1", "aVariable");

		stageHumanTask = queryCaseExecutionByActivityId("Stage_HumanTask");
		caseModelHumanTask = queryCaseExecutionByActivityId("CaseModel_HumanTask");
		assertTrue(caseModelHumanTask.Enabled);
		assertTrue(stageHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testStagesScope.cmmn"})]
	  public virtual void testStagesScope()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.setVariable(caseInstanceId, "variable_1", "aVariable");

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);

		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);

		CaseExecution humanTask3 = queryCaseExecutionByActivityId("HumanTask_3");
		assertTrue(humanTask3.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testStagesScope.cmmn"})]
	  public virtual void testStageLocalScope()
	  {
		caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecution1_Id = queryCaseExecutionByActivityId("Stage_1").Id;

		string stageExecution2_Id = queryCaseExecutionByActivityId("Stage_2").Id;

		// variable set to stage 1 scope, so that sentries in stage 2 and in case model should not be triggered
		caseService.setVariableLocal(stageExecution1_Id, "variable_1", "aVariable");

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);

		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertFalse(humanTask2.Enabled);

		CaseExecution humanTask3 = queryCaseExecutionByActivityId("HumanTask_3");
		assertFalse(humanTask3.Enabled);

		// variable set to stage 2 scope, so that sentries in the scope of case model should not be triggered
		caseService.setVariableLocal(stageExecution2_Id, "variable_1", "aVariable");
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);

		humanTask3 = queryCaseExecutionByActivityId("HumanTask_3");
		assertFalse(humanTask3.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testMultipleOnPartsInStage.cmmn"})]
	  public virtual void testMultipleOnPartsInStages()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		caseService.setVariable(caseInstanceId, "variable_1", 101);

		CaseExecution humanTask3 = queryCaseExecutionByActivityId("HumanTask_3");
		assertTrue(humanTask3.Enabled);

		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		// Not enabled as the sentry waits for human task 1 to complete
		assertFalse(humanTask2.Enabled);

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		manualStart(humanTask1.Id);
		complete(humanTask1.Id);

		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.sentryEvaluationBeforeCreation.cmmn"})]
	  public virtual void testShouldnotEvaluateSentryBeforeSentryCreation()
	  {
		caseService.createCaseInstanceByKey("Case_1").Id;

		CaseExecution stageExecution = queryCaseExecutionByActivityId("Stage_1");
		assertTrue(stageExecution.Enabled);

		CaseExecution humanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertNull(humanTask);

		// set the variable in the scope of stage - should not trigger sentry inside the stage as the sentry is not yet created.
		caseService.setVariableLocal(stageExecution.Id, "variable_1", "aVariable");

		manualStart(stageExecution.Id);

		humanTask = queryCaseExecutionByActivityId("HumanTask_1");
		// variable event occurred before sentry creation
		assertTrue(humanTask.Available);

		caseService.removeVariable(stageExecution.Id, "variable_1");
		// Sentry is active and would enable human task 1
		caseService.setVariableLocal(stageExecution.Id, "variable_1", "aVariable");
		humanTask = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask.Enabled);
	  }

	  // Evaluation of not affected sentries test
	  // i.e: Evaluation of a sentry's ifPart condition even if there are no evaluation of variableOnParts defined in the sentry
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSentryShouldNotBeEvaluatedAfterStageComplete.cmmn"})]
	  public virtual void testEvaluationOfNotAffectedSentries()
	  {
		caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecutionId = queryCaseExecutionByActivityId("Stage_1").Id;

		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Available);

		caseService.setVariableLocal(stageExecutionId, "value", 99);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		// if part is not satisfied
		assertFalse(humanTask2.Enabled);

		caseService.setVariableLocal(stageExecutionId, "value", 101);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testNotAffectedSentriesInMultipleStageScopes.cmmn"})]
	  public virtual void testNotAffectedSentriesInMultipleStageScopes()
	  {
		caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecution1_Id = queryCaseExecutionByActivityId("Stage_1").Id;

		caseService.setVariable(stageExecution1_Id, "value", 99);

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		// if part is not satisfied
		assertFalse(humanTask1.Enabled);

		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		// if part is not satisfied
		assertFalse(humanTask2.Enabled);

		// Evaluates the sentry's IfPart alone
		caseService.setVariable(stageExecution1_Id, "value", 101);
		humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);

		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSameVariableNameInDifferentScopes.cmmn"})]
	  public virtual void testSameVariableNameInDifferentScopes()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecution1_Id = queryCaseExecutionByActivityId("Stage_1").Id;

		// inner stage
		string stageExecution2_Id = queryCaseExecutionByActivityId("Stage_2").Id;

		// set the same variable 'value' in the scope of case model
		caseService.setVariable(caseInstanceId, "value", 102);

		// set the variable 'value' in the scope of stage 1
		caseService.setVariableLocal(stageExecution1_Id, "value", 99);

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Available);
		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Available);

		// update the variable 'value' in the case model scope
		caseService.setVariable(caseInstanceId, "value", 102);

		// then sentry of HumanTask 1 gets evaluated and sentry of HumanTask2 is not evaluated.
		humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertFalse(humanTask2.Enabled);

		// update the variable 'value' in the stage 2/stage 1 scope to evaluate the sentry inside stage 2
		caseService.setVariable(stageExecution2_Id, "value", 103);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSameVariableNameInDifferentScopes.cmmn"})]
	  public virtual void testNestedScopes()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecution1_Id = queryCaseExecutionByActivityId("Stage_1").Id;

		// set the variable 'value' in the scope of the case model
		caseService.setVariable(stageExecution1_Id, "value", 99);

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Available);
		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Available);

		// update the variable 'value' in the case model scope
		caseService.setVariable(caseInstanceId, "value", 102);

		// then sentry of HumanTask 1 and HumanTask 2 gets evaluated.
		humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSameVariableNameInDifferentScopes.cmmn"})]
	  public virtual void testNestedScopesWithNullVariableValue()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecution1_Id = queryCaseExecutionByActivityId("Stage_1").Id;

		// set the variable 'value' in the scope of the case model
		caseService.setVariable(caseInstanceId, "value", 99);

		// set the variable 'value' in the scope of the stage 1 with null value
		caseService.setVariableLocal(stageExecution1_Id, "value", null);

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Available);
		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Available);

		// update the variable 'value' in the case model scope
		caseService.setVariable(caseInstanceId, "value", 102);

		// then sentry of HumanTask 1 and HumanTask 2 gets evaluated.
		humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		// Sentry attached to HumanTask 2 is not evaluated because a variable 'value' exists in stage 2 even if the value is null 
		assertFalse(humanTask2.Enabled);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testDifferentVariableNameInDifferentScope.cmmn"})]
	  public virtual void testNestedScopesOfDifferentVariableNames()
	  {
		string caseInstanceId = caseService.createCaseInstanceByKey("Case_1").Id;

		string stageExecution1_Id = queryCaseExecutionByActivityId("Stage_1").Id;

		// inner stage
		string stageExecution2_Id = queryCaseExecutionByActivityId("Stage_2").Id;

		// set the variable 'value_1' in the scope of the case model
		caseService.setVariable(caseInstanceId, "value_1", 99);
		// set the variable 'value_1' in the scope of the stage 1
		caseService.setVariableLocal(stageExecution1_Id, "value_1", 99);
		// set the variable 'value_2' in the scope of the stage 1
		caseService.setVariableLocal(stageExecution1_Id, "value_2", 99);

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Available);
		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Available);

		// update the variable 'value_1' in the case model scope and stage scope
		caseService.setVariable(caseInstanceId, "value_1", 102);
		caseService.setVariableLocal(stageExecution1_Id, "value_1", 102);

		// then sentry of HumanTask 1 gets evaluated and sentry of HumanTask 2 does not gets evaluated.
		humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Enabled);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertFalse(humanTask2.Enabled);

		caseService.setVariable(stageExecution2_Id, "value_2", 102);
		humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Enabled);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/variableonpart/SentryVariableOnPartEntryCriteriaTest.testSameVariableOnPartAsEntryAndExitCriteria.cmmn"})]
	  public virtual void testSameVariableOnPartAsEntryAndExitCriteria()
	  {
		caseService.createCaseInstanceByKey("Case_1").Id;

		CaseExecution stageExecution = queryCaseExecutionByActivityId("Stage_1");

		caseService.setVariable(stageExecution.Id, "value", 99);

		CaseExecution humanTask = queryCaseExecutionByActivityId("HumanTask_1");
		// exit criteria not satisfied due to the variable 'value' must be greater than 100
		assertTrue(humanTask.Enabled);
		manualStart(humanTask.Id);

		caseService.setVariable(stageExecution.Id, "value", 101);
		stageExecution = queryCaseExecutionByActivityId("Stage_1");
		assertNull(stageExecution);
	  }
	}

}