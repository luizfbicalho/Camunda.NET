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


	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using StatisticsResultDto = org.camunda.bpm.engine.rest.dto.StatisticsResultDto;
	using ProcessDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto;
	using ProcessDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionSuspensionStateDto;
	using ProcessDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.ProcessDefinitionResource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface ProcessDefinitionRestService
	public interface ProcessDefinitionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.repository.ProcessDefinitionResource getProcessDefinitionById(@PathParam("id") String processDefinitionId);
	  ProcessDefinitionResource getProcessDefinitionById(string processDefinitionId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/key/{key}") org.camunda.bpm.engine.rest.sub.repository.ProcessDefinitionResource getProcessDefinitionByKey(@PathParam("key") String processDefinitionKey);
	  ProcessDefinitionResource getProcessDefinitionByKey(string processDefinitionKey);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/key/{key}/tenant-id/{tenantId}") org.camunda.bpm.engine.rest.sub.repository.ProcessDefinitionResource getProcessDefinitionByKeyAndTenantId(@PathParam("key") String processDefinitionKey, @PathParam("tenantId") String tenantId);
	  ProcessDefinitionResource getProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId);

	  /// <summary>
	  /// Exposes the <seealso cref="ProcessDefinitionQuery"/> interface as a REST service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto> getProcessDefinitions(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<ProcessDefinitionDto> getProcessDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getProcessDefinitionsCount(@Context UriInfo uriInfo);
	  CountResultDto getProcessDefinitionsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/statistics") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.StatisticsResultDto> getStatistics(@QueryParam("failedJobs") System.Nullable<bool> includeFailedJobs, @QueryParam("rootIncidents") System.Nullable<bool> includeRootIncidents, @QueryParam("incidents") System.Nullable<bool> includeIncidents, @QueryParam("incidentsForType") String includeIncidentsForType);
	  IList<StatisticsResultDto> getStatistics(bool? includeFailedJobs, bool? includeRootIncidents, bool? includeIncidents, string includeIncidentsForType);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionSuspensionStateDto dto);
	  void updateSuspensionState(ProcessDefinitionSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE @Path("/key/{key}/delete") void deleteProcessDefinitionsByKey(@PathParam("key") String processDefinitionKey, @QueryParam("cascade") boolean cascade, @QueryParam("skipCustomListeners") boolean skipCustomListeners, @QueryParam("skipIoMappings") boolean skipIoMappings);
	  void deleteProcessDefinitionsByKey(string processDefinitionKey, bool cascade, bool skipCustomListeners, bool skipIoMappings);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE @Path("/key/{key}/tenant-id/{tenantId}/delete") void deleteProcessDefinitionsByKeyAndTenantId(@PathParam("key") String processDefinitionKey, @QueryParam("cascade") boolean cascade, @QueryParam("skipCustomListeners") boolean skipCustomListeners, @QueryParam("skipIoMappings") boolean skipIoMappings, @PathParam("tenantId") String tenantId);
	  void deleteProcessDefinitionsByKeyAndTenantId(string processDefinitionKey, bool cascade, bool skipCustomListeners, bool skipIoMappings, string tenantId);
	}

	public static class ProcessDefinitionRestService_Fields
	{
	  public const string APPLICATION_BPMN20_XML = "application/bpmn20+xml";
	  public static readonly MediaType APPLICATION_BPMN20_XML_TYPE = new MediaType("application", "bpmn20+xml");
	  public const string PATH = "/process-definition";
	}

}