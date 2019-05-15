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

	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;

	public class HistoricCaseInstanceDto
	{

	  protected internal string id;
	  protected internal string businessKey;
	  protected internal string caseDefinitionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionName;
	  protected internal DateTime createTime;
	  protected internal DateTime closeTime;
	  protected internal long? durationInMillis;
	  protected internal string createUserId;
	  protected internal string superCaseInstanceId;
	  protected internal string superProcessInstanceId;
	  protected internal string tenantId;
	  protected internal bool? active;
	  protected internal bool? completed;
	  protected internal bool? terminated;
	  protected internal bool? closed;

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

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
	  }

	  public virtual string CaseDefinitionName
	  {
		  get
		  {
			return caseDefinitionName;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }

	  public virtual DateTime CloseTime
	  {
		  get
		  {
			return closeTime;
		  }
	  }

	  public virtual long? DurationInMillis
	  {
		  get
		  {
			return durationInMillis;
		  }
	  }

	  public virtual string CreateUserId
	  {
		  get
		  {
			return createUserId;
		  }
	  }

	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
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

	  public virtual bool? Closed
	  {
		  get
		  {
			return closed;
		  }
	  }

	  public static HistoricCaseInstanceDto fromHistoricCaseInstance(HistoricCaseInstance historicCaseInstance)
	  {

		HistoricCaseInstanceDto dto = new HistoricCaseInstanceDto();

		dto.id = historicCaseInstance.Id;
		dto.businessKey = historicCaseInstance.BusinessKey;
		dto.caseDefinitionId = historicCaseInstance.CaseDefinitionId;
		dto.caseDefinitionKey = historicCaseInstance.CaseDefinitionKey;
		dto.caseDefinitionName = historicCaseInstance.CaseDefinitionName;
		dto.createTime = historicCaseInstance.CreateTime;
		dto.closeTime = historicCaseInstance.CloseTime;
		dto.durationInMillis = historicCaseInstance.DurationInMillis;
		dto.createUserId = historicCaseInstance.CreateUserId;
		dto.superCaseInstanceId = historicCaseInstance.SuperCaseInstanceId;
		dto.superProcessInstanceId = historicCaseInstance.SuperProcessInstanceId;
		dto.tenantId = historicCaseInstance.TenantId;
		dto.active = historicCaseInstance.Active;
		dto.completed = historicCaseInstance.Completed;
		dto.terminated = historicCaseInstance.Terminated;
		dto.closed = historicCaseInstance.Closed;

		return dto;
	  }

	}

}