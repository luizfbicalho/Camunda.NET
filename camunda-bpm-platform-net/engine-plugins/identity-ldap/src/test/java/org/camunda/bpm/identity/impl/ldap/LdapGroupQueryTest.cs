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
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Group = org.camunda.bpm.engine.identity.Group;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.checkPagingResults;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.identity.impl.ldap.LdapTestUtilities.testGroupPaging;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LdapGroupQueryTest : LdapIdentityProviderTest
	{

	  public virtual void testQueryNoFilter()
	  {
		IList<Group> groupList = identityService.createGroupQuery().list();

		assertEquals(6, groupList.Count);
	  }

	  public virtual void testFilterByGroupId()
	  {
		Group group = identityService.createGroupQuery().groupId("management").singleResult();
		assertNotNull(group);

		// validate result
		assertEquals("management", group.Id);
		assertEquals("management", group.Name);

		group = identityService.createGroupQuery().groupId("whatever").singleResult();
		assertNull(group);
	  }

	  public virtual void testFilterByGroupIdIn()
	  {
		IList<Group> groups = identityService.createGroupQuery().groupIdIn("external", "management").list();

		assertEquals(2, groups.Count);
		foreach (Group group in groups)
		{
		  if (!group.Id.Equals("external") && !group.Id.Equals("management"))
		  {
			fail();
		  }
		}
	  }

	  public virtual void testFilterByGroupName()
	  {
		Group group = identityService.createGroupQuery().groupName("management").singleResult();
		assertNotNull(group);

		// validate result
		assertEquals("management", group.Id);
		assertEquals("management", group.Name);

		group = identityService.createGroupQuery().groupName("whatever").singleResult();
		assertNull(group);
	  }

	  public virtual void testFilterByGroupNameLike()
	  {
		Group group = identityService.createGroupQuery().groupNameLike("manage*").singleResult();
		assertNotNull(group);

		// validate result
		assertEquals("management", group.Id);
		assertEquals("management", group.Name);

		group = identityService.createGroupQuery().groupNameLike("what*").singleResult();
		assertNull(group);
	  }

	  public virtual void testFilterByGroupMember()
	  {
		IList<Group> list = identityService.createGroupQuery().groupMember("daniel").list();
		assertEquals(3, list.Count);
		list = identityService.createGroupQuery().groupMember("oscar").list();
		assertEquals(2, list.Count);
		list = identityService.createGroupQuery().groupMember("ruecker").list();
		assertEquals(4, list.Count);
		list = identityService.createGroupQuery().groupMember("non-existing").list();
		assertEquals(0, list.Count);
	  }

	  public virtual void testFilterByGroupMemberSpecialCharacter()
	  {
		IList<Group> list = identityService.createGroupQuery().groupMember("david(IT)").list();
		assertEquals(2, list.Count);
	  }

	  public virtual void testFilterByGroupMemberPosix()
	  {

		// by default the configuration does not use posix groups
		LdapConfiguration ldapConfiguration = new LdapConfiguration();
		ldapConfiguration.GroupMemberAttribute = "memberUid";
		ldapConfiguration.GroupSearchFilter = "(someFilter)";

		LdapIdentityProviderSession session = new LdapIdentityProviderSessionAnonymousInnerClass(this, ldapConfiguration);

		// if I query for groups by group member
		LdapGroupQuery query = new LdapGroupQuery();
		query.groupMember("jonny");

		// then the full DN is requested. This is the default behavior.
		string filter = session.getGroupSearchFilter(query);
		assertEquals("(&(someFilter)(memberUid=jonny, fullDn))", filter);

		// If I turn on posix groups
		ldapConfiguration.UsePosixGroups = true;

		//  then the filter string does not contain the full DN for the
		// user but the simple (unqualified) userId as provided in the query
		filter = session.getGroupSearchFilter(query);
		assertEquals("(&(someFilter)(memberUid=jonny))", filter);

	  }

	  private class LdapIdentityProviderSessionAnonymousInnerClass : LdapIdentityProviderSession
	  {
		  private readonly LdapGroupQueryTest outerInstance;

		  public LdapIdentityProviderSessionAnonymousInnerClass(LdapGroupQueryTest outerInstance, org.camunda.bpm.identity.impl.ldap.LdapConfiguration ldapConfiguration) : base(ldapConfiguration)
		  {
			  this.outerInstance = outerInstance;
		  }

			// mock getDnForUser
		  protected internal override string getDnForUser(string userId)
		  {
			return userId + ", fullDn";
		  }
	  }


	  public virtual void testPagination()
	  {
		testGroupPaging(identityService);
	  }

	  public virtual void testPaginationWithAuthenticatedUser()
	  {
		createGrantAuthorization(GROUP, "management", "oscar", READ);
		createGrantAuthorization(GROUP, "consulting", "oscar", READ);
		createGrantAuthorization(GROUP, "external", "oscar", READ);

		try
		{
		  processEngineConfiguration.AuthorizationEnabled = true;

		  identityService.AuthenticatedUserId = "oscar";

		  ISet<string> groupNames = new HashSet<string>();
		  IList<Group> groups = identityService.createGroupQuery().listPage(0, 2);
		  assertEquals(2, groups.Count);
		  checkPagingResults(groupNames, groups[0].Id, groups[1].Id);

		  groups = identityService.createGroupQuery().listPage(2, 2);
		  assertEquals(1, groups.Count);
		  assertFalse(groupNames.Contains(groups[0].Id));
		  groupNames.Add(groups[0].Id);

		  groups = identityService.createGroupQuery().listPage(4, 2);
		  assertEquals(0, groups.Count);

		  identityService.AuthenticatedUserId = "daniel";

		  groups = identityService.createGroupQuery().listPage(0, 2);
		  assertEquals(0, groups.Count);

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