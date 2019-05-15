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


	using PasswordPolicyResult = org.camunda.bpm.engine.identity.PasswordPolicyResult;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using BasicUserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.BasicUserCredentialsDto;
	using CheckPasswordPolicyResultDto = org.camunda.bpm.engine.rest.dto.identity.CheckPasswordPolicyResultDto;
	using PasswordDto = org.camunda.bpm.engine.rest.dto.identity.PasswordDto;
	using PasswordPolicyDto = org.camunda.bpm.engine.rest.dto.identity.PasswordPolicyDto;
	using GroupDto = org.camunda.bpm.engine.rest.dto.task.GroupDto;
	using GroupInfoDto = org.camunda.bpm.engine.rest.dto.task.GroupInfoDto;
	using UserDto = org.camunda.bpm.engine.rest.dto.task.UserDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using AuthenticationResult = org.camunda.bpm.engine.rest.security.auth.AuthenticationResult;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class IdentityRestServiceImpl : AbstractRestProcessEngineAware, IdentityRestService
	{

	  public IdentityRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual GroupInfoDto getGroupInfo(string userId)
	  {
		if (string.ReferenceEquals(userId, null))
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "No user id was supplied");
		}

		IdentityService identityService = ProcessEngine.IdentityService;

		GroupQuery query = identityService.createGroupQuery();
		IList<Group> userGroups = query.groupMember(userId).orderByGroupName().asc().list();

		ISet<UserDto> allGroupUsers = new HashSet<UserDto>();
		IList<GroupDto> allGroups = new List<GroupDto>();

		foreach (Group group in userGroups)
		{
		  IList<User> groupUsers = identityService.createUserQuery().memberOfGroup(group.Id).list();
		  foreach (User user in groupUsers)
		  {
			if (!user.Id.Equals(userId))
			{
			  allGroupUsers.Add(new UserDto(user.Id, user.FirstName, user.LastName));
			}
		  }
		  allGroups.Add(new GroupDto(group.Id, group.Name));
		}

		return new GroupInfoDto(allGroups, allGroupUsers);
	  }

	  public virtual AuthenticationResult verifyUser(BasicUserCredentialsDto credentialsDto)
	  {
		if (string.ReferenceEquals(credentialsDto.Username, null) || string.ReferenceEquals(credentialsDto.Password, null))
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Username and password are required");
		}
		IdentityService identityService = ProcessEngine.IdentityService;
		bool valid = identityService.checkPassword(credentialsDto.Username, credentialsDto.Password);
		if (valid)
		{
		  return AuthenticationResult.successful(credentialsDto.Username);
		}
		else
		{
		  return AuthenticationResult.unsuccessful(credentialsDto.Username);
		}
	  }

	  public virtual Response PasswordPolicy
	  {
		  get
		  {
			bool isEnabled = processEngine.ProcessEngineConfiguration.EnablePasswordPolicy;
    
			if (isEnabled)
			{
			  IdentityService identityService = processEngine.IdentityService;
    
			  return Response.status(Response.Status.OK.StatusCode).entity(PasswordPolicyDto.fromPasswordPolicy(identityService.PasswordPolicy)).build();
    
			}
			else
			{
			  return Response.status(Response.Status.NOT_FOUND.StatusCode).build();
    
			}
		  }
	  }

	  public virtual Response checkPassword(PasswordDto password)
	  {
		bool isEnabled = processEngine.ProcessEngineConfiguration.EnablePasswordPolicy;

		if (isEnabled)
		{
		  IdentityService identityService = processEngine.IdentityService;

		  PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(password.Password);

		  return Response.status(Response.Status.OK.StatusCode).entity(CheckPasswordPolicyResultDto.fromPasswordPolicyResult(result)).build();

		}
		else
		{
		  return Response.status(Response.Status.NOT_FOUND.StatusCode).build();

		}
	  }
	}

}