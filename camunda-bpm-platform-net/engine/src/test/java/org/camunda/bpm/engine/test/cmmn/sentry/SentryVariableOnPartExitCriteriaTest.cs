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
	public class SentryVariableOnPartExitCriteriaTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryVariableOnPartExitCriteriaTest.testExitTaskWithVariableOnPart.cmmn"})]
	  public virtual void testExitTaskWithVariableOnPartSatisfied()
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
		caseService.withCaseExecution(firstHumanTaskId).setVariable("variable_1", 100).complete();

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertNull(secondHumanTask);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryVariableOnPartExitCriteriaTest.testExitTaskWithVariableOnPart.cmmn"})]
	  public virtual void testExitTaskWithVariableOnPartNotSatisfied()
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
		caseService.withCaseExecution(firstHumanTaskId).setVariable("variable", 100).complete();

		// then
		firstHumanTask = queryCaseExecutionById(firstHumanTaskId);
		assertNull(firstHumanTask);

		secondHumanTask = queryCaseExecutionById(secondHumanTaskId);
		assertTrue(secondHumanTask.Active);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryVariableOnPartExitCriteriaTest.testExitTaskWithMultipleOnPart.cmmn"})]
	  public virtual void testExitTaskWithMultipleOnPartSatisfied()
	  {
		// given
		createCaseInstance();

		CaseExecution stageExecution;

		CaseExecution humanTask1 = queryCaseExecutionByActivityId("HumanTask_1");
		assertTrue(humanTask1.Active);

		CaseExecution humanTask2 = queryCaseExecutionByActivityId("HumanTask_2");
		assertTrue(humanTask2.Active);

		complete(humanTask1.Id);

		stageExecution = queryCaseExecutionByActivityId("Stage_1");
		// Still if part and variable on part conditions are yet to be satisfied for the exit criteria
		assertNotNull(stageExecution);

		caseService.setVariable(stageExecution.Id, "value", 99);
		stageExecution = queryCaseExecutionByActivityId("Stage_1");
		// Still if part is yet to be satisfied for the exit criteria
		assertNotNull(stageExecution);

		caseService.setVariable(stageExecution.Id, "value", 101);
		stageExecution = queryCaseExecutionByActivityId("Stage_1");
		// exit criteria satisfied
		assertNull(stageExecution);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryVariableOnPartExitCriteriaTest.testExitTasksOfDifferentScopes.cmmn"})]
	  public virtual void testExitMultipleTasksOfDifferentScopes()
	  {
		// given
		createCaseInstance();

		CaseExecution stageExecution1 = queryCaseExecutionByActivityId("Stage_1");

		caseService.setVariable(stageExecution1.Id, "value", 101);

		stageExecution1 = queryCaseExecutionByActivityId("Stage_1");
		assertNull(stageExecution1);

		CaseExecution stageExecution2 = queryCaseExecutionByActivityId("Stage_2");
		assertNull(stageExecution2);

	  }
	}

}