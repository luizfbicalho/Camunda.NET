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
	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// <para>Deployment operation responsible for stopping all process engines started by the deployment.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessesXmlStopProcessEnginesStep : DeploymentOperationStep
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication deployedProcessApplication = serviceContainer.getService(org.camunda.bpm.container.impl.spi.ServiceTypes.PROCESS_APPLICATION, processApplication.getName());
		JmxManagedProcessApplication deployedProcessApplication = serviceContainer.getService(ServiceTypes.PROCESS_APPLICATION, processApplication.Name);

		ensureNotNull("Cannot find process application with name " + processApplication.Name, "deployedProcessApplication", deployedProcessApplication);

		IList<ProcessesXml> processesXmls = deployedProcessApplication.ProcessesXmls;
		foreach (ProcessesXml processesXml in processesXmls)
		{
		  stopProcessEngines(processesXml.ProcessEngines, operationContext);
		}

	  }

	  protected internal virtual void stopProcessEngines(IList<ProcessEngineXml> processEngine, DeploymentOperation operationContext)
	  {
		foreach (ProcessEngineXml parsedProcessEngine in processEngine)
		{
		  stopProcessEngine(parsedProcessEngine.Name, operationContext);
		}
	  }

	  protected internal virtual void stopProcessEngine(string processEngineName, DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
		try
		{
		  serviceContainer.stopService(ServiceTypes.PROCESS_ENGINE, processEngineName);
		}
		catch (Exception e)
		{
		  LOG.exceptionWhileStopping("Process Engine", processEngineName, e);
		}

	  }

	}

}