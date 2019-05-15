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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>

	public class MultiTenancyTaskServiceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTaskServiceCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal static readonly BpmnModelInstance ONE_TASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;

	  protected internal Task task;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		testRule.deployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		task = engineRule.TaskService.createTaskQuery().singleResult();

		taskService = engineRule.TaskService;
		identityService = engineRule.IdentityService;
	  }

	  // save test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveTaskWithAuthenticatedTenant()
	  public virtual void saveTaskWithAuthenticatedTenant()
	  {

		task = taskService.newTask("newTask");
		task.TenantId = TENANT_ONE;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.saveTask(task);
		// then
		assertThat(taskService.createTaskQuery().taskId(task.Id).count(), @is(1L));

		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveTaskWithNoAuthenticatedTenant()
	  public virtual void saveTaskWithNoAuthenticatedTenant()
	  {

		task = taskService.newTask("newTask");
		task.TenantId = TENANT_ONE;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot create the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.saveTask(task);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveTaskWithDisabledTenantCheck()
	  public virtual void saveTaskWithDisabledTenantCheck()
	  {

		task = taskService.newTask("newTask");
		task.TenantId = TENANT_ONE;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		taskService.saveTask(task);
		// then
		assertThat(taskService.createTaskQuery().taskId(task.Id).count(), @is(1L));
		taskService.deleteTask(task.Id, true);
	  }

	  // update task test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTaskWithAuthenticatedTenant()
	  public virtual void updateTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		task.Assignee = "aUser";
		taskService.saveTask(task);

		// then
		assertThat(taskService.createTaskQuery().taskAssignee("aUser").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTaskWithNoAuthenticatedTenant()
	  public virtual void updateTaskWithNoAuthenticatedTenant()
	  {

		task.Assignee = "aUser";
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.saveTask(task);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTaskWithDisabledTenantCheck()
	  public virtual void updateTaskWithDisabledTenantCheck()
	  {

		task.Assignee = "aUser";
		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		taskService.saveTask(task);
		assertThat(taskService.createTaskQuery().taskAssignee("aUser").count(), @is(1L));

	  }

	  // claim task test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void claimTaskWithAuthenticatedTenant()
	  public virtual void claimTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		taskService.claim(task.Id, "bUser");
		assertThat(taskService.createTaskQuery().taskAssignee("bUser").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void claimTaskWithNoAuthenticatedTenant()
	  public virtual void claimTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot work on task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.claim(task.Id, "bUser");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void claimTaskWithDisableTenantCheck()
	  public virtual void claimTaskWithDisableTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		taskService.claim(task.Id, "bUser");
		assertThat(taskService.createTaskQuery().taskAssignee("bUser").count(), @is(1L));

	  }

	  // complete the task test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeTaskWithAuthenticatedTenant()
	  public virtual void completeTaskWithAuthenticatedTenant()
	  {
		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		taskService.complete(task.Id);
		assertThat(taskService.createTaskQuery().taskId(task.Id).active().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeTaskWithNoAuthenticatedTenant()
	  public virtual void completeTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot work on task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeWithDisabledTenantCheck()
	  public virtual void completeWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		taskService.complete(task.Id);
		assertThat(taskService.createTaskQuery().taskId(task.Id).active().count(), @is(0L));
	  }

	  // delegate task test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delegateTaskWithAuthenticatedTenant()
	  public virtual void delegateTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.delegateTask(task.Id, "demo");

		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delegateTaskWithNoAuthenticatedTenant()
	  public virtual void delegateTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.delegateTask(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delegateTaskWithDisabledTenantCheck()
	  public virtual void delegateTaskWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		taskService.delegateTask(task.Id, "demo");
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));
	  }

	  // resolve task test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveTaskWithAuthenticatedTenant()
	  public virtual void resolveTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.resolveTask(task.Id);

		assertThat(taskService.createTaskQuery().taskDelegationState(DelegationState.RESOLVED).taskId(task.Id).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveTaskWithNoAuthenticatedTenant()
	  public virtual void resolveTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot work on task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.resolveTask(task.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveTaskWithDisableTenantCheck()
	  public virtual void resolveTaskWithDisableTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		taskService.resolveTask(task.Id);
		assertThat(taskService.createTaskQuery().taskDelegationState(DelegationState.RESOLVED).taskId(task.Id).count(), @is(1L));
	  }

	  // set priority test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setPriorityForTaskWithAuthenticatedTenant()
	  public virtual void setPriorityForTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.setPriority(task.Id, 1);

		assertThat(taskService.createTaskQuery().taskPriority(1).taskId(task.Id).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setPriorityForTaskWithNoAuthenticatedTenant()
	  public virtual void setPriorityForTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.setPriority(task.Id, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setPriorityForTaskWithDisabledTenantCheck()
	  public virtual void setPriorityForTaskWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		taskService.setPriority(task.Id, 1);
		assertThat(taskService.createTaskQuery().taskPriority(1).taskId(task.Id).count(), @is(1L));
	  }

	  // delete task test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTaskWithAuthenticatedTenant()
	  public virtual void deleteTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		task = createTaskforTenant();
		assertThat(taskService.createTaskQuery().taskId(task.Id).count(), @is(1L));

		// then
		taskService.deleteTask(task.Id, true);
		assertThat(taskService.createTaskQuery().taskId(task.Id).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTaskWithNoAuthenticatedTenant()
	  public virtual void deleteTaskWithNoAuthenticatedTenant()
	  {

		try
		{
		  task = createTaskforTenant();
		  identityService.setAuthentication("aUserId", null);
		  // then
		  thrown.expect(typeof(ProcessEngineException));
		  thrown.expectMessage("Cannot delete the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		  taskService.deleteTask(task.Id, true);
		}
		finally
		{
		  identityService.clearAuthentication();
		  taskService.deleteTask(task.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTaskWithDisabledTenantCheck()
	  public virtual void deleteTaskWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		task = createTaskforTenant();
		assertThat(taskService.createTaskQuery().taskId(task.Id).count(), @is(1L));

		// then
		taskService.deleteTask(task.Id, true);
		assertThat(taskService.createTaskQuery().taskId(task.Id).count(), @is(0L));
	  }

	  protected internal virtual Task createTaskforTenant()
	  {
		Task newTask = taskService.newTask("newTask");
		newTask.TenantId = TENANT_ONE;
		taskService.saveTask(newTask);

		return newTask;

	  }
	}

}