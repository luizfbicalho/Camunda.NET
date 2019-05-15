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
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;

	using JsonTypeName = com.fasterxml.jackson.annotation.JsonTypeName;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeName("durationReportResult") public class DurationReportResultDto extends ReportResultDto
	public class DurationReportResultDto : ReportResultDto
	{

	  protected internal long minimum;
	  protected internal long maximum;
	  protected internal long average;

	  public virtual long Minimum
	  {
		  get
		  {
			return minimum;
		  }
	  }

	  public virtual long Maximum
	  {
		  get
		  {
			return maximum;
		  }
	  }

	  public virtual long Average
	  {
		  get
		  {
			return average;
		  }
	  }

	  public static DurationReportResultDto fromDurationReportResult(DurationReportResult durationReport)
	  {
		DurationReportResultDto dto = new DurationReportResultDto();

		dto.minimum = durationReport.Minimum;
		dto.maximum = durationReport.Maximum;
		dto.average = durationReport.Average;

		return dto;
	  }

	}

}