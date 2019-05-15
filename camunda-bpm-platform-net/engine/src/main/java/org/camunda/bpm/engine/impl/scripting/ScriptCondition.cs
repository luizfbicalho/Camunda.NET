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
namespace org.camunda.bpm.engine.impl.scripting
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureInstanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ScriptInvocation = org.camunda.bpm.engine.impl.@delegate.ScriptInvocation;

	/// <summary>
	/// A <seealso cref="Condition"/> which invokes a <seealso cref="ExecutableScript"/> when evaluated.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ScriptCondition : Condition
	{

	  protected internal readonly ExecutableScript script;

	  public ScriptCondition(ExecutableScript script)
	  {
		this.script = script;
	  }

	  public virtual bool evaluate(DelegateExecution execution)
	  {
		return evaluate(execution, execution);
	  }


	  public virtual bool evaluate(VariableScope scope, DelegateExecution execution)
	  {
		ScriptInvocation invocation = new ScriptInvocation(script, scope, execution);
		try
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException(e);
		}

		object result = invocation.InvocationResult;

		ensureNotNull("condition script returns null", "result", result);
		ensureInstanceOf("condition script returns non-Boolean", "result", result, typeof(Boolean));

		return (bool?) result.Value;
	  }

	  public virtual bool tryEvaluate(VariableScope scope, DelegateExecution execution)
	  {
		bool result = false;

		try
		{
		  result = evaluate(scope, execution);
		}
		catch (ProcessEngineException pee)
		{
		  if (!(pee.Message.contains("No such property") || pee.InnerException is ScriptEvaluationException))
		  {
			throw pee;
		  }
		}

		return result;
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