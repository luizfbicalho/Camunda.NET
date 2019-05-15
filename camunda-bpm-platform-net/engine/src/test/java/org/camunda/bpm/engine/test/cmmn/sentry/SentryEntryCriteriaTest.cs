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
namespace org.camunda.bpm.engine.test.cmmn.sentry
{

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CaseSentryPartQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartQueryImpl;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CmmnSentryPart = org.camunda.bpm.engine.impl.cmmn.execution.CmmnSentryPart;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SentryEntryCriteriaTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceEnableTask.cmmn"})]
	  public virtual void testSequenceEnableTask()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		// (1) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Available);

		assertNull(caseService.getVariable(caseInstanceId, "start"));

		// (2) when
		complete(firstHumanTaskId);

		// (2) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Enabled);

		object enableVariable = caseService.getVariable(caseInstanceId, "enable");
		assertNotNull(enableVariable);
		assertTrue((bool?) enableVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceAutoStartTask.cmmn"})]
	  public virtual void testSequenceAutoStartTask()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		// (1) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Available);

		assertNull(caseService.getVariable(caseInstanceId, "enable"));
		assertNull(caseService.getVariable(caseInstanceId, "start"));

		// (2) when
		complete(firstHumanTaskId);

		// (2) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

		assertNull(caseService.getVariable(caseInstanceId, "enable"));
		object startVariable = caseService.getVariable(caseInstanceId, "start");
		assertNotNull(startVariable);
		assertTrue((bool?) startVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceEnableStage.cmmn"})]
	  public virtual void testSequenceEnableStage()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		string stageId = stage.Id;
		assertTrue(stage.Available);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Available);

		assertNull(caseService.getVariable(caseInstanceId, "enable"));

		// (2) when
		complete(firstHumanTaskId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Enabled);

		object enableVariable = caseService.getVariable(caseInstanceId, "enable");
		assertNotNull(enableVariable);
		assertTrue((bool?) enableVariable);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceAutoStartStage.cmmn"})]
	  public virtual void testSequenceAutoStartStage()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		string stageId = stage.Id;
		assertTrue(stage.Available);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Available);

		assertNull(caseService.getVariable(caseInstanceId, "enable"));
		assertNull(caseService.getVariable(caseInstanceId, "start"));

		// (2) when
		complete(firstHumanTaskId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		assertNull(caseService.getVariable(caseInstanceId, "enable"));
		object startVariable = caseService.getVariable(caseInstanceId, "start");
		assertNotNull(startVariable);
		assertTrue((bool?) startVariable);

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().enabled();

		assertEquals(2, query.count());

		foreach (CaseExecution child in query.list())
		{
		  assertEquals(stageId, child.ParentId);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceOccurMilestone.cmmn"})]
	  public virtual void testSequenceOccurMilestone()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution milestone = queryCaseExecutionByActivityId("PI_Milestone_1");
		string milestoneId = milestone.Id;
		assertTrue(milestone.Available);

		// (1) then
		milestone = queryCaseExecutionById(milestoneId);
		assertTrue(milestone.Available);

		assertNull(caseService.getVariable(caseInstanceId, "occur"));

		// (2) when
		complete(firstHumanTaskId);

		// (2) then
		milestone = queryCaseExecutionById(milestoneId);
		assertNull(milestone);

		object occurVariable = caseService.getVariable(caseInstanceId, "occur");
		assertNotNull(occurVariable);
		assertTrue((bool?) occurVariable);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequence.cmmn"})]
	  public virtual void testSequence()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		// (1) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Available);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Available);

		assertNull(caseService.getVariable(caseInstanceId, "start"));

		// (2) when (complete first human task) /////////////////////////////////////////////
		complete(firstHumanTaskId);

		// (2) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Available);

		object enableVariable = caseService.getVariable(caseInstanceId, "start");
		assertNotNull(enableVariable);
		assertTrue((bool?) enableVariable);

		// reset variable
		caseService.withCaseExecution(caseInstanceId).removeVariable("start").execute();

		// (3) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Available);

		assertNull(caseService.getVariable(caseInstanceId, "start"));

		// (4) when (complete second human task) //////////////////////////////////////////
		complete(secondHumanTaskId);

		// (4) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

		enableVariable = caseService.getVariable(caseInstanceId, "start");
		assertNotNull(enableVariable);
		assertTrue((bool?) enableVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceWithIfPart.cmmn"})]
	  public virtual void testSequenceWithIfPartNotSatisfied()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		// when
		caseService.withCaseExecution(firstHumanTaskId).setVariable("value", 99).complete();

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Available);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceWithIfPart.cmmn"})]
	  public virtual void testSequenceWithIfPartSatisfied()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		// when
		caseService.withCaseExecution(firstHumanTaskId).setVariable("value", 100).complete();

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testAndFork.cmmn"})]
	  public virtual void testAndFork()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		CaseSentryPartQueryImpl query = createCaseSentryPartQuery();
		CmmnSentryPart part = query.singleResult();
		assertFalse(part.Satisfied);

		// when
		complete(firstHumanTaskId);

		// then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

		part = query.singleResult();
		assertNotNull(part);
		assertFalse(part.Satisfied);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testAndJoin.cmmn"})]
	  public virtual void testAndJoin()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		// (1) when
		complete(firstHumanTaskId);

		// (1) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Available);

		assertNull(caseService.getVariable(caseInstanceId, "start"));

		// (2) when
		complete(secondHumanTaskId);

		// (2) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

		object startVariable = caseService.getVariable(caseInstanceId, "start");
		assertNotNull(startVariable);
		assertTrue((bool?) startVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testSequenceCombinedWithAndJoin.cmmn"})]
	  public virtual void testSequenceCombinedWithAndJoin()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		// (1) when
		complete(firstHumanTaskId);

		// (1) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Available); // still available

		// (2) when
		complete(secondHumanTaskId);

		// (2) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testOrFork.cmmn"})]
	  public virtual void testOrFork()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		// when
		caseService.withCaseExecution(firstHumanTaskId).setVariable("value", 80).complete();

		// then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Available);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testOrJoin.cmmn"})]
	  public virtual void testOrJoin()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		// (1) when
		complete(firstHumanTaskId);

		// (1) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

		object startVariable = caseService.getVariable(caseInstanceId, "start");
		assertNotNull(startVariable);
		assertTrue((bool?) startVariable);

		// (2) when
		complete(secondHumanTaskId);

		// (2) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active); // is still active
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testCycle.cmmn"})]
	  public virtual void testCycle()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Available);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Available);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Available);

		try
		{
		  // (1) when
		  manualStart(firstHumanTaskId);
		  fail("It should not be possible to start the first human task manually.");
		}
		catch (NotAllowedException)
		{
		}

		try
		{
		  // (2) when
		  manualStart(secondHumanTaskId);
		  fail("It should not be possible to start the second human task manually.");
		}
		catch (NotAllowedException)
		{
		}

		try
		{
		  // (3) when
		  manualStart(thirdHumanTaskId);
		  fail("It should not be possible to third the second human task manually.");
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testEnableByInstanceCreation.cmmn"})]
	  public virtual void testEnableByInstanceCreation()
	  {
		// given + when
		createCaseInstance();

		// then
		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		assertTrue(secondHumanTask.Active);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testEnableOnParentSuspendInsideStage.cmmn"})]
	  public virtual void FAILING_testEnableOnParentSuspendInsideStage()
	  {
		// given
		createCaseInstance();

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		string stageId = stage.Id;

		manualStart(stageId);

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;

		assertTrue(firstHumanTask.Enabled);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;

		assertTrue(secondHumanTask.Available);

		// (1) when
		suspend(stageId);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(((CaseExecutionEntity)secondHumanTask).Suspended);
		assertEquals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED, ((CaseExecutionEntity)secondHumanTask).PreviousState);

		// (2) when
		resume(stageId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(firstHumanTask.Enabled);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Enabled);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testEnableOnParentResumeInsideStage.cmmn"})]
	  public virtual void FAILING_testEnableOnParentResumeInsideStage()
	  {
		// given
		createCaseInstance();

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		string stageId = stage.Id;

		manualStart(stageId);

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;

		assertTrue(firstHumanTask.Enabled);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;

		assertTrue(secondHumanTask.Available);

		// (1) when
		suspend(stageId);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(((CaseExecutionEntity)secondHumanTask).Suspended);
		assertEquals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE, ((CaseExecutionEntity)secondHumanTask).PreviousState);

		// (2) when
		resume(stageId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(firstHumanTask.Enabled);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Enabled);
	  }

	  /// <summary>
	  /// Please note that suspension and/or resuming is currently
	  /// not supported by the public API. Furthermore the given
	  /// test is not a very useful use case in that just a milestone
	  /// will be suspended.
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testResume.cmmn"})]
	  public virtual void FAILING_testResume()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;

		assertTrue(firstHumanTask.Enabled);

		CaseExecution milestone = queryCaseExecutionByActivityId("PI_Milestone_1");
		string milestoneId = milestone.Id;

		assertTrue(milestone.Available);

		suspend(milestoneId);

		// (1) when
		manualStart(firstHumanTaskId);
		complete(firstHumanTaskId);

		// (1) then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		milestone = queryCaseExecutionById(milestoneId);
		assertTrue(((CaseExecutionEntity) milestone).Suspended);

		// (2) when
		resume(milestoneId);

		// (2)
		milestone = queryCaseExecutionById(milestoneId);
		assertNull(milestone);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testFireAlsoNotAffectedSentries.cmmn"})]
	  public virtual void testFireAlsoNotAffectedSentries()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;

		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;

		assertTrue(secondHumanTask.Available);

		CaseExecution milestone = queryCaseExecutionByActivityId("PI_Milestone_1");
		string milestoneId = milestone.Id;

		assertTrue(milestone.Available);

		caseService.withCaseExecution(caseInstanceId).setVariable("value", 99).execute();

		// (1) when
		complete(firstHumanTaskId);

		// (1) then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Available);

		// (2) when
		caseService.withCaseExecution(caseInstanceId).setVariable("value", 101).execute();

		// (2) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

		milestone = queryCaseExecutionById(milestoneId);
		// milestone occurs when the sentry was evaluated successfully after value is set to 101
		assertNull(milestone);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCaseFileItemOnPart()
	  public virtual void testCaseFileItemOnPart()
	  {
		createCaseInstance().Id;

		CaseExecution humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");

		// sentry has been ignored
		assertTrue(humanTask.Active);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testReusableStage()
	  public virtual void testReusableStage()
	  {
		// given
		createCaseInstance();

		string firstStageId = queryCaseExecutionByActivityId("PI_Stage_1").Id;
		string secondStageId = queryCaseExecutionByActivityId("PI_Stage_2").Id;

		IList<CaseExecution> humanTasks = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").active().list();
		assertEquals(2, humanTasks.Count);

		string humanTaskInsideFirstStageId = null;
		if (((CaseExecutionEntity) humanTasks[0]).ParentId.Equals(firstStageId))
		{
		  humanTaskInsideFirstStageId = humanTasks[0].Id;
		}
		else
		{
		  humanTaskInsideFirstStageId = humanTasks[1].Id;
		}

		// when
		complete(humanTaskInsideFirstStageId);

		// then
		CaseExecution secondHumanTaskInsideFirstStage = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2").active().singleResult();
		assertEquals(firstStageId, ((CaseExecutionEntity) secondHumanTaskInsideFirstStage).ParentId);

		// PI_HumanTask_1 in PI_Stage_2 is enabled
		CaseExecution firstHumanTaskInsideSecondStage = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertNotNull(firstHumanTaskInsideSecondStage);
		assertTrue(firstHumanTaskInsideSecondStage.Active);
		assertEquals(secondStageId, ((CaseExecutionEntity) firstHumanTaskInsideSecondStage).ParentId);

		// PI_HumanTask_2 in PI_Stage_2 is available
		CaseExecution secondHumanTaskInsideSecondStage = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2").available().singleResult();
		assertNotNull(secondHumanTaskInsideSecondStage);
		assertTrue(secondHumanTaskInsideSecondStage.Available);
		assertEquals(secondStageId, ((CaseExecutionEntity) secondHumanTaskInsideSecondStage).ParentId);
	  }

	  /// <summary>
	  /// CAM-3226
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSentryShouldNotBeEvaluatedAfterStageComplete()
	  public virtual void testSentryShouldNotBeEvaluatedAfterStageComplete()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		// when
		CaseExecution stageExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();
		assertNotNull(stageExecution);

		// .. there is a local stage variable
		caseService.setVariableLocal(stageExecution.Id, "value", 99);

		// .. and the stage is activated (such that the tasks are instantiated)
		caseService.manuallyStartCaseExecution(stageExecution.Id);

		CaseExecution task1Execution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(task1Execution);

		// then
		// .. completing the stage should be successful; evaluating Sentry_1 should not fail
		caseService.completeCaseExecution(task1Execution.Id);
		stageExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();
		assertNull(stageExecution);

		// .. and the case plan model should have completed
		CaseExecution casePlanModelExecution = caseService.createCaseExecutionQuery().caseExecutionId(caseInstanceId).singleResult();
		assertNotNull(casePlanModelExecution);
		assertFalse(casePlanModelExecution.Active);

		caseService.closeCaseInstance(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIfPartOnCaseInstanceCreate()
	  public virtual void testIfPartOnCaseInstanceCreate()
	  {

		// when
		createCaseInstanceByKey("case", Variables.putValue("value", 101));

		// then
		CaseExecution caseExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(caseExecution.Active);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIfPartOnCaseInstanceCreateWithSentry()
	  public virtual void testIfPartOnCaseInstanceCreateWithSentry()
	  {

		// when
		createCaseInstanceByKey("case", Variables.putValue("myVar", 101));

		// then
		CaseExecution caseExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");
		assertTrue(caseExecution.Active);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.testShouldNotTriggerCompletionTwice.cmmn", "org/camunda/bpm/engine/test/cmmn/sentry/SentryEntryCriteriaTest.noop.bpmn20.xml" })]
	  public virtual void testShouldNotTriggerCompletionTwice()
	  {
		// when
		CaseInstance ci = caseService.createCaseInstanceByKey("case");

		// then
		assertTrue(ci.Completed);
	  }

	}

}