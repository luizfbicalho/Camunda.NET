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
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using Ignore = org.junit.Ignore;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore public class SentryCombinedEntryAndExitCriteriaTest extends org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase
	public class SentryCombinedEntryAndExitCriteriaTest : CmmnProcessEngineTestCase
	{
		[Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryCombinedEntryAndExitCriteriaTest.testParentResumeInsideStage.cmmn"})]
		public virtual void FAILING_testParentResumeInsideStage()
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

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;

		assertTrue(thirdHumanTask.Available);

		// (1) when
		suspend(stageId);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(((CaseExecutionEntity)secondHumanTask).Suspended);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(((CaseExecutionEntity)thirdHumanTask).Suspended);

		// (2) when
		resume(stageId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(firstHumanTask.Enabled);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Enabled);

		}

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryCombinedEntryAndExitCriteriaTest.testParentSuspendInsideStage.cmmn"})]
	  public virtual void FAILING_testParentSuspendInsideStage()
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

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;

		assertTrue(thirdHumanTask.Available);

		// when
		suspend(stageId);

		// then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(((CaseExecutionEntity)thirdHumanTask).Suspended);
		assertEquals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED, ((CaseExecutionEntity) thirdHumanTask).PreviousState);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryCombinedEntryAndExitCriteriaTest.testParentResumeInsideStageDifferentPlanItemOrder.cmmn"})]
	  public virtual void FAILING_testParentResumeInsideStageDifferentPlanItemOrder()
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

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;

		assertTrue(thirdHumanTask.Available);

		// (1) when
		suspend(stageId);

		// (1) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(((CaseExecutionEntity)secondHumanTask).Suspended);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(((CaseExecutionEntity)thirdHumanTask).Suspended);

		// (2) when
		resume(stageId);

		// (2) then
		stage = queryCaseExecutionById(stageId);
		assertTrue(stage.Active);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(firstHumanTask.Enabled);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(thirdHumanTask.Enabled);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryCombinedEntryAndExitCriteriaTest.testParentSuspendInsideStageDifferentPlanItemOrder.cmmn"})]
	  public virtual void FAILING_testParentSuspendInsideStageDifferentPlanItemOrder()
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

		CaseExecution thirdHumanTask = queryCaseExecutionByActivityId("PI_HumanTask_3");
		string thirdHumanTaskId = thirdHumanTask.Id;

		assertTrue(thirdHumanTask.Available);

		// when
		suspend(stageId);

		// then
		stage = queryCaseExecutionById(stageId);
		assertTrue(((CaseExecutionEntity)stage).Suspended);

		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertTrue(((CaseExecutionEntity)firstHumanTask).Suspended);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);

		thirdHumanTask = queryCaseExecutionById(thirdHumanTaskId);
		assertTrue(((CaseExecutionEntity)thirdHumanTask).Suspended);
		assertEquals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED, ((CaseExecutionEntity) thirdHumanTask).PreviousState);

	  }
	}

}