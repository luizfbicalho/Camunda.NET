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
namespace org.camunda.bpm.engine.management
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// Fluent builder to update the suspension state of job definitions.
	/// </summary>
	public interface UpdateJobDefinitionSuspensionStateBuilder
	{

	  /// <summary>
	  /// Specify if the suspension states of the jobs of the provided job
	  /// definitions should also be updated. Default is <code>false</code>.
	  /// </summary>
	  /// <param name="includeJobs">
	  ///          if <code>true</code>, all related jobs will be activated /
	  ///          suspended too. </param>
	  /// <returns> the builder </returns>
	  UpdateJobDefinitionSuspensionStateBuilder includeJobs(bool includeJobs);

	  /// <summary>
	  /// Specify when the suspension state should be updated. Note that the <b>job
	  /// executor</b> needs to be active to use this.
	  /// </summary>
	  /// <param name="executionDate">
	  ///          the date on which the job definition will be activated /
	  ///          suspended. If <code>null</code>, the job definition is activated /
	  ///          suspended immediately.
	  /// </param>
	  /// <returns> the builder </returns>
	  UpdateJobDefinitionSuspensionStateBuilder executionDate(DateTime executionDate);

	  /// <summary>
	  /// Activates the provided job definitions.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///           <li>if the current user has no <seealso cref="Permissions.UPDATE"/>
	  ///           permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li>If <seealso cref="includeJobs(bool)"/> is set to <code>true</code>
	  ///           and the user have no <seealso cref="Permissions.UPDATE_INSTANCE"/>
	  ///           permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///           <seealso cref="Permissions.UPDATE"/> permission on any
	  ///           <seealso cref="Resources.PROCESS_INSTANCE"/></li> </exception>
	  void activate();

	  /// <summary>
	  /// Suspends the provided job definitions. If a job definition is in state
	  /// suspended, it will be ignored by the job executor.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///           <li>if the current user has no <seealso cref="Permissions.UPDATE"/>
	  ///           permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li>If <seealso cref="includeJobs(bool)"/> is set to <code>true</code>
	  ///           and the user have no <seealso cref="Permissions.UPDATE_INSTANCE"/>
	  ///           permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///           <seealso cref="Permissions.UPDATE"/> permission on any
	  ///           <seealso cref="Resources.PROCESS_INSTANCE"/></li> </exception>
	  void suspend();

	}

}