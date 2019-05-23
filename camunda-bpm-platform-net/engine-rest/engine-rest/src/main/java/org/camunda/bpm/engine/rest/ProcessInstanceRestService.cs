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


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using ProcessInstanceSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto;
	using SetJobRetriesByProcessDto = org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesByProcessDto;
	using DeleteProcessInstancesDto = org.camunda.bpm.engine.rest.dto.runtime.batch.DeleteProcessInstancesDto;
	using ProcessInstanceResource = org.camunda.bpm.engine.rest.sub.runtime.ProcessInstanceResource;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface ProcessInstanceRestService
	public interface ProcessInstanceRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.runtime.ProcessInstanceResource getProcessInstance(@PathParam("id") String processInstanceId);
	  ProcessInstanceResource getProcessInstance(string processInstanceId);

	  /// <summary>
	  /// Exposes the <seealso cref="ProcessInstanceQuery"/> interface as a REST service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto> getProcessInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ProcessInstanceDto> getProcessInstances(UriInfo uriInfo, int? firstResult, int? maxResults);

	  /// <summary>
	  /// Expects the same parameters as
	  /// <seealso cref="ProcessInstanceRestService.getProcessInstances(UriInfo, Integer, Integer)"/> (as a JSON message body)
	  /// and allows for any number of variable checks.
	  /// </summary>
	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto> queryProcessInstances(org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ProcessInstanceDto> queryProcessInstances(ProcessInstanceQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getProcessInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getProcessInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryProcessInstancesCount(org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto query);
	  CountResultDto queryProcessInstancesCount(ProcessInstanceQueryDto query);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto dto);
	  void updateSuspensionState(ProcessInstanceSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/suspended-async") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto updateSuspensionStateAsync(org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto dto);
	  BatchDto updateSuspensionStateAsync(ProcessInstanceSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/delete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto deleteAsync(org.camunda.bpm.engine.rest.dto.runtime.batch.DeleteProcessInstancesDto dto);
	  BatchDto deleteAsync(DeleteProcessInstancesDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/delete-historic-query-based") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto deleteAsyncHistoricQueryBased(org.camunda.bpm.engine.rest.dto.runtime.batch.DeleteProcessInstancesDto dto);
	  BatchDto deleteAsyncHistoricQueryBased(DeleteProcessInstancesDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/job-retries") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto setRetriesByProcess(org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesByProcessDto setJobRetriesDto);
	  BatchDto setRetriesByProcess(SetJobRetriesByProcessDto setJobRetriesDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/job-retries-historic-query-based") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto setRetriesByProcessHistoricQueryBased(org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesByProcessDto setJobRetriesDto);
	  BatchDto setRetriesByProcessHistoricQueryBased(SetJobRetriesByProcessDto setJobRetriesDto);
	}

	public static class ProcessInstanceRestService_Fields
	{
	  public const string PATH = "/process-instance";
	}

}