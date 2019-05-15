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
namespace org.camunda.bpm.engine.test.api.mgmt
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerActivateJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateJobDefinitionHandler;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	public class ActivateJobDefinitionTest : PluggableProcessEngineTestCase
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
		  private readonly ActivateJobDefinitionTest outerInstance;

		  public CommandAnonymousInnerClass(ActivateJobDefinitionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateJobDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  // Test ManagementService#activateJobDefinitionById() /////////////////////////

	  public virtual void testActivationById_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionById(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testActivationByIdAndActivateJobsFlag_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionById(null, false);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionById(null, true);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testActivationByIdAndActivateJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionById(null, false, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionById(null, true, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionById(null, false, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionById(null, true, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationById_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id);

		// then
		// there exists a active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByIdAndActivateJobsFlag_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id, false);

		// then
		// there exists a active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByIdAndActivateJobsFlag_shouldSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id, true);

		// then
		// there exists an active job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and a active job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationById_shouldExecuteImmediatelyAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id, false, null);

		// then
		// there exists an active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationById_shouldExecuteImmediatelyAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id, true, null);

		// then
		// there exists an active job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and an active job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationById_shouldExecuteDelayedAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id, false, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be suspended
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationById_shouldExecuteDelayedAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionById(jobDefinition.Id, true, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is active
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);
	  }

	  // Test ManagementService#activateJobDefinitionByProcessDefinitionId() /////////////////////////

	  public virtual void testActivationByProcessDefinitionId_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlag_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null, false);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null, true);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null, false, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null, true, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null, false, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionId(null, true, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionId_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id);

		// then
		// there exists a active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlag_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id, false);

		// then
		// there exists an active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlag_shouldSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id, true);

		// then
		// there exists an active job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and an active job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionId_shouldExecuteImmediatelyAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id, false, null);

		// then
		// there exists an active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionId_shouldExecuteImmediatelyAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id, true, null);

		// then
		// there exists an active job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and an active job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionId_shouldExecuteDelayedAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id, false, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionId_shouldExecuteDelayedAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinition.Id, true, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is active
		jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);
	  }

	  // Test ManagementService#activateJobDefinitionByProcessDefinitionKey() /////////////////////////

	  public virtual void testActivationByProcessDefinitionKey_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlag_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null, false);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null, true);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null, false, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null, true, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null, false, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.activateJobDefinitionByProcessDefinitionKey(null, true, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKey_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key);

		// then
		// there exists a active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlag_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key, false);

		// then
		// there exists an active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlag_shouldSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key, true);

		// then
		// there exists an active job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and an active job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job suspendedJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertFalse(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKey_shouldExecuteImmediatelyAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, null);

		// then
		// there exists an active job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKey_shouldExecuteImmediatelyAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, null);

		// then
		// there exists an active job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();

		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(1, jobDefinitionQuery.active().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// ...and an active job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job suspendedJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertFalse(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKey_shouldExecuteDelayedAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is still suspended
		jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKey_shouldExecuteDelayedAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		JobDefinition activeJobDefinition = jobDefinitionQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJobDefinition.Id);
		assertFalse(activeJobDefinition.Suspended);

		// the corresponding job is active
		jobQuery = managementService.createJobQuery();

		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);

	  }

	  // Test ManagementService#activateJobDefinitionByProcessDefinitionKey() with multiple process definition
	  // with same process definition key

	  public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldRetainJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key);

		// then
		// all job definitions are active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(3, jobDefinitionQuery.active().count());

		// but the jobs are still suspended
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKeyAndActivateJobsFlag_shouldRetainJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key, false);

		// then
		// all job definitions are active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(3, jobDefinitionQuery.active().count());

		// but the jobs are still suspended
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKeyAndActivateJobsFlag_shouldSuspendJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key, true);

		// then
		// all job definitions are active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(3, jobDefinitionQuery.active().count());

		// and the jobs too
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndRetainJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key, false, null);

		// then
		// all job definitions are active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(3, jobDefinitionQuery.active().count());

		// but the jobs are still suspended
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndSuspendJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key, true, null);

		// then
		// all job definitions are active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.suspended().count());
		assertEquals(3, jobDefinitionQuery.active().count());

		// and the jobs too
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndRetainJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key, false, oneWeekLater());

		// then
		// the job definition is still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(3, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(3, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// but the jobs are still suspended
		jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndSuspendJobs()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionKey(key, true, oneWeekLater());

		// then
		// the job definitions are still suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(3, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be active
		assertEquals(3, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// the corresponding jobs are active
		jobQuery = managementService.createJobQuery();

		assertEquals(3, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByIdUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();

		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		// when
		// activate the job definition
		managementService.updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinition.Id).activate();

		// then
		// there exists a active job definition
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionIdUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// when
		// activate the job definition
		managementService.updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).activate();

		// then
		// there exists a active job definition
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKeyUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		// when
		// activate the job definition
		managementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey("suspensionProcess").activate();

		// then
		// there exists a active job definition
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationJobDefinitionIncludingJobsUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		// when
		// activate the job definition
		managementService.updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinition.Id).includeJobs(true).activate();

		// then
		// there exists a active job definition
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testDelayedActivationUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();

		// when
		// activate the job definition
		managementService.updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinition.Id).executionDate(oneWeekLater()).activate();

		// then
		// the job definition is still suspended
		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());

		// there exists a job for the delayed activation execution
		Job delayedActivationJob = managementService.createJobQuery().timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		// the job definition should be suspended
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());
	  }

	  protected internal virtual DateTime oneWeekLater()
	  {
		// one week from now
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);
		return new DateTime(oneWeekFromStartTime);
	  }

	}

}