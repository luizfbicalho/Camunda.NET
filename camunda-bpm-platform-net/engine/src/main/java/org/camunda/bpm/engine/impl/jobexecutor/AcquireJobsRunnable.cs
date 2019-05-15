using System;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AcquireJobsRunnable : ThreadStart
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal readonly JobExecutor jobExecutor;

	  protected internal volatile bool isInterrupted = false;
	  protected internal volatile bool isJobAdded = false;
	  protected internal readonly object MONITOR = new object();
	  protected internal readonly AtomicBoolean isWaiting = new AtomicBoolean(false);

	  public AcquireJobsRunnable(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
	  }

	  protected internal virtual void suspendAcquisition(long millis)
	  {
		if (millis <= 0)
		{
		  return;
		}

		try
		{
		  LOG.debugJobAcquisitionThreadSleeping(millis);
		  lock (MONITOR)
		  {
			if (!isInterrupted)
			{
			  isWaiting.set(true);
			  Monitor.Wait(MONITOR, TimeSpan.FromMilliseconds(millis));
			}
		  }
		  LOG.jobExecutorThreadWokeUp();
		}
		catch (InterruptedException)
		{
		  LOG.jobExecutionWaitInterrupted();
		}
		finally
		{
		  isWaiting.set(false);
		}
	  }

	  public virtual void stop()
	  {
		lock (MONITOR)
		{
		  isInterrupted = true;
		  if (isWaiting.compareAndSet(true, false))
		  {
			Monitor.PulseAll(MONITOR);
		  }
		}
	  }

	  public virtual void jobWasAdded()
	  {
		isJobAdded = true;
		if (isWaiting.compareAndSet(true, false))
		{
		  // ensures we only notify once
		  // I am OK with the race condition
		  lock (MONITOR)
		  {
			Monitor.PulseAll(MONITOR);
		  }
		}
	  }

	  protected internal virtual void clearJobAddedNotification()
	  {
		isJobAdded = false;
	  }

	  public virtual bool JobAdded
	  {
		  get
		  {
			return isJobAdded;
		  }
	  }
	}

}