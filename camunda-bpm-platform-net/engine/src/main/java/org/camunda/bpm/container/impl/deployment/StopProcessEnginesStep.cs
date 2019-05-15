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
namespace org.camunda.bpm.container.impl.deployment
{
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>Deployment operation step that stops ALL process engines registered inside the container.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StopProcessEnginesStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public override string Name
	  {
		  get
		  {
			return "Stopping process engines";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
		ISet<string> serviceNames = serviceContainer.getServiceNames(ServiceTypes.PROCESS_ENGINE);

		foreach (string serviceName in serviceNames)
		{
		  stopProcessEngine(serviceName, serviceContainer);
		}

	  }

	  /// <summary>
	  /// Stops a process engine, failures are logged but no exceptions are thrown.
	  /// 
	  /// </summary>
	  private void stopProcessEngine(string serviceName, PlatformServiceContainer serviceContainer)
	  {

		try
		{
		  serviceContainer.stopService(serviceName);
		}
		catch (Exception e)
		{
		  LOG.exceptionWhileStopping("Process Engine", serviceName, e);
		}

	  }

	}

}