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
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricCaseActivityInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseActivityInstanceDto;
	using HistoricCaseActivityInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseActivityInstanceQueryDto;
	using HistoricCaseActivityInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricCaseActivityInstanceRestService;
	using HistoricCaseActivityInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricCaseActivityInstanceResource;
	using HistoricCaseActivityInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricCaseActivityInstanceResourceImpl;


	public class HistoricCaseActivityInstanceRestServiceImpl : HistoricCaseActivityInstanceRestService
	{

	  protected internal ProcessEngine processEngine;
	  protected internal ObjectMapper objectMapper;

	  public HistoricCaseActivityInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricCaseActivityInstanceResource getHistoricCaseInstance(string caseActivityInstanceId)
	  {
		return new HistoricCaseActivityInstanceResourceImpl(processEngine, caseActivityInstanceId);
	  }

	  public virtual IList<HistoricCaseActivityInstanceDto> getHistoricCaseActivityInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		HistoricCaseActivityInstanceQueryDto queryHistoricCaseActivityInstanceDto = new HistoricCaseActivityInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricCaseActivityInstances(queryHistoricCaseActivityInstanceDto, firstResult, maxResults);
	  }

	  public virtual IList<HistoricCaseActivityInstanceDto> queryHistoricCaseActivityInstances(HistoricCaseActivityInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		HistoricCaseActivityInstanceQuery query = queryDto.toQuery(processEngine);

		IList<HistoricCaseActivityInstance> matchingHistoricCaseActivityInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricCaseActivityInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricCaseActivityInstances = query.list();
		}

		IList<HistoricCaseActivityInstanceDto> historicCaseActivityInstanceResults = new List<HistoricCaseActivityInstanceDto>();
		foreach (HistoricCaseActivityInstance historicCaseActivityInstance in matchingHistoricCaseActivityInstances)
		{
		  HistoricCaseActivityInstanceDto resultHistoricCaseActivityInstance = HistoricCaseActivityInstanceDto.fromHistoricCaseActivityInstance(historicCaseActivityInstance);
		  historicCaseActivityInstanceResults.Add(resultHistoricCaseActivityInstance);
		}
		return historicCaseActivityInstanceResults;
	  }

	  private IList<HistoricCaseActivityInstance> executePaginatedQuery(HistoricCaseActivityInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricCaseActivityInstancesCount(UriInfo uriInfo)
	  {
		HistoricCaseActivityInstanceQueryDto queryDto = new HistoricCaseActivityInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricCaseActivityInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricCaseActivityInstancesCount(HistoricCaseActivityInstanceQueryDto queryDto)
	  {
		HistoricCaseActivityInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();

		return new CountResultDto(count);
	  }
	}

}