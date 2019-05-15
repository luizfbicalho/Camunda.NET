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
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using User = org.camunda.bpm.engine.identity.User;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.identity.TestPermissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.identity.TestPermissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.identity.TestPermissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.identity.TestPermissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.identity.TestPermissions.UPDATE;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class AuthorizationServiceWithEnabledAuthorizationTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		base.setUp();
		processEngineConfiguration.AuthorizationEnabled = true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		cleanupAfterTest();
		base.tearDown();
	  }

	  public virtual void testAuthorizationCheckEmptyDb()
	  {
		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;

		IList<string> jonnysGroups = Arrays.asList("sales", "marketing");
		IList<string> someOneElsesGroups = Collections.singletonList("marketing");

		// if no authorizations are in Db, nothing is authorized
		assertFalse(authorizationService.isUserAuthorized("jonny", jonnysGroups, ALL, resource1));
		assertFalse(authorizationService.isUserAuthorized("someone", someOneElsesGroups, CREATE, resource2));
		assertFalse(authorizationService.isUserAuthorized("someone else", null, DELETE, resource1));
		assertFalse(authorizationService.isUserAuthorized("jonny", jonnysGroups, ALL, resource1, "someId"));
		assertFalse(authorizationService.isUserAuthorized("someone", someOneElsesGroups, CREATE, resource2, "someId"));
		assertFalse(authorizationService.isUserAuthorized("someone else", null, DELETE, resource1, "someOtherId"));
	  }

	  public virtual void testUserOverrideGlobalGrantAuthorizationCheck()
	  {
		Resource resource1 = TestResource.RESOURCE1;

		// create global authorization which grants all permissions to all users  (on resource1):
		Authorization globalGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		globalGrant.Resource = resource1;
		globalGrant.ResourceId = ANY;
		globalGrant.addPermission(ALL);
		authorizationService.saveAuthorization(globalGrant);

		// revoke READ for jonny
		Authorization localRevoke = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		localRevoke.UserId = "jonny";
		localRevoke.Resource = resource1;
		localRevoke.ResourceId = ANY;
		localRevoke.removePermission(READ);
		authorizationService.saveAuthorization(localRevoke);

		IList<string> jonnysGroups = Arrays.asList("sales", "marketing");
		IList<string> someOneElsesGroups = Collections.singletonList("marketing");

		// jonny does not have ALL permissions
		assertFalse(authorizationService.isUserAuthorized("jonny", null, ALL, resource1));
		assertFalse(authorizationService.isUserAuthorized("jonny", jonnysGroups, ALL, resource1));
		// jonny can't read
		assertFalse(authorizationService.isUserAuthorized("jonny", null, READ, resource1));
		assertFalse(authorizationService.isUserAuthorized("jonny", jonnysGroups, READ, resource1));
		// someone else can
		assertTrue(authorizationService.isUserAuthorized("someone else", null, ALL, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", someOneElsesGroups, READ, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", null, ALL, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", someOneElsesGroups, READ, resource1));
		// jonny can still delete
		assertTrue(authorizationService.isUserAuthorized("jonny", null, DELETE, resource1));
		assertTrue(authorizationService.isUserAuthorized("jonny", jonnysGroups, DELETE, resource1));
	  }

	  public virtual void testGroupOverrideGlobalGrantAuthorizationCheck()
	  {
		Resource resource1 = TestResource.RESOURCE1;

		// create global authorization which grants all permissions to all users  (on resource1):
		Authorization globalGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		globalGrant.Resource = resource1;
		globalGrant.ResourceId = ANY;
		globalGrant.addPermission(ALL);
		authorizationService.saveAuthorization(globalGrant);

		// revoke READ for group "sales"
		Authorization groupRevoke = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		groupRevoke.GroupId = "sales";
		groupRevoke.Resource = resource1;
		groupRevoke.ResourceId = ANY;
		groupRevoke.removePermission(READ);
		authorizationService.saveAuthorization(groupRevoke);

		IList<string> jonnysGroups = Arrays.asList("sales", "marketing");
		IList<string> someOneElsesGroups = Collections.singletonList("marketing");

		// jonny does not have ALL permissions if queried with groups
		assertFalse(authorizationService.isUserAuthorized("jonny", jonnysGroups, ALL, resource1));
		// if queried without groups he has
		assertTrue(authorizationService.isUserAuthorized("jonny", null, ALL, resource1));

		// jonny can't read if queried with groups
		assertFalse(authorizationService.isUserAuthorized("jonny", jonnysGroups, READ, resource1));
		// if queried without groups he has
		assertTrue(authorizationService.isUserAuthorized("jonny", null, READ, resource1));

		// someone else who is in group "marketing" but but not "sales" can
		assertTrue(authorizationService.isUserAuthorized("someone else", someOneElsesGroups, ALL, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", someOneElsesGroups, READ, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", null, ALL, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", null, READ, resource1));
		// he could'nt if he were in jonny's groups
		assertFalse(authorizationService.isUserAuthorized("someone else", jonnysGroups, ALL, resource1));
		assertFalse(authorizationService.isUserAuthorized("someone else", jonnysGroups, READ, resource1));

		// jonny can still delete
		assertTrue(authorizationService.isUserAuthorized("jonny", jonnysGroups, DELETE, resource1));
		assertTrue(authorizationService.isUserAuthorized("jonny", null, DELETE, resource1));
	  }

	  public virtual void testUserOverrideGlobalRevokeAuthorizationCheck()
	  {
		Resource resource1 = TestResource.RESOURCE1;

		// create global authorization which revokes all permissions to all users  (on resource1):
		Authorization globalGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		globalGrant.Resource = resource1;
		globalGrant.ResourceId = ANY;
		globalGrant.removePermission(ALL);
		authorizationService.saveAuthorization(globalGrant);

		// add READ for jonny
		Authorization localRevoke = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		localRevoke.UserId = "jonny";
		localRevoke.Resource = resource1;
		localRevoke.ResourceId = ANY;
		localRevoke.addPermission(READ);
		authorizationService.saveAuthorization(localRevoke);

		// jonny does not have ALL permissions
		assertFalse(authorizationService.isUserAuthorized("jonny", null, ALL, resource1));
		// jonny can read
		assertTrue(authorizationService.isUserAuthorized("jonny", null, READ, resource1));
		// jonny can't delete
		assertFalse(authorizationService.isUserAuthorized("jonny", null, DELETE, resource1));

		// someone else can't do anything
		assertFalse(authorizationService.isUserAuthorized("someone else", null, ALL, resource1));
		assertFalse(authorizationService.isUserAuthorized("someone else", null, READ, resource1));
		assertFalse(authorizationService.isUserAuthorized("someone else", null, DELETE, resource1));
	  }

	  public virtual void testNullAuthorizationCheckUserGroup()
	  {
		try
		{
		  authorizationService.isUserAuthorized(null, null, UPDATE, TestResource.RESOURCE1);
		  fail("Expected NullValueException");
		}
		catch (NullValueException e)
		{
		  assertTrue(e.Message.contains("Authorization must have a 'userId' or/and a 'groupId'"));
		}
	  }

	  public virtual void testNullAuthorizationCheckPermission()
	  {
		try
		{
		  authorizationService.isUserAuthorized("jonny", null, null, TestResource.RESOURCE1);
		  fail("Expected NullValueException");
		}
		catch (NullValueException e)
		{
		  assertTrue(e.Message.contains("Invalid permission for an authorization"));
		}
	  }

	  public virtual void testNullAuthorizationCheckResource()
	  {
		try
		{
		  authorizationService.isUserAuthorized("jonny", null, UPDATE, null);
		  fail("Expected NullValueException");
		}
		catch (NullValueException e)
		{
		  assertTrue(e.Message.contains("Invalid resource for an authorization"));
		}
	  }

	  public virtual void testUserOverrideGroupOverrideGlobalAuthorizationCheck()
	  {
		Resource resource1 = TestResource.RESOURCE1;

		// create global authorization which grants all permissions to all users  (on resource1):
		Authorization globalGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		globalGrant.Resource = resource1;
		globalGrant.ResourceId = ANY;
		globalGrant.addPermission(ALL);
		authorizationService.saveAuthorization(globalGrant);

		// revoke READ for group "sales"
		Authorization groupRevoke = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		groupRevoke.GroupId = "sales";
		groupRevoke.Resource = resource1;
		groupRevoke.ResourceId = ANY;
		groupRevoke.removePermission(READ);
		authorizationService.saveAuthorization(groupRevoke);

		// add READ for jonny
		Authorization userGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		userGrant.UserId = "jonny";
		userGrant.Resource = resource1;
		userGrant.ResourceId = ANY;
		userGrant.addPermission(READ);
		authorizationService.saveAuthorization(userGrant);

		IList<string> jonnysGroups = Arrays.asList("sales", "marketing");
		IList<string> someOneElsesGroups = Collections.singletonList("marketing");

		// jonny can read
		assertTrue(authorizationService.isUserAuthorized("jonny", jonnysGroups, READ, resource1));
		assertTrue(authorizationService.isUserAuthorized("jonny", null, READ, resource1));

		// someone else in the same groups cannot
		assertFalse(authorizationService.isUserAuthorized("someone else", jonnysGroups, READ, resource1));

		// someone else in different groups can
		assertTrue(authorizationService.isUserAuthorized("someone else", someOneElsesGroups, READ, resource1));
	  }

	  public virtual void testEnabledAuthorizationCheck()
	  {
		// given
		Resource resource1 = TestResource.RESOURCE1;

		// when
		bool isAuthorized = authorizationService.isUserAuthorized("jonny", null, UPDATE, resource1);

		// then
		assertFalse(isAuthorized);
	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }
	}

}