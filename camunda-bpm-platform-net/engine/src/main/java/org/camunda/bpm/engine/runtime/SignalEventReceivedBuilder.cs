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

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// Fluent builder to notify the process engine that a signal event has been
	/// received.
	/// </summary>
	public interface SignalEventReceivedBuilder
	{

	  /// <summary>
	  /// Add the given variables to the triggered executions.
	  /// </summary>
	  /// <param name="variables">
	  ///          a map of variables added to the executions </param>
	  /// <returns> the builder </returns>
	  SignalEventReceivedBuilder setVariables(IDictionary<string, object> variables);

	  /// <summary>
	  /// Specify a single execution to deliver the signal to.
	  /// </summary>
	  /// <param name="executionId">
	  ///          the id of the process instance or the execution to deliver the
	  ///          signal to </param>
	  /// <returns> the builder </returns>
	  SignalEventReceivedBuilder executionId(string executionId);

	  /// <summary>
	  /// Specify a tenant to deliver the signal to. The signal can only be received
	  /// on executions or process definitions which belongs to the given tenant.
	  /// Cannot be used in combination with <seealso cref="executionId(string)"/>.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <returns> the builder </returns>
	  SignalEventReceivedBuilder tenantId(string tenantId);

	  /// <summary>
	  /// Specify that the signal can only be received on executions or process
	  /// definitions which belongs to no tenant. Cannot be used in combination with
	  /// <seealso cref="executionId(string)"/>.
	  /// </summary>
	  /// <returns> the builder </returns>
	  SignalEventReceivedBuilder withoutTenantId();

	  /// <summary>
	  /// <para>
	  /// Delivers the signal to waiting executions and process definitions. The notification and instantiation happen
	  /// synchronously.
	  /// </para>
	  /// 
	  /// <para>
	  /// Note that the signal delivers to all tenants if no tenant is specified
	  /// using <seealso cref="tenantId(string)"/> or <seealso cref="withoutTenantId()"/>.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///           if a single execution is specified and no such execution exists
	  ///           or has not subscribed to the signal </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>if notify an execution and the user has no
	  ///           <seealso cref="Permissions.UPDATE"/> permission on
	  ///           <seealso cref="Resources.PROCESS_INSTANCE"/> or no
	  ///           <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources.PROCESS_DEFINITION"/>.</li>
	  ///           <li>if start a new process instance and the user has no
	  ///           <seealso cref="Permissions.CREATE"/> permission on
	  ///           <seealso cref="Resources.PROCESS_INSTANCE"/> and no
	  ///           <seealso cref="Permissions.CREATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources.PROCESS_DEFINITION"/>.</li> </exception>
	  void send();

	}

}