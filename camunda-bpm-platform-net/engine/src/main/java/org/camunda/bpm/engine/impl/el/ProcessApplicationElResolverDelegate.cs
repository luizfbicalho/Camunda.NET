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
	using CompositeELResolver = org.camunda.bpm.engine.impl.javax.el.CompositeELResolver;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;

	/// <summary>
	/// <para>This is an <seealso cref="ELResolver"/> implementation that delegates to a ProcessApplication-provided
	/// <seealso cref="ELResolver"/>. The idea is that in a multi-application setup, a shared process engine may orchestrate
	/// multiple process applications. In this setting we want to delegate to the current process application
	/// for performing expression resolving. This also allows individual process applications to integrate with
	/// different kinds of Di Containers or other expression-context providing frameworks. For instance, a first
	/// process application may use the spring application context for resolving Java Delegate implementations
	/// while a second application may use CDI or even an Apache Camel Context.</para>
	/// 
	/// <para>The behavior of this implementation is as follows: if we are not currently running in the context of
	/// a process application, we are skipped. If we are, this implementation delegates to the underlying
	/// application-provided <seealso cref="ELResolver"/> which may itself be a <seealso cref="CompositeELResolver"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationElResolverDelegate : AbstractElResolverDelegate
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
				return processApplication.ElResolver;
    
			  }
			  catch (ProcessApplicationUnavailableException e)
			  {
				throw new ProcessEngineException("Cannot access process application '" + processApplicationReference.Name + "'", e);
			  }
    
			}
			else
			{
			  return null;
			}
    
		  }
	  }
	}

}