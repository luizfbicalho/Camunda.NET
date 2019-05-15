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
namespace org.camunda.bpm.container.impl.jboss.service
{

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ExecuteJobsRunnable = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobsRunnable;
	using ManagedQueueExecutorService = org.jboss.@as.threads.ManagedQueueExecutorService;
	using Service = org.jboss.msc.service.Service;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;
	using InjectedValue = org.jboss.msc.value.InjectedValue;
	using ExecutionTimedOutException = org.jboss.threads.ExecutionTimedOutException;


	public class MscExecutorService : Service<MscExecutorService>, ExecutorService
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger log = Logger.getLogger(typeof(MscExecutorService).FullName);

	  private readonly InjectedValue<ManagedQueueExecutorService> managedQueueInjector = new InjectedValue<ManagedQueueExecutorService>();

	  private long lastWarningLogged = DateTimeHelper.CurrentUnixTimeMillis();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MscExecutorService getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual MscExecutorService Value
	  {
		  get
		  {
			return this;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void start(StartContext context)
	  {
		// nothing to do
	  }

	  public virtual void stop(StopContext context)
	  {
		// nothing to do    
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return new ExecuteJobsRunnable(jobIds, processEngine);
	  }

	  public virtual bool schedule(ThreadStart runnable, bool isLongRunning)
	  {

		if (isLongRunning)
		{
		  return scheduleLongRunningWork(runnable);

		}
		else
		{
		  return scheduleShortRunningWork(runnable);

		}

	  }

	  protected internal virtual bool scheduleShortRunningWork(ThreadStart runnable)
	  {

		ManagedQueueExecutorService managedQueueExecutorService = managedQueueInjector.Value;

		try
		{

		  managedQueueExecutorService.executeBlocking(runnable);
		  return true;

		}
		catch (InterruptedException)
		{
		  // the the acquisition thread is interrupted, this probably means the app server is turning the lights off -> ignore          
		}
		catch (Exception e)
		{
		  // we must be able to schedule this
		  log.log(Level.WARNING, "Cannot schedule long running work.", e);
		}

		return false;
	  }

	  protected internal virtual bool scheduleLongRunningWork(ThreadStart runnable)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.threads.ManagedQueueExecutorService managedQueueExecutorService = managedQueueInjector.getValue();
		ManagedQueueExecutorService managedQueueExecutorService = managedQueueInjector.Value;

		bool rejected = false;
		try
		{

		  // wait for 2 seconds for the job to be accepted by the pool.
		  managedQueueExecutorService.executeBlocking(runnable, 2, TimeUnit.SECONDS);

		}
		catch (InterruptedException)
		{
		  // the acquisition thread is interrupted, this probably means the app server is turning the lights off -> ignore          
		}
		catch (ExecutionTimedOutException)
		{
		  rejected = true;
		}
		catch (RejectedExecutionException)
		{
		  rejected = true;
		}
		catch (Exception e)
		{
		  // if it fails for some other reason, log a warning message
		  long now = DateTimeHelper.CurrentUnixTimeMillis();
		  // only log every 60 seconds to prevent log flooding
		  if ((now - lastWarningLogged) >= (60 * 1000))
		  {
			log.log(Level.WARNING, "Unexpected Exception while submitting job to executor pool.", e);
		  }
		  else
		  {
			log.log(Level.FINE, "Unexpected Exception while submitting job to executor pool.", e);
		  }
		}

		return !rejected;

	  }

	  public virtual InjectedValue<ManagedQueueExecutorService> ManagedQueueInjector
	  {
		  get
		  {
			return managedQueueInjector;
		  }
	  }

	}

}