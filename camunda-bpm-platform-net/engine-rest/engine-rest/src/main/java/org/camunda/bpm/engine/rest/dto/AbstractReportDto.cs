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
namespace org.camunda.bpm.engine.rest.dto
{

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ReportResult = org.camunda.bpm.engine.history.ReportResult;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using Report = org.camunda.bpm.engine.query.Report;
	using PeriodUnitConverter = org.camunda.bpm.engine.rest.dto.converter.PeriodUnitConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov </summary>
	/// @param <T> 
	///  </param>
	public abstract class AbstractReportDto<T> : AbstractSearchQueryDto where T : org.camunda.bpm.engine.query.Report
	{

	  protected internal PeriodUnit periodUnit;
	  protected internal string reportType;

	  public const string REPORT_TYPE_DURATION = "duration";
	  public const string REPORT_TYPE_COUNT = "count";

	  public static readonly IList<string> VALID_REPORT_TYPE_VALUES;
	  static AbstractReportDto()
	  {
		VALID_REPORT_TYPE_VALUES = new List<string>();
		VALID_REPORT_TYPE_VALUES.Add(REPORT_TYPE_DURATION);
		VALID_REPORT_TYPE_VALUES.Add(REPORT_TYPE_COUNT);
	  }

	  // required for populating via jackson
	  public AbstractReportDto()
	  {
	  }

	  public AbstractReportDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  protected internal virtual PeriodUnit PeriodUnit
	  {
		  get
		  {
			return periodUnit;
		  }
		  set
		  {
			this.periodUnit = value;
		  }
	  }

	  public virtual string ReportType
	  {
		  get
		  {
			return reportType;
		  }
		  set
		  {
			if (!VALID_REPORT_TYPE_VALUES.Contains(value))
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "reportType parameter has invalid value: " + value);
			}
			this.reportType = value;
		  }
	  }



//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.camunda.bpm.engine.history.ReportResult> executeReportQuery(T report)
	  protected internal virtual IList<ReportResult> executeReportQuery(T report)
	  {
		return report.duration(periodUnit);
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<? extends org.camunda.bpm.engine.history.ReportResult> executeReport(org.camunda.bpm.engine.ProcessEngine engine)
	  public virtual IList<ReportResult> executeReport(ProcessEngine engine)
	  {
		T reportQuery = createNewReportQuery(engine);
		applyFilters(reportQuery);

		try
		{
		  return executeReportQuery(reportQuery);
		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, e.Message);
		}
	  }

	  protected internal abstract T createNewReportQuery(ProcessEngine engine);

	  protected internal abstract void applyFilters(T reportQuery);

	}

}