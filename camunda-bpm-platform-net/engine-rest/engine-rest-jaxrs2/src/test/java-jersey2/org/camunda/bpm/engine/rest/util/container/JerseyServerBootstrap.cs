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
namespace org.camunda.bpm.engine.rest.util.container
{
	using HttpServer = org.glassfish.grizzly.http.server.HttpServer;
	using GrizzlyHttpServerFactory = org.glassfish.jersey.grizzly2.httpserver.GrizzlyHttpServerFactory;
	using ResourceConfig = org.glassfish.jersey.server.ResourceConfig;


	public class JerseyServerBootstrap : EmbeddedServerBootstrap
	{

	  private HttpServer server;

	  public JerseyServerBootstrap(Application application)
	  {
		setupServer(application);
	  }

	  public override void start()
	  {
		try
		{
		  server.start();
		}
		catch (IOException e)
		{
		  throw new ServerBootstrapException(e);
		}
	  }

	  private void setupServer(Application application)
	  {
		ResourceConfig rc = ResourceConfig.forApplication(application);

		Properties serverProperties = readProperties();
		int port = int.Parse(serverProperties.getProperty(PORT_PROPERTY));
		URI serverUri = UriBuilder.fromPath(ROOT_RESOURCE_PATH).scheme("http").host("localhost").port(port).build();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.glassfish.grizzly.http.server.HttpServer grizzlyServer = org.glassfish.jersey.grizzly2.httpserver.GrizzlyHttpServerFactory.createHttpServer(serverUri, rc);
		HttpServer grizzlyServer = GrizzlyHttpServerFactory.createHttpServer(serverUri, rc);
		try
		{
		  grizzlyServer.start();
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}

		server = grizzlyServer;

	  }

	  public override void stop()
	  {
		server.stop();
	  }
	}

}