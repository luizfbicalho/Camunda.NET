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
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using LocalDateTime = org.joda.time.LocalDateTime;


	public class IntermediateTimerEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchingTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCatchingTimerEvent()
	  {

		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		// After process start, there should be timer created
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("intermediateTimerEventExample");
		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi.Id);
		assertEquals(1, jobQuery.count());

		// After setting the clock to time '50minutes and 5 seconds', the second timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((50 * 60 * 1000) + 5000));
		waitForJobExecutorToProcessAllJobs(5000L);

		assertEquals(0, jobQuery.count());
		assertProcessEnded(pi.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpression()
	  public virtual void testExpression()
	  {
		// Set the clock fixed
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["dueDate"] = DateTime.Now;

		Dictionary<string, object> variables2 = new Dictionary<string, object>();
		variables2["dueDate"] = (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(DateTime.Now);

		// After process start, there should be timer created
		ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables1);
		ProcessInstance pi2 = runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables2);

		assertEquals(1, managementService.createJobQuery().processInstanceId(pi1.Id).count());
		assertEquals(1, managementService.createJobQuery().processInstanceId(pi2.Id).count());

		// After setting the clock to one second in the future the timers should fire
		IList<Job> jobs = managementService.createJobQuery().executable().list();
		assertEquals(2, jobs.Count);
		foreach (Job job in jobs)
		{
		  managementService.executeJob(job.Id);
		}

		assertEquals(0, managementService.createJobQuery().processInstanceId(pi1.Id).count());
		assertEquals(0, managementService.createJobQuery().processInstanceId(pi2.Id).count());

		assertProcessEnded(pi1.ProcessInstanceId);
		assertProcessEnded(pi2.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpressionRecalculateCurrentDateBased() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testExpressionRecalculateCurrentDateBased()
	  {
		// Set the clock fixed
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duration"] = "PT1H";

		// After process start, there should be timer created
		ProcessInstanceWithVariables pi1 = (ProcessInstanceWithVariables) runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables);
		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi1.Id);
		assertEquals(1, jobQuery.count());
		Job job = jobQuery.singleResult();
		DateTime firstDate = job.Duedate;

		// After variable change and recalculation, there should still be one timer only, with a changed due date
		moveByMinutes(1);
		DateTime currentTime = ClockUtil.CurrentTime;
		runtimeService.setVariable(pi1.ProcessInstanceId, "duration", "PT15M");
		processEngine.ManagementService.recalculateJobDuedate(job.Id, false);

		assertEquals(1, jobQuery.count());
		job = jobQuery.singleResult();
		assertNotEquals(firstDate, job.Duedate);
		assertTrue(firstDate > job.Duedate);
		DateTime expectedDate = LocalDateTime.fromDateFields(currentTime).plusMinutes(15).toDate();
		assertThat(job.Duedate).isCloseTo(expectedDate, 1000l);

		// After waiting for sixteen minutes the timer should fire
		ClockUtil.CurrentTime = new DateTime(firstDate.Ticks + TimeUnit.MINUTES.toMillis(16L));
		waitForJobExecutorToProcessAllJobs(5000L);

		assertEquals(0, managementService.createJobQuery().processInstanceId(pi1.Id).count());
		assertProcessEnded(pi1.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpressionRecalculateCurrentDateBased.bpmn20.xml") public void testExpressionRecalculateCreationDateBased() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpressionRecalculateCurrentDateBased.bpmn20.xml")]
	  public virtual void testExpressionRecalculateCreationDateBased()
	  {
		// Set the clock fixed
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["duration"] = "PT1H";

		// After process start, there should be timer created
		ProcessInstanceWithVariables pi1 = (ProcessInstanceWithVariables) runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables);
		JobQuery jobQuery = managementService.createJobQuery().processInstanceId(pi1.Id);
		assertEquals(1, jobQuery.count());
		Job job = jobQuery.singleResult();
		DateTime firstDate = job.Duedate;

		// After variable change and recalculation, there should still be one timer only, with a changed due date
		moveByMinutes(65); // move past first due date
		runtimeService.setVariable(pi1.ProcessInstanceId, "duration", "PT15M");
		processEngine.ManagementService.recalculateJobDuedate(job.Id, true);

		assertEquals(1, jobQuery.count());
		job = jobQuery.singleResult();
		assertNotEquals(firstDate, job.Duedate);
		assertTrue(firstDate > job.Duedate);
		DateTime expectedDate = LocalDateTime.fromDateFields(job.CreateTime).plusMinutes(15).toDate();
		assertEquals(expectedDate, job.Duedate);

		// After waiting for sixteen minutes the timer should fire
		ClockUtil.CurrentTime = new DateTime(firstDate.Ticks + TimeUnit.MINUTES.toMillis(16L));
		waitForJobExecutorToProcessAllJobs(5000L);

		assertEquals(0, managementService.createJobQuery().processInstanceId(pi1.Id).count());
		assertProcessEnded(pi1.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimeCycle()
	  public virtual void testTimeCycle()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		JobQuery query = managementService.createJobQuery();
		assertEquals(1, query.count());

		string jobId = query.singleResult().Id;
		managementService.executeJob(jobId);

		assertEquals(0, query.count());

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRecalculateTimeCycleExpressionCurrentDateBased()
	  public virtual void testRecalculateTimeCycleExpressionCurrentDateBased()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["cycle"] = "R/PT15M";
		string processInstanceId = runtimeService.startProcessInstanceByKey("process", variables).Id;

		JobQuery query = managementService.createJobQuery();
		assertEquals(1, query.count());
		Job job = query.singleResult();
		DateTime oldDuedate = job.Duedate;
		string jobId = job.Id;

		// when
		runtimeService.setVariable(processInstanceId, "cycle", "R/PT10M");
		managementService.recalculateJobDuedate(jobId, false);

		// then
		assertEquals(1, query.count());
		assertTrue(oldDuedate > query.singleResult().Duedate);

		managementService.executeJob(jobId);
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testRecalculateTimeCycleExpressionCurrentDateBased.bpmn20.xml")]
	  public virtual void testRecalculateTimeCycleExpressionCreationDateBased()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["cycle"] = "R/PT15M";
		string processInstanceId = runtimeService.startProcessInstanceByKey("process", variables).Id;

		JobQuery query = managementService.createJobQuery();
		assertEquals(1, query.count());
		Job job = query.singleResult();
		DateTime oldDuedate = job.Duedate;
		string jobId = job.Id;

		// when
		runtimeService.setVariable(processInstanceId, "cycle", "R/PT10M");
		managementService.recalculateJobDuedate(jobId, true);

		// then
		assertEquals(1, query.count());
		DateTime newDuedate = query.singleResult().Duedate;
		assertTrue(oldDuedate > newDuedate);
		DateTime expectedDate = LocalDateTime.fromDateFields(job.CreateTime).plusMinutes(10).toDate();
		assertEquals(expectedDate, newDuedate);

		managementService.executeJob(jobId);
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void moveByMinutes(int minutes) throws Exception
	  private void moveByMinutes(int minutes)
	  {
		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + ((minutes * 60 * 1000) + 5000));
	  }

	}
}