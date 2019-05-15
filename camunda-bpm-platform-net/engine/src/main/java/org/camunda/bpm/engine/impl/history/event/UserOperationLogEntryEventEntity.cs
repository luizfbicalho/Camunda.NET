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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	[Serializable]
	public class UserOperationLogEntryEventEntity : HistoryEvent, UserOperationLogEntry
	{

	  private const long serialVersionUID = 1L;

	  protected internal string operationId;
	  protected internal string operationType;
	  protected internal string jobId;
	  protected internal string jobDefinitionId;
	  protected internal string taskId;
	  protected internal string userId;
	  protected internal DateTime timestamp;
	  protected internal string property;
	  protected internal string orgValue;
	  protected internal string newValue;
	  protected internal string entityType;
	  protected internal string deploymentId;
	  protected internal string tenantId;
	  protected internal string batchId;
	  protected internal string category;
	  protected internal string externalTaskId;

	  public virtual string OperationId
	  {
		  get
		  {
			return operationId;
		  }
		  set
		  {
			this.operationId = value;
		  }
	  }

	  public virtual string OperationType
	  {
		  get
		  {
			return operationType;
		  }
		  set
		  {
			this.operationType = value;
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

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			this.userId = value;
		  }
	  }

	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
		  set
		  {
			this.timestamp = value;
		  }
	  }

	  public virtual string Property
	  {
		  get
		  {
			return property;
		  }
		  set
		  {
			this.property = value;
		  }
	  }

	  public virtual string OrgValue
	  {
		  get
		  {
			return orgValue;
		  }
		  set
		  {
			this.orgValue = value;
		  }
	  }

	  public virtual string NewValue
	  {
		  get
		  {
			return newValue;
		  }
		  set
		  {
			this.newValue = value;
		  }
	  }









	  public virtual string EntityType
	  {
		  get
		  {
			return entityType;
		  }
		  set
		  {
			this.entityType = value;
		  }
	  }


	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
		  set
		  {
			this.jobId = value;
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


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
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


	  public virtual string BatchId
	  {
		  get
		  {
			return batchId;
		  }
		  set
		  {
			this.batchId = value;
		  }
	  }


	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
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


	  public virtual string ExternalTaskId
	  {
		  get
		  {
			return externalTaskId;
		  }
		  set
		  {
			this.externalTaskId = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[taskId" + taskId + ", deploymentId" + deploymentId + ", processDefinitionKey =" + processDefinitionKey + ", jobId = " + jobId + ", jobDefinitionId = " + jobDefinitionId + ", batchId = " + batchId + ", operationId =" + operationId + ", operationType =" + operationType + ", userId =" + userId + ", timestamp =" + timestamp + ", property =" + property + ", orgValue =" + orgValue + ", newValue =" + newValue + ", id=" + id + ", eventType=" + eventType + ", executionId=" + executionId + ", processDefinitionId=" + processDefinitionId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", processInstanceId=" + processInstanceId + ", externalTaskId=" + externalTaskId + ", tenantId=" + tenantId + ", entityType=" + entityType + ", category=" + category + "]";
	  }
	}

}