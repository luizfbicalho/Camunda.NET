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
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using CompensationModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CompensationModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationCompensationAddSubProcessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationCompensationAddSubProcessTest()
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
//ORIGINAL LINE: @Test public void testCase1()
	  public virtual void testCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);

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

	  /// <summary>
	  /// The guarantee given by the API is: Compensation can be triggered in the scope that it could be triggered before
	  ///   migration. Thus, it should not be possible to trigger compensation from the new sub process instance but only from the
	  ///   parent scope, i.e. the process definition instance
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase1CannotTriggerCompensationInNewScope()
	  public virtual void testCase1CannotTriggerCompensationInNewScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL).endEventBuilder("subProcessEnd").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then compensation is only caught outside of the subProcess
		testHelper.completeTask("userTask2");

		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("subProcess").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase1AssertExecutionTree()
	  public virtual void testCase1AssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the execution tree is correct
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask2").scope().up().child("subProcess").scope().eventScope().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase2()
	  public virtual void testCase2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

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
//ORIGINAL LINE: @Test public void testCase2AssertExecutionTree()
	  public virtual void testCase2AssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("subProcess").scope().eventScope().done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase2AssertActivityInstance()
	  public virtual void testCase2AssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when throwing compensation
		testHelper.completeTask("userTask2");

		// then the activity instance tree is correct
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("subProcess").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoListenersCalled()
	  public virtual void testNoListenersCalled()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).activityBuilder("subProcess").camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, "${execution.setVariable('foo', 'bar')}").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(0, testHelper.snapshotAfterMigration.getVariables().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore("CAM-6035") @Test public void testNoInputMappingExecuted()
	  public virtual void testNoInputMappingExecuted()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).activityBuilder("subProcess").camundaInputParameter("foo", "bar").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(0, testHelper.snapshotAfterMigration.getVariables().Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariablesInParentEventScopeStillAccessible()
	  public virtual void testVariablesInParentEventScopeStillAccessible()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "outerSubProcess").mapActivities("compensationBoundary", "compensationBoundary").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Execution subProcessExecution = rule.RuntimeService.createExecutionQuery().activityId("userTask1").singleResult();
		rule.RuntimeService.setVariableLocal(subProcessExecution.Id, "foo", "bar");

		testHelper.completeTask("userTask1");

		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when throwing compensation
		testHelper.completeAnyTask("userTask2");

		// then the variable snapshot is available
		Task compensationTask = rule.TaskService.createTaskQuery().singleResult();
		Assert.assertEquals("bar", rule.TaskService.getVariable(compensationTask.Id, "foo"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotAddScopeOnTopOfEventSubProcess()
	  public virtual void testCannotAddScopeOnTopOfEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.DOUBLE_SUBPROCESS_MODEL).addSubProcessTo("innerSubProcess").id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent("eventSubProcessStart").compensateEventDefinition().compensateEventDefinitionDone().endEvent().done());


		try
		{
		  // when
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "outerSubProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").mapActivities("compensationBoundary", "compensationBoundary").mapActivities("userTask2", "userTask2").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("eventSubProcessStart", "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (innerSubProcess)");
		}
	  }

	}

}