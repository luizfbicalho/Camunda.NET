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
namespace org.camunda.bpm.engine.rest.impl.history
{

	using CleanableHistoricDecisionInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReport;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CleanableHistoricDecisionInstanceReportDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricDecisionInstanceReportDto;
	using CleanableHistoricDecisionInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricDecisionInstanceReportResultDto;
	using HistoricDecisionDefinitionRestService = org.camunda.bpm.engine.rest.history.HistoricDecisionDefinitionRestService;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricDecisionDefinitionRestServiceImpl : HistoricDecisionDefinitionRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricDecisionDefinitionRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual IList<CleanableHistoricDecisionInstanceReportResultDto> getCleanableHistoricDecisionInstanceReport(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CleanableHistoricDecisionInstanceReportDto queryDto = new CleanableHistoricDecisionInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		CleanableHistoricDecisionInstanceReport query = queryDto.toQuery(processEngine);

		IList<CleanableHistoricDecisionInstanceReportResult> reportResult;
		if (firstResult != null || maxResults != null)
		{
		  reportResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		  else
		  {
		  reportResult = query.list();
		  }

		return CleanableHistoricDecisionInstanceReportResultDto.convert(reportResult);
	  }

	  private IList<CleanableHistoricDecisionInstanceReportResult> executePaginatedQuery(CleanableHistoricDecisionInstanceReport query, int? firstResult, int? maxResults)
	  {
		if (firstResult == null)
		{
		  firstResult = 0;
		}
		if (maxResults == null)
		{
		  maxResults = int.MaxValue;
		}
		return query.listPage(firstResult, maxResults);
	  }

	  public virtual CountResultDto getCleanableHistoricDecisionInstanceReportCount(UriInfo uriInfo)
	  {
		CleanableHistoricDecisionInstanceReportDto queryDto = new CleanableHistoricDecisionInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		queryDto.ObjectMapper = objectMapper;
		CleanableHistoricDecisionInstanceReport query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }
	}

}