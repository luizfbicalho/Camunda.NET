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

	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class CleanableHistoricCaseInstanceReportDto : AbstractQueryDto<CleanableHistoricCaseInstanceReport>
	{

	  protected internal string[] caseDefinitionIdIn;
	  protected internal string[] caseDefinitionKeyIn;
	  protected internal string[] tenantIdIn;
	  protected internal bool? withoutTenantId;
	  protected internal bool? compact;

	  protected internal const string SORT_BY_FINISHED_VALUE = "finished";

	  public static readonly IList<string> VALID_SORT_BY_VALUES;

	  static CleanableHistoricCaseInstanceReportDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_FINISHED_VALUE);
	  }

	  public CleanableHistoricCaseInstanceReportDto()
	  {
	  }

	  public CleanableHistoricCaseInstanceReportDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam(value : "caseDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CaseDefinitionIdIn
	  {
		  set
		  {
			this.caseDefinitionIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseDefinitionKeyIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] CaseDefinitionKeyIn
	  {
		  set
		  {
			this.caseDefinitionKeyIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] TenantIdIn
	  {
		  set
		  {
			this.tenantIdIn = value;
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

	  [CamundaQueryParam(value : "compact", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Compact
	  {
		  set
		  {
			this.compact = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override CleanableHistoricCaseInstanceReport createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createCleanableHistoricCaseInstanceReport();
	  }

	  protected internal override void applyFilters(CleanableHistoricCaseInstanceReport query)
	  {
		if (caseDefinitionIdIn != null && caseDefinitionIdIn.Length > 0)
		{
		  query.caseDefinitionIdIn(caseDefinitionIdIn);
		}
		if (caseDefinitionKeyIn != null && caseDefinitionKeyIn.Length > 0)
		{
		  query.caseDefinitionKeyIn(caseDefinitionKeyIn);
		}
		if (true.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (tenantIdIn != null && tenantIdIn.Length > 0)
		{
		  query.tenantIdIn(tenantIdIn);
		}
		if (true.Equals(compact))
		{
		  query.compact();
		}

	  }

	  protected internal override void applySortBy(CleanableHistoricCaseInstanceReport query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_FINISHED_VALUE))
		{
		  query.orderByFinished();
		}
	  }
	}

}