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
namespace org.camunda.bpm.engine.impl.jobexecutor
{

	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JobExecutorLogger : ProcessEngineLogger
	{

	  public virtual void debugAcquiredJobNotFound(string jobId)
	  {
		logDebug("001", "Acquired job with id '{}' not found.", jobId);
	  }

	  public virtual void exceptionWhileExecutingJob(JobEntity job, Exception exception)
	  {
		logWarn("002", "Exception while executing job {}: ", job, exception);
	  }

	  public virtual void debugFallbackToDefaultRetryStrategy()
	  {
		logDebug("003", "Falling back to default retry strategy");
	  }

	  public virtual void debugDecrementingRetriesForJob(string id)
	  {
		logDebug("004", "Decrementing retries of job {}", id);
	  }

	  public virtual void debugInitiallyAppyingRetryCycleForJob(string id, int times)
	  {
		logDebug("005", "Applying job retry time cycle for the first time for job {}, retires {}", id, times);
	  }

	  public virtual void exceptionWhileExecutingJob(string nextJobId, Exception t)
	  {
		logWarn("006", "Exception while executing job {}: ", nextJobId, t);
	  }

	  public virtual void couldNotDeterminePriority(ExecutionEntity execution, object value, ProcessEngineException e)
	  {
		logWarn("007", "Could not determine priority for job created in context of execution {}. Using default priority {}", execution, value, e);
	  }

	  public virtual void debugAddingNewExclusiveJobToJobExecutorCOntext(string jobId)
	  {
		logDebug("008", "Adding new exclusive job to job executor context. Job Id='{}'", jobId);
	  }

	  public virtual void timeoutDuringShutdown()
	  {
		logWarn("009", "Timeout during shutdown of job executor. The current running jobs could not end within 60 seconds after shutdown operation");
	  }

	  public virtual void interruptedWhileShuttingDownjobExecutor(InterruptedException e)
	  {
		logWarn("010", "Interrupted while shutting down the job executor", e);
	  }

	  public virtual void debugJobAcquisitionThreadSleeping(long millis)
	  {
		logDebug("011", "Job acquisition thread sleeping for {} millis", millis);
	  }

	  public virtual void jobExecutorThreadWokeUp()
	  {
		logDebug("012", "Job acquisition thread woke up");
	  }

	  public virtual void jobExecutionWaitInterrupted()
	  {
		logDebug("013", "Job Execution wait interrupted");
	  }

	  public virtual void startingUpJobExecutor(string name)
	  {
		logInfo("014", "Starting up the JobExecutor[{}].", name);
	  }

	  public virtual void shuttingDownTheJobExecutor(string name)
	  {
		logInfo("015", "Shutting down the JobExecutor[{}]", name);
	  }

	  public virtual void ignoringSuspendedJob(ProcessDefinition processDefinition)
	  {
		logDebug("016", "Ignoring job of suspended {}", processDefinition);
	  }

	  public virtual void debugNotifyingJobExecutor(string @string)
	  {
		logDebug("017", "Notifying Job Executor of new job {}", @string);
	  }

	  public virtual void startingToAcquireJobs(string name)
	  {
		logInfo("018", "{} starting to acquire jobs", name);
	  }

	  public virtual void exceptionDuringJobAcquisition(Exception e)
	  {
		logError("019", "Exception during job acquisition {}", e.Message, e);
	  }

	  public virtual void stoppedJobAcquisition(string name)
	  {
		logInfo("020", "{} stopped job acquisition", name);
	  }

	  public virtual void exceptionWhileUnlockingJob(string jobId, Exception t)
	  {
		logWarn("021", "Exception while unaquiring job {}: ", jobId, t);
	  }

	  public virtual void acquiredJobs(string processEngine, AcquiredJobs acquiredJobs)
	  {
		logDebug("022", "Acquired {} jobs for process engine '{}': {}", acquiredJobs.size(), processEngine, acquiredJobs.JobIdBatches);
	  }

	  public virtual void executeJobs(string processEngine, ICollection<string> jobs)
	  {
		logDebug("023", "Execute jobs for process engine '{}': {}", processEngine, jobs);
	  }

	  public virtual void debugFailedJobNotFound(string jobId)
	  {
		logDebug("024", "Failed job with id '{}' not found.", jobId);
	  }

	  public virtual ProcessEngineException wrapJobExecutionFailure(JobFailureCollector jobFailureCollector, Exception cause)
	  {
		JobEntity job = jobFailureCollector.Job;
		if (job != null)
		{
		  return new ProcessEngineException(exceptionMessage("025", "Exception while executing job {}: ", jobFailureCollector.Job), cause);
		}
		else
		{
		  return new ProcessEngineException(exceptionMessage("025", "Exception while executing job {}: ", jobFailureCollector.JobId), cause);
		}
	  }

	  public virtual ProcessEngineException jobNotFoundException(string jobId)
	  {
		return new ProcessEngineException(exceptionMessage("026", "No job found with id '{}'", jobId));
	  }

	  public virtual void exceptionWhileParsingExpression(string jobId, string exceptionMessage)
	  {
		logWarn("027", "Falling back to default retry strategy. Exception while executing job {}: {}", jobId, exceptionMessage);
	  }

	  public virtual void warnHistoryCleanupBatchWindowNotFound()
	  {
		logWarn("028", "Batch window for history cleanup was not calculated. History cleanup job(s) will be suspended.");
	  }

	}

}