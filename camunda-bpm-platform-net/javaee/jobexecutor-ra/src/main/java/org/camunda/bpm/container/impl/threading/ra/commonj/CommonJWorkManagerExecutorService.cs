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
namespace org.camunda.bpm.container.impl.threading.ra.commonj
{

	using JcaInflowExecuteJobsRunnable = org.camunda.bpm.container.impl.threading.ra.inflow.JcaInflowExecuteJobsRunnable;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;

	using WorkException = commonj.work.WorkException;
	using WorkManager = commonj.work.WorkManager;
	using WorkRejectedException = commonj.work.WorkRejectedException;

	/// <summary>
	/// <seealso cref="AbstractPlatformJobExecutor"/> implementation delegating to a CommonJ <seealso cref="WorkManager"/>.
	/// 
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	public class CommonJWorkManagerExecutorService : ExecutorService
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger logger = Logger.getLogger(typeof(CommonJWorkManagerExecutorService).FullName);

	  protected internal WorkManager workManager;

	  protected internal JcaExecutorServiceConnector ra;

	  protected internal string commonJWorkManagerName;

	  protected internal virtual WorkManager lookupWorkMananger()
	  {
		try
		{
		  InitialContext initialContext = new InitialContext();
		  return (WorkManager) initialContext.lookup(commonJWorkManagerName);
		}
		catch (Exception e)
		{
		  throw new Exception("Error while starting JobExecutor: could not look up CommonJ WorkManager in Jndi: " + e.Message, e);
		}
	  }

	  public CommonJWorkManagerExecutorService(JcaExecutorServiceConnector ra, string commonJWorkManagerName)
	  {
		this.ra = ra;
		this.commonJWorkManagerName = commonJWorkManagerName;
	  }

	  public virtual bool schedule(ThreadStart runnable, bool isLongRunning)
	  {
		if (isLongRunning)
		{
		  return scheduleLongRunning(runnable);

		}
		else
		{
		  return executeShortRunning(runnable);

		}
	  }

	  protected internal virtual bool executeShortRunning(ThreadStart runnable)
	  {
		try
		{
		  workManager.schedule(new CommonjWorkRunnableAdapter(runnable));
		  return true;

		}
		catch (WorkRejectedException e)
		{
		  logger.log(Level.FINE, "Work rejected", e);

		}
		catch (WorkException e)
		{
		  logger.log(Level.WARNING, "WorkException while scheduling jobs for execution", e);

		}
		return false;
	  }

	  protected internal virtual bool scheduleLongRunning(ThreadStart acquisitionRunnable)
	  {
		// initialize the workManager here, because we have access to the initial context
		// of the calling thread (application), so the jndi lookup is working -> see JCA 1.6 specification
		if (workManager == null)
		{
		  workManager = lookupWorkMananger();
		}

		try
		{
		  workManager.schedule(new CommonjDeamonWorkRunnableAdapter(acquisitionRunnable));
		  return true;

		}
		catch (WorkException e)
		{
		  logger.log(Level.WARNING, "Could not schedule Job Acquisition Runnable: " + e.Message, e);
		  return false;

		}
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return new JcaInflowExecuteJobsRunnable(jobIds, processEngine, ra);
	  }

	  // getters / setters ////////////////////////////////////

	  public virtual WorkManager WorkManager
	  {
		  get
		  {
			return workManager;
		  }
	  }

	}

}