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

	using HistoricCaseActivityStatisticsDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseActivityStatisticsDto;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CleanableHistoricCaseInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricCaseInstanceReportResultDto;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricCaseDefinitionRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricCaseDefinitionRestService
	public interface HistoricCaseDefinitionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{id}/statistics") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricCaseActivityStatisticsDto> getHistoricCaseActivityStatistics(@PathParam("id") String caseDefinitionId);
	  IList<HistoricCaseActivityStatisticsDto> getHistoricCaseActivityStatistics(string caseDefinitionId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/cleanable-case-instance-report") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public java.util.List<org.camunda.bpm.engine.rest.dto.history.CleanableHistoricCaseInstanceReportResultDto> getCleanableHistoricCaseInstanceReport(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<CleanableHistoricCaseInstanceReportResultDto> getCleanableHistoricCaseInstanceReport(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/cleanable-case-instance-report/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public org.camunda.bpm.engine.rest.dto.CountResultDto getCleanableHistoricCaseInstanceReportCount(@Context UriInfo uriInfo);
	  CountResultDto getCleanableHistoricCaseInstanceReportCount(UriInfo uriInfo);

	}

	public static class HistoricCaseDefinitionRestService_Fields
	{
	  public const string PATH = "/case-definition";
	}

}