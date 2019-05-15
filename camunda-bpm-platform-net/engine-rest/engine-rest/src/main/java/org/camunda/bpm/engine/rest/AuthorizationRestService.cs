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
	using AuthorizationCheckResultDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationCheckResultDto;
	using AuthorizationCreateDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationCreateDto;
	using AuthorizationDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationDto;
	using AuthorizationResource = org.camunda.bpm.engine.rest.sub.authorization.AuthorizationResource;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface AuthorizationRestService
	public interface AuthorizationRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/check") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.authorization.AuthorizationCheckResultDto isUserAuthorized(@QueryParam("permissionName") String permissionName, @QueryParam("resourceName") String resourceName, @QueryParam("resourceType") System.Nullable<int> resourceType, @QueryParam("resourceId") String resourceId, @QueryParam("userId") String userId);
	  AuthorizationCheckResultDto isUserAuthorized(string permissionName, string resourceName, int? resourceType, string resourceId, string userId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.authorization.AuthorizationResource getAuthorization(@PathParam("id") String id);
	  AuthorizationResource getAuthorization(string id);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.authorization.AuthorizationDto> queryAuthorizations(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<AuthorizationDto> queryAuthorizations(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getAuthorizationCount(@Context UriInfo uriInfo);
	  CountResultDto getAuthorizationCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/create") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.authorization.AuthorizationDto createAuthorization(@Context UriInfo context, org.camunda.bpm.engine.rest.dto.authorization.AuthorizationCreateDto dto);
	  AuthorizationDto createAuthorization(UriInfo context, AuthorizationCreateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OPTIONS @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.ResourceOptionsDto availableOperations(@Context UriInfo context);
	  ResourceOptionsDto availableOperations(UriInfo context);

	}

	public static class AuthorizationRestService_Fields
	{
	  public const string PATH = "/authorization";
	}

}