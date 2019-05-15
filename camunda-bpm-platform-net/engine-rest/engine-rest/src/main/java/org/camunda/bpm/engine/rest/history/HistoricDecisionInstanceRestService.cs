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


	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricDecisionInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceDto;
	using SetRemovalTimeToHistoricDecisionInstancesDto = org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricDecisionInstancesDto;
	using DeleteHistoricDecisionInstancesDto = org.camunda.bpm.engine.rest.dto.history.batch.DeleteHistoricDecisionInstancesDto;
	using HistoricDecisionInstanceResource = org.camunda.bpm.engine.rest.sub.history.HistoricDecisionInstanceResource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricDecisionInstanceRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricDecisionInstanceRestService
	public interface HistoricDecisionInstanceRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.history.HistoricDecisionInstanceResource getHistoricDecisionInstance(@PathParam("id") String decisionInstanceId);
	  HistoricDecisionInstanceResource getHistoricDecisionInstance(string decisionInstanceId);

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricDecisionInstanceQuery"/> interface as a REST
	  /// service.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceDto> getHistoricDecisionInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricDecisionInstanceDto> getHistoricDecisionInstances(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricDecisionInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricDecisionInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/delete") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto deleteAsync(org.camunda.bpm.engine.rest.dto.history.batch.DeleteHistoricDecisionInstancesDto dto);
	  BatchDto deleteAsync(DeleteHistoricDecisionInstancesDto dto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/set-removal-time") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.batch.BatchDto setRemovalTimeAsync(org.camunda.bpm.engine.rest.dto.history.batch.removaltime.SetRemovalTimeToHistoricDecisionInstancesDto dto);
	  BatchDto setRemovalTimeAsync(SetRemovalTimeToHistoricDecisionInstancesDto dto);

	}

	public static class HistoricDecisionInstanceRestService_Fields
	{
	  public const string PATH = "/decision-instance";
	}

}