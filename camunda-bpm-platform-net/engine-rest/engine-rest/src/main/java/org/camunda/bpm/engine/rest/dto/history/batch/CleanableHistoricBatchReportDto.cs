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

	using CleanableHistoricBatchReport = org.camunda.bpm.engine.history.CleanableHistoricBatchReport;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class CleanableHistoricBatchReportDto : AbstractQueryDto<CleanableHistoricBatchReport>
	{

	  protected internal const string SORT_BY_FINISHED_VALUE = "finished";

	  public static readonly IList<string> VALID_SORT_BY_VALUES;

	  static CleanableHistoricBatchReportDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_FINISHED_VALUE);
	  }

	  public CleanableHistoricBatchReportDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override CleanableHistoricBatchReport createNewQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createCleanableHistoricBatchReport();
	  }

	  protected internal override void applyFilters(CleanableHistoricBatchReport query)
	  {
	  }

	  protected internal override void applySortBy(CleanableHistoricBatchReport query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_FINISHED_VALUE))
		{
		  query.orderByFinishedBatchOperation();
		}
	  }

	}

}