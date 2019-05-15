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
namespace org.camunda.bpm.application.impl.metadata
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS_ARCHIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS_ENGINE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROPERTIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.RESOURCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.TENANT_ID;


	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using DeploymentMetadataParse = org.camunda.bpm.container.impl.metadata.DeploymentMetadataParse;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using Parse = org.camunda.bpm.engine.impl.util.xml.Parse;
	using Parser = org.camunda.bpm.engine.impl.util.xml.Parser;

	/// <summary>
	/// <para><seealso cref="Parse"/> object for the <code>processes.xml</code> file.</para>
	/// 
	/// <para>This class is NOT Threadsafe</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessesXmlParse : DeploymentMetadataParse
	{

	  /// <summary>
	  /// the constructed ProcessXml </summary>
	  protected internal ProcessesXml processesXml;

	  public ProcessesXmlParse(Parser parser) : base(parser)
	  {
	  }

	  public override ProcessesXmlParse execute()
	  {
		base.execute();
		return this;
	  }

	  /// <summary>
	  /// we know this is a <code>&lt;process-application ... /&gt;</code> structure.
	  /// </summary>
	  protected internal override void parseRootElement()
	  {

		IList<ProcessEngineXml> processEngines = new List<ProcessEngineXml>();
		IList<ProcessArchiveXml> processArchives = new List<ProcessArchiveXml>();

		foreach (Element element in rootElement.elements())
		{

		  if (PROCESS_ENGINE.Equals(element.TagName))
		  {
			parseProcessEngine(element, processEngines);

		  }
		  else if (PROCESS_ARCHIVE.Equals(element.TagName))
		  {
			parseProcessArchive(element, processArchives);

		  }

		}

		processesXml = new ProcessesXmlImpl(processEngines, processArchives);

	  }

	  /// <summary>
	  /// parse a <code>&lt;process-archive .../&gt;</code> element and add it to the list of parsed elements
	  /// </summary>
	  protected internal virtual void parseProcessArchive(Element element, IList<ProcessArchiveXml> parsedProcessArchives)
	  {

		ProcessArchiveXmlImpl processArchive = new ProcessArchiveXmlImpl();

		processArchive.Name = element.attribute(NAME);
		processArchive.TenantId = element.attribute(TENANT_ID);

		IList<string> processResourceNames = new List<string>();

		IDictionary<string, string> properties = new Dictionary<string, string>();
		foreach (Element childElement in element.elements())
		{
		  if (PROCESS_ENGINE.Equals(childElement.TagName))
		  {
			processArchive.ProcessEngineName = childElement.Text;

		  }
		  else if (PROCESS.Equals(childElement.TagName) || RESOURCE.Equals(childElement.TagName))
		  {
			processResourceNames.Add(childElement.Text);

		  }
		  else if (PROPERTIES.Equals(childElement.TagName))
		  {
			parseProperties(childElement, properties);

		  }
		}

		// set properties
		processArchive.Properties = properties;

		// add collected resource names.
		processArchive.ProcessResourceNames = processResourceNames;

		// add process archive to list of parsed archives.
		parsedProcessArchives.Add(processArchive);

	  }

	  public virtual ProcessesXml ProcessesXml
	  {
		  get
		  {
			return processesXml;
		  }
	  }

	  public override ProcessesXmlParse sourceUrl(URL url)
	  {
		base.sourceUrl(url);
		return this;
	  }

	}

}