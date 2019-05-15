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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using BatchModificationHelper = org.camunda.bpm.engine.test.api.runtime.BatchModificationHelper;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class CleanableHistoricBatchReportTest
	{
		private bool InstanceFieldsInitialized = false;

		public CleanableHistoricBatchReportTest()
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
			testRule = new ProcessEngineTestRule(engineRule);
			migrationRule = new MigrationTestRule(engineRule);
			migrationHelper = new BatchMigrationHelper(engineRule, migrationRule);
			modificationHelper = new BatchModificationHelper(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(testRule).around(engineRule).around(migrationRule);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule();
	  public ProcessEngineRule engineRule;
	  public ProcessEngineTestRule testRule;
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper migrationHelper;
	  protected internal BatchModificationHelper modificationHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(testRule).around(engineRule).around(migrationRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal HistoryService historyService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		processEngineConfiguration = (ProcessEngineConfigurationImpl)bootstrapRule.ProcessEngine.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		ClockUtil.reset();
		migrationHelper.removeAllRunningAndHistoricBatches();
		processEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		processEngineConfiguration.BatchOperationsForHistoryCleanup = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportMixedConfiguration()
	  public virtual void testReportMixedConfiguration()
	  {
		IDictionary<string, string> map = new Dictionary<string, string>();
		int modOperationsTTL = 20;
		map["instance-modification"] = "P20D";
		int defaultTTL = 5;
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();

		DateTime startDate = DateTime.Now;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		Batch modificationBatch = createModificationBatch();
		IList<string> batchIds = new List<string>();
		batchIds.Add(modificationBatch.Id);

		int migrationCountBatch = 10;
		IList<string> batchIds1 = new List<string>();
		((IList<string>)batchIds1).AddRange(createMigrationBatchList(migrationCountBatch));

		int cancelationCountBatch = 20;
		IList<string> batchIds2 = new List<string>();
		((IList<string>)batchIds2).AddRange(createCancelationBatchList(cancelationCountBatch));


		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -8);

		foreach (string batchId in batchIds)
		{
		  managementService.deleteBatch(batchId, false);
		}

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -2);

		for (int i = 0; i < 4; i++)
		{
		  managementService.deleteBatch(batchIds1[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -7);
		for (int i = 6; i < batchIds1.Count; i++)
		{
		  managementService.deleteBatch(batchIds1[i], false);
		}

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -10);
		for (int i = 0; i < 7; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -5);
		for (int i = 7; i < 11; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -1);
		for (int i = 13; i < batchIds2.Count; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}

		ClockUtil.CurrentTime = DateUtils.addSeconds(startDate, 1);

		// when
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(31, historicList.Count);

		IList<CleanableHistoricBatchReportResult> list = historyService.createCleanableHistoricBatchReport().list();
		assertEquals(3, list.Count);
		foreach (CleanableHistoricBatchReportResult result in list)
		{
		  if (result.BatchType.Equals("instance-migration"))
		  {
			checkResultNumbers(result, 4, 8, defaultTTL);
		  }
		  else if (result.BatchType.Equals("instance-modification"))
		  {
			checkResultNumbers(result, 0, 1, modOperationsTTL);
		  }
		  else if (result.BatchType.Equals("instance-deletion"))
		  {
			checkResultNumbers(result, 11, 18, defaultTTL);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNoDefaultConfiguration()
	  public virtual void testReportNoDefaultConfiguration()
	  {
		IDictionary<string, string> map = new Dictionary<string, string>();
		int modOperationsTTL = 5;
		map["instance-modification"] = "P5D";
		int delOperationsTTL = 7;
		map["instance-deletion"] = "P7D";
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();
		assertNull(processEngineConfiguration.BatchOperationHistoryTimeToLive);

		DateTime startDate = DateTime.Now;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		Batch modificationBatch = createModificationBatch();
		IList<string> batchIds = new List<string>();
		batchIds.Add(modificationBatch.Id);

		int migrationCountBatch = 10;
		IList<string> batchIds1 = new List<string>();
		((IList<string>)batchIds1).AddRange(createMigrationBatchList(migrationCountBatch));

		int cancelationCountBatch = 20;
		IList<string> batchIds2 = new List<string>();
		((IList<string>)batchIds2).AddRange(createCancelationBatchList(cancelationCountBatch));


		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -8);

		foreach (string batchId in batchIds)
		{
		  managementService.deleteBatch(batchId, false);
		}

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -2);

		for (int i = 0; i < 4; i++)
		{
		  managementService.deleteBatch(batchIds1[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -7);
		for (int i = 6; i < batchIds1.Count; i++)
		{
		  managementService.deleteBatch(batchIds1[i], false);
		}

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -10);
		for (int i = 0; i < 7; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -5);
		for (int i = 7; i < 11; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -1);
		for (int i = 13; i < batchIds2.Count; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}

		ClockUtil.CurrentTime = DateUtils.addSeconds(startDate, 1);

		// when
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(31, historicList.Count);

		IList<CleanableHistoricBatchReportResult> list = historyService.createCleanableHistoricBatchReport().list();
		assertEquals(3, list.Count);
		foreach (CleanableHistoricBatchReportResult result in list)
		{
		  if (result.BatchType.Equals("instance-migration"))
		  {
			checkResultNumbers(result, 0, 8, null);
		  }
		  else if (result.BatchType.Equals("instance-modification"))
		  {
			checkResultNumbers(result, 1, 1, modOperationsTTL);
		  }
		  else if (result.BatchType.Equals("instance-deletion"))
		  {
			checkResultNumbers(result, delOperationsTTL, 18, delOperationsTTL);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportNoTTLConfiguration()
	  public virtual void testReportNoTTLConfiguration()
	  {
		processEngineConfiguration.initHistoryCleanup();
		assertNull(processEngineConfiguration.BatchOperationHistoryTimeToLive);

		DateTime startDate = DateTime.Now;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		int cancelationCountBatch = 20;
		IList<string> batchIds2 = new List<string>();
		((IList<string>)batchIds2).AddRange(createCancelationBatchList(cancelationCountBatch));

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -10);
		for (int i = 0; i < 7; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -5);
		for (int i = 7; i < 11; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -1);
		for (int i = 13; i < batchIds2.Count; i++)
		{
		  managementService.deleteBatch(batchIds2[i], false);
		}

		ClockUtil.CurrentTime = DateUtils.addSeconds(startDate, 1);

		// when
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(20, historicList.Count);

		assertEquals(1, historyService.createCleanableHistoricBatchReport().count());
		checkResultNumbers(historyService.createCleanableHistoricBatchReport().singleResult(), 0, 18, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportZeroTTL()
	  public virtual void testReportZeroTTL()
	  {
		IDictionary<string, string> map = new Dictionary<string, string>();
		int modOperationsTTL = 0;
		map["instance-modification"] = "P0D";
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();

		DateTime startDate = ClockUtil.CurrentTime;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		Batch modificationBatch = createModificationBatch();
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -7);

		managementService.deleteBatch(modificationBatch.Id, false);

		CleanableHistoricBatchReportResult result = historyService.createCleanableHistoricBatchReport().singleResult();
		assertNotNull(result);
		checkResultNumbers(result, 1, 1, modOperationsTTL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportOrderByFinishedProcessInstance()
	  public virtual void testReportOrderByFinishedProcessInstance()
	  {
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.initHistoryCleanup();
		assertNotNull(processEngineConfiguration.BatchOperationHistoryTimeToLive);

		DateTime startDate = DateTime.Now;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		IList<string> batchIds = new List<string>();

		Batch modificationBatch = createModificationBatch();
		batchIds.Add(modificationBatch.Id);

		int migrationCountBatch = 10;
		((IList<string>)batchIds).AddRange(createMigrationBatchList(migrationCountBatch));

		int cancelationCountBatch = 20;
		((IList<string>)batchIds).AddRange(createCancelationBatchList(cancelationCountBatch));

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -8);

		foreach (string batchId in batchIds)
		{
		  managementService.deleteBatch(batchId, false);
		}

		ClockUtil.CurrentTime = DateUtils.addSeconds(startDate, 1);

		// assume
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(31, historicList.Count);

		// then
		IList<CleanableHistoricBatchReportResult> reportResultAsc = historyService.createCleanableHistoricBatchReport().orderByFinishedBatchOperation().asc().list();
		assertEquals(3, reportResultAsc.Count);
		assertEquals("instance-modification", reportResultAsc[0].BatchType);
		assertEquals("instance-migration", reportResultAsc[1].BatchType);
		assertEquals("instance-deletion", reportResultAsc[2].BatchType);

		IList<CleanableHistoricBatchReportResult> reportResultDesc = historyService.createCleanableHistoricBatchReport().orderByFinishedBatchOperation().desc().list();
		assertEquals(3, reportResultDesc.Count);
		assertEquals("instance-deletion", reportResultDesc[0].BatchType);
		assertEquals("instance-migration", reportResultDesc[1].BatchType);
		assertEquals("instance-modification", reportResultDesc[2].BatchType);
	  }

	  private void checkResultNumbers(CleanableHistoricBatchReportResult result, int expectedCleanable, int expectedFinished, int? expectedTTL)
	  {
		assertEquals(expectedCleanable, result.CleanableBatchesCount);
		assertEquals(expectedFinished, result.FinishedBatchesCount);
		assertEquals(expectedTTL, result.HistoryTimeToLive);
	  }

	  private BpmnModelInstance createModelInstance()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent("start").userTask("userTask1").sequenceFlowId("seq").userTask("userTask2").endEvent("end").done();
		return instance;
	  }

	  private IList<string> createMigrationBatchList(int migrationCountBatch)
	  {
		IList<string> batchIds = new List<string>();
		for (int i = 0; i < migrationCountBatch; i++)
		{
		  batchIds.Add(migrationHelper.migrateProcessInstancesAsync(1).Id);
		}
		return batchIds;
	  }

	  private Batch createModificationBatch()
	  {
		BpmnModelInstance instance = createModelInstance();
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		Batch modificationBatch = modificationHelper.startAfterAsync("process", 1, "userTask1", processDefinition.Id);
		return modificationBatch;
	  }

	  private IList<string> createCancelationBatchList(int cancelationCountBatch)
	  {
		IList<string> batchIds = new List<string>();
		for (int i = 0; i < cancelationCountBatch; i++)
		{
		  batchIds.Add(runtimeService.deleteProcessInstancesAsync(Arrays.asList("unknownId"), "create-deletion-batch").Id);
		}
		return batchIds;
	  }

	}

}