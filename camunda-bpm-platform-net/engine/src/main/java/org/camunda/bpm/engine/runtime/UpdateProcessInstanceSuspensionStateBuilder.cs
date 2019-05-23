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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// Fluent builder to update the suspension state of process instances.
	/// </summary>
	public interface UpdateProcessInstanceSuspensionStateBuilder
	{

	  /// <summary>
	  /// <para>
	  /// Activates the provided process instances.
	  /// </para>
	  /// 
	  /// <para>
	  /// If you have a process instance hierarchy, activating one process instance
	  /// from the hierarchy will not activate other process instances from that
	  /// hierarchy.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///           If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has none of the following:
	  ///           <li><seealso cref="ProcessInstancePermissions.SUSPEND"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.SUSPEND_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li> </exception>
	  void activate();

	  /// <summary>
	  /// <para>
	  /// Suspends the provided process instances. This means that the execution is
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
	  /// <exception cref="ProcessEngineException">
	  ///           If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///            if the user has none of the following:
	  ///           <li><seealso cref="ProcessInstancePermissions.SUSPEND"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.SUSPEND_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///  </exception>
	  void suspend();

	}

}