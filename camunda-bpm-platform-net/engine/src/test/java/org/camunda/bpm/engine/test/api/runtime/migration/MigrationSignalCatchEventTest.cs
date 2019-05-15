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
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using SignalCatchModels = org.camunda.bpm.engine.test.api.runtime.migration.models.SignalCatchModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationSignalCatchEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationSignalCatchEventTest()
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
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("signalCatch", "signalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("signalCatch", "signalCatch", SignalCatchModels.SIGNAL_NAME);

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("signalCatch").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("signalCatch")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("signalCatch", testHelper.getSingleActivityInstanceBeforeMigration("signalCatch").Id).done());

		// and it is possible to trigger the event
		rule.RuntimeService.signalEventReceived(SignalCatchModels.SIGNAL_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionChangeActivityId()
	  public virtual void testMigrateEventSubscriptionChangeActivityId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS).changeElementId("signalCatch", "newSignalCatch"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("signalCatch", "newSignalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("signalCatch", "newSignalCatch", SignalCatchModels.SIGNAL_NAME);

		// and it is possible to trigger the event
		rule.RuntimeService.signalEventReceived(SignalCatchModels.SIGNAL_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionPreserveSignalName()
	  public virtual void testMigrateEventSubscriptionPreserveSignalName()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().intermediateCatchEvent("signalCatch").signal("new" + SignalCatchModels.SIGNAL_NAME).userTask("userTask").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("signalCatch", "signalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the signal name of the event subscription has not changed
		testHelper.assertEventSubscriptionMigrated("signalCatch", "signalCatch", SignalCatchModels.SIGNAL_NAME);

		// and it is possible to trigger the event
		rule.RuntimeService.signalEventReceived(SignalCatchModels.SIGNAL_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionUpdateSignalName()
	  public virtual void testMigrateEventSubscriptionUpdateSignalName()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().intermediateCatchEvent("signalCatch").signal("new" + SignalCatchModels.SIGNAL_NAME).userTask("userTask").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("signalCatch", "signalCatch").updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the message event subscription's event name has not changed
		testHelper.assertEventSubscriptionMigrated("signalCatch", SignalCatchModels.SIGNAL_NAME, "signalCatch", "new" + SignalCatchModels.SIGNAL_NAME);

		// and it is possible to trigger the event
		rule.RuntimeService.signalEventReceived("new" + SignalCatchModels.SIGNAL_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJobAddParentScope()
	  public virtual void testMigrateJobAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.SUBPROCESS_SIGNAL_CATCH_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("signalCatch", "signalCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertEventSubscriptionMigrated("signalCatch", "signalCatch", SignalCatchModels.SIGNAL_NAME);

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("signalCatch").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("signalCatch")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("signalCatch", testHelper.getSingleActivityInstanceBeforeMigration("signalCatch").Id).done());

		// and it is possible to trigger the event
		rule.RuntimeService.signalEventReceived(SignalCatchModels.SIGNAL_NAME);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubscriptionUpdateSignalExpressionNameWithVariables()
	  public virtual void testMigrateEventSubscriptionUpdateSignalExpressionNameWithVariables()
	  {
		// given
		string newSignalName = "new" + SignalCatchModels.SIGNAL_NAME + "-${var}";
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().intermediateCatchEvent("signalCatch").signal(newSignalName).userTask("userTask").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("signalCatch", "signalCatch").updateEventTrigger().build();

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "foo";


		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan, variables);

		// then there should be a variable
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("var");
		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);

		// and the signal event subscription's event name has changed
		string resolvedSignalName = "new" + SignalCatchModels.SIGNAL_NAME + "-foo";
		testHelper.assertEventSubscriptionMigrated("signalCatch", SignalCatchModels.SIGNAL_NAME, "signalCatch", resolvedSignalName);

		// and it is possible to trigger the event and complete the task afterwards
		rule.RuntimeService.signalEventReceived(resolvedSignalName);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

	}

}