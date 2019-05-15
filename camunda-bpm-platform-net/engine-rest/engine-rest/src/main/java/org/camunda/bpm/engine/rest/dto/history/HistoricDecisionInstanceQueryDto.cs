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


	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricDecisionInstanceQueryDto : AbstractQueryDto<HistoricDecisionInstanceQuery>
	{

	  public const string SORT_BY_EVALUATION_TIME_VALUE = "evaluationTime";
	  public const string SORT_BY_TENANT_ID = "tenantId";

	  public static readonly IList<string> VALID_SORT_BY_VALUES;

	  static HistoricDecisionInstanceQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_EVALUATION_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string decisionInstanceId;
	  protected internal string[] decisionInstanceIdIn;

	  protected internal string decisionDefinitionId;
	  protected internal string[] decisionDefinitionIdIn;

	  protected internal string decisionDefinitionKey;
	  protected internal string[] decisionDefinitionKeyIn;

	  protected internal string decisionDefinitionName;
	  protected internal string decisionDefinitionNameLike;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processInstanceId;
	  protected internal string caseDefinitionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseInstanceId;
	  protected internal string[] activityIdIn;
	  protected internal string[] activityInstanceIdIn;
	  protected internal DateTime evaluatedBefore;
	  protected internal DateTime evaluatedAfter;
	  protected internal string userId;
	  protected internal bool? includeInputs;
	  protected internal bool? includeOutputs;
	  protected internal bool? disableBinaryFetching;
	  protected internal bool? disableCustomObjectDeserialization;
	  protected internal string rootDecisionInstanceId;
	  protected internal bool? rootDecisionInstancesOnly;
	  protected internal string decisionRequirementsDefinitionId;
	  protected internal string decisionRequirementsDefinitionKey;
	  protected internal IList<string> tenantIds;

	  public HistoricDecisionInstanceQueryDto()
	  {
	  }

	  public HistoricDecisionInstanceQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("decisionInstanceId")]
	  public virtual string DecisionInstanceId
	  {
		  set
		  {
			this.decisionInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] DecisionInstanceIdIn
	  {
		  set
		  {
			this.decisionInstanceIdIn = value;
		  }
	  }

	  [CamundaQueryParam("decisionDefinitionId")]
	  public virtual string DecisionDefinitionId
	  {
		  set
		  {
			this.decisionDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] DecisionDefinitionIdIn
	  {
		  set
		  {
			this.decisionDefinitionIdIn = value;
		  }
	  }

	  [CamundaQueryParam("decisionDefinitionKey")]
	  public virtual string DecisionDefinitionKey
	  {
		  set
		  {
			this.decisionDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionDefinitionKeyIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] DecisionDefinitionKeyIn
	  {
		  set
		  {
			this.decisionDefinitionKeyIn = value;
		  }
	  }

	  [CamundaQueryParam("decisionDefinitionName")]
	  public virtual string DecisionDefinitionName
	  {
		  set
		  {
			this.decisionDefinitionName = value;
		  }
	  }

	  [CamundaQueryParam("decisionDefinitionNameLike")]
	  public virtual string DecisionDefinitionNameLike
	  {
		  set
		  {
			this.decisionDefinitionNameLike = value;
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

	  [CamundaQueryParam("processInstanceId")]
	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
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

	  [CamundaQueryParam("caseDefinitionKey")]
	  public virtual string CaseDefinitionKey
	  {
		  set
		  {
			this.caseDefinitionKey = value;
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

	  [CamundaQueryParam(value:"activityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityIdIn
	  {
		  set
		  {
			this.activityIdIn = value;
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

	  [CamundaQueryParam(value : "evaluatedBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime EvaluatedBefore
	  {
		  set
		  {
			this.evaluatedBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "evaluatedAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime EvaluatedAfter
	  {
		  set
		  {
			this.evaluatedAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "userId")]
	  public virtual string UserId
	  {
		  set
		  {
			this.userId = value;
		  }
	  }

	  [CamundaQueryParam(value : "includeInputs", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeInputs
	  {
		  set
		  {
			this.includeInputs = value;
		  }
	  }

	  [CamundaQueryParam(value : "includeOutputs", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeOutputs
	  {
		  set
		  {
			this.includeOutputs = value;
		  }
	  }

	  [CamundaQueryParam(value : "disableBinaryFetching", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? DisableBinaryFetching
	  {
		  set
		  {
			this.disableBinaryFetching = value;
		  }
	  }

	  [CamundaQueryParam(value : "disableCustomObjectDeserialization", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? DisableCustomObjectDeserialization
	  {
		  set
		  {
			this.disableCustomObjectDeserialization = value;
		  }
	  }

	  [CamundaQueryParam(value : "rootDecisionInstanceId")]
	  public virtual string RootDecisionInstanceId
	  {
		  set
		  {
			this.rootDecisionInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value : "rootDecisionInstancesOnly", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? RootDecisionInstancesOnly
	  {
		  set
		  {
			this.rootDecisionInstancesOnly = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionRequirementsDefinitionId")]
	  public virtual string DecisionRequirementsDefinitionId
	  {
		  set
		  {
			this.decisionRequirementsDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionRequirementsDefinitionKey")]
	  public virtual string DecisionRequirementsDefinitionKey
	  {
		  set
		  {
			this.decisionRequirementsDefinitionKey = value;
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

	  protected internal override HistoricDecisionInstanceQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricDecisionInstanceQuery();
	  }

	  protected internal override void applyFilters(HistoricDecisionInstanceQuery query)
	  {
		if (!string.ReferenceEquals(decisionInstanceId, null))
		{
		  query.decisionInstanceId(decisionInstanceId);
		}
		if (decisionInstanceIdIn != null)
		{
		  query.decisionInstanceIdIn(decisionInstanceIdIn);
		}
		if (!string.ReferenceEquals(decisionDefinitionId, null))
		{
		  query.decisionDefinitionId(decisionDefinitionId);
		}
		if (decisionDefinitionIdIn != null)
		{
		  query.decisionDefinitionIdIn(decisionDefinitionIdIn);
		}
		if (!string.ReferenceEquals(decisionDefinitionKey, null))
		{
		  query.decisionDefinitionKey(decisionDefinitionKey);
		}
		if (decisionDefinitionKeyIn != null)
		{
		  query.decisionDefinitionKeyIn(decisionDefinitionKeyIn);
		}
		if (!string.ReferenceEquals(decisionDefinitionName, null))
		{
		  query.decisionDefinitionName(decisionDefinitionName);
		}
		if (!string.ReferenceEquals(decisionDefinitionNameLike, null))
		{
		  query.decisionDefinitionNameLike(decisionDefinitionNameLike);
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}
		if (!string.ReferenceEquals(caseDefinitionKey, null))
		{
		  query.caseDefinitionKey(caseDefinitionKey);
		}
		if (!string.ReferenceEquals(caseInstanceId, null))
		{
		  query.caseInstanceId(caseInstanceId);
		}
		if (activityIdIn != null)
		{
		  query.activityIdIn(activityIdIn);
		}
		if (activityInstanceIdIn != null)
		{
		  query.activityInstanceIdIn(activityInstanceIdIn);
		}
		if (evaluatedBefore != null)
		{
		  query.evaluatedBefore(evaluatedBefore);
		}
		if (evaluatedAfter != null)
		{
		  query.evaluatedAfter(evaluatedAfter);
		}
		if (!string.ReferenceEquals(userId, null))
		{
		  query.userId(userId);
		}
		if (TRUE.Equals(includeInputs))
		{
		  query.includeInputs();
		}
		if (TRUE.Equals(includeOutputs))
		{
		  query.includeOutputs();
		}
		if (TRUE.Equals(disableBinaryFetching))
		{
		  query.disableBinaryFetching();
		}
		if (TRUE.Equals(disableCustomObjectDeserialization))
		{
		  query.disableCustomObjectDeserialization();
		}
		if (!string.ReferenceEquals(rootDecisionInstanceId, null))
		{
		  query.rootDecisionInstanceId(rootDecisionInstanceId);
		}
		if (TRUE.Equals(rootDecisionInstancesOnly))
		{
		  query.rootDecisionInstancesOnly();
		}
		if (!string.ReferenceEquals(decisionRequirementsDefinitionId, null))
		{
		  query.decisionRequirementsDefinitionId(decisionRequirementsDefinitionId);
		}
		if (!string.ReferenceEquals(decisionRequirementsDefinitionKey, null))
		{
		  query.decisionRequirementsDefinitionKey(decisionRequirementsDefinitionKey);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(HistoricDecisionInstanceQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_EVALUATION_TIME_VALUE))
		{
		  query.orderByEvaluationTime();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}