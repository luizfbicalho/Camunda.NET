using System;
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
namespace org.camunda.bpm.engine.impl.context
{

	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandInvocationContext = org.camunda.bpm.engine.impl.interceptor.CommandInvocationContext;
	using JobExecutorContext = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class Context
	{

	  protected internal static ThreadLocal<Stack<CommandContext>> commandContextThreadLocal = new ThreadLocal<Stack<CommandContext>>();
	  protected internal static ThreadLocal<Stack<CommandInvocationContext>> commandInvocationContextThreadLocal = new ThreadLocal<Stack<CommandInvocationContext>>();

	  protected internal static ThreadLocal<Stack<ProcessEngineConfigurationImpl>> processEngineConfigurationStackThreadLocal = new ThreadLocal<Stack<ProcessEngineConfigurationImpl>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected static ThreadLocal<java.util.Stack<CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution>>> executionContextStackThreadLocal = new ThreadLocal<java.util.Stack<CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution>>>();
	  protected internal static ThreadLocal<Stack<CoreExecutionContext<CoreExecution>>> executionContextStackThreadLocal = new ThreadLocal<Stack<CoreExecutionContext<CoreExecution>>>();
	  protected internal static ThreadLocal<JobExecutorContext> jobExecutorContextThreadLocal = new ThreadLocal<JobExecutorContext>();
	  protected internal static ThreadLocal<Stack<ProcessApplicationReference>> processApplicationContext = new ThreadLocal<Stack<ProcessApplicationReference>>();

	  public static CommandContext CommandContext
	  {
		  get
		  {
			Stack<CommandContext> stack = getStack(commandContextThreadLocal);
			if (stack.Count == 0)
			{
			  return null;
			}
			return stack.Peek();
		  }
		  set
		  {
			getStack(commandContextThreadLocal).Push(value);
		  }
	  }


	  public static void removeCommandContext()
	  {
		getStack(commandContextThreadLocal).Pop();
	  }

	  public static CommandInvocationContext CommandInvocationContext
	  {
		  get
		  {
			Stack<CommandInvocationContext> stack = getStack(commandInvocationContextThreadLocal);
			if (stack.Count == 0)
			{
			  return null;
			}
			return stack.Peek();
		  }
		  set
		  {
			getStack(commandInvocationContextThreadLocal).Push(value);
		  }
	  }


	  public static void removeCommandInvocationContext()
	  {
		getStack(commandInvocationContextThreadLocal).Pop();
	  }

	  public static ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			Stack<ProcessEngineConfigurationImpl> stack = getStack(processEngineConfigurationStackThreadLocal);
			if (stack.Count == 0)
			{
			  return null;
			}
			return stack.Peek();
		  }
		  set
		  {
			getStack(processEngineConfigurationStackThreadLocal).Push(value);
		  }
	  }


	  public static void removeProcessEngineConfiguration()
	  {
		getStack(processEngineConfigurationStackThreadLocal).Pop();
	  }

	  /// @deprecated since 7.2, use <seealso cref="#getBpmnExecutionContext()"/> 
	  [Obsolete("since 7.2, use <seealso cref=\"#getBpmnExecutionContext()\"/>")]
	  public static ExecutionContext getExecutionContext()
	  {
		return BpmnExecutionContext;
	  }

	  public static BpmnExecutionContext BpmnExecutionContext
	  {
		  get
		  {
			return (BpmnExecutionContext) CoreExecutionContext;
		  }
	  }

	  public static CaseExecutionContext CaseExecutionContext
	  {
		  get
		  {
			return (CaseExecutionContext) CoreExecutionContext;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public static CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> getCoreExecutionContext()
	  public static CoreExecutionContext<CoreExecution> CoreExecutionContext
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: java.util.Stack<CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution>> stack = getStack(executionContextStackThreadLocal);
			Stack<CoreExecutionContext<CoreExecution>> stack = getStack(executionContextStackThreadLocal);
			if (stack == null || stack.Count == 0)
			{
			  return null;
			}
			else
			{
			  return stack.Peek();
			}
		  }
	  }

	  public static void setExecutionContext(ExecutionEntity execution)
	  {
		getStack(executionContextStackThreadLocal).Push(new BpmnExecutionContext(execution));
	  }

	  public static void setExecutionContext(CaseExecutionEntity execution)
	  {
		getStack(executionContextStackThreadLocal).Push(new CaseExecutionContext(execution));
	  }

	  public static void removeExecutionContext()
	  {
		getStack(executionContextStackThreadLocal).Pop();
	  }

	  protected internal static Stack<T> getStack<T>(ThreadLocal<Stack<T>> threadLocal)
	  {
		Stack<T> stack = threadLocal.get();
		if (stack == null)
		{
		  stack = new Stack<T>();
		  threadLocal.set(stack);
		}
		return stack;
	  }

	  public static JobExecutorContext JobExecutorContext
	  {
		  get
		  {
			return jobExecutorContextThreadLocal.get();
		  }
		  set
		  {
			jobExecutorContextThreadLocal.set(value);
		  }
	  }


	  public static void removeJobExecutorContext()
	  {
		jobExecutorContextThreadLocal.remove();
	  }


	  public static ProcessApplicationReference CurrentProcessApplication
	  {
		  get
		  {
			Stack<ProcessApplicationReference> stack = getStack(processApplicationContext);
			if (stack.Count == 0)
			{
			  return null;
			}
			else
			{
			  return stack.Peek();
			}
		  }
		  set
		  {
			Stack<ProcessApplicationReference> stack = getStack(processApplicationContext);
			stack.Push(value);
		  }
	  }


	  public static void removeCurrentProcessApplication()
	  {
		Stack<ProcessApplicationReference> stack = getStack(processApplicationContext);
		stack.Pop();
	  }

	  /// <summary>
	  /// Use <seealso cref="#executeWithinProcessApplication(Callable, ProcessApplicationReference, InvocationContext)"/>
	  /// instead if an <seealso cref="InvocationContext"/> is available.
	  /// </summary>
	  public static T executeWithinProcessApplication<T>(Callable<T> callback, ProcessApplicationReference processApplicationReference)
	  {
		return executeWithinProcessApplication(callback, processApplicationReference, null);
	  }

	  public static T executeWithinProcessApplication<T>(Callable<T> callback, ProcessApplicationReference processApplicationReference, InvocationContext invocationContext)
	  {
		string paName = processApplicationReference.Name;
		try
		{
		  ProcessApplicationInterface processApplication = processApplicationReference.ProcessApplication;
		  CurrentProcessApplication = processApplicationReference;

		  try
		  {
			// wrap callback
			ProcessApplicationClassloaderInterceptor<T> wrappedCallback = new ProcessApplicationClassloaderInterceptor<T>(callback);
			// execute wrapped callback
			return processApplication.execute(wrappedCallback, invocationContext);

		  }
		  catch (Exception e)
		  {

			// unwrap exception
			if (e.InnerException != null && e.InnerException is Exception)
			{
			  throw (Exception) e.InnerException;
			}
			else
			{
			  throw new ProcessEngineException("Unexpected exeption while executing within process application ", e);
			}

		  }
		  finally
		  {
			removeCurrentProcessApplication();
		  }


		}
		catch (ProcessApplicationUnavailableException e)
		{
		  throw new ProcessEngineException("Cannot switch to process application '" + paName + "' for execution: " + e.Message, e);
		}
	  }
	}

}