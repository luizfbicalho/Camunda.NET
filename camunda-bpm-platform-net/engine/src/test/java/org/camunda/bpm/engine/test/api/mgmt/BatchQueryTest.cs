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
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.batchById;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsCollectionContaining.hasItems;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchQuery = org.camunda.bpm.engine.batch.BatchQuery;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class BatchQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchQueryTest()
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
//ORIGINAL LINE: @Test public void testBatchQuery()
	  public virtual void testBatchQuery()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		Batch batch2 = helper.migrateProcessInstancesAsync(1);

		// when
		IList<Batch> list = managementService.createBatchQuery().list();

		// then
		Assert.assertEquals(2, list.Count);

		IList<string> batchIds = new List<string>();
		foreach (Batch resultBatch in list)
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
		// given
		Batch batch = helper.migrateProcessInstancesAsync(1);

		// when
		Batch resultBatch = managementService.createBatchQuery().singleResult();

		// then
		Assert.assertNotNull(batch);

		Assert.assertEquals(batch.Id, resultBatch.Id);
		Assert.assertEquals(batch.BatchJobDefinitionId, resultBatch.BatchJobDefinitionId);
		Assert.assertEquals(batch.MonitorJobDefinitionId, resultBatch.MonitorJobDefinitionId);
		Assert.assertEquals(batch.SeedJobDefinitionId, resultBatch.SeedJobDefinitionId);
		Assert.assertEquals(batch.TenantId, resultBatch.TenantId);
		Assert.assertEquals(batch.Type, resultBatch.Type);
		Assert.assertEquals(batch.BatchJobsPerSeed, resultBatch.BatchJobsPerSeed);
		Assert.assertEquals(batch.InvocationsPerBatchJob, resultBatch.InvocationsPerBatchJob);
		Assert.assertEquals(batch.TotalJobs, resultBatch.TotalJobs);
		Assert.assertEquals(batch.JobsCreated, resultBatch.JobsCreated);
		Assert.assertEquals(batch.Suspended, resultBatch.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryById()
	  public virtual void testBatchQueryById()
	  {
		// given
		Batch batch1 = helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		Batch resultBatch = managementService.createBatchQuery().batchId(batch1.Id).singleResult();

		// then
		Assert.assertNotNull(resultBatch);
		Assert.assertEquals(batch1.Id, resultBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByIdNull()
	  public virtual void testBatchQueryByIdNull()
	  {
		try
		{
		  managementService.createBatchQuery().batchId(null).singleResult();
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
		long count = managementService.createBatchQuery().type(batch1.Type).count();

		// then
		Assert.assertEquals(2, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByNonExistingType()
	  public virtual void testBatchQueryByNonExistingType()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);

		// when
		long count = managementService.createBatchQuery().type("foo").count();

		// then
		Assert.assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByTypeNull()
	  public virtual void testBatchQueryByTypeNull()
	  {
		try
		{
		  managementService.createBatchQuery().type(null).singleResult();
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
		long count = managementService.createBatchQuery().count();

		// then
		Assert.assertEquals(2, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByIdAsc()
	  public virtual void testBatchQueryOrderByIdAsc()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<Batch> orderedBatches = managementService.createBatchQuery().orderById().asc().list();

		// then
		verifySorting(orderedBatches, batchById());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderByIdDec()
	  public virtual void testBatchQueryOrderByIdDec()
	  {
		// given
		helper.migrateProcessInstancesAsync(1);
		helper.migrateProcessInstancesAsync(1);

		// when
		IList<Batch> orderedBatches = managementService.createBatchQuery().orderById().desc().list();

		// then
		verifySorting(orderedBatches, inverted(batchById()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryOrderingPropertyWithoutOrder()
	  public virtual void testBatchQueryOrderingPropertyWithoutOrder()
	  {
		try
		{
		  managementService.createBatchQuery().orderById().singleResult();
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
		  managementService.createBatchQuery().asc().singleResult();
		  Assert.fail("exception expected");
		}
		catch (NotValidException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("You should call any of the orderBy methods " + "first before specifying a direction"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryBySuspendedBatches()
	  public virtual void testBatchQueryBySuspendedBatches()
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
		BatchQuery query = managementService.createBatchQuery().suspended();
		Assert.assertEquals(1, query.count());
		Assert.assertEquals(1, query.list().size());
		Assert.assertEquals(batch2.Id, query.singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByActiveBatches()
	  public virtual void testBatchQueryByActiveBatches()
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
		BatchQuery query = managementService.createBatchQuery().active();
		Assert.assertEquals(2, query.count());
		Assert.assertEquals(2, query.list().size());

		IList<string> foundIds = new List<string>();
		foreach (Batch batch in query.list())
		{
		  foundIds.Add(batch.Id);
		}
		Assert.assertThat(foundIds, hasItems(batch1.Id, batch3.Id));
	  }

	}

}