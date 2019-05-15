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
	using ActivityInstanceState = org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState;


	/// <summary>
	/// <para><seealso cref="HistoryEvent"/> implementation for events that happen in an activity.</para>
	/// 
	/// @author Daniel Meyer
	/// @author Marcel Wieczorek
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class HistoricActivityInstanceEventEntity : HistoricScopeInstanceEvent
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// the id of the activity </summary>
	  protected internal string activityId;

	  /// <summary>
	  /// the name of the activity </summary>
	  protected internal string activityName;

	  /// <summary>
	  /// the type of the activity (startEvent, serviceTask ...) </summary>
	  protected internal string activityType;

	  /// <summary>
	  /// the id of this activity instance </summary>
	  protected internal string activityInstanceId;

	  /// <summary>
	  /// the state of this activity instance </summary>
	  protected internal int activityInstanceState;

	  /// <summary>
	  /// the id of the parent activity instance </summary>
	  protected internal string parentActivityInstanceId;

	  /// <summary>
	  /// the id of the child process instance </summary>
	  protected internal string calledProcessInstanceId;

	  /// <summary>
	  /// the id of the child case instance </summary>
	  protected internal string calledCaseInstanceId;

	  protected internal string taskId;
	  protected internal string taskAssignee;

	  /// <summary>
	  /// id of the tenant which belongs to the activity instance </summary>
	  protected internal string tenantId;

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Assignee
	  {
		  get
		  {
			return taskAssignee;
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


	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
		  set
		  {
			this.activityType = value;
		  }
	  }


	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
		  set
		  {
			this.activityName = value;
		  }
	  }


	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }


	  public virtual string ParentActivityInstanceId
	  {
		  get
		  {
			return parentActivityInstanceId;
		  }
		  set
		  {
			this.parentActivityInstanceId = value;
		  }
	  }


	  public virtual string CalledProcessInstanceId
	  {
		  get
		  {
			return calledProcessInstanceId;
		  }
		  set
		  {
			this.calledProcessInstanceId = value;
		  }
	  }


	  public virtual string CalledCaseInstanceId
	  {
		  get
		  {
			return calledCaseInstanceId;
		  }
		  set
		  {
			this.calledCaseInstanceId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string TaskAssignee
	  {
		  get
		  {
			return taskAssignee;
		  }
		  set
		  {
			this.taskAssignee = value;
		  }
	  }


	  public virtual int ActivityInstanceState
	  {
		  set
		  {
			this.activityInstanceState = value;
		  }
		  get
		  {
			return activityInstanceState;
		  }
	  }


	  public virtual bool CompleteScope
	  {
		  get
		  {
			return org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.SCOPE_COMPLETE.StateCode == activityInstanceState;
		  }
	  }

	  public virtual bool Canceled
	  {
		  get
		  {
			return org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.CANCELED.StateCode == activityInstanceState;
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


	  public override string ToString()
	  {
		return this.GetType().Name + "[activityId=" + activityId + ", activityName=" + activityName + ", activityType=" + activityType + ", activityInstanceId=" + activityInstanceId + ", activityInstanceState=" + activityInstanceState + ", parentActivityInstanceId=" + parentActivityInstanceId + ", calledProcessInstanceId=" + calledProcessInstanceId + ", calledCaseInstanceId=" + calledCaseInstanceId + ", taskId=" + taskId + ", taskAssignee=" + taskAssignee + ", durationInMillis=" + durationInMillis + ", startTime=" + startTime + ", endTime=" + endTime + ", eventType=" + eventType + ", executionId=" + executionId + ", processDefinitionId=" + processDefinitionId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", processInstanceId=" + processInstanceId + ", tenantId=" + tenantId + "]";
	  }
	}

}