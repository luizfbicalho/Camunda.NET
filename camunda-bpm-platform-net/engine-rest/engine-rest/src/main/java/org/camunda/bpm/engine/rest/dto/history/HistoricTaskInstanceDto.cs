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

	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricTaskInstanceDto
	{

	  protected internal string id;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string activityInstanceId;
	  protected internal string name;
	  protected internal string description;
	  protected internal string deleteReason;
	  protected internal string owner;
	  protected internal string assignee;
	  protected internal DateTime startTime;
	  protected internal DateTime endTime;
	  protected internal long? duration;
	  protected internal string taskDefinitionKey;
	  protected internal int priority;
	  protected internal DateTime due;
	  protected internal string parentTaskId;
	  protected internal DateTime followUp;
	  private string tenantId;
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

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
	  }

	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
	  }

	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
	  }

	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
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

	  public virtual long? Duration
	  {
		  get
		  {
			return duration;
		  }
	  }

	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey;
		  }
	  }

	  public virtual int Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual DateTime Due
	  {
		  get
		  {
			return due;
		  }
	  }

	  public virtual string ParentTaskId
	  {
		  get
		  {
			return parentTaskId;
		  }
	  }

	  public virtual DateTime FollowUp
	  {
		  get
		  {
			return followUp;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
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

	  public static HistoricTaskInstanceDto fromHistoricTaskInstance(HistoricTaskInstance taskInstance)
	  {

		HistoricTaskInstanceDto dto = new HistoricTaskInstanceDto();

		dto.id = taskInstance.Id;
		dto.processDefinitionKey = taskInstance.ProcessDefinitionKey;
		dto.processDefinitionId = taskInstance.ProcessDefinitionId;
		dto.processInstanceId = taskInstance.ProcessInstanceId;
		dto.executionId = taskInstance.ExecutionId;
		dto.caseDefinitionKey = taskInstance.CaseDefinitionKey;
		dto.caseDefinitionId = taskInstance.CaseDefinitionId;
		dto.caseInstanceId = taskInstance.CaseInstanceId;
		dto.caseExecutionId = taskInstance.CaseExecutionId;
		dto.activityInstanceId = taskInstance.ActivityInstanceId;
		dto.name = taskInstance.Name;
		dto.description = taskInstance.Description;
		dto.deleteReason = taskInstance.DeleteReason;
		dto.owner = taskInstance.Owner;
		dto.assignee = taskInstance.Assignee;
		dto.startTime = taskInstance.StartTime;
		dto.endTime = taskInstance.EndTime;
		dto.duration = taskInstance.DurationInMillis;
		dto.taskDefinitionKey = taskInstance.TaskDefinitionKey;
		dto.priority = taskInstance.Priority;
		dto.due = taskInstance.DueDate;
		dto.parentTaskId = taskInstance.ParentTaskId;
		dto.followUp = taskInstance.FollowUpDate;
		dto.tenantId = taskInstance.TenantId;
		dto.removalTime = taskInstance.RemovalTime;
		dto.rootProcessInstanceId = taskInstance.RootProcessInstanceId;

		return dto;
	  }

	}

}