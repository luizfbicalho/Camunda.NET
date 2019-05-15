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
namespace org.camunda.bpm.engine.impl.cmd
{

	using DefaultFailedJobParseListener = org.camunda.bpm.engine.impl.bpmn.parser.DefaultFailedJobParseListener;
	using FailedJobRetryConfiguration = org.camunda.bpm.engine.impl.bpmn.parser.FailedJobRetryConfiguration;
	using DurationHelper = org.camunda.bpm.engine.impl.calendar.DurationHelper;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Expression = org.camunda.bpm.engine.impl.el.Expression;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using JobExecutorLogger = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorLogger;
	using TimerCatchIntermediateEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;

	/// <summary>
	/// @author Roman Smirnov
	/// </summary>
	public class DefaultJobRetryCmd : JobRetryCmd
	{

	  public static readonly IList<string> SUPPORTED_TYPES = Arrays.asList(TimerExecuteNestedActivityJobHandler.TYPE, TimerCatchIntermediateEventJobHandler.TYPE, TimerStartEventJobHandler.TYPE, TimerStartEventSubprocessJobHandler.TYPE, AsyncContinuationJobHandler.TYPE);
	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  public DefaultJobRetryCmd(string jobId, Exception exception) : base(jobId, exception)
	  {
	  }

	  public override object execute(CommandContext commandContext)
	  {
		JobEntity job = Job;

		ActivityImpl activity = getCurrentActivity(commandContext, job);

		if (activity == null)
		{
		  LOG.debugFallbackToDefaultRetryStrategy();
		  executeStandardStrategy(commandContext);

		}
		else
		{
		  try
		  {
			executeCustomStrategy(commandContext, job, activity);

		  }
		  catch (Exception)
		  {
			LOG.debugFallbackToDefaultRetryStrategy();
			executeStandardStrategy(commandContext);
		  }
		}

		return null;
	  }

	  protected internal virtual void executeStandardStrategy(CommandContext commandContext)
	  {
		JobEntity job = Job;
		if (job != null)
		{
		  job.unlock();
		  logException(job);
		  decrementRetries(job);
		  notifyAcquisition(commandContext);
		}
		else
		{
		  LOG.debugFailedJobNotFound(jobId);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void executeCustomStrategy(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, org.camunda.bpm.engine.impl.persistence.entity.JobEntity job, org.camunda.bpm.engine.impl.pvm.process.ActivityImpl activity) throws Exception
	  protected internal virtual void executeCustomStrategy(CommandContext commandContext, JobEntity job, ActivityImpl activity)
	  {
		FailedJobRetryConfiguration retryConfiguration = getFailedJobRetryConfiguration(job, activity);

		if (retryConfiguration == null)
		{
		  executeStandardStrategy(commandContext);

		}
		else
		{

		  if (isFirstJobExecution(job))
		  {
			// then change default retries to the ones configured
			initializeRetries(job, retryConfiguration.Retries);

		  }
		  else
		  {
			LOG.debugDecrementingRetriesForJob(job.Id);
		  }

		  IList<string> intervals = retryConfiguration.RetryIntervals;
		  int intervalsCount = intervals.Count;
		  int indexOfInterval = Math.Max(0, Math.Min(intervalsCount - 1, intervalsCount - (job.Retries - 1)));
		  DurationHelper durationHelper = getDurationHelper(intervals[indexOfInterval]);
		  job.LockExpirationTime = durationHelper.DateAfter;

		  logException(job);
		  decrementRetries(job);
		  notifyAcquisition(commandContext);
		}
	  }

	  protected internal virtual ActivityImpl getCurrentActivity(CommandContext commandContext, JobEntity job)
	  {
		string type = job.JobHandlerType;
		ActivityImpl activity = null;

		if (SUPPORTED_TYPES.Contains(type))
		{
		  DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
		  ProcessDefinitionEntity processDefinitionEntity = deploymentCache.findDeployedProcessDefinitionById(job.ProcessDefinitionId);
		  activity = processDefinitionEntity.findActivity(job.ActivityId);

		}
		else
		{
		  // noop, because activity type is not supported
		}

		return activity;
	  }

	  protected internal virtual ExecutionEntity fetchExecutionEntity(string executionId)
	  {
		return Context.CommandContext.ExecutionManager.findExecutionById(executionId);
	  }

	  protected internal virtual FailedJobRetryConfiguration getFailedJobRetryConfiguration(JobEntity job, ActivityImpl activity)
	  {
		FailedJobRetryConfiguration retryConfiguration = activity.Properties.get(DefaultFailedJobParseListener.FAILED_JOB_CONFIGURATION);

		while (retryConfiguration != null && retryConfiguration.Expression != null)
		{
		  string retryIntervals = getFailedJobRetryTimeCycle(job, retryConfiguration.Expression);
		  retryConfiguration = ParseUtil.parseRetryIntervals(retryIntervals);
		}

		return retryConfiguration;
	  }

	  protected internal virtual string getFailedJobRetryTimeCycle(JobEntity job, Expression expression)
	  {

		string executionId = job.ExecutionId;
		ExecutionEntity execution = null;

		if (!string.ReferenceEquals(executionId, null))
		{
		  execution = fetchExecutionEntity(executionId);
		}

		object value = null;

		if (expression == null)
		{
		  return null;
		}

		try
		{
		   value = expression.getValue(execution, execution);
		}
		catch (Exception e)
		{
		  LOG.exceptionWhileParsingExpression(jobId, e.InnerException.Message);
		}

		if (value is string)
		{
		  return (string) value;
		}
		else
		{
		  // default behavior
		  return null;
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.calendar.DurationHelper getDurationHelper(String failedJobRetryTimeCycle) throws Exception
	  protected internal virtual DurationHelper getDurationHelper(string failedJobRetryTimeCycle)
	  {
		return new DurationHelper(failedJobRetryTimeCycle);
	  }

	  protected internal virtual bool isFirstJobExecution(JobEntity job)
	  {
		// check if this is jobs' first execution (recognize
		// this because no exception is set. Only the first
		// execution can be without exception - because if
		// no exception occurred the job would have been completed)
		// see https://app.camunda.com/jira/browse/CAM-1039
		return string.ReferenceEquals(job.ExceptionByteArrayId, null) && string.ReferenceEquals(job.ExceptionMessage, null);
	  }

	  protected internal virtual void initializeRetries(JobEntity job, int retries)
	  {
		LOG.debugInitiallyAppyingRetryCycleForJob(job.Id, retries);
		job.Retries = retries;
	  }

	}

}