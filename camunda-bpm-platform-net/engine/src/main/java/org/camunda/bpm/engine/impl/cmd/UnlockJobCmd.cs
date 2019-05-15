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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobExecutorLogger = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorLogger;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// Unlock job.
	/// 
	/// @author Thomas Skjolberg
	/// </summary>

	public class UnlockJobCmd : Command<Void>
	{

	  protected internal const long serialVersionUID = 1L;

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal string jobId;

	  public UnlockJobCmd(string jobId)
	  {
		this.jobId = jobId;
	  }

	  protected internal virtual JobEntity Job
	  {
		  get
		  {
			return Context.CommandContext.JobManager.findJobById(jobId);
		  }
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		JobEntity job = Job;

		if (Context.JobExecutorContext == null)
		{
		  EnsureUtil.ensureNotNull("Job with id " + jobId + " does not exist", "job", job);
		}
		else if (Context.JobExecutorContext != null && job == null)
		{
		  // CAM-1842
		  // Job was acquired but does not exist anymore. This is not a problem.
		  // It usually means that the job has been deleted after it was acquired which can happen if the
		  // the activity instance corresponding to the job is cancelled.
		  LOG.debugAcquiredJobNotFound(jobId);
		  return null;
		}

		job.unlock();

		return null;
	  }
	}

}