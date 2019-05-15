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
namespace org.camunda.bpm.engine.test.cmmn.cmmn10
{
	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class Cmmn10CompatibilityTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testRequiredRule.cmmn")]
	  public virtual void testRequiredRule()
	  {
		CaseInstance caseInstance = createCaseInstanceByKey("case", Variables.createVariables().putValue("required", true));

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		assertNotNull(taskExecution);
		assertTrue(taskExecution.Required);

		try
		{
		  caseService.completeCaseExecution(caseInstance.Id);
		  fail("completing the containing stage should not be allowed");
		}
		catch (NotAllowedException)
		{
		  // happy path
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testManualActivationRule.cmmn")]
	  public virtual void testManualActivationRule()
	  {
		createCaseInstanceByKey("case", Variables.createVariables().putValue("manual", false));

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		assertNotNull(taskExecution);
		assertTrue(taskExecution.Active);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testManualActivationRuleWithoutCondition.cmmn")]
	  public virtual void testManualActivationRuleWithoutCondition()
	  {
		createCaseInstanceByKey("case", Variables.createVariables().putValue("manual", false));

		CaseExecution taskExecution = queryCaseExecutionByActivityId("PI_HumanTask_1");

		assertNotNull(taskExecution);
		assertTrue(taskExecution.Enabled);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testRepetitionRule.cmmn")]
	  public virtual void testRepetitionRule()
	  {
		// given
		createCaseInstanceByKey("case", Variables.createVariables().putValue("repetition", true));

		string secondHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_2").Id;

		// when
		complete(secondHumanTaskId);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(2, query.count());
		assertEquals(1, query.available().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testRepetitionRuleWithoutEntryCriteria.cmmn")]
	  public virtual void testRepetitionRuleWithoutEntryCriteria()
	  {
		// given
		createCaseInstanceByKey("case", Variables.createVariables().putValue("repetition", true));

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(firstHumanTaskId);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(1, query.count());
		assertEquals(1, query.active().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testRepetitionRuleCustomStandardEvent.cmmn")]
	  public virtual void testRepetitionRuleWithoutEntryCriteriaAndCustomStandardEvent()
	  {
		// given
		createCaseInstanceByKey("case", Variables.createVariables().putValue("repetition", true));

		string firstHumanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		disable(firstHumanTaskId);

		// then
		CaseExecutionQuery query = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1");
		assertEquals(2, query.count());
		assertEquals(1, query.enabled().count());
		assertEquals(1, query.disabled().count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testPlanItemEntryCriterion.cmmn")]
	  public virtual void testPlanItemEntryCriterion()
	  {
		// given
		createCaseInstanceByKey("case");
		string humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask);

		// then
		assertTrue(queryCaseExecutionByActivityId("PI_HumanTask_2").Active);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testPlanItemExitCriterion.cmmn")]
	  public virtual void testPlanItemExitCriterion()
	  {
		// given
		createCaseInstanceByKey("case");

		string humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask);

		// then
		assertNull(queryCaseExecutionByActivityId("PI_HumanTask_2"));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testCasePlanModelExitCriterion.cmmn")]
	  public virtual void testCasePlanModelExitCriterion()
	  {
		// given
		string caseInstanceId = createCaseInstanceByKey("case").Id;

		string humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// when
		complete(humanTask);

		// then
		assertTrue(queryCaseExecutionById(caseInstanceId).Terminated);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testSentryIfPartCondition.cmmn")]
	  public virtual void testSentryIfPartCondition()
	  {
		// given
		createCaseInstanceByKey("case", Variables.createVariables().putValue("value", 99));

		string humanTask1 = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;
		string humanTask2 = queryCaseExecutionByActivityId("PI_HumanTask_2").Id;

		assertTrue(queryCaseExecutionById(humanTask2).Available);

		// when
		caseService.withCaseExecution(humanTask1).setVariable("value", 999).manualStart();

		// then
		assertTrue(queryCaseExecutionById(humanTask2).Enabled);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/cmm10/Cmmn10CompatibilityTest.testDescription.cmmn")]
	  public virtual void testDescription()
	  {
		// given
		createCaseInstanceByKey("case");

		// when
		string humanTask = queryCaseExecutionByActivityId("PI_HumanTask_1").Id;

		// then
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("This is a description!", task.Description);

	  }

	}

}