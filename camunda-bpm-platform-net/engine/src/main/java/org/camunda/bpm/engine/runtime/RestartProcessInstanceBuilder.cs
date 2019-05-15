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

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>

	public interface RestartProcessInstanceBuilder : InstantiationBuilder<RestartProcessInstanceBuilder>
	{

	  /// <param name="query"> a query which selects the historic process instances to restart.
	  ///   Query results are restricted to process instances for which the user has <seealso cref="Permissions#READ_HISTORY"/> permission. </param>
	  RestartProcessInstanceBuilder historicProcessInstanceQuery(HistoricProcessInstanceQuery query);

	  /// <param name="processInstanceIds"> the process instance ids to restart. </param>
	  RestartProcessInstanceBuilder processInstanceIds(params string[] processInstanceIds);

	  /// <param name="processInstanceIds"> the process instance ids to restart. </param>
	  RestartProcessInstanceBuilder processInstanceIds(IList<string> processInstanceIds);

	  /// <summary>
	  /// Sets the initial set of variables during restart. By default, the last set of variables is used
	  /// </summary>
	  RestartProcessInstanceBuilder initialSetOfVariables();

	  /// <summary>
	  /// Does not take over the business key of the historic process instance
	  /// </summary>
	  RestartProcessInstanceBuilder withoutBusinessKey();

	  /// <summary>
	  /// Skips custom execution listeners when creating activity instances during restart
	  /// </summary>
	  RestartProcessInstanceBuilder skipCustomListeners();

	  /// <summary>
	  /// Skips io mappings when creating activity instances during restart
	  /// </summary>
	  RestartProcessInstanceBuilder skipIoMappings();

	  /// <summary>
	  /// Executes the restart synchronously.
	  /// </summary>
	  void execute();

	  /// <summary>
	  /// Executes the restart asynchronously as batch. The returned batch
	  /// can be used to track the progress of the restart.
	  /// </summary>
	  /// <returns> the batch which executes the restart asynchronously.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///   if the user has not all of the following permissions
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#CREATE"/> or <seealso cref="BatchPermissions#CREATE_BATCH_RESTART_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/></li>
	  ///   </ul> </exception>
	  Batch executeAsync();

	}

}