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
namespace org.camunda.bpm.engine.test.api.multitenancy
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
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
	public class MultiTenancyBatchTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyBatchTest()
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
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

	  protected internal ProcessDefinition tenant1Definition;
	  protected internal ProcessDefinition tenant2Definition;
	  protected internal ProcessDefinition sharedDefinition;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcesses()
	  public virtual void deployProcesses()
	  {
		sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		tenant1Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.ONE_TASK_PROCESS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		batchHelper.removeAllRunningAndHistoricBatches();
	  }

	  /// <summary>
	  /// Source: no tenant id
	  /// Target: no tenant id
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchTenantIdCase1()
	  public virtual void testBatchTenantIdCase1()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(sharedDefinition, sharedDefinition);

		// then
		Assert.assertNull(batch.TenantId);
	  }

	  /// <summary>
	  /// Source: tenant 1
	  /// Target: no tenant id
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchTenantIdCase2()
	  public virtual void testBatchTenantIdCase2()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, sharedDefinition);

		// then
		Assert.assertEquals(TENANT_ONE, batch.TenantId);
	  }

	  /// <summary>
	  /// Source: no tenant id
	  /// Target: tenant 1
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchTenantIdCase3()
	  public virtual void testBatchTenantIdCase3()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(sharedDefinition, tenant1Definition);

		// then
		Assert.assertNull(batch.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testHistoricBatchTenantId()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testHistoricBatchTenantId()
	  {
		// given
		batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// then
		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();
		Assert.assertEquals(TENANT_ONE, historicBatch.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchJobDefinitionsTenantId()
	  public virtual void testBatchJobDefinitionsTenantId()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// then
		JobDefinition migrationJobDefinition = batchHelper.getExecutionJobDefinition(batch);
		Assert.assertEquals(TENANT_ONE, migrationJobDefinition.TenantId);

		JobDefinition monitorJobDefinition = batchHelper.getMonitorJobDefinition(batch);
		Assert.assertEquals(TENANT_ONE, monitorJobDefinition.TenantId);

		JobDefinition seedJobDefinition = batchHelper.getSeedJobDefinition(batch);
		Assert.assertEquals(TENANT_ONE, seedJobDefinition.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchJobsTenantId()
	  public virtual void testBatchJobsTenantId()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// then
		Job seedJob = batchHelper.getSeedJob(batch);
		Assert.assertEquals(TENANT_ONE, seedJob.TenantId);

		batchHelper.executeSeedJob(batch);

		IList<Job> migrationJob = batchHelper.getExecutionJobs(batch);
		Assert.assertEquals(TENANT_ONE, migrationJob[0].TenantId);

		Job monitorJob = batchHelper.getMonitorJob(batch);
		Assert.assertEquals(TENANT_ONE, monitorJob.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteBatch()
	  public virtual void testDeleteBatch()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// when
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));
		managementService.deleteBatch(batch.Id, true);
		identityService.clearAuthentication();

		// then
		Assert.assertEquals(0, managementService.createBatchQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteBatchFailsWithWrongTenant()
	  public virtual void testDeleteBatchFailsWithWrongTenant()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant2Definition, tenant2Definition);

		// when
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));
		try
		{
		  managementService.deleteBatch(batch.Id, true);
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot delete batch '" + batch.Id + "' because it belongs to no authenticated tenant"));
		}
		finally
		{
		  identityService.clearAuthentication();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendBatch()
	  public virtual void testSuspendBatch()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// when
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));
		managementService.suspendBatchById(batch.Id);
		identityService.clearAuthentication();

		// then
		batch = managementService.createBatchQuery().batchId(batch.Id).singleResult();
		Assert.assertTrue(batch.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendBatchFailsWithWrongTenant()
	  public virtual void testSuspendBatchFailsWithWrongTenant()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant2Definition, tenant2Definition);

		// when
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));
		try
		{
		  managementService.suspendBatchById(batch.Id);
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot suspend batch '" + batch.Id + "' because it belongs to no authenticated tenant"));
		}
		finally
		{
		  identityService.clearAuthentication();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateBatch()
	  public virtual void testActivateBatch()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant1Definition, tenant1Definition);
		managementService.suspendBatchById(batch.Id);

		// when
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));
		managementService.activateBatchById(batch.Id);
		identityService.clearAuthentication();

		// then
		batch = managementService.createBatchQuery().batchId(batch.Id).singleResult();
		Assert.assertFalse(batch.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateBatchFailsWithWrongTenant()
	  public virtual void testActivateBatchFailsWithWrongTenant()
	  {
		// given
		Batch batch = batchHelper.migrateProcessInstanceAsync(tenant2Definition, tenant2Definition);
		managementService.suspendBatchById(batch.Id);

		// when
		identityService.setAuthentication("user", null, singletonList(TENANT_ONE));
		try
		{
		  managementService.activateBatchById(batch.Id);
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot activate batch '" + batch.Id + "' because it belongs to no authenticated tenant"));
		}
		finally
		{
		  identityService.clearAuthentication();
		}
	  }

	}

}