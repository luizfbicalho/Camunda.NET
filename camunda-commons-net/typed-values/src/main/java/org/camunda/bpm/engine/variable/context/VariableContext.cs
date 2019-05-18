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
namespace org.camunda.bpm.engine.variable.context
{

	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// A context for variables. Allows resolving variables.
	/// 
	/// An API may choose to accept a VariableContext instead of a map of concrete values
	/// in situations where passing all available variables would be expensive and
	/// lazy-loading is a desirable optimization.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface VariableContext
	{

	  /// <summary>
	  /// Resolve a value in this context.
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to resolve. </param>
	  /// <returns> the value of the variable or null in case the variable does not exist. </returns>
	  TypedValue resolve(string variableName);

	  /// <summary>
	  /// Checks whether a variable with the given name is resolve through this context.
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to check </param>
	  /// <returns> true if the variable is resolve. </returns>
	  bool containsVariable(string variableName);

	  /// <returns> a set of all variable names resolvable through this Context. </returns>
	  ISet<string> keySet();

	}

}