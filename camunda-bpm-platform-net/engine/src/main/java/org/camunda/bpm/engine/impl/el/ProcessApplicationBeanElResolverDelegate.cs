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
namespace org.camunda.bpm.engine.impl.el
{
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using BeanELResolver = org.camunda.bpm.engine.impl.javax.el.BeanELResolver;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;

	/// <summary>
	/// <para>Resolves a <seealso cref="BeanELResolver"/> from the current process application.
	/// This allows to cache resolvers on the process application level. Such a resolver
	/// cannot be cached globally as <seealso cref="BeanELResolver"/> keeps a cache of classes
	/// involved in expressions.</para>
	/// 
	/// <para>If resolution is attempted outside the context of a process application,
	/// then always a new resolver instance is returned (i.e. no caching in these cases).</para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class ProcessApplicationBeanElResolverDelegate : AbstractElResolverDelegate
	{

	  protected internal override ELResolver ElResolverDelegate
	  {
		  get
		  {
    
			ProcessApplicationReference processApplicationReference = Context.CurrentProcessApplication;
			if (processApplicationReference != null)
			{
    
			  try
			  {
				ProcessApplicationInterface processApplication = processApplicationReference.ProcessApplication;
				return processApplication.BeanElResolver;
    
			  }
			  catch (ProcessApplicationUnavailableException e)
			  {
				throw new ProcessEngineException("Cannot access process application '" + processApplicationReference.Name + "'", e);
			  }
    
			}
			else
			{
			  return new BeanELResolver();
			}
    
		  }
	  }
	}

}