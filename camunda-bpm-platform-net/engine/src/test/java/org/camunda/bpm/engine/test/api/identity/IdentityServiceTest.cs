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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Picture = org.camunda.bpm.engine.identity.Picture;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using Account = org.camunda.bpm.engine.impl.identity.Account;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class IdentityServiceTest
	{

	  private readonly string INVALID_ID_MESSAGE = "%s has an invalid id: '%s' is not a valid resource identifier.";

	  private static readonly SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal IdentityService identityService;
	  protected internal ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		ClockUtil.CurrentTime = DateTime.Now;

		if (processEngine != null)
		{

		  foreach (User user in processEngine.IdentityService.createUserQuery().list())
		  {
			processEngine.IdentityService.deleteUser(user.Id);
		  }
		  foreach (Group group in processEngine.IdentityService.createGroupQuery().list())
		  {
			processEngine.IdentityService.deleteGroup(group.Id);
		  }
		  foreach (Tenant tenant in processEngine.IdentityService.createTenantQuery().list())
		  {
			processEngine.IdentityService.deleteTenant(tenant.Id);
		  }
		  foreach (Authorization authorization in processEngine.AuthorizationService.createAuthorizationQuery().list())
		  {
			processEngine.AuthorizationService.deleteAuthorization(authorization.Id);
		  }

		  processEngine.close();
		  ProcessEngines.unregister(processEngine);
		  processEngine = null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsReadOnly()
	  public virtual void testIsReadOnly()
	  {
		assertFalse(identityService.ReadOnly);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserInfo()
	  public virtual void testUserInfo()
	  {
		User user = identityService.newUser("testuser");
		identityService.saveUser(user);

		identityService.setUserInfo("testuser", "myinfo", "myvalue");
		assertEquals("myvalue", identityService.getUserInfo("testuser", "myinfo"));

		identityService.setUserInfo("testuser", "myinfo", "myvalue2");
		assertEquals("myvalue2", identityService.getUserInfo("testuser", "myinfo"));

		identityService.deleteUserInfo("testuser", "myinfo");
		assertNull(identityService.getUserInfo("testuser", "myinfo"));

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserAccount()
	  public virtual void testUserAccount()
	  {
		User user = identityService.newUser("testuser");
		identityService.saveUser(user);

		identityService.setUserAccount("testuser", "123", "google", "mygoogleusername", "mygooglepwd", null);
		Account googleAccount = identityService.getUserAccount("testuser", "123", "google");
		assertEquals("google", googleAccount.Name);
		assertEquals("mygoogleusername", googleAccount.Username);
		assertEquals("mygooglepwd", googleAccount.Password);

		identityService.setUserAccount("testuser", "123", "google", "mygoogleusername2", "mygooglepwd2", null);
		googleAccount = identityService.getUserAccount("testuser", "123", "google");
		assertEquals("google", googleAccount.Name);
		assertEquals("mygoogleusername2", googleAccount.Username);
		assertEquals("mygooglepwd2", googleAccount.Password);

		identityService.setUserAccount("testuser", "123", "alfresco", "myalfrescousername", "myalfrescopwd", null);
		identityService.setUserInfo("testuser", "myinfo", "myvalue");
		identityService.setUserInfo("testuser", "myinfo2", "myvalue2");

		IList<string> expectedUserAccountNames = new List<string>();
		expectedUserAccountNames.Add("google");
		expectedUserAccountNames.Add("alfresco");
		IList<string> userAccountNames = identityService.getUserAccountNames("testuser");
		assertListElementsMatch(expectedUserAccountNames, userAccountNames);

		identityService.deleteUserAccount("testuser", "google");

		expectedUserAccountNames.Remove("google");

		userAccountNames = identityService.getUserAccountNames("testuser");
		assertListElementsMatch(expectedUserAccountNames, userAccountNames);

		identityService.deleteUser(user.Id);
	  }

	  private void assertListElementsMatch(IList<string> list1, IList<string> list2)
	  {
		if (list1 != null)
		{
		  assertNotNull(list2);
		  assertEquals(list1.Count, list2.Count);
		  foreach (string value in list1)
		  {
			assertTrue(list2.Contains(value));
		  }
		}
		else
		{
		  assertNull(list2);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserAccountDetails()
	  public virtual void testUserAccountDetails()
	  {
		User user = identityService.newUser("testuser");
		identityService.saveUser(user);

		IDictionary<string, string> accountDetails = new Dictionary<string, string>();
		accountDetails["server"] = "localhost";
		accountDetails["port"] = "35";
		identityService.setUserAccount("testuser", "123", "google", "mygoogleusername", "mygooglepwd", accountDetails);
		Account googleAccount = identityService.getUserAccount("testuser", "123", "google");
		assertEquals(accountDetails, googleAccount.Details);

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateExistingUser()
	  public virtual void testCreateExistingUser()
	  {
		User user = identityService.newUser("testuser");
		identityService.saveUser(user);

		User secondUser = identityService.newUser("testuser");

		try
		{
		  identityService.saveUser(secondUser);
		  fail("BadUserRequestException is expected");
		}
		catch (Exception ex)
		{
		  if (!(ex is BadUserRequestException))
		  {
			fail("BadUserRequestException is expected, but another exception was received:  " + ex);
		  }
		  assertEquals("The user already exists", ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateUser()
	  public virtual void testUpdateUser()
	  {
		// First, create a new user
		User user = identityService.newUser("johndoe");
		user.FirstName = "John";
		user.LastName = "Doe";
		user.Email = "johndoe@alfresco.com";
		user.Password = "s3cret";
		identityService.saveUser(user);

		// Fetch and update the user
		user = identityService.createUserQuery().userId("johndoe").singleResult();
		user.Email = "updated@alfresco.com";
		user.FirstName = "Jane";
		user.LastName = "Donnel";
		identityService.saveUser(user);

		user = identityService.createUserQuery().userId("johndoe").singleResult();
		assertEquals("Jane", user.FirstName);
		assertEquals("Donnel", user.LastName);
		assertEquals("updated@alfresco.com", user.Email);
		assertTrue(identityService.checkPassword("johndoe", "s3cret"));

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserPicture()
	  public virtual void testUserPicture()
	  {
		// First, create a new user
		User user = identityService.newUser("johndoe");
		identityService.saveUser(user);
		string userId = user.Id;

		Picture picture = new Picture("niceface".GetBytes(), "image/string");
		identityService.setUserPicture(userId, picture);

		picture = identityService.getUserPicture(userId);

		// Fetch and update the user
		user = identityService.createUserQuery().userId("johndoe").singleResult();
		assertTrue("byte arrays differ", Arrays.Equals("niceface".GetBytes(), picture.Bytes));
		assertEquals("image/string", picture.MimeType);

		identityService.deleteUserPicture("johndoe");
		// this is ignored
		identityService.deleteUserPicture("someone-else-we-dont-know");

		// picture does not exist
		picture = identityService.getUserPicture("johndoe");
		assertNull(picture);

		// add new picture
		picture = new Picture("niceface".GetBytes(), "image/string");
		identityService.setUserPicture(userId, picture);

		// makes the picture go away
		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateExistingGroup()
	  public virtual void testCreateExistingGroup()
	  {
		Group group = identityService.newGroup("greatGroup");
		identityService.saveGroup(group);

		Group secondGroup = identityService.newGroup("greatGroup");

		try
		{
		  identityService.saveGroup(secondGroup);
		  fail("BadUserRequestException is expected");
		}
		catch (Exception ex)
		{
		  if (!(ex is BadUserRequestException))
		  {
			fail("BadUserRequestException is expected, but another exception was received:  " + ex);
		  }
		  assertEquals("The group already exists", ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateGroup()
	  public virtual void testUpdateGroup()
	  {
		Group group = identityService.newGroup("sales");
		group.Name = "Sales";
		identityService.saveGroup(group);

		group = identityService.createGroupQuery().groupId("sales").singleResult();
		group.Name = "Updated";
		identityService.saveGroup(group);

		group = identityService.createGroupQuery().groupId("sales").singleResult();
		assertEquals("Updated", group.Name);

		identityService.deleteGroup(group.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void findUserByUnexistingId()
	  public virtual void findUserByUnexistingId()
	  {
		User user = identityService.createUserQuery().userId("unexistinguser").singleResult();
		assertNull(user);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void findGroupByUnexistingId()
	  public virtual void findGroupByUnexistingId()
	  {
		Group group = identityService.createGroupQuery().groupId("unexistinggroup").singleResult();
		assertNull(group);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateMembershipUnexistingGroup()
	  public virtual void testCreateMembershipUnexistingGroup()
	  {
		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);

		thrown.expect(typeof(ProcessEngineException));

		identityService.createMembership(johndoe.Id, "unexistinggroup");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateMembershipUnexistingUser()
	  public virtual void testCreateMembershipUnexistingUser()
	  {
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);

		thrown.expect(typeof(ProcessEngineException));

		identityService.createMembership("unexistinguser", sales.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateMembershipAlreadyExisting()
	  public virtual void testCreateMembershipAlreadyExisting()
	  {
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);
		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);

		// Create the membership
		identityService.createMembership(johndoe.Id, sales.Id);

		thrown.expect(typeof(ProcessEngineException));

		identityService.createMembership(johndoe.Id, sales.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveGroupNullArgument()
	  public virtual void testSaveGroupNullArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("group is null");

		identityService.saveGroup(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveUserNullArgument()
	  public virtual void testSaveUserNullArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("user is null");

		identityService.saveUser(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindGroupByIdNullArgument()
	  public virtual void testFindGroupByIdNullArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("id is null");

		identityService.createGroupQuery().groupId(null).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateMembershipNullUserArgument()
	  public virtual void testCreateMembershipNullUserArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("userId is null");

		identityService.createMembership(null, "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateMembershipNullGroupArgument()
	  public virtual void testCreateMembershipNullGroupArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("groupId is null");

		identityService.createMembership("userId", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindGroupsByUserIdNullArguments()
	  public virtual void testFindGroupsByUserIdNullArguments()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("userId is null");

		identityService.createGroupQuery().groupMember(null).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindUsersByGroupUnexistingGroup()
	  public virtual void testFindUsersByGroupUnexistingGroup()
	  {
		IList<User> users = identityService.createUserQuery().memberOfGroup("unexistinggroup").list();
		assertNotNull(users);
		assertTrue(users.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroupNullArguments()
	  public virtual void testDeleteGroupNullArguments()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("groupId is null");

		identityService.deleteGroup(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMembership()
	  public virtual void testDeleteMembership()
	  {
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);

		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);
		// Add membership
		identityService.createMembership(johndoe.Id, sales.Id);

		IList<Group> groups = identityService.createGroupQuery().groupMember(johndoe.Id).list();
		assertTrue(groups.Count == 1);
		assertEquals("sales", groups[0].Id);

		// Delete the membership and check members of sales group
		identityService.deleteMembership(johndoe.Id, sales.Id);
		groups = identityService.createGroupQuery().groupMember(johndoe.Id).list();
		assertTrue(groups.Count == 0);

		identityService.deleteGroup("sales");
		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMembershipWhenUserIsNoMember()
	  public virtual void testDeleteMembershipWhenUserIsNoMember()
	  {
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);

		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);

		// Delete the membership when the user is no member
		identityService.deleteMembership(johndoe.Id, sales.Id);

		identityService.deleteGroup("sales");
		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMembershipUnexistingGroup()
	  public virtual void testDeleteMembershipUnexistingGroup()
	  {
		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);
		// No exception should be thrown when group doesn't exist
		identityService.deleteMembership(johndoe.Id, "unexistinggroup");
		identityService.deleteUser(johndoe.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMembershipUnexistingUser()
	  public virtual void testDeleteMembershipUnexistingUser()
	  {
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);
		// No exception should be thrown when user doesn't exist
		identityService.deleteMembership("unexistinguser", sales.Id);
		identityService.deleteGroup(sales.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMemberschipNullUserArgument()
	  public virtual void testDeleteMemberschipNullUserArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("userId is null");

		identityService.deleteMembership(null, "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMemberschipNullGroupArgument()
	  public virtual void testDeleteMemberschipNullGroupArgument()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("groupId is null");

		identityService.deleteMembership("user", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteUserNullArguments()
	  public virtual void testDeleteUserNullArguments()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("userId is null");

		identityService.deleteUser(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteUserUnexistingUserId()
	  public virtual void testDeleteUserUnexistingUserId()
	  {
		// No exception should be thrown. Deleting an unexisting user should
		// be ignored silently
		identityService.deleteUser("unexistinguser");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCheckPassword()
	  public virtual void testCheckPassword()
	  {

		// store user with password
		User user = identityService.newUser("secureUser");
		user.Password = "s3cret";
		identityService.saveUser(user);

		assertTrue(identityService.checkPassword(user.Id, "s3cret"));
		assertFalse(identityService.checkPassword(user.Id, "wrong"));

		identityService.deleteUser(user.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdatePassword()
	  public virtual void testUpdatePassword()
	  {

		// store user with password
		User user = identityService.newUser("secureUser");
		user.Password = "s3cret";
		identityService.saveUser(user);

		assertTrue(identityService.checkPassword(user.Id, "s3cret"));

		user.Password = "new-password";
		identityService.saveUser(user);

		assertTrue(identityService.checkPassword(user.Id, "new-password"));

		identityService.deleteUser(user.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCheckPasswordNullSafe()
	  public virtual void testCheckPasswordNullSafe()
	  {
		assertFalse(identityService.checkPassword("userId", null));
		assertFalse(identityService.checkPassword(null, "passwd"));
		assertFalse(identityService.checkPassword(null, null));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserOptimisticLockingException()
	  public virtual void testUserOptimisticLockingException()
	  {
		User user = identityService.newUser("kermit");
		identityService.saveUser(user);

		User user1 = identityService.createUserQuery().singleResult();
		User user2 = identityService.createUserQuery().singleResult();

		user1.FirstName = "name one";
		identityService.saveUser(user1);

		thrown.expect(typeof(OptimisticLockingException));

		user2.FirstName = "name two";
		identityService.saveUser(user2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupOptimisticLockingException()
	  public virtual void testGroupOptimisticLockingException()
	  {
		Group group = identityService.newGroup("group");
		identityService.saveGroup(group);

		Group group1 = identityService.createGroupQuery().singleResult();
		Group group2 = identityService.createGroupQuery().singleResult();

		group1.Name = "name one";
		identityService.saveGroup(group1);

		thrown.expect(typeof(OptimisticLockingException));

		group2.Name = "name two";
		identityService.saveGroup(group2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveUserWithGenericResourceId()
	  public virtual void testSaveUserWithGenericResourceId()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/generic.resource.id.whitelist.camunda.cfg.xml").buildProcessEngine();

		User user = processEngine.IdentityService.newUser("*");

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("has an invalid id: id cannot be *. * is a reserved identifier.");

		processEngine.IdentityService.saveUser(user);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveGroupWithGenericResourceId()
	  public virtual void testSaveGroupWithGenericResourceId()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/generic.resource.id.whitelist.camunda.cfg.xml").buildProcessEngine();

		Group group = processEngine.IdentityService.newGroup("*");

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("has an invalid id: id cannot be *. * is a reserved identifier.");

		processEngine.IdentityService.saveGroup(group);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAuthenticatedIdToGenericId()
	  public virtual void testSetAuthenticatedIdToGenericId()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Invalid user id provided: id cannot be *. * is a reserved identifier.");

		identityService.AuthenticatedUserId = "*";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAuthenticationUserIdToGenericId()
	  public virtual void testSetAuthenticationUserIdToGenericId()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("invalid group id provided: id cannot be *. * is a reserved identifier.");

		identityService.setAuthentication("aUserId", Arrays.asList("*"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAuthenticatedTenantIdToGenericId()
	  public virtual void testSetAuthenticatedTenantIdToGenericId()
	  {
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("invalid tenant id provided: id cannot be *. * is a reserved identifier.");

		identityService.setAuthentication(null, null, Arrays.asList("*"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAuthenticatedUserId()
	  public virtual void testSetAuthenticatedUserId()
	  {
		identityService.AuthenticatedUserId = "john";

		Authentication currentAuthentication = identityService.CurrentAuthentication;

		assertNotNull(currentAuthentication);
		assertEquals("john", currentAuthentication.UserId);
		assertNull(currentAuthentication.GroupIds);
		assertNull(currentAuthentication.TenantIds);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAuthenticatedUserAndGroups()
	  public virtual void testSetAuthenticatedUserAndGroups()
	  {
		IList<string> groups = Arrays.asList("sales", "development");

		identityService.setAuthentication("john", groups);

		Authentication currentAuthentication = identityService.CurrentAuthentication;

		assertNotNull(currentAuthentication);
		assertEquals("john", currentAuthentication.UserId);
		assertEquals(groups, currentAuthentication.GroupIds);
		assertNull(currentAuthentication.TenantIds);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAuthenticatedUserGroupsAndTenants()
	  public virtual void testSetAuthenticatedUserGroupsAndTenants()
	  {
		IList<string> groups = Arrays.asList("sales", "development");
		IList<string> tenants = Arrays.asList("tenant1", "tenant2");

		identityService.setAuthentication("john", groups, tenants);

		Authentication currentAuthentication = identityService.CurrentAuthentication;

		assertNotNull(currentAuthentication);
		assertEquals("john", currentAuthentication.UserId);
		assertEquals(groups, currentAuthentication.GroupIds);
		assertEquals(tenants, currentAuthentication.TenantIds);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthentication()
	  public virtual void testAuthentication()
	  {
		User user = identityService.newUser("johndoe");
		user.Password = "xxx";
		identityService.saveUser(user);

		assertTrue(identityService.checkPassword("johndoe", "xxx"));
		assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));

		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUsuccessfulAttemptsResultInException() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testUsuccessfulAttemptsResultInException()
	  {
		User user = identityService.newUser("johndoe");
		user.Password = "xxx";
		identityService.saveUser(user);

		thrown.expect(typeof(AuthenticationException));
		thrown.expectMessage("The user with id 'johndoe' is permanently locked. Please contact your admin to unlock the account.");

		DateTime now = sdf.parse("2000-01-24T13:00:00");
		ClockUtil.CurrentTime = now;
		for (int i = 0; i <= 11; i++)
		{
		  assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));
		  now = DateUtils.addMinutes(now, 1);
		  ClockUtil.CurrentTime = now;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulLoginAfterFailureAndDelay()
	  public virtual void testSuccessfulLoginAfterFailureAndDelay()
	  {
		User user = identityService.newUser("johndoe");
		user.Password = "xxx";
		identityService.saveUser(user);

		DateTime now = null;
		now = ClockUtil.CurrentTime;
		assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));
		ClockUtil.CurrentTime = DateUtils.addSeconds(now, 30);
		assertTrue(identityService.checkPassword("johndoe", "xxx"));

		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulLoginAfterFailureWithoutDelay()
	  public virtual void testSuccessfulLoginAfterFailureWithoutDelay()
	  {
		User user = identityService.newUser("johndoe");
		user.Password = "xxx";
		identityService.saveUser(user);

		DateTime now = ClockUtil.CurrentTime;
		assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));
		try
		{
		assertFalse(identityService.checkPassword("johndoe", "xxx"));
		fail("expected exception");
		}
		catch (AuthenticationException e)
		{
		  assertTrue(e.Message.contains("The user with id 'johndoe' is locked."));
		}
		ClockUtil.CurrentTime = DateUtils.addSeconds(now, 30);
		assertTrue(identityService.checkPassword("johndoe", "xxx"));

		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnsuccessfulLoginAfterFailureWithoutDelay()
	  public virtual void testUnsuccessfulLoginAfterFailureWithoutDelay()
	  {
		User user = identityService.newUser("johndoe");
		user.Password = "xxx";
		identityService.saveUser(user);

		DateTime now = null;
		now = ClockUtil.CurrentTime;
		assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));


		// try again before exprTime
		ClockUtil.CurrentTime = DateUtils.addSeconds(now, 1);
		try
		{
		  assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));
		  fail("expected exception");
		}
		catch (AuthenticationException e)
		{
		  DateTime expectedLockExpitation = DateUtils.addSeconds(now, 3);
		  assertTrue(e.Message.contains("The lock will expire at " + expectedLockExpitation));
		}

		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFindGroupsByUserAndType()
	  public virtual void testFindGroupsByUserAndType()
	  {
		Group sales = identityService.newGroup("sales");
		sales.Type = "hierarchy";
		identityService.saveGroup(sales);

		Group development = identityService.newGroup("development");
		development.Type = "hierarchy";
		identityService.saveGroup(development);

		Group admin = identityService.newGroup("admin");
		admin.Type = "security-role";
		identityService.saveGroup(admin);

		Group user = identityService.newGroup("user");
		user.Type = "security-role";
		identityService.saveGroup(user);

		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);

		User joesmoe = identityService.newUser("joesmoe");
		identityService.saveUser(joesmoe);

		User jackblack = identityService.newUser("jackblack");
		identityService.saveUser(jackblack);

		identityService.createMembership("johndoe", "sales");
		identityService.createMembership("johndoe", "user");
		identityService.createMembership("johndoe", "admin");

		identityService.createMembership("joesmoe", "user");

		IList<Group> groups = identityService.createGroupQuery().groupMember("johndoe").groupType("security-role").list();
		ISet<string> groupIds = getGroupIds(groups);
		ISet<string> expectedGroupIds = new HashSet<string>();
		expectedGroupIds.Add("user");
		expectedGroupIds.Add("admin");
		assertEquals(expectedGroupIds, groupIds);

		groups = identityService.createGroupQuery().groupMember("joesmoe").groupType("security-role").list();
		groupIds = getGroupIds(groups);
		expectedGroupIds = new HashSet<string>();
		expectedGroupIds.Add("user");
		assertEquals(expectedGroupIds, groupIds);

		groups = identityService.createGroupQuery().groupMember("jackblack").groupType("security-role").list();
		assertTrue(groups.Count == 0);

		identityService.deleteGroup("sales");
		identityService.deleteGroup("development");
		identityService.deleteGroup("admin");
		identityService.deleteGroup("user");
		identityService.deleteUser("johndoe");
		identityService.deleteUser("joesmoe");
		identityService.deleteUser("jackblack");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUser()
	  public virtual void testUser()
	  {
		User user = identityService.newUser("johndoe");
		user.FirstName = "John";
		user.LastName = "Doe";
		user.Email = "johndoe@alfresco.com";
		identityService.saveUser(user);

		user = identityService.createUserQuery().userId("johndoe").singleResult();
		assertEquals("johndoe", user.Id);
		assertEquals("John", user.FirstName);
		assertEquals("Doe", user.LastName);
		assertEquals("johndoe@alfresco.com", user.Email);

		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroup()
	  public virtual void testGroup()
	  {
		Group group = identityService.newGroup("sales");
		group.Name = "Sales division";
		identityService.saveGroup(group);

		group = identityService.createGroupQuery().groupId("sales").singleResult();
		assertEquals("sales", group.Id);
		assertEquals("Sales division", group.Name);

		identityService.deleteGroup("sales");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMembership()
	  public virtual void testMembership()
	  {
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);

		Group development = identityService.newGroup("development");
		identityService.saveGroup(development);

		User johndoe = identityService.newUser("johndoe");
		identityService.saveUser(johndoe);

		User joesmoe = identityService.newUser("joesmoe");
		identityService.saveUser(joesmoe);

		User jackblack = identityService.newUser("jackblack");
		identityService.saveUser(jackblack);

		identityService.createMembership("johndoe", "sales");
		identityService.createMembership("joesmoe", "sales");

		identityService.createMembership("joesmoe", "development");
		identityService.createMembership("jackblack", "development");

		IList<Group> groups = identityService.createGroupQuery().groupMember("johndoe").list();
		assertEquals(createStringSet("sales"), getGroupIds(groups));

		groups = identityService.createGroupQuery().groupMember("joesmoe").list();
		assertEquals(createStringSet("sales", "development"), getGroupIds(groups));

		groups = identityService.createGroupQuery().groupMember("jackblack").list();
		assertEquals(createStringSet("development"), getGroupIds(groups));

		IList<User> users = identityService.createUserQuery().memberOfGroup("sales").list();
		assertEquals(createStringSet("johndoe", "joesmoe"), getUserIds(users));

		users = identityService.createUserQuery().memberOfGroup("development").list();
		assertEquals(createStringSet("joesmoe", "jackblack"), getUserIds(users));

		identityService.deleteGroup("sales");
		identityService.deleteGroup("development");

		identityService.deleteUser("jackblack");
		identityService.deleteUser("joesmoe");
		identityService.deleteUser("johndoe");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidUserId()
	  public virtual void testInvalidUserId()
	  {
		string invalidId = "john doe";
		try
		{
		  identityService.newUser(invalidId);
		  fail("Invalid user id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "User", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidUserIdOnSave()
	  public virtual void testInvalidUserIdOnSave()
	  {
		string invalidId = "john doe";
		try
		{
		  User updatedUser = identityService.newUser("john");
		  updatedUser.Id = invalidId;
		  identityService.saveUser(updatedUser);

		  fail("Invalid user id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "User", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidGroupId()
	  public virtual void testInvalidGroupId()
	  {
		string invalidId = "john's group";
		try
		{
		  identityService.newGroup(invalidId);
		  fail("Invalid group id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Group", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidGroupIdOnSave()
	  public virtual void testInvalidGroupIdOnSave()
	  {
		string invalidId = "john's group";
		try
		{
		  Group updatedGroup = identityService.newGroup("group");
		  updatedGroup.Id = invalidId;
		  identityService.saveGroup(updatedGroup);

		  fail("Invalid group id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Group", invalidId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaAdminId()
	  public virtual void testCamundaAdminId()
	  {
		string camundaAdminID = "camunda-admin";
		try
		{
		  identityService.newUser(camundaAdminID);
		  identityService.newGroup(camundaAdminID);
		  identityService.newTenant(camundaAdminID);
		}
		catch (ProcessEngineException)
		{
		  fail(camundaAdminID + " should be a valid id.");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomResourceWhitelist()
	  public virtual void testCustomResourceWhitelist()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/custom.whitelist.camunda.cfg.xml").buildProcessEngine();
		string invalidUserId = "johnDoe";
		string invalidGroupId = "johnsGroup";
		string invalidTenantId = "johnsTenant";

		try
		{
		  processEngine.IdentityService.newUser(invalidUserId);
		  fail("Invalid user id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "User", invalidUserId), ex.Message);
		}

		try
		{
		  processEngine.IdentityService.newGroup("johnsGroup");
		  fail("Invalid group id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Group", invalidGroupId), ex.Message);
		}

		try
		{
		  processEngine.IdentityService.newTenant(invalidTenantId);
		  fail("Invalid tenant id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Tenant", invalidTenantId), ex.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSeparateResourceWhitelistPatterns()
	  public virtual void testSeparateResourceWhitelistPatterns()
	  {
		processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/api/identity/custom.resource.whitelist.camunda.cfg.xml").buildProcessEngine();

		string invalidUserId = "12345";
		string invalidGroupId = "johnsGroup";
		string invalidTenantId = "!@##$%";

		// pattern: [a-zA-Z]+
		try
		{
		  processEngine.IdentityService.newUser(invalidUserId);
		  fail("Invalid user id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "User", invalidUserId), ex.Message);
		}

		// pattern: \d+
		try
		{
		  processEngine.IdentityService.newGroup(invalidGroupId);
		  fail("Invalid group id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Group", invalidGroupId), ex.Message);
		}

		// new general pattern (used for tenant whitelisting): [a-zA-Z0-9]+
		try
		{
		  processEngine.IdentityService.newTenant(invalidTenantId);
		  fail("Invalid tenant id exception expected!");
		}
		catch (ProcessEngineException ex)
		{
		  assertEquals(string.format(INVALID_ID_MESSAGE, "Tenant", invalidTenantId), ex.Message);
		}
	  }

	  private object createStringSet(params string[] strings)
	  {
		ISet<string> stringSet = new HashSet<string>();
		foreach (string @string in strings)
		{
		  stringSet.Add(@string);
		}
		return stringSet;
	  }

	  protected internal virtual ISet<string> getGroupIds(IList<Group> groups)
	  {
		ISet<string> groupIds = new HashSet<string>();
		foreach (Group group in groups)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }

	  protected internal virtual ISet<string> getUserIds(IList<User> users)
	  {
		ISet<string> userIds = new HashSet<string>();
		foreach (User user in users)
		{
		  userIds.Add(user.Id);
		}
		return userIds;
	  }

	}

}