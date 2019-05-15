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
	using VariableInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceDto;
	using VariableInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceQueryDto;
	using VariableInstanceResource = org.camunda.bpm.engine.rest.sub.runtime.VariableInstanceResource;
	using VariableInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.VariableInstanceResourceImpl;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;


	public class VariableInstanceRestServiceImpl : AbstractRestProcessEngineAware, VariableInstanceRestService
	{

	  public VariableInstanceRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual VariableInstanceResource getVariableInstance(string id)
	  {
		return new VariableInstanceResourceImpl(id, processEngine);
	  }

	  public virtual IList<VariableInstanceDto> getVariableInstances(UriInfo uriInfo, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {
		VariableInstanceQueryDto queryDto = new VariableInstanceQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryVariableInstances(queryDto, firstResult, maxResults, deserializeObjectValues);
	  }

	  public virtual IList<VariableInstanceDto> queryVariableInstances(VariableInstanceQueryDto queryDto, int? firstResult, int? maxResults, bool deserializeObjectValues)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		VariableInstanceQuery query = queryDto.toQuery(engine);

		// disable binary fetching by default.
		query.disableBinaryFetching();

		// disable custom object fetching by default. Cannot be done to not break existing API
		if (!deserializeObjectValues)
		{
		  query.disableCustomObjectDeserialization();
		}

		IList<VariableInstance> matchingInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingInstances = query.list();
		}

		IList<VariableInstanceDto> instanceResults = new List<VariableInstanceDto>();
		foreach (VariableInstance instance in matchingInstances)
		{
		  VariableInstanceDto resultInstance = VariableInstanceDto.fromVariableInstance(instance);
		  instanceResults.Add(resultInstance);
		}
		return instanceResults;
	  }

	  private IList<VariableInstance> executePaginatedQuery(VariableInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getVariableInstancesCount(UriInfo uriInfo)
	  {
		VariableInstanceQueryDto queryDto = new VariableInstanceQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryVariableInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryVariableInstancesCount(VariableInstanceQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		VariableInstanceQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	}

}