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
namespace org.camunda.bpm.engine.impl.identity
{
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using NativeUserQuery = org.camunda.bpm.engine.identity.NativeUserQuery;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;

	/// <summary>
	/// <para>SPI interface for read-only identity Service Providers.</para>
	/// 
	/// <para>This interface provides access to a read-only user / group
	/// repository</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ReadOnlyIdentityProvider : Session
	{

	  // users ////////////////////////////////////////

	  /// <returns> a <seealso cref="User"/> object for the given user id or null if no such user exists. </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  User findUserById(string userId);


	  /// <returns> a <seealso cref="UserQuery"/> object which can be used for querying for users. </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  UserQuery createUserQuery();

	  /// <returns> a <seealso cref="UserQuery"/> object which can be used in the current command context </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  UserQuery createUserQuery(CommandContext commandContext);

	  /// <summary>
	  /// Creates a <seealso cref="NativeUserQuery"/> that allows to select users with native queries. </summary>
	  /// <returns> NativeUserQuery </returns>
	  NativeUserQuery createNativeUserQuery();

	  /// <returns> 'true' if the password matches the </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  bool checkPassword(string userId, string password);

	  // groups //////////////////////////////////////

	  /// <returns> a <seealso cref="Group"/> object for the given group id or null if no such group exists. </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  Group findGroupById(string groupId);

	  /// <returns> a <seealso cref="GroupQuery"/> object which can be used for querying for groups. </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  GroupQuery createGroupQuery();

	  /// <returns> a <seealso cref="GroupQuery"/> object which can be used for querying for groups and can be reused in the current command context. </returns>
	  /// <exception cref="IdentityProviderException"> in case an error occurs </exception>
	  GroupQuery createGroupQuery(CommandContext commandContext);

	  // tenants //////////////////////////////////////

	  /// <returns> a <seealso cref="Tenant"/> object for the given id or null if no such tenant
	  ///         exists. </returns>
	  /// <exception cref="IdentityProviderException">
	  ///           in case an error occurs </exception>
	  Tenant findTenantById(string tenantId);

	  /// <returns> a <seealso cref="TenantQuery"/> object which can be used for querying for
	  ///         tenants. </returns>
	  /// <exception cref="IdentityProviderException">
	  ///           in case an error occurs </exception>
	  TenantQuery createTenantQuery();

	  /// <returns> a <seealso cref="TenantQuery"/> object which can be used for querying for
	  ///         tenants and can be reused in the current command context. </returns>
	  /// <exception cref="IdentityProviderException">
	  ///           in case an error occurs </exception>
	  TenantQuery createTenantQuery(CommandContext commandContext);

	}

}