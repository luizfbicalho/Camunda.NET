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
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using BatchSeedJobHandler = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using BatchHelper = org.camunda.bpm.engine.test.api.runtime.BatchHelper;
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
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class BatchHistoricDecisionInstanceDeletionTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BatchHistoricDecisionInstanceDeletionTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchHistoricDecisionInstanceDeletionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(rule);
			helper = new BatchDeletionHelper(this, rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}


	  protected internal static string DECISION = "decision";
	  protected internal static readonly DateTime TEST_DATE = new DateTime(1457326800000L);

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchDeletionHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  private int defaultBatchJobsPerSeed;
	  private int defaultInvocationsPerBatchJob;
	  private bool defaultEnsureJobDueDateSet;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;

	  protected internal IList<string> decisionInstanceIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public boolean ensureJobDueDateSet;
	  public bool ensureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public java.util.Date currentTime;
	  public DateTime currentTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Job DueDate is set: {0}") public static java.util.Collection<Object[]> scenarios() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {false, null},
			new object[] {true, TEST_DATE}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		ClockUtil.CurrentTime = TEST_DATE;
		historyService = rule.HistoryService;
		decisionService = rule.DecisionService;
		decisionInstanceIds = new List<string>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void storeEngineSettings()
	  public virtual void storeEngineSettings()
	  {
		configuration = rule.ProcessEngineConfiguration;
		defaultEnsureJobDueDateSet = configuration.EnsureJobDueDateNotNull;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
		configuration.EnsureJobDueDateNotNull = ensureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void executeDecisionInstances()
	  public virtual void executeDecisionInstances()
	  {
		testRule.deploy("org/camunda/bpm/engine/test/api/dmn/Example.dmn");

		VariableMap variables = Variables.createVariables().putValue("status", "silver").putValue("sum", 723);

		for (int i = 0; i < 10; i++)
		{
		  decisionService.evaluateDecisionByKey(DECISION).variables(variables).evaluate();
		}

		IList<HistoricDecisionInstance> decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		foreach (HistoricDecisionInstance decisionInstance in decisionInstances)
		{
		  decisionInstanceIds.Add(decisionInstance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreEngineSettings()
	  public virtual void restoreEngineSettings()
	  {
		configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
		configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
		configuration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchDeletionByIds()
	  public virtual void createBatchDeletionByIds()
	  {
		// when
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		// then
		assertBatchCreated(batch, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchDeletionByInvalidIds()
	  public virtual void createBatchDeletionByInvalidIds()
	  {
		// then
		thrown.expect(typeof(BadUserRequestException));

		// when
		historyService.deleteHistoricDecisionInstancesAsync((IList<string>) null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchDeletionByQuery()
	  public virtual void createBatchDeletionByQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		// when
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(query, null);

		// then
		assertBatchCreated(batch, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchDeletionByInvalidQuery()
	  public virtual void createBatchDeletionByInvalidQuery()
	  {
		// then
		thrown.expect(typeof(BadUserRequestException));

		// when
		historyService.deleteHistoricDecisionInstancesAsync((HistoricDecisionInstanceQuery) null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchDeletionByInvalidQueryByKey()
	  public virtual void createBatchDeletionByInvalidQueryByKey()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("foo");

		// then
		thrown.expect(typeof(BadUserRequestException));

		// when
		historyService.deleteHistoricDecisionInstancesAsync(query, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchDeletionByIdsAndQuery()
	  public virtual void createBatchDeletionByIdsAndQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		// when
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		// then
		assertBatchCreated(batch, 10);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createSeedJobByIds()
	  public virtual void createSeedJobByIds()
	  {
		// when
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		// then there exists a seed job definition with the batch id as
		// configuration
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);
		assertEquals(batch.Id, seedJobDefinition.JobConfiguration);
		assertEquals(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

		// and there exists a deletion job definition
		JobDefinition deletionJobDefinition = helper.getExecutionJobDefinition(batch);
		assertNotNull(deletionJobDefinition);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_DECISION_INSTANCE_DELETION, deletionJobDefinition.JobType);

		// and a seed job with no relation to a process or execution etc.
		Job seedJob = helper.getSeedJob(batch);
		assertNotNull(seedJob);
		assertEquals(seedJobDefinition.Id, seedJob.JobDefinitionId);
		assertEquals(currentTime, seedJob.Duedate);
		assertNull(seedJob.DeploymentId);
		assertNull(seedJob.ProcessDefinitionId);
		assertNull(seedJob.ProcessDefinitionKey);
		assertNull(seedJob.ProcessInstanceId);
		assertNull(seedJob.ExecutionId);

		// but no deletion jobs where created
		IList<Job> deletionJobs = helper.getExecutionJobs(batch);
		assertEquals(0, deletionJobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createSeedJobByQuery()
	  public virtual void createSeedJobByQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		// when
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		// then there exists a seed job definition with the batch id as
		// configuration
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);
		assertEquals(batch.Id, seedJobDefinition.JobConfiguration);
		assertEquals(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

		// and there exists a deletion job definition
		JobDefinition deletionJobDefinition = helper.getExecutionJobDefinition(batch);
		assertNotNull(deletionJobDefinition);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_DECISION_INSTANCE_DELETION, deletionJobDefinition.JobType);

		// and a seed job with no relation to a process or execution etc.
		Job seedJob = helper.getSeedJob(batch);
		assertNotNull(seedJob);
		assertEquals(seedJobDefinition.Id, seedJob.JobDefinitionId);
		assertEquals(currentTime, seedJob.Duedate);
		assertNull(seedJob.DeploymentId);
		assertNull(seedJob.ProcessDefinitionId);
		assertNull(seedJob.ProcessDefinitionKey);
		assertNull(seedJob.ProcessInstanceId);
		assertNull(seedJob.ExecutionId);

		// but no deletion jobs where created
		IList<Job> deletionJobs = helper.getExecutionJobs(batch);
		assertEquals(0, deletionJobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createSeedJobByIdsAndQuery()
	  public virtual void createSeedJobByIdsAndQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		// when
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(query, null);

		// then there exists a seed job definition with the batch id as
		// configuration
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);
		assertEquals(batch.Id, seedJobDefinition.JobConfiguration);
		assertEquals(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

		// and there exists a deletion job definition
		JobDefinition deletionJobDefinition = helper.getExecutionJobDefinition(batch);
		assertNotNull(deletionJobDefinition);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_DECISION_INSTANCE_DELETION, deletionJobDefinition.JobType);

		// and a seed job with no relation to a process or execution etc.
		Job seedJob = helper.getSeedJob(batch);
		assertNotNull(seedJob);
		assertEquals(seedJobDefinition.Id, seedJob.JobDefinitionId);
		assertEquals(currentTime, seedJob.Duedate);
		assertNull(seedJob.DeploymentId);
		assertNull(seedJob.ProcessDefinitionId);
		assertNull(seedJob.ProcessDefinitionKey);
		assertNull(seedJob.ProcessInstanceId);
		assertNull(seedJob.ExecutionId);

		// but no deletion jobs where created
		IList<Job> deletionJobs = helper.getExecutionJobs(batch);
		assertEquals(0, deletionJobs.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createDeletionJobsByIds()
	  public virtual void createDeletionJobsByIds()
	  {
		// given
		rule.ProcessEngineConfiguration.BatchJobsPerSeed = 5;

		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		JobDefinition deletionJobDefinition = helper.getExecutionJobDefinition(batch);

		// when
		helper.executeSeedJob(batch);

		// then
		IList<Job> deletionJobs = helper.getJobsForDefinition(deletionJobDefinition);
		assertEquals(5, deletionJobs.Count);

		foreach (Job deletionJob in deletionJobs)
		{
		  assertEquals(deletionJobDefinition.Id, deletionJob.JobDefinitionId);
		  assertEquals(currentTime, deletionJob.Duedate);
		  assertNull(deletionJob.ProcessDefinitionId);
		  assertNull(deletionJob.ProcessDefinitionKey);
		  assertNull(deletionJob.ProcessInstanceId);
		  assertNull(deletionJob.ExecutionId);
		}

		// and the seed job still exists
		Job seedJob = helper.getJobForDefinition(seedJobDefinition);
		assertNotNull(seedJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createDeletionJobsByQuery()
	  public virtual void createDeletionJobsByQuery()
	  {
		// given
		rule.ProcessEngineConfiguration.BatchJobsPerSeed = 5;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(query, null);

		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		JobDefinition deletionJobDefinition = helper.getExecutionJobDefinition(batch);

		// when
		helper.executeSeedJob(batch);

		// then
		IList<Job> deletionJobs = helper.getJobsForDefinition(deletionJobDefinition);
		assertEquals(5, deletionJobs.Count);

		foreach (Job deletionJob in deletionJobs)
		{
		  assertEquals(deletionJobDefinition.Id, deletionJob.JobDefinitionId);
		  assertEquals(currentTime, deletionJob.Duedate);
		  assertNull(deletionJob.ProcessDefinitionId);
		  assertNull(deletionJob.ProcessDefinitionKey);
		  assertNull(deletionJob.ProcessInstanceId);
		  assertNull(deletionJob.ExecutionId);
		}

		// and the seed job still exists
		Job seedJob = helper.getJobForDefinition(seedJobDefinition);
		assertNotNull(seedJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createDeletionJobsByIdsAndQuery()
	  public virtual void createDeletionJobsByIdsAndQuery()
	  {
		// given
		rule.ProcessEngineConfiguration.BatchJobsPerSeed = 5;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		JobDefinition deletionJobDefinition = helper.getExecutionJobDefinition(batch);

		// when
		helper.executeSeedJob(batch);

		// then
		IList<Job> deletionJobs = helper.getJobsForDefinition(deletionJobDefinition);
		assertEquals(5, deletionJobs.Count);

		foreach (Job deletionJob in deletionJobs)
		{
		  assertEquals(deletionJobDefinition.Id, deletionJob.JobDefinitionId);
		  assertEquals(currentTime, deletionJob.Duedate);
		  assertNull(deletionJob.ProcessDefinitionId);
		  assertNull(deletionJob.ProcessDefinitionKey);
		  assertNull(deletionJob.ProcessInstanceId);
		  assertNull(deletionJob.ExecutionId);
		}

		// and the seed job still exists
		Job seedJob = helper.getJobForDefinition(seedJobDefinition);
		assertNotNull(seedJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createMonitorJobByIds()
	  public virtual void createMonitorJobByIds()
	  {
		// given
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		// when
		helper.executeSeedJob(batch);

		// then the seed job definition still exists but the seed job is removed
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);

		Job seedJob = helper.getSeedJob(batch);
		assertNull(seedJob);

		// and a monitor job definition and job exists
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertNotNull(monitorJobDefinition);

		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createMonitorJobByQuery()
	  public virtual void createMonitorJobByQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(query, null);

		// when
		helper.executeSeedJob(batch);

		// then the seed job definition still exists but the seed job is removed
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);

		Job seedJob = helper.getSeedJob(batch);
		assertNull(seedJob);

		// and a monitor job definition and job exists
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertNotNull(monitorJobDefinition);

		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createMonitorJobByIdsAndQuery()
	  public virtual void createMonitorJobByIdsAndQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		// when
		helper.executeSeedJob(batch);

		// then the seed job definition still exists but the seed job is removed
		JobDefinition seedJobDefinition = helper.getSeedJobDefinition(batch);
		assertNotNull(seedJobDefinition);

		Job seedJob = helper.getSeedJob(batch);
		assertNull(seedJob);

		// and a monitor job definition and job exists
		JobDefinition monitorJobDefinition = helper.getMonitorJobDefinition(batch);
		assertNotNull(monitorJobDefinition);

		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteInstancesByIds()
	  public virtual void deleteInstancesByIds()
	  {
		// given
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		helper.executeSeedJob(batch);
		IList<Job> deletionJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job deletionJob in deletionJobs)
		{
		  helper.executeJob(deletionJob);
		}

		// then
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteInstancesByQuery()
	  public virtual void deleteInstancesByQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(query, null);

		helper.executeSeedJob(batch);
		IList<Job> deletionJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job deletionJob in deletionJobs)
		{
		  helper.executeJob(deletionJob);
		}

		// then
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteInstancesByIdsAndQuery()
	  public virtual void deleteInstancesByIdsAndQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		helper.executeSeedJob(batch);
		IList<Job> deletionJobs = helper.getExecutionJobs(batch);

		// when
		foreach (Job deletionJob in deletionJobs)
		{
		  helper.executeJob(deletionJob);
		}

		// then
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
	  }

	  protected internal virtual void assertBatchCreated(Batch batch, int decisionInstanceCount)
	  {
		assertNotNull(batch);
		assertNotNull(batch.Id);
		assertEquals(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_DECISION_INSTANCE_DELETION, batch.Type);
		assertEquals(decisionInstanceCount, batch.TotalJobs);
		assertEquals(defaultBatchJobsPerSeed, batch.BatchJobsPerSeed);
		assertEquals(defaultInvocationsPerBatchJob, batch.InvocationsPerBatchJob);
	  }

	  internal class BatchDeletionHelper : BatchHelper
	  {
		  private readonly BatchHistoricDecisionInstanceDeletionTest outerInstance;


		public BatchDeletionHelper(BatchHistoricDecisionInstanceDeletionTest outerInstance, ProcessEngineRule engineRule) : base(engineRule)
		{
			this.outerInstance = outerInstance;
		}

		public override JobDefinition getExecutionJobDefinition(Batch batch)
		{
		  return engineRule.ManagementService.createJobDefinitionQuery().jobDefinitionId(batch.BatchJobDefinitionId).jobType(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_DECISION_INSTANCE_DELETION).singleResult();
		}
	  }

	}

}