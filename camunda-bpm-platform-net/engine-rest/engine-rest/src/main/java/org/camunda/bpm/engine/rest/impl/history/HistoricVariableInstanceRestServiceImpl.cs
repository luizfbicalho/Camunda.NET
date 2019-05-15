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
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricVariableInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto;
	using HistoricVariableInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceQueryDto;
	using HistoricVariableInstanceRestService = org.camunda.bpm.engine.rest.history.HistoricVariableInstanceRestService;
	using HistoricVariableInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricVariableInstanceResource;
	using HistoricVariableInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.history.impl.HistoricVariableInstanceResourceImpl;


	public class HistoricVariableInstanceRestServiceImpl : HistoricVariableInstanceRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public HistoricVariableInstanceRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual HistoricVariableInstanceResource variableInstanceResource(string variableId)
	  {
		return new HistoricVariableInstanceResourceImpl(variableId, processEngine);
	  }

	  public virtual IList<HistoricVariableInstanceDto> getHistoricVariableInstances(UriInfo uriInfo, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {
		HistoricVariableInstanceQueryDto queryDto = new HistoricVariableInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricVariableInstances(queryDto, firstResult, maxResults, deserializeObjectValues);
	  }

	  public virtual IList<HistoricVariableInstanceDto> queryHistoricVariableInstances(HistoricVariableInstanceQueryDto queryDto, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricVariableInstanceQuery query = queryDto.toQuery(processEngine);
		query.disableBinaryFetching();

		if (!deserializeObjectValues)
		{
		  query.disableCustomObjectDeserialization();
		}

		IList<HistoricVariableInstance> matchingHistoricVariableInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingHistoricVariableInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingHistoricVariableInstances = query.list();
		}

		IList<HistoricVariableInstanceDto> historicVariableInstanceDtoResults = new List<HistoricVariableInstanceDto>();
		foreach (HistoricVariableInstance historicVariableInstance in matchingHistoricVariableInstances)
		{
		  HistoricVariableInstanceDto resultHistoricVariableInstance = HistoricVariableInstanceDto.fromHistoricVariableInstance(historicVariableInstance);
		  historicVariableInstanceDtoResults.Add(resultHistoricVariableInstance);
		}
		return historicVariableInstanceDtoResults;
	  }

	  private IList<HistoricVariableInstance> executePaginatedQuery(HistoricVariableInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getHistoricVariableInstancesCount(UriInfo uriInfo)
	  {
		HistoricVariableInstanceQueryDto queryDto = new HistoricVariableInstanceQueryDto(objectMapper, uriInfo.QueryParameters);
		return queryHistoricVariableInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryHistoricVariableInstancesCount(HistoricVariableInstanceQueryDto queryDto)
	  {
		queryDto.ObjectMapper = objectMapper;
		HistoricVariableInstanceQuery query = queryDto.toQuery(processEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }
	}

}