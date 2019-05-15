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
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using JobDefinitionDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto;
	using JobDefinitionQueryDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionQueryDto;
	using JobDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto;
	using JobDefinitionResource = org.camunda.bpm.engine.rest.sub.management.JobDefinitionResource;


	/// <summary>
	/// @author roman.smirnov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface JobDefinitionRestService
	public interface JobDefinitionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.management.JobDefinitionResource getJobDefinition(@PathParam("id") String jobDefinitionId);
	  JobDefinitionResource getJobDefinition(string jobDefinitionId);

	  /// <summary>
	  /// Exposes the <seealso cref="JobDefinitionQuery"/> interface as a REST service. </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto> getJobDefinitions(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<JobDefinitionDto> getJobDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.management.JobDefinitionDto> queryJobDefinitions(org.camunda.bpm.engine.rest.dto.management.JobDefinitionQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<JobDefinitionDto> queryJobDefinitions(JobDefinitionQueryDto queryDto, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getJobDefinitionsCount(@Context UriInfo uriInfo);
	  CountResultDto getJobDefinitionsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryJobDefinitionsCount(org.camunda.bpm.engine.rest.dto.management.JobDefinitionQueryDto queryDto);
	  CountResultDto queryJobDefinitionsCount(JobDefinitionQueryDto queryDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto dto);
	  void updateSuspensionState(JobDefinitionSuspensionStateDto dto);

	}

	public static class JobDefinitionRestService_Fields
	{
	  public const string PATH = "/job-definition";
	}

}