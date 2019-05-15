using System;
using System.Collections.Generic;
using System.IO;

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

	using DeleteJobsCmd = org.camunda.bpm.engine.impl.cmd.DeleteJobsCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Mocks = org.camunda.bpm.engine.test.mock.Mocks;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ProcessBuilder = org.camunda.bpm.model.bpmn.builder.ProcessBuilder;
	using LocalDateTime = org.joda.time.LocalDateTime;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class StartTimerEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDurationStartTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDurationStartTimerEvent()
	  {
		// Set the clock fixed
		DateTime startTime = DateTime.Now;

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		// After setting the clock to time '50minutes and 5 seconds', the second
		// timer should fire
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((50 * 60 * 1000) + 5000));

		executeAllJobs();

		executeAllJobs();

		IList<ProcessInstance> pi = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample").list();
		assertEquals(1, pi.Count);

		assertEquals(0, jobQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFixedDateStartTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testFixedDateStartTimerEvent()
	  {

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		ClockUtil.CurrentTime = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("15/11/2036 11:12:30");
		executeAllJobs();

		IList<ProcessInstance> pi = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample").list();
		assertEquals(1, pi.Count);

		assertEquals(0, jobQuery.count());

	  }

	  // FIXME: This test likes to run in an endless loop when invoking the
	  // waitForJobExecutorOnCondition method
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testCycleDateStartTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void FAILING_testCycleDateStartTimerEvent()
	  {
		ClockUtil.CurrentTime = DateTime.Now;

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstanceQuery piq = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample");
		ProcessInstanceQuery piq = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample");

		assertEquals(0, piq.count());

		moveByMinutes(5);
		executeAllJobs();
		assertEquals(1, piq.count());
		assertEquals(1, jobQuery.count());

		moveByMinutes(5);
		executeAllJobs();
		assertEquals(1, piq.count());

		assertEquals(1, jobQuery.count());
		// have to manually delete pending timer
	//    cleanDB();

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void moveByMinutes(int minutes) throws Exception
	  private void moveByMinutes(int minutes)
	  {
		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + ((minutes * 60 * 1000) + 5000));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCycleWithLimitStartTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCycleWithLimitStartTimerEvent()
	  {
		ClockUtil.CurrentTime = DateTime.Now;

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		// ensure that the deployment Id is set on the new job
		Job job = jobQuery.singleResult();
		assertNotNull(job.DeploymentId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstanceQuery piq = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExampleCycle");
		ProcessInstanceQuery piq = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExampleCycle");

		assertEquals(0, piq.count());

		moveByMinutes(5);
		executeAllJobs();
		assertEquals(1, piq.count());
		assertEquals(1, jobQuery.count());

		// ensure that the deployment Id is set on the new job
		job = jobQuery.singleResult();
		assertNotNull(job.DeploymentId);

		moveByMinutes(5);
		executeAllJobs();
		assertEquals(2, piq.count());
		assertEquals(0, jobQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPriorityInTimerCycleEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPriorityInTimerCycleEvent()
	  {
		ClockUtil.CurrentTime = DateTime.Now;

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		// ensure that the deployment Id is set on the new job
		Job job = jobQuery.singleResult();
		assertNotNull(job.DeploymentId);
		assertEquals(9999, job.Priority);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstanceQuery piq = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExampleCycle");
		ProcessInstanceQuery piq = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExampleCycle");

		assertEquals(0, piq.count());

		moveByMinutes(5);
		executeAllJobs();
		assertEquals(1, piq.count());
		assertEquals(1, jobQuery.count());

		// ensure that the deployment Id is set on the new job
		job = jobQuery.singleResult();
		assertNotNull(job.DeploymentId);

		// second job should have the same priority
		assertEquals(9999, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpressionStartTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testExpressionStartTimerEvent()
	  {
		// ACT-1415: fixed start-date is an expression
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		ClockUtil.CurrentTime = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("15/11/2036 11:12:30");
		executeAllJobs();

		IList<ProcessInstance> pi = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample").list();
		assertEquals(1, pi.Count);

		assertEquals(0, jobQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRecalculateExpressionStartTimerEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testRecalculateExpressionStartTimerEvent()
	  {
		// given
		JobQuery jobQuery = managementService.createJobQuery();
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample");
		assertEquals(1, jobQuery.count());
		assertEquals(0, processInstanceQuery.count());

		Job job = jobQuery.singleResult();
		DateTime oldDate = job.Duedate;

		// when
		moveByMinutes(2);
		DateTime currentTime = ClockUtil.CurrentTime;
		managementService.recalculateJobDuedate(job.Id, false);

		// then
		assertEquals(1, jobQuery.count());
		assertEquals(0, processInstanceQuery.count());

		DateTime newDate = jobQuery.singleResult().Duedate;
		assertNotEquals(oldDate, newDate);
		assertTrue(oldDate < newDate);
		DateTime expectedDate = LocalDateTime.fromDateFields(currentTime).plusHours(2).toDate();
		assertThat(newDate).isCloseTo(expectedDate, 1000l);

		// move the clock forward 2 hours and 2 min
		moveByMinutes(122);
		executeAllJobs();

		IList<ProcessInstance> pi = processInstanceQuery.list();
		assertEquals(1, pi.Count);

		assertEquals(0, jobQuery.count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/timer/StartTimerEventTest.testRecalculateExpressionStartTimerEvent.bpmn20.xml") public void testRecalculateUnchangedExpressionStartTimerEventCreationDateBased() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/timer/StartTimerEventTest.testRecalculateExpressionStartTimerEvent.bpmn20.xml")]
	  public virtual void testRecalculateUnchangedExpressionStartTimerEventCreationDateBased()
	  {
		// given
		JobQuery jobQuery = managementService.createJobQuery();
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample");
		assertEquals(1, jobQuery.count());
		assertEquals(0, processInstanceQuery.count());

		// when
		moveByMinutes(1);
		managementService.recalculateJobDuedate(jobQuery.singleResult().Id, true);

		// then due date should be based on the creation time
		assertEquals(1, jobQuery.count());
		assertEquals(0, processInstanceQuery.count());

		Job jobUpdated = jobQuery.singleResult();
		DateTime expectedDate = LocalDateTime.fromDateFields(jobUpdated.CreateTime).plusHours(2).toDate();
		assertEquals(expectedDate, jobUpdated.Duedate);

		// move the clock forward 2 hours and 1 minute
		moveByMinutes(121);
		executeAllJobs();

		IList<ProcessInstance> pi = processInstanceQuery.list();
		assertEquals(1, pi.Count);

		assertEquals(0, jobQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVersionUpgradeShouldCancelJobs() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testVersionUpgradeShouldCancelJobs()
	  {
		ClockUtil.CurrentTime = DateTime.Now;

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		// we deploy new process version, with some small change
		Stream @in = this.GetType().getResourceAsStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml");
		string process = (StringHelper.NewString(IoUtil.readInputStream(@in, ""))).replaceAll("beforeChange", "changed");
		IoUtil.closeSilently(@in);
		@in = new MemoryStream(process.GetBytes());
		string id = repositoryService.createDeployment().addInputStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml", @in).deploy().Id;
		IoUtil.closeSilently(@in);

		assertEquals(1, jobQuery.count());

		moveByMinutes(5);
		executeAllJobs();
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("startTimerEventExample").singleResult();
		string pi = processInstance.ProcessInstanceId;
		assertEquals("changed", runtimeService.getActiveActivityIds(pi)[0]);

		assertEquals(1, jobQuery.count());

	//    cleanDB();
		repositoryService.deleteDeployment(id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerShouldNotBeRecreatedOnDeploymentCacheReboot()
	  public virtual void testTimerShouldNotBeRecreatedOnDeploymentCacheReboot()
	  {

		// Just to be sure, I added this test. Sounds like something that could
		// easily happen
		// when the order of deploy/parsing is altered.

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		// Reset deployment cache
		processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();

		// Start one instance of the process definition, this will trigger a cache
		// reload
		runtimeService.startProcessInstanceByKey("startTimer");

		// No new jobs should have been created
		assertEquals(1, jobQuery.count());
	  }

	  // Test for ACT-1533
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testTimerShouldNotBeRemovedWhenUndeployingOldVersion() throws Exception
	  public virtual void testTimerShouldNotBeRemovedWhenUndeployingOldVersion()
	  {
		// Deploy test process
		Stream @in = this.GetType().getResourceAsStream("StartTimerEventTest.testTimerShouldNotBeRemovedWhenUndeployingOldVersion.bpmn20.xml");
		string process = StringHelper.NewString(IoUtil.readInputStream(@in, ""));
		IoUtil.closeSilently(@in);

		@in = new MemoryStream(process.GetBytes());
		string firstDeploymentId = repositoryService.createDeployment().addInputStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml", @in).deploy().Id;
		IoUtil.closeSilently(@in);

		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		// we deploy new process version, with some small change
		string processChanged = process.replaceAll("beforeChange", "changed");
		@in = new MemoryStream(processChanged.GetBytes());
		string secondDeploymentId = repositoryService.createDeployment().addInputStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml", @in).deploy().Id;
		IoUtil.closeSilently(@in);
		assertEquals(1, jobQuery.count());

		// Remove the first deployment
		repositoryService.deleteDeployment(firstDeploymentId, true);

		// The removal of an old version should not affect timer deletion
		// ACT-1533: this was a bug, and the timer was deleted!
		assertEquals(1, jobQuery.count());

		// Cleanup
		cleanDB();
		repositoryService.deleteDeployment(secondDeploymentId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventInEventSubProcess()
	  public virtual void testStartTimerEventInEventSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startTimerEventInEventSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		// execute existing timer job
		managementService.executeJob(managementService.createJobQuery().list().get(0).Id);
		assertEquals(0, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(0, taskQuery.count());

		// check if execution doesn't exist because timer start event is
		// interrupting
		assertEquals(0, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(0, processInstanceQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventInEventSubProcess()
	  public virtual void testNonInterruptingStartTimerEventInEventSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingStartTimerEventInEventSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		// execute existing job timer
		managementService.executeJob(managementService.createJobQuery().list().get(0).Id);
		assertEquals(0, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task still exists because timer start event is non
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if execution still exists because timer start event is non
		// interrupting
		assertEquals(1, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInSubProcess()
	  public virtual void testStartTimerEventSubProcessInSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startTimerEventSubProcessInSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(2, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		// execute existing timer job
		managementService.executeJob(managementService.createJobQuery().list().get(0).Id);
		assertEquals(0, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(0, taskQuery.count());

		// check if execution doesn't exist because timer start event is
		// interrupting
		assertEquals(0, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(0, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventSubProcessInSubProcess()
	  public virtual void testNonInterruptingStartTimerEventSubProcessInSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingStartTimerEventSubProcessInSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(2, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		// execute existing timer job
		managementService.executeJob(jobQuery.list().get(0).Id);
		assertEquals(0, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task still exists because timer start event is non
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if execution still exists because timer start event is non
		// interrupting
		assertEquals(2, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventWithTwoEventSubProcesses()
	  public virtual void testStartTimerEventWithTwoEventSubProcesses()
	  {
		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startTimerEventWithTwoEventSubProcesses");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// get all timer jobs ordered by dueDate
		IList<Job> orderedJobList = jobQuery.orderByJobDuedate().asc().list();
		// execute first timer job
		managementService.executeJob(orderedJobList[0].Id);
		assertEquals(0, jobQuery.count());

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(0, taskQuery.count());

		// check if execution doesn't exist because timer start event is
		// interrupting
		assertEquals(0, executionQuery.count());

		// check if process instance doesn't exist because timer start event is
		// interrupting
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(0, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventWithTwoEventSubProcesses()
	  public virtual void testNonInterruptingStartTimerEventWithTwoEventSubProcesses()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingStartTimerEventWithTwoEventSubProcesses");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// get all timer jobs ordered by dueDate
		IList<Job> orderedJobList = jobQuery.orderByJobDuedate().asc().list();
		// execute first timer job
		managementService.executeJob(orderedJobList[0].Id);
		assertEquals(1, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		DummyServiceTask.wasExecuted = false;

		// check if user task still exists because timer start event is non
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if execution still exists because timer start event is non
		// interrupting
		assertEquals(1, executionQuery.count());

		// execute second timer job
		managementService.executeJob(orderedJobList[1].Id);
		assertEquals(0, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task still exists because timer start event is non
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if execution still exists because timer event is non interrupting
		assertEquals(1, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessWithUserTask()
	  public virtual void testStartTimerEventSubProcessWithUserTask()
	  {
		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startTimerEventSubProcessWithUserTask");

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// get all timer jobs ordered by dueDate
		IList<Job> orderedJobList = jobQuery.orderByJobDuedate().asc().list();
		// execute first timer job
		managementService.executeJob(orderedJobList[0].Id);
		assertEquals(0, jobQuery.count());

		// check if user task of event subprocess named "subProcess" exists
		assertEquals(1, taskQuery.count());
		assertEquals("subprocessUserTask", taskQuery.list().get(0).TaskDefinitionKey);

		// check if process instance exists because subprocess named "subProcess" is
		// already running
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/simpleProcessWithCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/timer/StartTimerEventTest.testStartTimerEventWithTwoEventSubProcesses.bpmn20.xml" })]
	  public virtual void testStartTimerEventSubProcessCalledFromCallActivity()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["calledProcess"] = "startTimerEventWithTwoEventSubProcesses";
		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("simpleCallActivityProcess", variables);

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(2, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// get all timer jobs ordered by dueDate
		IList<Job> orderedJobList = jobQuery.orderByJobDuedate().asc().list();
		// execute first timer job
		managementService.executeJob(orderedJobList[0].Id);
		assertEquals(0, jobQuery.count());

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(0, taskQuery.count());

		// check if execution doesn't exist because timer start event is
		// interrupting
		assertEquals(0, executionQuery.count());

		// check if process instance doesn't exist because timer start event is
		// interrupting
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(0, processInstanceQuery.count());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/simpleProcessWithCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/timer/StartTimerEventTest.testNonInterruptingStartTimerEventWithTwoEventSubProcesses.bpmn20.xml" })]
	  public virtual void testNonInterruptingStartTimerEventSubProcessesCalledFromCallActivity()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingStartTimerEventWithTwoEventSubProcesses");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(1, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(processInstance.Id);
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// get all timer jobs ordered by dueDate
		IList<Job> orderedJobList = jobQuery.orderByJobDuedate().asc().list();
		// execute first timer job
		managementService.executeJob(orderedJobList[0].Id);
		assertEquals(1, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		DummyServiceTask.wasExecuted = false;

		// check if user task still exists because timer start event is non
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if execution still exists because timer start event is non
		// interrupting
		assertEquals(1, executionQuery.count());

		// execute second timer job
		managementService.executeJob(orderedJobList[1].Id);
		assertEquals(0, jobQuery.count());

		assertEquals(true, DummyServiceTask.wasExecuted);

		// check if user task still exists because timer start event is non
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if execution still exists because timer event is non interrupting
		assertEquals(1, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInMultiInstanceSubProcess()
	  public virtual void testStartTimerEventSubProcessInMultiInstanceSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startTimerEventSubProcessInMultiInstanceSubProcess");

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		string jobIdFirstLoop = jobQuery.list().get(0).Id;
		// execute timer job
		managementService.executeJob(jobIdFirstLoop);

		assertEquals(true, DummyServiceTask.wasExecuted);
		DummyServiceTask.wasExecuted = false;

		// execute multiInstance loop number 2
		assertEquals(1, taskQuery.count());
		assertEquals(1, jobQuery.count());
		string jobIdSecondLoop = jobQuery.list().get(0).Id;
		assertNotSame(jobIdFirstLoop, jobIdSecondLoop);
		// execute timer job
		managementService.executeJob(jobIdSecondLoop);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// multiInstance loop finished
		assertEquals(0, jobQuery.count());

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(0, taskQuery.count());

		// check if process instance doesn't exist because timer start event is
		// interrupting
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventInMultiInstanceEventSubProcess()
	  public virtual void testNonInterruptingStartTimerEventInMultiInstanceEventSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingStartTimerEventInMultiInstanceEventSubProcess");

		// execute multiInstance loop number 1

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());
		string jobIdFirstLoop = jobQuery.list().get(0).Id;
		// execute timer job
		managementService.executeJob(jobIdFirstLoop);

		assertEquals(true, DummyServiceTask.wasExecuted);
		DummyServiceTask.wasExecuted = false;

		assertEquals(1, taskQuery.count());
		// complete existing task to start new execution for multi instance loop
		// number 2
		taskService.complete(taskQuery.list().get(0).Id);

		// execute multiInstance loop number 2
		assertEquals(1, taskQuery.count());
		assertEquals(1, jobQuery.count());
		string jobIdSecondLoop = jobQuery.list().get(0).Id;
		assertNotSame(jobIdFirstLoop, jobIdSecondLoop);
		// execute timer job
		managementService.executeJob(jobIdSecondLoop);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// multiInstance loop finished
		assertEquals(0, jobQuery.count());

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(1, taskQuery.count());

		// check if process instance doesn't exist because timer start event is
		// interrupting
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInParallelMultiInstanceSubProcess()
	  public virtual void testStartTimerEventSubProcessInParallelMultiInstanceSubProcess()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startTimerEventSubProcessInParallelMultiInstanceSubProcess");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(6, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// execute timer job
		foreach (Job job in jobQuery.list())
		{
		  managementService.executeJob(job.Id);

		  assertEquals(true, DummyServiceTask.wasExecuted);
		  DummyServiceTask.wasExecuted = false;
		}

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(0, taskQuery.count());

		// check if execution doesn't exist because timer start event is
		// interrupting
		assertEquals(0, executionQuery.count());

		// check if process instance doesn't exist because timer start event is
		// interrupting
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(0, processInstanceQuery.count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventSubProcessWithParallelMultiInstance()
	  public virtual void testNonInterruptingStartTimerEventSubProcessWithParallelMultiInstance()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("nonInterruptingParallelMultiInstance");

		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(6, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(2, jobQuery.count());
		// execute all timer jobs
		foreach (Job job in jobQuery.list())
		{
		  managementService.executeJob(job.Id);

		  assertEquals(true, DummyServiceTask.wasExecuted);
		  DummyServiceTask.wasExecuted = false;
		}

		assertEquals(0, jobQuery.count());

		// check if user task doesn't exist because timer start event is
		// interrupting
		assertEquals(2, taskQuery.count());

		// check if execution doesn't exist because timer start event is
		// interrupting
		assertEquals(6, executionQuery.count());

		// check if process instance doesn't exist because timer start event is
		// interrupting
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

	  /// <summary>
	  /// test scenario: - start process instance with multiInstance sequential -
	  /// execute interrupting timer job of event subprocess - execute non
	  /// interrupting timer boundary event of subprocess
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInMultiInstanceSubProcessWithNonInterruptingBoundaryTimerEvent()
	  public virtual void testStartTimerEventSubProcessInMultiInstanceSubProcessWithNonInterruptingBoundaryTimerEvent()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		// 1 start timer job and 1 boundary timer job
		assertEquals(2, jobQuery.count());
		// execute interrupting start timer event subprocess job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(1).Id);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// after first interrupting start timer event sub process execution
		// multiInstance loop number 2
		assertEquals(1, taskQuery.count());
		assertEquals(2, jobQuery.count());

		// execute non interrupting boundary timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(0).Id);

		// after non interrupting boundary timer job execution
		assertEquals(1, jobQuery.count());
		assertEquals(1, taskQuery.count());
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

	  /// <summary>
	  /// test scenario: - start process instance with multiInstance sequential -
	  /// execute interrupting timer job of event subprocess - execute interrupting
	  /// timer boundary event of subprocess
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
	  public virtual void testStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// execute multiInstance loop number 1
		// check if execution exists

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		// 1 start timer job and 1 boundary timer job
		assertEquals(2, jobQuery.count());
		// execute interrupting start timer event subprocess job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(1).Id);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// after first interrupting start timer event sub process execution
		// multiInstance loop number 2
		assertEquals(1, taskQuery.count());
		assertEquals(2, jobQuery.count());

		// execute interrupting boundary timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(0).Id);

		// after interrupting boundary timer job execution
		assertEquals(0, jobQuery.count());
		assertEquals(0, taskQuery.count());

		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
	  public virtual void testNonInterruptingStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// execute multiInstance loop number 1
		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(3, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(1, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		// 1 start timer job and 1 boundary timer job
		assertEquals(2, jobQuery.count());
		// execute non interrupting start timer event subprocess job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(1).Id);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// complete user task to finish execution of first multiInstance loop
		assertEquals(1, taskQuery.count());
		taskService.complete(taskQuery.list().get(0).Id);

		// after first non interrupting start timer event sub process execution
		// multiInstance loop number 2
		assertEquals(1, taskQuery.count());
		assertEquals(2, jobQuery.count());

		// execute interrupting boundary timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(0).Id);

		// after interrupting boundary timer job execution
		assertEquals(0, jobQuery.count());
		assertEquals(0, taskQuery.count());
		assertEquals(0, executionQuery.count());
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(0, processInstanceQuery.count());

	  }

	  /// <summary>
	  /// test scenario: - start process instance with multiInstance parallel -
	  /// execute interrupting timer job of event subprocess - execute non
	  /// interrupting timer boundary event of subprocess
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInParallelMultiInstanceSubProcessWithNonInterruptingBoundaryTimerEvent()
	  public virtual void testStartTimerEventSubProcessInParallelMultiInstanceSubProcessWithNonInterruptingBoundaryTimerEvent()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// execute multiInstance loop number 1
		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(6, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.count());

		// execute interrupting timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(1).Id);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// after interrupting timer job execution
		assertEquals(2, jobQuery.count());
		assertEquals(1, taskQuery.count());
		assertEquals(5, executionQuery.count());

		// execute non interrupting boundary timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(0).Id);

		// after non interrupting boundary timer job execution
		assertEquals(1, jobQuery.count());
		assertEquals(1, taskQuery.count());
		assertEquals(5, executionQuery.count());

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id);
		assertEquals(1, processInstanceQuery.count());

	  }

	  /// <summary>
	  /// test scenario: - start process instance with multiInstance parallel -
	  /// execute interrupting timer job of event subprocess - execute interrupting
	  /// timer boundary event of subprocess
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartTimerEventSubProcessInParallelMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
	  public virtual void testStartTimerEventSubProcessInParallelMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
	  {
		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// execute multiInstance loop number 1
		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(6, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.count());

		// execute interrupting timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(1).Id);

		// after interrupting timer job execution
		assertEquals(2, jobQuery.count());
		assertEquals(1, taskQuery.count());
		assertEquals(5, executionQuery.count());

		// execute interrupting boundary timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(0).Id);

		// after interrupting boundary timer job execution
		assertEquals(0, jobQuery.count());
		assertEquals(0, taskQuery.count());
		assertEquals(0, executionQuery.count());

		assertProcessEnded(processInstance.Id);

	  }

	  /// <summary>
	  /// test scenario: - start process instance with multiInstance parallel -
	  /// execute non interrupting timer job of event subprocess - execute
	  /// interrupting timer boundary event of subprocess
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingStartTimerEventSubProcessInParallelMiSubProcessWithInterruptingBoundaryTimerEvent()
	  public virtual void testNonInterruptingStartTimerEventSubProcessInParallelMiSubProcessWithInterruptingBoundaryTimerEvent()
	  {
		DummyServiceTask.wasExecuted = false;

		// start process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// execute multiInstance loop number 1
		// check if execution exists
		ExecutionQuery executionQuery = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id);
		assertEquals(6, executionQuery.count());

		// check if user task exists
		TaskQuery taskQuery = taskService.createTaskQuery();
		assertEquals(2, taskQuery.count());

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.count());

		// execute non interrupting timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(1).Id);

		assertEquals(true, DummyServiceTask.wasExecuted);

		// after non interrupting timer job execution
		assertEquals(2, jobQuery.count());
		assertEquals(2, taskQuery.count());
		assertEquals(6, executionQuery.count());

		// execute interrupting boundary timer job
		managementService.executeJob(jobQuery.orderByJobDuedate().asc().list().get(0).Id);

		// after interrupting boundary timer job execution
		assertEquals(0, jobQuery.count());
		assertEquals(0, taskQuery.count());
		assertEquals(0, executionQuery.count());

		assertProcessEnded(processInstance.Id);

		// start process instance again and
		// test if boundary events deleted after all tasks are completed
		processInstance = runtimeService.startProcessInstanceByKey("process");
		jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.count());

		assertEquals(2, taskQuery.count());
		// complete all existing tasks
		foreach (Task task in taskQuery.list())
		{
		  taskService.complete(task.Id);
		}

		assertEquals(0, jobQuery.count());
		assertEquals(0, taskQuery.count());
		assertEquals(0, executionQuery.count());

		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimeCycle()
	  public virtual void testTimeCycle()
	  {
		// given
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRecalculateTimeCycleExpressionCurrentDateBased() throws Exception
	  public virtual void testRecalculateTimeCycleExpressionCurrentDateBased()
	  {
		// given
		Mocks.register("cycle", "R/PT15M");

		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().timerWithCycle("${cycle}").userTask("aTaskName").endEvent().done();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		Job job = jobQuery.singleResult();
		string jobId = job.Id;
		DateTime oldDuedate = job.Duedate;

		// when
		moveByMinutes(1);
		managementService.recalculateJobDuedate(jobId, false);

		// then
		Job jobUpdated = jobQuery.singleResult();
		assertEquals(jobId, jobUpdated.Id);
		assertNotEquals(oldDuedate, jobUpdated.Duedate);
		assertTrue(oldDuedate < jobUpdated.Duedate);

		// when
		Mocks.register("cycle", "R/PT10M");
		managementService.recalculateJobDuedate(jobId, false);

		// then
		jobUpdated = jobQuery.singleResult();
		assertEquals(jobId, jobUpdated.Id);
		assertNotEquals(oldDuedate, jobUpdated.Duedate);
		assertTrue(oldDuedate > jobUpdated.Duedate);

		Mocks.reset();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRecalculateTimeCycleExpressionCreationDateBased() throws Exception
	  public virtual void testRecalculateTimeCycleExpressionCreationDateBased()
	  {
		// given
		Mocks.register("cycle", "R/PT15M");

		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().timerWithCycle("${cycle}").userTask("aTaskName").endEvent().done();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		Job job = jobQuery.singleResult();
		string jobId = job.Id;
		DateTime oldDuedate = job.Duedate;

		// when
		moveByMinutes(1);
		managementService.recalculateJobDuedate(jobId, true);

		// then
		Job jobUpdated = jobQuery.singleResult();
		assertEquals(jobId, jobUpdated.Id);
		DateTime expectedDate = LocalDateTime.fromDateFields(jobUpdated.CreateTime).plusMinutes(15).toDate();
		assertEquals(expectedDate, jobUpdated.Duedate);

		// when
		Mocks.register("cycle", "R/PT10M");
		managementService.recalculateJobDuedate(jobId, true);

		// then
		jobUpdated = jobQuery.singleResult();
		assertEquals(jobId, jobUpdated.Id);
		assertNotEquals(oldDuedate, jobUpdated.Duedate);
		assertTrue(oldDuedate > jobUpdated.Duedate);
		expectedDate = LocalDateTime.fromDateFields(jobUpdated.CreateTime).plusMinutes(10).toDate();
		assertEquals(expectedDate, jobUpdated.Duedate);

		Mocks.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFailingTimeCycle() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testFailingTimeCycle()
	  {
		// given
		JobQuery query = managementService.createJobQuery();
		JobQuery failedJobQuery = managementService.createJobQuery();

		// a job to start a process instance
		assertEquals(1, query.count());

		string jobId = query.singleResult().Id;
		failedJobQuery.jobId(jobId);

		moveByMinutes(5);

		// when (1)
		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		  // expected
		}

		// then (1)
		Job failedJob = failedJobQuery.singleResult();
		assertEquals(2, failedJob.Retries);

		// a new timer job has been created
		assertEquals(2, query.count());

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
		assertEquals(2, query.count());

		assertEquals(1, managementService.createJobQuery().withException().count());
		assertEquals(0, managementService.createJobQuery().noRetriesLeft().count());
		assertEquals(2, managementService.createJobQuery().withRetriesLeft().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingTimeCycleInEventSubProcess()
	  public virtual void testNonInterruptingTimeCycleInEventSubProcess()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

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

	  public virtual void testInterruptingWithDurationExpression()
	  {
		// given
		Mocks.register("duration", "PT60S");

		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().timerWithDuration("${duration}").userTask("aTaskName").endEvent().done();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		// when
		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		// then
		assertEquals(1, taskService.createTaskQuery().taskName("aTaskName").list().size());

		// cleanup
		Mocks.reset();
	  }

	  public virtual void testInterruptingWithDurationExpressionInEventSubprocess()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().userTask().endEvent().done();

		processBuilder.eventSubProcess().startEvent().timerWithDuration("${duration}").userTask("taskInSubprocess").endEvent();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		// when
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("duration", "PT60S"));

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		// then
		assertEquals(1, taskService.createTaskQuery().taskName("taskInSubprocess").list().size());
	  }

	  public virtual void testNonInterruptingWithDurationExpressionInEventSubprocess()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().userTask().endEvent().done();

		processBuilder.eventSubProcess().startEvent().interrupting(false).timerWithDuration("${duration}").userTask("taskInSubprocess").endEvent();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		// when
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("duration", "PT60S"));

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		// then
		assertEquals(1, taskService.createTaskQuery().taskName("taskInSubprocess").list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRecalculateNonInterruptingWithUnchangedDurationExpressionInEventSubprocessCurrentDateBased() throws Exception
	  public virtual void testRecalculateNonInterruptingWithUnchangedDurationExpressionInEventSubprocessCurrentDateBased()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().userTask().endEvent().done();

		processBuilder.eventSubProcess().startEvent().interrupting(false).timerWithDuration("${duration}").userTask("taskInSubprocess").endEvent();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("duration", "PT70S"));

		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		string jobId = job.Id;
		DateTime oldDueDate = job.Duedate;

		// when
		moveByMinutes(2);
		DateTime currentTime = ClockUtil.CurrentTime;
		managementService.recalculateJobDuedate(jobId, false);

		// then
		assertEquals(1L, jobQuery.count());
		DateTime newDuedate = jobQuery.singleResult().Duedate;
		assertNotEquals(oldDueDate, newDuedate);
		assertTrue(oldDueDate < newDuedate);
		DateTime expectedDate = LocalDateTime.fromDateFields(currentTime).plusSeconds(70).toDate();
		assertThat(newDuedate).isCloseTo(expectedDate, 1000l);

		managementService.executeJob(jobId);
		assertEquals(1, taskService.createTaskQuery().taskName("taskInSubprocess").list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRecalculateNonInterruptingWithChangedDurationExpressionInEventSubprocessCreationDateBased() throws Exception
	  public virtual void testRecalculateNonInterruptingWithChangedDurationExpressionInEventSubprocessCreationDateBased()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess("process");

		BpmnModelInstance modelInstance = processBuilder.startEvent().userTask().endEvent().done();

		processBuilder.eventSubProcess().startEvent().interrupting(false).timerWithDuration("${duration}").userTask("taskInSubprocess").endEvent();

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("duration", "PT60S"));

		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		string jobId = job.Id;
		DateTime oldDueDate = job.Duedate;

		// when
		runtimeService.setVariable(pi.Id, "duration", "PT2M");
		managementService.recalculateJobDuedate(jobId, true);

		// then
		assertEquals(1L, jobQuery.count());
		DateTime newDuedate = jobQuery.singleResult().Duedate;
		DateTime expectedDate = LocalDateTime.fromDateFields(jobQuery.singleResult().CreateTime).plusMinutes(2).toDate();
		assertTrue(oldDueDate < newDuedate);
		assertTrue(expectedDate.Equals(newDuedate));

		managementService.executeJob(jobId);
		assertEquals(1, taskService.createTaskQuery().taskName("taskInSubprocess").list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingFailingTimeCycleInEventSubProcess()
	  public virtual void testNonInterruptingFailingTimeCycleInEventSubProcess()
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

	  // util methods ////////////////////////////////////////

	  /// <summary>
	  /// executes all jobs in this threads until they are either done or retries are
	  /// exhausted.
	  /// </summary>
	  protected internal virtual void executeAllJobs()
	  {
		string nextJobId = NextExecutableJobId;

		while (!string.ReferenceEquals(nextJobId, null))
		{
		  try
		  {
			managementService.executeJob(nextJobId);
		  }
		  catch (Exception)
		  { // ignore
		  }
		  nextJobId = NextExecutableJobId;
		}

	  }

	  protected internal virtual string NextExecutableJobId
	  {
		  get
		  {
			IList<Job> jobs = managementService.createJobQuery().executable().listPage(0, 1);
			if (jobs.Count == 1)
			{
			  return jobs[0].Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  private void cleanDB()
	  {
		string jobId = managementService.createJobQuery().singleResult().Id;
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new DeleteJobsCmd(jobId, true));
	  }

	}

}