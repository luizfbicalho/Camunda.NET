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
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;

	/// <summary>
	/// <para>SPI Interface for identity service implementations which offer
	/// read / write access to the user database.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface WritableIdentityProvider : Session
	{

	  // users /////////////////////////////////////////////////

	  /// <summary>
	  /// <para>Returns a new (transient) <seealso cref="User"/> object. The Object is not
	  /// yet persistent and must be saved using the <seealso cref="#saveUser(User)"/>
	  /// method.</para>
	  /// 
	  /// <para>NOTE: the implementation does not validate the uniqueness of the userId
	  /// parameter at this time.</para>
	  /// </summary>
	  /// <param name="userId"> </param>
	  /// <returns> an non-persistent user object. </returns>
	  User createNewUser(string userId);

	  /// <summary>
	  /// Allows saving or updates a <seealso cref="User"/> object
	  /// </summary>
	  /// <param name="user"> a User object. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
	  IdentityOperationResult saveUser(User user);

	  /// <summary>
	  /// Allows deleting a persistent <seealso cref="User"/> object.
	  /// </summary>
	  /// <param name="UserId"> the id of the User object to delete. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
	  IdentityOperationResult deleteUser(string userId);

	  /// <summary>
	  /// Allows unlocking a <seealso cref="User"/> object. </summary>
	  /// <param name="userId"> the id of the User object to delete. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="AuthorizationException"> if the user is not CAMUNDA_ADMIN </exception>
	  IdentityOperationResult unlockUser(string userId);

	  // groups /////////////////////////////////////////////////

	  /// <summary>
	  /// <para>Returns a new (transient) <seealso cref="Group"/> object. The Object is not
	  /// yet persistent and must be saved using the <seealso cref="#saveGroup(Group)"/>
	  /// method.</para>
	  /// 
	  /// <para>NOTE: the implementation does not validate the uniqueness of the groupId
	  /// parameter at this time.</para>
	  /// </summary>
	  /// <param name="groupId"> </param>
	  /// <returns> an non-persistent group object. </returns>
	  Group createNewGroup(string groupId);

	  /// <summary>
	  /// Allows saving a <seealso cref="Group"/> object which is not yet persistent.
	  /// </summary>
	  /// <param name="group"> a group object. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
	  IdentityOperationResult saveGroup(Group group);

	  /// <summary>
	  /// Allows deleting a persistent <seealso cref="Group"/> object.
	  /// </summary>
	  /// <param name="groupId"> the id of the group object to delete. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException"> in case an internal error occurs </exception>
	  IdentityOperationResult deleteGroup(string groupId);

	  /// <summary>
	  /// <para>
	  /// Returns a new (transient) <seealso cref="Tenant"/> object. The Object is not yet
	  /// persistent and must be saved using the <seealso cref="#saveTenant(Tenant)"/> method.
	  /// </para>
	  /// 
	  /// <para>
	  /// NOTE: the implementation does not validate the uniqueness of the tenantId
	  /// parameter at this time.
	  /// </para>
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the new tenant </param>
	  /// <returns> an non-persistent tenant object. </returns>
	  Tenant createNewTenant(string tenantId);

	  /// <summary>
	  /// Allows saving a <seealso cref="Tenant"/> object which is not yet persistent.
	  /// </summary>
	  /// <param name="tenant">
	  ///          the tenant object to save. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException">
	  ///           in case an internal error occurs </exception>
	  IdentityOperationResult saveTenant(Tenant tenant);

	  /// <summary>
	  /// Allows deleting a persistent <seealso cref="Tenant"/> object.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant object to delete. </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException">
	  ///           in case an internal error occurs </exception>
	  IdentityOperationResult deleteTenant(string tenantId);

	  // Membership ///////////////////////////////////////////////

	  /// <summary>
	  /// Creates a membership relation between a user and a group. If the user is already part of that group,
	  /// IdentityProviderException is thrown.
	  /// </summary>
	  /// <param name="userId"> the id of the user </param>
	  /// <param name="groupId"> id of the group </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException"> </exception>
	  IdentityOperationResult createMembership(string userId, string groupId);

	  /// <summary>
	  /// Deletes a membership relation between a user and a group.
	  /// </summary>
	  /// <param name="userId"> the id of the user </param>
	  /// <param name="groupId"> id of the group </param>
	  /// <returns> the operation result object. </returns>
	  /// <exception cref="IdentityProviderException"> </exception>
	  IdentityOperationResult deleteMembership(string userId, string groupId);

	  /// <summary>
	  /// Creates a membership relation between a tenant and a user.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <param name="userId">
	  ///          the id of the user </param>
	  /// <returns> the operation result object. </returns>
	  IdentityOperationResult createTenantUserMembership(string tenantId, string userId);

	  /// <summary>
	  /// Creates a membership relation between a tenant and a group.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <param name="groupId">
	  ///          the id of the group </param>
	  /// <returns> the operation result object. </returns>
	  IdentityOperationResult createTenantGroupMembership(string tenantId, string groupId);

	  /// <summary>
	  /// Deletes a membership relation between a tenant and a user.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <param name="userId">
	  ///          the id of the user </param>
	  /// <returns> the operation result object </returns>
	  IdentityOperationResult deleteTenantUserMembership(string tenantId, string userId);

	  /// <summary>
	  /// Deletes a membership relation between a tenant and a group.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <param name="groupId">
	  ///          the id of the group </param>
	  /// <returns> the operation result object. </returns>
	  IdentityOperationResult deleteTenantGroupMembership(string tenantId, string groupId);

	}

}