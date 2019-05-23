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
namespace org.camunda.bpm.container.impl.metadata
{
	using static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants;

	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using Parse = org.camunda.bpm.engine.impl.util.xml.Parse;
	using Parser = org.camunda.bpm.engine.impl.util.xml.Parser;

	/// <summary>
	/// <para><seealso cref="Parse"/> implementation for Deployment Metadata.</para>
	/// 
	/// <para>This class is NOT Threadsafe</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class DeploymentMetadataParse : Parse
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public DeploymentMetadataParse(Parser parser) : base(parser)
	  {
	  }

	  public override Parse execute()
	  {
		base.execute();

		try
		{
		  parseRootElement();
		}
		catch (Exception e)
		{
		  throw LOG.unknownExceptionWhileParsingDeploymentDescriptor(e);
		}
		finally
		{
		  if (hasWarnings())
		  {
			logWarnings();
		  }
		  if (hasErrors())
		  {
			throwExceptionForErrors();
		  }
		}

		return this;
	  }

	  /// <summary>
	  /// to be overridden by subclasses.
	  /// </summary>
	  protected internal abstract void parseRootElement();

	  /// <summary>
	  /// parse a <code>&lt;process-engine .../&gt;</code> element and add it to the list of parsed elements
	  /// </summary>
	  protected internal virtual void parseProcessEngine(Element element, IList<ProcessEngineXml> parsedProcessEngines)
	  {

		ProcessEngineXmlImpl processEngine = new ProcessEngineXmlImpl();

		// set name
		processEngine.Name = element.attribute(NAME);

		// set default
		string defaultValue = element.attribute(DEFAULT);
		if (string.ReferenceEquals(defaultValue, null) || defaultValue.Length == 0)
		{
		  processEngine.Default = false;
		}
		else
		{
		  processEngine.Default = bool.Parse(defaultValue);
		}

		IDictionary<string, string> properties = new Dictionary<string, string>();
		IList<ProcessEnginePluginXml> plugins = new List<ProcessEnginePluginXml>();

		foreach (Element childElement in element.elements())
		{
		  if (CONFIGURATION.Equals(childElement.TagName))
		  {
			processEngine.ConfigurationClass = childElement.Text;

		  }
		  else if (DATASOURCE.Equals(childElement.TagName))
		  {
			processEngine.Datasource = childElement.Text;

		  }
		  else if (JOB_ACQUISITION.Equals(childElement.TagName))
		  {
			processEngine.JobAcquisitionName = childElement.Text;

		  }
		  else if (PROPERTIES.Equals(childElement.TagName))
		  {
			parseProperties(childElement, properties);

		  }
		  else if (PLUGINS.Equals(childElement.TagName))
		  {
			parseProcessEnginePlugins(childElement, plugins);

		  }
		}

		// set collected properties
		processEngine.Properties = properties;
		// set plugins
		processEngine.Plugins = plugins;
		// add the process engine to the list of parsed engines.
		parsedProcessEngines.Add(processEngine);

	  }

	  /// <summary>
	  /// Transform a <code>&lt;plugins ... /&gt;</code> structure.
	  /// </summary>
	  protected internal virtual void parseProcessEnginePlugins(Element element, IList<ProcessEnginePluginXml> plugins)
	  {
		foreach (Element chidElement in element.elements())
		{
		  if (PLUGIN.Equals(chidElement.TagName))
		  {
			parseProcessEnginePlugin(chidElement, plugins);
		  }
		}
	  }

	  /// <summary>
	  /// Transform a <code>&lt;plugin ... /&gt;</code> structure.
	  /// </summary>
	  protected internal virtual void parseProcessEnginePlugin(Element element, IList<ProcessEnginePluginXml> plugins)
	  {

		ProcessEnginePluginXmlImpl plugin = new ProcessEnginePluginXmlImpl();

		IDictionary<string, string> properties = new Dictionary<string, string>();

		foreach (Element childElement in element.elements())
		{
		  if (PLUGIN_CLASS.Equals(childElement.TagName))
		  {
			plugin.PluginClass = childElement.Text;

		  }
		  else if (PROPERTIES.Equals(childElement.TagName))
		  {
			parseProperties(childElement, properties);

		  }

		}

		plugin.Properties = properties;
		plugins.Add(plugin);
	  }

	  /// <summary>
	  /// Transform a
	  /// <pre>
	  /// &lt;properties&gt;
	  ///   &lt;property name="name"&gt;value&lt;/property&gt;
	  /// &lt;/properties&gt;
	  /// </pre>
	  /// structure into a properties <seealso cref="System.Collections.IDictionary"/>
	  /// 
	  /// Supports resolution of Ant-style placeholders against system properties.
	  /// 
	  /// </summary>
	  protected internal virtual void parseProperties(Element element, IDictionary<string, string> properties)
	  {

		foreach (Element childElement in element.elements())
		{
		  if (PROPERTY.Equals(childElement.TagName))
		  {
			string resolved = PropertyHelper.resolveProperty(System.Properties, childElement.Text);
			properties[childElement.attribute(NAME)] = resolved;
		  }
		}

	  }

	}

}