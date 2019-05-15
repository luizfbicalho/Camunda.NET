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
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using ProcessInstanceSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto;
	using SetJobRetriesByProcessDto = org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesByProcessDto;
	using DeleteProcessInstancesDto = org.camunda.bpm.engine.rest.dto.runtime.batch.DeleteProcessInstancesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ProcessInstanceResource = org.camunda.bpm.engine.rest.sub.runtime.ProcessInstanceResource;
	using ProcessInstanceResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.ProcessInstanceResourceImpl;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ProcessInstanceRestServiceImpl : AbstractRestProcessEngineAware, ProcessInstanceRestService
	{

	  public ProcessInstanceRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }


	  public virtual IList<ProcessInstanceDto> getProcessInstances(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		ProcessInstanceQueryDto queryDto = new ProcessInstanceQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryProcessInstances(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<ProcessInstanceDto> queryProcessInstances(ProcessInstanceQueryDto queryDto, int? firstResult, int? maxResults)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		ProcessInstanceQuery query = queryDto.toQuery(engine);

		IList<ProcessInstance> matchingInstances;
		if (firstResult != null || maxResults != null)
		{
		  matchingInstances = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingInstances = query.list();
		}

		IList<ProcessInstanceDto> instanceResults = new List<ProcessInstanceDto>();
		foreach (ProcessInstance instance in matchingInstances)
		{
		  ProcessInstanceDto resultInstance = ProcessInstanceDto.fromProcessInstance(instance);
		  instanceResults.Add(resultInstance);
		}
		return instanceResults;
	  }

	  private IList<ProcessInstance> executePaginatedQuery(ProcessInstanceQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getProcessInstancesCount(UriInfo uriInfo)
	  {
		ProcessInstanceQueryDto queryDto = new ProcessInstanceQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryProcessInstancesCount(queryDto);
	  }

	  public virtual CountResultDto queryProcessInstancesCount(ProcessInstanceQueryDto queryDto)
	  {
		ProcessEngine engine = ProcessEngine;
		queryDto.ObjectMapper = ObjectMapper;
		ProcessInstanceQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;

		return result;
	  }

	  public virtual ProcessInstanceResource getProcessInstance(string processInstanceId)
	  {
		return new ProcessInstanceResourceImpl(ProcessEngine, processInstanceId, ObjectMapper);
	  }

	  public virtual void updateSuspensionState(ProcessInstanceSuspensionStateDto dto)
	  {
		if (!string.ReferenceEquals(dto.ProcessInstanceId, null))
		{
		  string message = "Either processDefinitionId or processDefinitionKey can be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		dto.updateSuspensionState(ProcessEngine);
	  }

	  public virtual BatchDto updateSuspensionStateAsync(ProcessInstanceSuspensionStateDto dto)
	  {
		Batch batch = null;
		try
		{
		  batch = dto.updateSuspensionStateAsync(ProcessEngine);
		  return BatchDto.fromBatch(batch);

		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto deleteAsync(DeleteProcessInstancesDto dto)
	  {
		RuntimeService runtimeService = ProcessEngine.RuntimeService;

		ProcessInstanceQuery processInstanceQuery = null;
		if (dto.ProcessInstanceQuery != null)
		{
		  processInstanceQuery = dto.ProcessInstanceQuery.toQuery(ProcessEngine);
		}

		Batch batch = null;

		try
		{
		  batch = runtimeService.deleteProcessInstancesAsync(dto.ProcessInstanceIds, processInstanceQuery, dto.DeleteReason, dto.SkipCustomListeners, dto.SkipSubprocesses);
		  return BatchDto.fromBatch(batch);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto deleteAsyncHistoricQueryBased(DeleteProcessInstancesDto deleteProcessInstancesDto)
	  {
		IList<string> processInstanceIds = new List<string>();

		HistoricProcessInstanceQueryDto queryDto = deleteProcessInstancesDto.HistoricProcessInstanceQuery;
		if (queryDto != null)
		{
		  HistoricProcessInstanceQuery query = queryDto.toQuery(ProcessEngine);
		  IList<HistoricProcessInstance> historicProcessInstances = query.list();

		  foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		  {
			processInstanceIds.Add(historicProcessInstance.Id);
		  }
		}

		if (deleteProcessInstancesDto.ProcessInstanceIds != null)
		{
		  ((IList<string>)processInstanceIds).AddRange(deleteProcessInstancesDto.ProcessInstanceIds);
		}

		try
		{
		  RuntimeService runtimeService = ProcessEngine.RuntimeService;
		  Batch batch = runtimeService.deleteProcessInstancesAsync(processInstanceIds, null, deleteProcessInstancesDto.DeleteReason, deleteProcessInstancesDto.SkipCustomListeners, deleteProcessInstancesDto.SkipSubprocesses);

		  return BatchDto.fromBatch(batch);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto setRetriesByProcess(SetJobRetriesByProcessDto setJobRetriesDto)
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
		ProcessInstanceQuery processInstanceQuery = null;
		if (setJobRetriesDto.ProcessInstanceQuery != null)
		{
		  processInstanceQuery = setJobRetriesDto.ProcessInstanceQuery.toQuery(ProcessEngine);
		}

		try
		{
		  Batch batch = ProcessEngine.ManagementService.setJobRetriesAsync(setJobRetriesDto.ProcessInstances, processInstanceQuery, setJobRetriesDto.Retries.Value);
		  return BatchDto.fromBatch(batch);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }

	  public virtual BatchDto setRetriesByProcessHistoricQueryBased(SetJobRetriesByProcessDto setJobRetriesDto)
	  {
		IList<string> processInstanceIds = new List<string>();

		HistoricProcessInstanceQueryDto queryDto = setJobRetriesDto.HistoricProcessInstanceQuery;
		if (queryDto != null)
		{
		  HistoricProcessInstanceQuery query = queryDto.toQuery(ProcessEngine);
		  IList<HistoricProcessInstance> historicProcessInstances = query.list();

		  foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		  {
			processInstanceIds.Add(historicProcessInstance.Id);
		  }
		}

		if (setJobRetriesDto.ProcessInstances != null)
		{
		  ((IList<string>)processInstanceIds).AddRange(setJobRetriesDto.ProcessInstances);
		}

		try
		{
		  ManagementService managementService = ProcessEngine.ManagementService;
		  Batch batch = managementService.setJobRetriesAsync(processInstanceIds, (ProcessInstanceQuery) null, setJobRetriesDto.Retries.Value);

		  return BatchDto.fromBatch(batch);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e.Message);
		}
	  }
	}

}