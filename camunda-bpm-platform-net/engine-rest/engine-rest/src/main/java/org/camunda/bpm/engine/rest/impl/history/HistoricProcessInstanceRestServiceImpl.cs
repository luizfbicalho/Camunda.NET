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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using ReportResult = org.camunda.bpm.engine.history.ReportResult;
	using SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ReportResultToCsvConverter = org.camunda.bpm.engine.rest.dto.converter.ReportResultToCsvConverter;
	using DeleteHistoricProcessInstancesDto = org.camunda.bpm.engine.rest.dto.history.DeleteHistoricProcessInstancesDto;
	using HistoricProcessInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using HistoricProcessInstanceReportDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceReportDto;
	using ReportResultDto = org.camunda.bpm.engine.rest.dto.history.ReportResultDto;
	using SetRemovalTimeToHistoricProcessInstancesDto = org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricProcessInstancesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using HistoricProcessInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricProcessInstanceRestService;
	using HistoricProcessInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricProcessInstanceResource;
	using HistoricProcessInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricProcessInstanceResourceImpl;


	  public class HistoricProcessInstanceRestServiceImpl : HistoricProcessInstanceRestService
	  {

	  public static readonly MediaType APPLICATION_CSV_TYPE = new MediaType("application", "csv");
	  public static readonly MediaType TEXT_CSV_TYPE = new MediaType("text", "csv");
	  public static readonly IList<Variant> VARIANTS = Variant.mediaTypes(MediaType.APPLICATION_JSON_TYPE, APPLICATION_CSV_TYPE, TEXT_CSV_TYPE).add().build();

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricProcessInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricProcessInstanceResource getHistoricProcessInstance(string processInstanceId)
	  {
		return new HistoricProcessInstanceResourceImpl(processEngine, processInstanceId);
	  }

	  public virtual IList<HistoricProcessInstanceDto> getHistoricProcessInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricProcessInstanceQueryDto queryHistoriProcessInstanceDto = new HistoricProcessInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricProcessInstances(queryHistoriProcessInstanceDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricProcessInstanceDto> queryHistoricProcessInstances(HistoricProcessInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricProcessInstanceQuery query = queryDto.toQuery(processEngine);

		IList<HistoricProcessInstance> matchingHistoricProcessInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricProcessInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricProcessInstances = query.list();
		}

		IList<HistoricProcessInstanceDto> historicProcessInstanceDtoResults = new List<HistoricProcessInstanceDto>();
		foreach (HistoricProcessInstance historicProcessInstance in matchingHistoricProcessInstances)
		{
		  HistoricProcessInstanceDto resultHistoricProcessInstanceDto = HistoricProcessInstanceDto.fromHistoricProcessInstance(historicProcessInstance);
		  historicProcessInstanceDtoResults.Add(resultHistoricProcessInstanceDto);
		}
		return historicProcessInstanceDtoResults;
	  }

	  private IList<HistoricProcessInstance> executePaginatedQuery(HistoricProcessInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricProcessInstancesCount(UriInfo uriInfo)
	  {
		HistoricProcessInstanceQueryDto queryDto = new HistoricProcessInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricProcessInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricProcessInstancesCount(HistoricProcessInstanceQueryDto queryDto)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricProcessInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<org.camunda.bpm.engine.history.ReportResult> queryHistoricProcessInstanceReport(javax.ws.rs.core.UriInfo uriInfo)
	  protected internal virtual IList<ReportResult> queryHistoricProcessInstanceReport(UriInfo uriInfo)
	  {
		HistoricProcessInstanceReportDto reportDto = new HistoricProcessInstanceReportDto(objectMapper, uriInfo.QueryParameters);
		return (IList<ReportResult>) reportDto.executeReport(processEngine);
	  }

	  public virtual Response getHistoricProcessInstancesReport(UriInfo uriInfo, Request request)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  MediaType mediaType = variant.MediaType;

		  if (MediaType.APPLICATION_JSON_TYPE.Equals(mediaType))
		  {
			IList<ReportResultDto> result = getReportResultAsJson(uriInfo);
			return Response.ok(result, mediaType).build();
		  }
		  else if (APPLICATION_CSV_TYPE.Equals(mediaType) || TEXT_CSV_TYPE.Equals(mediaType))
		  {
			string csv = getReportResultAsCsv(uriInfo);
			return Response.ok(csv, mediaType).header("Content-Disposition", "attachment; filename=process-instance-report.csv").build();
		  }
		}
		throw new InvalidRequestException(Response.Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

	  public virtual BatchDto deleteAsync(DeleteHistoricProcessInstancesDto dto)
	  {
		HistoryService historyService = processEngine.HistoryService;

		HistoricProcessInstanceQuery historicProcessInstanceQuery = null;
		if (dto.HistoricProcessInstanceQuery != null)
		{
		  historicProcessInstanceQuery = dto.HistoricProcessInstanceQuery.toQuery(processEngine);
		}

		try
		{
		  Batch batch;
		  batch = historyService.deleteHistoricProcessInstancesAsync(dto.HistoricProcessInstanceIds, historicProcessInstanceQuery, dto.DeleteReason);
		  return BatchDto.fromBatch(batch);

		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto setRemovalTimeAsync(SetRemovalTimeToHistoricProcessInstancesDto dto)
	  {
		HistoryService historyService = processEngine.HistoryService;

		HistoricProcessInstanceQuery historicProcessInstanceQuery = null;

		if (dto.HistoricProcessInstanceQuery != null)
		{
		  historicProcessInstanceQuery = dto.HistoricProcessInstanceQuery.toQuery(processEngine);

		}

		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyService.setRemovalTimeToHistoricProcessInstances();

		if (dto.CalculatedRemovalTime)
		{
		  builder.calculatedRemovalTime();

		}

		DateTime removalTime = dto.AbsoluteRemovalTime;
		if (dto.AbsoluteRemovalTime != null)
		{
		  builder.absoluteRemovalTime(removalTime);

		}

		if (dto.ClearedRemovalTime)
		{
		  builder.clearedRemovalTime();

		}

		builder.byIds(dto.HistoricProcessInstanceIds);
		builder.byQuery(historicProcessInstanceQuery);

		if (dto.Hierarchical)
		{
		  builder.hierarchical();

		}

		Batch batch = builder.executeAsync();
		return BatchDto.fromBatch(batch);
	  }

	  protected internal virtual IList<ReportResultDto> getReportResultAsJson(UriInfo uriInfo)
	  {
		IList<ReportResult> reports = queryHistoricProcessInstanceReport(uriInfo);
		IList<ReportResultDto> result = new List<ReportResultDto>();
		foreach (ReportResult report in reports)
		{
		  result.Add(ReportResultDto.fromReportResult(report));
		}
		return result;
	  }

	  protected internal virtual string getReportResultAsCsv(UriInfo uriInfo)
	  {
		IList<ReportResult> reports = queryHistoricProcessInstanceReport(uriInfo);
		MultivaluedMap<string, string> queryParameters = uriInfo.QueryParameters;
		string reportType = queryParameters.getFirst("reportType");
		return ReportResultToCsvConverter.convertReportResult(reports, reportType);
	  }

	  public virtual Response deleteHistoricVariableInstancesByProcessInstanceId(string processInstanceId)
	  {
		try
		{
		  processEngine.HistoryService.deleteHistoricVariableInstancesByProcessInstanceId(processInstanceId);
		}
		catch (NotFoundException nfe)
		{ // rewrite status code from bad request (400) to not found (404)
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, nfe.Message);
		}
		// return no content (204) since resource is deleted
		return Response.noContent().build();
	  }
	  }

}