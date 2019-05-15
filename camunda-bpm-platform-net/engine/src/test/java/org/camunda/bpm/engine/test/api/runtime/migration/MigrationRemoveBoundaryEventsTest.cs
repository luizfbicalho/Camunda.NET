using System;

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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MigrationRemoveBoundaryEventsTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationRemoveBoundaryEventsTest()
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
	  public const string ERROR_CODE = "Error";
	  public const string ESCALATION_CODE = "Escalation";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromUserTask()
	  public virtual void testRemoveMessageBoundaryEventFromUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromScopeUserTask()
	  public virtual void testRemoveMessageBoundaryEventFromScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromConcurrentUserTask()
	  public virtual void testRemoveMessageBoundaryEventFromConcurrentUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromConcurrentScopeUserTask()
	  public virtual void testRemoveMessageBoundaryEventFromConcurrentScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromSubProcess()
	  public virtual void testRemoveMessageBoundaryEventFromSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromSubProcessWithScopeUserTask()
	  public virtual void testRemoveMessageBoundaryEventFromSubProcessWithScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromParallelSubProcess()
	  public virtual void testRemoveMessageBoundaryEventFromParallelSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMessageBoundaryEventFromUserTaskInSubProcess()
	  public virtual void testRemoveMessageBoundaryEventFromUserTaskInSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromUserTask()
	  public virtual void testRemoveSignalBoundaryEventFromUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromScopeUserTask()
	  public virtual void testRemoveSignalBoundaryEventFromScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromConcurrentUserTask()
	  public virtual void testRemoveSignalBoundaryEventFromConcurrentUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromConcurrentScopeUserTask()
	  public virtual void testRemoveSignalBoundaryEventFromConcurrentScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromSubProcess()
	  public virtual void testRemoveSignalBoundaryEventFromSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromSubProcessWithScopeUserTask()
	  public virtual void testRemoveSignalBoundaryEventFromSubProcessWithScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromParallelSubProcess()
	  public virtual void testRemoveSignalBoundaryEventFromParallelSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSignalBoundaryEventFromUserTaskInSubProcess()
	  public virtual void testRemoveSignalBoundaryEventFromUserTaskInSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromUserTask()
	  public virtual void testRemoveTimerBoundaryEventFromUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromScopeUserTask()
	  public virtual void testRemoveTimerBoundaryEventFromScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromConcurrentUserTask()
	  public virtual void testRemoveTimerBoundaryEventFromConcurrentUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromConcurrentScopeUserTask()
	  public virtual void testRemoveTimerBoundaryEventFromConcurrentScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromSubProcess()
	  public virtual void testRemoveTimerBoundaryEventFromSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromSubProcessWithScopeUserTask()
	  public virtual void testRemoveTimerBoundaryEventFromSubProcessWithScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromParallelSubProcess()
	  public virtual void testRemoveTimerBoundaryEventFromParallelSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTimerBoundaryEventFromUserTaskInSubProcess()
	  public virtual void testRemoveTimerBoundaryEventFromUserTaskInSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobRemoved("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMultipleBoundaryEvents()
	  public virtual void testRemoveMultipleBoundaryEvents()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("timerBoundary").timerWithDate(TIMER_DATE).moveToActivity("userTask").boundaryEvent("messageBoundary").message(MESSAGE_NAME).moveToActivity("userTask").boundaryEvent("signalBoundary").signal(SIGNAL_NAME).done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("messageBoundary", MESSAGE_NAME);
		testHelper.assertEventSubscriptionRemoved("signalBoundary", SIGNAL_NAME);
		testHelper.assertBoundaryTimerJobRemoved("timerBoundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveErrorBoundaryEventFromSubProcess()
	  public virtual void testRemoveErrorBoundaryEventFromSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent().error(ERROR_CODE).done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveEscalationBoundaryEventFromSubProcess()
	  public virtual void testRemoveEscalationBoundaryEventFromSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent().escalation(ESCALATION_CODE).done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveIncidentForJob()
	  public virtual void testRemoveIncidentForJob()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).userTaskBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).serviceTask("failingTask").camundaClass("org.camunda.bpm.engine.test.api.runtime.FailingDelegate").endEvent().done();
		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId("userTask", "newUserTask").changeElementId("boundary", "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// a timer job exists
		Job jobBeforeMigration = rule.ManagementService.createJobQuery().singleResult();
		assertNotNull(jobBeforeMigration);

		// if the timer job is triggered the failing delegate fails and an incident is created
		executeJob(jobBeforeMigration);
		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();
		assertEquals("boundary", incidentBeforeMigration.ActivityId);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "newUserTask").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the incident was removed
		Job jobAfterMigration = rule.ManagementService.createJobQuery().jobId(jobBeforeMigration.Id).singleResult();
		assertNull(jobAfterMigration);

		assertEquals(0, rule.RuntimeService.createIncidentQuery().count());
	  }

	  protected internal virtual void executeJob(Job job)
	  {
		ManagementService managementService = rule.ManagementService;

		while (job != null && job.Retries > 0)
		{
		  try
		  {
			managementService.executeJob(job.Id);
		  }
		  catch (Exception)
		  {
			// ignore
		  }

		  job = managementService.createJobQuery().jobId(job.Id).singleResult();
		}
	  }

	}

}