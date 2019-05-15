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

	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricActivityInstanceQueryDto : AbstractQueryDto<HistoricActivityInstanceQuery>
	{

	  private const string SORT_BY_HISTORIC_ACTIVITY_INSTANCE_ID_VALUE = "activityInstanceId";
	  private const string SORT_BY_PROCESS_INSTANCE_ID_VALUE = "instanceId";
	  private const string SORT_BY_EXECUTION_ID_VALUE = "executionId";
	  private const string SORT_BY_ACTIVITY_ID_VALUE = "activityId";
	  private const string SORT_BY_ACTIVITY_NAME_VALUE = "activityName";
	  private const string SORT_BY_ACTIVITY_TYPE_VALUE = "activityType";
	  private const string SORT_BY_HISTORIC_ACTIVITY_INSTANCE_START_TIME_VALUE = "startTime";
	  private const string SORT_BY_HISTORIC_ACTIVITY_INSTANCE_END_TIME_VALUE = "endTime";
	  private const string SORT_BY_HISTORIC_ACTIVITY_INSTANCE_DURATION_VALUE = "duration";
	  private const string SORT_BY_PROCESS_DEFINITION_ID_VALUE = "definitionId";
	  private const string SORT_PARTIALLY_BY_OCCURRENCE = "occurrence";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricActivityInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_TYPE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_START_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_END_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_DURATION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_PARTIALLY_BY_OCCURRENCE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  private string activityInstanceId;
	  private string processInstanceId;
	  private string processDefinitionId;
	  private string executionId;
	  private string activityId;
	  private string activityName;
	  private string activityType;
	  private string taskAssignee;
	  private bool? finished;
	  private bool? unfinished;
	  private bool? completeScope;
	  private bool? canceled;
	  private DateTime startedBefore;
	  private DateTime startedAfter;
	  private DateTime finishedBefore;
	  private DateTime finishedAfter;
	  private IList<string> tenantIds;

	  public HistoricActivityInstanceQueryDto()
	  {
	  }

	  public HistoricActivityInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("activityInstanceId")]
	  public virtual string ActivityInstanceId
	  {
		  set
		  {
			this.activityInstanceId = value;
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

	  [CamundaQueryParam("executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
	  }

	  [CamundaQueryParam("activityId")]
	  public virtual string ActivityId
	  {
		  set
		  {
			this.activityId = value;
		  }
	  }

	  [CamundaQueryParam("activityName")]
	  public virtual string ActivityName
	  {
		  set
		  {
			this.activityName = value;
		  }
	  }

	  [CamundaQueryParam("activityType")]
	  public virtual string ActivityType
	  {
		  set
		  {
			this.activityType = value;
		  }
	  }

	  [CamundaQueryParam("taskAssignee")]
	  public virtual string TaskAssignee
	  {
		  set
		  {
			this.taskAssignee = value;
		  }
	  }

	  [CamundaQueryParam(value : "finished", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Finished
	  {
		  set
		  {
			this.finished = value;
		  }
	  }

	  [CamundaQueryParam(value : "unfinished", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Unfinished
	  {
		  set
		  {
			this.unfinished = value;
		  }
	  }

	  [CamundaQueryParam(value : "completeScope", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? CompleteScope
	  {
		  set
		  {
			this.completeScope = value;
		  }
	  }

	  [CamundaQueryParam(value : "canceled", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Canceled
	  {
		  set
		  {
			this.canceled = value;
		  }
	  }

	  [CamundaQueryParam(value : "startedBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime StartedBefore
	  {
		  set
		  {
			this.startedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "startedAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime StartedAfter
	  {
		  set
		  {
			this.startedAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "finishedBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FinishedBefore
	  {
		  set
		  {
			this.finishedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "finishedAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime FinishedAfter
	  {
		  set
		  {
			this.finishedAfter = value;
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

	  protected internal override HistoricActivityInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricActivityInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricActivityInstanceQuery query)
	  {
		if (!string.ReferenceEquals(activityInstanceId, null))
		{
		  query.activityInstanceId(activityInstanceId);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (!string.ReferenceEquals(activityId, null))
		{
		  query.activityId(activityId);
		}
		if (!string.ReferenceEquals(activityName, null))
		{
		  query.activityName(activityName);
		}
		if (!string.ReferenceEquals(activityType, null))
		{
		  query.activityType(activityType);
		}
		if (!string.ReferenceEquals(taskAssignee, null))
		{
		  query.taskAssignee(taskAssignee);
		}
		if (finished != null && finished)
		{
		  query.finished();
		}
		if (unfinished != null && unfinished)
		{
		  query.unfinished();
		}
		if (completeScope != null && completeScope)
		{
		  query.completeScope();
		}
		if (canceled != null && canceled)
		{
		  query.canceled();
		}
		if (startedBefore != null)
		{
		  query.startedBefore(startedBefore);
		}
		if (startedAfter != null)
		{
		  query.startedAfter(startedAfter);
		}
		if (finishedBefore != null)
		{
		  query.finishedBefore(finishedBefore);
		}
		if (finishedAfter != null)
		{
		  query.finishedAfter(finishedAfter);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(HistoricActivityInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_ID_VALUE))
		{
		  query.orderByHistoricActivityInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID_VALUE))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_ID_VALUE))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_EXECUTION_ID_VALUE))
		{
		  query.orderByExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_ID_VALUE))
		{
		  query.orderByActivityId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_NAME_VALUE))
		{
		  query.orderByActivityName();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_TYPE_VALUE))
		{
		  query.orderByActivityType();
		}
		else if (sortBy.Equals(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_START_TIME_VALUE))
		{
		  query.orderByHistoricActivityInstanceStartTime();
		}
		else if (sortBy.Equals(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_END_TIME_VALUE))
		{
		  query.orderByHistoricActivityInstanceEndTime();
		}
		else if (sortBy.Equals(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_DURATION_VALUE))
		{
		  query.orderByHistoricActivityInstanceDuration();
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