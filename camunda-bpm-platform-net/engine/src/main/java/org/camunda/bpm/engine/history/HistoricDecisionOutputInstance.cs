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
namespace org.camunda.bpm.engine.history
{

	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Represents one output variable of a decision evaluation.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public interface HistoricDecisionOutputInstance
	{

	  /// <summary>
	  /// The unique identifier of this historic decision output instance. </summary>
	  string Id {get;}

	  /// <summary>
	  /// The unique identifier of the historic decision instance. </summary>
	  string DecisionInstanceId {get;}

	  /// <summary>
	  /// The unique identifier of the clause that the value is assigned for.
	  /// Can be <code>null</code> if the decision is not implemented as decision table. 
	  /// </summary>
	  string ClauseId {get;}

	  /// <summary>
	  /// The name of the clause that the value is assigned for.
	  /// Can be <code>null</code> if the decision is not implemented as decision table. 
	  /// </summary>
	  string ClauseName {get;}

	  /// <summary>
	  /// The unique identifier of the rule that is matched.
	  /// Can be <code>null</code> if the decision is not implemented as decision table. 
	  /// </summary>
	  string RuleId {get;}

	  /// <summary>
	  /// The order of the rule that is matched.
	  /// Can be <code>null</code> if the decision is not implemented as decision table. 
	  /// </summary>
	  int? RuleOrder {get;}

	  /// <summary>
	  /// The name of the output variable. </summary>
	  string VariableName {get;}

	  /// <summary>
	  /// Returns the type name of the variable
	  /// </summary>
	  string TypeName {get;}

	  /// <summary>
	  /// Returns the value of this variable instance.
	  /// </summary>
	  object Value {get;}

	  /// <summary>
	  /// Returns the <seealso cref="TypedValue"/> for this value.
	  /// </summary>
	  TypedValue TypedValue {get;}

	  /// <summary>
	  /// If the variable value could not be loaded, this returns the error message.
	  /// </summary>
	  /// <returns> an error message indicating why the variable value could not be loaded. </returns>
	  string ErrorMessage {get;}

	  /// <summary>
	  /// Returns time when the variable was created.
	  /// </summary>
	  DateTime CreateTime {get;}

	  /// <summary>
	  /// Returns the root process instance id of the process instance
	  /// on which the associated business rule task has been called.
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The time the historic decision instance will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}