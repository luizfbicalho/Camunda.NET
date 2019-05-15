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

	/// <summary>
	/// Represents one evaluation of a decision.
	/// 
	/// @author Philipp Ossler
	/// @author Ingo Richtsmeier
	/// 
	/// </summary>
	public interface HistoricDecisionInstance
	{

	  /// <summary>
	  /// The unique identifier of this historic decision instance. </summary>
	  string Id {get;}

	  /// <summary>
	  /// The decision definition reference. </summary>
	  string DecisionDefinitionId {get;}

	  /// <summary>
	  /// The unique identifier of the decision definition </summary>
	  string DecisionDefinitionKey {get;}

	  /// <summary>
	  /// The name of the decision definition </summary>
	  string DecisionDefinitionName {get;}

	  /// <summary>
	  /// Time when the decision was evaluated. </summary>
	  DateTime EvaluationTime {get;}

	  /// <summary>
	  /// Time when the historic decision instance is to be removed. </summary>
	  DateTime RemovalTime {get;}

	  /// <summary>
	  /// The corresponding key of the process definition in case the decision was evaluated inside a process. </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// The corresponding id of the process definition in case the decision was evaluated inside a process. </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// The corresponding process instance in case the decision was evaluated inside a process. </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// The corresponding key of the case definition in case the decision was evaluated inside a case. </summary>
	  string CaseDefinitionKey {get;}

	  /// <summary>
	  /// The corresponding id of the case definition in case the decision was evaluated inside a case. </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// The corresponding case instance in case the decision was evaluated inside a case. </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// The corresponding activity in case the decision was evaluated inside a process or a case. </summary>
	  string ActivityId {get;}

	  /// <summary>
	  /// The corresponding activity instance in case the decision was evaluated inside a process or a case. </summary>
	  string ActivityInstanceId {get;}

	  /// <summary>
	  /// The user ID in case the decision was evaluated by an authenticated user using the decision service
	  /// outside of an execution context.
	  /// </summary>
	  string UserId {get;}

	  /// <summary>
	  /// The input values of the evaluated decision. The fetching of the input values must be enabled on the query.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> if the input values are not fetched.
	  /// </exception>
	  /// <seealso cref= HistoricDecisionInstanceQuery#includeInputs() </seealso>
	  IList<HistoricDecisionInputInstance> Inputs {get;}

	  /// <summary>
	  /// The output values of the evaluated decision. The fetching of the output values must be enabled on the query.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> if the output values are not fetched.
	  /// </exception>
	  /// <seealso cref= HistoricDecisionInstanceQuery#includeOutputs() </seealso>
	  IList<HistoricDecisionOutputInstance> Outputs {get;}

	  /// <summary>
	  /// The result of the collect operation if the hit policy 'collect' was used for the decision. </summary>
	  double? CollectResultValue {get;}

	  /// <summary>
	  /// The unique identifier of the historic decision instance of the evaluated root decision.
	  /// Can be <code>null</code> if this instance is the root decision instance of the evaluation.
	  /// </summary>
	  string RootDecisionInstanceId {get;}

	  /// <summary>
	  /// The unique identifier of the root historic process instance of the evaluated root decision
	  /// in case the decision was evaluated inside a process, otherwise <code>null</code>.
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The id of the related decision requirements definition. Can be
	  /// <code>null</code> if the decision has no relations to other decisions.
	  /// </summary>
	  string DecisionRequirementsDefinitionId {get;}

	  /// <summary>
	  /// The key of the related decision requirements definition. Can be
	  /// <code>null</code> if the decision has no relations to other decisions.
	  /// </summary>
	  string DecisionRequirementsDefinitionKey {get;}

	  /// <summary>
	  /// The id of the tenant this historic decision instance belongs to. Can be <code>null</code>
	  /// if the historic decision instance belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}
	}

}