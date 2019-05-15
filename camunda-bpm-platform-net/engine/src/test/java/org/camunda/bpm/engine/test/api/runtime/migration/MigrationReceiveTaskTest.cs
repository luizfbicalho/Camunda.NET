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
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using MessageReceiveModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MessageReceiveModels;
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
	public class MigrationReceiveTaskTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationReceiveTaskTest()
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
//ORIGINAL LINE: @Test public void testCannotMigrateActivityInstance()
	  public virtual void testCannotMigrateActivityInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask", "receiveTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive task
		rule.RuntimeService.correlateMessage(MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionProperties()
	  public virtual void testMigrateEventSubscriptionProperties()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask", "receiveTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		EventSubscription eventSubscriptionBefore = testHelper.snapshotBeforeMigration.EventSubscriptions[0];

		IList<EventSubscription> eventSubscriptionsAfter = testHelper.snapshotAfterMigration.EventSubscriptions;
		Assert.assertEquals(1, eventSubscriptionsAfter.Count);
		EventSubscription eventSubscriptionAfter = eventSubscriptionsAfter[0];
		Assert.assertEquals(eventSubscriptionBefore.Created, eventSubscriptionAfter.Created);
		Assert.assertEquals(eventSubscriptionBefore.ExecutionId, eventSubscriptionAfter.ExecutionId);
		Assert.assertEquals(eventSubscriptionBefore.ProcessInstanceId, eventSubscriptionAfter.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionChangeActivityId()
	  public virtual void testMigrateEventSubscriptionChangeActivityId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS).changeElementId("receiveTask", "newReceiveTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask", "newReceiveTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated("receiveTask", "newReceiveTask", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive task
		rule.RuntimeService.correlateMessage(MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionPreserveMessageName()
	  public virtual void testMigrateEventSubscriptionPreserveMessageName()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS).renameMessage(MessageReceiveModels.MESSAGE_NAME, "new" + MessageReceiveModels.MESSAGE_NAME));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask", "receiveTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the message event subscription's event name has not changed
		testHelper.assertEventSubscriptionMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive task
		rule.RuntimeService.correlateMessage(MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionUpdateMessageName()
	  public virtual void testMigrateEventSubscriptionUpdateMessageName()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS).renameMessage(MessageReceiveModels.MESSAGE_NAME, "new" + MessageReceiveModels.MESSAGE_NAME));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask", "receiveTask").updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the message event subscription's event name has not changed
		testHelper.assertEventSubscriptionMigrated("receiveTask", MessageReceiveModels.MESSAGE_NAME, "receiveTask", "new" + MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the event
		rule.RuntimeService.correlateMessage("new" + MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelMultiInstanceEventSubscription()
	  public virtual void testMigrateParallelMultiInstanceEventSubscription()
	  {
		BpmnModelInstance parallelMiReceiveTaskProcess = modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS).activityBuilder("receiveTask").multiInstance().parallel().cardinality("3").done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(parallelMiReceiveTaskProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(parallelMiReceiveTaskProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask#multiInstanceBody", "receiveTask#multiInstanceBody").mapActivities("receiveTask", "receiveTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionsMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive tasks
		rule.RuntimeService.createMessageCorrelation(MessageReceiveModels.MESSAGE_NAME).correlateAll();

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSequentialMultiInstanceEventSubscription()
	  public virtual void testMigrateSequentialMultiInstanceEventSubscription()
	  {
		BpmnModelInstance parallelMiReceiveTaskProcess = modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS).activityBuilder("receiveTask").multiInstance().sequential().cardinality("3").done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(parallelMiReceiveTaskProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(parallelMiReceiveTaskProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask#multiInstanceBody", "receiveTask#multiInstanceBody").mapActivities("receiveTask", "receiveTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionsMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive tasks
		for (int i = 0; i < 3; i++)
		{
		  rule.RuntimeService.correlateMessage(MessageReceiveModels.MESSAGE_NAME);
		}

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionAddParentScope()
	  public virtual void testMigrateEventSubscriptionAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.SUBPROCESS_RECEIVE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("receiveTask", "receiveTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive task
		rule.RuntimeService.correlateMessage(MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }
	}

}