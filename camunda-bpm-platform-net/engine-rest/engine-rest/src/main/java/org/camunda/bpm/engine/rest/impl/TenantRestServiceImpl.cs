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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT;



	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using TenantDto = org.camunda.bpm.engine.rest.dto.identity.TenantDto;
	using TenantQueryDto = org.camunda.bpm.engine.rest.dto.identity.TenantQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TenantResource = org.camunda.bpm.engine.rest.sub.identity.TenantResource;
	using TenantResourceImpl = org.camunda.bpm.engine.rest.sub.identity.impl.TenantResourceImpl;
	using PathUtil = org.camunda.bpm.engine.rest.util.PathUtil;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class TenantRestServiceImpl : AbstractAuthorizedRestResource, TenantRestService
	{

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public TenantRestServiceImpl(String engineName, final com.fasterxml.jackson.databind.ObjectMapper objectMapper)
	  public TenantRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, TENANT, ANY, objectMapper)
	  {
	  }

	  public virtual TenantResource getTenant(string id)
	  {
		id = PathUtil.decodePathParam(id);
		return new TenantResourceImpl(ProcessEngine.Name, id, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<TenantDto> queryTenants(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		TenantQueryDto queryDto = new TenantQueryDto(ObjectMapper, uriInfo.QueryParameters);

		TenantQuery query = queryDto.toQuery(ProcessEngine);

		IList<Tenant> tenants;
		if (firstResult != null || maxResults != null)
		{
		  tenants = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  tenants = query.list();
		}

		return TenantDto.fromTenantList(tenants);
	  }

	  public virtual CountResultDto getTenantCount(UriInfo uriInfo)
	  {
		TenantQueryDto queryDto = new TenantQueryDto(ObjectMapper, uriInfo.QueryParameters);

		TenantQuery query = queryDto.toQuery(ProcessEngine);
		long count = query.count();

		return new CountResultDto(count);
	  }

	  public virtual void createTenant(TenantDto dto)
	  {

		if (IdentityService.ReadOnly)
		{
		  throw new InvalidRequestException(Status.FORBIDDEN, "Identity service implementation is read-only.");
		}

		Tenant newTenant = IdentityService.newTenant(dto.Id);
		dto.update(newTenant);

		IdentityService.saveTenant(newTenant);
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.TenantRestService_Fields.PATH);

		ResourceOptionsDto resourceOptionsDto = new ResourceOptionsDto();

		// GET /
		URI baseUri = baseUriBuilder.build();
		resourceOptionsDto.addReflexiveLink(baseUri, HttpMethod.GET, "list");

		// GET /count
		URI countUri = baseUriBuilder.clone().path("/count").build();
		resourceOptionsDto.addReflexiveLink(countUri, HttpMethod.GET, "count");

		// POST /create
		if (!IdentityService.ReadOnly && isAuthorized(CREATE))
		{
		  URI createUri = baseUriBuilder.clone().path("/create").build();
		  resourceOptionsDto.addReflexiveLink(createUri, HttpMethod.POST, "create");
		}

		return resourceOptionsDto;
	  }

	  protected internal virtual IList<Tenant> executePaginatedQuery(TenantQuery query, int? firstResult, int? maxResults)
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

	  protected internal virtual IdentityService IdentityService
	  {
		  get
		  {
			return ProcessEngine.IdentityService;
		  }
	  }

	}

}