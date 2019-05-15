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
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[Serializable]
	public class SetJobRetriesCmd : AbstractSetJobRetriesCmd, Command<Void>
	{

	  protected internal const long serialVersionUID = 1L;


	  protected internal readonly string jobId;
	  protected internal readonly string jobDefinitionId;
	  protected internal readonly int retries;


	  public SetJobRetriesCmd(string jobId, string jobDefinitionId, int retries)
	  {
		if ((string.ReferenceEquals(jobId, null) || jobId.Length == 0) && (string.ReferenceEquals(jobDefinitionId, null) || jobDefinitionId.Length == 0))
		{
		  throw new ProcessEngineException("Either job definition id or job id has to be provided as parameter.");
		}

		if (retries < 0)
		{
		  throw new ProcessEngineException("The number of job retries must be a non-negative Integer, but '" + retries + "' has been provided.");
		}

		this.jobId = jobId;
		this.jobDefinitionId = jobDefinitionId;
		this.retries = retries;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (!string.ReferenceEquals(jobId, null))
		{
		  setJobRetriesByJobId(jobId, retries, commandContext);
		}
		else
		{
		  JobRetriesByJobDefinitionId = commandContext;
		}

		return null;
	  }

	  protected internal virtual CommandContext JobRetriesByJobDefinitionId
	  {
		  set
		  {
			JobDefinitionManager jobDefinitionManager = value.JobDefinitionManager;
			JobDefinitionEntity jobDefinition = jobDefinitionManager.findById(jobDefinitionId);
    
			if (jobDefinition != null)
			{
			  string processDefinitionId = jobDefinition.ProcessDefinitionId;
			  foreach (CommandChecker checker in value.ProcessEngineConfiguration.CommandCheckers)
			  {
				checker.checkUpdateRetriesProcessInstanceByProcessDefinitionId(processDefinitionId);
			  }
			}
    
			value.JobManager.updateFailedJobRetriesByJobDefinitionId(jobDefinitionId, retries);
    
			PropertyChange propertyChange = new PropertyChange(RETRIES, null, retries);
			value.OperationLogManager.logJobOperation(LogEntryOperation, null, jobDefinitionId, null, null, null, propertyChange);
		  }
	  }


	}

}