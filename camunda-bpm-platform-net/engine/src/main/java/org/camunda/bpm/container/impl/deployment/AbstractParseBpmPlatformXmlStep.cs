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
namespace org.camunda.bpm.container.impl.deployment
{


	using BpmPlatformXmlParser = org.camunda.bpm.container.impl.metadata.BpmPlatformXmlParser;
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// <para>Deployment operation step responsible for parsing and attaching the bpm-platform.xml file.</para>
	/// 
	/// @author Daniel Meyer
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	public abstract class AbstractParseBpmPlatformXmlStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public const string BPM_PLATFORM_XML_FILE = "bpm-platform.xml";

	  public const string BPM_PLATFORM_XML_LOCATION = "bpm-platform-xml";
	  public const string BPM_PLATFORM_XML_ENVIRONMENT_VARIABLE = "BPM_PLATFORM_XML";
	  public const string BPM_PLATFORM_XML_SYSTEM_PROPERTY = "bpm.platform.xml";
	  public static readonly string BPM_PLATFORM_XML_RESOURCE_LOCATION = "META-INF/" + BPM_PLATFORM_XML_FILE;

	  public override string Name
	  {
		  get
		  {
			return "Parsing bpm-platform.xml file";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

		URL bpmPlatformXmlSource = getBpmPlatformXmlStream(operationContext);
		ensureNotNull("Unable to find bpm-platform.xml. This file is necessary for deploying the camunda BPM platform", "bpmPlatformXmlSource", bpmPlatformXmlSource);

		// parse the bpm platform xml
		BpmPlatformXml bpmPlatformXml = (new BpmPlatformXmlParser()).createParse().sourceUrl(bpmPlatformXmlSource).execute().BpmPlatformXml;

		// attach to operation context
		operationContext.addAttachment(Attachments.BPM_PLATFORM_XML, bpmPlatformXml);

	  }

	  public virtual URL checkValidBpmPlatformXmlResourceLocation(string url)
	  {
		url = autoCompleteUrl(url);

		URL fileLocation = null;

		try
		{
		  fileLocation = checkValidUrlLocation(url);
		  if (fileLocation == null)
		  {
			fileLocation = checkValidFileLocation(url);
		  }
		}
		catch (MalformedURLException e)
		{
		  throw new ProcessEngineException("'" + url + "' is not a valid camunda bpm platform configuration resource location.", e);
		}

		return fileLocation;
	  }

	  public virtual string autoCompleteUrl(string url)
	  {
		if (!string.ReferenceEquals(url, null))
		{
		  LOG.debugAutoCompleteUrl(url);

		  if (!url.EndsWith(BPM_PLATFORM_XML_FILE, StringComparison.Ordinal))
		  {
			string appender;
			if (url.Contains("/"))
			{
			  appender = "/";
			}
			else
			{
			  appender = "\\";
			}

			if (!(url.EndsWith("/", StringComparison.Ordinal) || url.EndsWith("\\\\", StringComparison.Ordinal)))
			{
			  url += appender;
			}

			url += BPM_PLATFORM_XML_FILE;
		  }

		  LOG.debugAutoCompletedUrl(url);
		}

		return url;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL checkValidUrlLocation(String url) throws java.net.MalformedURLException
	  public virtual URL checkValidUrlLocation(string url)
	  {
		if (string.ReferenceEquals(url, null) || url.Length == 0)
		{
		  return null;
		}

		Pattern urlPattern = Pattern.compile("^(https?://).*/bpm-platform\\.xml$", Pattern.CASE_INSENSITIVE | Pattern.UNICODE_CASE);
		Matcher urlMatcher = urlPattern.matcher(url);
		if (urlMatcher.matches())
		{
		  return new URL(url);
		}

		return null;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL checkValidFileLocation(String url) throws java.net.MalformedURLException
	  public virtual URL checkValidFileLocation(string url)
	  {
		if (string.ReferenceEquals(url, null) || url.Length == 0)
		{
		  return null;
		}

		Pattern filePattern = Pattern.compile("^(/|[A-z]://?|[A-z]:\\\\).*[/|\\\\]bpm-platform\\.xml$", Pattern.CASE_INSENSITIVE | Pattern.UNICODE_CASE);
		Matcher fileMatcher = filePattern.matcher(url);
		if (fileMatcher.matches())
		{
		  File configurationLocation = new File(url);

		  if (configurationLocation.Absolute && configurationLocation.exists())
		  {
			return configurationLocation.toURI().toURL();
		  }
		}

		return null;
	  }

	  public virtual URL lookupBpmPlatformXmlLocationFromJndi()
	  {
		string jndi = "java:comp/env/" + BPM_PLATFORM_XML_LOCATION;

		try
		{
		  string bpmPlatformXmlLocation = InitialContext.doLookup(jndi);

		  URL fileLocation = checkValidBpmPlatformXmlResourceLocation(bpmPlatformXmlLocation);

		  if (fileLocation != null)
		  {
			LOG.foundConfigJndi(jndi, fileLocation.ToString());
		  }

		  return fileLocation;
		}
		catch (NamingException e)
		{
		  LOG.debugExceptionWhileGettingConfigFromJndi(jndi, e);
		  return null;
		}
	  }

	  public virtual URL lookupBpmPlatformXmlLocationFromEnvironmentVariable()
	  {
		string bpmPlatformXmlLocation = Environment.GetEnvironmentVariable(BPM_PLATFORM_XML_ENVIRONMENT_VARIABLE);
		string logStatement = "environment variable [" + BPM_PLATFORM_XML_ENVIRONMENT_VARIABLE + "]";

		if (string.ReferenceEquals(bpmPlatformXmlLocation, null))
		{
		  bpmPlatformXmlLocation = System.getProperty(BPM_PLATFORM_XML_SYSTEM_PROPERTY);
		  logStatement = "system property [" + BPM_PLATFORM_XML_SYSTEM_PROPERTY + "]";
		}

		URL fileLocation = checkValidBpmPlatformXmlResourceLocation(bpmPlatformXmlLocation);

		if (fileLocation != null)
		{
		  LOG.foundConfigAtLocation(logStatement, fileLocation.ToString());
		}

		return fileLocation;
	  }

	  public virtual URL lookupBpmPlatformXmlFromClassPath(string resourceLocation)
	  {
		URL fileLocation = ClassLoaderUtil.getClassloader(this.GetType()).getResource(resourceLocation);

		if (fileLocation != null)
		{
		  LOG.foundConfigAtLocation(resourceLocation, fileLocation.ToString());
		}

		return fileLocation;
	  }

	  public virtual URL lookupBpmPlatformXmlFromClassPath()
	  {
		return lookupBpmPlatformXmlFromClassPath(BPM_PLATFORM_XML_RESOURCE_LOCATION);
	  }

	  public virtual URL lookupBpmPlatformXml()
	  {
		URL fileLocation = lookupBpmPlatformXmlLocationFromJndi();

		if (fileLocation == null)
		{
		  fileLocation = lookupBpmPlatformXmlLocationFromEnvironmentVariable();
		}

		if (fileLocation == null)
		{
		  fileLocation = lookupBpmPlatformXmlFromClassPath();
		}

		return fileLocation;
	  }

	  public abstract URL getBpmPlatformXmlStream(DeploymentOperation operationContext);

	}

}