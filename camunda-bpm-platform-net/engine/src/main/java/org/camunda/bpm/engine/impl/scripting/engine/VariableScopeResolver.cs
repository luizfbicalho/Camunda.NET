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
namespace org.camunda.bpm.engine.impl.scripting.engine
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.ExecutionImpl;


	/// <summary>
	/// Bindings implementation using an <seealso cref="ExecutionImpl"/> as 'back-end'.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class VariableScopeResolver : Resolver
	{

	  protected internal VariableScope variableScope;
	  protected internal string variableScopeKey;

	  public VariableScopeResolver(VariableScope variableScope)
	  {
		ensureNotNull("variableScope", variableScope);
		variableScopeKey = variableScope.VariableScopeKey;
		this.variableScope = variableScope;
	  }

	  public virtual bool containsKey(object key)
	  {
		return variableScopeKey.Equals(key) || variableScope.hasVariable((string) key);
	  }

	  public virtual object get(object key)
	  {
		if (variableScopeKey.Equals(key))
		{
		  return variableScope;
		}
		return variableScope.getVariable((string) key);
	  }

	  public virtual ISet<string> keySet()
	  {
		// get variable names will return a new set instance
		ISet<string> variableNames = variableScope.VariableNames;
		variableNames.Add(variableScopeKey);
		return variableNames;
	  }
	}

}