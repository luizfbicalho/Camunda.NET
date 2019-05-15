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

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// Testcase for the non-spec extensions to the task candidate use case.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class TaskAssignmentExtensionsTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("gonzo"));
		identityService.saveUser(identityService.newUser("fozzie"));

		identityService.saveGroup(identityService.newGroup("management"));
		identityService.saveGroup(identityService.newGroup("accountancy"));

		identityService.createMembership("kermit", "management");
		identityService.createMembership("kermit", "accountancy");
		identityService.createMembership("fozzie", "management");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		identityService.deleteGroup("accountancy");
		identityService.deleteGroup("management");
		identityService.deleteUser("fozzie");
		identityService.deleteUser("gonzo");
		identityService.deleteUser("kermit");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAssigneeExtension()
	  public virtual void testAssigneeExtension()
	  {
		runtimeService.startProcessInstanceByKey("assigneeExtension");
		IList<Task> tasks = taskService.createTaskQuery().taskAssignee("kermit").list();
		assertEquals(1, tasks.Count);
		assertEquals("my task", tasks[0].Name);
	  }

	  public virtual void testDuplicateAssigneeDeclaration()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testDuplicateAssigneeDeclaration");
		  repositoryService.createDeployment().addClasspathResource(resource).deploy();
		  fail("Invalid BPMN 2.0 process should not parse, but it gets parsed sucessfully");
		}
		catch (ProcessEngineException)
		{
		  // Exception is to be expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCandidateUsersExtension()
	  public virtual void testCandidateUsersExtension()
	  {
		runtimeService.startProcessInstanceByKey("candidateUsersExtension");
		IList<Task> tasks = taskService.createTaskQuery().taskCandidateUser("kermit").list();
		assertEquals(1, tasks.Count);
		tasks = taskService.createTaskQuery().taskCandidateUser("gonzo").list();
		assertEquals(1, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCandidateGroupsExtension()
	  public virtual void testCandidateGroupsExtension()
	  {
		runtimeService.startProcessInstanceByKey("candidateGroupsExtension");

		// Bugfix check: potentially the query could return 2 tasks since
		// kermit is a member of the two candidate groups
		IList<Task> tasks = taskService.createTaskQuery().taskCandidateUser("kermit").list();
		assertEquals(1, tasks.Count);
		assertEquals("make profit", tasks[0].Name);

		tasks = taskService.createTaskQuery().taskCandidateUser("fozzie").list();
		assertEquals(1, tasks.Count);
		assertEquals("make profit", tasks[0].Name);

		// Test the task query find-by-candidate-group operation
		TaskQuery query = taskService.createTaskQuery();
		assertEquals(1, query.taskCandidateGroup("management").count());
		assertEquals(1, query.taskCandidateGroup("accountancy").count());
	  }

	  // Test where the candidate user extension is used together
	  // with the spec way of defining candidate users
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMixedCandidateUserDefinition()
	  public virtual void testMixedCandidateUserDefinition()
	  {
		runtimeService.startProcessInstanceByKey("mixedCandidateUser");

		IList<Task> tasks = taskService.createTaskQuery().taskCandidateUser("kermit").list();
		assertEquals(1, tasks.Count);

		tasks = taskService.createTaskQuery().taskCandidateUser("fozzie").list();
		assertEquals(1, tasks.Count);

		tasks = taskService.createTaskQuery().taskCandidateUser("gonzo").list();
		assertEquals(1, tasks.Count);

		tasks = taskService.createTaskQuery().taskCandidateUser("mispiggy").list();
		assertEquals(0, tasks.Count);
	  }

	}

}