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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobExecutorContext = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorContext;
	using JobExecutorLogger = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorLogger;
	using JobFailureCollector = org.camunda.bpm.engine.impl.jobexecutor.JobFailureCollector;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ExecuteJobsCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal string jobId;

	  protected internal JobFailureCollector jobFailureCollector;

	  public ExecuteJobsCmd(string jobId, JobFailureCollector jobFailureCollector)
	  {
		this.jobId = jobId;
		this.jobFailureCollector = jobFailureCollector;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("jobId", jobId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.JobEntity job = commandContext.getDbEntityManager().selectById(org.camunda.bpm.engine.impl.persistence.entity.JobEntity.class, jobId);
		JobEntity job = commandContext.DbEntityManager.selectById(typeof(JobEntity), jobId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = processEngineConfiguration.getIdentityService();
		IdentityService identityService = processEngineConfiguration.IdentityService;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.jobexecutor.JobExecutorContext jobExecutorContext = org.camunda.bpm.engine.impl.context.Context.getJobExecutorContext();
		JobExecutorContext jobExecutorContext = Context.JobExecutorContext;

		if (job == null)
		{
		  if (jobExecutorContext != null)
		  {
			// CAM-1842
			// Job was acquired but does not exist anymore. This is not a problem.
			// It usually means that the job has been deleted after it was acquired which can happen if the
			// the activity instance corresponding to the job is cancelled.
			LOG.debugAcquiredJobNotFound(jobId);
			return null;

		  }
		  else
		  {
			throw LOG.jobNotFoundException(jobId);
		  }
		}

		jobFailureCollector.Job = job;

		if (jobExecutorContext == null)
		{ // if null, then we are not called by the job executor
		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkUpdateJob(job);
		  }
		  // write a user operation log since we're not called by the job executor
		  commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_EXECUTE, jobId, job.JobDefinitionId, job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, PropertyChange.EMPTY_CHANGE);
		}
		else
		{
		  jobExecutorContext.CurrentJob = job;

		  // if the job is called by the job executor then set the tenant id of the job
		  // as authenticated tenant to enable tenant checks
		  string tenantId = job.TenantId;
		  if (!string.ReferenceEquals(tenantId, null))
		  {
			identityService.setAuthentication(null, null, Collections.singletonList(tenantId));
		  }
		}

		try
		{

		  // register as command context close lister to intercept exceptions on flush
		  commandContext.registerCommandContextListener(jobFailureCollector);

		  commandContext.CurrentJob = job;

		  job.execute(commandContext);

		}
		finally
		{
		  if (jobExecutorContext != null)
		  {
			jobExecutorContext.CurrentJob = null;
			identityService.clearAuthentication();
		  }
		}

		return null;
	  }

	}

}