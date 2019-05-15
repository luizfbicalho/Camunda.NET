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
namespace org.camunda.bpm.container.impl.jmx.services
{
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using SeExecutorService = org.camunda.bpm.container.impl.threading.se.SeExecutorService;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JmxManagedThreadPool : SeExecutorService, JmxManagedThreadPoolMBean, PlatformService<JmxManagedThreadPool>
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal readonly BlockingQueue<ThreadStart> threadPoolQueue;

	  public JmxManagedThreadPool(BlockingQueue<ThreadStart> queue, ThreadPoolExecutor executor) : base(executor)
	  {
		threadPoolQueue = queue;
	  }

	  public virtual void start(PlatformServiceContainer mBeanServiceContainer)
	  {
		// nothing to do
	  }

	  public virtual void stop(PlatformServiceContainer mBeanServiceContainer)
	  {

		// clear the queue
		threadPoolQueue.clear();

		// Ask the thread pool to finish and exit
		threadPoolExecutor.shutdown();

		// Waits for 1 minute to finish all currently executing jobs
		try
		{
		  if (!threadPoolExecutor.awaitTermination(60L, TimeUnit.SECONDS))
		  {
			LOG.timeoutDuringShutdownOfThreadPool(60, TimeUnit.SECONDS);
		  }
		}
		catch (InterruptedException e)
		{
		  LOG.interruptedWhileShuttingDownThreadPool(e);
		}

	  }

	  public virtual JmxManagedThreadPool Value
	  {
		  get
		  {
			return this;
		  }
	  }

	  public virtual int CorePoolSize
	  {
		  set
		  {
			threadPoolExecutor.CorePoolSize = value;
		  }
	  }

	  public virtual int MaximumPoolSize
	  {
		  set
		  {
			threadPoolExecutor.MaximumPoolSize = value;
		  }
		  get
		  {
			return threadPoolExecutor.MaximumPoolSize;
		  }
	  }


	  public virtual void setKeepAliveTime(long time, TimeUnit unit)
	  {
		threadPoolExecutor.setKeepAliveTime(time, unit);
	  }

	  public virtual void purgeThreadPool()
	  {
		threadPoolExecutor.purge();
	  }

	  public virtual int PoolSize
	  {
		  get
		  {
			return threadPoolExecutor.PoolSize;
		  }
	  }

	  public virtual int ActiveCount
	  {
		  get
		  {
			return threadPoolExecutor.ActiveCount;
		  }
	  }

	  public virtual int LargestPoolSize
	  {
		  get
		  {
			return threadPoolExecutor.LargestPoolSize;
		  }
	  }

	  public virtual long TaskCount
	  {
		  get
		  {
			return threadPoolExecutor.TaskCount;
		  }
	  }

	  public virtual long CompletedTaskCount
	  {
		  get
		  {
			return threadPoolExecutor.CompletedTaskCount;
		  }
	  }

	  public virtual int QueueCount
	  {
		  get
		  {
			return threadPoolQueue.size();
		  }
	  }

	  public virtual ThreadPoolExecutor ThreadPoolExecutor
	  {
		  get
		  {
			return threadPoolExecutor;
		  }
	  }
	}

}