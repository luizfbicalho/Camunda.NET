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
namespace org.camunda.bpm.engine.rest.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.query.PeriodUnit.MONTH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.query.PeriodUnit.QUARTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_PERIOD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockHistoricProcessInstanceDurationReportByMonth;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockHistoricProcessInstanceDurationReportByQuarter;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricProcessInstanceReport = org.camunda.bpm.engine.history.HistoricProcessInstanceReport;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ReportResultToCsvConverter = org.camunda.bpm.engine.rest.dto.converter.ReportResultToCsvConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricProcessInstanceRestServiceReportTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_PROCESS_INSTANCE_REPORT_URL = TEST_RESOURCE_ROOT_PATH + "/history/process-instance/report";

	  protected internal HistoricProcessInstanceReport mockedReportQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedReportQuery = setUpMockHistoricProcessInstanceReportQuery();
	  }

	  private HistoricProcessInstanceReport setUpMockHistoricProcessInstanceReportQuery()
	  {
		HistoricProcessInstanceReport mockedReportQuery = mock(typeof(HistoricProcessInstanceReport));

		when(mockedReportQuery.processDefinitionIdIn(anyString())).thenReturn(mockedReportQuery);
		when(mockedReportQuery.processDefinitionKeyIn(anyString())).thenReturn(mockedReportQuery);
		when(mockedReportQuery.startedAfter(any(typeof(DateTime)))).thenReturn(mockedReportQuery);
		when(mockedReportQuery.startedBefore(any(typeof(DateTime)))).thenReturn(mockedReportQuery);

		IList<DurationReportResult> durationReportByMonth = createMockHistoricProcessInstanceDurationReportByMonth();
		when(mockedReportQuery.duration(MONTH)).thenReturn(durationReportByMonth);

		IList<DurationReportResult> durationReportByQuarter = createMockHistoricProcessInstanceDurationReportByQuarter();
		when(mockedReportQuery.duration(QUARTER)).thenReturn(durationReportByQuarter);

		when(mockedReportQuery.duration(null)).thenThrow(new NotValidException("periodUnit is null"));

		when(processEngine.HistoryService.createHistoricProcessInstanceReport()).thenReturn(mockedReportQuery);

		return mockedReportQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyReportByMonth()
	  public virtual void testEmptyReportByMonth()
	  {
		given().queryParam("reportType", "duration").queryParam("periodUnit", "month").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verify(mockedReportQuery).duration(MONTH);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyReportByQuarter()
	  public virtual void testEmptyReportByQuarter()
	  {
		given().queryParam("reportType", "duration").queryParam("periodUnit", "quarter").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verify(mockedReportQuery).duration(QUARTER);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidReportType()
	  public virtual void testInvalidReportType()
	  {
		given().queryParam("reportType", "abc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot set query parameter 'reportType' to value 'abc'")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidPeriodUnit()
	  public virtual void testInvalidPeriodUnit()
	  {
		given().queryParam("periodUnit", "abc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot set query parameter 'periodUnit' to value 'abc'")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingReportType()
	  public virtual void testMissingReportType()
	  {
		given().then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Unknown report type null")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingPeriodUnit()
	  public virtual void testMissingPeriodUnit()
	  {
		given().queryParam("reportType", "duration").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("periodUnit is null")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthorization()
	  public virtual void testMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.duration(MONTH)).thenThrow(new AuthorizationException(message));

		given().queryParam("reportType", "duration").queryParam("periodUnit", "month").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDurationReportByMonth()
	  public virtual void testDurationReportByMonth()
	  {
		Response response = given().queryParam("periodUnit", "month").queryParam("reportType", "duration").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		string content = response.asString();
		IList<string> reports = from(content).getList("");
		Assert.assertEquals("There should be one report returned.", 1, reports.Count);
		Assert.assertNotNull("The returned report should not be null.", reports[0]);

		long returnedAvg = from(content).getLong("[0].average");
		long returnedMax = from(content).getLong("[0].maximum");
		long returnedMin = from(content).getLong("[0].minimum");
		int returnedPeriod = from(content).getInt("[0].period");
		string returnedPeriodUnit = from(content).getString("[0].periodUnit");

		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG, returnedAvg);
		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX, returnedMax);
		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN, returnedMin);
		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_PERIOD, returnedPeriod);
		Assert.assertEquals(MONTH.ToString(), returnedPeriodUnit);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDurationReportByQuarter()
	  public virtual void testDurationReportByQuarter()
	  {
		Response response = given().queryParam("periodUnit", "quarter").queryParam("reportType", "duration").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		string content = response.asString();
		IList<string> reports = from(content).getList("");
		Assert.assertEquals("There should be one report returned.", 1, reports.Count);
		Assert.assertNotNull("The returned report should not be null.", reports[0]);

		long returnedAvg = from(content).getLong("[0].average");
		long returnedMax = from(content).getLong("[0].maximum");
		long returnedMin = from(content).getLong("[0].minimum");
		int returnedPeriod = from(content).getInt("[0].period");
		string returnedPeriodUnit = from(content).getString("[0].periodUnit");

		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG, returnedAvg);
		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX, returnedMax);
		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN, returnedMin);
		Assert.assertEquals(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_PERIOD, returnedPeriod);
		Assert.assertEquals(QUARTER.ToString(), returnedPeriodUnit);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListParameters()
	  public virtual void testListParameters()
	  {
		string aProcDefId = "anProcDefId";
		string anotherProcDefId = "anotherProcDefId";

		string aProcDefKey = "anProcDefKey";
		string anotherProcDefKey = "anotherProcDefKey";

		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("processDefinitionIdIn", aProcDefId + "," + anotherProcDefId).queryParam("processDefinitionKeyIn", aProcDefKey + "," + anotherProcDefKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verify(mockedReportQuery).processDefinitionIdIn(aProcDefId, anotherProcDefId);
		verify(mockedReportQuery).processDefinitionKeyIn(aProcDefKey, anotherProcDefKey);
		verify(mockedReportQuery).duration(MONTH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterStartTimeQuery()
	  public virtual void testHistoricBeforeAndAfterStartTimeQuery()
	  {
		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("startedBefore", EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE).queryParam("startedAfter", EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verifyStringStartParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyCsvReportByMonth()
	  public virtual void testEmptyCsvReportByMonth()
	  {
		given().queryParam("reportType", "duration").queryParam("periodUnit", "month").accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verify(mockedReportQuery).duration(MONTH);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyCsvReportByQuarter()
	  public virtual void testEmptyCsvReportByQuarter()
	  {
		given().queryParam("reportType", "duration").queryParam("periodUnit", "quarter").accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verify(mockedReportQuery).duration(QUARTER);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvInvalidReportType()
	  public virtual void testCsvInvalidReportType()
	  {
		given().queryParam("reportType", "abc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot set query parameter 'reportType' to value 'abc'")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvInvalidPeriodUnit()
	  public virtual void testCsvInvalidPeriodUnit()
	  {
		given().queryParam("periodUnit", "abc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot set query parameter 'periodUnit' to value 'abc'")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvMissingReportType()
	  public virtual void testCsvMissingReportType()
	  {
		given().then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Unknown report type null")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvMissingPeriodUnit()
	  public virtual void testCsvMissingPeriodUnit()
	  {
		given().queryParam("reportType", "duration").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("periodUnit is null")).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvMissingAuthorization()
	  public virtual void testCsvMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.duration(MONTH)).thenThrow(new AuthorizationException(message));

		given().queryParam("reportType", "duration").queryParam("periodUnit", "month").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvDurationReportByMonth()
	  public virtual void testCsvDurationReportByMonth()
	  {
		Response response = given().queryParam("reportType", "duration").queryParam("periodUnit", "month").accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains(ReportResultToCsvConverter.DURATION_HEADER));
		assertTrue(responseContent.Contains(MONTH.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX.ToString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvDurationReportByQuarter()
	  public virtual void testCsvDurationReportByQuarter()
	  {
		Response response = given().queryParam("reportType", "duration").queryParam("periodUnit", "quarter").accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains(ReportResultToCsvConverter.DURATION_HEADER));
		assertTrue(responseContent.Contains(QUARTER.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX.ToString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testApplicationCsvDurationReportByMonth()
	  public virtual void testApplicationCsvDurationReportByMonth()
	  {
		Response response = given().queryParam("reportType", "duration").queryParam("periodUnit", "month").accept("application/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("application/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains(ReportResultToCsvConverter.DURATION_HEADER));
		assertTrue(responseContent.Contains(MONTH.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX.ToString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testApplicationCsvDurationReportByQuarter()
	  public virtual void testApplicationCsvDurationReportByQuarter()
	  {
		Response response = given().queryParam("reportType", "duration").queryParam("periodUnit", "quarter").accept("application/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("application/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		string responseContent = response.asString();
		assertTrue(responseContent.Contains(ReportResultToCsvConverter.DURATION_HEADER));
		assertTrue(responseContent.Contains(QUARTER.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN.ToString()));
		assertTrue(responseContent.Contains(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX.ToString()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvListParameters()
	  public virtual void testCsvListParameters()
	  {
		string aProcDefId = "anProcDefId";
		string anotherProcDefId = "anotherProcDefId";

		string aProcDefKey = "anProcDefKey";
		string anotherProcDefKey = "anotherProcDefKey";

		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("processDefinitionIdIn", aProcDefId + "," + anotherProcDefId).queryParam("processDefinitionKeyIn", aProcDefKey + "," + anotherProcDefKey).accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verify(mockedReportQuery).processDefinitionIdIn(aProcDefId, anotherProcDefId);
		verify(mockedReportQuery).processDefinitionKeyIn(aProcDefKey, anotherProcDefKey);
		verify(mockedReportQuery).duration(MONTH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCsvHistoricBeforeAndAfterStartTimeQuery()
	  public virtual void testCsvHistoricBeforeAndAfterStartTimeQuery()
	  {
		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("startedBefore", EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE).queryParam("startedAfter", EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER).accept("text/csv").then().expect().statusCode(Status.OK.StatusCode).contentType("text/csv").header("Content-Disposition", "attachment; filename=process-instance-report.csv").when().get(HISTORIC_PROCESS_INSTANCE_REPORT_URL);

		verifyStringStartParameterQueryInvocations();
	  }

	  private IDictionary<string, string> CompleteStartDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["startedAfter"] = EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER;
			parameters["startedBefore"] = EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE;
    
			return parameters;
		  }
	  }

	  private void verifyStringStartParameterQueryInvocations()
	  {
		IDictionary<string, string> startDateParameters = CompleteStartDateAsStringQueryParameters;

		verify(mockedReportQuery).startedBefore(DateTimeUtil.parseDate(startDateParameters["startedBefore"]));
		verify(mockedReportQuery).startedAfter(DateTimeUtil.parseDate(startDateParameters["startedAfter"]));
	  }

	}

}