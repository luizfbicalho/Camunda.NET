﻿using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.dto.externaltask
{

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Askar Akhmerov
	/// </summary>
	public class LockedExternalTaskDto
	{

	  protected internal string activityId;
	  protected internal string activityInstanceId;
	  protected internal string errorMessage;
	  protected internal string errorDetails;
	  protected internal string executionId;
	  protected internal string id;
	  protected internal DateTime lockExpirationTime;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processInstanceId;
	  protected internal int? retries;
	  protected internal bool suspended;
	  protected internal string workerId;
	  protected internal string topicName;
	  protected internal string tenantId;
	  protected internal IDictionary<string, VariableValueDto> variables;
	  protected internal long priority;
	  protected internal string businessKey;

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }
	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }
	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }
	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual DateTime LockExpirationTime
	  {
		  get
		  {
			return lockExpirationTime;
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
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }
	  public virtual int? Retries
	  {
		  get
		  {
			return retries;
		  }
	  }
	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }
	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId;
		  }
	  }
	  public virtual string TopicName
	  {
		  get
		  {
			return topicName;
		  }
	  }
	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }
	  public virtual IDictionary<string, VariableValueDto> Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual long Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual string ErrorDetails
	  {
		  get
		  {
			return errorDetails;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public static LockedExternalTaskDto fromLockedExternalTask(LockedExternalTask task)
	  {
		LockedExternalTaskDto dto = new LockedExternalTaskDto();
		dto.activityId = task.ActivityId;
		dto.activityInstanceId = task.ActivityInstanceId;
		dto.errorMessage = task.ErrorMessage;
		dto.errorDetails = task.ErrorDetails;
		dto.executionId = task.ExecutionId;
		dto.id = task.Id;
		dto.lockExpirationTime = task.LockExpirationTime;
		dto.processDefinitionId = task.ProcessDefinitionId;
		dto.processDefinitionKey = task.ProcessDefinitionKey;
		dto.processInstanceId = task.ProcessInstanceId;
		dto.retries = task.Retries;
		dto.topicName = task.TopicName;
		dto.workerId = task.WorkerId;
		dto.tenantId = task.TenantId;
		dto.variables = VariableValueDto.fromMap(task.Variables);
		dto.priority = task.Priority;
		dto.businessKey = task.BusinessKey;

		return dto;
	  }

	  public static IList<LockedExternalTaskDto> fromLockedExternalTasks(IList<LockedExternalTask> tasks)
	  {
		IList<LockedExternalTaskDto> dtos = new List<LockedExternalTaskDto>();

		foreach (LockedExternalTask task in tasks)
		{
		  dtos.Add(LockedExternalTaskDto.fromLockedExternalTask(task));
		}

		return dtos;
	  }
	  public override string ToString()
	  {
		return "LockedExternalTaskDto [activityId=" + activityId + ", activityInstanceId=" + activityInstanceId + ", errorMessage=" + errorMessage + ", errorDetails=" + errorDetails + ", executionId=" + executionId + ", id=" + id + ", lockExpirationTime=" + lockExpirationTime + ", processDefinitionId=" + processDefinitionId + ", processDefinitionKey=" + processDefinitionKey + ", processInstanceId=" + processInstanceId + ", retries=" + retries + ", suspended=" + suspended + ", workerId=" + workerId + ", topicName=" + topicName + ", tenantId=" + tenantId + ", variables=" + variables + ", priority=" + priority + ", businessKey=" + businessKey + "]";
	  }

	}

}