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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using LongConverter = org.camunda.bpm.engine.rest.dto.converter.LongConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;


	public class HistoricExternalTaskLogQueryDto : AbstractQueryDto<HistoricExternalTaskLogQuery>
	{

	  protected internal const string SORT_BY_TIMESTAMP = "timestamp";
	  protected internal const string SORT_BY_EXTERNAL_TASK_ID = "externalTaskId";
	  protected internal const string SORT_BY_RETRIES = "retries";
	  protected internal const string SORT_BY_PRIORITY = "priority";
	  protected internal const string SORT_BY_TOPIC_NAME = "topicName";
	  protected internal const string SORT_BY_WORKER_ID = "workerId";
	  protected internal const string SORT_BY_ACTIVITY_ID = "activityId";
	  protected internal const string SORT_BY_ACTIVITY_INSTANCE_ID = "activityInstanceId";
	  protected internal const string SORT_BY_EXECUTION_ID = "executionId";
	  protected internal const string SORT_BY_PROCESS_INSTANCE_ID = "processInstanceId";
	  protected internal const string SORT_BY_PROCESS_DEFINITION_ID = "processDefinitionId";
	  protected internal const string SORT_BY_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  protected internal const string SORT_BY_TENANT_ID = "tenantId";

	  protected internal static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricExternalTaskLogQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();

		VALID_SORT_BY_VALUES.Add(SORT_BY_TIMESTAMP);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXTERNAL_TASK_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_RETRIES);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PRIORITY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TOPIC_NAME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_WORKER_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string id;
	  protected internal string externalTaskId;
	  protected internal string topicName;
	  protected internal string workerId;
	  protected internal string errorMessage;
	  protected internal string[] activityIds;
	  protected internal string[] activityInstanceIds;
	  protected internal string[] executionIds;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal long? priorityHigherThanOrEquals;
	  protected internal long? priorityLowerThanOrEquals;
	  protected internal string[] tenantIds;
	  protected internal bool? creationLog;
	  protected internal bool? failureLog;
	  protected internal bool? successLog;
	  protected internal bool? deletionLog;

	  public HistoricExternalTaskLogQueryDto()
	  {
	  }

	  public HistoricExternalTaskLogQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("logId")]
	  public virtual string LogId
	  {
		  set
		  {
			this.id = value;
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

	  [CamundaQueryParam("topicName")]
	  public virtual string TopicName
	  {
		  set
		  {
			this.topicName = value;
		  }
	  }

	  [CamundaQueryParam("workerId")]
	  public virtual string WorkerId
	  {
		  set
		  {
			this.workerId = value;
		  }
	  }

	  [CamundaQueryParam("errorMessage")]
	  public virtual string ErrorMessage
	  {
		  set
		  {
			this.errorMessage = value;
		  }
	  }

	  [CamundaQueryParam(value:"activityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityIdIn
	  {
		  set
		  {
			this.activityIds = value;
		  }
	  }

	  [CamundaQueryParam(value:"activityInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityInstanceIdIn
	  {
		  set
		  {
			this.activityInstanceIds = value;
		  }
	  }

	  [CamundaQueryParam(value:"executionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ExecutionIdIn
	  {
		  set
		  {
			this.executionIds = value;
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

	  [CamundaQueryParam(value:"priorityHigherThanOrEquals", converter : org.camunda.bpm.engine.rest.dto.converter.LongConverter.class)]
	  public virtual long? PriorityHigherThanOrEquals
	  {
		  set
		  {
			this.priorityHigherThanOrEquals = value;
		  }
	  }

	  [CamundaQueryParam(value:"priorityLowerThanOrEquals", converter : org.camunda.bpm.engine.rest.dto.converter.LongConverter.class)]
	  public virtual long? PriorityLowerThanOrEquals
	  {
		  set
		  {
			this.priorityLowerThanOrEquals = value;
		  }
	  }

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] TenantIdIn
	  {
		  set
		  {
			this.tenantIds = value;
		  }
	  }

	  [CamundaQueryParam(value:"creationLog", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? CreationLog
	  {
		  set
		  {
			this.creationLog = value;
		  }
	  }

	  [CamundaQueryParam(value:"failureLog", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? FailureLog
	  {
		  set
		  {
			this.failureLog = value;
		  }
	  }

	  [CamundaQueryParam(value:"successLog", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? SuccessLog
	  {
		  set
		  {
			this.successLog = value;
		  }
	  }

	  [CamundaQueryParam(value:"deletionLog", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? DeletionLog
	  {
		  set
		  {
			this.deletionLog = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricExternalTaskLogQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricExternalTaskLogQuery();

	  }

	  protected internal override void applyFilters(HistoricExternalTaskLogQuery query)
	  {
		if (!string.ReferenceEquals(id, null))
		{
		  query.logId(id);
		}

		if (!string.ReferenceEquals(externalTaskId, null))
		{
		  query.externalTaskId(externalTaskId);
		}

		if (!string.ReferenceEquals(topicName, null))
		{
		  query.topicName(topicName);
		}

		if (!string.ReferenceEquals(workerId, null))
		{
		  query.workerId(workerId);
		}

		if (!string.ReferenceEquals(errorMessage, null))
		{
		  query.errorMessage(errorMessage);
		}

		if (activityIds != null && activityIds.Length > 0)
		{
		  query.activityIdIn(activityIds);
		}

		if (activityInstanceIds != null && activityInstanceIds.Length > 0)
		{
		  query.activityInstanceIdIn(activityInstanceIds);
		}

		if (executionIds != null && executionIds.Length > 0)
		{
		  query.executionIdIn(executionIds);
		}

		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}

		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}

		if (creationLog != null && creationLog)
		{
		  query.creationLog();
		}

		if (failureLog != null && failureLog)
		{
		  query.failureLog();
		}

		if (successLog != null && successLog)
		{
		  query.successLog();
		}

		if (deletionLog != null && deletionLog)
		{
		  query.deletionLog();
		}

		if (priorityHigherThanOrEquals != null)
		{
		  query.priorityHigherThanOrEquals(priorityHigherThanOrEquals.Value);
		}

		if (priorityLowerThanOrEquals != null)
		{
		  query.priorityLowerThanOrEquals(priorityLowerThanOrEquals.Value);
		}
		if (tenantIds != null && tenantIds.Length > 0)
		{
		  query.tenantIdIn(tenantIds);
		}
	  }

	  protected internal override void applySortBy(HistoricExternalTaskLogQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_TIMESTAMP))
		{
		  query.orderByTimestamp();
		}
		else if (sortBy.Equals(SORT_BY_EXTERNAL_TASK_ID))
		{
		  query.orderByExternalTaskId();
		}
		else if (sortBy.Equals(SORT_BY_RETRIES))
		{
		  query.orderByRetries();
		}
		else if (sortBy.Equals(SORT_BY_PRIORITY))
		{
		  query.orderByPriority();
		}
		else if (sortBy.Equals(SORT_BY_TOPIC_NAME))
		{
		  query.orderByTopicName();
		}
		else if (sortBy.Equals(SORT_BY_WORKER_ID))
		{
		  query.orderByWorkerId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_ID))
		{
		  query.orderByActivityId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_INSTANCE_ID))
		{
		  query.orderByActivityInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_EXECUTION_ID))
		{
		  query.orderByExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID))
		{
		  query.orderByProcessInstanceId();
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