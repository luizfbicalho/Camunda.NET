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
namespace org.camunda.bpm.engine.test.concurrency
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using ExecuteJobsCmd = org.camunda.bpm.engine.impl.cmd.ExecuteJobsCmd;
	using SetJobDefinitionPriorityCmd = org.camunda.bpm.engine.impl.cmd.SetJobDefinitionPriorityCmd;
	using SuspendJobCmd = org.camunda.bpm.engine.impl.cmd.SuspendJobCmd;
	using SuspendJobDefinitionCmd = org.camunda.bpm.engine.impl.cmd.SuspendJobDefinitionCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using ExecuteJobHelper = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobHelper;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobFailureCollector = org.camunda.bpm.engine.impl.jobexecutor.JobFailureCollector;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using UpdateJobSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobSuspensionStateBuilderImpl;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ConcurrentJobExecutorTest
	{
		private bool InstanceFieldsInitialized = false;

		public ConcurrentJobExecutorTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;


	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal ManagementService managementService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal Thread testThread = Thread.CurrentThread;
	  protected internal static ControllableThread activeThread;

	  protected internal static readonly BpmnModelInstance SIMPLE_ASYNC_PROCESS = Bpmn.createExecutableProcess("simpleAsyncProcess").startEvent().serviceTask().camundaExpression("${true}").camundaAsyncBefore().endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void deleteJobs()
	  public virtual void deleteJobs()
	  {
		foreach (Job job in managementService.createJobQuery().list())
		{

		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		}
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ConcurrentJobExecutorTest outerInstance;

		  public CommandAnonymousInnerClass(ConcurrentJobExecutorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			((JobEntity) job).delete();
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompetingJobExecutionDeleteJobDuringExecution()
	  public virtual void testCompetingJobExecutionDeleteJobDuringExecution()
	  {
		//given a simple process with a async service task
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().serviceTask("task").camundaAsyncBefore().camundaExpression("${true}").endEvent().done());
		runtimeService.startProcessInstanceByKey("process");
		Job currentJob = managementService.createJobQuery().singleResult();

		// when a job is executed
		JobExecutionThread threadOne = new JobExecutionThread(this, currentJob.Id);
		threadOne.startAndWaitUntilControlIsReturned();
		//and deleted in parallel
		managementService.deleteJob(currentJob.Id);

		// then the job fails with a OLE and the failed job listener throws no NPE
		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertTrue(threadOne.exception is OptimisticLockingException);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testCompetingJobExecutionDefaultRetryStrategy()
	  public virtual void testCompetingJobExecutionDefaultRetryStrategy()
	  {
		// given an MI subprocess with two instances
		runtimeService.startProcessInstanceByKey("miParallelSubprocess");

		IList<Job> currentJobs = managementService.createJobQuery().list();
		assertEquals(2, currentJobs.Count);

		// when the jobs are executed in parallel
		JobExecutionThread threadOne = new JobExecutionThread(this, currentJobs[0].Id);
		threadOne.startAndWaitUntilControlIsReturned();

		JobExecutionThread threadTwo = new JobExecutionThread(this, currentJobs[1].Id);
		threadTwo.startAndWaitUntilControlIsReturned();

		// then the first committing thread succeeds
		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		// then the second committing thread fails with an OptimisticLockingException
		// and the job retries have not been decremented
		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);

		Job remainingJob = managementService.createJobQuery().singleResult();
		assertEquals(currentJobs[1].Retries, remainingJob.Retries);

		assertNotNull(remainingJob.ExceptionMessage);

		JobEntity jobEntity = (JobEntity) remainingJob;
		assertNull(jobEntity.LockOwner);

		// and there is no lock expiration time due to the default retry strategy
		assertNull(jobEntity.LockExpirationTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testCompetingJobExecutionFoxRetryStrategy()
	  public virtual void testCompetingJobExecutionFoxRetryStrategy()
	  {
		// given an MI subprocess with two instances
		runtimeService.startProcessInstanceByKey("miParallelSubprocess");

		IList<Job> currentJobs = managementService.createJobQuery().list();
		assertEquals(2, currentJobs.Count);

		// when the jobs are executed in parallel
		JobExecutionThread threadOne = new JobExecutionThread(this, currentJobs[0].Id);
		threadOne.startAndWaitUntilControlIsReturned();

		JobExecutionThread threadTwo = new JobExecutionThread(this, currentJobs[1].Id);
		threadTwo.startAndWaitUntilControlIsReturned();

		// then the first committing thread succeeds
		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		// then the second committing thread fails with an OptimisticLockingException
		// and the job retries have not been decremented
		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);

		Job remainingJob = managementService.createJobQuery().singleResult();
		// retries are configured as R5/PT5M, so no decrement means 5 retries left
		assertEquals(5, remainingJob.Retries);

		assertNotNull(remainingJob.ExceptionMessage);

		JobEntity jobEntity = (JobEntity) remainingJob;
		assertNull(jobEntity.LockOwner);

		// and there is a custom lock expiration time
		assertNotNull(jobEntity.LockExpirationTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletingJobExecutionSuspendDuringExecution()
	  public virtual void testCompletingJobExecutionSuspendDuringExecution()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");
		Job job = managementService.createJobQuery().singleResult();

		// given a waiting execution and a waiting suspension
		JobExecutionThread executionthread = new JobExecutionThread(this, job.Id);
		executionthread.startAndWaitUntilControlIsReturned();

		JobSuspensionThread jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
		jobSuspensionThread.startAndWaitUntilControlIsReturned();

		// first complete suspension:
		jobSuspensionThread.proceedAndWaitTillDone();
		executionthread.proceedAndWaitTillDone();

		// then the execution will fail with optimistic locking
		assertNull(jobSuspensionThread.exception);
		assertNotNull(executionthread.exception);

		//--------------------------------------------

		// given a waiting execution and a waiting suspension
		executionthread = new JobExecutionThread(this, job.Id);
		executionthread.startAndWaitUntilControlIsReturned();

		jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
		jobSuspensionThread.startAndWaitUntilControlIsReturned();

		// first complete execution:
		executionthread.proceedAndWaitTillDone();
		jobSuspensionThread.proceedAndWaitTillDone();

		// then there are no optimistic locking exceptions
		assertNull(jobSuspensionThread.exception);
		assertNull(executionthread.exception);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletingSuspendJobDuringAcquisition()
	  public virtual void testCompletingSuspendJobDuringAcquisition()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// given a waiting acquisition and a waiting suspension
		JobAcquisitionThread acquisitionThread = new JobAcquisitionThread(this);
		acquisitionThread.startAndWaitUntilControlIsReturned();

		JobSuspensionThread jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
		jobSuspensionThread.startAndWaitUntilControlIsReturned();

		// first complete suspension:
		jobSuspensionThread.proceedAndWaitTillDone();
		acquisitionThread.proceedAndWaitTillDone();

		// then the acquisition will not fail with optimistic locking
		assertNull(jobSuspensionThread.exception);
		assertNull(acquisitionThread.exception);
		// but the job will also not be acquired
		assertEquals(0, acquisitionThread.acquiredJobs.size());

		//--------------------------------------------

		// given a waiting acquisition and a waiting suspension
		acquisitionThread = new JobAcquisitionThread(this);
		acquisitionThread.startAndWaitUntilControlIsReturned();

		jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
		jobSuspensionThread.startAndWaitUntilControlIsReturned();

		// first complete acquisition:
		acquisitionThread.proceedAndWaitTillDone();
		jobSuspensionThread.proceedAndWaitTillDone();

		// then there are no optimistic locking exceptions
		assertNull(jobSuspensionThread.exception);
		assertNull(acquisitionThread.exception);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletingSuspendedJobDuringRunningInstance()
	  public virtual void testCompletingSuspendedJobDuringRunningInstance()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().receiveTask().intermediateCatchEvent().timerWithDuration("PT0M").endEvent().done());

		// given
		// a process definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		// suspend the process definition (and the job definitions)
		repositoryService.suspendProcessDefinitionById(processDefinition.Id);

		// assert that there still exists a running and active process instance
		assertEquals(1, runtimeService.createProcessInstanceQuery().active().count());

		// when
		runtimeService.signal(processInstance.Id);

		// then
		// there should be one suspended job
		assertEquals(1, managementService.createJobQuery().suspended().count());
		assertEquals(0, managementService.createJobQuery().active().count());

		assertEquals(1, runtimeService.createProcessInstanceQuery().active().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletingUpdateJobDefinitionPriorityDuringExecution()
	  public virtual void testCompletingUpdateJobDefinitionPriorityDuringExecution()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		// given
		// two running instances
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// and a job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// and two jobs
		IList<Job> jobs = managementService.createJobQuery().list();

		// when the first job is executed but has not yet committed
		JobExecutionThread executionThread = new JobExecutionThread(this, jobs[0].Id);
		executionThread.startAndWaitUntilControlIsReturned();

		// and the job priority is updated
		JobDefinitionPriorityThread priorityThread = new JobDefinitionPriorityThread(this, jobDefinition.Id, 42L, true);
		priorityThread.startAndWaitUntilControlIsReturned();

		// and the priority threads commits first
		priorityThread.proceedAndWaitTillDone();

		// then both jobs priority has changed
		IList<Job> currentJobs = managementService.createJobQuery().list();
		foreach (Job job in currentJobs)
		{
		  assertEquals(42, job.Priority);
		}

		// and the execution thread can nevertheless successfully finish job execution
		executionThread.proceedAndWaitTillDone();

		assertNull(executionThread.exception);

		// and ultimately only one job with an updated priority is left
		Job remainingJob = managementService.createJobQuery().singleResult();
		assertNotNull(remainingJob);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletingSuspensionJobDuringPriorityUpdate()
	  public virtual void testCompletingSuspensionJobDuringPriorityUpdate()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		// given
		// two running instances (ie two jobs)
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// a job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when suspending the jobs is attempted
		JobSuspensionByJobDefinitionThread suspensionThread = new JobSuspensionByJobDefinitionThread(this, jobDefinition.Id);
		suspensionThread.startAndWaitUntilControlIsReturned();

		// and updating the priority is attempted
		JobDefinitionPriorityThread priorityUpdateThread = new JobDefinitionPriorityThread(this, jobDefinition.Id, 42L, true);
		priorityUpdateThread.startAndWaitUntilControlIsReturned();

		// and both commands overlap each other
		suspensionThread.proceedAndWaitTillDone();
		priorityUpdateThread.proceedAndWaitTillDone();

		// then both updates have been performed
		IList<Job> updatedJobs = managementService.createJobQuery().list();
		assertEquals(2, updatedJobs.Count);
		foreach (Job job in updatedJobs)
		{
		  assertEquals(42, job.Priority);
		  assertTrue(job.Suspended);
		}
	  }


	  public class JobExecutionThread : ControllableThread
	  {
		  private readonly ConcurrentJobExecutorTest outerInstance;


		internal OptimisticLockingException exception;
		internal string jobId;

		internal JobExecutionThread(ConcurrentJobExecutorTest outerInstance, string jobId)
		{
			this.outerInstance = outerInstance;
		  this.jobId = jobId;
		}

		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}

		public override void run()
		{
		  try
		  {
			JobFailureCollector jobFailureCollector = new JobFailureCollector(jobId);
			ExecuteJobHelper.executeJob(jobId, outerInstance.processEngineConfiguration.CommandExecutorTxRequired,jobFailureCollector, new ControlledCommand<Void>(activeThread, new ExecuteJobsCmd(jobId, jobFailureCollector)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

	  public class JobAcquisitionThread : ControllableThread
	  {
		  private readonly ConcurrentJobExecutorTest outerInstance;

		  public JobAcquisitionThread(ConcurrentJobExecutorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		internal OptimisticLockingException exception;
		internal AcquiredJobs acquiredJobs;
		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}
		public override void run()
		{
		  try
		  {
			JobExecutor jobExecutor = outerInstance.processEngineConfiguration.JobExecutor;
			acquiredJobs = outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand<AcquiredJobs>(activeThread, new AcquireJobsCmd(jobExecutor)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

	  public class JobSuspensionThread : ControllableThread
	  {
		  private readonly ConcurrentJobExecutorTest outerInstance;

		internal OptimisticLockingException exception;
		internal string processDefinitionKey;

		public JobSuspensionThread(ConcurrentJobExecutorTest outerInstance, string processDefinitionKey)
		{
			this.outerInstance = outerInstance;
		  this.processDefinitionKey = processDefinitionKey;
		}

		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}
		public override void run()
		{
		  try
		  {
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand<Void>(activeThread, createSuspendJobCommand()));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}

		protected internal virtual Command<Void> createSuspendJobCommand()
		{
		  UpdateJobDefinitionSuspensionStateBuilderImpl builder = (new UpdateJobDefinitionSuspensionStateBuilderImpl()).byProcessDefinitionKey(processDefinitionKey).includeJobs(true);

		  return new SuspendJobDefinitionCmd(builder);
		}
	  }

	  public class JobSuspensionByJobDefinitionThread : ControllableThread
	  {
		  private readonly ConcurrentJobExecutorTest outerInstance;

		internal OptimisticLockingException exception;
		internal string jobDefinitionId;

		public JobSuspensionByJobDefinitionThread(ConcurrentJobExecutorTest outerInstance, string jobDefinitionId)
		{
			this.outerInstance = outerInstance;
		  this.jobDefinitionId = jobDefinitionId;
		}

		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}
		public override void run()
		{
		  try
		  {
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand<Void>(activeThread, createSuspendJobCommand()));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}

		protected internal virtual SuspendJobCmd createSuspendJobCommand()
		{
		  UpdateJobSuspensionStateBuilderImpl builder = (new UpdateJobSuspensionStateBuilderImpl()).byJobDefinitionId(jobDefinitionId);
		  return new SuspendJobCmd(builder);
		}
	  }

	  public class JobDefinitionPriorityThread : ControllableThread
	  {
		  private readonly ConcurrentJobExecutorTest outerInstance;

		internal OptimisticLockingException exception;
		internal string jobDefinitionId;
		internal long? priority;
		internal bool cascade;

		public JobDefinitionPriorityThread(ConcurrentJobExecutorTest outerInstance, string jobDefinitionId, long? priority, bool cascade)
		{
			this.outerInstance = outerInstance;
		  this.jobDefinitionId = jobDefinitionId;
		  this.priority = priority;
		  this.cascade = cascade;
		}

		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}
		public override void run()
		{
		  try
		  {
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand<Void>(activeThread, new SetJobDefinitionPriorityCmd(jobDefinitionId, priority, cascade)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		}
	  }

	}

}