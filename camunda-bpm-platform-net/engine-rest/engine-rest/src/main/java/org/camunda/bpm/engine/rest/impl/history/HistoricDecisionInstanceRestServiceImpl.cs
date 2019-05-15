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
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricDecisionInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceDto;
	using HistoricDecisionInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceQueryDto;
	using SetRemovalTimeToHistoricDecisionInstancesDto = org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricDecisionInstancesDto;
	using DeleteHistoricDecisionInstancesDto = org.camunda.bpm.engine.rest.dto.history.batch.DeleteHistoricDecisionInstancesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using HistoricDecisionInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricDecisionInstanceRestService;
	using HistoricDecisionInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricDecisionInstanceResource;
	using HistoricDecisionInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricDecisionInstanceResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class HistoricDecisionInstanceRestServiceImpl : HistoricDecisionInstanceRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricDecisionInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricDecisionInstanceResource getHistoricDecisionInstance(string decisionInstanceId)
	  {
		return new HistoricDecisionInstanceResourceImpl(processEngine, decisionInstanceId);
	  }

	  public virtual IList<HistoricDecisionInstanceDto> getHistoricDecisionInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricDecisionInstanceQueryDto queryHistoricDecisionInstanceDto = new HistoricDecisionInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricDecisionInstances(queryHistoricDecisionInstanceDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricDecisionInstanceDto> queryHistoricDecisionInstances(HistoricDecisionInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		HistoricDecisionInstanceQuery query = queryDto.toQuery(processEngine);

		IList<HistoricDecisionInstance> matchingHistoricDecisionInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricDecisionInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricDecisionInstances = query.list();
		}

		IList<HistoricDecisionInstanceDto> historicDecisionInstanceDtoResults = new List<HistoricDecisionInstanceDto>();
		foreach (HistoricDecisionInstance historicDecisionInstance in matchingHistoricDecisionInstances)
		{
		  HistoricDecisionInstanceDto resultHistoricDecisionInstanceDto = HistoricDecisionInstanceDto.fromHistoricDecisionInstance(historicDecisionInstance);
		  historicDecisionInstanceDtoResults.Add(resultHistoricDecisionInstanceDto);
		}
		return historicDecisionInstanceDtoResults;
	  }

	  private IList<HistoricDecisionInstance> executePaginatedQuery(HistoricDecisionInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricDecisionInstancesCount(UriInfo uriInfo)
	  {
		HistoricDecisionInstanceQueryDto queryDto = new HistoricDecisionInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricDecisionInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricDecisionInstancesCount(HistoricDecisionInstanceQueryDto queryDto)
	  {
		HistoricDecisionInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();

		return new CountResultDto(count);
	  }

	  public virtual BatchDto deleteAsync(DeleteHistoricDecisionInstancesDto dto)
	  {
		HistoricDecisionInstanceQuery decisionInstanceQuery = null;
		if (dto.HistoricDecisionInstanceQuery != null)
		{
		  decisionInstanceQuery = dto.HistoricDecisionInstanceQuery.toQuery(processEngine);
		}

		try
		{
		  IList<string> historicDecisionInstanceIds = dto.HistoricDecisionInstanceIds;
		  string deleteReason = dto.DeleteReason;
		  Batch batch = processEngine.HistoryService.deleteHistoricDecisionInstancesAsync(historicDecisionInstanceIds, decisionInstanceQuery, deleteReason);
		  return BatchDto.fromBatch(batch);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto setRemovalTimeAsync(SetRemovalTimeToHistoricDecisionInstancesDto dto)
	  {
		HistoryService historyService = processEngine.HistoryService;

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = null;

		if (dto.HistoricDecisionInstanceQuery != null)
		{
		  historicDecisionInstanceQuery = dto.HistoricDecisionInstanceQuery.toQuery(processEngine);

		}

		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyService.setRemovalTimeToHistoricDecisionInstances();

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

		builder.byIds(dto.HistoricDecisionInstanceIds);
		builder.byQuery(historicDecisionInstanceQuery);

		if (dto.Hierarchical)
		{
		  builder.hierarchical();

		}

		Batch batch = builder.executeAsync();
		return BatchDto.fromBatch(batch);
	  }

	}

}