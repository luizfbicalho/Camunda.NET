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
namespace org.camunda.bpm.engine.impl.@delegate
{
	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ScriptInvocation : DelegateInvocation
	{

	  protected internal ExecutableScript script;
	  protected internal VariableScope scope;

	  public ScriptInvocation(ExecutableScript script, VariableScope scope) : this(script, scope, null)
	  {
	  }

	  public ScriptInvocation(ExecutableScript script, VariableScope scope, BaseDelegateExecution contextExecution) : base(contextExecution, null)
	  {
		this.script = script;
		this.scope = scope;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
		invocationResult = Context.ProcessEngineConfiguration.ScriptingEnvironment.execute(script, scope);
	  }

	}

}