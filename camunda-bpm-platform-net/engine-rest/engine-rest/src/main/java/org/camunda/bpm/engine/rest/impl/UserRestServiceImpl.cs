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
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using UserDto = org.camunda.bpm.engine.rest.dto.identity.UserDto;
	using UserProfileDto = org.camunda.bpm.engine.rest.dto.identity.UserProfileDto;
	using UserQueryDto = org.camunda.bpm.engine.rest.dto.identity.UserQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using UserResource = org.camunda.bpm.engine.rest.sub.identity.UserResource;
	using UserResourceImpl = org.camunda.bpm.engine.rest.sub.identity.impl.UserResourceImpl;
	using PathUtil = org.camunda.bpm.engine.rest.util.PathUtil;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UserRestServiceImpl : AbstractAuthorizedRestResource, UserRestService
	{

	  public UserRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, USER, ANY, objectMapper)
	  {
	  }

	  public virtual UserResource getUser(string id)
	  {
		id = PathUtil.decodePathParam(id);
		return new UserResourceImpl(ProcessEngine.Name, id, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<UserProfileDto> queryUsers(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		UserQueryDto queryDto = new UserQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryUsers(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<UserProfileDto> queryUsers(UserQueryDto queryDto, int? firstResult, int? maxResults)
	  {

		queryDto.ObjectMapper = ObjectMapper;
		UserQuery query = queryDto.toQuery(ProcessEngine);

		IList<User> resultList;
		if (firstResult != null || maxResults != null)
		{
		  resultList = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  resultList = query.list();
		}

		return UserProfileDto.fromUserList(resultList);
	  }


	  public virtual CountResultDto getUserCount(UriInfo uriInfo)
	  {
		UserQueryDto queryDto = new UserQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return getUserCount(queryDto);
	  }

	  protected internal virtual CountResultDto getUserCount(UserQueryDto queryDto)
	  {
		UserQuery query = queryDto.toQuery(ProcessEngine);
		long count = query.count();
		return new CountResultDto(count);
	  }

	  public virtual void createUser(UserDto userDto)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = getIdentityService();
		IdentityService identityService = IdentityService;

		if (identityService.ReadOnly)
		{
		  throw new InvalidRequestException(Status.FORBIDDEN, "Identity service implementation is read-only.");
		}

		UserProfileDto profile = userDto.Profile;
		if (profile == null || string.ReferenceEquals(profile.Id, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "request object must provide profile information with valid id.");
		}

		User newUser = identityService.newUser(profile.Id);
		profile.update(newUser);

		if (userDto.Credentials != null)
		{
		  newUser.Password = userDto.Credentials.Password;
		}

		identityService.saveUser(newUser);

	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = getIdentityService();
		IdentityService identityService = IdentityService;

		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH);

		ResourceOptionsDto resourceOptionsDto = new ResourceOptionsDto();

		// GET /
		URI baseUri = baseUriBuilder.build();
		resourceOptionsDto.addReflexiveLink(baseUri, HttpMethod.GET, "list");

		// GET /count
		URI countUri = baseUriBuilder.clone().path("/count").build();
		resourceOptionsDto.addReflexiveLink(countUri, HttpMethod.GET, "count");

		// POST /create
		if (!identityService.ReadOnly && isAuthorized(CREATE))
		{
		  URI createUri = baseUriBuilder.clone().path("/create").build();
		  resourceOptionsDto.addReflexiveLink(createUri, HttpMethod.POST, "create");
		}

		return resourceOptionsDto;
	  }

	  // utility methods //////////////////////////////////////

	  protected internal virtual IList<User> executePaginatedQuery(UserQuery query, int? firstResult, int? maxResults)
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