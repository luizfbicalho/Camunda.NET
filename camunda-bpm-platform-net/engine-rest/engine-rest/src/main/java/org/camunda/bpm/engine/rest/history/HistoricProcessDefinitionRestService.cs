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
	using HistoricActivityStatisticsDto = org.camunda.bpm.engine.rest.dto.history.HistoricActivityStatisticsDto;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using CleanableHistoricProcessInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.CleanableHistoricProcessInstanceReportResultDto;



	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path(HistoricProcessDefinitionRestService_Fields.PATH) @Produces(MediaType.APPLICATION_JSON) public interface HistoricProcessDefinitionRestService
	public interface HistoricProcessDefinitionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{id}/statistics") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricActivityStatisticsDto> getHistoricActivityStatistics(@Context UriInfo uriInfo, @PathParam("id") String processDefinitionId, @QueryParam("canceled") System.Nullable<bool> includeCanceled, @QueryParam("finished") System.Nullable<bool> includeFinished, @QueryParam("completeScope") System.Nullable<bool> includeCompleteScope, @QueryParam("sortBy") String sortBy, @QueryParam("sortOrder") String sortOrder);
	  IList<HistoricActivityStatisticsDto> getHistoricActivityStatistics(UriInfo uriInfo, string processDefinitionId, bool? includeCanceled, bool? includeFinished, bool? includeCompleteScope, string sortBy, string sortOrder);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/cleanable-process-instance-report") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public java.util.List<org.camunda.bpm.engine.rest.dto.history.CleanableHistoricProcessInstanceReportResultDto> getCleanableHistoricProcessInstanceReport(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<CleanableHistoricProcessInstanceReportResultDto> getCleanableHistoricProcessInstanceReport(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/cleanable-process-instance-report/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public org.camunda.bpm.engine.rest.dto.CountResultDto getCleanableHistoricProcessInstanceReportCount(@Context UriInfo uriInfo);
	  CountResultDto getCleanableHistoricProcessInstanceReportCount(UriInfo uriInfo);

	}

	public static class HistoricProcessDefinitionRestService_Fields
	{
	  public const string PATH = "/process-definition";
	}

}