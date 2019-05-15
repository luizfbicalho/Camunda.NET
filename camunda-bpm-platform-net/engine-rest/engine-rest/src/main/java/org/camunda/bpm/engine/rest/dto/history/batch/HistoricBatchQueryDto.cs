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
namespace org.camunda.bpm.engine.rest.dto.history.batch
{

	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricBatchQueryDto : AbstractQueryDto<HistoricBatchQuery>
	{

	  private const string SORT_BY_BATCH_ID_VALUE = "batchId";
	  private const string SORT_BY_BATCH_START_TIME_VALUE = "startTime";
	  private const string SORT_BY_BATCH_END_TIME_VALUE = "endTime";
	  private const string SORT_BY_TENANT_ID_VALUE = "tenantId";

	  protected internal string batchId;
	  protected internal string type;
	  protected internal bool? completed;
	  protected internal IList<string> tenantIds;
	  protected internal bool? withoutTenantId;

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static HistoricBatchQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_BATCH_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_BATCH_START_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_BATCH_END_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID_VALUE);
	  }

	  public HistoricBatchQueryDto()
	  {
	  }

	  public HistoricBatchQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("batchId")]
	  public virtual string BatchId
	  {
		  set
		  {
			this.batchId = value;
		  }
	  }

	  [CamundaQueryParam("type")]
	  public virtual string Type
	  {
		  set
		  {
			this.type = value;
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

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override HistoricBatchQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricBatchQuery();
	  }

	  protected internal virtual void applyFilters(HistoricBatchQuery query)
	  {
		if (!string.ReferenceEquals(batchId, null))
		{
		  query.batchId(batchId);
		}
		if (!string.ReferenceEquals(type, null))
		{
		  query.type(type);
		}
		if (completed != null)
		{
		  query.completed(completed.Value);
		}
		if (true.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal virtual void applySortBy(HistoricBatchQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_BATCH_ID_VALUE))
		{
		  query.orderById();
		}
		if (sortBy.Equals(SORT_BY_BATCH_START_TIME_VALUE))
		{
		  query.orderByStartTime();
		}
		if (sortBy.Equals(SORT_BY_BATCH_END_TIME_VALUE))
		{
		  query.orderByEndTime();
		}
		if (sortBy.Equals(SORT_BY_TENANT_ID_VALUE))
		{
		  query.orderByTenantId();
		}
	  }

	}

}