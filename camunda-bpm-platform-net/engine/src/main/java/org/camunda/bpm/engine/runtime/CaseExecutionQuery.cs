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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CaseExecutionQuery : Query<CaseExecutionQuery, CaseExecution>
	{

	  /// <summary>
	  /// Only select case executions which have the given case instance id.
	  /// </summary>
	  /// <param name="caseInstanceId"> the id of the case instance
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case instance id is null
	  ///  </exception>
	  CaseExecutionQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select case executions which have the given case definition id.
	  /// </summary>
	  /// <param name="caseDefinitionId"> the id of the case definition
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case definition id is null
	  ///  </exception>
	  CaseExecutionQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select case executions which have the given case definition key.
	  /// </summary>
	  /// <param name="caseDefinitionKey"> the key of the case definition
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case definition key is null
	  ///  </exception>
	  CaseExecutionQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Only select case executions that belong to a case instance with the given business key
	  /// </summary>
	  /// <param name="caseInstanceBusinessKey"> the business key of the case instance
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case instance business key is null
	  ///  </exception>
	  CaseExecutionQuery caseInstanceBusinessKey(string caseInstanceBusinessKey);

	  /// <summary>
	  /// Only select case executions with the given id.
	  /// </summary>
	  /// <param name="executionId"> the id of the case execution
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case execution id is null
	  ///  </exception>
	  CaseExecutionQuery caseExecutionId(string executionId);

	  /// <summary>
	  /// Only select case executions which contain an activity with the given id.
	  /// </summary>
	  /// <param name="activityId"> the id of the activity
	  /// </param>
	  /// <exception cref="NotValidException"> when the given activity id is null
	  ///  </exception>
	  CaseExecutionQuery activityId(string activityId);

	  /// <summary>
	  /// Only select case executions which are required. * </summary>
	  CaseExecutionQuery required();

	  /// <summary>
	  /// Only select case executions which are available. * </summary>
	  CaseExecutionQuery available();

	  /// <summary>
	  /// Only select case executions which are enabled. * </summary>
	  CaseExecutionQuery enabled();

	  /// <summary>
	  /// Only select case executions which are active. * </summary>
	  CaseExecutionQuery active();

	  /// <summary>
	  /// Only select case executions which are disabled. * </summary>
	  CaseExecutionQuery disabled();

	  /// <summary>
	  /// Only select case executions which have a local variable with the given value. The type
	  /// of variable is determined based on the value, using types configured in
	  /// <seealso cref="ProcessEngineConfigurationImpl#getVariableSerializers()"/>.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null
	  ///  </exception>
	  CaseExecutionQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select case executions which have a local variable with the given name, but
	  /// with a different value than the passed value.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null
	  ///  </exception>
	  CaseExecutionQuery variableValueNotEquals(string name, object value);


	  /// <summary>
	  /// Only select case executions which have a variable value greater than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select case executions which have a local variable value greater than or equal to
	  /// the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select case executions which have a local variable value less than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select case executions which have a local variable value less than or equal to the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select case executions which have a local variable value like the given value.
	  /// 
	  /// This can be used on string variables only.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null. The string can include the
	  ///              wildcard character '%' to express like-strategy:
	  ///              starts with (string%), ends with (%string) or contains (%string%).
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a variable
	  /// with the given name set to the given value. The type of variable is determined based
	  /// on the value, using types configured in <seealso cref="ProcessEngineConfiguration#getVariableSerializers()"/>.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a variable
	  /// with the given name, but with a different value than the passed value.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueNotEquals(string name, object value);


	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a variable
	  /// with the given name and a variable value greater than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a
	  /// variable value greater than or equal to the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a variable
	  /// value less than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a variable
	  /// value less than or equal to the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select case executions which are part of a case instance that have a variable value
	  /// like the given value.
	  /// 
	  /// This can be used on string variables only.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable, cannot be null. The string can include the
	  ///              wildcard character '%' to express like-strategy:
	  ///              starts with (string%), ends with (%string) or contains (%string%).
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseExecutionQuery caseInstanceVariableValueLike(string name, string value);

	  /// <summary>
	  /// Only select case execution with one of the given tenant ids. </summary>
	  CaseExecutionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select case executions which have no tenant id. </summary>
	  CaseExecutionQuery withoutTenantId();

	  // ordering //////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  CaseExecutionQuery orderByCaseExecutionId();

	  /// <summary>
	  /// Order by case definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  CaseExecutionQuery orderByCaseDefinitionKey();

	  /// <summary>
	  /// Order by case definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  CaseExecutionQuery orderByCaseDefinitionId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of case executions without tenant id is database-specific.
	  /// </summary>
	  CaseExecutionQuery orderByTenantId();

	}

}