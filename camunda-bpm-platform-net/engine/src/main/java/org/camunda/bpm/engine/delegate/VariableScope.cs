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
namespace org.camunda.bpm.engine.@delegate
{

	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public interface VariableScope
	{

	  string VariableScopeKey {get;}

	  IDictionary<string, object> Variables {get;set;}

	  VariableMap VariablesTyped {get;}

	  VariableMap getVariablesTyped(bool deserializeValues);

	  IDictionary<string, object> VariablesLocal {get;set;}

	  VariableMap VariablesLocalTyped {get;}

	  VariableMap getVariablesLocalTyped(bool deserializeValues);

	  object getVariable(string variableName);

	  object getVariableLocal(string variableName);

	  T getVariableTyped<T>(string variableName);

	  T getVariableTyped<T>(string variableName, bool deserializeValue);

	  T getVariableLocalTyped<T>(string variableName);

	  T getVariableLocalTyped<T>(string variableName, bool deserializeValue);

	  ISet<string> VariableNames {get;}

	  ISet<string> VariableNamesLocal {get;}

	  void setVariable(string variableName, object value);

	  void setVariableLocal(string variableName, object value);



	  bool hasVariables();

	  bool hasVariablesLocal();

	  bool hasVariable(string variableName);

	  bool hasVariableLocal(string variableName);

	  /// <summary>
	  /// Removes the variable and creates a new
	  /// <seealso cref="HistoricVariableUpdateEntity"/>.
	  /// </summary>
	  void removeVariable(string variableName);

	  /// <summary>
	  /// Removes the local variable and creates a new
	  /// <seealso cref="HistoricVariableUpdateEntity"/>.
	  /// </summary>
	  void removeVariableLocal(string variableName);

	  /// <summary>
	  /// Removes the variables and creates a new
	  /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
	  /// </summary>
	  void removeVariables(ICollection<string> variableNames);

	  /// <summary>
	  /// Removes the local variables and creates a new
	  /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
	  /// </summary>
	  void removeVariablesLocal(ICollection<string> variableNames);

	  /// <summary>
	  /// Removes the (local) variables and creates a new
	  /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
	  /// </summary>
	  void removeVariables();

	  /// <summary>
	  /// Removes the (local) variables and creates a new
	  /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
	  /// </summary>
	  void removeVariablesLocal();

	}

}