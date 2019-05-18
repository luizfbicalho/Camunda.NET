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
namespace org.camunda.bpm.engine.impl.interceptor
{

	using PersistenceException = org.apache.ibatis.exceptions.PersistenceException;
	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation;

	/// <summary>
	/// In contrast to <seealso cref="CommandContext"/>, this context holds resources that are only valid
	/// during execution of a single command (i.e. the current command or an exception that was thrown
	/// during its execution).
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class CommandInvocationContext<T1>
    {

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal Exception throwable;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected Command< ? > command;
	  protected internal Command<T1> command;
	  protected internal bool isExecuting = false;
	  protected internal IList<AtomicOperationInvocation> queuedInvocations = new List<AtomicOperationInvocation>();
	  protected internal BpmnStackTrace bpmnStackTrace = new BpmnStackTrace();

	  public CommandInvocationContext(Command<T1> command)
	  {
		this.command = command;
	  }

	  public virtual Exception Throwable
	  {
		  get
		  {
			return throwable;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public Command<?> getCommand()
	  public virtual Command<object> Command
	  {
		  get
		  {
			return command;
		  }
	  }

	  public virtual void trySetThrowable(Exception t)
	  {
		if (this.throwable == null)
		{
		  this.throwable = t;
		}
		else
		{
		  LOG.maskedExceptionInCommandContext(throwable);
		}
	  }

	  public virtual void performOperation(AtomicOperation executionOperation, ExecutionEntity execution)
	  {
		performOperation(executionOperation, execution, false);
	  }

	  public virtual void performOperationAsync(AtomicOperation executionOperation, ExecutionEntity execution)
	  {
		performOperation(executionOperation, execution, true);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void performOperation(final org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation executionOperation, final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity execution, final boolean performAsync)
	  public virtual void performOperation(AtomicOperation executionOperation, ExecutionEntity execution, bool performAsync)
	  {
		AtomicOperationInvocation invocation = new AtomicOperationInvocation(executionOperation, execution, performAsync);
		queuedInvocations.Insert(0, invocation);
		performNext();
	  }

	  protected internal virtual void performNext()
	  {
		AtomicOperationInvocation nextInvocation = queuedInvocations[0];

		if (nextInvocation.operation.AsyncCapable && isExecuting)
		{
		  // will be picked up by while loop below
		  return;
		}

		ProcessApplicationReference targetProcessApplication = getTargetProcessApplication(nextInvocation.execution);
		if (requiresContextSwitch(targetProcessApplication))
		{

		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this)
		 , targetProcessApplication, new InvocationContext(nextInvocation.execution));
		}
		else
		{
		  if (!nextInvocation.operation.AsyncCapable)
		  {
			// if operation is not async capable, perform right away.
			invokeNext();
		  }
		  else
		  {
			try
			{
			  isExecuting = true;
			  while (queuedInvocations.Count > 0)
			  {
				// assumption: all operations are executed within the same process application...
				invokeNext();
			  }
			}
			finally
			{
			  isExecuting = false;
			}
		  }
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly CommandInvocationContext outerInstance;

		  public CallableAnonymousInnerClass(CommandInvocationContext outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.performNext();
			return null;
		  }

	  }

	  protected internal virtual void invokeNext()
	  {
		AtomicOperationInvocation invocation = queuedInvocations.RemoveAt(0);
		try
		{
		  invocation.execute(bpmnStackTrace);
		}
		catch (Exception e)
		{
		  // log bpmn stacktrace
		  bpmnStackTrace.printStackTrace(Context.ProcessEngineConfiguration.BpmnStacktraceVerbose);
		  // rethrow
		  throw e;
		}
	  }

	  protected internal virtual bool requiresContextSwitch(ProcessApplicationReference processApplicationReference)
	  {
		return ProcessApplicationContextUtil.requiresContextSwitch(processApplicationReference);
	  }

	  protected internal virtual ProcessApplicationReference getTargetProcessApplication(ExecutionEntity execution)
	  {
		return ProcessApplicationContextUtil.getTargetProcessApplication(execution);
	  }

	  public virtual void rethrow()
	  {
		if (throwable != null)
		{
		  if (throwable is Exception)
		  {
			throw (Exception) throwable;
		  }
		  else if (throwable is PersistenceException)
		  {
			throw new ProcessEngineException("Process engine persistence exception", throwable);
		  }
		  else if (throwable is Exception)
		  {
			throw (Exception) throwable;
		  }
		  else
		  {
			throw new ProcessEngineException("exception while executing command " + command, throwable);
		  }
		}
	  }
	}

}