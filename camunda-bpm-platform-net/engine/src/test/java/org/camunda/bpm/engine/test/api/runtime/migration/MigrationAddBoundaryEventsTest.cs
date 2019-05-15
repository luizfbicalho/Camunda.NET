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
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MigrationAddBoundaryEventsTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationAddBoundaryEventsTest()
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
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToUserTask()
	  public virtual void testAddMessageBoundaryEventToUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToUserTaskAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToUserTaskAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToScopeUserTask()
	  public virtual void testAddMessageBoundaryEventToScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToScopeUserTaskAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToScopeUserTaskAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToConcurrentUserTask()
	  public virtual void testAddMessageBoundaryEventToConcurrentUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToConcurrentUserTaskAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToConcurrentUserTaskAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToConcurrentScopeUserTask()
	  public virtual void testAddMessageBoundaryEventToConcurrentScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToConcurrentScopeUserTaskAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToConcurrentScopeUserTaskAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToSubProcess()
	  public virtual void testAddMessageBoundaryEventToSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToSubProcessAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToSubProcessAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToSubProcessWithScopeUserTask()
	  public virtual void testAddMessageBoundaryEventToSubProcessWithScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToParallelSubProcess()
	  public virtual void testAddMessageBoundaryEventToParallelSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMessageBoundaryEventToParallelSubProcessAndCorrelateMessage()
	  public virtual void testAddMessageBoundaryEventToParallelSubProcessAndCorrelateMessage()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the message and successfully complete the migrated instance
		testHelper.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToUserTask()
	  public virtual void testAddSignalBoundaryEventToUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToUserTaskAndSendSignal()
	  public virtual void testAddSignalBoundaryEventToUserTaskAndSendSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToScopeUserTask()
	  public virtual void testAddSignalBoundaryEventToScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToScopeUserTaskAndSendSignal()
	  public virtual void testAddSignalBoundaryEventToScopeUserTaskAndSendSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToConcurrentUserTask()
	  public virtual void testAddSignalBoundaryEventToConcurrentUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToConcurrentUserTaskAndSendSignal()
	  public virtual void testAddSignalBoundaryEventToConcurrentUserTaskAndSendSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToConcurrentScopeUserTask()
	  public virtual void testAddSignalBoundaryEventToConcurrentScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToConcurrentScopeUserTaskAndSendSignal()
	  public virtual void testAddSignalBoundaryEventToConcurrentScopeUserTaskAndSendSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToSubProcess()
	  public virtual void testAddSignalBoundaryEventToSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToSubProcessAndCorrelateSignal()
	  public virtual void testAddSignalBoundaryEventToSubProcessAndCorrelateSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToSubProcessWithScopeUserTask()
	  public virtual void testAddSignalBoundaryEventToSubProcessWithScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateSignal()
	  public virtual void testAddSignalBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToParallelSubProcess()
	  public virtual void testAddSignalBoundaryEventToParallelSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("boundary", SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddSignalBoundaryEventToParallelSubProcessAndCorrelateSignal()
	  public virtual void testAddSignalBoundaryEventToParallelSubProcessAndCorrelateSignal()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the signal and successfully complete the migrated instance
		testHelper.sendSignal(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToUserTask()
	  public virtual void testAddTimerBoundaryEventToUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToUserTaskAndSendTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToUserTaskAndSendTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToScopeUserTask()
	  public virtual void testAddTimerBoundaryEventToScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToScopeUserTaskAndSendTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToScopeUserTaskAndSendTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToConcurrentUserTask()
	  public virtual void testAddTimerBoundaryEventToConcurrentUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToConcurrentUserTaskAndSendTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToConcurrentUserTaskAndSendTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToConcurrentScopeUserTask()
	  public virtual void testAddTimerBoundaryEventToConcurrentScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToConcurrentScopeUserTaskAndSendTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToConcurrentScopeUserTaskAndSendTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SCOPE_TASKS).activityBuilder("userTask1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to send the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToSubProcess()
	  public virtual void testAddTimerBoundaryEventToSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToSubProcessAndCorrelateTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToSubProcessAndCorrelateTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToSubProcessWithScopeUserTask()
	  public virtual void testAddTimerBoundaryEventToSubProcessWithScopeUserTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToParallelSubProcess()
	  public virtual void testAddTimerBoundaryEventToParallelSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertBoundaryTimerJobCreated("boundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTimerBoundaryEventToParallelSubProcessAndCorrelateTimerWithDate()
	  public virtual void testAddTimerBoundaryEventToParallelSubProcessAndCorrelateTimerWithDate()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_SUBPROCESS_PROCESS).activityBuilder("subProcess1").boundaryEvent("boundary").timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess2", "subProcess2").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to correlate the timer and successfully complete the migrated instance
		testHelper.triggerTimer();
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddMultipleBoundaryEvents()
	  public virtual void testAddMultipleBoundaryEvents()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("timerBoundary").timerWithDate(TIMER_DATE).moveToActivity("userTask").boundaryEvent("messageBoundary").message(MESSAGE_NAME).moveToActivity("userTask").boundaryEvent("signalBoundary").signal(SIGNAL_NAME).done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("messageBoundary", MESSAGE_NAME);
		testHelper.assertEventSubscriptionCreated("signalBoundary", SIGNAL_NAME);
		testHelper.assertBoundaryTimerJobCreated("timerBoundary");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddErrorBoundaryEventToSubProcessAndThrowError()
	  public virtual void testAddErrorBoundaryEventToSubProcessAndThrowError()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).endEventBuilder("subProcessEnd").error(ERROR_CODE).moveToActivity("subProcess").boundaryEvent().error(ERROR_CODE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddEscalationBoundaryEventToSubProcessAndThrowEscalation()
	  public virtual void testAddEscalationBoundaryEventToSubProcessAndThrowEscalation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).endEventBuilder("subProcessEnd").escalation(ESCALATION_CODE).moveToActivity("subProcess").boundaryEvent().escalation(ESCALATION_CODE).userTask(AFTER_BOUNDARY_TASK).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

	}

}