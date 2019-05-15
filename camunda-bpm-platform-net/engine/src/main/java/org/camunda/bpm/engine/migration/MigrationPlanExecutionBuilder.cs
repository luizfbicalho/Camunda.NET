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
namespace org.camunda.bpm.engine.migration
{

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	/// <summary>
	/// Builder to execute a migration.
	/// </summary>
	public interface MigrationPlanExecutionBuilder
	{

	  /// <param name="processInstanceIds"> the process instance ids to migrate. </param>
	  MigrationPlanExecutionBuilder processInstanceIds(IList<string> processInstanceIds);

	  /// <param name="processInstanceIds"> the process instance ids to migrate. </param>
	  MigrationPlanExecutionBuilder processInstanceIds(params string[] processInstanceIds);

	  /// <param name="processInstanceQuery"> a query which selects the process instances to migrate.
	  ///   Query results are restricted to process instances for which the user has <seealso cref="Permissions#READ"/> permission. </param>
	  MigrationPlanExecutionBuilder processInstanceQuery(ProcessInstanceQuery processInstanceQuery);

	  /// <summary>
	  /// Skips custom execution listeners when creating/removing activity instances during migration
	  /// </summary>
	  MigrationPlanExecutionBuilder skipCustomListeners();

	  /// <summary>
	  /// Skips io mappings when creating/removing activity instances during migration
	  /// </summary>
	  MigrationPlanExecutionBuilder skipIoMappings();

	  /// <summary>
	  /// Execute the migration synchronously.
	  /// </summary>
	  /// <exception cref="MigratingProcessInstanceValidationException"> if the migration plan contains
	  ///  instructions that are not applicable to any of the process instances </exception>
	  /// <exception cref="AuthorizationException">
	  ///   if the user has not all of the following permissions
	  ///   <ul>
	  ///      <li>if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or</li>
	  ///      <li>no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void execute();

	  /// <summary>
	  /// Execute the migration asynchronously as batch. The returned batch
	  /// can be used to track the progress of the migration.
	  /// </summary>
	  /// <returns> the batch which executes the migration asynchronously.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///   if the user has not all of the following permissions
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#MIGRATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/> for source and target</li>
	  ///     <li><seealso cref="Permissions#CREATE"/> or <seealso cref="BatchPermissions#CREATE_BATCH_MIGRATE_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/></li>
	  ///   </ul> </exception>
	  Batch executeAsync();
	}

}