using System;

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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ControllableJobExecutor = org.camunda.bpm.engine.test.jobexecutor.ControllableJobExecutor;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.commons.lang3.time.DateUtils.addDays;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.commons.lang3.time.DateUtils.addSeconds;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandlerConfiguration.START_DELAY;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class CompetingHistoryCleanupAcquisitionTest : ConcurrencyTestCase
	{

	  protected internal readonly DateTime CURRENT_DATE = new DateTime(1363608000000L);

	  protected internal static ThreadControl cleanupThread = null;

	  protected internal static ThreadLocal<bool> syncBeforeFlush = new ThreadLocal<bool>();

	  protected internal ControllableJobExecutor jobExecutor = new ControllableJobExecutor();

	  protected internal ThreadControl acquisitionThread;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		acquisitionThread = jobExecutor.AcquisitionThreadControl;
		acquisitionThread.reportInterrupts();

		ClockUtil.CurrentTime = CURRENT_DATE;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		if (jobExecutor.Active)
		{
		  jobExecutor.shutdown();
		}

		jobExecutor.resetOleThrown();

		clearDatabase();

		ClockUtil.reset();

		base.tearDown();
	  }

	  /// <summary>
	  /// Problem
	  /// 
	  /// GIVEN
	  /// Within the Execution TX the job lock was removed
	  /// 
	  /// WHEN
	  /// 1) the acquisition thread tries to lock the job
	  /// 2) the cleanup scheduler reschedules the job
	  /// 
	  /// THEN
	  /// The acquisition fails due to an Optimistic Locking Exception
	  /// </summary>
	  public virtual void testAcquiringEverLivingJobSucceeds()
	  {
		// given
		jobExecutor.indicateOptimisticLockingException();

		string jobId = historyService.cleanUpHistoryAsync(true).Id;

		lockEverLivingJob(jobId);

		cleanupThread = executeControllableCommand(new CleanupThread(this, jobId));

		cleanupThread.waitForSync(); // wait before flush of execution
		cleanupThread.makeContinueAndWaitForSync(); // flush execution and wait before flush of rescheduler

		jobExecutor.start();

		acquisitionThread.waitForSync();
		acquisitionThread.makeContinueAndWaitForSync(); // wait before flush of acquisition

		// when
		cleanupThread.makeContinue(); // flush rescheduler

		cleanupThread.join();

		acquisitionThread.makeContinueAndWaitForSync(); // flush acquisition

		Job job = managementService.createJobQuery().jobId(jobId).singleResult();

		// then
		assertThat(job.Duedate).isEqualTo(addSeconds(CURRENT_DATE, START_DELAY));
		assertThat(jobExecutor.OleThrown).False;
	  }

	  /// <summary>
	  /// Problem
	  /// 
	  /// GIVEN
	  /// Within the Execution TX the job lock was removed
	  /// 
	  /// WHEN
	  /// 1) the cleanup scheduler reschedules the job
	  /// 2) the acquisition thread tries to lock the job
	  /// 
	  /// THEN
	  /// The cleanup scheduler fails to reschedule the job due to an Optimistic Locking Exception
	  /// </summary>
	  public virtual void testReschedulingEverLivingJobSucceeds()
	  {
		// given
		string jobId = historyService.cleanUpHistoryAsync(true).Id;

		lockEverLivingJob(jobId);

		cleanupThread = executeControllableCommand(new CleanupThread(this, jobId));

		cleanupThread.waitForSync(); // wait before flush of execution
		cleanupThread.makeContinueAndWaitForSync(); // flush execution and wait before flush of rescheduler

		jobExecutor.start();

		acquisitionThread.waitForSync();
		acquisitionThread.makeContinueAndWaitForSync();

		// when
		acquisitionThread.makeContinueAndWaitForSync(); // flush acquisition

		cleanupThread.makeContinue(); // flush rescheduler

		cleanupThread.join();


		Job job = managementService.createJobQuery().jobId(jobId).singleResult();

		// then
		assertThat(job.Duedate).isEqualTo(addSeconds(CURRENT_DATE, START_DELAY));
	  }

	  public class CleanupThread : ControllableCommand<Void>
	  {
		  private readonly CompetingHistoryCleanupAcquisitionTest outerInstance;


		protected internal string jobId;

		protected internal CleanupThread(CompetingHistoryCleanupAcquisitionTest outerInstance, string jobId)
		{
			this.outerInstance = outerInstance;
		  this.jobId = jobId;
		}

		public override Void execute(CommandContext commandContext)
		{
		  syncBeforeFlush.set(true);

		  outerInstance.managementService.executeJob(jobId);

		  return null;
		}

	  }

	  // helpers ///////////////////////////////////////////////////////////////////////////////////////////////////////////

	  protected internal override void initializeProcessEngine()
	  {
		processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml");

		jobExecutor.MaxJobsPerAcquisition = 1;
		processEngineConfiguration.JobExecutor = jobExecutor;
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = "12:00";

		processEngineConfiguration.CustomPostCommandInterceptorsTxRequiresNew = Collections.singletonList<CommandInterceptor>(new CommandInterceptorAnonymousInnerClass(this));

		processEngine = processEngineConfiguration.buildProcessEngine();
	  }

	  private class CommandInterceptorAnonymousInnerClass : CommandInterceptor
	  {
		  private readonly CompetingHistoryCleanupAcquisitionTest outerInstance;

		  public CommandInterceptorAnonymousInnerClass(CompetingHistoryCleanupAcquisitionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override T execute<T>(Command<T> command)
		  {

			T executed = next.execute(command);
			if (syncBeforeFlush.get() != null && syncBeforeFlush.get())
			{
			  cleanupThread.sync();
			}

			return executed;
		  }
	  }

	  protected internal virtual void clearDatabase()
	  {
		deleteHistoryCleanupJobs();

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CompetingHistoryCleanupAcquisitionTest outerInstance;

		  public CommandAnonymousInnerClass(CompetingHistoryCleanupAcquisitionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.MeterLogManager.deleteAll();

			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType("history-cleanup");

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void lockEverLivingJob(final String jobId)
	  protected internal virtual void lockEverLivingJob(string jobId)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, jobId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly CompetingHistoryCleanupAcquisitionTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass2(CompetingHistoryCleanupAcquisitionTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			JobEntity job = commandContext.JobManager.findJobById(jobId);

			job.LockOwner = "foo";

			job.LockExpirationTime = addDays(outerInstance.CURRENT_DATE, 10);

			return null;
		  }
	  }

	}

}