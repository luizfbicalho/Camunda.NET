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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;



	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceQueryDto : AbstractQueryDto<CaseInstanceQuery>
	{

	  protected internal const string SORT_BY_INSTANCE_ID_VALUE = "caseInstanceId";
	  protected internal const string SORT_BY_DEFINITION_KEY_VALUE = "caseDefinitionKey";
	  protected internal const string SORT_BY_DEFINITION_ID_VALUE = "caseDefinitionId";
	  protected internal const string SORT_BY_TENANT_ID = "tenantId";

	  protected internal static readonly IList<string> VALID_SORT_BY_VALUES;
	  static CaseInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string caseInstanceId;
	  protected internal string businessKey;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
	  protected internal string deploymentId;
	  protected internal string superProcessInstance;
	  protected internal string subProcessInstance;
	  protected internal string superCaseInstance;
	  protected internal string subCaseInstance;
	  protected internal IList<string> tenantIds;
	  protected internal bool? withoutTenantId;
	  protected internal bool? active;
	  protected internal bool? completed;
	  protected internal bool? terminated;

	  protected internal IList<VariableQueryParameterDto> variables;

	  public CaseInstanceQueryDto()
	  {
	  }

	  public CaseInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam("businessKey")]
	  public virtual string BusinessKey
	  {
		  set
		  {
			this.businessKey = value;
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

	  [CamundaQueryParam("deploymentId")]
	  public virtual string DeploymentId
	  {
		  set
		  {
			this.deploymentId = value;
		  }
	  }

	  [CamundaQueryParam("superProcessInstance")]
	  public virtual string SuperProcessInstance
	  {
		  set
		  {
			this.superProcessInstance = value;
		  }
	  }

	  [CamundaQueryParam("subProcessInstance")]
	  public virtual string SubProcessInstance
	  {
		  set
		  {
			this.subProcessInstance = value;
		  }
	  }

	  [CamundaQueryParam("superCaseInstance")]
	  public virtual string SuperCaseInstance
	  {
		  set
		  {
			this.superCaseInstance = value;
		  }
	  }

	  [CamundaQueryParam("subCaseInstance")]
	  public virtual string SubCaseInstance
	  {
		  set
		  {
			this.subCaseInstance = value;
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

	  [CamundaQueryParam(value : "variables", converter : org.camunda.bpm.engine.rest.dto.converter.VariableListConverter.class)]
	  public virtual IList<VariableQueryParameterDto> Variables
	  {
		  set
		  {
			this.variables = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override CaseInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.CaseService.createCaseInstanceQuery();
	  }

	  protected internal override void applyFilters(CaseInstanceQuery query)
	  {
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
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(superProcessInstance, null))
		{
		  query.superProcessInstanceId(superProcessInstance);
		}
		if (!string.ReferenceEquals(subProcessInstance, null))
		{
		  query.subProcessInstanceId(subProcessInstance);
		}
		if (!string.ReferenceEquals(superCaseInstance, null))
		{
		  query.superCaseInstanceId(superCaseInstance);
		}
		if (!string.ReferenceEquals(subCaseInstance, null))
		{
		  query.subCaseInstanceId(subCaseInstance);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (active != null && active == true)
		{
		  query.active();
		}
		if (completed != null && completed == true)
		{
		  query.completed();
		}
		if (terminated != null && terminated == true)
		{
		  query.terminated();
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

	  protected internal override void applySortBy(CaseInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_INSTANCE_ID_VALUE))
		{
		  query.orderByCaseInstanceId();
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