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
namespace org.camunda.bpm.engine.spring
{

	using ApplicationContext = org.springframework.context.ApplicationContext;
	using GenericXmlApplicationContext = org.springframework.context.support.GenericXmlApplicationContext;
	using UrlResource = org.springframework.core.io.UrlResource;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class SpringConfigurationHelper
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger log = Logger.getLogger(typeof(SpringConfigurationHelper).FullName);

	  public static ProcessEngine buildProcessEngine(URL resource)
	  {
		log.fine("==== BUILDING SPRING APPLICATION CONTEXT AND PROCESS ENGINE =========================================");

		ApplicationContext applicationContext = new GenericXmlApplicationContext(new UrlResource(resource));
		IDictionary<string, ProcessEngine> beansOfType = applicationContext.getBeansOfType(typeof(ProcessEngine));
		if ((beansOfType == null) || (beansOfType.Count == 0))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("no " + typeof(ProcessEngine).FullName + " defined in the application context " + resource.ToString());
		}

		ProcessEngine processEngine = beansOfType.Values.GetEnumerator().next();

		log.fine("==== SPRING PROCESS ENGINE CREATED ==================================================================");
		return processEngine;
	  }


	}

}