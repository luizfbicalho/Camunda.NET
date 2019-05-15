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


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ExternalTaskQueryBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using UpdateExternalTaskRetriesBuilder = org.camunda.bpm.engine.externaltask.UpdateExternalTaskRetriesBuilder;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto;
	using ExternalTaskQueryDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskQueryDto;
	using FetchExternalTasksDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using LockedExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.LockedExternalTaskDto;
	using SetRetriesForExternalTasksDto = org.camunda.bpm.engine.rest.dto.externaltask.SetRetriesForExternalTasksDto;
	using ExternalTaskResource = org.camunda.bpm.engine.rest.sub.externaltask.ExternalTaskResource;
	using ExternalTaskResourceImpl = org.camunda.bpm.engine.rest.sub.externaltask.impl.ExternalTaskResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskRestServiceImpl : AbstractRestProcessEngineAware, ExternalTaskRestService
	{

	  public ExternalTaskRestServiceImpl(string processEngine, ObjectMapper objectMapper) : base(processEngine, objectMapper)
	  {
	  }

	  public virtual IList<ExternalTaskDto> getExternalTasks(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		ExternalTaskQueryDto queryDto = new ExternalTaskQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryExternalTasks(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<ExternalTaskDto> queryExternalTasks(ExternalTaskQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		ExternalTaskQuery query = queryDto.toQuery(engine);

		IList<ExternalTask> matchingTasks;
		if (firstResult != null || maxResults != null)
		{
		  matchingTasks = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingTasks = query.list();
		}

		IList<ExternalTaskDto> taskResults = new List<ExternalTaskDto>();
		foreach (ExternalTask task in matchingTasks)
		{
		  ExternalTaskDto resultInstance = ExternalTaskDto.fromExternalTask(task);
		  taskResults.Add(resultInstance);
		}
		return taskResults;
	  }

	  protected internal virtual IList<ExternalTask> executePaginatedQuery(ExternalTaskQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getExternalTasksCount(UriInfo uriInfo)
	  {
		ExternalTaskQueryDto queryDto = new ExternalTaskQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryExternalTasksCount(queryDto);
	  }

	  public virtual CountResultDto queryExternalTasksCount(ExternalTaskQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		ExternalTaskQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  public virtual IList<LockedExternalTaskDto> fetchAndLock(FetchExternalTasksDto fetchingDto)
	  {
		ExternalTaskQueryBuilder fetchBuilder = fetchingDto.buildQuery(processEngine);
		IList<LockedExternalTask> externalTasks = fetchBuilder.execute();
		return LockedExternalTaskDto.fromLockedExternalTasks(externalTasks);
	  }

	  public virtual ExternalTaskResource getExternalTask(string externalTaskId)
	  {
		return new ExternalTaskResourceImpl(ProcessEngine, externalTaskId, ObjectMapper);
	  }

	  public virtual BatchDto setRetriesAsync(SetRetriesForExternalTasksDto retriesDto)
	  {

		UpdateExternalTaskRetriesBuilder builder = updateRetries(retriesDto);
		int? retries = retriesDto.Retries;

		if (retries == null)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "The number of retries cannot be null.");
		}

		try
		{
		  Batch batch = builder.setAsync(retries.Value);
		  return BatchDto.fromBatch(batch);
		}
		catch (NotFoundException e)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}

	  }

	  public virtual SetRetriesForExternalTasksDto Retries
	  {
		  set
		  {
    
			UpdateExternalTaskRetriesBuilder builder = updateRetries(value);
			int? retries = value.Retries;
    
			if (retries == null)
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "The number of retries cannot be null.");
			}
    
			try
			{
			  builder.set(retries.Value);
			}
			catch (NotFoundException e)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
			}
			catch (BadUserRequestException e)
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
			}
		  }
	  }

	  protected internal virtual UpdateExternalTaskRetriesBuilder updateRetries(SetRetriesForExternalTasksDto retriesDto)
	  {

		ExternalTaskService externalTaskService = ProcessEngine.ExternalTaskService;

		IList<string> externalTaskIds = retriesDto.ExternalTaskIds;
		IList<string> processInstanceIds = retriesDto.ProcessInstanceIds;

		ExternalTaskQuery externalTaskQuery = null;
		ProcessInstanceQuery processInstanceQuery = null;
		HistoricProcessInstanceQuery historicProcessInstanceQuery = null;

		ExternalTaskQueryDto externalTaskQueryDto = retriesDto.ExternalTaskQuery;
		if (externalTaskQueryDto != null)
		{
		  externalTaskQuery = externalTaskQueryDto.toQuery(ProcessEngine);
		}

		ProcessInstanceQueryDto processInstanceQueryDto = retriesDto.ProcessInstanceQuery;
		if (processInstanceQueryDto != null)
		{
		  processInstanceQuery = processInstanceQueryDto.toQuery(ProcessEngine);
		}

		HistoricProcessInstanceQueryDto historicProcessInstanceQueryDto = retriesDto.HistoricProcessInstanceQuery;
		if (historicProcessInstanceQueryDto != null)
		{
		  historicProcessInstanceQuery = historicProcessInstanceQueryDto.toQuery(ProcessEngine);
		}

		return externalTaskService.updateRetries().externalTaskIds(externalTaskIds).processInstanceIds(processInstanceIds).externalTaskQuery(externalTaskQuery).processInstanceQuery(processInstanceQuery).historicProcessInstanceQuery(historicProcessInstanceQuery);
	  }

	}

}