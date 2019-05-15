using System;
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
namespace org.camunda.bpm.engine.rest.dto.history
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;


	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogEntryDto
	{

	  protected internal string id;
	  protected internal string deploymentId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string caseDefinitionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string taskId;
	  protected internal string jobId;
	  protected internal string jobDefinitionId;
	  protected internal string batchId;
	  protected internal string userId;
	  protected internal DateTime timestamp;
	  protected internal string operationId;
	  protected internal string externalTaskId;
	  protected internal string operationType;
	  protected internal string entityType;
	  protected internal string property;
	  protected internal string orgValue;
	  protected internal string newValue;
	  protected internal DateTime removalTime;
	  protected internal string rootProcessInstanceId;
	  protected internal string category;

	  public static UserOperationLogEntryDto map(UserOperationLogEntry entry)
	  {
		UserOperationLogEntryDto dto = new UserOperationLogEntryDto();

		dto.id = entry.Id;
		dto.deploymentId = entry.DeploymentId;
		dto.processDefinitionId = entry.ProcessDefinitionId;
		dto.processDefinitionKey = entry.ProcessDefinitionKey;
		dto.processInstanceId = entry.ProcessInstanceId;
		dto.executionId = entry.ExecutionId;
		dto.caseDefinitionId = entry.CaseDefinitionId;
		dto.caseInstanceId = entry.CaseInstanceId;
		dto.caseExecutionId = entry.CaseExecutionId;
		dto.taskId = entry.TaskId;
		dto.jobId = entry.JobId;
		dto.jobDefinitionId = entry.JobDefinitionId;
		dto.batchId = entry.BatchId;
		dto.userId = entry.UserId;
		dto.timestamp = entry.Timestamp;
		dto.operationId = entry.OperationId;
		dto.externalTaskId = entry.ExternalTaskId;
		dto.operationType = entry.OperationType;
		dto.entityType = entry.EntityType;
		dto.property = entry.Property;
		dto.orgValue = entry.OrgValue;
		dto.newValue = entry.NewValue;
		dto.removalTime = entry.RemovalTime;
		dto.rootProcessInstanceId = entry.RootProcessInstanceId;
		dto.category = entry.Category;

		return dto;
	  }

	  public static IList<UserOperationLogEntryDto> map(IList<UserOperationLogEntry> entries)
	  {
		IList<UserOperationLogEntryDto> result = new List<UserOperationLogEntryDto>();
		foreach (UserOperationLogEntry entry in entries)
		{
		  result.Add(map(entry));
		}
		return result;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
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

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
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

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
	  }

	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
	  }

	  public virtual string BatchId
	  {
		  get
		  {
			return batchId;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }

	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
	  }

	  public virtual string OperationId
	  {
		  get
		  {
			return operationId;
		  }
	  }

	  public virtual string ExternalTaskId
	  {
		  get
		  {
			return externalTaskId;
		  }
	  }

	  public virtual string OperationType
	  {
		  get
		  {
			return operationType;
		  }
	  }

	  public virtual string EntityType
	  {
		  get
		  {
			return entityType;
		  }
	  }

	  public virtual string Property
	  {
		  get
		  {
			return property;
		  }
	  }

	  public virtual string OrgValue
	  {
		  get
		  {
			return orgValue;
		  }
	  }

	  public virtual string NewValue
	  {
		  get
		  {
			return newValue;
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

	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
	  }

	}

}