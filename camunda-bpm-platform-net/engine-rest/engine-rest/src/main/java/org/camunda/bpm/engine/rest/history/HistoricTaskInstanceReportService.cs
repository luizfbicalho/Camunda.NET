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
	using HistoricTaskInstanceReportResultDto = org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceReportResultDto;
	using ReportResultDto = org.camunda.bpm.engine.rest.dto.history.ReportResultDto;


	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface HistoricTaskInstanceReportService
	public interface HistoricTaskInstanceReportService
	{

	  /// <summary>
	  /// creates a historic task instance report
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.HistoricTaskInstanceReportResultDto> getTaskReportResults(@Context UriInfo uriInfo);
	  IList<HistoricTaskInstanceReportResultDto> getTaskReportResults(UriInfo uriInfo);

	  /// <summary>
	  /// creates a historic task instance duration report.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/duration") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.history.ReportResultDto> getTaskDurationReportResults(@Context UriInfo uriInfo);
	  IList<ReportResultDto> getTaskDurationReportResults(UriInfo uriInfo);
	}

	public static class HistoricTaskInstanceReportService_Fields
	{
	  public const string PATH = "/report";
	}

}