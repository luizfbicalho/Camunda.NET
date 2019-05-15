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
namespace org.camunda.bpm.engine.impl
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;



	/// <summary>
	/// @author Tom Baeyens
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public interface Condition
	{

	  /// <summary>
	  /// Evaluates the condition and returns the result.
	  /// The scope will be the same as the execution.
	  /// </summary>
	  /// <param name="execution"> the execution which is used to evaluate the condition </param>
	  /// <returns> the result </returns>
	  bool evaluate(DelegateExecution execution);

	  /// <summary>
	  /// Evaluates the condition and returns the result.
	  /// </summary>
	  /// <param name="scope"> the variable scope which can differ of the execution </param>
	  /// <param name="execution"> the execution which is used to evaluate the condition </param>
	  /// <returns> the result </returns>
	  bool evaluate(VariableScope scope, DelegateExecution execution);

	  /// <summary>
	  /// Tries to evaluate the condition. If the property which is used in the condition does not exist
	  /// false will be returned.
	  /// </summary>
	  /// <param name="scope"> the variable scope which can differ of the execution </param>
	  /// <param name="execution"> the execution which is used to evaluate the condition </param>
	  /// <returns> the result </returns>
	  bool tryEvaluate(VariableScope scope, DelegateExecution execution);
	}

}