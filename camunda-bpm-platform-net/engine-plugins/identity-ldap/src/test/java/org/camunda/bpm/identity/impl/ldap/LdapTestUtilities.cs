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
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNotSame;
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;

	/// <summary>
	/// Contains some test utilities to test the Ldap plugin.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public sealed class LdapTestUtilities
	{

	  public static void checkPagingResults(ISet<string> results, string result1, string result2)
	  {
		assertNotSame(result1, result2);
		assertFalse(results.Contains(result1));
		results.Add(result1);
		assertFalse(results.Contains(result2));
		results.Add(result2);
	  }

	  public static void testGroupPaging(IdentityService identityService)
	  {
		ISet<string> groupNames = new HashSet<string>();
		IList<Group> groups = identityService.createGroupQuery().listPage(0, 2);
		assertEquals(2, groups.Count);
		checkPagingResults(groupNames, groups[0].Id, groups[1].Id);

		groups = identityService.createGroupQuery().listPage(2, 2);
		assertEquals(2, groups.Count);
		checkPagingResults(groupNames, groups[0].Id, groups[1].Id);

		groups = identityService.createGroupQuery().listPage(4, 2);
		assertEquals(2, groups.Count);
		assertFalse(groupNames.Contains(groups[0].Id));
		groupNames.Add(groups[0].Id);

		groups = identityService.createGroupQuery().listPage(6, 2);
		assertEquals(0, groups.Count);
	  }

	  public static void testUserPaging(IdentityService identityService)
	  {
		ISet<string> userNames = new HashSet<string>();
		IList<User> users = identityService.createUserQuery().listPage(0, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().listPage(2, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().listPage(4, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().listPage(6, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().listPage(12, 2);
		assertEquals(0, users.Count);
	  }

	  public static void testUserPagingWithMemberOfGroup(IdentityService identityService)
	  {
		ISet<string> userNames = new HashSet<string>();
		IList<User> users = identityService.createUserQuery().memberOfGroup("all").listPage(0, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().memberOfGroup("all").listPage(2, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().memberOfGroup("all").listPage(4, 2);
		assertEquals(2, users.Count);
		checkPagingResults(userNames, users[0].Id, users[1].Id);

		users = identityService.createUserQuery().memberOfGroup("all").listPage(11, 2);
		assertEquals(1, users.Count);
		assertFalse(userNames.Contains(users[0].Id));

		users = identityService.createUserQuery().memberOfGroup("all").listPage(12, 2);
		assertEquals(0, users.Count);
	  }
	}

}