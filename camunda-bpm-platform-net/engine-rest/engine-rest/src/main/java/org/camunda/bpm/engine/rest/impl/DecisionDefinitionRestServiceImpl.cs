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


	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using DecisionDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionDto;
	using DecisionDefinitionQueryDto = org.camunda.bpm.engine.rest.dto.repository.DecisionDefinitionQueryDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using DecisionDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.DecisionDefinitionResource;
	using DecisionDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.DecisionDefinitionResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DecisionDefinitionRestServiceImpl : AbstractRestProcessEngineAware, DecisionDefinitionRestService
	{

	  public DecisionDefinitionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual DecisionDefinitionResource getDecisionDefinitionByKey(string decisionDefinitionKey)
	  {

		DecisionDefinition decisionDefinition = ProcessEngine.RepositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(decisionDefinitionKey).withoutTenantId().latestVersion().singleResult();

		if (decisionDefinition == null)
		{
		  string errorMessage = string.Format("No matching decision definition with key: {0} and no tenant-id", decisionDefinitionKey);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getDecisionDefinitionById(decisionDefinition.Id);
		}
	  }

	  public virtual DecisionDefinitionResource getDecisionDefinitionByKeyAndTenantId(string decisionDefinitionKey, string tenantId)
	  {

		DecisionDefinition decisionDefinition = ProcessEngine.RepositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(decisionDefinitionKey).tenantIdIn(tenantId).latestVersion().singleResult();

		if (decisionDefinition == null)
		{
		  string errorMessage = string.Format("No matching decision definition with key: {0} and tenant-id: {1}", decisionDefinitionKey, tenantId);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getDecisionDefinitionById(decisionDefinition.Id);
		}
	  }

	  public virtual DecisionDefinitionResource getDecisionDefinitionById(string decisionDefinitionId)
	  {
		return new DecisionDefinitionResourceImpl(ProcessEngine, decisionDefinitionId, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<DecisionDefinitionDto> getDecisionDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		DecisionDefinitionQueryDto queryDto = new DecisionDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		IList<DecisionDefinitionDto> definitions = new List<DecisionDefinitionDto>();

		ProcessEngine engine = ProcessEngine;
		DecisionDefinitionQuery query = queryDto.toQuery(engine);

		IList<DecisionDefinition> matchingDefinitions = null;

		if (firstResult != null || maxResults != null)
		{
		  matchingDefinitions = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingDefinitions = query.list();
		}

		foreach (DecisionDefinition definition in matchingDefinitions)
		{
		  DecisionDefinitionDto def = DecisionDefinitionDto.fromDecisionDefinition(definition);
		  definitions.Add(def);
		}
		return definitions;
	  }

	  private IList<DecisionDefinition> executePaginatedQuery(DecisionDefinitionQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getDecisionDefinitionsCount(UriInfo uriInfo)
	  {
		DecisionDefinitionQueryDto queryDto = new DecisionDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);

		ProcessEngine engine = ProcessEngine;
		DecisionDefinitionQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;
		return result;
	  }

	}

}