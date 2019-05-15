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
namespace org.camunda.bpm.engine.rest.dto.repository
{
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class ProcessDefinitionSuspensionStateDto : SuspensionStateDto
	{

	  private string executionDate;
	  private bool includeProcessInstances;
	  private string processDefinitionId;
	  private string processDefinitionKey;

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ExecutionDate
	  {
		  set
		  {
			this.executionDate = value;
		  }
	  }

	  public virtual bool IncludeProcessInstances
	  {
		  set
		  {
			this.includeProcessInstances = value;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }

	  public override void updateSuspensionState(ProcessEngine engine)
	  {
		if (!string.ReferenceEquals(processDefinitionId, null) && !string.ReferenceEquals(processDefinitionKey, null))
		{
		  string message = "Only one of processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		RepositoryService repositoryService = engine.RepositoryService;

		DateTime delayedExecutionDate = null;
		if (!string.ReferenceEquals(executionDate, null) && !executionDate.Equals(""))
		{
		  delayedExecutionDate = DateTimeUtil.parseDate(executionDate);
		}

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  // activate/suspend process definition by id
		  if (Suspended)
		  {
			repositoryService.suspendProcessDefinitionById(processDefinitionId, includeProcessInstances, delayedExecutionDate);
		  }
		  else
		  {
			repositoryService.activateProcessDefinitionById(processDefinitionId, includeProcessInstances, delayedExecutionDate);
		  }
		}
		else
		{

		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  // activate/suspend process definition by key
		  if (Suspended)
		  {
			repositoryService.suspendProcessDefinitionByKey(processDefinitionKey, includeProcessInstances, delayedExecutionDate);
		  }
		  else
		  {
			repositoryService.activateProcessDefinitionByKey(processDefinitionKey, includeProcessInstances, delayedExecutionDate);
		  }
		}
		else
		{
		  string message = "Either processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}
		}
	  }

	}

}