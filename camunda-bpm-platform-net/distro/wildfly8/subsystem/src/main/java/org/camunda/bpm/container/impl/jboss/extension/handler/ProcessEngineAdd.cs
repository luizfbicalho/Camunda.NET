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
namespace org.camunda.bpm.container.impl.jboss.extension.handler
{
	using ManagedProcessEngineMetadata = org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata;
	using MscManagedProcessEngineController = org.camunda.bpm.container.impl.jboss.service.MscManagedProcessEngineController;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using org.jboss.@as.controller;
	using ModelNode = org.jboss.dmr.ModelNode;
	using Property = org.jboss.dmr.Property;
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using ServiceName = org.jboss.msc.service.ServiceName;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.ADDRESS;


	/// <summary>
	/// Provides the description and the implementation of the process-engine#add operation.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessEngineAdd : AbstractAddStepHandler
	{

	  public static readonly ProcessEngineAdd INSTANCE = new ProcessEngineAdd();

	  private ProcessEngineAdd() : base(SubsystemAttributeDefinitons.PROCESS_ENGINE_ATTRIBUTES)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performRuntime(OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model, ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController<?>> newControllers) throws OperationFailedException
	  protected internal override void performRuntime<T1>(OperationContext context, ModelNode operation, ModelNode model, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		string engineName = PathAddress.pathAddress(operation.get(ADDRESS)).LastElement.Value;

		ManagedProcessEngineMetadata processEngineConfiguration = transformConfiguration(context, engineName, model);

		ServiceController<ProcessEngine> controller = installService(context, verificationHandler, processEngineConfiguration);

		newControllers.Add(controller);
	  }

	  protected internal virtual ServiceController<ProcessEngine> installService(OperationContext context, ServiceVerificationHandler verificationHandler, ManagedProcessEngineMetadata processEngineConfiguration)
	  {

		MscManagedProcessEngineController service = new MscManagedProcessEngineController(processEngineConfiguration);
		ServiceName name = ServiceNames.forManagedProcessEngine(processEngineConfiguration.EngineName);

		ServiceBuilder<ProcessEngine> serviceBuilder = context.ServiceTarget.addService(name, service);

		MscManagedProcessEngineController.initializeServiceBuilder(processEngineConfiguration, service, serviceBuilder, processEngineConfiguration.JobExecutorAcquisitionName);

		serviceBuilder.addListener(verificationHandler);
		return serviceBuilder.install();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata transformConfiguration(final OperationContext context, String engineName, final org.jboss.dmr.ModelNode model) throws OperationFailedException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual ManagedProcessEngineMetadata transformConfiguration(OperationContext context, string engineName, ModelNode model)
	  {
		return new ManagedProcessEngineMetadata(SubsystemAttributeDefinitons.DEFAULT.resolveModelAttribute(context, model).asBoolean(), engineName, SubsystemAttributeDefinitons.DATASOURCE.resolveModelAttribute(context, model).asString(), SubsystemAttributeDefinitons.HISTORY_LEVEL.resolveModelAttribute(context, model).asString(), SubsystemAttributeDefinitons.CONFIGURATION.resolveModelAttribute(context, model).asString(), getProperties(SubsystemAttributeDefinitons.PROPERTIES.resolveModelAttribute(context, model)), getPlugins(SubsystemAttributeDefinitons.PLUGINS.resolveModelAttribute(context, model)));
	  }

	  protected internal virtual IList<ProcessEnginePluginXml> getPlugins(ModelNode plugins)
	  {
		IList<ProcessEnginePluginXml> pluginConfigurations = new List<ProcessEnginePluginXml>();

		if (plugins.Defined)
		{
		  foreach (ModelNode plugin in plugins.asList())
		  {
			ProcessEnginePluginXml processEnginePluginXml = new ProcessEnginePluginXmlAnonymousInnerClass(this);

			pluginConfigurations.Add(processEnginePluginXml);
		  }
		}

		return pluginConfigurations;
	  }

	  private class ProcessEnginePluginXmlAnonymousInnerClass : ProcessEnginePluginXml
	  {
		  private readonly ProcessEngineAdd outerInstance;

		  public ProcessEnginePluginXmlAnonymousInnerClass(ProcessEngineAdd outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public string PluginClass
		  {
			  get
			  {
				return plugin.get(Element.PLUGIN_CLASS.LocalName).asString();
			  }
		  }

		  public IDictionary<string, string> Properties
		  {
			  get
			  {
				return outerInstance.getProperties(plugin.get(Element.PROPERTIES.LocalName));
			  }
		  }
	  }

	  protected internal virtual IDictionary<string, string> getProperties(ModelNode properties)
	  {
		IDictionary<string, string> propertyMap = new Dictionary<string, string>();
		if (properties.Defined)
		{
		  foreach (Property property in properties.asPropertyList())
		  {
			propertyMap[property.Name] = property.Value.asString();
		  }
		}
		return propertyMap;
	  }

	}

}