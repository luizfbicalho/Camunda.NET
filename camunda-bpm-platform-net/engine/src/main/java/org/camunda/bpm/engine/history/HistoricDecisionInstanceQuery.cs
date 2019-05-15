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
	/// Allows programmatic querying of <seealso cref="HistoricDecisionInstance"/>s.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public interface HistoricDecisionInstanceQuery : Query<HistoricDecisionInstanceQuery, HistoricDecisionInstance>
	{

	  /// <summary>
	  /// Only select historic decision instances with the given decision instance id. </summary>
	  HistoricDecisionInstanceQuery decisionInstanceId(string decisionInstanceId);

	  /// <summary>
	  /// Only select historic decision instances whose id is in the given list of ids. </summary>
	  HistoricDecisionInstanceQuery decisionInstanceIdIn(params string[] decisionInstanceIdIn);

	  /// <summary>
	  /// Only select historic decision instances for the given decision definition </summary>
	  HistoricDecisionInstanceQuery decisionDefinitionId(string decisionDefinitionId);

	  /// <summary>
	  /// Only select historic decision instances for the given decision definitions </summary>
	  HistoricDecisionInstanceQuery decisionDefinitionIdIn(params string[] decisionDefinitionIdIn);

	  /// <summary>
	  /// Only select historic decision instances with the given key of the decision definition. </summary>
	  HistoricDecisionInstanceQuery decisionDefinitionKey(string decisionDefinitionKey);

	  /// <summary>
	  /// Only select historic decision instances with the given keys of the decision definition. </summary>
	  HistoricDecisionInstanceQuery decisionDefinitionKeyIn(params string[] decisionDefinitionKeyIn);

	  /// <summary>
	  /// Only select historic decision instances with the given name of the decision definition. </summary>
	  HistoricDecisionInstanceQuery decisionDefinitionName(string decisionDefinitionName);

	  /// <summary>
	  /// Only select historic decision instances with the given name of the decision definition using LIKE construct.
	  /// </summary>
	  HistoricDecisionInstanceQuery decisionDefinitionNameLike(string decisionDefinitionNameLike);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a process
	  /// with the given process definition key. 
	  /// </summary>
	  HistoricDecisionInstanceQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a process
	  /// with the given process definition id. 
	  /// </summary>
	  HistoricDecisionInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a process
	  /// with the given process instance id. 
	  /// </summary>
	  HistoricDecisionInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a case
	  /// with the given case definition key. 
	  /// </summary>
	  HistoricDecisionInstanceQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a case
	  /// with the given case definition id. 
	  /// </summary>
	  HistoricDecisionInstanceQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a case
	  /// with the given case instance id. 
	  /// </summary>
	  HistoricDecisionInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a process or a case
	  /// which have one of the activity ids. 
	  /// </summary>
	  HistoricDecisionInstanceQuery activityIdIn(params string[] activityIds);

	  /// <summary>
	  /// Only select historic decision instances that are evaluated inside a process or a case
	  /// which have one of the activity instance ids. 
	  /// </summary>
	  HistoricDecisionInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds);

	  /// <summary>
	  /// Only select historic decision instances that were evaluated before the given date. </summary>
	  HistoricDecisionInstanceQuery evaluatedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic decision instances that were evaluated after the given date. </summary>
	  HistoricDecisionInstanceQuery evaluatedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic decision instances that were evaluated by the user with the given user ID.
	  /// <para> The user ID is saved for decisions which are evaluated by a authenticated user without a process or
	  /// case instance 
	  /// </para>
	  /// </summary>
	  HistoricDecisionInstanceQuery userId(string userId);

	  /// <summary>
	  /// Enable fetching <seealso cref="HistoricDecisionInputInstance"/> of evaluated decision. </summary>
	  HistoricDecisionInstanceQuery includeInputs();

	  /// <summary>
	  /// Enable fetching <seealso cref="HistoricDecisionOutputInstance"/> of evaluated decision. </summary>
	  HistoricDecisionInstanceQuery includeOutputs();

	  /// <summary>
	  /// Disable fetching of byte array input and output values. By default, the query will fetch the value of a byte array.
	  /// By calling this method you can prevent the values of (potentially large) blob data chunks to be fetched. 
	  /// </summary>
	  HistoricDecisionInstanceQuery disableBinaryFetching();

	  /// <summary>
	  /// Disable deserialization of input and output values that are custom objects. By default, the query
	  /// will attempt to deserialize the value of these variables. By calling this method you can
	  /// prevent such attempts in environments where their classes are not available.
	  /// Independent of this setting, variable serialized values are accessible. 
	  /// </summary>
	  HistoricDecisionInstanceQuery disableCustomObjectDeserialization();

	  /// <summary>
	  /// Only select historic decision instances with a given root historic decision
	  /// instance id. This also includes the historic decision instance with the
	  /// given id.
	  /// </summary>
	  HistoricDecisionInstanceQuery rootDecisionInstanceId(string decisionInstanceId);

	  /// <summary>
	  /// Only select historic decision instances that are the root decision instance of an evaluation. </summary>
	  HistoricDecisionInstanceQuery rootDecisionInstancesOnly();

	  /// <summary>
	  /// Only select historic decision instances that belongs to a decision requirements definition with the given id. </summary>
	  HistoricDecisionInstanceQuery decisionRequirementsDefinitionId(string decisionRequirementsDefinitionId);

	  /// <summary>
	  /// Only select historic decision instances that belongs to a decision requirements definition with the given key. </summary>
	  HistoricDecisionInstanceQuery decisionRequirementsDefinitionKey(string decisionRequirementsDefinitionKey);

	  /// <summary>
	  /// Only select historic decision instances with one of the given tenant ids. </summary>
	  HistoricDecisionInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Order by the time when the decisions was evaluated
	  /// (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  HistoricDecisionInstanceQuery orderByEvaluationTime();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of historic decision instances without tenant id is database-specific.
	  /// </summary>
	  HistoricDecisionInstanceQuery orderByTenantId();

	}

}