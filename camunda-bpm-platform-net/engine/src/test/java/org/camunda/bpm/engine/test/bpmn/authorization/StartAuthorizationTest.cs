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
namespace org.camunda.bpm.engine.test.bpmn.authorization
{

	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;


	/// <summary>
	/// @author Saeid Mirzaei
	/// @author Tijs Rademakers
	/// </summary>
	public class StartAuthorizationTest : PluggableProcessEngineTestCase
	{

	  internal new IdentityService identityService;

	  internal User userInGroup1;
	  internal User userInGroup2;
	  internal User userInGroup3;

	  internal Group group1;
	  internal Group group2;
	  internal Group group3;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUpUsersAndGroups() throws Exception
	  protected internal virtual void setUpUsersAndGroups()
	  {

		identityService = processEngine.IdentityService;

		identityService.saveUser(identityService.newUser("user1"));
		identityService.saveUser(identityService.newUser("user2"));
		identityService.saveUser(identityService.newUser("user3"));

		// create users
		userInGroup1 = identityService.newUser("userInGroup1");
		identityService.saveUser(userInGroup1);

		userInGroup2 = identityService.newUser("userInGroup2");
		identityService.saveUser(userInGroup2);

		userInGroup3 = identityService.newUser("userInGroup3");
		identityService.saveUser(userInGroup3);

		// create groups
		group1 = identityService.newGroup("group1");
		identityService.saveGroup(group1);

		group2 = identityService.newGroup("group2");
		identityService.saveGroup(group2);

		group3 = identityService.newGroup("group3");
		identityService.saveGroup(group3);

		// relate users to groups
		identityService.createMembership(userInGroup1.Id, group1.Id);
		identityService.createMembership(userInGroup2.Id, group2.Id);
		identityService.createMembership(userInGroup3.Id, group3.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDownUsersAndGroups() throws Exception
	  protected internal virtual void tearDownUsersAndGroups()
	  {
		identityService.deleteMembership(userInGroup1.Id, group1.Id);
		identityService.deleteMembership(userInGroup2.Id, group2.Id);
		identityService.deleteMembership(userInGroup3.Id, group3.Id);

		identityService.deleteGroup(group1.Id);
		identityService.deleteGroup(group2.Id);
		identityService.deleteGroup(group3.Id);

		identityService.deleteUser(userInGroup1.Id);
		identityService.deleteUser(userInGroup2.Id);
		identityService.deleteUser(userInGroup3.Id);

		identityService.deleteUser("user1");
		identityService.deleteUser("user2");
		identityService.deleteUser("user3");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIdentityLinks() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testIdentityLinks()
	  {

		setUpUsersAndGroups();

		try
		{
		  ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult();
		  assertNotNull(latestProcessDef);
		  IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(0, links.Count);

		  latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").singleResult();
		  assertNotNull(latestProcessDef);
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(2, links.Count);
		  assertEquals(true, containsUserOrGroup("user1", null, links));
		  assertEquals(true, containsUserOrGroup("user2", null, links));

		  latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process3").singleResult();
		  assertNotNull(latestProcessDef);
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(1, links.Count);
		  assertEquals("user1", links[0].UserId);

		  latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process4").singleResult();
		  assertNotNull(latestProcessDef);
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(4, links.Count);
		  assertEquals(true, containsUserOrGroup("userInGroup2", null, links));
		  assertEquals(true, containsUserOrGroup(null, "group1", links));
		  assertEquals(true, containsUserOrGroup(null, "group2", links));
		  assertEquals(true, containsUserOrGroup(null, "group3", links));

		}
		finally
		{
		  tearDownUsersAndGroups();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAddAndRemoveIdentityLinks() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testAddAndRemoveIdentityLinks()
	  {

		setUpUsersAndGroups();

		try
		{
		  ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("potentialStarterNoDefinition").singleResult();
		  assertNotNull(latestProcessDef);
		  IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(0, links.Count);

		  repositoryService.addCandidateStarterGroup(latestProcessDef.Id, "group1");
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(1, links.Count);
		  assertEquals("group1", links[0].GroupId);

		  repositoryService.addCandidateStarterUser(latestProcessDef.Id, "user1");
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(2, links.Count);
		  assertEquals(true, containsUserOrGroup(null, "group1", links));
		  assertEquals(true, containsUserOrGroup("user1", null, links));

		  repositoryService.deleteCandidateStarterGroup(latestProcessDef.Id, "nonexisting");
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(2, links.Count);

		  repositoryService.deleteCandidateStarterGroup(latestProcessDef.Id, "group1");
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(1, links.Count);
		  assertEquals("user1", links[0].UserId);

		  repositoryService.deleteCandidateStarterUser(latestProcessDef.Id, "user1");
		  links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		  assertEquals(0, links.Count);

		}
		finally
		{
		  tearDownUsersAndGroups();
		}
	  }

	  private bool containsUserOrGroup(string userId, string groupId, IList<IdentityLink> links)
	  {
		bool found = false;
		foreach (IdentityLink identityLink in links)
		{
		  if (!string.ReferenceEquals(userId, null) && userId.Equals(identityLink.UserId))
		  {
			found = true;
			break;
		  }
		  else if (!string.ReferenceEquals(groupId, null) && groupId.Equals(identityLink.GroupId))
		  {
			found = true;
			break;
		  }
		}
		return found;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPotentialStarter() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPotentialStarter()
	  {
		// first check an unauthorized user. An exception is expected

		setUpUsersAndGroups();

		try
		{

			// Authentication should not be done. So an unidentified user should also be able to start the process
			identityService.AuthenticatedUserId = "unauthorizedUser";
			try
			{
			  runtimeService.startProcessInstanceByKey("potentialStarter");

			}
			catch (Exception e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  fail("No StartAuthorizationException expected, " + e.GetType().FullName + " caught.");
			}

			// check with an authorized user obviously it should be no problem starting the process
			identityService.AuthenticatedUserId = "user1";
			ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("potentialStarter");
			assertProcessEnded(processInstance.Id);
			assertTrue(processInstance.Ended);
		}
		finally
		{

		  tearDownUsersAndGroups();
		}
	  }

	  /*
	   * if there is no security definition, then user authorization check is not
	   * done. This ensures backward compatibility
	   */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPotentialStarterNoDefinition() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPotentialStarterNoDefinition()
	  {
		identityService = processEngine.IdentityService;

		identityService.AuthenticatedUserId = "someOneFromMars";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("potentialStarterNoDefinition");
		assertNotNull(processInstance.Id);
		assertProcessEnded(processInstance.Id);
		assertTrue(processInstance.Ended);
	  }

	  // this test checks the list without user constraint
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessDefinitionList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessDefinitionList()
	  {

		setUpUsersAndGroups();
		try
		{

		  // Process 1 has no potential starters
		  ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process1").singleResult();
		  IList<User> authorizedUsers = identityService.createUserQuery().potentialStarter(latestProcessDef.Id).list();
		  assertEquals(0, authorizedUsers.Count);

		  // user1 and user2 are potential Startes of Process2
		  latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").singleResult();
		  authorizedUsers = identityService.createUserQuery().potentialStarter(latestProcessDef.Id).orderByUserId().asc().list();
		  assertEquals(2, authorizedUsers.Count);
		  assertEquals("user1", authorizedUsers[0].Id);
		  assertEquals("user2", authorizedUsers[1].Id);

		  // Process 2 has no potential starter groups
		  latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process2").singleResult();
		  IList<Group> authorizedGroups = identityService.createGroupQuery().potentialStarter(latestProcessDef.Id).list();
		  assertEquals(0, authorizedGroups.Count);

		  // Process 3 has 3 groups as authorized starter groups
		  latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process4").singleResult();
		  authorizedGroups = identityService.createGroupQuery().potentialStarter(latestProcessDef.Id).orderByGroupId().asc().list();
		  assertEquals(3, authorizedGroups.Count);
		  assertEquals("group1", authorizedGroups[0].Id);
		  assertEquals("group2", authorizedGroups[1].Id);
		  assertEquals("group3", authorizedGroups[2].Id);

		  // do not mention user, all processes should be selected
		  IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionName().asc().list();

		  assertEquals(4, processDefinitions.Count);

		  assertEquals("process1", processDefinitions[0].Key);
		  assertEquals("process2", processDefinitions[1].Key);
		  assertEquals("process3", processDefinitions[2].Key);
		  assertEquals("process4", processDefinitions[3].Key);

		  // check user1, process3 has "user1" as only authorized starter, and
		  // process2 has two authorized starters, of which one is "user1"
		  processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionName().asc().startableByUser("user1").list();

		  assertEquals(2, processDefinitions.Count);
		  assertEquals("process2", processDefinitions[0].Key);
		  assertEquals("process3", processDefinitions[1].Key);


		  // "user2" can only start process2
		  processDefinitions = repositoryService.createProcessDefinitionQuery().startableByUser("user2").list();

		  assertEquals(1, processDefinitions.Count);
		  assertEquals("process2", processDefinitions[0].Key);

		  // no process could be started with "user4"
		  processDefinitions = repositoryService.createProcessDefinitionQuery().startableByUser("user4").list();
		  assertEquals(0, processDefinitions.Count);

		  // "userInGroup3" is in "group3" and can start only process4 via group authorization
		  processDefinitions = repositoryService.createProcessDefinitionQuery().startableByUser("userInGroup3").list();
		  assertEquals(1, processDefinitions.Count);
		  assertEquals("process4", processDefinitions[0].Key);

		  // "userInGroup2" can start process4, via both user and group authorizations
		  // but we have to be sure that process4 appears only once
		  processDefinitions = repositoryService.createProcessDefinitionQuery().startableByUser("userInGroup2").list();
		  assertEquals(1, processDefinitions.Count);
		  assertEquals("process4", processDefinitions[0].Key);

		}
		finally
		{
		  tearDownUsersAndGroups();
		}
	  }

	}

}