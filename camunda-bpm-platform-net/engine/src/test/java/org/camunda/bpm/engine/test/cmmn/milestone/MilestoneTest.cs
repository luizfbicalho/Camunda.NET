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
namespace org.camunda.bpm.engine.test.cmmn.milestone
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class MilestoneTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/milestone/MilestoneTest.testWithoutEntryCriterias.cmmn"})]
	  public virtual void testWithoutEntryCriterias()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		assertTrue(caseInstance.Completed);

		object occurVariable = caseService.getVariable(caseInstanceId, "occur");
		assertNotNull(occurVariable);
		assertTrue((bool?) occurVariable);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/milestone/MilestoneTest.testWithEntryCriteria.cmmn"})]
	  public virtual void testWithEntryCriteria()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		CaseExecution milestone = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult();

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		assertTrue(milestone.Available);

		// then
		assertNull(caseService.getVariable(caseInstanceId, "occur"));

		milestone = caseService.createCaseExecutionQuery().available().singleResult();

		assertTrue(milestone.Available);

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		object occurVariable = caseService.getVariable(caseInstanceId, "occur");
		assertNotNull(occurVariable);
		assertTrue((bool?) occurVariable);

		milestone = caseService.createCaseExecutionQuery().available().singleResult();

		assertNull(milestone);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		assertTrue(caseInstance.Completed);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/milestone/MilestoneTest.testWithMultipleEntryCriterias.cmmn"})]
	  public virtual void testWithMultipleEntryCriterias()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		CaseExecution milestone = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult();

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_2").singleResult().Id;

		assertTrue(milestone.Available);

		// then
		assertNull(caseService.getVariable(caseInstanceId, "occur"));

		milestone = caseService.createCaseExecutionQuery().available().singleResult();

		assertTrue(milestone.Available);

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		object occurVariable = caseService.getVariable(caseInstanceId, "occur");
		assertNotNull(occurVariable);
		assertTrue((bool?) occurVariable);

		milestone = caseService.createCaseExecutionQuery().available().singleResult();

		assertNull(milestone);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		assertTrue(caseInstance.Active);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/milestone/MilestoneTest.testWithEntryCriteria.cmmn"})]
	  public virtual void testActivityType()
	  {
		// given
		caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		CaseExecution milestone = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult();

		// then
		assertEquals("milestone", milestone.ActivityType);
	  }

	}

}