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
namespace org.camunda.bpm.engine.impl.cmd
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using HistoryCleanupContext = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupContext;
	using HistoryCleanupHelper = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHelper;
	using HistoryCleanupJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobDeclaration;
	using HistoryCleanupJobHandler = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandler;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using PropertyManager = org.camunda.bpm.engine.impl.persistence.entity.PropertyManager;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using Job = org.camunda.bpm.engine.runtime.Job;


	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	public class HistoryCleanupCmd : Command<Job>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public static readonly JobDeclaration HISTORY_CLEANUP_JOB_DECLARATION = new HistoryCleanupJobDeclaration();

	  public const int MAX_THREADS_NUMBER = 8;

	  private bool immediatelyDue;

	  public HistoryCleanupCmd(bool immediatelyDue)
	  {
		this.immediatelyDue = immediatelyDue;
	  }

	  public virtual Job execute(CommandContext commandContext)
	  {
		AuthorizationManager authorizationManager = commandContext.AuthorizationManager;
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;

		authorizationManager.checkCamundaAdmin();

		//validate
		if (!willBeScheduled())
		{
		  LOG.debugHistoryCleanupWrongConfiguration();
		}

		//find job instance
		IList<Job> historyCleanupJobs = HistoryCleanupJobs;

		int degreeOfParallelism = processEngineConfiguration.HistoryCleanupDegreeOfParallelism;
		int[][] minuteChunks = HistoryCleanupHelper.listMinuteChunks(degreeOfParallelism);

		if (shouldCreateJobs(historyCleanupJobs))
		{
		  historyCleanupJobs = createJobs(degreeOfParallelism, minuteChunks);

		}
		else if (shouldReconfigureJobs(historyCleanupJobs))
		{
		  historyCleanupJobs = reconfigureJobs(historyCleanupJobs, degreeOfParallelism, minuteChunks);

		}
		else if (shouldSuspendJobs(historyCleanupJobs))
		{
		  suspendJobs(historyCleanupJobs);

		}

		writeUserOperationLog(commandContext);

		return historyCleanupJobs.Count > 0 ? historyCleanupJobs[0] : null;
	  }

	  protected internal virtual IList<Job> HistoryCleanupJobs
	  {
		  get
		  {
			CommandContext commandContext = Context.CommandContext;
			return commandContext.JobManager.findJobsByHandlerType(HistoryCleanupJobHandler.TYPE);
		  }
	  }

	  protected internal virtual bool shouldCreateJobs(IList<Job> jobs)
	  {
		return jobs.Count == 0 && willBeScheduled();
	  }

	  protected internal virtual bool shouldReconfigureJobs(IList<Job> jobs)
	  {
		return jobs.Count > 0 && willBeScheduled();
	  }

	  protected internal virtual bool shouldSuspendJobs(IList<Job> jobs)
	  {
		return jobs.Count > 0 && !willBeScheduled();
	  }

	  protected internal virtual bool willBeScheduled()
	  {
		CommandContext commandContext = Context.CommandContext;
		return immediatelyDue || HistoryCleanupHelper.isBatchWindowConfigured(commandContext);
	  }

	  protected internal virtual IList<Job> createJobs(int degreeOfParallelism, int[][] minuteChunks)
	  {
		CommandContext commandContext = Context.CommandContext;

		PropertyManager propertyManager = commandContext.PropertyManager;
		JobManager jobManager = commandContext.JobManager;

		//exclusive lock
		propertyManager.acquireExclusiveLockForHistoryCleanupJob();

		//check again after lock
		IList<Job> historyCleanupJobs = HistoryCleanupJobs;

		if (historyCleanupJobs.Count == 0)
		{
		  foreach (int[] minuteChunk in minuteChunks)
		  {
			JobEntity job = createJob(minuteChunk);
			jobManager.insertAndHintJobExecutor(job);
			historyCleanupJobs.Add(job);
		  }
		}

		return historyCleanupJobs;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<org.camunda.bpm.engine.runtime.Job> reconfigureJobs(java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs, int degreeOfParallelism, int[][] minuteChunks)
	  protected internal virtual IList<Job> reconfigureJobs(IList<Job> historyCleanupJobs, int degreeOfParallelism, int[][] minuteChunks)
	  {
		CommandContext commandContext = Context.CommandContext;
		JobManager jobManager = commandContext.JobManager;

		int size = Math.Min(degreeOfParallelism, historyCleanupJobs.Count);

		for (int i = 0; i < size; i++)
		{
		  JobEntity historyCleanupJob = (JobEntity) historyCleanupJobs[i];

		  //apply new configuration
		  HistoryCleanupContext historyCleanupContext = createCleanupContext(minuteChunks[i]);

		  HISTORY_CLEANUP_JOB_DECLARATION.reconfigure(historyCleanupContext, historyCleanupJob);

		  DateTime newDueDate = HISTORY_CLEANUP_JOB_DECLARATION.resolveDueDate(historyCleanupContext);

		  jobManager.reschedule(historyCleanupJob, newDueDate);
		}

		int delta = degreeOfParallelism - historyCleanupJobs.Count;

		if (delta > 0)
		{
		  //create new job, as there are not enough of them
		  for (int i = size; i < degreeOfParallelism; i++)
		  {
			JobEntity job = createJob(minuteChunks[i]);
			jobManager.insertAndHintJobExecutor(job);
			historyCleanupJobs.Add(job);
		  }
		}
		else if (delta < 0)
		{
		  //remove jobs, if there are too much of them
		  IEnumerator<Job> iterator = historyCleanupJobs.listIterator(size);
		  while (iterator.MoveNext())
		  {
			JobEntity job = (JobEntity) iterator.Current;
			jobManager.deleteJob(job);
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			iterator.remove();
		  }
		}

		return historyCleanupJobs;
	  }

	  protected internal virtual void suspendJobs(IList<Job> jobs)
	  {
		foreach (Job job in jobs)
		{
		  JobEntity jobInstance = (JobEntity) job;
		  jobInstance.SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED.StateCode;
		  jobInstance.Duedate = null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.impl.persistence.entity.JobEntity createJob(int[] minuteChunk)
	  protected internal virtual JobEntity createJob(int[] minuteChunk)
	  {
		HistoryCleanupContext historyCleanupContext = createCleanupContext(minuteChunk);
		return HISTORY_CLEANUP_JOB_DECLARATION.createJobInstance(historyCleanupContext);
	  }

	  protected internal virtual HistoryCleanupContext createCleanupContext(int[] minuteChunk)
	  {
		int minuteFrom = minuteChunk[0];
		int minuteTo = minuteChunk[1];
		return new HistoryCleanupContext(immediatelyDue, minuteFrom, minuteTo);
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext)
	  {
		PropertyChange propertyChange = new PropertyChange("immediatelyDue", null, immediatelyDue);
		commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE_HISTORY_CLEANUP_JOB, null, null, null, null, null, propertyChange);
	  }
	}

}