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
	using static org.camunda.bpm.container.impl.jboss.extension.ModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.ADD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.DESCRIPTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.OPERATION_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.REQUEST_PROPERTIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.REQUIRED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.TYPE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.VALUE_TYPE;

	using ManagedProcessEngineMetadata = org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata;
	using MscManagedProcessEngineController = org.camunda.bpm.container.impl.jboss.service.MscManagedProcessEngineController;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using AbstractAddStepHandler = org.jboss.@as.controller.AbstractAddStepHandler;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using PathAddress = org.jboss.@as.controller.PathAddress;
	using ServiceVerificationHandler = org.jboss.@as.controller.ServiceVerificationHandler;
	using DescriptionProvider = org.jboss.@as.controller.descriptions.DescriptionProvider;
	using ModelDescriptionConstants = org.jboss.@as.controller.descriptions.ModelDescriptionConstants;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ModelType = org.jboss.dmr.ModelType;
	using Property = org.jboss.dmr.Property;
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using ServiceName = org.jboss.msc.service.ServiceName;



	/// <summary>
	/// Provides the description and the implementation of the process-engine#add operation.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessEngineAdd : AbstractAddStepHandler, DescriptionProvider
	{

	  public static readonly ProcessEngineAdd INSTANCE = new ProcessEngineAdd();

	  public virtual ModelNode getModelDescription(Locale locale)
	  {
		ModelNode node = new ModelNode();
		node.get(DESCRIPTION).set("Adds a process engine");
		node.get(OPERATION_NAME).set(ADD);

		node.get(REQUEST_PROPERTIES, NAME, DESCRIPTION).set("Name of the process engine");
		node.get(REQUEST_PROPERTIES, NAME, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, NAME, REQUIRED).set(true);

		node.get(REQUEST_PROPERTIES, DATASOURCE, DESCRIPTION).set("Which datasource to use");
		node.get(REQUEST_PROPERTIES, DATASOURCE, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, DATASOURCE, REQUIRED).set(true);

		node.get(REQUEST_PROPERTIES, DEFAULT, DESCRIPTION).set("Should it be the default engine");
		node.get(REQUEST_PROPERTIES, DEFAULT, TYPE).set(ModelType.BOOLEAN);
		node.get(REQUEST_PROPERTIES, DEFAULT, REQUIRED).set(false);

		node.get(REQUEST_PROPERTIES, HISTORY_LEVEL, DESCRIPTION).set("Which history level to use");
		node.get(REQUEST_PROPERTIES, HISTORY_LEVEL, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, HISTORY_LEVEL, REQUIRED).set(false);

		// engine properties
		node.get(REQUEST_PROPERTIES, PROPERTIES, DESCRIPTION).set("Additional properties");
		node.get(REQUEST_PROPERTIES, PROPERTIES, TYPE).set(ModelType.OBJECT);
		node.get(REQUEST_PROPERTIES, PROPERTIES, VALUE_TYPE).set(ModelType.LIST);
		node.get(REQUEST_PROPERTIES, PROPERTIES, REQUIRED).set(false);

		node.get(REQUEST_PROPERTIES, CONFIGURATION, DESCRIPTION).set("Which configuration class to use");
		node.get(REQUEST_PROPERTIES, CONFIGURATION, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, CONFIGURATION, REQUIRED).set(false);

		// plugins
		node.get(REQUEST_PROPERTIES, PLUGINS, DESCRIPTION).set("Additional plugins for process engine");
		node.get(REQUEST_PROPERTIES, PLUGINS, TYPE).set(ModelType.LIST);
		node.get(REQUEST_PROPERTIES, PLUGINS, VALUE_TYPE).set(ModelType.OBJECT);
		node.get(REQUEST_PROPERTIES, PLUGINS, REQUIRED).set(false);

		return node;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void populateModel(org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void populateModel(ModelNode operation, ModelNode model)
	  {
		foreach (AttributeDefinition attr in SubsystemAttributeDefinitons.PROCESS_ENGINE_ATTRIBUTES)
		{
		  attr.validateAndSet(operation, model);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performRuntime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model, org.jboss.as.controller.ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController<?>> newControllers) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void performRuntime<T1>(OperationContext context, ModelNode operation, ModelNode model, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		string engineName = PathAddress.pathAddress(operation.get(ModelDescriptionConstants.ADDRESS)).LastElement.Value;

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
//ORIGINAL LINE: protected org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata transformConfiguration(final org.jboss.as.controller.OperationContext context, String engineName, final org.jboss.dmr.ModelNode model) throws IllegalArgumentException, org.jboss.as.controller.OperationFailedException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual ManagedProcessEngineMetadata transformConfiguration(OperationContext context, string engineName, ModelNode model)
	  {
		return new ManagedProcessEngineMetadata(SubsystemAttributeDefinitons.DEFAULT.resolveModelAttribute(context, model).asBoolean(), engineName, SubsystemAttributeDefinitons.DATASOURCE.resolveModelAttribute(context, model).asString(), SubsystemAttributeDefinitons.HISTORY_LEVEL.resolveModelAttribute(context, model).asString(), SubsystemAttributeDefinitons.CONFIGURATION.resolveModelAttribute(context, model).asString(), getPropertiesMap(SubsystemAttributeDefinitons.PROPERTIES.resolveModelAttribute(context, model)), getPlugins(SubsystemAttributeDefinitons.PLUGINS.resolveModelAttribute(context, model)));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml> getPlugins(final org.jboss.dmr.ModelNode plugins)
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
				return outerInstance.getPropertiesMap(plugin.get(Element.PROPERTIES.LocalName));
			  }
		  }
	  }

	  protected internal virtual IDictionary<string, string> getPropertiesMap(ModelNode properties)
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