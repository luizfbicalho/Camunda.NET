﻿using System.Collections.Generic;

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
	using HistoricJobLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricJobLogDto;
	using HistoricJobLogQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricJobLogQueryDto;
	using HistoricJobLogResource = org.camunda.bpm.engine.rest.sub.history.HistoricJobLogResource;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// @since 7.3
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricJobLogRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricJobLogRestService
	public interface HistoricJobLogRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricJobLogResource getHistoricJobLog(@PathParam("id") String historicJobLogId);
	  HistoricJobLogResource getHistoricJobLog(string historicJobLogId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricJobLogDto> getHistoricJobLogs(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricJobLogDto> getHistoricJobLogs(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricJobLogDto> queryHistoricJobLogs(org.camunda.bpm.engine.rest.dto.history.HistoricJobLogQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricJobLogDto> queryHistoricJobLogs(HistoricJobLogQueryDto queryDto, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricJobLogsCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricJobLogsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryHistoricJobLogsCount(org.camunda.bpm.engine.rest.dto.history.HistoricJobLogQueryDto queryDto);
	  CountResultDto queryHistoricJobLogsCount(HistoricJobLogQueryDto queryDto);

	}

	public static class HistoricJobLogRestService_Fields
	{
	  public const string PATH = "/job-log";
	}

}