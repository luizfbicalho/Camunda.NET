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

	/// <summary>
	/// <para>Base interface providing access to the process engine's
	/// public API services.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessEngineServices
	{

	  /// <summary>
	  /// Returns the process engine's <seealso cref="RuntimeService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="RuntimeService"/> object. </returns>
	  RuntimeService RuntimeService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="RepositoryService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="RepositoryService"/> object. </returns>
	  RepositoryService RepositoryService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="FormService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="FormService"/> object. </returns>
	  FormService FormService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="TaskService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="TaskService"/> object. </returns>
	  TaskService TaskService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="HistoryService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="HistoryService"/> object. </returns>
	  HistoryService HistoryService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="IdentityService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="IdentityService"/> object. </returns>
	  IdentityService IdentityService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="ManagementService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="ManagementService"/> object. </returns>
	  ManagementService ManagementService {get;}

	  /// <summary>
	  /// Returns the process engine's <seealso cref="AuthorizationService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="AuthorizationService"/> object. </returns>
	  AuthorizationService AuthorizationService {get;}

	  /// <summary>
	  /// Returns the engine's <seealso cref="CaseService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="CaseService"/> object.
	  ///  </returns>
	  CaseService CaseService {get;}

	  /// <summary>
	  /// Returns the engine's <seealso cref="FilterService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="FilterService"/> object.
	  ///  </returns>
	  FilterService FilterService {get;}

	  /// <summary>
	  /// Returns the engine's <seealso cref="ExternalTaskService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="ExternalTaskService"/> object. </returns>
	  ExternalTaskService ExternalTaskService {get;}

	  /// <summary>
	  /// Returns the engine's <seealso cref="DecisionService"/>.
	  /// </summary>
	  /// <returns> the <seealso cref="DecisionService"/> object. </returns>
	  DecisionService DecisionService {get;}

	}

}