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
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using EventSubProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
	using MessageReceiveModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MessageReceiveModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	public class MigrationMessageStartEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationMessageStartEventTest()
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
	  protected internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscription()
	  public virtual void testMigrateEventSubscription()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.MESSAGE_START_PROCESS);
		string sourceProcessDefinitionId = sourceProcessDefinition.Id;

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinitionId, sourceProcessDefinitionId).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinitionId);
		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().activityId("startEvent").eventName(MessageReceiveModels.MESSAGE_NAME).singleResult();

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		assertEventSubscriptionMigrated(eventSubscription, "startEvent", MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionWithEventSubProcess()
	  public virtual void testMigrateEventSubscriptionWithEventSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		EventSubscription eventSubscriptionAfter = runtimeService.createEventSubscriptionQuery().singleResult();

		assertNotNull(eventSubscriptionAfter);
		assertEquals(EventSubProcessModels.MESSAGE_NAME, eventSubscriptionAfter.EventName);

		runtimeService.correlateMessage(EventSubProcessModels.MESSAGE_NAME);
		testHelper.completeTask("eventSubProcessTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

	  protected internal virtual void assertEventSubscriptionMigrated(EventSubscription eventSubscriptionBefore, string activityIdAfter, string eventName)
	  {
		EventSubscription eventSubscriptionAfter = runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull("Expected that an event subscription with id '" + eventSubscriptionBefore.Id + "' " + "exists after migration", eventSubscriptionAfter);

		assertEquals(eventSubscriptionBefore.EventType, eventSubscriptionAfter.EventType);
		assertEquals(activityIdAfter, eventSubscriptionAfter.ActivityId);
		assertEquals(eventName, eventSubscriptionAfter.EventName);
	  }
	}

}