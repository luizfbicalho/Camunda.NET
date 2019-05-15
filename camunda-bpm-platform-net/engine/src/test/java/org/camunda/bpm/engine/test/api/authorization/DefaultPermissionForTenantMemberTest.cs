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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class DefaultPermissionForTenantMemberTest
	{
		private bool InstanceFieldsInitialized = false;

		public DefaultPermissionForTenantMemberTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
	  protected internal const string USER_ID = "user";
	  protected internal const string GROUP_ID = "group";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal AuthorizationService authorizationService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		identityService = engineRule.IdentityService;
		authorizationService = engineRule.AuthorizationService;

		createTenant(TENANT_ONE);

		User user = identityService.newUser(USER_ID);
		identityService.saveUser(user);

		Group group = identityService.newGroup(GROUP_ID);
		identityService.saveGroup(group);

		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		identityService.clearAuthentication();

		identityService.deleteUser(USER_ID);
		identityService.deleteGroup(GROUP_ID);
		identityService.deleteTenant(TENANT_ONE);
		identityService.deleteTenant(TENANT_TWO);

		engineRule.ProcessEngineConfiguration.AuthorizationEnabled = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTenantUserMembership()
	  public virtual void testCreateTenantUserMembership()
	  {

		identityService.createTenantUserMembership(TENANT_ONE, USER_ID);

		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn(USER_ID).resourceType(Resources.TENANT).resourceId(TENANT_ONE).hasPermission(Permissions.READ).count());

		identityService.AuthenticatedUserId = USER_ID;

		assertEquals(TENANT_ONE,identityService.createTenantQuery().singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAndDeleteTenantUserMembership()
	  public virtual void testCreateAndDeleteTenantUserMembership()
	  {

		identityService.createTenantUserMembership(TENANT_ONE, USER_ID);

		identityService.deleteTenantUserMembership(TENANT_ONE, USER_ID);

		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn(USER_ID).resourceType(Resources.TENANT).hasPermission(Permissions.READ).count());

		identityService.AuthenticatedUserId = USER_ID;

		assertEquals(0,identityService.createTenantQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAndDeleteTenantUserMembershipForMultipleTenants()
	  public virtual void testCreateAndDeleteTenantUserMembershipForMultipleTenants()
	  {

		createTenant(TENANT_TWO);

		identityService.createTenantUserMembership(TENANT_ONE, USER_ID);
		identityService.createTenantUserMembership(TENANT_TWO, USER_ID);

		assertEquals(2, authorizationService.createAuthorizationQuery().userIdIn(USER_ID).resourceType(Resources.TENANT).hasPermission(Permissions.READ).count());

		identityService.deleteTenantUserMembership(TENANT_ONE, USER_ID);

		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn(USER_ID).resourceType(Resources.TENANT).hasPermission(Permissions.READ).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTenantGroupMembership()
	  public virtual void testCreateTenantGroupMembership()
	  {

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ID);

		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn(GROUP_ID).resourceType(Resources.TENANT).resourceId(TENANT_ONE).hasPermission(Permissions.READ).count());

		identityService.setAuthentication(USER_ID, Collections.singletonList(GROUP_ID));

		assertEquals(TENANT_ONE,identityService.createTenantQuery().singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAndDeleteTenantGroupMembership()
	  public virtual void testCreateAndDeleteTenantGroupMembership()
	  {

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ID);

		identityService.deleteTenantGroupMembership(TENANT_ONE, GROUP_ID);

		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn(GROUP_ID).resourceType(Resources.TENANT).hasPermission(Permissions.READ).count());

		identityService.setAuthentication(USER_ID, Collections.singletonList(GROUP_ID));

		assertEquals(0,identityService.createTenantQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAndDeleteTenantGroupMembershipForMultipleTenants()
	  public virtual void testCreateAndDeleteTenantGroupMembershipForMultipleTenants()
	  {

		createTenant(TENANT_TWO);

		identityService.createTenantGroupMembership(TENANT_ONE, GROUP_ID);
		identityService.createTenantGroupMembership(TENANT_TWO, GROUP_ID);

		assertEquals(2, authorizationService.createAuthorizationQuery().groupIdIn(GROUP_ID).resourceType(Resources.TENANT).hasPermission(Permissions.READ).count());

		identityService.deleteTenantGroupMembership(TENANT_ONE, GROUP_ID);

		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn(GROUP_ID).resourceType(Resources.TENANT).hasPermission(Permissions.READ).count());
	  }

	  protected internal virtual Tenant createTenant(string tenantId)
	  {
		Tenant newTenant = identityService.newTenant(tenantId);
		identityService.saveTenant(newTenant);
		return newTenant;
	  }
	}

}