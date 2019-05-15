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
namespace org.camunda.bpm.engine.test.api.task
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// Tests for cub-tasks querying
	/// @author Ionut Paduraru </summary>
	/// <seealso cref= TaskQueryTest  </seealso>
	public class SubTaskQueryTest : PluggableProcessEngineTestCase
	{

	  private IList<string> taskIds;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {

		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("gonzo"));

		identityService.saveGroup(identityService.newGroup("management"));
		identityService.saveGroup(identityService.newGroup("accountancy"));

		identityService.createMembership("kermit", "management");
		identityService.createMembership("kermit", "accountancy");

		taskIds = generateTestSubTasks();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		identityService.deleteGroup("accountancy");
		identityService.deleteGroup("management");
		identityService.deleteUser("gonzo");
		identityService.deleteUser("kermit");
		taskService.deleteTasks(taskIds, true);
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion (no other filters, no sort) 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryExcludeSubtasks() throws Exception
	  public virtual void testQueryExcludeSubtasks()
	  {
		// query all tasks, including subtasks
		TaskQuery query = taskService.createTaskQuery();
		assertEquals(10, query.count());
		assertEquals(10, query.list().size());
		// query only parent tasks (exclude subtasks)
		query = taskService.createTaskQuery().excludeSubtasks();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion (no other filters, no sort) 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryWithPagination() throws Exception
	  public virtual void testQueryWithPagination()
	  {
		// query all tasks, including subtasks
		TaskQuery query = taskService.createTaskQuery();
		assertEquals(10, query.count());
		assertEquals(2, query.listPage(0, 2).size());
		// query only parent tasks (exclude subtasks)
		query = taskService.createTaskQuery().excludeSubtasks();
		assertEquals(3, query.count());
		assertEquals(1, query.listPage(0, 1).size());
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion (no other filters, order by task assignee ) 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryExcludeSubtasksSorted() throws Exception
	  public virtual void testQueryExcludeSubtasksSorted()
	  {
		// query all tasks, including subtasks
		TaskQuery query = taskService.createTaskQuery().orderByTaskAssignee().asc();
		assertEquals(10, query.count());
		assertEquals(10, query.list().size());
		// query only parent tasks (exclude subtasks)
		query = taskService.createTaskQuery().excludeSubtasks().orderByTaskAssignee().desc();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion when additional filter is specified (like assignee), no order. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryByAssigneeExcludeSubtasks() throws Exception
	  public virtual void testQueryByAssigneeExcludeSubtasks()
	  {
		// gonzo has 2 root tasks and 3+2 subtasks assigned
		// include subtasks
		TaskQuery query = taskService.createTaskQuery().taskAssignee("gonzo");
		assertEquals(7, query.count());
		assertEquals(7, query.list().size());
		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("gonzo").excludeSubtasks();
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());

		// kermit has no root tasks and no subtasks assigned
		// include subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit");
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());
		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit").excludeSubtasks();
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion when additional filter is specified (like assignee), no order. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryByAssigneeExcludeSubtasksPaginated() throws Exception
	  public virtual void testQueryByAssigneeExcludeSubtasksPaginated()
	  {
		// gonzo has 2 root tasks and 3+2 subtasks assigned
		// include subtasks
		TaskQuery query = taskService.createTaskQuery().taskAssignee("gonzo");
		assertEquals(7, query.count());
		assertEquals(2, query.listPage(0, 2).size());
		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("gonzo").excludeSubtasks();
		assertEquals(2, query.count());
		assertEquals(1, query.listPage(0, 1).size());

		// kermit has no root tasks and no subtasks assigned
		// include subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit");
		assertEquals(0, query.count());
		assertEquals(0, query.listPage(0, 2).size());
		assertNull(query.singleResult());
		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit").excludeSubtasks();
		assertEquals(0, query.count());
		assertEquals(0, query.listPage(0, 2).size());
		assertNull(query.singleResult());
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion when additional filter is specified (like assignee), ordered. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryByAssigneeExcludeSubtasksOrdered() throws Exception
	  public virtual void testQueryByAssigneeExcludeSubtasksOrdered()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

		// gonzo has 2 root tasks and 3+2 subtasks assigned
		// include subtasks
		TaskQuery query = taskService.createTaskQuery().taskAssignee("gonzo").orderByTaskCreateTime().desc();
		assertEquals(7, query.count());
		assertEquals(7, query.list().size());
		assertEquals(sdf.parse("02/01/2009 01:01:01.000"), query.list().get(0).CreateTime);

		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("gonzo").excludeSubtasks().orderByTaskCreateTime().asc();
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
		assertEquals(sdf.parse("01/02/2008 02:02:02.000"), query.list().get(0).CreateTime);
		assertEquals(sdf.parse("05/02/2008 02:02:02.000"), query.list().get(1).CreateTime);

		// kermit has no root tasks and no subtasks assigned
		// include subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit").orderByTaskCreateTime().asc();
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());
		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit").excludeSubtasks().orderByTaskCreateTime().desc();
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());
	  }

	  /// <summary>
	  /// test for task inclusion/exclusion when additional filter is specified (like assignee), ordered. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryByAssigneeExcludeSubtasksOrderedAndPaginated() throws Exception
	  public virtual void testQueryByAssigneeExcludeSubtasksOrderedAndPaginated()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

		// gonzo has 2 root tasks and 3+2 subtasks assigned
		// include subtasks
		TaskQuery query = taskService.createTaskQuery().taskAssignee("gonzo").orderByTaskCreateTime().asc();
		assertEquals(7, query.count());
		assertEquals(1, query.listPage(0, 1).size());
		assertEquals(sdf.parse("01/02/2008 02:02:02.000"), query.listPage(0, 1).get(0).CreateTime);
		assertEquals(1, query.listPage(1, 1).size());
		assertEquals(sdf.parse("05/02/2008 02:02:02.000"), query.listPage(1, 1).get(0).CreateTime);
		assertEquals(2, query.listPage(0, 2).size());
		assertEquals(sdf.parse("01/02/2008 02:02:02.000"), query.listPage(0, 2).get(0).CreateTime);
		assertEquals(sdf.parse("05/02/2008 02:02:02.000"), query.listPage(0, 2).get(1).CreateTime);

		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("gonzo").excludeSubtasks().orderByTaskCreateTime().desc();
		assertEquals(2, query.count());
		assertEquals(1, query.listPage(1, 1).size());
		assertEquals(sdf.parse("01/02/2008 02:02:02.000"), query.listPage(1, 1).get(0).CreateTime);
		assertEquals(1, query.listPage(0, 1).size());
		assertEquals(sdf.parse("05/02/2008 02:02:02.000"), query.listPage(0, 1).get(0).CreateTime);

		// kermit has no root tasks and no subtasks assigned
		// include subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit").orderByTaskCreateTime().asc();
		assertEquals(0, query.count());
		assertEquals(0, query.listPage(0, 2).size());
		assertNull(query.singleResult());
		// exclude subtasks
		query = taskService.createTaskQuery().taskAssignee("kermit").excludeSubtasks().orderByTaskCreateTime().desc();
		assertEquals(0, query.count());
		assertEquals(0, query.listPage(0, 2).size());
		assertNull(query.singleResult());
	  }

	  /// <summary>
	  /// Generates some test sub-tasks to the tasks generated by <seealso cref="#generateTestTasks()"/>.<br/> 
	  /// - 1 root task where kermit is a candidate with 2 subtasks (both with kermit as candidate) <br/> 
	  /// - 2 root task where gonzo is assignee with 3 + 2 subtasks assigned to gonzo  
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.List<String> generateTestSubTasks() throws Exception
	  private IList<string> generateTestSubTasks()
	  {
		IList<string> ids = new List<string>();

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");
		// 1 parent task for kermit
		ClockUtil.CurrentTime = sdf.parse("01/01/2008 01:01:01.000");
		Task rootTask1 = taskService.newTask();
		rootTask1.Name = "rootTestTask";
		rootTask1.Description = "rootTestTask description";
		taskService.saveTask(rootTask1);
		ids.Add(rootTask1.Id);
		taskService.addCandidateUser(rootTask1.Id, "kermit");
		// 2 sub-tasks for the task above
		ClockUtil.CurrentTime = sdf.parse("01/01/2009 01:01:01.000");
		for (int i = 1; i <= 2; i++)
		{
		  Task subtask = taskService.newTask();
		  subtask.Name = "kermitSubTask" + i;
		  subtask.ParentTaskId = rootTask1.Id;
		  subtask.Description = "description for kermit sub-task" + i;
		  taskService.saveTask(subtask);
		  taskService.addCandidateUser(subtask.Id, "kermit");
		  ids.Add(subtask.Id);
		}

		// 2 parent tasks for gonzo
		  // first parent task for gonzo
		ClockUtil.CurrentTime = sdf.parse("01/02/2008 02:02:02.000");
		Task rootTask2 = taskService.newTask();
		rootTask2.Name = "gonzoRootTask1";
		rootTask2.Description = "gonzo Root task1 description";
		taskService.saveTask(rootTask2);
		taskService.setAssignee(rootTask2.Id, "gonzo");
		ids.Add(rootTask2.Id);
		  // second parent task for gonzo
		ClockUtil.CurrentTime = sdf.parse("05/02/2008 02:02:02.000");
		Task rootTask3 = taskService.newTask();
		rootTask3.Name = "gonzoRootTask2";
		rootTask3.Description = "gonzo Root task2 description";
		taskService.saveTask(rootTask3);
		taskService.setAssignee(rootTask3.Id, "gonzo");
		ids.Add(rootTask3.Id);
		// 3 sub-tasks for the first parent task
		ClockUtil.CurrentTime = sdf.parse("01/01/2009 01:01:01.000");
		for (int i = 1; i <= 3; i++)
		{
		  Task subtask = taskService.newTask();
		  subtask.Name = "gonzoSubTask1_" + i;
		  subtask.ParentTaskId = rootTask2.Id;
		  subtask.Description = "description for gonzo sub-task1_" + i;
		  taskService.saveTask(subtask);
		  taskService.setAssignee(subtask.Id, "gonzo");
		  ids.Add(subtask.Id);
		}
		// 2 sub-tasks for the second parent task
		ClockUtil.CurrentTime = sdf.parse("02/01/2009 01:01:01.000");
		for (int i = 1; i <= 2; i++)
		{
		  Task subtask = taskService.newTask();
		  subtask.Name = "gonzoSubTask2_" + i;
		  subtask.ParentTaskId = rootTask3.Id;
		  subtask.Description = "description for gonzo sub-task2_" + i;
		  taskService.saveTask(subtask);
		  taskService.setAssignee(subtask.Id, "gonzo");
		  ids.Add(subtask.Id);
		}
		return ids;
	  }

	}
}