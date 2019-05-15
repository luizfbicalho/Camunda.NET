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

	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class IncidentQueryDto : AbstractQueryDto<IncidentQuery>
	{

	  private const string SORT_BY_INCIDENT_ID = "incidentId";
	  private const string SORT_BY_INCIDENT_MESSAGE = "incidentMessage";
	  private const string SORT_BY_INCIDENT_TIMESTAMP = "incidentTimestamp";
	  private const string SORT_BY_INCIDENT_TYPE = "incidentType";
	  private const string SORT_BY_EXECUTION_ID = "executionId";
	  private const string SORT_BY_ACTIVITY_ID = "activityId";
	  private const string SORT_BY_PROCESS_INSTANCE_ID = "processInstanceId";
	  private const string SORT_BY_PROCESS_DEFINITION_ID = "processDefinitionId";
	  private const string SORT_BY_CAUSE_INCIDENT_ID = "causeIncidentId";
	  private const string SORT_BY_ROOT_CAUSE_INCIDENT_ID = "rootCauseIncidentId";
	  private const string SORT_BY_CONFIGURATION = "configuration";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static IncidentQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_INCIDENT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_INCIDENT_MESSAGE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_INCIDENT_TIMESTAMP);
		VALID_SORT_BY_VALUES.Add(SORT_BY_INCIDENT_TYPE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CAUSE_INCIDENT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ROOT_CAUSE_INCIDENT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_CONFIGURATION);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string incidentId;
	  protected internal string incidentType;
	  protected internal string incidentMessage;
	  protected internal string processDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string activityId;
	  protected internal string causeIncidentId;
	  protected internal string rootCauseIncidentId;
	  protected internal string configuration;
	  protected internal IList<string> tenantIds;
	  protected internal IList<string> jobDefinitionIds;

	  public IncidentQueryDto()
	  {
	  }

	  public IncidentQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("incidentId")]
	  public virtual string IncidentId
	  {
		  set
		  {
			this.incidentId = value;
		  }
	  }

	  [CamundaQueryParam("incidentType")]
	  public virtual string IncidentType
	  {
		  set
		  {
			this.incidentType = value;
		  }
	  }

	  [CamundaQueryParam("incidentMessage")]
	  public virtual string IncidentMessage
	  {
		  set
		  {
			this.incidentMessage = value;
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

	  [CamundaQueryParam("executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
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

	  [CamundaQueryParam("causeIncidentId")]
	  public virtual string CauseIncidentId
	  {
		  set
		  {
			this.causeIncidentId = value;
		  }
	  }

	  [CamundaQueryParam("rootCauseIncidentId")]
	  public virtual string RootCauseIncidentId
	  {
		  set
		  {
			this.rootCauseIncidentId = value;
		  }
	  }

	  [CamundaQueryParam("configuration")]
	  public virtual string Configuration
	  {
		  set
		  {
			this.configuration = value;
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

	  [CamundaQueryParam(value : "jobDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> JobDefinitionIdIn
	  {
		  set
		  {
			this.jobDefinitionIds = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }


	  protected internal override IncidentQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RuntimeService.createIncidentQuery();
	  }

	  protected internal override void applyFilters(IncidentQuery query)
	  {
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
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (!string.ReferenceEquals(activityId, null))
		{
		  query.activityId(activityId);
		}
		if (!string.ReferenceEquals(causeIncidentId, null))
		{
		  query.causeIncidentId(causeIncidentId);
		}
		if (!string.ReferenceEquals(rootCauseIncidentId, null))
		{
		  query.rootCauseIncidentId(rootCauseIncidentId);
		}
		if (!string.ReferenceEquals(configuration, null))
		{
		  query.configuration(configuration);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (jobDefinitionIds != null && jobDefinitionIds.Count > 0)
		{
		  query.jobDefinitionIdIn(jobDefinitionIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(IncidentQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_INCIDENT_ID))
		{
		  query.orderByIncidentId();
		}
		else if (sortBy.Equals(SORT_BY_INCIDENT_MESSAGE))
		{
		  query.orderByIncidentMessage();
		}
		else if (sortBy.Equals(SORT_BY_INCIDENT_TIMESTAMP))
		{
		  query.orderByIncidentTimestamp();
		}
		else if (sortBy.Equals(SORT_BY_INCIDENT_TYPE))
		{
		  query.orderByIncidentType();
		}
		else if (sortBy.Equals(SORT_BY_EXECUTION_ID))
		{
		  query.orderByExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_ID))
		{
		  query.orderByActivityId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_ID))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_CAUSE_INCIDENT_ID))
		{
		  query.orderByCauseIncidentId();
		}
		else if (sortBy.Equals(SORT_BY_ROOT_CAUSE_INCIDENT_ID))
		{
		  query.orderByRootCauseIncidentId();
		}
		else if (sortBy.Equals(SORT_BY_CONFIGURATION))
		{
		  query.orderByConfiguration();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}