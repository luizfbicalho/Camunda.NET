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
	using ReportResult = org.camunda.bpm.engine.history.ReportResult;

	using JsonSubTypes = com.fasterxml.jackson.annotation.JsonSubTypes;
	using Type = com.fasterxml.jackson.annotation.JsonSubTypes.Type;
	using JsonTypeInfo = com.fasterxml.jackson.annotation.JsonTypeInfo;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include=JsonTypeInfo.As.PROPERTY, property="type") @JsonSubTypes({ @Type(value = DurationReportResultDto.class) }) public abstract class ReportResultDto
	public abstract class ReportResultDto
	{

	  protected internal int period;
	  protected internal string periodUnit;

	  public virtual int Period
	  {
		  get
		  {
			return period;
		  }
	  }

	  public virtual string PeriodUnit
	  {
		  get
		  {
			return periodUnit;
		  }
	  }

	  public static ReportResultDto fromReportResult(ReportResult reportResult)
	  {

		ReportResultDto dto = null;

		if (reportResult is DurationReportResult)
		{
		  DurationReportResult durationReport = (DurationReportResult) reportResult;
		  dto = DurationReportResultDto.fromDurationReportResult(durationReport);
		}

		dto.period = reportResult.Period;
		dto.periodUnit = reportResult.PeriodUnit.ToString();

		return dto;
	  }

	}

}