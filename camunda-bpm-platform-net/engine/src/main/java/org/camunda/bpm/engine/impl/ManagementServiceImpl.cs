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
namespace org.camunda.bpm.engine.impl
{
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchQuery = org.camunda.bpm.engine.batch.BatchQuery;
	using BatchStatisticsQuery = org.camunda.bpm.engine.batch.BatchStatisticsQuery;
	using BatchQueryImpl = org.camunda.bpm.engine.impl.batch.BatchQueryImpl;
	using BatchStatisticsQueryImpl = org.camunda.bpm.engine.impl.batch.BatchStatisticsQueryImpl;
	using DeleteBatchCmd = org.camunda.bpm.engine.impl.batch.DeleteBatchCmd;
	using org.camunda.bpm.engine.impl.cmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbSqlSession = org.camunda.bpm.engine.impl.db.sql.DbSqlSession;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecuteJobHelper = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobHelper;
	using PurgeReport = org.camunda.bpm.engine.impl.management.PurgeReport;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using UpdateJobSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobSuspensionStateBuilderImpl;
	using MetricsQueryImpl = org.camunda.bpm.engine.impl.metrics.MetricsQueryImpl;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;
	using DeploymentStatisticsQuery = org.camunda.bpm.engine.management.DeploymentStatisticsQuery;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;
	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;
	using TableMetaData = org.camunda.bpm.engine.management.TableMetaData;
	using TablePageQuery = org.camunda.bpm.engine.management.TablePageQuery;
	using UpdateJobDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateSelectBuilder;
	using UpdateJobSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateSelectBuilder;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;



	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Saeid Mizaei
	/// @author Askar AKhmerov
	/// </summary>
	public class ManagementServiceImpl : ServiceImpl, ManagementService
	{

	  public virtual ProcessApplicationRegistration registerProcessApplication(string deploymentId, ProcessApplicationReference reference)
	  {
		return commandExecutor.execute(new RegisterProcessApplicationCmd(deploymentId, reference));
	  }

	  public virtual void unregisterProcessApplication(string deploymentId, bool removeProcessesFromCache)
	  {
		commandExecutor.execute(new UnregisterProcessApplicationCmd(deploymentId, removeProcessesFromCache));
	  }

	  public virtual void unregisterProcessApplication(ISet<string> deploymentIds, bool removeProcessesFromCache)
	  {
		commandExecutor.execute(new UnregisterProcessApplicationCmd(deploymentIds, removeProcessesFromCache));
	  }

	  public virtual string getProcessApplicationForDeployment(string deploymentId)
	  {
		return commandExecutor.execute(new GetProcessApplicationForDeploymentCmd(deploymentId));
	  }

	  public virtual IDictionary<string, long> TableCount
	  {
		  get
		  {
			return commandExecutor.execute(new GetTableCountCmd());
		  }
	  }

	  public virtual string getTableName(Type activitiEntityClass)
	  {
		return commandExecutor.execute(new GetTableNameCmd(activitiEntityClass));
	  }

	  public virtual TableMetaData getTableMetaData(string tableName)
	  {
		return commandExecutor.execute(new GetTableMetaDataCmd(tableName));
	  }

	  public virtual void executeJob(string jobId)
	  {
		ExecuteJobHelper.executeJob(jobId, commandExecutor);
	  }

	  public virtual void deleteJob(string jobId)
	  {
		commandExecutor.execute(new DeleteJobCmd(jobId));
	  }

	  public virtual void setJobRetries(string jobId, int retries)
	  {
		commandExecutor.execute(new SetJobRetriesCmd(jobId, null, retries));
	  }

	  public virtual void setJobRetries(IList<string> jobIds, int retries)
	  {
		commandExecutor.execute(new SetJobsRetriesCmd(jobIds, retries));
	  }

	  public virtual Batch setJobRetriesAsync(IList<string> jobIds, int retries)
	  {
		return this.setJobRetriesAsync(jobIds, (JobQuery) null, retries);
	  }

	  public virtual Batch setJobRetriesAsync(JobQuery jobQuery, int retries)
	  {
		return this.setJobRetriesAsync(null, jobQuery, retries);
	  }

	  public virtual Batch setJobRetriesAsync(IList<string> jobIds, JobQuery jobQuery, int retries)
	  {
		return commandExecutor.execute(new SetJobsRetriesBatchCmd(jobIds, jobQuery, retries));
	  }

	  public virtual Batch setJobRetriesAsync(IList<string> processInstanceIds, ProcessInstanceQuery query, int retries)
	  {
		return commandExecutor.execute(new SetJobsRetriesByProcessBatchCmd(processInstanceIds, query, retries));
	  }

	  public virtual void setJobRetriesByJobDefinitionId(string jobDefinitionId, int retries)
	  {
		commandExecutor.execute(new SetJobRetriesCmd(null, jobDefinitionId, retries));
	  }

	  public virtual void setJobDuedate(string jobId, DateTime newDuedate)
	  {
		commandExecutor.execute(new SetJobDuedateCmd(jobId, newDuedate));
	  }

	  public virtual void recalculateJobDuedate(string jobId, bool creationDateBased)
	  {
		commandExecutor.execute(new RecalculateJobDuedateCmd(jobId, creationDateBased));
	  }

	  public virtual void setJobPriority(string jobId, long priority)
	  {
		commandExecutor.execute(new SetJobPriorityCmd(jobId, priority));
	  }

	  public virtual TablePageQuery createTablePageQuery()
	  {
		return new TablePageQueryImpl(commandExecutor);
	  }

	  public virtual JobQuery createJobQuery()
	  {
		return new JobQueryImpl(commandExecutor);
	  }

	  public virtual JobDefinitionQuery createJobDefinitionQuery()
	  {
		return new JobDefinitionQueryImpl(commandExecutor);
	  }

	  public virtual string getJobExceptionStacktrace(string jobId)
	  {
		return commandExecutor.execute(new GetJobExceptionStacktraceCmd(jobId));
	  }

	  public virtual IDictionary<string, string> Properties
	  {
		  get
		  {
			return commandExecutor.execute(new GetPropertiesCmd());
		  }
	  }

	  public virtual void setProperty(string name, string value)
	  {
		commandExecutor.execute(new SetPropertyCmd(name, value));
	  }

	  public virtual void deleteProperty(string name)
	  {
		commandExecutor.execute(new DeletePropertyCmd(name));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String databaseSchemaUpgrade(final java.sql.Connection connection, final String catalog, final String schema)
	  public virtual string databaseSchemaUpgrade(Connection connection, string catalog, string schema)
	  {
		return commandExecutor.execute(new CommandAnonymousInnerClass(this, connection, catalog, schema));
	  }

	  private class CommandAnonymousInnerClass : Command<string>
	  {
		  private readonly ManagementServiceImpl outerInstance;

		  private Connection connection;
		  private string catalog;
		  private string schema;

		  public CommandAnonymousInnerClass(ManagementServiceImpl outerInstance, Connection connection, string catalog, string schema)
		  {
			  this.outerInstance = outerInstance;
			  this.connection = connection;
			  this.catalog = catalog;
			  this.schema = schema;
		  }

		  public string execute(CommandContext commandContext)
		  {
			commandContext.AuthorizationManager.checkCamundaAdmin();
			DbSqlSessionFactory dbSqlSessionFactory = (DbSqlSessionFactory) commandContext.SessionFactories[typeof(DbSqlSession)];
			DbSqlSession dbSqlSession = new DbSqlSession(dbSqlSessionFactory, connection, catalog, schema);
			commandContext.Sessions[typeof(DbSqlSession)] = dbSqlSession;
			dbSqlSession.dbSchemaUpdate();

			return "";
		  }
	  }

	  /// <summary>
	  /// Purges the database and the deployment cache.
	  /// </summary>
	  public virtual PurgeReport purge()
	  {
		return commandExecutor.execute(new PurgeDatabaseAndCacheCmd());
	  }


	  public virtual ProcessDefinitionStatisticsQuery createProcessDefinitionStatisticsQuery()
	  {
		return new ProcessDefinitionStatisticsQueryImpl(commandExecutor);
	  }

	  public virtual ActivityStatisticsQuery createActivityStatisticsQuery(string processDefinitionId)
	  {
		return new ActivityStatisticsQueryImpl(processDefinitionId, commandExecutor);
	  }

	  public virtual DeploymentStatisticsQuery createDeploymentStatisticsQuery()
	  {
		return new DeploymentStatisticsQueryImpl(commandExecutor);
	  }

	  public virtual ISet<string> RegisteredDeployments
	  {
		  get
		  {
			return commandExecutor.execute(new CommandAnonymousInnerClass2(this));
		  }
	  }

	  private class CommandAnonymousInnerClass2 : Command<ISet<string>>
	  {
		  private readonly ManagementServiceImpl outerInstance;

		  public CommandAnonymousInnerClass2(ManagementServiceImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ISet<string> execute(CommandContext commandContext)
		  {
			commandContext.AuthorizationManager.checkCamundaAdmin();
			ISet<string> registeredDeployments = Context.ProcessEngineConfiguration.RegisteredDeployments;
			return new HashSet<string>(registeredDeployments);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void registerDeploymentForJobExecutor(final String deploymentId)
	  public virtual void registerDeploymentForJobExecutor(string deploymentId)
	  {
		commandExecutor.execute(new RegisterDeploymentCmd(deploymentId));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void unregisterDeploymentForJobExecutor(final String deploymentId)
	  public virtual void unregisterDeploymentForJobExecutor(string deploymentId)
	  {
		commandExecutor.execute(new UnregisterDeploymentCmd(deploymentId));
	  }


	  public virtual void activateJobDefinitionById(string jobDefinitionId)
	  {
		updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinitionId).activate();
	  }

	  public virtual void activateJobDefinitionById(string jobDefinitionId, bool activateJobs)
	  {
		updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinitionId).includeJobs(activateJobs).activate();
	  }

	  public virtual void activateJobDefinitionById(string jobDefinitionId, bool activateJobs, DateTime activationDate)
	  {
		updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinitionId).includeJobs(activateJobs).executionDate(activationDate).activate();
	  }

	  public virtual void suspendJobDefinitionById(string jobDefinitionId)
	  {
		updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinitionId).suspend();
	  }

	  public virtual void suspendJobDefinitionById(string jobDefinitionId, bool suspendJobs)
	  {
		updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinitionId).includeJobs(suspendJobs).suspend();
	  }

	  public virtual void suspendJobDefinitionById(string jobDefinitionId, bool suspendJobs, DateTime suspensionDate)
	  {
		updateJobDefinitionSuspensionState().byJobDefinitionId(jobDefinitionId).includeJobs(suspendJobs).executionDate(suspensionDate).suspend();
	  }

	  public virtual void activateJobDefinitionByProcessDefinitionId(string processDefinitionId)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).activate();
	  }

	  public virtual void activateJobDefinitionByProcessDefinitionId(string processDefinitionId, bool activateJobs)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).includeJobs(activateJobs).activate();
	  }

	  public virtual void activateJobDefinitionByProcessDefinitionId(string processDefinitionId, bool activateJobs, DateTime activationDate)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).includeJobs(activateJobs).executionDate(activationDate).activate();
	  }

	  public virtual void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).suspend();
	  }

	  public virtual void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId, bool suspendJobs)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).includeJobs(suspendJobs).suspend();
	  }

	  public virtual void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId, bool suspendJobs, DateTime suspensionDate)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionId).includeJobs(suspendJobs).executionDate(suspensionDate).suspend();
	  }

	  public virtual void activateJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).activate();
	  }

	  public virtual void activateJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool activateJobs)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).includeJobs(activateJobs).activate();
	  }

	  public virtual void activateJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool activateJobs, DateTime activationDate)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).includeJobs(activateJobs).executionDate(activationDate).activate();
	  }

	  public virtual void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).suspend();
	  }

	  public virtual void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool suspendJobs)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).includeJobs(suspendJobs).suspend();
	  }

	  public virtual void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool suspendJobs, DateTime suspensionDate)
	  {
		updateJobDefinitionSuspensionState().byProcessDefinitionKey(processDefinitionKey).includeJobs(suspendJobs).executionDate(suspensionDate).suspend();
	  }

	  public virtual UpdateJobDefinitionSuspensionStateSelectBuilder updateJobDefinitionSuspensionState()
	  {
		return new UpdateJobDefinitionSuspensionStateBuilderImpl(commandExecutor);
	  }

	  public virtual void activateJobById(string jobId)
	  {
		updateJobSuspensionState().byJobId(jobId).activate();
	  }

	  public virtual void activateJobByProcessInstanceId(string processInstanceId)
	  {
		updateJobSuspensionState().byProcessInstanceId(processInstanceId).activate();
	  }

	  public virtual void activateJobByJobDefinitionId(string jobDefinitionId)
	  {
		updateJobSuspensionState().byJobDefinitionId(jobDefinitionId).activate();
	  }

	  public virtual void activateJobByProcessDefinitionId(string processDefinitionId)
	  {
		updateJobSuspensionState().byProcessDefinitionId(processDefinitionId).activate();
	  }

	  public virtual void activateJobByProcessDefinitionKey(string processDefinitionKey)
	  {
		updateJobSuspensionState().byProcessDefinitionKey(processDefinitionKey).activate();
	  }

	  public virtual void suspendJobById(string jobId)
	  {
		updateJobSuspensionState().byJobId(jobId).suspend();
	  }

	  public virtual void suspendJobByJobDefinitionId(string jobDefinitionId)
	  {
		updateJobSuspensionState().byJobDefinitionId(jobDefinitionId).suspend();
	  }

	  public virtual void suspendJobByProcessInstanceId(string processInstanceId)
	  {
		updateJobSuspensionState().byProcessInstanceId(processInstanceId).suspend();
	  }

	  public virtual void suspendJobByProcessDefinitionId(string processDefinitionId)
	  {
		updateJobSuspensionState().byProcessDefinitionId(processDefinitionId).suspend();
	  }

	  public virtual void suspendJobByProcessDefinitionKey(string processDefinitionKey)
	  {
		updateJobSuspensionState().byProcessDefinitionKey(processDefinitionKey).suspend();
	  }

	  public virtual UpdateJobSuspensionStateSelectBuilder updateJobSuspensionState()
	  {
		return new UpdateJobSuspensionStateBuilderImpl(commandExecutor);
	  }

	  public virtual int HistoryLevel
	  {
		  get
		  {
			return commandExecutor.execute(new GetHistoryLevelCmd());
		  }
	  }

	  public virtual MetricsQuery createMetricsQuery()
	  {
		return new MetricsQueryImpl(commandExecutor);
	  }

	  public virtual void deleteMetrics(DateTime timestamp)
	  {
		commandExecutor.execute(new DeleteMetricsCmd(timestamp, null));
	  }

	  public virtual void deleteMetrics(DateTime timestamp, string reporter)
	  {
		commandExecutor.execute(new DeleteMetricsCmd(timestamp, reporter));

	  }

	  public virtual void reportDbMetricsNow()
	  {
		commandExecutor.execute(new ReportDbMetricsCmd());
	  }

	  public virtual void setOverridingJobPriorityForJobDefinition(string jobDefinitionId, long priority)
	  {
		commandExecutor.execute(new SetJobDefinitionPriorityCmd(jobDefinitionId, priority, false));
	  }

	  public virtual void setOverridingJobPriorityForJobDefinition(string jobDefinitionId, long priority, bool cascade)
	  {
		commandExecutor.execute(new SetJobDefinitionPriorityCmd(jobDefinitionId, priority, true));
	  }

	  public virtual void clearOverridingJobPriorityForJobDefinition(string jobDefinitionId)
	  {
		commandExecutor.execute(new SetJobDefinitionPriorityCmd(jobDefinitionId, null, false));
	  }

	  public virtual BatchQuery createBatchQuery()
	  {
		return new BatchQueryImpl(commandExecutor);
	  }

	  public virtual void deleteBatch(string batchId, bool cascade)
	  {
		commandExecutor.execute(new DeleteBatchCmd(batchId, cascade));
	  }

	  public virtual void suspendBatchById(string batchId)
	  {
		commandExecutor.execute(new SuspendBatchCmd(batchId));
	  }

	  public virtual void activateBatchById(string batchId)
	  {
		commandExecutor.execute(new ActivateBatchCmd(batchId));
	  }

	  public virtual BatchStatisticsQuery createBatchStatisticsQuery()
	  {
		return new BatchStatisticsQueryImpl(commandExecutor);
	  }

	  public virtual SchemaLogQuery createSchemaLogQuery()
	  {
		return new SchemaLogQueryImpl(commandExecutor);
	  }
	}

}