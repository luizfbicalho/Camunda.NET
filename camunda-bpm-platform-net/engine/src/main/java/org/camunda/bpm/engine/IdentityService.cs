using System;
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
namespace org.camunda.bpm.engine
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using PasswordPolicyResult = org.camunda.bpm.engine.identity.PasswordPolicyResult;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using NativeUserQuery = org.camunda.bpm.engine.identity.NativeUserQuery;
	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using Picture = org.camunda.bpm.engine.identity.Picture;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using Account = org.camunda.bpm.engine.impl.identity.Account;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;

	/// <summary>
	/// Service to manage <seealso cref="User"/>s and <seealso cref="Group"/>s.
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface IdentityService
	{

	  /// <summary>
	  /// <para>Allows to inquire whether this identity service implementation provides
	  /// read-only access to the user repository, false otherwise.</para>
	  /// 
	  /// Read only identity service implementations do not support the following methods:
	  /// <ul>
	  /// <li> <seealso cref="newUser(string)"/> </li>
	  /// <li> <seealso cref="saveUser(User)"/> </li>
	  /// <li> <seealso cref="deleteUser(string)"/> </li>
	  /// 
	  /// <li> <seealso cref="newGroup(string)"/> </li>
	  /// <li> <seealso cref="saveGroup(Group)"/> </li>
	  /// <li> <seealso cref="deleteGroup(string)"/> </li>
	  /// 
	  /// <li> <seealso cref="newTenant(string)"/> </li>
	  /// <li> <seealso cref="saveTenant(Tenant)"/> </li>
	  /// <li> <seealso cref="deleteTenant(string)"/> </li>
	  /// 
	  /// <li> <seealso cref="createMembership(string, string)"/> </li>
	  /// <li> <seealso cref="deleteMembership(string, string)"/> </li>
	  /// 
	  /// <li> <seealso cref="createTenantUserMembership(string, string)"/> </li>
	  /// <li> <seealso cref="createTenantGroupMembership(string, string)"/> </li>
	  /// <li> <seealso cref="deleteTenantUserMembership(string, string)"/> </li>
	  /// <li> <seealso cref="deleteTenantGroupMembership(string, string)"/> </li>
	  /// </ul>
	  /// 
	  /// <para>If these methods are invoked on a read-only identity service implementation,
	  /// the invocation will throw an <seealso cref="System.NotSupportedException"/>.</para>
	  /// </summary>
	  /// <returns> true if this identity service implementation provides read-only
	  ///         access to the user repository, false otherwise. </returns>
	  bool ReadOnly {get;}

	  /// <summary>
	  /// Creates a new user. The user is transient and must be saved using
	  /// <seealso cref="saveUser(User)"/>. </summary>
	  /// <param name="userId"> id for the new user, cannot be null. </param>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.CREATE"/> permissions on <seealso cref="Resources.USER"/>. </exception>
	  User newUser(string userId);

	  /// <summary>
	  /// Saves the user. If the user already existed, the user is updated. </summary>
	  /// <param name="user"> user to save, cannot be null. </param>
	  /// <exception cref="RuntimeException"> when a user with the same name already exists. </exception>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.UPDATE"/> permissions on <seealso cref="Resources.USER"/> (update existing user)
	  /// or if user has no <seealso cref="Permissions.CREATE"/> permissions on <seealso cref="Resources.USER"/> (save new user). </exception>
	  void saveUser(User user);

	  /// <summary>
	  /// Creates a <seealso cref="UserQuery"/> that allows to programmatically query the users.
	  /// </summary>
	  UserQuery createUserQuery();

	  /// <param name="userId"> id of user to delete, cannot be null. When an id is passed
	  /// for an unexisting user, this operation is ignored. </param>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.DELETE"/> permissions on <seealso cref="Resources.USER"/>. </exception>
	  void deleteUser(string userId);

	  void unlockUser(string userId);

	  /// <summary>
	  /// Creates a new group. The group is transient and must be saved using
	  /// <seealso cref="saveGroup(Group)"/>. </summary>
	  /// <param name="groupId"> id for the new group, cannot be null. </param>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.CREATE"/> permissions on <seealso cref="Resources.GROUP"/>. </exception>
	  Group newGroup(string groupId);

	  /// <summary>
	  /// Creates a <seealso cref="NativeUserQuery"/> that allows to select users with native queries. </summary>
	  /// <returns> NativeUserQuery </returns>
	  NativeUserQuery createNativeUserQuery();

	  /// <summary>
	  /// Creates a <seealso cref="GroupQuery"/> thats allows to programmatically query the groups.
	  /// </summary>
	  GroupQuery createGroupQuery();

	  /// <summary>
	  /// Saves the group. If the group already existed, the group is updated. </summary>
	  /// <param name="group"> group to save. Cannot be null. </param>
	  /// <exception cref="RuntimeException"> when a group with the same name already exists. </exception>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.UPDATE"/> permissions on <seealso cref="Resources.GROUP"/> (update existing group)
	  /// or if user has no <seealso cref="Permissions.CREATE"/> permissions on <seealso cref="Resources.GROUP"/> (save new group). </exception>
	  void saveGroup(Group group);

	  /// <summary>
	  /// Deletes the group. When no group exists with the given id, this operation
	  /// is ignored. </summary>
	  /// <param name="groupId"> id of the group that should be deleted, cannot be null. </param>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.DELETE"/> permissions on <seealso cref="Resources.GROUP"/>. </exception>
	  void deleteGroup(string groupId);

	  /// <param name="userId"> the userId, cannot be null. </param>
	  /// <param name="groupId"> the groupId, cannot be null. </param>
	  /// <exception cref="RuntimeException"> when the given user or group doesn't exist or when the user
	  /// is already member of the group. </exception>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.CREATE"/> permissions on <seealso cref="Resources.GROUP_MEMBERSHIP"/>. </exception>
	  void createMembership(string userId, string groupId);

	  /// <summary>
	  /// Delete the membership of the user in the group. When the group or user don't exist
	  /// or when the user is not a member of the group, this operation is ignored. </summary>
	  /// <param name="userId"> the user's id, cannot be null. </param>
	  /// <param name="groupId"> the group's id, cannot be null. </param>
	  /// <exception cref="UnsupportedOperationException"> if identity service implementation is read only. See <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions.DELETE"/> permissions on <seealso cref="Resources.GROUP_MEMBERSHIP"/>. </exception>
	  void deleteMembership(string userId, string groupId);

	  /// <summary>
	  /// Creates a new tenant. The tenant is transient and must be saved using
	  /// <seealso cref="saveTenant(Tenant)"/>.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          id for the new tenant, cannot be <code>null</code>. </param>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.CREATE"/> permissions on
	  ///           <seealso cref="Resources.TENANT"/>. </exception>
	  Tenant newTenant(string tenantId);

	  /// <summary>
	  /// Creates a <seealso cref="TenantQuery"/> thats allows to programmatically query the
	  /// tenants.
	  /// </summary>
	  TenantQuery createTenantQuery();

	  /// <summary>
	  /// Saves the tenant. If the tenant already existed, it is updated.
	  /// </summary>
	  /// <param name="tenant">
	  ///          the tenant to save. Cannot be <code>null</code>. </param>
	  /// <exception cref="RuntimeException">
	  ///           when a tenant with the same name already exists. </exception>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.UPDATE"/> permissions on
	  ///           <seealso cref="Resources.TENANT"/> (update existing tenant) or if user has
	  ///           no <seealso cref="Permissions.CREATE"/> permissions on
	  ///           <seealso cref="Resources.TENANT"/> (save new tenant). </exception>
	  void saveTenant(Tenant tenant);

	  /// <summary>
	  /// Deletes the tenant. When no tenant exists with the given id, this operation
	  /// is ignored.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          id of the tenant that should be deleted, cannot be
	  ///          <code>null</code>. </param>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.DELETE"/> permissions on
	  ///           <seealso cref="Resources.TENANT"/>. </exception>
	  void deleteTenant(string tenantId);

	  /// <summary>
	  /// Creates a new membership between the given user and tenant.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant, cannot be null. </param>
	  /// <param name="userId">
	  ///          the id of the user, cannot be null. </param>
	  /// <exception cref="RuntimeException">
	  ///           when the given tenant or user doesn't exist or the user is
	  ///           already a member of this tenant. </exception>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.CREATE"/> permissions on
	  ///           <seealso cref="Resources.TENANT_MEMBERSHIP"/>. </exception>
	  void createTenantUserMembership(string tenantId, string userId);

	  /// <summary>
	  /// Creates a new membership between the given group and tenant.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant, cannot be null. </param>
	  /// <param name="groupId">
	  ///          the id of the group, cannot be null. </param>
	  /// <exception cref="RuntimeException">
	  ///           when the given tenant or group doesn't exist or when the group
	  ///           is already a member of this tenant. </exception>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.CREATE"/> permissions on
	  ///           <seealso cref="Resources.TENANT_MEMBERSHIP"/>. </exception>
	  void createTenantGroupMembership(string tenantId, string groupId);

	  /// <summary>
	  /// Deletes the membership between the given user and tenant. The operation is
	  /// ignored when the given user, tenant or membership don't exist.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant, cannot be null. </param>
	  /// <param name="userId">
	  ///          the id of the user, cannot be null. </param>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.DELETE"/> permissions on
	  ///           <seealso cref="Resources.TENANT_MEMBERSHIP"/>. </exception>
	  void deleteTenantUserMembership(string tenantId, string userId);

	  /// <summary>
	  /// Deletes the membership between the given group and tenant. The operation is
	  /// ignored when the given group, tenant or membership don't exist.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant, cannot be null. </param>
	  /// <param name="groupId">
	  ///          the id of the group, cannot be null. </param>
	  /// <exception cref="UnsupportedOperationException">
	  ///           if identity service implementation is read only. See
	  ///           <seealso cref="isReadOnly()"/> </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions.DELETE"/> permissions on
	  ///           <seealso cref="Resources.TENANT_MEMBERSHIP"/>. </exception>
	  void deleteTenantGroupMembership(string tenantId, string groupId);

	  /// <summary>
	  /// Checks if the password is valid for the given user. Arguments userId
	  /// and password are nullsafe.
	  /// </summary>
	  bool checkPassword(string userId, string password);

	  /// <summary>
	  /// Check a given password against the configured <seealso cref="PasswordPolicy"/>. The result
	  /// is returned as <seealso cref="PasswordPolicyResult"/> which contains all
	  /// passed and violated rules as well as a flag indicating if the password is
	  /// valid.
	  /// </summary>
	  /// <param name="password">
	  ///          the password that should be tested </param>
	  /// <returns> a <seealso cref="PasswordPolicyResult"/> containing passed and
	  ///         failed rules </returns>
	  PasswordPolicyResult checkPasswordAgainstPolicy(string password);

	  /// <summary>
	  /// Check a given password against a given <seealso cref="PasswordPolicy"/>. The result
	  /// is returned as <seealso cref="PasswordPolicyResult"/> which contains all
	  /// passed and violated rules as well as a flag indicating if the password is
	  /// valid.
	  /// </summary>
	  /// <param name="policy">
	  ///          the <seealso cref="PasswordPolicy"/> against which the password is tested </param>
	  /// <param name="password">
	  ///          the password that should be tested </param>
	  /// <returns> a <seealso cref="PasswordPolicyResult"/> containing passed and
	  ///         failed rules </returns>
	  PasswordPolicyResult checkPasswordAgainstPolicy(PasswordPolicy policy, string password);

	  /// <summary>
	  /// Returns the <seealso cref="PasswordPolicy"/> that is currently configured in the
	  /// engine.
	  /// </summary>
	  /// <returns> the current <seealso cref="PasswordPolicy"/> or <code>null</code> if no
	  ///         policy is set or the configured policy is disabled. </returns>
	  PasswordPolicy PasswordPolicy {get;}

	  /// <summary>
	  /// Passes the authenticated user id for this thread.
	  /// All service method (from any service) invocations done by the same
	  /// thread will have access to this authenticatedUserId. Should be followed by
	  /// a call to <seealso cref="clearAuthentication()"/> once the interaction is terminated.
	  /// </summary>
	  /// <param name="authenticatedUserId"> the id of the current user. </param>
	  string AuthenticatedUserId {set;}

	  /// <summary>
	  /// Passes the authenticated user id and groupIds for this thread.
	  /// All service method (from any service) invocations done by the same
	  /// thread will have access to this authentication. Should be followed by
	  /// a call to <seealso cref="clearAuthentication()"/> once the interaction is terminated.
	  /// </summary>
	  ///  <param name="authenticatedUserId"> the id of the current user. </param>
	  ///  <param name="groups"> the groups of the current user. </param>
	  void setAuthentication(string userId, IList<string> groups);

	  /// <summary>
	  /// Passes the authenticated user id, group ids and tenant ids for this thread.
	  /// All service method (from any service) invocations done by the same
	  /// thread will have access to this authentication. Should be followed by
	  /// a call to <seealso cref="clearAuthentication()"/> once the interaction is terminated.
	  /// </summary>
	  ///  <param name="userId"> the id of the current user. </param>
	  ///  <param name="groups"> the groups of the current user. </param>
	  ///  <param name="tenantIds"> the tenants of the current user. </param>
	  void setAuthentication(string userId, IList<string> groups, IList<string> tenantIds);

	  /// <param name="currentAuthentication"> </param>
	  Authentication Authentication {set;}

	  /// <returns> the current authentication for this process engine. </returns>
	  Authentication CurrentAuthentication {get;}

	  /// <summary>
	  /// Allows clearing the current authentication. Does not throw exception if
	  /// no authentication exists.
	  /// 
	  /// </summary>
	  void clearAuthentication();

	  /// <summary>
	  /// Sets the picture for a given user. </summary>
	  /// <exception cref="ProcessEngineException"> if the user doesn't exist. </exception>
	  /// <param name="picture"> can be null to delete the picture.  </param>
	  void setUserPicture(string userId, Picture picture);

	  /// <summary>
	  /// Retrieves the picture for a given user. </summary>
	  /// <exception cref="ProcessEngineException"> if the user doesn't exist.
	  /// @returns null if the user doesn't have a picture.  </exception>
	  Picture getUserPicture(string userId);

	  /// <summary>
	  /// Deletes the picture for a given user. If the user does not have a picture or if the user doesn't exists the call is ignored. </summary>
	  /// <exception cref="ProcessEngineException"> if the user doesn't exist.  </exception>
	  void deleteUserPicture(string userId);

	  /// <summary>
	  /// Generic extensibility key-value pairs associated with a user </summary>
	  void setUserInfo(string userId, string key, string value);

	  /// <summary>
	  /// Generic extensibility key-value pairs associated with a user </summary>
	  string getUserInfo(string userId, string key);

	  /// <summary>
	  /// Generic extensibility keys associated with a user </summary>
	  IList<string> getUserInfoKeys(string userId);

	  /// <summary>
	  /// Delete an entry of the generic extensibility key-value pairs associated with a user </summary>
	  void deleteUserInfo(string userId, string key);

	  /// <summary>
	  /// Store account information for a remote system </summary>
	  [Obsolete]
	  void setUserAccount(string userId, string userPassword, string accountName, string accountUsername, string accountPassword, IDictionary<string, string> accountDetails);

	  /// <summary>
	  /// Get account names associated with the given user </summary>
	  [Obsolete]
	  IList<string> getUserAccountNames(string userId);

	  /// <summary>
	  /// Get account information associated with a user </summary>
	  [Obsolete]
	  Account getUserAccount(string userId, string userPassword, string accountName);

	  /// <summary>
	  /// Delete an entry of the generic extensibility key-value pairs associated with a user </summary>
	  [Obsolete]
	  void deleteUserAccount(string userId, string accountName);

	}

}