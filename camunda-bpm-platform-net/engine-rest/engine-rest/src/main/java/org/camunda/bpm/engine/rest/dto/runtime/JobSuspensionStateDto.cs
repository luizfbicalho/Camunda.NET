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
namespace org.camunda.bpm.engine.rest.dto.runtime
{

	using UpdateJobSuspensionStateBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateBuilder;
	using UpdateJobSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateSelectBuilder;
	using UpdateJobSuspensionStateTenantBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateTenantBuilder;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class JobSuspensionStateDto : SuspensionStateDto
	{

	  private string jobId;
	  private string jobDefinitionId;
	  private string processInstanceId;
	  private string processDefinitionId;
	  private string processDefinitionKey;

	  private string processDefinitionTenantId;
	  private bool processDefinitionWithoutTenantId;

	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
		  set
		  {
			this.jobId = value;
		  }
	  }


	  public virtual string JobDefinitionId
	  {
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
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
		int @params = (!string.ReferenceEquals(jobId, null) ? 1 : 0) + (!string.ReferenceEquals(jobDefinitionId, null) ? 1 : 0) + (!string.ReferenceEquals(processInstanceId, null) ? 1 : 0) + (!string.ReferenceEquals(processDefinitionId, null) ? 1 : 0) + (!string.ReferenceEquals(processDefinitionKey, null) ? 1 : 0);

		if (@params > 1)
		{
		  string message = "Only one of jobId, jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);

		}
		else if (@params == 0)
		{
		  string message = "Either jobId, jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		UpdateJobSuspensionStateBuilder updateSuspensionStateBuilder = createUpdateSuspensionStateBuilder(engine);

		if (Suspended)
		{
		  updateSuspensionStateBuilder.suspend();
		}
		else
		{
		  updateSuspensionStateBuilder.activate();
		}
	  }

	  protected internal virtual UpdateJobSuspensionStateBuilder createUpdateSuspensionStateBuilder(ProcessEngine engine)
	  {
		UpdateJobSuspensionStateSelectBuilder selectBuilder = engine.ManagementService.updateJobSuspensionState();

		if (!string.ReferenceEquals(jobId, null))
		{
		  return selectBuilder.byJobId(jobId);

		}
		else if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  return selectBuilder.byJobDefinitionId(jobDefinitionId);

		}
		else if (!string.ReferenceEquals(processInstanceId, null))
		{
		  return selectBuilder.byProcessInstanceId(processInstanceId);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  return selectBuilder.byProcessDefinitionId(processDefinitionId);

		}
		else
		{
		  UpdateJobSuspensionStateTenantBuilder tenantBuilder = selectBuilder.byProcessDefinitionKey(processDefinitionKey);

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