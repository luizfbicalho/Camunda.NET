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
	/// @author Marcel Wieczorek
	/// </summary>
	[Serializable]
	public class HistoricTaskInstanceEventEntity : HistoricScopeInstanceEvent
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;
	  protected internal string assignee;
	  protected internal string owner;
	  protected internal string name;
	  protected internal string description;
	  protected internal DateTime dueDate;
	  protected internal DateTime followUpDate;
	  protected internal int priority;
	  protected internal string parentTaskId;
	  protected internal string deleteReason;
	  protected internal string taskDefinitionKey;
	  protected internal string activityInstanceId;
	  protected internal string tenantId;

	  // getters and setters //////////////////////////////////////////////////////

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

	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
		  set
		  {
			this.assignee = value;
		  }
	  }


	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
		  set
		  {
			this.owner = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
		  set
		  {
			this.description = value;
		  }
	  }


	  public virtual DateTime DueDate
	  {
		  get
		  {
			return dueDate;
		  }
		  set
		  {
			this.dueDate = value;
		  }
	  }


	  public virtual DateTime FollowUpDate
	  {
		  get
		  {
			return followUpDate;
		  }
		  set
		  {
			this.followUpDate = value;
		  }
	  }


	  public virtual int Priority
	  {
		  get
		  {
			return priority;
		  }
		  set
		  {
			this.priority = value;
		  }
	  }


	  public virtual string ParentTaskId
	  {
		  get
		  {
			return parentTaskId;
		  }
		  set
		  {
			this.parentTaskId = value;
		  }
	  }



	  public virtual string TaskId
	  {
		  set
		  {
			this.taskId = value;
		  }
		  get
		  {
			return taskId;
		  }
	  }


	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey;
		  }
		  set
		  {
			this.taskDefinitionKey = value;
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
		return this.GetType().Name + "[taskId" + taskId + ", assignee=" + assignee + ", owner=" + owner + ", name=" + name + ", description=" + description + ", dueDate=" + dueDate + ", followUpDate=" + followUpDate + ", priority=" + priority + ", parentTaskId=" + parentTaskId + ", deleteReason=" + deleteReason + ", taskDefinitionKey=" + taskDefinitionKey + ", durationInMillis=" + durationInMillis + ", startTime=" + startTime + ", endTime=" + endTime + ", id=" + id + ", eventType=" + eventType + ", executionId=" + executionId + ", processDefinitionId=" + processDefinitionId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", processInstanceId=" + processInstanceId + ", activityInstanceId=" + activityInstanceId + ", tenantId=" + tenantId + "]";
	  }
	}

}