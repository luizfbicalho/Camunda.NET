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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using StringSetConverter = org.camunda.bpm.engine.rest.dto.converter.StringSetConverter;
	using VariableListConverter = org.camunda.bpm.engine.rest.dto.converter.VariableListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;

	public class HistoricProcessInstanceQueryDto : AbstractQueryDto<HistoricProcessInstanceQuery>
	{

	  private const string SORT_BY_PROCESS_INSTANCE_ID_VALUE = "instanceId";
	  private const string SORT_BY_PROCESS_DEFINITION_ID_VALUE = "definitionId";
	  private const string SORT_BY_PROCESS_INSTANCE_BUSINESS_KEY_VALUE = "businessKey";
	  private const string SORT_BY_PROCESS_INSTANCE_START_TIME_VALUE = "startTime";
	  private const string SORT_BY_PROCESS_INSTANCE_END_TIME_VALUE = "endTime";
	  private const string SORT_BY_PROCESS_INSTANCE_DURATION_VALUE = "duration";
	  private const string SORT_BY_PROCESS_DEFINITION_KEY_VALUE = "definitionKey";
	  private const string SORT_BY_PROCESS_DEFINITION_NAME_VALUE = "definitionName";
	  private const string SORT_BY_PROCESS_DEFINITION_VERSION_VALUE = "definitionVersion";

	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricProcessInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_BUSINESS_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_START_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_END_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_DURATION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_VERSION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  private string processInstanceId;
	  private ISet<string> processInstanceIds;
	  private string processDefinitionId;
	  private string processDefinitionKey;
	  private string processDefinitionName;
	  private string processDefinitionNameLike;
	  private IList<string> processDefinitionKeyNotIn;
	  private string processInstanceBusinessKey;
	  private string processInstanceBusinessKeyLike;
	  private bool? rootProcessInstances;
	  private bool? finished;
	  private bool? unfinished;
	  private bool? withIncidents;
	  private bool? withRootIncidents;
	  private string incidentType;
	  private string incidentStatus;
	  private string incidentMessage;
	  private string incidentMessageLike;
	  private DateTime startedBefore;
	  private DateTime startedAfter;
	  private DateTime finishedBefore;
	  private DateTime finishedAfter;
	  private DateTime executedActivityAfter;
	  private DateTime executedActivityBefore;
	  private DateTime executedJobAfter;
	  private DateTime executedJobBefore;
	  private string startedBy;
	  private string superProcessInstanceId;
	  private string subProcessInstanceId;
	  private string superCaseInstanceId;
	  private string subCaseInstanceId;
	  private string caseInstanceId;
	  private IList<string> tenantIds;
	  private bool? withoutTenantId;
	  private IList<string> executedActivityIdIn;
	  private IList<string> activeActivityIdIn;
	  private bool? active;
	  private bool? suspended;
	  private bool? completed;
	  private bool? externallyTerminated;
	  private bool? internallyTerminated;

	  private IList<VariableQueryParameterDto> variables;

	  public HistoricProcessInstanceQueryDto()
	  {
	  }

	  public HistoricProcessInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam(value : "processInstanceIds", converter : org.camunda.bpm.engine.rest.dto.converter.StringSetConverter.class)]
	  public virtual ISet<string> ProcessInstanceIds
	  {
		  set
		  {
			this.processInstanceIds = value;
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


	  [CamundaQueryParam("processDefinitionName")]
	  public virtual string ProcessDefinitionName
	  {
		  set
		  {
			this.processDefinitionName = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionNameLike")]
	  public virtual string ProcessDefinitionNameLike
	  {
		  set
		  {
			this.processDefinitionNameLike = value;
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

	  [CamundaQueryParam(value : "processDefinitionKeyNotIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ProcessDefinitionKeyNotIn
	  {
		  set
		  {
			this.processDefinitionKeyNotIn = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKey")]
	  public virtual string ProcessInstanceBusinessKey
	  {
		  set
		  {
			this.processInstanceBusinessKey = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceBusinessKeyLike")]
	  public virtual string ProcessInstanceBusinessKeyLike
	  {
		  set
		  {
			this.processInstanceBusinessKeyLike = value;
		  }
	  }

	  [CamundaQueryParam(value : "rootProcessInstances", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? RootProcessInstances
	  {
		  set
		  {
			this.rootProcessInstances = value;
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

	  [CamundaQueryParam(value : "withIncidents", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithIncidents
	  {
		  set
		  {
			this.withIncidents = value;
		  }
	  }

	  [CamundaQueryParam(value : "withRootIncidents", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithRootIncidents
	  {
		  set
		  {
			this.withRootIncidents = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentStatus")]
	  public virtual string IncidentStatus
	  {
		  set
		  {
			this.incidentStatus = value;
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

	  [CamundaQueryParam("startedBy")]
	  public virtual string StartedBy
	  {
		  set
		  {
			this.startedBy = value;
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

	  [CamundaQueryParam("caseInstanceId")]
	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
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

	  [CamundaQueryParam(value : "executedActivityAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime ExecutedActivityAfter
	  {
		  set
		  {
			this.executedActivityAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "executedActivityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ExecutedActivityIdIn
	  {
		  set
		  {
			this.executedActivityIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "executedActivityBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime ExecutedActivityBefore
	  {
		  set
		  {
			this.executedActivityBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "activeActivityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ActiveActivityIdIn
	  {
		  set
		  {
			this.activeActivityIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "executedJobAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime ExecutedJobAfter
	  {
		  set
		  {
			this.executedJobAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "executedJobBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime ExecutedJobBefore
	  {
		  set
		  {
			this.executedJobBefore = value;
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

	  [CamundaQueryParam(value : "completed", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Completed
	  {
		  set
		  {
			this.completed = value;
		  }
	  }

	  [CamundaQueryParam(value : "externallyTerminated", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? ExternallyTerminated
	  {
		  set
		  {
			this.externallyTerminated = value;
		  }
	  }

	  [CamundaQueryParam(value : "internallyTerminated", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? InternallyTerminated
	  {
		  set
		  {
			this.internallyTerminated = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricProcessInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricProcessInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricProcessInstanceQuery query)
	  {

		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (processInstanceIds != null)
		{
		  query.processInstanceIds(processInstanceIds);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(processDefinitionName, null))
		{
		  query.processDefinitionName(processDefinitionName);
		}
		if (!string.ReferenceEquals(processDefinitionNameLike, null))
		{
		  query.processDefinitionNameLike(processDefinitionNameLike);
		}
		if (processDefinitionKeyNotIn != null)
		{
		  query.processDefinitionKeyNotIn(processDefinitionKeyNotIn);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKey, null))
		{
		  query.processInstanceBusinessKey(processInstanceBusinessKey);
		}
		if (!string.ReferenceEquals(processInstanceBusinessKeyLike, null))
		{
		  query.processInstanceBusinessKeyLike(processInstanceBusinessKeyLike);
		}
		if (rootProcessInstances != null && rootProcessInstances)
		{
		  query.rootProcessInstances();
		}
		if (finished != null && finished)
		{
		  query.finished();
		}
		if (unfinished != null && unfinished)
		{
		  query.unfinished();
		}
		if (withIncidents != null && withIncidents)
		{
		  query.withIncidents();
		}
		if (withRootIncidents != null && withRootIncidents)
		{
		  query.withRootIncidents();
		}
		if (!string.ReferenceEquals(incidentStatus, null))
		{
		  query.incidentStatus(incidentStatus);
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
		if (!string.ReferenceEquals(startedBy, null))
		{
		  query.startedBy(startedBy);
		}
		if (!string.ReferenceEquals(superProcessInstanceId, null))
		{
		  query.superProcessInstanceId(superProcessInstanceId);
		}
		if (!string.ReferenceEquals(subProcessInstanceId, null))
		{
		  query.subProcessInstanceId(subProcessInstanceId);
		}
		if (!string.ReferenceEquals(superCaseInstanceId, null))
		{
		  query.superCaseInstanceId(superCaseInstanceId);
		}
		if (!string.ReferenceEquals(subCaseInstanceId, null))
		{
		  query.subCaseInstanceId(subCaseInstanceId);
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
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

		if (executedActivityAfter != null)
		{
		  query.executedActivityAfter(executedActivityAfter);
		}

		if (executedActivityBefore != null)
		{
		  query.executedActivityBefore(executedActivityBefore);
		}

		if (executedActivityIdIn != null && executedActivityIdIn.Count > 0)
		{
		  query.executedActivityIdIn(executedActivityIdIn.ToArray());
		}

		if (activeActivityIdIn != null && activeActivityIdIn.Count > 0)
		{
		  query.activeActivityIdIn(activeActivityIdIn.ToArray());
		}

		if (executedJobAfter != null)
		{
		  query.executedJobAfter(executedJobAfter);
		}

		if (executedJobBefore != null)
		{
		  query.executedJobBefore(executedJobBefore);
		}

		if (active != null && active)
		{
		  query.active();
		}
		if (suspended != null && suspended)
		{
		  query.suspended();
		}
		if (completed != null && completed)
		{
		  query.completed();
		}
		if (externallyTerminated != null && externallyTerminated)
		{
		  query.externallyTerminated();
		}
		if (internallyTerminated != null && internallyTerminated)
		{
		  query.internallyTerminated();
		}
	  }

	  protected internal override void applySortBy(HistoricProcessInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID_VALUE))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_ID_VALUE))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_KEY_VALUE))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_NAME_VALUE))
		{
		  query.orderByProcessDefinitionName();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_VERSION_VALUE))
		{
		  query.orderByProcessDefinitionVersion();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_BUSINESS_KEY_VALUE))
		{
		  query.orderByProcessInstanceBusinessKey();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_START_TIME_VALUE))
		{
		  query.orderByProcessInstanceStartTime();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_END_TIME_VALUE))
		{
		  query.orderByProcessInstanceEndTime();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_DURATION_VALUE))
		{
		  query.orderByProcessInstanceDuration();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}