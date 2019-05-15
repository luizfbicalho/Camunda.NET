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
namespace org.camunda.bpm.engine.rest.dto.runtime
{

	using Incident = org.camunda.bpm.engine.runtime.Incident;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class IncidentDto
	{

	  protected internal string id;
	  protected internal string processDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal DateTime incidentTimestamp;
	  protected internal string incidentType;
	  protected internal string activityId;
	  protected internal string causeIncidentId;
	  protected internal string rootCauseIncidentId;
	  protected internal string configuration;
	  protected internal string incidentMessage;
	  protected internal string tenantId;
	  protected internal string jobDefinitionId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
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

	  public virtual DateTime IncidentTimestamp
	  {
		  get
		  {
			return incidentTimestamp;
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

	  public static IncidentDto fromIncident(Incident incident)
	  {
		IncidentDto dto = new IncidentDto();

		dto.id = incident.Id;
		dto.processDefinitionId = incident.ProcessDefinitionId;
		dto.processInstanceId = incident.ProcessInstanceId;
		dto.executionId = incident.ExecutionId;
		dto.incidentTimestamp = incident.IncidentTimestamp;
		dto.incidentType = incident.IncidentType;
		dto.activityId = incident.ActivityId;
		dto.causeIncidentId = incident.CauseIncidentId;
		dto.rootCauseIncidentId = incident.RootCauseIncidentId;
		dto.configuration = incident.Configuration;
		dto.incidentMessage = incident.IncidentMessage;
		dto.tenantId = incident.TenantId;
		dto.jobDefinitionId = incident.JobDefinitionId;

		return dto;
	  }

	}

}