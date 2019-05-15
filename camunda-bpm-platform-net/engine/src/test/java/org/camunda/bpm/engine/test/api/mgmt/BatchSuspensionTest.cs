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
namespace org.camunda.bpm.engine.test.api.mgmt
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.BATCH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AbstractSetBatchStateCmd = org.camunda.bpm.engine.impl.cmd.AbstractSetBatchStateCmd;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class BatchSuspensionTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchSuspensionTest()
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
			helper = new BatchMigrationHelper(engineRule, migrationRule);
			ruleChain = RuleChain.outerRule(engineRule).around(migrationRule);
		}


	  public const string USER_ID = "userId";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

	  protected internal int defaultBatchJobsPerSeed;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void saveAndReduceBatchJobsPerSeed()
	  public virtual void saveAndReduceBatchJobsPerSeed()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		// reduce number of batch jobs per seed to not have to create a lot of instances
		configuration.BatchJobsPerSeed = 1;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetBatchJobsPerSeed()
	  public virtual void resetBatchJobsPerSeed()
	  {
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSuspendBatch()
	  public virtual void shouldSuspendBatch()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		managementService.suspendBatchById(batch.Id);

		// then
		batch = managementService.createBatchQuery().batchId(batch.Id).singleResult();
		assertTrue(batch.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailWhenSuspendingUsingUnknownId()
	  public virtual void shouldFailWhenSuspendingUsingUnknownId()
	  {
		try
		{
		  managementService.suspendBatchById("unknown");
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("Batch for id 'unknown' cannot be found"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailWhenSuspendingUsingNullId()
	  public virtual void shouldFailWhenSuspendingUsingNullId()
	  {
		try
		{
		  managementService.suspendBatchById(null);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("batch id is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSuspendSeedJobAndDefinition()
	  public virtual void shouldSuspendSeedJobAndDefinition()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		managementService.suspendBatchById(batch.Id);

		// then
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertTrue(seedJobDefinition.Suspended);

		Job seedJob = helper.getSeedJob(batch);
		assertTrue(seedJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateSuspendedSeedJob()
	  public virtual void shouldCreateSuspendedSeedJob()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(2);
		managementService.suspendBatchById(batch.Id);

		// when
		helper.executeSeedJob(batch);

		// then
		Job seedJob = helper.getSeedJob(batch);
		assertTrue(seedJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSuspendMonitorJobAndDefinition()
	  public virtual void shouldSuspendMonitorJobAndDefinition()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);

		// when
		managementService.suspendBatchById(batch.Id);

		// then
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertTrue(monitorJobDefinition.Suspended);

		Job monitorJob = helper.getMonitorJob(batch);
		assertTrue(monitorJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateSuspendedMonitorJob()
	  public virtual void shouldCreateSuspendedMonitorJob()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);

		// when
		helper.executeSeedJob(batch);

		// then
		Job monitorJob = helper.getMonitorJob(batch);
		assertTrue(monitorJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSuspendExecutionJobsAndDefinition()
	  public virtual void shouldSuspendExecutionJobsAndDefinition()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);

		// when
		managementService.suspendBatchById(batch.Id);

		// then
		JobDefinition migrationJobDefinition = helper.getExecutionJobDefinition(batch);
		assertTrue(migrationJobDefinition.Suspended);

		Job migrationJob = helper.getExecutionJobs(batch)[0];
		assertTrue(migrationJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateSuspendedExecutionJobs()
	  public virtual void shouldCreateSuspendedExecutionJobs()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);

		// when
		helper.executeSeedJob(batch);

		// then
		Job migrationJob = helper.getExecutionJobs(batch)[0];
		assertTrue(migrationJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void shouldCreateUserOperationLogForBatchSuspension()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void shouldCreateUserOperationLogForBatchSuspension()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.suspendBatchById(batch.Id);
		identityService.clearAuthentication();

		// then
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();

		assertNotNull(entry);
		assertEquals(batch.Id, entry.BatchId);
		assertEquals(AbstractSetBatchStateCmd.SUSPENSION_STATE_PROPERTY, entry.Property);
		assertNull(entry.OrgValue);
		assertEquals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED.Name, entry.NewValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldActivateBatch()
	  public virtual void shouldActivateBatch()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);

		// when
		managementService.activateBatchById(batch.Id);

		// then
		batch = managementService.createBatchQuery().batchId(batch.Id).singleResult();
		assertFalse(batch.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailWhenActivatingUsingUnknownId()
	  public virtual void shouldFailWhenActivatingUsingUnknownId()
	  {
		try
		{
		  managementService.activateBatchById("unknown");
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("Batch for id 'unknown' cannot be found"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailWhenActivatingUsingNullId()
	  public virtual void shouldFailWhenActivatingUsingNullId()
	  {
		try
		{
		  managementService.activateBatchById(null);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("batch id is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldActivateSeedJobAndDefinition()
	  public virtual void shouldActivateSeedJobAndDefinition()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);

		// when
		managementService.activateBatchById(batch.Id);

		// then
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertFalse(seedJobDefinition.Suspended);

		Job seedJob = helper.getSeedJob(batch);
		assertFalse(seedJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateActivatedSeedJob()
	  public virtual void shouldCreateActivatedSeedJob()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(2);

		// when
		helper.executeSeedJob(batch);

		// then
		Job seedJob = helper.getSeedJob(batch);
		assertFalse(seedJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldActivateMonitorJobAndDefinition()
	  public virtual void shouldActivateMonitorJobAndDefinition()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);
		helper.executeSeedJob(batch);

		// when
		managementService.activateBatchById(batch.Id);

		// then
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertFalse(monitorJobDefinition.Suspended);

		Job monitorJob = helper.getMonitorJob(batch);
		assertFalse(monitorJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateActivatedMonitorJob()
	  public virtual void shouldCreateActivatedMonitorJob()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		helper.executeSeedJob(batch);

		// then
		Job monitorJob = helper.getMonitorJob(batch);
		assertFalse(monitorJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldActivateExecutionJobsAndDefinition()
	  public virtual void shouldActivateExecutionJobsAndDefinition()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);
		helper.executeSeedJob(batch);

		// when
		managementService.activateBatchById(batch.Id);

		// then
		JobDefinition migrationJobDefinition = helper.getExecutionJobDefinition(batch);
		assertFalse(migrationJobDefinition.Suspended);

		Job migrationJob = helper.getExecutionJobs(batch)[0];
		assertFalse(migrationJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateActivatedExecutionJobs()
	  public virtual void shouldCreateActivatedExecutionJobs()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		helper.executeSeedJob(batch);

		// then
		Job migrationJob = helper.getExecutionJobs(batch)[0];
		assertFalse(migrationJob.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void shouldCreateUserOperationLogForBatchActivation()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void shouldCreateUserOperationLogForBatchActivation()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.activateBatchById(batch.Id);
		identityService.clearAuthentication();

		// then
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();

		assertNotNull(entry);
		assertEquals(batch.Id, entry.BatchId);
		assertEquals(AbstractSetBatchStateCmd.SUSPENSION_STATE_PROPERTY, entry.Property);
		assertNull(entry.OrgValue);
		assertEquals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE.Name, entry.NewValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUserOperationLogQueryByBatchEntityType()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUserOperationLogQueryByBatchEntityType()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.suspendBatchById(batch1.Id);
		managementService.suspendBatchById(batch2.Id);
		managementService.activateBatchById(batch1.Id);
		identityService.clearAuthentication();

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().entityType(BATCH);
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testUserOperationLogQueryByBatchId()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testUserOperationLogQueryByBatchId()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.suspendBatchById(batch1.Id);
		managementService.suspendBatchById(batch2.Id);
		managementService.activateBatchById(batch1.Id);
		identityService.clearAuthentication();

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().batchId(batch1.Id);
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());

		query = historyService.createUserOperationLogQuery().batchId(batch2.Id);
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
	  }

	}

}