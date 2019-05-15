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
namespace org.camunda.bpm.engine.test.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using HashedMap = org.apache.commons.collections.map.HashedMap;
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class HistoricBatchManagerBatchesForCleanupTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricBatchManagerBatchesForCleanupTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricBatchManagerBatchesForCleanupTest()
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


	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
	  public MigrationTestRule migrationRule;
	  public BatchMigrationHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule);
	  public RuleChain ruleChain;

	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public int historicBatchHistoryTTL;
	  public int historicBatchHistoryTTL;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public int daysInThePast;
	  public int daysInThePast;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(2) public int batch1EndTime;
	  public int batch1EndTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(3) public int batch2EndTime;
	  public int batch2EndTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(4) public int batchSize;
	  public int batchSize;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(5) public int resultCount;
	  public int resultCount;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters public static java.util.Collection<Object[]> scenarios()
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {5, -11, -6, -7, 50, 2},
			new object[] {5, -11, -3, -7, 50, 1},
			new object[] {5, -11, -3, -4, 50, 0},
			new object[] {5, -11, -6, -7, 1, 1}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testFindHistoricBatchIdsForCleanup()
	  public virtual void testFindHistoricBatchIdsForCleanup()
	  {
		// given
		string batchType = prepareHistoricBatches(2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, int> batchOperationsMap = new org.apache.commons.collections.map.HashedMap();
		IDictionary<string, int> batchOperationsMap = new HashedMap();
		batchOperationsMap[batchType] = historicBatchHistoryTTL;


		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, batchOperationsMap));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly HistoricBatchManagerBatchesForCleanupTest outerInstance;

		  private IDictionary<string, int> batchOperationsMap;

		  public CommandAnonymousInnerClass(HistoricBatchManagerBatchesForCleanupTest outerInstance, IDictionary<string, int> batchOperationsMap)
		  {
			  this.outerInstance = outerInstance;
			  this.batchOperationsMap = batchOperationsMap;
		  }

		  public object execute(CommandContext commandContext)
		  {
			// when
			IList<string> historicBatchIdsForCleanup = commandContext.HistoricBatchManager.findHistoricBatchIdsForCleanup(outerInstance.batchSize, batchOperationsMap, 0, 59);

			// then
			assertEquals(outerInstance.resultCount, historicBatchIdsForCleanup.Count);

			if (outerInstance.resultCount > 0)
			{

			  IList<HistoricBatch> historicBatches = outerInstance.historyService.createHistoricBatchQuery().list();

			  foreach (HistoricBatch historicBatch in historicBatches)
			  {
				historicBatch.EndTime < DateUtils.addDays(ClockUtil.CurrentTime, outerInstance.historicBatchHistoryTTL);
			  }
			}

			return null;
		  }
	  }

	  private string prepareHistoricBatches(int batchesCount)
	  {
		DateTime startDate = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		IList<Batch> list = new List<Batch>();
		for (int i = 0; i < batchesCount; i++)
		{
		  list.Add(helper.migrateProcessInstancesAsync(1));
		}

		Batch batch1 = list[0];
		string batchType = batch1.Type;
		helper.executeSeedJob(batch1);
		helper.executeJobs(batch1);
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, batch1EndTime);
		helper.executeMonitorJob(batch1);

		Batch batch2 = list[1];
		helper.executeSeedJob(batch2);
		helper.executeJobs(batch2);
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, batch2EndTime);
		helper.executeMonitorJob(batch2);

		ClockUtil.CurrentTime = DateTime.Now;

		return batchType;
	  }
	}

}