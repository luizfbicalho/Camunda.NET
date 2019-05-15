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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using DefaultJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobExecutor;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using SequentialJobAcquisitionRunnable = org.camunda.bpm.engine.impl.jobexecutor.SequentialJobAcquisitionRunnable;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SequentialJobAcquisitionTest
	{

	  private static readonly string RESOURCE_BASE = typeof(SequentialJobAcquisitionTest).Assembly.GetName().Name.replace(".", "/");
	  private static readonly string PROCESS_RESOURCE = RESOURCE_BASE + "/IntermediateTimerEventTest.testCatchingTimerEvent.bpmn20.xml";

	  private JobExecutor jobExecutor = new DefaultJobExecutor();
	  private IList<ProcessEngine> createdProcessEngines = new List<ProcessEngine>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void stopJobExecutor()
	  public virtual void stopJobExecutor()
	  {
		jobExecutor.shutdown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void closeProcessEngines()
	  public virtual void closeProcessEngines()
	  {
		IEnumerator<ProcessEngine> iterator = createdProcessEngines.GetEnumerator();
		while (iterator.MoveNext())
		{
		  ProcessEngine processEngine = iterator.Current;
		  processEngine.close();
		  ProcessEngines.unregister(processEngine);
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
		  iterator.remove();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobsForSingleEngine()
	  public virtual void testExecuteJobsForSingleEngine()
	  {
		// configure and build a process engine
		StandaloneProcessEngineConfiguration standaloneProcessEngineConfiguration = new StandaloneInMemProcessEngineConfiguration();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		standaloneProcessEngineConfiguration.ProcessEngineName = this.GetType().FullName + "-engine1";
		standaloneProcessEngineConfiguration.JdbcUrl = "jdbc:h2:mem:jobexecutor-test-engine";
		standaloneProcessEngineConfiguration.JobExecutorActivate = false;
		standaloneProcessEngineConfiguration.JobExecutor = jobExecutor;
		standaloneProcessEngineConfiguration.DbMetricsReporterActivate = false;
		ProcessEngine engine = standaloneProcessEngineConfiguration.buildProcessEngine();

		createdProcessEngines.Add(engine);

		engine.RepositoryService.createDeployment().addClasspathResource(PROCESS_RESOURCE).deploy();

		jobExecutor.shutdown();

		engine.RuntimeService.startProcessInstanceByKey("intermediateTimerEventExample");

		Assert.assertEquals(1, engine.ManagementService.createJobQuery().count());

		DateTime calendar = new DateTime();
		calendar.add(Field.DAY_OF_YEAR.CalendarField, 6);
		ClockUtil.CurrentTime = calendar;
		jobExecutor.start();
		waitForJobExecutorToProcessAllJobs(10000, 100, jobExecutor, engine.ManagementService, true);

		Assert.assertEquals(0, engine.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobsForTwoEnginesSameAcquisition()
	  public virtual void testExecuteJobsForTwoEnginesSameAcquisition()
	  {
		// configure and build a process engine
		StandaloneProcessEngineConfiguration engineConfiguration1 = new StandaloneInMemProcessEngineConfiguration();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		engineConfiguration1.ProcessEngineName = this.GetType().FullName + "-engine1";
		engineConfiguration1.JdbcUrl = "jdbc:h2:mem:activiti1";
		engineConfiguration1.JobExecutorActivate = false;
		engineConfiguration1.JobExecutor = jobExecutor;
		engineConfiguration1.DbMetricsReporterActivate = false;
		ProcessEngine engine1 = engineConfiguration1.buildProcessEngine();
		createdProcessEngines.Add(engine1);

		// and a second one
		StandaloneProcessEngineConfiguration engineConfiguration2 = new StandaloneInMemProcessEngineConfiguration();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		engineConfiguration2.ProcessEngineName = this.GetType().FullName + "engine2";
		engineConfiguration2.JdbcUrl = "jdbc:h2:mem:activiti2";
		engineConfiguration2.JobExecutorActivate = false;
		engineConfiguration2.JobExecutor = jobExecutor;
		engineConfiguration2.DbMetricsReporterActivate = false;
		ProcessEngine engine2 = engineConfiguration2.buildProcessEngine();
		createdProcessEngines.Add(engine2);

		// stop the acquisition
		jobExecutor.shutdown();

		// deploy the processes

		engine1.RepositoryService.createDeployment().addClasspathResource(PROCESS_RESOURCE).deploy();

		engine2.RepositoryService.createDeployment().addClasspathResource(PROCESS_RESOURCE).deploy();

		// start one instance for each engine:

		engine1.RuntimeService.startProcessInstanceByKey("intermediateTimerEventExample");
		engine2.RuntimeService.startProcessInstanceByKey("intermediateTimerEventExample");

		Assert.assertEquals(1, engine1.ManagementService.createJobQuery().count());
		Assert.assertEquals(1, engine2.ManagementService.createJobQuery().count());

		DateTime calendar = new DateTime();
		calendar.add(Field.DAY_OF_YEAR.CalendarField, 6);
		ClockUtil.CurrentTime = calendar;

		jobExecutor.start();
		// assert task completed for the first engine
		waitForJobExecutorToProcessAllJobs(10000, 100, jobExecutor, engine1.ManagementService, true);

		jobExecutor.start();
		// assert task completed for the second engine
		waitForJobExecutorToProcessAllJobs(10000, 100, jobExecutor, engine2.ManagementService, true);

		Assert.assertEquals(0, engine1.ManagementService.createJobQuery().count());
		Assert.assertEquals(0, engine2.ManagementService.createJobQuery().count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobAddedGuardForTwoEnginesSameAcquisition() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testJobAddedGuardForTwoEnginesSameAcquisition()
	  {
	   // configure and build a process engine
		StandaloneProcessEngineConfiguration engineConfiguration1 = new StandaloneInMemProcessEngineConfiguration();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		engineConfiguration1.ProcessEngineName = this.GetType().FullName + "-engine1";
		engineConfiguration1.JdbcUrl = "jdbc:h2:mem:activiti1";
		engineConfiguration1.JobExecutorActivate = false;
		engineConfiguration1.JobExecutor = jobExecutor;
		engineConfiguration1.DbMetricsReporterActivate = false;
		ProcessEngine engine1 = engineConfiguration1.buildProcessEngine();
		createdProcessEngines.Add(engine1);

		// and a second one
		StandaloneProcessEngineConfiguration engineConfiguration2 = new StandaloneInMemProcessEngineConfiguration();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		engineConfiguration2.ProcessEngineName = this.GetType().FullName + "engine2";
		engineConfiguration2.JdbcUrl = "jdbc:h2:mem:activiti2";
		engineConfiguration2.JobExecutorActivate = false;
		engineConfiguration2.JobExecutor = jobExecutor;
		engineConfiguration2.DbMetricsReporterActivate = false;
		ProcessEngine engine2 = engineConfiguration2.buildProcessEngine();
		createdProcessEngines.Add(engine2);

		// stop the acquisition
		jobExecutor.shutdown();

		// deploy the processes

		engine1.RepositoryService.createDeployment().addClasspathResource(PROCESS_RESOURCE).deploy();

		engine2.RepositoryService.createDeployment().addClasspathResource(PROCESS_RESOURCE).deploy();

		// start one instance for each engine:

		engine1.RuntimeService.startProcessInstanceByKey("intermediateTimerEventExample");
		engine2.RuntimeService.startProcessInstanceByKey("intermediateTimerEventExample");

		DateTime calendar = new DateTime();
		calendar.add(Field.DAY_OF_YEAR.CalendarField, 6);
		ClockUtil.CurrentTime = calendar;

		Assert.assertEquals(1, engine1.ManagementService.createJobQuery().count());
		Assert.assertEquals(1, engine2.ManagementService.createJobQuery().count());

		// assert task completed for the first engine
		jobExecutor.start();
		waitForJobExecutorToProcessAllJobs(10000, 100, jobExecutor, engine1.ManagementService, false);

		// assert task completed for the second engine
		jobExecutor.start();
		waitForJobExecutorToProcessAllJobs(10000, 100, jobExecutor, engine2.ManagementService, false);

		Thread.Sleep(2000);

		Assert.assertFalse(((SequentialJobAcquisitionRunnable) jobExecutor.AcquireJobsRunnable).JobAdded);

		Assert.assertEquals(0, engine1.ManagementService.createJobQuery().count());
		Assert.assertEquals(0, engine2.ManagementService.createJobQuery().count());
	  }


	  ////////// helper methods ////////////////////////////


	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait, long intervalMillis, JobExecutor jobExecutor, ManagementService managementService, bool shutdown)
	  {

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
			  areJobsAvailable = areJobsAvailable(managementService);
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
		  if (shutdown)
		  {
			jobExecutor.shutdown();
		  }
		}
	  }

	  public virtual bool areJobsAvailable(ManagementService managementService)
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
		public virtual void run()
		{
		  timeLimitExceeded = true;
		  thread.Interrupt();
		}
	  }

	}

}