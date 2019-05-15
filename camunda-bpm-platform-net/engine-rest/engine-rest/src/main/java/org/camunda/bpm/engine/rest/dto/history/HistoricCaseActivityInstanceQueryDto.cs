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

	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricCaseActivityInstanceQueryDto : AbstractQueryDto<HistoricCaseActivityInstanceQuery>
	{

	  protected internal const string SORT_BY_HISTORIC_ACTIVITY_INSTANCE_ID_VALUE = "caseActivityInstanceId";
	  protected internal const string SORT_BY_CASE_INSTANCE_ID_VALUE = "caseInstanceId";
	  protected internal const string SORT_BY_CASE_EXECUTION_ID_VALUE = "caseExecutionId";
	  protected internal const string SORT_BY_CASE_ACTIVITY_ID_VALUE = "caseActivityId";
	  protected internal const string SORT_BY_CASE_ACTIVITY_NAME_VALUE = "caseActivityName";
	  protected internal const string SORT_BY_CASE_ACTIVITY_TYPE_VALUE = "caseActivityType";
	  protected internal const string SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME_VALUE = "createTime";
	  protected internal const string SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME_VALUE = "endTime";
	  protected internal const string SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_DURATION_VALUE = "duration";
	  protected internal const string SORT_BY_CASE_DEFINITION_ID_VALUE = "caseDefinitionId";
	  protected internal const string SORT_BY_TENANT_ID = "tenantId";

	  protected internal static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricCaseActivityInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_EXECUTION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_ACTIVITY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_ACTIVITY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_ACTIVITY_TYPE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_DURATION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string caseActivityInstanceId;
	  protected internal IList<string> caseActivityInstanceIds;
	  protected internal string caseInstanceId;
	  protected internal string caseDefinitionId;
	  protected internal string caseExecutionId;
	  protected internal string caseActivityId;
	  protected internal IList<string> caseActivityIds;
	  protected internal string caseActivityName;
	  protected internal string caseActivityType;
	  protected internal DateTime createdBefore;
	  protected internal DateTime createdAfter;
	  protected internal DateTime endedBefore;
	  protected internal DateTime endedAfter;
	  protected internal bool? required;
	  protected internal bool? finished;
	  protected internal bool? unfinished;
	  protected internal bool? available;
	  protected internal bool? enabled;
	  protected internal bool? disabled;
	  protected internal bool? active;
	  protected internal bool? completed;
	  protected internal bool? terminated;
	  protected internal IList<string> tenantIds;

	  public HistoricCaseActivityInstanceQueryDto()
	  {
	  }

	  public HistoricCaseActivityInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("caseActivityInstanceId")]
	  public virtual string CaseActivityInstanceId
	  {
		  set
		  {
			this.caseActivityInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseActivityInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> CaseActivityInstanceIdIn
	  {
		  set
		  {
			this.caseActivityInstanceIds = value;
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

	  [CamundaQueryParam("caseDefinitionId")]
	  public virtual string CaseDefinitionId
	  {
		  set
		  {
			this.caseDefinitionId = value;
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

	  [CamundaQueryParam("caseActivityId")]
	  public virtual string CaseActivityId
	  {
		  set
		  {
			this.caseActivityId = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseActivityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> CaseActivityIdIn
	  {
		  set
		  {
			this.caseActivityIds = value;
		  }
	  }

	  [CamundaQueryParam("caseActivityName")]
	  public virtual string CaseActivityName
	  {
		  set
		  {
			this.caseActivityName = value;
		  }
	  }

	  [CamundaQueryParam("caseActivityType")]
	  public virtual string CaseActivityType
	  {
		  set
		  {
			this.caseActivityType = value;
		  }
	  }

	  [CamundaQueryParam(value : "createdBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime CreatedBefore
	  {
		  set
		  {
			this.createdBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "createdAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime CreatedAfter
	  {
		  set
		  {
			this.createdAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "endedBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime EndedBefore
	  {
		  set
		  {
			this.endedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "endedAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime EndedAfter
	  {
		  set
		  {
			this.endedAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "required", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Required
	  {
		  set
		  {
			this.required = value;
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

	  [CamundaQueryParam(value : "available", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Available
	  {
		  set
		  {
			this.available = value;
		  }
	  }

	  [CamundaQueryParam(value : "enabled", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Enabled
	  {
		  set
		  {
			this.enabled = value;
		  }
	  }

	  [CamundaQueryParam(value : "disabled", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Disabled
	  {
		  set
		  {
			this.disabled = value;
		  }
	  }

	  [CamundaQueryParam(value : "active", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Active
	  {
		  set
		  {
			this.active = value;
		  }
	  }

	  [CamundaQueryParam(value : "completed", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Completed
	  {
		  set
		  {
			this.completed = value;
		  }
	  }

	  [CamundaQueryParam(value : "terminated", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Terminated
	  {
		  set
		  {
			this.terminated = value;
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

	  protected internal override HistoricCaseActivityInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricCaseActivityInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricCaseActivityInstanceQuery query)
	  {
		if (!string.ReferenceEquals(caseActivityInstanceId, null))
		{
		  query.caseActivityInstanceId(caseActivityInstanceId);
		}
		if (caseActivityInstanceIds != null && caseActivityInstanceIds.Count > 0)
		{
		  query.caseActivityInstanceIdIn(caseActivityInstanceIds.ToArray());
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  query.caseExecutionId(caseExecutionId);
		}
		if (!string.ReferenceEquals(caseActivityId, null))
		{
		  query.caseActivityId(caseActivityId);
		}
		if (caseActivityIds != null && caseActivityIds.Count > 0)
		{
		  query.caseActivityIdIn(caseActivityIds.ToArray());
		}
		if (!string.ReferenceEquals(caseActivityName, null))
		{
		  query.caseActivityName(caseActivityName);
		}
		if (!string.ReferenceEquals(caseActivityType, null))
		{
		  query.caseActivityType(caseActivityType);
		}
		if (createdBefore != null)
		{
		  query.createdBefore(createdBefore);
		}
		if (createdAfter != null)
		{
		  query.createdAfter(createdAfter);
		}
		if (endedBefore != null)
		{
		  query.endedBefore(endedBefore);
		}
		if (endedAfter != null)
		{
		  query.endedAfter(endedAfter);
		}
		if (required != null && required)
		{
		  query.required();
		}
		if (finished != null && finished)
		{
		  query.ended();
		}
		if (unfinished != null && unfinished)
		{
		  query.notEnded();
		}
		if (available != null && available)
		{
		  query.available();
		}
		if (enabled != null && enabled)
		{
		  query.enabled();
		}
		if (disabled != null && disabled)
		{
		  query.disabled();
		}
		if (active != null && active)
		{
		  query.active();
		}
		if (completed != null && completed)
		{
		  query.completed();
		}
		if (terminated != null && terminated)
		{
		  query.terminated();
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal virtual void applySortBy(HistoricCaseActivityInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_HISTORIC_ACTIVITY_INSTANCE_ID_VALUE))
		{
		  query.orderByHistoricCaseActivityInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_ID_VALUE))
		{
		  query.orderByCaseInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_EXECUTION_ID_VALUE))
		{
		  query.orderByCaseExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_ACTIVITY_ID_VALUE))
		{
		  query.orderByCaseActivityId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_ACTIVITY_NAME_VALUE))
		{
		  query.orderByCaseActivityName();
		}
		else if (sortBy.Equals(SORT_BY_CASE_ACTIVITY_TYPE_VALUE))
		{
		  query.orderByCaseActivityType();
		}
		else if (sortBy.Equals(SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME_VALUE))
		{
		  query.orderByHistoricCaseActivityInstanceCreateTime();
		}
		else if (sortBy.Equals(SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME_VALUE))
		{
		  query.orderByHistoricCaseActivityInstanceEndTime();
		}
		else if (sortBy.Equals(SORT_BY_HISTORIC_CASE_ACTIVITY_INSTANCE_DURATION_VALUE))
		{
		  query.orderByHistoricCaseActivityInstanceDuration();
		}
		else if (sortBy.Equals(SORT_BY_CASE_DEFINITION_ID_VALUE))
		{
		  query.orderByCaseDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}