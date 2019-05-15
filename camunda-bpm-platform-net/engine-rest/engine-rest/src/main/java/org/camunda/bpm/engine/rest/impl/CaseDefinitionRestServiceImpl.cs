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


	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CaseDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.CaseDefinitionDto;
	using CaseDefinitionQueryDto = org.camunda.bpm.engine.rest.dto.repository.CaseDefinitionQueryDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using CaseDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.CaseDefinitionResource;
	using CaseDefinitionResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.CaseDefinitionResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionRestServiceImpl : AbstractRestProcessEngineAware, CaseDefinitionRestService
	{

	  public CaseDefinitionRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual CaseDefinitionResource getCaseDefinitionByKey(string caseDefinitionKey)
	  {

		CaseDefinition caseDefinition = ProcessEngine.RepositoryService.createCaseDefinitionQuery().caseDefinitionKey(caseDefinitionKey).withoutTenantId().latestVersion().singleResult();

		if (caseDefinition == null)
		{
		  string errorMessage = string.Format("No matching case definition with key: {0} and no tenant-id", caseDefinitionKey);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getCaseDefinitionById(caseDefinition.Id);
		}
	  }

	  public virtual CaseDefinitionResource getCaseDefinitionByKeyAndTenantId(string caseDefinitionKey, string tenantId)
	  {

		CaseDefinition caseDefinition = ProcessEngine.RepositoryService.createCaseDefinitionQuery().caseDefinitionKey(caseDefinitionKey).tenantIdIn(tenantId).latestVersion().singleResult();

		if (caseDefinition == null)
		{
		  string errorMessage = string.Format("No matching case definition with key: {0} and tenant-id: {1}", caseDefinitionKey, tenantId);
		  throw new RestException(Status.NOT_FOUND, errorMessage);

		}
		else
		{
		  return getCaseDefinitionById(caseDefinition.Id);
		}
	  }

	  public virtual CaseDefinitionResource getCaseDefinitionById(string caseDefinitionId)
	  {
		return new CaseDefinitionResourceImpl(ProcessEngine, caseDefinitionId, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<CaseDefinitionDto> getCaseDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		CaseDefinitionQueryDto queryDto = new CaseDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);
		IList<CaseDefinitionDto> definitions = new List<CaseDefinitionDto>();

		ProcessEngine engine = ProcessEngine;
		CaseDefinitionQuery query = queryDto.toQuery(engine);

		IList<CaseDefinition> matchingDefinitions = null;

		if (firstResult != null || maxResults != null)
		{
		  matchingDefinitions = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingDefinitions = query.list();
		}

		foreach (CaseDefinition definition in matchingDefinitions)
		{
		  CaseDefinitionDto def = CaseDefinitionDto.fromCaseDefinition(definition);
		  definitions.Add(def);
		}
		return definitions;
	  }

	  private IList<CaseDefinition> executePaginatedQuery(CaseDefinitionQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getCaseDefinitionsCount(UriInfo uriInfo)
	  {
		CaseDefinitionQueryDto queryDto = new CaseDefinitionQueryDto(ObjectMapper, uriInfo.QueryParameters);

		ProcessEngine engine = ProcessEngine;
		CaseDefinitionQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;
		return result;
	  }

	}

}