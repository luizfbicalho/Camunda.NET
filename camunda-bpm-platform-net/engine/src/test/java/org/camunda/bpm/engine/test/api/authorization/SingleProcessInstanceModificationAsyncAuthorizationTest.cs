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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.BATCH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;

	public class SingleProcessInstanceModificationAsyncAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PARALLEL_GATEWAY_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.parallelGateway.bpmn20.xml";

	  public override void tearDown()
	  {
		disableAuthorization();
		IList<Batch> batches = managementService.createBatchQuery().list();
		foreach (Batch batch in batches)
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  managementService.deleteJob(job.Id);
		}
		enableAuthorization();
		base.tearDown();
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testModificationWithAllPermissions()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "parallelGateway", userId, CREATE_INSTANCE, READ_INSTANCE, UPDATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(BATCH, ANY, userId, CREATE);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");
		string processInstanceId = processInstance.Id;
		string processDefinitionId = processInstance.ProcessDefinitionId;
		disableAuthorization();
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionId(processDefinitionId).singleResult();
		enableAuthorization();

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		// when
		Batch modificationBatch = runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).executeAsync();
		assertNotNull(modificationBatch);
		Job job = managementService.createJobQuery().jobDefinitionId(modificationBatch.SeedJobDefinitionId).singleResult();
		// seed job
		managementService.executeJob(job.Id);

		// then
		foreach (Job pending in managementService.createJobQuery().jobDefinitionId(modificationBatch.BatchJobDefinitionId).list())
		{
		  managementService.executeJob(pending.Id);
		  assertEquals(processDefinition.DeploymentId, pending.DeploymentId);
		}

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("task2").done());

		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);

		assertThat(executionTree).matches(describeExecutionTree("task2").scope().done());

		// complete the process
		disableAuthorization();
		completeTasksInOrder("task2");
		assertProcessEnded(processInstanceId);
		enableAuthorization();
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testModificationWithoutBatchPermissions()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "parallelGateway", userId, CREATE_INSTANCE, READ_INSTANCE, UPDATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		try
		{
		  // when
		  runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).executeAsync();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("The user with id 'test' does not have"));
		  assertTrue(e.Message.contains("'CREATE' permission on resource 'Batch'"));
		  assertTrue(e.Message.contains("'CREATE_BATCH_MODIFY_PROCESS_INSTANCES' permission on resource 'Batch'"));
		}
	  }

	  [Deployment(resources : PARALLEL_GATEWAY_PROCESS)]
	  public virtual void testModificationRevoke()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "parallelGateway", userId, CREATE_INSTANCE, READ_INSTANCE, UPDATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);
		createGrantAuthorization(BATCH, ANY, userId, CREATE);
		createRevokeAuthorization(BATCH, ANY, userId, CREATE_BATCH_MODIFY_PROCESS_INSTANCES);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("parallelGateway");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		try
		{
		  // when
		  runtimeService.createProcessInstanceModification(processInstance.Id).cancelActivityInstance(getInstanceIdForActivity(tree, "task1")).executeAsync();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("The user with id 'test' does not have"));
		  assertTrue(e.Message.contains("'CREATE' permission on resource 'Batch'"));
		  assertTrue(e.Message.contains("'CREATE_BATCH_MODIFY_PROCESS_INSTANCES' permission on resource 'Batch'"));
		}
	  }

	  protected internal virtual string getInstanceIdForActivity(ActivityInstance activityInstance, string activityId)
	  {
		ActivityInstance instance = getChildInstanceForActivity(activityInstance, activityId);
		if (instance != null)
		{
		  return instance.Id;
		}
		return null;
	  }


	  protected internal virtual ActivityInstance getChildInstanceForActivity(ActivityInstance activityInstance, string activityId)
	  {
		if (activityId.Equals(activityInstance.ActivityId))
		{
		  return activityInstance;
		}

		foreach (ActivityInstance childInstance in activityInstance.ChildActivityInstances)
		{
		  ActivityInstance instance = getChildInstanceForActivity(childInstance, activityId);
		  if (instance != null)
		  {
			return instance;
		  }
		}

		return null;
	  }

	  protected internal virtual void completeTasksInOrder(params string[] taskNames)
	  {
		foreach (string taskName in taskNames)
		{
		  // complete any task with that name
		  IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey(taskName).listPage(0, 1);
		  assertTrue("task for activity " + taskName + " does not exist", tasks.Count > 0);
		  taskService.complete(tasks[0].Id);
		}
	  }

	}

}