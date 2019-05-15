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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Saeid Mirzaei
	/// @author Joram Barrez
	/// </summary>

	[Serializable]
	public class DeleteJobCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string jobId;

	  public DeleteJobCmd(string jobId)
	  {
		this.jobId = jobId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ensureNotNull("jobId", jobId);

		JobEntity job = commandContext.JobManager.findJobById(jobId);
		ensureNotNull("No job found with id '" + jobId + "'", "job", job);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateJob(job);
		}
		// We need to check if the job was locked, ie acquired by the job acquisition thread
		// This happens if the the job was already acquired, but not yet executed.
		// In that case, we can't allow to delete the job.
		if (!string.ReferenceEquals(job.LockOwner, null) || job.LockExpirationTime != null)
		{
		  throw new ProcessEngineException("Cannot delete job when the job is being executed. Try again later.");
		}

		commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, jobId, job.JobDefinitionId, job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, PropertyChange.EMPTY_CHANGE);

		job.delete();
		return null;
	  }

	}

}