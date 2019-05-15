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
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using DateTime = org.joda.time.DateTime;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.migration.validation.instruction.ConditionalEventUpdateEventTriggerValidator.MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	public class MigrationBoundaryEventsTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationBoundaryEventsTest()
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
	  protected internal const string FALSE_CONDITION = "${false}";
	  protected internal const string VAR_CONDITION = "${any=='any'}";
	  protected internal const string BOUNDARY_ID = "boundary";
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
//ORIGINAL LINE: @Test public void testMigrateMultipleBoundaryEvents()
	  public virtual void testMigrateMultipleBoundaryEvents()
	  {
		// given
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").boundaryEvent("timerBoundary1").timerWithDate(TIMER_DATE).moveToActivity("subProcess").boundaryEvent("messageBoundary1").message(MESSAGE_NAME).moveToActivity("subProcess").boundaryEvent("signalBoundary1").signal(SIGNAL_NAME).moveToActivity("subProcess").boundaryEvent("conditionalBoundary1").condition(VAR_CONDITION).moveToActivity(USER_TASK_ID).boundaryEvent("timerBoundary2").timerWithDate(TIMER_DATE).moveToActivity(USER_TASK_ID).boundaryEvent("messageBoundary2").message(MESSAGE_NAME).moveToActivity(USER_TASK_ID).boundaryEvent("signalBoundary2").signal(SIGNAL_NAME).moveToActivity(USER_TASK_ID).boundaryEvent("conditionalBoundary2").condition(VAR_CONDITION).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("timerBoundary1", "timerBoundary1").mapActivities("signalBoundary1", "signalBoundary1").mapActivities("conditionalBoundary1", "conditionalBoundary1").updateEventTrigger().mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities("messageBoundary2", "messageBoundary2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("messageBoundary1", MESSAGE_NAME);
		testHelper.assertEventSubscriptionRemoved("signalBoundary2", SIGNAL_NAME);
		testHelper.assertEventSubscriptionRemoved("conditionalBoundary2", null);
		testHelper.assertEventSubscriptionMigrated("signalBoundary1", "signalBoundary1", SIGNAL_NAME);
		testHelper.assertEventSubscriptionMigrated("messageBoundary2", "messageBoundary2", MESSAGE_NAME);
		testHelper.assertEventSubscriptionMigrated("conditionalBoundary1", "conditionalBoundary1", null);
		testHelper.assertEventSubscriptionCreated("messageBoundary1", MESSAGE_NAME);
		testHelper.assertEventSubscriptionCreated("signalBoundary2", SIGNAL_NAME);
		testHelper.assertEventSubscriptionCreated("conditionalBoundary2", null);
		testHelper.assertBoundaryTimerJobRemoved("timerBoundary2");
		testHelper.assertBoundaryTimerJobMigrated("timerBoundary1", "timerBoundary1");
		testHelper.assertBoundaryTimerJobCreated("timerBoundary2");

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateBoundaryEventAndEventSubProcess()
	  public virtual void testMigrateBoundaryEventAndEventSubProcess()
	  {
		BpmnModelInstance testProcess = modify(ProcessModels.SUBPROCESS_PROCESS).addSubProcessTo("subProcess").triggerByEvent().embeddedSubProcess().startEvent("eventStart").message(MESSAGE_NAME).endEvent().subProcessDone().moveToActivity(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).signal(SIGNAL_NAME).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(testProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(testProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(BOUNDARY_ID, BOUNDARY_ID).mapActivities(USER_TASK_ID, USER_TASK_ID).build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("eventStart", MESSAGE_NAME);
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, SIGNAL_NAME);
		testHelper.assertEventSubscriptionCreated("eventStart", MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask(USER_TASK_ID);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateIncidentForJob()
	  public virtual void testMigrateIncidentForJob()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).userTaskBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).timerWithDate(TIMER_DATE).serviceTask("failingTask").camundaClass(typeof(FailingDelegate).FullName).endEvent().done();
		BpmnModelInstance targetProcess = modify(sourceProcess).changeElementId(USER_TASK_ID, "newUserTask").changeElementId(BOUNDARY_ID, "newBoundary");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// a timer job exists
		Job jobBeforeMigration = rule.ManagementService.createJobQuery().singleResult();
		assertNotNull(jobBeforeMigration);

		// if the timer job is triggered the failing delegate fails and an incident is created
		executeJob(jobBeforeMigration);
		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, "newUserTask").mapActivities(BOUNDARY_ID, "newBoundary").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the job and incident still exists
		Job jobAfterMigration = rule.ManagementService.createJobQuery().jobId(jobBeforeMigration.Id).singleResult();
		assertNotNull(jobAfterMigration);
		Incident incidentAfterMigration = rule.RuntimeService.createIncidentQuery().singleResult();
		assertNotNull(incidentAfterMigration);

		// and it is still the same incident
		assertEquals(incidentBeforeMigration.Id, incidentAfterMigration.Id);
		assertEquals(jobAfterMigration.Id, incidentAfterMigration.Configuration);

		// and the activity, process definition and job definition references were updated
		assertEquals("newBoundary", incidentAfterMigration.ActivityId);
		assertEquals(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);
		assertEquals(jobAfterMigration.JobDefinitionId, incidentAfterMigration.JobDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventMessage()
	  public virtual void testUpdateEventMessage()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).message("new" + MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, MESSAGE_NAME, BOUNDARY_ID, "new" + MESSAGE_NAME);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.correlateMessage("new" + MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventSignal()
	  public virtual void testUpdateEventSignal()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).signal("new" + SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, SIGNAL_NAME, BOUNDARY_ID, "new" + SIGNAL_NAME);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.signalEventReceived("new" + SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventTimer()
	  public virtual void testUpdateEventTimer()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();

		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).timerWithDate(TIMER_DATE).userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).timerWithDuration("PT50M").userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).updateEventTrigger().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		DateTime newDueDate = (new DateTime(ClockUtil.CurrentTime)).plusMinutes(50).toDate();
		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], BOUNDARY_ID, newDueDate);

		// and it is possible to successfully complete the migrated instance
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventSignalNameWithExpression()
	  public virtual void testUpdateEventSignalNameWithExpression()
	  {
		// given
		string signalNameWithExpression = "new" + SIGNAL_NAME + "-${var}";
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).signal(signalNameWithExpression).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).build();

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "foo";

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan, variables);

		// the signal event subscription's event name has changed
		string resolvedSignalName = "new" + SIGNAL_NAME + "-foo";
		testHelper.assertEventSubscriptionCreated(BOUNDARY_ID, resolvedSignalName);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.signalEventReceived(resolvedSignalName);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventMessageNameWithExpression()
	  public virtual void testUpdateEventMessageNameWithExpression()
	  {
		// given
		string messageNameWithExpression = "new" + MESSAGE_NAME + "-${var}";
		BpmnModelInstance sourceProcess = ProcessModels.ONE_TASK_PROCESS;
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).message(messageNameWithExpression).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).build();

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "foo";

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan, variables);

		// the message event subscription's event name has changed
		string resolvedMessageName = "new" + MESSAGE_NAME + "-foo";
		testHelper.assertEventSubscriptionCreated(BOUNDARY_ID, resolvedMessageName);

		// and it is possible to successfully complete the migrated instance
		rule.RuntimeService.correlateMessage(resolvedMessageName);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateConditionalEventExpression()
	  public virtual void testUpdateConditionalEventExpression()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(FALSE_CONDITION).userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(VAR_CONDITION).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).updateEventTrigger().build();

		// when process is migrated without update event trigger
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then condition is migrated and has new condition expr
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, null);

		// and it is possible to successfully complete the migrated instance
		testHelper.AnyVariable = testHelper.snapshotAfterMigration.ProcessInstanceId;
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSignalBoundaryEventKeepTrigger()
	  public virtual void testMigrateSignalBoundaryEventKeepTrigger()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).signal(SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).signal("new" + SIGNAL_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		IDictionary<string, string> activities = new Dictionary<string, string>();
		activities[USER_TASK_ID] = USER_TASK_ID;
		activities[BOUNDARY_ID] = BOUNDARY_ID;

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).build();


		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, SIGNAL_NAME);

		// and no event subscription for the new message name exists
		EventSubscription eventSubscription = rule.RuntimeService.createEventSubscriptionQuery().eventName("new" + SIGNAL_NAME).singleResult();
		assertNull(eventSubscription);
		assertEquals(1, rule.RuntimeService.createEventSubscriptionQuery().count());

		// and it is possible to trigger the event with the old message name and successfully complete the migrated instance
		rule.ProcessEngine.RuntimeService.signalEventReceived(SIGNAL_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateMessageBoundaryEventKeepTrigger()
	  public virtual void testMigrateMessageBoundaryEventKeepTrigger()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).message(MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).message("new" + MESSAGE_NAME).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		IDictionary<string, string> activities = new Dictionary<string, string>();
		activities[USER_TASK_ID] = USER_TASK_ID;
		activities[BOUNDARY_ID] = BOUNDARY_ID;

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).build();


		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, MESSAGE_NAME);

		// and no event subscription for the new message name exists
		EventSubscription eventSubscription = rule.RuntimeService.createEventSubscriptionQuery().eventName("new" + MESSAGE_NAME).singleResult();
		assertNull(eventSubscription);
		assertEquals(1, rule.RuntimeService.createEventSubscriptionQuery().count());

		// and it is possible to trigger the event with the old message name and successfully complete the migrated instance
		rule.ProcessEngine.RuntimeService.correlateMessage(MESSAGE_NAME);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateTimerBoundaryEventKeepTrigger()
	  public virtual void testMigrateTimerBoundaryEventKeepTrigger()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).timerWithDuration("PT5S").userTask(AFTER_BOUNDARY_TASK).endEvent().done();
		BpmnModelInstance targetProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).timerWithDuration("PT10M").userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		IDictionary<string, string> activities = new Dictionary<string, string>();
		activities[USER_TASK_ID] = USER_TASK_ID;
		activities[BOUNDARY_ID] = BOUNDARY_ID;

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).build();


		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated(BOUNDARY_ID, BOUNDARY_ID, TimerExecuteNestedActivityJobHandler.TYPE);

		// and it is possible to trigger the event and successfully complete the migrated instance
		ManagementService managementService = rule.ManagementService;
		Job job = managementService.createJobQuery().singleResult();

		managementService.executeJob(job.Id);
		testHelper.completeTask(AFTER_BOUNDARY_TASK);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateConditionalBoundaryEventKeepTrigger()
	  public virtual void testMigrateConditionalBoundaryEventKeepTrigger()
	  {
		// given
		BpmnModelInstance sourceProcess = modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).condition(FALSE_CONDITION).userTask(AFTER_BOUNDARY_TASK).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);

		// expected migration validation exception
		exceptionRule.expect(typeof(MigrationPlanValidationException));
		exceptionRule.expectMessage(MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG);

		// when conditional boundary event is migrated without update event trigger
		rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(USER_TASK_ID, USER_TASK_ID).mapActivities(BOUNDARY_ID, BOUNDARY_ID).build();
	  }
	}

}