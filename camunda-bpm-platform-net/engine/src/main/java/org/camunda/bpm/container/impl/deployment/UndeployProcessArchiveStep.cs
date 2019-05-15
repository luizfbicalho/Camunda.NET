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

	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using DeployedProcessArchive = org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;

	/// <summary>
	/// <para>Deployment operation step responsible for performing the undeployment of a
	/// process archive</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UndeployProcessArchiveStep : DeploymentOperationStep
	{

	  protected internal string processArchvieName;
	  protected internal JmxManagedProcessApplication deployedProcessApplication;
	  protected internal ProcessArchiveXml processArchive;
	  protected internal string processEngineName;

	  public UndeployProcessArchiveStep(JmxManagedProcessApplication deployedProcessApplication, ProcessArchiveXml processArchive, string processEngineName)
	  {
		this.deployedProcessApplication = deployedProcessApplication;
		this.processArchive = processArchive;
		this.processEngineName = processEngineName;
	  }

	  public override string Name
	  {
		  get
		  {
			return "Undeploying process archvie " + processArchvieName;
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive> processArchiveDeploymentMap = deployedProcessApplication.getProcessArchiveDeploymentMap();
		IDictionary<string, DeployedProcessArchive> processArchiveDeploymentMap = deployedProcessApplication.ProcessArchiveDeploymentMap;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive deployedProcessArchive = processArchiveDeploymentMap.get(processArchive.getName());
		DeployedProcessArchive deployedProcessArchive = processArchiveDeploymentMap[processArchive.Name];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.ProcessEngine processEngine = serviceContainer.getServiceValue(org.camunda.bpm.container.impl.spi.ServiceTypes.PROCESS_ENGINE, processEngineName);
		ProcessEngine processEngine = serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, processEngineName);

		// unregrister with the process engine.
		processEngine.ManagementService.unregisterProcessApplication(deployedProcessArchive.AllDeploymentIds, true);

		// delete the deployment if not disabled
		if (PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DELETE_UPON_UNDEPLOY, false))
		{
		  if (processEngine != null)
		  {
			// always cascade & skip custom listeners
			deleteDeployment(deployedProcessArchive.PrimaryDeploymentId, processEngine.RepositoryService);
		  }
		}

	  }

	  protected internal virtual void deleteDeployment(string deploymentId, RepositoryService repositoryService)
	  {
		repositoryService.deleteDeployment(deploymentId, true, true);
	  }

	}

}