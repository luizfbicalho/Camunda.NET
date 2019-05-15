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
namespace org.camunda.bpm.engine.rest
{
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using UserDto = org.camunda.bpm.engine.rest.dto.identity.UserDto;
	using UserProfileDto = org.camunda.bpm.engine.rest.dto.identity.UserProfileDto;
	using UserResource = org.camunda.bpm.engine.rest.sub.identity.UserResource;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface UserRestService
	public interface UserRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.identity.UserResource getUser(@PathParam("id") String id);
	  UserResource getUser(string id);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.identity.UserProfileDto> queryUsers(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<UserProfileDto> queryUsers(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getUserCount(@Context UriInfo uriInfo);
	  CountResultDto getUserCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/create") @Consumes void createUser(org.camunda.bpm.engine.rest.dto.identity.UserDto userDto);
	  void createUser(UserDto userDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OPTIONS @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.ResourceOptionsDto availableOperations(@Context UriInfo context);
	  ResourceOptionsDto availableOperations(UriInfo context);
	}

	public static class UserRestService_Fields
	{
	  public const string PATH = "/user";
	}

}