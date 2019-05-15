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
	using TimerSuspendJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendJobDefinitionHandler;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class SuspendJobDefinitionTest : PluggableProcessEngineTestCase
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
		  private readonly SuspendJobDefinitionTest outerInstance;

		  public CommandAnonymousInnerClass(SuspendJobDefinitionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendJobDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  // Test ManagementService#suspendJobDefinitionById() /////////////////////////

	  public virtual void testSuspensionById_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionById(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testSuspensionByIdAndSuspendJobsFlag_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionById(null, false);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionById(null, true);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testSuspensionByIdAndSuspendJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionById(null, false, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionById(null, true, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionById(null, false, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionById(null, true, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionById_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);

		// there does not exist any active job definition
		jobDefinitionQuery = managementService.createJobDefinitionQuery().active();
		assertTrue(jobDefinitionQuery.list().Empty);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByIdAndSuspendJobsFlag_shouldRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id, false);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByIdAndSuspendJobsFlag_shouldSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id, true);

		// then
		// there exists a suspended job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and a suspended job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionById_shouldExecuteImmediatelyAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id, false, null);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionById_shouldExecuteImmediatelyAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id, true, null);

		// then
		// there exists a suspended job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and a suspended job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionById_shouldExecuteDelayedAndRetainJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id, false, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionById_shouldExecuteDelayedAndSuspendJobs()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// a job definition (which was created for the asynchronous continuation)
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionById(jobDefinition.Id, true, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still suspended
		jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);

		assertTrue(suspendedJob.Suspended);

		jobQuery = managementService.createJobQuery().active();
		assertEquals(0, jobQuery.count());
	  }

	  // Test ManagementService#suspendJobDefinitionByProcessDefinitionId() /////////////////////////

	  public virtual void testSuspensionByProcessDefinitionId_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlag_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null, false);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null, true);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null, false, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null, true, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null, false, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionId(null, true, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionId_shouldRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);

		// there does not exist any active job definition
		jobDefinitionQuery = managementService.createJobDefinitionQuery().active();
		assertTrue(jobDefinitionQuery.list().Empty);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlag_shouldRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id, false);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlag_shouldSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id, true);

		// then
		// there exists a suspended job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and a suspended job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionId_shouldExecuteImmediatelyAndRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id, false, null);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionId_shouldExecuteImmediatelyAndSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id, true, null);

		// then
		// there exists a suspended job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and a suspended job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionId_shouldExecuteDelayedAndRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id, false, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		jobQuery = managementService.createJobQuery().active();

		assertEquals(1, jobQuery.count());

		Job activeJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);

		jobQuery = managementService.createJobQuery().suspended();
		assertEquals(0, jobQuery.count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionId_shouldExecuteDelayedAndSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinition.Id, true, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is suspended
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  // Test ManagementService#suspendJobDefinitionByProcessDefinitionKey() /////////////////////////

	  public virtual void testSuspensionByProcessDefinitionKey_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null, false);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null, true);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null, false, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null, true, null);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null, false, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

		try
		{
		  managementService.suspendJobDefinitionByProcessDefinitionKey(null, true, DateTime.Now);
		  fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKey_shouldRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);

		// there does not exist any active job definition
		jobDefinitionQuery = managementService.createJobDefinitionQuery().active();
		assertTrue(jobDefinitionQuery.list().Empty);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, false);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);

		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, true);

		// then
		// there exists a suspended job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and a suspended job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, null);

		// then
		// there exists a suspended job definition
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, null);

		// then
		// there exists a suspended job definition...
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().suspended();

		assertEquals(1, jobDefinitionQuery.count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// ...and a suspended job of the provided job definition
		JobQuery jobQuery = managementService.createJobQuery().suspended();

		assertEquals(1, jobQuery.count());

		Job suspendedJob = jobQuery.singleResult();
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is still active
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(1, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(1, jobDefinitionQuery.suspended().count());

		JobDefinition suspendedJobDefinition = jobDefinitionQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJobDefinition.Id);
		assertTrue(suspendedJobDefinition.Suspended);

		// the corresponding job is suspended
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  // Test ManagementService#suspendJobDefinitionByProcessDefinitionKey() with multiple process definition
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key);

		// then
		// all job definitions are suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// but the jobs are still active
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldRetainJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, false);

		// then
		// all job definitions are suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// but the jobs are still active
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldSuspendJobs()
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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, true);

		// then
		// all job definitions are suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// and the jobs too
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, false, null);

		// then
		// all job definitions are suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// but the jobs are still active
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, true, null);

		// then
		// all job definitions are suspended
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.suspended().count());
		assertEquals(0, jobDefinitionQuery.active().count());

		// and the jobs too
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(3, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());

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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, false, oneWeekLater());

		// then
		// the job definition is still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(3, jobDefinitionQuery.suspended().count());

		// but the jobs are still active
		jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

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

		// when
		// suspend the job definition
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, true, oneWeekLater());

		// then
		// the job definitions are still active
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery();
		assertEquals(3, jobDefinitionQuery.active().count());
		assertEquals(0, jobDefinitionQuery.suspended().count());

		// there exists a job for the delayed suspension execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedSuspensionJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, jobDefinitionQuery.active().count());
		assertEquals(3, jobDefinitionQuery.suspended().count());

		// the corresponding jobs are suspended
		jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(3, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByIdUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();
		assertFalse(jobDefinition.Suspended);

		// when
		// suspend the job definition
		managementService.updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinition.Id).suspend();

		// then
		// there exists a suspended job definition
		assertEquals(1, query.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionIdUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();
		assertFalse(jobDefinition.Suspended);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// when
		// suspend the job definition
		managementService.updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();

		// then
		// there exists a suspended job definition
		assertEquals(1, query.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKeyUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();
		assertFalse(jobDefinition.Suspended);

		// when
		// suspend the job definition
		managementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey("suspensionProcess").suspend();

		// then
		// there exists a suspended job definition
		assertEquals(1, query.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionJobDefinitionIncludeJobsdUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();
		assertFalse(jobDefinition.Suspended);

		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());


		// when
		// suspend the job definition and the job
		managementService.updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinition.Id).includeJobs(true).suspend();

		// then
		// there exists a suspended job definition and job
		assertEquals(1, query.suspended().count());

		assertEquals(1, jobQuery.suspended().count());
		assertEquals(0, jobQuery.active().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testDelayedSuspensionUsingBuilder()
	  {
		// given
		// a deployed process definition with asynchronous continuation

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// a job definition (which was created for the asynchronous continuation)
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		JobDefinition jobDefinition = query.singleResult();

		// when
		// suspend the job definition in one week
		managementService.updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinition.Id).executionDate(oneWeekLater()).suspend();

		// then
		// the job definition is still active
		assertEquals(1, query.active().count());
		assertEquals(0, query.suspended().count());

		// there exists a job for the delayed suspension execution
		Job delayedSuspensionJob = managementService.createJobQuery().timers().active().singleResult();
		assertNotNull(delayedSuspensionJob);

		// execute job
		managementService.executeJob(delayedSuspensionJob.Id);

		// the job definition should be suspended
		assertEquals(0, query.active().count());
		assertEquals(1, query.suspended().count());
	  }

	  protected internal virtual DateTime oneWeekLater()
	  {
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);
		return new DateTime(oneWeekFromStartTime);
	  }

	}

}