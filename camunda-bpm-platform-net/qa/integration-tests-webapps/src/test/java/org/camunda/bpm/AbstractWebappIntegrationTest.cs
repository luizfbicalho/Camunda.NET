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
	using ClientConfig = com.sun.jersey.api.client.config.ClientConfig;
	using JSONConfiguration = com.sun.jersey.api.json.JSONConfiguration;
	using ApacheHttpClient4 = com.sun.jersey.client.apache4.ApacheHttpClient4;
	using DefaultApacheHttpClient4Config = com.sun.jersey.client.apache4.config.DefaultApacheHttpClient4Config;
	using DefaultHttpClient = org.apache.http.impl.client.DefaultHttpClient;
	using HttpConnectionParams = org.apache.http.@params.HttpConnectionParams;
	using HttpParams = org.apache.http.@params.HttpParams;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using BeforeClass = org.junit.BeforeClass;
	using ChromeDriverService = org.openqa.selenium.chrome.ChromeDriverService;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AbstractWebappIntegrationTest
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(AbstractWebappIntegrationTest).FullName);

	  public const string HOST_NAME = "localhost";
	  public string httpPort;
	  public string APP_BASE_PATH;

	  public ApacheHttpClient4 client;
	  public DefaultHttpClient defaultHttpClient;

	  protected internal TestProperties testProperties;
	  protected internal static ChromeDriverService service;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createClient() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void createClient()
	  {
		testProperties = new TestProperties();

		string applicationContextPath = ApplicationContextPath;
		APP_BASE_PATH = testProperties.getApplicationPath("/" + applicationContextPath);
		LOGGER.info("Connecting to application " + APP_BASE_PATH);

		ClientConfig clientConfig = new DefaultApacheHttpClient4Config();
		clientConfig.Features.put(JSONConfiguration.FEATURE_POJO_MAPPING, true);
		client = ApacheHttpClient4.create(clientConfig);

		defaultHttpClient = (DefaultHttpClient) client.ClientHandler.HttpClient;
		HttpParams @params = defaultHttpClient.Params;
		HttpConnectionParams.setConnectionTimeout(@params, 3 * 60 * 1000);
		HttpConnectionParams.setSoTimeout(@params, 10 * 60 * 1000);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void destroyClient()
	  public virtual void destroyClient()
	  {
		client.destroy();
	  }

	  /// <summary>
	  /// <para>subclasses must override this method and provide the local context path of the application they are testing.</para>
	  /// 
	  /// <para>Example: <code>cycle/</code></para>
	  /// </summary>
	  protected internal abstract string ApplicationContextPath {get;}

	}

}