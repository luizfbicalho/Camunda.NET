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

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using OptimisticLockingListener = org.camunda.bpm.engine.impl.db.entitymanager.OptimisticLockingListener;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AcquireJobsCommandFactory = org.camunda.bpm.engine.impl.jobexecutor.AcquireJobsCommandFactory;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ControllableCommand = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ControllableCommand;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using ControllableThread = org.camunda.bpm.engine.test.concurrency.ControllableThread;

	/// <summary>
	/// Job executor that uses a <seealso cref="ControllableThread"/> for job acquisition. That means,
	/// the job acquisition thread returns control with each iteration of acquiring jobs (specifically
	/// between selecting jobs and returning them to the acquisition runnable).
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class ControllableJobExecutor : JobExecutor
	{

	  protected internal ThreadControl acquisitionThreadControl;
	  protected internal ThreadControl executionThreadControl;

	  protected internal bool syncOnShutdown = true;
	  protected internal bool syncAsSuspendEnabled;

	  protected internal bool shouldThrowOle;
	  protected internal bool oleThrown;

	  public ControllableJobExecutor()
	  {
		acquireJobsRunnable = new RecordingAcquireJobsRunnable(this);
		jobAcquisitionThread = new Thread(acquireJobsRunnable);
		acquisitionThreadControl = new ThreadControl(jobAcquisitionThread);
		executionThreadControl = new ThreadControl(jobAcquisitionThread); // execution thread is same as acquisition thread
		acquireJobsCmdFactory = new ControllableJobAcquisitionCommandFactory(this);
	  }

	  /// <summary>
	  /// <para>Creates the job executor and registers the given process engine
	  /// with it.
	  /// 
	  /// </para>
	  /// <para>Use this constructor if the process engine is not registered
	  /// with the job executor when the process engine is bootstrapped.
	  /// 
	  /// </para>
	  /// <para>Note: this is a hack since it enables to use multiple job executors with
	  /// the same engine which is not a supported feature (and for example clashes with
	  /// processEngineConfiguration#getJobExecutor)
	  /// </para>
	  /// </summary>
	  public ControllableJobExecutor(ProcessEngineImpl processEngine) : this()
	  {
		processEngines.Add(processEngine);
	  }

	  public ControllableJobExecutor(bool syncAsSuspendEnabled) : this()
	  {
		this.syncAsSuspendEnabled = syncAsSuspendEnabled;
	  }

	  public virtual bool SyncAsSuspendEnabled
	  {
		  get
		  {
			return syncAsSuspendEnabled;
		  }
	  }

	  /// <summary>
	  /// <para>true: behave like embedded job executor where shutdown waits for all jobs to end
	  /// </para>
	  /// <para>false: behave like runtime container job executor where shutdown does not influence job execution
	  /// </para>
	  /// </summary>
	  public virtual ControllableJobExecutor proceedAndWaitOnShutdown(bool syncOnShutdown)
	  {
		this.syncOnShutdown = syncOnShutdown;
		return this;
	  }

	  protected internal override void ensureInitialization()
	  {
		// already initialized in constructor
	  }

	  public virtual ThreadControl AcquisitionThreadControl
	  {
		  get
		  {
			return acquisitionThreadControl;
		  }
	  }

	  public virtual ThreadControl ExecutionThreadControl
	  {
		  get
		  {
			return executionThreadControl;
		  }
	  }

	  protected internal override void startExecutingJobs()
	  {
		jobAcquisitionThread.Start();
	  }

	  protected internal override void stopExecutingJobs()
	  {
		if (syncOnShutdown)
		{
		  acquisitionThreadControl.waitUntilDone(true);
		}
	  }

	  public override RecordingAcquireJobsRunnable AcquireJobsRunnable
	  {
		  get
		  {
			return (RecordingAcquireJobsRunnable) base.AcquireJobsRunnable;
		  }
	  }

	  public override void executeJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		getExecuteJobsRunnable(jobIds, processEngine).run();
	  }

	  public class ControllableJobAcquisitionCommandFactory : AcquireJobsCommandFactory
	  {
		  private readonly ControllableJobExecutor outerInstance;

		  public ControllableJobAcquisitionCommandFactory(ControllableJobExecutor outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public virtual Command<AcquiredJobs> getCommand(int numJobsToAcquire)
		{
		  return new ControllableAcquisitionCommand(outerInstance, outerInstance.acquisitionThreadControl, numJobsToAcquire);
		}
	  }

	  public class ControllableAcquisitionCommand : ControllableCommand<AcquiredJobs>
	  {
		  private readonly ControllableJobExecutor outerInstance;


		protected internal int numJobsToAcquire;

		public ControllableAcquisitionCommand(ControllableJobExecutor outerInstance, ThreadControl threadControl, int numJobsToAcquire) : base(threadControl)
		{
			this.outerInstance = outerInstance;
		  this.numJobsToAcquire = numJobsToAcquire;
		}

		public override AcquiredJobs execute(CommandContext commandContext)
		{

		  if (outerInstance.shouldThrowOle)
		  {
			rethrowOptimisticLockingException(commandContext);
		  }

		  monitor.sync(); // wait till makeContinue() is called from test thread

		  AcquiredJobs acquiredJobs = (new AcquireJobsCmd(outerInstance, numJobsToAcquire)).execute(commandContext);

		  monitor.sync(); // wait till makeContinue() is called from test thread

		  return acquiredJobs;
		}

		protected internal virtual void rethrowOptimisticLockingException(CommandContext commandContext)
		{
		  commandContext.DbEntityManager.registerOptimisticLockingListener(new OptimisticLockingListenerAnonymousInnerClass(this));
		}

		private class OptimisticLockingListenerAnonymousInnerClass : OptimisticLockingListener
		{
			private readonly ControllableAcquisitionCommand outerInstance;

			public OptimisticLockingListenerAnonymousInnerClass(ControllableAcquisitionCommand outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public Type EntityType
			{
				get
				{
				  return typeof(JobEntity);
				}
			}

			public void failedOperation(DbOperation operation)
			{
			  outerInstance.outerInstance.oleThrown = true;
			}

		}

	  }

	  public virtual void indicateOptimisticLockingException()
	  {
		shouldThrowOle = true;
	  }

	  public virtual bool OleThrown
	  {
		  get
		  {
			return oleThrown;
		  }
	  }

	  public virtual void resetOleThrown()
	  {
		oleThrown = false;
	  }

	}

}