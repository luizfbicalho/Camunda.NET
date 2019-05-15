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
namespace org.camunda.bpm.engine.rest.impl.history
{
	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using HistoricActivityStatisticsDto = org.camunda.bpm.engine.rest.dto.history.HistoricActivityStatisticsDto;
	using CleanableHistoricProcessInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricProcessInstanceReportResultDto;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CleanableHistoricProcessInstanceReportDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricProcessInstanceReportDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using HistoricProcessDefinitionRestService = org.camunda.bpm.engine.rest.history.HistoricProcessDefinitionRestService;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;


	public class HistoricProcessDefinitionRestServiceImpl : AbstractRestProcessEngineAware, HistoricProcessDefinitionRestService
	{

	  public const string QUERY_PARAM_STARTED_AFTER = "startedAfter";
	  public const string QUERY_PARAM_STARTED_BEFORE = "startedBefore";
	  public const string QUERY_PARAM_FINISHED_AFTER = "finishedAfter";
	  public const string QUERY_PARAM_FINISHED_BEFORE = "finishedBefore";

	  public HistoricProcessDefinitionRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine) : base(processEngine.Name, objectMapper)
	  {
	  }

	  public virtual IList<HistoricActivityStatisticsDto> getHistoricActivityStatistics(UriInfo uriInfo, string processDefinitionId, bool? includeCanceled, bool? includeFinished, bool? includeCompleteScope, string sortBy, string sortOrder)
	  {
		HistoryService historyService = processEngine.HistoryService;

		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		if (includeCanceled != null && includeCanceled)
		{
		  query.includeCanceled();
		}

		if (includeFinished != null && includeFinished)
		{
		  query.includeFinished();
		}

		if (includeCompleteScope != null && includeCompleteScope)
		{
		  query.includeCompleteScope();
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.ws.rs.core.MultivaluedMap<String, String> queryParameters = uriInfo.getQueryParameters();
		MultivaluedMap<string, string> queryParameters = uriInfo.QueryParameters;

		DateConverter dateConverter = new DateConverter();
		dateConverter.ObjectMapper = objectMapper;

		if (queryParameters.getFirst(QUERY_PARAM_STARTED_AFTER) != null)
		{
		  DateTime startedAfter = dateConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_STARTED_AFTER));
		  query.startedAfter(startedAfter);
		}

		if (queryParameters.getFirst(QUERY_PARAM_STARTED_BEFORE) != null)
		{
		  DateTime startedBefore = dateConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_STARTED_BEFORE));
		  query.startedBefore(startedBefore);
		}

		if (queryParameters.getFirst(QUERY_PARAM_FINISHED_AFTER) != null)
		{
		  DateTime finishedAfter = dateConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_FINISHED_AFTER));
		  query.finishedAfter(finishedAfter);
		}

		if (queryParameters.getFirst(QUERY_PARAM_FINISHED_BEFORE) != null)
		{
		  DateTime finishedBefore = dateConverter.convertQueryParameterToType(queryParameters.getFirst(QUERY_PARAM_FINISHED_BEFORE));
		  query.finishedBefore(finishedBefore);
		}

		setSortOptions(query, sortOrder, sortBy);

		IList<HistoricActivityStatisticsDto> result = new List<HistoricActivityStatisticsDto>();

		IList<HistoricActivityStatistics> statistics = query.list();

		foreach (HistoricActivityStatistics currentStatistics in statistics)
		{
		  result.Add(HistoricActivityStatisticsDto.fromHistoricActivityStatistics(currentStatistics));
		}

		return result;
	  }

	  private void setSortOptions(HistoricActivityStatisticsQuery query, string sortOrder, string sortBy)
	  {
		bool sortOptionsValid = (!string.ReferenceEquals(sortBy, null) && !string.ReferenceEquals(sortOrder, null)) || (string.ReferenceEquals(sortBy, null) && string.ReferenceEquals(sortOrder, null));

		if (!sortOptionsValid)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Only a single sorting parameter specified. sortBy and sortOrder required");
		}

		if (!string.ReferenceEquals(sortBy, null))
		{
		  if (sortBy.Equals("activityId"))
		  {
			query.orderByActivityId();
		  }
		  else
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "sortBy parameter has invalid value: " + sortBy);
		  }
		}

		if (!string.ReferenceEquals(sortOrder, null))
		{
		  if (sortOrder.Equals("asc"))
		  {
			query.asc();
		  }
		  else
		  {
		  if (sortOrder.Equals("desc"))
		  {
			query.desc();
		  }
		  else
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "sortOrder parameter has invalid value: " + sortOrder);
		  }
		  }
		}

	  }

	  public virtual IList<CleanableHistoricProcessInstanceReportResultDto> getCleanableHistoricProcessInstanceReport(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CleanableHistoricProcessInstanceReportDto queryDto = new CleanableHistoricProcessInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		CleanableHistoricProcessInstanceReport query = queryDto.toQuery(processEngine);

		IList<CleanableHistoricProcessInstanceReportResult> reportResult;
		if (firstResult != null || maxResults != null)
		{
		reportResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		reportResult = query.list();
		}

		return CleanableHistoricProcessInstanceReportResultDto.convert(reportResult);
	  }

	  private IList<CleanableHistoricProcessInstanceReportResult> executePaginatedQuery(CleanableHistoricProcessInstanceReport query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getCleanableHistoricProcessInstanceReportCount(UriInfo uriInfo)
	  {
		CleanableHistoricProcessInstanceReportDto queryDto = new CleanableHistoricProcessInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		queryDto.ObjectMapper = objectMapper;
		CleanableHistoricProcessInstanceReport query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	}

}