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
namespace org.camunda.bpm.engine.test.bpmn.subprocess
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using GetActInstanceDelegate = org.camunda.bpm.engine.test.bpmn.subprocess.util.GetActInstanceDelegate;
	using ActivityInstanceAssert = org.camunda.bpm.engine.test.util.ActivityInstanceAssert;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public class SubProcessTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleSubProcess()
	  public virtual void testSimpleSubProcess()
	  {

		// After staring the process, the task in the subprocess should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleSubProcess");
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// we have 3 levels in the activityInstance:
		// pd
		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		//subprocess
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
		assertEquals("subProcess", subProcessInstance.ActivityId);
		// usertask
		assertEquals(1, subProcessInstance.ChildActivityInstances.Length);
		ActivityInstance userTaskInstance = subProcessInstance.ChildActivityInstances[0];
		assertEquals("subProcessTask", userTaskInstance.ActivityId);

		// After completing the task in the subprocess,
		// the subprocess scope is destroyed and the complete process ends
		taskService.complete(subProcessTask.Id);
		assertNull(runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).singleResult());
	  }

	  /// <summary>
	  /// Same test case as before, but now with all automatic steps
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleAutomaticSubProcess()
	  public virtual void testSimpleAutomaticSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleSubProcessAutomatic");
		assertTrue(pi.Ended);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleSubProcessWithTimer()
	  public virtual void testSimpleSubProcessWithTimer()
	  {

		DateTime startTime = DateTime.Now;

		// After staring the process, the task in the subprocess should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleSubProcess");
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// we have 3 levels in the activityInstance:
		// pd
		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		//subprocess
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
		assertEquals("subProcess", subProcessInstance.ActivityId);
		// usertask
		assertEquals(1, subProcessInstance.ChildActivityInstances.Length);
		ActivityInstance userTaskInstance = subProcessInstance.ChildActivityInstances[0];
		assertEquals("subProcessTask", userTaskInstance.ActivityId);

		// Setting the clock forward 2 hours 1 second (timer fires in 2 hours) and fire up the job executor
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + (2 * 60 * 60 * 1000) + 1000);
		waitForJobExecutorToProcessAllJobs(5000L);

		// The subprocess should be left, and the escalated task should be active
		Task escalationTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Fix escalated problem", escalationTask.Name);
	  }

	  /// <summary>
	  /// A test case that has a timer attached to the subprocess,
	  /// where 2 concurrent paths are defined when the timer fires.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void IGNORE_testSimpleSubProcessWithConcurrentTimer()
	  public virtual void IGNORE_testSimpleSubProcessWithConcurrentTimer()
	  {

		// After staring the process, the task in the subprocess should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleSubProcessWithConcurrentTimer");
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc();

		Task subProcessTask = taskQuery.singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// When the timer is fired (after 2 hours), two concurrent paths should be created
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		IList<Task> tasksAfterTimer = taskQuery.list();
		assertEquals(2, tasksAfterTimer.Count);
		Task taskAfterTimer1 = tasksAfterTimer[0];
		Task taskAfterTimer2 = tasksAfterTimer[1];
		assertEquals("Task after timer 1", taskAfterTimer1.Name);
		assertEquals("Task after timer 2", taskAfterTimer2.Name);

		// Completing the two tasks should end the process instance
		taskService.complete(taskAfterTimer1.Id);
		taskService.complete(taskAfterTimer2.Id);
		assertProcessEnded(pi.Id);
	  }

	  /// <summary>
	  /// Test case where the simple sub process of previous test cases
	  /// is nested within another subprocess.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSimpleSubProcess()
	  public virtual void testNestedSimpleSubProcess()
	  {

		// Start and delete a process with a nested subprocess when it is not yet ended
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nestedSimpleSubProcess", CollectionUtil.singletonMap("someVar", "abc"));
		runtimeService.deleteProcessInstance(pi.Id, "deleted");

		// After staring the process, the task in the inner subprocess must be active
		pi = runtimeService.startProcessInstanceByKey("nestedSimpleSubProcess");
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// now we have 4 levels in the activityInstance:
		// pd
		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		//subprocess1
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance1 = rootActivityInstance.ChildActivityInstances[0];
		assertEquals("outerSubProcess", subProcessInstance1.ActivityId);
		//subprocess2
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance2 = subProcessInstance1.ChildActivityInstances[0];
		assertEquals("innerSubProcess", subProcessInstance2.ActivityId);
		// usertask
		assertEquals(1, subProcessInstance2.ChildActivityInstances.Length);
		ActivityInstance userTaskInstance = subProcessInstance2.ChildActivityInstances[0];
		assertEquals("innerSubProcessTask", userTaskInstance.ActivityId);

		// After completing the task in the subprocess,
		// both subprocesses are destroyed and the task after the subprocess should be active
		taskService.complete(subProcessTask.Id);
		Task taskAfterSubProcesses = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(taskAfterSubProcesses);
		assertEquals("Task after subprocesses", taskAfterSubProcesses.Name);
		taskService.complete(taskAfterSubProcesses.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSimpleSubprocessWithTimerOnInnerSubProcess()
	  public virtual void testNestedSimpleSubprocessWithTimerOnInnerSubProcess()
	  {
		DateTime startTime = DateTime.Now;

		// After staring the process, the task in the subprocess should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nestedSubProcessWithTimer");
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// now we have 4 levels in the activityInstance:
		// pd
		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		//subprocess1
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance1 = rootActivityInstance.ChildActivityInstances[0];
		assertEquals("outerSubProcess", subProcessInstance1.ActivityId);
		//subprocess2
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance2 = subProcessInstance1.ChildActivityInstances[0];
		assertEquals("innerSubProcess", subProcessInstance2.ActivityId);
		// usertask
		assertEquals(1, subProcessInstance2.ChildActivityInstances.Length);
		ActivityInstance userTaskInstance = subProcessInstance2.ChildActivityInstances[0];
		assertEquals("innerSubProcessTask", userTaskInstance.ActivityId);

		// Setting the clock forward 1 hour 1 second (timer fires in 1 hour) and fire up the job executor
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + (60 * 60 * 1000) + 1000);
		waitForJobExecutorToProcessAllJobs(5000L);

		// The inner subprocess should be destoyed, and the escalated task should be active
		Task escalationTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Escalated task", escalationTask.Name);

		// now we have 3 levels in the activityInstance:
		// pd
		rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		//subprocess1
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		subProcessInstance1 = rootActivityInstance.ChildActivityInstances[0];
		assertEquals("outerSubProcess", subProcessInstance1.ActivityId);
		//subprocess2
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance escalationTaskInst = subProcessInstance1.ChildActivityInstances[0];
		assertEquals("escalationTask", escalationTaskInst.ActivityId);

		// Completing the escalated task, destroys the outer scope and activates the task after the subprocess
		taskService.complete(escalationTask.Id);
		Task taskAfterSubProcess = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task after subprocesses", taskAfterSubProcess.Name);
	  }

	  /// <summary>
	  /// Test case where the simple sub process of previous test cases
	  /// is nested within two other sub processes
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDoubleNestedSimpleSubProcess()
	  public virtual void testDoubleNestedSimpleSubProcess()
	  {
		// After staring the process, the task in the inner subprocess must be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nestedSimpleSubProcess");
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task in subprocess", subProcessTask.Name);

		// After completing the task in the subprocess,
		// both subprocesses are destroyed and the task after the subprocess should be active
		taskService.complete(subProcessTask.Id);
		Task taskAfterSubProcesses = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task after subprocesses", taskAfterSubProcesses.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleParallelSubProcess()
	  public virtual void testSimpleParallelSubProcess()
	  {

		// After starting the process, the two task in the subprocess should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleParallelSubProcess");
		IList<Task> subProcessTasks = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc().list();

		// Tasks are ordered by name (see query)
		Task taskA = subProcessTasks[0];
		Task taskB = subProcessTasks[1];
		assertEquals("Task A", taskA.Name);
		assertEquals("Task B", taskB.Name);

		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		//subprocess1
		assertEquals(1, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
		assertEquals("subProcess", subProcessInstance.ActivityId);
		// 2 tasks are present
		assertEquals(2, subProcessInstance.ChildActivityInstances.Length);

		// Completing both tasks, should destroy the subprocess and activate the task after the subprocess
		taskService.complete(taskA.Id);

		rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		subProcessInstance = rootActivityInstance.ChildActivityInstances[0];
		// 1 task + 1 join
		assertEquals(2, subProcessInstance.ChildActivityInstances.Length);

		taskService.complete(taskB.Id);
		Task taskAfterSubProcess = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Task after sub process", taskAfterSubProcess.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleParallelSubProcessWithTimer()
	  public virtual void testSimpleParallelSubProcessWithTimer()
	  {

		// After staring the process, the tasks in the subprocess should be active
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("simpleParallelSubProcessWithTimer");
		IList<Task> subProcessTasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).orderByTaskName().asc().list();

		// Tasks are ordered by name (see query)
		Task taskA = subProcessTasks[0];
		Task taskB = subProcessTasks[1];
		assertEquals("Task A", taskA.Name);
		assertEquals("Task B", taskB.Name);

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		managementService.executeJob(job.Id);

		// The inner subprocess should be destoyed, and the tsk after the timer should be active
		Task taskAfterTimer = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals("Task after timer", taskAfterTimer.Name);

		// Completing the task after the timer ends the process instance
		taskService.complete(taskAfterTimer.Id);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoSubProcessInParallel()
	  public virtual void testTwoSubProcessInParallel()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("twoSubProcessInParallel");
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc();
		IList<Task> tasks = taskQuery.list();

		// After process start, both tasks in the subprocesses should be active
		assertEquals("Task in subprocess A", tasks[0].Name);
		assertEquals("Task in subprocess B", tasks[1].Name);

		// validate activity instance tree
		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		assertEquals(pi.ProcessDefinitionId, rootActivityInstance.ActivityId);
		assertEquals(2, rootActivityInstance.ChildActivityInstances.Length);
		ActivityInstance[] childActivityInstances = rootActivityInstance.ChildActivityInstances;
		foreach (ActivityInstance activityInstance in childActivityInstances)
		{
		  assertTrue(Arrays.asList(new string[]{"subProcessA", "subProcessB"}).contains(activityInstance.ActivityId));
		  ActivityInstance[] subProcessChildren = activityInstance.ChildActivityInstances;
		  assertEquals(1, subProcessChildren.Length);
		  assertTrue(Arrays.asList(new string[]{"subProcessATask", "subProcessBTask"}).contains(subProcessChildren[0].ActivityId));
		}

		// Completing both tasks should active the tasks outside the subprocesses
		taskService.complete(tasks[0].Id);

		tasks = taskQuery.list();
		assertEquals("Task after subprocess A", tasks[0].Name);
		assertEquals("Task in subprocess B", tasks[1].Name);

		taskService.complete(tasks[1].Id);

		tasks = taskQuery.list();

		assertEquals("Task after subprocess A", tasks[0].Name);
		assertEquals("Task after subprocess B", tasks[1].Name);

		// Completing these tasks should end the process
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoSubProcessInParallelWithinSubProcess()
	  public virtual void testTwoSubProcessInParallelWithinSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("twoSubProcessInParallelWithinSubProcess");
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc();
		IList<Task> tasks = taskQuery.list();

		// After process start, both tasks in the subprocesses should be active
		Task taskA = tasks[0];
		Task taskB = tasks[1];
		assertEquals("Task in subprocess A", taskA.Name);
		assertEquals("Task in subprocess B", taskB.Name);

		// validate activity instance tree
		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(pi.ProcessInstanceId);
		ActivityInstanceAssert.assertThat(rootActivityInstance).hasStructure(ActivityInstanceAssert.describeActivityInstanceTree(pi.ProcessDefinitionId).beginScope("outerSubProcess").beginScope("subProcessA").activity("subProcessATask").endScope().beginScope("subProcessB").activity("subProcessBTask").done());

		// Completing both tasks should active the tasks outside the subprocesses
		taskService.complete(taskA.Id);
		taskService.complete(taskB.Id);

		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		// Completing this task should end the process
		taskService.complete(taskAfterSubProcess.Id);
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoNestedSubProcessesInParallelWithTimer()
	  public virtual void testTwoNestedSubProcessesInParallelWithTimer()
	  {

	//    Date startTime = new Date();

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nestedParallelSubProcessesWithTimer");
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc();
		IList<Task> tasks = taskQuery.list();

		// After process start, both tasks in the subprocesses should be active
		Task taskA = tasks[0];
		Task taskB = tasks[1];
		assertEquals("Task in subprocess A", taskA.Name);
		assertEquals("Task in subprocess B", taskB.Name);

		// Firing the timer should destroy all three subprocesses and activate the task after the timer
	//    ClockUtil.setCurrentTime(new Date(startTime.getTime() + (2 * 60 * 60 * 1000 ) + 1000));
	//    waitForJobExecutorToProcessAllJobs(5000L, 50L);
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		Task taskAfterTimer = taskQuery.singleResult();
		assertEquals("Task after timer", taskAfterTimer.Name);

		// Completing the task should end the process instance
		taskService.complete(taskAfterTimer.Id);
		assertProcessEnded(pi.Id);
	  }

	  /// <seealso cref= http://jira.codehaus.org/browse/ACT-1072 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSimpleSubProcessWithoutEndEvent()
	  public virtual void testNestedSimpleSubProcessWithoutEndEvent()
	  {
		testNestedSimpleSubProcess();
	  }

	  /// <seealso cref= http://jira.codehaus.org/browse/ACT-1072 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleSubProcessWithoutEndEvent()
	  public virtual void testSimpleSubProcessWithoutEndEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testSimpleSubProcessWithoutEndEvent");
		assertProcessEnded(pi.Id);
	  }

	  /// <seealso cref= http://jira.codehaus.org/browse/ACT-1072 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNestedSubProcessesWithoutEndEvents()
	  public virtual void testNestedSubProcessesWithoutEndEvents()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testNestedSubProcessesWithoutEndEvents");
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testActivityInstanceTreeNestedCmd()
	  public virtual void testActivityInstanceTreeNestedCmd()
	  {
		GetActInstanceDelegate.activityInstance = null;
		runtimeService.startProcessInstanceByKey("process");

		ActivityInstance activityInstance = GetActInstanceDelegate.activityInstance;

		assertNotNull(activityInstance);
		ActivityInstance subProcessInstance = activityInstance.ChildActivityInstances[0];
		assertNotNull(subProcessInstance);
		assertEquals("SubProcess_1", subProcessInstance.ActivityId);

		ActivityInstance serviceTaskInstance = subProcessInstance.ChildActivityInstances[0];
		assertNotNull(serviceTaskInstance);
		assertEquals("ServiceTask_1", serviceTaskInstance.ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testActivityInstanceTreeNestedCmdAfterTx()
	  public virtual void testActivityInstanceTreeNestedCmdAfterTx()
	  {
		GetActInstanceDelegate.activityInstance = null;
		runtimeService.startProcessInstanceByKey("process");

		// send message
		runtimeService.correlateMessage("message");

		ActivityInstance activityInstance = GetActInstanceDelegate.activityInstance;

		assertNotNull(activityInstance);
		ActivityInstance subProcessInstance = activityInstance.ChildActivityInstances[0];
		assertNotNull(subProcessInstance);
		assertEquals("SubProcess_1", subProcessInstance.ActivityId);

		ActivityInstance serviceTaskInstance = subProcessInstance.ChildActivityInstances[0];
		assertNotNull(serviceTaskInstance);
		assertEquals("ServiceTask_1", serviceTaskInstance.ActivityId);
	  }

	  public virtual void testConcurrencyInSubProcess()
	  {

		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/subprocess/SubProcessTest.fixSystemFailureProcess.bpmn20.xml").deploy();

		// After staring the process, both tasks in the subprocess should be active
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("fixSystemFailure");
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(pi.Id).orderByTaskName().asc().list();

		// Tasks are ordered by name (see query)
		assertEquals(2, tasks.Count);
		Task investigateHardwareTask = tasks[0];
		Task investigateSoftwareTask = tasks[1];
		assertEquals("Investigate hardware", investigateHardwareTask.Name);
		assertEquals("Investigate software", investigateSoftwareTask.Name);

		// Completing both the tasks finishes the subprocess and enables the task after the subprocess
		taskService.complete(investigateHardwareTask.Id);
		taskService.complete(investigateSoftwareTask.Id);

		Task writeReportTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Write report", writeReportTask.Name);

		// Clean up
		repositoryService.deleteDeployment(deployment.Id, true);
	  }
	}

}