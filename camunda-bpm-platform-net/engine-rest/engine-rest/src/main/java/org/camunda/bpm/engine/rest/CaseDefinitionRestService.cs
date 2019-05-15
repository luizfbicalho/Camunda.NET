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


	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CaseDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.CaseDefinitionDto;
	using CaseDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.CaseDefinitionResource;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface CaseDefinitionRestService
	public interface CaseDefinitionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.repository.CaseDefinitionResource getCaseDefinitionById(@PathParam("id") String caseDefinitionId);
	  CaseDefinitionResource getCaseDefinitionById(string caseDefinitionId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/key/{key}") org.camunda.bpm.engine.rest.sub.repository.CaseDefinitionResource getCaseDefinitionByKey(@PathParam("key") String caseDefinitionKey);
	  CaseDefinitionResource getCaseDefinitionByKey(string caseDefinitionKey);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/key/{key}/tenant-id/{tenantId}") org.camunda.bpm.engine.rest.sub.repository.CaseDefinitionResource getCaseDefinitionByKeyAndTenantId(@PathParam("key") String caseDefinitionKey, @PathParam("tenantId") String tenantId);
	  CaseDefinitionResource getCaseDefinitionByKeyAndTenantId(string caseDefinitionKey, string tenantId);

	  /// <summary>
	  /// Exposes the <seealso cref="CaseDefinitionQuery"/> interface as a REST service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.repository.CaseDefinitionDto> getCaseDefinitions(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<CaseDefinitionDto> getCaseDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getCaseDefinitionsCount(@Context UriInfo uriInfo);
	  CountResultDto getCaseDefinitionsCount(UriInfo uriInfo);

	}

	public static class CaseDefinitionRestService_Fields
	{
	  public const string PATH = "/case-definition";
	}

}