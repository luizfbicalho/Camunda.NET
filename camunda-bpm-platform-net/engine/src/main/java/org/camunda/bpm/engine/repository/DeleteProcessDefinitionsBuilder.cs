﻿/*
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
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;

	/// <summary>
	/// Fluent builder to delete process definitions by a process definition key or process definition ids.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public interface DeleteProcessDefinitionsBuilder
	{

	  /// <summary>
	  /// All process instances of the process definition as well as history data is deleted.
	  /// </summary>
	  /// <returns> the builder </returns>
	  DeleteProcessDefinitionsBuilder cascade();

	  /// <summary>
	  /// Only the built-in <seealso cref="ExecutionListener"/>s are notified with the
	  /// <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
	  /// Is only applied in conjunction with the cascade method.
	  /// </summary>
	  /// <returns> the builder </returns>
	  DeleteProcessDefinitionsBuilder skipCustomListeners();

	  /// <summary>
	  /// Specifies whether input/output mappings for tasks should be invoked
	  /// </summary>
	  /// <returns> the builder </returns>
	  DeleteProcessDefinitionsBuilder skipIoMappings();

	  /// <summary>
	  /// Performs the deletion of process definitions.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///           If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <ul><li>if the user has no <seealso cref="Permissions#UPDATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           <li>if <seealso cref="#cascade()"/> is applied and the user has
	  ///           no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///           no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li></ul> </exception>
	  void delete();

	}

}