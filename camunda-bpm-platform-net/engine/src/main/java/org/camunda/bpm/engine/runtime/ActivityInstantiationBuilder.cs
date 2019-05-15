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
namespace org.camunda.bpm.engine.runtime
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public interface ActivityInstantiationBuilder<T> where T : ActivityInstantiationBuilder<T>
	{

	  /// <summary>
	  /// If an instruction is submitted before then the variable is set when the
	  /// instruction is executed. Otherwise, the variable is set on the process
	  /// instance itself.
	  /// </summary>
	  T setVariable(string name, object value);

	  /// <summary>
	  /// If an instruction is submitted before then the local variable is set when
	  /// the instruction is executed. Otherwise, the variable is set on the process
	  /// instance itself.
	  /// </summary>
	  T setVariableLocal(string name, object value);

	  /// <summary>
	  /// If an instruction is submitted before then all variables are set when the
	  /// instruction is executed. Otherwise, the variables are set on the process
	  /// instance itself.
	  /// </summary>
	  T setVariables(IDictionary<string, object> variables);

	  /// <summary>
	  /// If an instruction is submitted before then all local variables are set when
	  /// the instruction is executed. Otherwise, the variables are set on the
	  /// process instance itself.
	  /// </summary>
	  T setVariablesLocal(IDictionary<string, object> variables);

	}

}