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
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricCaseInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceDto;
	using HistoricCaseInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceQueryDto;
	using HistoricCaseInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricCaseInstanceRestService;
	using HistoricCaseInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricCaseInstanceResource;
	using HistoricCaseInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricCaseInstanceResourceImpl;


	public class HistoricCaseInstanceRestServiceImpl : HistoricCaseInstanceRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricCaseInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricCaseInstanceResource getHistoricCaseInstance(string caseInstanceId)
	  {
		return new HistoricCaseInstanceResourceImpl(processEngine, caseInstanceId);
	  }

	  public virtual IList<HistoricCaseInstanceDto> getHistoricCaseInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricCaseInstanceQueryDto queryHistoricCaseInstanceDto = new HistoricCaseInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricCaseInstances(queryHistoricCaseInstanceDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricCaseInstanceDto> queryHistoricCaseInstances(HistoricCaseInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		HistoricCaseInstanceQuery query = queryDto.toQuery(processEngine);

		IList<HistoricCaseInstance> matchingHistoricCaseInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricCaseInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricCaseInstances = query.list();
		}

		IList<HistoricCaseInstanceDto> historicCaseInstanceDtoResults = new List<HistoricCaseInstanceDto>();
		foreach (HistoricCaseInstance historicCaseInstance in matchingHistoricCaseInstances)
		{
		  HistoricCaseInstanceDto resultHistoricCaseInstanceDto = HistoricCaseInstanceDto.fromHistoricCaseInstance(historicCaseInstance);
		  historicCaseInstanceDtoResults.Add(resultHistoricCaseInstanceDto);
		}
		return historicCaseInstanceDtoResults;
	  }

	  private IList<HistoricCaseInstance> executePaginatedQuery(HistoricCaseInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricCaseInstancesCount(UriInfo uriInfo)
	  {
		HistoricCaseInstanceQueryDto queryDto = new HistoricCaseInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricCaseInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricCaseInstancesCount(HistoricCaseInstanceQueryDto queryDto)
	  {
		HistoricCaseInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();

		return new CountResultDto(count);
	  }

	}

}