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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using TaskPermissions = org.camunda.bpm.engine.authorization.TaskPermissions;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class TaskReadVariablePermissionAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskReadVariablePermissionAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule);
		}


	  private const string PROCESS_KEY = "oneTaskProcess";
	  private const string DEMO = "demo";
	  private const string ACCOUNTING_GROUP = "accounting";
	  protected internal static string userId = "test";

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule);
	  public RuleChain ruleChain;

	  private ProcessEngineConfigurationImpl processEngineConfiguration;
	  private IdentityService identityService;
	  private AuthorizationService authorizationService;
	  private TaskService taskService;
	  private RuntimeService runtimeService;

	  private bool enforceSpecificVariablePermission;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService = engineRule.IdentityService;
		authorizationService = engineRule.AuthorizationService;
		taskService = engineRule.TaskService;
		runtimeService = engineRule.RuntimeService;

		enforceSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;
		processEngineConfiguration.EnforceSpecificVariablePermission = true;

		User user = identityService.newUser(userId);
		identityService.saveUser(user);
		identityService.AuthenticatedUserId = userId;
		authRule.createGrantAuthorization(Resources.AUTHORIZATION, "*", userId, Permissions.CREATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		authRule.disableAuthorization();
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().userIdIn(DEMO).list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().groupIdIn(ACCOUNTING_GROUP).list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
		processEngineConfiguration.EnforceSpecificVariablePermission = enforceSpecificVariablePermission;
	  }

	  // TaskService#saveTask() ///////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveStandaloneTaskAndCheckAssigneePermissions()
	  public virtual void testSaveStandaloneTaskAndCheckAssigneePermissions()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		Task task = selectSingleTask();
		task.Assignee = DEMO;

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals(DEMO, task.Assignee);
		verifyUserAuthorization(DEMO);
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testSaveProcessTaskAndCheckAssigneePermissions()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testSaveProcessTaskAndCheckAssigneePermissions()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = DEMO;

		authRule.createGrantAuthorization(TASK, task.Id, userId, UPDATE);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals(DEMO, task.Assignee);
		verifyUserAuthorization(DEMO);
	  }

	  // TaskService#setOwner() ///////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStandaloneTaskSetOwnerAndCheckOwnerPermissions()
	  public virtual void testStandaloneTaskSetOwnerAndCheckOwnerPermissions()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, DEMO);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(DEMO, task.Owner);
		verifyUserAuthorization(DEMO);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessTaskSetOwnerAndCheckOwnerPermissions()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessTaskSetOwnerAndCheckOwnerPermissions()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, DEMO);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(DEMO, task.Owner);
		verifyUserAuthorization(DEMO);
	  }

	  // TaskService#addUserIdentityLink() ///////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStandaloneTaskAddUserIdentityLinkAndUserOwnerPermissions()
	  public virtual void testStandaloneTaskAddUserIdentityLinkAndUserOwnerPermissions()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addUserIdentityLink(taskId, DEMO, IdentityLinkType.CANDIDATE);

		// then
		authRule.disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		authRule.disableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals(DEMO, identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
		verifyUserAuthorization(DEMO);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessTaskAddUserIdentityLinkWithUpdatePersmissionOnTask()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessTaskAddUserIdentityLinkWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addUserIdentityLink(taskId, DEMO, IdentityLinkType.CANDIDATE);

		// then
		authRule.disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		authRule.disableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals(DEMO, identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
		verifyUserAuthorization(DEMO);
	  }

	  // TaskService#addGroupIdentityLink() ///////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStandaloneTaskAddGroupIdentityLink()
	  public virtual void testStandaloneTaskAddGroupIdentityLink()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addGroupIdentityLink(taskId, ACCOUNTING_GROUP, IdentityLinkType.CANDIDATE);

		// then
		authRule.disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		authRule.disableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals(ACCOUNTING_GROUP, identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		verifyGroupAuthorization(ACCOUNTING_GROUP);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessTaskAddGroupIdentityLinkWithUpdatePersmissionOnTask()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessTaskAddGroupIdentityLinkWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		authRule.createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addGroupIdentityLink(taskId, ACCOUNTING_GROUP, IdentityLinkType.CANDIDATE);

		// then
		authRule.disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		authRule.disableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals(ACCOUNTING_GROUP, identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
		verifyGroupAuthorization(ACCOUNTING_GROUP);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void createTask(final String taskId)
	  protected internal virtual void createTask(string taskId)
	  {
		authRule.disableAuthorization();
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);
		authRule.enableAuthorization(userId);
	  }

	  protected internal virtual Task selectSingleTask()
	  {
		authRule.disableAuthorization();
		Task task = taskService.createTaskQuery().singleResult();
		authRule.enableAuthorization(userId);
		return task;
	  }

	  protected internal virtual void startProcessInstanceByKey(string processKey)
	  {
		authRule.disableAuthorization();
		runtimeService.startProcessInstanceByKey(processKey);
		authRule.enableAuthorization(userId);
	  }

	  protected internal virtual void verifyUserAuthorization(string userId)
	  {
		authRule.disableAuthorization();
		Authorization userAuthorization = authorizationService.createAuthorizationQuery().userIdIn(userId).singleResult();
		assertNotNull(userAuthorization);
		verifyReadVariablePermission(userAuthorization);
	  }

	  protected internal virtual void verifyGroupAuthorization(string groupId)
	  {
		authRule.disableAuthorization();
		Authorization groupAuthorization = authorizationService.createAuthorizationQuery().groupIdIn(groupId).singleResult();
		assertNotNull(groupAuthorization);
		verifyReadVariablePermission(groupAuthorization);
	  }

	  protected internal virtual void verifyReadVariablePermission(Authorization groupAuthorization)
	  {
		Permission[] permissions = groupAuthorization.getPermissions(new Permission[] {TaskPermissions.READ_VARIABLE});
		assertNotNull(permissions);
		assertEquals(TaskPermissions.READ_VARIABLE, permissions[0]);
	  }

	}

}