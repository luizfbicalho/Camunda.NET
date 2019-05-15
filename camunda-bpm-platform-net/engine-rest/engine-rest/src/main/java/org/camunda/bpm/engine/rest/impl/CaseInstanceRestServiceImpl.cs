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
	using CaseInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.CaseInstanceDto;
	using CaseInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.CaseInstanceQueryDto;
	using CaseInstanceResource = org.camunda.bpm.engine.rest.sub.runtime.CaseInstanceResource;
	using CaseInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.CaseInstanceResourceImpl;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceRestServiceImpl : AbstractRestProcessEngineAware, CaseInstanceRestService
	{

	  public CaseInstanceRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual CaseInstanceResource getCaseInstance(string caseInstanceId)
	  {
		return new CaseInstanceResourceImpl(ProcessEngine, caseInstanceId, ObjectMapper);
	  }

	  public virtual IList<CaseInstanceDto> getCaseInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CaseInstanceQueryDto queryDto = new CaseInstanceQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryCaseInstances(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<CaseInstanceDto> queryCaseInstances(CaseInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		CaseInstanceQuery query = queryDto.toQuery(engine);

		IList<CaseInstance> matchingInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingInstances = query.list();
		}

		IList<CaseInstanceDto> instanceResults = new List<CaseInstanceDto>();
		foreach (CaseInstance instance in matchingInstances)
		{
		  CaseInstanceDto resultInstance = CaseInstanceDto.fromCaseInstance(instance);
		  instanceResults.Add(resultInstance);
		}
		return instanceResults;
	  }

	  private IList<CaseInstance> executePaginatedQuery(CaseInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getCaseInstancesCount(UriInfo uriInfo)
	  {
		CaseInstanceQueryDto queryDto = new CaseInstanceQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryCaseInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryCaseInstancesCount(CaseInstanceQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		CaseInstanceQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	}

}