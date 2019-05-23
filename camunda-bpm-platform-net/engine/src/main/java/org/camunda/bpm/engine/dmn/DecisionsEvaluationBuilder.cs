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
namespace org.camunda.bpm.engine.dmn
{

	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;

	/// <summary>
	/// Fluent builder to evaluate a decision.
	/// </summary>
	public interface DecisionsEvaluationBuilder
	{

	  /// <summary>
	  /// Specify the id of the tenant the decision definition belongs to. Can only be
	  /// used when the definition is referenced by <code>key</code> and not by <code>id</code>.
	  /// </summary>
	  DecisionsEvaluationBuilder decisionDefinitionTenantId(string tenantId);

	  /// <summary>
	  /// Specify that the decision definition belongs to no tenant. Can only be
	  /// used when the definition is referenced by <code>key</code> and not by <code>id</code>.
	  /// </summary>
	  DecisionsEvaluationBuilder decisionDefinitionWithoutTenantId();

	  /// <summary>
	  /// Set the version of the decision definition. If <code>null</code> then
	  /// the latest version is taken.
	  /// </summary>
	  DecisionsEvaluationBuilder version(int? version);

	  /// <summary>
	  /// Set the input values of the decision.
	  /// </summary>
	  DecisionsEvaluationBuilder variables(IDictionary<string, object> variables);

	  /// <summary>
	  /// Evaluates the decision.
	  /// </summary>
	  /// <returns> the result of the evaluation.
	  /// </returns>
	  /// <exception cref="NotFoundException">
	  ///           when no decision definition is deployed with the given id / key.
	  /// </exception>
	  /// <exception cref="NotValidException">
	  ///           when the given decision definition id / key is null.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.CREATE_INSTANCE"/> permission
	  ///           on <seealso cref="Resources.DECISION_DEFINITION"/>. </exception>
	  DmnDecisionResult evaluate();

	}

}