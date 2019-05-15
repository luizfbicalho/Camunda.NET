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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Metrics = org.camunda.bpm.engine.management.Metrics;

	/// <summary>
	/// @author Frederik Heremans
	/// @author Bernd Ruecker
	/// </summary>
	public class FailedJobListener : Command<Void>
	{

	  protected internal CommandExecutor commandExecutor;
	  protected internal string jobId;
	  protected internal Exception exception;
	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;
	  private int countRetries = 0;
	  private int totalRetries = ProcessEngineConfigurationImpl.DEFAULT_FAILED_JOB_LISTENER_MAX_RETRIES;

	  public FailedJobListener(CommandExecutor commandExecutor, string jobId, Exception exception)
	  {
		this.commandExecutor = commandExecutor;
		this.jobId = jobId;
		this.exception = exception;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		initTotalRetries(commandContext);

		logJobFailure(commandContext);

		FailedJobCommandFactory failedJobCommandFactory = commandContext.FailedJobCommandFactory;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.Command<Object> cmd = failedJobCommandFactory.getCommand(jobId, exception);
		Command<object> cmd = failedJobCommandFactory.getCommand(jobId, exception);

		commandExecutor.execute(new CommandAnonymousInnerClass(this, commandContext, cmd));

		return null;
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly FailedJobListener outerInstance;

		  private CommandContext commandContext;
		  private Command<object> cmd;

		  public CommandAnonymousInnerClass(FailedJobListener outerInstance, CommandContext commandContext, Command<object> cmd)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.cmd = cmd;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			JobEntity job = commandContext.JobManager.findJobById(outerInstance.jobId);

			if (job != null)
			{
			  outerInstance.fireHistoricJobFailedEvt(job);
			  cmd.execute(commandContext);
			}
			else
			{
			  LOG.debugFailedJobNotFound(outerInstance.jobId);
			}
			return null;
		  }
	  }

	  private void initTotalRetries(CommandContext commandContext)
	  {
		totalRetries = commandContext.ProcessEngineConfiguration.FailedJobListenerMaxRetries;
	  }

	  protected internal virtual void fireHistoricJobFailedEvt(JobEntity job)
	  {
		CommandContext commandContext = Context.CommandContext;

		// the given job failed and a rollback happened,
		// that's why we have to increment the job
		// sequence counter once again
		job.incrementSequenceCounter();

		commandContext.HistoricJobLogManager.fireJobFailedEvent(job, exception);
	  }

	  protected internal virtual void logJobFailure(CommandContext commandContext)
	  {
		if (commandContext.ProcessEngineConfiguration.MetricsEnabled)
		{
		  commandContext.ProcessEngineConfiguration.MetricsRegistry.markOccurrence(Metrics.JOB_FAILED);
		}
	  }

	  public virtual void incrementCountRetries()
	  {
		this.countRetries++;
	  }

	  public virtual int RetriesLeft
	  {
		  get
		  {
			return Math.Max(0, totalRetries - countRetries);
		  }
	  }
	}

}