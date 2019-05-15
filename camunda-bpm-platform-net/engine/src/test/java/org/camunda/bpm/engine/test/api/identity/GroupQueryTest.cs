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

	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class GroupQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		createGroup("muppets", "Muppet show characters_", "user");
		createGroup("frogs", "Famous frogs", "user");
		createGroup("mammals", "Famous mammals from eighties", "user");
		createGroup("admin", "Administrators", "security");

		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("fozzie"));
		identityService.saveUser(identityService.newUser("mispiggy"));

		identityService.saveTenant(identityService.newTenant("tenant"));

		identityService.createMembership("kermit", "muppets");
		identityService.createMembership("fozzie", "muppets");
		identityService.createMembership("mispiggy", "muppets");

		identityService.createMembership("kermit", "frogs");

		identityService.createMembership("fozzie", "mammals");
		identityService.createMembership("mispiggy", "mammals");

		identityService.createMembership("kermit", "admin");

		identityService.createTenantGroupMembership("tenant", "frogs");

	  }

	  private Group createGroup(string id, string name, string type)
	  {
		Group group = identityService.newGroup(id);
		group.Name = name;
		group.Type = type;
		identityService.saveGroup(group);
		return group;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		identityService.deleteUser("kermit");
		identityService.deleteUser("fozzie");
		identityService.deleteUser("mispiggy");

		identityService.deleteGroup("muppets");
		identityService.deleteGroup("mammals");
		identityService.deleteGroup("frogs");
		identityService.deleteGroup("admin");

		identityService.deleteTenant("tenant");

		base.tearDown();
	  }

	  public virtual void testQueryById()
	  {
		GroupQuery query = identityService.createGroupQuery().groupId("muppets");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidId()
	  {
		GroupQuery query = identityService.createGroupQuery().groupId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createGroupQuery().groupId(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByIdIn()
	  {
		// empty list
		assertTrue(identityService.createGroupQuery().groupIdIn("a", "b").list().Empty);

		// collect all ids
		IList<Group> list = identityService.createGroupQuery().list();
		string[] ids = new string[list.Count];
		for (int i = 0; i < ids.Length; i++)
		{
		  ids[i] = list[i].Id;
		}

		IList<Group> idInList = identityService.createGroupQuery().groupIdIn(ids).list();
		assertEquals(list.Count, idInList.Count);
		foreach (Group group in idInList)
		{
		  bool found = false;
		  foreach (Group otherGroup in list)
		  {
			if (otherGroup.Id.Equals(group.Id))
			{
			  found = true;
			  break;
			}
		  }
		  if (!found)
		  {
			fail("Expected to find group " + group);
		  }
		}
	  }

	  public virtual void testQueryByName()
	  {
		GroupQuery query = identityService.createGroupQuery().groupName("Muppet show characters_");
		verifyQueryResults(query, 1);

		query = identityService.createGroupQuery().groupName("Famous frogs");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidName()
	  {
		GroupQuery query = identityService.createGroupQuery().groupName("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createGroupQuery().groupName(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNameLike()
	  {
		GroupQuery query = identityService.createGroupQuery().groupNameLike("%Famous%");
		verifyQueryResults(query, 2);

		query = identityService.createGroupQuery().groupNameLike("Famous%");
		verifyQueryResults(query, 2);

		query = identityService.createGroupQuery().groupNameLike("%show%");
		verifyQueryResults(query, 1);

		query = identityService.createGroupQuery().groupNameLike("%ters\\_");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidNameLike()
	  {
		GroupQuery query = identityService.createGroupQuery().groupNameLike("%invalid%");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createGroupQuery().groupNameLike(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByType()
	  {
		GroupQuery query = identityService.createGroupQuery().groupType("user");
		verifyQueryResults(query, 3);

		query = identityService.createGroupQuery().groupType("admin");
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryByInvalidType()
	  {
		GroupQuery query = identityService.createGroupQuery().groupType("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createGroupQuery().groupType(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByMember()
	  {
		GroupQuery query = identityService.createGroupQuery().groupMember("fozzie");
		verifyQueryResults(query, 2);

		query = identityService.createGroupQuery().groupMember("kermit");
		verifyQueryResults(query, 3);

		query = query.orderByGroupId().asc();
		IList<Group> groups = query.list();
		assertEquals(3, groups.Count);
		assertEquals("admin", groups[0].Id);
		assertEquals("frogs", groups[1].Id);
		assertEquals("muppets", groups[2].Id);

		query = query.groupType("user");
		groups = query.list();
		assertEquals(2, groups.Count);
		assertEquals("frogs", groups[0].Id);
		assertEquals("muppets", groups[1].Id);
	  }

	  public virtual void testQueryByInvalidMember()
	  {
		GroupQuery query = identityService.createGroupQuery().groupMember("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createGroupQuery().groupMember(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByMemberOfTenant()
	  {
		GroupQuery query = identityService.createGroupQuery().memberOfTenant("nonExisting");
		verifyQueryResults(query, 0);

		query = identityService.createGroupQuery().memberOfTenant("tenant");
		verifyQueryResults(query, 1);

		Group group = query.singleResult();
		assertEquals("frogs", group.Id);
	  }

	  public virtual void testQuerySorting()
	  {
		// asc
		assertEquals(4, identityService.createGroupQuery().orderByGroupId().asc().count());
		assertEquals(4, identityService.createGroupQuery().orderByGroupName().asc().count());
		assertEquals(4, identityService.createGroupQuery().orderByGroupType().asc().count());

		// desc
		assertEquals(4, identityService.createGroupQuery().orderByGroupId().desc().count());
		assertEquals(4, identityService.createGroupQuery().orderByGroupName().desc().count());
		assertEquals(4, identityService.createGroupQuery().orderByGroupType().desc().count());

		// Multiple sortings
		GroupQuery query = identityService.createGroupQuery().orderByGroupType().asc().orderByGroupName().desc();
		IList<Group> groups = query.list();
		assertEquals(4, query.count());

		assertEquals("security", groups[0].Type);
		assertEquals("user", groups[1].Type);
		assertEquals("user", groups[2].Type);
		assertEquals("user", groups[3].Type);

		assertEquals("admin", groups[0].Id);
		assertEquals("muppets", groups[1].Id);
		assertEquals("mammals", groups[2].Id);
		assertEquals("frogs", groups[3].Id);
	  }

	  public virtual void testQueryInvalidSortingUsage()
	  {
		try
		{
		  identityService.createGroupQuery().orderByGroupId().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  identityService.createGroupQuery().orderByGroupId().orderByGroupName().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  private void verifyQueryResults(GroupQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  private void verifySingleResultFails(GroupQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	}

}