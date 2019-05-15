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
namespace org.camunda.bpm.engine.rest.dto.task
{
	using DelegationStateConverter = org.camunda.bpm.engine.rest.dto.converter.DelegationStateConverter;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;

	public class TaskDto
	{

	  private string id;
	  private string name;
	  private string assignee;
	  private DateTime created;
	  private DateTime due;
	  private DateTime followUp;
	  private string delegationState;
	  private string description;
	  private string executionId;
	  private string owner;
	  private string parentTaskId;
	  private int priority;
	  private string processDefinitionId;
	  private string processInstanceId;
	  private string taskDefinitionKey;
	  private string caseExecutionId;
	  private string caseInstanceId;
	  private string caseDefinitionId;
	  private bool suspended;
	  private string formKey;
	  private string tenantId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
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


	  public virtual DateTime Created
	  {
		  get
		  {
			return created;
		  }
	  }

	  public virtual DateTime Due
	  {
		  get
		  {
			return due;
		  }
		  set
		  {
			this.due = value;
		  }
	  }


	  public virtual string DelegationState
	  {
		  get
		  {
			return delegationState;
		  }
		  set
		  {
			this.delegationState = value;
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


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
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

	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey;
		  }
	  }

	  public virtual DateTime FollowUp
	  {
		  get
		  {
			return followUp;
		  }
		  set
		  {
			this.followUp = value;
		  }
	  }


	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }

	  public virtual string FormKey
	  {
		  get
		  {
			return formKey;
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


	  public static TaskDto fromEntity(Task task)
	  {
		TaskDto dto = new TaskDto();
		dto.id = task.Id;
		dto.name = task.Name;
		dto.assignee = task.Assignee;
		dto.created = task.CreateTime;
		dto.due = task.DueDate;
		dto.followUp = task.FollowUpDate;

		if (task.DelegationState != null)
		{
		  dto.delegationState = task.DelegationState.ToString();
		}

		dto.description = task.Description;
		dto.executionId = task.ExecutionId;
		dto.owner = task.Owner;
		dto.parentTaskId = task.ParentTaskId;
		dto.priority = task.Priority;
		dto.processDefinitionId = task.ProcessDefinitionId;
		dto.processInstanceId = task.ProcessInstanceId;
		dto.taskDefinitionKey = task.TaskDefinitionKey;
		dto.caseDefinitionId = task.CaseDefinitionId;
		dto.caseExecutionId = task.CaseExecutionId;
		dto.caseInstanceId = task.CaseInstanceId;
		dto.suspended = task.Suspended;
		dto.tenantId = task.TenantId;

		try
		{
		  dto.formKey = task.FormKey;
		}
		catch (BadUserRequestException)
		{
		  // ignore (initializeFormKeys was not called)
		}
		return dto;
	  }

	  public virtual void updateTask(Task task)
	  {
		task.Name = Name;
		task.Description = Description;
		task.Priority = Priority;
		task.Assignee = Assignee;
		task.Owner = Owner;

		DelegationState state = null;
		if (!string.ReferenceEquals(DelegationState, null))
		{
		  DelegationStateConverter converter = new DelegationStateConverter();
		  state = converter.convertQueryParameterToType(DelegationState);
		}
		task.DelegationState = state;

		task.DueDate = Due;
		task.FollowUpDate = FollowUp;
		task.ParentTaskId = ParentTaskId;
		task.CaseInstanceId = CaseInstanceId;
		task.TenantId = TenantId;
	  }

	}

}