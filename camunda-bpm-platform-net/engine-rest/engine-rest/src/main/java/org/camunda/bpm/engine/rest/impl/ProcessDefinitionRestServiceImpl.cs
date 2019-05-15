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


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;
	using DeleteProcessDefinitionsBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using StatisticsResultDto = org.camunda.bpm.engine.rest.dto.StatisticsResultDto;
	using ProcessDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto;
	using ProcessDefinitionQueryDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionQueryDto;
	using ProcessDefinitionStatisticsResultDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionStatisticsResultDto;
	using ProcessDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionSuspensionStateDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using ProcessDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.ProcessDefinitionResource;
	using ProcessDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.ProcessDefinitionResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ProcessDefinitionRestServiceImpl : AbstractRestProcessEngineAware, ProcessDefinitionRestService
	{

		public ProcessDefinitionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
		{
		}

		public virtual ProcessDefinitionResource getProcessDefinitionByKey(string processDefinitionKey)
		{

		  ProcessDefinition processDefinition = ProcessEngine.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).withoutTenantId().latestVersion().singleResult();

		if (processDefinition == null)
		{
		  string errorMessage = string.Format("No matching process definition with key: {0} and no tenant-id", processDefinitionKey);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getProcessDefinitionById(processDefinition.Id);
		}
		}

		public virtual ProcessDefinitionResource getProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId)
		{

		ProcessDefinition processDefinition = ProcessEngine.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).tenantIdIn(tenantId).latestVersion().singleResult();

		if (processDefinition == null)
		{
		  string errorMessage = string.Format("No matching process definition with key: {0} and tenant-id: {1}", processDefinitionKey, tenantId);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getProcessDefinitionById(processDefinition.Id);
		}
		}

	  public virtual ProcessDefinitionResource getProcessDefinitionById(string processDefinitionId)
	  {
		return new ProcessDefinitionResourceImpl(ProcessEngine, processDefinitionId, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<ProcessDefinitionDto> getProcessDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		ProcessDefinitionQueryDto queryDto = new ProcessDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		  IList<ProcessDefinitionDto> definitions = new List<ProcessDefinitionDto>();

		  ProcessEngine engine = ProcessEngine;
		  ProcessDefinitionQuery query = queryDto.toQuery(engine);

		  IList<ProcessDefinition> matchingDefinitions = null;

		  if (firstResult != null || maxResults != null)
		  {
			matchingDefinitions = executePaginatedQuery(query, firstResult, maxResults);
		  }
		  else
		  {
			matchingDefinitions = query.list();
		  }

		  foreach (ProcessDefinition definition in matchingDefinitions)
		  {
			ProcessDefinitionDto def = ProcessDefinitionDto.fromProcessDefinition(definition);
			definitions.Add(def);
		  }
		  return definitions;
	  }

		private IList<ProcessDefinition> executePaginatedQuery(ProcessDefinitionQuery query, int? firstResult, int? maxResults)
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

		public virtual CountResultDto getProcessDefinitionsCount(UriInfo uriInfo)
		{
		  ProcessDefinitionQueryDto queryDto = new ProcessDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);

		  ProcessEngine engine = ProcessEngine;
		ProcessDefinitionQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;
		return result;
		}


	  public virtual IList<StatisticsResultDto> getStatistics(bool? includeFailedJobs, bool? includeRootIncidents, bool? includeIncidents, string includeIncidentsForType)
	  {
		if (includeIncidents != null && !string.ReferenceEquals(includeIncidentsForType, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Only one of the query parameter includeIncidents or includeIncidentsForType can be set.");
		}

		if (includeIncidents != null && includeRootIncidents != null)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Only one of the query parameter includeIncidents or includeRootIncidents can be set.");
		}

		if (includeRootIncidents != null && !string.ReferenceEquals(includeIncidentsForType, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Only one of the query parameter includeRootIncidents or includeIncidentsForType can be set.");
		}

		ManagementService mgmtService = ProcessEngine.ManagementService;
		ProcessDefinitionStatisticsQuery query = mgmtService.createProcessDefinitionStatisticsQuery();

		if (includeFailedJobs != null && includeFailedJobs)
		{
		  query.includeFailedJobs();
		}

		if (includeIncidents != null && includeIncidents)
		{
		  query.includeIncidents();
		}
		else if (!string.ReferenceEquals(includeIncidentsForType, null))
		{
		  query.includeIncidentsForType(includeIncidentsForType);
		}
		else if (includeRootIncidents != null && includeRootIncidents)
		{
		  query.includeRootIncidents();
		}

		IList<ProcessDefinitionStatistics> queryResults = query.list();

		IList<StatisticsResultDto> results = new List<StatisticsResultDto>();
		foreach (ProcessDefinitionStatistics queryResult in queryResults)
		{
		  StatisticsResultDto dto = ProcessDefinitionStatisticsResultDto.fromProcessDefinitionStatistics(queryResult);
		  results.Add(dto);
		}

		return results;
	  }

	  public virtual void updateSuspensionState(ProcessDefinitionSuspensionStateDto dto)
	  {
		if (!string.ReferenceEquals(dto.ProcessDefinitionId, null))
		{
		  string message = "Only processDefinitionKey can be set to update the suspension state.";
		  throw new InvalidRequestException(Status.BAD_REQUEST, message);
		}

		try
		{
		  dto.updateSuspensionState(ProcessEngine);

		}
		catch (System.ArgumentException e)
		{
		  string message = string.Format("Could not update the suspension state of Process Definitions due to: {0}", e.Message);
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, message);
		}
	  }

	  public virtual void deleteProcessDefinitionsByKey(string processDefinitionKey, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		RepositoryService repositoryService = processEngine.RepositoryService;

		DeleteProcessDefinitionsBuilder builder = repositoryService.deleteProcessDefinitions().byKey(processDefinitionKey);

		deleteProcessDefinitions(builder, cascade, skipCustomListeners, skipIoMappings);
	  }

	  public virtual void deleteProcessDefinitionsByKeyAndTenantId(string processDefinitionKey, bool cascade, bool skipCustomListeners, bool skipIoMappings, string tenantId)
	  {
		RepositoryService repositoryService = processEngine.RepositoryService;

		DeleteProcessDefinitionsBuilder builder = repositoryService.deleteProcessDefinitions().byKey(processDefinitionKey).withTenantId(tenantId);

		deleteProcessDefinitions(builder, cascade, skipCustomListeners, skipIoMappings);
	  }

	  protected internal virtual void deleteProcessDefinitions(DeleteProcessDefinitionsBuilder builder, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		if (skipCustomListeners)
		{
		  builder = builder.skipCustomListeners();
		}

		if (cascade)
		{
		  builder = builder.cascade();
		}

		if (skipIoMappings)
		{
		  builder = builder.skipIoMappings();
		}

		try
		{
		  builder.delete();
		}
		catch (NotFoundException e)
		{ // rewrite status code from bad request (400) to not found (404)
		  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
		}
	  }

	}

}