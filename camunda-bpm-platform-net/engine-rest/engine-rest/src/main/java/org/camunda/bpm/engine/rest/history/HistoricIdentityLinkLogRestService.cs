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
	using HistoricIdentityLinkLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricIdentityLinkLogDto;

	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricIdentityLinkLogRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricIdentityLinkLogRestService
	public interface HistoricIdentityLinkLogRestService
	{

	  /// <summary>
	  /// Exposes the <seealso cref="HistoricIdentityLinkLogQuery"/> interface as a REST service.
	  /// </summary>
	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricIdentityLinkLogDto> getHistoricIdentityLinks(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<HistoricIdentityLinkLogDto> getHistoricIdentityLinks(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getHistoricIdentityLinksCount(@Context UriInfo uriInfo);
	  CountResultDto getHistoricIdentityLinksCount(UriInfo uriInfo);

	}

	public static class HistoricIdentityLinkLogRestService_Fields
	{
	  public const string PATH = "/identity-link-log";
	}

}