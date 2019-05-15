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
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
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


	using CleanableHistoricDecisionInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReport;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
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

	public class CleanableHistoricDecisionInstanceReportServiceTest : AbstractRestServiceTest
	{

	  private const string EXAMPLE_DD_ID = "anId";
	  private const string EXAMPLE_DD_KEY = "aKey";
	  private const string EXAMPLE_DD_NAME = "aName";
	  private const int EXAMPLE_DD_VERSION = 42;
	  private const int EXAMPLE_TTL = 5;
	  private const long EXAMPLE_FINISHED_DI_COUNT = 1000l;
	  private const long EXAMPLE_CLEANABLE_DI_COUNT = 567l;
	  private const string EXAMPLE_TENANT_ID = "aTenantId";

	  protected internal const string ANOTHER_EXAMPLE_DD_ID = "anotherDefId";
	  protected internal const string ANOTHER_EXAMPLE_DD_KEY = "anotherDefKey";
	  protected internal const string ANOTHER_EXAMPLE_TENANT_ID = "anotherTenantId";


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_URL = TEST_RESOURCE_ROOT_PATH + "/history/decision-definition";
	  protected internal static readonly string HISTORIC_REPORT_URL = HISTORY_URL + "/cleanable-decision-instance-report";
	  protected internal static readonly string HISTORIC_REPORT_COUNT_URL = HISTORIC_REPORT_URL + "/count";

	  private CleanableHistoricDecisionInstanceReport historicDecisionInstanceReport;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupHistoryReportMock();
	  }

	  private void setupHistoryReportMock()
	  {
		CleanableHistoricDecisionInstanceReport report = mock(typeof(CleanableHistoricDecisionInstanceReport));

		when(report.decisionDefinitionIdIn(anyString())).thenReturn(report);
		when(report.decisionDefinitionKeyIn(anyString())).thenReturn(report);

		CleanableHistoricDecisionInstanceReportResult reportResult = mock(typeof(CleanableHistoricDecisionInstanceReportResult));

		when(reportResult.DecisionDefinitionId).thenReturn(EXAMPLE_DD_ID);
		when(reportResult.DecisionDefinitionKey).thenReturn(EXAMPLE_DD_KEY);
		when(reportResult.DecisionDefinitionName).thenReturn(EXAMPLE_DD_NAME);
		when(reportResult.DecisionDefinitionVersion).thenReturn(EXAMPLE_DD_VERSION);
		when(reportResult.HistoryTimeToLive).thenReturn(EXAMPLE_TTL);
		when(reportResult.FinishedDecisionInstanceCount).thenReturn(EXAMPLE_FINISHED_DI_COUNT);
		when(reportResult.CleanableDecisionInstanceCount).thenReturn(EXAMPLE_CLEANABLE_DI_COUNT);
		when(reportResult.TenantId).thenReturn(EXAMPLE_TENANT_ID);

		CleanableHistoricDecisionInstanceReportResult anotherReportResult = mock(typeof(CleanableHistoricDecisionInstanceReportResult));

		when(anotherReportResult.DecisionDefinitionId).thenReturn(ANOTHER_EXAMPLE_DD_ID);
		when(anotherReportResult.DecisionDefinitionKey).thenReturn(ANOTHER_EXAMPLE_DD_KEY);
		when(anotherReportResult.DecisionDefinitionName).thenReturn("dpName");
		when(anotherReportResult.DecisionDefinitionVersion).thenReturn(33);
		when(anotherReportResult.HistoryTimeToLive).thenReturn(5);
		when(anotherReportResult.FinishedDecisionInstanceCount).thenReturn(10l);
		when(anotherReportResult.CleanableDecisionInstanceCount).thenReturn(0l);
		when(anotherReportResult.TenantId).thenReturn(ANOTHER_EXAMPLE_TENANT_ID);


		IList<CleanableHistoricDecisionInstanceReportResult> mocks = new List<CleanableHistoricDecisionInstanceReportResult>();
		mocks.Add(reportResult);
		mocks.Add(anotherReportResult);

		when(report.list()).thenReturn(mocks);
		when(report.count()).thenReturn((long) mocks.Count);

		historicDecisionInstanceReport = report;
		when(processEngine.HistoryService.createCleanableHistoricDecisionInstanceReport()).thenReturn(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetReport()
	  public virtual void testGetReport()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		InOrder inOrder = Mockito.inOrder(historicDecisionInstanceReport);
		inOrder.verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportRetrieval()
	  public virtual void testReportRetrieval()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(historicDecisionInstanceReport);
		inOrder.verify(historicDecisionInstanceReport).list();

		string content = response.asString();
		IList<string> reportResults = from(content).getList("");
		Assert.assertEquals("There should be two report results returned.", 2, reportResults.Count);
		Assert.assertNotNull(reportResults[0]);

		string returnedDefinitionId = from(content).getString("[0].decisionDefinitionId");
		string returnedDefinitionKey = from(content).getString("[0].decisionDefinitionKey");
		string returnedDefinitionName = from(content).getString("[0].decisionDefinitionName");
		int returnedDefinitionVersion = from(content).getInt("[0].decisionDefinitionVersion");
		int returnedTTL = from(content).getInt("[0].historyTimeToLive");
		long returnedFinishedCount = from(content).getLong("[0].finishedDecisionInstanceCount");
		long returnedCleanableCount = from(content).getLong("[0].cleanableDecisionInstanceCount");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(EXAMPLE_DD_ID, returnedDefinitionId);
		Assert.assertEquals(EXAMPLE_DD_KEY, returnedDefinitionKey);
		Assert.assertEquals(EXAMPLE_DD_NAME, returnedDefinitionName);
		Assert.assertEquals(EXAMPLE_DD_VERSION, returnedDefinitionVersion);
		Assert.assertEquals(EXAMPLE_TTL, returnedTTL);
		Assert.assertEquals(EXAMPLE_FINISHED_DI_COUNT, returnedFinishedCount);
		Assert.assertEquals(EXAMPLE_CLEANABLE_DI_COUNT, returnedCleanableCount);
		Assert.assertEquals(EXAMPLE_TENANT_ID, returnedTenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthorization()
	  public virtual void testMissingAuthorization()
	  {
		string message = "not authorized";
		when(historicDecisionInstanceReport.list()).thenThrow(new AuthorizationException(message));

		given().then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(HISTORIC_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDefinitionId()
	  public virtual void testQueryByDefinitionId()
	  {
		given().queryParam("decisionDefinitionIdIn", EXAMPLE_DD_ID + "," + ANOTHER_EXAMPLE_DD_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).decisionDefinitionIdIn(EXAMPLE_DD_ID, ANOTHER_EXAMPLE_DD_ID);
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDefinitionKey()
	  public virtual void testQueryByDefinitionKey()
	  {
		given().queryParam("decisionDefinitionKeyIn", EXAMPLE_DD_KEY + "," + ANOTHER_EXAMPLE_DD_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).decisionDefinitionKeyIn(EXAMPLE_DD_KEY, ANOTHER_EXAMPLE_DD_KEY);
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	  public virtual void testQueryByTenantId()
	  {
		given().queryParam("tenantIdIn", EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).tenantIdIn(EXAMPLE_TENANT_ID, ANOTHER_EXAMPLE_TENANT_ID);
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithoutTenantId()
	  public virtual void testQueryWithoutTenantId()
	  {
		given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).withoutTenantId();
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCompact()
	  public virtual void testQueryCompact()
	  {
		given().queryParam("compact", true).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).compact();
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQuery()
	  public virtual void testFullQuery()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verifyQueryParameterInvocations();
		verify(historicDecisionInstanceReport).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verify(historicDecisionInstanceReport).count();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQueryCount()
	  public virtual void testFullQueryCount()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verifyQueryParameterInvocations();
		verify(historicDecisionInstanceReport).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedDecisionInstanceAsc()
	  public virtual void testOrderByFinishedDecisionInstanceAsc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "asc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).orderByFinished();
		verify(historicDecisionInstanceReport).asc();
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedDecisionInstanceDesc()
	  public virtual void testOrderByFinishedDecisionInstanceDesc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "desc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicDecisionInstanceReport).orderByFinished();
		verify(historicDecisionInstanceReport).desc();
		verify(historicDecisionInstanceReport).list();
		verifyNoMoreInteractions(historicDecisionInstanceReport);
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

	  protected internal virtual IDictionary<string, object> CompleteQueryParameters
	  {
		  get
		  {
			IDictionary<string, object> parameters = new Dictionary<string, object>();
    
			parameters["decisionDefinitionIdIn"] = EXAMPLE_DD_ID + "," + ANOTHER_EXAMPLE_DD_ID;
			parameters["decisionDefinitionKeyIn"] = EXAMPLE_DD_KEY + "," + ANOTHER_EXAMPLE_DD_KEY;
			parameters["tenantIdIn"] = EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID;
			parameters["withoutTenantId"] = true;
			parameters["compact"] = true;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyQueryParameterInvocations()
	  {
		verify(historicDecisionInstanceReport).decisionDefinitionIdIn(EXAMPLE_DD_ID, ANOTHER_EXAMPLE_DD_ID);
		verify(historicDecisionInstanceReport).decisionDefinitionKeyIn(EXAMPLE_DD_KEY, ANOTHER_EXAMPLE_DD_KEY);
		verify(historicDecisionInstanceReport).tenantIdIn(EXAMPLE_TENANT_ID, ANOTHER_EXAMPLE_TENANT_ID);
		verify(historicDecisionInstanceReport).withoutTenantId();
		verify(historicDecisionInstanceReport).compact();
	  }
	}

}