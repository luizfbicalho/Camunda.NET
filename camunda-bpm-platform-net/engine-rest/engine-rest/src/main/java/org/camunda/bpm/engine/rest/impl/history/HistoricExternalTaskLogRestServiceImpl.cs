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
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricExternalTaskLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogDto;
	using HistoricExternalTaskLogQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogQueryDto;
	using HistoricExternalTaskLogRestService = org.camunda.bpm.engine.rest.history.HistoricExternalTaskLogRestService;
	using HistoricExternalTaskLogResource = org.camunda.bpm.engine.rest.sub.history.HistoricExternalTaskLogResource;
	using HistoricExternalTaskLogResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricExternalTaskLogResourceImpl;


	public class HistoricExternalTaskLogRestServiceImpl : HistoricExternalTaskLogRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricExternalTaskLogRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricExternalTaskLogResource getHistoricExternalTaskLog(string historicExternalTaskLogId)
	  {
		return new HistoricExternalTaskLogResourceImpl(historicExternalTaskLogId, processEngine);
	  }

	  public virtual IList<HistoricExternalTaskLogDto> getHistoricExternalTaskLogs(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricExternalTaskLogQueryDto queryDto = new HistoricExternalTaskLogQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricExternalTaskLogs(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricExternalTaskLogDto> queryHistoricExternalTaskLogs(HistoricExternalTaskLogQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricExternalTaskLogQuery query = queryDto.toQuery(processEngine);

		IList<HistoricExternalTaskLog> matchingHistoricExternalTaskLogs;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricExternalTaskLogs = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricExternalTaskLogs = query.list();
		}

		IList<HistoricExternalTaskLogDto> results = new List<HistoricExternalTaskLogDto>();
		foreach (HistoricExternalTaskLog historicExternalTaskLog in matchingHistoricExternalTaskLogs)
		{
		  HistoricExternalTaskLogDto result = HistoricExternalTaskLogDto.fromHistoricExternalTaskLog(historicExternalTaskLog);
		  results.Add(result);
		}

		return results;
	  }

	  public virtual CountResultDto getHistoricExternalTaskLogsCount(UriInfo uriInfo)
	  {
		HistoricExternalTaskLogQueryDto queryDto = new HistoricExternalTaskLogQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricExternalTaskLogsCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricExternalTaskLogsCount(HistoricExternalTaskLogQueryDto queryDto)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricExternalTaskLogQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  protected internal virtual IList<HistoricExternalTaskLog> executePaginatedQuery(HistoricExternalTaskLogQuery query, int? firstResult, int? maxResults)
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