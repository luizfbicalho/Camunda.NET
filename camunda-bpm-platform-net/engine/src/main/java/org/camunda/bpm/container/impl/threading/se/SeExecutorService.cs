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
namespace org.camunda.bpm.container.impl.threading.se
{
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ExecuteJobsRunnable = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobsRunnable;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SeExecutorService : ExecutorService
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal ThreadPoolExecutor threadPoolExecutor;

	  public SeExecutorService(ThreadPoolExecutor threadPoolExecutor)
	  {
		this.threadPoolExecutor = threadPoolExecutor;
	  }

	  public virtual bool schedule(ThreadStart runnable, bool isLongRunning)
	  {

		if (isLongRunning)
		{
		  return executeLongRunning(runnable);

		}
		else
		{
		  return executeShortRunning(runnable);

		}
	  }

	  protected internal virtual bool executeLongRunning(ThreadStart runnable)
	  {
		(new Thread(runnable)).Start();
		return true;
	  }

	  protected internal virtual bool executeShortRunning(ThreadStart runnable)
	  {

		try
		{
		  threadPoolExecutor.execute(runnable);
		  return true;
		}
		catch (RejectedExecutionException e)
		{
		  LOG.debugRejectedExecutionException(e);
		  return false;
		}

	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return new ExecuteJobsRunnable(jobIds, processEngine);
	  }

	}

}