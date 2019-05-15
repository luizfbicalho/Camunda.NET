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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using User = org.camunda.bpm.engine.identity.User;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using UserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.UserCredentialsDto;
	using UserProfileDto = org.camunda.bpm.engine.rest.dto.identity.UserProfileDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UserResourceImpl : AbstractIdentityResource, UserResource
	{

	  protected internal string rootResourcePath;

	  public UserResourceImpl(string processEngineName, string userId, string rootResourcePath, ObjectMapper objectMapper) : base(processEngineName, USER, userId, objectMapper)
	  {
		this.rootResourcePath = rootResourcePath;
	  }

	  public virtual UserProfileDto getUserProfile(UriInfo context)
	  {

		User dbUser = findUserObject();
		if (dbUser == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "User with id " + resourceId + " does not exist");
		}

		UserProfileDto user = UserProfileDto.fromUser(dbUser);

		return user;
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {
		ResourceOptionsDto dto = new ResourceOptionsDto();

		// add links if operations are authorized
		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.UserRestService_Fields.PATH).path(resourceId);
		URI baseUri = baseUriBuilder.build();
		URI profileUri = baseUriBuilder.path("/profile").build();

		dto.addReflexiveLink(profileUri, HttpMethod.GET, "self");

		if (!identityService.ReadOnly && isAuthorized(DELETE))
		{
		  dto.addReflexiveLink(baseUri, HttpMethod.DELETE, "delete");
		}
		if (!identityService.ReadOnly && isAuthorized(UPDATE))
		{
		  dto.addReflexiveLink(profileUri, HttpMethod.PUT, "update");
		}

		return dto;
	  }

	  public virtual void deleteUser()
	  {
		ensureNotReadOnly();
		identityService.deleteUser(resourceId);
	  }

	  public virtual void unlockUser()
	  {
		ensureNotReadOnly();
		identityService.unlockUser(resourceId);
	  }

	  public virtual void updateCredentials(UserCredentialsDto account)
	  {
		ensureNotReadOnly();

		Authentication currentAuthentication = identityService.CurrentAuthentication;
		if (currentAuthentication != null && !string.ReferenceEquals(currentAuthentication.UserId, null))
		{
		  if (!identityService.checkPassword(currentAuthentication.UserId, account.AuthenticatedUserPassword))
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "The given authenticated user password is not valid.");
		  }
		}

		User dbUser = findUserObject();
		if (dbUser == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "User with id " + resourceId + " does not exist");
		}

		dbUser.Password = account.Password;

		identityService.saveUser(dbUser);
	  }

	  public virtual void updateProfile(UserProfileDto profile)
	  {
		ensureNotReadOnly();

		User dbUser = findUserObject();
		if (dbUser == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "User with id " + resourceId + " does not exist");
		}

		profile.update(dbUser);

		identityService.saveUser(dbUser);
	  }

	  protected internal virtual User findUserObject()
	  {
		try
		{
		  return identityService.createUserQuery().userId(resourceId).singleResult();
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, "Exception while performing user query: " + e.Message);
		}
	  }

	}

}