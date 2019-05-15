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
namespace org.camunda.bpm.engine.test.api.authorization.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationTestBaseRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestBaseRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricBatchQueryAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricBatchQueryAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			engineRule = new ProvidedProcessEngineRule(bootstrapRule);
			authRule = new AuthorizationTestBaseRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(authRule).around(testHelper);
		}


	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule();
	  public ProcessEngineRule engineRule;
	  public AuthorizationTestBaseRule authRule;
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();

	  protected internal MigrationPlan migrationPlan;
	  protected internal Batch batch1;
	  protected internal Batch batch2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("user", "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcessesAndCreateMigrationPlan()
	  public virtual void deployProcessesAndCreateMigrationPlan()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance pi = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		batch1 = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(pi.Id)).executeAsync();

		batch2 = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(pi.Id)).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
		removeAllRunningAndHistoricBatches();
		engineRule.ProcessEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		engineRule.ProcessEngineConfiguration.BatchOperationsForHistoryCleanup = null;
	  }

	  private void removeAllRunningAndHistoricBatches()
	  {
		HistoryService historyService = engineRule.HistoryService;
		ManagementService managementService = engineRule.ManagementService;

		foreach (Batch batch in managementService.createBatchQuery().list())
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		// remove history of completed batches
		foreach (HistoricBatch historicBatch in historyService.createHistoricBatchQuery().list())
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryList()
	  public virtual void testQueryList()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, batch1.Id, "user", Permissions.READ_HISTORY);

		// when
		authRule.enableAuthorization("user");
		IList<HistoricBatch> batches = engineRule.HistoryService.createHistoricBatchQuery().list();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(1, batches.Count);
		Assert.assertEquals(batch1.Id, batches[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, batch1.Id, "user", Permissions.READ_HISTORY);

		// when
		authRule.enableAuthorization("user");
		long count = engineRule.HistoryService.createHistoricBatchQuery().count();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(1, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoAuthorizations()
	  public virtual void testQueryNoAuthorizations()
	  {
		// when
		authRule.enableAuthorization("user");
		long count = engineRule.HistoryService.createHistoricBatchQuery().count();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryListAccessAll()
	  public virtual void testQueryListAccessAll()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, "*", "user", Permissions.READ_HISTORY);

		// when
		authRule.enableAuthorization("user");
		IList<HistoricBatch> batches = engineRule.HistoryService.createHistoricBatchQuery().list();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(2, batches.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryListMultiple()
	  public virtual void testQueryListMultiple()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, "*", "user", Permissions.READ_HISTORY);
		authRule.createGrantAuthorization(Resources.BATCH, batch1.Id, "user", Permissions.READ_HISTORY);

		// when
		authRule.enableAuthorization("user");
		IList<HistoricBatch> batches = engineRule.HistoryService.createHistoricBatchQuery().list();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(2, batches.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupReportQueryWithPermissions()
	  public virtual void testHistoryCleanupReportQueryWithPermissions()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, "*", "user", Permissions.READ_HISTORY);
		string migrationOperationsTTL = "P0D";
		prepareBatch(migrationOperationsTTL);

		authRule.enableAuthorization("user");
		CleanableHistoricBatchReportResult result = engineRule.HistoryService.createCleanableHistoricBatchReport().singleResult();
		authRule.disableAuthorization();

		assertNotNull(result);
		checkResultNumbers(result, 1, 1, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupReportQueryWithoutPermission()
	  public virtual void testHistoryCleanupReportQueryWithoutPermission()
	  {
		// given
		string migrationOperationsTTL = "P0D";
		prepareBatch(migrationOperationsTTL);
		// then
		thrown.expect(typeof(AuthorizationException));

		authRule.enableAuthorization("user");
		try
		{
		  // when
		  engineRule.HistoryService.createCleanableHistoricBatchReport().list();
		}
		finally
		{
		  authRule.disableAuthorization();
		}
	  }

	  private void prepareBatch(string migrationOperationsTTL)
	  {
		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = false;
		IDictionary<string, string> map = new Dictionary<string, string>();
		map["instance-migration"] = migrationOperationsTTL;
		engineRule.ProcessEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		engineRule.ProcessEngineConfiguration.initHistoryCleanup();

		DateTime startDate = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -11);
		string batchId = createBatch();
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -7);

		engineRule.ManagementService.deleteBatch(batchId, false);

		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = true;
	  }

	  private void checkResultNumbers(CleanableHistoricBatchReportResult result, int expectedCleanable, int expectedFinished, int? expectedTTL)
	  {
		assertEquals(expectedCleanable, result.CleanableBatchesCount);
		assertEquals(expectedFinished, result.FinishedBatchesCount);
		assertEquals(expectedTTL, result.HistoryTimeToLive);
	  }


	  private string createBatch()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan plan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance pi = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		 Batch batch = engineRule.RuntimeService.newMigration(plan).processInstanceIds(Arrays.asList(pi.Id)).executeAsync();

		 return batch.Id;
	  }
	}

}