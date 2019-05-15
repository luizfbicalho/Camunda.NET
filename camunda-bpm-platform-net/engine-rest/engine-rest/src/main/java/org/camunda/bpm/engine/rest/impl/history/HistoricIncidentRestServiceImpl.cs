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
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricIncidentDto = org.camunda.bpm.engine.rest.dto.history.HistoricIncidentDto;
	using HistoricIncidentQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricIncidentQueryDto;
	using HistoricIncidentRestService = org.camunda.bpm.engine.rest.history.HistoricIncidentRestService;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricIncidentRestServiceImpl : HistoricIncidentRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricIncidentRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual IList<HistoricIncidentDto> getHistoricIncidents(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricIncidentQueryDto queryDto = new HistoricIncidentQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricIncidentQuery query = queryDto.toQuery(processEngine);

		IList<HistoricIncident> queryResult;
		if (firstResult != null || maxResults != null)
		{
		  queryResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  queryResult = query.list();
		}

		IList<HistoricIncidentDto> result = new List<HistoricIncidentDto>();
		foreach (HistoricIncident historicIncident in queryResult)
		{
		  HistoricIncidentDto dto = HistoricIncidentDto.fromHistoricIncident(historicIncident);
		  result.Add(dto);
		}

		return result;
	  }

	  public virtual CountResultDto getHistoricIncidentsCount(UriInfo uriInfo)
	  {
		HistoricIncidentQueryDto queryDto = new HistoricIncidentQueryDto(objectMapper, uriInfo.QueryParameters);
		HistoricIncidentQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  private IList<HistoricIncident> executePaginatedQuery(HistoricIncidentQuery query, int? firstResult, int? maxResults)
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