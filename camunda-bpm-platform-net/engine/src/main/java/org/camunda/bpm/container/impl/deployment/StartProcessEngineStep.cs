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
	using JmxManagedProcessEngine = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessEngine;
	using JmxManagedProcessEngineController = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessEngineController;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using StrongUuidGenerator = org.camunda.bpm.engine.impl.persistence.StrongUuidGenerator;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESS_APPLICATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// <para>Deployment operation step responsible for starting a managed process engine
	/// inside the runtime container.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartProcessEngineStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  /// <summary>
	  /// the process engine Xml configuration passed in as a parameter to the operation step </summary>
	  protected internal readonly ProcessEngineXml processEngineXml;

	  public StartProcessEngineStep(ProcessEngineXml processEngineXml)
	  {
		this.processEngineXml = processEngineXml;
	  }

	  public override string Name
	  {
		  get
		  {
			return "Start process engine " + processEngineXml.Name;
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);

		ClassLoader classLoader = null;

		if (processApplication != null)
		{
		  classLoader = processApplication.ProcessApplicationClassloader;
		}

		string configurationClassName = processEngineXml.ConfigurationClass;

		if (string.ReferenceEquals(configurationClassName, null) || configurationClassName.Length == 0)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  configurationClassName = typeof(StandaloneProcessEngineConfiguration).FullName;
		}

		// create & instantiate configuration class
		Type configurationClass = loadClass(configurationClassName, classLoader, typeof(ProcessEngineConfigurationImpl));
		ProcessEngineConfigurationImpl configuration = createInstance(configurationClass);

		// set UUid generator
		// TODO: move this to configuration and use as default?
		ProcessEngineConfigurationImpl configurationImpl = configuration;
		configurationImpl.IdGenerator = new StrongUuidGenerator();

		// set configuration values
		string name = processEngineXml.Name;
		configuration.ProcessEngineName = name;

		string datasourceJndiName = processEngineXml.Datasource;
		configuration.DataSourceJndiName = datasourceJndiName;

		// apply properties
		IDictionary<string, string> properties = processEngineXml.Properties;
		setJobExecutorActivate(configuration, properties);
		PropertyHelper.applyProperties(configuration, properties);

		// instantiate plugins:
		configurePlugins(configuration, processEngineXml, classLoader);

		if (!string.ReferenceEquals(processEngineXml.JobAcquisitionName, null) && processEngineXml.JobAcquisitionName.Length > 0)
		{
		  JobExecutor jobExecutor = getJobExecutorService(serviceContainer);
		  ensureNotNull("Cannot find referenced job executor with name '" + processEngineXml.JobAcquisitionName + "'", "jobExecutor", jobExecutor);

		  // set JobExecutor on process engine
		  configurationImpl.JobExecutor = jobExecutor;
		}

		// start the process engine inside the container.
		JmxManagedProcessEngine managedProcessEngineService = createProcessEngineControllerInstance(configuration);
		serviceContainer.startService(ServiceTypes.PROCESS_ENGINE, configuration.ProcessEngineName, managedProcessEngineService);

	  }

	  protected internal virtual void setJobExecutorActivate(ProcessEngineConfigurationImpl configuration, IDictionary<string, string> properties)
	  {
		// override job executor auto activate: set to true in shared engine scenario
		// if it is not specified (see #CAM-4817)
		configuration.JobExecutorActivate = true;
	  }

	  protected internal virtual JmxManagedProcessEngineController createProcessEngineControllerInstance(ProcessEngineConfigurationImpl configuration)
	  {
		return new JmxManagedProcessEngineController(configuration);
	  }

	  /// <summary>
	  /// <para>Instantiates and applies all <seealso cref="ProcessEnginePlugin"/>s defined in the processEngineXml
	  /// </para>
	  /// </summary>
	  protected internal virtual void configurePlugins(ProcessEngineConfigurationImpl configuration, ProcessEngineXml processEngineXml, ClassLoader classLoader)
	  {

		foreach (ProcessEnginePluginXml pluginXml in processEngineXml.Plugins)
		{
		  // create plugin instance
		  Type pluginClass = loadClass(pluginXml.PluginClass, classLoader, typeof(ProcessEnginePlugin));
		  ProcessEnginePlugin plugin = createInstance(pluginClass);

		  // apply configured properties
		  IDictionary<string, string> properties = pluginXml.Properties;
		  PropertyHelper.applyProperties(plugin, properties);

		  // add to configuration
		  configuration.ProcessEnginePlugins.Add(plugin);
		}

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.jobexecutor.JobExecutor getJobExecutorService(final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer)
	  protected internal virtual JobExecutor getJobExecutorService(PlatformServiceContainer serviceContainer)
	  {
		// lookup container managed job executor
		string jobAcquisitionName = processEngineXml.JobAcquisitionName;
		JobExecutor jobExecutor = serviceContainer.getServiceValue(ServiceTypes.JOB_EXECUTOR, jobAcquisitionName);
		return jobExecutor;
	  }

	  protected internal virtual T createInstance<T>(Type clazz)
	  {
		return ReflectUtil.instantiate(clazz);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T> Class loadClass(String className, ClassLoader customClassloader, Class<T> clazz)
	  protected internal virtual Type loadClass<T>(string className, ClassLoader customClassloader, Type clazz)
	  {
			  clazz = typeof(T);
		try
		{
		  if (customClassloader != null)
		  {
			return (Type) customClassloader.loadClass(className);
		  }
		  else
		  {
			return (Type) ReflectUtil.loadClass(className);
		  }
		}
		catch (ClassNotFoundException e)
		{
		  throw LOG.camnnotLoadConfigurationClass(className, e);
		}
		catch (System.InvalidCastException e)
		{
		  throw LOG.configurationClassHasWrongType(className, clazz, e);
		}
	  }

	}

}