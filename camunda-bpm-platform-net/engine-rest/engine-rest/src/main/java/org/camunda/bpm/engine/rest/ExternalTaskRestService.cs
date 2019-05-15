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
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto;
	using ExternalTaskQueryDto = org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskQueryDto;
	using FetchExternalTasksDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksDto;
	using LockedExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.LockedExternalTaskDto;
	using SetRetriesForExternalTasksDto = org.camunda.bpm.engine.rest.dto.externaltask.SetRetriesForExternalTasksDto;
	using ExternalTaskResource = org.camunda.bpm.engine.rest.sub.externaltask.ExternalTaskResource;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface ExternalTaskRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto> getExternalTasks(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ExternalTaskDto> getExternalTasks(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskDto> queryExternalTasks(org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ExternalTaskDto> queryExternalTasks(ExternalTaskQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getExternalTasksCount(@Context UriInfo uriInfo);
	  CountResultDto getExternalTasksCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryExternalTasksCount(org.camunda.bpm.engine.rest.dto.externaltask.ExternalTaskQueryDto query);
	  CountResultDto queryExternalTasksCount(ExternalTaskQueryDto query);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/fetchAndLock") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.externaltask.LockedExternalTaskDto> fetchAndLock(org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksDto fetchingDto);
	  IList<LockedExternalTaskDto> fetchAndLock(FetchExternalTasksDto fetchingDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.externaltask.ExternalTaskResource getExternalTask(@PathParam("id") String externalTaskId);
	  ExternalTaskResource getExternalTask(string externalTaskId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/retries") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void setRetries(org.camunda.bpm.engine.rest.dto.externaltask.SetRetriesForExternalTasksDto retriesDto);
	  SetRetriesForExternalTasksDto Retries {set;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/retries-async") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto setRetriesAsync(org.camunda.bpm.engine.rest.dto.externaltask.SetRetriesForExternalTasksDto retriesDto);
	  BatchDto setRetriesAsync(SetRetriesForExternalTasksDto retriesDto);

	}

	public static class ExternalTaskRestService_Fields
	{
	  public const string PATH = "/external-task";
	}

}