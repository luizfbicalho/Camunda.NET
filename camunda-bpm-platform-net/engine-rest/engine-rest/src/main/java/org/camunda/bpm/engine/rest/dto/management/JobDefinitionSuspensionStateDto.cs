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
namespace org.camunda.bpm.engine.rest.dto.management
{

	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using UpdateJobDefinitionSuspensionStateBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateBuilder;
	using UpdateJobDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateSelectBuilder;
	using UpdateJobDefinitionSuspensionStateTenantBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateTenantBuilder;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class JobDefinitionSuspensionStateDto : SuspensionStateDto
	{

	  private string executionDate;
	  private bool includeJobs;
	  private string jobDefinitionId;
	  private string processDefinitionId;
	  private string processDefinitionKey;

	  private string processDefinitionTenantId;
	  private bool processDefinitionWithoutTenantId;

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }


	  public virtual string ExecutionDate
	  {
		  set
		  {
			this.executionDate = value;
		  }
	  }

	  public virtual bool IncludeJobs
	  {
		  set
		  {
			this.includeJobs = value;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }

	  public virtual string ProcessDefinitionTenantId
	  {
		  set
		  {
			this.processDefinitionTenantId = value;
		  }
	  }

	  public virtual bool ProcessDefinitionWithoutTenantId
	  {
		  set
		  {
			this.processDefinitionWithoutTenantId = value;
		  }
	  }

	  public override void updateSuspensionState(ProcessEngine engine)
	  {
		int @params = (!string.ReferenceEquals(jobDefinitionId, null) ? 1 : 0) + (!string.ReferenceEquals(processDefinitionId, null) ? 1 : 0) + (!string.ReferenceEquals(processDefinitionKey, null) ? 1 : 0);

		if (@params > 1)
		{
		  string message = "Only one of jobDefinitionId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);

		}
		else if (@params == 0)
		{
		  string message = "Either jobDefinitionId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		UpdateJobDefinitionSuspensionStateBuilder updateSuspensionStateBuilder = createUpdateSuspensionStateBuilder(engine);

		if (!string.ReferenceEquals(executionDate, null) && !executionDate.Equals(""))
		{
		  DateTime delayedExecutionDate = DateTimeUtil.parseDate(executionDate);

		  updateSuspensionStateBuilder.executionDate(delayedExecutionDate);
		}

		updateSuspensionStateBuilder.includeJobs(includeJobs);

		if (Suspended)
		{
		  updateSuspensionStateBuilder.suspend();
		}
		else
		{
		  updateSuspensionStateBuilder.activate();
		}
	  }

	  protected internal virtual UpdateJobDefinitionSuspensionStateBuilder createUpdateSuspensionStateBuilder(ProcessEngine engine)
	  {
		UpdateJobDefinitionSuspensionStateSelectBuilder selectBuilder = engine.ManagementService.updateJobDefinitionSuspensionState();

		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  return selectBuilder.byJobDefinitionId(jobDefinitionId);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  return selectBuilder.byProcessDefinitionId(processDefinitionId);

		}
		else
		{
		  UpdateJobDefinitionSuspensionStateTenantBuilder tenantBuilder = selectBuilder.byProcessDefinitionKey(processDefinitionKey);

		  if (!string.ReferenceEquals(processDefinitionTenantId, null))
		  {
			tenantBuilder.processDefinitionTenantId(processDefinitionTenantId);

		  }
		  else if (processDefinitionWithoutTenantId)
		  {
			tenantBuilder.processDefinitionWithoutTenantId();
		  }

		  return tenantBuilder;
		}
	  }

	}

}