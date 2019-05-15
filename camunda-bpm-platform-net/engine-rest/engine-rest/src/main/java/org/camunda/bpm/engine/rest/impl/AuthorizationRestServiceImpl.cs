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

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using AuthorizationCheckResultDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationCheckResultDto;
	using AuthorizationCreateDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationCreateDto;
	using AuthorizationDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationDto;
	using AuthorizationQueryDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using AuthorizationResource = org.camunda.bpm.engine.rest.sub.authorization.AuthorizationResource;
	using AuthorizationResourceImpl = org.camunda.bpm.engine.rest.sub.authorization.impl.AuthorizationResourceImpl;
	using ResourceUtil = org.camunda.bpm.engine.rest.util.ResourceUtil;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationRestServiceImpl : AbstractAuthorizedRestResource, AuthorizationRestService
	{

	  public AuthorizationRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName,AUTHORIZATION, ANY, objectMapper)
	  {
	  }

	  public virtual AuthorizationCheckResultDto isUserAuthorized(string permissionName, string resourceName, int? resourceType, string resourceId, string userId)
	  {

		// validate request:
		if (string.ReferenceEquals(permissionName, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Query parameter 'permissionName' cannot be null");

		}
		else if (string.ReferenceEquals(resourceName, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Query parameter 'resourceName' cannot be null");

		}
		else if (resourceType == null)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "Query parameter 'resourceType' cannot be null");

		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.Authentication currentAuthentication = processEngine.getIdentityService().getCurrentAuthentication();
		Authentication currentAuthentication = processEngine.IdentityService.CurrentAuthentication;
		if (currentAuthentication == null)
		{
		  throw new InvalidRequestException(Status.UNAUTHORIZED, "You must be authenticated in order to use this resource.");
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.AuthorizationService authorizationService = processEngine.getAuthorizationService();
		AuthorizationService authorizationService = processEngine.AuthorizationService;

		ResourceUtil resource = new ResourceUtil(resourceName, resourceType.Value);
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		Permission permission = processEngineConfiguration.PermissionProvider.getPermissionForName(permissionName, resourceType.Value);
		string currentUserId = currentAuthentication.UserId;

		bool isUserAuthorized = false;

		string userIdToCheck;
		IList<string> groupIdsToCheck = new List<string>();

		if (!string.ReferenceEquals(userId, null) && !userId.Equals(currentUserId))
		{
		  bool isCurrentUserAuthorized = authorizationService.isUserAuthorized(currentUserId, currentAuthentication.GroupIds, Permissions.READ, Resources.AUTHORIZATION);
		  if (!isCurrentUserAuthorized)
		  {
			throw new InvalidRequestException(Status.FORBIDDEN, "You must have READ permission for Authorization resource.");
		  }
		  userIdToCheck = userId;
		  groupIdsToCheck = getUserGroups(userId);
		}
		else
		{
		  // userId == null || userId.equals(currentUserId)
		  userIdToCheck = currentUserId;
		  groupIdsToCheck = currentAuthentication.GroupIds;
		}

		if (string.ReferenceEquals(resourceId, null) || org.camunda.bpm.engine.authorization.Authorization_Fields.ANY.Equals(resourceId))
		{
		  isUserAuthorized = authorizationService.isUserAuthorized(userIdToCheck, groupIdsToCheck, permission, resource);
		}
		else
		{
		  isUserAuthorized = authorizationService.isUserAuthorized(userIdToCheck, groupIdsToCheck, permission, resource, resourceId);
		}

		return new AuthorizationCheckResultDto(isUserAuthorized, permissionName, resource, resourceId);
	  }

	  public virtual AuthorizationResource getAuthorization(string id)
	  {
		return new AuthorizationResourceImpl(ProcessEngine.Name, id, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<AuthorizationDto> queryAuthorizations(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		AuthorizationQueryDto queryDto = new AuthorizationQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryAuthorizations(queryDto, firstResult, maxResults);
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.AuthorizationRestService_Fields.PATH);

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

	  public virtual IList<AuthorizationDto> queryAuthorizations(AuthorizationQueryDto queryDto, int? firstResult, int? maxResults)
	  {

		queryDto.ObjectMapper = ObjectMapper;
		AuthorizationQuery query = queryDto.toQuery(ProcessEngine);

		IList<Authorization> resultList;
		if (firstResult != null || maxResults != null)
		{
		  resultList = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  resultList = query.list();
		}

		return AuthorizationDto.fromAuthorizationList(resultList, processEngine.ProcessEngineConfiguration);
	  }

	  public virtual CountResultDto getAuthorizationCount(UriInfo uriInfo)
	  {
		AuthorizationQueryDto queryDto = new AuthorizationQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return getAuthorizationCount(queryDto);
	  }

	  protected internal virtual CountResultDto getAuthorizationCount(AuthorizationQueryDto queryDto)
	  {
		AuthorizationQuery query = queryDto.toQuery(ProcessEngine);
		long count = query.count();
		return new CountResultDto(count);
	  }

	  public virtual AuthorizationDto createAuthorization(UriInfo context, AuthorizationCreateDto dto)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.AuthorizationService authorizationService = processEngine.getAuthorizationService();
		AuthorizationService authorizationService = processEngine.AuthorizationService;

		Authorization newAuthorization = authorizationService.createNewAuthorization(dto.Type);
		AuthorizationCreateDto.update(dto, newAuthorization, processEngine.ProcessEngineConfiguration);

		newAuthorization = authorizationService.saveAuthorization(newAuthorization);

		return getAuthorization(newAuthorization.Id).getAuthorization(context);
	  }

	  // utility methods //////////////////////////////////////

	  protected internal virtual IList<Authorization> executePaginatedQuery(AuthorizationQuery query, int? firstResult, int? maxResults)
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

	  protected internal virtual IList<string> getUserGroups(string userId)
	  {
		IList<string> groupIds = new List<string>();
		IList<Group> userGroups = IdentityService.createGroupQuery().groupMember(userId).list();
		foreach (Group group in userGroups)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }

	}

}