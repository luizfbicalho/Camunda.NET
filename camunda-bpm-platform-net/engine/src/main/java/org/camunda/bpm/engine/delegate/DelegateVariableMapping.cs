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

	/// <summary>
	/// Represents a delegated mapping of input and output variables.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public interface DelegateVariableMapping
	{

	  /// <summary>
	  /// Maps the input variables into the given variables map.
	  /// The variables map will be used by the sub process.
	  /// </summary>
	  /// <param name="superExecution"> the execution object of the super (outer) process </param>
	  /// <param name="subVariables"> the variables map of the sub (inner) process </param>
	  void mapInputVariables(DelegateExecution superExecution, VariableMap subVariables);

	  /// <summary>
	  /// Maps the output variables into the outer process. This means the variables of
	  /// the sub process, which can be accessed via the subInstance, will be
	  /// set as variables into the super process, for example via ${superExecution.setVariables}.
	  /// </summary>
	  /// <param name="superExecution"> the execution object of the super (outer) process, which gets the output variables </param>
	  /// <param name="subInstance"> the instance of the sub process, which contains the variables </param>
	  void mapOutputVariables(DelegateExecution superExecution, VariableScope subInstance);

	}

}