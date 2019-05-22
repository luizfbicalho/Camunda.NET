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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ScriptInvocation = org.camunda.bpm.engine.impl.@delegate.ScriptInvocation;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;

	/// <summary>
	/// <para>
	/// <seealso cref="ActivityBehavior"/> implementation of the BPMN 2.0 script task.
	/// </para>
	/// 
	/// @author Joram Barrez
	/// @author Christian Stettler
	/// @author Falko Menge
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ScriptTaskActivityBehavior : TaskActivityBehavior
	{

	  protected internal ExecutableScript script;
	  protected internal string resultVariable;

	  public ScriptTaskActivityBehavior(ExecutableScript script, string resultVariable)
	  {
		this.script = script;
		this.resultVariable = resultVariable;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void performExecution(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public override void performExecution(ActivityExecution execution)
	  {
		executeWithErrorPropagation(execution, new CallableAnonymousInnerClass(this, execution));
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ScriptTaskActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass(ScriptTaskActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			ScriptInvocation invocation = new ScriptInvocation(outerInstance.script, execution);
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
			object result = invocation.InvocationResult;
			if (result != null && !string.ReferenceEquals(outerInstance.resultVariable, null))
			{
			  execution.setVariable(outerInstance.resultVariable, result);
			}
			outerInstance.leave(execution);
			return null;
		  }
	  }

	  /// <summary>
	  /// Searches recursively through the exception to see if the exception itself
	  /// or one of its causes is a <seealso cref="BpmnError"/>.
	  /// </summary>
	  /// <param name="e">
	  ///          the exception to check </param>
	  /// <returns> the BpmnError that was the cause of this exception or null if no
	  ///         BpmnError was found </returns>
	  protected internal virtual BpmnError checkIfCauseOfExceptionIsBpmnError(Exception e)
	  {
		if (e is BpmnError)
		{
		  return (BpmnError) e;
		}
		else if (e.InnerException == null)
		{
		  return null;
		}
		return checkIfCauseOfExceptionIsBpmnError(e.InnerException);
	  }

	  public virtual ExecutableScript Script
	  {
		  get
		  {
			return script;
		  }
	  }

	}

}