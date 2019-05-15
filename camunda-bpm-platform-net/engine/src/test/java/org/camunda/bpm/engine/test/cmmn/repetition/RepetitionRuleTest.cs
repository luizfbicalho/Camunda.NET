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
namespace org.camunda.bpm.engine.test.cmmn.repetition
{
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class RepetitionRuleTest : CmmnProcessEngineTestCase
	{

	  private const string CASE_ID = "case";

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testVariableBasedRepetitionRuleEvaluatesToTrue()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repeat", true);
		createCaseInstanceByKey("case", variables);

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(2, query.count());
		assertEquals(1, query.available().count());
		assertEquals(1, query.active().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testVariableBasedRepetitionRuleEvaluatesToFalse()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repeat", false);
		createCaseInstanceByKey("case", variables);

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(1, query.count());
		assertEquals(1, query.active().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testDefaultVariableBasedRule.cmmn")]
	  public virtual void testDefaultVariableBasedRepetitionRuleEvaluatesToTrue()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repeat", true);
		createCaseInstanceByKey("case", variables);

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(2, query.count());
		assertEquals(1, query.available().count());
		assertEquals(1, query.active().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testDefaultVariableBasedRule.cmmn")]
	  public virtual void testDefaultVariableBasedRepetitionRuleEvaluatesToFalse()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repeat", false);
		createCaseInstanceByKey("case", variables);

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(1, query.count());
		assertEquals(1, query.active().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatTask()
	  public virtual void testRepeatTask()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(2, query.count());

		CaseExecution originInstance = query.active().singleResult();
		assertNotNull(originInstance);

		CaseExecution repetitionInstance = query.available().singleResult();
		assertNotNull(repetitionInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatStage()
	  public virtual void testRepeatStage()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");

		assertEquals(2, query.count());

		CaseExecution originInstance = query.active().singleResult();
		assertNotNull(originInstance);

		CaseExecution repetitionInstance = query.available().singleResult();
		assertNotNull(repetitionInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatMilestone()
	  public virtual void testRepeatMilestone()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;
		string milestoneId = queryCaseExecutionByActivityId("PI_Milestone_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1");

		assertEquals(1, query.count());
		assertTrue(query.singleResult().Available);
		assertFalse(milestoneId.Equals(query.singleResult().Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatTaskMultipleTimes()
	  public virtual void testRepeatTaskMultipleTimes()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when (1)
		disable(firstHumanTaskId);

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(2, query.count());

		CaseExecution originInstance = query.active().singleResult();
		assertNotNull(originInstance);

		CaseExecution repetitionInstance = query.available().singleResult();
		assertNotNull(repetitionInstance);

		// when (2)
		reenable(firstHumanTaskId);
		disable(firstHumanTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(3, query.count());

		// active instances
		assertEquals(2, query.active().count());

		// available instances
		assertEquals(1, query.available().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatStageMultipleTimes()
	  public virtual void testRepeatStageMultipleTimes()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when (1)
		disable(firstHumanTaskId);

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");

		assertEquals(2, query.count());

		CaseExecution originInstance = query.active().singleResult();
		assertNotNull(originInstance);

		CaseExecution repetitionInstance = query.available().singleResult();
		assertNotNull(repetitionInstance);

		// when (2)
		reenable(firstHumanTaskId);
		disable(firstHumanTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");

		assertEquals(3, query.count());

		// enabled instances
		assertEquals(2, query.active().count());

		// available instances
		assertEquals(1, query.available().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatMilestoneMultipleTimes()
	  public virtual void testRepeatMilestoneMultipleTimes()
	  {
		// given
		createCaseInstance();

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;
		string milestoneId = queryCaseExecutionByActivityId("PI_Milestone_1").Id;

		// when (1)
		disable(firstHumanTaskId);

		// then (2)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1");

		assertEquals(1, query.count());
		assertTrue(query.singleResult().Available);
		assertFalse(milestoneId.Equals(query.singleResult().Id));

		// when (2)
		reenable(firstHumanTaskId);
		disable(firstHumanTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1");

		assertEquals(1, query.count());
		assertTrue(query.singleResult().Available);
		assertFalse(milestoneId.Equals(query.singleResult().Id));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatTaskWithoutEntryCriteria.cmmn")]
	  public virtual void testRepeatTaskWithoutEntryCriteriaWhenCompleting()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_ID,Variables.createVariables().putValue("repeating", true)).Id;

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		CaseExecution activeCaseExecution = query.active().singleResult();
		assertNotNull(activeCaseExecution);

		// when (1)
		complete(activeCaseExecution.Id);

		// then (1)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		activeCaseExecution = query.active().singleResult();
		assertNotNull(activeCaseExecution);

		// when (2)
		caseService.setVariable(caseInstanceId,"repeating",false);
		complete(activeCaseExecution.Id);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(0, query.count());

		// then (3)
		query = caseService.createCaseExecutionQuery();
		assertEquals(1, query.count());
		assertEquals(caseInstanceId, query.singleResult().Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatStageWithoutEntryCriteria.cmmn")]
	  public virtual void testRepeatStageWithoutEntryCriteriaWhenCompleting()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_ID,Variables.createVariables().putValue("repeating",true)).Id;

		CaseExecutionQuery stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(1, stageQuery.count());

		CaseExecution activeStageCaseExecution = stageQuery.active().singleResult();
		assertNotNull(activeStageCaseExecution);

		CaseExecution humanTaskCaseExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// when (1)
		complete(humanTaskCaseExecution.Id);

		// then (1)
		stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(1, stageQuery.count());

		activeStageCaseExecution = stageQuery.active().singleResult();
		assertNotNull(activeStageCaseExecution);

		humanTaskCaseExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// when (2)
		caseService.setVariable(caseInstanceId,"repeating",false);
		complete(humanTaskCaseExecution.Id);

		// then (3)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();
		assertEquals(1, query.count());
		assertEquals(caseInstanceId, query.singleResult().Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatTaskWithoutEntryCriteria.cmmn")]
	  public virtual void testRepeatTaskWithoutEntryCriteriaWhenTerminating()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_ID,Variables.createVariables().putValue("repeating",true)).Id;

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		CaseExecution activeCaseExecution = query.active().singleResult();
		assertNotNull(activeCaseExecution);

		// when (1)
		terminate(activeCaseExecution.Id);

		// then (1)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		activeCaseExecution = query.active().singleResult();
		assertNotNull(activeCaseExecution);

		// when (2)
		caseService.setVariable(caseInstanceId,"repeating",false);
		terminate(activeCaseExecution.Id);

		// then (2)
		query = caseService.createCaseExecutionQuery();
		assertEquals(1, query.count());
		assertEquals(caseInstanceId, query.singleResult().Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatStageWithoutEntryCriteria.cmmn")]
	  public virtual void testRepeatStageWithoutEntryCriteriaWhenTerminating()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey(CASE_ID,Variables.createVariables().putValue("repeating",true)).Id;

		CaseExecutionQuery stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(1, stageQuery.count());

		CaseExecution activeStageCaseExecution = stageQuery.active().singleResult();
		assertNotNull(activeStageCaseExecution);

		CaseExecution humanTaskCaseExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// when (1)
		terminate(humanTaskCaseExecution.Id);

		// then (1)
		stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(1, stageQuery.count());

		activeStageCaseExecution = stageQuery.active().singleResult();
		assertNotNull(activeStageCaseExecution);

		humanTaskCaseExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// when (2)
		caseService.setVariable(caseInstanceId,"repeating",false);
		terminate(humanTaskCaseExecution.Id);

		// then (2)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();
		assertEquals(1, query.count());
		assertEquals(caseInstanceId, query.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatTaskWithoutEntryCriteriaOnCustomStandardEvent()
	  public virtual void testRepeatTaskWithoutEntryCriteriaOnCustomStandardEvent()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		CaseExecution enabledCaseExecution = query.enabled().singleResult();
		assertNotNull(enabledCaseExecution);

		// when (1)
		disable(enabledCaseExecution.Id);

		// then (1)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(2, query.count());

		enabledCaseExecution = query.enabled().singleResult();
		assertNotNull(enabledCaseExecution);

		// when (2)
		disable(enabledCaseExecution.Id);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(3, query.count());

		enabledCaseExecution = query.enabled().singleResult();
		assertNotNull(enabledCaseExecution);

		// when (3)
		complete(caseInstanceId);

		// then (3)
		query = caseService.createCaseExecutionQuery();
		assertEquals(1, query.count());
		assertEquals(caseInstanceId, query.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatStageWithoutEntryCriteriaOnCustomStandardEvent()
	  public virtual void testRepeatStageWithoutEntryCriteriaOnCustomStandardEvent()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecutionQuery stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(1, stageQuery.count());

		CaseExecution enabledStageCaseExecution = stageQuery.enabled().singleResult();
		assertNotNull(enabledStageCaseExecution);

		// when (1)
		disable(enabledStageCaseExecution.Id);

		// then (1)
		stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(2, stageQuery.count());

		enabledStageCaseExecution = stageQuery.enabled().singleResult();
		assertNotNull(enabledStageCaseExecution);

		// when (2)
		disable(enabledStageCaseExecution.Id);

		// then (2)
		stageQuery = caseService.createCaseExecutionQuery().activityId("PI_Stage_1");
		assertEquals(3, stageQuery.count());

		enabledStageCaseExecution = stageQuery.enabled().singleResult();
		assertNotNull(enabledStageCaseExecution);

		// when (3)
		complete(caseInstanceId);

		// then (3)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();
		assertEquals(1, query.count());
		assertEquals(caseInstanceId, query.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonRepeatableTaskDependsOnRepeatableTask()
	  public virtual void testNonRepeatableTaskDependsOnRepeatableTask()
	  {
		// given
		createCaseInstance();

		CaseExecutionQuery availableQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").available();

		// fire three times entry criteria of repeatable task
		// -> three enabled tasks
		// -> one available task
		fireEntryCriteria(availableQuery.singleResult().Id);
		fireEntryCriteria(availableQuery.singleResult().Id);
		fireEntryCriteria(availableQuery.singleResult().Id);

		// get any enabled task
		CaseExecutionQuery enabledQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").active();

		string enabledTaskId = enabledQuery.listPage(0, 1).get(0).Id;

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		assertNotNull(secondHumanTask);
		assertTrue(secondHumanTask.Available);

		// when
		complete(enabledTaskId);

		// then
		// there is only one instance of PI_HumanTask_2
		secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		assertNotNull(secondHumanTask);
		assertTrue(secondHumanTask.Active);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatableTaskDependsOnAnotherRepeatableTask()
	  public virtual void testRepeatableTaskDependsOnAnotherRepeatableTask()
	  {
		// given
		createCaseInstance();

		CaseExecutionQuery availableQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").available();

		// fire three times entry criteria of repeatable task
		// -> three enabled tasks
		// -> one available task
		fireEntryCriteria(availableQuery.singleResult().Id);
		fireEntryCriteria(availableQuery.singleResult().Id);
		fireEntryCriteria(availableQuery.singleResult().Id);

		// get any enabled task
		CaseExecutionQuery activeQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").active();

		string activeTaskId = activeQuery.listPage(0, 1).get(0).Id;

		// when (1)
		complete(activeTaskId);

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(2, query.count());
		assertEquals(1, query.active().count());
		assertEquals(1, query.available().count());

		// when (2)
		// get another enabled task
		activeTaskId = activeQuery.listPage(0, 1).get(0).Id;
		complete(activeTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(3, query.count());
		assertEquals(2, query.active().count());
		assertEquals(1, query.available().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLimitedRepetitions()
	  public virtual void testLimitedRepetitions()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repetition", 0);
		createCaseInstanceByKey("case", variables);

		CaseExecutionQuery availableQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").available();

		// fire three times entry criteria of repeatable task
		// -> three enabled tasks
		// -> one available task
		fireEntryCriteria(availableQuery.singleResult().Id);
		fireEntryCriteria(availableQuery.singleResult().Id);
		fireEntryCriteria(availableQuery.singleResult().Id);

		// get any enabled task
		CaseExecutionQuery activeQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").active();

		string activeTaskId = activeQuery.listPage(0, 1).get(0).Id;

		// when (1)
		complete(activeTaskId);

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(2, query.count());
		assertEquals(1, query.active().count());
		assertEquals(1, query.available().count());

		// when (2)
		activeTaskId = activeQuery.listPage(0, 1).get(0).Id;

		complete(activeTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(3, query.count());
		assertEquals(2, query.active().count());
		assertEquals(1, query.available().count());

		// when (3)
		activeTaskId = activeQuery.listPage(0, 1).get(0).Id;

		complete(activeTaskId);

		// then (3)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(3, query.count());
		assertEquals(3, query.active().count());
		assertEquals(0, query.available().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLimitedSequentialRepetitions()
	  public virtual void testLimitedSequentialRepetitions()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repetition", 0);
		createCaseInstanceByKey("case", variables);

		CaseExecutionQuery activeQuery = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").active();
		string enabledCaseExecutionId = activeQuery.singleResult().Id;

		// when (1)
		complete(enabledCaseExecutionId);

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());
		assertEquals(1, query.active().count());

		// when (2)
		enabledCaseExecutionId = activeQuery.singleResult().Id;
		complete(enabledCaseExecutionId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());
		assertEquals(1, query.active().count());

		// when (3)
		enabledCaseExecutionId = activeQuery.singleResult().Id;
		complete(enabledCaseExecutionId);

		// then (3)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLimitedParallelRepetitions()
	  public virtual void testLimitedParallelRepetitions()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("repetition", 0);
		createCaseInstanceByKey("case", variables);

		// when (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");

		// then (1)
		assertEquals(3, query.count());

		// when (2)
		// complete any task
		string caseExecutionId = query.listPage(0, 1).get(0).Id;
		complete(caseExecutionId);

		// then (2)
		assertEquals(2, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAutoCompleteStage()
	  public virtual void testAutoCompleteStage()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		// then
		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		assertNull(stage);

		CaseInstance caseInstance = (CaseInstance) queryCaseExecutionById(caseInstanceId);
		assertTrue(caseInstance.Completed);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAutoCompleteStageWithoutEntryCriteria()
	  public virtual void testAutoCompleteStageWithoutEntryCriteria()
	  {
		// given
		VariableMap variables = Variables.createVariables().putValue("manualActivation", false);
		string caseInstanceId = createCaseInstanceByKey("case", variables).Id;

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		assertEquals(1, query.active().count());
		string activeTaskId = query.singleResult().Id;

		// when (2)
		// completing active task
		complete(activeTaskId);

		// then (2)
		// the stage should be completed
		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		assertNull(stage);

		CaseInstance caseInstance = (CaseInstance) queryCaseExecutionById(caseInstanceId);
		assertTrue(caseInstance.Completed);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAutoCompleteStageAutoActivationRepeatableTask()
	  public virtual void testAutoCompleteStageAutoActivationRepeatableTask()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		string stageId = queryCaseExecutionByActivityId("PI_Stage_1").Id;

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		assertEquals(1, query.active().count());
		string activeTaskId = query.singleResult().Id;

		// when (2)
		// completing active task
		complete(activeTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		assertEquals(1, query.active().count());

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		assertNotNull(stage);
		assertTrue(stage.Active);

		CaseInstance caseInstance = (CaseInstance) queryCaseExecutionById(caseInstanceId);
		assertTrue(caseInstance.Active);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAutoCompleteStageRequiredRepeatableTask()
	  public virtual void testAutoCompleteStageRequiredRepeatableTask()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());

		assertEquals(1, query.active().count());
		string activeTaskId = query.singleResult().Id;

		// when (2)
		complete(activeTaskId);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());
		assertEquals(1, query.active().count());

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		assertNotNull(stage);
		assertTrue(stage.Active);

		CaseInstance caseInstance = (CaseInstance) queryCaseExecutionById(caseInstanceId);
		assertTrue(caseInstance.Active);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/repetition/RepetitionRuleTest.testRepeatTask.cmmn")]
	  public virtual void testShouldNotRepeatTaskAfterCompletion()
	  {
		// given
		createCaseInstance();
		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when (1)
		complete(humanTask1);

		// then (1)
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(2, query.count());
		assertEquals(1, query.available().count());
		assertEquals(1, query.active().count());

		// when (2)
		string humanTask2 = query.active().singleResult().Id;
		complete(humanTask2);

		// then (2)
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(1, query.count());
		assertEquals(1, query.available().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIgnoreRepeatOnStandardEvent()
	  public virtual void testIgnoreRepeatOnStandardEvent()
	  {
		// given
		createCaseInstance();

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;
		complete(humanTask1);

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(2, query.count());

		// when
		string humanTask2 = query.enabled().singleResult().Id;
		disable(humanTask2);

		// then
		query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");
		assertEquals(2, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultValueWithoutCondition()
	  public virtual void testDefaultValueWithoutCondition()
	  {
		createCaseInstanceByKey("case");
		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(2, query.count());
		assertEquals(1, query.available().count());
		assertEquals(1, query.active().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultValueWithEmptyCondition()
	  public virtual void testDefaultValueWithEmptyCondition()
	  {
		createCaseInstanceByKey("case");
		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask1);

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2");

		assertEquals(2, query.count());
		assertEquals(1, query.available().count());
		assertEquals(1, query.active().count());
	  }

	  // helper ////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void fireEntryCriteria(final String caseExecutionId)
	  protected internal virtual void fireEntryCriteria(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass : HelperCaseCommand
	  {
		  private readonly RepetitionRuleTest outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass(RepetitionRuleTest outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).fireEntryCriteria();
		  }
	  }

	}

}