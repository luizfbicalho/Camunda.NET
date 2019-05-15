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
namespace org.camunda.bpm.container.impl.deployment.jobexecutor
{

	using JmxManagedThreadPool = org.camunda.bpm.container.impl.jmx.services.JmxManagedThreadPool;
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using JobExecutorXml = org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;

	/// <summary>
	/// <para>
	/// Deployment operation step responsible for deploying a thread pool for the
	/// JobExecutor
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartManagedThreadPoolStep : DeploymentOperationStep
	{

	  private const int DEFAULT_CORE_POOL_SIZE = 3;
	  private const int DEFAULT_MAX_POOL_SIZE = 10;
	  private const long DEFAULT_KEEP_ALIVE_TIME_MS = 0L;
	  private const int DEFAULT_QUEUE_SIZE = 3;

	  public override string Name
	  {
		  get
		  {
			return "Deploy Job Executor Thread Pool";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

		JobExecutorXml jobExecutorXml = getJobExecutorXml(operationContext);

		int queueSize = getQueueSize(jobExecutorXml);
		int corePoolSize = getCorePoolSize(jobExecutorXml);
		int maxPoolSize = getMaxPoolSize(jobExecutorXml);
		long keepAliveTime = getKeepAliveTime(jobExecutorXml);

		// initialize Queue & Executor services
		BlockingQueue<ThreadStart> threadPoolQueue = new ArrayBlockingQueue<ThreadStart>(queueSize);

		ThreadPoolExecutor threadPoolExecutor = new ThreadPoolExecutor(corePoolSize, maxPoolSize, keepAliveTime, TimeUnit.MILLISECONDS, threadPoolQueue);
		threadPoolExecutor.RejectedExecutionHandler = new ThreadPoolExecutor.AbortPolicy();

		// construct the service for the thread pool
		JmxManagedThreadPool managedThreadPool = new JmxManagedThreadPool(threadPoolQueue, threadPoolExecutor);

		// install the service into the container
		serviceContainer.startService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_EXECUTOR, managedThreadPool);

	  }

	  private JobExecutorXml getJobExecutorXml(DeploymentOperation operationContext)
	  {
		BpmPlatformXml bpmPlatformXml = operationContext.getAttachment(Attachments.BPM_PLATFORM_XML);
		JobExecutorXml jobExecutorXml = bpmPlatformXml.JobExecutor;
		return jobExecutorXml;
	  }

	  private int getQueueSize(JobExecutorXml jobExecutorXml)
	  {
		string queueSize = jobExecutorXml.Properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.QUEUE_SIZE];
		if (string.ReferenceEquals(queueSize, null))
		{
		  return DEFAULT_QUEUE_SIZE;
		}
		return int.Parse(queueSize);
	  }

	  private long getKeepAliveTime(JobExecutorXml jobExecutorXml)
	  {
		string keepAliveTime = jobExecutorXml.Properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.KEEP_ALIVE_TIME];
		if (string.ReferenceEquals(keepAliveTime, null))
		{
		  return DEFAULT_KEEP_ALIVE_TIME_MS;
		}
		return long.Parse(keepAliveTime);
	  }

	  private int getMaxPoolSize(JobExecutorXml jobExecutorXml)
	  {
		string maxPoolSize = jobExecutorXml.Properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.MAX_POOL_SIZE];
		if (string.ReferenceEquals(maxPoolSize, null))
		{
		  return DEFAULT_MAX_POOL_SIZE;
		}
		return int.Parse(maxPoolSize);
	  }

	  private int getCorePoolSize(JobExecutorXml jobExecutorXml)
	  {
		string corePoolSize = jobExecutorXml.Properties[org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml_Fields.CORE_POOL_SIZE];
		if (string.ReferenceEquals(corePoolSize, null))
		{
		  return DEFAULT_CORE_POOL_SIZE;
		}
		return int.Parse(corePoolSize);
	  }
	}

}