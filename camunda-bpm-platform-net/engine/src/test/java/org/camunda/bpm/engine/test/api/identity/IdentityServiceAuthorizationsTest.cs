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
namespace org.camunda.bpm.engine.test.api.identity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP_MEMBERSHIP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT_MEMBERSHIP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestUtil.assertExceptionInfo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
	using TenantEntity = org.camunda.bpm.engine.impl.persistence.entity.TenantEntity;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class IdentityServiceAuthorizationsTest : PluggableProcessEngineTestCase
	{

	  private const string jonny2 = "jonny2";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		cleanupAfterTest();
		base.tearDown();
	  }

	  public virtual void testUserCreateAuthorizations()
	  {

		// add base permission which allows nobody to create users:
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = USER;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'create'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.newUser("jonny1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, USER.resourceName(), null, info);
		}

		// circumvent auth check to get new transient userobject
		User newUser = new UserEntity("jonny1");

		try
		{
		  identityService.saveUser(newUser);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, USER.resourceName(), null, info);
		}
	  }

	  public virtual void testUserDeleteAuthorizations()
	  {

		// crate user while still in god-mode:
		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = USER;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(DELETE); // revoke delete
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.deleteUser("jonny1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, USER.resourceName(), "jonny1", info);
		}
	  }

	  public virtual void testTenantAuthorizationAfterDeleteUser()
	  {
		// given jonny2 who is allowed to do user operations
		User jonny = identityService.newUser(jonny2);
		identityService.saveUser(jonny);

		grantPermissions();

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		// create user
		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);
		string jonny1Id = jonny1.Id;

		// and tenant
		string tenant1 = "tenant1";
		Tenant tenant = identityService.newTenant(tenant1);
		identityService.saveTenant(tenant);
		identityService.createTenantUserMembership(tenant1, jonny1Id);

		// assume
		TenantQuery query = identityService.createTenantQuery().userMember(jonny1Id);
		assertThat(query.count(), @is(1L));

		// when
		identityService.deleteUser(jonny1Id);

		// turn off authorization
		processEngineConfiguration.AuthorizationEnabled = false;

		// then
		assertThat(query.count(), @is(0L));
		assertThat(authorizationService.createAuthorizationQuery().resourceType(TENANT).userIdIn(jonny1Id).count(), @is(0L));
	  }

	  public virtual void testUserUpdateAuthorizations()
	  {

		// crate user while still in god-mode:
		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = USER;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(UPDATE); // revoke update
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		// fetch user:
		jonny1 = identityService.createUserQuery().singleResult();
		jonny1.FirstName = "Jonny";

		try
		{
		  identityService.saveUser(jonny1);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(UPDATE.Name, USER.resourceName(), "jonny1", info);
		}

		// but I can create a new user:
		User jonny3 = identityService.newUser("jonny3");
		identityService.saveUser(jonny3);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testUserUnlock() throws java.text.ParseException
	  public virtual void testUserUnlock()
	  {

		// crate user while still in god-mode:
		string userId = "jonny";
		User jonny = identityService.newUser(userId);
		jonny.Password = "xxx";
		identityService.saveUser(jonny);

		lockUser(userId, "invalid pwd");

		// assume
		int maxNumOfAttempts = 10;
		UserEntity lockedUser = (UserEntity) identityService.createUserQuery().userId(jonny.Id).singleResult();
		assertNotNull(lockedUser);
		assertNotNull(lockedUser.LockExpirationTime);
		assertEquals(maxNumOfAttempts, lockedUser.Attempts);


		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = USER;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		authorizationService.saveAuthorization(basePerms);

		// set auth
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.setAuthentication("admin", Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN), null);

		// when
		identityService.unlockUser(lockedUser.Id);

		// then
		lockedUser = (UserEntity) identityService.createUserQuery().userId(jonny.Id).singleResult();
		assertNotNull(lockedUser);
		assertNull(lockedUser.LockExpirationTime);
		assertEquals(0, lockedUser.Attempts);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testUserUnlockWithoutAuthorization() throws java.text.ParseException
	  public virtual void testUserUnlockWithoutAuthorization()
	  {

		// crate user while still in god-mode:
		string userId = "jonny";
		User jonny = identityService.newUser(userId);
		jonny.Password = "xxx";
		identityService.saveUser(jonny);

		lockUser(userId, "invalid pwd");

		// assume
		int maxNumOfAttempts = 10;
		UserEntity lockedUser = (UserEntity) identityService.createUserQuery().userId(jonny.Id).singleResult();
		assertNotNull(lockedUser);
		assertNotNull(lockedUser.LockExpirationTime);
		assertEquals(maxNumOfAttempts, lockedUser.Attempts);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.setAuthentication("admin", null, null);

		// when
		try
		{
		  identityService.unlockUser(lockedUser.Id);
		  fail("expected exception");
		}
		catch (AuthorizationException e)
		{
		  assertTrue(e.Message.contains("ENGINE-03029 Required admin authenticated group or user."));
		}

		// return to god-mode
		processEngineConfiguration.AuthorizationEnabled = false;

		// then
		int maxNumOfLoginAttempts = 10;
		lockedUser = (UserEntity) identityService.createUserQuery().userId(jonny.Id).singleResult();
		assertNotNull(lockedUser);
		assertNotNull(lockedUser.LockExpirationTime);
		assertEquals(maxNumOfLoginAttempts, lockedUser.Attempts);
	  }

	  public virtual void testGroupCreateAuthorizations()
	  {

		// add base permission which allows nobody to create groups:
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = GROUP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'create'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.newGroup("group1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, GROUP.resourceName(), null, info);
		}

		// circumvent auth check to get new transient userobject
		Group group = new GroupEntity("group1");

		try
		{
		  identityService.saveGroup(group);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, GROUP.resourceName(), null, info);
		}
	  }

	  public virtual void testGroupDeleteAuthorizations()
	  {

		// crate group while still in god-mode:
		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = GROUP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(DELETE); // revoke delete
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.deleteGroup("group1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, GROUP.resourceName(), "group1", info);
		}

	  }

	  public virtual void testTenantAuthorizationAfterDeleteGroup()
	  {
		// given jonny2 who is allowed to do group operations
		User jonny = identityService.newUser(jonny2);
		identityService.saveUser(jonny);

		grantPermissions();

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		// create group
		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		// and tenant
		string tenant1 = "tenant1";
		Tenant tenant = identityService.newTenant(tenant1);
		identityService.saveTenant(tenant);
		identityService.createTenantGroupMembership(tenant1, "group1");

		// assume
		TenantQuery query = identityService.createTenantQuery().groupMember("group1");
		assertThat(query.count(), @is(1L));

		// when
		identityService.deleteGroup("group1");

		// turn off authorization
		processEngineConfiguration.AuthorizationEnabled = false;

		// then
		assertThat(query.count(), @is(0L));
		assertThat(authorizationService.createAuthorizationQuery().resourceType(TENANT).groupIdIn("group1").count(), @is(0L));
	  }


	  public virtual void testGroupUpdateAuthorizations()
	  {

		// crate group while still in god-mode:
		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = GROUP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(UPDATE); // revoke update
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		// fetch user:
		group1 = identityService.createGroupQuery().singleResult();
		group1.Name = "Group 1";

		try
		{
		  identityService.saveGroup(group1);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(UPDATE.Name, GROUP.resourceName(), "group1", info);
		}

		// but I can create a new group:
		Group group2 = identityService.newGroup("group2");
		identityService.saveGroup(group2);

	  }

	  public virtual void testTenantCreateAuthorizations()
	  {

		// add base permission which allows nobody to create tenants:
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'create'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.newTenant("tenant");

		  fail("exception expected");
		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, TENANT.resourceName(), null, info);
		}

		// circumvent auth check to get new transient userobject
		Tenant tenant = new TenantEntity("tenant");

		try
		{
		  identityService.saveTenant(tenant);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, TENANT.resourceName(), null, info);
		}
	  }

	  public virtual void testTenantDeleteAuthorizations()
	  {

		// create tenant
		Tenant tenant = new TenantEntity("tenant");
		identityService.saveTenant(tenant);

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(DELETE); // revoke delete
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.deleteTenant("tenant");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, TENANT.resourceName(), "tenant", info);
		}
	  }

	  public virtual void testTenantUpdateAuthorizations()
	  {

		// create tenant
		Tenant tenant = new TenantEntity("tenant");
		identityService.saveTenant(tenant);

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(UPDATE); // revoke update
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		// fetch user:
		tenant = identityService.createTenantQuery().singleResult();
		tenant.Name = "newName";

		try
		{
		  identityService.saveTenant(tenant);

		  fail("exception expected");
		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(UPDATE.Name, TENANT.resourceName(), "tenant", info);
		}

		// but I can create a new tenant:
		Tenant newTenant = identityService.newTenant("newTenant");
		identityService.saveTenant(newTenant);
	  }

	  public virtual void testMembershipCreateAuthorizations()
	  {

		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		// add base permission which allows nobody to add users to groups
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = GROUP_MEMBERSHIP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'crate'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.createMembership("jonny1", "group1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, GROUP_MEMBERSHIP.resourceName(), "group1", info);
		}
	  }

	  public virtual void testMembershipDeleteAuthorizations()
	  {

		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		// add base permission which allows nobody to add users to groups
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = GROUP_MEMBERSHIP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'delete'
		basePerms.removePermission(DELETE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.deleteMembership("jonny1", "group1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, GROUP_MEMBERSHIP.resourceName(), "group1", info);
		}
	  }

	  public virtual void testTenantUserMembershipCreateAuthorizations()
	  {

		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		Tenant tenant1 = identityService.newTenant("tenant1");
		identityService.saveTenant(tenant1);

		// add base permission which allows nobody to create memberships
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT_MEMBERSHIP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'create'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.createTenantUserMembership("tenant1", "jonny1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, TENANT_MEMBERSHIP.resourceName(), "tenant1", info);
		}
	  }

	  public virtual void testTenantGroupMembershipCreateAuthorizations()
	  {

		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		Tenant tenant1 = identityService.newTenant("tenant1");
		identityService.saveTenant(tenant1);

		// add base permission which allows nobody to create memberships
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT_MEMBERSHIP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'create'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.createTenantGroupMembership("tenant1", "group1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, TENANT_MEMBERSHIP.resourceName(), "tenant1", info);
		}
	  }

	  public virtual void testTenantUserMembershipDeleteAuthorizations()
	  {

		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		Tenant tenant1 = identityService.newTenant("tenant1");
		identityService.saveTenant(tenant1);

		// add base permission which allows nobody to delete memberships
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT_MEMBERSHIP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'delete'
		basePerms.removePermission(DELETE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.deleteTenantUserMembership("tenant1", "jonny1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, TENANT_MEMBERSHIP.resourceName(), "tenant1", info);
		}
	  }

	  public virtual void testTenanGroupMembershipDeleteAuthorizations()
	  {

		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		Tenant tenant1 = identityService.newTenant("tenant1");
		identityService.saveTenant(tenant1);

		// add base permission which allows nobody to delete memberships
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT_MEMBERSHIP;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'delete'
		basePerms.removePermission(DELETE);
		authorizationService.saveAuthorization(basePerms);

		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.deleteTenantGroupMembership("tenant1", "group1");
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, TENANT_MEMBERSHIP.resourceName(), "tenant1", info);
		}
	  }

	  public virtual void testUserQueryAuthorizations()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new user jonny1
		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);

		// set base permission for all users (no-one has any permissions on users)
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = USER;
		basePerms.ResourceId = ANY;
		authorizationService.saveAuthorization(basePerms);

		// now enable checks
		processEngineConfiguration.AuthorizationEnabled = true;

		// we cannot fetch the user
		assertNull(identityService.createUserQuery().singleResult());
		assertEquals(0, identityService.createUserQuery().count());

		processEngineConfiguration.AuthorizationEnabled = false;

		// now we add permission for jonny2 to read the user:
		Authorization ourPerms = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		ourPerms.UserId = authUserId;
		ourPerms.Resource = USER;
		ourPerms.ResourceId = ANY;
		ourPerms.addPermission(READ);
		authorizationService.saveAuthorization(ourPerms);

		processEngineConfiguration.AuthorizationEnabled = true;

		// now we can fetch the user
		assertNotNull(identityService.createUserQuery().singleResult());
		assertEquals(1, identityService.createUserQuery().count());

		// change the base permission:
		processEngineConfiguration.AuthorizationEnabled = false;
		basePerms = authorizationService.createAuthorizationQuery().resourceType(USER).userIdIn("*").singleResult();
		basePerms.addPermission(READ);
		authorizationService.saveAuthorization(basePerms);
		processEngineConfiguration.AuthorizationEnabled = true;

		// we can still fetch the user
		assertNotNull(identityService.createUserQuery().singleResult());
		assertEquals(1, identityService.createUserQuery().count());


		// revoke permission for jonny2:
		processEngineConfiguration.AuthorizationEnabled = false;
		ourPerms = authorizationService.createAuthorizationQuery().resourceType(USER).userIdIn(authUserId).singleResult();
		ourPerms.removePermission(READ);
		authorizationService.saveAuthorization(ourPerms);

		Authorization revoke = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		revoke.UserId = authUserId;
		revoke.Resource = USER;
		revoke.ResourceId = ANY;
		revoke.removePermission(READ);
		authorizationService.saveAuthorization(revoke);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now we cannot fetch the user
		assertNull(identityService.createUserQuery().singleResult());
		assertEquals(0, identityService.createUserQuery().count());


		// delete our perms
		processEngineConfiguration.AuthorizationEnabled = false;
		authorizationService.deleteAuthorization(ourPerms.Id);
		authorizationService.deleteAuthorization(revoke.Id);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now the base permission applies and grants us read access
		assertNotNull(identityService.createUserQuery().singleResult());
		assertEquals(1, identityService.createUserQuery().count());

	  }

	  public virtual void testUserQueryAuthorizationsMultipleGroups()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		User demo = identityService.newUser("demo");
		identityService.saveUser(demo);

		User mary = identityService.newUser("mary");
		identityService.saveUser(mary);

		User peter = identityService.newUser("peter");
		identityService.saveUser(peter);

		User john = identityService.newUser("john");
		identityService.saveUser(john);

		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);

		Group accounting = identityService.newGroup("accounting");
		identityService.saveGroup(accounting);

		Group management = identityService.newGroup("management");
		identityService.saveGroup(management);

		identityService.createMembership("demo", "sales");
		identityService.createMembership("demo", "accounting");
		identityService.createMembership("demo", "management");

		identityService.createMembership("john", "sales");
		identityService.createMembership("mary", "accounting");
		identityService.createMembership("peter", "management");

		Authorization demoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		demoAuth.UserId = "demo";
		demoAuth.Resource = USER;
		demoAuth.ResourceId = "demo";
		demoAuth.addPermission(ALL);
		authorizationService.saveAuthorization(demoAuth);

		Authorization johnAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		johnAuth.UserId = "john";
		johnAuth.Resource = USER;
		johnAuth.ResourceId = "john";
		johnAuth.addPermission(ALL);
		authorizationService.saveAuthorization(johnAuth);

		Authorization maryAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		maryAuth.UserId = "mary";
		maryAuth.Resource = USER;
		maryAuth.ResourceId = "mary";
		maryAuth.addPermission(ALL);
		authorizationService.saveAuthorization(maryAuth);

		Authorization peterAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		peterAuth.UserId = "peter";
		peterAuth.Resource = USER;
		peterAuth.ResourceId = "peter";
		peterAuth.addPermission(ALL);
		authorizationService.saveAuthorization(peterAuth);

		Authorization accAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		accAuth.GroupId = "accounting";
		accAuth.Resource = GROUP;
		accAuth.ResourceId = "accounting";
		accAuth.addPermission(READ);
		authorizationService.saveAuthorization(accAuth);

		Authorization salesAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		salesAuth.GroupId = "sales";
		salesAuth.Resource = GROUP;
		salesAuth.ResourceId = "sales";
		salesAuth.addPermission(READ);
		authorizationService.saveAuthorization(salesAuth);

		Authorization manAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		manAuth.GroupId = "management";
		manAuth.Resource = GROUP;
		manAuth.ResourceId = "management";
		manAuth.addPermission(READ);
		authorizationService.saveAuthorization(manAuth);

		Authorization salesDemoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		salesDemoAuth.GroupId = "sales";
		salesDemoAuth.Resource = USER;
		salesDemoAuth.ResourceId = "demo";
		salesDemoAuth.addPermission(READ);
		authorizationService.saveAuthorization(salesDemoAuth);

		Authorization salesJohnAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		salesJohnAuth.GroupId = "sales";
		salesJohnAuth.Resource = USER;
		salesJohnAuth.ResourceId = "john";
		salesJohnAuth.addPermission(READ);
		authorizationService.saveAuthorization(salesJohnAuth);

		Authorization manDemoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		manDemoAuth.GroupId = "management";
		manDemoAuth.Resource = USER;
		manDemoAuth.ResourceId = "demo";
		manDemoAuth.addPermission(READ);
		authorizationService.saveAuthorization(manDemoAuth);

		Authorization manPeterAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		manPeterAuth.GroupId = "management";
		manPeterAuth.Resource = USER;
		manPeterAuth.ResourceId = "peter";
		manPeterAuth.addPermission(READ);
		authorizationService.saveAuthorization(manPeterAuth);

		Authorization accDemoAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		accDemoAuth.GroupId = "accounting";
		accDemoAuth.Resource = USER;
		accDemoAuth.ResourceId = "demo";
		accDemoAuth.addPermission(READ);
		authorizationService.saveAuthorization(accDemoAuth);

		Authorization accMaryAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		accMaryAuth.GroupId = "accounting";
		accMaryAuth.Resource = USER;
		accMaryAuth.ResourceId = "mary";
		accMaryAuth.addPermission(READ);
		authorizationService.saveAuthorization(accMaryAuth);

		IList<string> groups = new List<string>();
		groups.Add("management");
		groups.Add("accounting");
		groups.Add("sales");

		identityService.setAuthentication("demo", groups);

		processEngineConfiguration.AuthorizationEnabled = true;

		IList<User> salesUser = identityService.createUserQuery().memberOfGroup("sales").list();
		assertEquals(2, salesUser.Count);

		foreach (User user in salesUser)
		{
		  if (!user.Id.Equals("demo") && !user.Id.Equals("john"))
		  {
			Assert.fail("Unexpected user for group sales: " + user.Id);
		  }
		}

		IList<User> accountingUser = identityService.createUserQuery().memberOfGroup("accounting").list();
		assertEquals(2, accountingUser.Count);

		foreach (User user in accountingUser)
		{
		  if (!user.Id.Equals("demo") && !user.Id.Equals("mary"))
		  {
			Assert.fail("Unexpected user for group accounting: " + user.Id);
		  }
		}

		IList<User> managementUser = identityService.createUserQuery().memberOfGroup("management").list();
		assertEquals(2, managementUser.Count);

		foreach (User user in managementUser)
		{
		  if (!user.Id.Equals("demo") && !user.Id.Equals("peter"))
		  {
			Assert.fail("Unexpected user for group managment: " + user.Id);
		  }
		}
	  }

	  public virtual void testGroupQueryAuthorizations()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new user jonny1
		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);
		// create new group
		Group group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		// set base permission for all users (no-one has any permissions on groups)
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = GROUP;
		basePerms.ResourceId = ANY;
		authorizationService.saveAuthorization(basePerms);

		// now enable checks
		processEngineConfiguration.AuthorizationEnabled = true;

		// we cannot fetch the group
		assertNull(identityService.createGroupQuery().singleResult());
		assertEquals(0, identityService.createGroupQuery().count());

		// now we add permission for jonny2 to read the group:
		processEngineConfiguration.AuthorizationEnabled = false;
		Authorization ourPerms = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		ourPerms.UserId = authUserId;
		ourPerms.Resource = GROUP;
		ourPerms.ResourceId = ANY;
		ourPerms.addPermission(READ);
		authorizationService.saveAuthorization(ourPerms);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now we can fetch the group
		assertNotNull(identityService.createGroupQuery().singleResult());
		assertEquals(1, identityService.createGroupQuery().count());

		// change the base permission:
		processEngineConfiguration.AuthorizationEnabled = false;
		basePerms = authorizationService.createAuthorizationQuery().resourceType(GROUP).userIdIn("*").singleResult();
		basePerms.addPermission(READ);
		authorizationService.saveAuthorization(basePerms);
		processEngineConfiguration.AuthorizationEnabled = true;

		// we can still fetch the group
		assertNotNull(identityService.createGroupQuery().singleResult());
		assertEquals(1, identityService.createGroupQuery().count());

		// revoke permission for jonny2:
		processEngineConfiguration.AuthorizationEnabled = false;
		ourPerms = authorizationService.createAuthorizationQuery().resourceType(GROUP).userIdIn(authUserId).singleResult();
		ourPerms.removePermission(READ);
		authorizationService.saveAuthorization(ourPerms);

		Authorization revoke = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		revoke.UserId = authUserId;
		revoke.Resource = GROUP;
		revoke.ResourceId = ANY;
		revoke.removePermission(READ);
		authorizationService.saveAuthorization(revoke);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now we cannot fetch the group
		assertNull(identityService.createGroupQuery().singleResult());
		assertEquals(0, identityService.createGroupQuery().count());

		// delete our perms
		processEngineConfiguration.AuthorizationEnabled = false;
		authorizationService.deleteAuthorization(ourPerms.Id);
		authorizationService.deleteAuthorization(revoke.Id);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now the base permission applies and grants us read access
		assertNotNull(identityService.createGroupQuery().singleResult());
		assertEquals(1, identityService.createGroupQuery().count());

	  }

	  public virtual void testTenantQueryAuthorizations()
	  {
		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new user jonny1
		User jonny1 = identityService.newUser("jonny1");
		identityService.saveUser(jonny1);
		// create new tenant
		Tenant tenant = identityService.newTenant("tenant");
		identityService.saveTenant(tenant);

		// set base permission for all users (no-one has any permissions on tenants)
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = TENANT;
		basePerms.ResourceId = ANY;
		authorizationService.saveAuthorization(basePerms);

		// now enable checks
		processEngineConfiguration.AuthorizationEnabled = true;

		// we cannot fetch the tenants
		assertEquals(0, identityService.createTenantQuery().count());

		// now we add permission for jonny2 to read the tenants:
		processEngineConfiguration.AuthorizationEnabled = false;
		Authorization ourPerms = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		ourPerms.UserId = authUserId;
		ourPerms.Resource = TENANT;
		ourPerms.ResourceId = ANY;
		ourPerms.addPermission(READ);
		authorizationService.saveAuthorization(ourPerms);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now we can fetch the tenants
		assertEquals(1, identityService.createTenantQuery().count());

		// change the base permission:
		processEngineConfiguration.AuthorizationEnabled = false;
		basePerms = authorizationService.createAuthorizationQuery().resourceType(TENANT).userIdIn("*").singleResult();
		basePerms.addPermission(READ);
		authorizationService.saveAuthorization(basePerms);
		processEngineConfiguration.AuthorizationEnabled = true;

		// we can still fetch the tenants
		assertEquals(1, identityService.createTenantQuery().count());

		// revoke permission for jonny2:
		processEngineConfiguration.AuthorizationEnabled = false;
		ourPerms = authorizationService.createAuthorizationQuery().resourceType(TENANT).userIdIn(authUserId).singleResult();
		ourPerms.removePermission(READ);
		authorizationService.saveAuthorization(ourPerms);

		Authorization revoke = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		revoke.UserId = authUserId;
		revoke.Resource = TENANT;
		revoke.ResourceId = ANY;
		revoke.removePermission(READ);
		authorizationService.saveAuthorization(revoke);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now we cannot fetch the tenants
		assertEquals(0, identityService.createTenantQuery().count());

		// delete our permissions
		processEngineConfiguration.AuthorizationEnabled = false;
		authorizationService.deleteAuthorization(ourPerms.Id);
		authorizationService.deleteAuthorization(revoke.Id);
		processEngineConfiguration.AuthorizationEnabled = true;

		// now the base permission applies and grants us read access
		assertEquals(1, identityService.createTenantQuery().count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void lockUser(String userId, String invalidPassword) throws java.text.ParseException
	  protected internal virtual void lockUser(string userId, string invalidPassword)
	  {
		DateTime now = ClockUtil.CurrentTime;
		try
		{
		  for (int i = 0; i <= 11; i++)
		  {
			assertFalse(identityService.checkPassword(userId, invalidPassword));
			now = DateUtils.addMinutes(ClockUtil.CurrentTime, 1);
			ClockUtil.CurrentTime = now;
		  }
		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
	  }

	  protected internal virtual void grantPermissions()
	  {
		AuthorizationEntity userAdminAuth = new AuthorizationEntity(AUTH_TYPE_GLOBAL);
		userAdminAuth.setResource(USER);
		userAdminAuth.ResourceId = ANY;
		userAdminAuth.addPermission(ALL);
		authorizationService.saveAuthorization(userAdminAuth);

		userAdminAuth = new AuthorizationEntity(AUTH_TYPE_GLOBAL);
		userAdminAuth.setResource(GROUP);
		userAdminAuth.ResourceId = ANY;
		userAdminAuth.addPermission(ALL);
		authorizationService.saveAuthorization(userAdminAuth);

		userAdminAuth = new AuthorizationEntity(AUTH_TYPE_GLOBAL);
		userAdminAuth.setResource(TENANT);
		userAdminAuth.ResourceId = ANY;
		userAdminAuth.addPermission(ALL);
		authorizationService.saveAuthorization(userAdminAuth);

		userAdminAuth = new AuthorizationEntity(AUTH_TYPE_GLOBAL);
		userAdminAuth.setResource(TENANT_MEMBERSHIP);
		userAdminAuth.ResourceId = ANY;
		userAdminAuth.addPermission(ALL);
		authorizationService.saveAuthorization(userAdminAuth);
	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Tenant tenant in identityService.createTenantQuery().list())
		{
		  identityService.deleteTenant(tenant.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	}

}