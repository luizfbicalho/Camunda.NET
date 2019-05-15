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
	using StringSetConverter = org.camunda.bpm.engine.rest.dto.converter.StringSetConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ProcessInstanceQueryDto : AbstractQueryDto<ProcessInstanceQuery>
	{

	  private const string SORT_BY_INSTANCE_ID_VALUE = "instanceId";
	  private const string SORT_BY_DEFINITION_KEY_VALUE = "definitionKey";
	  private const string SORT_BY_DEFINITION_ID_VALUE = "definitionId";
	  private const string SORT_BY_TENANT_ID = "tenantId";
	  private const string SORT_BY_BUSINESS_KEY = "businessKey";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static ProcessInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_BUSINESS_KEY);
	  }

	  private string deploymentId;
	  private string processDefinitionKey;
	  private string businessKey;
	  private string businessKeyLike;
	  private string caseInstanceId;
	  private string processDefinitionId;
	  private string superProcessInstance;
	  private string subProcessInstance;
	  private string superCaseInstance;
	  private string subCaseInstance;
	  private bool? active;
	  private bool? suspended;
	  private ISet<string> processInstanceIds;
	  private bool? withIncident;
	  private string incidentId;
	  private string incidentType;
	  private string incidentMessage;
	  private string incidentMessageLike;
	  private IList<string> tenantIds;
	  private bool? withoutTenantId;
	  private IList<string> activityIds;
	  private bool? rootProcessInstances;
	  private bool? leafProcessInstances;
	  private bool? isProcessDefinitionWithoutTenantId;

	  private IList<VariableQueryParameterDto> variables;

	  public ProcessInstanceQueryDto()
	  {

	  }

	  public ProcessInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  public virtual ISet<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
		  set
		  {
				this.processInstanceIds = value;
		  }
	  }


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual string BusinessKeyLike
	  {
		  get
		  {
			return businessKeyLike;
		  }
		  set
		  {
			this.businessKeyLike = value;
		  }
	  }


	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string SuperProcessInstance
	  {
		  get
		  {
			return superProcessInstance;
		  }
		  set
		  {
			this.superProcessInstance = value;
		  }
	  }


	  public virtual string SubProcessInstance
	  {
		  get
		  {
			return subProcessInstance;
		  }
		  set
		  {
			this.subProcessInstance = value;
		  }
	  }


	  public virtual string SuperCaseInstance
	  {
		  get
		  {
			return superCaseInstance;
		  }
		  set
		  {
			this.superCaseInstance = value;
		  }
	  }


	  public virtual string SubCaseInstance
	  {
		  get
		  {
			return subCaseInstance;
		  }
		  set
		  {
			this.subCaseInstance = value;
		  }
	  }


	  public virtual bool? Active
	  {
		  get
		  {
			return active;
		  }
		  set
		  {
			this.active = value;
		  }
	  }


	  public virtual bool? Suspended
	  {
		  get
		  {
			return suspended;
		  }
		  set
		  {
			this.suspended = value;
		  }
	  }


	  public virtual IList<VariableQueryParameterDto> Variables
	  {
		  get
		  {
			return variables;
		  }
		  set
		  {
			this.variables = value;
		  }
	  }


	  public virtual bool? WithIncident
	  {
		  get
		  {
			return withIncident;
		  }
		  set
		  {
			this.withIncident = value;
		  }
	  }


	  public virtual string IncidentId
	  {
		  get
		  {
			return incidentId;
		  }
		  set
		  {
			this.incidentId = value;
		  }
	  }


	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType;
		  }
		  set
		  {
			this.incidentType = value;
		  }
	  }


	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage;
		  }
		  set
		  {
			this.incidentMessage = value;
		  }
	  }


	  public virtual string IncidentMessageLike
	  {
		  get
		  {
			return incidentMessageLike;
		  }
		  set
		  {
			this.incidentMessageLike = value;
		  }
	  }


	  public virtual IList<string> TenantIdIn
	  {
		  get
		  {
			return tenantIds;
		  }
		  set
		  {
			this.tenantIds = value;
		  }
	  }


	  public virtual bool? WithoutTenantId
	  {
		  get
		  {
			return withoutTenantId;
		  }
		  set
		  {
			this.withoutTenantId = value;
		  }
	  }


	  public virtual IList<string> ActivityIds
	  {
		  get
		  {
			return activityIds;
		  }
	  }

	  [CamundaQueryParam(value : "activityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ActivityIdIn
	  {
		  set
		  {
			this.activityIds = value;
		  }
	  }

	  public virtual bool? RootProcessInstances
	  {
		  get
		  {
			return rootProcessInstances;
		  }
		  set
		  {
			this.rootProcessInstances = value;
		  }
	  }



	  public virtual bool? LeafProcessInstances
	  {
		  get
		  {
			return leafProcessInstances;
		  }
		  set
		  {
			this.leafProcessInstances = value;
		  }
	  }


	  public virtual bool? ProcessDefinitionWithoutTenantId
	  {
		  get
		  {
			return isProcessDefinitionWithoutTenantId;
		  }
		  set
		  {
			this.isProcessDefinitionWithoutTenantId = value;
		  }
	  }


	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override ProcessInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RuntimeService.createProcessInstanceQuery();
	  }

	  protected internal override void applyFilters(ProcessInstanceQuery query)
	  {

		if (processInstanceIds != null)
		{
		  query.processInstanceIds(processInstanceIds);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(businessKey, null))
		{
		  query.processInstanceBusinessKey(businessKey);
		}
		if (!string.ReferenceEquals(businessKeyLike, null))
		{
		  query.processInstanceBusinessKeyLike(businessKeyLike);
		}
		if (TRUE.Equals(withIncident))
		{
		  query.withIncident();
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
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
		if (TRUE.Equals(active))
		{
		  query.active();
		}
		if (TRUE.Equals(suspended))
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
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (activityIds != null && activityIds.Count > 0)
		{
		  query.activityIdIn(activityIds.ToArray());
		}
		if (TRUE.Equals(rootProcessInstances))
		{
		  query.rootProcessInstances();
		}
		if (TRUE.Equals(leafProcessInstances))
		{
		  query.leafProcessInstances();
		}
		if (TRUE.Equals(isProcessDefinitionWithoutTenantId))
		{
		  query.processDefinitionWithoutTenantId();
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

	  protected internal override void applySortBy(ProcessInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
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
		else if (sortBy.Equals(SORT_BY_BUSINESS_KEY))
		{
		  query.orderByBusinessKey();
		}
	  }

	}

}