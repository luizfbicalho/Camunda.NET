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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.JOB_ACQUISITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.JOB_EXECUTOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.JOB_EXECUTOR_CLASS_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROCESS_ENGINE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.metadata.DeploymentMetadataConstants.PROPERTIES;


	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using JobAcquisitionXml = org.camunda.bpm.container.impl.metadata.spi.JobAcquisitionXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using Parser = org.camunda.bpm.engine.impl.util.xml.Parser;

	/// <summary>
	/// <para>Parse implementation for parsing the <seealso cref="BpmPlatformXml"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmPlatformXmlParse : DeploymentMetadataParse
	{

	  /// <summary>
	  /// the parsed <seealso cref="BpmPlatformXml"/> </summary>
	  protected internal BpmPlatformXml bpmPlatformXml;

	  public BpmPlatformXmlParse(Parser parser) : base(parser)
	  {
	  }

	  public override BpmPlatformXmlParse execute()
	  {
		base.execute();
		return this;
	  }

	  /// <summary>
	  /// We know this is a <code>&lt;bpm-platform ../&gt;</code> element </summary>
	  protected internal override void parseRootElement()
	  {

		JobExecutorXmlImpl jobExecutor = new JobExecutorXmlImpl();
		IList<ProcessEngineXml> processEngines = new List<ProcessEngineXml>();

		foreach (Element element in rootElement.elements())
		{

		  if (JOB_EXECUTOR.Equals(element.TagName))
		  {
			parseJobExecutor(element, jobExecutor);

		  }
		  else if (PROCESS_ENGINE.Equals(element.TagName))
		  {
			parseProcessEngine(element, processEngines);

		  }

		}

		bpmPlatformXml = new BpmPlatformXmlImpl(jobExecutor, processEngines);
	  }

	  /// <summary>
	  /// parse a <code>&lt;job-executor .../&gt;</code> element and add it to the list of parsed elements
	  /// </summary>
	  protected internal virtual void parseJobExecutor(Element element, JobExecutorXmlImpl jobExecutorXml)
	  {

		IList<JobAcquisitionXml> jobAcquisitions = new List<JobAcquisitionXml>();
		IDictionary<string, string> properties = new Dictionary<string, string>();

		foreach (Element childElement in element.elements())
		{

		  if (JOB_ACQUISITION.Equals(childElement.TagName))
		  {
			parseJobAcquisition(childElement, jobAcquisitions);

		  }
		  else if (PROPERTIES.Equals(childElement.TagName))
		  {
			parseProperties(childElement, properties);
		  }

		}

		jobExecutorXml.JobAcquisitions = jobAcquisitions;
		jobExecutorXml.Properties = properties;

	  }

	  /// <summary>
	  /// parse a <code>&lt;job-acquisition .../&gt;</code> element and add it to the
	  /// list of parsed elements
	  /// </summary>
	  protected internal virtual void parseJobAcquisition(Element element, IList<JobAcquisitionXml> jobAcquisitions)
	  {

		JobAcquisitionXmlImpl jobAcquisition = new JobAcquisitionXmlImpl();

		// set name
		jobAcquisition.Name = element.attribute(NAME);

		IDictionary<string, string> properties = new Dictionary<string, string>();

		foreach (Element childElement in element.elements())
		{
		  if (JOB_EXECUTOR_CLASS_NAME.Equals(childElement.TagName))
		  {
			jobAcquisition.JobExecutorClassName = childElement.Text;

		  }
		  else if (PROPERTIES.Equals(childElement.TagName))
		  {
			parseProperties(childElement, properties);

		  }
		}

		// set collected properties
		jobAcquisition.Properties = properties;
		// add to list
		jobAcquisitions.Add(jobAcquisition);

	  }


	  public virtual BpmPlatformXml BpmPlatformXml
	  {
		  get
		  {
			return bpmPlatformXml;
		  }
	  }

	  public override BpmPlatformXmlParse sourceUrl(URL url)
	  {
		base.sourceUrl(url);
		return this;
	  }

	}

}