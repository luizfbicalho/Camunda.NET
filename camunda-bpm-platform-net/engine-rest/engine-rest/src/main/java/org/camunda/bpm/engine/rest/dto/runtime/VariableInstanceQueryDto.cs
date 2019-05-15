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


	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class VariableInstanceQueryDto : AbstractQueryDto<VariableInstanceQuery>
	{

	  private const string SORT_BY_VARIABLE_NAME_VALUE = "variableName";
	  private const string SORT_BY_VARIABLE_TYPE_VALUE = "variableType";
	  private const string SORT_BY_ACTIVITY_INSTANCE_ID_VALUE = "activityInstanceId";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static VariableInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_VARIABLE_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VARIABLE_TYPE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string variableName;
	  protected internal string variableNameLike;
	  protected internal IList<VariableQueryParameterDto> variableValues;
	  protected internal string[] executionIdIn;
	  protected internal string[] processInstanceIdIn;
	  protected internal string[] caseExecutionIdIn;
	  protected internal string[] caseInstanceIdIn;
	  protected internal string[] taskIdIn;
	  protected internal string[] variableScopeIdIn;
	  protected internal string[] activityInstanceIdIn;
	  private IList<string> tenantIds;

	  public VariableInstanceQueryDto()
	  {
	  }

	  public VariableInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("variableName")]
	  public virtual string VariableName
	  {
		  set
		  {
			this.variableName = value;
		  }
	  }

	  [CamundaQueryParam("variableNameLike")]
	  public virtual string VariableNameLike
	  {
		  set
		  {
			this.variableNameLike = value;
		  }
	  }

	  [CamundaQueryParam(value : "variableValues", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> VariableValues
	  {
		  set
		  {
			this.variableValues = value;
		  }
	  }

	  [CamundaQueryParam(value:"executionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ExecutionIdIn
	  {
		  set
		  {
			this.executionIdIn = value;
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

	  [CamundaQueryParam(value:"caseExecutionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CaseExecutionIdIn
	  {
		  set
		  {
			this.caseExecutionIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"caseInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CaseInstanceIdIn
	  {
		  set
		  {
			this.caseInstanceIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"taskIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] TaskIdIn
	  {
		  set
		  {
			this.taskIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"variableScopeIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] VariableScopeIdIn
	  {
		  set
		  {
			this.variableScopeIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"activityInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityInstanceIdIn
	  {
		  set
		  {
			this.activityInstanceIdIn = value;
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

	  protected internal override VariableInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RuntimeService.createVariableInstanceQuery();
	  }

	  protected internal override void applyFilters(VariableInstanceQuery query)
	  {
		if (!string.ReferenceEquals(variableName, null))
		{
		  query.variableName(variableName);
		}

		if (!string.ReferenceEquals(variableNameLike, null))
		{
		  query.variableNameLike(variableNameLike);
		}

		if (variableValues != null)
		{
		  foreach (VariableQueryParameterDto variableQueryParam in variableValues)
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

		if (executionIdIn != null && executionIdIn.Length > 0)
		{
		  query.executionIdIn(executionIdIn);
		}

		if (processInstanceIdIn != null && processInstanceIdIn.Length > 0)
		{
		  query.processInstanceIdIn(processInstanceIdIn);
		}

		if (caseExecutionIdIn != null && caseExecutionIdIn.Length > 0)
		{
		  query.caseExecutionIdIn(caseExecutionIdIn);
		}

		if (caseInstanceIdIn != null && caseInstanceIdIn.Length > 0)
		{
		  query.caseInstanceIdIn(caseInstanceIdIn);
		}

		if (taskIdIn != null && taskIdIn.Length > 0)
		{
		  query.taskIdIn(taskIdIn);
		}

		if (variableScopeIdIn != null && variableScopeIdIn.Length > 0)
		{
		  query.variableScopeIdIn(variableScopeIdIn);
		}

		if (activityInstanceIdIn != null && activityInstanceIdIn.Length > 0)
		{
		  query.activityInstanceIdIn(activityInstanceIdIn);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(VariableInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_VARIABLE_NAME_VALUE))
		{
		  query.orderByVariableName();
		}
		else if (sortBy.Equals(SORT_BY_VARIABLE_TYPE_VALUE))
		{
		  query.orderByVariableType();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_INSTANCE_ID_VALUE))
		{
		  query.orderByActivityInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}