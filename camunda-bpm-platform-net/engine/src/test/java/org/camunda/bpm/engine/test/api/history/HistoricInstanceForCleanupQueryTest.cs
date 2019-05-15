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
	using HashedMap = org.apache.commons.collections.map.HashedMap;
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using HistoricBatchManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricBatchManager;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricInstanceForCleanupQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricInstanceForCleanupQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			migrationRule = new MigrationTestRule(engineRule);
			helper = new BatchMigrationHelper(engineRule, migrationRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule).around(migrationRule);
		}


	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule).around(migrationRule);
	  public RuleChain ruleChain;

	  private HistoryService historyService;
	  private ManagementService managementService;
	  private CaseService caseService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		caseService = engineRule.CaseService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		helper.removeAllRunningAndHistoricBatches();

		clearMetrics();
	  }

	  protected internal virtual void clearMetrics()
	  {
		ICollection<Meter> meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
		foreach (Meter meter in meters)
		{
		  meter.AndClear;
		}
		managementService.deleteMetrics(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testSortHistoricBatchesForCleanup()
	  public virtual void testSortHistoricBatchesForCleanup()
	  {
		DateTime startDate = ClockUtil.CurrentTime;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		// given
		IList<Batch> list = Arrays.asList(helper.migrateProcessInstancesAsync(1), helper.migrateProcessInstancesAsync(1), helper.migrateProcessInstancesAsync(1));

		string batchType = list[0].Type;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, int> batchOperationsMap = new org.apache.commons.collections.map.HashedMap();
		IDictionary<string, int> batchOperationsMap = new HashedMap();
		batchOperationsMap[batchType] = 4;

		foreach (Batch batch in list)
		{
		  helper.executeSeedJob(batch);
		  helper.executeJobs(batch);

		  ClockUtil.CurrentTime = DateUtils.addDays(startDate, ++daysInThePast);
		  helper.executeMonitorJob(batch);
		}

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(3, historicList.Count);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, batchOperationsMap));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoricInstanceForCleanupQueryTest outerInstance;

		  private IDictionary<string, int> batchOperationsMap;

		  public CommandAnonymousInnerClass(HistoricInstanceForCleanupQueryTest outerInstance, IDictionary<string, int> batchOperationsMap)
		  {
			  this.outerInstance = outerInstance;
			  this.batchOperationsMap = batchOperationsMap;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			HistoricBatchManager historicBatchManager = commandContext.HistoricBatchManager;
			IList<string> ids = historicBatchManager.findHistoricBatchIdsForCleanup(7, batchOperationsMap, 0, 59);
			assertEquals(3, ids.Count);
			HistoricBatchEntity instance0 = historicBatchManager.findHistoricBatchById(ids[0]);
			HistoricBatchEntity instance1 = historicBatchManager.findHistoricBatchById(ids[1]);
			HistoricBatchEntity instance2 = historicBatchManager.findHistoricBatchById(ids[2]);
			assertTrue(instance0.EndTime < instance1.EndTime);
			assertTrue(instance1.EndTime < instance2.EndTime);

			return null;
		  }
	  }

	}

}