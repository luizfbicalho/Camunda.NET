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
namespace org.camunda.bpm.engine.test.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class JobExecutorBatchTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobExecutorBatchTest()
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


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule);
	  public RuleChain ruleChain;
	  public CountingJobExecutor jobExecutor;
	  protected internal JobExecutor defaultJobExecutor;
	  protected internal int defaultBatchJobsPerSeed;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void replaceJobExecutor() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void replaceJobExecutor()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		defaultJobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor = new CountingJobExecutor(this);
		processEngineConfiguration.JobExecutor = jobExecutor;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void saveBatchJobsPerSeed()
	  public virtual void saveBatchJobsPerSeed()
	  {
		defaultBatchJobsPerSeed = engineRule.ProcessEngineConfiguration.BatchJobsPerSeed;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetJobExecutor()
	  public virtual void resetJobExecutor()
	  {
		engineRule.ProcessEngineConfiguration.JobExecutor = defaultJobExecutor;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetBatchJobsPerSeed()
	  public virtual void resetBatchJobsPerSeed()
	  {
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobExecutorHintedOnBatchCreation()
	  public virtual void testJobExecutorHintedOnBatchCreation()
	  {
		// given
		jobExecutor.startRecord();

		// when a batch is created
		helper.migrateProcessInstancesAsync(2);

		// then the job executor is hinted for the seed job
		assertEquals(1, jobExecutor.JobsAdded);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobExecutorHintedSeedJobExecution()
	  public virtual void testJobExecutorHintedSeedJobExecution()
	  {
		// reduce number of batch jobs per seed to not have to create a lot of instances
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;

		// given
		Batch batch = helper.migrateProcessInstancesAsync(13);
		jobExecutor.startRecord();

		// when the seed job is executed
		helper.executeSeedJob(batch);

		// then the job executor is hinted for the seed job and 10 execution jobs
		assertEquals(11, jobExecutor.JobsAdded);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobExecutorHintedSeedJobCompletion()
	  public virtual void testJobExecutorHintedSeedJobCompletion()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(3);
		jobExecutor.startRecord();

		// when the seed job is executed
		helper.executeSeedJob(batch);

		// then the job executor is hinted for the monitor job and 3 execution jobs
		assertEquals(4, jobExecutor.JobsAdded);
	  }

	  public class CountingJobExecutor : JobExecutor
	  {
		  private readonly JobExecutorBatchTest outerInstance;

		  public CountingJobExecutor(JobExecutorBatchTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public bool record = false;
		public long jobsAdded = 0;

		public override bool Active
		{
			get
			{
			  return true;
			}
		}

		protected internal override void startExecutingJobs()
		{
		  // do nothing
		}

		protected internal override void stopExecutingJobs()
		{
		  // do nothing
		}

		public override void executeJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
		{
		  // do nothing
		}

		public virtual void startRecord()
		{
		  resetJobsAdded();
		  record = true;
		}

		public override void jobWasAdded()
		{
		  if (record)
		  {
			jobsAdded++;
		  }
		}

		public virtual long JobsAdded
		{
			get
			{
			  return jobsAdded;
			}
		}

		public virtual void resetJobsAdded()
		{
		  jobsAdded = 0;
		}

	  }

	}

}