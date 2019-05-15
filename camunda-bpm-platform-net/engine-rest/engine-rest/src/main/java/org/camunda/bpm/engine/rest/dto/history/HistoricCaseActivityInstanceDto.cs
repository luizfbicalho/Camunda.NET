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
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;

	public class HistoricCaseActivityInstanceDto
	{

	  private string id;
	  private string parentCaseActivityInstanceId;
	  private string caseActivityId;
	  private string caseActivityName;
	  private string caseActivityType;
	  private string caseDefinitionId;
	  private string caseInstanceId;
	  private string caseExecutionId;
	  private string taskId;
	  private string calledProcessInstanceId;
	  private string calledCaseInstanceId;
	  private string tenantId;
	  private DateTime createTime;
	  private DateTime endTime;
	  private long? durationInMillis;
	  private bool? required;
	  private bool? available;
	  private bool? enabled;
	  private bool? disabled;
	  private bool? active;
	  private bool? completed;
	  private bool? terminated;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string ParentCaseActivityInstanceId
	  {
		  get
		  {
			return parentCaseActivityInstanceId;
		  }
	  }

	  public virtual string CaseActivityId
	  {
		  get
		  {
			return caseActivityId;
		  }
	  }

	  public virtual string CaseActivityName
	  {
		  get
		  {
			return caseActivityName;
		  }
	  }

	  public virtual string CaseActivityType
	  {
		  get
		  {
			return caseActivityType;
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

	  public virtual string CalledProcessInstanceId
	  {
		  get
		  {
			return calledProcessInstanceId;
		  }
	  }

	  public virtual string CalledCaseInstanceId
	  {
		  get
		  {
			return calledCaseInstanceId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }

	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
	  }

	  public virtual long? DurationInMillis
	  {
		  get
		  {
			return durationInMillis;
		  }
	  }

	  public virtual bool? Required
	  {
		  get
		  {
			return required;
		  }
	  }

	  public virtual bool? Available
	  {
		  get
		  {
			return available;
		  }
	  }

	  public virtual bool? Enabled
	  {
		  get
		  {
			return enabled;
		  }
	  }

	  public virtual bool? Disabled
	  {
		  get
		  {
			return disabled;
		  }
	  }

	  public virtual bool? Active
	  {
		  get
		  {
			return active;
		  }
	  }

	  public virtual bool? Completed
	  {
		  get
		  {
			return completed;
		  }
	  }

	  public virtual bool? Terminated
	  {
		  get
		  {
			return terminated;
		  }
	  }

	  public static HistoricCaseActivityInstanceDto fromHistoricCaseActivityInstance(HistoricCaseActivityInstance historicCaseActivityInstance)
	  {

		HistoricCaseActivityInstanceDto dto = new HistoricCaseActivityInstanceDto();

		dto.id = historicCaseActivityInstance.Id;
		dto.parentCaseActivityInstanceId = historicCaseActivityInstance.ParentCaseActivityInstanceId;
		dto.caseActivityId = historicCaseActivityInstance.CaseActivityId;
		dto.caseActivityName = historicCaseActivityInstance.CaseActivityName;
		dto.caseActivityType = historicCaseActivityInstance.CaseActivityType;
		dto.caseDefinitionId = historicCaseActivityInstance.CaseDefinitionId;
		dto.caseInstanceId = historicCaseActivityInstance.CaseInstanceId;
		dto.caseExecutionId = historicCaseActivityInstance.CaseExecutionId;
		dto.taskId = historicCaseActivityInstance.TaskId;
		dto.calledProcessInstanceId = historicCaseActivityInstance.CalledProcessInstanceId;
		dto.calledCaseInstanceId = historicCaseActivityInstance.CalledCaseInstanceId;
		dto.tenantId = historicCaseActivityInstance.TenantId;
		dto.createTime = historicCaseActivityInstance.CreateTime;
		dto.endTime = historicCaseActivityInstance.EndTime;
		dto.durationInMillis = historicCaseActivityInstance.DurationInMillis;
		dto.required = historicCaseActivityInstance.Required;
		dto.available = historicCaseActivityInstance.Available;
		dto.enabled = historicCaseActivityInstance.Enabled;
		dto.disabled = historicCaseActivityInstance.Disabled;
		dto.active = historicCaseActivityInstance.Active;
		dto.completed = historicCaseActivityInstance.Completed;
		dto.terminated = historicCaseActivityInstance.Terminated;

		return dto;
	  }
	}

}