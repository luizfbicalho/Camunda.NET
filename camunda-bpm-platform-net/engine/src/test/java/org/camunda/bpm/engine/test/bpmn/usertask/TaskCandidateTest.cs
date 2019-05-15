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
namespace org.camunda.bpm.engine.test.bpmn.usertask
{

	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TaskCandidateTest : PluggableProcessEngineTestCase
	{

	  private const string KERMIT = "kermit";

	  private const string GONZO = "gonzo";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		base.setUp();

		Group accountants = identityService.newGroup("accountancy");
		identityService.saveGroup(accountants);
		Group managers = identityService.newGroup("management");
		identityService.saveGroup(managers);
		Group sales = identityService.newGroup("sales");
		identityService.saveGroup(sales);

		User kermit = identityService.newUser(KERMIT);
		identityService.saveUser(kermit);
		identityService.createMembership(KERMIT, "accountancy");

		User gonzo = identityService.newUser(GONZO);
		identityService.saveUser(gonzo);
		identityService.createMembership(GONZO, "management");
		identityService.createMembership(GONZO, "accountancy");
		identityService.createMembership(GONZO, "sales");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		identityService.deleteUser(KERMIT);
		identityService.deleteUser(GONZO);
		identityService.deleteGroup("sales");
		identityService.deleteGroup("accountancy");
		identityService.deleteGroup("management");

		base.tearDown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleCandidateGroup()
	  public virtual void testSingleCandidateGroup()
	  {

		// Deploy and start process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("singleCandidateGroup");

		// Task should not yet be assigned to kermit
		IList<Task> tasks = taskService.createTaskQuery().taskAssignee(KERMIT).list();
		assertTrue(tasks.Count == 0);

		// The task should be visible in the candidate task list
		tasks = taskService.createTaskQuery().taskCandidateUser(KERMIT).list();
		assertEquals(1, tasks.Count);
		Task task = tasks[0];
		assertEquals("Pay out expenses", task.Name);

		// Claim the task
		taskService.claim(task.Id, KERMIT);

		// The task must now be gone from the candidate task list
		tasks = taskService.createTaskQuery().taskCandidateUser(KERMIT).list();
		assertTrue(tasks.Count == 0);

		// The task will be visible on the personal task list
		tasks = taskService.createTaskQuery().taskAssignee(KERMIT).list();
		assertEquals(1, tasks.Count);
		task = tasks[0];
		assertEquals("Pay out expenses", task.Name);

		// Completing the task ends the process
		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleCandidateGroups()
	  public virtual void testMultipleCandidateGroups()
	  {

		// Deploy and start process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("multipleCandidatesGroup");

		// Task should not yet be assigned to anyone
		IList<Task> tasks = taskService.createTaskQuery().taskAssignee(KERMIT).list();

		assertTrue(tasks.Count == 0);
		tasks = taskService.createTaskQuery().taskAssignee(GONZO).list();

		assertTrue(tasks.Count == 0);

		// The task should be visible in the candidate task list of Gonzo and Kermit
		// and anyone in the management/accountancy group
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser(KERMIT).list().size());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser(GONZO).list().size());
		assertEquals(1, taskService.createTaskQuery().taskCandidateGroup("management").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateGroup("accountancy").count());
		assertEquals(0, taskService.createTaskQuery().taskCandidateGroup("sales").count());

		// Gonzo claims the task
		tasks = taskService.createTaskQuery().taskCandidateUser(GONZO).list();
		Task task = tasks[0];
		assertEquals("Approve expenses", task.Name);
		taskService.claim(task.Id, GONZO);

		// The task must now be gone from the candidate task lists
		assertTrue(taskService.createTaskQuery().taskCandidateUser(KERMIT).list().Empty);
		assertTrue(taskService.createTaskQuery().taskCandidateUser(GONZO).list().Empty);
		assertEquals(0, taskService.createTaskQuery().taskCandidateGroup("management").count());

		// The task will be visible on the personal task list of Gonzo
		assertEquals(1, taskService.createTaskQuery().taskAssignee(GONZO).count());

		// But not on the personal task list of (for example) Kermit
		assertEquals(0, taskService.createTaskQuery().taskAssignee(KERMIT).count());

		// Completing the task ends the process
		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleCandidateUsers()
	  public virtual void testMultipleCandidateUsers()
	  {
		runtimeService.startProcessInstanceByKey("multipleCandidateUsersExample");

		assertEquals(1, taskService.createTaskQuery().taskCandidateUser(GONZO).list().size());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser(KERMIT).list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMixedCandidateUserAndGroup()
	  public virtual void testMixedCandidateUserAndGroup()
	  {
		runtimeService.startProcessInstanceByKey("mixedCandidateUserAndGroupExample");

		assertEquals(1, taskService.createTaskQuery().taskCandidateUser(GONZO).list().size());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser(KERMIT).list().size());
	  }

	}

}