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
namespace org.camunda.bpm.container.impl.tomcat.deployment
{
	using AbstractParseBpmPlatformXmlStep = org.camunda.bpm.container.impl.deployment.AbstractParseBpmPlatformXmlStep;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;


	/// <summary>
	/// <para>This deployment operation step is responsible for parsing and attaching the bpm-platform.xml file on tomcat.</para>
	/// 
	/// <para>We assume that the bpm-platform.xml file is located under <code>$CATALINA_HOME/conf/bpm-platform.xml</code>.</para>
	/// 
	/// @author Daniel Meyer
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	public class TomcatParseBpmPlatformXmlStep : AbstractParseBpmPlatformXmlStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public const string CATALINA_BASE = "catalina.base";
	  public const string CATALINA_HOME = "catalina.home";

	  public override URL getBpmPlatformXmlStream(DeploymentOperation operationcontext)
	  {
		URL fileLocation = lookupBpmPlatformXml();

		if (fileLocation == null)
		{
		  fileLocation = lookupBpmPlatformXmlFromCatalinaConfDirectory();
		}

		return fileLocation;
	  }

	  public virtual URL lookupBpmPlatformXmlFromCatalinaConfDirectory()
	  {
		// read file from CATALINA_BASE if set, otherwise CATALINA_HOME directory.
		string catalinaHome = System.getProperty(CATALINA_BASE);
		if (string.ReferenceEquals(catalinaHome, null))
		{
		  catalinaHome = System.getProperty(CATALINA_HOME);
		}

		string bpmPlatformFileLocation = catalinaHome + File.separator + "conf" + File.separator + BPM_PLATFORM_XML_FILE;

		try
		{
		  URL fileLocation = checkValidFileLocation(bpmPlatformFileLocation);

		  if (fileLocation != null)
		  {
			LOG.foundTomcatDeploymentDescriptor(bpmPlatformFileLocation, fileLocation.ToString());
		  }

		  return fileLocation;
		}
		catch (MalformedURLException e)
		{
		  throw LOG.invalidDeploymentDescriptorLocation(bpmPlatformFileLocation, e);
		}
	  }


	}

}