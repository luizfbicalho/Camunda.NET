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

	using HistoricCaseActivityStatistics = org.camunda.bpm.engine.history.HistoricCaseActivityStatistics;
	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;
	using HistoricCaseActivityStatisticsDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseActivityStatisticsDto;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CleanableHistoricCaseInstanceReportDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricCaseInstanceReportDto;
	using CleanableHistoricCaseInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricCaseInstanceReportResultDto;
	using HistoricCaseDefinitionRestService = org.camunda.bpm.engine.rest.history.HistoricCaseDefinitionRestService;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricCaseDefinitionRestServiceImpl : HistoricCaseDefinitionRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricCaseDefinitionRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual IList<HistoricCaseActivityStatisticsDto> getHistoricCaseActivityStatistics(string caseDefinitionId)
	  {
		HistoryService historyService = processEngine.HistoryService;

		IList<HistoricCaseActivityStatistics> statistics = historyService.createHistoricCaseActivityStatisticsQuery(caseDefinitionId).list();

		IList<HistoricCaseActivityStatisticsDto> result = new List<HistoricCaseActivityStatisticsDto>();
		foreach (HistoricCaseActivityStatistics currentStatistics in statistics)
		{
		  result.Add(HistoricCaseActivityStatisticsDto.fromHistoricCaseActivityStatistics(currentStatistics));
		}

		return result;
	  }

	  public virtual IList<CleanableHistoricCaseInstanceReportResultDto> getCleanableHistoricCaseInstanceReport(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CleanableHistoricCaseInstanceReportDto queryDto = new CleanableHistoricCaseInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		CleanableHistoricCaseInstanceReport query = queryDto.toQuery(processEngine);

		IList<CleanableHistoricCaseInstanceReportResult> reportResult;
		if (firstResult != null || maxResults != null)
		{
		reportResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		reportResult = query.list();
		}

		return CleanableHistoricCaseInstanceReportResultDto.convert(reportResult);
	  }

	  private IList<CleanableHistoricCaseInstanceReportResult> executePaginatedQuery(CleanableHistoricCaseInstanceReport query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getCleanableHistoricCaseInstanceReportCount(UriInfo uriInfo)
	  {
		CleanableHistoricCaseInstanceReportDto queryDto = new CleanableHistoricCaseInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		queryDto.ObjectMapper = objectMapper;
		CleanableHistoricCaseInstanceReport query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }
	}

}