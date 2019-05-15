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
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricActivityInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricActivityInstanceDto;
	using HistoricActivityInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricActivityInstanceQueryDto;
	using HistoricActivityInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricActivityInstanceRestService;
	using HistoricActivityInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricActivityInstanceResource;
	using HistoricActivityInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricActivityInstanceResourceImpl;


	public class HistoricActivityInstanceRestServiceImpl : HistoricActivityInstanceRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricActivityInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricActivityInstanceResource getHistoricCaseInstance(string activityInstanceId)
	  {
		return new HistoricActivityInstanceResourceImpl(processEngine, activityInstanceId);
	  }

	  public virtual IList<HistoricActivityInstanceDto> getHistoricActivityInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricActivityInstanceQueryDto queryHistoricActivityInstanceDto = new HistoricActivityInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricActivityInstances(queryHistoricActivityInstanceDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricActivityInstanceDto> queryHistoricActivityInstances(HistoricActivityInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricActivityInstanceQuery query = queryDto.toQuery(processEngine);

		IList<HistoricActivityInstance> matchingHistoricActivityInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricActivityInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricActivityInstances = query.list();
		}

		IList<HistoricActivityInstanceDto> historicActivityInstanceResults = new List<HistoricActivityInstanceDto>();
		foreach (HistoricActivityInstance historicActivityInstance in matchingHistoricActivityInstances)
		{
		  HistoricActivityInstanceDto resultHistoricActivityInstance = HistoricActivityInstanceDto.fromHistoricActivityInstance(historicActivityInstance);
		  historicActivityInstanceResults.Add(resultHistoricActivityInstance);
		}
		return historicActivityInstanceResults;
	  }

	  private IList<HistoricActivityInstance> executePaginatedQuery(HistoricActivityInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricActivityInstancesCount(UriInfo uriInfo)
	  {
		HistoricActivityInstanceQueryDto queryDto = new HistoricActivityInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricActivityInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricActivityInstancesCount(HistoricActivityInstanceQueryDto queryDto)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricActivityInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }
	}

}