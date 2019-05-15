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

	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricJobLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricJobLogDto;
	using HistoricJobLogQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricJobLogQueryDto;
	using HistoricJobLogRestService = org.camunda.bpm.engine.rest.history.HistoricJobLogRestService;
	using HistoricJobLogResource = org.camunda.bpm.engine.rest.sub.history.HistoricJobLogResource;
	using HistoricJobLogResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricJobLogResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricJobLogRestServiceImpl : HistoricJobLogRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricJobLogRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricJobLogResource getHistoricJobLog(string historicJobLogId)
	  {
		return new HistoricJobLogResourceImpl(historicJobLogId, processEngine);
	  }

	  public virtual IList<HistoricJobLogDto> getHistoricJobLogs(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricJobLogQueryDto queryDto = new HistoricJobLogQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricJobLogs(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricJobLogDto> queryHistoricJobLogs(HistoricJobLogQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricJobLogQuery query = queryDto.toQuery(processEngine);

		IList<HistoricJobLog> matchingHistoricJobLogs;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricJobLogs = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricJobLogs = query.list();
		}

		IList<HistoricJobLogDto> results = new List<HistoricJobLogDto>();
		foreach (HistoricJobLog historicJobLog in matchingHistoricJobLogs)
		{
		  HistoricJobLogDto result = HistoricJobLogDto.fromHistoricJobLog(historicJobLog);
		  results.Add(result);
		}

		return results;
	  }

	  public virtual CountResultDto getHistoricJobLogsCount(UriInfo uriInfo)
	  {
		HistoricJobLogQueryDto queryDto = new HistoricJobLogQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricJobLogsCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricJobLogsCount(HistoricJobLogQueryDto queryDto)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricJobLogQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  protected internal virtual IList<HistoricJobLog> executePaginatedQuery(HistoricJobLogQuery query, int? firstResult, int? maxResults)
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