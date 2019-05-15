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
	using UnlockJobCmd = org.camunda.bpm.engine.impl.cmd.UnlockJobCmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class ExecuteJobsRunnable : ThreadStart
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal readonly IList<string> jobIds;
	  protected internal JobExecutor jobExecutor;
	  protected internal ProcessEngineImpl processEngine;

	  public ExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		this.jobIds = jobIds;
		this.processEngine = processEngine;
		this.jobExecutor = processEngine.ProcessEngineConfiguration.JobExecutor;
	  }

	  public virtual void run()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final JobExecutorContext jobExecutorContext = new JobExecutorContext();
		JobExecutorContext jobExecutorContext = new JobExecutorContext();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> currentProcessorJobQueue = jobExecutorContext.getCurrentProcessorJobQueue();
		IList<string> currentProcessorJobQueue = jobExecutorContext.CurrentProcessorJobQueue;
		CommandExecutor commandExecutor = processEngine.ProcessEngineConfiguration.CommandExecutorTxRequired;

		((IList<string>)currentProcessorJobQueue).AddRange(jobIds);

		Context.JobExecutorContext = jobExecutorContext;
		try
		{
		  while (currentProcessorJobQueue.Count > 0)
		  {

			string nextJobId = currentProcessorJobQueue.RemoveAt(0);
			if (jobExecutor.Active)
			{
			  try
			  {
				 executeJob(nextJobId, commandExecutor);
			  }
			  catch (Exception t)
			  {
				LOG.exceptionWhileExecutingJob(nextJobId, t);
			  }
			}
			else
			{
				try
				{
				  unlockJob(nextJobId, commandExecutor);
				}
				catch (Exception t)
				{
				  LOG.exceptionWhileUnlockingJob(nextJobId, t);
				}

			}
		  }

		  // if there were only exclusive jobs then the job executor
		  // does a backoff. In order to avoid too much waiting time
		  // we need to tell him to check once more if there were any jobs added.
		  jobExecutor.jobWasAdded();

		}
		finally
		{
		  Context.removeJobExecutorContext();
		}
	  }

	  /// <summary>
	  /// Note: this is a hook to be overridden by
	  /// org.camunda.bpm.container.impl.threading.ra.inflow.JcaInflowExecuteJobsRunnable.executeJob(String, CommandExecutor)
	  /// </summary>
	  protected internal virtual void executeJob(string nextJobId, CommandExecutor commandExecutor)
	  {
		ExecuteJobHelper.executeJob(nextJobId, commandExecutor);
	  }

	  protected internal virtual void unlockJob(string nextJobId, CommandExecutor commandExecutor)
	  {
		commandExecutor.execute(new UnlockJobCmd(nextJobId));
	  }

	}

}