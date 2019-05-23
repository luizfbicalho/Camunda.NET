using System;
using System.Collections.Generic;
using System.Threading;

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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// <para>Interface to the component responsible for performing
	/// background work (<seealso cref="Job Jobs"/>).</para>
	/// 
	/// <para>The <seealso cref="JobExecutor"/> is capable of dispatching to multiple process engines,
	/// ie. multiple process engines can share a single Thread Pool for performing Background
	/// Work. </para>
	/// 
	/// <para>In clustered situations, you can have multiple Job Executors running against the
	/// same queue + pending job list.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public abstract class JobExecutor
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal string name = "JobExecutor[" + this.GetType().FullName + "]";
	  protected internal IList<ProcessEngineImpl> processEngines = new CopyOnWriteArrayList<ProcessEngineImpl>();
	  protected internal AcquireJobsCommandFactory acquireJobsCmdFactory;
	  protected internal AcquireJobsRunnable acquireJobsRunnable;
	  protected internal RejectedJobsHandler rejectedJobsHandler;
	  protected internal Thread jobAcquisitionThread;

	  protected internal bool isAutoActivate = false;
	  protected internal bool isActive = false;

	  protected internal int maxJobsPerAcquisition = 3;

	  // waiting when job acquisition is idle
	  protected internal int waitTimeInMillis = 5 * 1000;
	  protected internal float waitIncreaseFactor = 2;
	  protected internal long maxWait = 60 * 1000;

	  // backoff when job acquisition fails to lock all jobs
	  protected internal int backoffTimeInMillis = 0;
	  protected internal long maxBackoff = 0;

	  /// <summary>
	  /// The number of job acquisition cycles without locking failures
	  /// until the backoff level is reduced.
	  /// </summary>
	  protected internal int backoffDecreaseThreshold = 100;

	  protected internal string lockOwner = System.Guid.randomUUID().ToString();
	  protected internal int lockTimeInMillis = 5 * 60 * 1000;

	  public virtual void start()
	  {
		if (isActive)
		{
		  return;
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		LOG.startingUpJobExecutor(this.GetType().FullName);
		ensureInitialization();
		startExecutingJobs();
		isActive = true;
	  }

	  public virtual void shutdown()
	  {
		  lock (this)
		  {
			if (!isActive)
			{
			  return;
			}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LOG.shuttingDownTheJobExecutor(this.GetType().FullName);
			acquireJobsRunnable.stop();
			stopExecutingJobs();
			ensureCleanup();
			isActive = false;
		  }
	  }

	  protected internal virtual void ensureInitialization()
	  {
		acquireJobsCmdFactory = new DefaultAcquireJobsCommandFactory(this);
		acquireJobsRunnable = new SequentialJobAcquisitionRunnable(this);
	  }

	  protected internal virtual void ensureCleanup()
	  {
		acquireJobsCmdFactory = null;
		acquireJobsRunnable = null;
	  }

	  public virtual void jobWasAdded()
	  {
		if (isActive)
		{
		  acquireJobsRunnable.jobWasAdded();
		}
	  }

	  public virtual void registerProcessEngine(ProcessEngineImpl processEngine)
	  {
		  lock (this)
		  {
			processEngines.Add(processEngine);
        
			// when we register the first process engine, start the jobexecutor
			if (processEngines.Count == 1 && isAutoActivate)
			{
			  start();
			}
		  }
	  }

	  public virtual void unregisterProcessEngine(ProcessEngineImpl processEngine)
	  {
		  lock (this)
		  {
			processEngines.Remove(processEngine);
        
			// if we unregister the last process engine, auto-shutdown the jobexecutor
			if (processEngines.Count == 0 && isActive)
			{
			  shutdown();
			}
		  }
	  }

	  protected internal abstract void startExecutingJobs();
	  protected internal abstract void stopExecutingJobs();
	  public abstract void executeJobs(IList<string> jobIds, ProcessEngineImpl processEngine);

	  /// <summary>
	  /// Deprecated: use <seealso cref="executeJobs(System.Collections.IList, ProcessEngineImpl)"/> instead </summary>
	  /// <param name="jobIds"> </param>
	  [Obsolete]
	  public virtual void executeJobs(IList<string> jobIds)
	  {
		if (processEngines.Count > 0)
		{
		  executeJobs(jobIds, processEngines[0]);
		}
	  }

	  public virtual void logAcquisitionAttempt(ProcessEngineImpl engine)
	  {
		if (engine.ProcessEngineConfiguration.MetricsEnabled)
		{
		  engine.ProcessEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.JOB_ACQUISITION_ATTEMPT);
		}
	  }

	  public virtual void logAcquiredJobs(ProcessEngineImpl engine, int numJobs)
	  {
		if (engine != null && engine.ProcessEngineConfiguration.MetricsEnabled)
		{
		  engine.ProcessEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.JOB_ACQUIRED_SUCCESS, numJobs);
		}
	  }

	  public virtual void logAcquisitionFailureJobs(ProcessEngineImpl engine, int numJobs)
	  {
		if (engine != null && engine.ProcessEngineConfiguration.MetricsEnabled)
		{
		  engine.ProcessEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.JOB_ACQUIRED_FAILURE, numJobs);
		}
	  }

	  public virtual void logRejectedExecution(ProcessEngineImpl engine, int numJobs)
	  {
		if (engine != null && engine.ProcessEngineConfiguration.MetricsEnabled)
		{
		  engine.ProcessEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.JOB_EXECUTION_REJECTED, numJobs);
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual IList<ProcessEngineImpl> ProcessEngines
	  {
		  get
		  {
			return processEngines;
		  }
		  set
		  {
			this.processEngines = value;
		  }
	  }

	  /// <summary>
	  /// Must return an iterator of registered process engines
	  /// that is independent of concurrent modifications
	  /// to the underlying data structure of engines.
	  /// </summary>
	  public virtual IEnumerator<ProcessEngineImpl> engineIterator()
	  {
		// a CopyOnWriteArrayList's iterator is safe in the presence
		// of modifications
		return processEngines.GetEnumerator();
	  }

	  public virtual bool hasRegisteredEngine(ProcessEngineImpl engine)
	  {
		return processEngines.Contains(engine);
	  }

	  /// <summary>
	  /// Deprecated: use <seealso cref="getProcessEngines()"/> instead
	  /// </summary>
	  [Obsolete]
	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			if (processEngines.Count == 0)
			{
			  return null;
			}
			else
			{
			  return processEngines[0].ProcessEngineConfiguration.CommandExecutorTxRequired;
			}
		  }
		  set
		  {
    
		  }
	  }


	  public virtual int WaitTimeInMillis
	  {
		  get
		  {
			return waitTimeInMillis;
		  }
		  set
		  {
			this.waitTimeInMillis = value;
		  }
	  }


	  public virtual int BackoffTimeInMillis
	  {
		  get
		  {
			return backoffTimeInMillis;
		  }
		  set
		  {
			this.backoffTimeInMillis = value;
		  }
	  }


	  public virtual int LockTimeInMillis
	  {
		  get
		  {
			return lockTimeInMillis;
		  }
		  set
		  {
			this.lockTimeInMillis = value;
		  }
	  }


	  public virtual string LockOwner
	  {
		  get
		  {
			return lockOwner;
		  }
		  set
		  {
			this.lockOwner = value;
		  }
	  }


	  public virtual bool AutoActivate
	  {
		  get
		  {
			return isAutoActivate;
		  }
		  set
		  {
			this.isAutoActivate = value;
		  }
	  }



	  public virtual int MaxJobsPerAcquisition
	  {
		  get
		  {
			return maxJobsPerAcquisition;
		  }
		  set
		  {
			this.maxJobsPerAcquisition = value;
		  }
	  }


	  public virtual float WaitIncreaseFactor
	  {
		  get
		  {
			return waitIncreaseFactor;
		  }
		  set
		  {
			this.waitIncreaseFactor = value;
		  }
	  }


	  public virtual long MaxWait
	  {
		  get
		  {
			return maxWait;
		  }
		  set
		  {
			this.maxWait = value;
		  }
	  }


	  public virtual long MaxBackoff
	  {
		  get
		  {
			return maxBackoff;
		  }
		  set
		  {
			this.maxBackoff = value;
		  }
	  }


	  public virtual int BackoffDecreaseThreshold
	  {
		  get
		  {
			return backoffDecreaseThreshold;
		  }
		  set
		  {
			this.backoffDecreaseThreshold = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual Command<AcquiredJobs> getAcquireJobsCmd(int numJobs)
	  {
		return acquireJobsCmdFactory.getCommand(numJobs);
	  }

	  public virtual AcquireJobsCommandFactory AcquireJobsCmdFactory
	  {
		  get
		  {
			return acquireJobsCmdFactory;
		  }
		  set
		  {
			this.acquireJobsCmdFactory = value;
		  }
	  }


	  public virtual bool Active
	  {
		  get
		  {
			return isActive;
		  }
	  }

	  public virtual RejectedJobsHandler RejectedJobsHandler
	  {
		  get
		  {
			return rejectedJobsHandler;
		  }
		  set
		  {
			this.rejectedJobsHandler = value;
		  }
	  }


	  protected internal virtual void startJobAcquisitionThread()
	  {
			if (jobAcquisitionThread == null)
			{
				jobAcquisitionThread = new Thread(acquireJobsRunnable, Name);
				jobAcquisitionThread.Start();
			}
	  }

		protected internal virtual void stopJobAcquisitionThread()
		{
			try
			{
				jobAcquisitionThread.Join();
			}
			catch (InterruptedException e)
			{
			  LOG.interruptedWhileShuttingDownjobExecutor(e);
			}
			jobAcquisitionThread = null;
		}

	  public virtual AcquireJobsRunnable AcquireJobsRunnable
	  {
		  get
		  {
			return acquireJobsRunnable;
		  }
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return new ExecuteJobsRunnable(jobIds, processEngine);
	  }

	}

}