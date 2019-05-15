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
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using JobDto = org.camunda.bpm.engine.rest.dto.runtime.JobDto;
	using JobQueryDto = org.camunda.bpm.engine.rest.dto.runtime.JobQueryDto;
	using JobSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto;
	using SetJobRetriesDto = org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using JobResource = org.camunda.bpm.engine.rest.sub.runtime.JobResource;
	using JobResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.JobResourceImpl;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;


	public class JobRestServiceImpl : AbstractRestProcessEngineAware, JobRestService
	{

	  public JobRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual JobResource getJob(string jobId)
	  {
		return new JobResourceImpl(ProcessEngine, jobId);
	  }

	  public virtual IList<JobDto> getJobs(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		JobQueryDto queryDto = new JobQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryJobs(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<JobDto> queryJobs(JobQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		JobQuery query = queryDto.toQuery(engine);

		IList<Job> matchingJobs;
		if (firstResult != null || maxResults != null)
		{
		  matchingJobs = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingJobs = query.list();
		}

		IList<JobDto> jobResults = new List<JobDto>();
		foreach (Job job in matchingJobs)
		{
		  JobDto resultJob = JobDto.fromJob(job);
		  jobResults.Add(resultJob);
		}
		return jobResults;
	  }

	  public virtual CountResultDto getJobsCount(UriInfo uriInfo)
	  {
		JobQueryDto queryDto = new JobQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryJobsCount(queryDto);
	  }

	  public virtual CountResultDto queryJobsCount(JobQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		JobQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  public virtual BatchDto setRetries(SetJobRetriesDto setJobRetriesDto)
	  {
		try
		{
		  EnsureUtil.ensureNotNull("setJobRetriesDto", setJobRetriesDto);
		  EnsureUtil.ensureNotNull("retries", setJobRetriesDto.Retries);
		}
		catch (NullValueException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
		JobQuery jobQuery = null;
		if (setJobRetriesDto.JobQuery != null)
		{
		  jobQuery = setJobRetriesDto.JobQuery.toQuery(ProcessEngine);
		}

		try
		{
		  Batch batch = ProcessEngine.ManagementService.setJobRetriesAsync(setJobRetriesDto.JobIds, jobQuery, setJobRetriesDto.Retries.Value);
		  return BatchDto.fromBatch(batch);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }

	  private IList<Job> executePaginatedQuery(JobQuery query, int? firstResult, int? maxResults)
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

	  public virtual void updateSuspensionState(JobSuspensionStateDto dto)
	  {
		if (!string.ReferenceEquals(dto.JobId, null))
		{
		  string message = "Either jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey can be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		dto.updateSuspensionState(ProcessEngine);
	  }

	}

}