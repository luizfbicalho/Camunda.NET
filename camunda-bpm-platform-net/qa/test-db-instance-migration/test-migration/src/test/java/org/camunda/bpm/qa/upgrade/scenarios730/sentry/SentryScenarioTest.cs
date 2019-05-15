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
namespace org.camunda.bpm.qa.upgrade.scenarios730.sentry
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using CaseService = org.camunda.bpm.engine.CaseService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseSentryPartEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity;
	using CaseSentryPartQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartQueryImpl;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[ScenarioUnderTest("SentryScenario"), Origin("7.3.0")]
	public class SentryScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

	  protected internal CaseService caseService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		ProcessEngine processEngine = rule.ProcessEngine;
		caseService = processEngine.CaseService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("triggerTaskEntryCriterion.1") public void testTriggerTaskEntryCriterion()
	  [ScenarioUnderTest("triggerTaskEntryCriterion.1")]
	  public virtual void testTriggerTaskEntryCriterion()
	  {
		// given
		// enabled human task inside a stage instance
		string firstHumanTaskId = rule.caseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		// start and complete human task
		caseService.manuallyStartCaseExecution(firstHumanTaskId);
		caseService.completeCaseExecution(firstHumanTaskId);

		// then
		// entry criterion of the second human task inside the stage instance
		// will be triggered
		CaseExecution secondHumanTask = rule.caseExecutionQuery().activityId("PI_HumanTask_2").singleResult();
		// ... and the task is enabled
		assertTrue(secondHumanTask.Enabled);

		CaseSentryPartEntity sentryPart = createCaseSentryPartQuery().sourceCaseExecutionId(firstHumanTaskId).singleResult();
		// the associated sentry part is not satisfied
		assertFalse(sentryPart.Satisfied);
		// the source is null (because this sentry part
		// has been migrated into 7.4)
		assertNull(sentryPart.Source);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("triggerStageEntryCriterion.1") public void testTriggerStageEntryCriterion()
	  [ScenarioUnderTest("triggerStageEntryCriterion.1")]
	  public virtual void testTriggerStageEntryCriterion()
	  {
		// given
		string secondHumanTaskId = rule.caseExecutionQuery().activityId("PI_HumanTask_2").singleResult().Id;
		string firstStageId = rule.caseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		// when
		// complete human task
		caseService.completeCaseExecution(secondHumanTaskId);

		// then
		// "PI_Stage_1" should be completed
		CaseExecution firstStage = rule.caseExecutionQuery().activityId("PI_Stage_1").singleResult();
		assertNull(firstStage);

		// "PI_Stage_2" should be enabled
		CaseExecution secondStage = rule.caseExecutionQuery().activityId("PI_Stage_2").singleResult();
		assertNotNull(secondStage);
		assertTrue(secondStage.Enabled);

		CaseSentryPartEntity sentryPart = createCaseSentryPartQuery().sourceCaseExecutionId(firstStageId).singleResult();
		// the associated sentry part is not satisfied
		assertFalse(sentryPart.Satisfied);
		// the source is null (since this sentry part
		// has been migrated into 7.4)
		assertNull(sentryPart.Source);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("newSentryInstance.1") public void testNewSentryInstance()
	  [ScenarioUnderTest("newSentryInstance.1")]
	  public virtual void testNewSentryInstance()
	  {
		// given
		string secondStageId = rule.caseExecutionQuery().activityId("PI_Stage_2").singleResult().Id;

		// when
		// start human task
		caseService.manuallyStartCaseExecution(secondStageId);

		// then
		// a new sentry instance should be created
		CaseSentryPartEntity sentryPart = createCaseSentryPartQuery().caseExecutionId(secondStageId).singleResult();
		assertNotNull(sentryPart);
		assertFalse(sentryPart.Satisfied);
		assertNull(sentryPart.SourceCaseExecutionId);
		assertEquals("PI_HumanTask_1", sentryPart.Source);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("completeInstance.1") public void testCompleteInstance()
	  [ScenarioUnderTest("completeInstance.1")]
	  public virtual void testCompleteInstance()
	  {
		// given

		// when
		string firstHumanTaskId = rule.caseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;
		caseService.manuallyStartCaseExecution(firstHumanTaskId);
		caseService.completeCaseExecution(firstHumanTaskId);

		string secondHumanTaskId = rule.caseExecutionQuery().activityId("PI_HumanTask_2").singleResult().Id;
		caseService.manuallyStartCaseExecution(secondHumanTaskId);
		caseService.completeCaseExecution(secondHumanTaskId);

		string secondStageId = rule.caseExecutionQuery().activityId("PI_Stage_2").singleResult().Id;
		caseService.manuallyStartCaseExecution(secondStageId);

		firstHumanTaskId = rule.caseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;
		caseService.manuallyStartCaseExecution(firstHumanTaskId);
		caseService.completeCaseExecution(firstHumanTaskId);

		secondHumanTaskId = rule.caseExecutionQuery().activityId("PI_HumanTask_2").singleResult().Id;
		caseService.manuallyStartCaseExecution(secondHumanTaskId);
		caseService.completeCaseExecution(secondHumanTaskId);

		// then
		CaseInstance caseInstance = rule.caseInstance();
		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);

		caseService.closeCaseInstance(caseInstance.Id);
		assertNull(rule.processInstanceQuery().singleResult());
	  }

	  // queries /////////////////////////////////

	  protected internal virtual CaseSentryPartQueryImpl createCaseSentryPartQuery()
	  {
		ProcessEngine processEngine = rule.ProcessEngine;
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
		return new CaseSentryPartQueryImpl(commandExecutor);
	  }

	}

}