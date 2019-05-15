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
	/// <para>This is a simple implementation of the <seealso cref="JobExecutor"/> using self-managed
	/// threads for performing background work.</para>
	/// 
	/// <para>This implementation uses a <seealso cref="ThreadPoolExecutor"/> backed by a queue to which
	/// work is submitted.</para>
	/// 
	/// <para><em>NOTE: use this class in environments in which self-management of threads
	/// is permitted. Consider using a different thread-management strategy in
	/// J(2)EE-Environments.</em></para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class DefaultJobExecutor : ThreadPoolJobExecutor
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal int queueSize = 3;
	  protected internal int corePoolSize = 3;
	  protected internal int maxPoolSize = 10;

	  protected internal override void startExecutingJobs()
	  {

		if (threadPoolExecutor == null || threadPoolExecutor.Shutdown)
		{
		  BlockingQueue<ThreadStart> threadPoolQueue = new ArrayBlockingQueue<ThreadStart>(queueSize);
		  threadPoolExecutor = new ThreadPoolExecutor(corePoolSize, maxPoolSize, 0L, TimeUnit.MILLISECONDS, threadPoolQueue);
		  threadPoolExecutor.RejectedExecutionHandler = new ThreadPoolExecutor.AbortPolicy();
		}

		base.startExecutingJobs();
	  }

	  protected internal override void stopExecutingJobs()
	  {

		base.stopExecutingJobs();

		// Ask the thread pool to finish and exit
		threadPoolExecutor.shutdown();

		// Waits for 1 minute to finish all currently executing jobs
		try
		{
		  if (!threadPoolExecutor.awaitTermination(60L, TimeUnit.SECONDS))
		  {
			LOG.timeoutDuringShutdown();
		  }
		}
		catch (InterruptedException e)
		{
		  LOG.interruptedWhileShuttingDownjobExecutor(e);
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual int QueueSize
	  {
		  get
		  {
			return queueSize;
		  }
		  set
		  {
			this.queueSize = value;
		  }
	  }


	  public virtual int CorePoolSize
	  {
		  get
		  {
			return corePoolSize;
		  }
		  set
		  {
			this.corePoolSize = value;
		  }
	  }


	  public virtual int MaxPoolSize
	  {
		  get
		  {
			return maxPoolSize;
		  }
		  set
		  {
			this.maxPoolSize = value;
		  }
	  }


	}


}