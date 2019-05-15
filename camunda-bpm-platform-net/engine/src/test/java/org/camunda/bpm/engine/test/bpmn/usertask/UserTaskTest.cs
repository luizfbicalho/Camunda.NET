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

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class UserTaskTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		identityService.saveUser(identityService.newUser("fozzie"));
		identityService.saveUser(identityService.newUser("kermit"));

		identityService.saveGroup(identityService.newGroup("accountancy"));
		identityService.saveGroup(identityService.newGroup("management"));

		identityService.createMembership("fozzie", "accountancy");
		identityService.createMembership("kermit", "management");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		identityService.deleteUser("fozzie");
		identityService.deleteUser("kermit");
		identityService.deleteGroup("accountancy");
		identityService.deleteGroup("management");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskPropertiesNotNull()
	  public virtual void testTaskPropertiesNotNull()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(processInstance.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task.Id);
		assertEquals("my task", task.Name);
		assertEquals("Very important", task.Description);
		assertTrue(task.Priority > 0);
		assertEquals("kermit", task.Assignee);
		assertEquals(processInstance.Id, task.ProcessInstanceId);
		assertEquals(processInstance.Id, task.ExecutionId);
		assertNotNull(task.ProcessDefinitionId);
		assertNotNull(task.TaskDefinitionKey);
		assertNotNull(task.CreateTime);

		// the next test verifies that if an execution creates a task, that no events are created during creation of the task.
		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  assertEquals(0, taskService.getTaskEvents(task.Id).Count);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQuerySortingWithParameter()
	  public virtual void testQuerySortingWithParameter()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertEquals(1, taskService.createTaskQuery().processInstanceId(processInstance.Id).list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompleteAfterParallelGateway() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCompleteAfterParallelGateway()
	  {
		  // related to http://jira.codehaus.org/browse/ACT-1054

		  // start the process
		runtimeService.startProcessInstanceByKey("ForkProcess");
		IList<Task> taskList = taskService.createTaskQuery().list();
		assertNotNull(taskList);
		assertEquals(2, taskList.Count);

		// make sure user task exists
		Task task = taskService.createTaskQuery().taskDefinitionKey("SimpleUser").singleResult();
		  assertNotNull(task);

		  // attempt to complete the task and get PersistenceException pointing to "referential integrity constraint violation"
		  taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testComplexScenarioWithSubprocessesAndParallelGateways()
	  public virtual void testComplexScenarioWithSubprocessesAndParallelGateways()
	  {
		runtimeService.startProcessInstanceByKey("processWithSubProcessesAndParallelGateways");

		IList<Task> taskList = taskService.createTaskQuery().list();
		assertNotNull(taskList);
		assertEquals(13, taskList.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleProcess()
	  public virtual void testSimpleProcess()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("financialReport");

		IList<Task> tasks = taskService.createTaskQuery().taskCandidateUser("fozzie").list();
		assertEquals(1, tasks.Count);
		Task task = tasks[0];
		assertEquals("Write monthly financial report", task.Name);

		taskService.claim(task.Id, "fozzie");
		tasks = taskService.createTaskQuery().taskAssignee("fozzie").list();

		assertEquals(1, tasks.Count);
		taskService.complete(task.Id);

		tasks = taskService.createTaskQuery().taskCandidateUser("fozzie").list();
		assertEquals(0, tasks.Count);
		tasks = taskService.createTaskQuery().taskCandidateUser("kermit").list();
		assertEquals(1, tasks.Count);
		assertEquals("Verify monthly financial report", tasks[0].Name);
		taskService.complete(tasks[0].Id);

		assertProcessEnded(processInstance.Id);
	  }
	}

}