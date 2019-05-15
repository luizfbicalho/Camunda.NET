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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ResourceAuthorizationProviderTest : ResourceProcessEngineTestCase
	{

	  protected internal string userId = "test";
	  protected internal string groupId = "accounting";
	  protected internal User user;
	  protected internal Group group;

	  public ResourceAuthorizationProviderTest() : base("org/camunda/bpm/engine/test/api/authorization/resource.authorization.provider.camunda.cfg.xml")
	  {
	  }

	  protected internal override void initializeProcessEngine()
	  {
		base.initializeProcessEngine();

		processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		processEngineConfiguration.ResourceAuthorizationProvider = new MyResourceAuthorizationProvider();

		identityService = processEngineConfiguration.IdentityService;
		authorizationService = processEngineConfiguration.AuthorizationService;

		user = createUser(userId);
		group = createGroup(groupId);

		identityService.createMembership(userId, groupId);

		identityService.setAuthentication(userId, Arrays.asList(groupId));
		processEngineConfiguration.AuthorizationEnabled = true;
	  }

	  public virtual void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	  public virtual void testNewTaskAssignee()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);

		// when (1)
		taskService.setAssignee(taskId, "demo");

		// then (1)
		assertNull(MyResourceAuthorizationProvider.OLD_ASSIGNEE);
		assertEquals("demo", MyResourceAuthorizationProvider.NEW_ASSIGNEE);

		MyResourceAuthorizationProvider.clearProperties();

		// when (2)
		taskService.setAssignee(taskId, userId);

		// then (2)
		assertEquals("demo", MyResourceAuthorizationProvider.OLD_ASSIGNEE);
		assertEquals(userId, MyResourceAuthorizationProvider.NEW_ASSIGNEE);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testNewTaskOwner()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);

		// when (1)
		taskService.setOwner(taskId, "demo");

		// then (1)
		assertNull(MyResourceAuthorizationProvider.OLD_OWNER);
		assertEquals("demo", MyResourceAuthorizationProvider.NEW_OWNER);

		MyResourceAuthorizationProvider.clearProperties();

		// when (2)
		taskService.setOwner(taskId, userId);

		// then (2)
		assertEquals("demo", MyResourceAuthorizationProvider.OLD_OWNER);
		assertEquals(userId, MyResourceAuthorizationProvider.NEW_OWNER);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testAddCandidateUser()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		assertEquals(IdentityLinkType.CANDIDATE, MyResourceAuthorizationProvider.ADD_USER_IDENTITY_LINK_TYPE);
		assertEquals("demo", MyResourceAuthorizationProvider.ADD_USER_IDENTITY_LINK_USER);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testAddUserIdentityLink()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);

		// when
		taskService.addUserIdentityLink(taskId, "demo", "myIdentityLink");

		// then
		assertEquals("myIdentityLink", MyResourceAuthorizationProvider.ADD_USER_IDENTITY_LINK_TYPE);
		assertEquals("demo", MyResourceAuthorizationProvider.ADD_USER_IDENTITY_LINK_USER);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testAddCandidateGroup()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);

		// when
		taskService.addCandidateGroup(taskId, "management");

		// then
		assertEquals(IdentityLinkType.CANDIDATE, MyResourceAuthorizationProvider.ADD_GROUP_IDENTITY_LINK_TYPE);
		assertEquals("management", MyResourceAuthorizationProvider.ADD_GROUP_IDENTITY_LINK_GROUP);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testAddGroupIdentityLink()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);

		// when
		taskService.addGroupIdentityLink(taskId, "management", "myIdentityLink");

		// then
		assertEquals("myIdentityLink", MyResourceAuthorizationProvider.ADD_GROUP_IDENTITY_LINK_TYPE);
		assertEquals("management", MyResourceAuthorizationProvider.ADD_GROUP_IDENTITY_LINK_GROUP);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testDeleteUserIdentityLink()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);
		taskService.addCandidateUser(taskId, "demo");

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		assertEquals(IdentityLinkType.CANDIDATE, MyResourceAuthorizationProvider.DELETE_USER_IDENTITY_LINK_TYPE);
		assertEquals("demo", MyResourceAuthorizationProvider.DELETE_USER_IDENTITY_LINK_USER);

		taskService.deleteTask(taskId, true);
	  }

	  public virtual void testDeleteGroupIdentityLink()
	  {
		// given
		MyResourceAuthorizationProvider.clearProperties();

		createGrantAuthorization(TASK, ANY, ALL, userId);

		string taskId = "myTask";
		Task newTask = taskService.newTask(taskId);
		taskService.saveTask(newTask);
		taskService.addCandidateGroup(taskId, "management");

		// when
		taskService.deleteCandidateGroup(taskId, "management");

		// then
		assertEquals(IdentityLinkType.CANDIDATE, MyResourceAuthorizationProvider.DELETE_GROUP_IDENTITY_LINK_TYPE);
		assertEquals("management", MyResourceAuthorizationProvider.DELETE_GROUP_IDENTITY_LINK_GROUP);

		taskService.deleteTask(taskId, true);
	  }

	  // user ////////////////////////////////////////////////////////////////

	  protected internal virtual User createUser(string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);

		// give user all permission to manipulate authorizations
		Authorization authorization = createGrantAuthorization(AUTHORIZATION, ANY);
		authorization.UserId = userId;
		authorization.addPermission(ALL);
		saveAuthorization(authorization);

		// give user all permission to manipulate users
		authorization = createGrantAuthorization(USER, ANY);
		authorization.UserId = userId;
		authorization.addPermission(Permissions.ALL);
		saveAuthorization(authorization);

		return user;
	  }

	  // group //////////////////////////////////////////////////////////////

	  protected internal virtual Group createGroup(string groupId)
	  {
		Group group = identityService.newGroup(groupId);
		identityService.saveGroup(group);
		return group;
	  }

	  // authorization ///////////////////////////////////////////////////////

	  protected internal virtual void createGrantAuthorization(Resource resource, string resourceId, Permission permission, string userId)
	  {
		Authorization authorization = createGrantAuthorization(resource, resourceId);
		authorization.UserId = userId;
		authorization.addPermission(permission);
		saveAuthorization(authorization);
	  }

	  protected internal virtual Authorization createGrantAuthorization(Resource resource, string resourceId)
	  {
		Authorization authorization = createAuthorization(AUTH_TYPE_GRANT, resource, resourceId);
		return authorization;
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

	  protected internal virtual void saveAuthorization(Authorization authorization)
	  {
		authorizationService.saveAuthorization(authorization);
	  }

	}

}