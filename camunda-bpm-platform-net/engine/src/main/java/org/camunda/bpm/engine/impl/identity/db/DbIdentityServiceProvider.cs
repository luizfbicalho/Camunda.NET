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
namespace org.camunda.bpm.engine.impl.identity.db
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
	using MembershipEntity = org.camunda.bpm.engine.impl.persistence.entity.MembershipEntity;
	using TenantEntity = org.camunda.bpm.engine.impl.persistence.entity.TenantEntity;
	using TenantMembershipEntity = org.camunda.bpm.engine.impl.persistence.entity.TenantMembershipEntity;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// <para><seealso cref="WritableIdentityProvider"/> implementation backed by a
	/// database. This implementation is used for the built-in user management.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbIdentityServiceProvider : DbReadOnlyIdentityServiceProvider, WritableIdentityProvider
	{

	  // users ////////////////////////////////////////////////////////

	  public virtual UserEntity createNewUser(string userId)
	  {
		checkAuthorization(Permissions.CREATE, Resources.USER, null);
		return new UserEntity(userId);
	  }

	  public virtual IdentityOperationResult saveUser(User user)
	  {
		UserEntity userEntity = (UserEntity) user;

		// encrypt password
		userEntity.encryptPassword();

		string operation = null;
		if (userEntity.Revision == 0)
		{
		  operation = IdentityOperationResult.OPERATION_CREATE;
		  checkAuthorization(Permissions.CREATE, Resources.USER, null);
		  DbEntityManager.insert(userEntity);
		  createDefaultAuthorizations(userEntity);
		}
		else
		{
		  operation = IdentityOperationResult.OPERATION_UPDATE;
		  checkAuthorization(Permissions.UPDATE, Resources.USER, user.Id);
		  DbEntityManager.merge(userEntity);
		}

		return new IdentityOperationResult(userEntity, operation);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.identity.IdentityOperationResult deleteUser(final String userId)
	  public virtual IdentityOperationResult deleteUser(string userId)
	  {
		checkAuthorization(Permissions.DELETE, Resources.USER, userId);
		UserEntity user = findUserById(userId);
		if (user != null)
		{
		  deleteMembershipsByUserId(userId);
		  deleteTenantMembershipsOfUser(userId);

		  deleteAuthorizations(Resources.USER, userId);

		  Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, userId));

		  DbEntityManager.delete(user);
		  return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_DELETE);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly DbIdentityServiceProvider outerInstance;

		  private string userId;

		  public CallableAnonymousInnerClass(DbIdentityServiceProvider outerInstance, string userId)
		  {
			  this.outerInstance = outerInstance;
			  this.userId = userId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.identity.Tenant> tenants = createTenantQuery().userMember(userId).list();
			IList<Tenant> tenants = outerInstance.createTenantQuery().userMember(userId).list();
			if (tenants != null && tenants.Count > 0)
			{
			  foreach (Tenant tenant in tenants)
			  {
				outerInstance.deleteAuthorizationsForUser(Resources.TENANT, tenant.Id, userId);
			  }
			}
			return null;
		  }
	  }

	  public override bool checkPassword(string userId, string password)
	  {
		UserEntity user = findUserById(userId);
		if (user == null || string.ReferenceEquals(password, null))
		{
		  return false;
		}

		if (isUserLocked(user))
		{
		  throw new AuthenticationException(userId, user.LockExpirationTime);
		}

		if (matchPassword(password, user))
		{
		  unlockUser(user);
		  return true;
		}
		else
		{
		  lockUser(user);
		  return false;
		}
	  }

	  protected internal virtual bool isUserLocked(UserEntity user)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		int maxAttempts = processEngineConfiguration.LoginMaxAttempts;
		int attempts = user.Attempts;

		if (attempts >= maxAttempts)
		{
		  throw new AuthenticationException(user.Id);
		}

		DateTime lockExpirationTime = user.LockExpirationTime;
		DateTime currentTime = ClockUtil.CurrentTime;

		return lockExpirationTime != null && lockExpirationTime > currentTime;
	  }

	  protected internal virtual void lockUser(UserEntity user)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		int max = processEngineConfiguration.LoginDelayMaxTime;
		int baseTime = processEngineConfiguration.LoginDelayBase;
		int factor = processEngineConfiguration.LoginDelayFactor;
		int attempts = user.Attempts + 1;

		long delay = (long)(baseTime * Math.Pow(factor, attempts - 1));
		delay = Math.Min(delay, max) * 1000;

		long currentTime = ClockUtil.CurrentTime.Ticks;
		DateTime lockExpirationTime = new DateTime(currentTime + delay);

		IdentityInfoManager.updateUserLock(user, attempts, lockExpirationTime);
	  }

	  public virtual IdentityOperationResult unlockUser(string userId)
	  {
		UserEntity user = findUserById(userId);
		if (user != null)
		{
		  return unlockUser(user);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  protected internal virtual IdentityOperationResult unlockUser(UserEntity user)
	  {
		if (user.Attempts > 0 || user.LockExpirationTime != null)
		{
		  IdentityInfoManager.updateUserLock(user, 0, null);
		  return new IdentityOperationResult(user, IdentityOperationResult.OPERATION_UNLOCK);
		}
		return new IdentityOperationResult(user, IdentityOperationResult.OPERATION_NONE);
	  }

	  // groups ////////////////////////////////////////////////////////

	  public virtual GroupEntity createNewGroup(string groupId)
	  {
		checkAuthorization(Permissions.CREATE, Resources.GROUP, null);
		return new GroupEntity(groupId);
	  }

	  public virtual IdentityOperationResult saveGroup(Group group)
	  {
		GroupEntity groupEntity = (GroupEntity) group;
		string operation = null;
		if (groupEntity.Revision == 0)
		{
		  operation = IdentityOperationResult.OPERATION_CREATE;
		  checkAuthorization(Permissions.CREATE, Resources.GROUP, null);
		  DbEntityManager.insert(groupEntity);
		  createDefaultAuthorizations(group);
		}
		else
		{
		  operation = IdentityOperationResult.OPERATION_UPDATE;
		  checkAuthorization(Permissions.UPDATE, Resources.GROUP, group.Id);
		  DbEntityManager.merge(groupEntity);
		}
		return new IdentityOperationResult(groupEntity, operation);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.identity.IdentityOperationResult deleteGroup(final String groupId)
	  public virtual IdentityOperationResult deleteGroup(string groupId)
	  {
		checkAuthorization(Permissions.DELETE, Resources.GROUP, groupId);
		GroupEntity group = findGroupById(groupId);
		if (group != null)
		{
		  deleteMembershipsByGroupId(groupId);
		  deleteTenantMembershipsOfGroup(groupId);

		  deleteAuthorizations(Resources.GROUP, groupId);

		  Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass2(this, groupId));
		  DbEntityManager.delete(group);
		  return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_DELETE);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly DbIdentityServiceProvider outerInstance;

		  private string groupId;

		  public CallableAnonymousInnerClass2(DbIdentityServiceProvider outerInstance, string groupId)
		  {
			  this.outerInstance = outerInstance;
			  this.groupId = groupId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.identity.Tenant> tenants = createTenantQuery().groupMember(groupId).list();
			IList<Tenant> tenants = outerInstance.createTenantQuery().groupMember(groupId).list();
			if (tenants != null && tenants.Count > 0)
			{
			  foreach (Tenant tenant in tenants)
			  {
				outerInstance.deleteAuthorizationsForGroup(Resources.TENANT, tenant.Id, groupId);
			  }
			}
			return null;
		  }
	  }

	  // tenants //////////////////////////////////////////////////////

	  public virtual Tenant createNewTenant(string tenantId)
	  {
		checkAuthorization(Permissions.CREATE, Resources.TENANT, null);
		return new TenantEntity(tenantId);
	  }

	  public virtual IdentityOperationResult saveTenant(Tenant tenant)
	  {
		TenantEntity tenantEntity = (TenantEntity) tenant;
		string operation = null;
		if (tenantEntity.Revision == 0)
		{
		  operation = IdentityOperationResult.OPERATION_CREATE;
		  checkAuthorization(Permissions.CREATE, Resources.TENANT, null);
		  DbEntityManager.insert(tenantEntity);
		  createDefaultAuthorizations(tenant);
		}
		else
		{
		  operation = IdentityOperationResult.OPERATION_UPDATE;
		  checkAuthorization(Permissions.UPDATE, Resources.TENANT, tenant.Id);
		  DbEntityManager.merge(tenantEntity);
		}
		return new IdentityOperationResult(tenantEntity, operation);
	  }

	  public virtual IdentityOperationResult deleteTenant(string tenantId)
	  {
		checkAuthorization(Permissions.DELETE, Resources.TENANT, tenantId);
		TenantEntity tenant = findTenantById(tenantId);
		if (tenant != null)
		{
		  deleteTenantMembershipsOfTenant(tenantId);

		  deleteAuthorizations(Resources.TENANT, tenantId);
		  DbEntityManager.delete(tenant);
		  return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_DELETE);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  // membership //////////////////////////////////////////////////////

	  public virtual IdentityOperationResult createMembership(string userId, string groupId)
	  {
		checkAuthorization(Permissions.CREATE, Resources.GROUP_MEMBERSHIP, groupId);
		UserEntity user = findUserById(userId);
		GroupEntity group = findGroupById(groupId);
		MembershipEntity membership = new MembershipEntity();
		membership.User = user;
		membership.Group = group;
		DbEntityManager.insert(membership);
		createDefaultMembershipAuthorizations(userId, groupId);
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_CREATE);
	  }

	  public virtual IdentityOperationResult deleteMembership(string userId, string groupId)
	  {
		checkAuthorization(Permissions.DELETE, Resources.GROUP_MEMBERSHIP, groupId);
		if (existsMembership(userId, groupId))
		{
		  deleteAuthorizations(Resources.GROUP_MEMBERSHIP, groupId);

		  IDictionary<string, object> parameters = new Dictionary<string, object>();
		  parameters["userId"] = userId;
		  parameters["groupId"] = groupId;
		  DbEntityManager.delete(typeof(MembershipEntity), "deleteMembership", parameters);
		  return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_DELETE);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  protected internal virtual void deleteMembershipsByUserId(string userId)
	  {
		DbEntityManager.delete(typeof(MembershipEntity), "deleteMembershipsByUserId", userId);
	  }

	  protected internal virtual void deleteMembershipsByGroupId(string groupId)
	  {
		DbEntityManager.delete(typeof(MembershipEntity), "deleteMembershipsByGroupId", groupId);
	  }

	  public virtual IdentityOperationResult createTenantUserMembership(string tenantId, string userId)
	  {
		checkAuthorization(Permissions.CREATE, Resources.TENANT_MEMBERSHIP, tenantId);

		TenantEntity tenant = findTenantById(tenantId);
		UserEntity user = findUserById(userId);

		ensureNotNull("No tenant found with id '" + tenantId + "'.", "tenant", tenant);
		ensureNotNull("No user found with id '" + userId + "'.", "user", user);

		TenantMembershipEntity membership = new TenantMembershipEntity();
		membership.Tenant = tenant;
		membership.User = user;

		DbEntityManager.insert(membership);

		createDefaultTenantMembershipAuthorizations(tenant, user);
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_CREATE);
	  }

	  public virtual IdentityOperationResult createTenantGroupMembership(string tenantId, string groupId)
	  {
		checkAuthorization(Permissions.CREATE, Resources.TENANT_MEMBERSHIP, tenantId);

		TenantEntity tenant = findTenantById(tenantId);
		GroupEntity group = findGroupById(groupId);

		ensureNotNull("No tenant found with id '" + tenantId + "'.", "tenant", tenant);
		ensureNotNull("No group found with id '" + groupId + "'.", "group", group);

		TenantMembershipEntity membership = new TenantMembershipEntity();
		membership.Tenant = tenant;
		membership.Group = group;

		DbEntityManager.insert(membership);

		createDefaultTenantMembershipAuthorizations(tenant, group);
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_CREATE);
	  }

	  public virtual IdentityOperationResult deleteTenantUserMembership(string tenantId, string userId)
	  {
		checkAuthorization(Permissions.DELETE, Resources.TENANT_MEMBERSHIP, tenantId);
		if (existsTenantMembership(tenantId, userId, null))
		{
		  deleteAuthorizations(Resources.TENANT_MEMBERSHIP, userId);

		  deleteAuthorizationsForUser(Resources.TENANT, tenantId, userId);

		  IDictionary<string, object> parameters = new Dictionary<string, object>();
		  parameters["tenantId"] = tenantId;
		  parameters["userId"] = userId;
		  DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembership", parameters);
		  return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_DELETE);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  public virtual IdentityOperationResult deleteTenantGroupMembership(string tenantId, string groupId)
	  {
		checkAuthorization(Permissions.DELETE, Resources.TENANT_MEMBERSHIP, tenantId);

		if (existsTenantMembership(tenantId, null, groupId))
		{
		  deleteAuthorizations(Resources.TENANT_MEMBERSHIP, groupId);

		  deleteAuthorizationsForGroup(Resources.TENANT, tenantId, groupId);

		  IDictionary<string, object> parameters = new Dictionary<string, object>();
		  parameters["tenantId"] = tenantId;
		  parameters["groupId"] = groupId;
		  DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembership", parameters);
		  return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_DELETE);
		}
		return new IdentityOperationResult(null, IdentityOperationResult.OPERATION_NONE);
	  }

	  protected internal virtual void deleteTenantMembershipsOfUser(string userId)
	  {
		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembershipsOfUser", userId);
	  }

	  protected internal virtual void deleteTenantMembershipsOfGroup(string groupId)
	  {
		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembershipsOfGroup", groupId);
	  }

	  protected internal virtual void deleteTenantMembershipsOfTenant(string tenant)
	  {
		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembershipsOfTenant", tenant);
	  }

	  // authorizations ////////////////////////////////////////////////////////////

	  protected internal virtual void createDefaultAuthorizations(UserEntity userEntity)
	  {
		if (Context.ProcessEngineConfiguration.AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newUser(userEntity));
		}
	  }

	  protected internal virtual void createDefaultAuthorizations(Group group)
	  {
		if (AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newGroup(group));
		}
	  }

	  protected internal virtual void createDefaultAuthorizations(Tenant tenant)
	  {
		if (AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newTenant(tenant));
		}
	  }

	  protected internal virtual void createDefaultMembershipAuthorizations(string userId, string groupId)
	  {
		if (AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.groupMembershipCreated(groupId, userId));
		}
	  }

	  protected internal virtual void createDefaultTenantMembershipAuthorizations(Tenant tenant, User user)
	  {
		if (AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.tenantMembershipCreated(tenant, user));
		}
	  }

	  protected internal virtual void createDefaultTenantMembershipAuthorizations(Tenant tenant, Group group)
	  {
		if (AuthorizationEnabled)
		{
		  saveDefaultAuthorizations(ResourceAuthorizationProvider.tenantMembershipCreated(tenant, group));
		}
	  }

	}

}