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
namespace org.camunda.bpm.engine.rest.sub.authorization.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using AuthorizationDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using AbstractAuthorizedRestResource = org.camunda.bpm.engine.rest.impl.AbstractAuthorizedRestResource;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationResourceImpl : AbstractAuthorizedRestResource, AuthorizationResource
	{

	  protected internal readonly AuthorizationService authorizationService;
	  protected internal new string relativeRootResourcePath;

	  public AuthorizationResourceImpl(string processEngineName, string resourceId, string relativeRootResourcePath, ObjectMapper objectMapper) : base(processEngineName, AUTHORIZATION, resourceId, objectMapper)
	  {
		this.relativeRootResourcePath = relativeRootResourcePath;
		authorizationService = processEngine.AuthorizationService;
	  }

	  public virtual AuthorizationDto getAuthorization(UriInfo context)
	  {

		Authorization dbAuthorization = DbAuthorization;

		return AuthorizationDto.fromAuthorization(dbAuthorization, processEngine.ProcessEngineConfiguration);

	  }

	  public virtual void deleteAuthorization()
	  {
		Authorization dbAuthorization = DbAuthorization;
		authorizationService.deleteAuthorization(dbAuthorization.Id);
	  }

	  public virtual void updateAuthorization(AuthorizationDto dto)
	  {
		// get db auth
		Authorization dbAuthorization = DbAuthorization;
		// copy values from dto
		AuthorizationDto.update(dto, dbAuthorization, processEngine.ProcessEngineConfiguration);
		// save
		authorizationService.saveAuthorization(dbAuthorization);
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		ResourceOptionsDto dto = new ResourceOptionsDto();

		URI uri = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.AuthorizationRestService_Fields.PATH).path(resourceId).build();

		dto.addReflexiveLink(uri, HttpMethod.GET, "self");

		if (isAuthorized(DELETE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.DELETE, "delete");
		}
		if (isAuthorized(UPDATE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.PUT, "update");
		}

		return dto;
	  }

	  // utils //////////////////////////////////////////////////

	  protected internal virtual Authorization DbAuthorization
	  {
		  get
		  {
			Authorization dbAuthorization = authorizationService.createAuthorizationQuery().authorizationId(resourceId).singleResult();
    
			if (dbAuthorization == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Authorization with id " + resourceId + " does not exist.");
    
			}
			else
			{
			  return dbAuthorization;
    
			}
		  }
	  }

	}

}