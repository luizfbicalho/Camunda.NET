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
	using CaseExecutionDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto;
	using CaseExecutionQueryDto = org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionQueryDto;
	using CaseExecutionResource = org.camunda.bpm.engine.rest.sub.runtime.CaseExecutionResource;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;


	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface CaseExecutionRestService
	public interface CaseExecutionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.runtime.CaseExecutionResource getCaseExecution(@PathParam("id") String caseExecutionId);
	  CaseExecutionResource getCaseExecution(string caseExecutionId);

	  /// <summary>
	  /// Exposes the <seealso cref="CaseExecutionQuery"/> interface as a REST service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto> getCaseExecutions(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<CaseExecutionDto> getCaseExecutions(UriInfo uriInfo, int? firstResult, int? maxResults);

	  /// <summary>
	  /// Expects the same parameters as
	  /// <seealso cref="CaseExecutionRestService#getCaseExecutions(UriInfo, Integer, Integer)"/> (as a JSON message body)
	  /// and allows for any number of variable checks.
	  /// </summary>
	  /// <param name="query"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionDto> queryCaseExecutions(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionQueryDto query, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<CaseExecutionDto> queryCaseExecutions(CaseExecutionQueryDto query, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getCaseExecutionsCount(@Context UriInfo uriInfo);
	  CountResultDto getCaseExecutionsCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryCaseExecutionsCount(org.camunda.bpm.engine.rest.dto.runtime.CaseExecutionQueryDto query);
	  CountResultDto queryCaseExecutionsCount(CaseExecutionQueryDto query);

	}

	public static class CaseExecutionRestService_Fields
	{
	  public const string PATH = "/case-execution";
	}

}