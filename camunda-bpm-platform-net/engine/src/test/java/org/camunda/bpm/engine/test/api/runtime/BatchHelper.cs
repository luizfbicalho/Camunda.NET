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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using BatchMonitorJobHandler = org.camunda.bpm.engine.impl.batch.BatchMonitorJobHandler;
	using BatchSeedJobHandler = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;

	public abstract class BatchHelper
	{

	  protected internal ProcessEngineRule engineRule;
	  protected internal PluggableProcessEngineTestCase testCase;

	  public BatchHelper(ProcessEngineRule engineRule)
	  {
		this.engineRule = engineRule;
	  }

	  public BatchHelper(PluggableProcessEngineTestCase testCase)
	  {
		this.testCase = testCase;
	  }

	  public virtual Job getJobForDefinition(JobDefinition jobDefinition)
	  {
		if (jobDefinition != null)
		{
		  return ManagementService.createJobQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		}
		else
		{
		  return null;
		}
	  }

	  public virtual IList<Job> getJobsForDefinition(JobDefinition jobDefinition)
	  {
		return ManagementService.createJobQuery().jobDefinitionId(jobDefinition.Id).list();
	  }

	  public virtual void executeJob(Job job)
	  {
		assertNotNull("Job to execute does not exist", job);
		try
		{
		  ManagementService.executeJob(job.Id);
		}
		catch (BadUserRequestException e)
		{
		  throw e;
		}
		catch (Exception)
		{
		  // ignore
		}
	  }

	  public virtual JobDefinition getSeedJobDefinition(Batch batch)
	  {
		return ManagementService.createJobDefinitionQuery().jobDefinitionId(batch.SeedJobDefinitionId).jobType(BatchSeedJobHandler.TYPE).singleResult();
	  }

	  public virtual Job getSeedJob(Batch batch)
	  {
		return getJobForDefinition(getSeedJobDefinition(batch));
	  }

	  public virtual void executeSeedJob(Batch batch)
	  {
		executeJob(getSeedJob(batch));
	  }

	  public virtual JobDefinition getMonitorJobDefinition(Batch batch)
	  {
		return ManagementService.createJobDefinitionQuery().jobDefinitionId(batch.MonitorJobDefinitionId).jobType(BatchMonitorJobHandler.TYPE).singleResult();
	  }

	  public virtual Job getMonitorJob(Batch batch)
	  {
		return getJobForDefinition(getMonitorJobDefinition(batch));
	  }

	  public virtual void executeMonitorJob(Batch batch)
	  {
		executeJob(getMonitorJob(batch));
	  }

	  public virtual void completeMonitorJobs(Batch batch)
	  {
		while (getMonitorJob(batch) != null)
		{
		  executeMonitorJob(batch);
		}
	  }

	  public virtual void completeSeedJobs(Batch batch)
	  {
		while (getSeedJob(batch) != null)
		{
		  executeSeedJob(batch);
		}
	  }

	  public abstract JobDefinition getExecutionJobDefinition(Batch batch);

	  public virtual IList<Job> getExecutionJobs(Batch batch)
	  {
		return getJobsForDefinition(getExecutionJobDefinition(batch));
	  }

	  public virtual void executeJobs(Batch batch)
	  {
		foreach (Job job in getExecutionJobs(batch))
		{
		  executeJob(job);
		}
	  }

	  public virtual void completeBatch(Batch batch)
	  {
		completeSeedJobs(batch);
		completeExecutionJobs(batch);
		completeMonitorJobs(batch);
	  }

	  public virtual void completeJobs(Batch batch, int count)
	  {
		IList<Job> jobs = getExecutionJobs(batch);
		assertTrue(jobs.Count >= count);
		for (int i = 0; i < count; i++)
		{
		  executeJob(jobs[i]);
		}
	  }

	  public virtual void failExecutionJobs(Batch batch, int count)
	  {
		setRetries(batch, count, 0);
	  }

	  public virtual void setRetries(Batch batch, int count, int retries)
	  {
		IList<Job> jobs = getExecutionJobs(batch);
		assertTrue(jobs.Count >= count);

		ManagementService managementService = ManagementService;
		for (int i = 0; i < count; i++)
		{
		  managementService.setJobRetries(jobs[i].Id, retries);
		}

	  }

	  public virtual void completeExecutionJobs(Batch batch)
	  {
		while (getExecutionJobs(batch).Count > 0)
		{
		  executeJobs(batch);
		}
	  }

	  public virtual HistoricBatch getHistoricBatch(Batch batch)
	  {
		return HistoryService.createHistoricBatchQuery().batchId(batch.Id).singleResult();
	  }
	  public virtual IList<HistoricJobLog> getHistoricSeedJobLog(Batch batch)
	  {
		return HistoryService.createHistoricJobLogQuery().jobDefinitionId(batch.SeedJobDefinitionId).orderPartiallyByOccurrence().asc().list();
	  }

	  public virtual IList<HistoricJobLog> getHistoricMonitorJobLog(Batch batch)
	  {
		return HistoryService.createHistoricJobLogQuery().jobDefinitionId(batch.MonitorJobDefinitionId).orderPartiallyByOccurrence().asc().list();
	  }

	  public virtual IList<HistoricJobLog> getHistoricMonitorJobLog(Batch batch, Job monitorJob)
	  {
		return HistoryService.createHistoricJobLogQuery().jobDefinitionId(batch.MonitorJobDefinitionId).jobId(monitorJob.Id).orderPartiallyByOccurrence().asc().list();
	  }

	  public virtual IList<HistoricJobLog> getHistoricBatchJobLog(Batch batch)
	  {
		return HistoryService.createHistoricJobLogQuery().jobDefinitionId(batch.BatchJobDefinitionId).orderPartiallyByOccurrence().asc().list();
	  }

	  public virtual DateTime addSeconds(DateTime date, int seconds)
	  {
		return new DateTime(date.Ticks + seconds * 1000);
	  }

	  public virtual DateTime addSecondsToClock(int seconds)
	  {
		DateTime newDate = addSeconds(ClockUtil.CurrentTime, seconds);
		ClockUtil.CurrentTime = newDate;
		return newDate;
	  }

	  /// <summary>
	  /// Remove all batches and historic batches. Usually called in <seealso cref="org.junit.After"/> method.
	  /// </summary>
	  public virtual void removeAllRunningAndHistoricBatches()
	  {
		HistoryService historyService = HistoryService;
		ManagementService managementService = ManagementService;

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

	  protected internal virtual ManagementService ManagementService
	  {
		  get
		  {
			if (engineRule != null)
			{
			  return engineRule.ManagementService;
			}
			else
			{
			  return PluggableProcessEngineTestCase.ProcessEngine.ManagementService;
			}
		  }
	  }

	  protected internal virtual HistoryService HistoryService
	  {
		  get
		  {
			if (engineRule != null)
			{
			  return engineRule.HistoryService;
			}
			else
			{
			  return PluggableProcessEngineTestCase.ProcessEngine.HistoryService;
			}
		  }
	  }

	}

}