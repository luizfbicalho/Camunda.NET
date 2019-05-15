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
	/// Defines a report query for finished decision instances.
	/// 
	/// </summary>
	public interface CleanableHistoricDecisionInstanceReport : Query<CleanableHistoricDecisionInstanceReport, CleanableHistoricDecisionInstanceReportResult>
	{

	  /// <summary>
	  /// Only takes historic decision instances into account for the given decision definition ids.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given ids is null </exception>
	  CleanableHistoricDecisionInstanceReport decisionDefinitionIdIn(params string[] decisionDefinitionIds);

	  /// <summary>
	  /// Only takes historic decision instances into account for the given decision definition keys.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given keys is null </exception>
	  CleanableHistoricDecisionInstanceReport decisionDefinitionKeyIn(params string[] decisionDefinitionKeys);

	  /// <summary>
	  /// Only select historic decision instances with one of the given tenant ids.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given ids is null </exception>
	  CleanableHistoricDecisionInstanceReport tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects historic decision instances which have no tenant id.
	  /// </summary>
	  CleanableHistoricDecisionInstanceReport withoutTenantId();

	  /// <summary>
	  /// Only selects historic decision instances which have more than zero finished instances.
	  /// </summary>
	  CleanableHistoricDecisionInstanceReport compact();

	  /// <summary>
	  /// Order by finished decision instances amount (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  CleanableHistoricDecisionInstanceReport orderByFinished();

	}

}