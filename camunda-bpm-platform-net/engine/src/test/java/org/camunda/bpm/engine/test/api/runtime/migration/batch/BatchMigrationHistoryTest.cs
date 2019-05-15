﻿using System;
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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using BatchMonitorJobHandler = org.camunda.bpm.engine.impl.batch.BatchMonitorJobHandler;
	using BatchSeedJobHandler = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class BatchMigrationHistoryTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BatchMigrationHistoryTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchMigrationHistoryTest()
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


	  protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper helper;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;

	  protected internal ProcessDefinition sourceProcessDefinition;
	  protected internal ProcessDefinition targetProcessDefinition;

	  protected internal bool defaultEnsureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public boolean ensureJobDueDateSet;
	  public bool ensureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public java.util.Date currentTime;
	  public DateTime currentTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Job DueDate is set: {0}") public static java.util.Collection<Object[]> scenarios() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {false, null},
			new object[] {true, START_DATE}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupConfiguration()
	  public virtual void setupConfiguration()
	  {
		configuration = engineRule.ProcessEngineConfiguration;
		defaultEnsureJobDueDateSet = configuration.EnsureJobDueDateNotNull;
		configuration.EnsureJobDueDateNotNull = ensureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setClock()
	  public virtual void setClock()
	  {
		ClockUtil.CurrentTime = START_DATE;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetConfiguration()
	  public virtual void resetConfiguration()
	  {
		configuration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchCreation()
	  public virtual void testHistoricBatchCreation()
	  {
		// when
		Batch batch = helper.migrateProcessInstancesAsync(10);

		// then a historic batch was created
		HistoricBatch historicBatch = helper.getHistoricBatch(batch);
		assertNotNull(historicBatch);
		assertEquals(batch.Id, historicBatch.Id);
		assertEquals(batch.Type, historicBatch.Type);
		assertEquals(batch.TotalJobs, historicBatch.TotalJobs);
		assertEquals(batch.BatchJobsPerSeed, historicBatch.BatchJobsPerSeed);
		assertEquals(batch.InvocationsPerBatchJob, historicBatch.InvocationsPerBatchJob);
		assertEquals(batch.SeedJobDefinitionId, historicBatch.SeedJobDefinitionId);
		assertEquals(batch.MonitorJobDefinitionId, historicBatch.MonitorJobDefinitionId);
		assertEquals(batch.BatchJobDefinitionId, historicBatch.BatchJobDefinitionId);
		assertEquals(START_DATE, historicBatch.StartTime);
		assertNull(historicBatch.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchCompletion()
	  public virtual void testHistoricBatchCompletion()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		DateTime endDate = helper.addSecondsToClock(12);

		// when
		helper.executeMonitorJob(batch);

		// then the historic batch has an end time set
		HistoricBatch historicBatch = helper.getHistoricBatch(batch);
		assertNotNull(historicBatch);
		assertEquals(endDate, historicBatch.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricSeedJobLog()
	  public virtual void testHistoricSeedJobLog()
	  {
		// when
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// then a historic job log exists for the seed job
		HistoricJobLog jobLog = helper.getHistoricSeedJobLog(batch)[0];
		assertNotNull(jobLog);
		assertTrue(jobLog.CreationLog);
		assertEquals(batch.SeedJobDefinitionId, jobLog.JobDefinitionId);
		assertEquals(BatchSeedJobHandler.TYPE, jobLog.JobDefinitionType);
		assertEquals(batch.Id, jobLog.JobDefinitionConfiguration);
		assertEquals(START_DATE, jobLog.Timestamp);
		assertNull(jobLog.DeploymentId);
		assertNull(jobLog.ProcessDefinitionId);
		assertNull(jobLog.ExecutionId);
		assertEquals(currentTime, jobLog.JobDueDate);

		// when the seed job is executed
		DateTime executionDate = helper.addSecondsToClock(12);
		helper.executeSeedJob(batch);

		// then a new historic job log exists for the seed job
		jobLog = helper.getHistoricSeedJobLog(batch)[1];
		assertNotNull(jobLog);
		assertTrue(jobLog.SuccessLog);
		assertEquals(batch.SeedJobDefinitionId, jobLog.JobDefinitionId);
		assertEquals(BatchSeedJobHandler.TYPE, jobLog.JobDefinitionType);
		assertEquals(batch.Id, jobLog.JobDefinitionConfiguration);
		assertEquals(executionDate, jobLog.Timestamp);
		assertNull(jobLog.DeploymentId);
		assertNull(jobLog.ProcessDefinitionId);
		assertNull(jobLog.ExecutionId);
		assertEquals(currentTime, jobLog.JobDueDate);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricMonitorJobLog()
	  public virtual void testHistoricMonitorJobLog()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when the seed job is executed
		helper.executeSeedJob(batch);

		Job monitorJob = helper.getMonitorJob(batch);
		IList<HistoricJobLog> jobLogs = helper.getHistoricMonitorJobLog(batch, monitorJob);
		assertEquals(1, jobLogs.Count);

		// then a creation historic job log exists for the monitor job without due date
		HistoricJobLog jobLog = jobLogs[0];
		assertCommonMonitorJobLogProperties(batch, jobLog);
		assertTrue(jobLog.CreationLog);
		assertEquals(START_DATE, jobLog.Timestamp);
		assertEquals(currentTime, jobLog.JobDueDate);

		// when the monitor job is executed
		DateTime executionDate = helper.addSecondsToClock(15);
		DateTime monitorJobDueDate = helper.addSeconds(executionDate, 30);
		helper.executeMonitorJob(batch);

		jobLogs = helper.getHistoricMonitorJobLog(batch, monitorJob);
		assertEquals(2, jobLogs.Count);

		// then a success job log was created for the last monitor job
		jobLog = jobLogs[1];
		assertCommonMonitorJobLogProperties(batch, jobLog);
		assertTrue(jobLog.SuccessLog);
		assertEquals(executionDate, jobLog.Timestamp);
		assertEquals(currentTime, jobLog.JobDueDate);

		// and a creation job log for the new monitor job was created with due date
		monitorJob = helper.getMonitorJob(batch);
		jobLogs = helper.getHistoricMonitorJobLog(batch, monitorJob);
		assertEquals(1, jobLogs.Count);

		jobLog = jobLogs[0];
		assertCommonMonitorJobLogProperties(batch, jobLog);
		assertTrue(jobLog.CreationLog);
		assertEquals(executionDate, jobLog.Timestamp);
		assertEquals(monitorJobDueDate, jobLog.JobDueDate);

		// when the migration and monitor jobs are executed
		executionDate = helper.addSecondsToClock(15);
		helper.executeJobs(batch);
		helper.executeMonitorJob(batch);

		jobLogs = helper.getHistoricMonitorJobLog(batch, monitorJob);
		assertEquals(2, jobLogs.Count);

		// then a success job log was created for the last monitor job
		jobLog = jobLogs[1];
		assertCommonMonitorJobLogProperties(batch, jobLog);
		assertTrue(jobLog.SuccessLog);
		assertEquals(executionDate, jobLog.Timestamp);
		assertEquals(monitorJobDueDate, jobLog.JobDueDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchJobLog()
	  public virtual void testHistoricBatchJobLog()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);

		string sourceDeploymentId = helper.SourceProcessDefinition.DeploymentId;

		// when
		DateTime executionDate = helper.addSecondsToClock(12);
		helper.executeJobs(batch);

		// then a historic job log exists for the batch job
		HistoricJobLog jobLog = helper.getHistoricBatchJobLog(batch)[0];
		assertNotNull(jobLog);
		assertTrue(jobLog.CreationLog);
		assertEquals(batch.BatchJobDefinitionId, jobLog.JobDefinitionId);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MIGRATION, jobLog.JobDefinitionType);
		assertEquals(batch.Id, jobLog.JobDefinitionConfiguration);
		assertEquals(START_DATE, jobLog.Timestamp);
		assertEquals(sourceDeploymentId, jobLog.DeploymentId);
		assertNull(jobLog.ProcessDefinitionId);
		assertNull(jobLog.ExecutionId);
		assertEquals(currentTime, jobLog.JobDueDate);

		jobLog = helper.getHistoricBatchJobLog(batch)[1];
		assertNotNull(jobLog);
		assertTrue(jobLog.SuccessLog);
		assertEquals(batch.BatchJobDefinitionId, jobLog.JobDefinitionId);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MIGRATION, jobLog.JobDefinitionType);
		assertEquals(batch.Id, jobLog.JobDefinitionConfiguration);
		assertEquals(executionDate, jobLog.Timestamp);
		assertEquals(sourceDeploymentId, jobLog.DeploymentId);
		assertNull(jobLog.ProcessDefinitionId);
		assertNull(jobLog.ExecutionId);
		assertEquals(currentTime, jobLog.JobDueDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchForBatchDeletion()
	  public virtual void testHistoricBatchForBatchDeletion()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		DateTime deletionDate = helper.addSecondsToClock(12);
		managementService.deleteBatch(batch.Id, false);

		// then the end time was set for the historic batch
		HistoricBatch historicBatch = helper.getHistoricBatch(batch);
		assertNotNull(historicBatch);
		assertEquals(deletionDate, historicBatch.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricSeedJobLogForBatchDeletion()
	  public virtual void testHistoricSeedJobLogForBatchDeletion()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		DateTime deletionDate = helper.addSecondsToClock(12);
		managementService.deleteBatch(batch.Id, false);

		// then a deletion historic job log was added
		HistoricJobLog jobLog = helper.getHistoricSeedJobLog(batch)[1];
		assertNotNull(jobLog);
		assertTrue(jobLog.DeletionLog);
		assertEquals(deletionDate, jobLog.Timestamp);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricMonitorJobLogForBatchDeletion()
	  public virtual void testHistoricMonitorJobLogForBatchDeletion()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);

		// when
		DateTime deletionDate = helper.addSecondsToClock(12);
		managementService.deleteBatch(batch.Id, false);

		// then a deletion historic job log was added
		HistoricJobLog jobLog = helper.getHistoricMonitorJobLog(batch)[1];
		assertNotNull(jobLog);
		assertTrue(jobLog.DeletionLog);
		assertEquals(deletionDate, jobLog.Timestamp);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchJobLogForBatchDeletion()
	  public virtual void testHistoricBatchJobLogForBatchDeletion()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);

		// when
		DateTime deletionDate = helper.addSecondsToClock(12);
		managementService.deleteBatch(batch.Id, false);

		// then a deletion historic job log was added
		HistoricJobLog jobLog = helper.getHistoricBatchJobLog(batch)[1];
		assertNotNull(jobLog);
		assertTrue(jobLog.DeletionLog);
		assertEquals(deletionDate, jobLog.Timestamp);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoricBatch()
	  public virtual void testDeleteHistoricBatch()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);
		helper.executeJobs(batch);
		helper.executeMonitorJob(batch);

		// when
		HistoricBatch historicBatch = helper.getHistoricBatch(batch);
		historyService.deleteHistoricBatch(historicBatch.Id);

		// then the historic batch was removed and all job logs
		assertNull(helper.getHistoricBatch(batch));
		assertTrue(helper.getHistoricSeedJobLog(batch).Count == 0);
		assertTrue(helper.getHistoricMonitorJobLog(batch).Count == 0);
		assertTrue(helper.getHistoricBatchJobLog(batch).Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricSeedJobIncidentDeletion()
	  public virtual void testHistoricSeedJobIncidentDeletion()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		Job seedJob = helper.getSeedJob(batch);
		managementService.setJobRetries(seedJob.Id, 0);

		managementService.deleteBatch(batch.Id, false);

		// when
		historyService.deleteHistoricBatch(batch.Id);

		// then the historic incident was deleted
		long historicIncidents = historyService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricMonitorJobIncidentDeletion()
	  public virtual void testHistoricMonitorJobIncidentDeletion()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		helper.executeSeedJob(batch);
		Job monitorJob = helper.getMonitorJob(batch);
		managementService.setJobRetries(monitorJob.Id, 0);

		managementService.deleteBatch(batch.Id, false);

		// when
		historyService.deleteHistoricBatch(batch.Id);

		// then the historic incident was deleted
		long historicIncidents = historyService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchJobLogIncidentDeletion()
	  public virtual void testHistoricBatchJobLogIncidentDeletion()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(3);

		helper.executeSeedJob(batch);
		helper.failExecutionJobs(batch, 3);

		managementService.deleteBatch(batch.Id, false);

		// when
		historyService.deleteHistoricBatch(batch.Id);

		// then the historic incident was deleted
		long historicIncidents = historyService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

	  protected internal virtual void assertCommonMonitorJobLogProperties(Batch batch, HistoricJobLog jobLog)
	  {
		assertNotNull(jobLog);
		assertEquals(batch.MonitorJobDefinitionId, jobLog.JobDefinitionId);
		assertEquals(BatchMonitorJobHandler.TYPE, jobLog.JobDefinitionType);
		assertEquals(batch.Id, jobLog.JobDefinitionConfiguration);
		assertNull(jobLog.DeploymentId);
		assertNull(jobLog.ProcessDefinitionId);
		assertNull(jobLog.ExecutionId);
	  }


	}

}