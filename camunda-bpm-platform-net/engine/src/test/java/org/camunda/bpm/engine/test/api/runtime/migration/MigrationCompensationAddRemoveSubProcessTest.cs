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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using CompensationModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CompensationModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationCompensationAddRemoveSubProcessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationCompensationAddRemoveSubProcessTest()
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


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateCompensationSubscriptionAddRemoveSubProcess()
	  public virtual void testMigrateCompensationSubscriptionAddRemoveSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		// subProcess is not mapped
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("compensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateCompensationSubscriptionAddRemoveSubProcessAssertActivityInstance()
	  public virtual void testMigrateCompensationSubscriptionAddRemoveSubProcessAssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		// subProcess is not mapped
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when compensating
		testHelper.completeTask("userTask2");

		// then the activity instance tree is correct
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("subProcess").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateCompensationSubscriptionAddRemoveSubProcessAssertExecutionTree()
	  public virtual void testMigrateCompensationSubscriptionAddRemoveSubProcessAssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		// subProcess is not mapped
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the execution tree is correct
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("subProcess").scope().eventScope().done());

	  }
	}

}