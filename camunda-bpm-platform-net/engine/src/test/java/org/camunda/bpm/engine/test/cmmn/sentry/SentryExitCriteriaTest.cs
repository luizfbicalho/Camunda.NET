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
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SentryExitCriteriaTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitTask.cmmn"})]
	  public virtual void testExitTask()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;

		assertTrue(secondHumanTask.Active);

		assertNull(caseService.getVariable(caseInstanceId, "exit"));

		// (1) when
		complete(firstHumanTaskId);

		// (2) then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		object exitVariable = caseService.getVariable(caseInstanceId, "exit");
		assertNotNull(exitVariable);
		assertTrue((bool?) exitVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitStage.cmmn"})]
	  public virtual void testExitStage()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution stage = queryCaseExecutionByActivityId("PI_Stage_1");
		string stageId = stage.Id;
		assertTrue(stage.Active);

		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		CaseExecution milestone = queryCaseExecutionByActivityId("PI_Milestone_1");
		string milestoneId = milestone.Id;
		assertTrue(milestone.Available);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		assertNull(caseService.getVariable(caseInstanceId, "exit"));
		assertNull(caseService.getVariable(caseInstanceId, "parentTerminate"));

		// (2) when
		complete(firstHumanTaskId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertNull(stage);

		milestone = queryCaseExecutionById(milestoneId);
		assertNull(milestone);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		object exitVariable = caseService.getVariable(caseInstanceId, "exit");
		assertNotNull(exitVariable);
		assertTrue((bool?) exitVariable);

		object parentTerminateVariable = caseService.getVariable(caseInstanceId, "parentTerminate");
		assertNotNull(parentTerminateVariable);
		assertTrue((bool?) parentTerminateVariable);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testAndJoin.cmmn"})]
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
		assertTrue(thirdHumanTask.Active);

		// (1) when
		complete(firstHumanTaskId);

		// (1) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Active);

		assertNull(caseService.getVariable(caseInstanceId, "exit"));

		// (2) when
		complete(secondHumanTaskId);

		// (2) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertNull(thirdHumanTask);

		object exitVariable = caseService.getVariable(caseInstanceId, "exit");
		assertNotNull(exitVariable);
		assertTrue((bool?) exitVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testAndFork.cmmn"})]
	  public virtual void testAndFork()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Active);

		// when
		complete(firstHumanTaskId);

		// then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertNull(thirdHumanTask);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testOrJoin.cmmn"})]
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
		assertTrue(thirdHumanTask.Active);

		// (1) when
		complete(firstHumanTaskId);

		// (1) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertNull(thirdHumanTask);

		object exitVariable = caseService.getVariable(caseInstanceId, "exit");
		assertNotNull(exitVariable);
		assertTrue((bool?) exitVariable);

		// (2) when
		complete(secondHumanTaskId);

		// (2) then
		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertNull(thirdHumanTask);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testOrFork.cmmn"})]
	  public virtual void testOrFork()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Active);

		// when
		caseService.withCaseExecution(firstHumanTaskId).setVariable("value", 80).complete();

		// then
		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNotNull(secondHumanTask);
		assertTrue(secondHumanTask.Active);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertNull(thirdHumanTask);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testCycle.cmmn"})]
	  public virtual void testCycle()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;
		assertTrue(thirdHumanTask.Active);

		// when
		complete(firstHumanTaskId);

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertNull(thirdHumanTask);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitTaskWithIfPart.cmmn"})]
	  public virtual void testExitTaskWithIfPartSatisfied()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		// when
		caseService.withCaseExecution(firstHumanTaskId).setVariable("value", 100).complete();

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitTaskWithIfPart.cmmn"})]
	  public virtual void testExitTaskWithIfPartNotSatisfied()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;
		assertTrue(firstHumanTask.Active);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;
		assertTrue(secondHumanTask.Active);

		// when
		caseService.withCaseExecution(firstHumanTaskId).setVariable("value", 99).complete();

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitCriteriaOnCasePlanModel.cmmn"})]
	  public virtual void testExitCriteriaOnCasePlanModel()
	  {
		// given
		string caseInstanceId = createCaseInstance().Id;

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;

		assertTrue(firstHumanTask.Active);

		// when
		complete(firstHumanTaskId);

		// then
		CaseExecution caseInstance = queryCaseExecutionById(caseInstanceId);
		assertTrue(caseInstance.Terminated);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitOnParentSuspendInsideStage.cmmn"})]
	  public virtual void FAILING_testExitOnParentSuspendInsideStage()
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

		assertTrue(secondHumanTask.Enabled);

		// when
		suspend(stageId);

		// then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitOnParentResumeInsideStage.cmmn"})]
	  public virtual void FAILING_testExitOnParentResumeInsideStage()
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

		assertTrue(secondHumanTask.Enabled);

		// (1) when
		suspend(stageId);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(((CaseExecutionEntity)secondHumanTask).Suspended);

		// (2) when
		resume(stageId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Active);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(firstHumanTask.Enabled);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryExitCriteriaTest.testExitActiveTask.cmmn"})]
	  public virtual void testExitActiveTask()
	  {
		// given
		createCaseInstance();

		CaseExecution firstHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_1");
		string firstHumanTaskId = firstHumanTask.Id;

		assertTrue(firstHumanTask.Enabled);

		CaseExecution secondHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_2");
		string secondHumanTaskId = secondHumanTask.Id;

		assertTrue(secondHumanTask.Enabled);

		manualStart(secondHumanTaskId);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

		Task secondTask = taskService.createTaskQuery().singleResult();
		assertNotNull(secondTask);

		// when
		manualStart(firstHumanTaskId);

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(firstHumanTask.Active);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		secondTask = taskService.createTaskQuery().taskId(secondTask.Id).singleResult();
		assertNull(secondTask);

	  }
	}

}