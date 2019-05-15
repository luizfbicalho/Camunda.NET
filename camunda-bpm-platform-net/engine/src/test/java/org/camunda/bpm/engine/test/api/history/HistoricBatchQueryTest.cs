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
namespace org.camunda.bpm.engine.test.api.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicBatchByEndTime;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicBatchById;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicBatchByStartTime;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricBatchQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricBatchQueryTest()
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

	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
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
//ORIGINAL LINE: @Test public void testBatchQuery()
	  public virtual void testBatchQuery()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);

		// when
		IList<HistoricBatch> list = historyService.createHistoricBatchQuery().list();

		// then
		assertEquals(2, list.Count);

		IList<string> batchIds = new List<string>();
		foreach (HistoricBatch resultBatch in list)
		{
		  batchIds.Add(resultBatch.Id);
		}

		Assert.assertTrue(batchIds.Contains(batch1.Id));
		Assert.assertTrue(batchIds.Contains(batch2.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryResult()
	  public virtual void testBatchQueryResult()
	  {
		DateTime startDate = new DateTime(10000L);
		DateTime endDate = new DateTime(40000L);

		// given
		ClockUtil.CurrentTime = startDate;
		Batch batch = helper.migrateProcessInstancesAsync(1);
		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		ClockUtil.CurrentTime = endDate;
		helper.executeMonitorJob(batch);

		// when
		HistoricBatch resultBatch = historyService.createHistoricBatchQuery().singleResult();

		// then
		Assert.assertNotNull(resultBatch);

		assertEquals(batch.Id, resultBatch.Id);
		assertEquals(batch.BatchJobDefinitionId, resultBatch.BatchJobDefinitionId);
		assertEquals(batch.MonitorJobDefinitionId, resultBatch.MonitorJobDefinitionId);
		assertEquals(batch.SeedJobDefinitionId, resultBatch.SeedJobDefinitionId);
		assertEquals(batch.TenantId, resultBatch.TenantId);
		assertEquals(batch.Type, resultBatch.Type);
		assertEquals(batch.BatchJobsPerSeed, resultBatch.BatchJobsPerSeed);
		assertEquals(batch.InvocationsPerBatchJob, resultBatch.InvocationsPerBatchJob);
		assertEquals(batch.TotalJobs, resultBatch.TotalJobs);
		assertEquals(startDate, resultBatch.StartTime);
		assertEquals(endDate, resultBatch.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryById()
	  public virtual void testBatchQueryById()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		HistoricBatch resultBatch = historyService.createHistoricBatchQuery().batchId(batch1.Id).singleResult();

		// then
		Assert.assertNotNull(resultBatch);
		assertEquals(batch1.Id, resultBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByIdNull()
	  public virtual void testBatchQueryByIdNull()
	  {
		try
		{
		  historyService.createHistoricBatchQuery().batchId(null).singleResult();
		  Assert.fail("exception expected");
		}
		catch (NullValueException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Batch id is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByType()
	  public virtual void testBatchQueryByType()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		long count = historyService.createHistoricBatchQuery().type(batch1.Type).count();

		// then
		assertEquals(2, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByNonExistingType()
	  public virtual void testBatchQueryByNonExistingType()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);

		// when
		long count = historyService.createHistoricBatchQuery().type("foo").count();

		// then
		assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchByState()
	  public virtual void testBatchByState()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);

		helper.completeBatch(batch1);

		// when
		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().completed(true).singleResult();

		// then
		assertEquals(batch1.Id, historicBatch.Id);

		// when
		historicBatch = historyService.createHistoricBatchQuery().completed(false).singleResult();

		// then
		assertEquals(batch2.Id, historicBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByTypeNull()
	  public virtual void testBatchQueryByTypeNull()
	  {
		try
		{
		  historyService.createHistoricBatchQuery().type(null).singleResult();
		  Assert.fail("exception expected");
		}
		catch (NullValueException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Type is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryCount()
	  public virtual void testBatchQueryCount()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		long count = historyService.createHistoricBatchQuery().count();

		// then
		assertEquals(2, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByIdAsc()
	  public virtual void testBatchQueryOrderByIdAsc()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderById().asc().list();

		// then
		verifySorting(orderedBatches, historicBatchById());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByIdDec()
	  public virtual void testBatchQueryOrderByIdDec()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderById().desc().list();

		// then
		verifySorting(orderedBatches, inverted(historicBatchById()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByStartTimeAsc()
	  public virtual void testBatchQueryOrderByStartTimeAsc()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();
		helper.migrateProcessInstancesAsync(1);
		ClockTestUtil.incrementClock(1000);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderByStartTime().asc().list();

		// then
		verifySorting(orderedBatches, historicBatchByStartTime());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByStartTimeDec()
	  public virtual void testBatchQueryOrderByStartTimeDec()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();
		helper.migrateProcessInstancesAsync(1);
		ClockTestUtil.incrementClock(1000);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderByStartTime().desc().list();

		// then
		verifySorting(orderedBatches, inverted(historicBatchByStartTime()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByEndTimeAsc()
	  public virtual void testBatchQueryOrderByEndTimeAsc()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		helper.completeBatch(batch1);

		ClockTestUtil.incrementClock(1000);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);
		helper.completeBatch(batch2);

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderByEndTime().asc().list();

		// then
		verifySorting(orderedBatches, historicBatchByEndTime());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByEndTimeDec()
	  public virtual void testBatchQueryOrderByEndTimeDec()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		helper.completeBatch(batch1);

		ClockTestUtil.incrementClock(1000);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);
		helper.completeBatch(batch2);

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderByEndTime().desc().list();

		// then
		verifySorting(orderedBatches, inverted(historicBatchByEndTime()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderingPropertyWithoutOrder()
	  public virtual void testBatchQueryOrderingPropertyWithoutOrder()
	  {
		try
		{
		  historyService.createHistoricBatchQuery().orderById().singleResult();
		  Assert.fail("exception expected");
		}
		catch (NotValidException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Invalid query: " + "call asc() or desc() after using orderByXX()"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderWithoutOrderingProperty()
	  public virtual void testBatchQueryOrderWithoutOrderingProperty()
	  {
		try
		{
		  historyService.createHistoricBatchQuery().asc().singleResult();
		  Assert.fail("exception expected");
		}
		catch (NotValidException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("You should call any of the orderBy methods " + "first before specifying a direction"));
		}
	  }
	}

}