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
namespace org.camunda.bpm.engine.rest.impl
{
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using FetchAndLockHandler = org.camunda.bpm.engine.rest.spi.FetchAndLockHandler;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class FetchAndLockContextListener : ServletContextListener
	{

	  protected internal static FetchAndLockHandler fetchAndLockHandler;

	  public override void contextInitialized(ServletContextEvent sce)
	  {
		if (fetchAndLockHandler == null)
		{
		  fetchAndLockHandler = lookupFetchAndLockHandler();
		  fetchAndLockHandler.contextInitialized(sce);
		  fetchAndLockHandler.start();
		}
	  }

	  public override void contextDestroyed(ServletContextEvent sce)
	  {
		fetchAndLockHandler.shutdown();
	  }

	  public static FetchAndLockHandler FetchAndLockHandler
	  {
		  get
		  {
			return fetchAndLockHandler;
		  }
	  }

	  protected internal virtual FetchAndLockHandler lookupFetchAndLockHandler()
	  {
		ServiceLoader<FetchAndLockHandler> serviceLoader = ServiceLoader.load(typeof(FetchAndLockHandler));
		IEnumerator<FetchAndLockHandler> iterator = serviceLoader.GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		if (iterator.hasNext())
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  return iterator.next();
		}
		else
		{
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, "Could not find an implementation of the " + typeof(FetchAndLockHandler).Name + "- SPI");
		}
	  }

	}

}