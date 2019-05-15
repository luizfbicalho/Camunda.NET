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
//	import static org.junit.Assert.assertEquals;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultJobPriorityProvider = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobPriorityProvider;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class BatchPriorityTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchPriorityTest()
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


	  public static readonly long CUSTOM_PRIORITY = DefaultJobPriorityProvider.DEFAULT_PRIORITY + 10;

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
	  protected internal long defaultBatchJobPriority;

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
//ORIGINAL LINE: @Before public void saveAndReduceBatchConfiguration()
	  public virtual void saveAndReduceBatchConfiguration()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		defaultBatchJobPriority = configuration.BatchJobPriority;
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
		ProcessEngineConfigurationImpl processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		processEngineConfiguration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
		processEngineConfiguration.BatchJobPriority = defaultBatchJobPriority;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void seedJobShouldHaveDefaultPriority()
	  public virtual void seedJobShouldHaveDefaultPriority()
	  {
		// when
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// then
		Job seedJob = helper.getSeedJob(batch);
		assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY, seedJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void monitorJobShouldHaveDefaultPriority()
	  public virtual void monitorJobShouldHaveDefaultPriority()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		helper.executeSeedJob(batch);

		// then
		Job monitorJob = helper.getMonitorJob(batch);
		assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY, monitorJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void batchExecutionJobShouldHaveDefaultPriority()
	  public virtual void batchExecutionJobShouldHaveDefaultPriority()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		helper.executeSeedJob(batch);

		// then
		Job executionJob = helper.getExecutionJobs(batch)[0];
		assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY, executionJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void seedJobShouldGetPriorityFromProcessEngineConfiguration()
	  public virtual void seedJobShouldGetPriorityFromProcessEngineConfiguration()
	  {
		// given
		BatchJobPriority = CUSTOM_PRIORITY;

		// when
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// then
		Job seedJob = helper.getSeedJob(batch);
		assertEquals(CUSTOM_PRIORITY, seedJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void monitorJobShouldGetPriorityFromProcessEngineConfiguration()
	  public virtual void monitorJobShouldGetPriorityFromProcessEngineConfiguration()
	  {
		// given
		BatchJobPriority = CUSTOM_PRIORITY;
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		helper.executeSeedJob(batch);

		// then
		Job monitorJob = helper.getMonitorJob(batch);
		assertEquals(CUSTOM_PRIORITY, monitorJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executionJobShouldGetPriorityFromProcessEngineConfiguration()
	  public virtual void executionJobShouldGetPriorityFromProcessEngineConfiguration()
	  {
		// given
		BatchJobPriority = CUSTOM_PRIORITY;
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		helper.executeSeedJob(batch);

		// then
		Job executionJob = helper.getExecutionJobs(batch)[0];
		assertEquals(CUSTOM_PRIORITY, executionJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void seedJobShouldGetPriorityFromOverridingJobDefinitionPriority()
	  public virtual void seedJobShouldGetPriorityFromOverridingJobDefinitionPriority()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(2);
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		managementService.setOverridingJobPriorityForJobDefinition(seedJobDefinition.Id, CUSTOM_PRIORITY);

		// when
		helper.executeSeedJob(batch);

		// then
		Job seedJob = helper.getSeedJob(batch);
		assertEquals(CUSTOM_PRIORITY, seedJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void seedJobShouldGetPriorityFromOverridingJobDefinitionPriorityWithCascade()
	  public virtual void seedJobShouldGetPriorityFromOverridingJobDefinitionPriorityWithCascade()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);

		// when
		managementService.setOverridingJobPriorityForJobDefinition(seedJobDefinition.Id, CUSTOM_PRIORITY, true);

		// then
		Job seedJob = helper.getSeedJob(batch);
		assertEquals(CUSTOM_PRIORITY, seedJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void monitorJobShouldGetPriorityOverridingJobDefinitionPriority()
	  public virtual void monitorJobShouldGetPriorityOverridingJobDefinitionPriority()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		managementService.setOverridingJobPriorityForJobDefinition(monitorJobDefinition.Id, CUSTOM_PRIORITY);

		// when
		helper.executeSeedJob(batch);

		// then
		Job monitorJob = helper.getMonitorJob(batch);
		assertEquals(CUSTOM_PRIORITY, monitorJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void monitorJobShouldGetPriorityOverridingJobDefinitionPriorityWithCascade()
	  public virtual void monitorJobShouldGetPriorityOverridingJobDefinitionPriorityWithCascade()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		helper.executeSeedJob(batch);

		// when
		managementService.setOverridingJobPriorityForJobDefinition(monitorJobDefinition.Id, CUSTOM_PRIORITY, true);

		// then
		Job monitorJob = helper.getMonitorJob(batch);
		assertEquals(CUSTOM_PRIORITY, monitorJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executionJobShouldGetPriorityFromOverridingJobDefinitionPriority()
	  public virtual void executionJobShouldGetPriorityFromOverridingJobDefinitionPriority()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		JobDefinition executionJobDefinition = helper.getExecutionJobDefinition(batch);
		managementService.setOverridingJobPriorityForJobDefinition(executionJobDefinition.Id, CUSTOM_PRIORITY, true);

		// when
		helper.executeSeedJob(batch);

		// then
		Job executionJob = helper.getExecutionJobs(batch)[0];
		assertEquals(CUSTOM_PRIORITY, executionJob.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executionJobShouldGetPriorityFromOverridingJobDefinitionPriorityWithCascade()
	  public virtual void executionJobShouldGetPriorityFromOverridingJobDefinitionPriorityWithCascade()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		JobDefinition executionJobDefinition = helper.getExecutionJobDefinition(batch);
		helper.executeSeedJob(batch);

		// when
		managementService.setOverridingJobPriorityForJobDefinition(executionJobDefinition.Id, CUSTOM_PRIORITY, true);

		// then
		Job executionJob = helper.getExecutionJobs(batch)[0];
		assertEquals(CUSTOM_PRIORITY, executionJob.Priority);
	  }

	  protected internal virtual long BatchJobPriority
	  {
		  set
		  {
			engineRule.ProcessEngineConfiguration.BatchJobPriority = value;
		  }
	  }
	}

}