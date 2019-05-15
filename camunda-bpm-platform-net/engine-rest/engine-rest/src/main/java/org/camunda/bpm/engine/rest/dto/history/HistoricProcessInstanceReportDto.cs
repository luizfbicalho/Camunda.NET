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
	using HistoricProcessInstanceReport = org.camunda.bpm.engine.history.HistoricProcessInstanceReport;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricProcessInstanceReportDto : AbstractReportDto<HistoricProcessInstanceReport>
	{

	  protected internal string[] processDefinitionIdIn;
	  protected internal string[] processDefinitionKeyIn;
	  protected internal DateTime startedAfter;
	  protected internal DateTime startedBefore;

	  public new const string REPORT_TYPE_DURATION = "duration";

	  public static readonly new IList<string> VALID_REPORT_TYPE_VALUES;
	  static HistoricProcessInstanceReportDto()
	  {
		VALID_REPORT_TYPE_VALUES = new List<string>();
		VALID_REPORT_TYPE_VALUES.Add(REPORT_TYPE_DURATION);
	  }

	  public HistoricProcessInstanceReportDto()
	  {
	  }

	  public HistoricProcessInstanceReportDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam(value : "processDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessDefinitionIdIn
	  {
		  set
		  {
			this.processDefinitionIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "processDefinitionKeyIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ProcessDefinitionKeyIn
	  {
		  set
		  {
			this.processDefinitionKeyIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "startedAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime StartedAfter
	  {
		  set
		  {
			this.startedAfter = value;
		  }
	  }

	  [CamundaQueryParam(value : "startedBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime StartedBefore
	  {
		  set
		  {
			this.startedBefore = value;
		  }
	  }

	  protected internal override HistoricProcessInstanceReport createNewReportQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createHistoricProcessInstanceReport();
	  }

	  protected internal override void applyFilters(HistoricProcessInstanceReport reportQuery)
	  {
		if (processDefinitionIdIn != null && processDefinitionIdIn.Length > 0)
		{
		  reportQuery.processDefinitionIdIn(processDefinitionIdIn);
		}
		if (processDefinitionKeyIn != null && processDefinitionKeyIn.Length > 0)
		{
		  reportQuery.processDefinitionKeyIn(processDefinitionKeyIn);
		}
		if (startedAfter != null)
		{
		  reportQuery.startedAfter(startedAfter);
		}
		if (startedBefore != null)
		{
		  reportQuery.startedBefore(startedBefore);
		}
		if (!REPORT_TYPE_DURATION.Equals(reportType))
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Unknown report type " + reportType);
		}
	  }

	}

}