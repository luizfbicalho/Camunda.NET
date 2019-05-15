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

	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="ProcessInstance"/>s.
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// </summary>
	public interface ProcessInstanceQuery : Query<ProcessInstanceQuery, ProcessInstance>
	{

	  /// <summary>
	  /// Select the process instance with the given id </summary>
	  ProcessInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Select process instances whose id is in the given set of ids </summary>
	  ProcessInstanceQuery processInstanceIds(ISet<string> processInstanceIds);

	  /// <summary>
	  /// Select process instances with the given business key </summary>
	  ProcessInstanceQuery processInstanceBusinessKey(string processInstanceBusinessKey);

	  /// <summary>
	  /// Select process instance with the given business key, unique for the given process definition </summary>
	  ProcessInstanceQuery processInstanceBusinessKey(string processInstanceBusinessKey, string processDefinitionKey);

	  /// <summary>
	  /// Select process instances with a business key like the given value.
	  /// </summary>
	  /// <param name="processInstanceBusinessKeyLike"> The string can include the wildcard character '%' to express
	  ///    like-strategy: starts with (string%), ends with (%string) or contains (%string%). </param>
	  ProcessInstanceQuery processInstanceBusinessKeyLike(string processInstanceBusinessKeyLike);

	  /// <summary>
	  /// Select the process instances which are defined by a process definition with
	  /// the given key.
	  /// </summary>
	  ProcessInstanceQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Selects the process instances which are defined by a process definition
	  /// with the given id.
	  /// </summary>
	  ProcessInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Selects the process instances which belong to the given deployment id.
	  /// @since 7.4
	  /// </summary>
	  ProcessInstanceQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Select the process instances which are a sub process instance of the given
	  /// super process instance.
	  /// </summary>
	  ProcessInstanceQuery superProcessInstanceId(string superProcessInstanceId);

	  /// <summary>
	  /// Select the process instance that have as sub process instance the given
	  /// process instance. Note that there will always be maximum only <b>one</b>
	  /// such process instance that can be the result of this query.
	  /// </summary>
	  ProcessInstanceQuery subProcessInstanceId(string subProcessInstanceId);

	  /// <summary>
	  /// Selects the process instances which are associated with the given case instance id.
	  /// </summary>
	  ProcessInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Select the process instances which are a sub process instance of the given
	  /// super case instance.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  ProcessInstanceQuery superCaseInstanceId(string superCaseInstanceId);

	  /// <summary>
	  /// Select the process instance that has as sub case instance the given
	  /// case instance. Note that there will always be at most <b>one</b>
	  /// such process instance that can be the result of this query.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  ProcessInstanceQuery subCaseInstanceId(string subCaseInstanceId);

	  /// <summary>
	  /// Only select process instances which have a global variable with the given value. The type
	  /// of variable is determined based on the value, using types configured in
	  /// <seealso cref="ProcessEngineConfiguration#getVariableSerializers()"/>.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  ProcessInstanceQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select process instances which have a global variable with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  ProcessInstanceQuery variableValueNotEquals(string name, object value);


	  /// <summary>
	  /// Only select process instances which have a variable value greater than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ProcessInstanceQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select process instances which have a global variable value greater than or equal to
	  /// the passed value. Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ProcessInstanceQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select process instances which have a global variable value less than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ProcessInstanceQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select process instances which have a global variable value less than or equal to the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ProcessInstanceQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select process instances which have a global variable value like the given value.
	  /// This be used on string variables only. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. The string can include the
	  /// wildcard character '%' to express like-strategy:
	  /// starts with (string%), ends with (%string) or contains (%string%). </param>
	  ProcessInstanceQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only selects process instances which are suspended, either because the
	  /// process instance itself is suspended or because the corresponding process
	  /// definition is suspended
	  /// </summary>
	  ProcessInstanceQuery suspended();

	  /// <summary>
	  /// Only selects process instances which are active, which means that
	  /// neither the process instance nor the corresponding process definition
	  /// are suspended.
	  /// </summary>
	  ProcessInstanceQuery active();

	  /// <summary>
	  /// Only selects process instances with at least one incident.
	  /// </summary>
	  ProcessInstanceQuery withIncident();

	  /// <summary>
	  /// Only selects process instances with the given incident type.
	  /// </summary>
	  ProcessInstanceQuery incidentType(string incidentType);

	  /// <summary>
	  /// Only selects process instances with the given incident id.
	  /// </summary>
	  ProcessInstanceQuery incidentId(string incidentId);

	  /// <summary>
	  /// Only selects process instances with the given incident message.
	  /// </summary>
	  ProcessInstanceQuery incidentMessage(string incidentMessage);

	  /// <summary>
	  /// Only selects process instances with an incident message like the given.
	  /// </summary>
	  ProcessInstanceQuery incidentMessageLike(string incidentMessageLike);

	  /// <summary>
	  /// Only select process instances with one of the given tenant ids. </summary>
	  ProcessInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects process instances which have no tenant id. </summary>
	  ProcessInstanceQuery withoutTenantId();

	  /// <summary>
	  /// <para>Only selects process instances with leaf activity instances
	  /// or transition instances (async before, async after) in
	  /// at least one of the given activity ids.
	  /// 
	  /// </para>
	  /// <para><i>Leaf instance</i> means this filter works for instances
	  /// of a user task is matched, but not the embedded sub process it is
	  /// contained in.
	  /// </para>
	  /// </summary>
	  ProcessInstanceQuery activityIdIn(params string[] activityIds);

	  /// <summary>
	  /// Only selects process instances which are top level process instances. </summary>
	  ProcessInstanceQuery rootProcessInstances();

	  /// <summary>
	  /// Only selects process instances which don't have subprocesses and thus are leaves of the execution tree. </summary>
	  ProcessInstanceQuery leafProcessInstances();

	  /// <summary>
	  /// Only selects process instances which process definition has no tenant id. </summary>
	  ProcessInstanceQuery processDefinitionWithoutTenantId();

	  //ordering /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessInstanceQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessInstanceQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessInstanceQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of process instances without tenant id is database-specific.
	  /// </summary>
	  ProcessInstanceQuery orderByTenantId();

	  /// <summary>
	  /// Order by the business key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessInstanceQuery orderByBusinessKey();
	}

}