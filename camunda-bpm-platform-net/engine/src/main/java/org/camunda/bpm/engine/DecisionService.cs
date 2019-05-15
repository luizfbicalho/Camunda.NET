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
namespace org.camunda.bpm.engine
{

	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using DecisionEvaluationBuilder = org.camunda.bpm.engine.dmn.DecisionEvaluationBuilder;
	using DecisionsEvaluationBuilder = org.camunda.bpm.engine.dmn.DecisionsEvaluationBuilder;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;

	/// <summary>
	/// Service to evaluate decisions inside the DMN engine.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public interface DecisionService
	{

	  /// <summary>
	  /// Evaluates the decision with the given id.
	  /// </summary>
	  /// <param name="decisionDefinitionId">
	  ///          the id of the decision definition, cannot be null. </param>
	  /// <param name="variables">
	  ///          the input values of the decision. </param>
	  /// <returns> the result of the evaluation.
	  /// </returns>
	  /// <exception cref="NotFoundException">
	  ///           when no decision definition is deployed with the given id.
	  /// </exception>
	  /// <exception cref="NotValidException">
	  ///           when the given decision definition id is null.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE_INSTANCE"/> permission
	  ///           on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  DmnDecisionTableResult evaluateDecisionTableById(string decisionDefinitionId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Evaluates the decision with the given key in the latest version.
	  /// </summary>
	  /// <param name="decisionDefinitionKey">
	  ///          the key of the decision definition, cannot be null. </param>
	  /// <param name="variables">
	  ///          the input values of the decision. </param>
	  /// <returns> the result of the evaluation.
	  /// </returns>
	  /// <exception cref="NotFoundException">
	  ///           when no decision definition is deployed with the given key.
	  /// </exception>
	  /// <exception cref="NotValidException">
	  ///           when the given decision definition key is null.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE_INSTANCE"/> permission
	  ///           on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  DmnDecisionTableResult evaluateDecisionTableByKey(string decisionDefinitionKey, IDictionary<string, object> variables);

	  /// <summary>
	  /// Evaluates the decision with the given key in the specified version. If no
	  /// version is provided then the latest version of the decision definition is
	  /// taken.
	  /// </summary>
	  /// <param name="decisionDefinitionKey">
	  ///          the key of the decision definition, cannot be null. </param>
	  /// <param name="version">
	  ///          the version of the decision definition. If <code>null</code> then
	  ///          the latest version is taken. </param>
	  /// <param name="variables">
	  ///          the input values of the decision. </param>
	  /// <returns> the result of the evaluation.
	  /// </returns>
	  /// <exception cref="NotFoundException">
	  ///           when no decision definition is deployed with the given key and
	  ///           version.
	  /// </exception>
	  /// <exception cref="NotValidException">
	  ///           when the given decision definition key is null.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE_INSTANCE"/> permission
	  ///           on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  DmnDecisionTableResult evaluateDecisionTableByKeyAndVersion(string decisionDefinitionKey, int? version, IDictionary<string, object> variables);

	  /// <summary>
	  /// Returns a fluent builder to evaluate the decision table with the given key.
	  /// The builder can be used to set further properties and specify evaluation
	  /// instructions.
	  /// </summary>
	  /// <param name="decisionDefinitionKey">
	  ///          the key of the decision definition, cannot be <code>null</code>.
	  /// </param>
	  /// <returns> a builder to evaluate a decision table
	  /// </returns>
	  /// <seealso cref= #evaluateDecisionByKey(String) </seealso>
	  DecisionEvaluationBuilder evaluateDecisionTableByKey(string decisionDefinitionKey);

	  /// <summary>
	  /// Returns a fluent builder to evaluate the decision table with the given id.
	  /// The builder can be used to set further properties and specify evaluation
	  /// instructions.
	  /// </summary>
	  /// <param name="decisionDefinitionId">
	  ///          the id of the decision definition, cannot be <code>null<code>.
	  /// </param>
	  /// <returns> a builder to evaluate a decision table
	  /// </returns>
	  /// <seealso cref= #evaluateDecisionById(String) </seealso>
	  DecisionEvaluationBuilder evaluateDecisionTableById(string decisionDefinitionId);

	  /// <summary>
	  /// Returns a fluent builder to evaluate the decision with the given key.
	  /// The builder can be used to set further properties and specify evaluation
	  /// instructions.
	  /// </summary>
	  /// <param name="decisionDefinitionKey">
	  ///          the key of the decision definition, cannot be <code>null</code>.
	  /// </param>
	  /// <returns> a builder to evaluate a decision </returns>
	  DecisionsEvaluationBuilder evaluateDecisionByKey(string decisionDefinitionKey);

	  /// <summary>
	  /// Returns a fluent builder to evaluate the decision with the given id.
	  /// The builder can be used to set further properties and specify evaluation
	  /// instructions.
	  /// </summary>
	  /// <param name="decisionDefinitionId">
	  ///          the id of the decision definition, cannot be <code>null<code>.
	  /// </param>
	  /// <returns> a builder to evaluate a decision </returns>
	  DecisionsEvaluationBuilder evaluateDecisionById(string decisionDefinitionId);

	}

}