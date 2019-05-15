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
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricTaskInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto;
	using HistoricTaskInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceQueryDto;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricVariableInstanceRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricTaskInstanceRestService
	public interface HistoricTaskInstanceRestService
	{

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricTaskInstanceQuery"/> interface as a REST
	  /// service.
	  /// </summary>
	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto> getHistoricTaskInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricTaskInstanceDto> getHistoricTaskInstances(UriInfo uriInfo, int? firstResult, int? maxResults);

	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceDto> queryHistoricTaskInstances(org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricTaskInstanceDto> queryHistoricTaskInstances(HistoricTaskInstanceQueryDto queryDto, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricTaskInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricTaskInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryHistoricTaskInstancesCount(org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceQueryDto queryDto);
	  CountResultDto queryHistoricTaskInstancesCount(HistoricTaskInstanceQueryDto queryDto);

	  /// <summary>
	  /// Provides a report sub module
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("report") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) javax.ws.rs.core.Response getHistoricTaskInstanceReport(@Context UriInfo uriInfo);
	  Response getHistoricTaskInstanceReport(UriInfo uriInfo);
	}

	public static class HistoricTaskInstanceRestService_Fields
	{
	  public const string PATH = "/task";
	}

}