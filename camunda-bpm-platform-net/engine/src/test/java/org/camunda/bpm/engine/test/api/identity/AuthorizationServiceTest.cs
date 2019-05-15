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
//	import static org.camunda.bpm.engine.authorization.Permissions.NONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DASHBOARD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.REPORT;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using User = org.camunda.bpm.engine.identity.User;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationServiceTest : PluggableProcessEngineTestCase
	{

	  protected internal string userId = "test";
	  protected internal string groupId = "accounting";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		cleanupAfterTest();
		base.tearDown();
	  }

	  public virtual void testGlobalAuthorizationType()
	  {
		Authorization globalAuthorization = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		// I can set userId = null
		globalAuthorization.UserId = null;
		// I can set userId = ANY
		globalAuthorization.UserId = ANY;

		try
		{
		  // I cannot set anything else:
		  globalAuthorization.UserId = "something";
		  fail("exception expected");

		}
		catch (Exception e)
		{
		  assertTextPresent("ENGINE-03028 Illegal value 'something' for userId for GLOBAL authorization. Must be '*'", e.Message);

		}

		// I can set groupId = null
		globalAuthorization.GroupId = null;

		try
		{
		  // I cannot set anything else:
		  globalAuthorization.GroupId = "something";
		  fail("exception expected");

		}
		catch (Exception e)
		{
		  assertTextPresent("ENGINE-03027 Cannot use 'groupId' for GLOBAL authorization", e.Message);
		}
	  }

	  public virtual void testGrantAuthorizationType()
	  {
		Authorization grantAuthorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		// I can set userId = null
		grantAuthorization.UserId = null;
		// I can set userId = ANY
		grantAuthorization.UserId = ANY;
		// I can set anything else:
		grantAuthorization.UserId = "something";
		// I can set groupId = null
		grantAuthorization.GroupId = null;
		// I can set anything else:
		grantAuthorization.GroupId = "something";
	  }

	  public virtual void testRevokeAuthorizationType()
	  {
		Authorization revokeAuthorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		// I can set userId = null
		revokeAuthorization.UserId = null;
		// I can set userId = ANY
		revokeAuthorization.UserId = ANY;
		// I can set anything else:
		revokeAuthorization.UserId = "something";
		// I can set groupId = null
		revokeAuthorization.GroupId = null;
		// I can set anything else:
		revokeAuthorization.GroupId = "something";
	  }

	  public virtual void testDeleteNonExistingAuthorization()
	  {

		try
		{
		  authorizationService.deleteAuthorization("nonExisiting");
		  fail();
		}
		catch (Exception e)
		{
		  assertTextPresent("Authorization for Id 'nonExisiting' does not exist: authorization is null", e.Message);
		}

	  }

	  public virtual void testCreateAuthorizationWithUserId()
	  {

		Resource resource1 = TestResource.RESOURCE1;

		// initially, no authorization exists:
		assertEquals(0, authorizationService.createAuthorizationQuery().count());

		// simple create / delete with userId
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "aUserId";
		authorization.Resource = resource1;

		// save the authorization
		authorizationService.saveAuthorization(authorization);
		// authorization exists
		assertEquals(1, authorizationService.createAuthorizationQuery().count());
		// delete the authorization
		authorizationService.deleteAuthorization(authorization.Id);
		// it's gone
		assertEquals(0, authorizationService.createAuthorizationQuery().count());

	  }

	  public virtual void testCreateAuthorizationWithGroupId()
	  {

		Resource resource1 = TestResource.RESOURCE1;

		// initially, no authorization exists:
		assertEquals(0, authorizationService.createAuthorizationQuery().count());

		// simple create / delete with userId
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.GroupId = "aGroupId";
		authorization.Resource = resource1;

		// save the authorization
		authorizationService.saveAuthorization(authorization);
		// authorization exists
		assertEquals(1, authorizationService.createAuthorizationQuery().count());
		// delete the authorization
		authorizationService.deleteAuthorization(authorization.Id);
		// it's gone
		assertEquals(0, authorizationService.createAuthorizationQuery().count());

	  }

	  public virtual void testInvalidCreateAuthorization()
	  {

		Resource resource1 = TestResource.RESOURCE1;

		// case 1: no user id & no group id ////////////

		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.Resource = resource1;

		try
		{
		  authorizationService.saveAuthorization(authorization);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Authorization must either have a 'userId' or a 'groupId'."));
		}

		// case 2: both user id & group id ////////////

		authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.GroupId = "someId";
		authorization.UserId = "someOtherId";
		authorization.Resource = resource1;

		try
		{
		  authorizationService.saveAuthorization(authorization);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Authorization must either have a 'userId' or a 'groupId'.", e.Message);
		}

		// case 3: no resourceType ////////////

		authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "someId";

		try
		{
		  authorizationService.saveAuthorization(authorization);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Authorization 'resourceType' cannot be null."));
		}

		// case 4: no permissions /////////////////

		authorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		authorization.UserId = "someId";

		try
		{
		  authorizationService.saveAuthorization(authorization);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("Authorization 'resourceType' cannot be null."));
		}
	  }

	  public virtual void testUniqueUserConstraints()
	  {

		Resource resource1 = TestResource.RESOURCE1;

		Authorization authorization1 = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		Authorization authorization2 = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);

		authorization1.Resource = resource1;
		authorization1.ResourceId = "someId";
		authorization1.UserId = "someUser";

		authorization2.Resource = resource1;
		authorization2.ResourceId = "someId";
		authorization2.UserId = "someUser";

		// the first one can be saved
		authorizationService.saveAuthorization(authorization1);

		// the second one cannot
		try
		{
		  authorizationService.saveAuthorization(authorization2);
		  fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  //expected
		}

		// but I can add a AUTH_TYPE_REVOKE auth

		Authorization authorization3 = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);

		authorization3.Resource = resource1;
		authorization3.ResourceId = "someId";
		authorization3.UserId = "someUser";

		authorizationService.saveAuthorization(authorization3);

		// but not a second

		Authorization authorization4 = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);

		authorization4.Resource = resource1;
		authorization4.ResourceId = "someId";
		authorization4.UserId = "someUser";

		try
		{
		  authorizationService.saveAuthorization(authorization4);
		  fail("exception expected");
		}
		catch (Exception)
		{
		  //expected
		}
	  }

	  public virtual void testUniqueGroupConstraints()
	  {

		Resource resource1 = TestResource.RESOURCE1;

		Authorization authorization1 = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		Authorization authorization2 = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);

		authorization1.Resource = resource1;
		authorization1.ResourceId = "someId";
		authorization1.GroupId = "someGroup";

		authorization2.Resource = resource1;
		authorization2.ResourceId = "someId";
		authorization2.GroupId = "someGroup";

		// the first one can be saved
		authorizationService.saveAuthorization(authorization1);

		// the second one cannot
		try
		{
		  authorizationService.saveAuthorization(authorization2);
		  fail("exception expected");
		}
		catch (Exception)
		{
		  //expected
		}

		// but I can add a AUTH_TYPE_REVOKE auth

		Authorization authorization3 = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);

		authorization3.Resource = resource1;
		authorization3.ResourceId = "someId";
		authorization3.GroupId = "someGroup";

		authorizationService.saveAuthorization(authorization3);

		// but not a second

		Authorization authorization4 = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);

		authorization4.Resource = resource1;
		authorization4.ResourceId = "someId";
		authorization4.GroupId = "someGroup";

		try
		{
		  authorizationService.saveAuthorization(authorization4);
		  fail("exception expected");
		}
		catch (Exception)
		{
		  //expected
		}

	  }

	  public virtual void testGlobalUniqueConstraints()
	  {

		Resource resource1 = TestResource.RESOURCE1;

		Authorization authorization1 = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		Authorization authorization2 = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);

		authorization1.Resource = resource1;
		authorization1.ResourceId = "someId";

		authorization2.Resource = resource1;
		authorization2.ResourceId = "someId";

		// the first one can be saved
		authorizationService.saveAuthorization(authorization1);

		// the second one cannot
		try
		{
		  authorizationService.saveAuthorization(authorization2);
		  fail("exception expected");
		}
		catch (Exception)
		{
		  //expected
		}
	  }

	  public virtual void testUpdateNewAuthorization()
	  {

		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;

		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "aUserId";
		authorization.Resource = resource1;
		authorization.ResourceId = "aResourceId";
		authorization.addPermission(TestPermissions.ACCESS);

		// save the authorization
		authorizationService.saveAuthorization(authorization);

		// validate authorization
		Authorization savedAuthorization = authorizationService.createAuthorizationQuery().singleResult();
		assertEquals("aUserId", savedAuthorization.UserId);
		assertEquals(resource1.resourceType(), savedAuthorization.ResourceType);
		assertEquals("aResourceId", savedAuthorization.ResourceId);
		assertTrue(savedAuthorization.isPermissionGranted(TestPermissions.ACCESS));

		// update authorization
		authorization.UserId = "anotherUserId";
		authorization.Resource = resource2;
		authorization.ResourceId = "anotherResourceId";
		authorization.addPermission(TestPermissions.DELETE);
		authorizationService.saveAuthorization(authorization);

		// validate authorization updated
		savedAuthorization = authorizationService.createAuthorizationQuery().singleResult();
		assertEquals("anotherUserId", savedAuthorization.UserId);
		assertEquals(resource2.resourceType(), savedAuthorization.ResourceType);
		assertEquals("anotherResourceId", savedAuthorization.ResourceId);
		assertTrue(savedAuthorization.isPermissionGranted(TestPermissions.ACCESS));
		assertTrue(savedAuthorization.isPermissionGranted(TestPermissions.DELETE));

	  }

	  public virtual void testUpdatePersistentAuthorization()
	  {

		Resource resource1 = TestResource.RESOURCE1;
		Resource resource2 = TestResource.RESOURCE2;
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "aUserId";
		authorization.Resource = resource1;
		authorization.ResourceId = "aResourceId";
		authorization.addPermission(TestPermissions.ACCESS);

		// save the authorization
		authorizationService.saveAuthorization(authorization);

		// validate authorization
		Authorization savedAuthorization = authorizationService.createAuthorizationQuery().singleResult();
		assertEquals("aUserId", savedAuthorization.UserId);
		assertEquals(resource1.resourceType(), savedAuthorization.ResourceType);
		assertEquals("aResourceId", savedAuthorization.ResourceId);
		assertTrue(savedAuthorization.isPermissionGranted(TestPermissions.ACCESS));

		// update authorization
		savedAuthorization.UserId = "anotherUserId";
		savedAuthorization.Resource = resource2;
		savedAuthorization.ResourceId = "anotherResourceId";
		savedAuthorization.addPermission(TestPermissions.DELETE);
		authorizationService.saveAuthorization(savedAuthorization);

		// validate authorization updated
		savedAuthorization = authorizationService.createAuthorizationQuery().singleResult();
		assertEquals("anotherUserId", savedAuthorization.UserId);
		assertEquals(resource2.resourceType(), savedAuthorization.ResourceType);
		assertEquals("anotherResourceId", savedAuthorization.ResourceId);
		assertTrue(savedAuthorization.isPermissionGranted(TestPermissions.ACCESS));
		assertTrue(savedAuthorization.isPermissionGranted(TestPermissions.DELETE));

	  }

	  public virtual void testPermissions()
	  {

		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.Resource = Resources.USER;

		assertEquals(1, authorization.getPermissions(Permissions.values()).Length);

		assertFalse(authorization.isPermissionGranted(CREATE));
		assertFalse(authorization.isPermissionGranted(DELETE));
		assertFalse(authorization.isPermissionGranted(READ));
		assertFalse(authorization.isPermissionGranted(UPDATE));

		authorization.addPermission(CREATE);
		assertTrue(authorization.isPermissionGranted(CREATE));
		assertFalse(authorization.isPermissionGranted(DELETE));
		assertFalse(authorization.isPermissionGranted(READ));
		assertFalse(authorization.isPermissionGranted(UPDATE));

		authorization.addPermission(DELETE);
		assertTrue(authorization.isPermissionGranted(CREATE));
		assertTrue(authorization.isPermissionGranted(DELETE));
		assertFalse(authorization.isPermissionGranted(READ));
		assertFalse(authorization.isPermissionGranted(UPDATE));

		authorization.addPermission(READ);
		assertTrue(authorization.isPermissionGranted(CREATE));
		assertTrue(authorization.isPermissionGranted(DELETE));
		assertTrue(authorization.isPermissionGranted(READ));
		assertFalse(authorization.isPermissionGranted(UPDATE));

		authorization.addPermission(UPDATE);
		assertTrue(authorization.isPermissionGranted(CREATE));
		assertTrue(authorization.isPermissionGranted(DELETE));
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(UPDATE));

		authorization.removePermission(CREATE);
		assertFalse(authorization.isPermissionGranted(CREATE));
		assertTrue(authorization.isPermissionGranted(DELETE));
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(UPDATE));

		authorization.removePermission(DELETE);
		assertFalse(authorization.isPermissionGranted(CREATE));
		assertFalse(authorization.isPermissionGranted(DELETE));
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(UPDATE));

		authorization.removePermission(READ);
		assertFalse(authorization.isPermissionGranted(CREATE));
		assertFalse(authorization.isPermissionGranted(DELETE));
		assertFalse(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(UPDATE));

		authorization.removePermission(UPDATE);
		assertFalse(authorization.isPermissionGranted(CREATE));
		assertFalse(authorization.isPermissionGranted(DELETE));
		assertFalse(authorization.isPermissionGranted(READ));
		assertFalse(authorization.isPermissionGranted(UPDATE));

	  }

	  public virtual void testGrantAuthPermissions()
	  {

		AuthorizationEntity authorization = new AuthorizationEntity(AUTH_TYPE_GRANT);
		authorization.setResource(Resources.DEPLOYMENT);

		assertFalse(authorization.isPermissionGranted(ALL));
		assertTrue(authorization.isPermissionGranted(NONE));
		IList<Permission> perms = Arrays.asList(authorization.getPermissions(Permissions.values()));
		assertTrue(perms.Contains(NONE));
		assertEquals(1, perms.Count);

		authorization.addPermission(READ);
		perms = Arrays.asList(authorization.getPermissions(Permissions.values()));
		assertTrue(perms.Contains(NONE));
		assertTrue(perms.Contains(READ));
		assertEquals(2, perms.Count);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(NONE)); // (none is always granted => you are always authorized to do nothing)

		try
		{
		  authorization.isPermissionRevoked(READ);
		  fail("Exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTextPresent("ENGINE-03026 Method 'isPermissionRevoked' cannot be used for authorization with type 'GRANT'.", e.Message);
		}

	  }

	  public virtual void testGlobalAuthPermissions()
	  {

		AuthorizationEntity authorization = new AuthorizationEntity(AUTH_TYPE_GRANT);
		authorization.setResource(Resources.DEPLOYMENT);

		assertFalse(authorization.isPermissionGranted(ALL));
		assertTrue(authorization.isPermissionGranted(NONE));
		IList<Permission> perms = Arrays.asList(authorization.getPermissions(Permissions.values()));
		assertTrue(perms.Contains(NONE));
		assertEquals(1, perms.Count);

		authorization.addPermission(READ);
		perms = Arrays.asList(authorization.getPermissions(Permissions.values()));
		assertTrue(perms.Contains(NONE));
		assertTrue(perms.Contains(READ));
		assertEquals(2, perms.Count);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(NONE)); // (none is always granted => you are always authorized to do nothing)

		try
		{
		  authorization.isPermissionRevoked(READ);
		  fail("Exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTextPresent("ENGINE-03026 Method 'isPermissionRevoked' cannot be used for authorization with type 'GRANT'.", e.Message);
		}

	  }

	  public virtual void testRevokeAuthPermissions()
	  {

		AuthorizationEntity authorization = new AuthorizationEntity(AUTH_TYPE_REVOKE);
		authorization.setResource(Resources.DEPLOYMENT);

		assertFalse(authorization.isPermissionRevoked(ALL));
		IList<Permission> perms = Arrays.asList(authorization.getPermissions(Permissions.values()));
		assertEquals(0, perms.Count);

		authorization.removePermission(READ);
		perms = Arrays.asList(authorization.getPermissions(Permissions.values()));
		assertTrue(perms.Contains(READ));
		assertTrue(perms.Contains(ALL));
		assertEquals(2, perms.Count);

		try
		{
		  authorization.isPermissionGranted(READ);
		  fail("Exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTextPresent("ENGINE-03026 Method 'isPermissionGranted' cannot be used for authorization with type 'REVOKE'.", e.Message);
		}

	  }

	  public virtual void testGlobalGrantAuthorizationCheck()
	  {
		Resource resource1 = TestResource.RESOURCE1;

		// create global authorization which grants all permissions to all users (on resource1):
		Authorization globalAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		globalAuth.Resource = resource1;
		globalAuth.ResourceId = ANY;
		globalAuth.addPermission(TestPermissions.ALL);
		authorizationService.saveAuthorization(globalAuth);

		IList<string> jonnysGroups = Arrays.asList(new string[]{"sales", "marketing"});
		IList<string> someOneElsesGroups = Arrays.asList(new string[]{"marketing"});

		// this authorizes any user to do anything in this resource:
		processEngineConfiguration.AuthorizationEnabled = true;
		assertTrue(authorizationService.isUserAuthorized("jonny", null, TestPermissions.ALL, resource1));
		assertTrue(authorizationService.isUserAuthorized("jonny", jonnysGroups, TestPermissions.ALL, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone", null, TestPermissions.ACCESS, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone", someOneElsesGroups, TestPermissions.ACCESS, resource1));
		assertTrue(authorizationService.isUserAuthorized("someone else", null, TestPermissions.DELETE, resource1));
		assertTrue(authorizationService.isUserAuthorized("jonny", null, TestPermissions.ALL, resource1, "someId"));
		assertTrue(authorizationService.isUserAuthorized("jonny", jonnysGroups, TestPermissions.ALL, resource1, "someId"));
		assertTrue(authorizationService.isUserAuthorized("someone", null, TestPermissions.ACCESS, resource1, "someId"));
		assertTrue(authorizationService.isUserAuthorized("someone else", null, TestPermissions.DELETE, resource1, "someOtherId"));
		processEngineConfiguration.AuthorizationEnabled = true;
	  }

	  public virtual void testDisabledAuthorizationCheck()
	  {
		// given
		Resource resource1 = TestResource.RESOURCE1;

		// when
		bool isAuthorized = authorizationService.isUserAuthorized("jonny", null, UPDATE, resource1);

		// then
		assertTrue(isAuthorized);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testConcurrentIsUserAuthorized() throws Exception
	  public virtual void testConcurrentIsUserAuthorized()
	  {
		int threadCount = 2;
		int invocationCount = 500;
		ExecutorService executorService = Executors.newFixedThreadPool(threadCount);

		try
		{
		  List<Callable<Exception>> callables = new List<Callable<Exception>>();

		  for (int i = 0; i < invocationCount; i++)
		  {
			callables.Add(new CallableAnonymousInnerClass(this));
		  }

		  IList<Future<Exception>> futures = executorService.invokeAll(callables);

		  foreach (Future<Exception> future in futures)
		  {
			Exception exception = future.get();
			if (exception != null)
			{
			  fail("No exception expected: " + exception.Message);
			}
		  }

		}
		finally
		{
		  // reset original logging level
		  executorService.shutdownNow();
		  executorService.awaitTermination(10, TimeUnit.SECONDS);
		}

	  }

	  private class CallableAnonymousInnerClass : Callable<Exception>
	  {
		  private readonly AuthorizationServiceTest outerInstance;

		  public CallableAnonymousInnerClass(AuthorizationServiceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Exception call() throws Exception
		  public Exception call()
		  {
			try
			{
			  outerInstance.authorizationService.isUserAuthorized("jonny", null, UPDATE, TestResource.RESOURCE1, ANY);
			}
			catch (Exception e)
			{
			  return e;
			}
			return null;
		  }
	  }

	  public virtual void testReportResourceAuthorization()
	  {
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = userId;
		authorization.addPermission(ALL);
		authorization.Resource = REPORT;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;
		assertEquals(true, authorizationService.isUserAuthorized(userId, Arrays.asList(groupId), ALL, REPORT));
		processEngineConfiguration.AuthorizationEnabled = false;
	  }

	  public virtual void testReportResourcePermissions()
	  {
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = userId;
		authorization.addPermission(CREATE);
		authorization.addPermission(READ);
		authorization.addPermission(UPDATE);
		authorization.addPermission(DELETE);
		authorization.Resource = REPORT;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, CREATE, REPORT));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, READ, REPORT));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, UPDATE, REPORT));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, DELETE, REPORT));
		processEngineConfiguration.AuthorizationEnabled = false;
	  }

	  public virtual void testDashboardResourceAuthorization()
	  {
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = userId;
		authorization.addPermission(ALL);
		authorization.Resource = DASHBOARD;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;
		assertEquals(true, authorizationService.isUserAuthorized(userId, Arrays.asList(groupId), ALL, DASHBOARD));
		processEngineConfiguration.AuthorizationEnabled = false;
	  }

	  public virtual void testDashboardResourcePermission()
	  {
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = userId;
		authorization.addPermission(CREATE);
		authorization.addPermission(READ);
		authorization.addPermission(UPDATE);
		authorization.addPermission(DELETE);
		authorization.Resource = DASHBOARD;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, CREATE, DASHBOARD));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, READ, DASHBOARD));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, UPDATE, DASHBOARD));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, DELETE, DASHBOARD));
		processEngineConfiguration.AuthorizationEnabled = false;
	  }

	  public virtual void testIsPermissionGrantedAccess()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.addPermission(Permissions.ACCESS);
		authorization.Resource = Resources.APPLICATION;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertTrue(authorizationResult.isPermissionGranted(Permissions.ACCESS));
		assertFalse(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionGranted(ProcessInstancePermissions.RETRY_JOB));
		assertFalse(authorizationResult.isPermissionGranted(ProcessDefinitionPermissions.RETRY_JOB));
	  }

	  public virtual void testIsPermissionGrantedRetryJob()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.addPermission(ProcessInstancePermissions.RETRY_JOB);
		authorization.Resource = Resources.PROCESS_INSTANCE;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertTrue(authorizationResult.isPermissionGranted(ProcessInstancePermissions.RETRY_JOB));
		assertFalse(authorizationResult.isPermissionGranted(Permissions.ACCESS));
		assertFalse(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionGranted(ProcessDefinitionPermissions.RETRY_JOB));
	  }

	  public virtual void testIsPermissionGrantedBatchResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.addPermission(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		authorization.addPermission(BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES);
		authorization.addPermission(BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES);
		authorization.Resource = Resources.BATCH;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertTrue(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertTrue(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES));
		assertTrue(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionGranted(Permissions.ACCESS));
		assertFalse(authorizationResult.isPermissionGranted(Permissions.CREATE));
	  }

	  public virtual void testIsPermissionRevokedAccess()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.removePermission(Permissions.ACCESS);
		authorization.Resource = Resources.APPLICATION;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertTrue(authorizationResult.isPermissionRevoked(Permissions.ACCESS));
		assertFalse(authorizationResult.isPermissionRevoked(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionRevoked(ProcessInstancePermissions.RETRY_JOB));
		assertFalse(authorizationResult.isPermissionRevoked(ProcessDefinitionPermissions.RETRY_JOB));
	  }

	  public virtual void testIsPermissionRevokedRetryJob()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.removePermission(ProcessInstancePermissions.RETRY_JOB);
		authorization.Resource = Resources.PROCESS_INSTANCE;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertTrue(authorizationResult.isPermissionRevoked(ProcessInstancePermissions.RETRY_JOB));
		assertFalse(authorizationResult.isPermissionRevoked(Permissions.ACCESS));
		assertFalse(authorizationResult.isPermissionRevoked(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionRevoked(ProcessDefinitionPermissions.RETRY_JOB));
	  }

	  public virtual void testIsPermissionRevokedBatchResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.removePermission(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		authorization.removePermission(BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES);
		authorization.removePermission(BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES);
		authorization.Resource = Resources.BATCH;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertTrue(authorizationResult.isPermissionRevoked(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertTrue(authorizationResult.isPermissionRevoked(BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES));
		assertTrue(authorizationResult.isPermissionRevoked(BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionRevoked(BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES));
		assertFalse(authorizationResult.isPermissionRevoked(Permissions.ACCESS));
		assertFalse(authorizationResult.isPermissionRevoked(Permissions.CREATE));
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