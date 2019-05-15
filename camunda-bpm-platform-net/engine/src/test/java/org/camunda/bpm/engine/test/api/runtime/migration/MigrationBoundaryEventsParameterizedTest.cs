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


	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using BpmnEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.BpmnEventFactory;
	using ConditionalEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.ConditionalEventFactory;
	using MessageEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.MessageEventFactory;
	using MigratingBpmnEventTrigger = org.camunda.bpm.engine.test.api.runtime.migration.util.MigratingBpmnEventTrigger;
	using SignalEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.SignalEventFactory;
	using TimerEventFactory = org.camunda.bpm.engine.test.api.runtime.migration.util.TimerEventFactory;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrationBoundaryEventsParameterizedTest
	public class MigrationBoundaryEventsParameterizedTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationBoundaryEventsParameterizedTest()
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


	  public const string AFTER_BOUNDARY_TASK = "afterBoundary";
	  public const string MESSAGE_NAME = "Message";
	  public const string SIGNAL_NAME = "Signal";
	  public const string TIMER_DATE = "2016-02-11T12:13:14Z";
	  public const string NEW_TIMER_DATE = "2018-02-11T12:13:14Z";
	  protected internal const string BOUNDARY_ID = "boundary";
	  protected internal const string MIGRATE_MESSAGE_BOUNDARY_EVENT = "MigrateMessageBoundaryEvent";
	  protected internal const string MIGRATE_SIGNAL_BOUNDARY_EVENT = "MigrateSignalBoundaryEvent";
	  protected internal const string MIGRATE_TIMER_BOUNDARY_EVENT = "MigrateTimerBoundaryEvent";
	  protected internal const string MIGRATE_CONDITIONAL_BOUNDARY_EVENT = "MigrateConditionalBoundaryEvent";
	  protected internal const string USER_TASK_ID = "userTask";
	  protected internal const string NEW_BOUNDARY_ID = "newBoundary";
	  public const string USER_TASK_1_ID = "userTask1";
	  public const string USER_TASK_2_ID = "userTask2";
	  public const string SUB_PROCESS_ID = "subProcess";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		  return Arrays.asList(new object[][]
		  {
			  new object[]{new TimerEventFactory()},
			  new object[]{new MessageEventFactory()},
			  new object[]{new SignalEventFactory()},
			  new object[]{new ConditionalEventFactory()}
		  });
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.runtime.migration.util.BpmnEventFactory eventFactory;
	  public BpmnEventFactory eventFactory;


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

	  // tests ////////////////////////////////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventOnUserTask()
	  public virtual void testMigrateBoundaryEventOnUserTask()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, USER_TASK_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventOnUserTaskAndTriggerEvent()
	  public virtual void testMigrateBoundaryEventOnUserTaskAndTriggerEvent()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, USER_TASK_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger boundary event and successfully complete the migrated instance
		eventTrigger.inContextOf(NEW_BOUNDARY_ID).trigger(processInstance.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventOnConcurrentUserTask()
	  public virtual void testMigrateBoundaryEventOnConcurrentUserTask()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_GATEWAY_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, USER_TASK_1_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("boundary", "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);


		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_1_ID, USER_TASK_1_ID).mapActivities(USER_TASK_2_ID, USER_TASK_2_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_1_ID);
		testHelper.completeTask(USER_TASK_2_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventOnConcurrentUserTaskAndTriggerEvent()
	  public virtual void testMigrateBoundaryEventOnConcurrentUserTaskAndTriggerEvent()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_GATEWAY_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, USER_TASK_1_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("boundary", "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);


		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_1_ID, USER_TASK_1_ID).mapActivities(USER_TASK_2_ID, USER_TASK_2_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger the event and successfully complete the migrated instance
		eventTrigger.inContextOf(NEW_BOUNDARY_ID).trigger(processInstance.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask(USER_TASK_2_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventOnConcurrentScopeUserTask()
	  public virtual void testMigrateBoundaryEventOnConcurrentScopeUserTask()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_SCOPE_TASKS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, USER_TASK_1_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("boundary", "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);


		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_1_ID, USER_TASK_1_ID).mapActivities(USER_TASK_2_ID, USER_TASK_2_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_1_ID);
		testHelper.completeTask(USER_TASK_2_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventOnConcurrentScopeUserTaskAndTriggerEvent()
	  public virtual void testMigrateBoundaryEventOnConcurrentScopeUserTaskAndTriggerEvent()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_SCOPE_TASKS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, USER_TASK_1_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("boundary", "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);


		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_1_ID, USER_TASK_1_ID).mapActivities(USER_TASK_2_ID, USER_TASK_2_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger the event and successfully complete the migrated instance
		eventTrigger.inContextOf(NEW_BOUNDARY_ID).trigger(processInstance.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask(USER_TASK_2_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventToSubProcess()
	  public virtual void testMigrateBoundaryEventToSubProcess()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, SUB_PROCESS_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventToSubProcessAndTriggerEvent()
	  public virtual void testMigrateBoundaryEventToSubProcessAndTriggerEvent()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.SUBPROCESS_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, SUB_PROCESS_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger the event and successfully complete the migrated instance
		eventTrigger.inContextOf(NEW_BOUNDARY_ID).trigger(processInstance.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventToSubProcessWithScopeUserTask()
	  public virtual void testMigrateBoundaryEventToSubProcessWithScopeUserTask()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, SUB_PROCESS_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventToSubProcessWithScopeUserTaskAndTriggerEvent()
	  public virtual void testMigrateBoundaryEventToSubProcessWithScopeUserTaskAndTriggerEvent()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, SUB_PROCESS_ID, BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger the event and successfully complete the migrated instance
		eventTrigger.inContextOf(NEW_BOUNDARY_ID).trigger(processInstance.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventToParallelSubProcess()
	  public virtual void testMigrateBoundaryEventToParallelSubProcess()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_SUBPROCESS_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, "subProcess1", BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities(USER_TASK_1_ID, USER_TASK_1_ID).mapActivities("subProcess2", "subProcess2").mapActivities(USER_TASK_2_ID, USER_TASK_2_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		eventTrigger.assertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_1_ID);
		testHelper.completeTask(USER_TASK_2_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventToParallelSubProcessAndTriggerEvent()
	  public virtual void testMigrateBoundaryEventToParallelSubProcessAndTriggerEvent()
	  {
		// given
		BpmnModelInstance sourceProcess = ProcessModels.PARALLEL_SUBPROCESS_PROCESS.clone();
		MigratingBpmnEventTrigger eventTrigger = eventFactory.addBoundaryEvent(rule.ProcessEngine, sourceProcess, "subProcess1", BOUNDARY_ID);
		ModifiableBpmnModelInstance.wrap(sourceProcess).flowNodeBuilder(BOUNDARY_ID).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities(USER_TASK_1_ID, USER_TASK_1_ID).mapActivities("subProcess2", "subProcess2").mapActivities(USER_TASK_2_ID, USER_TASK_2_ID).mapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID).updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to trigger the event and successfully complete the migrated instance
		eventTrigger.inContextOf(NEW_BOUNDARY_ID).trigger(processInstance.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask(USER_TASK_2_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

	}

}