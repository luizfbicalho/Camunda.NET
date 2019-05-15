using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.rest.dto.converter
{

	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using ReportResult = org.camunda.bpm.engine.history.ReportResult;
	using HistoricProcessInstanceReportDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceReportDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ReportResultToCsvConverter
	{

	  protected internal static string DELIMITER = ",";
	  protected internal static string NEW_LINE_SEPARATOR = "\n";

	  public static string DURATION_HEADER = "PERIOD"
								  + DELIMITER + "PERIOD_UNIT"
								  + DELIMITER + "MINIMUM"
								  + DELIMITER + "MAXIMUM"
								  + DELIMITER + "AVERAGE";

	  public static string convertReportResult(IList<ReportResult> reports, string reportType)
	  {
		if (HistoricProcessInstanceReportDto.REPORT_TYPE_DURATION.Equals(reportType))
		{
		  return convertDurationReportResult(reports);
		}

		throw new InvalidRequestException(Status.BAD_REQUEST, "Unkown report type " + reportType);
	  }

	  protected internal static string convertDurationReportResult(IList<ReportResult> reports)
	  {
		StringBuilder buffer = new StringBuilder();

		buffer.Append(DURATION_HEADER);

		foreach (ReportResult report in reports)
		{
		  DurationReportResult durationReport = (DurationReportResult) report;
		  buffer.Append(NEW_LINE_SEPARATOR);
		  buffer.Append(durationReport.Period);
		  buffer.Append(DELIMITER);
		  buffer.Append(durationReport.PeriodUnit.ToString());
		  buffer.Append(DELIMITER);
		  buffer.Append(durationReport.Minimum);
		  buffer.Append(DELIMITER);
		  buffer.Append(durationReport.Maximum);
		  buffer.Append(DELIMITER);
		  buffer.Append(durationReport.Average);
		}

		return buffer.ToString();
	  }

	}

}