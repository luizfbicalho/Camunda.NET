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
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using CleanableHistoricBatchReport = org.camunda.bpm.engine.history.CleanableHistoricBatchReport;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class CleanableHistoricBatchReportServiceTest : AbstractRestServiceTest
	{

	  private const string EXAMPLE_TYPE = "batchId1";
	  private const int EXAMPLE_TTL = 5;
	  private const long EXAMPLE_FINISHED_COUNT = 10l;
	  private const long EXAMPLE_CLEANABLE_COUNT = 5l;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_URL = TEST_RESOURCE_ROOT_PATH + "/history/batch";
	  protected internal static readonly string HISTORIC_REPORT_URL = HISTORY_URL + "/cleanable-batch-report";
	  protected internal static readonly string HISTORIC_REPORT_COUNT_URL = HISTORIC_REPORT_URL + "/count";

	  private CleanableHistoricBatchReport historicBatchReport;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupHistoryReportMock();
	  }

	  private void setupHistoryReportMock()
	  {
		CleanableHistoricBatchReport report = mock(typeof(CleanableHistoricBatchReport));

		CleanableHistoricBatchReportResult reportResult = mock(typeof(CleanableHistoricBatchReportResult));

		when(reportResult.BatchType).thenReturn(EXAMPLE_TYPE);
		when(reportResult.HistoryTimeToLive).thenReturn(EXAMPLE_TTL);
		when(reportResult.FinishedBatchesCount).thenReturn(EXAMPLE_FINISHED_COUNT);
		when(reportResult.CleanableBatchesCount).thenReturn(EXAMPLE_CLEANABLE_COUNT);

		CleanableHistoricBatchReportResult anotherReportResult = mock(typeof(CleanableHistoricBatchReportResult));

		when(anotherReportResult.BatchType).thenReturn("batchId2");
		when(anotherReportResult.HistoryTimeToLive).thenReturn(null);
		when(anotherReportResult.FinishedBatchesCount).thenReturn(13l);
		when(anotherReportResult.CleanableBatchesCount).thenReturn(0l);

		IList<CleanableHistoricBatchReportResult> mocks = new List<CleanableHistoricBatchReportResult>();
		mocks.Add(reportResult);
		mocks.Add(anotherReportResult);

		when(report.list()).thenReturn(mocks);
		when(report.count()).thenReturn((long) mocks.Count);

		historicBatchReport = report;
		when(processEngine.HistoryService.createCleanableHistoricBatchReport()).thenReturn(historicBatchReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetReport()
	  public virtual void testGetReport()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		InOrder inOrder = Mockito.inOrder(historicBatchReport);
		inOrder.verify(historicBatchReport).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportRetrieval()
	  public virtual void testReportRetrieval()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(historicBatchReport);
		inOrder.verify(historicBatchReport).list();

		string content = response.asString();
		IList<string> reportResults = from(content).getList("");
		Assert.assertEquals("There should be two report results returned.", 2, reportResults.Count);
		Assert.assertNotNull(reportResults[0]);

		string returnedBatchType = from(content).getString("[0].batchType");
		int returnedTTL = from(content).getInt("[0].historyTimeToLive");
		long returnedFinishedCount = from(content).getLong("[0].finishedBatchesCount");
		long returnedCleanableCount = from(content).getLong("[0].cleanableBatchesCount");

		Assert.assertEquals(EXAMPLE_TYPE, returnedBatchType);
		Assert.assertEquals(EXAMPLE_TTL, returnedTTL);
		Assert.assertEquals(EXAMPLE_FINISHED_COUNT, returnedFinishedCount);
		Assert.assertEquals(EXAMPLE_CLEANABLE_COUNT, returnedCleanableCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthorization()
	  public virtual void testMissingAuthorization()
	  {
		string message = "not authorized";
		when(historicBatchReport.list()).thenThrow(new AuthorizationException(message));

		given().then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(HISTORIC_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verify(historicBatchReport).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedBatchOperationAsc()
	  public virtual void testOrderByFinishedBatchOperationAsc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "asc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicBatchReport).orderByFinishedBatchOperation();
		verify(historicBatchReport).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedBatchOperationDesc()
	  public virtual void testOrderByFinishedBatchOperationDesc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "desc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicBatchReport).orderByFinishedBatchOperation();
		verify(historicBatchReport).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("finished", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_REPORT_URL);
	  }
	}

}