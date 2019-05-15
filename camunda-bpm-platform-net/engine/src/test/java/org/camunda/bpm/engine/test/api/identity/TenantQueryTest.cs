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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	public class TenantQueryTest
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string USER = "user";
	  protected internal const string GROUP = "group";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		identityService = engineRule.IdentityService;

		createTenant(TENANT_ONE, "Tenant_1");
		createTenant(TENANT_TWO, "Tenant_2");

		User user = identityService.newUser(USER);
		identityService.saveUser(user);

		Group group = identityService.newGroup(GROUP);
		identityService.saveGroup(group);

		identityService.createMembership(USER, GROUP);

		identityService.createTenantUserMembership(TENANT_ONE, USER);
		identityService.createTenantGroupMembership(TENANT_TWO, GROUP);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryById()
	  public virtual void queryById()
	  {
		TenantQuery query = identityService.createTenantQuery().tenantId(TENANT_ONE);

		assertThat(query.count(), @is(1L));
		assertThat(query.list().size(), @is(1));

		Tenant tenant = query.singleResult();
		assertThat(tenant, @is(notNullValue()));
		assertThat(tenant.Name, @is("Tenant_1"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByNonExistingId()
	  public virtual void queryByNonExistingId()
	  {
		TenantQuery query = identityService.createTenantQuery().tenantId("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByIdIn()
	  public virtual void queryByIdIn()
	  {
		TenantQuery query = identityService.createTenantQuery();

		assertThat(query.tenantIdIn("non", "existing").count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByName()
	  public virtual void queryByName()
	  {
		TenantQuery query = identityService.createTenantQuery();

		assertThat(query.tenantName("nonExisting").count(), @is(0L));
		assertThat(query.tenantName("Tenant_1").count(), @is(1L));
		assertThat(query.tenantName("Tenant_2").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByNameLike()
	  public virtual void queryByNameLike()
	  {
		TenantQuery query = identityService.createTenantQuery();

		assertThat(query.tenantNameLike("%nonExisting%").count(), @is(0L));
		assertThat(query.tenantNameLike("%Tenant\\_1%").count(), @is(1L));
		assertThat(query.tenantNameLike("%Tenant%").count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByUser()
	  public virtual void queryByUser()
	  {
		TenantQuery query = identityService.createTenantQuery();

		assertThat(query.userMember("nonExisting").count(), @is(0L));
		assertThat(query.userMember(USER).count(), @is(1L));
		assertThat(query.userMember(USER).tenantId(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByGroup()
	  public virtual void queryByGroup()
	  {
		TenantQuery query = identityService.createTenantQuery();

		assertThat(query.groupMember("nonExisting").count(), @is(0L));
		assertThat(query.groupMember(GROUP).count(), @is(1L));
		assertThat(query.groupMember(GROUP).tenantId(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByUserIncludingGroups()
	  public virtual void queryByUserIncludingGroups()
	  {
		TenantQuery query = identityService.createTenantQuery().userMember(USER);

		assertThat(query.includingGroupsOfUser(false).count(), @is(1L));
		assertThat(query.includingGroupsOfUser(true).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryOrderById()
	  public virtual void queryOrderById()
	  {
		// ascending
		IList<Tenant> tenants = identityService.createTenantQuery().orderByTenantId().asc().list();
		assertThat(tenants.Count, @is(2));

		assertThat(tenants[0].Id, @is(TENANT_ONE));
		assertThat(tenants[1].Id, @is(TENANT_TWO));

		// descending
		tenants = identityService.createTenantQuery().orderByTenantId().desc().list();

		assertThat(tenants[0].Id, @is(TENANT_TWO));
		assertThat(tenants[1].Id, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryOrderByName()
	  public virtual void queryOrderByName()
	  {
		// ascending
		IList<Tenant> tenants = identityService.createTenantQuery().orderByTenantName().asc().list();
		assertThat(tenants.Count, @is(2));

		assertThat(tenants[0].Name, @is("Tenant_1"));
		assertThat(tenants[1].Name, @is("Tenant_2"));

		// descending
		tenants = identityService.createTenantQuery().orderByTenantName().desc().list();

		assertThat(tenants[0].Name, @is("Tenant_2"));
		assertThat(tenants[1].Name, @is("Tenant_1"));
	  }

	  protected internal virtual Tenant createTenant(string id, string name)
	  {
		Tenant tenant = engineRule.IdentityService.newTenant(id);
		tenant.Name = name;
		identityService.saveTenant(tenant);

		return tenant;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		identityService.deleteTenant(TENANT_ONE);
		identityService.deleteTenant(TENANT_TWO);

		identityService.deleteUser(USER);
		identityService.deleteGroup(GROUP);
	  }

	}

}