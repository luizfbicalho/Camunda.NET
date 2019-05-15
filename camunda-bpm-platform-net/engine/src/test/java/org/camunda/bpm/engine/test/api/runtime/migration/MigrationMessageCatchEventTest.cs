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
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using MessageReceiveModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MessageReceiveModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationMessageCatchEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationMessageCatchEventTest()
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
//ORIGINAL LINE: @Test public void testMigrateEventSubscription()
	  public virtual void testMigrateEventSubscription()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("messageCatch", "messageCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated("messageCatch", "messageCatch", MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the receive task
		rule.RuntimeService.correlateMessage(MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionChangeActivityId()
	  public virtual void testMigrateEventSubscriptionChangeActivityId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS).changeElementId("messageCatch", "newMessageCatch"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("messageCatch", "newMessageCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertEventSubscriptionMigrated("messageCatch", "newMessageCatch", MessageReceiveModels.MESSAGE_NAME);

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
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().intermediateCatchEvent("messageCatch").message("new" + MessageReceiveModels.MESSAGE_NAME).userTask("userTask").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("messageCatch", "messageCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the message event subscription's event name has not changed
		testHelper.assertEventSubscriptionMigrated("messageCatch", "messageCatch", MessageReceiveModels.MESSAGE_NAME);

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
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS).renameMessage(MessageReceiveModels.MESSAGE_NAME, "new" + MessageReceiveModels.MESSAGE_NAME));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("messageCatch", "messageCatch").updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the message event subscription's event name has changed
		testHelper.assertEventSubscriptionMigrated("messageCatch", MessageReceiveModels.MESSAGE_NAME, "messageCatch", "new" + MessageReceiveModels.MESSAGE_NAME);

		// and it is possible to trigger the event
		rule.RuntimeService.correlateMessage("new" + MessageReceiveModels.MESSAGE_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionUpdateMessageNameWithExpression()
	  public virtual void testMigrateEventSubscriptionUpdateMessageNameWithExpression()
	  {
		// given
		string newMessageName = "new" + MessageReceiveModels.MESSAGE_NAME + "-${var}";
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS).renameMessage(MessageReceiveModels.MESSAGE_NAME, newMessageName));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("messageCatch", "messageCatch").updateEventTrigger().build();

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "foo";

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan, variables);

		// then the message event subscription's event name has changed
		string resolvedMessageName = "new" + MessageReceiveModels.MESSAGE_NAME + "-foo";
		testHelper.assertEventSubscriptionMigrated("messageCatch", MessageReceiveModels.MESSAGE_NAME, "messageCatch", resolvedMessageName);

		// and it is possible to trigger the event
		rule.RuntimeService.correlateMessage(resolvedMessageName);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }
	}

}