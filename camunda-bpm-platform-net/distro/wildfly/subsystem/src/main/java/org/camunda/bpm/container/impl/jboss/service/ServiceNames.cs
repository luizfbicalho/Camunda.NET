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
	using CommonAttributes = org.jboss.@as.threads.CommonAttributes;
	using ThreadsServices = org.jboss.@as.threads.ThreadsServices;
	using ServiceName = org.jboss.msc.service.ServiceName;

	/// <summary>
	/// <para>All ServiceName references run through here.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ServiceNames
	{

	  private static readonly ServiceName BPM_PLATFORM = ServiceName.of("org", "camunda", "bpm", "platform");

	  private static readonly ServiceName PROCESS_ENGINE = BPM_PLATFORM.append("process-engine");
	  private static readonly ServiceName JOB_EXECUTOR = BPM_PLATFORM.append("job-executor");
	  private static readonly ServiceName DEFAULT_PROCESS_ENGINE = PROCESS_ENGINE.append("default");

	  private static readonly ServiceName MSC_RUNTIME_CONTAINER_DELEGATE = BPM_PLATFORM.append("runtime-container");

	  private static readonly ServiceName PROCESS_APPLICATION = BPM_PLATFORM.append("process-application");

	  private static readonly ServiceName PROCESS_APPLICATION_MODULE = BPM_PLATFORM.append("process-application-module");

	  private static readonly ServiceName BPM_PLATFORM_PLUGINS = BPM_PLATFORM.append("bpm-platform-plugins");

	  /// <summary>
	  /// Returns the service name for a <seealso cref="MscManagedProcessEngine"/>.
	  /// </summary>
	  /// <param name="the">
	  ///          name of the process engine </param>
	  /// <returns> the composed service name </returns>
	  public static ServiceName forManagedProcessEngine(string processEngineName)
	  {
		return PROCESS_ENGINE.append(processEngineName);
	  }

	  /// <returns> the <seealso cref="ServiceName"/> for the default
	  ///         <seealso cref="MscManagedProcessEngine"/>. This is a constant name since
	  ///         there can only be one default process engine. </returns>
	  public static ServiceName forDefaultProcessEngine()
	  {
		return DEFAULT_PROCESS_ENGINE;
	  }

	  /// <returns> the <seealso cref="ServiceName"/> for the <seealso cref="MscRuntimeContainerDelegate"/> </returns>
	  public static ServiceName forMscRuntimeContainerDelegate()
	  {
		return MSC_RUNTIME_CONTAINER_DELEGATE;
	  }

	  /// <returns> the <seealso cref="ServiceName"/> that is the longest common prefix of all
	  /// ServiceNames used for <seealso cref="MscManagedProcessEngine"/>. </returns>
	  public static ServiceName forManagedProcessEngines()
	  {
		return PROCESS_ENGINE;
	  }

	  /// <returns> the <seealso cref="ServiceName"/> that is the longest common prefix of all
	  /// ServiceNames used for <seealso cref="MscManagedProcessApplication"/>. </returns>
	  public static ServiceName forManagedProcessApplications()
	  {
		return PROCESS_APPLICATION;
	  }

	  /// <param name="applicationName"> </param>
	  /// <returns> the name to be used for an <seealso cref="MscManagedProcessApplication"/> service. </returns>
	  public static ServiceName forManagedProcessApplication(string applicationName)
	  {
		return PROCESS_APPLICATION.append(applicationName);
	  }

	  public static ServiceName forProcessApplicationModuleService(string moduleName)
	  {
		return PROCESS_APPLICATION_MODULE.append(moduleName);
	  }

	  /// <param name="applicationName"> </param>
	  /// <returns> the name to be used for an <seealso cref="MscManagedProcessApplication"/> service. </returns>
	  public static ServiceName forProcessApplicationStartService(string moduleName)
	  {
		return PROCESS_APPLICATION_MODULE.append(moduleName).append("START");
	  }

	  /// <summary>
	  /// <para>Returns the name for a <seealso cref="ProcessApplicationDeploymentService"/> given
	  /// the name of the deployment unit and the name of the deployment.</para>
	  /// </summary>
	  /// <param name="processApplicationName"> </param>
	  /// <param name="deploymentId"> </param>
	  public static ServiceName forProcessApplicationDeploymentService(string moduleName, string deploymentName)
	  {
		return PROCESS_APPLICATION_MODULE.append(moduleName).append("DEPLOY").append(deploymentName);
	  }

	  public static ServiceName forNoViewProcessApplicationStartService(string moduleName)
	  {
		return PROCESS_APPLICATION_MODULE.append(moduleName).append("NO_VIEW");
	  }

	  /// <returns> the <seealso cref="ServiceName"/> of the <seealso cref="MscExecutorService"/>. </returns>
	  public static ServiceName forMscExecutorService()
	  {
		return BPM_PLATFORM.append("executor-service");
	  }

	  /// <returns> the <seealso cref="ServiceName"/> of the <seealso cref="MscRuntimeContainerJobExecutor"/> </returns>
	  public static ServiceName forMscRuntimeContainerJobExecutorService(string jobExecutorName)
	  {
		return JOB_EXECUTOR.append(jobExecutorName);
	  }

	  /// <returns> the <seealso cref="ServiceName"/> of the <seealso cref="MscBpmPlatformPlugins"/> </returns>
	  public static ServiceName forBpmPlatformPlugins()
	  {
		return BPM_PLATFORM_PLUGINS;
	  }

	  /// <returns> the <seealso cref="ServiceName"/> of the <seealso cref="ProcessApplicationStopService"/> </returns>
	  public static ServiceName forProcessApplicationStopService(string moduleName)
	  {
		return PROCESS_APPLICATION_MODULE.append(moduleName).append("STOP");
	  }

	  /// <returns> the <seealso cref="ServiceName"/> of the <seealso cref="org.jboss.as.threads.BoundedQueueThreadPoolService"/> </returns>
	  public static ServiceName forManagedThreadPool(string threadPoolName)
	  {
		return JOB_EXECUTOR.append(threadPoolName);
	  }

	  /// <returns> the <seealso cref="ServiceName"/> of the <seealso cref="org.jboss.as.threads.ThreadFactoryService"/> </returns>
	  public static ServiceName forThreadFactoryService(string threadFactoryName)
	  {
		return ThreadsServices.threadFactoryName(threadFactoryName);
	  }

	}

}