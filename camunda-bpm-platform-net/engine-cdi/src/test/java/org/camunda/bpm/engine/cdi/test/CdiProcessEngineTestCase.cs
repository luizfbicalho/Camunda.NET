using System;
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
namespace org.camunda.bpm.engine.cdi.test
{

	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using LogUtil = org.camunda.bpm.engine.impl.util.LogUtil;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public abstract class CdiProcessEngineTestCase
	public abstract class CdiProcessEngineTestCase
	{

	  static CdiProcessEngineTestCase()
	  {
		LogUtil.readJavaUtilLoggingConfigFromClasspath();
	  }

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal Logger logger = Logger.getLogger(this.GetType().FullName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.JavaArchive createDeployment()
	  public static JavaArchive createDeployment()
	  {

		return ShrinkWrap.create(typeof(JavaArchive)).addPackages(true, "org.camunda.bpm.engine.cdi").addAsManifestResource("META-INF/beans.xml", "beans.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule processEngineRule = new org.camunda.bpm.engine.test.ProcessEngineRule();
	  public ProcessEngineRule processEngineRule = new ProcessEngineRule();

	  protected internal BeanManager beanManager;

	  protected internal ProcessEngine processEngine;
	  protected internal FormService formService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal AuthorizationService authorizationService;
	  protected internal FilterService filterService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal CaseService caseService;
	  protected internal DecisionService decisionService;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpCdiProcessEngineTestCase() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUpCdiProcessEngineTestCase()
	  {

		if (BpmPlatform.ProcessEngineService.DefaultProcessEngine == null)
		{
		  org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().registerProcessEngine(processEngineRule.ProcessEngine);
		}

		beanManager = ProgrammaticBeanLookup.lookup(typeof(BeanManager));
		processEngine = processEngineRule.ProcessEngine;
		processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngineRule.ProcessEngine.ProcessEngineConfiguration;
		formService = processEngine.FormService;
		historyService = processEngine.HistoryService;
		identityService = processEngine.IdentityService;
		managementService = processEngine.ManagementService;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		authorizationService = processEngine.AuthorizationService;
		filterService = processEngine.FilterService;
		externalTaskService = processEngine.ExternalTaskService;
		caseService = processEngine.CaseService;
		decisionService = processEngine.DecisionService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDownCdiProcessEngineTestCase() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDownCdiProcessEngineTestCase()
	  {
		org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().unregisterProcessEngine(processEngine);
		beanManager = null;
		processEngine = null;
		processEngineConfiguration = null;
		formService = null;
		historyService = null;
		identityService = null;
		managementService = null;
		repositoryService = null;
		runtimeService = null;
		taskService = null;
		authorizationService = null;
		filterService = null;
		externalTaskService = null;
		caseService = null;
		decisionService = null;
		processEngineRule = null;
	  }

	  protected internal virtual void endConversationAndBeginNew(string processInstanceId)
	  {
		getBeanInstance(typeof(BusinessProcess)).associateExecutionById(processInstanceId);
	  }

	  protected internal virtual T getBeanInstance<T>(Type<T> clazz)
	  {
		return ProgrammaticBeanLookup.lookup(clazz);
	  }

	  protected internal virtual object getBeanInstance(string name)
	  {
		return ProgrammaticBeanLookup.lookup(name);
	  }

	  //////////////////////// copied from AbstractActivitiTestcase

	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait, long intervalMillis)
	  {
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor.start();

		try
		{
		  Timer timer = new Timer();
		  InteruptTask task = new InteruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool areJobsAvailable = true;
		  try
		  {
			while (areJobsAvailable && !task.TimeLimitExceeded)
			{
			  Thread.Sleep(intervalMillis);
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
			throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
		  jobExecutor.shutdown();
		}
	  }

	  public virtual void waitForJobExecutorOnCondition(long maxMillisToWait, long intervalMillis, Callable<bool> condition)
	  {
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor.start();

		try
		{
		  Timer timer = new Timer();
		  InteruptTask task = new InteruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool conditionIsViolated = true;
		  try
		  {
			while (conditionIsViolated)
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

	  public virtual bool areJobsAvailable()
	  {
		return !managementService.createJobQuery().executable().list().Empty;
	  }

	  private class InteruptTask : TimerTask
	  {
		protected internal bool timeLimitExceeded = false;
		protected internal Thread thread;
		public InteruptTask(Thread thread)
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