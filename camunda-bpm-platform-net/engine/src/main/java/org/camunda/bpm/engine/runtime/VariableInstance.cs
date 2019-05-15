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
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// A <seealso cref="VariableInstance"/> represents a variable in the execution of
	/// a process instance.
	/// 
	/// @author roman.smirnov
	/// 
	/// </summary>
	public interface VariableInstance
	{

	  /// <returns> the Id of this variable instance </returns>
	  string Id {get;}

	  /// <summary>
	  /// Returns the name of this variable instance.
	  /// </summary>
	  string Name {get;}

	  /// <summary>
	  /// Returns the name of the type of this variable instance
	  /// </summary>
	  /// <returns> the type name of the variable </returns>
	  string TypeName {get;}

	  /// <summary>
	  /// Returns the value of this variable instance.
	  /// </summary>
	  object Value {get;}

	  /// <summary>
	  /// Returns the TypedValue of this variable instance.
	  /// </summary>
	  TypedValue TypedValue {get;}

	  /// <summary>
	  /// Returns the corresponding process instance id.
	  /// </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Returns the corresponding execution id.
	  /// </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Returns the corresponding case instance id.
	  /// </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// Returns the corresponding case execution id.
	  /// </summary>
	  string CaseExecutionId {get;}

	  /// <summary>
	  /// Returns the corresponding task id.
	  /// </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// Returns the corresponding activity instance id.
	  /// </summary>
	  string ActivityInstanceId {get;}

	  /// <summary>
	  /// If the variable value could not be loaded, this returns the error message. </summary>
	  /// <returns> an error message indicating why the variable value could not be loaded. </returns>
	  string ErrorMessage {get;}

	  /// <summary>
	  /// The id of the tenant this variable belongs to. Can be <code>null</code>
	  /// if the variable belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	}

}