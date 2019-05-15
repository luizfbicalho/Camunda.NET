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
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using FilterDto = org.camunda.bpm.engine.rest.dto.runtime.FilterDto;
	using FilterQueryDto = org.camunda.bpm.engine.rest.dto.runtime.FilterQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using FilterResource = org.camunda.bpm.engine.rest.sub.runtime.FilterResource;
	using FilterResourceImpl = org.camunda.bpm.engine.rest.sub.runtime.impl.FilterResourceImpl;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.FILTER;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterRestServiceImpl : AbstractAuthorizedRestResource, FilterRestService
	{

	  public FilterRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, FILTER, ANY, objectMapper)
	  {
	  }

	  public virtual FilterResource getFilter(string filterId)
	  {
		return new FilterResourceImpl(ProcessEngine.Name, ObjectMapper, filterId, relativeRootResourcePath);
	  }

	  public virtual IList<FilterDto> getFilters(UriInfo uriInfo, bool? itemCount, int? firstResult, int? maxResults)
	  {
		FilterService filterService = ProcessEngine.FilterService;
		FilterQuery query = getQueryFromQueryParameters(uriInfo.QueryParameters);

		IList<Filter> matchingFilters = executeFilterQuery(query, firstResult, maxResults);

		IList<FilterDto> filters = new List<FilterDto>();
		foreach (Filter filter in matchingFilters)
		{
		  FilterDto dto = FilterDto.fromFilter(filter);
		  if (itemCount != null && itemCount)
		  {
			dto.ItemCount = filterService.count(filter.Id);
		  }
		  filters.Add(dto);
		}

		return filters;
	  }

	  public virtual IList<Filter> executeFilterQuery(FilterQuery query, int? firstResult, int? maxResults)
	  {
		if (firstResult != null || maxResults != null)
		{
		  return executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  return query.list();
		}
	  }

	  protected internal virtual IList<Filter> executePaginatedQuery(FilterQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getFiltersCount(UriInfo uriInfo)
	  {
		FilterQuery query = getQueryFromQueryParameters(uriInfo.QueryParameters);
		return new CountResultDto(query.count());
	  }

	  public virtual FilterDto createFilter(FilterDto filterDto)
	  {
		FilterService filterService = ProcessEngine.FilterService;

		string resourceType = filterDto.ResourceType;

		Filter filter;

		if (EntityTypes.TASK.Equals(resourceType))
		{
		  filter = filterService.newTaskFilter();
		}
		else
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Unable to create filter with invalid resource type '" + resourceType + "'");
		}

		try
		{
		  filterDto.updateFilter(filter, ProcessEngine);
		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, "Unable to create filter with invalid content");
		}

		filterService.saveFilter(filter);

		return FilterDto.fromFilter(filter);
	  }

	  protected internal virtual FilterQuery getQueryFromQueryParameters(MultivaluedMap<string, string> queryParameters)
	  {
		ProcessEngine engine = ProcessEngine;
		FilterQueryDto queryDto = new FilterQueryDto(ObjectMapper, queryParameters);
		return queryDto.toQuery(engine);
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.FilterRestService_Fields.PATH);

		ResourceOptionsDto resourceOptionsDto = new ResourceOptionsDto();

		// GET /
		URI baseUri = baseUriBuilder.build();
		resourceOptionsDto.addReflexiveLink(baseUri, HttpMethod.GET, "list");

		// GET /count
		URI countUri = baseUriBuilder.clone().path("/count").build();
		resourceOptionsDto.addReflexiveLink(countUri, HttpMethod.GET, "count");

		// POST /create
		if (isAuthorized(CREATE))
		{
		  URI createUri = baseUriBuilder.clone().path("/create").build();
		  resourceOptionsDto.addReflexiveLink(createUri, HttpMethod.POST, "create");
		}

		return resourceOptionsDto;
	  }

	}

}