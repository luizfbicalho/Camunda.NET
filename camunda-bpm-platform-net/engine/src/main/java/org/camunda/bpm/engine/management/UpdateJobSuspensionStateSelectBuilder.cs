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
namespace org.camunda.bpm.engine.management
{
	/// <summary>
	/// Fluent builder to update the suspension state of jobs.
	/// </summary>
	public interface UpdateJobSuspensionStateSelectBuilder
	{

	  /// <summary>
	  /// Selects the job with the given id.
	  /// </summary>
	  /// <param name="jobId">
	  ///          id of the job </param>
	  /// <returns> the builder </returns>
	  UpdateJobSuspensionStateBuilder byJobId(string jobId);

	  /// <summary>
	  /// Selects the jobs of the job definition with the given id.
	  /// </summary>
	  /// <param name="jobDefinitionId">
	  ///          id of the job definition </param>
	  /// <returns> the builder </returns>
	  UpdateJobSuspensionStateBuilder byJobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// Selects the jobs of the process instance with the given id.
	  /// </summary>
	  /// <param name="processInstanceId">
	  ///          id of the process instance </param>
	  /// <returns> the builder </returns>
	  UpdateJobSuspensionStateBuilder byProcessInstanceId(string processInstanceId);

	  /// <summary>
	  /// Selects the jobs of the process definition with the given id.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          id of the process definition </param>
	  /// <returns> the builder </returns>
	  UpdateJobSuspensionStateBuilder byProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Selects the jobs of the process definitions with the given key.
	  /// </summary>
	  /// <param name="processDefinitionKey">
	  ///          key of the process definition </param>
	  /// <returns> the builder </returns>
	  UpdateJobSuspensionStateTenantBuilder byProcessDefinitionKey(string processDefinitionKey);

	}

}