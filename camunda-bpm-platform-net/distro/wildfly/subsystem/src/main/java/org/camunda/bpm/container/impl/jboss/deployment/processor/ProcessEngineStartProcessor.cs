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
namespace org.camunda.bpm.container.impl.jboss.deployment.processor
{

	using ManagedProcessEngineMetadata = org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata;
	using ProcessApplicationAttachments = org.camunda.bpm.container.impl.jboss.deployment.marker.ProcessApplicationAttachments;
	using MscManagedProcessEngineController = org.camunda.bpm.container.impl.jboss.service.MscManagedProcessEngineController;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using ProcessesXmlWrapper = org.camunda.bpm.container.impl.jboss.util.ProcessesXmlWrapper;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using DeploymentPhaseContext = org.jboss.@as.server.deployment.DeploymentPhaseContext;
	using DeploymentUnit = org.jboss.@as.server.deployment.DeploymentUnit;
	using DeploymentUnitProcessingException = org.jboss.@as.server.deployment.DeploymentUnitProcessingException;
	using DeploymentUnitProcessor = org.jboss.@as.server.deployment.DeploymentUnitProcessor;
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using ServiceTarget = org.jboss.msc.service.ServiceTarget;


	/// <summary>
	/// <para>Deployment Unit Processor that creates process engine services for each 
	/// process engine configured in a <code>processes.xml</code> file</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessEngineStartProcessor : DeploymentUnitProcessor
	{

	  // this can happen at the beginning of the phase
	  public const int PRIORITY = 0x0000;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deploy(org.jboss.as.server.deployment.DeploymentPhaseContext phaseContext) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  public virtual void deploy(DeploymentPhaseContext phaseContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.DeploymentUnit deploymentUnit = phaseContext.getDeploymentUnit();
		DeploymentUnit deploymentUnit = phaseContext.DeploymentUnit;

		if (!ProcessApplicationAttachments.isProcessApplication(deploymentUnit))
		{
		  return;
		}

		IList<ProcessesXmlWrapper> processesXmls = ProcessApplicationAttachments.getProcessesXmls(deploymentUnit);
		foreach (ProcessesXmlWrapper wrapper in processesXmls)
		{
		  foreach (ProcessEngineXml processEngineXml in wrapper.ProcessesXml.ProcessEngines)
		  {
			startProcessEngine(processEngineXml, phaseContext);
		  }
		}

	  }

	  protected internal virtual void startProcessEngine(ProcessEngineXml processEngineXml, DeploymentPhaseContext phaseContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceTarget serviceTarget = phaseContext.getServiceTarget();
		ServiceTarget serviceTarget = phaseContext.ServiceTarget;

		// transform configuration
		ManagedProcessEngineMetadata configuration = transformConfiguration(processEngineXml);

		// validate the configuration
		configuration.validate();

		// create service instance
		MscManagedProcessEngineController service = new MscManagedProcessEngineController(configuration);

		// get the service name for the process engine
		ServiceName serviceName = ServiceNames.forManagedProcessEngine(processEngineXml.Name);

		// get service builder
		ServiceBuilder<ProcessEngine> serviceBuilder = serviceTarget.addService(serviceName, service);

		// make this service depend on the current phase -> makes sure it is removed with the phase service at undeployment
		serviceBuilder.addDependency(phaseContext.PhaseServiceName);

		// add Service dependencies
		MscManagedProcessEngineController.initializeServiceBuilder(configuration, service, serviceBuilder, processEngineXml.JobAcquisitionName);

		// install the service
		serviceBuilder.install();

	  }

	  /// <summary>
	  /// transforms the configuration as provided via the <seealso cref="ProcessEngineXml"/> 
	  /// into a <seealso cref="ManagedProcessEngineMetadata"/> 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) protected org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata transformConfiguration(org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml processEngineXml)
	  protected internal virtual ManagedProcessEngineMetadata transformConfiguration(ProcessEngineXml processEngineXml)
	  {
		return new ManagedProcessEngineMetadata(processEngineXml.Name.Equals("default"), processEngineXml.Name, processEngineXml.Datasource, processEngineXml.Properties["history"], processEngineXml.ConfigurationClass, (System.Collections.IDictionary) processEngineXml.Properties, processEngineXml.Plugins);
	  }

	  public virtual void undeploy(DeploymentUnit deploymentUnit)
	  {

	  }

	}

}