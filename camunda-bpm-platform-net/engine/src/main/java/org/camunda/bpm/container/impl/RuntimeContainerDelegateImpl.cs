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
namespace org.camunda.bpm.container.impl
{
	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using Attachments = org.camunda.bpm.container.impl.deployment.Attachments;
	using DeployProcessArchivesStep = org.camunda.bpm.container.impl.deployment.DeployProcessArchivesStep;
	using NotifyPostProcessApplicationUndeployedStep = org.camunda.bpm.container.impl.deployment.NotifyPostProcessApplicationUndeployedStep;
	using ParseProcessesXmlStep = org.camunda.bpm.container.impl.deployment.ParseProcessesXmlStep;
	using PostDeployInvocationStep = org.camunda.bpm.container.impl.deployment.PostDeployInvocationStep;
	using PreUndeployInvocationStep = org.camunda.bpm.container.impl.deployment.PreUndeployInvocationStep;
	using ProcessesXmlStartProcessEnginesStep = org.camunda.bpm.container.impl.deployment.ProcessesXmlStartProcessEnginesStep;
	using ProcessesXmlStopProcessEnginesStep = org.camunda.bpm.container.impl.deployment.ProcessesXmlStopProcessEnginesStep;
	using StartProcessApplicationServiceStep = org.camunda.bpm.container.impl.deployment.StartProcessApplicationServiceStep;
	using StopProcessApplicationServiceStep = org.camunda.bpm.container.impl.deployment.StopProcessApplicationServiceStep;
	using UndeployProcessArchivesStep = org.camunda.bpm.container.impl.deployment.UndeployProcessArchivesStep;
	using MBeanServiceContainer = org.camunda.bpm.container.impl.jmx.MBeanServiceContainer;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using JmxManagedProcessEngine = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessEngine;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// <para>This is the default <seealso cref="RuntimeContainerDelegate"/> implementation that delegates
	/// to the local <seealso cref="MBeanServer"/> infrastructure. The MBeanServer is available
	/// as per the Java Virtual Machine and allows the process engine to expose
	/// Management Resources.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class RuntimeContainerDelegateImpl : RuntimeContainerDelegate, ProcessEngineService, ProcessApplicationService
	{

	  protected internal static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal MBeanServiceContainer serviceContainer = new MBeanServiceContainer();

	  public const string SERVICE_NAME_EXECUTOR = "executor-service";
	  public const string SERVICE_NAME_PLATFORM_PLUGINS = "bpm-platform-plugins";

	  // runtime container delegate implementation ///////////////////////////////////////////////

	  public virtual void registerProcessEngine(ProcessEngine processEngine)
	  {
		ensureNotNull("Cannot register process engine in Jmx Runtime Container", "process engine", processEngine);

		string processEngineName = processEngine.Name;

		// build and start the service.
		JmxManagedProcessEngine managedProcessEngine = new JmxManagedProcessEngine(processEngine);
		serviceContainer.startService(ServiceTypes.PROCESS_ENGINE, processEngineName, managedProcessEngine);

	  }

	  public virtual void unregisterProcessEngine(ProcessEngine processEngine)
	  {
		ensureNotNull("Cannot unregister process engine in Jmx Runtime Container", "process engine", processEngine);

		serviceContainer.stopService(ServiceTypes.PROCESS_ENGINE, processEngine.Name);

	  }

	  public virtual void deployProcessApplication(AbstractProcessApplication processApplication)
	  {
		ensureNotNull("Process application", processApplication);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String operationName = "Deployment of Process Application " + processApplication.getName();
		string operationName = "Deployment of Process Application " + processApplication.Name;

		serviceContainer.createDeploymentOperation(operationName).addAttachment(Attachments.PROCESS_APPLICATION, processApplication).addSteps(DeploymentSteps).execute();

		LOG.paDeployed(processApplication.Name);
	  }

	  public virtual void undeployProcessApplication(AbstractProcessApplication processApplication)
	  {
		ensureNotNull("Process application", processApplication);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processAppName = processApplication.getName();
		string processAppName = processApplication.Name;

		// if the process application is not deployed, ignore the request.
		if (serviceContainer.getService(ServiceTypes.PROCESS_APPLICATION, processAppName) == null)
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String operationName = "Undeployment of Process Application " + processAppName;
		string operationName = "Undeployment of Process Application " + processAppName;

		// perform the undeployment
		serviceContainer.createUndeploymentOperation(operationName).addAttachment(Attachments.PROCESS_APPLICATION, processApplication).addSteps(UndeploymentSteps).execute();

		LOG.paUndeployed(processApplication.Name);
	  }


	  protected internal virtual IList<DeploymentOperationStep> DeploymentSteps
	  {
		  get
		  {
			return Arrays.asList(new ParseProcessesXmlStep(), new ProcessesXmlStartProcessEnginesStep(), new DeployProcessArchivesStep(), new StartProcessApplicationServiceStep(), new PostDeployInvocationStep());
		  }
	  }

	  protected internal virtual IList<DeploymentOperationStep> UndeploymentSteps
	  {
		  get
		  {
			return Arrays.asList(new PreUndeployInvocationStep(), new UndeployProcessArchivesStep(), new ProcessesXmlStopProcessEnginesStep(), new StopProcessApplicationServiceStep(), new NotifyPostProcessApplicationUndeployedStep()
		   );
		  }
	  }


	  public virtual ProcessEngineService ProcessEngineService
	  {
		  get
		  {
			return this;
		  }
	  }


	  public virtual ProcessApplicationService ProcessApplicationService
	  {
		  get
		  {
			return this;
		  }
	  }

	  public virtual ExecutorService ExecutorService
	  {
		  get
		  {
			return serviceContainer.getServiceValue(ServiceTypes.BPM_PLATFORM, SERVICE_NAME_EXECUTOR);
		  }
	  }

	  // ProcessEngineServiceDelegate //////////////////////////////////////////////

	  public virtual ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			return serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, "default");
		  }
	  }

	  public virtual ProcessEngine getProcessEngine(string name)
	  {
		return serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, name);
	  }

	  public virtual IList<ProcessEngine> ProcessEngines
	  {
		  get
		  {
			return serviceContainer.getServiceValuesByType(ServiceTypes.PROCESS_ENGINE);
		  }
	  }

	  public virtual ISet<string> ProcessEngineNames
	  {
		  get
		  {
			ISet<string> processEngineNames = new HashSet<string>();
			IList<ProcessEngine> processEngines = ProcessEngines;
			foreach (ProcessEngine processEngine in processEngines)
			{
			  processEngineNames.Add(processEngine.Name);
			}
			return processEngineNames;
		  }
	  }

	  // process application service implementation /////////////////////////////////

	  public virtual ISet<string> ProcessApplicationNames
	  {
		  get
		  {
			IList<JmxManagedProcessApplication> processApplications = serviceContainer.getServiceValuesByType(ServiceTypes.PROCESS_APPLICATION);
			ISet<string> processApplicationNames = new HashSet<string>();
			foreach (JmxManagedProcessApplication jmxManagedProcessApplication in processApplications)
			{
			  processApplicationNames.Add(jmxManagedProcessApplication.ProcessApplicationName);
			}
			return processApplicationNames;
		  }
	  }

	  public virtual ProcessApplicationInfo getProcessApplicationInfo(string processApplicationName)
	  {

		JmxManagedProcessApplication processApplicationService = serviceContainer.getServiceValue(ServiceTypes.PROCESS_APPLICATION, processApplicationName);

		if (processApplicationService == null)
		{
		  return null;
		}
		else
		{
		  return processApplicationService.ProcessApplicationInfo;
		}
	  }

	  public virtual ProcessApplicationReference getDeployedProcessApplication(string processApplicationName)
	  {
		JmxManagedProcessApplication processApplicationService = serviceContainer.getServiceValue(ServiceTypes.PROCESS_APPLICATION, processApplicationName);

		if (processApplicationService == null)
		{
		  return null;
		}
		else
		{
		  return processApplicationService.ProcessApplicationReference;
		}
	  }

	  // Getter / Setter ////////////////////////////////////////////////////////////

	  public virtual PlatformServiceContainer ServiceContainer
	  {
		  get
		  {
			return serviceContainer;
		  }
	  }

	}

}