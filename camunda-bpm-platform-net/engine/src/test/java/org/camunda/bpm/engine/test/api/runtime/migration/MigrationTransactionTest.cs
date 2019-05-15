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
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using EventSubProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using TransactionModels = org.camunda.bpm.engine.test.api.runtime.migration.models.TransactionModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationTransactionTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationTransactionTest()
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
//ORIGINAL LINE: @Test public void testContinueProcess()
	  public virtual void testContinueProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("transaction", "transaction").mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testContinueProcessTriggerCancellation()
	  public virtual void testContinueProcessTriggerCancellation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.CANCEL_BOUNDARY_EVENT);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("transaction", "transaction").mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.completeTask("userTask");
		testHelper.completeTask("afterBoundaryTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAssertTrees()
	  public virtual void testAssertTrees()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("transaction", "transaction").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("userTask")).up().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("transaction", testHelper.getSingleActivityInstanceBeforeMigration("transaction").Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTransactionContinueProcess()
	  public virtual void testAddTransactionContinueProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTransactionTriggerCancellation()
	  public virtual void testAddTransactionTriggerCancellation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.CANCEL_BOUNDARY_EVENT);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.completeTask("userTask");
		testHelper.completeTask("afterBoundaryTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTransactionAssertTrees()
	  public virtual void testAddTransactionAssertTrees()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("transaction").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTransactionContinueProcess()
	  public virtual void testRemoveTransactionContinueProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveTransactionAssertTrees()
	  public virtual void testRemoveTransactionAssertTrees()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateTransactionToEmbeddedSubProcess()
	  public virtual void testMigrateTransactionToEmbeddedSubProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("transaction", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		Assert.assertEquals(testHelper.getSingleActivityInstanceBeforeMigration("transaction").Id, testHelper.getSingleActivityInstanceAfterMigration("subProcess").Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateEventSubProcessToTransaction()
	  public virtual void testMigrateEventSubProcessToTransaction()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("eventSubProcess", "transaction").mapActivities("eventSubProcessTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("eventSubProcessTask").execute();

		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(testHelper.getSingleActivityInstanceBeforeMigration("eventSubProcess").Id, testHelper.getSingleActivityInstanceAfterMigration("transaction").Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

	}

}