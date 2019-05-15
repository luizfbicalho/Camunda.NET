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
namespace org.camunda.bpm.identity.impl.ldap
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.testGroupPaging;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.testUserPaging;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class LdapDisableAuthorizationCheckTest : ResourceProcessEngineTestCase
	{

	  public LdapDisableAuthorizationCheckTest() : base("camunda.ldap.disable.authorization.check.cfg.xml")
	  {
	  }

	  protected internal static LdapTestEnvironment ldapTestEnvironment;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		if (ldapTestEnvironment == null)
		{
		  ldapTestEnvironment = new LdapTestEnvironment();
		  ldapTestEnvironment.init();
		}
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		if (ldapTestEnvironment != null)
		{
		  ldapTestEnvironment.shutdown();
		  ldapTestEnvironment = null;
		}
		base.tearDown();
	  }

	  public virtual void testUserQueryPagination()
	  {
		LdapTestUtilities.testUserPaging(identityService);
	  }

	  public virtual void testUserQueryPaginationWithAuthenticatedUserWithoutAuthorizations()
	  {
		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "oscar";
		  testUserPaging(identityService);

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		  identityService.clearAuthentication();
		}
	  }

	  public virtual void testUserQueryPaginationWithAuthenticatedUserWithAuthorizations()
	  {
		createGrantAuthorization(USER, "roman", "oscar", READ);
		createGrantAuthorization(USER, "daniel", "oscar", READ);
		createGrantAuthorization(USER, "monster", "oscar", READ);
		createGrantAuthorization(USER, "ruecker", "oscar", READ);

		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "oscar";
		  testUserPaging(identityService);

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		  identityService.clearAuthentication();

		  foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		  {
			authorizationService.deleteAuthorization(authorization.Id);
		  }

		}
	  }

	  public virtual void testGroupQueryPagination()
	  {
		testGroupPaging(identityService);
	  }

	  public virtual void testGroupQueryPaginationWithAuthenticatedUserWithoutAuthorizations()
	  {
		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "oscar";
		  testGroupPaging(identityService);

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		  identityService.clearAuthentication();
		}
	  }

	  public virtual void testGroupQueryPaginationWithAuthenticatedUserWithAuthorizations()
	  {
		createGrantAuthorization(GROUP, "management", "oscar", READ);
		createGrantAuthorization(GROUP, "consulting", "oscar", READ);
		createGrantAuthorization(GROUP, "external", "oscar", READ);

		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "oscar";
		  testGroupPaging(identityService);

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		  identityService.clearAuthentication();

		  foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		  {
			authorizationService.deleteAuthorization(authorization.Id);
		  }

		}
	  }

	  protected internal virtual void createGrantAuthorization(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		Authorization authorization = createAuthorization(AUTH_TYPE_GRANT, resource, resourceId);
		authorization.UserId = userId;
		foreach (Permission permission in permissions)
		{
		  authorization.addPermission(permission);
		}
		authorizationService.saveAuthorization(authorization);
	  }

	  protected internal virtual Authorization createAuthorization(int type, Resource resource, string resourceId)
	  {
		Authorization authorization = authorizationService.createNewAuthorization(type);

		authorization.Resource = resource;
		if (!string.ReferenceEquals(resourceId, null))
		{
		  authorization.ResourceId = resourceId;
		}

		return authorization;
	  }

	}

}