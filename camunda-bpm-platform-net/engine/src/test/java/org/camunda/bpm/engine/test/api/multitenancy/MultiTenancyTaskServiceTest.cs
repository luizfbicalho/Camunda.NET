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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using Task = org.camunda.bpm.engine.task.Task;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MultiTenancyTaskServiceTest : PluggableProcessEngineTestCase
	{

	  private const string tenant1 = "the-tenant-1";
	  private const string tenant2 = "the-tenant-2";

	  public virtual void testStandaloneTaskCreateWithTenantId()
	  {

		// given a transient task with tenant id
		Task task = taskService.newTask();
		task.TenantId = tenant1;

		// if
		// it is saved
		taskService.saveTask(task);

		// then
		// when I load it, the tenant id is preserved
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals(tenant1, task.TenantId);

		// Finally, delete task
		deleteTasks(task);
	  }

	  public virtual void testStandaloneTaskCannotChangeTenantIdIfNull()
	  {

		// given a persistent task without tenant id
		Task task = taskService.newTask();
		taskService.saveTask(task);
		task = taskService.createTaskQuery().singleResult();

		// if
		// change the tenant id
		task.TenantId = tenant1;

		// then
		// an exception is thrown on 'save'
		try
		{
		  taskService.saveTask(task);
		  fail("Expected an exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-03072 Cannot change tenantId of Task", e.Message);
		}

		// Finally, delete task
		deleteTasks(task);
	  }

	  public virtual void testStandaloneTaskCannotChangeTenantId()
	  {

		// given a persistent task with tenant id
		Task task = taskService.newTask();
		task.TenantId = tenant1;
		taskService.saveTask(task);
		task = taskService.createTaskQuery().singleResult();

		// if
		// change the tenant id
		task.TenantId = tenant2;

		// then
		// an exception is thrown on 'save'
		try
		{
		  taskService.saveTask(task);
		  fail("Expected an exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-03072 Cannot change tenantId of Task", e.Message);
		}

		// Finally, delete task
		deleteTasks(task);
	  }

	  public virtual void testStandaloneTaskCannotSetDifferentTenantIdOnSubTask()
	  {

		// given a persistent task with a tenant id
		Task task = taskService.newTask();
		task.TenantId = tenant1;
		taskService.saveTask(task);

		// if
		// I create a subtask with a different tenant id
		Task subTask = taskService.newTask();
		subTask.ParentTaskId = task.Id;
		subTask.TenantId = tenant2;

		// then an exception is thrown on save
		try
		{
		  taskService.saveTask(subTask);
		  fail("Exception expected.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-03073 Cannot set different tenantId on subtask than on parent Task", e.Message);
		}
		// Finally, delete task
		deleteTasks(task);
	  }

	  public virtual void testStandaloneTaskCannotSetDifferentTenantIdOnSubTaskWithNull()
	  {

		// given a persistent task without tenant id
		Task task = taskService.newTask();
		taskService.saveTask(task);

		// if
		// I create a subtask with a different tenant id
		Task subTask = taskService.newTask();
		subTask.ParentTaskId = task.Id;
		subTask.TenantId = tenant1;

		// then an exception is thrown on save
		try
		{
		  taskService.saveTask(subTask);
		  fail("Exception expected.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-03073 Cannot set different tenantId on subtask than on parent Task", e.Message);
		}
		// Finally, delete task
		deleteTasks(task);
	  }

	  public virtual void testStandaloneTaskPropagateTenantIdToSubTask()
	  {

		// given a persistent task with a tenant id
		Task task = taskService.newTask();
		task.TenantId = tenant1;
		taskService.saveTask(task);

		// if
		// I create a subtask without tenant id
		Task subTask = taskService.newTask();
		subTask.ParentTaskId = task.Id;
		taskService.saveTask(subTask);

		// then
		// the parent task's tenant id is propagated to the sub task
		subTask = taskService.createTaskQuery().taskId(subTask.Id).singleResult();
		assertEquals(tenant1, subTask.TenantId);

		// Finally, delete task
		deleteTasks(subTask, task);
	  }

	  public virtual void testStandaloneTaskPropagatesTenantIdToVariableInstance()
	  {
		// given a task with tenant id
		Task task = taskService.newTask();
		task.TenantId = tenant1;
		taskService.saveTask(task);

		// if we set a variable for the task
		taskService.setVariable(task.Id, "var", "test");

		// then a variable instance with the same tenant id is created
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		assertThat(variableInstance.TenantId, @is(tenant1));

		deleteTasks(task);
	  }

	  public virtual void testGetIdentityLinkWithTenantIdForCandidateUsers()
	  {

		// given
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().userTask("task").camundaCandidateUsers("aUserId").endEvent().done();

		deploymentForTenant("tenant", oneTaskProcess);

		ProcessInstance tenantProcessInstance = runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId("tenant").execute();

		Task tenantTask = taskService.createTaskQuery().processInstanceId(tenantProcessInstance.Id).singleResult();

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(tenantTask.Id);
		assertEquals(identityLinks.Count,1);
		assertEquals(identityLinks[0].TenantId, "tenant");
	  }

	  public virtual void testGetIdentityLinkWithTenantIdForCandidateGroup()
	  {

		// given
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().userTask("task").camundaCandidateGroups("aGroupId").endEvent().done();

		deploymentForTenant("tenant", oneTaskProcess);

		ProcessInstance tenantProcessInstance = runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId("tenant").execute();

		Task tenantTask = taskService.createTaskQuery().processInstanceId(tenantProcessInstance.Id).singleResult();

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(tenantTask.Id);
		assertEquals(identityLinks.Count,1);
		assertEquals(identityLinks[0].TenantId, "tenant");
	  }

	  protected internal virtual void deleteTasks(params Task[] tasks)
	  {
		foreach (Task task in tasks)
		{
		  taskService.deleteTask(task.Id, true);
		}
	  }

	}

}