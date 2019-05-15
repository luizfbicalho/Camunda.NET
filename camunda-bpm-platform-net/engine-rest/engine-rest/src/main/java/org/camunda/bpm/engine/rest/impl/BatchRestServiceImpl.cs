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
namespace org.camunda.bpm.engine.rest.impl
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchQuery = org.camunda.bpm.engine.batch.BatchQuery;
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using BatchStatisticsQuery = org.camunda.bpm.engine.batch.BatchStatisticsQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using BatchQueryDto = org.camunda.bpm.engine.rest.dto.batch.BatchQueryDto;
	using BatchStatisticsDto = org.camunda.bpm.engine.rest.dto.batch.BatchStatisticsDto;
	using BatchStatisticsQueryDto = org.camunda.bpm.engine.rest.dto.batch.BatchStatisticsQueryDto;
	using BatchResource = org.camunda.bpm.engine.rest.sub.batch.BatchResource;
	using BatchResourceImpl = org.camunda.bpm.engine.rest.sub.batch.impl.BatchResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class BatchRestServiceImpl : AbstractRestProcessEngineAware, BatchRestService
	{

	  public BatchRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual BatchResource getBatch(string batchId)
	  {
		return new BatchResourceImpl(ProcessEngine, batchId);
	  }

	  public virtual IList<BatchDto> getBatches(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		BatchQueryDto queryDto = new BatchQueryDto(ObjectMapper, uriInfo.QueryParameters);
		BatchQuery query = queryDto.toQuery(ProcessEngine);

		IList<Batch> matchingBatches;
		if (firstResult != null || maxResults != null)
		{
		  matchingBatches = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingBatches = query.list();
		}

		IList<BatchDto> batchResults = new List<BatchDto>();
		foreach (Batch matchingBatch in matchingBatches)
		{
		  batchResults.Add(BatchDto.fromBatch(matchingBatch));
		}
		return batchResults;
	  }

	  public virtual CountResultDto getBatchesCount(UriInfo uriInfo)
	  {
		ProcessEngine processEngine = ProcessEngine;
		BatchQueryDto queryDto = new BatchQueryDto(ObjectMapper, uriInfo.QueryParameters);
		BatchQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		return new CountResultDto(count);
	  }

	  public virtual IList<BatchStatisticsDto> getStatistics(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		BatchStatisticsQueryDto queryDto = new BatchStatisticsQueryDto(ObjectMapper, uriInfo.QueryParameters);
		BatchStatisticsQuery query = queryDto.toQuery(ProcessEngine);

		IList<BatchStatistics> batchStatisticsList;
		if (firstResult != null || maxResults != null)
		{
		  batchStatisticsList = executePaginatedStatisticsQuery(query, firstResult, maxResults);
		}
		else
		{
		  batchStatisticsList = query.list();
		}

		IList<BatchStatisticsDto> statisticsResults = new List<BatchStatisticsDto>();
		foreach (BatchStatistics batchStatistics in batchStatisticsList)
		{
		  statisticsResults.Add(BatchStatisticsDto.fromBatchStatistics(batchStatistics));
		}

		return statisticsResults;
	  }

	  public virtual CountResultDto getStatisticsCount(UriInfo uriInfo)
	  {
		BatchStatisticsQueryDto queryDto = new BatchStatisticsQueryDto(ObjectMapper, uriInfo.QueryParameters);
		BatchStatisticsQuery query = queryDto.toQuery(ProcessEngine);

		long count = query.count();
		return new CountResultDto(count);
	  }

	  protected internal virtual IList<Batch> executePaginatedQuery(BatchQuery query, int? firstResult, int? maxResults)
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

	  protected internal virtual IList<BatchStatistics> executePaginatedStatisticsQuery(BatchStatisticsQuery query, int? firstResult, int? maxResults)
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
	}

}