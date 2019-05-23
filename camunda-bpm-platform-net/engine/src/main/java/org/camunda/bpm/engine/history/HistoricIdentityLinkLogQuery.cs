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

	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public interface HistoricIdentityLinkLogQuery : Query<HistoricIdentityLinkLogQuery, HistoricIdentityLinkLog>
	{

	  /// <summary>
	  /// Only select historic identity links which have the date before the give date. * </summary>
		HistoricIdentityLinkLogQuery dateBefore(DateTime dateBefore);

		/// <summary>
		/// Only select historic identity links which have the date after the give date. * </summary>
		HistoricIdentityLinkLogQuery dateAfter(DateTime dateAfter);

		/// <summary>
		/// Only select historic identity links which have the given identity link type. * </summary>
		HistoricIdentityLinkLogQuery type(string type);

		/// <summary>
		/// Only select historic identity links which have the given user id. * </summary>
		HistoricIdentityLinkLogQuery userId(string userId);

		/// <summary>
		/// Only select historic identity links which have the given group id. * </summary>
		HistoricIdentityLinkLogQuery groupId(string groupId);

		/// <summary>
		/// Only select historic identity links which have the given task id. * </summary>
		HistoricIdentityLinkLogQuery taskId(string taskId);

		/// <summary>
		/// Only select historic identity links which have the given process definition id. * </summary>
		HistoricIdentityLinkLogQuery processDefinitionId(string processDefinitionId);

		/// <summary>
		/// Only select historic identity links which have the given process definition key. * </summary>
	  HistoricIdentityLinkLogQuery processDefinitionKey(string processDefinitionKey);

		/// <summary>
		/// Only select historic identity links which have the given operation type (add/delete). * </summary>
		HistoricIdentityLinkLogQuery operationType(string operationType);

		/// <summary>
		/// Only select historic identity links which have the given assigner id. * </summary>
		HistoricIdentityLinkLogQuery assignerId(string assignerId);

		/// <summary>
		/// Only select historic identity links which have the given tenant id. * </summary>
		HistoricIdentityLinkLogQuery tenantIdIn(params string[] tenantId);

		/// <summary>
		/// Order by time (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIdentityLinkLogQuery orderByTime();

		/// <summary>
		/// Order by type (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByType();

		/// <summary>
		/// Order by userId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByUserId();

		/// <summary>
		/// Order by groupId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByGroupId();

		/// <summary>
		/// Order by taskId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByTaskId();

		/// <summary>
		/// Order by processDefinitionId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByProcessDefinitionId();

		/// <summary>
		/// Order by processDefinitionKey (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIdentityLinkLogQuery orderByProcessDefinitionKey();

		/// <summary>
		/// Order by operationType (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByOperationType();

		/// <summary>
		/// Order by assignerId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
		HistoricIdentityLinkLogQuery orderByAssignerId();

		 /// <summary>
		 /// Order by tenantId (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricIdentityLinkLogQuery orderByTenantId();

	}

}