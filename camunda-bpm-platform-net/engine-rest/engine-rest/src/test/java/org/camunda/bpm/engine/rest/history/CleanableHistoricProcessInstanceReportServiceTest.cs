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


	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
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

	public class CleanableHistoricProcessInstanceReportServiceTest : AbstractRestServiceTest
	{

	  private const string EXAMPLE_PD_NAME = "aName";
	  private const string EXAMPLE_PD_KEY = "aKey";
	  private const int EXAMPLE_PD_VERSION = 42;
	  private const int EXAMPLE_TTL = 5;
	  private const long EXAMPLE_FINISHED_PI_COUNT = 10l;
	  private const long EXAMPLE_CLEANABLE_PI_COUNT = 5l;
	  private const string EXAMPLE_TENANT_ID = "aTenantId";

	  protected internal const string ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID = "anotherDefId";
	  protected internal const string ANOTHER_EXAMPLE_PD_KEY = "anotherDefKey";
	  protected internal const string ANOTHER_EXAMPLE_TENANT_ID = "anotherTenantId";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_URL = TEST_RESOURCE_ROOT_PATH + "/history/process-definition";
	  protected internal static readonly string HISTORIC_REPORT_URL = HISTORY_URL + "/cleanable-process-instance-report";
	  protected internal static readonly string HISTORIC_REPORT_COUNT_URL = HISTORIC_REPORT_URL + "/count";

	  private CleanableHistoricProcessInstanceReport historicProcessInstanceReport;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupHistoryReportMock();
	  }

	  private void setupHistoryReportMock()
	  {
		CleanableHistoricProcessInstanceReport report = mock(typeof(CleanableHistoricProcessInstanceReport));

		when(report.processDefinitionIdIn(anyString())).thenReturn(report);
		when(report.processDefinitionKeyIn(anyString())).thenReturn(report);

		CleanableHistoricProcessInstanceReportResult reportResult = mock(typeof(CleanableHistoricProcessInstanceReportResult));

		when(reportResult.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(reportResult.ProcessDefinitionKey).thenReturn(EXAMPLE_PD_KEY);
		when(reportResult.ProcessDefinitionName).thenReturn(EXAMPLE_PD_NAME);
		when(reportResult.ProcessDefinitionVersion).thenReturn(EXAMPLE_PD_VERSION);
		when(reportResult.HistoryTimeToLive).thenReturn(EXAMPLE_TTL);
		when(reportResult.FinishedProcessInstanceCount).thenReturn(EXAMPLE_FINISHED_PI_COUNT);
		when(reportResult.CleanableProcessInstanceCount).thenReturn(EXAMPLE_CLEANABLE_PI_COUNT);
		when(reportResult.TenantId).thenReturn(EXAMPLE_TENANT_ID);

		CleanableHistoricProcessInstanceReportResult anotherReportResult = mock(typeof(CleanableHistoricProcessInstanceReportResult));

		when(anotherReportResult.ProcessDefinitionId).thenReturn(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
		when(anotherReportResult.ProcessDefinitionKey).thenReturn(ANOTHER_EXAMPLE_PD_KEY);
		when(anotherReportResult.ProcessDefinitionName).thenReturn("pdName");
		when(anotherReportResult.ProcessDefinitionVersion).thenReturn(33);
		when(anotherReportResult.HistoryTimeToLive).thenReturn(null);
		when(anotherReportResult.FinishedProcessInstanceCount).thenReturn(13l);
		when(anotherReportResult.CleanableProcessInstanceCount).thenReturn(0l);
		when(anotherReportResult.TenantId).thenReturn(ANOTHER_EXAMPLE_TENANT_ID);

		IList<CleanableHistoricProcessInstanceReportResult> mocks = new List<CleanableHistoricProcessInstanceReportResult>();
		mocks.Add(reportResult);
		mocks.Add(anotherReportResult);

		when(report.list()).thenReturn(mocks);
		when(report.count()).thenReturn((long) mocks.Count);

		historicProcessInstanceReport = report;
		when(processEngine.HistoryService.createCleanableHistoricProcessInstanceReport()).thenReturn(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetReport()
	  public virtual void testGetReport()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		InOrder inOrder = Mockito.inOrder(historicProcessInstanceReport);
		inOrder.verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReportRetrieval()
	  public virtual void testReportRetrieval()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(historicProcessInstanceReport);
		inOrder.verify(historicProcessInstanceReport).list();

		string content = response.asString();
		IList<string> reportResults = from(content).getList("");
		Assert.assertEquals("There should be two report results returned.", 2, reportResults.Count);
		Assert.assertNotNull(reportResults[0]);

		string returnedDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedDefinitionName = from(content).getString("[0].processDefinitionName");
		int returnedDefinitionVersion = from(content).getInt("[0].processDefinitionVersion");
		int returnedTTL = from(content).getInt("[0].historyTimeToLive");
		long returnedFinishedCount = from(content).getLong("[0].finishedProcessInstanceCount");
		long returnedCleanableCount = from(content).getLong("[0].cleanableProcessInstanceCount");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(EXAMPLE_PROCESS_DEFINITION_ID, returnedDefinitionId);
		Assert.assertEquals(EXAMPLE_PD_KEY, returnedDefinitionKey);
		Assert.assertEquals(EXAMPLE_PD_NAME, returnedDefinitionName);
		Assert.assertEquals(EXAMPLE_PD_VERSION, returnedDefinitionVersion);
		Assert.assertEquals(EXAMPLE_TTL, returnedTTL);
		Assert.assertEquals(EXAMPLE_FINISHED_PI_COUNT, returnedFinishedCount);
		Assert.assertEquals(EXAMPLE_CLEANABLE_PI_COUNT, returnedCleanableCount);
		Assert.assertEquals(EXAMPLE_TENANT_ID, returnedTenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthorization()
	  public virtual void testMissingAuthorization()
	  {
		string message = "not authorized";
		when(historicProcessInstanceReport.list()).thenThrow(new AuthorizationException(message));

		given().then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(HISTORIC_REPORT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDefinitionId()
	  public virtual void testQueryByDefinitionId()
	  {
		given().queryParam("processDefinitionIdIn", EXAMPLE_PROCESS_DEFINITION_ID + "," + ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).processDefinitionIdIn(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDefinitionKey()
	  public virtual void testQueryByDefinitionKey()
	  {
		given().queryParam("processDefinitionKeyIn", EXAMPLE_PD_KEY + "," + ANOTHER_EXAMPLE_PD_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).processDefinitionKeyIn(EXAMPLE_PD_KEY, ANOTHER_EXAMPLE_PD_KEY);
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	  public virtual void testQueryByTenantId()
	  {
		given().queryParam("tenantIdIn", EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).tenantIdIn(EXAMPLE_TENANT_ID, ANOTHER_EXAMPLE_TENANT_ID);
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithoutTenantId()
	  public virtual void testQueryWithoutTenantId()
	  {
		given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).withoutTenantId();
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCompact()
	  public virtual void testQueryCompact()
	  {
		given().queryParam("compact", true).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).compact();
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQuery()
	  public virtual void testFullQuery()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).when().get(HISTORIC_REPORT_URL);

		verifyQueryParameterInvocations();
		verify(historicProcessInstanceReport).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verify(historicProcessInstanceReport).count();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQueryCount()
	  public virtual void testFullQueryCount()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(2)).when().get(HISTORIC_REPORT_COUNT_URL);

		verifyQueryParameterInvocations();
		verify(historicProcessInstanceReport).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedProcessInstanceAsc()
	  public virtual void testOrderByFinishedProcessInstanceAsc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "asc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).orderByFinished();
		verify(historicProcessInstanceReport).asc();
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByFinishedProcessInstanceDesc()
	  public virtual void testOrderByFinishedProcessInstanceDesc()
	  {
		given().queryParam("sortBy", "finished").queryParam("sortOrder", "desc").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_REPORT_URL);

		verify(historicProcessInstanceReport).orderByFinished();
		verify(historicProcessInstanceReport).desc();
		verify(historicProcessInstanceReport).list();
		verifyNoMoreInteractions(historicProcessInstanceReport);
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
    
			parameters["processDefinitionIdIn"] = EXAMPLE_PROCESS_DEFINITION_ID + "," + ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID;
			parameters["processDefinitionKeyIn"] = EXAMPLE_PD_KEY + "," + ANOTHER_EXAMPLE_PD_KEY;
			parameters["tenantIdIn"] = EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID;
			parameters["withoutTenantId"] = true;
			parameters["compact"] = true;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyQueryParameterInvocations()
	  {
		verify(historicProcessInstanceReport).processDefinitionIdIn(EXAMPLE_PROCESS_DEFINITION_ID, ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
		verify(historicProcessInstanceReport).processDefinitionKeyIn(EXAMPLE_PD_KEY, ANOTHER_EXAMPLE_PD_KEY);
		verify(historicProcessInstanceReport).tenantIdIn(EXAMPLE_TENANT_ID, ANOTHER_EXAMPLE_TENANT_ID);
		verify(historicProcessInstanceReport).withoutTenantId();
		verify(historicProcessInstanceReport).compact();
	  }
	}

}