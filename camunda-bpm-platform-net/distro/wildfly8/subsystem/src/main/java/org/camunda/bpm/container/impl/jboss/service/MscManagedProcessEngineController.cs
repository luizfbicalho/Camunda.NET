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
namespace org.camunda.bpm.container.impl.jboss.service
{
	using ManagedJtaProcessEngineConfiguration = org.camunda.bpm.container.impl.jboss.config.ManagedJtaProcessEngineConfiguration;
	using ManagedProcessEngineMetadata = org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata;
	using JBossCompatibilityExtension = org.camunda.bpm.container.impl.jboss.util.JBossCompatibilityExtension;
	using Tccl = org.camunda.bpm.container.impl.jboss.util.Tccl;
	using Operation = org.camunda.bpm.container.impl.jboss.util.Tccl.Operation;
	using JmxManagedProcessEngineController = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessEngineController;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using JtaProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.JtaProcessEngineConfiguration;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using DataSourceReferenceFactoryService = org.jboss.@as.connector.subsystems.datasources.DataSourceReferenceFactoryService;
	using ContextNames = org.jboss.@as.naming.deployment.ContextNames;
	using Injector = org.jboss.msc.inject.Injector;
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;
	using InjectedValue = org.jboss.msc.value.InjectedValue;



	/// <summary>
	/// <para>Service responsible for starting / stopping a managed process engine inside the Msc</para>
	/// 
	/// <para>This service is used for managing process engines that are started / stopped
	/// through the application server management infrastructure.</para>
	/// 
	/// <para>This is the Msc counterpart of the <seealso cref="JmxManagedProcessEngineController"/></para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class MscManagedProcessEngineController : MscManagedProcessEngine
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(MscManagedProcessEngineController).FullName);

	  protected internal InjectedValue<ExecutorService> executorInjector = new InjectedValue<ExecutorService>();

	  // Injecting these values makes the MSC aware of our dependencies on these resources.
	  // This ensures that they are available when this service is started
	  protected internal readonly InjectedValue<TransactionManager> transactionManagerInjector = new InjectedValue<TransactionManager>();
	  protected internal readonly InjectedValue<DataSourceReferenceFactoryService> datasourceBinderServiceInjector = new InjectedValue<DataSourceReferenceFactoryService>();
	  protected internal readonly InjectedValue<MscRuntimeContainerJobExecutor> mscRuntimeContainerJobExecutorInjector = new InjectedValue<MscRuntimeContainerJobExecutor>();

	  protected internal ManagedProcessEngineMetadata processEngineMetadata;

	  protected internal JtaProcessEngineConfiguration processEngineConfiguration;

	  /// <summary>
	  /// Instantiate  the process engine controller for a process engine configuration.
	  /// 
	  /// </summary>
	  public MscManagedProcessEngineController(ManagedProcessEngineMetadata processEngineConfiguration)
	  {
		this.processEngineMetadata = processEngineConfiguration;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(final org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public override void start(StartContext context)
	  {
		context.asynchronous();
		executorInjector.Value.submit(() =>
		{
	try
	{
	  startInternal(context);
	  context.complete();

	}
	catch (StartException e)
	{
	  context.failed(e);

	}
	catch (Exception e)
	{
	  context.failed(new StartException(e));

	}
		});
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void stop(final org.jboss.msc.service.StopContext context)
	  public override void stop(StopContext context)
	  {
		stopInternal(context);
	  }

	  protected internal virtual void stopInternal(StopContext context)
	  {

		try
		{
		  base.stop(context);

		}
		finally
		{

		  try
		  {
			processEngine.close();

		  }
		  catch (Exception e)
		  {
			LOGGER.log(Level.SEVERE, "exception while closing process engine", e);

		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startInternal(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void startInternal(StartContext context)
	  {
		// setting the TCCL to the Classloader of this module.
		// this exploits a hack in MyBatis allowing it to use the TCCL to load the
		// mapping files from the process engine module
		Tccl.runUnderClassloader(new OperationAnonymousInnerClass(this)
	   , typeof(ProcessEngine).ClassLoader);

		// invoke super start behavior.
		base.start(context);
	  }

	  private class OperationAnonymousInnerClass : Tccl.Operation<Void>
	  {
		  private readonly MscManagedProcessEngineController outerInstance;

		  public OperationAnonymousInnerClass(MscManagedProcessEngineController outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void run()
		  {
			outerInstance.startProcessEngine();
			return null;
		  }

	  }

	  protected internal virtual void startProcessEngine()
	  {

		processEngineConfiguration = createProcessEngineConfiguration();

		// set the name for the process engine
		processEngineConfiguration.ProcessEngineName = processEngineMetadata.EngineName;

		// set the value for the history
		processEngineConfiguration.History = processEngineMetadata.HistoryLevel;

		// use the injected datasource
		processEngineConfiguration.DataSource = (DataSource) datasourceBinderServiceInjector.Value.Reference.Instance;

		// use the injected transaction manager
		processEngineConfiguration.TransactionManager = transactionManagerInjector.Value;

		// set auto schema update
		if (processEngineMetadata.AutoSchemaUpdate)
		{
		  processEngineConfiguration.DatabaseSchemaUpdate = ProcessEngineConfiguration.DB_SCHEMA_UPDATE_TRUE;
		}
		else
		{
		  processEngineConfiguration.DatabaseSchemaUpdate = "off";
		}

		// set db table prefix
		if (!string.ReferenceEquals(processEngineMetadata.DbTablePrefix, null))
		{
		  processEngineConfiguration.DatabaseTablePrefix = processEngineMetadata.DbTablePrefix;
		}

		// set job executor on process engine.
		MscRuntimeContainerJobExecutor mscRuntimeContainerJobExecutor = mscRuntimeContainerJobExecutorInjector.Value;
		processEngineConfiguration.JobExecutor = mscRuntimeContainerJobExecutor;

		PropertyHelper.applyProperties(processEngineConfiguration, processEngineMetadata.ConfigurationProperties);

		addProcessEnginePlugins(processEngineConfiguration);

		processEngine = processEngineConfiguration.buildProcessEngine();
	  }

	  protected internal virtual void addProcessEnginePlugins(JtaProcessEngineConfiguration processEngineConfiguration)
	  {
		// add process engine plugins:
		IList<ProcessEnginePluginXml> pluginConfigurations = processEngineMetadata.PluginConfigurations;

		foreach (ProcessEnginePluginXml pluginXml in pluginConfigurations)
		{
		  // create plugin instance
		  ProcessEnginePlugin plugin = null;
		  string pluginClassName = pluginXml.PluginClass;
		  try
		  {
			plugin = (ProcessEnginePlugin) createInstance(pluginClassName);
		  }
		  catch (System.InvalidCastException)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ProcessEngineException("Process engine plugin '" + pluginClassName + "' does not implement interface " + typeof(ProcessEnginePlugin).FullName + "'.");
		  }

		  // apply configured properties
		  IDictionary<string, string> properties = pluginXml.Properties;
		  PropertyHelper.applyProperties(plugin, properties);

		  // add to configuration
		  processEngineConfiguration.ProcessEnginePlugins.Add(plugin);
		}
	  }

	  protected internal virtual JtaProcessEngineConfiguration createProcessEngineConfiguration()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		string configurationClassName = typeof(ManagedJtaProcessEngineConfiguration).FullName;
		if (!string.ReferenceEquals(processEngineMetadata.Configuration, null) && processEngineMetadata.Configuration.Length > 0)
		{
		  configurationClassName = processEngineMetadata.Configuration;
		}

		object configurationObject = createInstance(configurationClassName);

		if (configurationObject is JtaProcessEngineConfiguration)
		{
		  return (JtaProcessEngineConfiguration) configurationObject;

		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("Configuration class '" + configurationClassName + "' " + "is not a subclass of " + typeof(JtaProcessEngineConfiguration).FullName);
		}

	  }

	  private object createInstance(string configurationClassName)
	  {
		try
		{
		  Type configurationClass = this.GetType().ClassLoader.loadClass(configurationClassName);
		  return System.Activator.CreateInstance(configurationClass);

		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Could not load '" + configurationClassName + "': the class must be visible from the camunda-wildfly8-subsystem module.", e);
		}
	  }

	  public virtual Injector<TransactionManager> TransactionManagerInjector
	  {
		  get
		  {
			return transactionManagerInjector;
		  }
	  }

	  public virtual Injector<DataSourceReferenceFactoryService> DatasourceBinderServiceInjector
	  {
		  get
		  {
			return datasourceBinderServiceInjector;
		  }
	  }

	  public virtual InjectedValue<MscRuntimeContainerJobExecutor> MscRuntimeContainerJobExecutorInjector
	  {
		  get
		  {
			return mscRuntimeContainerJobExecutorInjector;
		  }
	  }

	  public static void initializeServiceBuilder(ManagedProcessEngineMetadata processEngineConfiguration, MscManagedProcessEngineController service, ServiceBuilder<ProcessEngine> serviceBuilder, string jobExecutorName)
	  {

		ContextNames.BindInfo datasourceBindInfo = ContextNames.bindInfoFor(processEngineConfiguration.DatasourceJndiName);
		serviceBuilder.addDependency(ServiceName.JBOSS.append("txn").append("TransactionManager"), typeof(TransactionManager), service.TransactionManagerInjector).addDependency(datasourceBindInfo.BinderServiceName, typeof(DataSourceReferenceFactoryService), service.DatasourceBinderServiceInjector).addDependency(ServiceNames.forMscRuntimeContainerDelegate(), typeof(MscRuntimeContainerDelegate), service.RuntimeContainerDelegateInjector).addDependency(ServiceNames.forMscRuntimeContainerJobExecutorService(jobExecutorName), typeof(MscRuntimeContainerJobExecutor), service.MscRuntimeContainerJobExecutorInjector).addDependency(ServiceNames.forMscExecutorService()).InitialMode = Mode.ACTIVE;

		if (processEngineConfiguration.Default)
		{
		  serviceBuilder.addAliases(ServiceNames.forDefaultProcessEngine());
		}

		JBossCompatibilityExtension.addServerExecutorDependency(serviceBuilder, service.ExecutorInjector, false);

	  }

	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			return processEngine;
		  }
	  }

	  public virtual InjectedValue<ExecutorService> ExecutorInjector
	  {
		  get
		  {
			return executorInjector;
		  }
	  }

	  public virtual ManagedProcessEngineMetadata ProcessEngineMetadata
	  {
		  get
		  {
			return processEngineMetadata;
		  }
	  }
	}

}