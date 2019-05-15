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
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
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
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoryCleanupHistoricBatchTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupHistoricBatchTest()
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
			migrationHelper = new BatchMigrationHelper(engineRule, migrationRule);
			modificationHelper = new BatchModificationHelper(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule).around(migrationRule);
		}


	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.HistoryCleanupDegreeOfParallelism = 3;
			return configuration;
		  }
	  }
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper migrationHelper;
	  protected internal BatchModificationHelper modificationHelper;

	  private const string DEFAULT_TTL_DAYS = "P5D";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private Random random = new Random();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule).around(migrationRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		migrationHelper.removeAllRunningAndHistoricBatches();

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupHistoricBatchTest outerInstance;

		  public CommandAnonymousInnerClass(HistoryCleanupHistoricBatchTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.managementService.createJobQuery().list();
			foreach (Job job in jobs)
			{
			  commandContext.JobManager.deleteJob((JobEntity) job);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			}

			IList<HistoricIncident> historicIncidents = outerInstance.historyService.createHistoricIncidentQuery().list();
			foreach (HistoricIncident historicIncident in historicIncidents)
			{
			  commandContext.DbEntityManager.delete((HistoricIncidentEntity) historicIncident);
			}

			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetConfiguration()
	  public virtual void resetConfiguration()
	  {
		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
		processEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		processEngineConfiguration.BatchOperationsForHistoryCleanup = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCleanupHistoricBatch()
	  public virtual void testCleanupHistoricBatch()
	  {
		initBatchOperationHistoryTimeToLive(DEFAULT_TTL_DAYS);
		int daysInThePast = -11;

		// given
		prepareHistoricBatches(3, daysInThePast);
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(3, historicList.Count);

		// when
		runHistoryCleanup();

		// then
		assertEquals(0, historyService.createHistoricBatchQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCleanupHistoricJobLog()
	  public virtual void testCleanupHistoricJobLog()
	  {
		initBatchOperationHistoryTimeToLive(DEFAULT_TTL_DAYS);
		int daysInThePast = -11;

		// given
		prepareHistoricBatches(1, daysInThePast);
		HistoricBatch batch = historyService.createHistoricBatchQuery().singleResult();
		string batchId = batch.Id;

		// when
		runHistoryCleanup();

		// then
		assertEquals(0, historyService.createHistoricBatchQuery().count());
		assertEquals(0, historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batchId).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCleanupHistoricIncident()
	  public virtual void testCleanupHistoricIncident()
	  {
		initBatchOperationHistoryTimeToLive(DEFAULT_TTL_DAYS);
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, -11);

		BatchEntity batch = (BatchEntity) createFailingMigrationBatch();

		migrationHelper.executeSeedJob(batch);

		IList<Job> list = managementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  if (((JobEntity) job).JobHandlerType.Equals("instance-migration"))
		  {
			managementService.setJobRetries(job.Id, 1);
		  }
		}
		migrationHelper.executeJobs(batch);

		IList<string> byteArrayIds = findExceptionByteArrayIds();

		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, -10);
		managementService.deleteBatch(batch.Id, false);
		ClockUtil.CurrentTime = DateTime.Now;

		// given
		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();
		string batchId = historicBatch.Id;

		// when
		runHistoryCleanup();

		assertEquals(0, historyService.createHistoricBatchQuery().count());
		assertEquals(0, historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batchId).count());
		assertEquals(0, historyService.createHistoricIncidentQuery().count());
		verifyByteArraysWereRemoved(byteArrayIds.ToArray());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupBatchMetrics()
	  public virtual void testHistoryCleanupBatchMetrics()
	  {
		initBatchOperationHistoryTimeToLive(DEFAULT_TTL_DAYS);
		// given
		int daysInThePast = -11;
		int batchesCount = 5;
		prepareHistoricBatches(batchesCount, daysInThePast);

		// when
		runHistoryCleanup();

		// then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long removedBatches = managementService.createMetricsQuery().name(org.camunda.bpm.engine.management.Metrics.HISTORY_CLEANUP_REMOVED_BATCH_OPERATIONS).sum();
		long removedBatches = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_BATCH_OPERATIONS).sum();

		assertEquals(batchesCount, removedBatches);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchOperationTypeConfigurationOnly()
	  public virtual void testBatchOperationTypeConfigurationOnly()
	  {
		IDictionary<string, string> map = new Dictionary<string, string>();
		map["instance-migration"] = "P2D";
		map["instance-deletion"] = DEFAULT_TTL_DAYS;
		processEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();

		assertNull(processEngineConfiguration.BatchOperationHistoryTimeToLive);

		DateTime startDate = ClockUtil.CurrentTime;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		IList<string> batchIds = new List<string>();

		int migrationCountBatch = 10;
		((IList<string>)batchIds).AddRange(createMigrationBatchList(migrationCountBatch));

		int cancelationCountBatch = 20;
		((IList<string>)batchIds).AddRange(createCancelationBatchList(cancelationCountBatch));

		ClockUtil.CurrentTime = DateUtils.addDays(startDate, -7);

		foreach (string batchId in batchIds)
		{
		  managementService.deleteBatch(batchId, false);
		}

		ClockUtil.CurrentTime = DateTime.Now;

		// when
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(30, historicList.Count);
		runHistoryCleanup();

		// then
		assertEquals(0, historyService.createHistoricBatchQuery().count());
		foreach (string batchId in batchIds)
		{
		  assertEquals(0, historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batchId).count());
		}
	  }

	  private void runHistoryCleanup()
	  {
		historyService.cleanUpHistoryAsync(true);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job historyCleanupJob in historyCleanupJobs)
		{
		  managementService.executeJob(historyCleanupJob.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixedConfiguration()
	  public virtual void testMixedConfiguration()
	  {
		IDictionary<string, string> map = new Dictionary<string, string>();
		map["instance-modification"] = "P20D";
		processEngineConfiguration.BatchOperationHistoryTimeToLive = DEFAULT_TTL_DAYS;
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();

		DateTime startDate = ClockUtil.CurrentTime;
		int daysInThePast = -11;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		Batch modificationBatch = createModificationBatch();
		IList<string> batchIds = new List<string>();
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

		ClockUtil.CurrentTime = DateTime.Now;

		// when
		IList<HistoricBatch> historicList = historyService.createHistoricBatchQuery().list();
		assertEquals(31, historicList.Count);
		runHistoryCleanup();

		// then
		HistoricBatch modificationHistoricBatch = historyService.createHistoricBatchQuery().singleResult(); // the other batches should be cleaned
		assertEquals(modificationBatch.Id, modificationHistoricBatch.Id);
		assertEquals(2, historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(modificationBatch.Id).count());
		batchIds.Remove(modificationBatch.Id);
		foreach (string batchId in batchIds)
		{
		  assertEquals(0, historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batchId).count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWrongGlobalConfiguration()
	  public virtual void testWrongGlobalConfiguration()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid value");
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "PD";
		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWrongSpecificConfiguration()
	  public virtual void testWrongSpecificConfiguration()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid value");
		IDictionary<string, string> map = new Dictionary<string, string>();
		map["instance-modification"] = "PD";
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWrongGlobalConfigurationNegativeTTL()
	  public virtual void testWrongGlobalConfigurationNegativeTTL()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid value");
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P-1D";
		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWrongSpecificConfigurationNegativeTTL()
	  public virtual void testWrongSpecificConfigurationNegativeTTL()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid value");
		IDictionary<string, string> map = new Dictionary<string, string>();
		map["instance-modification"] = "P-5D";
		processEngineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		processEngineConfiguration.BatchOperationsForHistoryCleanup = map;
		processEngineConfiguration.initHistoryCleanup();
	  }

	  private void initBatchOperationHistoryTimeToLive(string days)
	  {
		processEngineConfiguration.BatchOperationHistoryTimeToLive = days;
		processEngineConfiguration.initHistoryCleanup();
	  }

	  private BpmnModelInstance createModelInstance()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent("start").userTask("userTask1").sequenceFlowId("seq").userTask("userTask2").endEvent("end").done();
		return instance;
	  }

	  private void prepareHistoricBatches(int batchesCount, int daysInThePast)
	  {
		DateTime startDate = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(startDate, daysInThePast);

		IList<Batch> list = new List<Batch>();
		for (int i = 0; i < batchesCount; i++)
		{
		  list.Add(migrationHelper.migrateProcessInstancesAsync(1));
		}

		foreach (Batch batch in list)
		{
		  migrationHelper.executeSeedJob(batch);
		  migrationHelper.executeJobs(batch);

		  ClockUtil.CurrentTime = DateUtils.setMinutes(DateUtils.addDays(startDate, ++daysInThePast), random.Next(60));
		  migrationHelper.executeMonitorJob(batch);
		}

		ClockUtil.CurrentTime = DateTime.Now;
	  }

	  private Batch createFailingMigrationBatch()
	  {
		BpmnModelInstance instance = createModelInstance();

		ProcessDefinition sourceProcessDefinition = migrationRule.deployAndGetDefinition(instance);
		ProcessDefinition targetProcessDefinition = migrationRule.deployAndGetDefinition(instance);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Batch batch = runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id, "unknownId")).executeAsync();
		return batch;
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

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void verifyByteArraysWereRemoved(final String... errorDetailsByteArrayIds)
	  private void verifyByteArraysWereRemoved(params string[] errorDetailsByteArrayIds)
	  {
		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, errorDetailsByteArrayIds));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly HistoryCleanupHistoricBatchTest outerInstance;

		  private string[] errorDetailsByteArrayIds;

		  public CommandAnonymousInnerClass2(HistoryCleanupHistoricBatchTest outerInstance, string[] errorDetailsByteArrayIds)
		  {
			  this.outerInstance = outerInstance;
			  this.errorDetailsByteArrayIds = errorDetailsByteArrayIds;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			foreach (string errorDetailsByteArrayId in errorDetailsByteArrayIds)
			{
			  assertNull(commandContext.DbEntityManager.selectOne("selectByteArray", errorDetailsByteArrayId));
			}
			return null;
		  }
	  }

	  private IList<string> findExceptionByteArrayIds()
	  {
		IList<string> exceptionByteArrayIds = new List<string>();
		IList<HistoricJobLog> historicJobLogs = historyService.createHistoricJobLogQuery().list();
		foreach (HistoricJobLog historicJobLog in historicJobLogs)
		{
		  HistoricJobLogEventEntity historicJobLogEventEntity = (HistoricJobLogEventEntity) historicJobLog;
		  if (!string.ReferenceEquals(historicJobLogEventEntity.ExceptionByteArrayId, null))
		  {
			exceptionByteArrayIds.Add(historicJobLogEventEntity.ExceptionByteArrayId);
		  }
		}
		return exceptionByteArrayIds;
	  }

	}

}