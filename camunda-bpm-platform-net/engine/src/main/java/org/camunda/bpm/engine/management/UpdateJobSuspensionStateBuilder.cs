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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// Fluent builder to update the suspension state of jobs.
	/// </summary>
	public interface UpdateJobSuspensionStateBuilder
	{

	  /// <summary>
	  /// Activates the provided jobs.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#UPDATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_INSTANCE"/> or no
	  ///           <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void activate();

	  /// <summary>
	  /// Suspends the provided jobs. If a job is in state suspended, it will not be
	  /// executed by the job executor.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#UPDATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_INSTANCE"/> or no
	  ///           <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void suspend();

	}

}