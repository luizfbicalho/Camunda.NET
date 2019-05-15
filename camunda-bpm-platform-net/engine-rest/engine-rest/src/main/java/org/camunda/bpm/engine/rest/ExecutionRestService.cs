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
	using ExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionDto;
	using ExecutionQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionQueryDto;
	using ExecutionResource = org.camunda.bpm.engine.rest.sub.runtime.ExecutionResource;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface ExecutionRestService
	public interface ExecutionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.runtime.ExecutionResource getExecution(@PathParam("id") String executionId);
	  ExecutionResource getExecution(string executionId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.ExecutionDto> getExecutions(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ExecutionDto> getExecutions(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.ExecutionDto> queryExecutions(org.camunda.bpm.engine.rest.dto.runtime.ExecutionQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ExecutionDto> queryExecutions(ExecutionQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getExecutionsCount(@Context UriInfo uriInfo);
	  CountResultDto getExecutionsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryExecutionsCount(org.camunda.bpm.engine.rest.dto.runtime.ExecutionQueryDto query);
	  CountResultDto queryExecutionsCount(ExecutionQueryDto query);
	}

	public static class ExecutionRestService_Fields
	{
	  public const string PATH = "/execution";
	}

}