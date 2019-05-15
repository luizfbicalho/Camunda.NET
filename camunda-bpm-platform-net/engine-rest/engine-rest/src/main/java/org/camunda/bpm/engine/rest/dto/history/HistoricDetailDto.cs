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

	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricFormField = org.camunda.bpm.engine.history.HistoricFormField;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;

	using JsonSubTypes = com.fasterxml.jackson.annotation.JsonSubTypes;
	using Type = com.fasterxml.jackson.annotation.JsonSubTypes.Type;
	using JsonTypeInfo = com.fasterxml.jackson.annotation.JsonTypeInfo;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include=JsonTypeInfo.As.PROPERTY, property="type") @JsonSubTypes({ @Type(value = HistoricFormFieldDto.class), @Type(value = HistoricVariableUpdateDto.class) }) public abstract class HistoricDetailDto
	public abstract class HistoricDetailDto
	{

	  protected internal string id;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string activityInstanceId;
	  protected internal string executionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string taskId;
	  protected internal string tenantId;
	  protected internal string userOperationId;
	  protected internal DateTime time;
	  protected internal DateTime removalTime;
	  protected internal string rootProcessInstanceId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
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

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
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

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string UserOperationId
	  {
		  get
		  {
			return userOperationId;
		  }
	  }

	  public virtual DateTime Time
	  {
		  get
		  {
			return time;
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

	  public static HistoricDetailDto fromHistoricDetail(HistoricDetail historicDetail)
	  {

		HistoricDetailDto dto = null;

		if (historicDetail is HistoricFormField)
		{
		  HistoricFormField historicFormField = (HistoricFormField) historicDetail;
		  dto = HistoricFormFieldDto.fromHistoricFormField(historicFormField);

		}
		else if (historicDetail is HistoricVariableUpdate)
		{
		  HistoricVariableUpdate historicVariableUpdate = (HistoricVariableUpdate) historicDetail;
		  dto = HistoricVariableUpdateDto.fromHistoricVariableUpdate(historicVariableUpdate);
		}

		fromHistoricDetail(historicDetail, dto);
		return dto;
	  }

	  protected internal static void fromHistoricDetail(HistoricDetail historicDetail, HistoricDetailDto dto)
	  {
		dto.id = historicDetail.Id;
		dto.processDefinitionKey = historicDetail.ProcessDefinitionKey;
		dto.processDefinitionId = historicDetail.ProcessDefinitionId;
		dto.processInstanceId = historicDetail.ProcessInstanceId;
		dto.activityInstanceId = historicDetail.ActivityInstanceId;
		dto.executionId = historicDetail.ExecutionId;
		dto.taskId = historicDetail.TaskId;
		dto.caseDefinitionKey = historicDetail.CaseDefinitionKey;
		dto.caseDefinitionId = historicDetail.CaseDefinitionId;
		dto.caseInstanceId = historicDetail.CaseInstanceId;
		dto.caseExecutionId = historicDetail.CaseExecutionId;
		dto.tenantId = historicDetail.TenantId;
		dto.userOperationId = historicDetail.UserOperationId;
		dto.time = historicDetail.Time;
		dto.removalTime = historicDetail.RemovalTime;
		dto.rootProcessInstanceId = historicDetail.RootProcessInstanceId;
	  }


	}

}