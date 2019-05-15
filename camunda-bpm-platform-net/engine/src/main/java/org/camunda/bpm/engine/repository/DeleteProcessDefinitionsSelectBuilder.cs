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
namespace org.camunda.bpm.engine.repository
{
	/// <summary>
	/// Fluent builder to delete process definitions by a process definition key or process definition ids.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public interface DeleteProcessDefinitionsSelectBuilder
	{

	  /// <summary>
	  /// Selects process definitions with given process definition ids.
	  /// </summary>
	  /// <param name="processDefinitionId"> at least one process definition id </param>
	  /// <returns> the builder </returns>
	  DeleteProcessDefinitionsBuilder byIds(params string[] processDefinitionId);

	  /// <summary>
	  /// Selects process definitions with a given key.
	  /// </summary>
	  /// <param name="processDefinitionKey"> process definition key </param>
	  /// <returns> the builder </returns>
	  DeleteProcessDefinitionsTenantBuilder byKey(string processDefinitionKey);

	}

}