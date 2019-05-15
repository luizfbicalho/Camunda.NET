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
namespace org.camunda.bpm.engine.test.api.runtime.migration.batch
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using BatchSeedJobHandler = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using DelegateEvent = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateEvent;
	using DelegateExecutionListener = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateExecutionListener;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.startsWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class BatchMigrationTest
	public class BatchMigrationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchMigrationTest()
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
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(migrationRule).around(testRule);
		}


	  protected internal static readonly DateTime TEST_DATE = new DateTime(1457326800000L);

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper helper;
	  protected internal ProcessEngineTestRule testRule;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;

	  protected internal int defaultBatchJobsPerSeed;
	  protected internal int defaultInvocationsPerBatchJob;
	  protected internal bool defaultEnsureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule).around(testRule);
	  public RuleChain ruleChain;

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
			new object[] {true, TEST_DATE}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void storeEngineSettings()
	  public virtual void storeEngineSettings()
	  {
		configuration = engineRule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
		defaultEnsureJobDueDateSet = configuration.EnsureJobDueDateNotNull;
		configuration.EnsureJobDueDateNotNull = ensureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreEngineSettings()
	  public virtual void restoreEngineSettings()
	  {
		configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
		configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
		configuration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullMigrationPlan()
	  public virtual void testNullMigrationPlan()
	  {
		try
		{
		  runtimeService.newMigration(null).processInstanceIds(Collections.singletonList("process")).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("migration plan is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceIdsList()
	  public virtual void testNullProcessInstanceIdsList()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds((IList<string>) null).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceIdsListWithNullValue()
	  public virtual void testProcessInstanceIdsListWithNullValue()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList("foo", null, "bar")).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceIdsList()
	  public virtual void testEmptyProcessInstanceIdsList()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(System.Linq.Enumerable.Empty<string>()).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceIdsArray()
	  public virtual void testNullProcessInstanceIdsArray()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds((string[]) null).executeAsync();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceIdsArrayWithNullValue()
	  public virtual void testProcessInstanceIdsArrayWithNullValue()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds("foo", null, "bar").executeAsync();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceQuery()
	  public virtual void testNullProcessInstanceQuery()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceQuery(null).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceQuery()
	  public virtual void testEmptyProcessInstanceQuery()
	  {
		ProcessDefinition testProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery emptyProcessInstanceQuery = runtimeService.createProcessInstanceQuery();
		assertEquals(0, emptyProcessInstanceQuery.count());

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceQuery(emptyProcessInstanceQuery).executeAsync();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchCreation()
	  public virtual void testBatchCreation()
	  {
		// when
		Batch batch = helper.migrateProcessInstancesAsync(15);

		// then a batch is created
		assertBatchCreated(batch, 15);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSeedJobCreation()
	  public virtual void testSeedJobCreation()
	  {
		ClockUtil.CurrentTime = TEST_DATE;

		// when
		Batch batch = helper.migrateProcessInstancesAsync(10);

		// then there exists a seed job definition with the batch id as configuration
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);
		assertEquals(batch.Id, seedJobDefinition.JobConfiguration);
		assertEquals(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

		// and there exists a migration job definition
		JobDefinition migrationJobDefinition = helper.getExecutionJobDefinition(batch);
		assertNotNull(migrationJobDefinition);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MIGRATION, migrationJobDefinition.JobType);

		// and a seed job with no relation to a process or execution etc.
		Job seedJob = helper.getSeedJob(batch);
		assertNotNull(seedJob);
		assertEquals(seedJobDefinition.Id, seedJob.JobDefinitionId);
		assertEquals(currentTime, seedJob.Duedate);
		assertNull(seedJob.DeploymentId);
		assertNull(seedJob.ProcessDefinitionId);
		assertNull(seedJob.ProcessDefinitionKey);
		assertNull(seedJob.ProcessInstanceId);
		assertNull(seedJob.ExecutionId);

		// but no migration jobs where created
		IList<Job> migrationJobs = helper.getExecutionJobs(batch);
		assertEquals(0, migrationJobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationJobsCreation()
	  public virtual void testMigrationJobsCreation()
	  {
		ClockUtil.CurrentTime = TEST_DATE;

		// reduce number of batch jobs per seed to not have to create a lot of instances
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;

		Batch batch = helper.migrateProcessInstancesAsync(20);
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		JobDefinition migrationJobDefinition = helper.getExecutionJobDefinition(batch);
		string sourceDeploymentId = helper.SourceProcessDefinition.DeploymentId;

		// when
		helper.executeSeedJob(batch);

		// then there exist migration jobs
		IList<Job> migrationJobs = helper.getJobsForDefinition(migrationJobDefinition);
		assertEquals(10, migrationJobs.Count);

		foreach (Job migrationJob in migrationJobs)
		{
		  assertEquals(migrationJobDefinition.Id, migrationJob.JobDefinitionId);
		  assertEquals(currentTime, migrationJob.Duedate);
		  assertEquals(sourceDeploymentId, migrationJob.DeploymentId);
		  assertNull(migrationJob.ProcessDefinitionId);
		  assertNull(migrationJob.ProcessDefinitionKey);
		  assertNull(migrationJob.ProcessInstanceId);
		  assertNull(migrationJob.ExecutionId);
		}

		// and the seed job still exists
		Job seedJob = helper.getJobForDefinition(seedJobDefinition);
		assertNotNull(seedJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobCreation()
	  public virtual void testMonitorJobCreation()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(10);

		// when
		helper.executeSeedJob(batch);

		// then the seed job definition still exists but the seed job is removed
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);

		Job seedJob = helper.getSeedJob(batch);
		assertNull(seedJob);

		// and a monitor job definition and job exists
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertNotNull(monitorJobDefinition);

		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationJobsExecution()
	  public virtual void testMigrationJobsExecution()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(10);
		helper.executeSeedJob(batch);
		IList<Job> migrationJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job migrationJob in migrationJobs)
		{
		  helper.executeJob(migrationJob);
		}

		// then all process instances where migrated
		assertEquals(0, helper.countSourceProcessInstances());
		assertEquals(10, helper.countTargetProcessInstances());

		// and the no migration jobs exist
		assertEquals(0, helper.getExecutionJobs(batch).Count);

		// but a monitor job exists
		assertNotNull(helper.getMonitorJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrationJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
	  public virtual void testMigrationJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = engineRule.ProcessEngineConfiguration;

		processEngineConfiguration.AuthorizationEnabled = true;

		try
		{
		  Batch batch = helper.migrateProcessInstancesAsyncForTenant(10, "someTenantId");
		  helper.executeSeedJob(batch);

		  testRule.waitForJobExecutorToProcessAllJobs();

		  // then all process instances where migrated
		  assertEquals(0, helper.countSourceProcessInstances());
		  assertEquals(10, helper.countTargetProcessInstances());

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNumberOfJobsCreatedBySeedJobPerInvocation()
	  public virtual void testNumberOfJobsCreatedBySeedJobPerInvocation()
	  {
		// reduce number of batch jobs per seed to not have to create a lot of instances
		int batchJobsPerSeed = 10;
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;

		Batch batch = helper.migrateProcessInstancesAsync(batchJobsPerSeed * 2 + 4);

		// when
		helper.executeSeedJob(batch);

		// then the default number of jobs was created
		assertEquals(batch.BatchJobsPerSeed, helper.getExecutionJobs(batch).Count);

		// when the seed job is executed a second time
		helper.executeSeedJob(batch);

		// then the same amount of jobs was created
		assertEquals(2 * batch.BatchJobsPerSeed, helper.getExecutionJobs(batch).Count);

		// when the seed job is executed a third time
		helper.executeSeedJob(batch);

		// then the all jobs where created
		assertEquals(2 * batch.BatchJobsPerSeed + 4, helper.getExecutionJobs(batch).Count);

		// and the seed job is removed
		assertNull(helper.getSeedJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultBatchConfiguration()
	  public virtual void testDefaultBatchConfiguration()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		assertEquals(100, configuration.BatchJobsPerSeed);
		assertEquals(1, configuration.InvocationsPerBatchJob);
		assertEquals(30, configuration.BatchPollTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomNumberOfJobsCreateBySeedJob()
	  public virtual void testCustomNumberOfJobsCreateBySeedJob()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		configuration.BatchJobsPerSeed = 2;
		configuration.InvocationsPerBatchJob = 5;

		// when
		Batch batch = helper.migrateProcessInstancesAsync(20);

		// then the configuration was saved in the batch job
		assertEquals(2, batch.BatchJobsPerSeed);
		assertEquals(5, batch.InvocationsPerBatchJob);

		// and the size was correctly calculated
		assertEquals(4, batch.TotalJobs);

		// when the seed job is executed
		helper.executeSeedJob(batch);

		// then there exist the first batch of migration jobs
		assertEquals(2, helper.getExecutionJobs(batch).Count);

		// when the seed job is executed a second time
		helper.executeSeedJob(batch);

		// then the full batch of migration jobs exist
		assertEquals(4, helper.getExecutionJobs(batch).Count);

		// and the seed job is removed
		assertNull(helper.getSeedJob(batch));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobPollingForCompletion()
	  public virtual void testMonitorJobPollingForCompletion()
	  {
		ClockUtil.CurrentTime = TEST_DATE;

		Batch batch = helper.migrateProcessInstancesAsync(10);

		// when the seed job creates the monitor job
		DateTime createDate = TEST_DATE;
		helper.executeSeedJob(batch);

		// then the monitor job has a no due date set
		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
		assertEquals(currentTime, monitorJob.Duedate);

		// when the monitor job is executed
		helper.executeMonitorJob(batch);

		// then the monitor job has a due date of the default batch poll time
		monitorJob = helper.getMonitorJob(batch);
		DateTime dueDate = helper.addSeconds(createDate, 30);
		assertEquals(dueDate, monitorJob.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobRemovesBatchAfterCompletion()
	  public virtual void testMonitorJobRemovesBatchAfterCompletion()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(10);
		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		// when
		helper.executeMonitorJob(batch);

		// then the batch was completed and removed
		assertEquals(0, managementService.createBatchQuery().count());

		// and the seed jobs was removed
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchDeletionWithCascade()
	  public virtual void testBatchDeletionWithCascade()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(10);
		helper.executeSeedJob(batch);

		// when
		managementService.deleteBatch(batch.Id, true);

		// then the batch was deleted
		assertEquals(0, managementService.createBatchQuery().count());

		// and the seed and migration job definition were deleted
		assertEquals(0, managementService.createJobDefinitionQuery().count());

		// and the seed job and migration jobs were deleted
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchDeletionWithoutCascade()
	  public virtual void testBatchDeletionWithoutCascade()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(10);
		helper.executeSeedJob(batch);

		// when
		managementService.deleteBatch(batch.Id, false);

		// then the batch was deleted
		assertEquals(0, managementService.createBatchQuery().count());

		// and the seed and migration job definition were deleted
		assertEquals(0, managementService.createJobDefinitionQuery().count());

		// and the seed job and migration jobs were deleted
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedSeedJobDeletionWithCascade()
	  public virtual void testBatchWithFailedSeedJobDeletionWithCascade()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(2);

		// create incident
		Job seedJob = helper.getSeedJob(batch);
		managementService.setJobRetries(seedJob.Id, 0);

		// when
		managementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = historyService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedMigrationJobDeletionWithCascade()
	  public virtual void testBatchWithFailedMigrationJobDeletionWithCascade()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(2);
		helper.executeSeedJob(batch);

		// create incidents
		IList<Job> migrationJobs = helper.getExecutionJobs(batch);
		foreach (Job migrationJob in migrationJobs)
		{
		  managementService.setJobRetries(migrationJob.Id, 0);
		}

		// when
		managementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = historyService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedMonitorJobDeletionWithCascade()
	  public virtual void testBatchWithFailedMonitorJobDeletionWithCascade()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(2);
		helper.executeSeedJob(batch);

		// create incident
		Job monitorJob = helper.getMonitorJob(batch);
		managementService.setJobRetries(monitorJob.Id, 0);

		// when
		managementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = historyService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchExecutionFailureWithMissingProcessInstance()
	  public virtual void testBatchExecutionFailureWithMissingProcessInstance()
	  {
		Batch batch = helper.migrateProcessInstancesAsync(2);
		helper.executeSeedJob(batch);

		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().list();
		string deletedProcessInstanceId = processInstances[0].Id;

		// when
		runtimeService.deleteProcessInstance(deletedProcessInstanceId, "test");
		helper.executeJobs(batch);

		// then the remaining process instance was migrated
		assertEquals(0, helper.countSourceProcessInstances());
		assertEquals(1, helper.countTargetProcessInstances());

		// and one batch job failed and has 2 retries left
		IList<Job> migrationJobs = helper.getExecutionJobs(batch);
		assertEquals(1, migrationJobs.Count);

		Job failedJob = migrationJobs[0];
		assertEquals(2, failedJob.Retries);
		assertThat(failedJob.ExceptionMessage, startsWith("ENGINE-23003"));
		assertThat(failedJob.ExceptionMessage, containsString("Process instance '" + deletedProcessInstanceId + "' cannot be migrated"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchCreationWithProcessInstanceQuery()
	  public virtual void testBatchCreationWithProcessInstanceQuery()
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;
		int processInstanceCount = 15;

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		for (int i = 0; i < processInstanceCount; i++)
		{
		  runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		}

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		assertEquals(processInstanceCount, sourceProcessInstanceQuery.count());

		// when
		Batch batch = runtimeService.newMigration(migrationPlan).processInstanceQuery(sourceProcessInstanceQuery).executeAsync();

		// then a batch is created
		assertBatchCreated(batch, processInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
	  public virtual void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;
		int processInstanceCount = 15;

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		IList<string> processInstanceIds = new List<string>();
		for (int i = 0; i < processInstanceCount; i++)
		{
		  processInstanceIds.Add(runtimeService.startProcessInstanceById(sourceProcessDefinition.Id).Id);
		}

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		assertEquals(processInstanceCount, sourceProcessInstanceQuery.count());

		// when
		Batch batch = runtimeService.newMigration(migrationPlan).processInstanceIds(processInstanceIds).processInstanceQuery(sourceProcessInstanceQuery).executeAsync();

		// then a batch is created
		assertBatchCreated(batch, processInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListenerInvocationForNewlyCreatedScope()
	  public virtual void testListenerInvocationForNewlyCreatedScope()
	  {
		// given
		DelegateEvent.clearEvents();

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(DelegateExecutionListener).FullName).done());

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Batch batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();
		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		IList<DelegateEvent> recordedEvents = DelegateEvent.Events;
		assertEquals(1, recordedEvents.Count);

		DelegateEvent @event = recordedEvents[0];
		assertEquals(targetProcessDefinition.Id, @event.ProcessDefinitionId);
		assertEquals("subProcess", @event.CurrentActivityId);

		DelegateEvent.clearEvents();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipListenerInvocationForNewlyCreatedScope()
	  public virtual void testSkipListenerInvocationForNewlyCreatedScope()
	  {
		// given
		DelegateEvent.clearEvents();

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(DelegateExecutionListener).FullName).done());

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Batch batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).skipCustomListeners().executeAsync();
		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		assertEquals(0, DelegateEvent.Events.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIoMappingInvocationForNewlyCreatedScope()
	  public virtual void testIoMappingInvocationForNewlyCreatedScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaInputParameter("foo", "bar").done());

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Batch batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();
		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		VariableInstance inputVariable = engineRule.RuntimeService.createVariableInstanceQuery().singleResult();
		Assert.assertNotNull(inputVariable);
		assertEquals("foo", inputVariable.Name);
		assertEquals("bar", inputVariable.Value);

		ActivityInstance activityInstance = engineRule.RuntimeService.getActivityInstance(processInstance.Id);
		assertEquals(activityInstance.getActivityInstances("subProcess")[0].Id, inputVariable.ActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipIoMappingInvocationForNewlyCreatedScope()
	  public virtual void testSkipIoMappingInvocationForNewlyCreatedScope()
	  {
	 // given
		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaInputParameter("foo", "bar").done());

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Batch batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).skipIoMappings().executeAsync();
		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then
		assertEquals(0, engineRule.RuntimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateEventTrigger()
	  public virtual void testUpdateEventTrigger()
	  {
		// given
		string newMessageName = "newMessage";

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_RECEIVE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(modify(ProcessModels.ONE_RECEIVE_TASK_PROCESS).renameMessage("Message", newMessageName));

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().updateEventTriggers().build();

		Batch batch = runtimeService.newMigration(migrationPlan).processInstanceIds(Collections.singletonList(processInstance.Id)).executeAsync();

		helper.executeSeedJob(batch);

		// when
		helper.executeJobs(batch);

		// then the message event subscription's event name was changed
		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertEquals(newMessageName, eventSubscription.EventName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteBatchJobManually()
	  public virtual void testDeleteBatchJobManually()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(1);
		helper.executeSeedJob(batch);

		JobEntity migrationJob = (JobEntity) helper.getExecutionJobs(batch)[0];
		string byteArrayId = migrationJob.JobHandlerConfigurationRaw;

		ByteArrayEntity byteArrayEntity = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(this, byteArrayId));
		assertNotNull(byteArrayEntity);

		// when
		managementService.deleteJob(migrationJob.Id);

		// then
		byteArrayEntity = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(this, byteArrayId));
		assertNull(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateWithVarargsArray()
	  public virtual void testMigrateWithVarargsArray()
	  {
		ProcessDefinition sourceDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = migrationRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceById(sourceDefinition.Id);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceById(sourceDefinition.Id);

		// when
		Batch batch = runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.executeSeedJob(batch);
		helper.executeJobs(batch);
		helper.executeMonitorJob(batch);

		// then
		Assert.assertEquals(2, runtimeService.createProcessInstanceQuery().processDefinitionId(targetDefinition.Id).count());
	  }

	  protected internal virtual void assertBatchCreated(Batch batch, int processInstanceCount)
	  {
		assertNotNull(batch);
		assertNotNull(batch.Id);
		assertEquals("instance-migration", batch.Type);
		assertEquals(processInstanceCount, batch.TotalJobs);
		assertEquals(defaultBatchJobsPerSeed, batch.BatchJobsPerSeed);
		assertEquals(defaultInvocationsPerBatchJob, batch.InvocationsPerBatchJob);
	  }

	  public class GetByteArrayCommand : Command<ByteArrayEntity>
	  {
		  private readonly BatchMigrationTest outerInstance;


		protected internal string byteArrayId;

		public GetByteArrayCommand(BatchMigrationTest outerInstance, string byteArrayId)
		{
			this.outerInstance = outerInstance;
		  this.byteArrayId = byteArrayId;
		}

		public virtual ByteArrayEntity execute(CommandContext commandContext)
		{
		  return (ByteArrayEntity) commandContext.DbEntityManager.selectOne("selectByteArray", byteArrayId);
		}

	  }

	}

}