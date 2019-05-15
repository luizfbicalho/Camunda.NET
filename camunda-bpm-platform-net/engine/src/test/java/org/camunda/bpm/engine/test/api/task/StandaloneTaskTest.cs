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

	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class StandaloneTaskTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		base.setUp();
		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("gonzo"));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		identityService.deleteUser("kermit");
		identityService.deleteUser("gonzo");
		base.tearDown();
	  }

	  public virtual void testCreateToComplete()
	  {

		// Create and save task
		Task task = taskService.newTask();
		task.Name = "testTask";
		taskService.saveTask(task);
		string taskId = task.Id;

		// Add user as candidate user
		taskService.addCandidateUser(taskId, "kermit");
		taskService.addCandidateUser(taskId, "gonzo");

		// Retrieve task list for jbarrez
		IList<Task> tasks = taskService.createTaskQuery().taskCandidateUser("kermit").list();
		assertEquals(1, tasks.Count);
		assertEquals("testTask", tasks[0].Name);

		// Retrieve task list for tbaeyens
		tasks = taskService.createTaskQuery().taskCandidateUser("gonzo").list();
		assertEquals(1, tasks.Count);
		assertEquals("testTask", tasks[0].Name);

		// Claim task
		taskService.claim(taskId, "kermit");

		// Tasks shouldn't appear in the candidate tasklists anymore
		assertTrue(taskService.createTaskQuery().taskCandidateUser("kermit").list().Empty);
		assertTrue(taskService.createTaskQuery().taskCandidateUser("gonzo").list().Empty);

		// Complete task
		taskService.deleteTask(taskId, true);

		// Task should be removed from runtime data
		// TODO: check for historic data when implemented!
		assertNull(taskService.createTaskQuery().taskId(taskId).singleResult());
	  }

	  public virtual void testOptimisticLockingThrownOnMultipleUpdates()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		// first modification
		Task task1 = taskService.createTaskQuery().taskId(taskId).singleResult();
		Task task2 = taskService.createTaskQuery().taskId(taskId).singleResult();

		task1.Description = "first modification";
		taskService.saveTask(task1);

		// second modification on the initial instance
		task2.Description = "second modification";
		try
		{
		  taskService.saveTask(task2);
		  fail("should get an exception here as the task was modified by someone else.");
		}
		catch (OptimisticLockingException)
		{
		  //  exception was thrown as expected
		}

		taskService.deleteTask(taskId, true);
	  }

	  // See http://jira.codehaus.org/browse/ACT-1290
	  public virtual void testRevisionUpdatedOnSave()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		assertEquals(1, ((TaskEntity) task).Revision);

		task.Description = "first modification";
		taskService.saveTask(task);
		assertEquals(2, ((TaskEntity) task).Revision);

		task.Description = "second modification";
		taskService.saveTask(task);
		assertEquals(3, ((TaskEntity) task).Revision);

		taskService.deleteTask(task.Id, true);
	  }

	  public virtual void testSaveTaskWithGenericResourceId()
	  {
		Task task = taskService.newTask("*");
		try
		{
		  taskService.saveTask(task);
		  fail("it should not be possible to save a task with the generic resource id *");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Entity Task[*] has an invalid id: id cannot be *. * is a reserved identifier", e.Message);
		}
	  }


	}

}