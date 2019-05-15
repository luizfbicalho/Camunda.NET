﻿using System.Collections.Generic;

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
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricIdentityLinkLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricIdentityLinkLogDto;
	using HistoricIdentityLinkLogQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricIdentityLinkLogQueryDto;
	using HistoricIdentityLinkLogRestService = org.camunda.bpm.engine.rest.history.HistoricIdentityLinkLogRestService;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class HistoricIdentityLinkLogRestServiceImpl : HistoricIdentityLinkLogRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricIdentityLinkLogRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual IList<HistoricIdentityLinkLogDto> getHistoricIdentityLinks(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricIdentityLinkLogQueryDto queryDto = new HistoricIdentityLinkLogQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricIdentityLinkLogQuery query = queryDto.toQuery(processEngine);

		IList<HistoricIdentityLinkLog> queryResult;
		if (firstResult != null || maxResults != null)
		{
		  queryResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  queryResult = query.list();
		}
		IList<HistoricIdentityLinkLogDto> result = new List<HistoricIdentityLinkLogDto>();
		foreach (HistoricIdentityLinkLog historicIdentityLink in queryResult)
		{
		  HistoricIdentityLinkLogDto dto = HistoricIdentityLinkLogDto.fromHistoricIdentityLink(historicIdentityLink);
		  result.Add(dto);
		}
		return result;
	  }

	  public virtual CountResultDto getHistoricIdentityLinksCount(UriInfo uriInfo)
	  {
		HistoricIdentityLinkLogQueryDto queryDto = new HistoricIdentityLinkLogQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricIdentityLinkLogQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;

	  }
	  private IList<HistoricIdentityLinkLog> executePaginatedQuery(HistoricIdentityLinkLogQuery query, int? firstResult, int? maxResults)
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