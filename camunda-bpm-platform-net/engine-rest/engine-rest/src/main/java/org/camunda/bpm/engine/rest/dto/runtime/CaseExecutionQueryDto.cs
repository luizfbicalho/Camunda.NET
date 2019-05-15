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
namespace org.camunda.bpm.engine.rest.dto.runtime
{


	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionQueryDto : AbstractQueryDto<CaseExecutionQuery>
	{

	  protected internal const string SORT_BY_EXECUTION_ID_VALUE = "caseExecutionId";
	  protected internal const string SORT_BY_DEFINITION_KEY_VALUE = "caseDefinitionKey";
	  protected internal const string SORT_BY_DEFINITION_ID_VALUE = "caseDefinitionId";
	  protected internal const string SORT_BY_TENANT_ID = "tenantId";

	  protected internal static readonly IList<string> VALID_SORT_BY_VALUES;
	  static CaseExecutionQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string caseExecutionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
	  protected internal string caseInstanceId;
	  protected internal string businessKey;
	  protected internal string activityId;
	  protected internal IList<string> tenantIds;
	  protected internal bool? required;
	  protected internal bool? enabled;
	  protected internal bool? active;
	  protected internal bool? disabled;

	  protected internal IList<VariableQueryParameterDto> variables;
	  protected internal IList<VariableQueryParameterDto> caseInstanceVariables;

	  public CaseExecutionQueryDto()
	  {
	  }

	  public CaseExecutionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("caseExecutionId")]
	  public virtual string CaseExecutionId
	  {
		  set
		  {
			this.caseExecutionId = value;
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

	  [CamundaQueryParam("businessKey")]
	  public virtual string BusinessKey
	  {
		  set
		  {
			this.businessKey = value;
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

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> TenantIdIn
	  {
		  set
		  {
			this.tenantIds = value;
		  }
	  }

	  [CamundaQueryParam(value:"required", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Required
	  {
		  set
		  {
			this.required = value;
		  }
	  }

	  [CamundaQueryParam(value:"enabled", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Enabled
	  {
		  set
		  {
			this.enabled = value;
		  }
	  }

	  [CamundaQueryParam(value:"active", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Active
	  {
		  set
		  {
			this.active = value;
		  }
	  }

	  [CamundaQueryParam(value:"disabled", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Disabled
	  {
		  set
		  {
			this.disabled = value;
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

	  [CamundaQueryParam(value : "caseInstanceVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> CaseInstanceVariables
	  {
		  set
		  {
			this.caseInstanceVariables = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override CaseExecutionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.CaseService.createCaseExecutionQuery();
	  }

	  protected internal override void applyFilters(CaseExecutionQuery query)
	  {
		if (!string.ReferenceEquals(caseExecutionId, null))
		{
		  query.caseExecutionId(caseExecutionId);
		}

		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}

		if (!string.ReferenceEquals(businessKey, null))
		{
		  query.caseInstanceBusinessKey(businessKey);
		}

		if (!string.ReferenceEquals(caseDefinitionKey, null))
		{
		  query.caseDefinitionKey(caseDefinitionKey);
		}

		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}

		if (!string.ReferenceEquals(activityId, null))
		{
		  query.activityId(activityId);
		}

		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}

		if (required != null && required == true)
		{
		  query.required();
		}

		if (active != null && active == true)
		{
		  query.active();
		}

		if (enabled != null && enabled == true)
		{
		  query.enabled();
		}

		if (disabled != null && disabled == true)
		{
		  query.disabled();
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

		if (caseInstanceVariables != null)
		{

		  foreach (VariableQueryParameterDto variableQueryParam in caseInstanceVariables)
		  {

			string variableName = variableQueryParam.Name;
			string op = variableQueryParam.Operator;
			object variableValue = variableQueryParam.resolveValue(objectMapper);

			if (op.Equals(VariableQueryParameterDto.EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueGreaterThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.GREATER_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueGreaterThanOrEqual(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueLessThan(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LESS_THAN_OR_EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueLessThanOrEqual(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.NOT_EQUALS_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueNotEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.LIKE_OPERATOR_NAME))
			{
			  query.caseInstanceVariableValueLike(variableName, variableValue.ToString());
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid variable comparator specified: " + op);
			}
		  }
		}
	  }

	  protected internal virtual void applySortBy(CaseExecutionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_EXECUTION_ID_VALUE))
		{
		  query.orderByCaseExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_DEFINITION_KEY_VALUE))
		{
		  query.orderByCaseDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_DEFINITION_ID_VALUE))
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