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
namespace org.camunda.bpm.engine.runtime
{

	/// <summary>
	/// <para>A fluent builder for defining conditional start event correlation</para>
	/// 
	/// @author Yana Vasileva
	/// </summary>
	public interface ConditionEvaluationBuilder
	{

	  /// <summary>
	  /// <para>
	  /// Correlate the condition such that the process instance has a business key with
	  /// the given name. If the condition is correlated to a conditional start
	  /// event then the given business key is set on the created process instance.
	  /// Is only supported for <seealso cref="#evaluateStartConditions()"/>.</para>
	  /// </summary>
	  /// <param name="businessKey">
	  ///          the businessKey to correlate on. </param>
	  /// <returns> the builder </returns>
	  ConditionEvaluationBuilder processInstanceBusinessKey(string businessKey);

	  /// <summary>
	  /// <para>Correlate the condition such that a process definition with the given id is selected.
	  /// Is only supported for <seealso cref="#evaluateStartConditions()"/>.</para>
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition to correlate on. </param>
	  /// <returns> the builder </returns>
	  ConditionEvaluationBuilder processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Pass a variable to the condition.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to set </param>
	  /// <param name="variableValue"> the value of the variable to set </param>
	  /// <returns> the builder </returns>
	  ConditionEvaluationBuilder setVariable(string variableName, object variableValue);

	  /// <summary>
	  /// <para>Pass a variables to the condition.</para>
	  /// </summary>
	  /// <param name="variables">
	  ///          the map of variables </param>
	  /// <returns> the builder </returns>
	  ConditionEvaluationBuilder setVariables(IDictionary<string, object> variables);

	  /// <summary>
	  /// Specify a tenant to correlate a condition to. The condition can only be
	  /// correlated on process definitions which belongs to the given tenant.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <returns> the builder </returns>
	  ConditionEvaluationBuilder tenantId(string tenantId);

	  /// <summary>
	  /// Specify that the condition can only be correlated on process
	  /// definitions which belongs to no tenant.
	  /// </summary>
	  /// <returns> the builder </returns>
	  ConditionEvaluationBuilder withoutTenantId();

	  /// 
	  /// <returns> the list of the newly created process instances </returns>
	  IList<ProcessInstance> evaluateStartConditions();

	}

}