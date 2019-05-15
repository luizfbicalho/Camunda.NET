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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SetJobPriorityCmd : Command<Void>
	{

	  public const string JOB_PRIORITY_PROPERTY = "priority";

	  protected internal string jobId;
	  protected internal long priority;

	  public SetJobPriorityCmd(string jobId, long priority)
	  {
		this.jobId = jobId;
		this.priority = priority;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		EnsureUtil.ensureNotNull("job id must not be null", "jobId", jobId);

		JobEntity job = commandContext.JobManager.findJobById(jobId);
		EnsureUtil.ensureNotNull(typeof(NotFoundException), "No job found with id '" + jobId + "'", "job", job);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateJob(job);
		}

		long currentPriority = job.Priority;
		job.Priority = priority;

		createOpLogEntry(commandContext, currentPriority, job);

		return null;
	  }

	  protected internal virtual void createOpLogEntry(CommandContext commandContext, long previousPriority, JobEntity job)
	  {
		PropertyChange propertyChange = new PropertyChange(JOB_PRIORITY_PROPERTY, previousPriority, job.Priority);
		commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, job.Id, job.JobDefinitionId, job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, propertyChange);
	  }
	}

}