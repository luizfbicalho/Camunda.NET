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
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricCaseInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceDto;
	using HistoricCaseInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceQueryDto;
	using HistoricCaseInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricCaseInstanceResource;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricCaseInstanceRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricCaseInstanceRestService
	public interface HistoricCaseInstanceRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricCaseInstanceResource getHistoricCaseInstance(@PathParam("id") String caseInstanceId);
	  HistoricCaseInstanceResource getHistoricCaseInstance(string caseInstanceId);

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricCaseInstanceQuery"/> interface as a REST
	  /// service.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceDto> getHistoricCaseInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricCaseInstanceDto> getHistoricCaseInstances(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceDto> queryHistoricCaseInstances(org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricCaseInstanceDto> queryHistoricCaseInstances(HistoricCaseInstanceQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricCaseInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricCaseInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryHistoricCaseInstancesCount(org.camunda.bpm.engine.rest.dto.history.HistoricCaseInstanceQueryDto query);
	  CountResultDto queryHistoricCaseInstancesCount(HistoricCaseInstanceQueryDto query);

	}

	public static class HistoricCaseInstanceRestService_Fields
	{
	  public const string PATH = "/case-instance";
	}

}