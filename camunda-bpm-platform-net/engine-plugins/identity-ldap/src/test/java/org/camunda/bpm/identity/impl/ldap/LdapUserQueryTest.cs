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
namespace org.camunda.bpm.identity.impl.ldap
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;


	using BadUserRequestException = org.camunda.bpm.engine.BadUserRequestException;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using User = org.camunda.bpm.engine.identity.User;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.checkPagingResults;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.testUserPaging;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.testUserPagingWithMemberOfGroup;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LdapUserQueryTest : LdapIdentityProviderTest
	{

	  public virtual void testQueryNoFilter()
	  {
		IList<User> result = identityService.createUserQuery().list();
		assertEquals(12, result.Count);
	  }

	  public virtual void testFilterByUserId()
	  {
		User user = identityService.createUserQuery().userId("oscar").singleResult();
		assertNotNull(user);

		// validate user
		assertEquals("oscar", user.Id);
		assertEquals("Oscar", user.FirstName);
		assertEquals("The Crouch", user.LastName);
		assertEquals("oscar@camunda.org", user.Email);


		user = identityService.createUserQuery().userId("non-existing").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByUserIdIn()
	  {
		IList<User> users = identityService.createUserQuery().userIdIn("oscar", "monster").list();
		assertNotNull(users);
		assertEquals(2, users.Count);

		users = identityService.createUserQuery().userIdIn("oscar", "monster", "daniel").list();
		assertNotNull(users);
		assertEquals(3, users.Count);

		users = identityService.createUserQuery().userIdIn("oscar", "monster", "daniel", "non-existing").list();
		assertNotNull(users);
		assertEquals(3, users.Count);
	  }

	  public virtual void testFilterByUserIdWithCapitalization()
	  {
		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;
		  identityService.AuthenticatedUserId = "Oscar";
		  User user = identityService.createUserQuery().userId("Oscar").singleResult();
		  assertNotNull(user);

		  // validate user
		  assertEquals("oscar", user.Id);
		  assertEquals("Oscar", user.FirstName);
		  assertEquals("The Crouch", user.LastName);
		  assertEquals("oscar@camunda.org", user.Email);
		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		  identityService.clearAuthentication();
		}
	  }

	  public virtual void testFilterByFirstname()
	  {
		User user = identityService.createUserQuery().userFirstName("Oscar").singleResult();
		assertNotNull(user);

		user = identityService.createUserQuery().userFirstName("non-existing").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByFirstnameLike()
	  {
		User user = identityService.createUserQuery().userFirstNameLike("Osc*").singleResult();
		assertNotNull(user);

		user = identityService.createUserQuery().userFirstNameLike("non-exist*").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByLastname()
	  {
		User user = identityService.createUserQuery().userLastName("The Crouch").singleResult();
		assertNotNull(user);

		user = identityService.createUserQuery().userLastName("non-existing").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByLastnameLike()
	  {
		User user = identityService.createUserQuery().userLastNameLike("The Cro*").singleResult();
		assertNotNull(user);
		user = identityService.createUserQuery().userLastNameLike("The C*").singleResult();
		assertNotNull(user);

		user = identityService.createUserQuery().userLastNameLike("non-exist*").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByEmail()
	  {
		User user = identityService.createUserQuery().userEmail("oscar@camunda.org").singleResult();
		assertNotNull(user);

		user = identityService.createUserQuery().userEmail("non-exist*").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByEmailLike()
	  {
		User user = identityService.createUserQuery().userEmailLike("oscar@*").singleResult();
		assertNotNull(user);

		user = identityService.createUserQuery().userEmailLike("non-exist*").singleResult();
		assertNull(user);
	  }

	  public virtual void testFilterByGroupId()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("development").list();
		assertEquals(3, result.Count);
	  }

	  public virtual void testFilterByGroupIdAndFirstname()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("development").userFirstName("Oscar").list();
		assertEquals(1, result.Count);
	  }

	  public virtual void testFilterByGroupIdAndId()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("development").userId("oscar").list();
		assertEquals(1, result.Count);
	  }

	  public virtual void testFilterByGroupIdAndLastname()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("development").userLastName("The Crouch").list();
		assertEquals(1, result.Count);
	  }

	  public virtual void testFilterByGroupIdAndEmail()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("development").userEmail("oscar@camunda.org").list();
		assertEquals(1, result.Count);
	  }

	  public virtual void testFilterByGroupIdAndEmailLike()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("development").userEmailLike("*@camunda.org").list();
		assertEquals(3, result.Count);
	  }

	  public virtual void testFilterByGroupIdAndIdForDnUsingCn()
	  {
		IList<User> result = identityService.createUserQuery().memberOfGroup("external").userId("fozzie").list();
		assertEquals(1, result.Count);
	  }

	  public virtual void testAuthenticatedUserSeesHimself()
	  {
		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "non-existing";
		  assertEquals(0, identityService.createUserQuery().count());

		  identityService.AuthenticatedUserId = "oscar";
		  assertEquals(1, identityService.createUserQuery().count());

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		  identityService.clearAuthentication();
		}
	  }

	  public virtual void testPagination()
	  {
		testUserPaging(identityService);
	  }

	  public virtual void testPaginationWithMemberOfGroup()
	  {
		testUserPagingWithMemberOfGroup(identityService);
	  }

	  public virtual void testPaginationWithAuthenticatedUser()
	  {
		createGrantAuthorization(USER, "roman", "oscar", READ);
		createGrantAuthorization(USER, "daniel", "oscar", READ);
		createGrantAuthorization(USER, "monster", "oscar", READ);
		createGrantAuthorization(USER, "ruecker", "oscar", READ);

		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "oscar";

		  ISet<string> userNames = new HashSet<string>();
		  IList<User> users = identityService.createUserQuery().listPage(0, 2);
		  assertEquals(2, users.Count);
		  checkPagingResults(userNames, users[0].Id, users[1].Id);

		  users = identityService.createUserQuery().listPage(2, 2);
		  assertEquals(2, users.Count);
		  checkPagingResults(userNames, users[0].Id, users[1].Id);

		  users = identityService.createUserQuery().listPage(4, 2);
		  assertEquals(1, users.Count);
		  assertFalse(userNames.Contains(users[0].Id));
		  userNames.Add(users[0].Id);

		  identityService.AuthenticatedUserId = "daniel";

		  users = identityService.createUserQuery().listPage(0, 2);
		  assertEquals(1, users.Count);

		  assertEquals("daniel", users[0].Id);

		  users = identityService.createUserQuery().listPage(2, 2);
		  assertEquals(0, users.Count);

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

	  public virtual void testNativeQueryFail()
	  {
		try
		{
		  identityService.createNativeUserQuery();
		  fail("Native queries are not supported in LDAP case.");
		}
		catch (BadUserRequestException ex)
		{
		  assertTrue("Wrong exception", ex.Message.contains("Native user queries are not supported for LDAP"));
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