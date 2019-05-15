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

	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class HistoricIdentityLinkLogQueryDto : AbstractQueryDto<HistoricIdentityLinkLogQuery>
	{

	  private const string SORT_BY_TIME = "time";
	  private const string SORT_BY_TYPE = "type";
	  private const string SORT_BY_USER_ID = "userId";
	  private const string SORT_BY_GROUP_ID = "groupId";
	  private const string SORT_BY_TASK_ID = "taskId";
	  private const string SORT_BY_PROCESS_DEFINITION_ID = "processDefinitionId";
	  private const string SORT_BY_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  private const string SORT_BY_OPERATION_TYPE = "operationType";
	  private const string SORT_BY_ASSIGNER_ID = "assignerId";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricIdentityLinkLogQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_TIME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TYPE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_USER_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_GROUP_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TASK_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_OPERATION_TYPE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ASSIGNER_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal DateTime dateBefore;
	  protected internal DateTime dateAfter;
	  protected internal string type;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal string taskId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string operationType;
	  protected internal string assignerId;
	  protected internal IList<string> tenantIds;

	  public HistoricIdentityLinkLogQueryDto()
	  {
	  }

	  public HistoricIdentityLinkLogQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricIdentityLinkLogQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricIdentityLinkLogQuery();
	  }

	  [CamundaQueryParam("type")]
	  public virtual string Type
	  {
		  set
		  {
			this.type = value;
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

	  [CamundaQueryParam("groupId")]
	  public virtual string GroupId
	  {
		  set
		  {
			this.groupId = value;
		  }
	  }

	  [CamundaQueryParam(value : "dateBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DateBefore
	  {
		  set
		  {
			this.dateBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "dateAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DateAfter
	  {
		  set
		  {
			this.dateAfter = value;
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

	  [CamundaQueryParam("operationType")]
	  public virtual string OperationType
	  {
		  set
		  {
			this.operationType = value;
		  }
	  }

	  [CamundaQueryParam("assignerId")]
	  public virtual string AssignerId
	  {
		  set
		  {
			this.assignerId = value;
		  }
	  }

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> TenantIdIn
	  {
		  set
		  {
			this.tenantIds = value;
		  }
	  }

	  protected internal override void applyFilters(HistoricIdentityLinkLogQuery query)
	  {
		if (dateBefore != null)
		{
		  query.dateBefore(dateBefore);
		}
		if (dateAfter != null)
		{
		  query.dateAfter(dateAfter);
		}
		if (!string.ReferenceEquals(type, null))
		{
		  query.type(type);
		}
		if (!string.ReferenceEquals(userId, null))
		{
		  query.userId(userId);
		}
		if (!string.ReferenceEquals(groupId, null))
		{
		  query.groupId(groupId);
		}
		if (!string.ReferenceEquals(taskId, null))
		{
		  query.taskId(taskId);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(operationType, null))
		{
		  query.operationType(operationType);
		}
		if (!string.ReferenceEquals(assignerId, null))
		{
		  query.assignerId(assignerId);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(HistoricIdentityLinkLogQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_TIME))
		{
		  query.orderByTime();
		}
		else if (sortBy.Equals(SORT_BY_TYPE))
		{
		  query.orderByType();
		}
		else if (sortBy.Equals(SORT_BY_USER_ID))
		{
		  query.orderByUserId();
		}
		else if (sortBy.Equals(SORT_BY_GROUP_ID))
		{
		  query.orderByGroupId();
		}
		else if (sortBy.Equals(SORT_BY_TASK_ID))
		{
		  query.orderByTaskId();
		}
		else if (sortBy.Equals(SORT_BY_OPERATION_TYPE))
		{
		  query.orderByOperationType();
		}
		else if (sortBy.Equals(SORT_BY_ASSIGNER_ID))
		{
		  query.orderByAssignerId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_ID))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_KEY))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }
	}

}