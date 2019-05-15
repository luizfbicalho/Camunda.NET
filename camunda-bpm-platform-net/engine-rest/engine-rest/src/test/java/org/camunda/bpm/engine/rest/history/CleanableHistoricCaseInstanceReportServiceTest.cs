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


	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;
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

	public class CleanableHistoricCaseInstanceReportServiceTest : AbstractRestServiceTest
	{

	  private const string EXAMPLE_CD_ID = "anId";
	  private const string EXAMPLE_CD_KEY = "aKey";
	  private const string EXAMPLE_CD_NAME = "aName";
	  private const int EXAMPLE_CD_VERSION = 42;
	  private const int EXAMPLE_TTL = 5;
	  private const long EXAMPLE_FINISHED_CI_COUNT = 10l;
	  private const long EXAMPLE_CLEANABLE_CI_COUNT = 5l;
	  private const string EXAMPLE_TENANT_ID = "aTenantId";

	  protected internal const string ANOTHER_EXAMPLE_CD_ID = "anotherCaseDefId";
	  protected internal const string ANOTHER_EXAMPLE_CD_KEY = "anotherCaseDefKey";
	  protected internal const string ANOTHER_EXAMPLE_TENANT_ID = "anotherTenantId";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_URL = TEST_RESOURCE_ROOT_PATH + "/history/case-definition";
	  protected internal static readonly string HISTORIC_REPORT_URL = HISTORY_URL + "/cleanable-case-instance-report";
	  protected internal static readonly string HISTORIC_REPORT_COUNT_URL = HISTORIC_REPORT_URL + "/count";

	  private CleanableHistoricCaseInstanceReport historicCaseInstanceReport;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupHistoryReportMock();
	  }

	  private void setupHistoryReportMock()
	  {
		CleanableHistoricCaseInstanceReport report = mock(typeof(CleanableHistoricCaseInstanceReport));

		when(report.caseDefinitionIdIn(anyString())).thenReturn(report);
		when(report.caseDefinitionKeyIn(anyString())).thenReturn(report);

		CleanableHistoricCaseInstanceReportResult reportResult = mock(typeof(CleanableHistoricCaseInstanceReportResult));

		when(reportResult.CaseDefinitionId).thenReturn(EXAMPLE_CD_ID);
		when(reportResult.CaseDefinitionKey).thenReturn(EXAMPLE_CD_KEY);
		when(reportResult.CaseDefinitionName).thenReturn(EXAMPLE_CD_NAME);
		when(reportResult.CaseDefinitionVersion).thenReturn(EXAMPLE_CD_VERSION);
		when(reportResult.HistoryTimeToLive).thenReturn(EXAMPLE_TTL);
		when(reportResult.FinishedCaseInstanceCount).thenReturn(EXAMPLE_FINISHED_CI_COUNT);
		when(reportResult.CleanableCaseInstanceCount).thenReturn(EXAMPLE_CLEANABLE_CI_COUNT);
		when(reportResult.TenantId).thenReturn(EXAMPLE_TENANT_ID);

		CleanableHistoricCaseInstanceReportResult anotherReportResult = mock(typeof(CleanableHistoricCaseInstanceReportResult));

		when(anotherReportResult.CaseDefinitionId).thenReturn(ANOTHER_EXAMPLE_CD_ID);
		when(anotherReportResult.CaseDefinitionKey).thenReturn(ANOTHER_EXAMPLE_CD_KEY);
		when(anotherReportResult.CaseDefinitionName).thenReturn("cdName");
		when(anotherReportResult.CaseDefinitionVersion).thenReturn(33);
		when(anotherReportResult.HistoryTimeToLive).thenReturn(null);
		when(anotherReportResult.FinishedCaseInstanceCount).thenReturn(13l);
		when(anotherReportResult.CleanableCaseInstanceCount).thenReturn(0l);
		when(anotherReportResult.TenantId).thenReturn(ANOTHER_EXAMPLE_TENANT_ID);


		IList<CleanableHistoricCaseInstanceReportResult> mocks = new List<CleanableHistoricCaseInstanceReportResult>();
		mocks.Add(reportResult);
		mocks.Add(anotherReportResult);

		when(report.list()).thenReturn(mocks);
		when(report.count()).thenReturn((long) mocks.Count);

		historicCaseInstanceReport = report;
		when(processEngine.HistoryService.createCleanableHistoricCaseInstanceReport()).thenReturn(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetReport()
	  public virtual void testGetReport()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		InOrder inOrder = Mockito.inOrder(historicCaseInstanceReport);
		inOrder.verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportRetrieval()
	  public virtual void testReportRetrieval()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(historicCaseInstanceReport);
		inOrder.verify(historicCaseInstanceReport).list();

		string content = response.asString();
		IList<string> reportResults = from(content).getList("");
		Assert.assertEquals("There should be two report results returned.", 2, reportResults.Count);
		Assert.assertNotNull(reportResults[0]);

		string returnedDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedDefinitionKey = from(content).getString("[0].caseDefinitionKey");
		string returnedDefinitionName = from(content).getString("[0].caseDefinitionName");
		int returnedDefinitionVersion = from(content).getInt("[0].caseDefinitionVersion");
		int returnedTTL = from(content).getInt("[0].historyTimeToLive");
		long returnedFinishedCount = from(content).getLong("[0].finishedCaseInstanceCount");
		long returnedCleanableCount = from(content).getLong("[0].cleanableCaseInstanceCount");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(EXAMPLE_CD_ID, returnedDefinitionId);
		Assert.assertEquals(EXAMPLE_CD_KEY, returnedDefinitionKey);
		Assert.assertEquals(EXAMPLE_CD_NAME, returnedDefinitionName);
		Assert.assertEquals(EXAMPLE_CD_VERSION, returnedDefinitionVersion);
		Assert.assertEquals(EXAMPLE_TTL, returnedTTL);
		Assert.assertEquals(EXAMPLE_FINISHED_CI_COUNT, returnedFinishedCount);
		Assert.assertEquals(EXAMPLE_CLEANABLE_CI_COUNT, returnedCleanableCount);
		Assert.assertEquals(EXAMPLE_TENANT_ID, returnedTenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthorization()
	  public virtual void testMissingAuthorization()
	  {
		string message = "not authorized";
		when(historicCaseInstanceReport.list()).thenThrow(new AuthorizationException(message));

		given().then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(HISTORIC_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDefinitionId()
	  public virtual void testQueryByDefinitionId()
	  {
		given().queryParam("caseDefinitionIdIn", EXAMPLE_CD_ID + "," + ANOTHER_EXAMPLE_CD_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).caseDefinitionIdIn(EXAMPLE_CD_ID, ANOTHER_EXAMPLE_CD_ID);
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDefinitionKey()
	  public virtual void testQueryByDefinitionKey()
	  {
		given().queryParam("caseDefinitionKeyIn", EXAMPLE_CD_KEY + "," + ANOTHER_EXAMPLE_CD_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).caseDefinitionKeyIn(EXAMPLE_CD_KEY, ANOTHER_EXAMPLE_CD_KEY);
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	  public virtual void testQueryByTenantId()
	  {
		given().queryParam("tenantIdIn", EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).tenantIdIn(EXAMPLE_TENANT_ID, ANOTHER_EXAMPLE_TENANT_ID);
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithoutTenantId()
	  public virtual void testQueryWithoutTenantId()
	  {
		given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).withoutTenantId();
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCompact()
	  public virtual void testQueryCompact()
	  {
		given().queryParam("compact", true).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).compact();
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQuery()
	  public virtual void testFullQuery()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verifyQueryParameterInvocations();
		verify(historicCaseInstanceReport).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verify(historicCaseInstanceReport).count();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQueryCount()
	  public virtual void testFullQueryCount()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verifyQueryParameterInvocations();
		verify(historicCaseInstanceReport).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedCaseInstanceAsc()
	  public virtual void testOrderByFinishedCaseInstanceAsc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "asc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).orderByFinished();
		verify(historicCaseInstanceReport).asc();
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedCaseInstanceDesc()
	  public virtual void testOrderByFinishedCaseInstanceDesc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "desc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicCaseInstanceReport).orderByFinished();
		verify(historicCaseInstanceReport).desc();
		verify(historicCaseInstanceReport).list();
		verifyNoMoreInteractions(historicCaseInstanceReport);
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
    
			parameters["caseDefinitionIdIn"] = EXAMPLE_CD_ID + "," + ANOTHER_EXAMPLE_CD_ID;
			parameters["caseDefinitionKeyIn"] = EXAMPLE_CD_KEY + "," + ANOTHER_EXAMPLE_CD_KEY;
			parameters["tenantIdIn"] = EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID;
			parameters["withoutTenantId"] = true;
			parameters["compact"] = true;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyQueryParameterInvocations()
	  {
		verify(historicCaseInstanceReport).caseDefinitionIdIn(EXAMPLE_CD_ID, ANOTHER_EXAMPLE_CD_ID);
		verify(historicCaseInstanceReport).caseDefinitionKeyIn(EXAMPLE_CD_KEY, ANOTHER_EXAMPLE_CD_KEY);
		verify(historicCaseInstanceReport).tenantIdIn(EXAMPLE_TENANT_ID, ANOTHER_EXAMPLE_TENANT_ID);
		verify(historicCaseInstanceReport).withoutTenantId();
		verify(historicCaseInstanceReport).compact();
	  }
	}

}