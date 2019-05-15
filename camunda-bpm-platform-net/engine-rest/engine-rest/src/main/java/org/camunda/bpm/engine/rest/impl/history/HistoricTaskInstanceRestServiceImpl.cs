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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using ReportResult = org.camunda.bpm.engine.history.ReportResult;
	using AbstractReportDto = org.camunda.bpm.engine.rest.dto.AbstractReportDto;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricTaskInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto;
	using HistoricTaskInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceQueryDto;
	using HistoricTaskInstanceReportQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceReportQueryDto;
	using HistoricTaskInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceReportResultDto;
	using ReportResultDto = org.camunda.bpm.engine.rest.dto.history.ReportResultDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using HistoricTaskInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricTaskInstanceRestService;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricTaskInstanceRestServiceImpl : HistoricTaskInstanceRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricTaskInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual IList<HistoricTaskInstanceDto> getHistoricTaskInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricTaskInstanceQueryDto queryDto = new HistoricTaskInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricTaskInstances(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricTaskInstanceDto> queryHistoricTaskInstances(HistoricTaskInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricTaskInstanceQuery query = queryDto.toQuery(processEngine);

		IList<HistoricTaskInstance> match;
		if (firstResult != null || maxResults != null)
		{
		  match = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  match = query.list();
		}

		IList<HistoricTaskInstanceDto> result = new List<HistoricTaskInstanceDto>();
		foreach (HistoricTaskInstance taskInstance in match)
		{
		  HistoricTaskInstanceDto taskInstanceDto = HistoricTaskInstanceDto.fromHistoricTaskInstance(taskInstance);
		  result.Add(taskInstanceDto);
		}
		return result;
	  }

	  private IList<HistoricTaskInstance> executePaginatedQuery(HistoricTaskInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricTaskInstancesCount(UriInfo uriInfo)
	  {
		HistoricTaskInstanceQueryDto queryDto = new HistoricTaskInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricTaskInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricTaskInstancesCount(HistoricTaskInstanceQueryDto queryDto)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricTaskInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  public virtual Response getHistoricTaskInstanceReport(UriInfo uriInfo)
	  {
		HistoricTaskInstanceReportQueryDto queryDto = new HistoricTaskInstanceReportQueryDto(objectMapper, uriInfo.QueryParameters);
		Response response;

		if (AbstractReportDto.REPORT_TYPE_DURATION.Equals(queryDto.ReportType))
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.history.ReportResult> reportResults = queryDto.executeReport(processEngine);
		  IList<ReportResult> reportResults = queryDto.executeReport(processEngine);
		  response = Response.ok(generateDurationDto(reportResults)).build();
		}
		else if (AbstractReportDto.REPORT_TYPE_COUNT.Equals(queryDto.ReportType))
		{
		  IList<HistoricTaskInstanceReportResult> reportResults = queryDto.executeCompletedReport(processEngine);
		  response = Response.ok(generateCountDto(reportResults)).build();
		}
		else
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Parameter reportType is not set.");
		}

		return response;
	  }

	  protected internal virtual IList<HistoricTaskInstanceReportResultDto> generateCountDto(IList<HistoricTaskInstanceReportResult> results)
	  {
		IList<HistoricTaskInstanceReportResultDto> dtoList = new List<HistoricTaskInstanceReportResultDto>();

		foreach (HistoricTaskInstanceReportResult result in results)
		{
		  dtoList.Add(HistoricTaskInstanceReportResultDto.fromHistoricTaskInstanceReportResult(result));
		}

		return dtoList;
	  }

	  protected internal virtual IList<ReportResultDto> generateDurationDto<T1>(IList<T1> results) where T1 : org.camunda.bpm.engine.history.ReportResult
	  {
		IList<ReportResultDto> dtoList = new List<ReportResultDto>();

		foreach (ReportResult result in results)
		{
		  dtoList.Add(ReportResultDto.fromReportResult(result));
		}

		return dtoList;
	  }

	}

}