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
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using HistoricVariableInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto;
	using HistoricVariableInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceQueryDto;
	using VariableResource = org.camunda.bpm.engine.rest.sub.VariableResource;
	using HistoricVariableInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricVariableInstanceResource;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricVariableInstanceRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricVariableInstanceRestService
	public interface HistoricVariableInstanceRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricVariableInstanceResource variableInstanceResource(@PathParam("id") String id);
	  HistoricVariableInstanceResource variableInstanceResource(string id);

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricVariableInstanceQuery"/> interface as a REST
	  /// service.
	  /// </summary>
	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto> getHistoricVariableInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeValues);
	  IList<HistoricVariableInstanceDto> getHistoricVariableInstances(UriInfo uriInfo, int? firstResult, int? maxResults, bool deserializeValues);

	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto> queryHistoricVariableInstances(org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeValues);
	  IList<HistoricVariableInstanceDto> queryHistoricVariableInstances(HistoricVariableInstanceQueryDto query, int? firstResult, int? maxResults, bool deserializeValues);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricVariableInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricVariableInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryHistoricVariableInstancesCount(org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceQueryDto query);
	  CountResultDto queryHistoricVariableInstancesCount(HistoricVariableInstanceQueryDto query);
	}

	public static class HistoricVariableInstanceRestService_Fields
	{
	  public const string PATH = "/variable-instance";
	}

}