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
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchWindowConfiguration = org.camunda.bpm.engine.impl.cfg.BatchWindowConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryCleanupCmd = org.camunda.bpm.engine.impl.cmd.HistoryCleanupCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoryCleanupHelper = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHelper;
	using HistoryCleanupJobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandlerConfiguration;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TestPojo = org.camunda.bpm.engine.test.dmn.businessruletask.TestPojo;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE_HISTORY_CLEANUP_JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoryCleanupTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  private const int PROCESS_INSTANCES_COUNT = 3;
	  private const int DECISIONS_IN_PROCESS_INSTANCES = 3;
	  private const int DECISION_INSTANCES_COUNT = 10;
	  private const int CASE_INSTANCES_COUNT = 4;
	  private const int HISTORY_TIME_TO_LIVE = 5;
	  private const int DAYS_IN_THE_PAST = -6;
	  protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";
	  protected internal const string DECISION = "decision";
	  protected internal const string ONE_TASK_CASE = "case";
	  private const int NUMBER_OF_THREADS = 3;
	  private const string USER_ID = "demo";

	  private static readonly SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

	  protected internal string defaultStartTime;
	  protected internal string defaultEndTime;
	  protected internal int defaultBatchSize;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.HistoryCleanupBatchSize = 20;
			configuration.HistoryCleanupBatchThreshold = 10;
			configuration.DefaultNumberOfRetries = 5;
			configuration.HistoryCleanupDegreeOfParallelism = NUMBER_OF_THREADS;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

	  private Random random = new Random();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private HistoryService historyService;
	  private RuntimeService runtimeService;
	  private ManagementService managementService;
	  private CaseService caseService;
	  private RepositoryService repositoryService;
	  private IdentityService identityService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		caseService = engineRule.CaseService;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		testRule.deploy("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/dmn/Example.dmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithHistoryTimeToLive.cmmn");
		defaultStartTime = processEngineConfiguration.HistoryCleanupBatchWindowStartTime;
		defaultEndTime = processEngineConfiguration.HistoryCleanupBatchWindowEndTime;
		defaultBatchSize = processEngineConfiguration.HistoryCleanupBatchSize;
		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;

		identityService.AuthenticatedUserId = USER_ID;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		//reset configuration changes
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = defaultStartTime;
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = defaultEndTime;
		processEngineConfiguration.HistoryCleanupBatchSize = defaultBatchSize;
		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		{
		  historyService.deleteHistoricProcessInstance(historicProcessInstance.Id);
		}

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
		}

		IList<HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().list();
		foreach (HistoricCaseInstance historicCaseInstance in historicCaseInstances)
		{
		  historyService.deleteHistoricCaseInstance(historicCaseInstance.Id);
		}

		clearMetrics();

		identityService.clearAuthentication();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupTest outerInstance;

		  public CommandAnonymousInnerClass(HistoryCleanupTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.historyService.findHistoryCleanupJobs();
			foreach (Job job in jobs)
			{
			  commandContext.JobManager.deleteJob((JobEntity) job);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			}

			//cleanup "detached" historic job logs
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricJobLog> list = historyService.createHistoricJobLogQuery().list();
			IList<HistoricJobLog> list = outerInstance.historyService.createHistoricJobLogQuery().list();
			foreach (HistoricJobLog jobLog in list)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobLog.JobId);
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
//ORIGINAL LINE: @Test public void testHistoryCleanupManualRun()
	  public virtual void testHistoryCleanupManualRun()
	  {
		//given
		prepareData(15);

		ClockUtil.CurrentTime = DateTime.Now;
		//when
		runHistoryCleanup(true);

		//then
		assertResult(0);


		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_CREATE_HISTORY_CLEANUP_JOB).list();

		assertEquals(1, userOperationLogEntries.Count);

		UserOperationLogEntry entry = userOperationLogEntries[0];
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDataSplitBetweenThreads()
	  public virtual void testDataSplitBetweenThreads()
	  {
		//given
		prepareData(15);

		ClockUtil.CurrentTime = DateTime.Now;

		//when
		historyService.cleanUpHistoryAsync(true).Id;
		foreach (Job job in historyService.findHistoryCleanupJobs())
		{
		  managementService.executeJob(job.Id);
		  //assert that the corresponding data was removed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandlerConfiguration jobHandlerConfiguration = getHistoryCleanupJobHandlerConfiguration(job);
		  HistoryCleanupJobHandlerConfiguration jobHandlerConfiguration = getHistoryCleanupJobHandlerConfiguration(job);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int minuteFrom = jobHandlerConfiguration.getMinuteFrom();
		  int minuteFrom = jobHandlerConfiguration.MinuteFrom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int minuteTo = jobHandlerConfiguration.getMinuteTo();
		  int minuteTo = jobHandlerConfiguration.MinuteTo;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();
		  IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();
		  foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		  {
			if (historicProcessInstance.EndTime != null)
			{
			  DateTime calendar = new DateTime();
			  calendar = new DateTime(historicProcessInstance.EndTime);
			  assertTrue(minuteFrom > calendar.Minute || calendar.Minute > minuteTo);
			}
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		  IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		  foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		  {
			if (historicDecisionInstance.EvaluationTime != null)
			{
			  DateTime calendar = new DateTime();
			  calendar = new DateTime(historicDecisionInstance.EvaluationTime);
			  assertTrue(minuteFrom > calendar.Minute || calendar.Minute > minuteTo);
			}
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().list();
		  IList<HistoricCaseInstance> historicCaseInstances = historyService.createHistoricCaseInstanceQuery().list();
		  foreach (HistoricCaseInstance historicCaseInstance in historicCaseInstances)
		  {
			if (historicCaseInstance.CloseTime != null)
			{
			  DateTime calendar = new DateTime();
			  calendar = new DateTime(historicCaseInstance.CloseTime);
			  assertTrue(minuteFrom > calendar.Minute || calendar.Minute > minuteTo);
			}
		  }

		}

		assertResult(0);
	  }

	  private HistoryCleanupJobHandlerConfiguration getHistoryCleanupJobHandlerConfiguration(Job job)
	  {
		return HistoryCleanupJobHandlerConfiguration.fromJson(JsonUtil.asObject(((JobEntity) job).JobHandlerConfigurationRaw));
	  }

	  private void runHistoryCleanup()
	  {
		runHistoryCleanup(false);
	  }

	  private void runHistoryCleanup(bool manualRun)
	  {
		historyService.cleanUpHistoryAsync(manualRun);

		foreach (Job job in historyService.findHistoryCleanupJobs())
		{
		  managementService.executeJob(job.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupMetrics()
	  public virtual void testHistoryCleanupMetrics()
	  {
		//given
		processEngineConfiguration.HistoryCleanupMetricsEnabled = true;
		prepareData(15);

		ClockUtil.CurrentTime = DateTime.Now;
		//when
		runHistoryCleanup(true);

		//then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long removedProcessInstances = managementService.createMetricsQuery().name(org.camunda.bpm.engine.management.Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES).sum();
		long removedProcessInstances = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES).sum();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long removedDecisionInstances = managementService.createMetricsQuery().name(org.camunda.bpm.engine.management.Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES).sum();
		long removedDecisionInstances = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES).sum();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long removedCaseInstances = managementService.createMetricsQuery().name(org.camunda.bpm.engine.management.Metrics.HISTORY_CLEANUP_REMOVED_CASE_INSTANCES).sum();
		long removedCaseInstances = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_CASE_INSTANCES).sum();

		assertTrue(removedProcessInstances > 0);
		assertTrue(removedDecisionInstances > 0);
		assertTrue(removedCaseInstances > 0);

		assertEquals(15, removedProcessInstances + removedCaseInstances + removedDecisionInstances);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupMetricsExtend()
	  public virtual void testHistoryCleanupMetricsExtend()
	  {
		DateTime currentDate = DateTime.Now;
		// given
		processEngineConfiguration.HistoryCleanupMetricsEnabled = true;
		prepareData(15);

		ClockUtil.CurrentTime = currentDate;
		// when
		runHistoryCleanup(true);

		// assume
		assertResult(0);

		// then
		MetricsQuery processMetricsQuery = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES);
		long removedProcessInstances = processMetricsQuery.startDate(DateUtils.addDays(currentDate, DAYS_IN_THE_PAST)).endDate(DateUtils.addHours(currentDate, 1)).sum();
		assertEquals(5, removedProcessInstances);
		MetricsQuery decisionMetricsQuery = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES);
		long removedDecisionInstances = decisionMetricsQuery.startDate(DateUtils.addDays(currentDate, DAYS_IN_THE_PAST)).endDate(DateUtils.addHours(currentDate, 1)).sum();
		assertEquals(5, removedDecisionInstances);
		MetricsQuery caseMetricsQuery = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_CASE_INSTANCES);
		long removedCaseInstances = caseMetricsQuery.startDate(DateUtils.addDays(currentDate, DAYS_IN_THE_PAST)).endDate(DateUtils.addHours(currentDate, 1)).sum();
		assertEquals(5, removedCaseInstances);

		long noneProcessInstances = processMetricsQuery.startDate(DateUtils.addHours(currentDate, 1)).limit(1).sum();
		assertEquals(0, noneProcessInstances);
		long noneDecisionInstances = decisionMetricsQuery.startDate(DateUtils.addHours(currentDate, 1)).limit(1).sum();
		assertEquals(0, noneDecisionInstances);
		long noneCaseInstances = caseMetricsQuery.startDate(DateUtils.addHours(currentDate, 1)).limit(1).sum();
		assertEquals(0, noneCaseInstances);

		IList<MetricIntervalValue> piList = processMetricsQuery.startDate(currentDate).interval(900);
		assertEquals(1, piList.Count);
		assertEquals(5, piList[0].Value);
		IList<MetricIntervalValue> diList = decisionMetricsQuery.startDate(DateUtils.addDays(currentDate, DAYS_IN_THE_PAST)).interval(900);
		assertEquals(1, diList.Count);
		assertEquals(5, diList[0].Value);
		IList<MetricIntervalValue> ciList = caseMetricsQuery.startDate(DateUtils.addDays(currentDate, DAYS_IN_THE_PAST)).interval(900);
		assertEquals(1, ciList.Count);
		assertEquals(5, ciList[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupOnlyDecisionInstancesRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupOnlyDecisionInstancesRemoved()
	  {
		// given
		prepareInstances(null, HISTORY_TIME_TO_LIVE, null);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		runHistoryCleanup(true);

		// then
		assertEquals(PROCESS_INSTANCES_COUNT, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(CASE_INSTANCES_COUNT, historyService.createHistoricCaseInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn"}) public void testHistoryCleanupOnlyProcessInstancesRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn"})]
	  public virtual void testHistoryCleanupOnlyProcessInstancesRemoved()
	  {
		// given
		prepareInstances(HISTORY_TIME_TO_LIVE, null, null);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		runHistoryCleanup(true);

		// then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(DECISION_INSTANCES_COUNT + DECISIONS_IN_PROCESS_INSTANCES, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(CASE_INSTANCES_COUNT, historyService.createHistoricCaseInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupOnlyCaseInstancesRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupOnlyCaseInstancesRemoved()
	  {
		// given
		prepareInstances(null, null, HISTORY_TIME_TO_LIVE);

		ClockUtil.CurrentTime = DateTime.Now;

		// when
		runHistoryCleanup(true);

		// then
		assertEquals(PROCESS_INSTANCES_COUNT, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(DECISION_INSTANCES_COUNT + DECISIONS_IN_PROCESS_INSTANCES, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(0, historyService.createHistoricCaseInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupOnlyDecisionInstancesNotRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupOnlyDecisionInstancesNotRemoved()
	  {
		// given
		prepareInstances(HISTORY_TIME_TO_LIVE, null, HISTORY_TIME_TO_LIVE);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		runHistoryCleanup(true);

		// then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(DECISION_INSTANCES_COUNT + DECISIONS_IN_PROCESS_INSTANCES, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(0, historyService.createHistoricCaseInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupOnlyProcessInstancesNotRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupOnlyProcessInstancesNotRemoved()
	  {
		// given
		prepareInstances(null, HISTORY_TIME_TO_LIVE, HISTORY_TIME_TO_LIVE);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		runHistoryCleanup(true);

		// then
		assertEquals(PROCESS_INSTANCES_COUNT, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(0, historyService.createHistoricCaseInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupOnlyCaseInstancesNotRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupOnlyCaseInstancesNotRemoved()
	  {
		// given
		prepareInstances(HISTORY_TIME_TO_LIVE, HISTORY_TIME_TO_LIVE, null);

		ClockUtil.CurrentTime = DateTime.Now;

		// when
		runHistoryCleanup(true);

		// then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(CASE_INSTANCES_COUNT, historyService.createHistoricCaseInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupEverythingRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupEverythingRemoved()
	  {
		// given
		prepareInstances(HISTORY_TIME_TO_LIVE, HISTORY_TIME_TO_LIVE, HISTORY_TIME_TO_LIVE);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		runHistoryCleanup(true);

		// then
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" }) public void testHistoryCleanupNothingRemoved()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn" })]
	  public virtual void testHistoryCleanupNothingRemoved()
	  {
		// given
		prepareInstances(null, null, null);

		ClockUtil.CurrentTime = DateTime.Now;
		// when
		runHistoryCleanup(true);

		// then
		assertEquals(PROCESS_INSTANCES_COUNT, historyService.createHistoricProcessInstanceQuery().count());
		assertEquals(DECISION_INSTANCES_COUNT + DECISIONS_IN_PROCESS_INSTANCES, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(CASE_INSTANCES_COUNT, historyService.createHistoricCaseInstanceQuery().count());
	  }

	  private void prepareInstances(int? processInstanceTimeToLive, int? decisionTimeToLive, int? caseTimeToLive)
	  {
		//update time to live
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("testProcess").list();
		assertEquals(1, processDefinitions.Count);
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, processInstanceTimeToLive);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.repository.DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").list();
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("testDecision").list();
		assertEquals(1, decisionDefinitions.Count);
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, decisionTimeToLive);

		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionKey("oneTaskCase").list();
		assertEquals(1, caseDefinitions.Count);
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitions[0].Id, caseTimeToLive);

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, DAYS_IN_THE_PAST);

		//create 3 process instances
		IList<string> processInstanceIds = new List<string>();
		IDictionary<string, object> variables = Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37));
		for (int i = 0; i < PROCESS_INSTANCES_COUNT; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);
		  processInstanceIds.Add(processInstance.Id);
		}
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);

		//+10 standalone decisions
		for (int i = 0; i < DECISION_INSTANCES_COUNT; i++)
		{
		  engineRule.DecisionService.evaluateDecisionByKey("testDecision").variables(variables).evaluate();
		}

		// create 4 case instances
		for (int i = 0; i < CASE_INSTANCES_COUNT; i++)
		{
		  CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37 + i)));
		  caseService.terminateCaseExecution(caseInstance.Id);
		  caseService.closeCaseInstance(caseInstance.Id);
		}

		ClockUtil.CurrentTime = oldCurrentTime;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupWithinBatchWindow()
	  public virtual void testHistoryCleanupWithinBatchWindow()
	  {
		//given
		prepareData(15);

		//we're within batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(now);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, HISTORY_TIME_TO_LIVE));
		processEngineConfiguration.initHistoryCleanup();

		//when
		runHistoryCleanup();

		//then
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupJobNullTTL()
	  public virtual void testHistoryCleanupJobNullTTL()
	  {
		//given
		removeHistoryTimeToLive();

		prepareData(15);

		ClockUtil.CurrentTime = DateTime.Now;
		//when
		runHistoryCleanup(true);

		//then
		assertResult(15);
	  }

	  private void removeHistoryTimeToLive()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(ONE_TASK_PROCESS).list();
		assertEquals(1, processDefinitions.Count);
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitions[0].Id, null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.repository.DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION).list();
		IList<DecisionDefinition> decisionDefinitions = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION).list();
		assertEquals(1, decisionDefinitions.Count);
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitions[0].Id, null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.repository.CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).list();
		IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(ONE_TASK_CASE).list();
		assertEquals(1, caseDefinitions.Count);
		repositoryService.updateCaseDefinitionHistoryTimeToLive(caseDefinitions[0].Id, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml" }) public void testHistoryCleanupJobDefaultTTL()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml" })]
	  public virtual void testHistoryCleanupJobDefaultTTL()
	  {
		//given
		prepareBPMNData(15, "twoTasksProcess");

		ClockUtil.CurrentTime = DateTime.Now;
		//when
		runHistoryCleanup(true);

		//then
		assertResult(15);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindHistoryCleanupJob()
	  public virtual void testFindHistoryCleanupJob()
	  {
		//given
		historyService.cleanUpHistoryAsync(true).Id;

		//when
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();

		//then
		assertEquals(NUMBER_OF_THREADS, historyCleanupJobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRescheduleForNever()
	  public virtual void testRescheduleForNever()
	  {
		//given

		//force creation of job
		historyService.cleanUpHistoryAsync(true);
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		foreach (Job job in historyCleanupJobs)
		{
		  assertNotNull(job.Duedate);
		}

		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = null;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = null;
		processEngineConfiguration.initHistoryCleanup();

		ClockUtil.CurrentTime = DateTime.Now;

		//when
		historyService.cleanUpHistoryAsync(false);

		//then
		historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  assertTrue(job.Suspended);
		  assertNull(job.Duedate);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupJobResolveIncident()
	  public virtual void testHistoryCleanupJobResolveIncident()
	  {
		//given
		string jobId = historyService.cleanUpHistoryAsync(true).Id;
		imitateFailedJob(jobId);

		assertEquals(5, processEngineConfiguration.DefaultNumberOfRetries);
		//when
		//call to cleanup history means that incident was resolved
		jobId = historyService.cleanUpHistoryAsync(true).Id;

		//then
		JobEntity jobEntity = getJobEntity(jobId);
		assertEquals(5, jobEntity.Retries);
		assertEquals(null, jobEntity.ExceptionByteArrayId);
		assertEquals(null, jobEntity.ExceptionMessage);

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void imitateFailedJob(final String jobId)
	  private void imitateFailedJob(string jobId)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, jobId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly HistoryCleanupTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass2(HistoryCleanupTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobEntity jobEntity = outerInstance.getJobEntity(jobId);
			jobEntity.Retries = 0;
			jobEntity.ExceptionMessage = "Something bad happened";
			jobEntity.ExceptionStacktrace = ExceptionUtil.getExceptionStacktrace(new Exception("Something bad happened"));
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdManualRun()
	  public virtual void testLessThanThresholdManualRun()
	  {
		//given
		prepareData(5);

		ClockUtil.CurrentTime = DateTime.Now;
		//when
		runHistoryCleanup(true);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());

		foreach (Job job in historyService.findHistoryCleanupJobs())
		{
		  assertTrue(job.Suspended);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotEnoughTimeToDeleteEverything()
	  public virtual void testNotEnoughTimeToDeleteEverything()
	  {
		//given
		//we have something to cleanup
		prepareData(80);
		//we call history cleanup within batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(now);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, HISTORY_TIME_TO_LIVE));
		processEngineConfiguration.initHistoryCleanup();
		//job is executed once within batch window
		//we run the job in 3 threads, so not more than 60 instances can be removed in one run
		runHistoryCleanup();

		//when
		//time passed -> outside batch window
		ClockUtil.CurrentTime = DateUtils.addHours(now, 6);
		//the job is called for the second time
		foreach (Job job in historyService.findHistoryCleanupJobs())
		{
		  managementService.executeJob(job.Id);
		}

		//then
		//second execution was not able to delete rest data
		assertResultNotLess(20);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testManualRunDoesNotRespectBatchWindow()
	  public virtual void testManualRunDoesNotRespectBatchWindow()
	  {
		//given
		//we have something to cleanup
		int processInstanceCount = 40;
		prepareData(processInstanceCount);

		//we call history cleanup outside batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, 1)); //now + 1 hour
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, HISTORY_TIME_TO_LIVE)); //now + 5 hours
		processEngineConfiguration.initHistoryCleanup();

		//when
		//job is executed before batch window start
		runHistoryCleanup(true);

		//the job is called for the second time after batch window end
		ClockUtil.CurrentTime = DateUtils.addHours(now, 6); //now + 6 hours
		foreach (Job job in historyService.findHistoryCleanupJobs())
		{
		  managementService.executeJob(job.Id);
		}

		//then
		assertResult(0);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdWithinBatchWindow()
	  public virtual void testLessThanThresholdWithinBatchWindow()
	  {
		//given
		prepareData(5);

		//we're within batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(now);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, HISTORY_TIME_TO_LIVE));
		processEngineConfiguration.initHistoryCleanup();

		//when
		runHistoryCleanup();

		//then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till current time + delay
		  DateTime nextRun = getNextRunWithDelay(ClockUtil.CurrentTime, 0);
		  assertTrue(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
		  DateTime nextRunMax = DateUtils.addSeconds(ClockUtil.CurrentTime, HistoryCleanupJobHandlerConfiguration.MAX_DELAY);
		  assertTrue(jobEntity.Duedate < nextRunMax);

		  //countEmptyRuns incremented
		  assertEquals(1, configuration.CountEmptyRuns);
		}

		//data is still removed
		assertResult(0);
	  }

	  private DateTime getNextRunWithDelay(DateTime date, int countEmptyRuns)
	  {
		//ignore milliseconds because MySQL does not support them, and it's not important for test
		return DateUtils.setMilliseconds(DateUtils.addSeconds(date, Math.Min((int)(Math.Pow(2.0, countEmptyRuns) * HistoryCleanupJobHandlerConfiguration.START_DELAY), HistoryCleanupJobHandlerConfiguration.MAX_DELAY)), 0);
	  }

	  private JobEntity getJobEntity(string jobId)
	  {
		return (JobEntity)managementService.createJobQuery().jobId(jobId).list().get(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdWithinBatchWindowAgain()
	  public virtual void testLessThanThresholdWithinBatchWindowAgain()
	  {
		//given
		prepareData(5);

		//we're within batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(now);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, 1));
		processEngineConfiguration.initHistoryCleanup();

		//when
		historyService.cleanUpHistoryAsync();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		for (int i = 1; i <= 6; i++)
		{
		  foreach (Job job in historyCleanupJobs)
		  {
			managementService.executeJob(job.Id);
		  }
		}

		//then
		historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till current time + (2 power count)*delay
		  DateTime nextRun = getNextRunWithDelay(ClockUtil.CurrentTime, HISTORY_TIME_TO_LIVE);
		  assertTrue(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
		  DateTime nextRunMax = DateUtils.addSeconds(ClockUtil.CurrentTime, HistoryCleanupJobHandlerConfiguration.MAX_DELAY);
		  assertTrue(jobEntity.Duedate < nextRunMax);

		  //countEmptyRuns incremented
		  assertEquals(6, configuration.CountEmptyRuns);
		}

		//data is still removed
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdWithinBatchWindowMaxDelayReached()
	  public virtual void testLessThanThresholdWithinBatchWindowMaxDelayReached()
	  {
		//given
		prepareData(5);

		//we're within batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(now);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(now, 2));
		processEngineConfiguration.initHistoryCleanup();

		//when
		historyService.cleanUpHistoryAsync();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		for (int i = 1; i <= 11; i++)
		{
		  foreach (Job job in historyCleanupJobs)
		  {
			managementService.executeJob(job.Id);
		  }
		}

		//then
		historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till current time + max delay
		  DateTime nextRun = getNextRunWithDelay(ClockUtil.CurrentTime, 10);
		  assertTrue(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
		  assertTrue(jobEntity.Duedate < getNextRunWithinBatchWindow(now));

		  //countEmptyRuns incremented
		  assertEquals(11, configuration.CountEmptyRuns);
		}

		//data is still removed
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdCloseToBatchWindowEndTime()
	  public virtual void testLessThanThresholdCloseToBatchWindowEndTime()
	  {
		//given
		prepareData(5);

		//we're within batch window
		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(now);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addMinutes(now, 30));
		processEngineConfiguration.initHistoryCleanup();

		//when
		historyService.cleanUpHistoryAsync();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		for (int i = 1; i <= 9; i++)
		{
		  foreach (Job job in historyCleanupJobs)
		  {
			managementService.executeJob(job.Id);
		  }
		}

		//then
		historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity)job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till next batch window start time
		  DateTime nextRun = getNextRunWithinBatchWindow(ClockUtil.CurrentTime);
		  assertTrue(jobEntity.Duedate.Equals(nextRun));

		  //countEmptyRuns canceled
		  assertEquals(0, configuration.CountEmptyRuns);
		}

		//data is still removed
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdOutsideBatchWindow()
	  public virtual void testLessThanThresholdOutsideBatchWindow()
	  {
		//given
		prepareData(5);

		//we're outside batch window
		DateTime twoHoursAgo = DateTime.Now;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = (new SimpleDateFormat("HH:mm")).format(twoHoursAgo);
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = (new SimpleDateFormat("HH:mm")).format(DateUtils.addHours(twoHoursAgo, 1));
		processEngineConfiguration.initHistoryCleanup();
		ClockUtil.CurrentTime = DateUtils.addHours(twoHoursAgo, 2);

		//when
		for (int i = 1; i <= 3; i++)
		{
		  runHistoryCleanup();
		}

		//then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till next batch window start
		  DateTime nextRun = getNextRunWithinBatchWindow(ClockUtil.CurrentTime);
		  assertTrue(jobEntity.Duedate.Equals(nextRun));

		  //countEmptyRuns canceled
		  assertEquals(0, configuration.CountEmptyRuns);
		}

		//nothing was removed
		assertResult(5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdOutsideBatchWindowAfterMidnight()
	  public virtual void testLessThanThresholdOutsideBatchWindowAfterMidnight()
	  {
		//given
		prepareData(5);

		//we're outside batch window, batch window passes midnight
		DateTime date = DateTime.Now;
		ClockUtil.CurrentTime = DateUtils.setMinutes(DateUtils.setHours(date, 1), 10); //01:10
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
		processEngineConfiguration.initHistoryCleanup();

		//when
		string jobId = historyService.cleanUpHistoryAsync().Id;
		managementService.executeJob(jobId);

		//then
		JobEntity jobEntity = getJobEntity(jobId);
		HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		//job rescheduled till next batch window start
		DateTime nextRun = getNextRunWithinBatchWindow(ClockUtil.CurrentTime);
		assertTrue(jobEntity.Duedate.Equals(nextRun));
		assertTrue(nextRun > ClockUtil.CurrentTime);

		//countEmptyRuns canceled
		assertEquals(0, configuration.CountEmptyRuns);

		//nothing was removed
		assertResult(5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdOutsideBatchWindowBeforeMidnight()
	  public virtual void testLessThanThresholdOutsideBatchWindowBeforeMidnight()
	  {
		//given
		prepareData(5);

		//we're outside batch window, batch window passes midnight
		DateTime date = DateTime.Now;
		ClockUtil.CurrentTime = DateUtils.setMinutes(DateUtils.setHours(date, 22), 10); //22:10
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
		processEngineConfiguration.initHistoryCleanup();

		//when
		string jobId = historyService.cleanUpHistoryAsync().Id;
		managementService.executeJob(jobId);

		//then
		JobEntity jobEntity = getJobEntity(jobId);
		HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		//job rescheduled till next batch window start
		DateTime nextRun = getNextRunWithinBatchWindow(ClockUtil.CurrentTime);
		assertTrue(jobEntity.Duedate.Equals(nextRun));
		assertTrue(nextRun > ClockUtil.CurrentTime);

		//countEmptyRuns cancelled
		assertEquals(0, configuration.CountEmptyRuns);

		//nothing was removed
		assertResult(5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdWithinBatchWindowBeforeMidnight()
	  public virtual void testLessThanThresholdWithinBatchWindowBeforeMidnight()
	  {
		//given
		prepareData(5);

		//we're within batch window, but batch window passes midnight
		DateTime date = DateTime.Now;
		ClockUtil.CurrentTime = DateUtils.setMinutes(DateUtils.setHours(date, 23), 10); //23:10
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
		processEngineConfiguration.initHistoryCleanup();

		//when
		runHistoryCleanup();

		//then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till current time + delay
		  DateTime nextRun = getNextRunWithDelay(ClockUtil.CurrentTime, 0);
		  assertTrue(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
		  DateTime nextRunMax = DateUtils.addSeconds(ClockUtil.CurrentTime, HistoryCleanupJobHandlerConfiguration.MAX_DELAY);
		  assertTrue(jobEntity.Duedate < nextRunMax);

		  //countEmptyRuns incremented
		  assertEquals(1, configuration.CountEmptyRuns);
		}

		//data is still removed
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLessThanThresholdWithinBatchWindowAfterMidnight()
	  public virtual void testLessThanThresholdWithinBatchWindowAfterMidnight()
	  {
		//given
		prepareData(5);

		//we're within batch window, but batch window passes midnight
		DateTime date = DateTime.Now;
		ClockUtil.CurrentTime = DateUtils.setMinutes(DateUtils.setHours(date, 0), 10); //00:10
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
		processEngineConfiguration.initHistoryCleanup();

		//when
		runHistoryCleanup(false);

		//then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in historyCleanupJobs)
		{
		  JobEntity jobEntity = (JobEntity) job;
		  HistoryCleanupJobHandlerConfiguration configuration = getConfiguration(jobEntity);

		  //job rescheduled till current time + delay
		  DateTime nextRun = getNextRunWithDelay(ClockUtil.CurrentTime, 0);
		  assertTrue(jobEntity.Duedate.Equals(nextRun) || jobEntity.Duedate > nextRun);
		  DateTime nextRunMax = DateUtils.addSeconds(ClockUtil.CurrentTime, HistoryCleanupJobHandlerConfiguration.MAX_DELAY);
		  assertTrue(jobEntity.Duedate < nextRunMax);

		  //countEmptyRuns incremented
		  assertEquals(1, configuration.CountEmptyRuns);
		}

		//data is still removed
		assertResult(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfiguration()
	  public virtual void testConfiguration()
	  {
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00+0200";
		processEngineConfiguration.initHistoryCleanup();
		DateTime c = DateTime.getInstance(TimeZone.getTimeZone("GMT+2:00"));
		DateTime startTime = processEngineConfiguration.HistoryCleanupBatchWindowStartTimeAsDate;
		c = new DateTime(startTime);
		assertEquals(23, c.Hour);
		assertEquals(0, c.Minute);
		assertEquals(0, c.Second);

		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		processEngineConfiguration.initHistoryCleanup();
		c = new DateTime();
		startTime = processEngineConfiguration.HistoryCleanupBatchWindowStartTimeAsDate;
		c = new DateTime(startTime);
		assertEquals(23, c.Hour);
		assertEquals(0, c.Minute);
		assertEquals(0, c.Second);

		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:35-0800";
		processEngineConfiguration.initHistoryCleanup();
		c = DateTime.getInstance(TimeZone.getTimeZone("GMT-8:00"));
		DateTime endTime = processEngineConfiguration.HistoryCleanupBatchWindowEndTimeAsDate;
		c = new DateTime(endTime);
		assertEquals(1, c.Hour);
		assertEquals(35, c.Minute);
		assertEquals(0, c.Second);

		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:35";
		processEngineConfiguration.initHistoryCleanup();
		c = new DateTime();
		endTime = processEngineConfiguration.HistoryCleanupBatchWindowEndTimeAsDate;
		c = new DateTime(endTime);
		assertEquals(1, c.Hour);
		assertEquals(35, c.Minute);
		assertEquals(0, c.Second);

		processEngineConfiguration.HistoryCleanupBatchSize = 500;
		processEngineConfiguration.initHistoryCleanup();
		assertEquals(processEngineConfiguration.HistoryCleanupBatchSize, 500);

		processEngineConfiguration.HistoryTimeToLive = "5";
		processEngineConfiguration.initHistoryCleanup();
		assertEquals(5, ParseUtil.parseHistoryTimeToLive(processEngineConfiguration.HistoryTimeToLive).Value);

		processEngineConfiguration.HistoryTimeToLive = "P6D";
		processEngineConfiguration.initHistoryCleanup();
		assertEquals(6, ParseUtil.parseHistoryTimeToLive(processEngineConfiguration.HistoryTimeToLive).Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupHelper() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoryCleanupHelper()
	  {
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "22:00+0100";
		processEngineConfiguration.initHistoryCleanup();

		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssZ");
		DateTime date = sdf.parse("2017-09-06T22:15:00+0100");

		assertTrue(HistoryCleanupHelper.isWithinBatchWindow(date, processEngineConfiguration));

		date = sdf.parse("2017-09-06T22:15:00+0200");
		assertFalse(HistoryCleanupHelper.isWithinBatchWindow(date, processEngineConfiguration));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongStartTime()
	  public virtual void testConfigurationFailureWrongStartTime()
	  {
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23";
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupBatchWindowStartTime");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongDayOfTheWeekStartTime() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testConfigurationFailureWrongDayOfTheWeekStartTime()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("startTime");
		processEngineConfiguration.HistoryCleanupBatchWindows[DayOfWeek.Monday] = new BatchWindowConfiguration("23", "01:00");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongDayOfTheWeekEndTime() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testConfigurationFailureWrongDayOfTheWeekEndTime()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("endTime");
		processEngineConfiguration.HistoryCleanupBatchWindows[DayOfWeek.Monday] = new BatchWindowConfiguration("23:00", "01");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongDegreeOfParallelism()
	  public virtual void testConfigurationFailureWrongDegreeOfParallelism()
	  {
		processEngineConfiguration.HistoryCleanupDegreeOfParallelism = 0;

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupDegreeOfParallelism");

		processEngineConfiguration.initHistoryCleanup();

		processEngineConfiguration.HistoryCleanupDegreeOfParallelism = HistoryCleanupCmd.MAX_THREADS_NUMBER;

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupDegreeOfParallelism");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongEndTime()
	  public virtual void testConfigurationFailureWrongEndTime()
	  {
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = "wrongValue";

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupBatchWindowEndTime");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongBatchSize()
	  public virtual void testConfigurationFailureWrongBatchSize()
	  {
		processEngineConfiguration.HistoryCleanupBatchSize = 501;

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupBatchSize");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongBatchSize2()
	  public virtual void testConfigurationFailureWrongBatchSize2()
	  {
		processEngineConfiguration.HistoryCleanupBatchSize = -5;

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupBatchSize");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureWrongBatchThreshold()
	  public virtual void testConfigurationFailureWrongBatchThreshold()
	  {
		processEngineConfiguration.HistoryCleanupBatchThreshold = -1;

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyCleanupBatchThreshold");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureMalformedHistoryTimeToLive()
	  public virtual void testConfigurationFailureMalformedHistoryTimeToLive()
	  {
		processEngineConfiguration.HistoryTimeToLive = "PP5555DDDD";

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyTimeToLive");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureInvalidHistoryTimeToLive()
	  public virtual void testConfigurationFailureInvalidHistoryTimeToLive()
	  {
		processEngineConfiguration.HistoryTimeToLive = "invalidValue";

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyTimeToLive");

		processEngineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConfigurationFailureNegativeHistoryTimeToLive()
	  public virtual void testConfigurationFailureNegativeHistoryTimeToLive()
	  {
		processEngineConfiguration.HistoryTimeToLive = "-6";

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("historyTimeToLive");

		processEngineConfiguration.initHistoryCleanup();
	  }

	  private DateTime getNextRunWithinBatchWindow(DateTime currentTime)
	  {
		return processEngineConfiguration.BatchWindowManager.getNextBatchWindow(currentTime, processEngineConfiguration).Start;
	  }

	  private HistoryCleanupJobHandlerConfiguration getConfiguration(JobEntity jobEntity)
	  {
		string jobHandlerConfigurationRaw = jobEntity.JobHandlerConfigurationRaw;
		return HistoryCleanupJobHandlerConfiguration.fromJson(JsonUtil.asObject(jobHandlerConfigurationRaw));
	  }

	  private void prepareData(int instanceCount)
	  {
		int createdInstances = instanceCount / 3;
		prepareBPMNData(createdInstances, ONE_TASK_PROCESS);
		prepareDMNData(createdInstances);
		prepareCMMNData(instanceCount - 2 * createdInstances);
	  }

	  private void prepareBPMNData(int instanceCount, string businesskey)
	  {
		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, DAYS_IN_THE_PAST);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses(businesskey, getVariables(), instanceCount);
		IList<string> ids = prepareHistoricProcesses(businesskey, Variables, instanceCount);
		deleteProcessInstances(ids);
		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	  private void deleteProcessInstances(IList<string> ids)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date currentTime = org.camunda.bpm.engine.impl.util.ClockUtil.getCurrentTime();
		DateTime currentTime = ClockUtil.CurrentTime;
		foreach (string id in ids)
		{
		  //spread end_time between different "minutes"
		  ClockUtil.CurrentTime = DateUtils.setMinutes(currentTime, random.Next(60));
		  runtimeService.deleteProcessInstance(id, null, true, true);
		}
	  }

	  private void prepareDMNData(int instanceCount)
	  {
		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, DAYS_IN_THE_PAST);
		for (int i = 0; i < instanceCount; i++)
		{
		  //spread end_time between different "minutes"
		  ClockUtil.CurrentTime = DateUtils.setMinutes(ClockUtil.CurrentTime, random.Next(60));
		  engineRule.DecisionService.evaluateDecisionByKey(DECISION).variables(DMNVariables).evaluate();
		}
		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	  private void prepareCMMNData(int instanceCount)
	  {
		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, DAYS_IN_THE_PAST);

		for (int i = 0; i < instanceCount; i++)
		{
		  CaseInstance caseInstance = caseService.createCaseInstanceByKey(ONE_TASK_CASE);
		  //spread end_time between different "minutes"
		  ClockUtil.CurrentTime = DateUtils.setMinutes(ClockUtil.CurrentTime, random.Next(60));
		  caseService.terminateCaseExecution(caseInstance.Id);
		  caseService.closeCaseInstance(caseInstance.Id);
		}
		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey, VariableMap variables, int? processInstanceCount)
	  {
		IList<string> processInstanceIds = new List<string>();

		for (int i = 0; i < processInstanceCount.Value; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(businessKey, variables);
		  processInstanceIds.Add(processInstance.Id);
		}

		return processInstanceIds;
	  }

	  private VariableMap Variables
	  {
		  get
		  {
			return Variables.createVariables().putValue("aVariableName", "aVariableValue").putValue("anotherVariableName", "anotherVariableValue");
		  }
	  }

	  protected internal virtual VariableMap DMNVariables
	  {
		  get
		  {
			return Variables.createVariables().putValue("status", "silver").putValue("sum", 723);
		  }
	  }

	  private void assertResult(long expectedInstanceCount)
	  {
		long count = historyService.createHistoricProcessInstanceQuery().count() + historyService.createHistoricDecisionInstanceQuery().count() + historyService.createHistoricCaseInstanceQuery().count();
		assertEquals(expectedInstanceCount, count);
	  }

	  private void assertResultNotLess(long expectedInstanceCount)
	  {
		long count = historyService.createHistoricProcessInstanceQuery().count() + historyService.createHistoricDecisionInstanceQuery().count() + historyService.createHistoricCaseInstanceQuery().count();
		assertTrue(expectedInstanceCount <= count);
	  }

	}

}