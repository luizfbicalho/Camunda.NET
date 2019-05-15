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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;


	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using StringSetConverter = org.camunda.bpm.engine.rest.dto.converter.StringSetConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricCaseInstanceQueryDto : AbstractQueryDto<HistoricCaseInstanceQuery>
	{

	  public const string SORT_BY_CASE_INSTANCE_ID_VALUE = "instanceId";
	  public const string SORT_BY_CASE_DEFINITION_ID_VALUE = "definitionId";
	  public const string SORT_BY_CASE_INSTANCE_BUSINESS_KEY_VALUE = "businessKey";
	  public const string SORT_BY_CASE_INSTANCE_CREATE_TIME_VALUE = "createTime";
	  public const string SORT_BY_CASE_INSTANCE_CLOSE_TIME_VALUE = "closeTime";
	  public const string SORT_BY_CASE_INSTANCE_DURATION_VALUE = "duration";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  public static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricCaseInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_BUSINESS_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_CREATE_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_CLOSE_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CASE_INSTANCE_DURATION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  public string caseInstanceId;
	  public ISet<string> caseInstanceIds;
	  public string caseDefinitionId;
	  public string caseDefinitionKey;
	  public string caseDefinitionName;
	  public string caseDefinitionNameLike;
	  public IList<string> caseDefinitionKeyNotIn;
	  public string caseInstanceBusinessKey;
	  public string caseInstanceBusinessKeyLike;
	  public string superCaseInstanceId;
	  public string subCaseInstanceId;
	  private string superProcessInstanceId;
	  private string subProcessInstanceId;
	  private IList<string> tenantIds;
	  private bool? withoutTenantId;
	  public string createdBy;
	  public IList<string> caseActivityIdIn;

	  public DateTime createdBefore;
	  public DateTime createdAfter;
	  public DateTime closedBefore;
	  public DateTime closedAfter;

	  public bool? active;
	  public bool? completed;
	  public bool? terminated;
	  public bool? closed;
	  public bool? notClosed;

	  protected internal IList<VariableQueryParameterDto> variables;

	  public HistoricCaseInstanceQueryDto()
	  {
	  }

	  public HistoricCaseInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseInstanceIds", converter : org.camunda.bpm.engine.rest.dto.converter.StringSetConverter.class)]
	  public virtual ISet<string> CaseInstanceIds
	  {
		  set
		  {
			this.caseInstanceIds = value;
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

	  [CamundaQueryParam("caseDefinitionName")]
	  public virtual string CaseDefinitionName
	  {
		  set
		  {
			this.caseDefinitionName = value;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionNameLike")]
	  public virtual string CaseDefinitionNameLike
	  {
		  set
		  {
			this.caseDefinitionNameLike = value;
		  }
	  }

	  [CamundaQueryParam("caseDefinitionKey")]
	  public virtual string CaseDefinitionKey
	  {
		  set
		  {
			this.caseDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseDefinitionKeyNotIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> CaseDefinitionKeyNotIn
	  {
		  set
		  {
			this.caseDefinitionKeyNotIn = value;
		  }
	  }

	  [CamundaQueryParam("caseInstanceBusinessKey")]
	  public virtual string CaseInstanceBusinessKey
	  {
		  set
		  {
			this.caseInstanceBusinessKey = value;
		  }
	  }

	  [CamundaQueryParam("caseInstanceBusinessKeyLike")]
	  public virtual string CaseInstanceBusinessKeyLike
	  {
		  set
		  {
			this.caseInstanceBusinessKeyLike = value;
		  }
	  }

	  [CamundaQueryParam("superCaseInstanceId")]
	  public virtual string SuperCaseInstanceId
	  {
		  set
		  {
			this.superCaseInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("subCaseInstanceId")]
	  public virtual string SubCaseInstanceId
	  {
		  set
		  {
			this.subCaseInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("superProcessInstanceId")]
	  public virtual string SuperProcessInstanceId
	  {
		  set
		  {
			this.superProcessInstanceId = value;
		  }
	  }

	  [CamundaQueryParam("subProcessInstanceId")]
	  public virtual string SubProcessInstanceId
	  {
		  set
		  {
			this.subProcessInstanceId = value;
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

	  [CamundaQueryParam(value : "withoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutTenantId
	  {
		  set
		  {
			this.withoutTenantId = value;
		  }
	  }

	  [CamundaQueryParam("createdBy")]
	  public virtual string CreatedBy
	  {
		  set
		  {
			this.createdBy = value;
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

	  [CamundaQueryParam(value : "closedBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime ClosedBefore
	  {
		  set
		  {
			this.closedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "closedAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime ClosedAfter
	  {
		  set
		  {
			this.closedAfter = value;
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

	  [CamundaQueryParam(value : "closed", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Closed
	  {
		  set
		  {
			this.closed = value;
		  }
	  }

	  [CamundaQueryParam(value : "notClosed", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? NotClosed
	  {
		  set
		  {
			this.notClosed = value;
		  }
	  }

	  [CamundaQueryParam(value : "variables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> Variables
	  {
		  set
		  {
			this.variables = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseActivityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> CaseActivityIdIn
	  {
		  set
		  {
			this.caseActivityIdIn = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricCaseInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricCaseInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricCaseInstanceQuery query)
	  {

		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (caseInstanceIds != null)
		{
		  query.caseInstanceIds(caseInstanceIds);
		}
		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}
		if (!string.ReferenceEquals(caseDefinitionKey, null))
		{
		  query.caseDefinitionKey(caseDefinitionKey);
		}
		if (!string.ReferenceEquals(caseDefinitionName, null))
		{
		  query.caseDefinitionName(caseDefinitionName);
		}
		if (!string.ReferenceEquals(caseDefinitionNameLike, null))
		{
		  query.caseDefinitionNameLike(caseDefinitionNameLike);
		}
		if (caseDefinitionKeyNotIn != null)
		{
		  query.caseDefinitionKeyNotIn(caseDefinitionKeyNotIn);
		}
		if (!string.ReferenceEquals(caseInstanceBusinessKey, null))
		{
		  query.caseInstanceBusinessKey(caseInstanceBusinessKey);
		}
		if (!string.ReferenceEquals(caseInstanceBusinessKeyLike, null))
		{
		  query.caseInstanceBusinessKeyLike(caseInstanceBusinessKeyLike);
		}
		if (!string.ReferenceEquals(superCaseInstanceId, null))
		{
		  query.superCaseInstanceId(superCaseInstanceId);
		}
		if (!string.ReferenceEquals(subCaseInstanceId, null))
		{
		  query.subCaseInstanceId(subCaseInstanceId);
		}
		if (!string.ReferenceEquals(superProcessInstanceId, null))
		{
		  query.superProcessInstanceId(superProcessInstanceId);
		}
		if (!string.ReferenceEquals(subProcessInstanceId, null))
		{
		  query.subProcessInstanceId(subProcessInstanceId);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (!string.ReferenceEquals(createdBy, null))
		{
		  query.createdBy(createdBy);
		}
		if (createdBefore != null)
		{
		  query.createdBefore(createdBefore);
		}
		if (createdAfter != null)
		{
		  query.createdAfter(createdAfter);
		}
		if (closedBefore != null)
		{
		  query.closedBefore(closedBefore);
		}
		if (closedAfter != null)
		{
		  query.closedAfter(closedAfter);
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
		if (closed != null && closed)
		{
		  query.closed();
		}
		if (notClosed != null && notClosed)
		{
		  query.notClosed();
		}
		if (caseActivityIdIn != null && caseActivityIdIn.Count > 0)
		{
		  query.caseActivityIdIn(caseActivityIdIn.ToArray());
		}
		if (variables != null)
		{
		  foreach (VariableQueryParameterDto variableQueryParam in variables)
		  {
			string variableName = variableQueryParam.Name;
			string op = variableQueryParam.Operator;
			object variableValue = variableQueryParam.resolveValue(objectMapper);

			if (op.Equals(VariableQueryParameterDto.EQUALS_OPERATOR_NAME))
			{
			  query.variableValueEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OPERATOR_NAME))
			{
			  query.variableValueGreaterThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.variableValueGreaterThanOrEqual(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OPERATOR_NAME))
			{
			  query.variableValueLessThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.variableValueLessThanOrEqual(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.NOT_EQUALS_OPERATOR_NAME))
			{
			  query.variableValueNotEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LIKE_OPERATOR_NAME))
			{
			  query.variableValueLike(variableName, variableValue.ToString());
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid variable comparator specified: " + op);
			}
		  }
		}
	  }

	  protected internal override void applySortBy(HistoricCaseInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_CASE_INSTANCE_ID_VALUE))
		{
		  query.orderByCaseInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_DEFINITION_ID_VALUE))
		{
		  query.orderByCaseDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_BUSINESS_KEY_VALUE))
		{
		  query.orderByCaseInstanceBusinessKey();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_CREATE_TIME_VALUE))
		{
		  query.orderByCaseInstanceCreateTime();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_CLOSE_TIME_VALUE))
		{
		  query.orderByCaseInstanceCloseTime();
		}
		else if (sortBy.Equals(SORT_BY_CASE_INSTANCE_DURATION_VALUE))
		{
		  query.orderByCaseInstanceDuration();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}