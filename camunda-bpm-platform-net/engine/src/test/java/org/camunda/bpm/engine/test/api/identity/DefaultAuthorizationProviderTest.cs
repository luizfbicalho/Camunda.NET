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
	using static org.camunda.bpm.engine.authorization.Authorization;
	using static org.camunda.bpm.engine.authorization.Resources;
	using static org.camunda.bpm.engine.authorization.Permissions;

	using DefaultAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultAuthorizationProvider;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// <para>Test authorizations provided by <seealso cref="DefaultAuthorizationProvider"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultAuthorizationProviderTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		// we are jonny
		identityService.AuthenticatedUserId = "jonny";
		// make sure we can do stuff:
		Authorization jonnyIsGod = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		jonnyIsGod.UserId = "jonny";
		jonnyIsGod.Resource = USER;
		jonnyIsGod.ResourceId = ANY;
		jonnyIsGod.addPermission(ALL);
		authorizationService.saveAuthorization(jonnyIsGod);

		jonnyIsGod = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		jonnyIsGod.UserId = "jonny";
		jonnyIsGod.Resource = GROUP;
		jonnyIsGod.ResourceId = ANY;
		jonnyIsGod.addPermission(ALL);
		authorizationService.saveAuthorization(jonnyIsGod);

		jonnyIsGod = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		jonnyIsGod.UserId = "jonny";
		jonnyIsGod.Resource = AUTHORIZATION;
		jonnyIsGod.ResourceId = ANY;
		jonnyIsGod.addPermission(ALL);
		authorizationService.saveAuthorization(jonnyIsGod);

		// enable authorizations
		processEngineConfiguration.AuthorizationEnabled = true;
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		IList<Authorization> jonnysAuths = authorizationService.createAuthorizationQuery().userIdIn("jonny").list();
		foreach (Authorization authorization in jonnysAuths)
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
		base.tearDown();
	  }

	  public virtual void testCreateUser()
	  {
		// initially there are no authorizations for jonny2:
		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn("jonny2").count());

		// create new user
		identityService.saveUser(identityService.newUser("jonny2"));

		// now there is an authorization for jonny2 which grants him ALL permissions on himself
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("jonny2").singleResult();
		assertNotNull(authorization);
		assertEquals(AUTH_TYPE_GRANT, authorization.AuthorizationType);
		assertEquals(USER.resourceType(), authorization.ResourceType);
		assertEquals("jonny2", authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(ALL));

		// delete the user
		identityService.deleteUser("jonny2");

		// the authorization is deleted as well:
		assertEquals(0, authorizationService.createAuthorizationQuery().userIdIn("jonny2").count());
	  }

	  public virtual void testCreateGroup()
	  {
		// initially there are no authorizations for group "sales":
		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn("sales").count());

		// create new group
		identityService.saveGroup(identityService.newGroup("sales"));

		// now there is an authorization for sales which grants all members READ permissions
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("sales").singleResult();
		assertNotNull(authorization);
		assertEquals(AUTH_TYPE_GRANT, authorization.AuthorizationType);
		assertEquals(GROUP.resourceType(), authorization.ResourceType);
		assertEquals("sales", authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));

		// delete the group
		identityService.deleteGroup("sales");

		// the authorization is deleted as well:
		assertEquals(0, authorizationService.createAuthorizationQuery().groupIdIn("sales").count());
	  }

	}

}