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


	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using DecisionRequirementsDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.DecisionRequirementsDefinitionDto;
	using DecisionRequirementsDefinitionQueryDto = org.camunda.bpm.engine.rest.dto.repository.DecisionRequirementsDefinitionQueryDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using DecisionRequirementsDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.DecisionRequirementsDefinitionResource;
	using DecisionRequirementsDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.DecisionRequirementsDefinitionResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DecisionRequirementsDefinitionRestServiceImpl : AbstractRestProcessEngineAware, DecisionRequirementsDefinitionRestService
	{

	  public DecisionRequirementsDefinitionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual IList<DecisionRequirementsDefinitionDto> getDecisionRequirementsDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		DecisionRequirementsDefinitionQueryDto queryDto = new DecisionRequirementsDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		IList<DecisionRequirementsDefinitionDto> dtos = new List<DecisionRequirementsDefinitionDto>();

		ProcessEngine engine = ProcessEngine;
		DecisionRequirementsDefinitionQuery query = queryDto.toQuery(engine);

		IList<DecisionRequirementsDefinition> matchingDefinitions = null;

		if (firstResult != null || maxResults != null)
		{
		  matchingDefinitions = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingDefinitions = query.list();
		}

		foreach (DecisionRequirementsDefinition definition in matchingDefinitions)
		{
		  DecisionRequirementsDefinitionDto dto = DecisionRequirementsDefinitionDto.fromDecisionRequirementsDefinition(definition);
		  dtos.Add(dto);
		}
		return dtos;
	  }

	  private IList<DecisionRequirementsDefinition> executePaginatedQuery(DecisionRequirementsDefinitionQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getDecisionRequirementsDefinitionsCount(UriInfo uriInfo)
	  {
		DecisionRequirementsDefinitionQueryDto queryDto = new DecisionRequirementsDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);

		ProcessEngine engine = ProcessEngine;
		DecisionRequirementsDefinitionQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;
		return result;
	  }

	  public virtual DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionById(string decisionRequirementsDefinitionId)
	  {
		return new DecisionRequirementsDefinitionResourceImpl(ProcessEngine, decisionRequirementsDefinitionId);
	  }

	  public virtual DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionByKey(string decisionRequirementsDefinitionKey)
	  {
		DecisionRequirementsDefinition decisionRequirementsDefinition = ProcessEngine.RepositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(decisionRequirementsDefinitionKey).withoutTenantId().latestVersion().singleResult();

		if (decisionRequirementsDefinition == null)
		{
		  string errorMessage = string.Format("No matching decision requirements definition with key: {0} and no tenant-id", decisionRequirementsDefinitionKey);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getDecisionRequirementsDefinitionById(decisionRequirementsDefinition.Id);
		}
	  }

	  public virtual DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionByKeyAndTenantId(string decisionRequirementsDefinitionKey, string tenantId)
	  {
		DecisionRequirementsDefinition decisionRequirementsDefinition = ProcessEngine.RepositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(decisionRequirementsDefinitionKey).tenantIdIn(tenantId).latestVersion().singleResult();

		if (decisionRequirementsDefinition == null)
		{
		  string errorMessage = string.Format("No matching decision requirements definition with key: {0} and tenant-id: {1}", decisionRequirementsDefinitionKey, tenantId);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getDecisionRequirementsDefinitionById(decisionRequirementsDefinition.Id);
		}
	  }

	}

}