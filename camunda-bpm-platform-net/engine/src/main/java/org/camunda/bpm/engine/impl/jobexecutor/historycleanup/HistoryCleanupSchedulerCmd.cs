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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class HistoryCleanupSchedulerCmd : Command<Void>
	{

	  protected internal static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal bool isRescheduleNow;
	  protected internal HistoryCleanupJobHandlerConfiguration configuration;
	  protected internal string jobId;
	  protected internal IDictionary<string, long> reports;

	  public HistoryCleanupSchedulerCmd(bool isRescheduleNow, IDictionary<string, long> reports, HistoryCleanupJobHandlerConfiguration configuration, string jobId)
	  {
		this.isRescheduleNow = isRescheduleNow;
		this.configuration = configuration;
		this.jobId = jobId;
		this.reports = reports;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (MetricsEnabled)
		{
		  reportMetrics(commandContext);
		}

		JobEntity jobEntity = commandContext.JobManager.findJobById(jobId);

		bool rescheduled = false;

		if (isRescheduleNow)
		{
		  commandContext.JobManager.reschedule(jobEntity, ClockUtil.CurrentTime);
		  rescheduled = true;
		  cancelCountEmptyRuns(configuration, jobEntity);
		}
		else
		{
		  if (HistoryCleanupHelper.isWithinBatchWindow(ClockUtil.CurrentTime, commandContext.ProcessEngineConfiguration))
		  {
			DateTime nextRunDate = configuration.getNextRunWithDelay(ClockUtil.CurrentTime);
			if (HistoryCleanupHelper.isWithinBatchWindow(nextRunDate, commandContext.ProcessEngineConfiguration))
			{
			  commandContext.JobManager.reschedule(jobEntity, nextRunDate);
			  rescheduled = true;
			  incrementCountEmptyRuns(configuration, jobEntity);
			}
		  }
		}

		if (!rescheduled)
		{
		  if (HistoryCleanupHelper.isBatchWindowConfigured(commandContext))
		  {
			rescheduleRegularCall(commandContext, jobEntity);
		  }
		  else
		  {
			suspendJob(jobEntity);
		  }
		  cancelCountEmptyRuns(configuration, jobEntity);
		}

		return null;
	  }

	  protected internal virtual void rescheduleRegularCall(CommandContext commandContext, JobEntity jobEntity)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BatchWindow nextBatchWindow = commandContext.getProcessEngineConfiguration().getBatchWindowManager().getNextBatchWindow(org.camunda.bpm.engine.impl.util.ClockUtil.getCurrentTime(), commandContext.getProcessEngineConfiguration());
		BatchWindow nextBatchWindow = commandContext.ProcessEngineConfiguration.BatchWindowManager.getNextBatchWindow(ClockUtil.CurrentTime, commandContext.ProcessEngineConfiguration);
		if (nextBatchWindow != null)
		{
		  commandContext.JobManager.reschedule(jobEntity, nextBatchWindow.Start);
		}
		else
		{
		  LOG.warnHistoryCleanupBatchWindowNotFound();
		  suspendJob(jobEntity);
		}
	  }

	  protected internal virtual void suspendJob(JobEntity jobEntity)
	  {
		jobEntity.SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED.StateCode;
	  }

	  protected internal virtual void incrementCountEmptyRuns(HistoryCleanupJobHandlerConfiguration configuration, JobEntity jobEntity)
	  {
		configuration.CountEmptyRuns = configuration.CountEmptyRuns + 1;
		jobEntity.JobHandlerConfiguration = configuration;
	  }

	  protected internal virtual void cancelCountEmptyRuns(HistoryCleanupJobHandlerConfiguration configuration, JobEntity jobEntity)
	  {
		configuration.CountEmptyRuns = 0;
		jobEntity.JobHandlerConfiguration = configuration;
	  }

	  protected internal virtual void reportMetrics(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl engineConfiguration = commandContext.ProcessEngineConfiguration;
		if (engineConfiguration.HistoryCleanupMetricsEnabled)
		{
		  foreach (KeyValuePair<string, long> report in reports.SetOfKeyValuePairs())
		  {
			engineConfiguration.DbMetricsReporter.reportValueAtOnce(report.Key, report.Value);
		  }
		}
	  }

	  protected internal virtual bool MetricsEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryCleanupMetricsEnabled;
		  }
	  }
	}

}