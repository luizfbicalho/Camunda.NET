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
namespace org.camunda.bpm.container.impl.threading.ra
{


	using JcaInflowExecuteJobsRunnable = org.camunda.bpm.container.impl.threading.ra.inflow.JcaInflowExecuteJobsRunnable;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;



	/// <summary>
	/// <seealso cref="AbstractPlatformJobExecutor"/> implementation delegating to a JCA <seealso cref="WorkManager"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JcaWorkManagerExecutorService : Referenceable, ExecutorService
	{

	  public static int START_WORK_TIMEOUT = 1500;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger logger = Logger.getLogger(typeof(JcaWorkManagerExecutorService).FullName);

	  protected internal readonly JcaExecutorServiceConnector ra;
	  protected internal WorkManager workManager;

	  public JcaWorkManagerExecutorService(JcaExecutorServiceConnector connector, WorkManager workManager)
	  {
		this.workManager = workManager;
		this.ra = connector;
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

	  protected internal virtual bool scheduleLongRunning(ThreadStart runnable)
	  {
		try
		{
		  workManager.scheduleWork(new JcaWorkRunnableAdapter(runnable));
		  return true;

		}
		catch (WorkException e)
		{
		  logger.log(Level.WARNING, "Could not schedule : " + e.Message, e);
		  return false;

		}
	  }

	  protected internal virtual bool executeShortRunning(ThreadStart runnable)
	  {

		try
		{
		  workManager.startWork(new JcaWorkRunnableAdapter(runnable), START_WORK_TIMEOUT, null, null);
		  return true;

		}
		catch (WorkRejectedException e)
		{
		  logger.log(Level.FINE, "WorkRejectedException while scheduling jobs for execution", e);

		}
		catch (WorkException e)
		{
		  logger.log(Level.WARNING, "WorkException while scheduling jobs for execution", e);
		}

		return false;
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return new JcaInflowExecuteJobsRunnable(jobIds, processEngine, ra);
	  }

	  // javax.resource.Referenceable /////////////////////////

	  protected internal Reference reference;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.naming.Reference getReference() throws javax.naming.NamingException
	  public virtual Reference Reference
	  {
		  get
		  {
			return reference;
		  }
		  set
		  {
			this.reference = value;
		  }
	  }


	  // getters / setters ////////////////////////////////////

	  public virtual WorkManager WorkManager
	  {
		  get
		  {
			return workManager;
		  }
	  }

	  public virtual JcaExecutorServiceConnector PlatformJobExecutorConnector
	  {
		  get
		  {
			return ra;
		  }
	  }

	}

}