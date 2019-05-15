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
namespace org.camunda.bpm.engine.test.api.multitenancy.query
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.batchByTenantId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.batchStatisticsByTenantId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
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
	public class MultiTenancyBatchQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyBatchQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			defaultRuleChin = RuleChain.outerRule(engineRule).around(testHelper);
			batchHelper = new BatchMigrationHelper(engineRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain defaultRuleChin = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain defaultRuleChin;

	  protected internal BatchMigrationHelper batchHelper;

	  protected internal ManagementService managementService;
	  protected internal IdentityService identityService;

	  protected internal Batch sharedBatch;
	  protected internal Batch tenant1Batch;
	  protected internal Batch tenant2Batch;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcesses()
	  public virtual void deployProcesses()
	  {
		ProcessDefinition sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant1Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.ONE_TASK_PROCESS);

		sharedBatch = batchHelper.migrateProcessInstanceAsync(sharedDefinition, sharedDefinition);
		tenant1Batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);
		tenant2Batch = batchHelper.migrateProcessInstanceAsync(tenant2Definition, tenant2Definition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		batchHelper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryNoAuthenticatedTenant()
	  public virtual void testBatchQueryNoAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, null);

		// then
		IList<Batch> batches = managementService.createBatchQuery().list();
		Assert.assertEquals(1, batches.Count);
		Assert.assertEquals(sharedBatch.Id, batches[0].Id);

		Assert.assertEquals(1, managementService.createBatchQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryAuthenticatedTenant()
	  public virtual void testBatchQueryAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		IList<Batch> batches = managementService.createBatchQuery().list();

		// then
		Assert.assertEquals(2, batches.Count);
		assertBatches(batches, tenant1Batch.Id, sharedBatch.Id);

		Assert.assertEquals(2, managementService.createBatchQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryAuthenticatedTenants()
	  public virtual void testBatchQueryAuthenticatedTenants()
	  {
		// given
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		// when
		IList<Batch> batches = managementService.createBatchQuery().list();

		// then
		Assert.assertEquals(3, batches.Count);
		Assert.assertEquals(3, managementService.createBatchQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsNoAuthenticatedTenant()
	  public virtual void testBatchStatisticsNoAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, null);

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().list();

		// then
		Assert.assertEquals(1, statistics.Count);
		Assert.assertEquals(sharedBatch.Id, statistics[0].Id);

		Assert.assertEquals(1, managementService.createBatchStatisticsQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsAuthenticatedTenant()
	  public virtual void testBatchStatisticsAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().list();

		// then
		Assert.assertEquals(2, statistics.Count);

		Assert.assertEquals(2, managementService.createBatchStatisticsQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsAuthenticatedTenants()
	  public virtual void testBatchStatisticsAuthenticatedTenants()
	  {
		// given
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		// then
		IList<BatchStatistics> statistics = managementService.createBatchStatisticsQuery().list();
		Assert.assertEquals(3, statistics.Count);

		Assert.assertEquals(3, managementService.createBatchStatisticsQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryFilterByTenant()
	  public virtual void testBatchQueryFilterByTenant()
	  {
		// when
		Batch returnedBatch = managementService.createBatchQuery().tenantIdIn(TENANT_ONE).singleResult();

		// then
		Assert.assertNotNull(returnedBatch);
		Assert.assertEquals(tenant1Batch.Id, returnedBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryFilterByTenants()
	  public virtual void testBatchQueryFilterByTenants()
	  {
		// when
		IList<Batch> returnedBatches = managementService.createBatchQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		// then
		Assert.assertEquals(2, returnedBatches.Count);
		Assert.assertEquals(tenant1Batch.Id, returnedBatches[0].Id);
		Assert.assertEquals(tenant2Batch.Id, returnedBatches[1].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryFilterWithoutTenantId()
	  public virtual void testBatchQueryFilterWithoutTenantId()
	  {
		// when
		Batch returnedBatch = managementService.createBatchQuery().withoutTenantId().singleResult();

		// then
		Assert.assertNotNull(returnedBatch);
		Assert.assertEquals(sharedBatch.Id, returnedBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryFailOnNullTenantIdCase1()
	  public virtual void testBatchQueryFailOnNullTenantIdCase1()
	  {

		string[] tenantIds = null;
		try
		{
		  managementService.createBatchQuery().tenantIdIn(tenantIds);
		  Assert.fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryFailOnNullTenantIdCase2()
	  public virtual void testBatchQueryFailOnNullTenantIdCase2()
	  {

		string[] tenantIds = new string[]{null};
		try
		{
		  managementService.createBatchQuery().tenantIdIn(tenantIds);
		  Assert.fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByTenantIdAsc()
	  public virtual void testOrderByTenantIdAsc()
	  {

		// when
		IList<Batch> orderedBatches = managementService.createBatchQuery().orderByTenantId().asc().list();

		// then
		verifySorting(orderedBatches, batchByTenantId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByTenantIdDesc()
	  public virtual void testOrderByTenantIdDesc()
	  {

		// when
		IList<Batch> orderedBatches = managementService.createBatchQuery().orderByTenantId().desc().list();

		// then
		verifySorting(orderedBatches, inverted(batchByTenantId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryFilterByTenant()
	  public virtual void testBatchStatisticsQueryFilterByTenant()
	  {
		// when
		BatchStatistics returnedBatch = managementService.createBatchStatisticsQuery().tenantIdIn(TENANT_ONE).singleResult();

		// then
		Assert.assertNotNull(returnedBatch);
		Assert.assertEquals(tenant1Batch.Id, returnedBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryFilterByTenants()
	  public virtual void testBatchStatisticsQueryFilterByTenants()
	  {
		// when
		IList<BatchStatistics> returnedBatches = managementService.createBatchStatisticsQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		// then
		Assert.assertEquals(2, returnedBatches.Count);
		Assert.assertEquals(tenant1Batch.Id, returnedBatches[0].Id);
		Assert.assertEquals(tenant2Batch.Id, returnedBatches[1].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryFilterWithoutTenantId()
	  public virtual void testBatchStatisticsQueryFilterWithoutTenantId()
	  {
		// when
		BatchStatistics returnedBatch = managementService.createBatchStatisticsQuery().withoutTenantId().singleResult();

		// then
		Assert.assertNotNull(returnedBatch);
		Assert.assertEquals(sharedBatch.Id, returnedBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryFailOnNullTenantIdCase1()
	  public virtual void testBatchStatisticsQueryFailOnNullTenantIdCase1()
	  {

		string[] tenantIds = null;
		try
		{
		  managementService.createBatchStatisticsQuery().tenantIdIn(tenantIds);
		  Assert.fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryFailOnNullTenantIdCase2()
	  public virtual void testBatchStatisticsQueryFailOnNullTenantIdCase2()
	  {

		string[] tenantIds = new string[]{null};
		try
		{
		  managementService.createBatchStatisticsQuery().tenantIdIn(tenantIds);
		  Assert.fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryOrderByTenantIdAsc()
	  public virtual void testBatchStatisticsQueryOrderByTenantIdAsc()
	  {
		// when
		IList<BatchStatistics> orderedBatches = managementService.createBatchStatisticsQuery().orderByTenantId().asc().list();

		// then
		verifySorting(orderedBatches, batchStatisticsByTenantId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsQueryOrderByTenantIdDesc()
	  public virtual void testBatchStatisticsQueryOrderByTenantIdDesc()
	  {
		// when
		IList<BatchStatistics> orderedBatches = managementService.createBatchStatisticsQuery().orderByTenantId().desc().list();

		// then
		verifySorting(orderedBatches, inverted(batchStatisticsByTenantId()));
	  }

	  protected internal virtual void assertBatches<T1>(IList<T1> actualBatches, params string[] expectedIds) where T1 : org.camunda.bpm.engine.batch.Batch
	  {
		Assert.assertEquals(expectedIds.Length, actualBatches.Count);

		ISet<string> actualIds = new HashSet<string>();
		foreach (Batch batch in actualBatches)
		{
		  actualIds.Add(batch.Id);
		}

		foreach (string expectedId in expectedIds)
		{
		  Assert.assertTrue(actualIds.Contains(expectedId));
		}
	  }
	}

}