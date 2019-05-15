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

	/// <summary>
	/// <para>Determines the number of jobs to acquire and the time to wait between acquisition cycles
	/// by an exponential backoff strategy.
	/// 
	/// </para>
	/// <para>Manages two kinds of backoff times:
	///   <ul>
	///     <li>idle time: Wait for a certain amount of time when no jobs are available
	///     <li>backoff time: Wait for a certain amount of time when jobs are available
	///       but could not successfully be acquired
	///   </ul>
	/// Both times are calculated by applying an exponential backoff. This means, when the respective conditions
	/// repeatedly hold, the time increases exponentially from one acquisition cycle to the next.
	/// 
	/// </para>
	/// <para>This implementation manages idle and backoff time in terms of levels. The initial backoff level is 0,
	/// meaning that no backoff is applied. In case the condition for increasing backoff applies, the backoff
	/// level is incremented. The actual time to wait is then computed as follows
	/// 
	/// <pre>timeToWait = baseBackoffTime * (backoffFactor ^ (backoffLevel - 1))</pre>
	/// 
	/// </para>
	/// <para>Accordingly, the maximum possible backoff level is
	/// 
	/// <pre>maximumLevel = floor( log( backoffFactor, maximumBackoffTime / baseBackoffTime) ) + 1</pre>
	/// (where log(a, b) is the logarithm of b to the base of a)
	/// 
	/// @author Thorben Lindhauer
	/// </para>
	/// </summary>
	public class BackoffJobAcquisitionStrategy : JobAcquisitionStrategy
	{

	  public static long DEFAULT_EXECUTION_SATURATION_WAIT_TIME = 100;

	  /*
	   * all wait times are in milliseconds
	   */

	  /*
	   * managing the idle level
	   */
	  protected internal long baseIdleWaitTime;
	  protected internal float idleIncreaseFactor;
	  protected internal int idleLevel;
	  protected internal int maxIdleLevel;
	  protected internal long maxIdleWaitTime;

	  /*
	   * managing the backoff level
	   */
	  protected internal long baseBackoffWaitTime;
	  protected internal float backoffIncreaseFactor;
	  protected internal int backoffLevel;
	  protected internal int maxBackoffLevel;
	  protected internal long maxBackoffWaitTime;
	  protected internal bool applyJitter = false;

	  /*
	   * Keeping a history of recent acquisitions without locking failure
	   * for backoff level decrease
	   */
	  protected internal int numAcquisitionsWithoutLockingFailure = 0;
	  protected internal int backoffDecreaseThreshold;

	  protected internal int baseNumJobsToAcquire;

	  protected internal IDictionary<string, int> jobsToAcquire = new Dictionary<string, int>();

	  /*
	   * Backing off when the execution resources (queue) are saturated
	   * in order to not busy wait for free resources
	   */
	  protected internal bool executionSaturated = false;
	  protected internal long executionSaturationWaitTime = DEFAULT_EXECUTION_SATURATION_WAIT_TIME;

	  public BackoffJobAcquisitionStrategy(long baseIdleWaitTime, float idleIncreaseFactor, long maxIdleTime, long baseBackoffWaitTime, float backoffIncreaseFactor, long maxBackoffTime, int backoffDecreaseThreshold, int baseNumJobsToAcquire)
	  {

		this.baseIdleWaitTime = baseIdleWaitTime;
		this.idleIncreaseFactor = idleIncreaseFactor;
		this.idleLevel = 0;
		this.maxIdleWaitTime = maxIdleTime;

		this.baseBackoffWaitTime = baseBackoffWaitTime;
		this.backoffIncreaseFactor = backoffIncreaseFactor;
		this.backoffLevel = 0;
		this.maxBackoffWaitTime = maxBackoffTime;
		this.backoffDecreaseThreshold = backoffDecreaseThreshold;

		this.baseNumJobsToAcquire = baseNumJobsToAcquire;

		initializeMaxLevels();
	  }

	  public BackoffJobAcquisitionStrategy(JobExecutor jobExecutor) : this(jobExecutor.WaitTimeInMillis, jobExecutor.WaitIncreaseFactor, jobExecutor.MaxWait, jobExecutor.BackoffTimeInMillis, jobExecutor.WaitIncreaseFactor, jobExecutor.MaxBackoff, jobExecutor.BackoffDecreaseThreshold, jobExecutor.MaxJobsPerAcquisition)
	  {
	  }

	  protected internal virtual void initializeMaxLevels()
	  {
		if (baseIdleWaitTime > 0 && maxIdleWaitTime > 0 && idleIncreaseFactor > 0 && maxIdleWaitTime >= baseIdleWaitTime)
		{
		  // the maximum level that produces an idle time <= maxIdleTime:
		  // see class docs for an explanation
		  maxIdleLevel = (int) log(idleIncreaseFactor, maxIdleWaitTime / baseIdleWaitTime) + 1;

		  // + 1 to get the minimum level that produces an idle time > maxIdleTime
		  maxIdleLevel += 1;
		}
		else
		{
		  maxIdleLevel = 0;
		}

		if (baseBackoffWaitTime > 0 && maxBackoffWaitTime > 0 && backoffIncreaseFactor > 0 && maxBackoffWaitTime >= baseBackoffWaitTime)
		{
		  // the maximum level that produces a backoff time < maxBackoffTime:
		  // see class docs for an explanation
		  maxBackoffLevel = (int) log(backoffIncreaseFactor, maxBackoffWaitTime / baseBackoffWaitTime) + 1;

		  // + 1 to get the minimum level that produces a backoff time > maxBackoffTime
		  maxBackoffLevel += 1;
		}
		else
		{
		  maxBackoffLevel = 0;
		}
	  }

	  protected internal virtual double log(double @base, double value)
	  {
		return Math.Log10(value) / Math.Log10(@base);
	  }

	  public virtual void reconfigure(JobAcquisitionContext context)
	  {
		reconfigureIdleLevel(context);
		reconfigureBackoffLevel(context);
		reconfigureNumberOfJobsToAcquire(context);
		executionSaturated = allSubmittedJobsRejected(context);
	  }

	  /// <returns> true, if all acquired jobs (spanning all engines) were rejected for execution </returns>
	  protected internal virtual bool allSubmittedJobsRejected(JobAcquisitionContext context)
	  {
		foreach (KeyValuePair<string, AcquiredJobs> acquiredJobsForEngine in context.AcquiredJobsByEngine.SetOfKeyValuePairs())
		{
		  string engineName = acquiredJobsForEngine.Key;

		  IList<IList<string>> acquiredJobBatches = acquiredJobsForEngine.Value.JobIdBatches;
		  IList<IList<string>> resubmittedJobBatches = context.AdditionalJobsByEngine[engineName];
		  IList<IList<string>> rejectedJobBatches = context.RejectedJobsByEngine[engineName];

		  int numJobsSubmittedForExecution = acquiredJobBatches.Count;
		  if (resubmittedJobBatches != null)
		  {
			numJobsSubmittedForExecution += resubmittedJobBatches.Count;
		  }

		  int numJobsRejected = 0;
		  if (rejectedJobBatches != null)
		  {
			numJobsRejected += rejectedJobBatches.Count;
		  }

		  // if not all jobs scheduled for execution have been rejected
		  if (numJobsRejected == 0 || numJobsSubmittedForExecution > numJobsRejected)
		  {
			return false;
		  }
		}

		return true;
	  }

	  protected internal virtual void reconfigureIdleLevel(JobAcquisitionContext context)
	  {
		if (context.JobAdded)
		{
		  idleLevel = 0;
		}
		else
		{
		  if (context.areAllEnginesIdle() || context.AcquisitionException != null)
		  {
			if (idleLevel < maxIdleLevel)
			{
			  idleLevel++;
			}
		  }
		  else
		  {
			idleLevel = 0;
		  }
		}
	  }

	  protected internal virtual void reconfigureBackoffLevel(JobAcquisitionContext context)
	  {
		// if for any engine, jobs could not be locked due to optimistic locking, back off

		if (context.hasJobAcquisitionLockFailureOccurred())
		{
		  numAcquisitionsWithoutLockingFailure = 0;
		  applyJitter = true;
		  if (backoffLevel < maxBackoffLevel)
		  {
			backoffLevel++;
		  }
		}
		else
		{
		  applyJitter = false;
		  numAcquisitionsWithoutLockingFailure++;
		  if (numAcquisitionsWithoutLockingFailure >= backoffDecreaseThreshold && backoffLevel > 0)
		  {
			backoffLevel--;
			numAcquisitionsWithoutLockingFailure = 0;
		  }
		}
	  }

	  protected internal virtual void reconfigureNumberOfJobsToAcquire(JobAcquisitionContext context)
	  {
		// calculate the number of jobs to acquire next time
		jobsToAcquire.Clear();
		foreach (KeyValuePair<string, AcquiredJobs> acquiredJobsEntry in context.AcquiredJobsByEngine.SetOfKeyValuePairs())
		{
		  string engineName = acquiredJobsEntry.Key;

		  int numJobsToAcquire = (int)(baseNumJobsToAcquire * Math.Pow(backoffIncreaseFactor, backoffLevel));
		  IList<IList<string>> rejectedJobBatchesForEngine = context.RejectedJobsByEngine[engineName];
		  if (rejectedJobBatchesForEngine != null)
		  {
			numJobsToAcquire -= rejectedJobBatchesForEngine.Count;
		  }
		  numJobsToAcquire = Math.Max(0, numJobsToAcquire);

		  jobsToAcquire[engineName] = numJobsToAcquire;
		}
	  }

	  public virtual long WaitTime
	  {
		  get
		  {
			if (idleLevel > 0)
			{
			  return calculateIdleTime();
			}
			else if (backoffLevel > 0)
			{
			  return calculateBackoffTime();
			}
			else if (executionSaturated)
			{
			  return executionSaturationWaitTime;
			}
			else
			{
			  return 0;
			}
		  }
	  }

	  protected internal virtual long calculateIdleTime()
	  {
		if (idleLevel <= 0)
		{
		  return 0;
		}
		else if (idleLevel >= maxIdleLevel)
		{
		  return maxIdleWaitTime;
		}
		else
		{
		  return (long)(baseIdleWaitTime * Math.Pow(idleIncreaseFactor, idleLevel - 1));
		}
	  }

	  protected internal virtual long calculateBackoffTime()
	  {
		long backoffTime = 0;

		if (backoffLevel <= 0)
		{
		  backoffTime = 0;
		}
		else if (backoffLevel >= maxBackoffLevel)
		{
		  backoffTime = maxBackoffWaitTime;
		}
		else
		{
		  backoffTime = (long)(baseBackoffWaitTime * Math.Pow(backoffIncreaseFactor, backoffLevel - 1));
		}

		if (applyJitter)
		{
		  // add a bounded random jitter to avoid multiple job acquisitions getting exactly the same
		  // polling interval
		  backoffTime += GlobalRandom.NextDouble * (backoffTime / 2);
		}

		return backoffTime;
	  }

	  public virtual int getNumJobsToAcquire(string processEngine)
	  {
		int? numJobsToAcquire = jobsToAcquire[processEngine];
		if (numJobsToAcquire != null)
		{
		  return numJobsToAcquire.Value;
		}
		else
		{
		  return baseNumJobsToAcquire;
		}
	  }
	}

}