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

	/// <summary>
	/// <para><seealso cref="HistoryEvent"/> signifying a top-level event in a process instance.</para>
	/// 
	/// @author Daniel Meyer
	/// @author Marcel Wieczorek
	/// 
	/// </summary>
	[Serializable]
	public class HistoricProcessInstanceEventEntity : HistoricScopeInstanceEvent
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// the business key of the process instance </summary>
	  protected internal string businessKey;

	  /// <summary>
	  /// the id of the user that started the process instance </summary>
	  protected internal string startUserId;

	  /// <summary>
	  /// the id of the super process instance </summary>
	  protected internal string superProcessInstanceId;

	  /// <summary>
	  /// the id of the super case instance </summary>
	  protected internal string superCaseInstanceId;

	  /// <summary>
	  /// the reason why this process instance was cancelled (deleted) </summary>
	  protected internal string deleteReason;

	  /// <summary>
	  /// id of the activity which ended the process instance </summary>
	  protected internal string endActivityId;

	  /// <summary>
	  /// id of the activity which started the process instance </summary>
	  protected internal string startActivityId;

	  /// <summary>
	  /// id of the tenant which belongs to the process instance </summary>
	  protected internal string tenantId;

	  protected internal string state;

	  // getters / setters ////////////////////////////////////////

	  public virtual string EndActivityId
	  {
		  get
		  {
			return endActivityId;
		  }
		  set
		  {
			this.endActivityId = value;
		  }
	  }


	  public virtual string StartActivityId
	  {
		  get
		  {
			return startActivityId;
		  }
		  set
		  {
			this.startActivityId = value;
		  }
	  }


	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual string StartUserId
	  {
		  get
		  {
			return startUserId;
		  }
		  set
		  {
			this.startUserId = value;
		  }
	  }


	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
		  set
		  {
			this.superProcessInstanceId = value;
		  }
	  }


	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId;
		  }
		  set
		  {
			this.superCaseInstanceId = value;
		  }
	  }


	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
		  set
		  {
			this.deleteReason = value;
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


	  public override string ToString()
	  {
		return this.GetType().Name + "[businessKey=" + businessKey + ", startUserId=" + startUserId + ", superProcessInstanceId=" + superProcessInstanceId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", superCaseInstanceId=" + superCaseInstanceId + ", deleteReason=" + deleteReason + ", durationInMillis=" + durationInMillis + ", startTime=" + startTime + ", endTime=" + endTime + ", removalTime=" + removalTime + ", endActivityId=" + endActivityId + ", startActivityId=" + startActivityId + ", id=" + id + ", eventType=" + eventType + ", executionId=" + executionId + ", processDefinitionId=" + processDefinitionId + ", processInstanceId=" + processInstanceId + ", tenantId=" + tenantId + "]";
	  }

	}

}