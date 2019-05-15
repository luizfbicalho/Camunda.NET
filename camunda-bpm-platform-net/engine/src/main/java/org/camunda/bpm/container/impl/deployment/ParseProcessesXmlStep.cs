using System.Collections.Generic;
using System.IO;

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
	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ProcessesXmlParser = org.camunda.bpm.application.impl.metadata.ProcessesXmlParser;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESSES_XML_RESOURCES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESS_APPLICATION;


	/// <summary>
	/// <para>Detects and parses all META-INF/processes.xml files within the process application
	/// and attaches the parsed Metadata to the operation context.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ParseProcessesXmlStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public override string Name
	  {
		  get
		  {
			return "Parse processes.xml deployment descriptor files.";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);

		IDictionary<URL, ProcessesXml> parsedFiles = parseProcessesXmlFiles(processApplication);

		// attach parsed metadata
		operationContext.addAttachment(PROCESSES_XML_RESOURCES, parsedFiles);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.Map<java.net.URL, org.camunda.bpm.application.impl.metadata.spi.ProcessesXml> parseProcessesXmlFiles(final org.camunda.bpm.application.AbstractProcessApplication processApplication)
	  protected internal virtual IDictionary<URL, ProcessesXml> parseProcessesXmlFiles(AbstractProcessApplication processApplication)
	  {

		string[] deploymentDescriptors = getDeploymentDescriptorLocations(processApplication);
		IList<URL> processesXmlUrls = getProcessesXmlUrls(deploymentDescriptors, processApplication);

		IDictionary<URL, ProcessesXml> parsedFiles = new Dictionary<URL, ProcessesXml>();

		// perform parsing
		foreach (URL url in processesXmlUrls)
		{

		  LOG.foundProcessesXmlFile(url.ToString());

		  if (isEmptyFile(url))
		  {
			parsedFiles[url] = ProcessesXml.EMPTY_PROCESSES_XML;
			LOG.emptyProcessesXml();

		  }
		  else
		  {
			parsedFiles[url] = parseProcessesXml(url);
		  }
		}

		if (parsedFiles.Count == 0)
		{
		  LOG.noProcessesXmlForPa(processApplication.Name);
		}

		return parsedFiles;
	  }

	  protected internal virtual IList<URL> getProcessesXmlUrls(string[] deploymentDescriptors, AbstractProcessApplication processApplication)
	  {
		ClassLoader processApplicationClassloader = processApplication.ProcessApplicationClassloader;

		IList<URL> result = new List<URL>();

		// load all deployment descriptor files using the classloader of the process application
		foreach (string deploymentDescriptor in deploymentDescriptors)
		{

		  IEnumerator<URL> processesXmlFileLocations = null;
		  try
		  {
			processesXmlFileLocations = processApplicationClassloader.getResources(deploymentDescriptor);
		  }
		  catch (IOException e)
		  {
			throw LOG.exceptionWhileReadingProcessesXml(deploymentDescriptor, e);
		  }

		  while (processesXmlFileLocations.MoveNext())
		  {
			result.Add(processesXmlFileLocations.Current);
		  }

		}

		return result;
	  }

	  protected internal virtual string[] getDeploymentDescriptorLocations(AbstractProcessApplication processApplication)
	  {
		ProcessApplication annotation = processApplication.GetType().getAnnotation(typeof(ProcessApplication));
		if (annotation == null)
		{
		  return new string[] {ProcessApplication.DEFAULT_META_INF_PROCESSES_XML};

		}
		else
		{
		  return annotation.deploymentDescriptors();

		}
	  }

	  protected internal virtual bool isEmptyFile(URL url)
	  {

		Stream inputStream = null;

		try
		{
		  inputStream = url.openStream();
		  return inputStream.available() == 0;

		}
		catch (IOException e)
		{
		  throw LOG.exceptionWhileReadingProcessesXml(url.ToString(), e);
		}
		finally
		{
		  IoUtil.closeSilently(inputStream);

		}
	  }

	  protected internal virtual ProcessesXml parseProcessesXml(URL url)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.impl.metadata.ProcessesXmlParser processesXmlParser = new org.camunda.bpm.application.impl.metadata.ProcessesXmlParser();
		ProcessesXmlParser processesXmlParser = new ProcessesXmlParser();

		ProcessesXml processesXml = processesXmlParser.createParse().sourceUrl(url).execute().ProcessesXml;

		return processesXml;

	  }

	}

}