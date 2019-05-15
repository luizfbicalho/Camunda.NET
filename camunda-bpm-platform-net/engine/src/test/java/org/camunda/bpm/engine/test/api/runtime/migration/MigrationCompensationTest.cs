using System.Collections.Generic;

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
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigratingProcessInstanceValidationReportAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;

	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using CompensationModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CompensationModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationCompensationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationCompensationTest()
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
//ORIGINAL LINE: @Test public void testCannotMigrateActivityInstanceForCompensationThrowingEvent()
	  public virtual void testCannotMigrateActivityInstanceForCompensationThrowingEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("compensationEvent", "compensationEvent").mapActivities("compensationHandler", "compensationHandler").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures("compensationEvent", "The type of the source activity is not supported for activity instance migration");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateActivityInstanceForCancelEndEvent()
	  public virtual void testCannotMigrateActivityInstanceForCancelEndEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.TRANSACTION_COMPENSATION_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.TRANSACTION_COMPENSATION_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("transactionEndEvent", "transactionEndEvent").mapActivities("compensationHandler", "compensationHandler").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures("transactionEndEvent", "The type of the source activity is not supported for activity instance migration");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateActiveCompensationWithoutInstructionForThrowingEventCase1()
	  public virtual void testCannotMigrateActiveCompensationWithoutInstructionForThrowingEventCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("compensationHandler", "compensationHandler").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures("compensationEvent", "There is no migration instruction for this instance's activity", "The type of the source activity is not supported for activity instance migration");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateActiveCompensationWithoutInstructionForThrowingEventCase2()
	  public virtual void testCannotMigrateActiveCompensationWithoutInstructionForThrowingEventCase2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_END_EVENT_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_END_EVENT_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("compensationHandler", "compensationHandler").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures("compensationEvent", "There is no migration instruction for this instance's activity", "The type of the source activity is not supported for activity instance migration");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateWithoutMappingCompensationBoundaryEvents()
	  public virtual void testCannotMigrateWithoutMappingCompensationBoundaryEvents()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures(sourceProcessDefinition.Id, "Cannot migrate subscription for compensation handler 'compensationHandler'. " + "There is no migration instruction for the compensation boundary event");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotRemoveCompensationEventSubscriptions()
	  public virtual void testCannotRemoveCompensationEventSubscriptions()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures(sourceProcessDefinition.Id, "Cannot migrate subscription for compensation handler 'compensationHandler'. " + "There is no migration instruction for the compensation boundary event");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanRemoveCompensationBoundaryWithoutEventSubscriptions()
	  public virtual void testCanRemoveCompensationBoundaryWithoutEventSubscriptions()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);
		testHelper.completeTask("userTask1");

		// then
		Assert.assertEquals(0, testHelper.snapshotAfterMigration.EventSubscriptions.Count);

		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotTriggerAddedCompensationForCompletedInstances()
	  public virtual void testCannotTriggerAddedCompensationForCompletedInstances()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(0, testHelper.snapshotAfterMigration.EventSubscriptions.Count);

		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanTriggerAddedCompensationForActiveInstances()
	  public virtual void testCanTriggerAddedCompensationForActiveInstances()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask1").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.completeTask("userTask1");
		Assert.assertEquals(1, rule.RuntimeService.createEventSubscriptionQuery().count());

		testHelper.completeTask("userTask2");
		testHelper.completeTask("compensationHandler");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationSubscriptionsInMigratingScope()
	  public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

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
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationSubscriptionsInMigratingScopeAssertActivityInstance()
	  public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScopeAssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// a migrated process instance
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when triggering compensation
		testHelper.completeTask("userTask2");

		// then the activity instance tree is correct
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationSubscriptionsInMigratingScopeAssertExecutionTree()
	  public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScopeAssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationSubscriptionsInMigratingScopeChangeIds()
	  public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScopeChangeIds()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.ONE_COMPENSATION_TASK_MODEL).changeElementId("userTask1", "newUserTask1").changeElementId("compensationBoundary", "newCompensationBoundary").changeElementId("compensationHandler", "newCompensationHandler"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "newCompensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated("compensationHandler", "newCompensationHandler", null);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("newCompensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationEventScopeExecution()
	  public virtual void testCanMigrateWithCompensationEventScopeExecution()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated("subProcess", "subProcess", null);
		testHelper.assertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("compensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationEventScopeExecutionAssertActivityInstance()
	  public virtual void testCanMigrateWithCompensationEventScopeExecutionAssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when
		testHelper.completeTask("userTask2");

		// then
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("subProcess").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationEventScopeExecutionAssertExecutionTree()
	  public virtual void testCanMigrateWithCompensationEventScopeExecutionAssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		Execution eventScopeExecution = rule.RuntimeService.createExecutionQuery().activityId("subProcess").singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("subProcess").scope().eventScope().id(eventScopeExecution.Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationEventScopeExecutionChangeIds()
	  public virtual void testCanMigrateWithCompensationEventScopeExecutionChangeIds()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).changeElementId("subProcess", "newSubProcess").changeElementId("userTask1", "newUserTask1").changeElementId("compensationBoundary", "newCompensationBoundary").changeElementId("compensationHandler", "newCompensationHandler"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "newSubProcess").mapActivities("compensationBoundary", "newCompensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated("subProcess", "newSubProcess", null);
		testHelper.assertEventSubscriptionMigrated("compensationHandler", "newCompensationHandler", null);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("newCompensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationEventScopeExecutionChangeIdsAssertActivityInstance()
	  public virtual void testCanMigrateWithCompensationEventScopeExecutionChangeIdsAssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).changeElementId("subProcess", "newSubProcess").changeElementId("userTask1", "newUserTask1").changeElementId("compensationBoundary", "newCompensationBoundary").changeElementId("compensationHandler", "newCompensationHandler"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "newSubProcess").mapActivities("compensationBoundary", "newCompensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when
		testHelper.completeTask("userTask2");

		// then
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("newSubProcess").activity("newCompensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithCompensationEventScopeExecutionChangeIdsAssertExecutionTree()
	  public virtual void testCanMigrateWithCompensationEventScopeExecutionChangeIdsAssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).changeElementId("subProcess", "newSubProcess").changeElementId("userTask1", "newUserTask1").changeElementId("compensationBoundary", "newCompensationBoundary").changeElementId("compensationHandler", "newCompensationHandler"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "newSubProcess").mapActivities("compensationBoundary", "newCompensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		Execution eventScopeExecution = rule.RuntimeService.createExecutionQuery().activityId("subProcess").singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("newSubProcess").scope().eventScope().id(eventScopeExecution.Id).done());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateEventScopeVariables()
	  public virtual void testCanMigrateEventScopeVariables()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Execution subProcessExecution = rule.RuntimeService.createExecutionQuery().activityId("userTask1").singleResult();
		rule.RuntimeService.setVariableLocal(subProcessExecution.Id, "foo", "bar");

		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("compensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithEventSubProcessHandler()
	  public virtual void testCanMigrateWithEventSubProcessHandler()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "subProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated("eventSubProcess", "eventSubProcess", null);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("eventSubProcessTask");
		testHelper.completeTask("compensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithEventSubProcessHandlerAssertActivityInstance()
	  public virtual void testCanMigrateWithEventSubProcessHandlerAssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "subProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("userTask1");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when compensation is triggered
		testHelper.completeTask("userTask2");

		// then
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("subProcess").beginScope("eventSubProcess").activity("eventSubProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithEventSubProcessHandlerAssertExecutionTree()
	  public virtual void testCanMigrateWithEventSubProcessHandlerAssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "subProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("userTask1");

		Execution eventScopeExecution = rule.RuntimeService.createExecutionQuery().activityId("subProcess").singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("subProcess").scope().eventScope().id(eventScopeExecution.Id).done());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithEventSubProcessHandlerChangeIds()
	  public virtual void testCanMigrateWithEventSubProcessHandlerChangeIds()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL).changeElementId("eventSubProcess", "newEventSubProcess"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").mapActivities("subProcess", "subProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated("eventSubProcess", "newEventSubProcess", null);

		// and the compensation can be triggered and completed
		testHelper.completeTask("userTask2");
		testHelper.completeTask("eventSubProcessTask");
		testHelper.completeTask("compensationHandler");

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateSiblingEventScopeExecutions()
	  public virtual void testCanMigrateSiblingEventScopeExecutions()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "outerSubProcess").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// starting a second instances of the sub process
		rule.RuntimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("subProcess").execute();

		IList<Execution> subProcessExecutions = rule.RuntimeService.createExecutionQuery().activityId("userTask1").list();
		foreach (Execution subProcessExecution in subProcessExecutions)
		{
		  // set the same variable to a distinct value
		  rule.RuntimeService.setVariableLocal(subProcessExecution.Id, "var", subProcessExecution.Id);
		}

		testHelper.completeAnyTask("userTask1");
		testHelper.completeAnyTask("userTask1");


		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the variable snapshots during compensation are not shared
		testHelper.completeAnyTask("userTask2");

		IList<Task> compensationTasks = rule.TaskService.createTaskQuery().taskDefinitionKey("compensationHandler").list();
		Assert.assertEquals(2, compensationTasks.Count);

		object value1 = rule.TaskService.getVariable(compensationTasks[0].Id, "var");
		object value2 = rule.TaskService.getVariable(compensationTasks[1].Id, "var");
		Assert.assertNotEquals(value1, value2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateWithoutCompensationStartEventCase1()
	  public virtual void testCannotMigrateWithoutCompensationStartEventCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures(sourceProcessDefinition.Id, "Cannot migrate subscription for compensation handler 'eventSubProcess'. " + "There is no migration instruction for the compensation start event");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateWithoutCompensationStartEventCase2()
	  public virtual void testCannotMigrateWithoutCompensationStartEventCase2()
	  {
		// given
		BpmnModelInstance model = modify(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL).removeFlowNode("compensationBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask2", "userTask2").build();

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasProcessInstanceId(testHelper.snapshotBeforeMigration.ProcessInstanceId).hasActivityInstanceFailures(sourceProcessDefinition.Id, "Cannot migrate subscription for compensation handler 'eventSubProcess'. " + "There is no migration instruction for the compensation start event");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEventScopeHierarchyPreservation()
	  public virtual void testEventScopeHierarchyPreservation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);

		try
		{
		  // when
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("outerSubProcess", "innerSubProcess").mapActivities("innerSubProcess", "outerSubProcess").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("innerSubProcess", "The closest mapped ancestor 'outerSubProcess' is mapped to scope 'innerSubProcess' " + "which is not an ancestor of target scope 'outerSubProcess'");
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompensationBoundaryHierarchyPreservation()
	  public virtual void testCompensationBoundaryHierarchyPreservation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).addSubProcessTo(ProcessModels.PROCESS_KEY).id("addedSubProcess").embeddedSubProcess().startEvent().endEvent().done());

		try
		{
		  // when
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "addedSubProcess").mapActivities("compensationBoundary", "compensationBoundary").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("compensationBoundary", "The closest mapped ancestor 'subProcess' is mapped to scope 'addedSubProcess' " + "which is not an ancestor of target scope 'compensationBoundary'");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMapCompensateStartEventWithoutMappingEventScopeCase1()
	  public virtual void testCannotMapCompensateStartEventWithoutMappingEventScopeCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

		try
		{
		  // when
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcessStart", "eventSubProcessStart").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("eventSubProcessStart", "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (subProcess)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMapCompensateStartEventWithoutMappingEventScopeCase2()
	  public virtual void testCannotMapCompensateStartEventWithoutMappingEventScopeCase2()
	  {
		// given
		BpmnModelInstance model = modify(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL).removeFlowNode("compensationBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		try
		{
		  // when
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcessStart", "eventSubProcessStart").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("eventSubProcessStart", "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (subProcess)");
		}
	  }
	}

}