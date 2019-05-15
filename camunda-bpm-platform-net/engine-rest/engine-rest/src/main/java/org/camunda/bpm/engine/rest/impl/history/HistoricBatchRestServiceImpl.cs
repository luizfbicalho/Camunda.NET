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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using CleanableHistoricBatchReport = org.camunda.bpm.engine.history.CleanableHistoricBatchReport;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using SetRemovalTimeSelectModeForHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricBatchesBuilder;
	using Query = org.camunda.bpm.engine.query.Query;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using CleanableHistoricBatchReportDto = org.camunda.bpm.engine.rest.dto.history.batch.CleanableHistoricBatchReportDto;
	using CleanableHistoricBatchReportResultDto = org.camunda.bpm.engine.rest.dto.history.batch.CleanableHistoricBatchReportResultDto;
	using HistoricBatchDto = org.camunda.bpm.engine.rest.dto.history.batch.HistoricBatchDto;
	using HistoricBatchQueryDto = org.camunda.bpm.engine.rest.dto.history.batch.HistoricBatchQueryDto;
	using SetRemovalTimeToHistoricBatchesDto = org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricBatchesDto;
	using HistoricBatchRestService = org.camunda.bpm.engine.rest.history.HistoricBatchRestService;
	using HistoricBatchResource = org.camunda.bpm.engine.rest.sub.history.HistoricBatchResource;
	using HistoricBatchResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricBatchResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricBatchRestServiceImpl : HistoricBatchRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricBatchRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricBatchResource getHistoricBatch(string batchId)
	  {
		return new HistoricBatchResourceImpl(processEngine, batchId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.List<org.camunda.bpm.engine.rest.dto.history.batch.HistoricBatchDto> getHistoricBatches(javax.ws.rs.core.UriInfo uriInfo, System.Nullable<int> firstResult, System.Nullable<int> maxResults)
	  public virtual IList<HistoricBatchDto> getHistoricBatches(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricBatchQueryDto queryDto = new HistoricBatchQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricBatchQuery query = queryDto.toQuery(processEngine);

		IList<HistoricBatch> matchingBatches;
		if (firstResult != null || maxResults != null)
		{
		  matchingBatches = (IList<HistoricBatch>) executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingBatches = query.list();
		}

		IList<HistoricBatchDto> batchResults = new List<HistoricBatchDto>();
		foreach (HistoricBatch matchingBatch in matchingBatches)
		{
		  batchResults.Add(HistoricBatchDto.fromBatch(matchingBatch));
		}
		return batchResults;
	  }

	  public virtual CountResultDto getHistoricBatchesCount(UriInfo uriInfo)
	  {
		HistoricBatchQueryDto queryDto = new HistoricBatchQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricBatchQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		return new CountResultDto(count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected java.util.List<?> executePaginatedQuery(org.camunda.bpm.engine.query.Query query, System.Nullable<int> firstResult, System.Nullable<int> maxResults)
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	  protected internal virtual IList<object> executePaginatedQuery(Query query, int? firstResult, int? maxResults)
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.List<org.camunda.bpm.engine.rest.dto.history.batch.CleanableHistoricBatchReportResultDto> getCleanableHistoricBatchesReport(javax.ws.rs.core.UriInfo uriInfo, System.Nullable<int> firstResult, System.Nullable<int> maxResults)
	  public virtual IList<CleanableHistoricBatchReportResultDto> getCleanableHistoricBatchesReport(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CleanableHistoricBatchReportDto queryDto = new CleanableHistoricBatchReportDto(objectMapper, uriInfo.QueryParameters);
		CleanableHistoricBatchReport query = queryDto.toQuery(processEngine);

		IList<CleanableHistoricBatchReportResult> reportResult;
		if (firstResult != null || maxResults != null)
		{
		  reportResult = (IList<CleanableHistoricBatchReportResult>) executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  reportResult = query.list();
		}

		return CleanableHistoricBatchReportResultDto.convert(reportResult);
	  }

	  public virtual CountResultDto getCleanableHistoricBatchesReportCount(UriInfo uriInfo)
	  {
		CleanableHistoricBatchReportDto queryDto = new CleanableHistoricBatchReportDto(objectMapper, uriInfo.QueryParameters);
		queryDto.ObjectMapper = objectMapper;
		CleanableHistoricBatchReport query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  public virtual BatchDto setRemovalTimeAsync(SetRemovalTimeToHistoricBatchesDto dto)
	  {
		HistoryService historyService = processEngine.HistoryService;

		HistoricBatchQuery historicBatchQuery = null;

		if (dto.HistoricBatchQuery != null)
		{
		  historicBatchQuery = dto.HistoricBatchQuery.toQuery(processEngine);

		}

		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyService.setRemovalTimeToHistoricBatches();

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

		builder.byIds(dto.HistoricBatchIds);
		builder.byQuery(historicBatchQuery);

		Batch batch = builder.executeAsync();
		return BatchDto.fromBatch(batch);
	  }

	}

}