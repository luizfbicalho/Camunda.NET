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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using IncidentDto = org.camunda.bpm.engine.rest.dto.runtime.IncidentDto;
	using IncidentQueryDto = org.camunda.bpm.engine.rest.dto.runtime.IncidentQueryDto;
	using IncidentResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.IncidentResourceImpl;
	using IncidentResource = org.camunda.bpm.engine.rest.sub.runtime.IncidentResource;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class IncidentRestServiceImpl : AbstractRestProcessEngineAware, IncidentRestService
	{

	  public IncidentRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual IList<IncidentDto> getIncidents(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		IncidentQueryDto queryDto = new IncidentQueryDto(ObjectMapper, uriInfo.QueryParameters);
		IncidentQuery query = queryDto.toQuery(processEngine);

		IList<Incident> queryResult;
		if (firstResult != null || maxResults != null)
		{
		  queryResult = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  queryResult = query.list();
		}

		IList<IncidentDto> result = new List<IncidentDto>();
		foreach (Incident incident in queryResult)
		{
		  IncidentDto dto = IncidentDto.fromIncident(incident);
		  result.Add(dto);
		}

		return result;
	  }

	  public virtual CountResultDto getIncidentsCount(UriInfo uriInfo)
	  {
		IncidentQueryDto queryDto = new IncidentQueryDto(ObjectMapper, uriInfo.QueryParameters);
		IncidentQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  private IList<Incident> executePaginatedQuery(IncidentQuery query, int? firstResult, int? maxResults)
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

	  public virtual IncidentResource getIncident(string incidentId)
	  {
		return new IncidentResourceImpl(ProcessEngine, incidentId, ObjectMapper);
	  }
	}

}