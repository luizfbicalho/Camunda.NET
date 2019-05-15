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

	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogQueryDto : AbstractQueryDto<UserOperationLogQuery>
	{

	  public const string TIMESTAMP = "timestamp";

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
	  protected internal string operationId;
	  protected internal string externalTaskId;
	  protected internal string operationType;
	  protected internal string entityType;
	  protected internal string property;
	  protected internal string category;
	  protected internal DateTime afterTimestamp;
	  protected internal DateTime beforeTimestamp;

	  protected internal string[] entityTypes;
	  protected internal string[] categories;

	  public UserOperationLogQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return TIMESTAMP.Equals(value);
	  }

	  protected internal override UserOperationLogQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createUserOperationLogQuery();
	  }

	  protected internal override void applyFilters(UserOperationLogQuery query)
	  {
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  query.caseExecutionId(caseExecutionId);
		}
		if (!string.ReferenceEquals(taskId, null))
		{
		  query.taskId(taskId);
		}
		if (!string.ReferenceEquals(jobId, null))
		{
		  query.jobId(jobId);
		}
		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  query.jobDefinitionId(jobDefinitionId);
		}
		if (!string.ReferenceEquals(batchId, null))
		{
		  query.batchId(batchId);
		}
		if (!string.ReferenceEquals(userId, null))
		{
		  query.userId(userId);
		}
		if (!string.ReferenceEquals(operationId, null))
		{
		  query.operationId(operationId);
		}
		if (!string.ReferenceEquals(externalTaskId, null))
		{
		  query.externalTaskId(externalTaskId);
		}
		if (!string.ReferenceEquals(operationType, null))
		{
		  query.operationType(operationType);
		}
		if (!string.ReferenceEquals(entityType, null))
		{
		  query.entityType(entityType);
		}
		if (entityTypes != null)
		{
		  query.entityTypeIn(entityTypes);
		}
		if (!string.ReferenceEquals(category, null))
		{
		  query.category(category);
		}
		if (categories != null)
		{
		  query.categoryIn(categories);
		}
		if (!string.ReferenceEquals(property, null))
		{
		  query.property(property);
		}
		if (afterTimestamp != null)
		{
		  query.afterTimestamp(afterTimestamp);
		}
		if (beforeTimestamp != null)
		{
		  query.beforeTimestamp(beforeTimestamp);
		}
	  }

	  protected internal override void applySortBy(UserOperationLogQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (TIMESTAMP.Equals(sortBy))
		{
		  query.orderByTimestamp();
		}
	  }

	  [CamundaQueryParam("deploymentId")]
	  public virtual string DeploymentId
	  {
		  set
		  {
			this.deploymentId = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionId")]
	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionKey")]
	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceId")]
	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionId")]
	  public virtual string CaseDefinitionId
	  {
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam("caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("caseExecutionId")]
	  public virtual string CaseExecutionId
	  {
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }

	  [CamundaQueryParam("taskId")]
	  public virtual string TaskId
	  {
		  set
		  {
			this.taskId = value;
		  }
	  }

	  [CamundaQueryParam("jobId")]
	  public virtual string JobId
	  {
		  set
		  {
			this.jobId = value;
		  }
	  }

	  [CamundaQueryParam("jobDefinitionId")]
	  public virtual string JobDefinitionId
	  {
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam("batchId")]
	  public virtual string BatchId
	  {
		  set
		  {
			this.batchId = value;
		  }
	  }

	  [CamundaQueryParam("userId")]
	  public virtual string UserId
	  {
		  set
		  {
			this.userId = value;
		  }
	  }

	  [CamundaQueryParam("operationId")]
	  public virtual string OperationId
	  {
		  set
		  {
			this.operationId = value;
		  }
	  }

	  [CamundaQueryParam("externalTaskId")]
	  public virtual string ExternalTaskId
	  {
		  set
		  {
			this.externalTaskId = value;
		  }
	  }

	  [CamundaQueryParam("operationType")]
	  public virtual string OperationType
	  {
		  set
		  {
			this.operationType = value;
		  }
	  }

	  [CamundaQueryParam("entityType")]
	  public virtual string EntityType
	  {
		  set
		  {
			this.entityType = value;
		  }
	  }

	  [CamundaQueryParam(value : "entityTypeIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] EntityTypeIn
	  {
		  set
		  {
			this.entityTypes = value;
		  }
	  }

	  [CamundaQueryParam("category")]
	  public virtual void setcategory(string category)
	  {
		this.category = category;
	  }

	  [CamundaQueryParam(value : "categoryIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CategoryIn
	  {
		  set
		  {
			this.categories = value;
		  }
	  }

	  [CamundaQueryParam("property")]
	  public virtual string Property
	  {
		  set
		  {
			this.property = value;
		  }
	  }

	  [CamundaQueryParam(value : "afterTimestamp", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime AfterTimestamp
	  {
		  set
		  {
			this.afterTimestamp = value;
		  }
	  }

	  [CamundaQueryParam(value : "beforeTimestamp", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime BeforeTimestamp
	  {
		  set
		  {
			this.beforeTimestamp = value;
		  }
	  }
	}

}