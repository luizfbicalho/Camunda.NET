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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


	/// <summary>
	/// @author Kristin Polenz
	/// </summary>
	[Serializable]
	public class SetJobDuedateCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  private readonly string jobId;
	  private readonly DateTime newDuedate;

	  public SetJobDuedateCmd(string jobId, DateTime newDuedate)
	  {
		if (string.ReferenceEquals(jobId, null) || jobId.Length < 1)
		{
		  throw new ProcessEngineException("The job id is mandatory, but '" + jobId + "' has been provided.");
		}
		this.jobId = jobId;
		this.newDuedate = newDuedate;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		JobEntity job = commandContext.JobManager.findJobById(jobId);
		if (job != null)
		{

		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkUpdateJob(job);
		  }

		  commandContext.OperationLogManager.logJobOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_DUEDATE, jobId, job.JobDefinitionId, job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, Collections.singletonList(new PropertyChange("duedate", job.Duedate, newDuedate)));

		  job.Duedate = newDuedate;
		}
		else
		{
		  throw new ProcessEngineException("No job found with id '" + jobId + "'.");
		}
		return null;
	  }
	}

}