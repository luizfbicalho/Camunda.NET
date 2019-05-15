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
namespace org.camunda.bpm.engine.test.api.multitenancy.query.history
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.historicBatchByTenantId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
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
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricBatchQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricBatchQueryTest()
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

	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

	  protected internal Batch sharedBatch;
	  protected internal Batch tenant1Batch;
	  protected internal Batch tenant2Batch;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		historyService = engineRule.HistoryService;
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
//ORIGINAL LINE: @Test public void testHistoricBatchQueryNoAuthenticatedTenant()
	  public virtual void testHistoricBatchQueryNoAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, null);

		// when
		IList<HistoricBatch> batches = historyService.createHistoricBatchQuery().list();

		// then
		Assert.assertEquals(1, batches.Count);
		Assert.assertEquals(sharedBatch.Id, batches[0].Id);

		Assert.assertEquals(1, historyService.createHistoricBatchQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryAuthenticatedTenant()
	  public virtual void testHistoricBatchQueryAuthenticatedTenant()
	  {
		// given
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		IList<HistoricBatch> batches = historyService.createHistoricBatchQuery().list();

		// then
		Assert.assertEquals(2, batches.Count);
		assertBatches(batches, tenant1Batch.Id, sharedBatch.Id);

		Assert.assertEquals(2, historyService.createHistoricBatchQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryAuthenticatedTenants()
	  public virtual void testHistoricBatchQueryAuthenticatedTenants()
	  {
		// given
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		// when
		IList<HistoricBatch> batches = historyService.createHistoricBatchQuery().list();

		// then
		Assert.assertEquals(3, batches.Count);

		Assert.assertEquals(3, historyService.createHistoricBatchQuery().count());

		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoricBatch()
	  public virtual void testDeleteHistoricBatch()
	  {
		// given
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		historyService.deleteHistoricBatch(tenant1Batch.Id);

		// then
		identityService.clearAuthentication();
		Assert.assertEquals(2, historyService.createHistoricBatchQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoricBatchFailsWithWrongTenant()
	  public virtual void testDeleteHistoricBatchFailsWithWrongTenant()
	  {
		// given
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		try
		{
		  historyService.deleteHistoricBatch(tenant2Batch.Id);
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot delete historic batch '" + tenant2Batch.Id + "' because it belongs to no authenticated tenant"));
		}

		identityService.clearAuthentication();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryFilterByTenant()
	  public virtual void testHistoricBatchQueryFilterByTenant()
	  {
		// when
		HistoricBatch returnedBatch = historyService.createHistoricBatchQuery().tenantIdIn(TENANT_ONE).singleResult();

		// then
		Assert.assertNotNull(returnedBatch);
		Assert.assertEquals(tenant1Batch.Id, returnedBatch.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryFilterByTenants()
	  public virtual void testHistoricBatchQueryFilterByTenants()
	  {
		// when
		IList<HistoricBatch> returnedBatches = historyService.createHistoricBatchQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		// then
		Assert.assertEquals(2, returnedBatches.Count);
		Assert.assertEquals(tenant1Batch.Id, returnedBatches[0].Id);
		Assert.assertEquals(tenant2Batch.Id, returnedBatches[1].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryFilterWithoutTenantId()
	  public virtual void testHistoricBatchQueryFilterWithoutTenantId()
	  {
		// when
		HistoricBatch returnedBatch = historyService.createHistoricBatchQuery().withoutTenantId().singleResult();

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
		  historyService.createHistoricBatchQuery().tenantIdIn(tenantIds);
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
		  historyService.createHistoricBatchQuery().tenantIdIn(tenantIds);
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
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderByTenantId().asc().list();

		// then
		verifySorting(orderedBatches, historicBatchByTenantId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByTenantIdDesc()
	  public virtual void testOrderByTenantIdDesc()
	  {

		// when
		IList<HistoricBatch> orderedBatches = historyService.createHistoricBatchQuery().orderByTenantId().desc().list();

		// then
		verifySorting(orderedBatches, inverted(historicBatchByTenantId()));
	  }

	  protected internal virtual void assertBatches(IList<HistoricBatch> actualBatches, params string[] expectedIds)
	  {
		Assert.assertEquals(expectedIds.Length, actualBatches.Count);

		ISet<string> actualIds = new HashSet<string>();
		foreach (HistoricBatch batch in actualBatches)
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