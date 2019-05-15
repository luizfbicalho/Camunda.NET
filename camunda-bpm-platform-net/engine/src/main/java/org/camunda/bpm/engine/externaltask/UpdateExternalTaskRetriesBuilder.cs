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
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;

	public interface UpdateExternalTaskRetriesBuilder : UpdateExternalTaskRetriesSelectBuilder
	{

	  /// <summary>
	  /// Sets the retries for external tasks.
	  /// 
	  /// If the new value is 0, a new incident with a <code>null</code> message is created.
	  /// If the old value is 0 and the new value is greater than 0, an existing incident
	  /// is resolved.
	  /// </summary>
	  /// <param name="retries">
	  /// </param>
	  /// <exception cref="org.camunda.bpm.engine.BadUserRequestException">
	  ///           If no external tasks are found
	  ///           If a external task id is set to null
	  /// </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void set(int retries);

	  /// <summary>
	  /// Sets the retries for external tasks asynchronously as batch. The returned batch
	  /// can be used to track the progress.
	  /// 
	  /// If the new value is 0, a new incident with a <code>null</code> message is created.
	  /// If the old value is 0 and the new value is greater than 0, an existing incident
	  /// is resolved.
	  /// </summary>
	  /// <param name="retries">
	  /// </param>
	  /// <exception cref="org.camunda.bpm.engine.BadUserRequestException">
	  ///           If no external tasks are found or if a external task id is set to null
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE"/> or
	  ///           <seealso cref="BatchPermissions#CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch setAsync(int retries);

	}

}