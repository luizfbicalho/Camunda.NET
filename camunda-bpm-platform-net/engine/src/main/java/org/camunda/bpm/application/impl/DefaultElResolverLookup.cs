using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.application.impl
{

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using CompositeELResolver = org.camunda.bpm.engine.impl.javax.el.CompositeELResolver;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultElResolverLookup
	{

	  private static readonly ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

	  public static ELResolver lookupResolver(AbstractProcessApplication processApplication)
	  {

		ServiceLoader<ProcessApplicationElResolver> providers = ServiceLoader.load(typeof(ProcessApplicationElResolver));
		IList<ProcessApplicationElResolver> sortedProviders = new List<ProcessApplicationElResolver>();
		foreach (ProcessApplicationElResolver provider in providers)
		{
		  sortedProviders.Add(provider);
		}

		if (sortedProviders.Count == 0)
		{
		  return null;

		}
		else
		{
		  // sort providers first
		  sortedProviders.Sort(new org.camunda.bpm.application.ProcessApplicationElResolver_ProcessApplicationElResolverSorter());

		  // add all providers to a composite resolver
		  CompositeELResolver compositeResolver = new CompositeELResolver();
		  StringBuilder summary = new StringBuilder();
		  summary.Append(string.Format("ElResolvers found for Process Application {0}", processApplication.Name));

		  foreach (ProcessApplicationElResolver processApplicationElResolver in sortedProviders)
		  {
			ELResolver elResolver = processApplicationElResolver.getElResolver(processApplication);

			if (elResolver != null)
			{
			  compositeResolver.add(elResolver);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  summary.Append(string.Format("Class {0}", processApplicationElResolver.GetType().FullName));
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  LOG.noElResolverProvided(processApplication.Name, processApplicationElResolver.GetType().FullName);
			}
		  }

		  LOG.paElResolversDiscovered(summary.ToString());

		  return compositeResolver;
		}

	  }

	}

}