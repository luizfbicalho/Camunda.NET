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
namespace org.camunda.bpm.engine.impl
{

	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using DecisionEvaluationBuilder = org.camunda.bpm.engine.dmn.DecisionEvaluationBuilder;
	using DecisionsEvaluationBuilder = org.camunda.bpm.engine.dmn.DecisionsEvaluationBuilder;
	using DecisionEvaluationBuilderImpl = org.camunda.bpm.engine.impl.dmn.DecisionEvaluationBuilderImpl;
	using DecisionTableEvaluationBuilderImpl = org.camunda.bpm.engine.impl.dmn.DecisionTableEvaluationBuilderImpl;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class DecisionServiceImpl : ServiceImpl, DecisionService
	{

	  public virtual DmnDecisionTableResult evaluateDecisionTableById(string decisionDefinitionId, IDictionary<string, object> variables)
	  {
		return evaluateDecisionTableById(decisionDefinitionId).variables(variables).evaluate();
	  }

	  public virtual DmnDecisionTableResult evaluateDecisionTableByKey(string decisionDefinitionKey, IDictionary<string, object> variables)
	  {
		return evaluateDecisionTableByKey(decisionDefinitionKey).variables(variables).evaluate();
	  }

	  public virtual DmnDecisionTableResult evaluateDecisionTableByKeyAndVersion(string decisionDefinitionKey, int? version, IDictionary<string, object> variables)
	  {
		return evaluateDecisionTableByKey(decisionDefinitionKey).version(version).variables(variables).evaluate();
	  }

	  public virtual DecisionEvaluationBuilder evaluateDecisionTableByKey(string decisionDefinitionKey)
	  {
		return DecisionTableEvaluationBuilderImpl.evaluateDecisionTableByKey(commandExecutor, decisionDefinitionKey);
	  }

	  public virtual DecisionEvaluationBuilder evaluateDecisionTableById(string decisionDefinitionId)
	  {
		return DecisionTableEvaluationBuilderImpl.evaluateDecisionTableById(commandExecutor, decisionDefinitionId);
	  }

	  public virtual DecisionsEvaluationBuilder evaluateDecisionByKey(string decisionDefinitionKey)
	  {
		return DecisionEvaluationBuilderImpl.evaluateDecisionByKey(commandExecutor, decisionDefinitionKey);
	  }

	  public virtual DecisionsEvaluationBuilder evaluateDecisionById(string decisionDefinitionId)
	  {
		return DecisionEvaluationBuilderImpl.evaluateDecisionById(commandExecutor, decisionDefinitionId);
	  }

	}

}