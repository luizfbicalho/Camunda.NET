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
	using ExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionDto;
	using ExecutionQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionQueryDto;
	using ExecutionResource = org.camunda.bpm.engine.rest.sub.runtime.ExecutionResource;
	using ExecutionResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.ExecutionResourceImpl;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;


	public class ExecutionRestServiceImpl : AbstractRestProcessEngineAware, ExecutionRestService
	{

	  public ExecutionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual ExecutionResource getExecution(string executionId)
	  {
		return new ExecutionResourceImpl(ProcessEngine, executionId, ObjectMapper);
	  }

	  public virtual IList<ExecutionDto> getExecutions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		ExecutionQueryDto queryDto = new ExecutionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryExecutions(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<ExecutionDto> queryExecutions(ExecutionQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		ExecutionQuery query = queryDto.toQuery(engine);

		IList<Execution> matchingExecutions;
		if (firstResult != null || maxResults != null)
		{
		  matchingExecutions = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingExecutions = query.list();
		}

		IList<ExecutionDto> executionResults = new List<ExecutionDto>();
		foreach (Execution execution in matchingExecutions)
		{
		  ExecutionDto resultExecution = ExecutionDto.fromExecution(execution);
		  executionResults.Add(resultExecution);
		}
		return executionResults;
	  }

	  private IList<Execution> executePaginatedQuery(ExecutionQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getExecutionsCount(UriInfo uriInfo)
	  {
		ExecutionQueryDto queryDto = new ExecutionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryExecutionsCount(queryDto);
	  }

	  public virtual CountResultDto queryExecutionsCount(ExecutionQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		ExecutionQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }
	}

}