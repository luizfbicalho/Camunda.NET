﻿using System;
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

	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>Deployment operation step that is responsible for stopping (undeploying) all process applications</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StopProcessApplicationsStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public override string Name
	  {
		  get
		  {
			return "Stopping process applications";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
		IList<JmxManagedProcessApplication> processApplicationsReferences = serviceContainer.getServiceValuesByType(ServiceTypes.PROCESS_APPLICATION);

		foreach (JmxManagedProcessApplication processApplication in processApplicationsReferences)
		{
		  stopProcessApplication(processApplication.ProcessApplicationReference);
		}

	  }

	  /// <summary>
	  /// <para> Stops a process application. Exceptions are logged but not re-thrown).
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="processApplicationReference"> </param>
	  protected internal virtual void stopProcessApplication(ProcessApplicationReference processApplicationReference)
	  {

		try
		{
		  // unless the user has overridden the stop behavior,
		  // this causes the process application to remove its services
		  // (triggers nested undeployment operation)
		  ProcessApplicationInterface processApplication = processApplicationReference.ProcessApplication;
		  processApplication.undeploy();
		}
		catch (Exception t)
		{
		  LOG.exceptionWhileStopping("Process Application", processApplicationReference.Name, t);
		}

	  }

	}

}