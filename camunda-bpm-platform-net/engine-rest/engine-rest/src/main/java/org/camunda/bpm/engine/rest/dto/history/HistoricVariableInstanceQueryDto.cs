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


	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricVariableInstanceQueryDto : AbstractQueryDto<HistoricVariableInstanceQuery>
	{

	  private const string SORT_BY_PROCESS_INSTANCE_ID_VALUE = "instanceId";
	  private const string SORT_BY_VARIABLE_NAME_VALUE = "variableName";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricVariableInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VARIABLE_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string caseInstanceId;
	  protected internal string variableName;
	  protected internal string variableNameLike;
	  protected internal object variableValue;
	  protected internal string[] variableTypeIn;
	  protected internal string[] executionIdIn;
	  protected internal string[] taskIdIn;
	  protected internal string[] activityInstanceIdIn;
	  protected internal string[] caseExecutionIdIn;
	  protected internal string[] caseActivityIdIn;
	  protected internal string[] processInstanceIdIn;
	  protected internal IList<string> tenantIds;
	  protected internal bool includeDeleted;

	  public HistoricVariableInstanceQueryDto()
	  {
	  }

	  public HistoricVariableInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
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

	  [CamundaQueryParam("caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
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

	  [CamundaQueryParam("variableValue")]
	  public virtual object VariableValue
	  {
		  set
		  {
			this.variableValue = value;
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

	  [CamundaQueryParam(value:"executionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ExecutionIdIn
	  {
		  set
		  {
			this.executionIdIn = value;
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

	  [CamundaQueryParam(value:"processInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessInstanceIdIn
	  {
		  set
		  {
			this.processInstanceIdIn = value;
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

	  [CamundaQueryParam(value:"caseExecutionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CaseExecutionIdIn
	  {
		  set
		  {
			this.caseExecutionIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"caseActivityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CaseActivityIdIn
	  {
		  set
		  {
			this.caseActivityIdIn = value;
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

	  public virtual bool IncludeDeleted
	  {
		  get
		  {
			return includeDeleted;
		  }
		  set
		  {
			this.includeDeleted = value;
		  }
	  }


	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricVariableInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricVariableInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricVariableInstanceQuery query)
	  {
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
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (!string.ReferenceEquals(variableName, null))
		{
		  query.variableName(variableName);
		}
		if (!string.ReferenceEquals(variableNameLike, null))
		{
		  query.variableNameLike(variableNameLike);
		}
		if (variableValue != null)
		{
		  if (!string.ReferenceEquals(variableName, null))
		  {
			query.variableValueEquals(variableName, variableValue);
		  }
		  else
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "Only a single variable value parameter specified: variable name and value are required to be able to query after a specific variable value.");
		  }
		}
		if (variableTypeIn != null && variableTypeIn.Length > 0)
		{
		  query.variableTypeIn(variableTypeIn);
		}

		if (executionIdIn != null && executionIdIn.Length > 0)
		{
		  query.executionIdIn(executionIdIn);
		}
		if (taskIdIn != null && taskIdIn.Length > 0)
		{
		  query.taskIdIn(taskIdIn);
		}
		if (processInstanceIdIn != null && processInstanceIdIn.Length > 0)
		{
		  query.processInstanceIdIn(processInstanceIdIn);
		}
		if (activityInstanceIdIn != null && activityInstanceIdIn.Length > 0)
		{
		  query.activityInstanceIdIn(activityInstanceIdIn);
		}
		if (caseExecutionIdIn != null && caseExecutionIdIn.Length > 0)
		{
		  query.caseExecutionIdIn(caseExecutionIdIn);
		}
		if (caseActivityIdIn != null && caseActivityIdIn.Length > 0)
		{
		  query.caseActivityIdIn(caseActivityIdIn);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (includeDeleted)
		{
		  query.includeDeleted();
		}
	  }

	  protected internal override void applySortBy(HistoricVariableInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID_VALUE))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_VARIABLE_NAME_VALUE))
		{
		  query.orderByVariableName();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}