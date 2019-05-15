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
	using User = org.camunda.bpm.engine.identity.User;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Stefan Hentschel
	/// 
	/// </summary>
	public class TaskCountByCandidateGroupsTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskCountByCandidateGroupsTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
		}


	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule processEngineTestRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
	  public RuleChain ruleChain;


	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;
	  protected internal AuthorizationService authorizationService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string userId = "user";
	  protected internal IList<string> tasks = new List<string>();
	  protected internal IList<string> tenants = Arrays.asList("tenant1", "tenant2");
	  protected internal IList<string> groups = Arrays.asList("aGroupId", "anotherGroupId");


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		taskService = processEngineRule.TaskService;
		identityService = processEngineRule.IdentityService;
		authorizationService = processEngineRule.AuthorizationService;
		processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;

		createTask(groups[0], tenants[0]);
		createTask(groups[0], tenants[1]);
		createTask(groups[1], tenants[1]);
		createTask(null, tenants[1]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (string taskId in tasks)
		{
		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnTaskCountsByGroup()
	  public virtual void shouldReturnTaskCountsByGroup()
	  {
		// when
		IList<TaskCountByCandidateGroupResult> results = taskService.createTaskReport().taskCountByCandidateGroup();

		// then
		assertEquals(3, results.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldProvideTaskCountForEachGroup()
	  public virtual void shouldProvideTaskCountForEachGroup()
	  {
		// when
		IList<TaskCountByCandidateGroupResult> results = taskService.createTaskReport().taskCountByCandidateGroup();

		// then
		foreach (TaskCountByCandidateGroupResult result in results)
		{
		  checkResultCount(result, null, 1);
		  checkResultCount(result, groups[0], 2);
		  checkResultCount(result, groups[1], 1);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldProvideGroupNameForEachGroup()
	  public virtual void shouldProvideGroupNameForEachGroup()
	  {
		// when
		IList<TaskCountByCandidateGroupResult> results = taskService.createTaskReport().taskCountByCandidateGroup();

		// then
		foreach (TaskCountByCandidateGroupResult result in results)
		{
		  assertTrue(checkResultName(result));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFetchCountOfTasksWithoutAssignee()
	  public virtual void shouldFetchCountOfTasksWithoutAssignee()
	  {
		// given
		User user = identityService.newUser(userId);
		identityService.saveUser(user);

		// when
		taskService.delegateTask(tasks[2], userId);
		IList<TaskCountByCandidateGroupResult> results = taskService.createTaskReport().taskCountByCandidateGroup();

		identityService.deleteUser(userId);

		// then
		assertEquals(2, results.Count);
	  }

	  protected internal virtual void createTask(string groupId, string tenantId)
	  {
		Task task = taskService.newTask();
		task.TenantId = tenantId;
		taskService.saveTask(task);

		if (!string.ReferenceEquals(groupId, null))
		{
		  taskService.addCandidateGroup(task.Id, groupId);
		}

		tasks.Add(task.Id);
	  }

	  protected internal virtual void checkResultCount(TaskCountByCandidateGroupResult result, string expectedResultName, int expectedResultCount)
	  {
		if ((string.ReferenceEquals(expectedResultName, null) && string.ReferenceEquals(result.GroupName, null)) || (!string.ReferenceEquals(result.GroupName, null) && result.GroupName.Equals(expectedResultName)))
		{
		  assertEquals(expectedResultCount, result.TaskCount);
		}
	  }

	  protected internal virtual bool checkResultName(TaskCountByCandidateGroupResult result)
	  {
		return string.ReferenceEquals(result.GroupName, null) || result.GroupName.Equals(groups[0]) || result.GroupName.Equals(groups[1]);
	  }
	}

}