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
namespace org.camunda.bpm.container.impl.jmx.deployment
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.AbstractParseBpmPlatformXmlStep.BPM_PLATFORM_XML_FILE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.AbstractParseBpmPlatformXmlStep.BPM_PLATFORM_XML_LOCATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.AbstractParseBpmPlatformXmlStep.BPM_PLATFORM_XML_SYSTEM_PROPERTY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.tomcat.deployment.TomcatParseBpmPlatformXmlStep.CATALINA_HOME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;



	using TomcatParseBpmPlatformXmlStep = org.camunda.bpm.container.impl.tomcat.deployment.TomcatParseBpmPlatformXmlStep;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using SimpleNamingContext = org.springframework.mock.jndi.SimpleNamingContext;

	/// <summary>
	/// Checks the correct retrieval of bpm-platform.xml file through JNDI,
	/// environment variable, classpath and Tomcat's conf directory.
	/// 
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	public class BpmPlatformXmlLocationTest
	{

	  private static readonly string BPM_PLATFORM_XML_LOCATION_PARENT_DIR = BpmPlatformXmlLocationParentDir;
	  private static readonly string BPM_PLATFORM_XML_LOCATION_ABSOLUTE_DIR = BPM_PLATFORM_XML_LOCATION_PARENT_DIR + File.separator + "conf";
	  private static readonly string BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION = BPM_PLATFORM_XML_LOCATION_ABSOLUTE_DIR + File.separator + BPM_PLATFORM_XML_FILE;

	  private const string BPM_PLATFORM_XML_LOCATION_RELATIVE_PATH = "home/hawky4s/.camunda";

	  private static readonly string BPM_PLATFORM_XML_LOCATION_VALID_PATH_UNIX = "/" + BPM_PLATFORM_XML_LOCATION_RELATIVE_PATH;
	  private const string BPM_PLATFORM_XML_LOCATION_VALID_PATH_WINDOWS = "C:\\users\\hawky4s\\.camunda";

	  private static readonly string BPM_PLATFORM_XML_LOCATION_FILE_INVALID_PATH_UNIX = "C:" + File.separator + BPM_PLATFORM_XML_FILE;
	  private static readonly string BPM_PLATFORM_XML_LOCATION_FILE_INVALID_PATH_WINDOWS = "C://users//hawky4s//.camunda//" + BPM_PLATFORM_XML_FILE;

	  private static readonly string BPM_PLATFORM_XML_LOCATION_URL_HTTP_PROTOCOL = "http://localhost:8080/camunda/" + BPM_PLATFORM_XML_FILE;
	  private static readonly string BPM_PLATFORM_XML_LOCATION_URL_HTTPS_PROTOCOL = "https://localhost:8080/camunda/" + BPM_PLATFORM_XML_FILE;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public MockInitialContextRule initialContextRule = new MockInitialContextRule(new org.springframework.mock.jndi.SimpleNamingContext());
	  public MockInitialContextRule initialContextRule = new MockInitialContextRule(new SimpleNamingContext());

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void checkValidBpmPlatformXmlResourceLocationForUrl() throws javax.naming.NamingException, java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void checkValidBpmPlatformXmlResourceLocationForUrl()
	  {
		TomcatParseBpmPlatformXmlStep tomcatParseBpmPlatformXmlStep = new TomcatParseBpmPlatformXmlStep();

		assertNull(tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION));
		assertNull(tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_LOCATION_FILE_INVALID_PATH_WINDOWS));
		assertNull(tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_LOCATION_FILE_INVALID_PATH_UNIX));
		assertNull(tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_LOCATION_VALID_PATH_WINDOWS));
		assertNull(tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_LOCATION_VALID_PATH_UNIX));

		URL httpUrl = tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_LOCATION_URL_HTTP_PROTOCOL);
		assertEquals(BPM_PLATFORM_XML_LOCATION_URL_HTTP_PROTOCOL, httpUrl.ToString());
		URL httpsUrl = tomcatParseBpmPlatformXmlStep.checkValidUrlLocation(BPM_PLATFORM_XML_LOCATION_URL_HTTPS_PROTOCOL);
		assertEquals(BPM_PLATFORM_XML_LOCATION_URL_HTTPS_PROTOCOL, httpsUrl.ToString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void checkValidBpmPlatformXmlResourceLocationForFile() throws javax.naming.NamingException, java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void checkValidBpmPlatformXmlResourceLocationForFile()
	  {
		TomcatParseBpmPlatformXmlStep tomcatParseBpmPlatformXmlStep = new TomcatParseBpmPlatformXmlStep();

		URL url = tomcatParseBpmPlatformXmlStep.checkValidFileLocation(BPM_PLATFORM_XML_LOCATION_RELATIVE_PATH);
		assertNull("Relative path is invalid.", url);

		url = tomcatParseBpmPlatformXmlStep.checkValidFileLocation(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION);
		assertEquals((new File(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION)).toURI().toURL(), url);

		url = tomcatParseBpmPlatformXmlStep.checkValidFileLocation(BPM_PLATFORM_XML_LOCATION_FILE_INVALID_PATH_WINDOWS);
		assertNull("Path is invalid.", url);

		assertNull(tomcatParseBpmPlatformXmlStep.checkValidFileLocation(BPM_PLATFORM_XML_LOCATION_URL_HTTP_PROTOCOL));
		assertNull(tomcatParseBpmPlatformXmlStep.checkValidFileLocation(BPM_PLATFORM_XML_LOCATION_URL_HTTPS_PROTOCOL));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void checkUrlAutoCompletion() throws javax.naming.NamingException, java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void checkUrlAutoCompletion()
	  {
		TomcatParseBpmPlatformXmlStep tomcatParseBpmPlatformXmlStep = new TomcatParseBpmPlatformXmlStep();

		string correctedUrl = tomcatParseBpmPlatformXmlStep.autoCompleteUrl(BPM_PLATFORM_XML_LOCATION_VALID_PATH_UNIX);
		assertEquals(BPM_PLATFORM_XML_LOCATION_VALID_PATH_UNIX + "/" + BPM_PLATFORM_XML_FILE, correctedUrl);

		correctedUrl = tomcatParseBpmPlatformXmlStep.autoCompleteUrl(BPM_PLATFORM_XML_LOCATION_VALID_PATH_UNIX + "/");
		assertEquals(BPM_PLATFORM_XML_LOCATION_VALID_PATH_UNIX + "/" + BPM_PLATFORM_XML_FILE, correctedUrl);

		correctedUrl = tomcatParseBpmPlatformXmlStep.autoCompleteUrl(BPM_PLATFORM_XML_LOCATION_VALID_PATH_WINDOWS);
		assertEquals(BPM_PLATFORM_XML_LOCATION_VALID_PATH_WINDOWS + "\\" + BPM_PLATFORM_XML_FILE, correctedUrl);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void checkValidBpmPlatformXmlResourceLocation() throws javax.naming.NamingException, java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void checkValidBpmPlatformXmlResourceLocation()
	  {
		URL url = (new TomcatParseBpmPlatformXmlStep()).checkValidBpmPlatformXmlResourceLocation(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION);
		assertEquals((new File(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION)).toURI().toURL(), url);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmPlatformXmlLocationFromJndi() throws javax.naming.NamingException, java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void getBpmPlatformXmlLocationFromJndi()
	  {
		Context context = new InitialContext();
		context.bind("java:comp/env/" + BPM_PLATFORM_XML_LOCATION, BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION);

		URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXmlLocationFromJndi();

		assertEquals((new File(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION)).toURI().toURL(), url);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void bpmPlatformXmlLocationNotRegisteredInJndi() throws javax.naming.NamingException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void bpmPlatformXmlLocationNotRegisteredInJndi()
	  {
		URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXmlLocationFromJndi();
		assertNull(url);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmPlatformXmlFromEnvironmentVariableAsUrlLocation()
	  public virtual void getBpmPlatformXmlFromEnvironmentVariableAsUrlLocation()
	  {
		try
		{
		  System.setProperty(BPM_PLATFORM_XML_SYSTEM_PROPERTY, BPM_PLATFORM_XML_LOCATION_URL_HTTP_PROTOCOL);

		  URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXmlLocationFromEnvironmentVariable();

		  assertEquals(BPM_PLATFORM_XML_LOCATION_URL_HTTP_PROTOCOL, url.ToString());
		}
		finally
		{
		  System.clearProperty(BPM_PLATFORM_XML_SYSTEM_PROPERTY);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmPlatformXmlFromSystemPropertyAsFileLocation() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void getBpmPlatformXmlFromSystemPropertyAsFileLocation()
	  {
		try
		{
		  System.setProperty(BPM_PLATFORM_XML_SYSTEM_PROPERTY, BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION);

		  URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXmlLocationFromEnvironmentVariable();

		  assertEquals((new File(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION)).toURI().toURL(), url);
		}
		finally
		{
		  System.clearProperty(BPM_PLATFORM_XML_SYSTEM_PROPERTY);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmPlatformXmlFromClasspath()
	  public virtual void getBpmPlatformXmlFromClasspath()
	  {
		string classPathResourceLocation = typeof(BpmPlatformXmlLocationTest).Assembly.GetName().Name.replace(".", "/") + "/conf/" + BPM_PLATFORM_XML_FILE;

		URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXmlFromClassPath(classPathResourceLocation);
		assertNotNull("Url should point to a bpm-platform.xml file.", url);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getBpmPlatformXmlFromCatalinaConfDirectory() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void getBpmPlatformXmlFromCatalinaConfDirectory()
	  {
		System.setProperty(CATALINA_HOME, BPM_PLATFORM_XML_LOCATION_PARENT_DIR);

		try
		{
		  URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXmlFromCatalinaConfDirectory();

		  assertEquals((new File(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION)).toURI().toURL(), url);
		}
		finally
		{
		  System.clearProperty(CATALINA_HOME);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void lookupBpmPlatformXml() throws javax.naming.NamingException, java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void lookupBpmPlatformXml()
	  {
		Context context = new InitialContext();
		context.bind("java:comp/env/" + BPM_PLATFORM_XML_LOCATION, BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION);

		URL url = (new TomcatParseBpmPlatformXmlStep()).lookupBpmPlatformXml();

		assertEquals((new File(BPM_PLATFORM_XML_FILE_ABSOLUTE_LOCATION)).toURI().toURL(), url);
	  }

	  private static string BpmPlatformXmlLocationParentDir
	  {
		  get
		  {
			string baseDir = typeof(BpmPlatformXmlLocationTest).ProtectionDomain.CodeSource.Location.File;
			try
			{
			  // replace escaped whitespaces in path
			  baseDir = URLDecoder.decode(baseDir, "UTF-8");
			}
			catch (UnsupportedEncodingException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
			return baseDir + typeof(BpmPlatformXmlLocationTest).Assembly.GetName().Name.replace(".", File.separator);
		  }
	  }

	}

}