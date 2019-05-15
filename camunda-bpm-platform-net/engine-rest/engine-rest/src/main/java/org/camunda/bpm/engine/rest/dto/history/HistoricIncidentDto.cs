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

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricIncidentDto
	{

	  protected internal string id;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string rootProcessInstanceId;
	  protected internal DateTime createTime;
	  protected internal DateTime endTime;
	  protected internal DateTime removalTime;
	  protected internal string incidentType;
	  protected internal string activityId;
	  protected internal string causeIncidentId;
	  protected internal string rootCauseIncidentId;
	  protected internal string configuration;
	  protected internal string incidentMessage;
	  protected internal string tenantId;
	  protected internal string jobDefinitionId;
	  protected internal bool? open;
	  protected internal bool? deleted;
	  protected internal bool? resolved;

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

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
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

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string CauseIncidentId
	  {
		  get
		  {
			return causeIncidentId;
		  }
	  }

	  public virtual string RootCauseIncidentId
	  {
		  get
		  {
			return rootCauseIncidentId;
		  }
	  }

	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
	  }

	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
	  }

	  public virtual bool? Open
	  {
		  get
		  {
			return open;
		  }
	  }

	  public virtual bool? Deleted
	  {
		  get
		  {
			return deleted;
		  }
	  }

	  public virtual bool? Resolved
	  {
		  get
		  {
			return resolved;
		  }
	  }

	  public static HistoricIncidentDto fromHistoricIncident(HistoricIncident historicIncident)
	  {
		HistoricIncidentDto dto = new HistoricIncidentDto();

		dto.id = historicIncident.Id;
		dto.processDefinitionKey = historicIncident.ProcessDefinitionKey;
		dto.processDefinitionId = historicIncident.ProcessDefinitionId;
		dto.processInstanceId = historicIncident.ProcessInstanceId;
		dto.executionId = historicIncident.ExecutionId;
		dto.createTime = historicIncident.CreateTime;
		dto.endTime = historicIncident.EndTime;
		dto.incidentType = historicIncident.IncidentType;
		dto.activityId = historicIncident.ActivityId;
		dto.causeIncidentId = historicIncident.CauseIncidentId;
		dto.rootCauseIncidentId = historicIncident.RootCauseIncidentId;
		dto.configuration = historicIncident.Configuration;
		dto.incidentMessage = historicIncident.IncidentMessage;
		dto.open = historicIncident.Open;
		dto.deleted = historicIncident.Deleted;
		dto.resolved = historicIncident.Resolved;
		dto.tenantId = historicIncident.TenantId;
		dto.jobDefinitionId = historicIncident.JobDefinitionId;
		dto.removalTime = historicIncident.RemovalTime;
		dto.rootProcessInstanceId = historicIncident.RootProcessInstanceId;

		return dto;
	  }

	}

}