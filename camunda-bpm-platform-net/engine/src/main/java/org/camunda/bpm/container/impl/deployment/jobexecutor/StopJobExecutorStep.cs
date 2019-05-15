using System;
using System.Collections.Generic;

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
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>Deployment operation step responsible for stopping all job acquisitions</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StopJobExecutorStep : DeploymentOperationStep
	{

	  protected internal static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public override string Name
	  {
		  get
		  {
			return "Stop managed job acquisitions";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

		ISet<string> jobExecutorServiceNames = serviceContainer.getServiceNames(ServiceTypes.JOB_EXECUTOR);

		foreach (string serviceName in jobExecutorServiceNames)
		{
		  try
		  {
			serviceContainer.stopService(serviceName);
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileStopping("Job Executor Service", serviceName, e);
		  }
		}

	  }

	}

}