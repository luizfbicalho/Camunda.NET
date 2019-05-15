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
namespace org.camunda.bpm.engine.rest.history
{


	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using DeleteHistoricProcessInstancesDto = org.camunda.bpm.engine.rest.dto.history.DeleteHistoricProcessInstancesDto;
	using HistoricProcessInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using SetRemovalTimeToHistoricProcessInstancesDto = org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricProcessInstancesDto;
	using HistoricProcessInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricProcessInstanceResource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricProcessInstanceRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricProcessInstanceRestService
	public interface HistoricProcessInstanceRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricProcessInstanceResource getHistoricProcessInstance(@PathParam("id") String processInstanceId);
	  HistoricProcessInstanceResource getHistoricProcessInstance(string processInstanceId);

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricProcessInstanceQuery"/> interface as a REST
	  /// service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto> getHistoricProcessInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricProcessInstanceDto> getHistoricProcessInstances(UriInfo uriInfo, int? firstResult, int? maxResults);

	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto> queryHistoricProcessInstances(org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricProcessInstanceDto> queryHistoricProcessInstances(HistoricProcessInstanceQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricProcessInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricProcessInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryHistoricProcessInstancesCount(org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto query);
	  CountResultDto queryHistoricProcessInstancesCount(HistoricProcessInstanceQueryDto query);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/report") @Produces({ javax.ws.rs.core.MediaType.APPLICATION_JSON, "text/csv", "application/csv" }) javax.ws.rs.core.Response getHistoricProcessInstancesReport(@Context UriInfo uriInfo, @Context Request request);
	  Response getHistoricProcessInstancesReport(UriInfo uriInfo, Request request);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/delete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto deleteAsync(org.camunda.bpm.engine.rest.dto.history.DeleteHistoricProcessInstancesDto dto);
	  BatchDto deleteAsync(DeleteHistoricProcessInstancesDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/set-removal-time") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto setRemovalTimeAsync(org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricProcessInstancesDto dto);
	  BatchDto setRemovalTimeAsync(SetRemovalTimeToHistoricProcessInstancesDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE @Path("/{id}/variable-instances") javax.ws.rs.core.Response deleteHistoricVariableInstancesByProcessInstanceId(@PathParam("id") String processInstanceId);
	  Response deleteHistoricVariableInstancesByProcessInstanceId(string processInstanceId);
	}

	public static class HistoricProcessInstanceRestService_Fields
	{
	  public const string PATH = "/process-instance";
	}

}