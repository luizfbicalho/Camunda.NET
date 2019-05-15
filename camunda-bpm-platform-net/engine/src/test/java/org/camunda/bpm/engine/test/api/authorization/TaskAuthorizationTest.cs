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
namespace org.camunda.bpm.engine.test.api.authorization
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.TASK_ASSIGN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.TASK_WORK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.TaskPermissions.UPDATE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;

	using org.camunda.bpm.engine.authorization;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using DefaultAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultAuthorizationProvider;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string CASE_KEY = "oneTaskCase";
	  protected internal const string DEMO_ASSIGNEE_PROCESS_KEY = "demoAssigneeProcess";
	  protected internal const string CANDIDATE_USERS_PROCESS_KEY = "candidateUsersProcess";
	  protected internal const string CANDIDATE_GROUPS_PROCESS_KEY = "candidateGroupsProcess";
	  protected internal const string INVALID_PERMISSION = "invalidPermission";
	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/authorization/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/candidateUsersProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/candidateGroupsProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly TaskAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(TaskAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<HistoricVariableInstance> variables = outerInstance.historyService.createHistoricVariableInstanceQuery().includeDeleted().list();
			foreach (HistoricVariableInstance variable in variables)
			{
			  commandContext.DbEntityManager.delete((HistoricVariableInstanceEntity) variable);
			}
			return null;
		  }
	  }

	  // task query ///////////////////////////////////////////////////////

	  public virtual void testSimpleQueryWithTaskInsideProcessWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnOneTaskProcess()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_TASK);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithTaskInsideProcessWithReadPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_TASK);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_TASK);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_TASK);
		createGrantAuthorization(TASK, ANY, userId, READ);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithTaskInsideProcessWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

		disableAuthorization();
		string taskId = taskService.createTaskQuery().processDefinitionKey(PROCESS_KEY).listPage(0, 1).get(0).Id;
		enableAuthorization();

		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnOneTaskProcess()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_TASK);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithTaskInsideProcessWithReadPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);
		startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_TASK);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testQueryWithTaskInsideCaseWithoutAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithStandaloneTaskWithoutAuthorization()
	  {
		// given
		string taskId = "newTask";
		createTask(taskId);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryWithStandaloneTaskWithReadPermissionOnTask()
	  {
		// given
		string taskId = "newTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskQuery query = taskService.createTaskQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  // new task /////////////////////////////////////////////////////////////

	  public virtual void testNewTaskWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  taskService.newTask();
		  fail("Exception expected: It should not be possible to create a new task.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'Task'", e.Message);
		}
	  }

	  public virtual void testNewTask()
	  {
		// given
		createGrantAuthorization(TASK, ANY, userId, CREATE);

		// when
		Task task = taskService.newTask();

		// then
		assertNotNull(task);
	  }

	  // save task (insert) //////////////////////////////////////////////////////////

	  public virtual void testSaveTaskInsertWithoutAuthorization()
	  {
		// given
		TaskEntity task = TaskEntity.create();

		try
		{
		  // when
		  taskService.saveTask(task);
		  fail("Exception expected: It should not be possible to save a task.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have 'CREATE' permission on resource 'Task'", e.Message);
		}
	  }

	  public virtual void testSaveTaskInsert()
	  {
		// given
		TaskEntity task = TaskEntity.create();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, ANY, userId, CREATE);

		// when
		taskService.saveTask(task);

		// then
		task = (TaskEntity) selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		string taskId = task.Id;
		deleteTask(taskId, true);
	  }

	  public virtual void testSaveAndUpdateTaskWithTaskAssignPermission()
	  {
		// given
		TaskEntity task = TaskEntity.create();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, ANY, userId, CREATE, TASK_ASSIGN);

		// when
		taskService.saveTask(task);

		task.@delegate("demoNew");

		taskService.saveTask(task);

		// then
		task = (TaskEntity) selectSingleTask();
		assertNotNull(task);
		assertEquals("demoNew", task.Assignee);

		string taskId = task.Id;
		deleteTask(taskId, true);
	  }

	  // save (standalone) task (update) //////////////////////////////////////////////////////////

	  public virtual void testSaveStandaloneTaskUpdateWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		Task task = selectSingleTask();

		try
		{
		  // when
		  taskService.saveTask(task);
		  fail("Exception expected: It should not be possible to save a task.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN'", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testSaveStandaloneTaskUpdate()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  // save (process) task (update) //////////////////////////////////////////////////////////

	  public virtual void testSaveProcessTaskUpdateWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();

		try
		{
		  // when
		  taskService.saveTask(task);
		  fail("Exception expected: It should not be possible to save a task.");
		}
		catch (AuthorizationException e)
		{

		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(task.Id, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSaveProcessTaskUpdateWithUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, task.Id, userId, UPDATE);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testSaveProcessTaskUpdateWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, task.Id, userId, TASK_ASSIGN);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testSaveProcessTaskUpdateWithUpdatePermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testSaveProcessTaskUpdateWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testSaveProcessTaskUpdateWithUpdateTasksPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testSaveProcessTaskUpdateWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // save (case) task (update) //////////////////////////////////////////////////////////

	  public virtual void testSaveCaseTaskUpdate()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		Task task = selectSingleTask();
		task.Assignee = "demo";

		// when
		taskService.saveTask(task);

		// then
		task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // delete task ///////////////////////////////////////////////////////////////////////

	  public virtual void testDeleteTaskWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.deleteTask(taskId);
		  fail("Exception expected: It should not be possible to delete a task.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'DELETE' permission on resource 'myTask' of type 'Task'", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testDeleteTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, DELETE);

		// when
		taskService.deleteTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);

		// triggers a db clean up
		deleteTask(taskId, true);
	  }

	  // delete tasks ///////////////////////////////////////////////////////////////////////

	  public virtual void testDeleteTasksWithoutAuthorization()
	  {
		// given
		string firstTaskId = "myTask1";
		createTask(firstTaskId);
		string secondTaskId = "myTask2";
		createTask(secondTaskId);

		try
		{
		  // when
		  taskService.deleteTasks(Arrays.asList(firstTaskId, secondTaskId));
		  fail("Exception expected: It should not be possible to delete tasks.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'DELETE' permission on resource 'myTask1' of type 'Task'", e.Message);
		}

		deleteTask(firstTaskId, true);
		deleteTask(secondTaskId, true);
	  }

	  public virtual void testDeleteTasksWithDeletePermissionOnFirstTask()
	  {
		// given
		string firstTaskId = "myTask1";
		createTask(firstTaskId);
		createGrantAuthorization(TASK, firstTaskId, userId, DELETE);

		string secondTaskId = "myTask2";
		createTask(secondTaskId);

		try
		{
		  // when
		  taskService.deleteTasks(Arrays.asList(firstTaskId, secondTaskId));
		  fail("Exception expected: It should not be possible to delete tasks.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'DELETE' permission on resource 'myTask2' of type 'Task'", e.Message);
		}

		deleteTask(firstTaskId, true);
		deleteTask(secondTaskId, true);
	  }

	  public virtual void testDeleteTasks()
	  {
		// given
		string firstTaskId = "myTask1";
		createTask(firstTaskId);
		string secondTaskId = "myTask2";
		createTask(secondTaskId);

		createGrantAuthorization(TASK, ANY, userId, DELETE);

		// when
		taskService.deleteTasks(Arrays.asList(firstTaskId, secondTaskId));

		// then
		Task task = selectSingleTask();
		assertNull(task);

		// triggers a db clean up
		deleteTask(firstTaskId, true);
		deleteTask(secondTaskId, true);
	  }

	  // set assignee on standalone task /////////////////////////////////////////////

	  public virtual void testStandaloneTaskSetAssigneeWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.setAssignee(taskId, "demo");
		  fail("Exception expected: It should not be possible to set an assignee");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetAssignee()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetAssigneeWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  // set assignee on process task /////////////////////////////////////////////

	  public virtual void testProcessTaskSetAssigneeWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.setAssignee(taskId, "demo");
		  fail("Exception expected: It should not be possible to set an assignee");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskSetAssigneeWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskSetAssigneeWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskSetAssigneeWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskSetAssigneeWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskSetAssigneeWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskSetAssigneeWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskSetAssignee()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // set assignee on case task /////////////////////////////////////////////

	  public virtual void testCaseTaskSetAssignee()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // set owner on standalone task /////////////////////////////////////////////

	  public virtual void testStandaloneTaskSetOwnerWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.setOwner(taskId, "demo");
		  fail("Exception expected: It should not be possible to set an owner");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetOwner()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetOwnerWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);

		deleteTask(taskId, true);
	  }

	  // set owner on process task /////////////////////////////////////////////

	  public virtual void testProcessTaskSetOwnerWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.setOwner(taskId, "demo");
		  fail("Exception expected: It should not be possible to set an owner");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskSetOwnerWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwnerWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwnerWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwnerWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwnerWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwnerWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwner()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  public virtual void testProcessTaskSetOwnerWithTaskAssignPermission()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  // set owner on case task /////////////////////////////////////////////

	  public virtual void testCaseTaskSetOwner()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.setOwner(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Owner);
	  }

	  // add candidate user ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskAddCandidateUserWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.addCandidateUser(taskId, "demo");
		  fail("Exception expected: It should not be possible to add a candidate user");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddCandidateUser()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		deleteTask(taskId, true);
	  }

	  // add candidate user ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskAddCandidateUserWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.addCandidateUser(taskId, "demo");
		  fail("Exception expected: It should not be possible to add a candidate user");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskAddCandidateUserWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUserWithTaskAssignPermissionRevokeOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createRevokeAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		try
		{
		  // when
		  taskService.addCandidateUser(taskId, "demo");
		  fail("Exception expected: It should not be possible to add an user identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}
	  }

	  public virtual void testProcessTaskAddCandidateUserWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUserWithGrantTaskAssignAndRevokeUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createRevokeAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUserWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUserWithTaskAssignPersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUserWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUserWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateUser()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add candidate user ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskAddCandidateUser()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add candidate group ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskAddCandidateGroupWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.addCandidateGroup(taskId, "accounting");
		  fail("Exception expected: It should not be possible to add a candidate group");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddCandidateGroup()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddCandidateGroupWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		deleteTask(taskId, true);
	  }

	  // add candidate group ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskAddCandidateGroupWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.addCandidateGroup(taskId, "accounting");
		  fail("Exception expected: It should not be possible to add a candidate group");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroup()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermission()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddCandidateGroupWithTaskAssignPermissionRevoked()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createRevokeAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);
		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add candidate group ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskAddCandidateGroup()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.addCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add user identity link ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskAddUserIdentityLinkWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to add an user identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddUserIdentityLink()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddUserIdentityLinkWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);


		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		deleteTask(taskId, true);
	  }

	  // add user identity link ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskAddUserIdentityLinkWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to add an user identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskAddUserIdentityLinkWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddUserIdentityLinkWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddUserIdentityLinkWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddUserIdentityLinkWithTaskAssignPersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddUserIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddUserIdentityLinkWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddUserIdentityLink()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add user identity link ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskAddUserIdentityLink()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.addUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("demo", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add group identity link ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskAddGroupIdentityLinkWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to add a group identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddGroupIdentityLink()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		deleteTask(taskId, true);
	  }

	  // add group identity link ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskAddGroupIdentityLinkWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to add a group identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskAddGroupIdentityLinkWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddGroupIdentityLinkWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddGroupIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  public virtual void testProcessTaskAddGroupIdentityLink()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // add group identity link ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskAddGroupIdentityLink()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.addGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertEquals(1, linksForTask.Count);

		IdentityLink identityLink = linksForTask[0];
		assertNotNull(identityLink);

		assertEquals("accounting", identityLink.GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
	  }

	  // delete candidate user ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskDeleteCandidateUserWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		try
		{
		  // when
		  taskService.deleteCandidateUser(taskId, "demo");
		  fail("Exception expected: It should not be possible to delete a candidate user");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteCandidateUser()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteCandidateUserWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  // delete candidate user ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskDeleteCandidateUserWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		try
		{
		  // when
		  taskService.deleteCandidateUser(taskId, "demo");
		  fail("Exception expected: It should not be possible to delete a candidate user");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskDeleteCandidateUserWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateUserWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateUserWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateUserWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateUserWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateUserWithTaskAssignPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateUser()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete candidate user ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskDeleteCandidateUser()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		// when
		taskService.deleteCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete candidate group ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskDeleteCandidateGroupWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateGroup(taskId, "accounting");

		try
		{
		  // when
		  taskService.deleteCandidateGroup(taskId, "accounting");
		  fail("Exception expected: It should not be possible to delete a candidate group");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteCandidateGroup()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteCandidateGroupWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  // delete candidate group ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskDeleteCandidateGroupWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		try
		{
		  // when
		  taskService.deleteCandidateGroup(taskId, "accounting");
		  fail("Exception expected: It should not be possible to delete a candidate group");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskDeleteCandidateGroupWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateGroupWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateGroupWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateGroupWithTaskAssignPersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateGroupWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateGroupWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteCandidateGroup()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete candidate group ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskDeleteCandidateGroup()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		// when
		taskService.deleteCandidateGroup(taskId, "accounting");

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete user identity link ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskDeleteUserIdentityLinkWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		try
		{
		  // when
		  taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to delete an user identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteUserIdentityLink()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteUserIdentityLinkWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  // delete user identity link ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		try
		{
		  // when
		  taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to delete an user identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLink()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteUserIdentityLinkWithTaskAssignPermission()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete user identity link ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskDeleteUserIdentityLink()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		// when
		taskService.deleteUserIdentityLink(taskId, "demo", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete group identity link ((standalone) task) /////////////////////////////////////////////

	  public virtual void testStandaloneTaskDeleteGroupIdentityLinkWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateGroup(taskId, "accounting");

		try
		{
		  // when
		  taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to delete a group identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDeleteGroupIdentityLink()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  // delete group identity link ((process) task) /////////////////////////////////////////////

	  public virtual void testProcessTaskDeleteGroupIdentityLinkWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		try
		{
		  // when
		  taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);
		  fail("Exception expected: It should not be possible to delete a group identity link");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskDeleteGroupIdentityLinkWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteGroupIdentityLinkWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteGroupIdentityLinkWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskDeleteGroupIdentityLink()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // delete group identity link ((case) task) /////////////////////////////////////////////

	  public virtual void testCaseTaskDeleteGroupIdentityLink()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateGroup(taskId, "accounting");

		// when
		taskService.deleteGroupIdentityLink(taskId, "accounting", IdentityLinkType.CANDIDATE);

		// then
		disableAuthorization();
		IList<IdentityLink> linksForTask = taskService.getIdentityLinksForTask(taskId);
		enableAuthorization();

		assertNotNull(linksForTask);
		assertTrue(linksForTask.Count == 0);
	  }

	  // get identity links ((standalone) task) ////////////////////////////////////////////////

	  public virtual void testStandaloneTaskGetIdentityLinksWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		try
		{
		  // when
		  taskService.getIdentityLinksForTask(taskId);
		  fail("Exception expected: It should not be possible to get identity links");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have 'READ' permission on resource 'myTask' of type 'Task'", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskGetIdentityLinks()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		IList<IdentityLink> identityLinksForTask = taskService.getIdentityLinksForTask(taskId);

		// then
		assertNotNull(identityLinksForTask);
		assertFalse(identityLinksForTask.Count == 0);

		deleteTask(taskId, true);
	  }

	  // get identity links ((process) task) ////////////////////////////////////////////////

	  public virtual void testProcessTaskGetIdentityLinksWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		try
		{
		  // when
		  taskService.getIdentityLinksForTask(taskId);
		  fail("Exception expected: It should not be possible to get the identity links");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(READ_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskGetIdentityLinksWithReadPersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		IList<IdentityLink> identityLinksForTask = taskService.getIdentityLinksForTask(taskId);

		// then
		assertNotNull(identityLinksForTask);
		assertFalse(identityLinksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskGetIdentityLinksWithReadPersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		IList<IdentityLink> identityLinksForTask = taskService.getIdentityLinksForTask(taskId);

		// then
		assertNotNull(identityLinksForTask);
		assertFalse(identityLinksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskGetIdentityLinksWithReadTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_TASK);

		// when
		IList<IdentityLink> identityLinksForTask = taskService.getIdentityLinksForTask(taskId);

		// then
		assertNotNull(identityLinksForTask);
		assertFalse(identityLinksForTask.Count == 0);
	  }

	  public virtual void testProcessTaskGetIdentityLinks()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_TASK);

		// when
		IList<IdentityLink> identityLinksForTask = taskService.getIdentityLinksForTask(taskId);

		// then
		assertNotNull(identityLinksForTask);
		assertFalse(identityLinksForTask.Count == 0);
	  }

	  // get identity links ((case) task) ////////////////////////////////////////////////

	  public virtual void testCaseTaskGetIdentityLinks()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		addCandidateUser(taskId, "demo");

		// when
		IList<IdentityLink> identityLinksForTask = taskService.getIdentityLinksForTask(taskId);

		// then
		assertNotNull(identityLinksForTask);
		assertFalse(identityLinksForTask.Count == 0);
	  }

	  // claim (standalone) task ////////////////////////////////////////////////////////////

	  public virtual void testStandaloneTaskClaimTaskWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.claim(taskId, "demo");
		  fail("Exception expected: It should not be possible to claim the task.");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions:", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskClaimTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskClaimTaskWithTaskWorkPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_WORK);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskClaimTaskWithRevokeTaskWorkPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createRevokeAuthorization(TASK, taskId, userId, TASK_WORK);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		try
		{
		  // when
		  taskService.claim(taskId, "demo");
		  fail("Exception expected: It should not be possible to complete a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_WORK", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  // claim (process) task ////////////////////////////////////////////////////////////

	  public virtual void testProcessTaskClaimTaskWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.claim(taskId, "demo");
		  fail("Exception expected: It should not be possible to claim the task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskClaimTaskWithUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskClaimTaskWithTaskWorkPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_WORK);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskClaimTaskWithGrantTaskWorkAndRevokeUpdatePermissionsOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_WORK);
		createRevokeAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskClaimTaskWithRevokeTaskWorkPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createRevokeAuthorization(TASK, taskId, userId, TASK_WORK);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		try
		{
		  // when
		  taskService.complete(taskId);
		  fail("Exception expected: It should not be possible to complete a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_WORK", e.Message);
		}

	  }

	  public virtual void testProcessTaskClaimTaskWithUpdatePermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskClaimTaskWithTaskWorkPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_WORK);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskClaimTaskWithUpdateTasksPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskClaimTaskWithTaskWorkPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_WORK);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

	  }

	  public virtual void testProcessTaskClaimTaskWithRevokeTaskWorkPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createRevokeAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_WORK);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		try
		{
		  // when
		  taskService.complete(taskId);
		  fail("Exception expected: It should not be possible to complete a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_WORK", e.Message);
		}

	  }

	  public virtual void testProcessTaskClaimTask()
	  {
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // claim (case) task ////////////////////////////////////////////////////////////

	  public virtual void testCaseTaskClaimTask()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.claim(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // complete (standalone) task ////////////////////////////////////////////////////////////

	  public virtual void testStandaloneTaskCompleteTaskWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.complete(taskId);
		  fail("Exception expected: It should not be possible to complete a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_WORK", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskCompleteTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);

		if (!processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE))
		{
		  historyService.deleteHistoricTaskInstance(taskId);
		}
	  }

	  public virtual void testStandaloneTaskCompleteWithTaskWorkPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_WORK);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);

		if (!processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE))
		{
		  historyService.deleteHistoricTaskInstance(taskId);
		}
	  }

	  // complete (process) task ////////////////////////////////////////////////////////////

	  public virtual void testProcessTaskCompleteTaskWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.complete(taskId);
		  fail("Exception expected: It should not be possible to complete a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskCompleteTaskWithUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskCompleteTaskWithTaskWorkPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_WORK);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskCompleteTaskWithUpdatePermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskCompleteTaskWithUpdateTasksPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskCompleteTaskWithTaskWorkPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_WORK);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  public virtual void testProcessTaskCompleteTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  // complete (case) task ////////////////////////////////////////////////////////////

	  public virtual void testCaseTaskCompleteTask()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.complete(taskId);

		// then
		Task task = selectSingleTask();
		assertNull(task);
	  }

	  // delegate (standalone) task ///////////////////////////////////////////////////////

	  public virtual void testStandaloneTaskDelegateTaskWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.delegateTask(taskId, "demo");
		  fail("Exception expected: It should not be possible to delegate a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDelegateTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskDelegateTaskWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);

		deleteTask(taskId, true);
	  }

	  // delegate (process) task ///////////////////////////////////////////////////////////

	  public virtual void testProcessTaskDelegateTaskWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.delegateTask(taskId, "demo");
		  fail("Exception expected: It should not be possible to delegate a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskDelegateTaskWithUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTaskWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTaskWithUpdatePermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTaskWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTaskWithUpdateTasksPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTaskWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  public virtual void testProcessTaskDelegateTaskWithTaskAssignPermission()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // delegate (case) task /////////////////////////////////////////////////////////////////

	  public virtual void testCaseTaskDelegateTask()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("demo", task.Assignee);
	  }

	  // resolve (standalone) task ///////////////////////////////////////////////////////

	  public virtual void testStandaloneTaskResolveTaskWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.resolveTask(taskId);
		  fail("Exception expected: It should not be possible to resolve a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_WORK", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskResolveTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, userId);
		delegateTask(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.resolveTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(userId, task.Assignee);

		deleteTask(taskId, true);
	  }

	  // delegate (process) task ///////////////////////////////////////////////////////////

	  public virtual void testProcessTaskResolveTaskWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.resolveTask(taskId);
		  fail("Exception expected: It should not be possible to resolve a task");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskResolveTaskWithUpdatePermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, userId);
		delegateTask(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.resolveTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(userId, task.Assignee);
	  }

	  public virtual void testProcessTaskResolveTaskWithUpdatePermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, userId);
		delegateTask(taskId, "demo");

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.resolveTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(userId, task.Assignee);
	  }

	  public virtual void testProcessTaskResolveTaskWithUpdateTasksPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, userId);
		delegateTask(taskId, "demo");

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.resolveTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(userId, task.Assignee);
	  }

	  public virtual void testProcessTaskResolveTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, userId);
		delegateTask(taskId, "demo");

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.resolveTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(userId, task.Assignee);
	  }

	  // delegate (case) task /////////////////////////////////////////////////////////////////

	  public virtual void testCaseTaskResolveTask()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, userId);
		delegateTask(taskId, "demo");

		// when
		taskService.resolveTask(taskId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(userId, task.Assignee);
	  }

	  // set priority on standalone task /////////////////////////////////////////////

	  public virtual void testStandaloneTaskSetPriorityWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		try
		{
		  // when
		  taskService.setPriority(taskId, 80);
		  fail("Exception expected: It should not be possible to set a priority");
		}
		catch (AuthorizationException e)
		{
		  // then
		  assertTextPresent("The user with id 'test' does not have one of the following permissions: 'TASK_ASSIGN'", e.Message);
		}

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetPriority()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetPriorityWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);

		deleteTask(taskId, true);
	  }

	  // set priority on process task /////////////////////////////////////////////

	  public virtual void testProcessTaskSetPriorityWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  taskService.setPriority(taskId, 80);
		  fail("Exception expected: It should not be possible to set a priority");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(taskId, message);
		  assertTextPresent(TASK.resourceName(), message);
		  assertTextPresent(UPDATE_TASK.Name, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testProcessTaskSetPriorityWithUpdatePersmissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriorityWithTaskAssignPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriorityWithUpdatePersmissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, UPDATE);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriorityWithTaskAssignPermissionOnAnyTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, ANY, userId, TASK_ASSIGN);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriorityWithUpdateTasksPersmissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriorityWithTaskAssignPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriority()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, UPDATE_TASK);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  public virtual void testProcessTaskSetPriorityWithTaskAssignPermission()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, TASK_ASSIGN);

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  // set priority on case task /////////////////////////////////////////////

	  public virtual void testCaseTaskSetPriority()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		// when
		taskService.setPriority(taskId, 80);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals(80, task.Priority);
	  }

	  // get sub tasks ((standalone) task) ////////////////////////////////////

	  public virtual void testStandaloneTaskGetSubTasksWithoutAuthorization()
	  {
		// given
		string parentTaskId = "parentTaskId";
		createTask(parentTaskId);

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertTrue(subTasks.Count == 0);

		deleteTask(parentTaskId, true);
	  }

	  public virtual void testStandaloneTaskGetSubTasksWithReadPermissionOnSub1()
	  {
		// given
		string parentTaskId = "parentTaskId";
		createTask(parentTaskId);

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		createGrantAuthorization(TASK, "sub1", userId, READ);

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertFalse(subTasks.Count == 0);
		assertEquals(1, subTasks.Count);

		assertEquals("sub1", subTasks[0].Id);

		deleteTask(parentTaskId, true);
	  }

	  public virtual void testStandaloneTaskGetSubTasks()
	  {
		// given
		string parentTaskId = "parentTaskId";
		createTask(parentTaskId);

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertFalse(subTasks.Count == 0);
		assertEquals(2, subTasks.Count);

		deleteTask(parentTaskId, true);
	  }

	  // get sub tasks ((process) task) ////////////////////////////////////

	  public virtual void testProcessTaskGetSubTasksWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string parentTaskId = selectSingleTask().Id;

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertTrue(subTasks.Count == 0);
	  }

	  public virtual void testProcessTaskGetSubTasksWithReadPermissionOnSub1()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string parentTaskId = selectSingleTask().Id;

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		createGrantAuthorization(TASK, "sub1", userId, READ);

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertFalse(subTasks.Count == 0);
		assertEquals(1, subTasks.Count);

		assertEquals("sub1", subTasks[0].Id);
	  }

	  public virtual void testProcessTaskGetSubTasks()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string parentTaskId = selectSingleTask().Id;

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertFalse(subTasks.Count == 0);
		assertEquals(2, subTasks.Count);
	  }

	  // get sub tasks ((case) task) ////////////////////////////////////

	  public virtual void testCaseTaskGetSubTasksWithoutAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string parentTaskId = selectSingleTask().Id;

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertTrue(subTasks.Count == 0);
	  }

	  public virtual void testCaseTaskGetSubTasksWithReadPermissionOnSub1()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string parentTaskId = selectSingleTask().Id;

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		createGrantAuthorization(TASK, "sub1", userId, READ);

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertFalse(subTasks.Count == 0);
		assertEquals(1, subTasks.Count);

		assertEquals("sub1", subTasks[0].Id);
	  }

	  public virtual void testCaseTaskGetSubTasks()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string parentTaskId = selectSingleTask().Id;

		disableAuthorization();
		Task sub1 = taskService.newTask("sub1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("sub2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);
		enableAuthorization();

		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		IList<Task> subTasks = taskService.getSubTasks(parentTaskId);

		// then
		assertFalse(subTasks.Count == 0);
		assertEquals(2, subTasks.Count);
	  }

	  // clear authorization ((standalone) task) ////////////////////////

	  public virtual void testStandaloneTaskClearAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().resourceId(taskId).singleResult();
		enableAuthorization();
		assertNotNull(authorization);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();
		authorization = authorizationService.createAuthorizationQuery().resourceId(taskId).singleResult();
		enableAuthorization();

		assertNull(authorization);

		deleteTask(taskId, true);
	  }

	  // clear authorization ((process) task) ////////////////////////

	  public virtual void testProcessTaskClearAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().resourceId(taskId).singleResult();
		enableAuthorization();
		assertNotNull(authorization);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();
		authorization = authorizationService.createAuthorizationQuery().resourceId(taskId).singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  // set assignee -> an authorization is available (standalone task) /////////////////////////////////////////

	  public virtual void testStandaloneTaskSetAssigneeCreateNewAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetAssigneeUpdateAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetAssigneeToNullAuthorizationStillAvailable()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// set assignee to demo -> an authorization for demo is available
		taskService.setAssignee(taskId, "demo");

		// when
		taskService.setAssignee(taskId, null);

		// then
		// authorization for demo is still available
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryStandaloneTaskSetAssignee()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// set assignee to demo -> an authorization for demo is available
		taskService.setAssignee(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetAssigneeOutsideCommandContextInsert()
	  {
		// given
		string taskId = "myTask";
		createGrantAuthorization(TASK, ANY, userId, CREATE);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		Task task = taskService.newTask(taskId);
		task.Assignee = "demo";

		// when
		taskService.saveTask(task);

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetAssigneeOutsideCommandContextSave()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		Task task = selectSingleTask();

		task.Assignee = "demo";

		// when
		taskService.saveTask(task);

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  // set assignee -> an authorization is available (process task) /////////////////////////////////////////

	  public virtual void testProcessTaskSetAssigneeCreateNewAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testProcessTaskSetAssigneeUpdateAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testProcessTaskSetAssigneeToNullAuthorizationStillAvailable()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// set assignee to demo -> an authorization for demo is available
		taskService.setAssignee(taskId, "demo");

		// when
		taskService.setAssignee(taskId, null);

		// then
		// authorization for demo is still available
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testQueryProcessTaskSetAssignee()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// set assignee to demo -> an authorization for demo is available
		taskService.setAssignee(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
	  }

	  public virtual void testProcessTaskAssignee()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, DEMO_ASSIGNEE_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		runtimeService.startProcessInstanceByKey(DEMO_ASSIGNEE_PROCESS_KEY);

		// then
		// an authorization for demo has been created
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		// demo is able to retrieve the task
		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
	  }

	  // set assignee -> should not create an authorization (case task) /////////////////////////////////////////

	  public virtual void testCaseTaskSetAssigneeNoAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  // set owner -> an authorization is available (standalone task) /////////////////////////////////////////

	  public virtual void testStandaloneTaskSetOwnerCreateNewAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetOwnerUpdateAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryStandaloneTaskSetOwner()
	  {
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// set owner to demo -> an authorization for demo is available
		taskService.setOwner(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetOwnerOutsideCommandContextInsert()
	  {
		// given
		string taskId = "myTask";
		createGrantAuthorization(TASK, ANY, userId, CREATE);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		Task task = taskService.newTask(taskId);
		task.Owner = "demo";

		// when
		taskService.saveTask(task);

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskSetOwnerOutsideCommandContextSave()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		Task task = selectSingleTask();

		task.Owner = "demo";

		// when
		taskService.saveTask(task);

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  // set owner -> an authorization is available (process task) /////////////////////////////////////////

	  public virtual void testProcessTaskSetOwnerCreateNewAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testProcessTaskSetOwnerUpdateAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testQueryProcessTaskSetOwner()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// set owner to demo -> an authorization for demo is available
		taskService.setOwner(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
	  }

	  // set owner -> should not create an authorization  (case task) /////////////////////////////////

	  public virtual void testCaseTaskSetOwnerNoAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.setOwner(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  // add candidate user -> an authorization is available (standalone task) /////////////////

	  public virtual void testStandaloneTaskAddCandidateUserCreateNewAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddCandidateUserUpdateAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryStandaloneTaskAddCandidateUser()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// add candidate user -> an authorization for demo is available
		taskService.addCandidateUser(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
		deleteTask(taskId, true);
	  }

	  public virtual void testQueryStandaloneTaskAddCandidateUserWithTaskAssignPermission()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, TASK_ASSIGN);

		// add candidate user -> an authorization for demo is available
		taskService.addCandidateUser(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
		deleteTask(taskId, true);
	  }

	  // add candidate user -> an authorization is available (process task) ////////////////////

	  public virtual void testProcessTaskAddCandidateUserCreateNewAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testProcessTaskAddCandidateUserUpdateAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testQueryProcessTaskAddCandidateUser()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// add candidate user -> an authorization for demo is available
		taskService.addCandidateUser(taskId, "demo");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
	  }

	  public virtual void testProcessTaskCandidateUsers()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, CANDIDATE_USERS_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		runtimeService.startProcessInstanceByKey(CANDIDATE_USERS_PROCESS_KEY);

		// then
		// an authorization for demo has been created
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		// an authorization for test has been created
		disableAuthorization();
		authorization = authorizationService.createAuthorizationQuery().userIdIn("test").resourceId(taskId).singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		// demo is able to retrieve the task
		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals(taskId, task.Id);

		// test is able to retrieve the task
		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals(taskId, task.Id);
	  }

	  // add candidate user -> should not create an authorization  (case task) /////////////////////////////////

	  public virtual void testCaseTaskAddCandidateUserNoAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateUser(taskId, "demo");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().userIdIn("demo").singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  // add candidate group -> an authorization is available (standalone task) /////////////////

	  public virtual void testStandaloneTaskAddCandidateGroupCreateNewAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateGroup(taskId, "management");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("management").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskAddCandidateGroupUpdateAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.addCandidateGroup(taskId, "management");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("management").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		deleteTask(taskId, true);
	  }

	  public virtual void testQueryStandaloneTaskAddCandidateGroup()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// add candidate group -> an authorization for group management is available
		taskService.addCandidateGroup(taskId, "management");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", Arrays.asList("management"));

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
		deleteTask(taskId, true);
	  }

	  // add candidate group -> an authorization is available (process task) ////////////////////

	  public virtual void testProcessTaskAddCandidateGroupCreateNewAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateGroup(taskId, "management");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("management").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testProcessTaskAddCandidateGroupUpdateAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);
		createGrantAuthorization(TASK, taskId, "demo", DELETE);

		// when
		taskService.addCandidateGroup(taskId, "management");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("management").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));
	  }

	  public virtual void testQueryProcessTaskAddCandidateGroup()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// add candidate group -> an authorization for group management is available
		taskService.addCandidateGroup(taskId, "management");

		identityService.clearAuthentication();
		identityService.setAuthentication("demo", Arrays.asList("management"));

		// when
		Task task = taskService.createTaskQuery().singleResult();

		// then
		assertNotNull(task);
		assertEquals(taskId, task.Id);

		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));
	  }

	  public virtual void testProcessTaskCandidateGroups()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, CANDIDATE_GROUPS_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		runtimeService.startProcessInstanceByKey(CANDIDATE_GROUPS_PROCESS_KEY);

		// then
		// an authorization for management has been created
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("management").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		// an authorization for accounting has been created
		disableAuthorization();
		authorization = authorizationService.createAuthorizationQuery().groupIdIn("accounting").singleResult();
		enableAuthorization();

		assertNotNull(authorization);
		assertEquals(TASK.resourceType(), authorization.ResourceType);
		assertEquals(taskId, authorization.ResourceId);
		assertTrue(authorization.isPermissionGranted(READ));
		assertTrue(authorization.isPermissionGranted(DefaultTaskPermissionForUser));

		// management is able to retrieve the task
		identityService.clearAuthentication();
		identityService.setAuthentication("demo", Arrays.asList("management"));

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals(taskId, task.Id);

		// accounting is able to retrieve the task
		identityService.clearAuthentication();
		identityService.setAuthentication(userId, Arrays.asList(groupId));

		task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);
		assertEquals(taskId, task.Id);
	  }

	  // add candidate group -> should not create an authorization (case task) /////////////////////////////////

	  public virtual void testCaseTaskAddCandidateGroupNoAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.addCandidateGroup(taskId, "management");

		// then
		disableAuthorization();
		Authorization authorization = authorizationService.createAuthorizationQuery().groupIdIn("management").singleResult();
		enableAuthorization();

		assertNull(authorization);
	  }

	  // TaskService#getVariable() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariable()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		// when
		object variable = taskService.getVariable(taskId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  // TaskService#getVariableLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariableLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariablesLocal(taskId, Variables);
		enableAuthorization();

		// when
		object variable = taskService.getVariableLocal(taskId, VARIABLE_NAME);

		// then
		assertEquals(VARIABLE_VALUE, variable);
	  }

	  // TaskService#getVariableTyped() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariableTyped()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		// when
		TypedValue typedValue = taskService.getVariableTyped(taskId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  // TaskService#getVariableLocalTyped() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariableLocalTyped()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariablesLocal(taskId, Variables);
		enableAuthorization();

		// when
		TypedValue typedValue = taskService.getVariableLocalTyped(taskId, VARIABLE_NAME);

		// then
		assertNotNull(typedValue);
		assertEquals(VARIABLE_VALUE, typedValue.Value);
	  }

	  // TaskService#getVariables() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		// when
		IDictionary<string, object> variables = taskService.getVariables(taskId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  // TaskService#getVariablesLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariablesLocal(taskId, Variables);
		enableAuthorization();

		// when
		IDictionary<string, object> variables = taskService.getVariablesLocal(taskId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  // TaskService#getVariablesTyped() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesTyped()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		// when
		VariableMap variables = taskService.getVariablesTyped(taskId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  // TaskService#getVariablesLocalTyped() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesLocalTyped()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariablesLocal(taskId, Variables);
		enableAuthorization();

		// when
		IDictionary<string, object> variables = taskService.getVariablesLocalTyped(taskId);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  // TaskService#getVariables() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesByName()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		// when
		IDictionary<string, object> variables = taskService.getVariables(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  // TaskService#getVariablesLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesLocalByName()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariablesLocal(taskId, Variables);
		enableAuthorization();

		// when
		IDictionary<string, object> variables = taskService.getVariablesLocal(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  // TaskService#getVariables() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesTypedByName()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		// when
		VariableMap variables = taskService.getVariablesTyped(taskId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Empty);
		assertEquals(1, variables.size());

		assertEquals(VARIABLE_VALUE, variables.get(VARIABLE_NAME));
	  }

	  // TaskService#getVariablesLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskGetVariablesLocalTypedByName()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariablesLocal(taskId, Variables);
		enableAuthorization();

		// when
		IDictionary<string, object> variables = taskService.getVariablesLocalTyped(taskId, Arrays.asList(VARIABLE_NAME), false);

		// then
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	  // TaskService#setVariable() (case task) /////////////////////////////////////

	  public virtual void testCaseTaskSetVariable()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		verifySetVariable(taskId);
	  }

	  // TaskService#setVariableLocal() (case task) /////////////////////////////////////

	  public virtual void testCaseTaskSetVariableLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		verifySetVariableLocal(taskId);
	  }

	  // TaskService#setVariables() (case task) /////////////////////////////////////

	  public virtual void testCaseTaskSetVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		verifySetVariables(taskId);
	  }

	  // TaskService#setVariablesLocal() (case task) /////////////////////////////////////

	  public virtual void testCaseTaskSetVariablesLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		verifySetVariablesLocal(taskId);
	  }

	  // TaskService#removeVariable() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskRemoveVariable()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		verifyRemoveVariable(taskId);
	  }

	  // TaskService#removeVariableLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskRemoveVariableLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		enableAuthorization();

		verifyRemoveVariableLocal(taskId);
	  }

	  // TaskService#removeVariables() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskRemoveVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);
		string taskId = selectSingleTask().Id;

		verifyRemoveVariables(taskId);
	  }

	  // TaskService#removeVariablesLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskRemoveVariablesLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		enableAuthorization();

		verifyRemoveVariablesLocal(taskId);
	  }

	  // TaskServiceImpl#updateVariablesLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskUpdateVariablesLocal()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		verifyUpdateVariablesLocal(taskId);
	  }

	  // TaskServiceImpl#updateVariablesLocal() (case task) ////////////////////////////////////////////

	  public virtual void testCaseTaskUpdateVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;

		verifyUpdateVariables(taskId);
	  }

	  public virtual void testStandaloneTaskSaveWithGenericResourceIdOwner()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE);

		Task task = taskService.newTask();
		task.Owner = "*";

		try
		{
		  taskService.saveTask(task);
		  fail("it should not be possible to save a task with the generic resource id *");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot create default authorization for owner *: " + "id cannot be *. * is a reserved identifier", e.Message);
		}
	  }

	  public virtual void testStandaloneTaskSaveWithGenericResourceIdOwnerTaskServiceApi()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, UPDATE);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.setOwner(task.Id, "*");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot create default authorization for owner *: " + "id cannot be *. * is a reserved identifier", e.Message);
		}

		deleteTask(task.Id, true);
	  }

	  public virtual void testStandaloneTaskSaveWithGenericResourceIdAssignee()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE);

		Task task = taskService.newTask();
		task.Assignee = "*";

		try
		{
		  taskService.saveTask(task);
		  fail("it should not be possible to save a task with the generic resource id *");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot create default authorization for assignee *: " + "id cannot be *. * is a reserved identifier", e.Message);
		}
	  }

	  public virtual void testStandaloneTaskSaveWithGenericResourceIdAssigneeTaskServiceApi()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, UPDATE);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.setAssignee(task.Id, "*");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot create default authorization for assignee *: " + "id cannot be *. * is a reserved identifier", e.Message);
		}

		deleteTask(task.Id, true);
	  }

	  public virtual void testStandaloneTaskSaveIdentityLinkWithGenericUserId()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, UPDATE);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.addUserIdentityLink(task.Id, "*", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot grant default authorization for identity link to user *: " + "id cannot be *. * is a reserved identifier.", e.Message);
		}

		deleteTask(task.Id, true);
	  }

	  public virtual void testStandaloneTaskSaveIdentityLinkWithGenericGroupId()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, UPDATE);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.addGroupIdentityLink(task.Id, "*", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot grant default authorization for identity link to group *: " + "id cannot be *. * is a reserved identifier.", e.Message);
		}

		deleteTask(task.Id, true);
	  }

	  public virtual void testStandaloneTaskSaveIdentityLinkWithGenericGroupIdAndTaskAssignPermission()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, TASK_ASSIGN);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.addGroupIdentityLink(task.Id, "*", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot grant default authorization for identity link to group *: " + "id cannot be *. * is a reserved identifier.", e.Message);
		}

		deleteTask(task.Id, true);
	  }

	  public virtual void testStandaloneTaskSaveIdentityLinkWithGenericTaskId()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, UPDATE);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.addUserIdentityLink("*", "aUserId", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find task with id *", e.Message);
		}

		try
		{
		  taskService.addGroupIdentityLink("*", "aGroupId", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find task with id *", e.Message);
		}

		deleteTask(task.Id, true);
	  }

	  public virtual void testStandaloneTaskSaveIdentityLinkWithGenericTaskIdAndTaskAssignPermission()
	  {
		createGrantAuthorization(TASK, ANY, userId, CREATE, TASK_ASSIGN);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		try
		{
		  taskService.addUserIdentityLink("*", "aUserId", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find task with id *", e.Message);
		}

		try
		{
		  taskService.addGroupIdentityLink("*", "aGroupId", "someLink");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot find task with id *", e.Message);
		}

		deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetGenericResourceIdAssignee()
	  public virtual void testSetGenericResourceIdAssignee()
	  {
		createGrantAuthorization(Resources.PROCESS_DEFINITION, Authorization_Fields.ANY, userId, CREATE_INSTANCE);
		createGrantAuthorization(Resources.PROCESS_INSTANCE, Authorization_Fields.ANY, userId, CREATE);

		try
		{
		  runtimeService.startProcessInstanceByKey("genericResourceIdAssignmentProcess");
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot create default authorization for assignee *: " + "id cannot be *. * is a reserved identifier.", e.Message);
		}
	  }

	  public virtual void testAssignSameAssigneeAndOwnerToTask()
	  {

		// given
		createGrantAuthorization(Resources.TASK, Authorization_Fields.ANY, userId, Permissions.ALL);

		// when
		Task newTask = taskService.newTask();
		newTask.Assignee = "Horst";
		newTask.Owner = "Horst";

		// then
		try
		{
		  taskService.saveTask(newTask);
		}
		catch (Exception)
		{
		  fail("Setting same assignee and owner to user should not fail!");
		}

		taskService.deleteTask(newTask.Id, true);
	  }

	  public virtual void testPermissionsOnAssignSameAssigneeAndOwnerToTask()
	  {

		try
		{
		  // given
		  createGrantAuthorization(Resources.TASK, Authorization_Fields.ANY, userId, Permissions.CREATE, Permissions.DELETE, Permissions.READ);
		  processEngineConfiguration.ResourceAuthorizationProvider = new MyExtendedPermissionDefaultAuthorizationProvider();

		  // when
		  Task newTask = taskService.newTask();
		  newTask.Assignee = "Horst";
		  newTask.Owner = "Horst";
		  taskService.saveTask(newTask);

		  // then
		  Authorization auth = authorizationService.createAuthorizationQuery().userIdIn("Horst").singleResult();
		  assertTrue(auth.isPermissionGranted(Permissions.DELETE));

		  taskService.deleteTask(newTask.Id, true);

		}
		finally
		{
		  processEngineConfiguration.ResourceAuthorizationProvider = new DefaultAuthorizationProvider();
		}


	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAssignSameAssigneeAndOwnerToProcess()
	  public virtual void testAssignSameAssigneeAndOwnerToProcess()
	  {
		//given
		createGrantAuthorization(Resources.PROCESS_DEFINITION, Authorization_Fields.ANY, userId, Permissions.ALL);
		createGrantAuthorization(Resources.PROCESS_INSTANCE, Authorization_Fields.ANY, userId, Permissions.ALL);

		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		IList<Authorization> auths = authorizationService.createAuthorizationQuery().userIdIn("horst").list();
		assertTrue(auths.Count == 1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAssignSameUserToProcessTwice()
	  public virtual void testAssignSameUserToProcessTwice()
	  {
		//given
		createGrantAuthorization(Resources.PROCESS_DEFINITION, Authorization_Fields.ANY, userId, Permissions.ALL);
		createGrantAuthorization(Resources.PROCESS_INSTANCE, Authorization_Fields.ANY, userId, Permissions.ALL);

		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		IList<Authorization> auths = authorizationService.createAuthorizationQuery().userIdIn("hans").list();
		assertTrue(auths.Count == 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAssignSameGroupToProcessTwice()
	  public virtual void testAssignSameGroupToProcessTwice()
	  {
		//given
		createGrantAuthorization(Resources.PROCESS_DEFINITION, Authorization_Fields.ANY, userId, Permissions.ALL);
		createGrantAuthorization(Resources.PROCESS_INSTANCE, Authorization_Fields.ANY, userId, Permissions.ALL);

		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		IList<Authorization> auths = authorizationService.createAuthorizationQuery().groupIdIn("abc").list();
		assertTrue(auths.Count == 1);
	  }


	  // helper ////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(TaskQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyQueryResults(VariableInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyMessageIsValid(string taskId, string message)
	  {
		assertTextPresent(userId, message);
		assertTextPresent(UPDATE.Name, message);
		assertTextPresent(UPDATE_VARIABLE.Name, message);
		assertTextPresent(taskId, message);
		assertTextPresent(TASK.resourceName(), message);
		assertTextPresent(UPDATE_TASK.Name, message);
		assertTextPresent(UPDATE_TASK_VARIABLE.Name, message);
		assertTextPresent(PROCESS_KEY, message);
		assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
	  }

	  protected internal virtual void verifyVariableInstanceCountDisabledAuthorization(int count)
	  {
		disableAuthorization();
		verifyQueryResults(runtimeService.createVariableInstanceQuery(), count);
		enableAuthorization();
	  }

	  protected internal virtual void verifySetVariable(string taskId)
	  {
		// when
		taskService.setVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifySetVariableLocal(string taskId)
	  {
		// when
		taskService.setVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifySetVariables(string taskId)
	  {
		// when
		taskService.setVariables(taskId, Variables);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifySetVariablesLocal(string taskId)
	  {
		// when
		taskService.setVariablesLocal(taskId, Variables);

		// then
		verifyVariableInstanceCountDisabledAuthorization(1);
	  }

	  protected internal virtual void verifyRemoveVariable(string taskId)
	  {
		// when
		taskService.removeVariable(taskId, VARIABLE_NAME);

		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyRemoveVariableLocal(string taskId)
	  {
		// when
		taskService.removeVariableLocal(taskId, VARIABLE_NAME);

		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyRemoveVariables(string taskId)
	  {
		// when
		taskService.removeVariables(taskId, Arrays.asList(VARIABLE_NAME));

		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyRemoveVariablesLocal(string taskId)
	  {
		// when
		taskService.removeVariablesLocal(taskId, Arrays.asList(VARIABLE_NAME));

		// then
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyUpdateVariables(string taskId)
	  {
		// when (1)
		((TaskServiceImpl) taskService).updateVariables(taskId, Variables, null);

		// then (1)
		verifyVariableInstanceCountDisabledAuthorization(1);

		// when (2)
		((TaskServiceImpl) taskService).updateVariables(taskId, null, Arrays.asList(VARIABLE_NAME));

		// then (2)
		verifyVariableInstanceCountDisabledAuthorization(0);

		// when (3)
		((TaskServiceImpl) taskService).updateVariables(taskId, Variables, Arrays.asList(VARIABLE_NAME));

		// then (3)
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyUpdateVariablesLocal(string taskId)
	  {
		// when (1)
		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, Variables, null);

		// then (1)
		verifyVariableInstanceCountDisabledAuthorization(1);

		// when (2)
		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, null, Arrays.asList(VARIABLE_NAME));

		// then (2)
		verifyVariableInstanceCountDisabledAuthorization(0);

		// when (3)
		((TaskServiceImpl) taskService).updateVariablesLocal(taskId, Variables, Arrays.asList(VARIABLE_NAME));

		// then (3)
		verifyVariableInstanceCountDisabledAuthorization(0);
	  }

	  protected internal virtual void verifyGetVariables(IDictionary<string, object> variables)
	  {
		assertNotNull(variables);
		assertFalse(variables.Count == 0);
		assertEquals(1, variables.Count);

		assertEquals(VARIABLE_VALUE, variables[VARIABLE_NAME]);
	  }

	}

}