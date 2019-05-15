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
namespace org.camunda.bpm.engine.rest
{
	using BasicUserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.BasicUserCredentialsDto;
	using PasswordDto = org.camunda.bpm.engine.rest.dto.identity.PasswordDto;
	using GroupInfoDto = org.camunda.bpm.engine.rest.dto.task.GroupInfoDto;
	using AuthenticationResult = org.camunda.bpm.engine.rest.security.auth.AuthenticationResult;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface IdentityRestService
	public interface IdentityRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/groups") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.task.GroupInfoDto getGroupInfo(@QueryParam("userId") String userId);
	  GroupInfoDto getGroupInfo(string userId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/verify") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.security.auth.AuthenticationResult verifyUser(org.camunda.bpm.engine.rest.dto.identity.BasicUserCredentialsDto credentialsDto);
	  AuthenticationResult verifyUser(BasicUserCredentialsDto credentialsDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/password-policy") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) javax.ws.rs.core.Response getPasswordPolicy();
	  Response PasswordPolicy {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/password-policy") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) javax.ws.rs.core.Response checkPassword(org.camunda.bpm.engine.rest.dto.identity.PasswordDto password);
	  Response checkPassword(PasswordDto password);
	}

	public static class IdentityRestService_Fields
	{
	  public const string PATH = "/identity";
	}

}