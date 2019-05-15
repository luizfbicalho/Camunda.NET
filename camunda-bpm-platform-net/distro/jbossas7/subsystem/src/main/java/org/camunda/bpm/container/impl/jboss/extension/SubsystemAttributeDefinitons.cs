using System;

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
namespace org.camunda.bpm.container.impl.jboss.extension
{
	using ManagedJtaProcessEngineConfiguration = org.camunda.bpm.container.impl.jboss.config.ManagedJtaProcessEngineConfiguration;
	using CustomMarshaller = org.camunda.bpm.container.impl.jboss.util.CustomMarshaller;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using ObjectListAttributeDefinition = org.jboss.@as.controller.ObjectListAttributeDefinition;
	using ObjectTypeAttributeDefinition = org.jboss.@as.controller.ObjectTypeAttributeDefinition;
	using SimpleAttributeDefinition = org.jboss.@as.controller.SimpleAttributeDefinition;
	using SimpleAttributeDefinitionBuilder = org.jboss.@as.controller.SimpleAttributeDefinitionBuilder;
	using SimpleMapAttributeDefinition = org.jboss.@as.controller.SimpleMapAttributeDefinition;
	using AttributeAccess = org.jboss.@as.controller.registry.AttributeAccess;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ModelType = org.jboss.dmr.ModelType;

	public class SubsystemAttributeDefinitons
	{

	  public const string DEFAULT_DATASOURCE = "java:jboss/datasources/ExampleDS";
	  public const string DEFAULT_HISTORY_LEVEL = "audit";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly string DEFAULT_PROCESS_ENGINE_CONFIGURATION_CLASS = typeof(ManagedJtaProcessEngineConfiguration).FullName;
	  public const string DEFAULT_ACQUISITION_STRATEGY = "SEQUENTIAL";
	  public const string DEFAULT_JOB_EXECUTOR_THREADPOOL_NAME = "job-executor-tp";

	  // general
	  public static readonly SimpleAttributeDefinition NAME = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.NAME, ModelType.STRING, false).setDefaultValue(new ModelNode("default")).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).build();
	  public static readonly SimpleMapAttributeDefinition PROPERTIES = new SimpleMapAttributeDefinition.Builder(ModelConstants_Fields.PROPERTIES, true).setAttributeMarshaller(CustomMarshaller.PROPERTIES_MARSHALLER).setRestartAllServices().setAllowExpression(true).build();

	  // process engine
	  public static readonly SimpleAttributeDefinition DEFAULT = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.DEFAULT, ModelType.BOOLEAN, true).setDefaultValue(new ModelNode(false)).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).setAllowExpression(true).build();
	  public static readonly AttributeDefinition DATASOURCE = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.DATASOURCE, ModelType.STRING, false).setDefaultValue(new ModelNode(DEFAULT_DATASOURCE)).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).setAllowExpression(true).build();
	  public static readonly AttributeDefinition HISTORY_LEVEL = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.HISTORY_LEVEL, ModelType.STRING, true).setDefaultValue(new ModelNode(DEFAULT_HISTORY_LEVEL)).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).setAllowExpression(true).build();
	  public static readonly AttributeDefinition CONFIGURATION = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.CONFIGURATION, ModelType.STRING, true).setDefaultValue(new ModelNode(DEFAULT_PROCESS_ENGINE_CONFIGURATION_CLASS)).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).setAllowExpression(true).build();

	  // job executor
	  public static readonly AttributeDefinition THREAD_POOL_NAME = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.THREAD_POOL_NAME, ModelType.STRING, true).setDefaultValue(new ModelNode(DEFAULT_JOB_EXECUTOR_THREADPOOL_NAME)).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).setAllowExpression(true).build();
	  [Obsolete]
	  public static readonly AttributeDefinition ACQUISITION_STRATEGY = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.ACQUISITION_STRATEGY, ModelType.STRING, true).setDefaultValue(new ModelNode(DEFAULT_ACQUISITION_STRATEGY)).setFlags(AttributeAccess.Flag.RESTART_ALL_SERVICES).setAllowExpression(true).build();

	  public static readonly SimpleAttributeDefinition PLUGIN_CLASS = new SimpleAttributeDefinitionBuilder(ModelConstants_Fields.PLUGIN_CLASS, ModelType.STRING, true).setAttributeMarshaller(CustomMarshaller.ATTRIBUTE_AS_ELEMENT).setAllowExpression(true).build();

	  public static readonly AttributeDefinition[] PLUGIN_ATTRIBUTES = new AttributeDefinition[] {PLUGIN_CLASS, PROPERTIES};

	  public static readonly ObjectTypeAttributeDefinition PLUGIN = ObjectTypeAttributeDefinition.Builder.of(ModelConstants_Fields.PLUGIN, PLUGIN_ATTRIBUTES).setAttributeMarshaller(CustomMarshaller.OBJECT_AS_ELEMENT).setRequires(ModelConstants_Fields.PLUGIN_CLASS).setAllowNull(true).setRestartAllServices().build();

	  public static readonly ObjectListAttributeDefinition PLUGINS = ObjectListAttributeDefinition.Builder.of(ModelConstants_Fields.PLUGINS, PLUGIN).setAttributeMarshaller(CustomMarshaller.OBJECT_LIST).setAllowNull(true).setAllowExpression(true).setRestartAllServices().build();

	  public static readonly AttributeDefinition[] JOB_EXECUTOR_ATTRIBUTES = new AttributeDefinition[] {THREAD_POOL_NAME};

	  public static readonly AttributeDefinition[] JOB_ACQUISITION_ATTRIBUTES = new AttributeDefinition[] {NAME, ACQUISITION_STRATEGY, PROPERTIES};

	  public static readonly AttributeDefinition[] PROCESS_ENGINE_ATTRIBUTES = new AttributeDefinition[] {NAME, DEFAULT, DATASOURCE, HISTORY_LEVEL, CONFIGURATION, PROPERTIES, PLUGINS};

	}

}