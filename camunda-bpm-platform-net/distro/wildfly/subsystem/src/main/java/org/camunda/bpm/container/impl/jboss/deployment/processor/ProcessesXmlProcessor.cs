using System;
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
namespace org.camunda.bpm.container.impl.jboss.deployment.processor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.server.deployment.Attachments.MODULE;


	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ProcessesXmlParser = org.camunda.bpm.application.impl.metadata.ProcessesXmlParser;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using ProcessApplicationAttachments = org.camunda.bpm.container.impl.jboss.deployment.marker.ProcessApplicationAttachments;
	using ProcessesXmlWrapper = org.camunda.bpm.container.impl.jboss.util.ProcessesXmlWrapper;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ComponentDescription = org.jboss.@as.ee.component.ComponentDescription;
	using DeploymentPhaseContext = org.jboss.@as.server.deployment.DeploymentPhaseContext;
	using DeploymentUnit = org.jboss.@as.server.deployment.DeploymentUnit;
	using DeploymentUnitProcessingException = org.jboss.@as.server.deployment.DeploymentUnitProcessingException;
	using DeploymentUnitProcessor = org.jboss.@as.server.deployment.DeploymentUnitProcessor;
	using Module = org.jboss.modules.Module;
	using VFS = org.jboss.vfs.VFS;
	using VirtualFile = org.jboss.vfs.VirtualFile;


	/// <summary>
	/// <para>Detects and processes all <em>META-INF/processes.xml</em> files that are visible from the module
	/// classloader of the <seealso cref="DeploymentUnit"/>.</para>
	/// 
	/// <para>This is POST_MODULE so we can take into account module visibility in case of composite deployments
	/// (EARs)</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessesXmlProcessor : DeploymentUnitProcessor
	{

	  public const string PROCESSES_XML = "META-INF/processes.xml";

	  public const int PRIORITY = 0x0000; // this can happen ASAP in the POST_MODULE Phase

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deploy(org.jboss.as.server.deployment.DeploymentPhaseContext phaseContext) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  public virtual void deploy(DeploymentPhaseContext phaseContext)
	  {

		DeploymentUnit deploymentUnit = phaseContext.DeploymentUnit;

		if (!ProcessApplicationAttachments.isProcessApplication(deploymentUnit))
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.modules.Module module = deploymentUnit.getAttachment(MODULE);
		Module module = deploymentUnit.getAttachment(MODULE);

		// read @ProcessApplication annotation of PA-component
		string[] deploymentDescriptors = getDeploymentDescriptors(deploymentUnit);

		// load all processes.xml files
		IList<URL> deploymentDescriptorURLs = getDeploymentDescriptorUrls(module, deploymentDescriptors);

		foreach (URL processesXmlResource in deploymentDescriptorURLs)
		{
		  VirtualFile processesXmlFile = getFile(processesXmlResource);

		  // parse processes.xml metadata.
		  ProcessesXml processesXml = null;
		  if (isEmptyFile(processesXmlResource))
		  {
			processesXml = ProcessesXml.EMPTY_PROCESSES_XML;
		  }
		  else
		  {
			processesXml = parseProcessesXml(processesXmlResource);
		  }

		  // add the parsed metadata to the attachment list
		  ProcessApplicationAttachments.addProcessesXml(deploymentUnit, new ProcessesXmlWrapper(processesXml, processesXmlFile));
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.List<java.net.URL> getDeploymentDescriptorUrls(final org.jboss.modules.Module module, String[] deploymentDescriptors) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual IList<URL> getDeploymentDescriptorUrls(Module module, string[] deploymentDescriptors)
	  {
		IList<URL> deploymentDescriptorURLs = new List<URL>();
		foreach (string deploymentDescriptor in deploymentDescriptors)
		{
		  IEnumerator<URL> resources = null;
		  try
		  {
			resources = module.ClassLoader.getResources(deploymentDescriptor);
		  }
		  catch (IOException e)
		  {
			throw new DeploymentUnitProcessingException("Could not load processes.xml resource: ", e);
		  }
		  while (resources.MoveNext())
		  {
			deploymentDescriptorURLs.Add((URL) resources.Current);
		  }
		}
		return deploymentDescriptorURLs;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String[] getDeploymentDescriptors(org.jboss.as.server.deployment.DeploymentUnit deploymentUnit) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  protected internal virtual string[] getDeploymentDescriptors(DeploymentUnit deploymentUnit)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.ee.component.ComponentDescription processApplicationComponent = org.camunda.bpm.container.impl.jboss.deployment.marker.ProcessApplicationAttachments.getProcessApplicationComponent(deploymentUnit);
		ComponentDescription processApplicationComponent = ProcessApplicationAttachments.getProcessApplicationComponent(deploymentUnit);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String paClassName = processApplicationComponent.getComponentClassName();
		string paClassName = processApplicationComponent.ComponentClassName;

		string[] deploymentDescriptorResourceNames = null;

		Module module = deploymentUnit.getAttachment(MODULE);

		Type paClass = null;
		try
		{
		  paClass = (Type) module.ClassLoader.loadClass(paClassName);
		}
		catch (ClassNotFoundException)
		{
		  throw new DeploymentUnitProcessingException("Unable to load process application class '" + paClassName + "'.");
		}

		ProcessApplication annotation = paClass.getAnnotation(typeof(ProcessApplication));

		if (annotation == null)
		{
		  deploymentDescriptorResourceNames = new string[]{PROCESSES_XML};

		}
		else
		{
		  deploymentDescriptorResourceNames = annotation.deploymentDescriptors();

		}
		return deploymentDescriptorResourceNames;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.Iterator<java.net.URL> getProcessesXmlResources(org.jboss.modules.Module module, String[] deploymentDescriptors) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  protected internal virtual IEnumerator<URL> getProcessesXmlResources(Module module, string[] deploymentDescriptors)
	  {
		try
		{
		  return module.ClassLoader.getResources(PROCESSES_XML);
		}
		catch (IOException e)
		{
		  throw new DeploymentUnitProcessingException(e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.jboss.vfs.VirtualFile getFile(java.net.URL processesXmlResource) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  protected internal virtual VirtualFile getFile(URL processesXmlResource)
	  {
		try
		{
		  return VFS.getChild(processesXmlResource.toURI());
		}
		catch (Exception e)
		{
		  throw new DeploymentUnitProcessingException(e);
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
		  throw new ProcessEngineException("Could not open stream for " + url, e);

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

	  public virtual void undeploy(DeploymentUnit context)
	  {

	  }

	}

}