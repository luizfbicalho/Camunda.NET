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
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;

	/// <summary>
	/// Fluent builder to update the suspension state of process instances.
	/// </summary>
	public interface UpdateProcessInstanceSuspensionStateSelectBuilder : UpdateProcessInstancesRequest
	{

	  /// <summary>
	  /// Selects the process instance with the given id.
	  /// </summary>
	  /// <param name="processInstanceId">
	  ///          id of the process instance </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstanceSuspensionStateBuilder byProcessInstanceId(string processInstanceId);

	  /// <summary>
	  /// Selects the instances of the process definition with the given id.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          id of the process definition </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstanceSuspensionStateBuilder byProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Selects the instances of the process definitions with the given key.
	  /// </summary>
	  /// <param name="processDefinitionKey">
	  ///          key of the process definition </param>
	  /// <returns> the builder </returns>
	  UpdateProcessInstanceSuspensionStateTenantBuilder byProcessDefinitionKey(string processDefinitionKey);

	}

}