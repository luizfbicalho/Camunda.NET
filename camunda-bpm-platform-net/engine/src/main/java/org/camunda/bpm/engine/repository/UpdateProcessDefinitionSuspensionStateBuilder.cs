using System;

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

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// Fluent builder to update the suspension state of process definitions.
	/// </summary>
	public interface UpdateProcessDefinitionSuspensionStateBuilder
	{

	  /// <summary>
	  /// Specify if the suspension states of the process instances of the provided
	  /// process definitions should also be updated. Default is <code>false</code>.
	  /// </summary>
	  /// <param name="includeProcessInstances">
	  ///          if <code>true</code>, all related process instances will be
	  ///          activated / suspended too. </param>
	  /// <returns> the builder </returns>
	  UpdateProcessDefinitionSuspensionStateBuilder includeProcessInstances(bool includeProcessInstances);

	  /// <summary>
	  /// Specify when the suspension state should be updated. Note that the <b>job
	  /// executor</b> needs to be active to use this.
	  /// </summary>
	  /// <param name="executionDate">
	  ///          the date on which the process definition will be activated /
	  ///          suspended. If <code>null</code>, the process definition is
	  ///          activated / suspended immediately.
	  /// </param>
	  /// <returns> the builder </returns>
	  UpdateProcessDefinitionSuspensionStateBuilder executionDate(DateTime executionDate);

	  /// <summary>
	  /// Activates the provided process definitions.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///           If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>if the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.SUSPEND"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul>
	  ///           <li>if <seealso cref="includeProcessInstances(bool)"/> is set to <code>true</code> and the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessInstancePermissions.SUSPEND"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.SUSPEND_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void activate();

	  /// <summary>
	  /// Suspends the provided process definitions. If a process definition is in
	  /// state suspended, it will not be possible to start new process instances
	  /// based on this process definition.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///           If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>if the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.SUSPEND"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul>
	  ///           <li>if <seealso cref="includeProcessInstances(bool)"/> is set to <code>true</code> and the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessInstancePermissions.SUSPEND"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.SUSPEND_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void suspend();

	}

}