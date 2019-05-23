using System;
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
namespace org.camunda.bpm.engine.history
{
	using Query = org.camunda.bpm.engine.query.Query;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="HistoricProcessInstance"/>s.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public interface HistoricProcessInstanceQuery : Query<HistoricProcessInstanceQuery, HistoricProcessInstance>
	{

	  /// <summary>
	  /// Only select historic process instances with the given process instance.
	  /// {@link ProcessInstance) ids and <seealso cref="HistoricProcessInstance"/> ids match. 
	  /// </summary>
	  HistoricProcessInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic process instances whose id is in the given set of ids.
	  /// {@link ProcessInstance) ids and <seealso cref="HistoricProcessInstance"/> ids match. 
	  /// </summary>
	  HistoricProcessInstanceQuery processInstanceIds(ISet<string> processInstanceIds);

	  /// <summary>
	  /// Only select historic process instances for the given process definition </summary>
	  HistoricProcessInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic process instances that are defined by a process
	  /// definition with the given key.  
	  /// </summary>
	  HistoricProcessInstanceQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select historic process instances that don't have a process-definition of which the key is present in the given list </summary>
	  HistoricProcessInstanceQuery processDefinitionKeyNotIn(IList<string> processDefinitionKeys);

	  /// <summary>
	  /// Only select historic process instances that are defined by a process
	  /// definition with the given name.  
	  /// </summary>
	  HistoricProcessInstanceQuery processDefinitionName(string processDefinitionName);

	  /// <summary>
	  /// Only select historic process instances that are defined by process definition which name
	  /// is like the given value.
	  /// </summary>
	  /// <param name="nameLike"> The string can include the wildcard character '%' to express
	  ///    like-strategy: starts with (string%), ends with (%string) or contains (%string%). </param>
	  HistoricProcessInstanceQuery processDefinitionNameLike(string nameLike);

	  /// <summary>
	  /// Only select historic process instances with the given business key </summary>
	  HistoricProcessInstanceQuery processInstanceBusinessKey(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only select historic process instances which had a business key like the given value.
	  /// </summary>
	  /// <param name="processInstanceBusinessKeyLike"> The string can include the wildcard character '%' to express
	  ///    like-strategy: starts with (string%), ends with (%string) or contains (%string%). </param>
	  HistoricProcessInstanceQuery processInstanceBusinessKeyLike(string processInstanceBusinessKeyLike);

	  /// <summary>
	  /// Only select historic process instances that are completely finished. </summary>
	  HistoricProcessInstanceQuery finished();

	  /// <summary>
	  /// Only select historic process instance that are not yet finished. </summary>
	  HistoricProcessInstanceQuery unfinished();

	  /// <summary>
	  /// Only select historic process instances with incidents
	  /// </summary>
	  /// <returns> HistoricProcessInstanceQuery </returns>
	  HistoricProcessInstanceQuery withIncidents();

	  /// <summary>
	  /// Only select historic process instances with root incidents
	  /// </summary>
	  /// <returns> HistoricProcessInstanceQuery </returns>
	  HistoricProcessInstanceQuery withRootIncidents();

	  /// <summary>
	  /// Only select historic process instances with incident status either 'open' or 'resolved'.
	  /// To get all process instances with incidents, use <seealso cref="HistoricProcessInstanceQuery.withIncidents()"/>.
	  /// </summary>
	  /// <param name="status"> indicates the incident status, which is either 'open' or 'resolved' </param>
	  /// <returns> <seealso cref="HistoricProcessInstanceQuery"/> </returns>
	  HistoricProcessInstanceQuery incidentStatus(string status);

	  /// <summary>
	  /// Only selects process instances with the given incident type.
	  /// </summary>
	  HistoricProcessInstanceQuery incidentType(string incidentType);

	  /// <summary>
	  /// Only select historic process instances with the given incident message.
	  /// </summary>
	  /// <param name="incidentMessage"> Incidents Message for which the historic process instances should be selected
	  /// </param>
	  /// <returns> HistoricProcessInstanceQuery </returns>
	  HistoricProcessInstanceQuery incidentMessage(string incidentMessage);

	  /// <summary>
	  /// Only select historic process instances which had an incident message like the given value.
	  /// </summary>
	  /// <param name="incidentMessageLike"> The string can include the wildcard character '%' to express
	  ///    like-strategy: starts with (string%), ends with (%string) or contains (%string%).
	  /// </param>
	  /// <returns> HistoricProcessInstanceQuery </returns>
	  HistoricProcessInstanceQuery incidentMessageLike(string incidentMessageLike);

	  /// <summary>
	  /// Only select historic process instances which are associated with the given case instance id. </summary>
	  HistoricProcessInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select process instances which had a global variable with the given value
	  /// when they ended. Only select process instances which have a variable value
	  /// greater than the passed value. The type only applies to already ended
	  /// process instances, otherwise use a <seealso cref="ProcessInstanceQuery"/> instead! of
	  /// variable is determined based on the value, using types configured in
	  /// <seealso cref="ProcessEngineConfiguration.getVariableSerializers()"/>. Byte-arrays and
	  /// <seealso cref="Serializable"/> objects (which are not primitive type wrappers) are
	  /// not supported. </summary>
	  /// <param name="name"> of the variable, cannot be null.  </param>
	  HistoricProcessInstanceQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select process instances which had a global variable with the given name, but
	  /// with a different value than the passed value when they ended. Only select
	  /// process instances which have a variable value greater than the passed
	  /// value. Byte-arrays and <seealso cref="Serializable"/> objects (which are not
	  /// primitive type wrappers) are not supported. </summary>
	  /// <param name="name"> of the variable, cannot be null.  </param>
	  HistoricProcessInstanceQuery variableValueNotEquals(string name, object value);

	  /// <summary>
	  /// Only select process instances which had a global variable value greater than the
	  /// passed value when they ended. Booleans, Byte-arrays and
	  /// <seealso cref="Serializable"/> objects (which are not primitive type wrappers) are
	  /// not supported. Only select process instances which have a variable value
	  /// greater than the passed value. </summary>
	  /// <param name="name"> cannot be null. </param>
	  /// <param name="value"> cannot be null.  </param>
	  HistoricProcessInstanceQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select process instances which had a global variable value greater than or
	  /// equal to the passed value when they ended. Booleans, Byte-arrays and
	  /// <seealso cref="Serializable"/> objects (which are not primitive type wrappers) are
	  /// not supported. Only applies to already ended process instances, otherwise
	  /// use a <seealso cref="ProcessInstanceQuery"/> instead! </summary>
	  /// <param name="name"> cannot be null. </param>
	  /// <param name="value"> cannot be null.  </param>
	  HistoricProcessInstanceQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select process instances which had a global variable value less than the
	  /// passed value when the ended. Only applies to already ended process
	  /// instances, otherwise use a <seealso cref="ProcessInstanceQuery"/> instead! Booleans,
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type
	  /// wrappers) are not supported. </summary>
	  /// <param name="name"> cannot be null. </param>
	  /// <param name="value"> cannot be null.  </param>
	  HistoricProcessInstanceQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select process instances which has a global variable value less than or equal
	  /// to the passed value when they ended. Only applies to already ended process
	  /// instances, otherwise use a <seealso cref="ProcessInstanceQuery"/> instead! Booleans,
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type
	  /// wrappers) are not supported. </summary>
	  /// <param name="name"> cannot be null. </param>
	  /// <param name="value"> cannot be null.  </param>
	  HistoricProcessInstanceQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select process instances which had global variable value like the given value
	  /// when they ended. Only applies to already ended process instances, otherwise
	  /// use a <seealso cref="ProcessInstanceQuery"/> instead! This can be used on string
	  /// variables only. </summary>
	  /// <param name="name"> cannot be null. </param>
	  /// <param name="value"> cannot be null. The string can include the
	  ///          wildcard character '%' to express like-strategy: starts with
	  ///          (string%), ends with (%string) or contains (%string%).  </param>
	  HistoricProcessInstanceQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only select historic process instances that were started before the given date. </summary>
	  HistoricProcessInstanceQuery startedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that were started after the given date. </summary>
	  HistoricProcessInstanceQuery startedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that were started before the given date. </summary>
	  HistoricProcessInstanceQuery finishedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that were started after the given date. </summary>
	  HistoricProcessInstanceQuery finishedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic process instance that are started by the given user. </summary>
	  HistoricProcessInstanceQuery startedBy(string userId);

	  /// <summary>
	  /// Order by the process instance id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by the process definition id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by the process definition key (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by the process definition name (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessDefinitionName();

	  /// <summary>
	  /// Order by the process definition version (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessDefinitionVersion();

	  /// <summary>
	  /// Order by the business key (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessInstanceBusinessKey();

	  /// <summary>
	  /// Order by the start time (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessInstanceStartTime();

	  /// <summary>
	  /// Order by the end time (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessInstanceEndTime();

	  /// <summary>
	  /// Order by the duration of the process instance (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricProcessInstanceQuery orderByProcessInstanceDuration();

	  /// <summary>
	  /// Only select historic process instances that are top level process instances. </summary>
	  HistoricProcessInstanceQuery rootProcessInstances();

	  /// <summary>
	  /// Only select historic process instances started by the given process
	  /// instance. {@link ProcessInstance) ids and <seealso cref="HistoricProcessInstance"/>
	  /// ids match. 
	  /// </summary>
	  HistoricProcessInstanceQuery superProcessInstanceId(string superProcessInstanceId);

	  /// <summary>
	  /// Only select historic process instances having a sub process instance
	  /// with the given process instance id.
	  /// 
	  /// Note that there will always be maximum only <b>one</b>
	  /// such process instance that can be the result of this query.
	  /// </summary>
	  HistoricProcessInstanceQuery subProcessInstanceId(string subProcessInstanceId);

	  /// <summary>
	  /// Only select historic process instances started by the given case
	  /// instance. 
	  /// </summary>
	  HistoricProcessInstanceQuery superCaseInstanceId(string superCaseInstanceId);

	  /// <summary>
	  /// Only select historic process instances having a sub case instance
	  /// with the given case instance id.
	  /// 
	  /// Note that there will always be maximum only <b>one</b>
	  /// such process instance that can be the result of this query.
	  /// </summary>
	  HistoricProcessInstanceQuery subCaseInstanceId(string subCaseInstanceId);

	  /// <summary>
	  /// Only select historic process instances with one of the given tenant ids. </summary>
	  HistoricProcessInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects historic process instances which have no tenant id. </summary>
	  HistoricProcessInstanceQuery withoutTenantId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of historic process instances without tenant id is database-specific.
	  /// </summary>
	  HistoricProcessInstanceQuery orderByTenantId();

	  /// <summary>
	  /// Only select historic process instances that were started as of the provided
	  /// date. (Date will be adjusted to reflect midnight) </summary>
	  /// @deprecated use <seealso cref="startedAfter(System.DateTime)"/> and <seealso cref="startedBefore(System.DateTime)"/> instead  
	  [Obsolete("use <seealso cref=\"startedAfter(System.DateTime)\"/> and <seealso cref=\"startedBefore(System.DateTime)\"/> instead")]
	  HistoricProcessInstanceQuery startDateBy(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that were started on the provided date. </summary>
	  /// @deprecated use <seealso cref="startedAfter(System.DateTime)"/> and <seealso cref="startedBefore(System.DateTime)"/> instead  
	  [Obsolete("use <seealso cref=\"startedAfter(System.DateTime)\"/> and <seealso cref=\"startedBefore(System.DateTime)\"/> instead")]
	  HistoricProcessInstanceQuery startDateOn(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that were finished as of the
	  /// provided date. (Date will be adjusted to reflect one second before midnight) </summary>
	  /// @deprecated use <seealso cref="startedAfter(System.DateTime)"/> and <seealso cref="startedBefore(System.DateTime)"/> instead  
	  [Obsolete("use <seealso cref=\"startedAfter(System.DateTime)\"/> and <seealso cref=\"startedBefore(System.DateTime)\"/> instead")]
	  HistoricProcessInstanceQuery finishDateBy(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that were finished on provided date. </summary>
	  /// @deprecated use <seealso cref="startedAfter(System.DateTime)"/> and <seealso cref="startedBefore(System.DateTime)"/> instead  
	  [Obsolete("use <seealso cref=\"startedAfter(System.DateTime)\"/> and <seealso cref=\"startedBefore(System.DateTime)\"/> instead")]
	  HistoricProcessInstanceQuery finishDateOn(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that executed an activity after the given date. </summary>
	  HistoricProcessInstanceQuery executedActivityAfter(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that executed an activity before the given date. </summary>
	  HistoricProcessInstanceQuery executedActivityBefore(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that executed activities with given ids. </summary>
	  HistoricProcessInstanceQuery executedActivityIdIn(params string[] ids);

	  /// <summary>
	  /// Only select historic process instances that have active activities with given ids. </summary>
	  HistoricProcessInstanceQuery activeActivityIdIn(params string[] ids);

	  /// <summary>
	  /// Only select historic process instances that executed an job after the given date. </summary>
	  HistoricProcessInstanceQuery executedJobAfter(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that executed an job before the given date. </summary>
	  HistoricProcessInstanceQuery executedJobBefore(DateTime date);

	  /// <summary>
	  /// Only select historic process instances that are active. </summary>
	  HistoricProcessInstanceQuery active();

	  /// <summary>
	  /// Only select historic process instances that are suspended. </summary>
	  HistoricProcessInstanceQuery suspended();

	  /// <summary>
	  /// Only select historic process instances that are completed. </summary>
	  HistoricProcessInstanceQuery completed();

	  /// <summary>
	  /// Only select historic process instances that are externallyTerminated. </summary>
	  HistoricProcessInstanceQuery externallyTerminated();

	  /// <summary>
	  /// Only select historic process instances that are internallyTerminated. </summary>
	  HistoricProcessInstanceQuery internallyTerminated();
	}

}