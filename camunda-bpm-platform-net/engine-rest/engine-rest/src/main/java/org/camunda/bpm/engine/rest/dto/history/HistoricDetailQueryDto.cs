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


	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricDetailQueryDto : AbstractQueryDto<HistoricDetailQuery>
	{

	  private const string SORT_BY_PROCESS_INSTANCE_ID = "processInstanceId";
	  private const string SORT_BY_VARIABLE_NAME = "variableName";
	  private const string SORT_BY_VARIABLE_TYPE = "variableType";
	  private const string SORT_BY_VARIABLE_REVISION = "variableRevision";
	  private const string SORT_BY_FORM_PROPERTY_ID = "formPropertyId";
	  private const string SORT_BY_TIME = "time";
	  private const string SORT_PARTIALLY_BY_OCCURENCE = "occurrence";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricDetailQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VARIABLE_NAME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VARIABLE_TYPE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VARIABLE_REVISION);
		VALID_SORT_BY_VALUES.Add(SORT_BY_FORM_PROPERTY_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TIME);
		VALID_SORT_BY_VALUES.Add(SORT_PARTIALLY_BY_OCCURENCE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string activityInstanceId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string variableInstanceId;
	  protected internal string[] variableTypeIn;
	  protected internal string taskId;
	  protected internal bool? formFields;
	  protected internal bool? variableUpdates;
	  protected internal bool? excludeTaskDetails;
	  protected internal IList<string> tenantIds;
	  protected internal string[] processInstanceIdIn;
	  protected internal string userOperationId;
	  private DateTime occurredBefore;
	  private DateTime occurredAfter;

	  public HistoricDetailQueryDto()
	  {
	  }

	  public HistoricDetailQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam(value : "processInstanceId")]
	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "activityInstanceId")]
	  public virtual string ActivityInstanceId
	  {
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseExecutionId")]
	  public virtual string CaseExecutionId
	  {
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "variableInstanceId")]
	  public virtual string VariableInstanceId
	  {
		  set
		  {
			this.variableInstanceId = value;
		  }
	  }

	   [CamundaQueryParam(value:"variableTypeIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	   public virtual string[] VariableTypeIn
	   {
		   set
		   {
			this.variableTypeIn = value;
		   }
	   }

	  [CamundaQueryParam(value : "taskId")]
	  public virtual string TaskId
	  {
		  set
		  {
			this.taskId = value;
		  }
	  }

	  [CamundaQueryParam(value : "formFields", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? FormFields
	  {
		  set
		  {
			this.formFields = value;
		  }
	  }

	  [CamundaQueryParam(value : "variableUpdates", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? VariableUpdates
	  {
		  set
		  {
			this.variableUpdates = value;
		  }
	  }

	  [CamundaQueryParam(value : "excludeTaskDetails", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? ExcludeTaskDetails
	  {
		  set
		  {
			this.excludeTaskDetails = value;
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

	  [CamundaQueryParam(value:"processInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessInstanceIdIn
	  {
		  set
		  {
			this.processInstanceIdIn = value;
		  }
	  }


	  [CamundaQueryParam(value : "userOperationId")]
	  public virtual string UserOperationId
	  {
		  set
		  {
			this.userOperationId = value;
		  }
	  }

	  [CamundaQueryParam(value : "occurredBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime OccurredBefore
	  {
		  set
		  {
			this.occurredBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "occurredAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime OccurredAfter
	  {
		  set
		  {
			this.occurredAfter = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricDetailQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricDetailQuery();
	  }

	  protected internal override bool sortOptionsValid()
	  {
		if (sortings != null)
		{
		  foreach (SortingDto sorting in sortings)
		  {
			string sortingOrder = sorting.SortOrder;
			string sortingBy = sorting.SortBy;

			if (!VALID_SORT_BY_VALUES.Contains(sortingBy))
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "sortBy parameter has invalid value: " + sortingBy);
			}

			if (string.ReferenceEquals(sortingBy, null) || string.ReferenceEquals(sortingOrder, null))
			{
			  return false;
			}
		  }
		}

		return base.sortOptionsValid();
	  }

	  protected internal override void applyFilters(HistoricDetailQuery query)
	  {
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (!string.ReferenceEquals(activityInstanceId, null))
		{
		  query.activityInstanceId(activityInstanceId);
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  query.caseExecutionId(caseExecutionId);
		}
		if (!string.ReferenceEquals(variableInstanceId, null))
		{
		  query.variableInstanceId(variableInstanceId);
		}
		if (variableTypeIn != null && variableTypeIn.Length > 0)
		{
		  query.variableTypeIn(variableTypeIn);
		}
		if (!string.ReferenceEquals(taskId, null))
		{
		  query.taskId(taskId);
		}
		if (formFields != null)
		{
		  query.formFields();
		}
		if (variableUpdates != null)
		{
		  query.variableUpdates();
		}
		if (excludeTaskDetails != null)
		{
		  query.excludeTaskDetails();
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (processInstanceIdIn != null && processInstanceIdIn.Length > 0)
		{
		  query.processInstanceIdIn(processInstanceIdIn);
		}
		if (!string.ReferenceEquals(userOperationId, null))
		{
		  query.userOperationId(userOperationId);
		}
		if (occurredBefore != null)
		{
		  query.occurredBefore(occurredBefore);
		}
		if (occurredAfter != null)
		{
		  query.occurredAfter(occurredAfter);
		}
	  }

	  protected internal override void applySortBy(HistoricDetailQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_VARIABLE_NAME))
		{
		  query.orderByVariableName();
		}
		else if (sortBy.Equals(SORT_BY_VARIABLE_TYPE))
		{
		  query.orderByVariableType();
		}
		else if (sortBy.Equals(SORT_BY_VARIABLE_REVISION))
		{
		  query.orderByVariableRevision();
		}
		else if (sortBy.Equals(SORT_BY_FORM_PROPERTY_ID))
		{
		  query.orderByFormPropertyId();
		}
		else if (sortBy.Equals(SORT_BY_TIME))
		{
		  query.orderByTime();
		}
		else if (sortBy.Equals(SORT_PARTIALLY_BY_OCCURENCE))
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