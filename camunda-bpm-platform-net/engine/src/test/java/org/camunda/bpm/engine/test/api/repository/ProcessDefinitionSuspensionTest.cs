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
namespace org.camunda.bpm.engine.test.api.repository
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerActivateProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateProcessDefinitionHandler;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	//import org.camunda.bpm.engine.impl.test.TestHelper;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class ProcessDefinitionSuspensionTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly ProcessDefinitionSuspensionTest outerInstance;

		  public CommandAnonymousInnerClass(ProcessDefinitionSuspensionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateProcessDefinitionHandler.TYPE);
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testProcessDefinitionActiveByDefault()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		assertFalse(processDefinition.Suspended);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testSuspendActivateProcessDefinitionById()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		// suspend
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);
		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);

		// activate
		repositoryService.activateProcessDefinitionById(processDefinition.Id);
		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testSuspendActivateProcessDefinitionByKey()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		//suspend
		repositoryService.suspendProcessDefinitionByKey(processDefinition.Key);
		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);

		//activate
		repositoryService.activateProcessDefinitionByKey(processDefinition.Key);
		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testActivateAlreadyActiveProcessDefinition()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		try
		{
		  repositoryService.activateProcessDefinitionById(processDefinition.Id);
		  processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		  assertFalse(processDefinition.Suspended);
		}
		catch (Exception)
		{
		  fail("Should be successful");
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testSuspendAlreadySuspendedProcessDefinition()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		try
		{
		  repositoryService.suspendProcessDefinitionById(processDefinition.Id);
		  processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		  assertTrue(processDefinition.Suspended);
		}
		catch (Exception)
		{
		  fail("Should be successful");
		}

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml", "org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml" })]
	  public virtual void testQueryForActiveDefinitions()
	  {

		// default = all definitions
		IList<ProcessDefinition> processDefinitionList = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(2, processDefinitionList.Count);

		assertEquals(2, repositoryService.createProcessDefinitionQuery().active().count());

		ProcessDefinition processDefinition = processDefinitionList[0];
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml", "org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml" })]
	  public virtual void testQueryForSuspendedDefinitions()
	  {

		// default = all definitions
		IList<ProcessDefinition> processDefinitionList = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(2, processDefinitionList.Count);

		assertEquals(2, repositoryService.createProcessDefinitionQuery().active().count());

		ProcessDefinition processDefinition = processDefinitionList[0];
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		assertEquals(2, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testStartProcessInstanceForSuspendedProcessDefinition()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		// By id
		try
		{
		  runtimeService.startProcessInstanceById(processDefinition.Id);
		  fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}

		// By Key
		try
		{
		  runtimeService.startProcessInstanceByKey(processDefinition.Key);
		  fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testContinueProcessAfterProcessDefinitionSuspend()
	  {

		// Start Process Instance
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(processDefinition.Key);

		// Verify one task is created
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// Suspend process definition
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		// Process should be able to continue
		taskService.complete(task.Id);
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendProcessInstancesDuringProcessDefinitionSuspend()
	  {

		int nrOfProcessInstances = 9;

		// Fire up a few processes for the deployed process definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		for (int i = 0; i < nrOfProcessInstances; i++)
		{
		  runtimeService.startProcessInstanceByKey(processDefinition.Key);
		}
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().active().count());

		// Suspend process definitions and include process instances
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		// Verify all process instances are also suspended
		foreach (ProcessInstance processInstance in runtimeService.createProcessInstanceQuery().list())
		{
		  assertTrue(processInstance.Suspended);
		}

		// Verify all process instances can't be continued
		foreach (Task task in taskService.createTaskQuery().list())
		{
		  try
		  {
			assertTrue(task.Suspended);
			taskService.complete(task.Id);
			fail("A suspended task shouldn't be able to be continued");
		  }
		  catch (SuspendedEntityInteractionException e)
		  {
			assertTextPresentIgnoreCase("is suspended", e.Message);
		  }
		}
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().count());
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().active().count());

		// Activate the process definition again
		repositoryService.activateProcessDefinitionById(processDefinition.Id, true, null);

		// Verify that all process instances can be completed
		foreach (Task task in taskService.createTaskQuery().list())
		{
		  taskService.complete(task.Id);
		}
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().active().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSubmitStartFormAfterProcessDefinitionSuspend()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		try
		{
		  formService.submitStartFormData(processDefinition.Id, new Dictionary<string, string>());
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}

		try
		{
		  formService.submitStartFormData(processDefinition.Id, "someKey", new Dictionary<string, string>());
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJobIsExecutedOnProcessDefinitionSuspend()
	  public virtual void testJobIsExecutedOnProcessDefinitionSuspend()
	  {

		DateTime now = DateTime.Now;
		ClockUtil.CurrentTime = now;

		// Suspending the process definition should not stop the execution of jobs
		// Added this test because in previous implementations, this was the case.
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);
		assertEquals(1, managementService.createJobQuery().count());

		// The jobs should simply be executed
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);
		assertEquals(0, managementService.createJobQuery().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDelayedSuspendProcessDefinition()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;

		// Suspend process definition in one week from now
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, false, new DateTime(oneWeekFromStartTime));

		// Verify we can just start process instances
		runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());

		// execute job
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// Try to start process instance. It should fail now.
		try
		{
		  runtimeService.startProcessInstanceById(processDefinition.Id);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("suspended", e.Message);
		}
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Activate again
		repositoryService.activateProcessDefinitionById(processDefinition.Id);
		runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDelayedSuspendProcessDefinitionIncludingProcessInstances()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;

		// Start some process instances
		int nrOfProcessInstances = 30;
		for (int i = 0; i < nrOfProcessInstances; i++)
		{
		  runtimeService.startProcessInstanceById(processDefinition.Id);
		}

		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().count());
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(0, taskService.createTaskQuery().suspended().count());
		assertEquals(nrOfProcessInstances, taskService.createTaskQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Suspend process definition in one week from now
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, new DateTime(oneWeekFromStartTime));

		// Verify we can start process instances
		runtimeService.startProcessInstanceById(processDefinition.Id);
		nrOfProcessInstances = nrOfProcessInstances + 1;
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().count());

		// execute job
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// Try to start process instance. It should fail now.
		try
		{
		  runtimeService.startProcessInstanceById(processDefinition.Id);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("suspended", e.Message);
		}
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(nrOfProcessInstances, taskService.createTaskQuery().suspended().count());
		assertEquals(0, taskService.createTaskQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Activate again
		repositoryService.activateProcessDefinitionById(processDefinition.Id, true, null);
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().count());
		assertEquals(nrOfProcessInstances, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(0, taskService.createTaskQuery().suspended().count());
		assertEquals(nrOfProcessInstances, taskService.createTaskQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDelayedActivateProcessDefinition()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		// Try to start process instance. It should fail now.
		try
		{
		  runtimeService.startProcessInstanceById(processDefinition.Id);
		  fail();
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("suspended", e.Message);
		}
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Activate in a day from now
		long oneDayFromStart = startTime.Ticks + (24 * 60 * 60 * 1000);
		repositoryService.activateProcessDefinitionById(processDefinition.Id, false, new DateTime(oneDayFromStart));

		// execute job
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// Starting a process instance should now succeed
		runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
	  }

	  public virtual void testSuspendMultipleProcessDefinitionsByKey()
	  {

		// Deploy three processes
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml").deploy();
		}
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Suspend all process definitions with same key
		repositoryService.suspendProcessDefinitionByKey("oneTaskProcess");
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Activate again
		repositoryService.activateProcessDefinitionByKey("oneTaskProcess");
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Start process instance
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// And suspend again, cascading to process instances
		repositoryService.suspendProcessDefinitionByKey("oneTaskProcess", true, null);
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().suspended().count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().active().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testDelayedSuspendMultipleProcessDefinitionsByKey()
	  {

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// Deploy five versions of the same process
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml").deploy();
		}
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());

		// Start process instance
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// Suspend all process definitions with same key in 2 hours from now
		repositoryService.suspendProcessDefinitionByKey("oneTaskProcess", true, new DateTime(startTime.Ticks + (2 * hourInMs)));
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().active().count());

		// execute job
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().suspended().count());

		// Activate again in 5 hourse from now
		repositoryService.activateProcessDefinitionByKey("oneTaskProcess", true, new DateTime(startTime.Ticks + (5 * hourInMs)));
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().suspended().count());

		// execute job
		job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().count());
		assertEquals(nrOfProcessDefinitions, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().suspended().count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testSuspendById_shouldSuspendJobDefinitionAndRetainJob()
	  {
		// given

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testSuspendByKey_shouldSuspendJobDefinitionAndRetainJob()
	  {
		// given

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(processDefinition.Key);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testSuspendByIdAndIncludeInstancesFlag_shouldSuspendAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, false, null);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testSuspendByKeyAndIncludeInstancesFlag_shouldSuspendAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(processDefinition.Key, false, null);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be suspended too
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(processDefinition.Key, true, null);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be suspended too
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(2, jobQuery.active().count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still active
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.suspendProcessDefinitionByKey(processDefinition.Key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(2, jobQuery.active().count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still active
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedSuspendByIdAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(2, jobQuery.active().count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still active
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedSuspendByKeyAndIncludeInstancesFlag_shouldSuspendJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// when
		// the process definition will be suspended in 2 hours
		repositoryService.suspendProcessDefinitionByKey(processDefinition.Key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to suspend process definition
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// the job definition should still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(2, jobQuery.active().count()); // there exists two jobs, a failing job and a timer job

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still active
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  public virtual void testMultipleSuspendByKey_shouldSuspendJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(key);

		// then
		// the job definitions should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(5, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(5, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, false, null);

		// then
		// the job definitions should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(5, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(5, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, null);

		// then
		// the job definitions should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(5, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be suspended too
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(5, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testDelayedMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to suspend the process definition delayed
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// the job definitions should be still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(5, jobDefinitionQuery.active().count());

		// ...and the corresponding jobs should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(6, jobQuery.active().count());

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definitions should be suspended...
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(5, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be still active
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(5, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testDelayedMultipleSuspendByKeyAndIncludeInstances_shouldSuspendJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to suspend the process definition delayed
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// the job definitions should be still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(5, jobDefinitionQuery.active().count());

		// ...and the corresponding jobs should be still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(6, jobQuery.active().count());

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// the job definitions should be suspended...
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(5, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be suspended too
		assertEquals(0, jobQuery.active().count());
		assertEquals(5, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testActivationById_shouldActivateJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionById(processDefinition.Id);

		// then
		// the job definition should be active...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and the corresponding job should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testActivationByKey_shouldActivateJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(processDefinition.Key);

		// then
		// the job definition should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// ...and the corresponding job should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testActivationByIdAndIncludeInstancesFlag_shouldActivateAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionById(processDefinition.Id, false, null);

		// then
		// the job definition should be suspended...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// ...and the corresponding job should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testActivationByKeyAndIncludeInstancesFlag_shouldActivateAlsoJobDefinitionAndRetainJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(processDefinition.Key, false, null);

		// then
		// the job definition should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// ...and the corresponding job should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionById(processDefinition.Id, true, null);

		// then
		// the job definition should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// ...and the corresponding job should be activated too
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(processDefinition.Key, true, null);

		// then
		// the job definition should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertFalse(suspendedJobDefinition.Suspended);

		// ...and the corresponding job should be activated too
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.activateProcessDefinitionById(processDefinition.Id, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		Job timerToActivateProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count()); // the timer job is active

		// when
		// execute job
		managementService.executeJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be active
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the job is still suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.activateProcessDefinitionByKey(processDefinition.Key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		Job timerToActivateProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count()); // the timer job is active

		// when
		// execute job
		managementService.executeJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// the job is still suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job job = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertTrue(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedActivationByIdAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.activateProcessDefinitionById(processDefinition.Id, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		Job timerToActivateProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count()); // the timer job is active

		// when
		// execute job
		managementService.executeJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be activated
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// the job is activated
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testDelayedActivationByKeyAndIncludeInstancesFlag_shouldActivateJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		// a process definition with a asynchronous continuation, so that there
		// exists a job definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceById(processDefinition.Id, @params);

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(1, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated in 2 hours
		repositoryService.activateProcessDefinitionByKey(processDefinition.Key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a job to activate process definition
		Job timerToActivateProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToActivateProcessDefinition);

		// the job definition should still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count()); // the timer job is active

		// when
		// execute job
		managementService.executeJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definition should be activated
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activatedJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activatedJobDefinition.Id);
		assertFalse(activatedJobDefinition.Suspended);

		// the job is activated too
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job job = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, job.JobDefinitionId);
		assertFalse(job.Suspended);
	  }

	  public virtual void testMultipleActivationByKey_shouldActivateJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(5, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(5, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(key);

		// then
		// the job definitions should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(5, jobDefinitionQuery.active().count());

		// ...and the corresponding jobs should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(5, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndRetainJob()
	  {
		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(5, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(5, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(key, false, null);

		// then
		// the job definitions should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(5, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(5, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndJob()
	  {

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(5, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(5, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(key, true, null);

		// then
		// the job definitions should be activated...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(5, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be activated too
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(5, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testDelayedMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndRetainJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(5, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(5, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(key, false, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to activate the process definition delayed
		Job timerToActivateProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToActivateProcessDefinition);

		// the job definitions should be still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(5, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// ...and the corresponding jobs should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(5, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		// when
		// execute job
		managementService.executeJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definitions should be activated...
		assertEquals(5, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be still suspended
		assertEquals(5, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testDelayedMultipleActivationByKeyAndIncludeInstances_shouldActivateJobDefinitionAndJob()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// Deploy five versions of the same process, so that there exists
		// five job definitions
		int nrOfProcessDefinitions = 5;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn").deploy();

		  // a running process instance with a failed service task
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// the process definition, job definition, process instance and job will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, true, null);

		assertEquals(0, repositoryService.createProcessDefinitionQuery().active().count());
		assertEquals(5, repositoryService.createProcessDefinitionQuery().suspended().count());

		assertEquals(0, managementService.createJobDefinitionQuery().active().count());
		assertEquals(5, managementService.createJobDefinitionQuery().suspended().count());

		// when
		// the process definition will be activated
		repositoryService.activateProcessDefinitionByKey(key, true, new DateTime(startTime.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to activate the process definition delayed
		Job timerToActivateProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToActivateProcessDefinition);

		// the job definitions should be still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(5, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// ...and the corresponding jobs should be still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(5, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		// when
		// execute job
		managementService.executeJob(timerToActivateProcessDefinition.Id);

		// then
		// the job definitions should be activated...
		assertEquals(5, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// ...and the corresponding jobs should be activated too
		assertEquals(5, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }


	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"})]
	  public virtual void testSuspendStartTimerOnProcessDefinitionSuspensionByKey()
	  {
		Job startTimer = managementService.createJobQuery().timers().singleResult();

		assertFalse(startTimer.Suspended);

		// when
		repositoryService.suspendProcessDefinitionByKey("process");

		// then

		// refresh job
		startTimer = managementService.createJobQuery().timers().singleResult();
		assertTrue(startTimer.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"})]
	  public virtual void testSuspendStartTimerOnProcessDefinitionSuspensionById()
	  {
		ProcessDefinition pd = repositoryService.createProcessDefinitionQuery().singleResult();

		Job startTimer = managementService.createJobQuery().timers().singleResult();

		assertFalse(startTimer.Suspended);

		// when
		repositoryService.suspendProcessDefinitionById(pd.Id);

		// then

		// refresh job
		startTimer = managementService.createJobQuery().timers().singleResult();
		assertTrue(startTimer.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"})]
	  public virtual void testActivateStartTimerOnProcessDefinitionSuspensionByKey()
	  {
		repositoryService.suspendProcessDefinitionByKey("process");

		Job startTimer = managementService.createJobQuery().timers().singleResult();
		assertTrue(startTimer.Suspended);

		// when
		repositoryService.activateProcessDefinitionByKey("process");
		// then

		// refresh job
		startTimer = managementService.createJobQuery().timers().singleResult();
		assertFalse(startTimer.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testSuspendStartTimerOnProcessDefinitionSuspension.bpmn20.xml"})]
	  public virtual void testActivateStartTimerOnProcessDefinitionSuspensionById()
	  {
		ProcessDefinition pd = repositoryService.createProcessDefinitionQuery().singleResult();
		repositoryService.suspendProcessDefinitionById(pd.Id);

		Job startTimer = managementService.createJobQuery().timers().singleResult();

		assertTrue(startTimer.Suspended);

		// when
		repositoryService.activateProcessDefinitionById(pd.Id);

		// then

		// refresh job
		startTimer = managementService.createJobQuery().timers().singleResult();
		assertFalse(startTimer.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartBeforeActivityForSuspendProcessDefinition()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		//start process instance
		runtimeService.startProcessInstanceById(processDefinition.Id);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspend process definition
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		// try to start before activity for suspended processDefinition
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();
		  fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStartAfterActivityForSuspendProcessDefinition()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		//start process instance
		runtimeService.startProcessInstanceById(processDefinition.Id);
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();

		// Suspend process definition
		repositoryService.suspendProcessDefinitionById(processDefinition.Id, true, null);

		// try to start after activity for suspended processDefinition
		try
		{
		  runtimeService.createProcessInstanceModification(processInstance.Id).startAfterActivity("theTask").execute();
		  fail("Exception is expected but not thrown");
		}
		catch (SuspendedEntityInteractionException e)
		{
		  assertTextPresentIgnoreCase("is suspended", e.Message);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testSuspendAndActivateProcessDefinitionByIdUsingBuilder()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		// suspend
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);

		// activate
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).activate();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testSuspendAndActivateProcessDefinitionByKeyUsingBuilder()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		// suspend
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinition.Key).suspend();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);

		// activate
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinition.Key).activate();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDelayedSuspendProcessDefinitionUsingBuilder()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// suspend process definition in one week from now
		long oneWeekFromStartTime = (DateTime.Now).Ticks + (7 * 24 * 60 * 60 * 1000);

		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).executionDate(new DateTime(oneWeekFromStartTime)).suspend();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		// execute the suspension job
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDelayedActivateProcessDefinitionUsingBuilder()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// suspend
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(processDefinition.Key).suspend();

		// activate process definition in one week from now
		long oneWeekFromStartTime = (DateTime.Now).Ticks + (7 * 24 * 60 * 60 * 1000);

		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).executionDate(new DateTime(oneWeekFromStartTime)).activate();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);

		// execute the activation job
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testSuspendAndActivateProcessDefinitionIncludeInstancesUsingBuilder()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		assertFalse(processDefinition.Suspended);
		assertFalse(processInstance.Suspended);

		// suspend
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).includeProcessInstances(true).suspend();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertTrue(processDefinition.Suspended);

		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertTrue(processInstance.Suspended);

		// activate
		repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).includeProcessInstances(true).activate();

		processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertFalse(processDefinition.Suspended);

		processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertFalse(processInstance.Suspended);
	  }

	}

}