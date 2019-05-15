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

	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// <para><seealso cref="AcquireJobsRunnable"/> able to serve multiple process engines.</para>
	/// 
	/// <para>
	///   Continuously acquires jobs for all registered process engines until interruption.
	///   For every such <i>acquisition cycle</i>, jobs are acquired and submitted for execution.
	/// </para>
	/// 
	/// <para>
	///   For one cycle, all acquisition-related events (acquired jobs by engine, rejected jobs by engine,
	///   exceptions during acquisition, etc.) are collected in an instance of <seealso cref="JobAcquisitionContext"/>.
	///   The context is then handed to a <seealso cref="JobAcquisitionStrategy"/> that
	///   determines the there is before the next acquisition cycles begins and how many jobs
	///   are to be acquired next.
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class SequentialJobAcquisitionRunnable : AcquireJobsRunnable
	{

	  protected internal readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal JobAcquisitionContext acquisitionContext;

	  public SequentialJobAcquisitionRunnable(JobExecutor jobExecutor) : base(jobExecutor)
	  {
		acquisitionContext = initializeAcquisitionContext();
	  }

	  public virtual void run()
	  {
		  lock (this)
		  {
			LOG.startingToAcquireJobs(jobExecutor.Name);
        
			JobAcquisitionStrategy acquisitionStrategy = initializeAcquisitionStrategy();
        
			while (!isInterrupted)
			{
			  acquisitionContext.reset();
			  acquisitionContext.AcquisitionTime = DateTimeHelper.CurrentUnixTimeMillis();
        
        
			  IEnumerator<ProcessEngineImpl> engineIterator = jobExecutor.engineIterator();
        
			  try
			  {
				while (engineIterator.MoveNext())
				{
				  ProcessEngineImpl currentProcessEngine = engineIterator.Current;
				  if (!jobExecutor.hasRegisteredEngine(currentProcessEngine))
				  {
					// if engine has been unregistered meanwhile
					continue;
				  }
        
				  AcquiredJobs acquiredJobs = acquireJobs(acquisitionContext, acquisitionStrategy, currentProcessEngine);
				  executeJobs(acquisitionContext, currentProcessEngine, acquiredJobs);
				}
			  }
			  catch (Exception e)
			  {
				LOG.exceptionDuringJobAcquisition(e);
        
				acquisitionContext.AcquisitionException = e;
			  }
        
			  acquisitionContext.JobAdded = isJobAdded;
			  configureNextAcquisitionCycle(acquisitionContext, acquisitionStrategy);
			  //The clear had to be done after the configuration, since a hint can be
			  //appear in the suspend and the flag shouldn't be cleaned in this case.
			  //The loop will restart after suspend with the isJobAdded flag and
			  //reconfigure with this flag
			  clearJobAddedNotification();
        
			  long waitTime = acquisitionStrategy.WaitTime;
			  // wait the requested wait time minus the time that acquisition itself took
			  // this makes the intervals of job acquisition more constant and therefore predictable
			  waitTime = Math.Max(0, (acquisitionContext.AcquisitionTime + waitTime) - DateTimeHelper.CurrentUnixTimeMillis());
        
			  suspendAcquisition(waitTime);
			}
        
			LOG.stoppedJobAcquisition(jobExecutor.Name);
		  }
	  }

	  protected internal virtual JobAcquisitionContext initializeAcquisitionContext()
	  {
		return new JobAcquisitionContext();
	  }

	  /// <summary>
	  /// Reconfigure the acquisition strategy based on the current cycle's acquisition context.
	  /// A strategy implementation may update internal data structure to calculate a different wait time
	  /// before the next cycle of acquisition is performed.
	  /// </summary>
	  protected internal virtual void configureNextAcquisitionCycle(JobAcquisitionContext acquisitionContext, JobAcquisitionStrategy acquisitionStrategy)
	  {
		acquisitionStrategy.reconfigure(acquisitionContext);
	  }

	  protected internal virtual JobAcquisitionStrategy initializeAcquisitionStrategy()
	  {
		return new BackoffJobAcquisitionStrategy(jobExecutor);
	  }

	  public virtual JobAcquisitionContext AcquisitionContext
	  {
		  get
		  {
			return acquisitionContext;
    
		  }
	  }

	  protected internal virtual void executeJobs(JobAcquisitionContext context, ProcessEngineImpl currentProcessEngine, AcquiredJobs acquiredJobs)
	  {
		// submit those jobs that were acquired in previous cycles but could not be scheduled for execution
		IList<IList<string>> additionalJobs = context.AdditionalJobsByEngine[currentProcessEngine.Name];
		if (additionalJobs != null)
		{
		  foreach (IList<string> jobBatch in additionalJobs)
		  {
			LOG.executeJobs(currentProcessEngine.Name, jobBatch);

			jobExecutor.executeJobs(jobBatch, currentProcessEngine);
		  }
		}

		// submit those jobs that were acquired in the current cycle
		foreach (IList<string> jobIds in acquiredJobs.JobIdBatches)
		{
		  LOG.executeJobs(currentProcessEngine.Name, jobIds);

		  jobExecutor.executeJobs(jobIds, currentProcessEngine);
		}
	  }

	  protected internal virtual AcquiredJobs acquireJobs(JobAcquisitionContext context, JobAcquisitionStrategy acquisitionStrategy, ProcessEngineImpl currentProcessEngine)
	  {
		CommandExecutor commandExecutor = currentProcessEngine.ProcessEngineConfiguration.CommandExecutorTxRequired;

		int numJobsToAcquire = acquisitionStrategy.getNumJobsToAcquire(currentProcessEngine.Name);

		AcquiredJobs acquiredJobs = null;

		if (numJobsToAcquire > 0)
		{
		  jobExecutor.logAcquisitionAttempt(currentProcessEngine);
		  acquiredJobs = commandExecutor.execute(jobExecutor.getAcquireJobsCmd(numJobsToAcquire));
		}
		else
		{
		  acquiredJobs = new AcquiredJobs(numJobsToAcquire);
		}

		context.submitAcquiredJobs(currentProcessEngine.Name, acquiredJobs);

		jobExecutor.logAcquiredJobs(currentProcessEngine, acquiredJobs.size());
		jobExecutor.logAcquisitionFailureJobs(currentProcessEngine, acquiredJobs.NumberOfJobsFailedToLock);

		LOG.acquiredJobs(currentProcessEngine.Name, acquiredJobs);

		return acquiredJobs;
	  }

	}

}