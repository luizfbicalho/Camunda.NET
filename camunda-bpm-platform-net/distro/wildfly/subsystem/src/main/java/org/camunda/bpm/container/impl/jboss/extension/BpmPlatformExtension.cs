using System.Text;

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
	using BpmPlatformRootDefinition = org.camunda.bpm.container.impl.jboss.extension.resource.BpmPlatformRootDefinition;
	using Extension = org.jboss.@as.controller.Extension;
	using ExtensionContext = org.jboss.@as.controller.ExtensionContext;
	using PathElement = org.jboss.@as.controller.PathElement;
	using SubsystemRegistration = org.jboss.@as.controller.SubsystemRegistration;
	using StandardResourceDescriptionResolver = org.jboss.@as.controller.descriptions.StandardResourceDescriptionResolver;
	using ExtensionParsingContext = org.jboss.@as.controller.parsing.ExtensionParsingContext;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.SUBSYSTEM;



	/// <summary>
	/// Defines the bpm-platform subsystem for Wildfly 8+ application server
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class BpmPlatformExtension : Extension
	{

	  public const int BPM_PLATFORM_SUBSYSTEM_MAJOR_VERSION = 1;
	  public const int BPM_PLATFORM_SUBSYSTEM_MINOR_VERSION = 1;

	  /// <summary>
	  /// The parser used for parsing our subsystem </summary>
	  private readonly BpmPlatformParser1_1.BpmPlatformSubsystemParser parser = BpmPlatformParser1_1.BpmPlatformSubsystemParser.INSTANCE;

	  public static readonly string RESOURCE_NAME = typeof(BpmPlatformExtension).Assembly.GetName().Name + ".LocalDescriptions";

	  /// <summary>
	  /// Path elements for the resources offered by the subsystem.
	  /// </summary>
	  public static readonly PathElement SUBSYSTEM_PATH = PathElement.pathElement(SUBSYSTEM, SUBSYSTEM_NAME);
	  public static readonly PathElement PROCESS_ENGINES_PATH = PathElement.pathElement(ModelConstants_Fields.PROCESS_ENGINES);
	  public static readonly PathElement JOB_EXECUTOR_PATH = PathElement.pathElement(ModelConstants_Fields.JOB_EXECUTOR);
	  public static readonly PathElement JOB_ACQUISTIONS_PATH = PathElement.pathElement(ModelConstants_Fields.JOB_ACQUISITIONS);

	  public override void initialize(ExtensionContext context)
	  {
		SubsystemRegistration subsystem = context.registerSubsystem(SUBSYSTEM_NAME, BPM_PLATFORM_SUBSYSTEM_MAJOR_VERSION, BPM_PLATFORM_SUBSYSTEM_MINOR_VERSION);
		subsystem.registerSubsystemModel(BpmPlatformRootDefinition.INSTANCE);
		subsystem.registerXMLElementWriter(parser);
	  }

	  public override void initializeParsers(ExtensionParsingContext context)
	  {
		context.setSubsystemXmlMapping(ModelConstants_Fields.SUBSYSTEM_NAME, Namespace.CAMUNDA_BPM_PLATFORM_1_1.UriString, parser);
	  }

	  /// <summary>
	  /// Resolve the descriptions of the resources from the 'LocalDescriptions.properties' file.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static org.jboss.as.controller.descriptions.StandardResourceDescriptionResolver getResourceDescriptionResolver(final String... keyPrefix)
	  public static StandardResourceDescriptionResolver getResourceDescriptionResolver(params string[] keyPrefix)
	  {
		StringBuilder prefix = new StringBuilder(SUBSYSTEM_NAME);
		foreach (string kp in keyPrefix)
		{
		  prefix.Append('.').Append(kp);
		}
		return new StandardResourceDescriptionResolver(prefix.ToString(), RESOURCE_NAME, typeof(BpmPlatformExtension).ClassLoader, true, false);
	  }

	}

}