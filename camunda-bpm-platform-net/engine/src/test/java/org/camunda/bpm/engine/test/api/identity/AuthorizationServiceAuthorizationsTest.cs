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
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestUtil.assertExceptionInfo;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// <para>Ensures authorizations are properly
	/// enforced by the <seealso cref="AuthorizationService"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationServiceAuthorizationsTest : PluggableProcessEngineTestCase
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

	  public virtual void testCreateAuthorization()
	  {

		// add base permission which allows nobody to create authorizations
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = AUTHORIZATION;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL); // add all then remove 'create'
		basePerms.removePermission(CREATE);
		authorizationService.saveAuthorization(basePerms);

		// now enable authorizations:
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  // we cannot create another authorization
		  authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, AUTHORIZATION.resourceName(), null, info);
		}

		// circumvent auth check to get new transient object
		Authorization authorization = new AuthorizationEntity(AUTH_TYPE_REVOKE);
		authorization.UserId = "someUserId";
		authorization.Resource = Resources.APPLICATION;

		try
		{
		  authorizationService.saveAuthorization(authorization);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(CREATE.Name, AUTHORIZATION.resourceName(), null, info);
		}
	  }

	  public virtual void testDeleteAuthorization()
	  {

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = AUTHORIZATION;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(DELETE); // revoke delete
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  // try to delete authorization
		  authorizationService.deleteAuthorization(basePerms.Id);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(DELETE.Name, AUTHORIZATION.resourceName(), basePerms.Id, info);
		}
	  }

	  public virtual void testUserUpdateAuthorizations()
	  {

		// create global auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = AUTHORIZATION;
		basePerms.ResourceId = ANY;
		basePerms.addPermission(ALL);
		basePerms.removePermission(UPDATE); // revoke update
		authorizationService.saveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.AuthorizationEnabled = true;
		identityService.AuthenticatedUserId = jonny2;

		// fetch authhorization
		basePerms = authorizationService.createAuthorizationQuery().singleResult();
		// make some change to the perms
		basePerms.addPermission(ALL);

		try
		{
		  authorizationService.saveAuthorization(basePerms);
		  fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  assertEquals(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  assertEquals(jonny2, e.UserId);
		  assertExceptionInfo(UPDATE.Name, AUTHORIZATION.resourceName(), basePerms.Id, info);
		}

		// but we can create a new auth
		Authorization newAuth = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		newAuth.UserId = "jonny2";
		newAuth.Resource = AUTHORIZATION;
		newAuth.ResourceId = ANY;
		newAuth.addPermission(ALL);
		authorizationService.saveAuthorization(newAuth);

	  }

	  public virtual void testAuthorizationQueryAuthorizations()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new auth wich revokes read access on auth
		Authorization basePerms = authorizationService.createNewAuthorization(AUTH_TYPE_GLOBAL);
		basePerms.Resource = AUTHORIZATION;
		basePerms.ResourceId = ANY;
		authorizationService.saveAuthorization(basePerms);

		// I can see it
		assertEquals(1, authorizationService.createAuthorizationQuery().count());

		// now enable checks
		processEngineConfiguration.AuthorizationEnabled = true;

		// I can't see it
		assertEquals(0, authorizationService.createAuthorizationQuery().count());

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSaveAuthorizationAddPermissionWithInvalidResource() throws Exception
	  public virtual void testSaveAuthorizationAddPermissionWithInvalidResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.addPermission(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		authorization.Resource = Resources.APPLICATION;
		authorization.ResourceId = ANY;

		processEngineConfiguration.AuthorizationEnabled = true;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'0' is not valid for 'CREATE_BATCH_MIGRATE_PROCESS_INSTANCES' permission."));
		}

		// given
		authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.addPermission(Permissions.ACCESS);
		authorization.Resource = Resources.BATCH;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'13' is not valid for 'ACCESS' permission."));
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSaveAuthorizationMultipleResourcesIncludingInvalidResource() throws Exception
	  public virtual void testSaveAuthorizationMultipleResourcesIncludingInvalidResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.addPermission(Permissions.READ_HISTORY);
		authorization.addPermission(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		authorization.Resource = Resources.PROCESS_DEFINITION;

		processEngineConfiguration.AuthorizationEnabled = true;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'6' is not valid for 'CREATE_BATCH_MIGRATE_PROCESS_INSTANCES' permission."));
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSaveAuthorizationRemovePermissionWithInvalidResource() throws Exception
	  public virtual void testSaveAuthorizationRemovePermissionWithInvalidResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		authorization.UserId = "userId";
		authorization.removePermission(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		authorization.Resource = Resources.PROCESS_DEFINITION;
		authorization.ResourceId = ANY;

		processEngineConfiguration.AuthorizationEnabled = true;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'6' is not valid for 'CREATE_BATCH_MIGRATE_PROCESS_INSTANCES' permission."));
		}

		// given
		authorization = authorizationService.createNewAuthorization(AUTH_TYPE_REVOKE);
		authorization.UserId = "userId";
		authorization.addPermission(Permissions.ACCESS);
		authorization.Resource = Resources.PROCESS_DEFINITION;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'6' is not valid for 'ACCESS' permission."));
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSaveAuthorizationSetPermissionsWithInvalidResource() throws Exception
	  public virtual void testSaveAuthorizationSetPermissionsWithInvalidResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Permissions = new BatchPermissions[] {BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES};
		authorization.Resource = Resources.PROCESS_INSTANCE;
		authorization.ResourceId = ANY;

		processEngineConfiguration.AuthorizationEnabled = true;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'8' is not valid for 'CREATE_BATCH_MIGRATE_PROCESS_INSTANCES' permission."));
		}

		// given
		authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Permissions = new Permissions[] {Permissions.CREATE, Permissions.ACCESS};
		authorization.Resource = Resources.PROCESS_INSTANCE;

		try
		{
		  // when
		  authorizationService.saveAuthorization(authorization);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  assertTrue(e.Message.contains("The resource type with id:'8' is not valid for 'ACCESS' permission."));
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSaveAuthorizationSetPermissionsWithValidResource() throws Exception
	  public virtual void testSaveAuthorizationSetPermissionsWithValidResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.addPermission(Permissions.ACCESS);
		// 'ACCESS' is not allowed for Batches
		// however, it will be reset by next line, so saveAuthorization will be successful
		authorization.Permissions = new BatchPermissions[] {BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES, BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES};
		authorization.Resource = Resources.BATCH;
		authorization.ResourceId = ANY;

		processEngineConfiguration.AuthorizationEnabled = true;

		// when
		authorizationService.saveAuthorization(authorization);

		// then
		Authorization authorizationResult = authorizationService.createAuthorizationQuery().resourceType(Resources.BATCH).singleResult();
		assertNotNull(authorizationResult);
		assertTrue(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES));
		assertTrue(authorizationResult.isPermissionGranted(BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES));
	  }

	  public virtual void testIsUserAuthorizedWithInvalidResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.addPermission(Permissions.ACCESS);
		authorization.Resource = Resources.APPLICATION;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// then
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, Permissions.ACCESS, Resources.APPLICATION));
		assertEquals(false, authorizationService.isUserAuthorized(userId, null, BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES, Resources.BATCH));
		assertEquals(false, authorizationService.isUserAuthorized(userId, null, ProcessDefinitionPermissions.RETRY_JOB, Resources.PROCESS_DEFINITION));
		assertEquals(false, authorizationService.isUserAuthorized(userId, null, ProcessInstancePermissions.RETRY_JOB, Resources.PROCESS_INSTANCE));
		try
		{
		  authorizationService.isUserAuthorized(userId, null, BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES, Resources.APPLICATION);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("The resource type 'Application' is not valid"));
		  assertTrue(e.Message.contains(BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES.Name));
		}
		try
		{
		  authorizationService.isUserAuthorized(userId, null, ProcessDefinitionPermissions.RETRY_JOB, Resources.APPLICATION);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("The resource type 'Application' is not valid"));
		  assertTrue(e.Message.contains(ProcessDefinitionPermissions.RETRY_JOB.Name));
		}
		try
		{
		  authorizationService.isUserAuthorized(userId, null, ProcessInstancePermissions.RETRY_JOB, Resources.APPLICATION);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("The resource type 'Application' is not valid"));
		  assertTrue(e.Message.contains(ProcessInstancePermissions.RETRY_JOB.Name));
		}

	  }

	  public virtual void testIsUserAuthorizedWithInvalidResourceMultiplePermissions()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.addPermission(ProcessInstancePermissions.READ);
		authorization.addPermission(ProcessInstancePermissions.RETRY_JOB);
		authorization.Resource = Resources.PROCESS_INSTANCE;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// then
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, Permissions.READ, Resources.PROCESS_INSTANCE));
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, ProcessInstancePermissions.RETRY_JOB, Resources.PROCESS_INSTANCE));
		assertEquals(false, authorizationService.isUserAuthorized(userId, null, BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES, Resources.BATCH));
		assertEquals(false, authorizationService.isUserAuthorized(userId, null, ProcessDefinitionPermissions.RETRY_JOB, Resources.PROCESS_DEFINITION));
		assertEquals(false, authorizationService.isUserAuthorized(userId, null, Permissions.ACCESS, Resources.APPLICATION));
		try
		{
		  authorizationService.isUserAuthorized(userId, null, ProcessDefinitionPermissions.RETRY_JOB, Resources.PROCESS_INSTANCE);
		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertTrue(e.Message.contains("The resource type 'ProcessInstance' is not valid"));
		  assertTrue(e.Message.contains(ProcessDefinitionPermissions.RETRY_JOB.Name));
		}
	  }

	  public virtual void testIsUserAuthorizedWithValidResourceImpl()
	  {
		// given
		ResourceImpl resource = new ResourceImpl(this, "application", 0);
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		string userId = "userId";
		authorization.UserId = userId;
		authorization.addPermission(Permissions.ACCESS);
		authorization.Resource = Resources.APPLICATION;
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// then
		assertEquals(true, authorizationService.isUserAuthorized(userId, null, Permissions.ACCESS, resource));
	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	  internal class ResourceImpl : Resource
	  {
		  private readonly AuthorizationServiceAuthorizationsTest outerInstance;


//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		internal string resourceName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		internal int resourceType_Renamed;

		public ResourceImpl(AuthorizationServiceAuthorizationsTest outerInstance, string resourceName, int resourceType)
		{
			this.outerInstance = outerInstance;
		  this.resourceName_Renamed = resourceName;
		  this.resourceType_Renamed = resourceType;
		}

		public virtual string resourceName()
		{
		  return resourceName_Renamed;
		}

		public virtual int resourceType()
		{
		  return resourceType_Renamed;
		}

	  }

	}

}