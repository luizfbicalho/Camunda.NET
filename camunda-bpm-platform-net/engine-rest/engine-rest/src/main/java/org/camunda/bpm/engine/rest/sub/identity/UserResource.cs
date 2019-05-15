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
namespace org.camunda.bpm.engine.rest.sub.identity
{
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using UserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.UserCredentialsDto;
	using UserProfileDto = org.camunda.bpm.engine.rest.dto.identity.UserProfileDto;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface UserResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public void deleteUser();
	  void deleteUser();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/unlock") public void unlockUser();
	  void unlockUser();

	  // profile ///////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/profile") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public org.camunda.bpm.engine.rest.dto.identity.UserProfileDto getUserProfile(@Context UriInfo context);
	  UserProfileDto getUserProfile(UriInfo context);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/profile") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) public void updateProfile(org.camunda.bpm.engine.rest.dto.identity.UserProfileDto profile);
	  void updateProfile(UserProfileDto profile);

	  // credentials //////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/credentials") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) public void updateCredentials(org.camunda.bpm.engine.rest.dto.identity.UserCredentialsDto account);
	  void updateCredentials(UserCredentialsDto account);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OPTIONS @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.ResourceOptionsDto availableOperations(@Context UriInfo context);
	  ResourceOptionsDto availableOperations(UriInfo context);

	}

}