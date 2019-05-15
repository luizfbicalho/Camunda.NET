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
	public interface CaseInstanceQuery : Query<CaseInstanceQuery, CaseInstance>
	{

	  /// <summary>
	  /// Select the case instance with the given id
	  /// </summary>
	  /// <param name="caseInstanceId"> the id of the case instance
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case instance id is null </exception>
	  CaseInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Select case instances with the given business key
	  /// </summary>
	  /// <param name="caseInstanceBusinessKey"> the business key of the case instance
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case instance business key is null </exception>
	  CaseInstanceQuery caseInstanceBusinessKey(string caseInstanceBusinessKey);

	  /// <summary>
	  /// Select the case instances which are defined by a case definition with
	  /// the given key.
	  /// </summary>
	  /// <param name="caseDefinitionKey"> the key of the case definition
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case definition key is null </exception>
	  CaseInstanceQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Selects the case instances which are defined by a case definition
	  /// with the given id.
	  /// </summary>
	  /// <param name="caseDefinitionId"> the id of the case definition
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case definition id is null </exception>
	  CaseInstanceQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Selects the case instances which belong to the given deployment id.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  CaseInstanceQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Select the case instances which are a sub case instance of the given
	  /// super process instance.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  CaseInstanceQuery superProcessInstanceId(string superProcessInstanceId);

	  /// <summary>
	  /// Select the case instance that has as sub process instance the given
	  /// process instance. Note that there will always be at most <b>one</b>
	  /// such case instance that can be the result of this query.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  CaseInstanceQuery subProcessInstanceId(string subProcessInstanceId);

	  /// <summary>
	  /// Select the case instances which are a sub case instance of the given
	  /// super case instance.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  CaseInstanceQuery superCaseInstanceId(string superCaseInstanceId);

	  /// <summary>
	  /// Select the case instance that has as sub case instance the given
	  /// case instance. Note that there will always be at most <b>one</b>
	  /// such process instance that can be the result of this query.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  CaseInstanceQuery subCaseInstanceId(string subCaseInstanceId);

	  /// <summary>
	  /// Only select case instances which are active. * </summary>
	  CaseInstanceQuery active();

	  /// <summary>
	  /// Only select case instances which are completed. * </summary>
	  CaseInstanceQuery completed();

	  /// <summary>
	  /// Only select case instances which are terminated. * </summary>
	  CaseInstanceQuery terminated();

	  /// <summary>
	  /// Only select cases instances which have a global variable with the given value. The type
	  /// of variable is determined based on the value, using types configured in
	  /// <seealso cref="ProcessEngineConfigurationImpl#getVariableSerializers()"/>.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> the name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null </exception>
	  CaseInstanceQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select cases instances which have a global variable with the given name, but
	  /// with a different value than the passed value.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null </param>
	  /// <param name="value"> the value of the variable
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null
	  ///  </exception>
	  CaseInstanceQuery variableValueNotEquals(string name, object value);


	  /// <summary>
	  /// Only select cases instances which have a global variable value greater than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null </param>
	  /// <param name="value"> variable value, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseInstanceQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select cases instances which have a global variable value greater than or equal to
	  /// the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null </param>
	  /// <param name="value"> variable value, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseInstanceQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select cases instances which have a global variable value less than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null </param>
	  /// <param name="value"> variable value, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseInstanceQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select cases instances which have a global variable value less than or equal to the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null </param>
	  /// <param name="value"> variable value, cannot be null
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseInstanceQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select cases instances which have a global variable value like the given value.
	  /// This can be used on string variables only.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null </param>
	  /// <param name="value"> variable value, cannot be null. The string can include the
	  ///              wildcard character '%' to express like-strategy:
	  ///              starts with (string%), ends with (%string) or contains (%string%).
	  /// </param>
	  /// <exception cref="NotValidException"> when the given name is null or a null-value or a boolean-value is used
	  ///  </exception>
	  CaseInstanceQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only select case instances with one of the given tenant ids. </summary>
	  CaseInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select case instances which have no tenant id. </summary>
	  CaseInstanceQuery withoutTenantId();

	  //ordering /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  CaseInstanceQuery orderByCaseInstanceId();

	  /// <summary>
	  /// Order by case definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  CaseInstanceQuery orderByCaseDefinitionKey();

	  /// <summary>
	  /// Order by case definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  CaseInstanceQuery orderByCaseDefinitionId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of case instances without tenant id is database-specific.
	  /// </summary>
	  CaseInstanceQuery orderByTenantId();

	}

}