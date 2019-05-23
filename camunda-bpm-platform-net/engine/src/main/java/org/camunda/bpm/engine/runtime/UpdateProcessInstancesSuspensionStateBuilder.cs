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

	public interface UpdateProcessInstancesSuspensionStateBuilder : UpdateProcessInstanceSuspensionStateBuilder, UpdateProcessInstancesRequest
	{

	  /// <summary>
	  /// <para>
	  /// Activates the provided process instances asynchronously.
	  /// </para>
	  /// 
	  /// <para>
	  /// If you have a process instance hierarchy, activating one process instance
	  /// from the hierarchy will not activate other process instances from that
	  /// hierarchy.
	  /// </para>
	  /// </summary>
	  /// <exception cref="org.camunda.bpm.engine.BadUserRequestException">
	  ///           If no process Instances are found
	  ///           If a process Instance is set to null </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.CREATE"/> or
	  ///           <seealso cref="BatchPermissions.CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND"/> permission
	  ///           on <seealso cref="Resources.BATCH"/>. </exception>
	  Batch activateAsync();

	  /// <summary>
	  /// <para>
	  /// Suspends the provided process instances asynchronously. This means that the execution is
	  /// stopped, so the <i>token state</i> will not change. However, actions that
	  /// do not change token state, like setting/removing variables, etc. will
	  /// succeed.
	  /// </para>
	  /// 
	  /// <para>
	  /// Tasks belonging to the suspended process instance will also be suspended.
	  /// This means that any actions influencing the tasks' lifecycles will fail,
	  /// such as
	  /// <ul>
	  /// <li>claiming</li>
	  /// <li>completing</li>
	  /// <li>delegation</li>
	  /// <li>changes in task assignees, owners, etc.</li>
	  /// </ul>
	  /// Actions that only change task properties will succeed, such as changing
	  /// variables or adding comments.
	  /// </para>
	  /// 
	  /// <para>
	  /// If a process instance is in state suspended, the engine will also not
	  /// execute jobs (timers, messages) associated with this instance.
	  /// </para>
	  /// 
	  /// <para>
	  /// If you have a process instance hierarchy, suspending one process instance
	  /// from the hierarchy will not suspend other process instances from that
	  /// hierarchy.
	  /// </para>
	  /// </summary>
	  /// <exception cref="org.camunda.bpm.engine.BadUserRequestException">
	  ///           If no process Instances are found
	  ///           If a process Instance is set to null </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.CREATE"/> or
	  ///           <seealso cref="BatchPermissions.CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND"/> permission
	  ///           on <seealso cref="Resources.BATCH"/>. </exception>
	  Batch suspendAsync();
	}

}