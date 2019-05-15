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
namespace org.camunda.bpm.engine.test.cmmn.required
{

	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class RequiredRuleTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testRequiredRuleEvaluatesToTrue()
	  {
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", true));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
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

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testRequiredRuleEvaluatesToFalse()
	  {
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", false));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNotNull(taskExecution);
		assertFalse(taskExecution.Required);

		// completing manually should be allowed
		caseService.completeCaseExecution(caseInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testDefaultVariableBasedRule.cmmn")]
	  public virtual void testDefaultRequiredRuleEvaluatesToTrue()
	  {
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", true));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

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

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testDefaultVariableBasedRule.cmmn")]
	  public virtual void testDefaultRequiredRuleEvaluatesToFalse()
	  {
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", false));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNotNull(taskExecution);
		assertFalse(taskExecution.Required);

		// completing manually should be allowed
		caseService.completeCaseExecution(caseInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultRequiredRuleWithoutConditionEvaluatesToTrue()
	  public virtual void testDefaultRequiredRuleWithoutConditionEvaluatesToTrue()
	  {
		caseService.createCaseInstanceByKey("case");

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertThat(taskExecution, @is(notNullValue()));
		assertThat(taskExecution.Required, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultRequiredRuleWithEmptyConditionEvaluatesToTrue()
	  public virtual void testDefaultRequiredRuleWithEmptyConditionEvaluatesToTrue()
	  {
		caseService.createCaseInstanceByKey("case");

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertThat(taskExecution, @is(notNullValue()));
		assertThat(taskExecution.Required, @is(true));
	  }
	}

}