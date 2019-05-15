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
	using JobDto = org.camunda.bpm.engine.rest.dto.runtime.JobDto;
	using JobQueryDto = org.camunda.bpm.engine.rest.dto.runtime.JobQueryDto;
	using JobSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto;
	using SetJobRetriesDto = org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesDto;
	using JobResource = org.camunda.bpm.engine.rest.sub.runtime.JobResource;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface JobRestService
	public interface JobRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.runtime.JobResource getJob(@PathParam("id") String jobId);
	  JobResource getJob(string jobId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.JobDto> getJobs(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<JobDto> getJobs(UriInfo uriInfo, int? firstResult, int? maxResults);


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.JobDto> queryJobs(org.camunda.bpm.engine.rest.dto.runtime.JobQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<JobDto> queryJobs(JobQueryDto queryDto, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getJobsCount(@Context UriInfo uriInfo);
	  CountResultDto getJobsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryJobsCount(org.camunda.bpm.engine.rest.dto.runtime.JobQueryDto queryDto);
	  CountResultDto queryJobsCount(JobQueryDto queryDto);


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/retries") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto setRetries(org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesDto setJobRetriesDto);
	  BatchDto setRetries(SetJobRetriesDto setJobRetriesDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto dto);
	  void updateSuspensionState(JobSuspensionStateDto dto);
	}

	public static class JobRestService_Fields
	{
		public const string PATH = "/job";
	}

}