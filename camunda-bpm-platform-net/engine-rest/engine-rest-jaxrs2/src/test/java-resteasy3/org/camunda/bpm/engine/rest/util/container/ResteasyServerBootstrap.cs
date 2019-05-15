﻿/*
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
	using NettyJaxrsServer = org.jboss.resteasy.plugins.server.netty.NettyJaxrsServer;


	public class ResteasyServerBootstrap : EmbeddedServerBootstrap
	{

	  private NettyJaxrsServer server;

	  public ResteasyServerBootstrap(Application application)
	  {
		setupServer(application);
	  }

	  public override void start()
	  {
		server.start();
	  }

	  public override void stop()
	  {
		server.stop();
	  }

	  private void setupServer(Application application)
	  {
		Properties serverProperties = readProperties();
		int port = int.Parse(serverProperties.getProperty(PORT_PROPERTY));

		server = new NettyJaxrsServer();

		server.RootResourcePath = ROOT_RESOURCE_PATH;
		server.Port = port;

		server.Deployment.Application = application;
	  }

	}

}