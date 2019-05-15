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
namespace org.camunda.bpm.engine.cdi.impl.context
{


	using BusinessProcessScoped = org.camunda.bpm.engine.cdi.annotation.BusinessProcessScoped;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;

	/// <summary>
	/// Implementation of the BusinessProcessContext-scope.
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public class BusinessProcessContext implements javax.enterprise.context.spi.Context
	public class BusinessProcessContext : Context
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  internal static readonly Logger logger = Logger.getLogger(typeof(BusinessProcessContext).FullName);

	  private readonly BeanManager beanManager;

	  public BusinessProcessContext(BeanManager beanManager)
	  {
		this.beanManager = beanManager;
	  }

	  protected internal virtual BusinessProcess BusinessProcess
	  {
		  get
		  {
			return ProgrammaticBeanLookup.lookup(typeof(BusinessProcess), beanManager);
		  }
	  }

	  public override Type Scope
	  {
		  get
		  {
			return typeof(BusinessProcessScoped);
		  }
	  }

	  public override T get<T>(Contextual<T> contextual)
	  {
		Bean<T> bean = (Bean<T>) contextual;
		string variableName = bean.Name;

		BusinessProcess businessProcess = BusinessProcess;
		object variable = businessProcess.getVariable(variableName);
		if (variable != null)
		{

		  if (logger.isLoggable(Level.FINE))
		  {
			if (businessProcess.Associated)
			{
			  logger.fine("Getting instance of bean '" + variableName + "' from Execution[" + businessProcess.ExecutionId + "].");
			}
			else
			{
			  logger.fine("Getting instance of bean '" + variableName + "' from transient bean store");
			}
		  }

		  return (T) variable;
		}
		else
		{
		  return null;
		}

	  }

	  public override T get<T>(Contextual<T> contextual, CreationalContext<T> arg1)
	  {

		Bean<T> bean = (Bean<T>) contextual;
		string variableName = bean.Name;

		BusinessProcess businessProcess = BusinessProcess;
		object variable = businessProcess.getVariable(variableName);
		if (variable != null)
		{

		  if (logger.isLoggable(Level.FINE))
		  {
			if (businessProcess.Associated)
			{
			  logger.fine("Getting instance of bean '" + variableName + "' from Execution[" + businessProcess.ExecutionId + "].");
			}
			else
			{
			  logger.fine("Getting instance of bean '" + variableName + "' from transient bean store");
			}
		  }

		  return (T) variable;
		}
		else
		{
		  if (logger.isLoggable(Level.FINE))
		  {
			if (businessProcess.Associated)
			{
			  logger.fine("Creating instance of bean '" + variableName + "' in business process context representing Execution[" + businessProcess.ExecutionId + "].");
			}
			else
			{
			  logger.fine("Creating instance of bean '" + variableName + "' in transient bean store");
			}
		  }

		  T beanInstance = bean.create(arg1);
		  businessProcess.setVariable(variableName, beanInstance);
		  return beanInstance;
		}

	  }

	  public override bool Active
	  {
		  get
		  {
			// we assume the business process is always 'active'. If no task/execution is 
			// associated, temporary instances of @BusinessProcesScoped beans are cached in the 
			// conversation / request 
			return true;
		  }
	  }

	}

}