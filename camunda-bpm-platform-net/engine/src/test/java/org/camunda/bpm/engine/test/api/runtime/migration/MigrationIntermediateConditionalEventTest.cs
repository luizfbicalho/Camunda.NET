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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.migration.validation.instruction.ConditionalEventUpdateEventTriggerValidator.MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG;
	using static org.camunda.bpm.engine.test.api.runtime.migration.models.ConditionalModels;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class MigrationIntermediateConditionalEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationIntermediateConditionalEventTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new MigrationTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}



	  public static readonly BpmnModelInstance ONE_CONDITION_PROCESS = ProcessModels.newModel().startEvent().intermediateCatchEvent(CONDITION_ID).conditionalEventDefinition("test").condition(VAR_CONDITION).conditionalEventDefinitionDone().userTask(USER_TASK_ID).endEvent().done();

	  protected internal const string VAR_NAME = "variable";
	  protected internal const string NEW_CONDITION_ID = "newCondition";
	  protected internal const string NEW_VAR_CONDITION = "${variable == 2}";
	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException exceptionRule = org.junit.rules.ExpectedException.none();
	  public ExpectedException exceptionRule = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscription()
	  public virtual void testMigrateEventSubscription()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_CONDITION_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ONE_CONDITION_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(CONDITION_ID, CONDITION_ID).updateEventTrigger().build();

		//when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated(CONDITION_ID, CONDITION_ID, null);

		//then it is possible to trigger the conditional event
		testHelper.setVariable(processInstance.Id, VAR_NAME, "1");

		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateConditionalEventWithoutUpdateTrigger()
	  public virtual void testMigrateConditionalEventWithoutUpdateTrigger()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_CONDITION_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ONE_CONDITION_PROCESS);

		//expect migration validation exception
		exceptionRule.expect(typeof(MigrationPlanValidationException));
		exceptionRule.expectMessage(MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG);

		//when conditional event is migrated without update event trigger
		rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(CONDITION_ID, CONDITION_ID).build();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionChangeCondition()
	  public virtual void testMigrateEventSubscriptionChangeCondition()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_CONDITION_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(Bpmn.createExecutableProcess().startEvent().intermediateCatchEvent(NEW_CONDITION_ID).conditionalEventDefinition().condition(NEW_VAR_CONDITION).conditionalEventDefinitionDone().userTask(USER_TASK_ID).done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(CONDITION_ID, NEW_CONDITION_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated(CONDITION_ID, NEW_CONDITION_ID, null);

		//and var is set with value of old condition
		testHelper.setVariable(processInstance.Id, VAR_NAME, "1");

		//then nothing happens
		assertNull(rule.TaskService.createTaskQuery().singleResult());

		//when correct value is set
		testHelper.setVariable(processInstance.Id, VAR_NAME, "2");

		//then condition is satisfied
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }
	}

}