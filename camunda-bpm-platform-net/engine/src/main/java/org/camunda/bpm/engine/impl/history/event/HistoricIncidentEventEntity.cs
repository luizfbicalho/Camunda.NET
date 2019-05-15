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
namespace org.camunda.bpm.engine.impl.history.@event
{

	using IncidentState = org.camunda.bpm.engine.history.IncidentState;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class HistoricIncidentEventEntity : HistoryEvent
	{

	  private const long serialVersionUID = 1L;

	  protected internal DateTime createTime;
	  protected internal DateTime endTime;
	  protected internal string incidentType;
	  protected internal string activityId;
	  protected internal string causeIncidentId;
	  protected internal string rootCauseIncidentId;
	  protected internal string configuration;
	  protected internal string incidentMessage;
	  protected internal int incidentState;
	  protected internal string tenantId;
	  protected internal string jobDefinitionId;

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
		  set
		  {
			this.createTime = value;
		  }
	  }


	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
		  set
		  {
			this.endTime = value;
		  }
	  }


	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType;
		  }
		  set
		  {
			this.incidentType = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string CauseIncidentId
	  {
		  get
		  {
			return causeIncidentId;
		  }
		  set
		  {
			this.causeIncidentId = value;
		  }
	  }


	  public virtual string RootCauseIncidentId
	  {
		  get
		  {
			return rootCauseIncidentId;
		  }
		  set
		  {
			this.rootCauseIncidentId = value;
		  }
	  }


	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage;
		  }
		  set
		  {
			this.incidentMessage = value;
		  }
	  }


	  public virtual int IncidentState
	  {
		  set
		  {
			this.incidentState = value;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


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


	  public virtual bool Open
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.IncidentState_Fields.DEFAULT.StateCode == incidentState;
		  }
	  }

	  public virtual bool Deleted
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.IncidentState_Fields.DELETED.StateCode == incidentState;
		  }
	  }

	  public virtual bool Resolved
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.IncidentState_Fields.RESOLVED.StateCode == incidentState;
		  }
	  }

	  public override string RootProcessInstanceId
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


	}

}