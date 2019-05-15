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
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.DATE_FORMAT_WITH_TIMEZONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricActivityStatisticsRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_URL = TEST_RESOURCE_ROOT_PATH + "/history";
	  protected internal static readonly string HISTORIC_ACTIVITY_STATISTICS_URL = HISTORY_URL + "/process-definition/{id}/statistics";

	  private HistoricActivityStatisticsQuery historicActivityStatisticsQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupHistoricActivityStatisticsMock();
	  }

	  private void setupHistoricActivityStatisticsMock()
	  {
		IList<HistoricActivityStatistics> mocks = MockProvider.createMockHistoricActivityStatistics();

		historicActivityStatisticsQuery = mock(typeof(HistoricActivityStatisticsQuery));
		when(processEngine.HistoryService.createHistoricActivityStatisticsQuery(eq(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID))).thenReturn(historicActivityStatisticsQuery);
		when(historicActivityStatisticsQuery.list()).thenReturn(mocks);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricActivityStatisticsRetrieval()
	  public virtual void testHistoricActivityStatisticsRetrieval()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", @is(2)).body("id", hasItems(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID)).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCanceledOption()
	  public virtual void testAdditionalCanceledOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("canceled", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).includeCanceled();
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalFinishedOption()
	  public virtual void testAdditionalFinishedOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("finished", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).includeFinished();
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCompleteScopeOption()
	  public virtual void testAdditionalCompleteScopeOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("completeScope", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).includeCompleteScope();
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalStartedAfterOption()
	  public virtual void testAdditionalStartedAfterOption()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date testDate = new java.util.Date(0);
		DateTime testDate = new DateTime();
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("startedAfter", DATE_FORMAT_WITH_TIMEZONE.format(testDate)).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).startedAfter(testDate);
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalStartedBeforeOption()
	  public virtual void testAdditionalStartedBeforeOption()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date testDate = new java.util.Date(0);
		DateTime testDate = new DateTime();
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("startedBefore", DATE_FORMAT_WITH_TIMEZONE.format(testDate)).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).startedBefore(testDate);
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalFinishedAfterOption()
	  public virtual void testAdditionalFinishedAfterOption()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date testDate = new java.util.Date(0);
		DateTime testDate = new DateTime();
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("finishedAfter", DATE_FORMAT_WITH_TIMEZONE.format(testDate)).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).finishedAfter(testDate);
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalFinishedBeforeOption()
	  public virtual void testAdditionalFinishedBeforeOption()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date testDate = new java.util.Date(0);
		DateTime testDate = new DateTime();
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("finishedBefore", DATE_FORMAT_WITH_TIMEZONE.format(testDate)).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).finishedBefore(testDate);
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCompleteScopeAndCanceledOption()
	  public virtual void testAdditionalCompleteScopeAndCanceledOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("completeScope", "true").queryParam("canceled", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		verify(historicActivityStatisticsQuery).includeCompleteScope();
		verify(historicActivityStatisticsQuery).includeCanceled();
		verify(historicActivityStatisticsQuery).list();
		verifyNoMoreInteractions(historicActivityStatisticsQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCompleteScopeAndFinishedOption()
	  public virtual void testAdditionalCompleteScopeAndFinishedOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("completeScope", "true").queryParam("finished", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		verify(historicActivityStatisticsQuery).includeCompleteScope();
		verify(historicActivityStatisticsQuery).includeFinished();
		verify(historicActivityStatisticsQuery).list();
		verifyNoMoreInteractions(historicActivityStatisticsQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCanceledAndFinishedOption()
	  public virtual void testAdditionalCanceledAndFinishedOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("canceled", "true").queryParam("finished", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		verify(historicActivityStatisticsQuery).includeCanceled();
		verify(historicActivityStatisticsQuery).includeFinished();
		verify(historicActivityStatisticsQuery).list();
		verifyNoMoreInteractions(historicActivityStatisticsQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCompleteScopeAndFinishedAndCanceledOption()
	  public virtual void testAdditionalCompleteScopeAndFinishedAndCanceledOption()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("completeScope", "true").queryParam("finished", "true").queryParam("canceled", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		verify(historicActivityStatisticsQuery).includeCompleteScope();
		verify(historicActivityStatisticsQuery).includeFinished();
		verify(historicActivityStatisticsQuery).includeCanceled();
		verify(historicActivityStatisticsQuery).list();
		verifyNoMoreInteractions(historicActivityStatisticsQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalCompleteScopeAndFinishedAndCanceledOptionFalse()
	  public virtual void testAdditionalCompleteScopeAndFinishedAndCanceledOptionFalse()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("completeScope", "false").queryParam("finished", "false").queryParam("canceled", "false").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		verify(historicActivityStatisticsQuery).list();
		verifyNoMoreInteractions(historicActivityStatisticsQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleTaskQuery()
	  public virtual void testSimpleTaskQuery()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		string content = response.asString();
		IList<string> result = from(content).getList("");
		Assert.assertEquals(2, result.Count);

		Assert.assertNotNull(result[0]);
		Assert.assertNotNull(result[1]);

		string id = from(content).getString("[0].id");
		long instances = from(content).getLong("[0].instances");
		long canceled = from(content).getLong("[0].canceled");
		long finished = from(content).getLong("[0].finished");
		long completeScope = from(content).getLong("[0].completeScope");

		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_ID, id);
		Assert.assertEquals(MockProvider.EXAMPLE_INSTANCES_LONG, instances);
		Assert.assertEquals(MockProvider.EXAMPLE_CANCELED_LONG, canceled);
		Assert.assertEquals(MockProvider.EXAMPLE_FINISHED_LONG, finished);
		Assert.assertEquals(MockProvider.EXAMPLE_COMPLETE_SCOPE_LONG, completeScope);

		id = from(content).getString("[1].id");
		instances = from(content).getLong("[1].instances");
		canceled = from(content).getLong("[1].canceled");
		finished = from(content).getLong("[1].finished");
		completeScope = from(content).getLong("[1].completeScope");

		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID, id);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_INSTANCES_LONG, instances);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_CANCELED_LONG, canceled);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_FINISHED_LONG, finished);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_COMPLETE_SCOPE_LONG, completeScope);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("sortBy", "dueDate").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortOrder()
	  public virtual void testInvalidSortOrder()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("sortOrder", "invalid").queryParam("sortBy", "activityId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("sortOrder parameter has invalid value: invalid")).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortByParameterOnly()
	  public virtual void testInvalidSortByParameterOnly()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("sortOrder", "asc").queryParam("sortBy", "invalid").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("sortBy parameter has invalid value: invalid")).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testValidSortingParameters()
	  public virtual void testValidSortingParameters()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("sortOrder", "asc").queryParam("sortBy", "activityId").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).orderByActivityId();
		inOrder.verify(historicActivityStatisticsQuery).asc();
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).queryParam("sortOrder", "desc").queryParam("sortBy", "activityId").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_STATISTICS_URL);

		inOrder = Mockito.inOrder(historicActivityStatisticsQuery);
		inOrder.verify(historicActivityStatisticsQuery).orderByActivityId();
		inOrder.verify(historicActivityStatisticsQuery).desc();
		inOrder.verify(historicActivityStatisticsQuery).list();
		inOrder.verifyNoMoreInteractions();
	  }

	}

}