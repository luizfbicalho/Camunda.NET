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
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using IncidentDto = org.camunda.bpm.engine.rest.dto.runtime.IncidentDto;
	using IncidentResource = org.camunda.bpm.engine.rest.sub.runtime.IncidentResource;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface IncidentRestService
	public interface IncidentRestService
	{

	  /// <summary>
	  /// Exposes the <seealso cref="IncidentQuery"/> interface as a REST service. </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.IncidentDto> getIncidents(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<IncidentDto> getIncidents(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getIncidentsCount(@Context UriInfo uriInfo);
	  CountResultDto getIncidentsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.runtime.IncidentResource getIncident(@PathParam("id") String incidentId);
	  IncidentResource getIncident(string incidentId);
	}

	public static class IncidentRestService_Fields
	{
	  public const string PATH = "/incident";
	}

}