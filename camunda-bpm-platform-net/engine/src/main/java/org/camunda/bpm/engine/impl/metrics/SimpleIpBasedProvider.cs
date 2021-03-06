﻿using System;

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
namespace org.camunda.bpm.engine.impl.metrics
{


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SimpleIpBasedProvider : MetricsReporterIdProvider
	{

	  private static readonly MetricsLogger LOG = ProcessEngineLogger.METRICS_LOGGER;

	  public virtual string provideId(ProcessEngine processEngine)
	  {
		string localIp = "";
		try
		{
		  localIp = InetAddress.LocalHost.HostAddress;
		}
		catch (Exception e)
		{
		  // do not throw an exception; failure to determine an IP should not prevent from using the engine
		  LOG.couldNotDetermineIp(e);
		}

		return createId(localIp, processEngine.Name);
	  }

	  public static string createId(string ip, string engineName)
	  {
		return ip + "$" + engineName;
	  }
	}

}