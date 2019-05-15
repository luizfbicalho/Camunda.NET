using System;
using System.Collections.Generic;
using System.IO;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.server.deployment.Attachments.MODULE;


	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using VfsProcessApplicationScanner = org.camunda.bpm.container.impl.deployment.scanning.VfsProcessApplicationScanner;
	using ProcessApplicationAttachments = org.camunda.bpm.container.impl.jboss.deployment.marker.ProcessApplicationAttachments;
	using MscManagedProcessApplication = org.camunda.bpm.container.impl.jboss.service.MscManagedProcessApplication;
	using ProcessApplicationDeploymentService = org.camunda.bpm.container.impl.jboss.service.ProcessApplicationDeploymentService;
	using ProcessApplicationStartService = org.camunda.bpm.container.impl.jboss.service.ProcessApplicationStartService;
	using ProcessApplicationStopService = org.camunda.bpm.container.impl.jboss.service.ProcessApplicationStopService;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using JBossCompatibilityExtension = org.camunda.bpm.container.impl.jboss.util.JBossCompatibilityExtension;
	using ProcessesXmlWrapper = org.camunda.bpm.container.impl.jboss.util.ProcessesXmlWrapper;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using ComponentDescription = org.jboss.@as.ee.component.ComponentDescription;
	using ComponentView = org.jboss.@as.ee.component.ComponentView;
	using ViewDescription = org.jboss.@as.ee.component.ViewDescription;
	using Attachments = org.jboss.@as.server.deployment.Attachments;
	using DeploymentPhaseContext = org.jboss.@as.server.deployment.DeploymentPhaseContext;
	using DeploymentUnit = org.jboss.@as.server.deployment.DeploymentUnit;
	using DeploymentUnitProcessingException = org.jboss.@as.server.deployment.DeploymentUnitProcessingException;
	using DeploymentUnitProcessor = org.jboss.@as.server.deployment.DeploymentUnitProcessor;
	using AnnotationInstance = org.jboss.jandex.AnnotationInstance;
	using Module = org.jboss.modules.Module;
	using ModuleClassLoader = org.jboss.modules.ModuleClassLoader;
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using ServiceRegistry = org.jboss.msc.service.ServiceRegistry;
	using VirtualFile = org.jboss.vfs.VirtualFile;


	/// <summary>
	/// <para>This processor installs the process application into the container.</para>
	/// 
	/// <para>First, we initialize the deployments for all process archives declared by the process application.
	/// It then registers a <seealso cref="ProcessApplicationDeploymentService"/> for each process archive to be deployed.
	/// Finally it registers the <seealso cref="MscManagedProcessApplication"/> service which depends on all the deployment services
	/// to have completed deployment</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationDeploymentProcessor : DeploymentUnitProcessor
	{

	  public const int PRIORITY = 0x0000; // this can happen at the beginning of the phase

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void deploy(org.jboss.as.server.deployment.DeploymentPhaseContext phaseContext) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  public override void deploy(DeploymentPhaseContext phaseContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.DeploymentUnit deploymentUnit = phaseContext.getDeploymentUnit();
		DeploymentUnit deploymentUnit = phaseContext.DeploymentUnit;

		if (!ProcessApplicationAttachments.isProcessApplication(deploymentUnit))
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.ee.component.ComponentDescription paComponent = getProcessApplicationComponent(deploymentUnit);
		ComponentDescription paComponent = getProcessApplicationComponent(deploymentUnit);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName paViewServiceName = getProcessApplicationViewServiceName(paComponent);
		ServiceName paViewServiceName = getProcessApplicationViewServiceName(paComponent);

		Module module = deploymentUnit.getAttachment(Attachments.MODULE);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String moduleName = module.getIdentifier().toString();
		string moduleName = module.Identifier.ToString();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName paStartServiceName = org.camunda.bpm.container.impl.jboss.service.ServiceNames.forProcessApplicationStartService(moduleName);
		ServiceName paStartServiceName = ServiceNames.forProcessApplicationStartService(moduleName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName paStopServiceName = org.camunda.bpm.container.impl.jboss.service.ServiceNames.forProcessApplicationStopService(moduleName);
		ServiceName paStopServiceName = ServiceNames.forProcessApplicationStopService(moduleName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceName noViewStartService = org.camunda.bpm.container.impl.jboss.service.ServiceNames.forNoViewProcessApplicationStartService(moduleName);
		ServiceName noViewStartService = ServiceNames.forNoViewProcessApplicationStartService(moduleName);

		IList<ServiceName> deploymentServiceNames = new List<ServiceName>();

		ProcessApplicationStopService paStopService = new ProcessApplicationStopService();
		ServiceBuilder<ProcessApplicationStopService> stopServiceBuilder = phaseContext.ServiceTarget.addService(paStopServiceName, paStopService).addDependency(phaseContext.PhaseServiceName).addDependency(ServiceNames.forBpmPlatformPlugins(), typeof(BpmPlatformPlugins), paStopService.PlatformPluginsInjector).setInitialMode(ServiceController.Mode.ACTIVE);

		if (paViewServiceName != null)
		{
		  stopServiceBuilder.addDependency(paViewServiceName, typeof(ComponentView), paStopService.PaComponentViewInjector);
		}
		else
		{
		  stopServiceBuilder.addDependency(noViewStartService, typeof(ProcessApplicationInterface), paStopService.NoViewProcessApplication);
		}

		stopServiceBuilder.install();

		// deploy all process archives
		IList<ProcessesXmlWrapper> processesXmlWrappers = ProcessApplicationAttachments.getProcessesXmls(deploymentUnit);
		foreach (ProcessesXmlWrapper processesXmlWrapper in processesXmlWrappers)
		{

		  ProcessesXml processesXml = processesXmlWrapper.ProcessesXml;
		  foreach (ProcessArchiveXml processArchive in processesXml.ProcessArchives)
		  {

			ServiceName processEngineServiceName = getProcessEngineServiceName(processArchive);
			IDictionary<string, sbyte[]> deploymentResources = getDeploymentResources(processArchive, deploymentUnit, processesXmlWrapper.ProcessesXmlFile);

			// add the deployment service for each process archive we deploy.
			ProcessApplicationDeploymentService deploymentService = new ProcessApplicationDeploymentService(deploymentResources, processArchive, module);
			string processArachiveName = processArchive.Name;
			if (string.ReferenceEquals(processArachiveName, null))
			{
			  // use random name for deployment service if name is null (we cannot ask the process application yet since the component might not be up.
			  processArachiveName = System.Guid.randomUUID().ToString();
			}
			ServiceName deploymentServiceName = ServiceNames.forProcessApplicationDeploymentService(deploymentUnit.Name, processArachiveName);
			ServiceBuilder<ProcessApplicationDeploymentService> serviceBuilder = phaseContext.ServiceTarget.addService(deploymentServiceName, deploymentService).addDependency(phaseContext.PhaseServiceName).addDependency(paStopServiceName).addDependency(processEngineServiceName, typeof(ProcessEngine), deploymentService.ProcessEngineInjector).setInitialMode(ServiceController.Mode.ACTIVE);

			if (paViewServiceName != null)
			{
			  // add a dependency on the component start service to make sure we are started after the pa-component (Singleton EJB) has started
			  serviceBuilder.addDependency(paComponent.StartServiceName);
			  serviceBuilder.addDependency(paViewServiceName, typeof(ComponentView), deploymentService.PaComponentViewInjector);
			}
			else
			{
			  serviceBuilder.addDependency(noViewStartService, typeof(ProcessApplicationInterface), deploymentService.NoViewProcessApplication);
			}

			JBossCompatibilityExtension.addServerExecutorDependency(serviceBuilder, deploymentService.ExecutorInjector, false);

			serviceBuilder.install();

			deploymentServiceNames.Add(deploymentServiceName);

		  }
		}

		AnnotationInstance postDeploy = ProcessApplicationAttachments.getPostDeployDescription(deploymentUnit);
		AnnotationInstance preUndeploy = ProcessApplicationAttachments.getPreUndeployDescription(deploymentUnit);

		// register the managed process application start service
		ProcessApplicationStartService paStartService = new ProcessApplicationStartService(deploymentServiceNames, postDeploy, preUndeploy, module);
		ServiceBuilder<ProcessApplicationStartService> serviceBuilder = phaseContext.ServiceTarget.addService(paStartServiceName, paStartService).addDependency(phaseContext.PhaseServiceName).addDependency(ServiceNames.forDefaultProcessEngine(), typeof(ProcessEngine), paStartService.DefaultProcessEngineInjector).addDependency(ServiceNames.forBpmPlatformPlugins(), typeof(BpmPlatformPlugins), paStartService.PlatformPluginsInjector).addDependencies(deploymentServiceNames).setInitialMode(ServiceController.Mode.ACTIVE);

		if (paViewServiceName != null)
		{
		  serviceBuilder.addDependency(paViewServiceName, typeof(ComponentView), paStartService.PaComponentViewInjector);
		}
		else
		{
		  serviceBuilder.addDependency(noViewStartService, typeof(ProcessApplicationInterface), paStartService.NoViewProcessApplication);
		}

		serviceBuilder.install();
	  }

	  public override void undeploy(DeploymentUnit deploymentUnit)
	  {

	  }

	  protected internal virtual ServiceName getProcessApplicationViewServiceName(ComponentDescription paComponent)
	  {
		ISet<ViewDescription> views = paComponent.Views;
		if (views == null || views.Count == 0)
		{
		  return null;
		}
		else
		{
		  ViewDescription next = views.GetEnumerator().next();
		  return next.ServiceName;
		}
	  }

	  protected internal virtual ComponentDescription getProcessApplicationComponent(DeploymentUnit deploymentUnit)
	  {
		ComponentDescription paComponentDescription = ProcessApplicationAttachments.getProcessApplicationComponent(deploymentUnit);
		return paComponentDescription;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.ProcessEngine getProcessEngineForArchive(org.jboss.msc.service.ServiceName serviceName, org.jboss.msc.service.ServiceRegistry serviceRegistry)
	  protected internal virtual ProcessEngine getProcessEngineForArchive(ServiceName serviceName, ServiceRegistry serviceRegistry)
	  {
		ServiceController<ProcessEngine> processEngineServiceController = (ServiceController<ProcessEngine>) serviceRegistry.getRequiredService(serviceName);
		return processEngineServiceController.Value;
	  }

	  protected internal virtual ServiceName getProcessEngineServiceName(ProcessArchiveXml processArchive)
	  {
		ServiceName serviceName = null;
		if (string.ReferenceEquals(processArchive.ProcessEngineName, null) || processArchive.ProcessEngineName.Length == 0)
		{
		  serviceName = ServiceNames.forDefaultProcessEngine();
		}
		else
		{
		  serviceName = ServiceNames.forManagedProcessEngine(processArchive.ProcessEngineName);
		}
		return serviceName;
	  }

	  protected internal virtual IDictionary<string, sbyte[]> getDeploymentResources(ProcessArchiveXml processArchive, DeploymentUnit deploymentUnit, VirtualFile processesXmlFile)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.modules.Module module = deploymentUnit.getAttachment(MODULE);
		Module module = deploymentUnit.getAttachment(MODULE);

		IDictionary<string, sbyte[]> resources = new Dictionary<string, sbyte[]>();

		// first, add all resources listed in the processe.xml
		IList<string> process = processArchive.ProcessResourceNames;
		ModuleClassLoader classLoader = module.ClassLoader;

		foreach (string resource in process)
		{
		  Stream inputStream = null;
		  try
		  {
			inputStream = classLoader.getResourceAsStream(resource);
			resources[resource] = IoUtil.readInputStream(inputStream, resource);
		  }
		  finally
		  {
			IoUtil.closeSilently(inputStream);
		  }
		}

		// scan for process definitions
		if (PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS, process.Count == 0))
		{

		  //always use VFS scanner on JBoss
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.deployment.scanning.VfsProcessApplicationScanner scanner = new org.camunda.bpm.container.impl.deployment.scanning.VfsProcessApplicationScanner();
		  VfsProcessApplicationScanner scanner = new VfsProcessApplicationScanner();

		  string resourceRootPath = processArchive.Properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_RESOURCE_ROOT_PATH];
		  string[] additionalResourceSuffixes = StringUtil.Split(processArchive.Properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_ADDITIONAL_RESOURCE_SUFFIXES], org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_ADDITIONAL_RESOURCE_SUFFIXES_SEPARATOR);
		  URL processesXmlUrl = vfsFileAsUrl(processesXmlFile);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  resources.putAll(scanner.findResources(classLoader, resourceRootPath, processesXmlUrl, additionalResourceSuffixes));
		}

		return resources;
	  }

	  protected internal virtual URL vfsFileAsUrl(VirtualFile processesXmlFile)
	  {
		try
		{
		  return processesXmlFile.toURL();
		}
		catch (MalformedURLException e)
		{
		  throw new Exception(e);
		}
	  }

	}

}