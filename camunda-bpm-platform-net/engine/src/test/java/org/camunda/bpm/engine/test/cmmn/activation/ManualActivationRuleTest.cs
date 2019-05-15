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
namespace org.camunda.bpm.engine.test.cmmn.activation
{

	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using Ignore = org.junit.Ignore;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ManualActivationRuleTest : CmmnProcessEngineTestCase
	{

	  /// <summary>
	  /// CAM-3170
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testManualActivationRuleEvaluatesToTrue()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("manualActivation", true));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);
		assertTrue(taskExecution.Enabled);
		assertFalse(taskExecution.Active);
	  }

	  /// <summary>
	  /// CAM-3170
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testManualActivationRuleEvaluatesToFalse()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("manualActivation", false));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);
		assertFalse(taskExecution.Enabled);
		assertTrue(taskExecution.Active);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testDefaultVariableBasedRule.cmmn")]
	  public virtual void testDefaultManualActivationRuleEvaluatesToTrue()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("manualActivation", true));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);
		assertTrue(taskExecution.Enabled);
		assertFalse(taskExecution.Active);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testDefaultVariableBasedRule.cmmn")]
	  public virtual void testDefaultManualActivationRuleEvaluatesToFalse()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("manualActivation", false));

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);
		assertFalse(taskExecution.Enabled);
		assertTrue(taskExecution.Active);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testActivationWithoutDefinition.cmmn")]
	  public virtual void testActivationWithoutManualActivationDefined()
	  {
		caseService.createCaseInstanceByKey("case");

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertThat(taskExecution,@is(notNullValue()));
		assertThat(taskExecution.Enabled,@is(false));
		assertThat("Human Task is active, when ManualActivation is omitted",taskExecution.Active,@is(true));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testActivationWithoutManualActivationExpressionDefined.cmmn")]
	  public virtual void testActivationWithoutManualActivationExpressionDefined()
	  {
		caseService.createCaseInstanceByKey("case");

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertThat(taskExecution,@is(notNullValue()));
		assertThat(taskExecution.Enabled,@is(true));
		assertThat("Human Task is not active, when ManualActivation's condition is empty",taskExecution.Active,@is(false));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/activation/ManualActivationRuleTest.testActivationWithoutManualActivationConditionDefined.cmmn")]
	  public virtual void testActivationWithoutManualActivationConditionDefined()
	  {
		caseService.createCaseInstanceByKey("case");

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertThat(taskExecution,@is(notNullValue()));
		assertThat(taskExecution.Enabled,@is(true));
		assertThat("Human Task is not active, when ManualActivation's condition is empty",taskExecution.Active,@is(false));
	  }

	}

}