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

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;

		createAuthorization("user1", null, resource1, "resource1-1", TestPermissions.ACCESS);
		createAuthorization("user1", null, resource2, "resource2-1", TestPermissions.DELETE);
		createAuthorization("user2", null, resource1, "resource1-2");
		createAuthorization("user3", null, resource2, "resource2-1", TestPermissions.READ, TestPermissions.UPDATE);

		createAuthorization(null, "group1", resource1, "resource1-1");
		createAuthorization(null, "group1", resource1, "resource1-2", TestPermissions.UPDATE);
		createAuthorization(null, "group2", resource2, "resource2-2", TestPermissions.READ, TestPermissions.UPDATE);
		createAuthorization(null, "group3", resource2, "resource2-3", TestPermissions.DELETE);

	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		IList<Authorization> list = authorizationService.createAuthorizationQuery().list();
		foreach (Authorization authorization in list)
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
		base.tearDown();
	  }

	  protected internal virtual void createAuthorization(string userId, string groupId, Resource resourceType, string resourceId, params Permission[] permissions)
	  {

		Authorization authorization = authorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.UserId = userId;
		authorization.GroupId = groupId;
		authorization.Resource = resourceType;
		authorization.ResourceId = resourceId;

		foreach (Permission permission in permissions)
		{
		  authorization.addPermission(permission);
		}

		authorizationService.saveAuthorization(authorization);
	  }

	  public virtual void testValidQueryCounts()
	  {

		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;
		Resource nonExisting = new NonExistingResource(this, "non-existing", 102);

		// query by user id
		assertEquals(2, authorizationService.createAuthorizationQuery().userIdIn("user1").count());
		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn("user2").count());
		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn("user3").count());
		assertEquals(3, authorizationService.createAuthorizationQuery().userIdIn("user1", "user2").count());
		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn("non-existing").count());

		// query by group id
		assertEquals(2, authorizationService.createAuthorizationQuery().groupIdIn("group1").count());
		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn("group2").count());
		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn("group3").count());
		assertEquals(3, authorizationService.createAuthorizationQuery().groupIdIn("group1", "group2").count());
		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn("non-existing").count());

		// query by resource type
		assertEquals(4, authorizationService.createAuthorizationQuery().resourceType(resource1).count());
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceType(nonExisting).count());
		assertEquals(4, authorizationService.createAuthorizationQuery().resourceType(resource1.resourceType()).count());
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceType(nonExisting.resourceType()).count());

		// query by resource id
		assertEquals(2, authorizationService.createAuthorizationQuery().resourceId("resource1-2").count());
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceId("non-existing").count());

		// query by permission
		assertEquals(1, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.ACCESS).count());
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.DELETE).count());
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.READ).count());
		assertEquals(3, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.UPDATE).count());
		// multiple permissions at the same time
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.READ).hasPermission(TestPermissions.UPDATE).count());
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.UPDATE).hasPermission(TestPermissions.READ).count());
		assertEquals(0, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.READ).hasPermission(TestPermissions.ACCESS).count());

		// user id & resource type
		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn("user1").resourceType(resource1).count());
		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn("user1").resourceType(nonExisting).count());

		// group id & resource type
		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn("group2").resourceType(resource2).count());
		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn("group1").resourceType(nonExisting).count());
	  }

	  public virtual void testValidQueryLists()
	  {

		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;
		Resource nonExisting = new NonExistingResource(this, "non-existing", 102);

		// query by user id
		assertEquals(2, authorizationService.createAuthorizationQuery().userIdIn("user1").list().size());
		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn("user2").list().size());
		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn("user3").list().size());
		assertEquals(3, authorizationService.createAuthorizationQuery().userIdIn("user1", "user2").list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn("non-existing").list().size());

		// query by group id
		assertEquals(2, authorizationService.createAuthorizationQuery().groupIdIn("group1").list().size());
		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn("group2").list().size());
		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn("group3").list().size());
		assertEquals(3, authorizationService.createAuthorizationQuery().groupIdIn("group1", "group2").list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn("non-existing").list().size());

		// query by resource type
		assertEquals(4, authorizationService.createAuthorizationQuery().resourceType(resource1).list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceType(nonExisting).list().size());

		// query by resource id
		assertEquals(2, authorizationService.createAuthorizationQuery().resourceId("resource1-2").list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceId("non-existing").list().size());

		// query by permission
		assertEquals(1, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.ACCESS).list().size());
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.DELETE).list().size());
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.READ).list().size());
		assertEquals(3, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.UPDATE).list().size());
		// multiple permissions at the same time
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.READ).hasPermission(TestPermissions.UPDATE).list().size());
		assertEquals(2, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.UPDATE).hasPermission(TestPermissions.READ).list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().hasPermission(TestPermissions.READ).hasPermission(TestPermissions.ACCESS).list().size());

		// user id & resource type
		assertEquals(1, authorizationService.createAuthorizationQuery().userIdIn("user1").resourceType(resource1).list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn("user1").resourceType(nonExisting).list().size());

		// group id & resource type
		assertEquals(1, authorizationService.createAuthorizationQuery().groupIdIn("group2").resourceType(resource2).list().size());
		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn("group1").resourceType(nonExisting).list().size());
	  }

	  public virtual void testOrderByQueries()
	  {

		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;

		IList<Authorization> list = authorizationService.createAuthorizationQuery().orderByResourceType().asc().list();
		assertEquals(resource1.resourceType(), list[0].ResourceType);
		assertEquals(resource1.resourceType(), list[1].ResourceType);
		assertEquals(resource1.resourceType(), list[2].ResourceType);
		assertEquals(resource1.resourceType(), list[3].ResourceType);
		assertEquals(resource2.resourceType(), list[4].ResourceType);
		assertEquals(resource2.resourceType(), list[5].ResourceType);
		assertEquals(resource2.resourceType(), list[6].ResourceType);
		assertEquals(resource2.resourceType(), list[7].ResourceType);

		list = authorizationService.createAuthorizationQuery().orderByResourceType().desc().list();
		assertEquals(resource2.resourceType(), list[0].ResourceType);
		assertEquals(resource2.resourceType(), list[1].ResourceType);
		assertEquals(resource2.resourceType(), list[2].ResourceType);
		assertEquals(resource2.resourceType(), list[3].ResourceType);
		assertEquals(resource1.resourceType(), list[4].ResourceType);
		assertEquals(resource1.resourceType(), list[5].ResourceType);
		assertEquals(resource1.resourceType(), list[6].ResourceType);
		assertEquals(resource1.resourceType(), list[7].ResourceType);

		list = authorizationService.createAuthorizationQuery().orderByResourceId().asc().list();
		assertEquals("resource1-1", list[0].ResourceId);
		assertEquals("resource1-1", list[1].ResourceId);
		assertEquals("resource1-2", list[2].ResourceId);
		assertEquals("resource1-2", list[3].ResourceId);
		assertEquals("resource2-1", list[4].ResourceId);
		assertEquals("resource2-1", list[5].ResourceId);
		assertEquals("resource2-2", list[6].ResourceId);
		assertEquals("resource2-3", list[7].ResourceId);

		list = authorizationService.createAuthorizationQuery().orderByResourceId().desc().list();
		assertEquals("resource2-3", list[0].ResourceId);
		assertEquals("resource2-2", list[1].ResourceId);
		assertEquals("resource2-1", list[2].ResourceId);
		assertEquals("resource2-1", list[3].ResourceId);
		assertEquals("resource1-2", list[4].ResourceId);
		assertEquals("resource1-2", list[5].ResourceId);
		assertEquals("resource1-1", list[6].ResourceId);
		assertEquals("resource1-1", list[7].ResourceId);

	  }

	  public virtual void testInvalidOrderByQueries()
	  {
		try
		{
		  authorizationService.createAuthorizationQuery().orderByResourceType().list();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Invalid query: call asc() or desc() after using orderByXX()", e.Message);
		}

		try
		{
		  authorizationService.createAuthorizationQuery().orderByResourceId().list();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Invalid query: call asc() or desc() after using orderByXX()", e.Message);
		}

		try
		{
		  authorizationService.createAuthorizationQuery().orderByResourceId().orderByResourceType().list();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Invalid query: call asc() or desc() after using orderByXX()", e.Message);
		}

		try
		{
		  authorizationService.createAuthorizationQuery().orderByResourceType().orderByResourceId().list();
		  fail("Exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Invalid query: call asc() or desc() after using orderByXX()", e.Message);
		}
	  }

	  public virtual void testInvalidQueries()
	  {

		// cannot query for user id and group id at the same time

		try
		{
		  authorizationService.createAuthorizationQuery().groupIdIn("a").userIdIn("b").count();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot query for user and group authorizations at the same time.", e.Message);
		}

		try
		{
		  authorizationService.createAuthorizationQuery().userIdIn("b").groupIdIn("a").count();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot query for user and group authorizations at the same time.", e.Message);
		}

	  }

	  internal class NonExistingResource : Resource
	  {
		  private readonly AuthorizationQueryTest outerInstance;


		protected internal int id;
		protected internal string name;

		public NonExistingResource(AuthorizationQueryTest outerInstance, string name, int id)
		{
			this.outerInstance = outerInstance;
		  this.name = name;
		  this.id = id;
		}

		public virtual string resourceName()
		{
		  return name;
		}

		public virtual int resourceType()
		{
		  return id;
		}

	  }

	}

}