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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using Page = org.camunda.bpm.engine.impl.Page;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using DeleteJobsCmd = org.camunda.bpm.engine.impl.cmd.DeleteJobsCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Assert = org.junit.Assert;

	public class DeploymentAwareJobExecutorTest : PluggableProcessEngineTestCase
	{

	  protected internal ProcessEngine otherProcessEngine = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		base.setUp();
		processEngineConfiguration.JobExecutorDeploymentAware = true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		processEngineConfiguration.JobExecutorDeploymentAware = false;
		base.tearDown();
	  }

	  protected internal override void closeDownProcessEngine()
	  {
		base.closeDownProcessEngine();
		if (otherProcessEngine != null)
		{
		  otherProcessEngine.close();
		  ProcessEngines.unregister(otherProcessEngine);
		  otherProcessEngine = null;
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testProcessingOfJobsWithMatchingDeployment()
	  {

		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		ISet<string> registeredDeployments = managementService.RegisteredDeployments;
		Assert.assertEquals(1, registeredDeployments.Count);
		Assert.assertTrue(registeredDeployments.Contains(deploymentId));

		Job executableJob = managementService.createJobQuery().singleResult();

		string otherDeploymentId = deployAndInstantiateWithNewEngineConfiguration("org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcessVersion2.bpmn20.xml");

		// assert that two jobs have been created, one for each deployment
		IList<Job> jobs = managementService.createJobQuery().list();
		Assert.assertEquals(2, jobs.Count);
		ISet<string> jobDeploymentIds = new HashSet<string>();
		jobDeploymentIds.Add(jobs[0].DeploymentId);
		jobDeploymentIds.Add(jobs[1].DeploymentId);

		Assert.assertTrue(jobDeploymentIds.Contains(deploymentId));
		Assert.assertTrue(jobDeploymentIds.Contains(otherDeploymentId));

		// select executable jobs for executor of first engine
		AcquiredJobs acquiredJobs = getExecutableJobs(processEngineConfiguration.JobExecutor);
		Assert.assertEquals(1, acquiredJobs.size());
		Assert.assertTrue(acquiredJobs.contains(executableJob.Id));

		repositoryService.deleteDeployment(otherDeploymentId, true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testExplicitDeploymentRegistration()
	  {
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		string otherDeploymentId = deployAndInstantiateWithNewEngineConfiguration("org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcessVersion2.bpmn20.xml");

		processEngine.ManagementService.registerDeploymentForJobExecutor(otherDeploymentId);

		IList<Job> jobs = managementService.createJobQuery().list();

		AcquiredJobs acquiredJobs = getExecutableJobs(processEngineConfiguration.JobExecutor);
		Assert.assertEquals(2, acquiredJobs.size());
		foreach (Job job in jobs)
		{
		  Assert.assertTrue(acquiredJobs.contains(job.Id));
		}

		repositoryService.deleteDeployment(otherDeploymentId, true);
	  }

	  public virtual void testRegistrationOfNonExistingDeployment()
	  {
		string nonExistingDeploymentId = "some non-existing id";

		try
		{
		  processEngine.ManagementService.registerDeploymentForJobExecutor(nonExistingDeploymentId);
		  Assert.fail("Registering a non-existing deployment should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Deployment " + nonExistingDeploymentId + " does not exist", e.Message);
		  // happy path
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testDeploymentUnregistrationOnUndeployment()
	  {
		Assert.assertEquals(1, managementService.RegisteredDeployments.Count);

		repositoryService.deleteDeployment(deploymentId, true);

		Assert.assertEquals(0, managementService.RegisteredDeployments.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testNoUnregistrationOnFailingUndeployment()
	  {
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		try
		{
		  repositoryService.deleteDeployment(deploymentId, false);
		  Assert.fail();
		}
		catch (Exception)
		{
		  // should still be registered, if not successfully undeployed
		  Assert.assertEquals(1, managementService.RegisteredDeployments.Count);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testExplicitDeploymentUnregistration()
	  {
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		processEngine.ManagementService.unregisterDeploymentForJobExecutor(deploymentId);

		AcquiredJobs acquiredJobs = getExecutableJobs(processEngineConfiguration.JobExecutor);
		Assert.assertEquals(0, acquiredJobs.size());
	  }

	  public virtual void testJobsWithoutDeploymentIdAreAlwaysProcessed()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;

		string messageId = commandExecutor.execute(new CommandAnonymousInnerClass(this));

		AcquiredJobs acquiredJobs = getExecutableJobs(processEngineConfiguration.JobExecutor);
		Assert.assertEquals(1, acquiredJobs.size());
		Assert.assertTrue(acquiredJobs.contains(messageId));

		commandExecutor.execute(new DeleteJobsCmd(messageId, true));
	  }

	  private class CommandAnonymousInnerClass : Command<string>
	  {
		  private readonly DeploymentAwareJobExecutorTest outerInstance;

		  public CommandAnonymousInnerClass(DeploymentAwareJobExecutorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public string execute(CommandContext commandContext)
		  {
			MessageEntity message = new MessageEntity();
			commandContext.JobManager.send(message);
			return message.Id;
		  }
	  }

	  private AcquiredJobs getExecutableJobs(JobExecutor jobExecutor)
	  {
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new AcquireJobsCmd(jobExecutor));
	  }

	  private string deployAndInstantiateWithNewEngineConfiguration(string resource)
	  {
		// 1. create another process engine
		try
		{
		  otherProcessEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml").buildProcessEngine();
		}
		catch (Exception ex)
		{
		  if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
		  {
			otherProcessEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("activiti.cfg.xml").buildProcessEngine();
		  }
		  else
		  {
			throw ex;
		  }
		}

		// 2. deploy again
		RepositoryService otherRepositoryService = otherProcessEngine.RepositoryService;

		string deploymentId = otherRepositoryService.createDeployment().addClasspathResource(resource).deploy().Id;

		// 3. start instance (i.e. create job)
		ProcessDefinition newDefinition = otherRepositoryService.createProcessDefinitionQuery().deploymentId(deploymentId).singleResult();
		otherProcessEngine.RuntimeService.startProcessInstanceById(newDefinition.Id);

		return deploymentId;
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/jobexecutor/processWithTimerCatch.bpmn20.xml")]
	  public virtual void testIntermediateTimerEvent()
	  {


		runtimeService.startProcessInstanceByKey("testProcess");

		ISet<string> registeredDeployments = processEngineConfiguration.RegisteredDeployments;


		Job existingJob = managementService.createJobQuery().singleResult();

		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis() + 61 * 1000);

		IList<JobEntity> acquirableJobs = findAcquirableJobs();

		assertEquals(1, acquirableJobs.Count);
		assertEquals(existingJob.Id, acquirableJobs[0].Id);

		registeredDeployments.Clear();

		acquirableJobs = findAcquirableJobs();

		assertEquals(0, acquirableJobs.Count);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/jobexecutor/processWithTimerStart.bpmn20.xml")]
	  public virtual void testTimerStartEvent()
	  {

		ISet<string> registeredDeployments = processEngineConfiguration.RegisteredDeployments;

		Job existingJob = managementService.createJobQuery().singleResult();

		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis() + 1000);

		IList<JobEntity> acquirableJobs = findAcquirableJobs();

		assertEquals(1, acquirableJobs.Count);
		assertEquals(existingJob.Id, acquirableJobs[0].Id);

		registeredDeployments.Clear();

		acquirableJobs = findAcquirableJobs();

		assertEquals(0, acquirableJobs.Count);
	  }

	  protected internal virtual IList<JobEntity> findAcquirableJobs()
	  {
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));
	  }

	  private class CommandAnonymousInnerClass2 : Command<IList<JobEntity>>
	  {
		  private readonly DeploymentAwareJobExecutorTest outerInstance;

		  public CommandAnonymousInnerClass2(DeploymentAwareJobExecutorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public IList<JobEntity> execute(CommandContext commandContext)
		  {
			return commandContext.JobManager.findNextJobsToExecute(new Page(0, 100));
		  }
	  }

	}

}