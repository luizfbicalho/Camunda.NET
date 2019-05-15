using System;
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
namespace org.camunda.bpm.engine.test.bpmn.gateway
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.either;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.collection.IsCollectionWithSize.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using CoreMatchers = org.hamcrest.CoreMatchers;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ParallelGatewayTest : PluggableProcessEngineTestCase
	{

	  /// <summary>
	  /// Case where there is a parallel gateway that splits into 3 paths of
	  /// execution, that are immediately joined, without any wait states in between.
	  /// In the end, no executions should be in the database.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSplitMergeNoWaitstates()
	  public virtual void testSplitMergeNoWaitstates()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("forkJoinNoWaitStates");
		assertTrue(processInstance.Ended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUnstructuredConcurrencyTwoForks()
	  public virtual void testUnstructuredConcurrencyTwoForks()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("unstructuredConcurrencyTwoForks");
		assertTrue(processInstance.Ended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUnstructuredConcurrencyTwoJoins()
	  public virtual void testUnstructuredConcurrencyTwoJoins()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("unstructuredConcurrencyTwoJoins");
		assertTrue(processInstance.Ended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkFollowedByOnlyEndEvents()
	  public virtual void testForkFollowedByOnlyEndEvents()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("forkFollowedByEndEvents");
		assertTrue(processInstance.Ended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedForksFollowedByEndEvents()
	  public virtual void testNestedForksFollowedByEndEvents()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nestedForksFollowedByEndEvents");
		assertTrue(processInstance.Ended);
	  }

	  // ACT-482
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedForkJoin()
	  public virtual void testNestedForkJoin()
	  {
		string pid = runtimeService.startProcessInstanceByKey("nestedForkJoin").Id;

		// After process startm, only task 0 should be active
		TaskQuery query = taskService.createTaskQuery().orderByTaskName().asc();
		IList<Task> tasks = query.list();
		assertEquals(1, tasks.Count);
		assertEquals("Task 0", tasks[0].Name);
		assertEquals(1, runtimeService.getActivityInstance(pid).ChildActivityInstances.Length);

		// Completing task 0 will create Task A and B
		taskService.complete(tasks[0].Id);
		tasks = query.list();
		assertEquals(2, tasks.Count);
		assertEquals("Task A", tasks[0].Name);
		assertEquals("Task B", tasks[1].Name);
		assertEquals(2, runtimeService.getActivityInstance(pid).ChildActivityInstances.Length);

		// Completing task A should not trigger any new tasks
		taskService.complete(tasks[0].Id);
		tasks = query.list();
		assertEquals(1, tasks.Count);
		assertEquals("Task B", tasks[0].Name);
		assertEquals(2, runtimeService.getActivityInstance(pid).ChildActivityInstances.Length);

		// Completing task B creates tasks B1 and B2
		taskService.complete(tasks[0].Id);
		tasks = query.list();
		assertEquals(2, tasks.Count);
		assertEquals("Task B1", tasks[0].Name);
		assertEquals("Task B2", tasks[1].Name);
		assertEquals(3, runtimeService.getActivityInstance(pid).ChildActivityInstances.Length);

		// Completing B1 and B2 will activate both joins, and process reaches task C
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);
		tasks = query.list();
		assertEquals(1, tasks.Count);
		assertEquals("Task C", tasks[0].Name);
		assertEquals(1, runtimeService.getActivityInstance(pid).ChildActivityInstances.Length);
	  }

	  /// <summary>
	  /// http://jira.codehaus.org/browse/ACT-1222
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testReceyclingExecutionWithCallActivity()
	  public virtual void testReceyclingExecutionWithCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("parent-process").Id;

		// After process start we have two tasks, one from the parent and one from
		// the sub process
		TaskQuery query = taskService.createTaskQuery().orderByTaskName().asc();
		IList<Task> tasks = query.list();
		assertEquals(2, tasks.Count);
		assertEquals("Another task", tasks[0].Name);
		assertEquals("Some Task", tasks[1].Name);

		// we complete the task from the parent process, the root execution is
		// receycled, the task in the sub process is still there
		taskService.complete(tasks[1].Id);
		tasks = query.list();
		assertEquals(1, tasks.Count);
		assertEquals("Another task", tasks[0].Name);

		// we end the task in the sub process and the sub process instance end is
		// propagated to the parent process
		taskService.complete(tasks[0].Id);
		assertEquals(0, taskService.createTaskQuery().count());

		// There is a QA config without history, so we cannot work with this:
		// assertEquals(1,
		// historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).finished().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompletingJoin()
	  public virtual void testCompletingJoin()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		assertTrue(processInstance.Ended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncParallelGateway()
	  public virtual void testAsyncParallelGateway()
	  {

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		assertNotNull(jobDefinition);
		assertEquals("parallelJoinEnd", jobDefinition.ActivityId);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertFalse(processInstance.Ended);

		// there are two jobs to continue the gateway:
		IList<Job> list = managementService.createJobQuery().list();
		assertEquals(2, list.Count);

		managementService.executeJob(list[0].Id);
		managementService.executeJob(list[1].Id);

		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncParallelGatewayAfterScopeTask()
	  public virtual void testAsyncParallelGatewayAfterScopeTask()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertFalse(processInstance.Ended);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		// there are two jobs to continue the gateway:
		IList<Job> list = managementService.createJobQuery().list();
		assertEquals(2, list.Count);

		managementService.executeJob(list[0].Id);
		managementService.executeJob(list[1].Id);

		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompletingJoinInSubProcess()
	  public virtual void testCompletingJoinInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		assertTrue(processInstance.Ended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelGatewayBeforeAndInSubProcess()
	  public virtual void testParallelGatewayBeforeAndInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertThat(tasks, hasSize(3));

		ActivityInstance instance = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(instance.ActivityName, @is("Process1"));
		ActivityInstance[] childActivityInstances = instance.ChildActivityInstances;
		foreach (ActivityInstance activityInstance in childActivityInstances)
		{
		  if (activityInstance.ActivityId.Equals("SubProcess_1"))
		  {
			ActivityInstance[] instances = activityInstance.ChildActivityInstances;
			foreach (ActivityInstance activityInstance2 in instances)
			{
			  assertThat(activityInstance2.ActivityName, @is(either(equalTo("Inner User Task 1")).or(CoreMatchers.equalTo<object>("Inner User Task 2"))));
			}
		  }
		  else
		  {
			assertThat(activityInstance.ActivityName, @is("Outer User Task"));
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkJoin()
	  public virtual void testForkJoin()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("forkJoin");
		TaskQuery query = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc();

		IList<Task> tasks = query.list();
		assertEquals(2, tasks.Count);
		// the tasks are ordered by name (see above)
		Task task1 = tasks[0];
		assertEquals("Receive Payment", task1.Name);
		Task task2 = tasks[1];
		assertEquals("Ship Order", task2.Name);

		// Completing both tasks will join the concurrent executions
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		tasks = query.list();
		assertEquals(1, tasks.Count);
		assertEquals("Archive Order", tasks[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUnbalancedForkJoin()
	  public virtual void testUnbalancedForkJoin()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("UnbalancedForkJoin");
		TaskQuery query = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc();

		IList<Task> tasks = query.list();
		assertEquals(3, tasks.Count);
		// the tasks are ordered by name (see above)
		Task task1 = tasks[0];
		assertEquals("Task 1", task1.Name);
		Task task2 = tasks[1];
		assertEquals("Task 2", task2.Name);

		// Completing the first task should *not* trigger the join
		taskService.complete(task1.Id);

		// Completing the second task should trigger the first join
		taskService.complete(task2.Id);

		tasks = query.list();
		Task task3 = tasks[0];
		assertEquals(2, tasks.Count);
		assertEquals("Task 3", task3.Name);
		Task task4 = tasks[1];
		assertEquals("Task 4", task4.Name);

		// Completing the remaing tasks should trigger the second join and end the process
		taskService.complete(task3.Id);
		taskService.complete(task4.Id);

		assertProcessEnded(pi.Id);
	  }

	  public virtual void testRemoveConcurrentExecutionLocalVariablesOnJoin()
	  {
		deployment(Bpmn.createExecutableProcess("process").startEvent().parallelGateway("fork").userTask("task1").parallelGateway("join").userTask("afterTask").endEvent().moveToNode("fork").userTask("task2").connectTo("join").done());

		// given
		runtimeService.startProcessInstanceByKey("process");

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  runtimeService.setVariableLocal(task.ExecutionId, "var", "value");
		}

		// when
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// then
		assertEquals(0, runtimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testImplicitParallelGatewayAfterSignalBehavior()
	  public virtual void testImplicitParallelGatewayAfterSignalBehavior()
	  {
		// given
		Exception exceptionOccurred = null;
		runtimeService.startProcessInstanceByKey("process");
		Execution execution = runtimeService.createExecutionQuery().activityId("service").singleResult();

		// when
		try
		{
		  runtimeService.signal(execution.Id);
		}
		catch (Exception e)
		{
		  exceptionOccurred = e;
		}

		// then
		assertNull(exceptionOccurred);
		assertEquals(3, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExplicitParallelGatewayAfterSignalBehavior()
	  public virtual void testExplicitParallelGatewayAfterSignalBehavior()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");
		Execution execution = runtimeService.createExecutionQuery().activityId("service").singleResult();

		// when
		runtimeService.signal(execution.Id);

		// then
		assertEquals(3, taskService.createTaskQuery().count());
	  }
	}

}