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
namespace org.camunda.bpm.qa.performance.engine.junit
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthHelper
	{

	  public static T withAuthentication<T>(Callable<T> callable, ProcessEngine processEngine, string userId, params string[] groupIds)
	  {
		try
		{
		  processEngine.ProcessEngineConfiguration.AuthorizationEnabled = true;
		  processEngine.IdentityService.setAuthentication(userId, Arrays.asList(groupIds));

		  return callable.call();

		}
		catch (Exception t)
		{

		  if (t is Exception)
		  {
			throw (Exception) t;
		  }
		  else
		  {
			throw new Exception(t);
		  }

		}
		finally
		{
		  processEngine.IdentityService.clearAuthentication();
		  processEngine.ProcessEngineConfiguration.AuthorizationEnabled = false;
		}
	  }

	}

}