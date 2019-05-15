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
	/// Programmatic querying for <seealso cref="HistoricCaseActivityInstance"/>s.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface HistoricCaseActivityInstanceQuery : Query<HistoricCaseActivityInstanceQuery, HistoricCaseActivityInstance>
	{

	  /// <summary>
	  /// Only select historic case activity instances with the given id (primary key within history tables). </summary>
	  HistoricCaseActivityInstanceQuery caseActivityInstanceId(string caseActivityInstanceId);

	  /// <summary>
	  /// Only select historic case activity instances with one of the given case activity instance ids. </summary>
	  HistoricCaseActivityInstanceQuery caseActivityInstanceIdIn(params string[] caseActivityInstanceIds);

	  /// <summary>
	  /// Only select historic case activity instances for the given case execution </summary>
	  HistoricCaseActivityInstanceQuery caseExecutionId(string caseExecutionId);

	  /// <summary>
	  /// Only select historic case activity instances with the given case instance. </summary>
	  HistoricCaseActivityInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select historic case activity instances for the given case definition </summary>
	  HistoricCaseActivityInstanceQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select historic case activity instances for the given case activity (id from CMMN 1.0 XML) </summary>
	  HistoricCaseActivityInstanceQuery caseActivityId(string caseActivityId);

	  /// <summary>
	  /// Only select historic case activity instances with one of the given case activity ids. </summary>
	  HistoricCaseActivityInstanceQuery caseActivityIdIn(params string[] caseActivityIds);

	  /// <summary>
	  /// Only select historic case activity instances for activities with the given name </summary>
	  HistoricCaseActivityInstanceQuery caseActivityName(string caseActivityName);

	  /// <summary>
	  /// Only select historic case activity instances for activities with the given type </summary>
	  HistoricCaseActivityInstanceQuery caseActivityType(string caseActivityType);

	  /// <summary>
	  /// Only select historic case activity instances that were created before the given date. </summary>
	  HistoricCaseActivityInstanceQuery createdBefore(DateTime date);

	  /// <summary>
	  /// Only select historic case activity instances that were created after the given date. </summary>
	  HistoricCaseActivityInstanceQuery createdAfter(DateTime date);

	  /// <summary>
	  /// Only select historic case activity instances that were ended (ie. completed or terminated) before the given date. </summary>
	  HistoricCaseActivityInstanceQuery endedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic case activity instances that were ended (ie. completed or terminated) after the given date. </summary>
	  HistoricCaseActivityInstanceQuery endedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic case activity instances which are required. </summary>
	  HistoricCaseActivityInstanceQuery required();

	  /// <summary>
	  /// Only select historic case activity instances which are already ended (ie. completed or terminated). </summary>
	  HistoricCaseActivityInstanceQuery ended();

	  /// <summary>
	  /// Only select historic case activity instances which are not ended (ie. completed or terminated). </summary>
	  HistoricCaseActivityInstanceQuery notEnded();

	  /// <summary>
	  /// Only select historic case activity instances which are available </summary>
	  HistoricCaseActivityInstanceQuery available();

	  /// <summary>
	  /// Only select historic case activity instances which are enabled </summary>
	  HistoricCaseActivityInstanceQuery enabled();

	  /// <summary>
	  /// Only select historic case activity instances which are disabled </summary>
	  HistoricCaseActivityInstanceQuery disabled();

	  /// <summary>
	  /// Only select historic case activity instances which are active </summary>
	  HistoricCaseActivityInstanceQuery active();

	  /// <summary>
	  /// Only select historic case activity instances which are completed </summary>
	  HistoricCaseActivityInstanceQuery completed();

	  /// <summary>
	  /// Only select historic case activity instances which are terminated </summary>
	  HistoricCaseActivityInstanceQuery terminated();

	  /// <summary>
	  /// Only select historic case activity instances with one of the given tenant ids. </summary>
	  HistoricCaseActivityInstanceQuery tenantIdIn(params string[] tenantIds);

	  // ordering /////////////////////////////////////////////////////////////////
	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceId();

	  /// <summary>
	  /// Order by caseInstanceId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByCaseInstanceId();

	  /// <summary>
	  /// Order by caseExecutionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByCaseExecutionId();

	  /// <summary>
	  /// Order by caseActivityId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByCaseActivityId();

	  /// <summary>
	  /// Order by caseActivityName (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByCaseActivityName();

	  /// <summary>
	  /// Order by caseActivityType (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByCaseActivityType();

	  /// <summary>
	  /// Order by create time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceCreateTime();

	  /// <summary>
	  /// Order by end time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceEndTime();

	  /// <summary>
	  /// Order by duration (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceDuration();

	  /// <summary>
	  /// Order by caseDefinitionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseActivityInstanceQuery orderByCaseDefinitionId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of historic case activity instances without tenant id is database-specific.
	  /// </summary>
	  HistoricCaseActivityInstanceQuery orderByTenantId();

	}

}