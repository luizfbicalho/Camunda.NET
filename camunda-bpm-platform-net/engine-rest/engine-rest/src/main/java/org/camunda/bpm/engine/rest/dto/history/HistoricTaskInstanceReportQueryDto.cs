using System;
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
namespace org.camunda.bpm.engine.rest.dto.history
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using HistoricTaskInstanceReport = org.camunda.bpm.engine.history.HistoricTaskInstanceReport;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class HistoricTaskInstanceReportQueryDto : AbstractReportDto<HistoricTaskInstanceReport>
	{

	  public const string PROCESS_DEFINITION = "processDefinition";
	  public const string TASK_NAME = "taskName";

	  protected internal DateTime completedBefore;
	  protected internal DateTime completedAfter;
	  protected internal string groupby;


	  public HistoricTaskInstanceReportQueryDto()
	  {
	  }

	  public HistoricTaskInstanceReportQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  public virtual DateTime CompletedBefore
	  {
		  get
		  {
			return completedBefore;
		  }
		  set
		  {
			this.completedBefore = value;
		  }
	  }

	  public virtual DateTime CompletedAfter
	  {
		  get
		  {
			return completedAfter;
		  }
		  set
		  {
			this.completedAfter = value;
		  }
	  }

	  public virtual string GroupBy
	  {
		  get
		  {
			return groupby;
		  }
		  set
		  {
			this.groupby = value;
		  }
	  }




	  protected internal virtual void applyFilters(HistoricTaskInstanceReport reportQuery)
	  {
		if (completedBefore != null)
		{
		  reportQuery.completedBefore(completedBefore);
		}
		if (completedAfter != null)
		{
		  reportQuery.completedAfter(completedAfter);
		}

		if (REPORT_TYPE_DURATION.Equals(reportType))
		{
		  if (periodUnit == null)
		  {
			throw new InvalidRequestException(Response.Status.BAD_REQUEST, "periodUnit is null");
		  }
		}

	  }

	  protected internal override HistoricTaskInstanceReport createNewReportQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricTaskInstanceReport();
	  }

	  public virtual IList<HistoricTaskInstanceReportResult> executeCompletedReport(ProcessEngine engine)
	  {
		HistoricTaskInstanceReport reportQuery = createNewReportQuery(engine);
		applyFilters(reportQuery);

		if (PROCESS_DEFINITION.Equals(groupby))
		{
		  return reportQuery.countByProcessDefinitionKey();
		}
		else if (TASK_NAME.Equals(groupby))
		{
		  return reportQuery.countByTaskName();
		}
		else
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "groupBy parameter has invalid value: " + groupby);
		}
	  }
	}

}