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
namespace org.camunda.bpm.engine.test.api.mgmt
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.batchStatisticsById;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsCollectionContaining.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using BatchStatisticsQuery = org.camunda.bpm.engine.batch.BatchStatisticsQuery;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class BatchStatisticsQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchStatisticsQueryTest()
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

	  protected internal ManagementService managementService;
	  protected internal int defaultBatchJobsPerSeed;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void saveAndReduceBatchJobsPerSeed()
	  public virtual void saveAndReduceBatchJobsPerSeed()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		// reduce number of batch jobs per seed to not have to create a lot of instances
		configuration.BatchJobsPerSeed = 10;
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
//ORIGINAL LINE: @Test public void testQuery()
	  public virtual void testQuery()
	  {
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().list();
		assertEquals(0, statistics.Count);

		Batch batch1 = helper.createMigrationBatchWithSize(1);

		statistics = managementService.createBatchStatisticsQuery().list();
		assertEquals(1, statistics.Count);
		assertEquals(batch1.Id, statistics[0].Id);

		Batch batch2 = helper.createMigrationBatchWithSize(1);
		Batch batch3 = helper.createMigrationBatchWithSize(1);

		statistics = managementService.createBatchStatisticsQuery().list();
		assertEquals(3, statistics.Count);

		helper.completeBatch(batch1);
		helper.completeBatch(batch3);

		statistics = managementService.createBatchStatisticsQuery().list();
		assertEquals(1, statistics.Count);
		assertEquals(batch2.Id, statistics[0].Id);

		helper.completeBatch(batch2);

		statistics = managementService.createBatchStatisticsQuery().list();
		assertEquals(0, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		long count = managementService.createBatchStatisticsQuery().count();
		assertEquals(0, count);

		Batch batch1 = helper.createMigrationBatchWithSize(1);

		count = managementService.createBatchStatisticsQuery().count();
		assertEquals(1, count);

		Batch batch2 = helper.createMigrationBatchWithSize(1);
		Batch batch3 = helper.createMigrationBatchWithSize(1);

		count = managementService.createBatchStatisticsQuery().count();
		assertEquals(3, count);

		helper.completeBatch(batch1);
		helper.completeBatch(batch3);

		count = managementService.createBatchStatisticsQuery().count();
		assertEquals(1, count);

		helper.completeBatch(batch2);

		count = managementService.createBatchStatisticsQuery().count();
		assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryById()
	  public virtual void testQueryById()
	  {
		// given
		helper.createMigrationBatchWithSize(1);
		Batch batch = helper.createMigrationBatchWithSize(1);

		// when
		BatchStatistics statistics = managementService.createBatchStatisticsQuery().batchId(batch.Id).singleResult();

		// then
		assertEquals(batch.Id, statistics.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullId()
	  public virtual void testQueryByNullId()
	  {
		try
		{
		  managementService.createBatchStatisticsQuery().batchId(null).singleResult();
		  Assert.fail("exception expected");
		}
		catch (NullValueException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Batch id is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnknownId()
	  public virtual void testQueryByUnknownId()
	  {
		// given
		helper.createMigrationBatchWithSize(1);
		helper.createMigrationBatchWithSize(1);

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().batchId("unknown").list();

		// then
		assertEquals(0, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByType()
	  public virtual void testQueryByType()
	  {
		// given
		helper.createMigrationBatchWithSize(1);
		helper.createMigrationBatchWithSize(1);

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MIGRATION).list();

		// then
		assertEquals(2, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullType()
	  public virtual void testQueryByNullType()
	  {
		try
		{
		  managementService.createBatchStatisticsQuery().type(null).list();
		  Assert.fail("exception expected");
		}
		catch (NullValueException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Type is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnknownType()
	  public virtual void testQueryByUnknownType()
	  {
		// given
		helper.createMigrationBatchWithSize(1);
		helper.createMigrationBatchWithSize(1);

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().type("unknown").list();

		// then
		assertEquals(0, statistics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryOrderByIdAsc()
	  public virtual void testQueryOrderByIdAsc()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().orderById().asc().list();

		// then
		verifySorting(statistics, batchStatisticsById());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryOrderByIdDec()
	  public virtual void testQueryOrderByIdDec()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().orderById().desc().list();

		// then
		verifySorting(statistics, inverted(batchStatisticsById()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryOrderingPropertyWithoutOrder()
	  public virtual void testQueryOrderingPropertyWithoutOrder()
	  {
		try
		{
		  managementService.createBatchStatisticsQuery().orderById().list();
		  Assert.fail("exception expected");
		}
		catch (NotValidException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Invalid query: " + "call asc() or desc() after using orderByXX()"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryOrderWithoutOrderingProperty()
	  public virtual void testQueryOrderWithoutOrderingProperty()
	  {
		try
		{
		  managementService.createBatchStatisticsQuery().asc().list();
		  Assert.fail("exception expected");
		}
		catch (NotValidException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("You should call any of the orderBy methods " + "first before specifying a direction"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsNoExecutionJobsGenerated()
	  public virtual void testStatisticsNoExecutionJobsGenerated()
	  {
		// given
		helper.createMigrationBatchWithSize(3);

		// when
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		// then
		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(0, batchStatistics.JobsCreated);
		assertEquals(3, batchStatistics.RemainingJobs);
		assertEquals(0, batchStatistics.CompletedJobs);
		assertEquals(0, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsMostExecutionJobsGenerated()
	  public virtual void testStatisticsMostExecutionJobsGenerated()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(13);

		// when
		helper.executeJob(helper.getSeedJob(batch));

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(13, batchStatistics.TotalJobs);
		assertEquals(10, batchStatistics.JobsCreated);
		assertEquals(13, batchStatistics.RemainingJobs);
		assertEquals(0, batchStatistics.CompletedJobs);
		assertEquals(0, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsAllExecutionJobsGenerated()
	  public virtual void testStatisticsAllExecutionJobsGenerated()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(3);

		// when
		helper.completeSeedJobs(batch);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(3, batchStatistics.JobsCreated);
		assertEquals(3, batchStatistics.RemainingJobs);
		assertEquals(0, batchStatistics.CompletedJobs);
		assertEquals(0, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsOneCompletedJob()
	  public virtual void testStatisticsOneCompletedJob()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(3);

		// when
		helper.completeSeedJobs(batch);
		helper.completeJobs(batch, 1);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(3, batchStatistics.JobsCreated);
		assertEquals(2, batchStatistics.RemainingJobs);
		assertEquals(1, batchStatistics.CompletedJobs);
		assertEquals(0, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsOneFailedJob()
	  public virtual void testStatisticsOneFailedJob()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(3);

		// when
		helper.completeSeedJobs(batch);
		helper.failExecutionJobs(batch, 1);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(3, batchStatistics.JobsCreated);
		assertEquals(3, batchStatistics.RemainingJobs);
		assertEquals(0, batchStatistics.CompletedJobs);
		assertEquals(1, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsOneCompletedAndOneFailedJob()
	  public virtual void testStatisticsOneCompletedAndOneFailedJob()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(3);

		// when
		helper.completeSeedJobs(batch);
		helper.completeJobs(batch, 1);
		helper.failExecutionJobs(batch, 1);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(3, batchStatistics.JobsCreated);
		assertEquals(2, batchStatistics.RemainingJobs);
		assertEquals(1, batchStatistics.CompletedJobs);
		assertEquals(1, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsRetriedFailedJobs()
	  public virtual void testStatisticsRetriedFailedJobs()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(3);

		// when
		helper.completeSeedJobs(batch);
		helper.failExecutionJobs(batch, 3);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(3, batchStatistics.JobsCreated);
		assertEquals(3, batchStatistics.RemainingJobs);
		assertEquals(0, batchStatistics.CompletedJobs);
		assertEquals(3, batchStatistics.FailedJobs);

		// when
		helper.setRetries(batch, 3, 1);
		helper.completeJobs(batch, 3);

		// then
		batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(3, batchStatistics.TotalJobs);
		assertEquals(3, batchStatistics.JobsCreated);
		assertEquals(0, batchStatistics.RemainingJobs);
		assertEquals(3, batchStatistics.CompletedJobs);
		assertEquals(0, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsWithDeletedJobs()
	  public virtual void testStatisticsWithDeletedJobs()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(13);

		// when
		helper.executeJob(helper.getSeedJob(batch));
		deleteMigrationJobs(batch);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(13, batchStatistics.TotalJobs);
		assertEquals(10, batchStatistics.JobsCreated);
		assertEquals(3, batchStatistics.RemainingJobs);
		assertEquals(10, batchStatistics.CompletedJobs);
		assertEquals(0, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsWithNotAllGeneratedAndAlreadyCompletedAndFailedJobs()
	  public virtual void testStatisticsWithNotAllGeneratedAndAlreadyCompletedAndFailedJobs()
	  {
		// given
		Batch batch = helper.createMigrationBatchWithSize(13);

		// when
		helper.executeJob(helper.getSeedJob(batch));
		helper.completeJobs(batch, 2);
		helper.failExecutionJobs(batch, 2);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().singleResult();

		assertEquals(13, batchStatistics.TotalJobs);
		assertEquals(10, batchStatistics.JobsCreated);
		assertEquals(11, batchStatistics.RemainingJobs);
		assertEquals(2, batchStatistics.CompletedJobs);
		assertEquals(2, batchStatistics.FailedJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleBatchesStatistics()
	  public virtual void testMultipleBatchesStatistics()
	  {
		// given
		Batch batch1 = helper.createMigrationBatchWithSize(3);
		Batch batch2 = helper.createMigrationBatchWithSize(13);
		Batch batch3 = helper.createMigrationBatchWithSize(15);

		// when
		helper.executeJob(helper.getSeedJob(batch2));
		helper.completeJobs(batch2, 2);
		helper.failExecutionJobs(batch2, 3);

		helper.executeJob(helper.getSeedJob(batch3));
		deleteMigrationJobs(batch3);
		helper.executeJob(helper.getSeedJob(batch3));
		helper.completeJobs(batch3, 2);
		helper.failExecutionJobs(batch3, 3);

		// then
		IList<BatchStatistics> batchStatisticsList = managementService.createBatchStatisticsQuery().list();

		foreach (BatchStatistics batchStatistics in batchStatisticsList)
		{
		  if (batch1.Id.Equals(batchStatistics.Id))
		  {
			// batch 1
			assertEquals(3, batchStatistics.TotalJobs);
			assertEquals(0, batchStatistics.JobsCreated);
			assertEquals(3, batchStatistics.RemainingJobs);
			assertEquals(0, batchStatistics.CompletedJobs);
			assertEquals(0, batchStatistics.FailedJobs);
		  }
		  else if (batch2.Id.Equals(batchStatistics.Id))
		  {
			// batch 2
			assertEquals(13, batchStatistics.TotalJobs);
			assertEquals(10, batchStatistics.JobsCreated);
			assertEquals(11, batchStatistics.RemainingJobs);
			assertEquals(2, batchStatistics.CompletedJobs);
			assertEquals(3, batchStatistics.FailedJobs);
		  }
		  else if (batch3.Id.Equals(batchStatistics.Id))
		  {
			// batch 3
			assertEquals(15, batchStatistics.TotalJobs);
			assertEquals(15, batchStatistics.JobsCreated);
			assertEquals(3, batchStatistics.RemainingJobs);
			assertEquals(12, batchStatistics.CompletedJobs);
			assertEquals(3, batchStatistics.FailedJobs);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsSuspend()
	  public virtual void testStatisticsSuspend()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		managementService.suspendBatchById(batch.Id);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().batchId(batch.Id).singleResult();

		assertTrue(batchStatistics.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsActivate()
	  public virtual void testStatisticsActivate()
	  {
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);
		managementService.suspendBatchById(batch.Id);

		// when
		managementService.activateBatchById(batch.Id);

		// then
		BatchStatistics batchStatistics = managementService.createBatchStatisticsQuery().batchId(batch.Id).singleResult();

		assertFalse(batchStatistics.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsQueryBySuspendedBatches()
	  public virtual void testStatisticsQueryBySuspendedBatches()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		managementService.suspendBatchById(batch1.Id);
		managementService.suspendBatchById(batch2.Id);
		managementService.activateBatchById(batch1.Id);

		// then
		BatchStatisticsQuery query = managementService.createBatchStatisticsQuery().suspended();
		Assert.assertEquals(1, query.count());
		Assert.assertEquals(1, query.list().size());
		Assert.assertEquals(batch2.Id, query.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsQueryByActiveBatches()
	  public virtual void testStatisticsQueryByActiveBatches()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);
		Batch batch3 = helper.migrateProcessInstancesAsync(1);

		// when
		managementService.suspendBatchById(batch1.Id);
		managementService.suspendBatchById(batch2.Id);
		managementService.activateBatchById(batch1.Id);

		// then
		BatchStatisticsQuery query = managementService.createBatchStatisticsQuery().active();
		Assert.assertEquals(2, query.count());
		Assert.assertEquals(2, query.list().size());

		IList<string> foundIds = new List<string>();
		foreach (Batch batch in query.list())
		{
		  foundIds.Add(batch.Id);
		}
		assertThat(foundIds, hasItems(batch1.Id, batch3.Id));
	  }

	  protected internal virtual void deleteMigrationJobs(Batch batch)
	  {
		foreach (Job migrationJob in helper.getExecutionJobs(batch))
		{
		  managementService.deleteJob(migrationJob.Id);
		}
	  }

	}

}