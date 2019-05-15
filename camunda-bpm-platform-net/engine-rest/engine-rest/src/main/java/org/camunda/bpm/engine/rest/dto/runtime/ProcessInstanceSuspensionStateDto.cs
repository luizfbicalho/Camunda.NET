using System.Collections.Generic;

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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using UpdateProcessInstanceSuspensionStateBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateBuilder;
	using UpdateProcessInstanceSuspensionStateSelectBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateSelectBuilder;
	using UpdateProcessInstanceSuspensionStateTenantBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateTenantBuilder;
	using UpdateProcessInstancesSuspensionStateBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstancesSuspensionStateBuilder;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class ProcessInstanceSuspensionStateDto : SuspensionStateDto
	{

	  private string processInstanceId;
	  private string processDefinitionId;
	  private string processDefinitionKey;

	  private IList<string> processInstanceIds;
	  private ProcessInstanceQueryDto processInstanceQuery;
	  private HistoricProcessInstanceQueryDto historicProcessInstanceQuery;

	  private string processDefinitionTenantId;
	  private bool processDefinitionWithoutTenantId;

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
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

	  public virtual IList<string> ProcessInstanceIds
	  {
		  set
		  {
			this.processInstanceIds = value;
		  }
	  }

	  public virtual ProcessInstanceQueryDto ProcessInstanceQuery
	  {
		  set
		  {
			this.processInstanceQuery = value;
		  }
		  get
		  {
			return processInstanceQuery;
		  }
	  }


	  public virtual HistoricProcessInstanceQueryDto HistoricProcessInstanceQuery
	  {
		  set
		  {
			this.historicProcessInstanceQuery = value;
		  }
		  get
		  {
			return historicProcessInstanceQuery;
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
		int @params = parameterCount(processInstanceId, processDefinitionId, processDefinitionKey);
		int syncParams = parameterCount(processInstanceIds, processInstanceQuery, historicProcessInstanceQuery);

		if (@params >= 1 && syncParams >= 1)
		{
		  string message = "Choose either a single processInstance with processInstanceId, processDefinitionId or processDefinitionKey or a group of processInstances with processInstanceIds, procesInstanceQuery or historicProcessInstanceQuery.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}
		else if (@params > 1)
		{
		  string message = "Only one of processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}
		else if (@params == 0 && syncParams == 0)
		{
		  string message = "Either processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		UpdateProcessInstanceSuspensionStateBuilder updateSuspensionStateBuilder = null;
		if (@params == 1)
		{
		  updateSuspensionStateBuilder = createUpdateSuspensionStateBuilder(engine);
		}
		else if (syncParams >= 1)
		{
		  updateSuspensionStateBuilder = createUpdateSuspensionStateGroupBuilder(engine);
		}

		if (Suspended)
		{
		  updateSuspensionStateBuilder.suspend();
		}
		else
		{
		  updateSuspensionStateBuilder.activate();
		}
	  }

	  public virtual Batch updateSuspensionStateAsync(ProcessEngine engine)
	  {

		int @params = parameterCount(processInstanceIds, processInstanceQuery, historicProcessInstanceQuery);

		if (@params == 0)
		{
		   string message = "Either processInstanceIds, processInstanceQuery or historicProcessInstanceQuery should be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		UpdateProcessInstancesSuspensionStateBuilder updateSuspensionStateBuilder = createUpdateSuspensionStateGroupBuilder(engine);
		if (Suspended)
		{
		  return updateSuspensionStateBuilder.suspendAsync();
		}
		else
		{
		  return updateSuspensionStateBuilder.activateAsync();
		}
	  }

	  protected internal virtual UpdateProcessInstanceSuspensionStateBuilder createUpdateSuspensionStateBuilder(ProcessEngine engine)
	  {
		UpdateProcessInstanceSuspensionStateSelectBuilder selectBuilder = engine.RuntimeService.updateProcessInstanceSuspensionState();

		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  return selectBuilder.byProcessInstanceId(processInstanceId);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  return selectBuilder.byProcessDefinitionId(processDefinitionId);

		}
		else
		{ //processDefinitionKey != null
		  UpdateProcessInstanceSuspensionStateTenantBuilder tenantBuilder = selectBuilder.byProcessDefinitionKey(processDefinitionKey);

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

	  protected internal virtual UpdateProcessInstancesSuspensionStateBuilder createUpdateSuspensionStateGroupBuilder(ProcessEngine engine)
	  {
		UpdateProcessInstanceSuspensionStateSelectBuilder selectBuilder = engine.RuntimeService.updateProcessInstanceSuspensionState();
		UpdateProcessInstancesSuspensionStateBuilder groupBuilder = null;
		if (processInstanceIds != null)
		{
		  groupBuilder = selectBuilder.byProcessInstanceIds(processInstanceIds);
		}

		if (processInstanceQuery != null)
		{
		  if (groupBuilder == null)
		  {
			groupBuilder = selectBuilder.byProcessInstanceQuery(processInstanceQuery.toQuery(engine));
		  }
		  else
		  {
			groupBuilder.byProcessInstanceQuery(processInstanceQuery.toQuery(engine));
		  }
		}

		if (historicProcessInstanceQuery != null)
		{
		  if (groupBuilder == null)
		  {
			groupBuilder = selectBuilder.byHistoricProcessInstanceQuery(historicProcessInstanceQuery.toQuery(engine));
		  }
		  else
		  {
			groupBuilder.byHistoricProcessInstanceQuery(historicProcessInstanceQuery.toQuery(engine));
		  }
		}

		return groupBuilder;
	  }



	  protected internal virtual int parameterCount(params object[] o)
	  {
		int count = 0;
		foreach (object o1 in o)
		{
		  count += (o1 != null ? 1 : 0);
		}
		return count;
	  }

	}

}