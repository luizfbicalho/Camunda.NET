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
namespace org.camunda.bpm.engine.rest.sub.identity.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT;


	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using TenantDto = org.camunda.bpm.engine.rest.dto.identity.TenantDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class TenantResourceImpl : AbstractIdentityResource, TenantResource
	{

	  private string rootResourcePath;

	  public TenantResourceImpl(string processEngineName, string tenantId, string rootResourcePath, ObjectMapper objectMapper) : base(processEngineName, TENANT, tenantId, objectMapper)
	  {
		this.rootResourcePath = rootResourcePath;
	  }

	  public virtual TenantDto getTenant(UriInfo context)
	  {

		Tenant tenant = findTenantObject();
		if (tenant == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Tenant with id " + resourceId + " does not exist");
		}

		TenantDto dto = TenantDto.fromTenant(tenant);
		return dto;
	  }

	  public virtual void updateTenant(TenantDto tenantDto)
	  {
		ensureNotReadOnly();

		Tenant tenant = findTenantObject();
		if (tenant == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Tenant with id " + resourceId + " does not exist");
		}

		tenantDto.update(tenant);

		identityService.saveTenant(tenant);
	  }

	  public virtual void deleteTenant()
	  {
		ensureNotReadOnly();

		identityService.deleteTenant(resourceId);
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {
		ResourceOptionsDto dto = new ResourceOptionsDto();

		// add links if operations are authorized
		URI uri = context.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.TenantRestService_Fields.PATH).path(resourceId).build();

		dto.addReflexiveLink(uri, HttpMethod.GET, "self");

		if (!identityService.ReadOnly && isAuthorized(DELETE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.DELETE, "delete");
		}
		if (!identityService.ReadOnly && isAuthorized(UPDATE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.PUT, "update");
		}
		return dto;
	  }

	  public virtual TenantUserMembersResource TenantUserMembersResource
	  {
		  get
		  {
			return new TenantUserMembersResourceImpl(processEngine.Name, resourceId, rootResourcePath, ObjectMapper);
		  }
	  }

	  public virtual TenantGroupMembersResource TenantGroupMembersResource
	  {
		  get
		  {
			return new TenantGroupMembersResourceImpl(processEngine.Name, resourceId, rootResourcePath, ObjectMapper);
		  }
	  }

	  protected internal virtual Tenant findTenantObject()
	  {
		try
		{
		  return identityService.createTenantQuery().tenantId(resourceId).singleResult();

		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, "Exception while performing tenant query: " + e.Message);
		}
	  }

	}

}