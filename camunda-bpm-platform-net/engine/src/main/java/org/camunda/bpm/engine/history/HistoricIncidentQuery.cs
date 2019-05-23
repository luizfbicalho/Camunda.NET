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

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HistoricIncidentQuery : Query<HistoricIncidentQuery, HistoricIncident>
	{

	  /// <summary>
	  /// Only select historic incidents which have the given id. * </summary>
	  HistoricIncidentQuery incidentId(string incidentId);

	  /// <summary>
	  /// Only select historic incidents which have the given incident type. * </summary>
	  HistoricIncidentQuery incidentType(string incidentType);

	  /// <summary>
	  /// Only select historic incidents which have the given incident message. * </summary>
	  HistoricIncidentQuery incidentMessage(string incidentMessage);

	  /// <summary>
	  /// Only select historic incidents which have the given process definition id. * </summary>
	  HistoricIncidentQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic incidents which have the given process instance id. * </summary>
	  HistoricIncidentQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic incidents with the given id. * </summary>
	  HistoricIncidentQuery executionId(string executionId);

	  /// <summary>
	  /// Only select historic incidents which contain an activity with the given id. * </summary>
	  HistoricIncidentQuery activityId(string activityId);

	  /// <summary>
	  /// Only select historic incidents which contain the id of the cause incident. * </summary>
	  HistoricIncidentQuery causeIncidentId(string causeIncidentId);

	  /// <summary>
	  /// Only select historic incidents which contain the id of the root cause incident. * </summary>
	  HistoricIncidentQuery rootCauseIncidentId(string rootCauseIncidentId);

	  /// <summary>
	  /// Only select historic incidents that belong to one of the given tenant ids. </summary>
	  HistoricIncidentQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select incidents which contain the configuration. * </summary>
	  HistoricIncidentQuery configuration(string configuration);

	  /// <summary>
	  /// Only select incidents that belong to one of the given job definition ids. </summary>
	  HistoricIncidentQuery jobDefinitionIdIn(params string[] jobDefinitionIds);

	  /// <summary>
	  /// Only select historic incidents which are open. * </summary>
	  HistoricIncidentQuery open();

	  /// <summary>
	  /// Only select historic incidents which are resolved. * </summary>
	  HistoricIncidentQuery resolved();

	  /// <summary>
	  /// Only select historic incidents which are deleted. * </summary>
	  HistoricIncidentQuery deleted();

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByIncidentId();

	  /// <summary>
	  /// Order by message (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByIncidentMessage();

	  /// <summary>
	  /// Order by create time (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByCreateTime();

	  /// <summary>
	  /// Order by end time (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByEndTime();

	  /// <summary>
	  /// Order by incidentType (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByIncidentType();

	  /// <summary>
	  /// Order by executionId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByExecutionId();

	  /// <summary>
	  /// Order by activityId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByActivityId();

	  /// <summary>
	  /// Order by processInstanceId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by processDefinitionId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by causeIncidentId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByCauseIncidentId();

	  /// <summary>
	  /// Order by rootCauseIncidentId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByRootCauseIncidentId();

	  /// <summary>
	  /// Order by configuration (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByConfiguration();

	  /// <summary>
	  /// Order by incidentState (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIncidentQuery orderByIncidentState();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of incidents without tenant id is database-specific.
	  /// </summary>
	  HistoricIncidentQuery orderByTenantId();

	}

}