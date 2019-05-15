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
namespace org.camunda.bpm.engine.rest.util
{
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using ProcessEngineProvider = org.camunda.bpm.engine.rest.spi.ProcessEngineProvider;


	public class EngineUtil
	{

	  /// <summary>
	  /// Look up the process engine from the <seealso cref="ProcessEngineProvider"/>. If engineName is null, the default engine is returned. </summary>
	  /// <param name="engineName">
	  /// @return </param>
	  public static ProcessEngine lookupProcessEngine(string engineName)
	  {

		ServiceLoader<ProcessEngineProvider> serviceLoader = ServiceLoader.load(typeof(ProcessEngineProvider));
		IEnumerator<ProcessEngineProvider> iterator = serviceLoader.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		if (iterator.hasNext())
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  ProcessEngineProvider provider = iterator.next();
		  if (string.ReferenceEquals(engineName, null))
		  {
			return provider.DefaultProcessEngine;
		  }
		  else
		  {
			return provider.getProcessEngine(engineName);
		  }
		}
		else
		{
		  throw new RestException(Status.INTERNAL_SERVER_ERROR, "Could not find an implementation of the " + typeof(ProcessEngineProvider) + "- SPI");
		}
	  }
	}

}