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
namespace org.camunda.bpm.engine.rest.sub.repository
{
	using StatisticsResultDto = org.camunda.bpm.engine.rest.dto.StatisticsResultDto;
	using HistoryTimeToLiveDto = org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using ProcessDefinitionDiagramDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDiagramDto;
	using ProcessDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto;
	using ProcessDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionSuspensionStateDto;
	using ProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto;
	using RestartProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.RestartProcessInstanceDto;
	using StartProcessInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.StartProcessInstanceDto;
	using FormDto = org.camunda.bpm.engine.rest.dto.task.FormDto;


	public interface ProcessDefinitionResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto getProcessDefinition();
	  ProcessDefinitionDto ProcessDefinition {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/xml") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDiagramDto getProcessDefinitionBpmn20Xml();
	  ProcessDefinitionDiagramDto ProcessDefinitionBpmn20Xml {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/diagram") javax.ws.rs.core.Response getProcessDefinitionDiagram();
	  Response ProcessDefinitionDiagram {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE Response deleteProcessDefinition(@QueryParam("cascade") boolean cascade, @QueryParam("skipCustomListeners") boolean skipCustomListeners, @QueryParam("skipIoMappings") boolean skipIoMappings);
	  Response deleteProcessDefinition(bool cascade, bool skipCustomListeners, bool skipIoMappings);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/start") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto startProcessInstance(@Context UriInfo context, org.camunda.bpm.engine.rest.dto.runtime.StartProcessInstanceDto parameters);
	  ProcessInstanceDto startProcessInstance(UriInfo context, StartProcessInstanceDto parameters);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/restart") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void restartProcessInstance(org.camunda.bpm.engine.rest.dto.runtime.RestartProcessInstanceDto restartProcessInstanceDto);
	  void restartProcessInstance(RestartProcessInstanceDto restartProcessInstanceDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/restart-async") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto restartProcessInstanceAsync(org.camunda.bpm.engine.rest.dto.runtime.RestartProcessInstanceDto restartProcessInstanceDto);
	  BatchDto restartProcessInstanceAsync(RestartProcessInstanceDto restartProcessInstanceDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/submit-form") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceDto submitForm(@Context UriInfo context, org.camunda.bpm.engine.rest.dto.runtime.StartProcessInstanceDto parameters);
	  ProcessInstanceDto submitForm(UriInfo context, StartProcessInstanceDto parameters);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/statistics") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.StatisticsResultDto> getActivityStatistics(@QueryParam("failedJobs") System.Nullable<bool> includeFailedJobs, @QueryParam("incidents") System.Nullable<bool> includeIncidents, @QueryParam("incidentsForType") String includeIncidentsForType);
	  IList<StatisticsResultDto> getActivityStatistics(bool? includeFailedJobs, bool? includeIncidents, string includeIncidentsForType);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/startForm") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.task.FormDto getStartForm();
	  FormDto StartForm {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/deployed-start-form") javax.ws.rs.core.Response getDeployedStartForm();
	  Response DeployedStartForm {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/rendered-form") @Produces(javax.ws.rs.core.MediaType.APPLICATION_XHTML_XML) javax.ws.rs.core.Response getRenderedForm();
	  Response RenderedForm {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/suspended") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateSuspensionState(org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionSuspensionStateDto dto);
	  void updateSuspensionState(ProcessDefinitionSuspensionStateDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/history-time-to-live") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateHistoryTimeToLive(org.camunda.bpm.engine.rest.dto.HistoryTimeToLiveDto historyTimeToLiveDto);
	  void updateHistoryTimeToLive(HistoryTimeToLiveDto historyTimeToLiveDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/form-variables") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.Map<String, org.camunda.bpm.engine.rest.dto.VariableValueDto> getFormVariables(@QueryParam("variableNames") String variableNames, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeValues);
	  IDictionary<string, VariableValueDto> getFormVariables(string variableNames, bool deserializeValues);
	}

}