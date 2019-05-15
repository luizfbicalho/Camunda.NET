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
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using JobDefinitionDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto;
	using JobDefinitionQueryDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionQueryDto;
	using JobDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using JobDefinitionResource = org.camunda.bpm.engine.rest.sub.management.JobDefinitionResource;
	using JobDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.management.JobDefinitionResourceImpl;


	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class JobDefinitionRestServiceImpl : AbstractRestProcessEngineAware, JobDefinitionRestService
	{

	  public JobDefinitionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual JobDefinitionResource getJobDefinition(string jobDefinitionId)
	  {
		return new JobDefinitionResourceImpl(ProcessEngine, jobDefinitionId);
	  }

	  public virtual IList<JobDefinitionDto> getJobDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		JobDefinitionQueryDto queryDto = new JobDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryJobDefinitions(queryDto, firstResult, maxResults);

	  }

	  public virtual CountResultDto getJobDefinitionsCount(UriInfo uriInfo)
	  {
		JobDefinitionQueryDto queryDto = new JobDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryJobDefinitionsCount(queryDto);
	  }

	  public virtual IList<JobDefinitionDto> queryJobDefinitions(JobDefinitionQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		queryDto.ObjectMapper = ObjectMapper;
		JobDefinitionQuery query = queryDto.toQuery(ProcessEngine);

		IList<JobDefinition> matchingJobDefinitions;
		if (firstResult != null || maxResults != null)
		{
		  matchingJobDefinitions = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingJobDefinitions = query.list();
		}

		IList<JobDefinitionDto> jobDefinitionResults = new List<JobDefinitionDto>();
		foreach (JobDefinition jobDefinition in matchingJobDefinitions)
		{
		  JobDefinitionDto result = JobDefinitionDto.fromJobDefinition(jobDefinition);
		  jobDefinitionResults.Add(result);
		}

		return jobDefinitionResults;
	  }

	  public virtual CountResultDto queryJobDefinitionsCount(JobDefinitionQueryDto queryDto)
	  {
		queryDto.ObjectMapper = ObjectMapper;
		JobDefinitionQuery query = queryDto.toQuery(ProcessEngine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  private IList<JobDefinition> executePaginatedQuery(JobDefinitionQuery query, int? firstResult, int? maxResults)
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

	  public virtual void updateSuspensionState(JobDefinitionSuspensionStateDto dto)
	  {
		if (!string.ReferenceEquals(dto.JobDefinitionId, null))
		{
		  string message = "Either processDefinitionId or processDefinitionKey can be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		try
		{
		  dto.updateSuspensionState(ProcessEngine);

		}
		catch (System.ArgumentException e)
		{
		  string message = string.Format("Could not update the suspension state of Job Definitions due to: {0}", e.Message);
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, message);
		}
	  }

	}

}