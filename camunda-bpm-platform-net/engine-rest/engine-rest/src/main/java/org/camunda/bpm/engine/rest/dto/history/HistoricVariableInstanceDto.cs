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
namespace org.camunda.bpm.engine.rest.dto.history
{

	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;

	public class HistoricVariableInstanceDto : VariableValueDto
	{

	  private string id;
	  private string name;
	  private string processDefinitionKey;
	  private string processDefinitionId;
	  private string processInstanceId;
	  private string executionId;
	  private string activityInstanceId;
	  private string caseDefinitionKey;
	  private string caseDefinitionId;
	  private string caseInstanceId;
	  private string caseExecutionId;
	  private string taskId;
	  private string errorMessage;
	  private string tenantId;
	  private string state;
	  private DateTime createTime;
	  private DateTime removalTime;
	  private string rootProcessInstanceId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string State
	  {
		  get
		  {
			return state;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
	  }

	  public static HistoricVariableInstanceDto fromHistoricVariableInstance(HistoricVariableInstance historicVariableInstance)
	  {

		HistoricVariableInstanceDto dto = new HistoricVariableInstanceDto();

		dto.id = historicVariableInstance.Id;
		dto.name = historicVariableInstance.Name;
		dto.processDefinitionKey = historicVariableInstance.ProcessDefinitionKey;
		dto.processDefinitionId = historicVariableInstance.ProcessDefinitionId;
		dto.processInstanceId = historicVariableInstance.ProcessInstanceId;
		dto.executionId = historicVariableInstance.ExecutionId;
		dto.activityInstanceId = historicVariableInstance.ActivityInstanceId;
		dto.caseDefinitionKey = historicVariableInstance.CaseDefinitionKey;
		dto.caseDefinitionId = historicVariableInstance.CaseDefinitionId;
		dto.caseInstanceId = historicVariableInstance.CaseInstanceId;
		dto.caseExecutionId = historicVariableInstance.CaseExecutionId;
		dto.taskId = historicVariableInstance.TaskId;
		dto.tenantId = historicVariableInstance.TenantId;
		dto.state = historicVariableInstance.State;
		dto.createTime = historicVariableInstance.CreateTime;
		dto.removalTime = historicVariableInstance.RemovalTime;
		dto.rootProcessInstanceId = historicVariableInstance.RootProcessInstanceId;

		if (string.ReferenceEquals(historicVariableInstance.ErrorMessage, null))
		{
		  VariableValueDto.fromTypedValue(dto, historicVariableInstance.TypedValue);
		}
		else
		{
		  dto.errorMessage = historicVariableInstance.ErrorMessage;
		  dto.type = VariableValueDto.toRestApiTypeName(historicVariableInstance.TypeName);
		}

		return dto;
	  }

	}

}