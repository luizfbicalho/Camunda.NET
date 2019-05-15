using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.engine.test.bpmn.@event.timer
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class BoundaryTimerNonInterruptingEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleTimersOnUserTask()
	  public virtual void testMultipleTimersOnUserTask()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		// After process start, there should be 3 timers created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingTimersOnUserTask");
		Task task1 = taskService.createTaskQuery().singleResult();
		assertEquals("First Task", task1.Name);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(2, jobs.Count);

		// After setting the clock to time '1 hour and 5 seconds', the first timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((60 * 60 * 1000) + 5000));
		waitForJobExecutorToProcessAllJobs(5000L);

		// we still have one timer more to fire
		assertEquals(1L, jobQuery.count());

		// and we are still in the first state, but in the second state as well!
		assertEquals(2L, taskService.createTaskQuery().count());
		IList<Task> taskList = taskService.createTaskQuery().orderByTaskName().desc().list();
		assertEquals("First Task", taskList[0].Name);
		assertEquals("Escalation Task 1", taskList[1].Name);

		// complete the task and end the forked execution
		taskService.complete(taskList[1].Id);

		// but we still have the original executions
		assertEquals(1L, taskService.createTaskQuery().count());
		assertEquals("First Task", taskService.createTaskQuery().singleResult().Name);

		// After setting the clock to time '2 hour and 5 seconds', the second timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((2 * 60 * 60 * 1000) + 5000));
		waitForJobExecutorToProcessAllJobs(5000L);

		// no more timers to fire
		assertEquals(0L, jobQuery.count());

		// and we are still in the first state, but in the next escalation state as well
		assertEquals(2L, taskService.createTaskQuery().count());
		taskList = taskService.createTaskQuery().orderByTaskName().desc().list();
		assertEquals("First Task", taskList[0].Name);
		assertEquals("Escalation Task 2", taskList[1].Name);

		// This time we end the main task
		taskService.complete(taskList[0].Id);

		// but we still have the escalation task
		assertEquals(1L, taskService.createTaskQuery().count());
		Task escalationTask = taskService.createTaskQuery().singleResult();
		assertEquals("Escalation Task 2", escalationTask.Name);

		taskService.complete(escalationTask.Id);

		// now we are really done :-)
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerOnMiUserTask()
	  public virtual void testTimerOnMiUserTask()
	  {

		// After process start, there should be 1 timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingTimersOnUserTask");
		IList<Task> taskList = taskService.createTaskQuery().list();
		assertEquals(5, taskList.Count);
		foreach (Task task in taskList)
		{
		  assertEquals("First Task", task.Name);
		}

		Job job = managementService.createJobQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(job);

		// execute the timer
		managementService.executeJob(job.Id);

		// now there are 6 tasks
		taskList = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(6, taskList.Count);

		// first task is the escalation task
		Task escalationTask = taskList.RemoveAt(0);
		assertEquals("Escalation Task 1", escalationTask.Name);
		// complete it
		taskService.complete(escalationTask.Id);

		// now complete the remaining tasks
		foreach (Task task in taskList)
		{
		  taskService.complete(task.Id);
		}

		// process instance is ended
		assertProcessEnded(pi.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJoin()
	  public virtual void testJoin()
	  {
		// After process start, there should be 3 timers created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testJoin");
		Task task1 = taskService.createTaskQuery().singleResult();
		assertEquals("Main Task", task1.Name);

		Job job = managementService.createJobQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		// we now have both tasks
		assertEquals(2L, taskService.createTaskQuery().count());

		// end the first
		taskService.complete(task1.Id);

		// we now have one task left
		assertEquals(1L, taskService.createTaskQuery().count());
		Task task2 = taskService.createTaskQuery().singleResult();
		assertEquals("Escalation Task", task2.Name);

		// complete the task, the parallel gateway should fire
		taskService.complete(task2.Id);

		// and the process has ended
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerOnConcurrentMiTasks()
	  public virtual void testTimerOnConcurrentMiTasks()
	  {

		// After process start, there should be 1 timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("timerOnConcurrentMiTasks");
		IList<Task> taskList = taskService.createTaskQuery().orderByTaskName().desc().list();
		assertEquals(6, taskList.Count);
		Task secondTask = taskList.RemoveAt(0);
		assertEquals("Second Task", secondTask.Name);
		foreach (Task task in taskList)
		{
		  assertEquals("First Task", task.Name);
		}

		Job job = managementService.createJobQuery().processInstanceId(pi.Id).singleResult();
		assertNotNull(job);

		// execute the timer
		managementService.executeJob(job.Id);

		// now there are 7 tasks
		taskList = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(7, taskList.Count);

		// first task is the escalation task
		Task escalationTask = taskList.RemoveAt(0);
		assertEquals("Escalation Task 1", escalationTask.Name);
		// complete it
		taskService.complete(escalationTask.Id);

		// now complete the remaining tasks
		foreach (Task task in taskList)
		{
		  taskService.complete(task.Id);
		}

		// process instance is ended
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerOnConcurrentTasks()
	  public virtual void testTimerOnConcurrentTasks()
	  {
		string procId = runtimeService.startProcessInstanceByKey("nonInterruptingOnConcurrentTasks").Id;
		assertEquals(2, taskService.createTaskQuery().count());

		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);
		assertEquals(3, taskService.createTaskQuery().count());

		// Complete task that was reached by non interrupting timer
		Task task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask").singleResult();
		taskService.complete(task.Id);
		assertEquals(2, taskService.createTaskQuery().count());

		// Complete other tasks
		foreach (Task t in taskService.createTaskQuery().list())
		{
		  taskService.complete(t.Id);
		}
		assertProcessEnded(procId);
	  }

	  // Difference with previous test: now the join will be reached first
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerOnConcurrentTasks.bpmn20.xml"})]
	  public virtual void testTimerOnConcurrentTasks2()
	  {
		string procId = runtimeService.startProcessInstanceByKey("nonInterruptingOnConcurrentTasks").Id;
		assertEquals(2, taskService.createTaskQuery().count());

		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);
		assertEquals(3, taskService.createTaskQuery().count());

		// Complete 2 tasks that will trigger the join
		Task task = taskService.createTaskQuery().taskDefinitionKey("firstTask").singleResult();
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().taskDefinitionKey("secondTask").singleResult();
		taskService.complete(task.Id);
		assertEquals(1, taskService.createTaskQuery().count());

		// Finally, complete the task that was created due to the timer
		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerWithCycle() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimerWithCycle()
	  {
		runtimeService.startProcessInstanceByKey("nonInterruptingCycle").Id;
		TaskQuery tq = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask");
		assertEquals(0, tq.count());
		moveByHours(1);
		assertEquals(1, tq.count());
		moveByHours(1);
		assertEquals(2, tq.count());

		Task task = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		taskService.complete(task.Id);

		moveByHours(1);
		assertEquals(2, tq.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerOnEmbeddedSubprocess()
	  public virtual void testTimerOnEmbeddedSubprocess()
	  {
		string id = runtimeService.startProcessInstanceByKey("nonInterruptingTimerOnEmbeddedSubprocess").Id;

		TaskQuery tq = taskService.createTaskQuery().taskAssignee("kermit");

		assertEquals(1, tq.count());

		// Simulate timer
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		tq = taskService.createTaskQuery().taskAssignee("kermit");

		assertEquals(2, tq.count());

		IList<Task> tasks = tq.list();

		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		assertProcessEnded(id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testReceiveTaskWithBoundaryTimer()
	  public virtual void testReceiveTaskWithBoundaryTimer()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["timeCycle"] = "R/PT1H";

		// After process start, there should be a timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingCycle",variables);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(1, jobs.Count);

		// The Execution Query should work normally and find executions in state "task"
		IList<Execution> executions = runtimeService.createExecutionQuery().activityId("task").list();
		assertEquals(1, executions.Count);
		IList<string> activeActivityIds = runtimeService.getActiveActivityIds(executions[0].Id);
		assertEquals(1, activeActivityIds.Count);
		assertEquals("task", activeActivityIds[0]);

		runtimeService.signal(executions[0].Id);

	//    // After setting the clock to time '1 hour and 5 seconds', the second timer should fire
	//    ClockUtil.setCurrentTime(new Date(startTime.getTime() + ((60 * 60 * 1000) + 5000)));
	//    waitForJobExecutorToProcessAllJobs(5000L);
	//    assertEquals(0L, jobQuery.count());

		// which means the process has ended
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerOnConcurrentSubprocess()
	  public virtual void testTimerOnConcurrentSubprocess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("testTimerOnConcurrentSubprocess").Id;
		assertEquals(4, taskService.createTaskQuery().count());

		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);
		assertEquals(5, taskService.createTaskQuery().count());

		// Complete 4 tasks that will trigger the join
		Task task = taskService.createTaskQuery().taskDefinitionKey("sub1task1").singleResult();
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().taskDefinitionKey("sub1task2").singleResult();
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().taskDefinitionKey("sub2task1").singleResult();
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().taskDefinitionKey("sub2task2").singleResult();
		taskService.complete(task.Id);
		assertEquals(1, taskService.createTaskQuery().count());

		// Finally, complete the task that was created due to the timer
		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(procId);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerOnConcurrentSubprocess.bpmn20.xml")]
	  public virtual void testTimerOnConcurrentSubprocess2()
	  {
		string procId = runtimeService.startProcessInstanceByKey("testTimerOnConcurrentSubprocess").Id;
		assertEquals(4, taskService.createTaskQuery().count());

		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);
		assertEquals(5, taskService.createTaskQuery().count());

		Task task = taskService.createTaskQuery().taskDefinitionKey("sub1task1").singleResult();
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().taskDefinitionKey("sub1task2").singleResult();
		taskService.complete(task.Id);

		// complete the task that was created due to the timer
		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask").singleResult();
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("sub2task1").singleResult();
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().taskDefinitionKey("sub2task2").singleResult();
		taskService.complete(task.Id);
		assertEquals(0, taskService.createTaskQuery().count());

		assertProcessEnded(procId);
	  }

	  //we cannot use waitForExecutor... method since there will always be one job left
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void moveByHours(int hours) throws Exception
	  private void moveByHours(int hours)
	  {
		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + ((hours * 60 * 1000 * 60) + 5000));
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor.start();
		Thread.Sleep(1000);
		jobExecutor.shutdown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleOutgoingSequenceFlows()
	  public virtual void testMultipleOutgoingSequenceFlows()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingTimer");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(3, taskQuery.count());

		IList<Task> tasks = taskQuery.list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleOutgoingSequenceFlowsOnSubprocess()
	  public virtual void testMultipleOutgoingSequenceFlowsOnSubprocess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingTimer");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		Task task = taskService.createTaskQuery().taskDefinitionKey("innerTask1").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("innerTask2").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask1").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask2").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);

		// Case 2: fire outer tasks first

		pi = runtimeService.startProcessInstanceByKey("nonInterruptingTimer");

		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask1").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("timerFiredTask2").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("innerTask1").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().taskDefinitionKey("innerTask2").singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleOutgoingSequenceFlowsOnSubprocessMi()
	  public virtual void testMultipleOutgoingSequenceFlowsOnSubprocessMi()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("nonInterruptingTimer");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(10, taskQuery.count());

		IList<Task> tasks = taskQuery.list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerWithCycle.bpmn20.xml"}) public void testTimeCycle() throws Exception
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerNonInterruptingEventTest.testTimerWithCycle.bpmn20.xml"})]
	  public virtual void testTimeCycle()
	  {
		// given
		runtimeService.startProcessInstanceByKey("nonInterruptingCycle");

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		string jobId = jobQuery.singleResult().Id;

		// when
		managementService.executeJob(jobId);

		// then
		assertEquals(1, jobQuery.count());

		string anotherJobId = jobQuery.singleResult().Id;
		assertFalse(jobId.Equals(anotherJobId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFailingTimeCycle()
	  public virtual void testFailingTimeCycle()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		JobQuery failedJobQuery = managementService.createJobQuery();
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.count());

		string jobId = jobQuery.singleResult().Id;
		failedJobQuery.jobId(jobId);

		// when (1)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (1)
		Job failedJob = failedJobQuery.singleResult();
		assertEquals(2, failedJob.Retries);

		// a new timer job has been created
		assertEquals(2, jobQuery.count());

		assertEquals(1, managementService.createJobQuery().withException().count());
		assertEquals(0, managementService.createJobQuery().noRetriesLeft().count());
		assertEquals(2, managementService.createJobQuery().withRetriesLeft().count());

		// when (2)
		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		  // expected
		}

		// then (2)
		failedJob = failedJobQuery.singleResult();
		assertEquals(1, failedJob.Retries);

		// there are still two jobs
		assertEquals(2, jobQuery.count());

		assertEquals(1, managementService.createJobQuery().withException().count());
		assertEquals(0, managementService.createJobQuery().noRetriesLeft().count());
		assertEquals(2, managementService.createJobQuery().withRetriesLeft().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUpdateTimerRepeat()
	  public virtual void testUpdateTimerRepeat()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime currentTime = new DateTime();
		ClockUtil.CurrentTime = currentTime;

		// GIVEN
		// Start process instance with a non-interrupting boundary timer event
		// on a user task
		runtimeService.startProcessInstanceByKey("timerRepeat");

		// there should be a single user task for the process instance
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(1, tasks.Count);
		assertEquals("User Waiting", tasks[0].Name);

		// there should be a single timer job (R5/PT1H)
		TimerEntity timerJob = (TimerEntity) managementService.createJobQuery().singleResult();
		assertNotNull(timerJob);
		assertEquals("R5/" + sdf.format(ClockUtil.CurrentTime) + "/PT1H", timerJob.Repeat);

		// WHEN
		// we update the repeat property of the timer job
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

		// THEN
		// the timer job should be updated
		TimerEntity updatedTimerJob = (TimerEntity) managementService.createJobQuery().singleResult();
		assertEquals("R3/PT3H", updatedTimerJob.Repeat);

		currentTime.add(DateTime.HOUR, 1);
		ClockUtil.CurrentTime = currentTime;
		managementService.executeJob(timerJob.Id);

		// and when the timer executes, there should be 2 user tasks waiting
		tasks = taskService.createTaskQuery().orderByTaskCreateTime().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("User Waiting", tasks[0].Name);
		assertEquals("Timer Fired", tasks[1].Name);

		// finally, the second timer job should have a DueDate in 3 hours instead of 1 hour
		// and its repeat property should be the one we updated
		TimerEntity secondTimerJob = (TimerEntity) managementService.createJobQuery().singleResult();
		currentTime.add(DateTime.HOUR, 3);
		assertEquals("R3/PT3H", secondTimerJob.Repeat);
		assertEquals(sdf.format(currentTime), sdf.format(secondTimerJob.Duedate));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly BoundaryTimerNonInterruptingEventTest outerInstance;

		  public CommandAnonymousInnerClass(BoundaryTimerNonInterruptingEventTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			TimerEntity timerEntity = (TimerEntity) commandContext.ProcessEngineConfiguration.ManagementService.createJobQuery().singleResult();

			// update repeat property
			timerEntity.Repeat = "R3/PT3H";

			return null;
		  }
	  }
	}

}