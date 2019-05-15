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
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using EventBasedGatewayModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventBasedGatewayModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationEventBasedGatewayTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationEventBasedGatewayTest()
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
//ORIGINAL LINE: @Test public void testMigrateGatewayExecutionTree()
	  public virtual void testMigrateGatewayExecutionTree()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("eventBasedGateway").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("eventBasedGateway")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("eventBasedGateway", testHelper.getSingleActivityInstanceBeforeMigration("eventBasedGateway").Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithTimerEvent()
	  public virtual void testMigrateGatewayWithTimerEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertIntermediateTimerJobRemoved("timerCatch");
		testHelper.assertIntermediateTimerJobCreated("timerCatch");

		Job timerJob = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(timerJob.Id);

		testHelper.completeTask("afterTimerCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithMessageEvent()
	  public virtual void testMigrateGatewayWithMessageEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("messageCatch", EventBasedGatewayModels.MESSAGE_NAME);
		testHelper.assertEventSubscriptionCreated("messageCatch", EventBasedGatewayModels.MESSAGE_NAME);

		rule.RuntimeService.correlateMessage(EventBasedGatewayModels.MESSAGE_NAME);

		testHelper.completeTask("afterMessageCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithSignalEvent()
	  public virtual void testMigrateGatewayWithSignalEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("signalCatch", EventBasedGatewayModels.SIGNAL_NAME);
		testHelper.assertEventSubscriptionCreated("signalCatch", EventBasedGatewayModels.SIGNAL_NAME);

		rule.RuntimeService.signalEventReceived(EventBasedGatewayModels.SIGNAL_NAME);

		testHelper.completeTask("afterSignalCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithTimerEventMapEvent()
	  public virtual void testMigrateGatewayWithTimerEventMapEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("timerCatch", "timerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertIntermediateTimerJobMigrated("timerCatch", "timerCatch");

		Job timerJob = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(timerJob.Id);

		testHelper.completeTask("afterTimerCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithMessageEventMapEvent()
	  public virtual void testMigrateGatewayWithMessageEventMapEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("messageCatch", "messageCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("messageCatch", "messageCatch", EventBasedGatewayModels.MESSAGE_NAME);

		rule.RuntimeService.correlateMessage(EventBasedGatewayModels.MESSAGE_NAME);

		testHelper.completeTask("afterMessageCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithSignalEventMapEvent()
	  public virtual void testMigrateGatewayWithSignalEventMapEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("signalCatch", "signalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("signalCatch", "signalCatch", EventBasedGatewayModels.SIGNAL_NAME);

		rule.RuntimeService.signalEventReceived(EventBasedGatewayModels.SIGNAL_NAME);

		testHelper.completeTask("afterSignalCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayAddTimerEvent()
	  public virtual void testMigrateGatewayAddTimerEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS).flowNodeBuilder("eventBasedGateway").intermediateCatchEvent("newTimerCatch").timerWithDuration("PT50M").userTask("afterNewTimerCatch").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("timerCatch", "timerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertIntermediateTimerJobCreated("newTimerCatch");
		testHelper.assertIntermediateTimerJobMigrated("timerCatch", "timerCatch");

		Job newTimerJob = rule.ManagementService.createJobQuery().activityId("newTimerCatch").singleResult();
		rule.ManagementService.executeJob(newTimerJob.Id);

		testHelper.completeTask("afterNewTimerCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayAddMessageEvent()
	  public virtual void testMigrateGatewayAddMessageEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS).flowNodeBuilder("eventBasedGateway").intermediateCatchEvent("newMessageCatch").message("new" + EventBasedGatewayModels.MESSAGE_NAME).userTask("afterNewMessageCatch").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("messageCatch", "messageCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("newMessageCatch", "new" + EventBasedGatewayModels.MESSAGE_NAME);
		testHelper.assertEventSubscriptionMigrated("messageCatch", "messageCatch", EventBasedGatewayModels.MESSAGE_NAME);

		rule.RuntimeService.correlateMessage("new" + EventBasedGatewayModels.MESSAGE_NAME);

		testHelper.completeTask("afterNewMessageCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayAddSignalEvent()
	  public virtual void testMigrateGatewayAddSignalEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS).flowNodeBuilder("eventBasedGateway").intermediateCatchEvent("newSignalCatch").signal("new" + EventBasedGatewayModels.SIGNAL_NAME).userTask("afterNewSignalCatch").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("signalCatch", "signalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionCreated("newSignalCatch", "new" + EventBasedGatewayModels.SIGNAL_NAME);
		testHelper.assertEventSubscriptionMigrated("signalCatch", "signalCatch", EventBasedGatewayModels.SIGNAL_NAME);

		rule.RuntimeService.signalEventReceived("new" + EventBasedGatewayModels.SIGNAL_NAME);

		testHelper.completeTask("afterNewSignalCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayRemoveTimerEvent()
	  public virtual void testMigrateGatewayRemoveTimerEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS).flowNodeBuilder("eventBasedGateway").intermediateCatchEvent("oldTimerCatch").timerWithDuration("PT50M").endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("timerCatch", "timerCatch").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertIntermediateTimerJobRemoved("oldTimerCatch");
		testHelper.assertIntermediateTimerJobMigrated("timerCatch", "timerCatch");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayRemoveMessageEvent()
	  public virtual void testMigrateGatewayRemoveMessageEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS).flowNodeBuilder("eventBasedGateway").intermediateCatchEvent("oldMessageCatch").message("old" + EventBasedGatewayModels.MESSAGE_NAME).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("messageCatch", "messageCatch").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("oldMessageCatch", "old" + EventBasedGatewayModels.MESSAGE_NAME);
		testHelper.assertEventSubscriptionMigrated("messageCatch", "messageCatch", EventBasedGatewayModels.MESSAGE_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayRemoveSignalEvent()
	  public virtual void testMigrateGatewayRemoveSignalEvent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS).flowNodeBuilder("eventBasedGateway").intermediateCatchEvent("oldSignalCatch").signal("old" + EventBasedGatewayModels.SIGNAL_NAME).endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("signalCatch", "signalCatch").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionRemoved("oldSignalCatch", "old" + EventBasedGatewayModels.SIGNAL_NAME);
		testHelper.assertEventSubscriptionMigrated("signalCatch", "signalCatch", EventBasedGatewayModels.SIGNAL_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithTimerEventChangeId()
	  public virtual void testMigrateGatewayWithTimerEventChangeId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS).changeElementId("timerCatch", "newTimerCatch"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("timerCatch", "newTimerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertIntermediateTimerJobMigrated("timerCatch", "newTimerCatch");

		Job timerJob = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(timerJob.Id);

		testHelper.completeTask("afterTimerCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithMessageEventChangeId()
	  public virtual void testMigrateGatewayWithMessageEventChangeId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS).changeElementId("messageCatch", "newMessageCatch"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("messageCatch", "newMessageCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("messageCatch", "newMessageCatch", EventBasedGatewayModels.MESSAGE_NAME);

		rule.RuntimeService.correlateMessage(EventBasedGatewayModels.MESSAGE_NAME);

		testHelper.completeTask("afterMessageCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithSignalEventChangeId()
	  public virtual void testMigrateGatewayWithSignalEventChangeId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS).changeElementId("signalCatch", "newSignalCatch"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("signalCatch", "newSignalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("signalCatch", "newSignalCatch", EventBasedGatewayModels.SIGNAL_NAME);

		rule.RuntimeService.signalEventReceived(EventBasedGatewayModels.SIGNAL_NAME);

		testHelper.completeTask("afterSignalCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayWithIncident()
	  public virtual void testMigrateGatewayWithIncident()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").mapActivities("timerCatch", "timerCatch").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		Job timerJob = rule.ManagementService.createJobQuery().singleResult();
		// create an incident
		rule.ManagementService.setJobRetries(timerJob.Id, 0);
		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then job and incident still exist
		testHelper.assertIntermediateTimerJobMigrated("timerCatch", "timerCatch");

		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];

		Incident incidentAfterMigration = rule.RuntimeService.createIncidentQuery().singleResult();
		assertNotNull(incidentAfterMigration);

		assertEquals(incidentBeforeMigration.Id, incidentAfterMigration.Id);
		assertEquals(jobAfterMigration.Id, incidentAfterMigration.Configuration);

		assertEquals("timerCatch", incidentAfterMigration.ActivityId);
		assertEquals(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);

		// and it is possible to complete the process
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("afterTimerCatch");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateGatewayRemoveIncidentOnMigration()
	  public virtual void testMigrateGatewayRemoveIncidentOnMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventBasedGateway", "eventBasedGateway").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		Job timerJob = rule.ManagementService.createJobQuery().singleResult();
		// create an incident
		rule.ManagementService.setJobRetries(timerJob.Id, 0);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the incident is gone
		Assert.assertEquals(0, rule.RuntimeService.createIncidentQuery().count());
	  }

	}

}