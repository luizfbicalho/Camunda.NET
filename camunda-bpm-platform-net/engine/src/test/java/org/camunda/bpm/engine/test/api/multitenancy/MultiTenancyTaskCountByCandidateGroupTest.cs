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
namespace org.camunda.bpm.engine.test.api.multitenancy
{
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

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class MultiTenancyTaskCountByCandidateGroupTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTaskCountByCandidateGroupTest()
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

	  protected internal string userId = "aUser";
	  protected internal string groupId = "aGroup";
	  protected internal string tenantId = "aTenant";
	  protected internal string anotherTenantId = "anotherTenant";

	  protected internal IList<string> taskIds = new List<string>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		taskService = processEngineRule.TaskService;
		identityService = processEngineRule.IdentityService;
		authorizationService = processEngineRule.AuthorizationService;
		processEngineConfiguration = processEngineRule.ProcessEngineConfiguration;

		createTask(groupId, tenantId);
		createTask(groupId, anotherTenantId);
		createTask(groupId, anotherTenantId);

		processEngineConfiguration.TenantCheckEnabled = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;

		foreach (string taskId in taskIds)
		{
		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldOnlyShowTenantSpecificTasks()
	  public virtual void shouldOnlyShowTenantSpecificTasks()
	  {
		// given

		identityService.setAuthentication(userId, null, Collections.singletonList(tenantId));

		// when
		IList<TaskCountByCandidateGroupResult> results = taskService.createTaskReport().taskCountByCandidateGroup();

		// then
		assertEquals(1, results.Count);
	  }

	  protected internal virtual void createTask(string groupId, string tenantId)
	  {
		Task task = taskService.newTask();
		task.TenantId = tenantId;
		taskService.saveTask(task);

		if (!string.ReferenceEquals(groupId, null))
		{
		  taskService.addCandidateGroup(task.Id, groupId);
		  taskIds.Add(task.Id);
		}
	  }
	}

}