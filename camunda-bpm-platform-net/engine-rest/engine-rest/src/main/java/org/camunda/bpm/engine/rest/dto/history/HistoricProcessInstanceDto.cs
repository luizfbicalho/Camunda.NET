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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;

	public class HistoricProcessInstanceDto
	{

	  private string id;
	  private string businessKey;
	  private string processDefinitionId;
	  private string processDefinitionKey;
	  private string processDefinitionName;
	  private int? processDefinitionVersion;
	  private DateTime startTime;
	  private DateTime endTime;
	  private DateTime removalTime;
	  private long? durationInMillis;
	  private string startUserId;
	  private string startActivityId;
	  private string deleteReason;
	  private string rootProcessInstanceId;
	  private string superProcessInstanceId;
	  private string superCaseInstanceId;
	  private string caseInstanceId;
	  private string tenantId;
	  private string state;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName;
		  }
	  }

	  public virtual int? ProcessDefinitionVersion
	  {
		  get
		  {
			return processDefinitionVersion;
		  }
	  }

	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
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

	  public virtual string StartUserId
	  {
		  get
		  {
			return startUserId;
		  }
	  }

	  public virtual string StartActivityId
	  {
		  get
		  {
			return startActivityId;
		  }
	  }

	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
	  }

	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
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
		  set
		  {
			this.state = value;
		  }
	  }


	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
		  set
		  {
			this.removalTime = value;
		  }
	  }


	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }


	  public static HistoricProcessInstanceDto fromHistoricProcessInstance(HistoricProcessInstance historicProcessInstance)
	  {

		HistoricProcessInstanceDto dto = new HistoricProcessInstanceDto();

		dto.id = historicProcessInstance.Id;
		dto.businessKey = historicProcessInstance.BusinessKey;
		dto.processDefinitionId = historicProcessInstance.ProcessDefinitionId;
		dto.processDefinitionKey = historicProcessInstance.ProcessDefinitionKey;
		dto.processDefinitionName = historicProcessInstance.ProcessDefinitionName;
		dto.processDefinitionVersion = historicProcessInstance.ProcessDefinitionVersion;
		dto.startTime = historicProcessInstance.StartTime;
		dto.endTime = historicProcessInstance.EndTime;
		dto.removalTime = historicProcessInstance.RemovalTime;
		dto.durationInMillis = historicProcessInstance.DurationInMillis;
		dto.startUserId = historicProcessInstance.StartUserId;
		dto.startActivityId = historicProcessInstance.StartActivityId;
		dto.deleteReason = historicProcessInstance.DeleteReason;
		dto.rootProcessInstanceId = historicProcessInstance.RootProcessInstanceId;
		dto.superProcessInstanceId = historicProcessInstance.SuperProcessInstanceId;
		dto.superCaseInstanceId = historicProcessInstance.SuperCaseInstanceId;
		dto.caseInstanceId = historicProcessInstance.CaseInstanceId;
		dto.tenantId = historicProcessInstance.TenantId;
		dto.state = historicProcessInstance.State;

		return dto;
	  }

	}

}