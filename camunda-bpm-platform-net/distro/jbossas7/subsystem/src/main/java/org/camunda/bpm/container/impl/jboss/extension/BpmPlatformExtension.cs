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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.SUBSYSTEM;

	using BpmPlatformSubsystemAdd = org.camunda.bpm.container.impl.jboss.extension.handler.BpmPlatformSubsystemAdd;
	using BpmPlatformSubsystemRemove = org.camunda.bpm.container.impl.jboss.extension.handler.BpmPlatformSubsystemRemove;
	using JobAcquisitionAdd = org.camunda.bpm.container.impl.jboss.extension.handler.JobAcquisitionAdd;
	using JobAcquisitionRemove = org.camunda.bpm.container.impl.jboss.extension.handler.JobAcquisitionRemove;
	using JobExecutorAdd = org.camunda.bpm.container.impl.jboss.extension.handler.JobExecutorAdd;
	using JobExecutorRemove = org.camunda.bpm.container.impl.jboss.extension.handler.JobExecutorRemove;
	using ProcessEngineAdd = org.camunda.bpm.container.impl.jboss.extension.handler.ProcessEngineAdd;
	using ProcessEngineRemove = org.camunda.bpm.container.impl.jboss.extension.handler.ProcessEngineRemove;
	using Extension = org.jboss.@as.controller.Extension;
	using ExtensionContext = org.jboss.@as.controller.ExtensionContext;
	using PathElement = org.jboss.@as.controller.PathElement;
	using ResourceBuilder = org.jboss.@as.controller.ResourceBuilder;
	using ResourceDefinition = org.jboss.@as.controller.ResourceDefinition;
	using SubsystemRegistration = org.jboss.@as.controller.SubsystemRegistration;
	using StandardResourceDescriptionResolver = org.jboss.@as.controller.descriptions.StandardResourceDescriptionResolver;
	using ExtensionParsingContext = org.jboss.@as.controller.parsing.ExtensionParsingContext;



	/// <summary>
	/// Defines the bpm-platform subsystem for jboss application server
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class BpmPlatformExtension : Extension
	{

	  public const int BPM_PLATFORM_SUBSYSTEM_MAJOR_VERSION = 1;
	  public const int BPM_PLATFORM_SUBSYSTEM_MINOR_VERSION = 1;

	  /// <summary>
	  /// The parser used for parsing our subsystem </summary>
	  private readonly BpmPlatformParser parser = new BpmPlatformParser();

	  public static readonly string RESOURCE_NAME = typeof(BpmPlatformExtension).Assembly.GetName().Name + ".LocalDescriptions";

	  private static readonly PathElement SUBSYSTEM_PATH = PathElement.pathElement(SUBSYSTEM, SUBSYSTEM_NAME);
	  private static readonly PathElement PROCESS_ENGINES_PATH = PathElement.pathElement(ModelConstants_Fields.PROCESS_ENGINES);
	  private static readonly PathElement JOB_EXECUTOR_PATH = PathElement.pathElement(ModelConstants_Fields.JOB_EXECUTOR);
	  private static readonly PathElement JOB_ACQUISTIONS_PATH = PathElement.pathElement(ModelConstants_Fields.JOB_ACQUISITIONS);


	  public virtual void initialize(ExtensionContext context)
	  {
		// Register the subsystem and operation handlers
		SubsystemRegistration subsystem = context.registerSubsystem(SUBSYSTEM_NAME, BPM_PLATFORM_SUBSYSTEM_MAJOR_VERSION, BPM_PLATFORM_SUBSYSTEM_MINOR_VERSION);
		subsystem.registerXMLElementWriter(parser);

		// build resource definitions

		ResourceBuilder processEnginesResource = ResourceBuilder.Factory.create(PROCESS_ENGINES_PATH, getResourceDescriptionResolver(ModelConstants_Fields.PROCESS_ENGINES)).setAddOperation(ProcessEngineAdd.INSTANCE).setRemoveOperation(ProcessEngineRemove.INSTANCE);

		ResourceBuilder jobAcquisitionResource = ResourceBuilder.Factory.create(JOB_ACQUISTIONS_PATH, getResourceDescriptionResolver(ModelConstants_Fields.JOB_ACQUISITIONS)).setAddOperation(JobAcquisitionAdd.INSTANCE).setRemoveOperation(JobAcquisitionRemove.INSTANCE);

		ResourceBuilder jobExecutorResource = ResourceBuilder.Factory.create(JOB_EXECUTOR_PATH, getResourceDescriptionResolver(ModelConstants_Fields.JOB_EXECUTOR)).setAddOperation(JobExecutorAdd.INSTANCE).setRemoveOperation(JobExecutorRemove.INSTANCE).pushChild(jobAcquisitionResource).pop();

		ResourceDefinition subsystemResource = ResourceBuilder.Factory.createSubsystemRoot(SUBSYSTEM_PATH, getResourceDescriptionResolver(SUBSYSTEM_NAME), BpmPlatformSubsystemAdd.INSTANCE, BpmPlatformSubsystemRemove.INSTANCE).pushChild(processEnginesResource).pop().pushChild(jobExecutorResource).pop().build();

		subsystem.registerSubsystemModel(subsystemResource);

	  }

	  public virtual void initializeParsers(ExtensionParsingContext context)
	  {
		context.setSubsystemXmlMapping(ModelConstants_Fields.SUBSYSTEM_NAME, Namespace.CAMUNDA_BPM_PLATFORM_1_1.UriString, parser);
	  }

	  public static StandardResourceDescriptionResolver getResourceDescriptionResolver(string keyPrefix)
	  {
		return new StandardResourceDescriptionResolver(keyPrefix, RESOURCE_NAME, typeof(BpmPlatformExtension).ClassLoader, true, true);
	  }

	}

}