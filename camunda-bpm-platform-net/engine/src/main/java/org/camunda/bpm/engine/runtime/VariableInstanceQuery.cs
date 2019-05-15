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

	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public interface VariableInstanceQuery : Query<VariableInstanceQuery, VariableInstance>
	{

	  /// <summary>
	  /// Only select the variable with the given Id </summary>
	  /// <param name="the"> id of the variable to select </param>
	  /// <returns> the query object  </returns>
	  VariableInstanceQuery variableId(string id);

	  /// <summary>
	  /// Only select variable instances which have the variable name. * </summary>
	  VariableInstanceQuery variableName(string variableName);

	  /// <summary>
	  /// Only select variable instances which have one of the variables names. * </summary>
	  VariableInstanceQuery variableNameIn(params string[] variableNames);

	  /// <summary>
	  /// Only select variable instances which have the name like the assigned variable name.
	  /// The string can include the wildcard character '%' to express like-strategy:
	  /// starts with (string%), ends with (%string) or contains (%string%).
	  /// 
	  /// </summary>
	  VariableInstanceQuery variableNameLike(string variableNameLike);

	  /// <summary>
	  /// Only select variable instances which have one of the executions ids. * </summary>
	  VariableInstanceQuery executionIdIn(params string[] executionIds);

	  /// <summary>
	  /// Only select variable instances which have one of the process instance ids. * </summary>
	  VariableInstanceQuery processInstanceIdIn(params string[] processInstanceIds);

	  /// <summary>
	  /// Only select variable instances which have one of the case execution ids. * </summary>
	  VariableInstanceQuery caseExecutionIdIn(params string[] caseExecutionIds);

	  /// <summary>
	  /// Only select variable instances which have one of the case instance ids. * </summary>
	  VariableInstanceQuery caseInstanceIdIn(params string[] caseInstanceIds);

	  /// <summary>
	  /// Only select variable instances which have one of the task ids. * </summary>
	  VariableInstanceQuery taskIdIn(params string[] taskIds);

	  /// <summary>
	  /// Only select variables instances which have on of the variable scope ids. * </summary>
	  VariableInstanceQuery variableScopeIdIn(params string[] variableScopeIds);

	  /// <summary>
	  /// Only select variable instances which have one of the activity instance ids. * </summary>
	  VariableInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds);

	  /// <summary>
	  /// Only select variables instances which have the given name and value. The type
	  /// of variable is determined based on the value, using types configured in
	  /// <seealso cref="ProcessEngineConfiguration#getVariableSerializers()"/>.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  /// <param name="value"> variable value, can be null. </param>
	  VariableInstanceQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select variable instances which have the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  /// <param name="value"> variable value, can be null. </param>
	  VariableInstanceQuery variableValueNotEquals(string name, object value);

	  /// <summary>
	  /// Only select variable instances which value is greater than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  VariableInstanceQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select variable instances which value is greater than or equal to
	  /// the passed value. Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  VariableInstanceQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select variable instances which value is less than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  VariableInstanceQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select variable instances which value is less than or equal to the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  VariableInstanceQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Disable fetching of byte array and file values. By default, the query will fetch such values.
	  /// By calling this method you can prevent the values of (potentially large) blob data chunks
	  /// to be fetched. The variables themselves are nonetheless included in the query result.
	  /// </summary>
	  /// <returns> the query builder </returns>
	  VariableInstanceQuery disableBinaryFetching();

	  /// <summary>
	  /// Disable deserialization of variable values that are custom objects. By default, the query
	  /// will attempt to deserialize the value of these variables. By calling this method you can
	  /// prevent such attempts in environments where their classes are not available.
	  /// Independent of this setting, variable serialized values are accessible.
	  /// </summary>
	  VariableInstanceQuery disableCustomObjectDeserialization();

	  /// <summary>
	  /// Only select variable instances which value is like the given value.
	  /// This be used on string variables only. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. The string can include the
	  /// wildcard character '%' to express like-strategy:
	  /// starts with (string%), ends with (%string) or contains (%string%). </param>
	  VariableInstanceQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only select variable instances with one of the given tenant ids. </summary>
	  VariableInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Order by variable name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  VariableInstanceQuery orderByVariableName();

	  /// <summary>
	  /// Order by variable type (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  VariableInstanceQuery orderByVariableType();

	  /// <summary>
	  /// Order by activity instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  VariableInstanceQuery orderByActivityInstanceId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of variable instances without tenant id is database-specific.
	  /// </summary>
	  VariableInstanceQuery orderByTenantId();

	}

}