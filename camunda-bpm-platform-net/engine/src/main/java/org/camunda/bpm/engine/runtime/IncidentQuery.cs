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
	public interface IncidentQuery : Query<IncidentQuery, Incident>
	{

	  /// <summary>
	  /// Only select incidents which have the given id. * </summary>
	  IncidentQuery incidentId(string incidentId);

	  /// <summary>
	  /// Only select incidents which have the given incident type. * </summary>
	  IncidentQuery incidentType(string incidentType);

	  /// <summary>
	  /// Only select incidents which have the given incident message. * </summary>
	  IncidentQuery incidentMessage(string incidentMessage);

	  /// <summary>
	  /// Only select incidents which have the given process definition id. * </summary>
	  IncidentQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select incidents which have the given process instance id. * </summary>
	  IncidentQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select incidents with the given id. * </summary>
	  IncidentQuery executionId(string executionId);

	  /// <summary>
	  /// Only select incidents which contain an activity with the given id. * </summary>
	  IncidentQuery activityId(string activityId);

	  /// <summary>
	  /// Only select incidents which contain the id of the cause incident. * </summary>
	  IncidentQuery causeIncidentId(string causeIncidentId);

	  /// <summary>
	  /// Only select incidents which contain the id of the root cause incident. * </summary>
	  IncidentQuery rootCauseIncidentId(string rootCauseIncidentId);

	  /// <summary>
	  /// Only select incidents which contain the configuration. * </summary>
	  IncidentQuery configuration(string configuration);

	  /// <summary>
	  /// Only select incidents that belong to one of the given tenant ids. </summary>
	  IncidentQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select incidents that belong to one of the given job definition ids. </summary>
	  IncidentQuery jobDefinitionIdIn(params string[] jobDefinitionIds);

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByIncidentId();

	  /// <summary>
	  /// Order by incidentTimestamp (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByIncidentTimestamp();

	  /// <summary>
	  /// Order by incident message (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByIncidentMessage();

	  /// <summary>
	  /// Order by incidentType (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByIncidentType();

	  /// <summary>
	  /// Order by executionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByExecutionId();

	  /// <summary>
	  /// Order by activityId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByActivityId();

	  /// <summary>
	  /// Order by processInstanceId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by processDefinitionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by causeIncidentId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByCauseIncidentId();

	  /// <summary>
	  /// Order by rootCauseIncidentId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByRootCauseIncidentId();

	  /// <summary>
	  /// Order by configuration (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  IncidentQuery orderByConfiguration();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of incidents without tenant id is database-specific.
	  /// </summary>
	  IncidentQuery orderByTenantId();

	}


}