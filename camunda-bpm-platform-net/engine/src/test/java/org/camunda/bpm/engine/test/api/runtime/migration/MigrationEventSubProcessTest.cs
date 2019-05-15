using System;
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
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using EventSubProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using DateTime = org.joda.time.DateTime;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.migration.validation.instruction.ConditionalEventUpdateEventTriggerValidator.MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels.CONDITIONAL_EVENT_SUBPROCESS_PROCESS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	public class MigrationEventSubProcessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationEventSubProcessTest()
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


	  public const string SIGNAL_NAME = "Signal";
	  protected internal const string EVENT_SUB_PROCESS_START_ID = "eventSubProcessStart";
	  protected internal const string EVENT_SUB_PROCESS_TASK_ID = "eventSubProcessTask";
	  protected internal const string USER_TASK_ID = "userTask";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException exceptionRule = org.junit.rules.ExpectedException.none();
	  public ExpectedException exceptionRule = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveEventSubProcess()
	  public virtual void testMigrateActiveEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity(EVENT_SUB_PROCESS_TASK_ID).execute();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "eventSubProcess").mapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(processInstance.Id).child(EVENT_SUB_PROCESS_TASK_ID).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("eventSubProcess")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("eventSubProcess", testHelper.getSingleActivityInstanceBeforeMigration("eventSubProcess").Id).activity(EVENT_SUB_PROCESS_TASK_ID, testHelper.getSingleActivityInstanceBeforeMigration(EVENT_SUB_PROCESS_TASK_ID).Id).done());

		testHelper.assertEventSubscriptionRemoved(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);
		testHelper.assertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

		// and it is possible to complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveEventSubProcessToEmbeddedSubProcess()
	  public virtual void testMigrateActiveEventSubProcessToEmbeddedSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity(EVENT_SUB_PROCESS_TASK_ID).execute();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "subProcess").mapActivities(EVENT_SUB_PROCESS_TASK_ID, USER_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(processInstance.Id).child(USER_TASK_ID).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("eventSubProcess")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess", testHelper.getSingleActivityInstanceBeforeMigration("eventSubProcess").Id).activity(USER_TASK_ID, testHelper.getSingleActivityInstanceBeforeMigration(EVENT_SUB_PROCESS_TASK_ID).Id).done());

		testHelper.assertEventSubscriptionRemoved(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);
		Assert.assertEquals(0, testHelper.snapshotAfterMigration.EventSubscriptions.Count);

		// and it is possible to complete the process instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveEmbeddedSubProcessToEventSubProcess()
	  public virtual void testMigrateActiveEmbeddedSubProcessToEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "eventSubProcess").mapActivities(USER_TASK_ID, EVENT_SUB_PROCESS_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(processInstance.Id).child(EVENT_SUB_PROCESS_TASK_ID).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("subProcess")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("eventSubProcess", testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id).activity(EVENT_SUB_PROCESS_TASK_ID, testHelper.getSingleActivityInstanceBeforeMigration(USER_TASK_ID).Id).done());

		testHelper.assertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

		// and it is possible to complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveErrorEventSubProcess()
	  public virtual void testMigrateActiveErrorEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.ERROR_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.ERROR_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity(EVENT_SUB_PROCESS_TASK_ID).execute();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "eventSubProcess").mapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveCompensationEventSubProcess()
	  public virtual void testMigrateActiveCompensationEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity(EVENT_SUB_PROCESS_TASK_ID).execute();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "eventSubProcess").mapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateActiveEscalationEventSubProcess()
	  public virtual void testMigrateActiveEscalationEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.ESCALATION_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.ESCALATION_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity(EVENT_SUB_PROCESS_TASK_ID).execute();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "eventSubProcess").mapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateTaskAddEventSubProcess()
	  public virtual void testMigrateTaskAddEventSubProcess()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, EVENT_SUB_PROCESS_TASK_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(EVENT_SUB_PROCESS_TASK_ID).scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("eventSubProcess").activity(EVENT_SUB_PROCESS_TASK_ID, testHelper.getSingleActivityInstanceBeforeMigration(USER_TASK_ID).Id).done());

		testHelper.assertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

		// and it is possible to complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubprocessMessageKeepTrigger()
	  public virtual void testMigrateEventSubprocessMessageKeepTrigger()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

		// and it is possible to trigger the event subprocess
		rule.RuntimeService.correlateMessage(EventSubProcessModels.MESSAGE_NAME);
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());

		// and complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubprocessTimerKeepTrigger()
	  public virtual void testMigrateEventSubprocessTimerKeepTrigger()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, TimerStartEventSubprocessJobHandler.TYPE);

		// and it is possible to trigger the event subprocess
		Job timerJob = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(timerJob.Id);
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());

		// and complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubprocessSignalKeepTrigger()
	  public virtual void testMigrateEventSubprocessSignalKeepTrigger()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.SIGNAL_NAME);

		// and it is possible to trigger the event subprocess
		rule.RuntimeService.signalEventReceived(EventSubProcessModels.SIGNAL_NAME);
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());

		// and complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateConditionalBoundaryEventKeepTrigger()
	  public virtual void testMigrateConditionalBoundaryEventKeepTrigger()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CONDITIONAL_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CONDITIONAL_EVENT_SUBPROCESS_PROCESS);

		// expected migration validation exception
		exceptionRule.expect(typeof(MigrationPlanValidationException));
		exceptionRule.expectMessage(MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG);

		// when conditional event sub process is migrated without update event trigger
		rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubprocessChangeStartEventType()
	  public virtual void testMigrateEventSubprocessChangeStartEventType()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

		try
		{
		  // when
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures(EVENT_SUB_PROCESS_START_ID, "Events are not of the same type (signalStartEvent != startTimerEvent)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubprocessTimerIncident()
	  public virtual void testMigrateEventSubprocessTimerIncident()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Job timerTriggerJob = rule.ManagementService.createJobQuery().singleResult();
		// create an incident
		rule.ManagementService.setJobRetries(timerTriggerJob.Id, 0);
		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Incident incidentAfterMigration = rule.RuntimeService.createIncidentQuery().singleResult();
		assertNotNull(incidentAfterMigration);

		assertEquals(incidentBeforeMigration.Id, incidentAfterMigration.Id);
		assertEquals(timerTriggerJob.Id, incidentAfterMigration.Configuration);

		assertEquals(EVENT_SUB_PROCESS_START_ID, incidentAfterMigration.ActivityId);
		assertEquals(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);

		// and it is possible to complete the process
		rule.ManagementService.executeJob(timerTriggerJob.Id);
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateNonInterruptingEventSubprocessMessageTrigger()
	  public virtual void testMigrateNonInterruptingEventSubprocessMessageTrigger()
	  {
		BpmnModelInstance nonInterruptingModel = modify(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS).startEventBuilder(EVENT_SUB_PROCESS_START_ID).interrupting(false).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(nonInterruptingModel);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(nonInterruptingModel);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

		// and it is possible to trigger the event subprocess
		rule.RuntimeService.correlateMessage(EventSubProcessModels.MESSAGE_NAME);
		Assert.assertEquals(2, rule.TaskService.createTaskQuery().count());

		// and complete the process instance
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventMessage()
	  public virtual void testUpdateEventMessage()
	  {
		// given
		BpmnModelInstance sourceProcess = EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS).renameMessage(EventSubProcessModels.MESSAGE_NAME, "new" + EventSubProcessModels.MESSAGE_NAME);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME, EVENT_SUB_PROCESS_START_ID, "new" + EventSubProcessModels.MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.correlateMessage("new" + EventSubProcessModels.MESSAGE_NAME);
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventSignal()
	  public virtual void testUpdateEventSignal()
	  {
		// given
		BpmnModelInstance sourceProcess = EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS).renameSignal(EventSubProcessModels.SIGNAL_NAME, "new" + EventSubProcessModels.SIGNAL_NAME);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.SIGNAL_NAME, EVENT_SUB_PROCESS_START_ID, "new" + EventSubProcessModels.SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.signalEventReceived("new" + EventSubProcessModels.SIGNAL_NAME);
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventTimer()
	  public virtual void testUpdateEventTimer()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();

		BpmnModelInstance sourceProcess = EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS).removeChildren(EVENT_SUB_PROCESS_START_ID).startEventBuilder(EVENT_SUB_PROCESS_START_ID).timerWithDuration("PT50M").done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		DateTime newDueDate = (new DateTime(ClockUtil.CurrentTime)).plusMinutes(50).toDate();
		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], EVENT_SUB_PROCESS_START_ID, newDueDate);

		// and it is possible to successfully complete the migrated instance
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventMessageWithExpression()
	  public virtual void testUpdateEventMessageWithExpression()
	  {

		// given
		string newMessageNameWithExpression = "new" + EventSubProcessModels.MESSAGE_NAME + "-${var}";
		BpmnModelInstance sourceProcess = EventSubProcessModels.MESSAGE_INTERMEDIATE_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(EventSubProcessModels.MESSAGE_INTERMEDIATE_EVENT_SUBPROCESS_PROCESS).renameMessage(EventSubProcessModels.MESSAGE_NAME, newMessageNameWithExpression);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities("eventSubProcess", "eventSubProcess").mapActivities("catchMessage", "catchMessage").updateEventTrigger().build();
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "foo";

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan, variables);

		// then
		string resolvedMessageName = "new" + EventSubProcessModels.MESSAGE_NAME + "-foo";
		testHelper.assertEventSubscriptionMigrated("catchMessage", EventSubProcessModels.MESSAGE_NAME, "catchMessage", resolvedMessageName);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.correlateMessage(resolvedMessageName);
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventSignalWithExpression()
	  public virtual void testUpdateEventSignalWithExpression()
	  {
		// given
		string newSignalNameWithExpression = "new" + EventSubProcessModels.MESSAGE_NAME + "-${var}";
		BpmnModelInstance sourceProcess = EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS).renameSignal(EventSubProcessModels.SIGNAL_NAME, newSignalNameWithExpression);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "foo";

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan, variables);

		// then
		string resolvedsignalName = "new" + EventSubProcessModels.MESSAGE_NAME + "-foo";
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.SIGNAL_NAME, EVENT_SUB_PROCESS_START_ID, resolvedsignalName);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.signalEventReceived(resolvedsignalName);
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateConditionalEventExpression()
	  public virtual void testUpdateConditionalEventExpression()
	  {
		// given
		BpmnModelInstance sourceProcess = EventSubProcessModels.FALSE_CONDITIONAL_EVENT_SUBPROCESS_PROCESS;
		BpmnModelInstance targetProcess = modify(CONDITIONAL_EVENT_SUBPROCESS_PROCESS);

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);


		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID).updateEventTrigger().build();

		// when process is migrated without update event trigger
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then condition is migrated and has new condition expr
		testHelper.assertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, null);

		// and it is possible to successfully complete the migrated instance
		testHelper.AnyVariable = testHelper.snapshotAfterMigration.ProcessInstanceId;
		testHelper.completeTask(EVENT_SUB_PROCESS_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }
	}

}