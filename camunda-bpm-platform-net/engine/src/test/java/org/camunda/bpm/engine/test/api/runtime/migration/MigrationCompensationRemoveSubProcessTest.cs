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

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using CompensationModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CompensationModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationCompensationRemoveSubProcessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationCompensationRemoveSubProcessTest()
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
//ORIGINAL LINE: @Before @After public void clearExecutionListener()
	  public virtual void clearExecutionListener()
	  {
		RecorderExecutionListener.clear();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase1()
	  public virtual void testCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);
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
//ORIGINAL LINE: @Test public void testCase1AssertActivityInstance()
	  public virtual void testCase1AssertActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when
		testHelper.completeTask("userTask2");

		// then
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase1AssertExecutionTree()
	  public virtual void testCase1AssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);
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
//ORIGINAL LINE: @Test public void testCase2()
	  public virtual void testCase2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
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
//ORIGINAL LINE: @Test public void testCase2ActivityInstance()
	  public virtual void testCase2ActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when
		testHelper.completeTask("userTask2");

		// then
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCase2AssertExecutionTree()
	  public virtual void testCase2AssertExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
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
//ORIGINAL LINE: @Test public void testCanOnlyTriggerCompensationInParentOfRemovedScope()
	  public virtual void testCanOnlyTriggerCompensationInParentOfRemovedScope()
	  {

		BpmnModelInstance sourceModel = ProcessModels.newModel().startEvent().subProcess("outerSubProcess").embeddedSubProcess().startEvent().userTask("userTask1").boundaryEvent("compensationBoundary").compensateEventDefinition().compensateEventDefinitionDone().moveToActivity("userTask1").subProcess("innerSubProcess").embeddedSubProcess().startEvent().userTask("userTask2").endEvent().subProcessDone().endEvent().subProcessDone().done();
		CompensationModels.addUserTaskCompensationHandler(sourceModel, "compensationBoundary", "compensationHandler");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL).endEventBuilder("subProcessEnd").compensateEventDefinition().waitForCompletion(true).compensateEventDefinitionDone().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("innerSubProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when
		testHelper.completeTask("userTask2");

		// then compensation is not triggered from inside the inner sub process
		// but only on process definition level
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);

		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("compensationEvent").beginScope("subProcess").activity("compensationHandler").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanRemoveEventScopeWithVariables()
	  public virtual void testCanRemoveEventScopeWithVariables()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Execution subProcessExecution = rule.RuntimeService.createExecutionQuery().activityId("userTask1").singleResult();
		rule.RuntimeService.setVariableLocal(subProcessExecution.Id, "foo", "bar");

		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeletesOnlyVariablesFromRemovingScope()
	  public virtual void testDeletesOnlyVariablesFromRemovingScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("innerSubProcess", "subProcess").mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Execution innerSubProcessExecution = rule.RuntimeService.createExecutionQuery().activityId("userTask1").singleResult();

		string outerSubProcessExecutionId = ((ExecutionEntity) innerSubProcessExecution).ParentId;

		rule.RuntimeService.setVariableLocal(outerSubProcessExecutionId, "outerVariable", "outerValue");
		rule.RuntimeService.setVariableLocal(innerSubProcessExecution.Id, "innerVariable", "innerValue");

		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);

		VariableInstance migratedVariable = testHelper.snapshotAfterMigration.getSingleVariable("innerVariable");
		Assert.assertNotNull(migratedVariable);
		Assert.assertEquals("innerValue", migratedVariable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoListenersCalled()
	  public virtual void testNoListenersCalled()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		// the listener was only called once when the sub process completed properly
		Assert.assertEquals(1, RecorderExecutionListener.RecordedEvents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoOutputMappingExecuted()
	  public virtual void testNoOutputMappingExecuted()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL).activityBuilder("subProcess").camundaOutputParameter("foo", "${bar}").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		rule.RuntimeService.setVariable(processInstance.Id, "bar", "value1");
		testHelper.completeTask("userTask1"); // => sets "foo" to "value1"

		rule.RuntimeService.setVariable(processInstance.Id, "bar", "value2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then "foo" has not been set to "value2"
		Assert.assertEquals(2, testHelper.snapshotAfterMigration.getVariables().Count); // "foo" and "bar"
		VariableInstance variableInstance = testHelper.snapshotAfterMigration.getSingleVariable("foo");
		Assert.assertEquals("value1", variableInstance.Value);
	  }
	}

}