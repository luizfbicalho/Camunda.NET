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
	/// A single process variable containing the last value when its process instance has finished.
	/// It is only available when HISTORY_LEVEL is set >= AUDIT
	/// 
	/// @author Christian Lipphardt (camunda)
	/// @author ruecker
	/// </summary>
	public interface HistoricVariableInstance
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
	  /// Returns the <seealso cref="TypedValue"/> of this variable instance.
	  /// </summary>
	  TypedValue TypedValue {get;}

	  /// <summary>
	  /// Returns the name of this variable instance.
	  /// 
	  /// <para>Deprecated since 7.2: use <seealso cref="#getName()"/> instead.</para>
	  /// 
	  /// </summary>
	   [Obsolete]
	   string VariableName {get;}

	  /// <summary>
	  /// <para>Returns the name of the type of this variable instance</para>
	  /// 
	  /// <para>Deprecated since 7.2: use <seealso cref="#getTypeName()"/> instead.</para>
	  /// 
	  /// </summary>
	  [Obsolete]
	  string VariableTypeName {get;}

	  /// <summary>
	  /// The process definition key reference.
	  /// </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// The process definition reference.
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// The root process instance reference.
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The process instance reference.
	  /// </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Return the corresponding execution id.
	  /// </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Returns the corresponding activity instance id.
	  /// </summary>
	  [Obsolete]
	  string ActivtyInstanceId {get;}

	  /// <summary>
	  /// Returns the corresponding activity instance id.
	  /// </summary>
	  string ActivityInstanceId {get;}

	  /// <summary>
	  /// The case definition key reference.
	  /// </summary>
	  string CaseDefinitionKey {get;}

	  /// <summary>
	  /// The case definition reference.
	  /// </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// The case instance reference.
	  /// </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// Return the corresponding case execution id.
	  /// </summary>
	  string CaseExecutionId {get;}

	  /// <summary>
	  /// Return the corresponding task id.
	  /// </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// If the variable value could not be loaded, this returns the error message. </summary>
	  /// <returns> an error message indicating why the variable value could not be loaded. </returns>
	  string ErrorMessage {get;}

	  /// <summary>
	  /// The id of the tenant this variable belongs to. Can be <code>null</code>
	  /// if the variable belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// The current state of the variable. Can be 'CREATED' or 'DELETED'
	  /// </summary>
	  string State {get;}

	  /// <summary>
	  /// The time when the variable was created.
	  /// </summary>
	  DateTime CreateTime {get;}

	  /// <summary>
	  /// The time when the historic variable instance will be removed. </summary>
	  DateTime RemovalTime {get;}
	}

	public static class HistoricVariableInstance_Fields
	{
	  public const string STATE_CREATED = "CREATED";
	  public const string STATE_DELETED = "DELETED";
	}

}