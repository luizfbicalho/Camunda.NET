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
namespace org.camunda.bpm.engine.test.bpmn.@event.timer
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;


	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using LocalDateTime = org.joda.time.LocalDateTime;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class BoundaryTimerEventTest : PluggableProcessEngineTestCase
	{

	  /*
	   * Test for when multiple boundary timer events are defined on the same user
	   * task
	   *
	   * Configuration: - timer 1 -> 2 hours -> secondTask - timer 2 -> 1 hour ->
	   * thirdTask - timer 3 -> 3 hours -> fourthTask
	   *
	   * See process image next to the process xml resource
	   */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleTimersOnUserTask()
	  public virtual void testMultipleTimersOnUserTask()
	  {

		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		// After process start, there should be 3 timers created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("multipleTimersOnUserTask");
		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(3, jobs.Count);

		// After setting the clock to time '1 hour and 5 seconds', the second timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((60 * 60 * 1000) + 5000));
		waitForJobExecutorToProcessAllJobs(5000L);
		assertEquals(0L, jobQuery.count());

		// which means that the third task is reached
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Third Task", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerOnNestingOfSubprocesses()
	  public virtual void testTimerOnNestingOfSubprocesses()
	  {

		runtimeService.startProcessInstanceByKey("timerOnNestedSubprocesses");
		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("Inner subprocess task 1", tasks[0].Name);
		assertEquals("Inner subprocess task 2", tasks[1].Name);

		Job timer = managementService.createJobQuery().timers().singleResult();
		managementService.executeJob(timer.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task outside subprocess", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpressionOnTimer()
	  public virtual void testExpressionOnTimer()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duration"] = "PT1H";

		// After process start, there should be a timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExpressionOnTimer", variables);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(1, jobs.Count);

		// After setting the clock to time '1 hour and 5 seconds', the second timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((60 * 60 * 1000) + 5000));
		waitForJobExecutorToProcessAllJobs(5000L);
		assertEquals(0L, jobQuery.count());

		// which means the process has ended
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRecalculateUnchangedExpressionOnTimerCurrentDateBased()
	  public virtual void testRecalculateUnchangedExpressionOnTimerCurrentDateBased()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duedate"] = "PT1H";

		// After process start, there should be a timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExpressionOnTimer", variables);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(1, jobs.Count);
		Job job = jobs[0];
		DateTime oldDate = job.Duedate;

		// After recalculation of the timer, the job's duedate should be changed
		DateTime currentTime = new DateTime(startTime.Ticks + TimeUnit.MINUTES.toMillis(5));
		ClockUtil.CurrentTime = currentTime;
		managementService.recalculateJobDuedate(job.Id, false);
		Job jobUpdated = jobQuery.singleResult();
		assertEquals(job.Id, jobUpdated.Id);
		assertNotEquals(oldDate, jobUpdated.Duedate);
		assertTrue(oldDate < jobUpdated.Duedate);
		DateTime expectedDate = LocalDateTime.fromDateFields(currentTime).plusHours(1).toDate();
		assertThat(jobUpdated.Duedate).isCloseTo(expectedDate, 1000l);

		// After setting the clock to time '1 hour and 6 min', the second timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + TimeUnit.HOURS.toMillis(1L) + TimeUnit.MINUTES.toMillis(6L));
		waitForJobExecutorToProcessAllJobs(5000L);
		assertEquals(0L, jobQuery.count());

		// which means the process has ended
		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerEventTest.testRecalculateUnchangedExpressionOnTimerCurrentDateBased.bpmn20.xml")]
	  public virtual void testRecalculateUnchangedExpressionOnTimerCreationDateBased()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duedate"] = "PT1H";

		// After process start, there should be a timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExpressionOnTimer", variables);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(1, jobs.Count);
		Job job = jobs[0];

		// After recalculation of the timer, the job's duedate should be based on the creation date
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + TimeUnit.SECONDS.toMillis(5));
		managementService.recalculateJobDuedate(job.Id, true);
		Job jobUpdated = jobQuery.singleResult();
		assertEquals(job.Id, jobUpdated.Id);
		DateTime expectedDate = LocalDateTime.fromDateFields(jobUpdated.CreateTime).plusHours(1).toDate();
		assertEquals(expectedDate, jobUpdated.Duedate);

		// After setting the clock to time '1 hour and 15 seconds', the second timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + TimeUnit.HOURS.toMillis(1L) + TimeUnit.SECONDS.toMillis(15L));
		waitForJobExecutorToProcessAllJobs(5000L);
		assertEquals(0L, jobQuery.count());

		// which means the process has ended
		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerEventTest.testRecalculateUnchangedExpressionOnTimerCurrentDateBased.bpmn20.xml")]
	  public virtual void testRecalculateChangedExpressionOnTimerCurrentDateBased()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duedate"] = "PT1H";

		// After process start, there should be a timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExpressionOnTimer", variables);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(1, jobs.Count);
		Job job = jobs[0];
		DateTime oldDate = job.Duedate;

		// After recalculation of the timer, the job's duedate should be changed
		runtimeService.setVariable(pi.Id, "duedate", "PT15M");
		managementService.recalculateJobDuedate(job.Id, false);
		Job jobUpdated = jobQuery.singleResult();
		assertEquals(job.Id, jobUpdated.Id);
		assertNotEquals(oldDate, jobUpdated.Duedate);
		assertTrue(oldDate > jobUpdated.Duedate);

		// After setting the clock to time '16 minutes', the timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + TimeUnit.MINUTES.toMillis(16L));
		waitForJobExecutorToProcessAllJobs(5000L);
		assertEquals(0L, jobQuery.count());

		// which means the process has ended
		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/timer/BoundaryTimerEventTest.testRecalculateUnchangedExpressionOnTimerCurrentDateBased.bpmn20.xml")]
	  public virtual void testRecalculateChangedExpressionOnTimerCreationDateBased()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duedate"] = "PT1H";

		// After process start, there should be a timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testExpressionOnTimer", variables);

		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		IList<Job> jobs = jobQuery.list();
		assertEquals(1, jobs.Count);
		Job job = jobs[0];
		DateTime oldDate = job.Duedate;

		// After recalculation of the timer, the job's duedate should be the same
		runtimeService.setVariable(pi.Id, "duedate", "PT15M");
		managementService.recalculateJobDuedate(job.Id, true);
		Job jobUpdated = jobQuery.singleResult();
		assertEquals(job.Id, jobUpdated.Id);
		assertNotEquals(oldDate, jobUpdated.Duedate);
		assertEquals(LocalDateTime.fromDateFields(jobUpdated.CreateTime).plusMinutes(15).toDate(), jobUpdated.Duedate);

		// After setting the clock to time '16 minutes', the timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + TimeUnit.MINUTES.toMillis(16L));
		waitForJobExecutorToProcessAllJobs(5000L);
		assertEquals(0L, jobQuery.count());

		// which means the process has ended
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerInSingleTransactionProcess()
	  public virtual void testTimerInSingleTransactionProcess()
	  {
		// make sure that if a PI completes in single transaction, JobEntities associated with the execution are deleted.
		// broken before 5.10, see ACT-1133
		runtimeService.startProcessInstanceByKey("timerOnSubprocesses");
		assertEquals(0, managementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRepeatingTimerWithCancelActivity()
	  public virtual void testRepeatingTimerWithCancelActivity()
	  {
		runtimeService.startProcessInstanceByKey("repeatingTimerAndCallActivity");
		assertEquals(1, managementService.createJobQuery().count());
		assertEquals(1, taskService.createTaskQuery().count());

		// Firing job should cancel the user task, destroy the scope,
		// re-enter the task and recreate the task. A new timer should also be created.
		// This didn't happen before 5.11 (new jobs kept being created). See ACT-1427
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);
		assertEquals(1, managementService.createJobQuery().count());
		assertEquals(1, taskService.createTaskQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleOutgoingSequenceFlows()
	  public virtual void testMultipleOutgoingSequenceFlows()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("interruptingTimer");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

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
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("interruptingTimer");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		IList<Task> tasks = taskQuery.list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleOutgoingSequenceFlowsOnSubprocessMi()
	  public virtual void testMultipleOutgoingSequenceFlowsOnSubprocessMi()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("interruptingTimer");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		IList<Task> tasks = taskQuery.list();

		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingTimerDuration()
	  public virtual void testInterruptingTimerDuration()
	  {

		// Start process instance
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("escalationExample");

		// There should be one task, with a timer : first line support
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("First line support", task.Name);

		// Manually execute the job
		Job timer = managementService.createJobQuery().singleResult();
		managementService.executeJob(timer.Id);

		// The timer has fired, and the second task (secondlinesupport) now exists
		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("Handle escalated issue", task.Name);
	  }

	}

}