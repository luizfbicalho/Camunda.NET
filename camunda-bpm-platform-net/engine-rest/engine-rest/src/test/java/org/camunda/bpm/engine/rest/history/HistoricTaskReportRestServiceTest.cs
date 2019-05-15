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
	using ContentType = io.restassured.http.ContentType;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricTaskInstanceReport = org.camunda.bpm.engine.history.HistoricTaskInstanceReport;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.query.PeriodUnit.MONTH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.query.PeriodUnit.QUARTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_END_TIME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_AVG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MAX;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MIN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_PERIOD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_INST_END_TIME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_INST_START_TIME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_REPORT_COUNT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_REPORT_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_REPORT_TASK_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_HISTORIC_TASK_START_TIME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TENANT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockHistoricTaskInstanceDurationReport;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockHistoricTaskInstanceReport;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockHistoricTaskInstanceReportWithProcDef;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class HistoricTaskReportRestServiceTest : AbstractRestServiceTest
	{


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string TASK_REPORT_URL = TEST_RESOURCE_ROOT_PATH + "/history/task/report";

	  protected internal HistoricTaskInstanceReport mockedReportQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedReportQuery = setUpMockReportQuery();
	  }

	  private HistoricTaskInstanceReport setUpMockReportQuery()
	  {
		HistoricTaskInstanceReport mockedReportQuery = mock(typeof(HistoricTaskInstanceReport));

		IList<HistoricTaskInstanceReportResult> taskReportResults = createMockHistoricTaskInstanceReport();
		IList<HistoricTaskInstanceReportResult> taskReportResultsWithProcDef = createMockHistoricTaskInstanceReportWithProcDef();

		when(mockedReportQuery.completedAfter(any(typeof(DateTime)))).thenReturn(mockedReportQuery);
		when(mockedReportQuery.completedBefore(any(typeof(DateTime)))).thenReturn(mockedReportQuery);

		when(mockedReportQuery.countByTaskName()).thenReturn(taskReportResults);
		when(mockedReportQuery.countByProcessDefinitionKey()).thenReturn(taskReportResultsWithProcDef);

		IList<DurationReportResult> durationReportByMonth = createMockHistoricTaskInstanceDurationReport(MONTH);
		when(mockedReportQuery.duration(MONTH)).thenReturn(durationReportByMonth);

		IList<DurationReportResult> durationReportByQuarter = createMockHistoricTaskInstanceDurationReport(QUARTER);
		when(mockedReportQuery.duration(QUARTER)).thenReturn(durationReportByQuarter);

		when(processEngine.HistoryService.createHistoricTaskInstanceReport()).thenReturn(mockedReportQuery);

		return mockedReportQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountMissingAuthorization()
	  public virtual void testTaskCountMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.countByTaskName()).thenThrow(new AuthorizationException(message));

		given().queryParam("reportType", "count").queryParam("groupBy", "taskName").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(TASK_REPORT_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountByProcDefMissingAuthorization()
	  public virtual void testTaskCountByProcDefMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.countByProcessDefinitionKey()).thenThrow(new AuthorizationException(message));

		given().queryParam("reportType", "count").queryParam("groupBy", "processDefinition").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountReport()
	  public virtual void testTaskCountReport()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "taskName").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("[0].count", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_COUNT.intValue())).body("[0].processDefinitionId", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID)).body("[0].processDefinitionName", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME)).body("[0].processDefinitionKey", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION)).body("[0].taskName", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_TASK_NAME)).body("[0].tenantId", equalTo(EXAMPLE_TENANT_ID)).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).countByTaskName();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountReportWithCompletedBefore()
	  public virtual void testTaskCountReportWithCompletedBefore()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "taskName").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_END_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedBefore(any(typeof(DateTime)));
		verify(mockedReportQuery).countByTaskName();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountReportWithCompletedAfter()
	  public virtual void testTaskCountReportWithCompletedAfter()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "taskName").queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_START_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedAfter(any(typeof(DateTime)));
		verify(mockedReportQuery).countByTaskName();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountByProcDefReportWithCompletedBefore()
	  public virtual void testTaskCountByProcDefReportWithCompletedBefore()
	  {
		given().queryParam("reportType", "count").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_END_TIME).queryParam("groupBy", "processDefinition").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedBefore(any(typeof(DateTime)));
		verify(mockedReportQuery).countByProcessDefinitionKey();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountByProcDefReportWithCompletedAfter()
	  public virtual void testTaskCountByProcDefReportWithCompletedAfter()
	  {
		given().queryParam("reportType", "count").queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_START_TIME).queryParam("groupBy", "processDefinition").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedAfter(any(typeof(DateTime)));
		verify(mockedReportQuery).countByProcessDefinitionKey();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountReportWithGroupByProcDef()
	  public virtual void testTaskCountReportWithGroupByProcDef()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "processDefinition").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).countByProcessDefinitionKey();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountReportWithGroupByTaskDef()
	  public virtual void testTaskCountReportWithGroupByTaskDef()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "taskName").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).countByTaskName();
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountReportWithGroupByAnyDef()
	  public virtual void testTaskCountReportWithGroupByAnyDef()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "anotherDefinition").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("groupBy parameter has invalid value: anotherDefinition")).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountWithAllParameters()
	  public virtual void testTaskCountWithAllParameters()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "processDefinition").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_INST_END_TIME).queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_INST_START_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("[0].count", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_COUNT.intValue())).body("[0].processDefinitionId", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID)).body("[0].processDefinitionName", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME)).body("[0].processDefinitionKey", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION)).body("[0].taskName", equalTo(null)).body("[0].tenantId", equalTo(EXAMPLE_TENANT_ID)).when().get(TASK_REPORT_URL);

		verifyStringStartParameterQueryInvocations();
		verify(mockedReportQuery).countByProcessDefinitionKey();
		verifyNoMoreInteractions(mockedReportQuery);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCountWithAllParametersGroupByTask()
	  public virtual void testTaskCountWithAllParametersGroupByTask()
	  {
		given().queryParam("reportType", "count").queryParam("groupBy", "taskName").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_INST_END_TIME).queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_INST_START_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("[0].count", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_COUNT.intValue())).body("[0].processDefinitionId", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID)).body("[0].processDefinitionName", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME)).body("[0].processDefinitionKey", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION)).body("[0].taskName", equalTo(EXAMPLE_HISTORIC_TASK_REPORT_TASK_NAME)).body("[0].tenantId", equalTo(EXAMPLE_TENANT_ID)).when().get(TASK_REPORT_URL);

		verifyStringStartParameterQueryInvocations();
		verify(mockedReportQuery).countByTaskName();
		verifyNoMoreInteractions(mockedReportQuery);

	  }

	  // TASK DURATION REPORT ///////////////////////////////////////////////////////
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationMonthMissingAuthorization()
	  public virtual void testTaskDurationMonthMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.duration(MONTH)).thenThrow(new AuthorizationException(message));

		given().queryParam("reportType", "duration").queryParam("periodUnit", "month").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationQuarterMissingAuthorization()
	  public virtual void testTaskDurationQuarterMissingAuthorization()
	  {
		string message = "not authorized";
		when(mockedReportQuery.duration(QUARTER)).thenThrow(new AuthorizationException(message));

		given().queryParam("reportType", "duration").queryParam("periodUnit", "quarter").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWrongReportType()
	   public virtual void testWrongReportType()
	   {
		given().queryParam("reportType", "abc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot set query parameter 'reportType' to value 'abc'")).when().get(TASK_REPORT_URL);
	   }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportWithoutDurationParam()
	  public virtual void testTaskDurationReportWithoutDurationParam()
	  {
		given().queryParam("periodUnit", "month").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Parameter reportType is not set.")).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationQuarterReportWithoutDurationParam()
	  public virtual void testTaskDurationQuarterReportWithoutDurationParam()
	  {
		given().queryParam("periodUnit", "quarter").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Parameter reportType is not set.")).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportWithInvalidPeriodUnit()
	  public virtual void testTaskDurationReportWithInvalidPeriodUnit()
	  {
		given().queryParam("periodUnit", "abc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Cannot set query parameter 'periodUnit' to value 'abc'")).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportWithMissingPeriodUnit()
	  public virtual void testTaskDurationReportWithMissingPeriodUnit()
	  {
		given().queryParam("reportType", "duration").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("periodUnit is null")).when().get(TASK_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportByMonth()
	  public virtual void testTaskDurationReportByMonth()
	  {
		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("[0].average", equalTo((int) EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_AVG)).body("[0].maximum", equalTo((int) EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MAX)).body("[0].minimum", equalTo((int) EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MIN)).body("[0].period", equalTo(EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_PERIOD)).body("[0].periodUnit", equalTo(MONTH.ToString())).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).duration(PeriodUnit.MONTH);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportByQuarter()
	  public virtual void testTaskDurationReportByQuarter()
	  {
		given().queryParam("periodUnit", "quarter").queryParam("reportType", "duration").then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("[0].average", equalTo((int) EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_AVG)).body("[0].maximum", equalTo((int) EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MAX)).body("[0].minimum", equalTo((int) EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MIN)).body("[0].period", equalTo(EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_PERIOD)).body("[0].periodUnit", equalTo(QUARTER.ToString())).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).duration(PeriodUnit.QUARTER);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportWithCompletedBeforeAndCompletedAfter()
	  public virtual void testTaskDurationReportWithCompletedBeforeAndCompletedAfter()
	  {
		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_INST_START_TIME).queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_INST_END_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verifyStringStartParameterQueryInvocations();
		verify(mockedReportQuery).duration(PeriodUnit.MONTH);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportWithCompletedBefore()
	  public virtual void testTaskDurationReportWithCompletedBefore()
	  {
		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_END_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedBefore(any(typeof(DateTime)));
		verify(mockedReportQuery).duration(PeriodUnit.MONTH);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationReportWithCompletedAfter()
	  public virtual void testTaskDurationReportWithCompletedAfter()
	  {
		given().queryParam("periodUnit", "month").queryParam("reportType", "duration").queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_START_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedAfter(any(typeof(DateTime)));
		verify(mockedReportQuery).duration(PeriodUnit.MONTH);
		verifyNoMoreInteractions(mockedReportQuery);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationQuarterReportWithCompletedBeforeAndCompletedAfter()
	  public virtual void testTaskDurationQuarterReportWithCompletedBeforeAndCompletedAfter()
	  {
		given().queryParam("periodUnit", "quarter").queryParam("reportType", "duration").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_INST_START_TIME).queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_INST_END_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verifyStringStartParameterQueryInvocations();
		verify(mockedReportQuery).duration(PeriodUnit.QUARTER);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationQuarterReportWithCompletedBefore()
	  public virtual void testTaskDurationQuarterReportWithCompletedBefore()
	  {
		given().queryParam("periodUnit", "quarter").queryParam("reportType", "duration").queryParam("completedBefore", EXAMPLE_HISTORIC_TASK_END_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedBefore(any(typeof(DateTime)));
		verify(mockedReportQuery).duration(PeriodUnit.QUARTER);
		verifyNoMoreInteractions(mockedReportQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDurationQuarterReportWithCompletedAfter()
	  public virtual void testTaskDurationQuarterReportWithCompletedAfter()
	  {
		given().queryParam("periodUnit", "quarter").queryParam("reportType", "duration").queryParam("completedAfter", EXAMPLE_HISTORIC_TASK_START_TIME).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(TASK_REPORT_URL);

		verify(mockedReportQuery).completedAfter(any(typeof(DateTime)));
		verify(mockedReportQuery).duration(PeriodUnit.QUARTER);
		verifyNoMoreInteractions(mockedReportQuery);
	  }


	  private IDictionary<string, string> CompleteStartDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["completedBefore"] = EXAMPLE_HISTORIC_TASK_INST_START_TIME;
			parameters["completedAfter"] = EXAMPLE_HISTORIC_TASK_INST_END_TIME;
    
			return parameters;
		  }
	  }

	  private void verifyStringStartParameterQueryInvocations()
	  {
		IDictionary<string, string> startDateParameters = CompleteStartDateAsStringQueryParameters;

		verify(mockedReportQuery).completedBefore(DateTimeUtil.parseDate(startDateParameters["completedBefore"]));
		verify(mockedReportQuery).completedAfter(DateTimeUtil.parseDate(startDateParameters["completedAfter"]));
	  }
	}

}