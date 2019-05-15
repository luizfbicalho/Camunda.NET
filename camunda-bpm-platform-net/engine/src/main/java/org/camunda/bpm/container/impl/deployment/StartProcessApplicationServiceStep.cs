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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESSES_XML_RESOURCES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESS_APPLICATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESS_ARCHIVE_DEPLOYMENT_MAP;


	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationDeploymentInfo = org.camunda.bpm.application.ProcessApplicationDeploymentInfo;
	using ProcessApplicationDeploymentInfoImpl = org.camunda.bpm.application.impl.ProcessApplicationDeploymentInfoImpl;
	using ProcessApplicationInfoImpl = org.camunda.bpm.application.impl.ProcessApplicationInfoImpl;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using DeployedProcessArchive = org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive;
	using JmxManagedBpmPlatformPlugins = org.camunda.bpm.container.impl.jmx.services.JmxManagedBpmPlatformPlugins;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;

	/// <summary>
	/// <para>This deployment operation step starts an <seealso cref="MBeanService"/> for the process application.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartProcessApplicationServiceStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "Start Process Application Service";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<java.net.URL, org.camunda.bpm.application.impl.metadata.spi.ProcessesXml> processesXmls = operationContext.getAttachment(PROCESSES_XML_RESOURCES);
		IDictionary<URL, ProcessesXml> processesXmls = operationContext.getAttachment(PROCESSES_XML_RESOURCES);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive> processArchiveDeploymentMap = operationContext.getAttachment(PROCESS_ARCHIVE_DEPLOYMENT_MAP);
		IDictionary<string, DeployedProcessArchive> processArchiveDeploymentMap = operationContext.getAttachment(PROCESS_ARCHIVE_DEPLOYMENT_MAP);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

		ProcessApplicationInfoImpl processApplicationInfo = createProcessApplicationInfo(processApplication, processArchiveDeploymentMap);

		// create service
		JmxManagedProcessApplication mbean = new JmxManagedProcessApplication(processApplicationInfo, processApplication.Reference);
		mbean.ProcessesXmls = new List<ProcessesXml>(processesXmls.Values);
		mbean.DeploymentMap = processArchiveDeploymentMap;

		// start service
		serviceContainer.startService(ServiceTypes.PROCESS_APPLICATION, processApplication.Name, mbean);

		notifyBpmPlatformPlugins(serviceContainer, processApplication);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.application.impl.ProcessApplicationInfoImpl createProcessApplicationInfo(final org.camunda.bpm.application.AbstractProcessApplication processApplication, final java.util.Map<String, org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive> processArchiveDeploymentMap)
	  protected internal virtual ProcessApplicationInfoImpl createProcessApplicationInfo(AbstractProcessApplication processApplication, IDictionary<string, DeployedProcessArchive> processArchiveDeploymentMap)
	  {
		// populate process application info
		ProcessApplicationInfoImpl processApplicationInfo = new ProcessApplicationInfoImpl();

		processApplicationInfo.Name = processApplication.Name;
		processApplicationInfo.Properties = processApplication.Properties;

		// create deployment infos
		IList<ProcessApplicationDeploymentInfo> deploymentInfoList = new List<ProcessApplicationDeploymentInfo>();
		if (processArchiveDeploymentMap != null)
		{
		  foreach (KeyValuePair<string, DeployedProcessArchive> deployment in processArchiveDeploymentMap.SetOfKeyValuePairs())
		  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive deployedProcessArchive = deployment.getValue();
			DeployedProcessArchive deployedProcessArchive = deployment.Value;
			foreach (string deploymentId in deployedProcessArchive.AllDeploymentIds)
			{
			  ProcessApplicationDeploymentInfoImpl deploymentInfo = new ProcessApplicationDeploymentInfoImpl();
			  deploymentInfo.DeploymentId = deploymentId;
			  deploymentInfo.ProcessEngineName = deployedProcessArchive.ProcessEngineName;
			  deploymentInfoList.Add(deploymentInfo);
			}

		  }
		}

		processApplicationInfo.DeploymentInfo = deploymentInfoList;

		return processApplicationInfo;
	  }

	  protected internal virtual void notifyBpmPlatformPlugins(PlatformServiceContainer serviceContainer, AbstractProcessApplication processApplication)
	  {
		JmxManagedBpmPlatformPlugins plugins = serviceContainer.getService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_PLATFORM_PLUGINS);

		if (plugins != null)
		{
		  foreach (BpmPlatformPlugin plugin in plugins.Value.Plugins)
		  {
			plugin.postProcessApplicationDeploy(processApplication);
		  }
		}
	  }

	}

}