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
namespace org.camunda.bpm.engine.test.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.lessThanOrEqualTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseControlRuleImpl = org.camunda.bpm.engine.impl.cmmn.behavior.CaseControlRuleImpl;
	using FixedValue = org.camunda.bpm.engine.impl.el.FixedValue;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryLevelFull = org.camunda.bpm.engine.impl.history.HistoryLevelFull;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using AbstractProcessEngineTestCase = org.camunda.bpm.engine.impl.test.AbstractProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	using AssertionFailedError = junit.framework.AssertionFailedError;

	public class ProcessEngineTestRule : TestWatcher
	{

	  public const string DEFAULT_BPMN_RESOURCE_NAME = "process.bpmn20.xml";

	  protected internal ProcessEngineRule processEngineRule;
	  protected internal ProcessEngine processEngine;

	  public ProcessEngineTestRule(ProcessEngineRule processEngineRule)
	  {
		this.processEngineRule = processEngineRule;
	  }

	  protected internal override void starting(Description description)
	  {
		this.processEngine = processEngineRule.ProcessEngine;
	  }

	  protected internal override void finished(Description description)
	  {
		this.processEngine = null;
	  }

	  public virtual void assertProcessEnded(string processInstanceId)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		assertThat("Process instance with id " + processInstanceId + " is not finished", processInstance, @is(nullValue()));
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


	  public virtual void assertCaseEnded(string caseInstanceId)
	  {
		CaseInstance caseInstance = processEngine.CaseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		assertThat("Case instance with id " + caseInstanceId + " is not finished", caseInstance, @is(nullValue()));
	  }

	  public virtual DeploymentWithDefinitions deploy(params BpmnModelInstance[] bpmnModelInstances)
	  {
		return deploy(createDeploymentBuilder(), Arrays.asList(bpmnModelInstances), System.Linq.Enumerable.Empty<string> ());
	  }

	  public virtual DeploymentWithDefinitions deploy(params string[] resources)
	  {
		return deploy(createDeploymentBuilder(), System.Linq.Enumerable.Empty<BpmnModelInstance> (), Arrays.asList(resources));
	  }

	  public virtual DeploymentWithDefinitions deploy(DeploymentBuilder deploymentBuilder)
	  {
		DeploymentWithDefinitions deployment = deploymentBuilder.deployWithResult();

		processEngineRule.manageDeployment(deployment);

		return deployment;
	  }

	  public virtual Deployment deploy(BpmnModelInstance bpmnModelInstance, string resource)
	  {
		return deploy(createDeploymentBuilder(), Collections.singletonList(bpmnModelInstance), Collections.singletonList(resource));
	  }

	  public virtual Deployment deployForTenant(string tenantId, params BpmnModelInstance[] bpmnModelInstances)
	  {
		return deploy(createDeploymentBuilder().tenantId(tenantId), Arrays.asList(bpmnModelInstances), System.Linq.Enumerable.Empty<string> ());
	  }

	  public virtual Deployment deployForTenant(string tenantId, params string[] resources)
	  {
		return deploy(createDeploymentBuilder().tenantId(tenantId), System.Linq.Enumerable.Empty<BpmnModelInstance> (), Arrays.asList(resources));
	  }

	  public virtual Deployment deployForTenant(string tenant, BpmnModelInstance bpmnModelInstance, string resource)
	  {
		return deploy(createDeploymentBuilder().tenantId(tenant), Collections.singletonList(bpmnModelInstance), Collections.singletonList(resource));
	  }

	  public virtual ProcessDefinition deployAndGetDefinition(BpmnModelInstance bpmnModel)
	  {
		return deployForTenantAndGetDefinition(null, bpmnModel);
	  }

	  public virtual ProcessDefinition deployForTenantAndGetDefinition(string tenant, BpmnModelInstance bpmnModel)
	  {
		Deployment deployment = deploy(createDeploymentBuilder().tenantId(tenant), Collections.singletonList(bpmnModel), System.Linq.Enumerable.Empty<string>());

		return processEngineRule.RepositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
	  }

	  protected internal virtual DeploymentWithDefinitions deploy(DeploymentBuilder deploymentBuilder, IList<BpmnModelInstance> bpmnModelInstances, IList<string> resources)
	  {
		int i = 0;
		foreach (BpmnModelInstance bpmnModelInstance in bpmnModelInstances)
		{
		  deploymentBuilder.addModelInstance(i + "_" + DEFAULT_BPMN_RESOURCE_NAME, bpmnModelInstance);
		  i++;
		}

		foreach (string resource in resources)
		{
		  deploymentBuilder.addClasspathResource(resource);
		}

		return deploy(deploymentBuilder);
	  }

	  protected internal virtual DeploymentBuilder createDeploymentBuilder()
	  {
		return processEngine.RepositoryService.createDeployment();
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs()
	  {
		waitForJobExecutorToProcessAllJobs(0);
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
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
			throw new AssertionError("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
		  jobExecutor.shutdown();
		}
	  }

	  protected internal virtual bool areJobsAvailable()
	  {
		IList<Job> list = processEngine.ManagementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  if (!job.Suspended && job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime > job.Duedate))
		  {
			return true;
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Execute all available jobs recursively till no more jobs found.
	  /// </summary>
	  public virtual void executeAvailableJobs()
	  {
		executeAvailableJobs(0, int.MaxValue);
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
		executeAvailableJobs(0, expectedExecutions);
	  }

	  private void executeAvailableJobs(int jobsExecuted, int expectedExecutions)
	  {
		IList<Job> jobs = processEngine.ManagementService.createJobQuery().withRetriesLeft().list();

		if (jobs.Count == 0)
		{
		  if (expectedExecutions != int.MaxValue)
		  {
			assertThat("executed less jobs than expected.", jobsExecuted, @is(expectedExecutions));
		  }
		  return;
		}

		foreach (Job job in jobs)
		{
		  try
		  {
			processEngine.ManagementService.executeJob(job.Id);
			jobsExecuted += 1;
		  }
		  catch (Exception)
		  {
		  }
		}

		assertThat("executed more jobs than expected.", jobsExecuted, lessThanOrEqualTo(expectedExecutions));

		executeAvailableJobs(jobsExecuted, expectedExecutions);
	  }

	  public virtual void completeTask(string taskKey)
	  {
		TaskService taskService = processEngine.TaskService;
		Task task = taskService.createTaskQuery().taskDefinitionKey(taskKey).singleResult();
		assertNotNull("Expected a task with key '" + taskKey + "' to exist", task);
		taskService.complete(task.Id);
	  }

	  public virtual void completeAnyTask(string taskKey)
	  {
		TaskService taskService = processEngine.TaskService;
		IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey(taskKey).list();
		assertTrue(tasks.Count > 0);
		taskService.complete(tasks[0].Id);
	  }

	  public virtual string AnyVariable
	  {
		  set
		  {
			setVariable(value, "any", "any");
		  }
	  }

	  public virtual void setVariable(string executionId, string varName, object varValue)
	  {
		processEngine.RuntimeService.setVariable(executionId, varName, varValue);
	  }

	  public virtual void correlateMessage(string messageName)
	  {
		processEngine.RuntimeService.createMessageCorrelation(messageName).correlate();
	  }

	  public virtual void sendSignal(string signalName)
	  {
		processEngine.RuntimeService.signalEventReceived(signalName);
	  }

	  public virtual bool HistoryLevelNone
	  {
		  get
		  {
			HistoryLevel historyLevel = processEngineRule.ProcessEngineConfiguration.HistoryLevel;
			return org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE.Equals(historyLevel);
		  }
	  }

	  public virtual bool HistoryLevelActivity
	  {
		  get
		  {
			HistoryLevel historyLevel = processEngineRule.ProcessEngineConfiguration.HistoryLevel;
			return org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Equals(historyLevel);
		  }
	  }

	  public virtual bool HistoryLevelAudit
	  {
		  get
		  {
			HistoryLevel historyLevel = processEngineRule.ProcessEngineConfiguration.HistoryLevel;
			return org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Equals(historyLevel);
		  }
	  }

	  public virtual bool HistoryLevelFull
	  {
		  get
		  {
			HistoryLevel historyLevel = processEngineRule.ProcessEngineConfiguration.HistoryLevel;
			return org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL.Equals(historyLevel);
		  }
	  }

	  /// <summary>
	  /// Asserts if the provided text is part of some text.
	  /// </summary>
	  public virtual void assertTextPresent(string expected, string actual)
	  {
		if ((string.ReferenceEquals(actual, null)) || (actual.IndexOf(expected, StringComparison.Ordinal) == -1))
		{
		  throw new AssertionFailedError("expected presence of [" + expected + "], but was [" + actual + "]");
		}
	  }

	  /// <summary>
	  /// Asserts if the provided text is part of some text, ignoring any uppercase characters
	  /// </summary>
	  public virtual void assertTextPresentIgnoreCase(string expected, string actual)
	  {
		assertTextPresent(expected.ToLower(), actual.ToLower());
	  }

	  public virtual object defaultManualActivation()
	  {
		Expression expression = new FixedValue(true);
		CaseControlRuleImpl caseControlRule = new CaseControlRuleImpl(expression);
		return caseControlRule;
	  }

	  protected internal class InterruptTask : TimerTask
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

	}

}