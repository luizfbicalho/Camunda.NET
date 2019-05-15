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
namespace org.camunda.bpm.engine.impl.variable.listener
{
	using CaseVariableListener = org.camunda.bpm.engine.@delegate.CaseVariableListener;
	using DelegateCaseVariableInstance = org.camunda.bpm.engine.@delegate.DelegateCaseVariableInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ScriptInvocation = org.camunda.bpm.engine.impl.@delegate.ScriptInvocation;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ScriptCaseVariableListener : CaseVariableListener
	{

	  protected internal readonly ExecutableScript script;

	  public ScriptCaseVariableListener(ExecutableScript script)
	  {
		this.script = script;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseVariableInstance variableInstance) throws Exception
	  public virtual void notify(DelegateCaseVariableInstance variableInstance)
	  {
		DelegateCaseVariableInstanceImpl variableInstanceImpl = (DelegateCaseVariableInstanceImpl) variableInstance;

		ScriptInvocation invocation = new ScriptInvocation(script, variableInstanceImpl.ScopeExecution);
		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
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