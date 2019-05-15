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
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ExecutionQueryDto : AbstractQueryDto<ExecutionQuery>
	{

	  private const string SORT_BY_INSTANCE_ID_VALUE = "instanceId";
	  private const string SORT_BY_DEFINITION_KEY_VALUE = "definitionKey";
	  private const string SORT_BY_DEFINITION_ID_VALUE = "definitionId";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static ExecutionQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  private string processDefinitionKey;
	  private string businessKey;
	  private string processDefinitionId;
	  private string processInstanceId;
	  private string activityId;
	  private string signalEventSubscriptionName;
	  private string messageEventSubscriptionName;
	  private bool? active;
	  private bool? suspended;
	  private string incidentId;
	  private string incidentType;
	  private string incidentMessage;
	  private string incidentMessageLike;
	  private IList<string> tenantIdIn;

	  private IList<VariableQueryParameterDto> variables;
	  private IList<VariableQueryParameterDto> processVariables;

	  public ExecutionQueryDto()
	  {

	  }

	  public ExecutionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("processDefinitionKey")]
	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
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

	  [CamundaQueryParam("processDefinitionId")]
	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
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

	  [CamundaQueryParam("activityId")]
	  public virtual string ActivityId
	  {
		  set
		  {
			this.activityId = value;
		  }
	  }

	  [CamundaQueryParam("signalEventSubscriptionName")]
	  public virtual string SignalEventSubscriptionName
	  {
		  set
		  {
			this.signalEventSubscriptionName = value;
		  }
	  }

	  [CamundaQueryParam("messageEventSubscriptionName")]
	  public virtual string MessageEventSubscriptionName
	  {
		  set
		  {
			this.messageEventSubscriptionName = value;
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

	  [CamundaQueryParam(value : "processVariables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> ProcessVariables
	  {
		  set
		  {
			this.processVariables = value;
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

	  [CamundaQueryParam(value : "suspended", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Suspended
	  {
		  set
		  {
			this.suspended = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentId")]
	  public virtual string IncidentId
	  {
		  set
		  {
			this.incidentId = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentType")]
	  public virtual string IncidentType
	  {
		  set
		  {
			this.incidentType = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentMessage")]
	  public virtual string IncidentMessage
	  {
		  set
		  {
			this.incidentMessage = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentMessageLike")]
	  public virtual string IncidentMessageLike
	  {
		  set
		  {
			this.incidentMessageLike = value;
		  }
	  }

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> TenantIdIn
	  {
		  set
		  {
			this.tenantIdIn = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override ExecutionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RuntimeService.createExecutionQuery();
	  }

	  protected internal override void applyFilters(ExecutionQuery query)
	  {
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(businessKey, null))
		{
		  query.processInstanceBusinessKey(businessKey);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(activityId, null))
		{
		  query.activityId(activityId);
		}
		if (!string.ReferenceEquals(signalEventSubscriptionName, null))
		{
		  query.signalEventSubscriptionName(signalEventSubscriptionName);
		}
		if (!string.ReferenceEquals(messageEventSubscriptionName, null))
		{
		  query.messageEventSubscriptionName(messageEventSubscriptionName);
		}
		if (active != null && active == true)
		{
		  query.active();
		}
		if (suspended != null && suspended == true)
		{
		  query.suspended();
		}
		if (!string.ReferenceEquals(incidentId, null))
		{
		  query.incidentId(incidentId);
		}
		if (!string.ReferenceEquals(incidentType, null))
		{
		  query.incidentType(incidentType);
		}
		if (!string.ReferenceEquals(incidentMessage, null))
		{
		  query.incidentMessage(incidentMessage);
		}
		if (!string.ReferenceEquals(incidentMessageLike, null))
		{
		  query.incidentMessageLike(incidentMessageLike);
		}
		if (tenantIdIn != null && tenantIdIn.Count > 0)
		{
		  query.tenantIdIn(tenantIdIn.ToArray());
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

		if (processVariables != null)
		{
		  foreach (VariableQueryParameterDto variableQueryParam in processVariables)
		  {
			string variableName = variableQueryParam.Name;
			string op = variableQueryParam.Operator;
			object variableValue = variableQueryParam.resolveValue(objectMapper);

			if (op.Equals(VariableQueryParameterDto.EQUALS_OPERATOR_NAME))
			{
			  query.processVariableValueEquals(variableName, variableValue);
			}
			else if (op.Equals(VariableQueryParameterDto.NOT_EQUALS_OPERATOR_NAME))
			{
			  query.processVariableValueNotEquals(variableName, variableValue);
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid process variable comparator specified: " + op);
			}
		  }
		}
	  }

	  protected internal override void applySortBy(ExecutionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_INSTANCE_ID_VALUE))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_DEFINITION_KEY_VALUE))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_DEFINITION_ID_VALUE))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }
	}

}