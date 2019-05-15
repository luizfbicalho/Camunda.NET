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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class AbstractSetJobRetriesCmd
	{
	  protected internal const string RETRIES = "retries";

	  protected internal virtual void setJobRetriesByJobId(string jobId, int retries, CommandContext commandContext)
	  {
		JobEntity job = commandContext.JobManager.findJobById(jobId);
		if (job != null)
		{
		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkUpdateRetriesJob(job);
		  }

		  if (job.InInconsistentLockState)
		  {
			job.resetLock();
		  }
		  int oldRetries = job.Retries;
		  job.Retries = retries;

		  PropertyChange propertyChange = new PropertyChange(RETRIES, oldRetries, job.Retries);
		  commandContext.OperationLogManager.logJobOperation(LogEntryOperation, job.Id, job.JobDefinitionId, job.ProcessInstanceId, job.ProcessDefinitionId, job.ProcessDefinitionKey, propertyChange);
		}
		else
		{
		  throw new ProcessEngineException("No job found with id '" + jobId + "'.");
		}

	  }

	  protected internal virtual string LogEntryOperation
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES;
		  }
	  }
	}

}