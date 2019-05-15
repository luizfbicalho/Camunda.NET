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
namespace org.camunda.bpm.engine.test.concurrency
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;


	using BootstrapEngineCommand = org.camunda.bpm.engine.impl.BootstrapEngineCommand;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandInvocationContext = org.camunda.bpm.engine.impl.interceptor.CommandInvocationContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// <para>Tests a concurrent attempt of a bootstrapping Process Engine to reconfigure
	/// the HistoryCleanupJob while the JobExecutor tries to execute it.</para>
	/// 
	/// @author Nikola Koevski
	/// </summary>
	public class ConcurrentProcessEngineJobExecutorHistoryCleanupJobTest : ConcurrencyTestCase
	{

	  private const string PROCESS_ENGINE_NAME = "historyCleanupJobEngine";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {

		// Ensure that current time is outside batch window
		DateTime timeOfDay = new DateTime();
		timeOfDay.set(DateTime.HOUR_OF_DAY, 17);
		ClockUtil.CurrentTime = timeOfDay;

		processEngineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;

		base.setUp();
	  }

	  protected internal override void closeDownProcessEngine()
	  {
		base.closeDownProcessEngine();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.ProcessEngine otherProcessEngine = org.camunda.bpm.engine.ProcessEngines.getProcessEngine(PROCESS_ENGINE_NAME);
		ProcessEngine otherProcessEngine = ProcessEngines.getProcessEngine(PROCESS_ENGINE_NAME);
		if (otherProcessEngine != null)
		{

		  ((ProcessEngineConfigurationImpl)otherProcessEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, otherProcessEngine));

		  otherProcessEngine.close();
		  ProcessEngines.unregister(otherProcessEngine);
		}
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ConcurrentProcessEngineJobExecutorHistoryCleanupJobTest outerInstance;

		  private ProcessEngine otherProcessEngine;

		  public CommandAnonymousInnerClass(ConcurrentProcessEngineJobExecutorHistoryCleanupJobTest outerInstance, ProcessEngine otherProcessEngine)
		  {
			  this.outerInstance = outerInstance;
			  this.otherProcessEngine = otherProcessEngine;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = otherProcessEngine.ManagementService.createJobQuery().list();
			if (jobs.Count > 0)
			{
			  assertEquals(1, jobs.Count);
			  string jobId = jobs[0].Id;
			  commandContext.JobManager.deleteJob((JobEntity) jobs[0]);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		((ProcessEngineConfigurationImpl)processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));
		ClockUtil.CurrentTime = DateTime.Now;
		base.tearDown();
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly ConcurrentProcessEngineJobExecutorHistoryCleanupJobTest outerInstance;

		  public CommandAnonymousInnerClass2(ConcurrentProcessEngineJobExecutorHistoryCleanupJobTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.processEngine.ManagementService.createJobQuery().list();
			if (jobs.Count > 0)
			{
			  assertEquals(1, jobs.Count);
			  string jobId = jobs[0].Id;
			  commandContext.JobManager.deleteJob((JobEntity) jobs[0]);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testConcurrentHistoryCleanupJobReconfigurationExecution() throws InterruptedException
	  public virtual void testConcurrentHistoryCleanupJobReconfigurationExecution()
	  {

		ProcessEngine.HistoryService.cleanUpHistoryAsync(true);

		ThreadControl thread1 = executeControllableCommand(new ControllableJobExecutionCommand());
		thread1.reportInterrupts();
		thread1.waitForSync();

		ControllableProcessEngineBootstrapCommand bootstrapCommand = new ControllableProcessEngineBootstrapCommand();
		ThreadControl thread2 = executeControllableCommand(bootstrapCommand);
		thread2.reportInterrupts();
		thread2.waitForSync();

		thread1.makeContinue();
		thread1.waitForSync();

		thread2.makeContinue();

		Thread.Sleep(2000);

		thread1.waitUntilDone();

		thread2.waitForSync();
		thread2.waitUntilDone(true);

		assertNull(thread1.Exception);
		assertNull(thread2.Exception);

		assertNull(bootstrapCommand.ContextSpy.Throwable);

		assertNotNull(ProcessEngines.ProcessEngines[PROCESS_ENGINE_NAME]);
	  }

	  protected internal class ControllableProcessEngineBootstrapCommand : ControllableCommand<Void>
	  {

		protected internal ControllableBootstrapEngineCommand bootstrapCommand;

		public override Void execute(CommandContext commandContext)
		{

		  bootstrapCommand = new ControllableBootstrapEngineCommand(this.monitor);

		  ProcessEngineConfiguration processEngineConfiguration = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/concurrency/historycleanup.camunda.cfg.xml");


		  processEngineConfiguration.ProcessEngineBootstrapCommand = bootstrapCommand;

		  processEngineConfiguration.ProcessEngineName = PROCESS_ENGINE_NAME;
		  processEngineConfiguration.buildProcessEngine();

		  return null;
		}

		public virtual CommandInvocationContext ContextSpy
		{
			get
			{
			  return bootstrapCommand.Spy;
			}
		}
	  }

	  protected internal class ControllableJobExecutionCommand : ControllableCommand<Void>
	  {

		public override Void execute(CommandContext commandContext)
		{

		  monitor.sync();

		  IList<Job> historyCleanupJobs = commandContext.ProcessEngineConfiguration.HistoryService.findHistoryCleanupJobs();

		  foreach (Job job in historyCleanupJobs)
		  {
			commandContext.ProcessEngineConfiguration.ManagementService.executeJob(job.Id);
		  }

		  monitor.sync();

		  return null;
		}
	  }

	  protected internal class ControllableBootstrapEngineCommand : BootstrapEngineCommand, Command<Void>
	  {

		protected internal readonly ThreadControl monitor;
		protected internal CommandInvocationContext spy;

		public ControllableBootstrapEngineCommand(ThreadControl threadControl)
		{
		  this.monitor = threadControl;
		}

		protected internal override void createHistoryCleanupJob(CommandContext commandContext)
		{

		  monitor.sync();

		  base.createHistoryCleanupJob(commandContext);
		  spy = Context.CommandInvocationContext;

		  monitor.sync();
		}

		public virtual CommandInvocationContext Spy
		{
			get
			{
			  return spy;
			}
		}
	  }
	}

}