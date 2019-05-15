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
namespace org.camunda.bpm.engine.impl.test
{

	using LogFactory = org.apache.ibatis.logging.LogFactory;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Logger = org.slf4j.Logger;

	using AssertionFailedError = junit.framework.AssertionFailedError;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class AbstractProcessEngineTestCase : PvmTestCase
	{

	  private static readonly Logger LOG = TestLogger.TEST_LOGGER.Logger;

	  static AbstractProcessEngineTestCase()
	  {
		// this ensures that mybatis uses slf4j logging
		LogFactory.useSlf4jLogging();
	  }

	  protected internal ProcessEngine processEngine;

	  protected internal string deploymentId;
	  protected internal ISet<string> deploymentIds = new HashSet<string>();

	  protected internal Exception exception;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal FormService formService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;
	  protected internal AuthorizationService authorizationService;
	  protected internal CaseService caseService;
	  protected internal FilterService filterService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal DecisionService decisionService;

	  protected internal abstract void initializeProcessEngine();

	  // Default: do nothing
	  protected internal virtual void closeDownProcessEngine()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void runBare() throws Throwable
	  public override void runBare()
	  {
		initializeProcessEngine();
		if (repositoryService == null)
		{
		  initializeServices();
		}

		try
		{

		  bool hasRequiredHistoryLevel = TestHelper.annotationRequiredHistoryLevelCheck(processEngine, this.GetType(), Name);
		  // ignore test case when current history level is too low
		  if (hasRequiredHistoryLevel)
		  {

			deploymentId = TestHelper.annotationDeploymentSetUp(processEngine, this.GetType(), Name);

			base.runBare();
		  }

		}
		catch (AssertionFailedError e)
		{
		  LOG.error("ASSERTION FAILED: " + e, e);
		  exception = e;
		  throw e;

		}
		catch (Exception e)
		{
		  LOG.error("EXCEPTION: " + e, e);
		  exception = e;
		  throw e;

		}
		finally
		{

		  identityService.clearAuthentication();
		  processEngineConfiguration.TenantCheckEnabled = true;

		  deleteDeployments();

		  deleteHistoryCleanupJobs();

		  // only fail if no test failure was recorded
		  TestHelper.assertAndEnsureCleanDbAndCache(processEngine, exception == null);
		  TestHelper.resetIdGenerator(processEngineConfiguration);
		  ClockUtil.reset();

		  // Can't do this in the teardown, as the teardown will be called as part
		  // of the super.runBare
		  closeDownProcessEngine();
		  clearServiceReferences();
		}
	  }

	  protected internal virtual void deleteHistoryCleanupJobs()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> jobs = historyService.findHistoryCleanupJobs();
		IList<Job> jobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in jobs)
		{
		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		}
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly AbstractProcessEngineTestCase outerInstance;

		  public CommandAnonymousInnerClass(AbstractProcessEngineTestCase outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			  commandContext.JobManager.deleteJob((JobEntity) job);
			return null;
		  }
	  }

	  protected internal virtual void deleteDeployments()
	  {
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  deploymentIds.Add(deploymentId);
		}

		foreach (string deploymentId in deploymentIds)
		{
		  TestHelper.annotationDeploymentTearDown(processEngine, deploymentId, this.GetType(), Name);
		}

		deploymentId = null;
		deploymentIds.Clear();
	  }

	  protected internal virtual void initializeServices()
	  {
		processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		formService = processEngine.FormService;
		historyService = processEngine.HistoryService;
		identityService = processEngine.IdentityService;
		managementService = processEngine.ManagementService;
		authorizationService = processEngine.AuthorizationService;
		caseService = processEngine.CaseService;
		filterService = processEngine.FilterService;
		externalTaskService = processEngine.ExternalTaskService;
		decisionService = processEngine.DecisionService;
	  }

	  protected internal virtual void clearServiceReferences()
	  {
		processEngineConfiguration = null;
		repositoryService = null;
		runtimeService = null;
		taskService = null;
		formService = null;
		historyService = null;
		identityService = null;
		managementService = null;
		authorizationService = null;
		caseService = null;
		filterService = null;
		externalTaskService = null;
		decisionService = null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void assertProcessEnded(final String processInstanceId)
	  public virtual void assertProcessEnded(string processInstanceId)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		if (processInstance != null)
		{
		  throw new AssertionFailedError("Expected finished process instance '" + processInstanceId + "' but it was still in the db");
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void assertProcessNotEnded(final String processInstanceId)
	  public virtual void assertProcessNotEnded(string processInstanceId)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		if (processInstance == null)
		{
		  throw new AssertionFailedError("Expected process instance '" + processInstanceId + "' to be still active but it was not in the db");
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void assertCaseEnded(final String caseInstanceId)
	  public virtual void assertCaseEnded(string caseInstanceId)
	  {
		CaseInstance caseInstance = processEngine.CaseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		if (caseInstance != null)
		{
		  throw new AssertionFailedError("Expected finished case instance '" + caseInstanceId + "' but it was still in the db");
		}
	  }

	  [Obsolete]
	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait, long intervalMillis)
	  {
		waitForJobExecutorToProcessAllJobs(maxMillisToWait);
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait)
	  {
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor.start();
		long intervalMillis = 1000;

		int jobExecutorWaitTime = jobExecutor.WaitTimeInMillis * 2;
		if (maxMillisToWait < jobExecutorWaitTime)
		{
		  maxMillisToWait = jobExecutorWaitTime;
		}

		try
		{
		  Timer timer = new Timer();
		  InterruptTask task = new InterruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool areJobsAvailable = true;
		  try
		  {
			while (areJobsAvailable && !task.TimeLimitExceeded)
			{
			  Thread.Sleep(intervalMillis);
			  try
			  {
				areJobsAvailable = areJobsAvailable();
			  }
			  catch (Exception)
			  {
				// Ignore, possible that exception occurs due to locking/updating of table on MSSQL when
				// isolation level doesn't allow READ of the table
			  }
			}
		  }
		  catch (InterruptedException)
		  {
		  }
		  finally
		  {
			timer.cancel();
		  }
		  if (areJobsAvailable)
		  {
			throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
		  jobExecutor.shutdown();
		}
	  }

	  [Obsolete]
	  public virtual void waitForJobExecutorOnCondition(long maxMillisToWait, long intervalMillis, Callable<bool> condition)
	  {
		waitForJobExecutorOnCondition(maxMillisToWait, condition);
	  }

	  public virtual void waitForJobExecutorOnCondition(long maxMillisToWait, Callable<bool> condition)
	  {
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor.start();
		long intervalMillis = 500;

		if (maxMillisToWait < (jobExecutor.WaitTimeInMillis * 2))
		{
		  maxMillisToWait = (jobExecutor.WaitTimeInMillis * 2);
		}

		try
		{
		  Timer timer = new Timer();
		  InterruptTask task = new InterruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool conditionIsViolated = true;
		  try
		  {
			while (conditionIsViolated && !task.TimeLimitExceeded)
			{
			  Thread.Sleep(intervalMillis);
			  conditionIsViolated = !condition.call();
			}
		  }
		  catch (InterruptedException)
		  {
		  }
		  catch (Exception e)
		  {
			throw new ProcessEngineException("Exception while waiting on condition: " + e.Message, e);
		  }
		  finally
		  {
			timer.cancel();
		  }
		  if (conditionIsViolated)
		  {
			throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
		  jobExecutor.shutdown();
		}
	  }

	  /// <summary>
	  /// Execute all available jobs recursively till no more jobs found.
	  /// </summary>
	  public virtual void executeAvailableJobs()
	  {
		executeAvailableJobs(0, int.MaxValue, true);
	  }

	  /// <summary>
	  /// Execute all available jobs recursively till no more jobs found or the number of executions is higher than expected.
	  /// </summary>
	  /// <param name="expectedExecutions"> number of expected job executions
	  /// </param>
	  /// <exception cref="AssertionFailedError"> when execute less or more jobs than expected
	  /// </exception>
	  /// <seealso cref= #executeAvailableJobs() </seealso>
	  public virtual void executeAvailableJobs(int expectedExecutions)
	  {
		executeAvailableJobs(0, expectedExecutions, false);
	  }

	  private void executeAvailableJobs(int jobsExecuted, int expectedExecutions, bool ignoreLessExecutions)
	  {
		IList<Job> jobs = managementService.createJobQuery().withRetriesLeft().list();

		if (jobs.Count == 0)
		{
		  assertTrue("executed less jobs than expected. expected <" + expectedExecutions + "> actual <" + jobsExecuted + ">", jobsExecuted == expectedExecutions || ignoreLessExecutions);
		  return;
		}

		foreach (Job job in jobs)
		{
		  try
		  {
			managementService.executeJob(job.Id);
			jobsExecuted += 1;
		  }
		  catch (Exception)
		  {
		  }
		}

		assertTrue("executed more jobs than expected. expected <" + expectedExecutions + "> actual <" + jobsExecuted + ">", jobsExecuted <= expectedExecutions);

		executeAvailableJobs(jobsExecuted, expectedExecutions, ignoreLessExecutions);
	  }

	  public virtual bool areJobsAvailable()
	  {
		IList<Job> list = managementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  if (!job.Suspended && job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime > job.Duedate))
		  {
			return true;
		  }
		}
		return false;
	  }

	  private class InterruptTask : TimerTask
	  {
		protected internal bool timeLimitExceeded = false;
		protected internal Thread thread;
		public InterruptTask(Thread thread)
		{
		  this.thread = thread;
		}
		public virtual bool TimeLimitExceeded
		{
			get
			{
			  return timeLimitExceeded;
			}
		}
		public override void run()
		{
		  timeLimitExceeded = true;
		  thread.Interrupt();
		}
	  }

	  [Obsolete]
	  protected internal virtual IList<ActivityInstance> getInstancesForActivitiyId(ActivityInstance activityInstance, string activityId)
	  {
		return getInstancesForActivityId(activityInstance, activityId);
	  }

	  protected internal virtual IList<ActivityInstance> getInstancesForActivityId(ActivityInstance activityInstance, string activityId)
	  {
		IList<ActivityInstance> result = new List<ActivityInstance>();
		if (activityInstance.ActivityId.Equals(activityId))
		{
		  result.Add(activityInstance);
		}
		foreach (ActivityInstance childInstance in activityInstance.ChildActivityInstances)
		{
		  ((IList<ActivityInstance>)result).AddRange(getInstancesForActivityId(childInstance, activityId));
		}
		return result;
	  }

	  protected internal virtual void runAsUser(string userId, IList<string> groupIds, ThreadStart r)
	  {
		try
		{
		  identityService.AuthenticatedUserId = userId;
		  processEngineConfiguration.AuthorizationEnabled = true;

		  r.run();

		}
		finally
		{
		  identityService.AuthenticatedUserId = null;
		  processEngineConfiguration.AuthorizationEnabled = false;
		}
	  }

	  protected internal virtual string deployment(params BpmnModelInstance[] bpmnModelInstances)
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment();

		return deployment(deploymentBuilder, bpmnModelInstances);
	  }

	  protected internal virtual string deployment(params string[] resources)
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment();

		return deployment(deploymentBuilder, resources);
	  }

	  protected internal virtual string deploymentForTenant(string tenantId, params BpmnModelInstance[] bpmnModelInstances)
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().tenantId(tenantId);

		return deployment(deploymentBuilder, bpmnModelInstances);
	  }

	  protected internal virtual string deploymentForTenant(string tenantId, params string[] resources)
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().tenantId(tenantId);

		return deployment(deploymentBuilder, resources);
	  }

	  protected internal virtual string deploymentForTenant(string tenantId, string classpathResource, BpmnModelInstance modelInstance)
	  {
		return deployment(repositoryService.createDeployment().tenantId(tenantId).addClasspathResource(classpathResource), modelInstance);
	  }

	  protected internal virtual string deployment(DeploymentBuilder deploymentBuilder, params BpmnModelInstance[] bpmnModelInstances)
	  {
		for (int i = 0; i < bpmnModelInstances.Length; i++)
		{
		  BpmnModelInstance bpmnModelInstance = bpmnModelInstances[i];
		  deploymentBuilder.addModelInstance("testProcess-" + i + ".bpmn", bpmnModelInstance);
		}

		return deploymentWithBuilder(deploymentBuilder);
	  }

	  protected internal virtual string deployment(DeploymentBuilder deploymentBuilder, params string[] resources)
	  {
		for (int i = 0; i < resources.Length; i++)
		{
		  deploymentBuilder.addClasspathResource(resources[i]);
		}

		return deploymentWithBuilder(deploymentBuilder);
	  }

	  protected internal virtual string deploymentWithBuilder(DeploymentBuilder builder)
	  {
		deploymentId = builder.deploy().Id;
		deploymentIds.Add(deploymentId);

		return deploymentId;
	  }

	}

}