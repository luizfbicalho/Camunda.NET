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
	/// Update of a process variable.  This is only available if history
	/// level is configured to FULL.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricVariableUpdate : HistoricDetail
	{

	  string VariableName {get;}

	  /// <summary>
	  /// Returns the id of the corresponding variable instance.
	  /// </summary>
	  string VariableInstanceId {get;}

	  /// <summary>
	  /// Returns the type name of the variable
	  /// </summary>
	  /// <returns> the type name of the variable </returns>
	  string TypeName {get;}

	  /// <returns> the name of the variable type. </returns>
	  /// @deprecated since 7.2. Use <seealso cref="#getTypeName()"/> 
	  [Obsolete("since 7.2. Use <seealso cref=\"#getTypeName()\"/>")]
	  string VariableTypeName {get;}

	  object Value {get;}

	  /// <returns> the <seealso cref="TypedValue"/> for this variable update </returns>
	  TypedValue TypedValue {get;}

	  int Revision {get;}

	  /// <summary>
	  /// If the variable value could not be loaded, this returns the error message. </summary>
	  /// <returns> an error message indicating why the variable value could not be loaded. </returns>
	  string ErrorMessage {get;}
	}

}