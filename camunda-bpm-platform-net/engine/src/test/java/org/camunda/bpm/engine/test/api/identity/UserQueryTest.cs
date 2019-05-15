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

	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class UserQueryTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		createUser("kermit", "Kermit_", "The_frog", "kermit_@muppetshow.com");
		createUser("fozzie", "Fozzie", "Bear", "fozzie@muppetshow.com");
		createUser("gonzo", "Gonzo", "The great", "gonzo@muppetshow.com");

		identityService.saveGroup(identityService.newGroup("muppets"));
		identityService.saveGroup(identityService.newGroup("frogs"));

		identityService.saveTenant(identityService.newTenant("tenant"));

		identityService.createMembership("kermit", "muppets");
		identityService.createMembership("kermit", "frogs");
		identityService.createMembership("fozzie", "muppets");
		identityService.createMembership("gonzo", "muppets");

		identityService.createTenantUserMembership("tenant", "kermit");
	  }

	  private User createUser(string id, string firstName, string lastName, string email)
	  {
		User user = identityService.newUser(id);
		user.FirstName = firstName;
		user.LastName = lastName;
		user.Email = email;
		identityService.saveUser(user);
		return user;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		identityService.deleteUser("kermit");
		identityService.deleteUser("fozzie");
		identityService.deleteUser("gonzo");

		identityService.deleteGroup("muppets");
		identityService.deleteGroup("frogs");

		identityService.deleteTenant("tenant");

		base.tearDown();
	  }

	  public virtual void testQueryByNoCriteria()
	  {
		UserQuery query = identityService.createUserQuery();
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryById()
	  {
		UserQuery query = identityService.createUserQuery().userId("kermit");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidId()
	  {
		UserQuery query = identityService.createUserQuery().userId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userId(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByFirstName()
	  {
		UserQuery query = identityService.createUserQuery().userFirstName("Gonzo");
		verifyQueryResults(query, 1);

		User result = query.singleResult();
		assertEquals("gonzo", result.Id);
	  }

	  public virtual void testQueryByInvalidFirstName()
	  {
		UserQuery query = identityService.createUserQuery().userFirstName("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userFirstName(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByFirstNameLike()
	  {
		UserQuery query = identityService.createUserQuery().userFirstNameLike("%o%");
		verifyQueryResults(query, 2);

		query = identityService.createUserQuery().userFirstNameLike("Ker%");
		verifyQueryResults(query, 1);

		identityService.createUserQuery().userFirstNameLike("%mit\\_");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidFirstNameLike()
	  {
		UserQuery query = identityService.createUserQuery().userFirstNameLike("%mispiggy%");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userFirstNameLike(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByLastName()
	  {
		UserQuery query = identityService.createUserQuery().userLastName("Bear");
		verifyQueryResults(query, 1);

		User result = query.singleResult();
		assertEquals("fozzie", result.Id);
	  }

	  public virtual void testQueryByInvalidLastName()
	  {
		UserQuery query = identityService.createUserQuery().userLastName("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userLastName(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByLastNameLike()
	  {
		UserQuery query = identityService.createUserQuery().userLastNameLike("%\\_frog%");
		verifyQueryResults(query, 1);

		query = identityService.createUserQuery().userLastNameLike("%ea%");
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidLastNameLike()
	  {
		UserQuery query = identityService.createUserQuery().userLastNameLike("%invalid%");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userLastNameLike(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByEmail()
	  {
		UserQuery query = identityService.createUserQuery().userEmail("kermit_@muppetshow.com");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidEmail()
	  {
		UserQuery query = identityService.createUserQuery().userEmail("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userEmail(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByEmailLike()
	  {
		UserQuery query = identityService.createUserQuery().userEmailLike("%muppetshow.com");
		verifyQueryResults(query, 3);

		query = identityService.createUserQuery().userEmailLike("%kermit\\_%");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidEmailLike()
	  {
		UserQuery query = identityService.createUserQuery().userEmailLike("%invalid%");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().userEmailLike(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQuerySorting()
	  {
		// asc
		assertEquals(3, identityService.createUserQuery().orderByUserId().asc().count());
		assertEquals(3, identityService.createUserQuery().orderByUserEmail().asc().count());
		assertEquals(3, identityService.createUserQuery().orderByUserFirstName().asc().count());
		assertEquals(3, identityService.createUserQuery().orderByUserLastName().asc().count());

		// desc
		assertEquals(3, identityService.createUserQuery().orderByUserId().desc().count());
		assertEquals(3, identityService.createUserQuery().orderByUserEmail().desc().count());
		assertEquals(3, identityService.createUserQuery().orderByUserFirstName().desc().count());
		assertEquals(3, identityService.createUserQuery().orderByUserLastName().desc().count());

		// Combined with criteria
		UserQuery query = identityService.createUserQuery().userLastNameLike("%ea%").orderByUserFirstName().asc();
		IList<User> users = query.list();
		assertEquals(2,users.Count);
		assertEquals("Fozzie", users[0].FirstName);
		assertEquals("Gonzo", users[1].FirstName);
	  }

	  public virtual void testQueryInvalidSortingUsage()
	  {
		try
		{
		  identityService.createUserQuery().orderByUserId().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  identityService.createUserQuery().orderByUserId().orderByUserEmail().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByMemberOfGroup()
	  {
		UserQuery query = identityService.createUserQuery().memberOfGroup("muppets");
		verifyQueryResults(query, 3);

		query = identityService.createUserQuery().memberOfGroup("frogs");
		verifyQueryResults(query, 1);

		User result = query.singleResult();
		assertEquals("kermit", result.Id);
	  }

	  public virtual void testQueryByInvalidMemberOfGoup()
	  {
		UserQuery query = identityService.createUserQuery().memberOfGroup("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  identityService.createUserQuery().memberOfGroup(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByMemberOfTenant()
	  {
		UserQuery query = identityService.createUserQuery().memberOfTenant("nonExisting");
		verifyQueryResults(query, 0);

		query = identityService.createUserQuery().memberOfTenant("tenant");
		verifyQueryResults(query, 1);

		User result = query.singleResult();
		assertEquals("kermit", result.Id);
	  }

	  private void verifyQueryResults(UserQuery query, int countExpected)
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

	  private void verifySingleResultFails(UserQuery query)
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

	  public virtual void testQueryByIdIn()
	  {

		// empty list
		assertTrue(identityService.createUserQuery().userIdIn("a", "b").list().Empty);


		// collect all ids
		IList<User> list = identityService.createUserQuery().list();
		string[] ids = new string[list.Count];
		for (int i = 0; i < ids.Length; i++)
		{
		  ids[i] = list[i].Id;
		}

		IList<User> idInList = identityService.createUserQuery().userIdIn(ids).list();
		foreach (User user in idInList)
		{
		  bool found = false;
		  foreach (User otherUser in list)
		  {
			if (otherUser.Id.Equals(user.Id))
			{
			  found = true;
			  break;
			}
		  }
		  if (!found)
		  {
			fail("Expected to find user " + user);
		  }
		}
	  }

	  public virtual void testNativeQuery()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		// just test that the query will be constructed and executed, details are tested in the TaskQueryTest
		assertEquals(tablePrefix + "ACT_ID_USER", managementService.getTableName(typeof(UserEntity)));

		long userCount = identityService.createUserQuery().count();

		assertEquals(userCount, identityService.createNativeUserQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(UserEntity))).list().size());
		assertEquals(userCount, identityService.createNativeUserQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(UserEntity))).count());
	  }

	  public virtual void testNativeQueryOrLike()
	  {
		string searchPattern = "%frog";

		string fromWhereClauses = string.Format("FROM {0} WHERE FIRST_ LIKE #{{searchPattern}} OR LAST_ LIKE #{{searchPattern}} OR EMAIL_ LIKE #{{searchPattern}}", managementService.getTableName(typeof(UserEntity)));

		assertEquals(1, identityService.createNativeUserQuery().sql("SELECT * " + fromWhereClauses).parameter("searchPattern", searchPattern).list().size());
		assertEquals(1, identityService.createNativeUserQuery().sql("SELECT count(*) " + fromWhereClauses).parameter("searchPattern", searchPattern).count());
	  }

	  public virtual void testNativeQueryPaging()
	  {
		assertEquals(2, identityService.createNativeUserQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(UserEntity))).listPage(1, 2).size());
		assertEquals(1, identityService.createNativeUserQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(UserEntity))).listPage(2, 1).size());
	  }

	}

}