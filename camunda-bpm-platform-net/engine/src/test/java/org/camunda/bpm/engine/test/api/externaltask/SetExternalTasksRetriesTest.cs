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
namespace org.camunda.bpm.engine.test.api.externaltask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class SetExternalTasksRetriesTest
	{
		private bool InstanceFieldsInitialized = false;

		public SetExternalTasksRetriesTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  private static string PROCESS_DEFINITION_KEY = "oneExternalTaskProcess";
	  private static string PROCESS_DEFINITION_KEY_2 = "twoExternalTaskWithPriorityProcess";

	  private int defaultBatchJobsPerSeed;
	  private int defaultInvocationsPerBatchJob;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal ManagementService managementService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal HistoryService historyService;

	  protected internal IList<string> processInstanceIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		externalTaskService = engineRule.ExternalTaskService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployTestProcesses() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void deployTestProcesses()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = engineRule.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml").deploy();

		engineRule.manageDeployment(deployment);

		RuntimeService runtimeService = engineRule.RuntimeService;
		processInstanceIds = new List<string>();
		for (int i = 0; i < 4; i++)
		{
		  processInstanceIds.Add(runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "").Id);
		}
		processInstanceIds.Add(runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY_2).Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanBatch()
	  public virtual void cleanBatch()
	  {
		IList<Batch> batches = managementService.createBatchQuery().list();
		if (batches.Count > 0)
		{
		  foreach (Batch batch in batches)
		  {
			managementService.deleteBatch(batch.Id, true);
		  }
		}

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void storeEngineSettings()
	  public virtual void storeEngineSettings()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
		defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreEngineSettings()
	  public virtual void restoreEngineSettings()
	  {
		ProcessEngineConfigurationImpl configuration = engineRule.ProcessEngineConfiguration;
		configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
		configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetExternalTaskRetriesSync()
	  public virtual void shouldSetExternalTaskRetriesSync()
	  {

		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}
		// when
		externalTaskService.setRetries(externalTaskIds, 10);

		// then
		externalTasks = externalTaskService.createExternalTaskQuery().list();
		foreach (ExternalTask task in externalTasks)
		{
		 Assert.assertEquals(10, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNonExistingExternalTaskIdSync()
	  public virtual void shouldFailForNonExistingExternalTaskIdSync()
	  {

		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}

		externalTaskIds.Add("nonExistingExternalTaskId");

		try
		{
		  externalTaskService.setRetries(externalTaskIds, 10);
		  fail("exception expected");
		}
		catch (NotFoundException e)
		{
		  Assert.assertThat(e.Message, containsString("Cannot find external task with id nonExistingExternalTaskId"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNullExternalTaskIdSync()
	  public virtual void shouldFailForNullExternalTaskIdSync()
	  {

		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}

		externalTaskIds.Add(null);

		try
		{
		  externalTaskService.setRetries(externalTaskIds, 10);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("External task id cannot be null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNullExternalTaskIdsSync()
	  public virtual void shouldFailForNullExternalTaskIdsSync()
	  {
		try
		{
		  externalTaskService.setRetries((IList<string>) null, 10);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("externalTaskIds is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNonExistingExternalTaskIdAsync()
	  public virtual void shouldFailForNonExistingExternalTaskIdAsync()
	  {

		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}

		externalTaskIds.Add("nonExistingExternalTaskId");
		Batch batch = externalTaskService.setRetriesAsync(externalTaskIds, null, 10);

		try
		{
		  executeSeedAndBatchJobs(batch);
		  fail("exception expected");
		}
		catch (NotFoundException e)
		{
		  Assert.assertThat(e.Message, containsString("Cannot find external task with id nonExistingExternalTaskId"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNullExternalTaskIdAsync()
	  public virtual void shouldFailForNullExternalTaskIdAsync()
	  {

		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}

		externalTaskIds.Add(null);
		Batch batch = null;

		try
		{
		  batch = externalTaskService.setRetriesAsync(externalTaskIds, null, 10);
		  executeSeedAndBatchJobs(batch);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("External task id cannot be null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNullExternalTaskIdsAsync()
	  public virtual void shouldFailForNullExternalTaskIdsAsync()
	  {
		try
		{
		  externalTaskService.setRetriesAsync((IList<string>) null, null, 10);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("externalTaskIds is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNegativeRetriesSync()
	  public virtual void shouldFailForNegativeRetriesSync()
	  {

		IList<string> externalTaskIds = Arrays.asList("externalTaskId");

		try
		{
		  externalTaskService.setRetries(externalTaskIds, -10);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("The number of retries cannot be negative"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailForNegativeRetriesAsync()
	  public virtual void shouldFailForNegativeRetriesAsync()
	  {

		IList<string> externalTaskIds = Arrays.asList("externalTaskId");

		try
		{
		  Batch batch = externalTaskService.setRetriesAsync(externalTaskIds, null, -10);
		  executeSeedAndBatchJobs(batch);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("The number of retries cannot be negative"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetExternalTaskRetriesWithQueryAsync()
	  public virtual void shouldSetExternalTaskRetriesWithQueryAsync()
	  {

		ExternalTaskQuery externalTaskQuery = engineRule.ExternalTaskService.createExternalTaskQuery();

		// when
		Batch batch = externalTaskService.setRetriesAsync(null, externalTaskQuery, 5);

		// then
		executeSeedAndBatchJobs(batch);

		foreach (ExternalTask task in externalTaskQuery.list())
		{
		  Assert.assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetExternalTaskRetriesWithListAsync()
	  public virtual void shouldSetExternalTaskRetriesWithListAsync()
	  {

		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}
		// when
		Batch batch = externalTaskService.setRetriesAsync(externalTaskIds, null, 5);

		// then
		executeSeedAndBatchJobs(batch);

		externalTasks = externalTaskService.createExternalTaskQuery().list();
		foreach (ExternalTask task in externalTasks)
		{
		  Assert.assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetExternalTaskRetriesWithListAndQueryAsync()
	  public virtual void shouldSetExternalTaskRetriesWithListAndQueryAsync()
	  {

		ExternalTaskQuery externalTaskQuery = externalTaskService.createExternalTaskQuery();
		IList<ExternalTask> externalTasks = externalTaskQuery.list();

		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}
		// when
		Batch batch = externalTaskService.setRetriesAsync(externalTaskIds, externalTaskQuery, 5);

		// then
		executeSeedAndBatchJobs(batch);

		externalTasks = externalTaskService.createExternalTaskQuery().list();
		foreach (ExternalTask task in externalTasks)
		{
		  Assert.assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void shouldSetExternalTaskRetriesWithLargeList()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void shouldSetExternalTaskRetriesWithLargeList()
	  {
		// given
		engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 1010;
		IList<string> processIds = startProcessInstance(PROCESS_DEFINITION_KEY, 1100);

		HistoricProcessInstanceQuery processInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		Batch batch = externalTaskService.updateRetries().historicProcessInstanceQuery(processInstanceQuery).setAsync(3);

		createAndExecuteSeedJobs(batch.SeedJobDefinitionId, 2);
		executeBatchJobs(batch);

		// then no error is thrown
		assertHistoricBatchExists();

		// cleanup
		if (!testHelper.HistoryLevelNone)
		{
		  batch = historyService.deleteHistoricProcessInstancesAsync(processIds, null);
		  createAndExecuteSeedJobs(batch.SeedJobDefinitionId, 2);
		  executeBatchJobs(batch);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetExternalTaskRetriesWithDifferentListAndQueryAsync()
	  public virtual void shouldSetExternalTaskRetriesWithDifferentListAndQueryAsync()
	  {
		// given
		ExternalTaskQuery externalTaskQuery = externalTaskService.createExternalTaskQuery().processInstanceId(processInstanceIds[0]);
		IList<ExternalTask> externalTasks = externalTaskService.createExternalTaskQuery().processInstanceId(processInstanceIds[processInstanceIds.Count - 1]).list();
		List<string> externalTaskIds = new List<string>();
		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}

		// when
		Batch batch = externalTaskService.setRetriesAsync(externalTaskIds, externalTaskQuery, 8);
		executeSeedAndBatchJobs(batch);

		// then
		ExternalTask task = externalTaskService.createExternalTaskQuery().processInstanceId(processInstanceIds[0]).singleResult();
		Assert.assertEquals(8, (int) task.Retries);
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().processInstanceId(processInstanceIds[processInstanceIds.Count - 1]).list();
		foreach (ExternalTask t in tasks)
		{
		  Assert.assertEquals(8, (int) t.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateRetriesByExternalTaskIds()
	  public virtual void shouldUpdateRetriesByExternalTaskIds()
	  {
		// given
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		IList<string> externalTaskIds = Arrays.asList(tasks[0].Id, tasks[1].Id, tasks[2].Id, tasks[3].Id, tasks[4].Id, tasks[5].Id);

		// when
		Batch batch = externalTaskService.updateRetries().externalTaskIds(externalTaskIds).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateRetriesByExternalTaskIdArray()
	  public virtual void shouldUpdateRetriesByExternalTaskIdArray()
	  {
		// given
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		IList<string> externalTaskIds = Arrays.asList(tasks[0].Id, tasks[1].Id, tasks[2].Id, tasks[3].Id, tasks[4].Id, tasks[5].Id);

		// when
		Batch batch = externalTaskService.updateRetries().externalTaskIds(externalTaskIds.ToArray()).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateRetriesByProcessInstanceIds()
	  public virtual void shouldUpdateRetriesByProcessInstanceIds()
	  {
		// when
		Batch batch = externalTaskService.updateRetries().processInstanceIds(processInstanceIds).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateRetriesByProcessInstanceIdArray()
	  public virtual void shouldUpdateRetriesByProcessInstanceIdArray()
	  {
		// given

		// when
		Batch batch = externalTaskService.updateRetries().processInstanceIds(processInstanceIds.ToArray()).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateRetriesByExternalTaskQuery()
	  public virtual void shouldUpdateRetriesByExternalTaskQuery()
	  {
		// given
		ExternalTaskQuery query = externalTaskService.createExternalTaskQuery();

		// when
		Batch batch = externalTaskService.updateRetries().externalTaskQuery(query).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		IList<ExternalTask> tasks = query.list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateRetriesByProcessInstanceQuery()
	  public virtual void shouldUpdateRetriesByProcessInstanceQuery()
	  {
		// given
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery();

		// when
		Batch batch = externalTaskService.updateRetries().processInstanceQuery(processInstanceQuery).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void shouldUpdateRetriesByHistoricProcessInstanceQuery()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void shouldUpdateRetriesByHistoricProcessInstanceQuery()
	  {
		// given
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		Batch batch = externalTaskService.updateRetries().historicProcessInstanceQuery(query).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(5, (int) task.Retries);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void shouldUpdateRetriesByAllParameters()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void shouldUpdateRetriesByAllParameters()
	  {
		// given
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().processInstanceId(processInstanceIds[0]).singleResult();

		ExternalTaskQuery externalTaskQuery = externalTaskService.createExternalTaskQuery().processInstanceId(processInstanceIds[1]);

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstanceIds[2]);


		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceIds[3]);

		// when
		Batch batch = externalTaskService.updateRetries().externalTaskIds(externalTask.Id).externalTaskQuery(externalTaskQuery).processInstanceQuery(processInstanceQuery).historicProcessInstanceQuery(historicProcessInstanceQuery).processInstanceIds(processInstanceIds[4]).setAsync(5);
		executeSeedAndBatchJobs(batch);

		// then
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().list();
		assertEquals(6, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertEquals(Convert.ToInt32(5), task.Retries);
		}
	  }

	  public virtual void executeSeedAndBatchJobs(Batch batch)
	  {
		Job job = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();
		// seed job
		managementService.executeJob(job.Id);

		foreach (Job pending in managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		{
		  managementService.executeJob(pending.Id);
		}
	  }

	  protected internal virtual void assertHistoricBatchExists()
	  {
		if (testHelper.HistoryLevelFull)
		{
		  assertEquals(1, historyService.createHistoricBatchQuery().count());
		}
	  }

	  protected internal virtual void createAndExecuteSeedJobs(string seedJobDefinitionId, int expectedSeedJobsCount)
	  {
		for (int i = 0; i <= expectedSeedJobsCount; i++)
		{
		  Job seedJob = managementService.createJobQuery().jobDefinitionId(seedJobDefinitionId).singleResult();
		  if (i != expectedSeedJobsCount)
		  {
			assertNotNull(seedJob);
			managementService.executeJob(seedJob.Id);
		  }
		  else
		  {
			//the last seed job should not trigger another seed job
			assertNull(seedJob);
		  }
		}
	  }

	  /// <summary>
	  /// Execute all batch jobs of batch once and collect exceptions during job execution.
	  /// </summary>
	  /// <param name="batch"> the batch for which the batch jobs should be executed </param>
	  /// <returns> the catched exceptions of the batch job executions, is empty if non where thrown </returns>
	  protected internal virtual IList<Exception> executeBatchJobs(Batch batch)
	  {
		string batchJobDefinitionId = batch.BatchJobDefinitionId;
		IList<Job> batchJobs = managementService.createJobQuery().jobDefinitionId(batchJobDefinitionId).list();
		assertFalse(batchJobs.Count == 0);

		IList<Exception> catchedExceptions = new List<Exception>();

		foreach (Job batchJob in batchJobs)
		{
		  try
		  {
			managementService.executeJob(batchJob.Id);
		  }
		  catch (Exception e)
		  {
			catchedExceptions.Add(e);
		  }
		}

		return catchedExceptions;
	  }

	  protected internal virtual void startTestProcesses()
	  {
		RuntimeService runtimeService = engineRule.RuntimeService;
		for (int i = 4; i < 1000; i++)
		{
		  processInstanceIds.Add(runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "").Id);
		}

	  }

	  protected internal virtual IList<string> startProcessInstance(string key, int instances)
	  {
		IList<string> ids = new List<string>();
		for (int i = 0; i < instances; i++)
		{
		  ids.Add(runtimeService.startProcessInstanceByKey(key, i.ToString()).Id);
		}
		((IList<string>)processInstanceIds).AddRange(ids);
		return ids;
	  }

	}

}