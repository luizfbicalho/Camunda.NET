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
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	public class AuthorizationQueryAuthorizationsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal AuthorizationService authorizationService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		authorizationService = engineRule.AuthorizationService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		cleanupAfterTest();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySingleCorrectPermission() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQuerySingleCorrectPermission()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Resource = Resources.PROCESS_DEFINITION;
		authorization.addPermission(Permissions.READ);
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// assume
		Authorization authResult = authorizationService.createAuthorizationQuery().userIdIn("userId").resourceType(Resources.PROCESS_DEFINITION).singleResult();
		assertNotNull(authResult);

		// then
		assertEquals(1, authorizationService.createAuthorizationQuery().hasPermission(Permissions.READ).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySingleIncorrectPermission() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQuerySingleIncorrectPermission()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Resource = Resources.BATCH;
		authorization.addPermission(BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES);
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// assume
		Authorization authResult = authorizationService.createAuthorizationQuery().userIdIn("userId").resourceType(Resources.BATCH).singleResult();
		assertNotNull(authResult);

		// then
		assertEquals(0, authorizationService.createAuthorizationQuery().hasPermission(Permissions.CREATE_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryPermissionsWithWrongResource() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryPermissionsWithWrongResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Resource = Resources.APPLICATION;
		authorization.addPermission(Permissions.ACCESS);
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// assume
		Authorization authResult = authorizationService.createAuthorizationQuery().userIdIn("userId").resourceType(Resources.APPLICATION).singleResult();
		assertNotNull(authResult);

		// when
		Authorization accessResult = authorizationService.createAuthorizationQuery().hasPermission(Permissions.ACCESS).singleResult();
		IList<Authorization> retryJobPDResult = authorizationService.createAuthorizationQuery().hasPermission(ProcessDefinitionPermissions.RETRY_JOB).list();
		IList<Authorization> retryJobPIResult = authorizationService.createAuthorizationQuery().hasPermission(ProcessInstancePermissions.RETRY_JOB).list();

		// then
		assertNotNull(accessResult);
		assertEquals(1, authorizationService.createAuthorizationQuery().hasPermission(Permissions.ACCESS).count());
		assertTrue(retryJobPDResult.Count == 0);
		assertEquals(0, authorizationService.createAuthorizationQuery().hasPermission(ProcessDefinitionPermissions.RETRY_JOB).count());
		assertTrue(retryJobPIResult.Count == 0);
		assertEquals(0, authorizationService.createAuthorizationQuery().hasPermission(ProcessInstancePermissions.RETRY_JOB).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryPermissionWithMixedResource() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryPermissionWithMixedResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Resource = Resources.APPLICATION;
		authorization.addPermission(Permissions.ACCESS);
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// assume
		Authorization authResult = authorizationService.createAuthorizationQuery().userIdIn("userId").resourceType(Resources.APPLICATION).singleResult();
		assertNotNull(authResult);

		// then
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceType(Resources.BATCH).hasPermission(Permissions.ACCESS).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryPermissionsWithMixedResource() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryPermissionsWithMixedResource()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Resource = Resources.PROCESS_DEFINITION;
		authorization.addPermission(Permissions.READ);
		authorization.addPermission(ProcessDefinitionPermissions.RETRY_JOB);
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// assume
		Authorization authResult = authorizationService.createAuthorizationQuery().userIdIn("userId").resourceType(Resources.PROCESS_DEFINITION).singleResult();
		assertNotNull(authResult);
		assertEquals(1, authorizationService.createAuthorizationQuery().resourceType(Resources.PROCESS_DEFINITION).hasPermission(ProcessDefinitionPermissions.READ).hasPermission(ProcessDefinitionPermissions.RETRY_JOB).count());
		assertEquals(1, authorizationService.createAuthorizationQuery().resourceType(Resources.PROCESS_DEFINITION).hasPermission(ProcessDefinitionPermissions.READ).count());

		// then
		assertEquals(0, authorizationService.createAuthorizationQuery().resourceType(Resources.PROCESS_DEFINITION).hasPermission(Permissions.READ).hasPermission(Permissions.ACCESS).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCorrectAndIncorrectPersmission() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryCorrectAndIncorrectPersmission()
	  {
		// given
		Authorization authorization = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		authorization.UserId = "userId";
		authorization.Resource = Resources.PROCESS_DEFINITION;
		authorization.addPermission(Permissions.READ);
		authorization.addPermission(ProcessDefinitionPermissions.RETRY_JOB);
		authorization.ResourceId = ANY;
		authorizationService.saveAuthorization(authorization);

		processEngineConfiguration.AuthorizationEnabled = true;

		// assume
		Authorization authResult = authorizationService.createAuthorizationQuery().userIdIn("userId").resourceType(Resources.PROCESS_DEFINITION).singleResult();
		assertNotNull(authResult);

		// then
		assertEquals(0, authorizationService.createAuthorizationQuery().hasPermission(Permissions.READ).hasPermission(Permissions.ACCESS).count());
	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }
	}

}