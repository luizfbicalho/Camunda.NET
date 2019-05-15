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
namespace org.camunda.bpm.engine.externaltask
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface ExternalTaskQueryTopicBuilder : ExternalTaskQueryBuilder
	{

	  /// <summary>
	  /// Define variables to fetch with all tasks for the current topic. Calling
	  /// this method multiple times overrides the previously specified variables.
	  /// </summary>
	  /// <param name="variables"> the variable names to fetch, if null all variables will be fetched </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder variables(params string[] variables);

	  /// <summary>
	  /// Define variables to fetch with all tasks for the current topic. Calling
	  /// this method multiple times overrides the previously specified variables.
	  /// </summary>
	  /// <param name="variables"> the variable names to fetch, if null all variables will be fetched </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder variables(IList<string> variables);

	  /// <summary>
	  /// Define a HashMap of variables and their values to filter correlated tasks.
	  /// Calling this method multiple times overrides the previously specified variables.
	  /// </summary>
	  /// <param name="variables"> a HashMap of the variable names (keys) and the values to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder processInstanceVariableEquals(IDictionary<string, object> variables);

	  /// <summary>
	  /// Define a single variable and its name to filter tasks in a topic. Multiple calls to
	  /// this method add to the existing "variable filters".
	  /// </summary>
	  /// <param name="name"> the name of the variable you want to fetch and query by </param>
	  /// <param name="value"> the value of the variable which you want to filter </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder processInstanceVariableEquals(string name, object value);

	  /// <summary>
	  /// Define business key value to filter external tasks by (Process Instance) Business Key.
	  /// </summary>
	  /// <param name="businessKey"> the value of the Business Key to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder businessKey(string businessKey);

	  /// <summary>
	  /// Define process definition id to filter external tasks by.
	  /// </summary>
	  /// <param name="processDefinitionId"> the definition id to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Define process definition ids to filter external tasksb by.
	  /// </summary>
	  /// <param name="processDefinitionIds"> the definition ids to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder processDefinitionIdIn(params string[] processDefinitionIds);

	  /// <summary>
	  /// Define process definition key to filter external tasks by.
	  /// </summary>
	  /// <param name="processDefinitionKey"> the definition key to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Define process definition keys to filter external tasks by.
	  /// </summary>
	  /// <param name="processDefinitionKey"> the definition keys to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder processDefinitionKeyIn(params string[] processDefinitionKeys);

	  /// <summary>
	  /// Filter external tasks only with null tenant id.
	  /// </summary>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder withoutTenantId();

	  /// <summary>
	  /// Define tenant ids to filter external tasks by.
	  /// </summary>
	  /// <param name="tenantIds"> the tenant ids to filter by </param>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Enable deserialization of variable values that are custom objects. By default, the query
	  /// will not attempt to deserialize the value of these variables.
	  /// </summary>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder enableCustomObjectDeserialization();

	  /// <summary>
	  /// Define whether only local variables will be fetched with all tasks for the current topic.
	  /// </summary>
	  /// <returns> this builder </returns>
	  ExternalTaskQueryTopicBuilder localVariables();

	}

}