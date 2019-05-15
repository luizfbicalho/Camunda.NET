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
namespace org.camunda.bpm.engine.rest.util.container
{
	using HttpServer = org.glassfish.grizzly.http.server.HttpServer;
	using GrizzlyHttpServerFactory = org.glassfish.jersey.grizzly2.httpserver.GrizzlyHttpServerFactory;
	using ResourceConfig = org.glassfish.jersey.server.ResourceConfig;
	using ServerProperties = org.glassfish.jersey.server.ServerProperties;


	public class JerseyServerBootstrap : EmbeddedServerBootstrap
	{

	  private HttpServer server;

	  public JerseyServerBootstrap()
	  {
		setupServer(new JaxrsApplication());
	  }

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

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties[ServerProperties.TRACING] = "ALL";
		rc.addProperties(properties);

		Properties serverProperties = readProperties();
		int port = int.Parse(serverProperties.getProperty(PORT_PROPERTY));
		URI serverUri = UriBuilder.fromPath(ROOT_RESOURCE_PATH).scheme("http").host("localhost").port(port).build();
		try
		{
		  server = GrizzlyHttpServerFactory.createHttpServer(serverUri, rc);
		}
		catch (System.ArgumentException e)
		{
		  throw new ServerBootstrapException(e);
		}
		catch (System.NullReferenceException e)
		{
		  throw new ServerBootstrapException(e);
		}
	  }

	  public override void stop()
	  {
		server.shutdownNow();
	  }
	}

}