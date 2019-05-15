﻿using System;
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
namespace org.camunda.bpm.engine.rest.util.container
{


	public abstract class EmbeddedServerBootstrap
	{

	  protected internal const string PORT_PROPERTY = "rest.http.port";
	  protected internal const string ROOT_RESOURCE_PATH = "";
	  private const string PROPERTIES_FILE = "/testconfig.properties";

	  public abstract void start();

	  public abstract void stop();

	  protected internal virtual Properties readProperties()
	  {
		Stream propStream = null;
		Properties properties = new Properties();

		try
		{
		  propStream = typeof(AbstractRestServiceTest).getResourceAsStream(PROPERTIES_FILE);
		  properties.load(propStream);
		}
		catch (IOException e)
		{
		  throw new ServerBootstrapException(e);
		}
		finally
		{
		  try
		  {
			propStream.Close();
		  }
		  catch (IOException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
		}

		return properties;
	  }
	}

}