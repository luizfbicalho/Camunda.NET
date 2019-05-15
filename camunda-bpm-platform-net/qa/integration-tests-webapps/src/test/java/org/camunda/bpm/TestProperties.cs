using System;
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
namespace org.camunda.bpm
{

	/// <summary>
	/// @author drobisch
	/// </summary>
	public class TestProperties
	{

	  public const string TESTCONFIG_PROPERTIES_FILE = "/testconfig.properties";

	  private readonly Properties properties;
	  private readonly int defaultPort;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TestProperties() throws java.io.IOException
	  public TestProperties() : this(8080)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TestProperties(int defaultPort) throws java.io.IOException
	  public TestProperties(int defaultPort)
	  {

		this.defaultPort = defaultPort;

		properties = getTestProperties();
	  }

	  public virtual Properties Props
	  {
		  get
		  {
			return properties;
		  }
	  }

	  public virtual string getApplicationPath(string contextPath)
	  {
		return "http://" + HttpHost + ":" + HttpPort + contextPath;
	  }

	  public virtual int HttpPort
	  {
		  get
		  {
    
			try
			{
			  return int.Parse(properties.getProperty("http.port"));
			}
			catch (Exception)
			{
			  return defaultPort;
			}
		  }
	  }

	  public virtual string getStringProperty(string propName, string defaultValue)
	  {
		return properties.getProperty(propName, defaultValue);
	  }

	  public virtual string HttpHost
	  {
		  get
		  {
			return properties.getProperty("http.host", "localhost");
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.Properties getTestProperties() throws java.io.IOException
	  public static Properties getTestProperties()
	  {
		Properties properties = new Properties();

		Stream propertiesStream = null;
		try
		{
		  propertiesStream = typeof(TestProperties).getResourceAsStream(TESTCONFIG_PROPERTIES_FILE);
		  properties.load(propertiesStream);
		  string httpPort = (string) properties.get("http.port");
		}
		finally
		{
		  try
		  {
			if (propertiesStream != null)
			{
			  propertiesStream.Close();
			}
		  }
		  catch (Exception)
		  {
			// nop
		  }
		}

		return properties;
	  }
	}

}