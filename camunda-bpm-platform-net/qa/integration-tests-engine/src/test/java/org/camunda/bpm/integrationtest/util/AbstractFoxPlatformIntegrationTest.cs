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
namespace org.camunda.bpm.integrationtest.util
{
	using org.camunda.bpm.engine;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Before = org.junit.Before;



	public abstract class AbstractFoxPlatformIntegrationTest
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal Logger logger = Logger.getLogger(typeof(AbstractFoxPlatformIntegrationTest).FullName);

	  protected internal ProcessEngineService processEngineService;
	//  protected ProcessArchiveService processArchiveService;
	  protected internal ProcessEngine processEngine;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal FormService formService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal CaseService caseService;
	  protected internal DecisionService decisionService;

	  public static WebArchive initWebArchiveDeployment(string name, string processesXmlPath)
	  {
		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), name).addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource(processesXmlPath, "META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestConstants));

		TestContainer.addContainerSpecificResources(archive);

		return archive;
	  }

	  public static WebArchive initWebArchiveDeployment(string name)
	  {
		return initWebArchiveDeployment(name, "META-INF/processes.xml");
	  }

	  public static WebArchive initWebArchiveDeployment()
	  {
		return initWebArchiveDeployment("test.war");
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupBeforeTest()
	  public virtual void setupBeforeTest()
	  {
		processEngineService = BpmPlatform.ProcessEngineService;
		processEngine = processEngineService.DefaultProcessEngine;
		processEngineConfiguration = ((ProcessEngineImpl)processEngine).ProcessEngineConfiguration;
		processEngineConfiguration.JobExecutor.shutdown(); // make sure the job executor is down
		formService = processEngine.FormService;
		historyService = processEngine.HistoryService;
		identityService = processEngine.IdentityService;
		managementService = processEngine.ManagementService;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		caseService = processEngine.CaseService;
		decisionService = processEngine.DecisionService;
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs()
	  {
		waitForJobExecutorToProcessAllJobs(12000);
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait)
	  {

		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		waitForJobExecutorToProcessAllJobs(jobExecutor, maxMillisToWait);
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs(JobExecutor jobExecutor, long maxMillisToWait)
	  {

		int checkInterval = 1000;

		jobExecutor.start();

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
			  Thread.Sleep(checkInterval);
			  areJobsAvailable = areJobsAvailable();
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
			throw new Exception("time limit of " + maxMillisToWait + " was exceeded (still " + numberOfJobsAvailable() + " jobs available)");
		  }

		}
		finally
		{
		  jobExecutor.shutdown();
		}
	  }

	  public virtual bool areJobsAvailable()
	  {
		IList<Job> list = managementService.createJobQuery().list();
		foreach (Job job in list)
		{
		  if (isJobAvailable(job))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public virtual bool isJobAvailable(Job job)
	  {
		return job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime > job.Duedate);
	  }

	  public virtual int numberOfJobsAvailable()
	  {
		int numberOfJobs = 0;
		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  if (isJobAvailable(job))
		  {
			numberOfJobs++;
		  }
		}
		return numberOfJobs;
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
		public virtual void run()
		{
		  timeLimitExceeded = true;
		  thread.Interrupt();
		}
	  }

	}

}