using System;

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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.DATE_FORMAT_WITH_TIMEZONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class MetricsRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  public static readonly string METRICS_URL = TEST_RESOURCE_ROOT_PATH + MetricsRestService_Fields.PATH;
	  public static readonly string SINGLE_METER_URL = METRICS_URL + "/{name}";
	  public static readonly string SUM_URL = SINGLE_METER_URL + "/sum";

	  protected internal ManagementService managementServiceMock;
	  private MetricsQuery meterQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		managementServiceMock = mock(typeof(ManagementService));

		when(processEngine.ManagementService).thenReturn(managementServiceMock);

		meterQueryMock = MockProvider.createMockMeterQuery();
		when(managementServiceMock.createMetricsQuery()).thenReturn(meterQueryMock);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetInterval()
	  public virtual void testGetInterval()
	  {
		when(meterQueryMock.interval()).thenReturn(MockProvider.createMockMetricIntervalResult());

		given().then().expect().body("[0].name", equalTo("metricName")).body("[0].timestamp", equalTo(withTimezone(new DateTime(15 * 60 * 1000 * 3)))).body("[0].reporter", equalTo("REPORTER")).body("[0].value", equalTo(23)).body("[1].name", equalTo("metricName")).body("[1].timestamp", equalTo(withTimezone(new DateTime(15 * 60 * 1000 * 2)))).body("[1].reporter", equalTo("REPORTER")).body("[1].value", equalTo(22)).body("[2].name", equalTo("metricName")).body("[2].timestamp", equalTo(withTimezone(new DateTime(15 * 60 * 1000 * 1)))).body("[2].reporter", equalTo("REPORTER")).body("[2].value", equalTo(21)).statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalByName()
	  public virtual void testGetIntervalByName()
	  {
		given().queryParam("name", MockProvider.EXAMPLE_METRICS_NAME).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(MockProvider.EXAMPLE_METRICS_NAME);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalByReporter()
	  public virtual void testGetIntervalByReporter()
	  {
		given().queryParam("reporter", MockProvider.EXAMPLE_METRICS_REPORTER).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(MockProvider.EXAMPLE_METRICS_REPORTER);
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalWithOffset()
	  public virtual void testGetIntervalWithOffset()
	  {
		given().queryParam("firstResult", 10).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock).offset(10);
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalWithLimit()
	  public virtual void testGetIntervalWithLimit()
	  {
		given().queryParam("maxResults", 10).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock).limit(10);
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalAggregation()
	  public virtual void testGetIntervalAggregation()
	  {
		given().queryParam("aggregateByReporter", true).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock).aggregateByReporter();
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalWithStartDate()
	  public virtual void testGetIntervalWithStartDate()
	  {

		given().queryParam("startDate", DATE_FORMAT_WITH_TIMEZONE.format(new DateTime())).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock).startDate(new DateTime());
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalWithEndDate()
	  public virtual void testGetIntervalWithEndDate()
	  {

		given().queryParam("endDate", DATE_FORMAT_WITH_TIMEZONE.format(new DateTime(15 * 60 * 1000))).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock).endDate(new DateTime(15 * 60 * 1000));
		verify(meterQueryMock, times(1)).interval();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalWithCustomInterval()
	  public virtual void testGetIntervalWithCustomInterval()
	  {
		given().queryParam("interval", 300).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(null);
		verify(meterQueryMock).reporter(null);
		verify(meterQueryMock, times(1)).interval(300);
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIntervalWithAll()
	  public virtual void testGetIntervalWithAll()
	  {

		given().queryParam("name", MockProvider.EXAMPLE_METRICS_NAME).queryParam("reporter", MockProvider.EXAMPLE_METRICS_REPORTER).queryParam("maxResults", 10).queryParam("firstResult", 10).queryParam("startDate", DATE_FORMAT_WITH_TIMEZONE.format(new DateTime())).queryParam("endDate", DATE_FORMAT_WITH_TIMEZONE.format(new DateTime(15 * 60 * 1000))).queryParam("aggregateByReporter", true).queryParam("interval", 300).then().expect().statusCode(Status.OK.StatusCode).when().get(METRICS_URL);

		verify(meterQueryMock).name(MockProvider.EXAMPLE_METRICS_NAME);
		verify(meterQueryMock).reporter(MockProvider.EXAMPLE_METRICS_REPORTER);
		verify(meterQueryMock).offset(10);
		verify(meterQueryMock).limit(10);
		verify(meterQueryMock).startDate(new DateTime());
		verify(meterQueryMock).endDate(new DateTime(15 * 60 * 1000));
		verify(meterQueryMock).aggregateByReporter();
		verify(meterQueryMock, times(1)).interval(300);
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSum()
	  public virtual void testGetSum()
	  {

		when(meterQueryMock.sum()).thenReturn(10l);

		given().pathParam("name", Metrics.ACTIVTY_INSTANCE_START).then().expect().statusCode(Status.OK.StatusCode).body("result", equalTo(10)).when().get(SUM_URL);

		verify(meterQueryMock).name(Metrics.ACTIVTY_INSTANCE_START);
		verify(meterQueryMock, times(1)).sum();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSumWithTimestamps()
	  public virtual void testGetSumWithTimestamps()
	  {

		when(meterQueryMock.sum()).thenReturn(10l);

		given().pathParam("name", Metrics.ACTIVTY_INSTANCE_START).queryParam("startDate", MockProvider.EXAMPLE_METRICS_START_DATE).queryParam("endDate", MockProvider.EXAMPLE_METRICS_END_DATE).then().expect().statusCode(Status.OK.StatusCode).body("result", equalTo(10)).when().get(SUM_URL);

		verify(meterQueryMock).name(Metrics.ACTIVTY_INSTANCE_START);
		verify(meterQueryMock).startDate(any(typeof(DateTime)));
		verify(meterQueryMock).endDate(any(typeof(DateTime)));
		verify(meterQueryMock, times(1)).sum();
		verifyNoMoreInteractions(meterQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSumWithInvalidTimestamp()
	  public virtual void testGetSumWithInvalidTimestamp()
	  {

		when(meterQueryMock.sum()).thenReturn(10l);

		given().pathParam("name", Metrics.ACTIVTY_INSTANCE_START).queryParam("startDate", "INVALID-TIME-STAMP").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SUM_URL);

	  }


	}

}