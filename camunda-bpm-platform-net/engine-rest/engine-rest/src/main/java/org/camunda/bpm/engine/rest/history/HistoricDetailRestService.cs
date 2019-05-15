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
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricDetailDto = org.camunda.bpm.engine.rest.dto.history.HistoricDetailDto;
	using HistoricDetailQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricDetailQueryDto;
	using VariableResource = org.camunda.bpm.engine.rest.sub.VariableResource;
	using HistoricDetailResource = org.camunda.bpm.engine.rest.sub.history.HistoricDetailResource;


	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricVariableInstanceRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricDetailRestService
	public interface HistoricDetailRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricDetailResource historicDetail(@PathParam("id") String detailId);
	  HistoricDetailResource historicDetail(string detailId);

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricActivityInstanceQuery"/> interface as a REST
	  /// service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricDetailDto> getHistoricDetails(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeObjectValues);
	  IList<HistoricDetailDto> getHistoricDetails(UriInfo uriInfo, int? firstResult, int? maxResults, bool deserializeObjectValues);

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricActivityInstanceQuery"/> interface as a REST
	  /// service with additional query parameters (compared to the GET alternative).
	  /// </summary>
	  /// <param name="queryDto"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricDetailDto> queryHistoricDetails(org.camunda.bpm.engine.rest.dto.history.HistoricDetailQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeObjectValues);
	  IList<HistoricDetailDto> queryHistoricDetails(HistoricDetailQueryDto queryDto, int? firstResult, int? maxResults, bool deserializeObjectValues);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricDetailsCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricDetailsCount(UriInfo uriInfo);

	}

	public static class HistoricDetailRestService_Fields
	{
	  public const string PATH = "/detail";
	}

}