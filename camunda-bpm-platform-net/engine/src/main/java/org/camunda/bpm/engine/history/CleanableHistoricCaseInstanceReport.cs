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
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Defines a report query for cleanable case instances.
	/// 
	/// </summary>
	public interface CleanableHistoricCaseInstanceReport : Query<CleanableHistoricCaseInstanceReport, CleanableHistoricCaseInstanceReportResult>
	{

	  /// <summary>
	  /// Only takes historic case instances into account for the given case definition ids.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given ids is null </exception>
	  CleanableHistoricCaseInstanceReport caseDefinitionIdIn(params string[] caseDefinitionIds);

	  /// <summary>
	  /// Only takes historic case instances into account for the given case definition keys.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given keys is null </exception>
	  CleanableHistoricCaseInstanceReport caseDefinitionKeyIn(params string[] caseDefinitionKeys);

	  /// <summary>
	  /// Only select historic case instances with one of the given tenant ids.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given ids is null </exception>
	  CleanableHistoricCaseInstanceReport tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects historic case instances which have no tenant id.
	  /// </summary>
	  CleanableHistoricCaseInstanceReport withoutTenantId();

	  /// <summary>
	  /// Only selects historic case instances which have more than zero finished instances.
	  /// </summary>
	  CleanableHistoricCaseInstanceReport compact();

	  /// <summary>
	  /// Order by finished case instances amount (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  CleanableHistoricCaseInstanceReport orderByFinished();

	}

}