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
namespace org.camunda.bpm.engine.test.api.identity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	public class IdentityServiceTenantTest
	{

	  protected internal const string USER_ONE = "user1";
	  protected internal const string USER_TWO = "user2";

	  protected internal const string GROUP_ONE = "group1";
	  protected internal const string GROUP_TWO = "group2";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  private readonly string INVALID_ID_MESSAGE = "%s has an invalid id: '%s' is not a valid resource identifier.";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal IdentityService identityService;
	  protected internal ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initService()
	  public virtual void initService()
	  {
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		identityService.deleteTenant(TENANT_ONE);
		identityService.deleteTenant(TENANT_TWO);

		identityService.deleteGroup(GROUP_ONE);
		identityService.deleteGroup(GROUP_TWO);

		identityService.deleteUser(USER_ONE);
		identityService.deleteUser(USER_TWO);

		if (processEngine != null)
		{
		  foreach (Tenant deleteTenant in processEngine.IdentityService.createTenantQuery().list())
		  {
			processEngine.IdentityService.deleteTenant(deleteTenant.Id);
		  }
		  foreach (Authorization authorization in processEngine.AuthorizationService.createAuthorizationQuery().list())
		  {
			processEngine.AuthorizationService.deleteAuthorization(authorization.Id);
		  }

		  processEngine.close();
		  ProcessEngines.unregister(processEngine);
		  processEngine = null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenant()
	  public virtual void createTenant()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		tenant.Name = "Tenant";
		identityService.saveTenant(tenant);

		tenant = identityService.createTenantQuery().singleResult();
		assertThat(tenant, @is(notNullValue()));
		assertThat(tenant.Id, @is(TENANT_ONE));
		assertThat(tenant.Name, @is("Tenant"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createExistingTenant()
	  public virtual void createExistingTenant()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		tenant.Name = "Tenant";
		identityService.saveTenant(tenant);

		Tenant secondTenant = identityService.newTenant(TENANT_ONE);
		secondTenant.Name = "Tenant";
		try
		{
		  identityService.saveTenant(secondTenant);
		  fail("BadUserRequestException is expected");
		}
		catch (Exception ex)
		{
		  if (!(ex is BadUserRequestException))
		  {
			fail("BadUserRequestException is expected, but another exception was received:  " + ex);
		  }
		  assertEquals("The tenant already exists", ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTenant()
	  public virtual void updateTenant()
	  {
		// create
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		tenant.Name = "Tenant";
		identityService.saveTenant(tenant);

		// update
		tenant = identityService.createTenantQuery().singleResult();
		assertThat(tenant, @is(notNullValue()));

		tenant.Name = "newName";
		identityService.saveTenant(tenant);

		tenant = identityService.createTenantQuery().singleResult();
		assertEquals("newName", tenant.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidTenantId()
	  public virtual void testInvalidTenantId()
	  {
		string invalidId = "john's tenant";
		try
		{
		  identityService.newTenant(invalidId);
		  fail("Invalid tenant id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Tenant", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidTenantIdOnUpdate()
	  public virtual void testInvalidTenantIdOnUpdate()
	  {
		string invalidId = "john's tenant";
		try
		{
		  Tenant updatedTenant = identityService.newTenant("john");
		  updatedTenant.Id = invalidId;
		  identityService.saveTenant(updatedTenant);

		  fail("Invalid tenant id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Tenant", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomCreateTenantWhitelistPattern()
	  public virtual void testCustomCreateTenantWhitelistPattern()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/generic.resource.id.whitelist.camunda.cfg.xml").buildProcessEngine();
		processEngine.ProcessEngineConfiguration.TenantResourceWhitelistPattern = "[a-zA-Z]+";

		string invalidId = "john's tenant";

		try
		{
		  processEngine.IdentityService.newTenant(invalidId);
		  fail("Invalid tenant id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Tenant", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomTenantWhitelistPattern()
	  public virtual void testCustomTenantWhitelistPattern()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/generic.resource.id.whitelist.camunda.cfg.xml").buildProcessEngine();
		processEngine.ProcessEngineConfiguration.TenantResourceWhitelistPattern = "[a-zA-Z]+";

		string validId = "johnsTenant";
		string invalidId = "john!@#$%";

		try
		{
		  Tenant tenant = processEngine.IdentityService.newTenant(validId);
		  tenant.Id = invalidId;
		  processEngine.IdentityService.saveTenant(tenant);

		  fail("Invalid tenant id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Tenant", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenant()
	  public virtual void deleteTenant()
	  {
		// create
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		TenantQuery query = identityService.createTenantQuery();
		assertThat(query.count(), @is(1L));

		identityService.deleteTenant("nonExisting");
		assertThat(query.count(), @is(1L));

		identityService.deleteTenant(TENANT_ONE);
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTenantOptimisticLockingException()
	  public virtual void updateTenantOptimisticLockingException()
	  {
		// create
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		Tenant tenant1 = identityService.createTenantQuery().singleResult();
		Tenant tenant2 = identityService.createTenantQuery().singleResult();

		// update
		tenant1.Name = "name";
		identityService.saveTenant(tenant1);

		thrown.expect(typeof(ProcessEngineException));

		// fail to update old revision
		tenant2.Name = "other name";
		identityService.saveTenant(tenant2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantWithGenericResourceId()
	  public virtual void createTenantWithGenericResourceId()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/generic.resource.id.whitelist.camunda.cfg.xml").buildProcessEngine();

		Tenant tenant = processEngine.IdentityService.newTenant("*");

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("has an invalid id: id cannot be *. * is a reserved identifier.");

		processEngine.IdentityService.saveTenant(tenant);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantMembershipUnexistingTenant()
	  public virtual void createTenantMembershipUnexistingTenant()
	  {
		User user = identityService.newUser(USER_ONE);
		identityService.saveUser(user);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("No tenant found with id 'nonExisting'.");

		identityService.createTenantUserMembership("nonExisting", user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantMembershipUnexistingUser()
	  public virtual void createTenantMembershipUnexistingUser()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("No user found with id 'nonExisting'.");

		identityService.createTenantUserMembership(tenant.Id, "nonExisting");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantMembershipUnexistingGroup()
	  public virtual void createTenantMembershipUnexistingGroup()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("No group found with id 'nonExisting'.");

		identityService.createTenantGroupMembership(tenant.Id, "nonExisting");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantUserMembershipAlreadyExisting()
	  public virtual void createTenantUserMembershipAlreadyExisting()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		User user = identityService.newUser(USER_ONE);
		identityService.saveUser(user);

		identityService.createTenantUserMembership(TENANT_ONE, USER_ONE);

		thrown.expect(typeof(ProcessEngineException));

		identityService.createTenantUserMembership(TENANT_ONE, USER_ONE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantGroupMembershipAlreadyExisting()
	  public virtual void createTenantGroupMembershipAlreadyExisting()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		Group group = identityService.newGroup(GROUP_ONE);
		identityService.saveGroup(group);

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		thrown.expect(typeof(ProcessEngineException));

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ONE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantUserMembership()
	  public virtual void deleteTenantUserMembership()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		User user = identityService.newUser(USER_ONE);
		identityService.saveUser(user);

		identityService.createTenantUserMembership(TENANT_ONE, USER_ONE);

		TenantQuery query = identityService.createTenantQuery().userMember(USER_ONE);
		assertThat(query.count(), @is(1L));

		identityService.deleteTenantUserMembership("nonExisting", USER_ONE);
		assertThat(query.count(), @is(1L));

		identityService.deleteTenantUserMembership(TENANT_ONE, "nonExisting");
		assertThat(query.count(), @is(1L));

		identityService.deleteTenantUserMembership(TENANT_ONE, USER_ONE);
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantGroupMembership()
	  public virtual void deleteTenantGroupMembership()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		Group group = identityService.newGroup(GROUP_ONE);
		identityService.saveGroup(group);

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		TenantQuery query = identityService.createTenantQuery().groupMember(GROUP_ONE);
		assertThat(query.count(), @is(1L));

		identityService.deleteTenantGroupMembership("nonExisting", GROUP_ONE);
		assertThat(query.count(), @is(1L));

		identityService.deleteTenantGroupMembership(TENANT_ONE, "nonExisting");
		assertThat(query.count(), @is(1L));

		identityService.deleteTenantGroupMembership(TENANT_ONE, GROUP_ONE);
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantMembershipsWileDeleteUser()
	  public virtual void deleteTenantMembershipsWileDeleteUser()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		User user = identityService.newUser(USER_ONE);
		identityService.saveUser(user);

		identityService.createTenantUserMembership(TENANT_ONE, USER_ONE);

		TenantQuery query = identityService.createTenantQuery().userMember(USER_ONE);
		assertThat(query.count(), @is(1L));

		identityService.deleteUser(USER_ONE);
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantMembershipsWhileDeleteGroup()
	  public virtual void deleteTenantMembershipsWhileDeleteGroup()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		Group group = identityService.newGroup(GROUP_ONE);
		identityService.saveGroup(group);

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		TenantQuery query = identityService.createTenantQuery().groupMember(GROUP_ONE);
		assertThat(query.count(), @is(1L));

		identityService.deleteGroup(GROUP_ONE);
		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantMembershipsOfTenant()
	  public virtual void deleteTenantMembershipsOfTenant()
	  {
		Tenant tenant = identityService.newTenant(TENANT_ONE);
		identityService.saveTenant(tenant);

		User user = identityService.newUser(USER_ONE);
		identityService.saveUser(user);

		Group group = identityService.newGroup(GROUP_ONE);
		identityService.saveGroup(group);

		identityService.createTenantUserMembership(TENANT_ONE, USER_ONE);
		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		UserQuery userQuery = identityService.createUserQuery().memberOfTenant(TENANT_ONE);
		GroupQuery groupQuery = identityService.createGroupQuery().memberOfTenant(TENANT_ONE);
		assertThat(userQuery.count(), @is(1L));
		assertThat(groupQuery.count(), @is(1L));

		identityService.deleteTenant(TENANT_ONE);
		assertThat(userQuery.count(), @is(0L));
		assertThat(groupQuery.count(), @is(0L));
	  }

	}

}