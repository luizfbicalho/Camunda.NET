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
	using CaseExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto;
	using CaseExecutionQueryDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionQueryDto;
	using CaseExecutionResource = org.camunda.bpm.engine.rest.sub.runtime.CaseExecutionResource;
	using CaseExecutionResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.CaseExecutionResourceImpl;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;


	public class CaseExecutionRestServiceImpl : AbstractRestProcessEngineAware, CaseExecutionRestService
	{

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CaseExecutionRestServiceImpl(String engineName, final com.fasterxml.jackson.databind.ObjectMapper objectMapper)
	  public CaseExecutionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual CaseExecutionResource getCaseExecution(string caseExecutionId)
	  {
		return new CaseExecutionResourceImpl(ProcessEngine, caseExecutionId, ObjectMapper);
	  }

	  public virtual IList<CaseExecutionDto> getCaseExecutions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CaseExecutionQueryDto queryDto = new CaseExecutionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryCaseExecutions(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<CaseExecutionDto> queryCaseExecutions(CaseExecutionQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		CaseExecutionQuery query = queryDto.toQuery(engine);

		IList<CaseExecution> matchingExecutions;
		if (firstResult != null || maxResults != null)
		{
		  matchingExecutions = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingExecutions = query.list();
		}

		IList<CaseExecutionDto> executionResults = new List<CaseExecutionDto>();
		foreach (CaseExecution execution in matchingExecutions)
		{
		  CaseExecutionDto resultExecution = CaseExecutionDto.fromCaseExecution(execution);
		  executionResults.Add(resultExecution);
		}
		return executionResults;
	  }

	  private IList<CaseExecution> executePaginatedQuery(CaseExecutionQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getCaseExecutionsCount(UriInfo uriInfo)
	  {
		CaseExecutionQueryDto queryDto = new CaseExecutionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryCaseExecutionsCount(queryDto);
	  }

	  public virtual CountResultDto queryCaseExecutionsCount(CaseExecutionQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		CaseExecutionQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	}

}