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
namespace org.camunda.bpm.engine.test.api.history.removaltime.cleanup
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultHistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.DefaultHistoryRemovalTimeProvider;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using After = org.junit.After;
	using AfterClass = org.junit.AfterClass;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHandler.MAX_BATCH_SIZE;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public abstract class AbstractHistoryCleanupSchedulerTest
	{
		private bool InstanceFieldsInitialized = false;

		public AbstractHistoryCleanupSchedulerTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			thisClass = this.GetType();
		}


	  protected internal Type thisClass;

	  protected internal HistoryLevel customHistoryLevel = new CustomHistoryLevelRemovalTime();

	  protected internal static ProcessEngineConfigurationImpl engineConfiguration;

	  protected internal ISet<string> jobIds = new HashSet<string>();

	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

	  protected internal readonly DateTime END_DATE = new DateTime(1363608000000L);

	  public virtual void initEngineConfiguration(ProcessEngineConfigurationImpl engineConfiguration)
	  {
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).setHistoryRemovalTimeProvider(new DefaultHistoryRemovalTimeProvider()).initHistoryRemovalTime();

		engineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

		engineConfiguration.HistoryCleanupBatchSize = MAX_BATCH_SIZE;
		engineConfiguration.HistoryCleanupBatchWindowStartTime = "13:00";
		engineConfiguration.HistoryCleanupDegreeOfParallelism = 1;

		engineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		clearMeterLog();

		foreach (string jobId in jobIds)
		{
		  clearJobLog(jobId);
		  clearJob(jobId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDownAfterAll()
	  public static void tearDownAfterAll()
	  {
		if (engineConfiguration != null)
		{
		  engineConfiguration.setHistoryRemovalTimeProvider(null).setHistoryRemovalTimeStrategy(null).initHistoryRemovalTime();

		  engineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;

		  engineConfiguration.HistoryCleanupBatchSize = MAX_BATCH_SIZE;
		  engineConfiguration.HistoryCleanupBatchWindowStartTime = null;
		  engineConfiguration.HistoryCleanupDegreeOfParallelism = 1;

		  engineConfiguration.initHistoryCleanup();
		}

		ClockUtil.reset();
	  }

	  // helper /////////////////////////////////////////////////////////////////

	  protected internal virtual IList<HistoryLevel> setCustomHistoryLevel(HistoryEventTypes eventType)
	  {
		((CustomHistoryLevelRemovalTime)customHistoryLevel).EventTypes = eventType;

		return Collections.singletonList(customHistoryLevel);
	  }

	  protected internal virtual IList<Job> runHistoryCleanup()
	  {
		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in jobs)
		{
		  jobIds.Add(job.Id);
		  managementService.executeJob(job.Id);
		}

		return jobs;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearJobLog(final String jobId)
	  protected internal virtual void clearJobLog(string jobId)
	  {
		CommandExecutor commandExecutor = engineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this, jobId));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly AbstractHistoryCleanupSchedulerTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass(AbstractHistoryCleanupSchedulerTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearJob(final String jobId)
	  protected internal virtual void clearJob(string jobId)
	  {
		engineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, jobId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly AbstractHistoryCleanupSchedulerTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass2(AbstractHistoryCleanupSchedulerTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			JobEntity job = commandContext.JobManager.findJobById(jobId);
			if (job != null)
			{
			  commandContext.JobManager.delete(job);
			}
			return null;
		  }
	  }

	  protected internal virtual void clearMeterLog()
	  {
		engineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this));
	  }

	  private class CommandAnonymousInnerClass3 : Command<object>
	  {
		  private readonly AbstractHistoryCleanupSchedulerTest outerInstance;

		  public CommandAnonymousInnerClass3(AbstractHistoryCleanupSchedulerTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

	  protected internal virtual IList<HistoryLevel> setCustomHistoryLevel(params HistoryEventTypes[] eventType)
	  {
		((CustomHistoryLevelRemovalTime)customHistoryLevel).EventTypes = eventType;

		return Collections.singletonList(customHistoryLevel);
	  }

	  public virtual ProcessEngineConfiguration configure(ProcessEngineConfigurationImpl configuration, params HistoryEventTypes[] historyEventTypes)
	  {
		configuration.JdbcUrl = "jdbc:h2:mem:" + thisClass.Name;
		configuration.CustomHistoryLevels = setCustomHistoryLevel(historyEventTypes);
		configuration.History = customHistoryLevel.Name;
		configuration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_CREATE_DROP;
		return configuration;
	  }

	}

}