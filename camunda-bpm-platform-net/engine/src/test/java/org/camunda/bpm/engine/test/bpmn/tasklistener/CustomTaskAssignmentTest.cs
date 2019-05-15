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
namespace org.camunda.bpm.engine.test.bpmn.tasklistener
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge <falko.menge@camunda.com>
	/// @author Frederik Heremans
	/// </summary>
	public class CustomTaskAssignmentTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("fozzie"));
		identityService.saveUser(identityService.newUser("gonzo"));

		identityService.saveGroup(identityService.newGroup("management"));

		identityService.createMembership("kermit", "management");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		identityService.deleteUser("kermit");
		identityService.deleteUser("fozzie");
		identityService.deleteUser("gonzo");
		identityService.deleteGroup("management");
		base.tearDown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCandidateGroupAssignment()
	  public virtual void testCandidateGroupAssignment()
	  {
		runtimeService.startProcessInstanceByKey("customTaskAssignment");
		assertEquals(1, taskService.createTaskQuery().taskCandidateGroup("management").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser("kermit").count());
		assertEquals(0, taskService.createTaskQuery().taskCandidateUser("fozzie").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCandidateUserAssignment()
	  public virtual void testCandidateUserAssignment()
	  {
		runtimeService.startProcessInstanceByKey("customTaskAssignment");
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser("kermit").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser("fozzie").count());
		assertEquals(0, taskService.createTaskQuery().taskCandidateUser("gonzo").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAssigneeAssignment()
	  public virtual void testAssigneeAssignment()
	  {
		runtimeService.startProcessInstanceByKey("setAssigneeInListener");
		assertNotNull(taskService.createTaskQuery().taskAssignee("kermit").singleResult());
		assertEquals(0, taskService.createTaskQuery().taskAssignee("fozzie").count());
		assertEquals(0, taskService.createTaskQuery().taskAssignee("gonzo").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOverwriteExistingAssignments()
	  public virtual void testOverwriteExistingAssignments()
	  {
		runtimeService.startProcessInstanceByKey("overrideAssigneeInListener");
		assertNotNull(taskService.createTaskQuery().taskAssignee("kermit").singleResult());
		assertEquals(0, taskService.createTaskQuery().taskAssignee("fozzie").count());
		assertEquals(0, taskService.createTaskQuery().taskAssignee("gonzo").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testOverwriteExistingAssignmentsFromVariable()
	  public virtual void testOverwriteExistingAssignmentsFromVariable()
	  {
		// prepare variables
		IDictionary<string, string> assigneeMappingTable = new Dictionary<string, string>();
		assigneeMappingTable["fozzie"] = "gonzo";

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["assigneeMappingTable"] = assigneeMappingTable;

		// start process instance
		runtimeService.startProcessInstanceByKey("customTaskAssignment", variables);

		// check task lists
		assertNotNull(taskService.createTaskQuery().taskAssignee("gonzo").singleResult());
		assertEquals(0, taskService.createTaskQuery().taskAssignee("fozzie").count());
		assertEquals(0, taskService.createTaskQuery().taskAssignee("kermit").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testReleaseTask() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testReleaseTask()
	  {
		runtimeService.startProcessInstanceByKey("releaseTaskProcess");

		Task task = taskService.createTaskQuery().taskAssignee("fozzie").singleResult();
		assertNotNull(task);
		string taskId = task.Id;

		// Set assignee to null
		taskService.setAssignee(taskId, null);

		task = taskService.createTaskQuery().taskAssignee("fozzie").singleResult();
		assertNull(task);

		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertNotNull(task);
		assertNull(task.Assignee);
	  }

	}

}