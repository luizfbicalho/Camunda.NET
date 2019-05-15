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
namespace org.camunda.bpm.engine.test.api.runtime.migration.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BatchMigrationUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchMigrationUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			migrationRule = new MigrationTestRule(engineRule);
			batchHelper = new BatchMigrationHelper(engineRule, migrationRule);
			ruleChain = RuleChain.outerRule(engineRule).around(migrationRule);
		}


	  public const string USER_ID = "userId";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule migrationRule;

	  protected internal BatchMigrationHelper batchHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		batchHelper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLogCreation()
	  public virtual void testLogCreation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		engineRule.IdentityService.AuthenticatedUserId = USER_ID;
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();
		engineRule.IdentityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = engineRule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry procDefEntry = entries["processDefinitionId"];
		Assert.assertNotNull(procDefEntry);
		Assert.assertEquals("ProcessInstance", procDefEntry.EntityType);
		Assert.assertEquals("Migrate", procDefEntry.OperationType);
		Assert.assertEquals(sourceProcessDefinition.Id, procDefEntry.ProcessDefinitionId);
		Assert.assertEquals(sourceProcessDefinition.Key, procDefEntry.ProcessDefinitionKey);
		Assert.assertNull(procDefEntry.ProcessInstanceId);
		Assert.assertEquals(sourceProcessDefinition.Id, procDefEntry.OrgValue);
		Assert.assertEquals(targetProcessDefinition.Id, procDefEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, procDefEntry.Category);

		UserOperationLogEntry asyncEntry = entries["async"];
		Assert.assertNotNull(asyncEntry);
		Assert.assertEquals("ProcessInstance", asyncEntry.EntityType);
		Assert.assertEquals("Migrate", asyncEntry.OperationType);
		Assert.assertEquals(sourceProcessDefinition.Id, asyncEntry.ProcessDefinitionId);
		Assert.assertEquals(sourceProcessDefinition.Key, asyncEntry.ProcessDefinitionKey);
		Assert.assertNull(asyncEntry.ProcessInstanceId);
		Assert.assertNull(asyncEntry.OrgValue);
		Assert.assertEquals("true", asyncEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		Assert.assertNotNull(numInstancesEntry);
		Assert.assertEquals("ProcessInstance", numInstancesEntry.EntityType);
		Assert.assertEquals("Migrate", numInstancesEntry.OperationType);
		Assert.assertEquals(sourceProcessDefinition.Id, numInstancesEntry.ProcessDefinitionId);
		Assert.assertEquals(sourceProcessDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
		Assert.assertNull(numInstancesEntry.ProcessInstanceId);
		Assert.assertNull(numInstancesEntry.OrgValue);
		Assert.assertEquals("1", numInstancesEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, numInstancesEntry.Category);

		Assert.assertEquals(procDefEntry.OperationId, asyncEntry.OperationId);
		Assert.assertEquals(asyncEntry.OperationId, numInstancesEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnSyncBatchJobExecution()
	  public virtual void testNoCreationOnSyncBatchJobExecution()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		Batch batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();
		batchHelper.executeSeedJob(batch);

		// when
		engineRule.IdentityService.AuthenticatedUserId = USER_ID;
		batchHelper.executeJobs(batch);
		engineRule.IdentityService.clearAuthentication();

		// then
		Assert.assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnJobExecutorBatchJobExecution()
	  public virtual void testNoCreationOnJobExecutorBatchJobExecution()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		// when
		migrationRule.waitForJobExecutorToProcessAllJobs(5000L);

		// then
		Assert.assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().count());
	  }

	  protected internal virtual IDictionary<string, UserOperationLogEntry> asMap(IList<UserOperationLogEntry> logEntries)
	  {
		IDictionary<string, UserOperationLogEntry> map = new Dictionary<string, UserOperationLogEntry>();

		foreach (UserOperationLogEntry entry in logEntries)
		{

		  UserOperationLogEntry previousValue = map[entry.Property] = entry;
		  if (previousValue != null)
		  {
			Assert.fail("expected only entry for every property");
		  }
		}

		return map;
	  }
	}

}