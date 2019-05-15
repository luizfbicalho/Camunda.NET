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

	public interface ModificationBuilder : InstantiationBuilder<ModificationBuilder>
	{

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Cancel all instances of the given activity in an arbitrary order, which are:
	  /// <ul>
	  ///   <li>activity instances of that activity
	  ///   <li>transition instances entering or leaving that activity
	  /// </ul></para>
	  /// 
	  /// <para>The cancellation order of the instances is arbitrary</para>
	  /// </summary>
	  /// <param name="activityId"> the activity for which all instances should be cancelled </param>
	  ModificationBuilder cancelAllForActivity(string activityId);

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Cancel all instances of the given activity in an arbitrary order, which are:
	  /// <ul>
	  ///   <li>activity instances of that activity
	  ///   <li>transition instances entering or leaving that activity
	  /// </ul></para>
	  /// 
	  /// <para>The cancellation order of the instances is arbitrary</para>
	  /// </summary>
	  /// <param name="activityId"> the activity for which all instances should be cancelled </param>
	  /// <param name="cancelCurrentActiveActivityInstances"> </param>
	  ModificationBuilder cancelAllForActivity(string activityId, bool cancelCurrentActiveActivityInstances);

	  /// <param name="processInstanceIds"> the process instance ids to modify. </param>
	  ModificationBuilder processInstanceIds(IList<string> processInstanceIds);

	  /// <param name="processInstanceIds"> the process instance ids to modify. </param>
	  ModificationBuilder processInstanceIds(params string[] processInstanceIds);

	  /// <param name="processInstanceQuery"> a query which selects the process instances to modify.
	  ///   Query results are restricted to process instances for which the user has <seealso cref="Permissions#READ"/> permission. </param>
	  ModificationBuilder processInstanceQuery(ProcessInstanceQuery processInstanceQuery);

	  /// <summary>
	  /// Skips custom execution listeners when creating/removing activity instances during modification
	  /// </summary>
	  ModificationBuilder skipCustomListeners();

	  /// <summary>
	  /// Skips io mappings when creating/removing activity instances during modification
	  /// </summary>
	  ModificationBuilder skipIoMappings();

	  /// <summary>
	  /// Execute the modification synchronously.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///   if the user has not all of the following permissions
	  ///   <ul>
	  ///      <li>if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void execute();

	  /// <summary>
	  /// Execute the modification asynchronously as batch. The returned batch
	  /// can be used to track the progress of the modification.
	  /// </summary>
	  /// <returns> the batch which executes the modification asynchronously.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///   if the user has not all of the following permissions
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#CREATE"/> or <seealso cref="BatchPermissions#CREATE_BATCH_MODIFY_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/></li>
	  ///   </ul> </exception>
	  Batch executeAsync();
	}


}