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
//	import static org.camunda.bpm.engine.authorization.Permissions.TASK_WORK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class DefaultUserPermissionsForTaskTest : AuthorizationTest
	{

	  protected internal string userId2 = "demo";
	  protected internal User user2;

	  protected internal string groupId2 = "accounting2";
	  protected internal Group group2;

	  protected internal string defaultTaskPermissionValue;

	  public override void tearDown()
	  {
		// reset default permission
		processEngineConfiguration.DefaultUserPermissionForTask = UPDATE;
		base.tearDown();
	  }

	  public virtual void testShouldGrantTaskWorkOnAssign()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = TASK_WORK;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.setAssignee(taskId, userId2);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, null, Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, null,Permissions.TASK_WORK, Resources.TASK, taskId));
		assertEquals(false, authorizationService.isUserAuthorized(userId2, null,Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	  public virtual void testShouldGrantUpdateOnAssign()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = UPDATE;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.setAssignee(taskId, userId2);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, null, Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, null,Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	  public virtual void testShouldGrantTaskWorkOnSetCandidateUser()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = TASK_WORK;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.addCandidateUser(taskId, userId2);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, null, Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, null,Permissions.TASK_WORK, Resources.TASK, taskId));
		assertEquals(false, authorizationService.isUserAuthorized(userId2, null,Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	  public virtual void testShouldGrantUpdateOnSetCandidateUser()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = UPDATE;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.addCandidateUser(taskId, userId2);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, null, Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, null,Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	  public virtual void testShouldGrantTaskWorkOnSetOwner()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = TASK_WORK;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.setOwner(taskId, userId2);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, null, Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, null,Permissions.TASK_WORK, Resources.TASK, taskId));
		assertEquals(false, authorizationService.isUserAuthorized(userId2, null,Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	  public virtual void testShouldGrantUpdateOnSetOwner()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = UPDATE;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.setOwner(taskId, userId2);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, null, Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, null,Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }


	  public virtual void testShouldGrantTaskWorkOnSetCandidateGroup()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = TASK_WORK;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.addCandidateGroup(taskId, groupId);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, Arrays.asList(groupId), Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, Arrays.asList(groupId),Permissions.TASK_WORK, Resources.TASK, taskId));
		assertEquals(false, authorizationService.isUserAuthorized(userId2, Arrays.asList(groupId),Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	  public virtual void testShouldGrantUpdateOnSetCandidateGroup()
	  {

		// given
		processEngineConfiguration.DefaultUserPermissionForTask = UPDATE;

		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		processEngine.TaskService.addCandidateGroup(taskId, groupId);

		// then
		assertEquals(true,authorizationService.isUserAuthorized(userId2, Arrays.asList(groupId), Permissions.READ, Resources.TASK, taskId));
		assertEquals(true, authorizationService.isUserAuthorized(userId2, Arrays.asList(groupId),Permissions.UPDATE, Resources.TASK, taskId));

		deleteTask(taskId, true);
	  }

	}

}