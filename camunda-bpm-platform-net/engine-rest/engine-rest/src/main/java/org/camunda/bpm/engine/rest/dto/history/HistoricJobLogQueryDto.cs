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

	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using LongConverter = org.camunda.bpm.engine.rest.dto.converter.LongConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricJobLogQueryDto : AbstractQueryDto<HistoricJobLogQuery>
	{

	  protected internal const string SORT_BY_TIMESTAMP = "timestamp";
	  protected internal const string SORT_BY_JOB_ID = "jobId";
	  protected internal const string SORT_BY_JOB_DUE_DATE = "jobDueDate";
	  protected internal const string SORT_BY_JOB_RETRIES = "jobRetries";
	  protected internal const string SORT_BY_JOB_PRIORITY = "jobPriority";
	  protected internal const string SORT_BY_JOB_DEFINITION_ID = "jobDefinitionId";
	  protected internal const string SORT_BY_ACTIVITY_ID = "activityId";
	  protected internal const string SORT_BY_EXECUTION_ID = "executionId";
	  protected internal const string SORT_BY_PROCESS_INSTANCE_ID = "processInstanceId";
	  protected internal const string SORT_BY_PROCESS_DEFINITION_ID = "processDefinitionId";
	  protected internal const string SORT_BY_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  protected internal const string SORT_BY_DEPLOYMENT_ID = "deploymentId";
	  protected internal const string SORT_PARTIALLY_BY_OCCURRENCE = "occurrence";
	  protected internal const string SORT_BY_TENANT_ID = "tenantId";

	  protected internal static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricJobLogQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();

		VALID_SORT_BY_VALUES.Add(SORT_BY_TIMESTAMP);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_DUE_DATE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_RETRIES);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_PRIORITY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEPLOYMENT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_PARTIALLY_BY_OCCURRENCE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string id;
	  protected internal string jobId;
	  protected internal string jobExceptionMessage;
	  protected internal string jobDefinitionId;
	  protected internal string jobDefinitionType;
	  protected internal string jobDefinitionConfiguration;
	  protected internal string[] activityIds;
	  protected internal string[] executionIds;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string deploymentId;
	  protected internal bool? creationLog;
	  protected internal bool? failureLog;
	  protected internal bool? successLog;
	  protected internal bool? deletionLog;
	  protected internal long? jobPriorityHigherThanOrEquals;
	  protected internal long? jobPriorityLowerThanOrEquals;
	  protected internal IList<string> tenantIds;

	  public HistoricJobLogQueryDto()
	  {
	  }

	  public HistoricJobLogQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam("jobId")]
	  public virtual string JobId
	  {
		  set
		  {
			this.jobId = value;
		  }
	  }

	  [CamundaQueryParam("jobExceptionMessage")]
	  public virtual string JobExceptionMessage
	  {
		  set
		  {
			this.jobExceptionMessage = value;
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

	  [CamundaQueryParam("jobDefinitionType")]
	  public virtual string JobDefinitionType
	  {
		  set
		  {
			this.jobDefinitionType = value;
		  }
	  }

	  [CamundaQueryParam("jobDefinitionConfiguration")]
	  public virtual string JobDefinitionConfiguration
	  {
		  set
		  {
			this.jobDefinitionConfiguration = value;
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

	  [CamundaQueryParam("deploymentId")]
	  public virtual string DeploymentId
	  {
		  set
		  {
			this.deploymentId = value;
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

	  [CamundaQueryParam(value:"jobPriorityHigherThanOrEquals", converter : org.camunda.bpm.engine.rest.dto.converter.LongConverter.class)]
	  public virtual long? JobPriorityHigherThanOrEquals
	  {
		  set
		  {
			this.jobPriorityHigherThanOrEquals = value;
		  }
	  }

	  [CamundaQueryParam(value:"jobPriorityLowerThanOrEquals", converter : org.camunda.bpm.engine.rest.dto.converter.LongConverter.class)]
	  public virtual long? JobPriorityLowerThanOrEquals
	  {
		  set
		  {
			this.jobPriorityLowerThanOrEquals = value;
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

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricJobLogQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricJobLogQuery();
	  }

	  protected internal override void applyFilters(HistoricJobLogQuery query)
	  {
		if (!string.ReferenceEquals(id, null))
		{
		  query.logId(id);
		}

		if (!string.ReferenceEquals(jobId, null))
		{
		  query.jobId(jobId);
		}

		if (!string.ReferenceEquals(jobExceptionMessage, null))
		{
		  query.jobExceptionMessage(jobExceptionMessage);
		}

		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  query.jobDefinitionId(jobDefinitionId);
		}

		if (!string.ReferenceEquals(jobDefinitionType, null))
		{
		  query.jobDefinitionType(jobDefinitionType);
		}

		if (!string.ReferenceEquals(jobDefinitionConfiguration, null))
		{
		  query.jobDefinitionConfiguration(jobDefinitionConfiguration);
		}

		if (activityIds != null && activityIds.Length > 0)
		{
		  query.activityIdIn(activityIds);
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

		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
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

		if (jobPriorityLowerThanOrEquals != null)
		{
		  query.jobPriorityLowerThanOrEquals(jobPriorityLowerThanOrEquals.Value);
		}

		if (jobPriorityHigherThanOrEquals != null)
		{
		  query.jobPriorityHigherThanOrEquals(jobPriorityHigherThanOrEquals.Value);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(HistoricJobLogQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_TIMESTAMP))
		{
		  query.orderByTimestamp();
		}
		else if (sortBy.Equals(SORT_BY_JOB_ID))
		{
		  query.orderByJobId();
		}
		else if (sortBy.Equals(SORT_BY_JOB_DUE_DATE))
		{
		  query.orderByJobDueDate();
		}
		else if (sortBy.Equals(SORT_BY_JOB_RETRIES))
		{
		  query.orderByJobRetries();
		}
		else if (sortBy.Equals(SORT_BY_JOB_PRIORITY))
		{
		  query.orderByJobPriority();
		}
		else if (sortBy.Equals(SORT_BY_JOB_DEFINITION_ID))
		{
		  query.orderByJobDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_ID))
		{
		  query.orderByActivityId();
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
		else if (sortBy.Equals(SORT_BY_DEPLOYMENT_ID))
		{
		  query.orderByDeploymentId();
		}
		else if (sortBy.Equals(SORT_PARTIALLY_BY_OCCURRENCE))
		{
		  query.orderPartiallyByOccurrence();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}