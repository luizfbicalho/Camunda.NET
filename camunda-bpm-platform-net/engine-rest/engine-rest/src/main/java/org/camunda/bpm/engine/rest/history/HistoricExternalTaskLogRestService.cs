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
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricExternalTaskLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogDto;
	using HistoricExternalTaskLogQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogQueryDto;
	using HistoricExternalTaskLogResource = org.camunda.bpm.engine.rest.sub.history.HistoricExternalTaskLogResource;


	/// <summary>
	/// @since 7.7
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricExternalTaskLogRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricExternalTaskLogRestService
	public interface HistoricExternalTaskLogRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricExternalTaskLogResource getHistoricExternalTaskLog(@PathParam("id") String historicExternalTaskLogId);
	  HistoricExternalTaskLogResource getHistoricExternalTaskLog(string historicExternalTaskLogId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogDto> getHistoricExternalTaskLogs(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricExternalTaskLogDto> getHistoricExternalTaskLogs(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogDto> queryHistoricExternalTaskLogs(org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricExternalTaskLogDto> queryHistoricExternalTaskLogs(HistoricExternalTaskLogQueryDto queryDto, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricExternalTaskLogsCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricExternalTaskLogsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryHistoricExternalTaskLogsCount(org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogQueryDto queryDto);
	  CountResultDto queryHistoricExternalTaskLogsCount(HistoricExternalTaskLogQueryDto queryDto);
	}

	public static class HistoricExternalTaskLogRestService_Fields
	{
	  public const string PATH = "/external-task-log";
	}

}