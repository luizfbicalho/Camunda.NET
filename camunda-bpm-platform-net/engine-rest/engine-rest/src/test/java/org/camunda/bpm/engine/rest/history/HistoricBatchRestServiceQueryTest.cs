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
//	import static org.camunda.bpm.engine.rest.util.JsonPathUtil.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using HistoricBatchDto = org.camunda.bpm.engine.rest.dto.history.batch.HistoricBatchDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricBatchRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_BATCH_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/batch";
	  protected internal static readonly string HISTORIC_BATCH_QUERY_COUNT_URL = HISTORIC_BATCH_RESOURCE_URL + "/count";

	  protected internal HistoricBatchQuery queryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpHistoricBatchQueryMock()
	  public virtual void setUpHistoricBatchQueryMock()
	  {
		IList<HistoricBatch> mockHistoricBatches = MockProvider.createMockHistoricBatches();
		queryMock = mock(typeof(HistoricBatchQuery));

		when(queryMock.list()).thenReturn(mockHistoricBatches);
		when(queryMock.count()).thenReturn((long) mockHistoricBatches.Count);

		when(processEngine.HistoryService.createHistoricBatchQuery()).thenReturn(queryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);

		verify(queryMock).list();
		verifyNoMoreInteractions(queryMock);

		verifyHistoricBatchListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnknownQueryParameter()
	  public virtual void testUnknownQueryParameter()
	  {
		Response response = given().queryParam("unknown", "unknown").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);

		verify(queryMock, never()).batchId(anyString());
		verify(queryMock).list();

		verifyHistoricBatchListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "batchId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryByBatchId()
	  public virtual void testHistoricBatchQueryByBatchId()
	  {
		Response response = given().queryParam("batchId", MockProvider.EXAMPLE_BATCH_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		inOrder.verify(queryMock).list();
		inOrder.verifyNoMoreInteractions();

		verifyHistoricBatchListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryByCompleted()
	  public virtual void testHistoricBatchQueryByCompleted()
	  {
		Response response = given().queryParam("completed", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).completed(true);
		inOrder.verify(queryMock).list();
		inOrder.verifyNoMoreInteractions();

		verifyHistoricBatchListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBatchQueryByNotCompleted()
	  public virtual void testHistoricBatchQueryByNotCompleted()
	  {
		Response response = given().queryParam("completed", false).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).completed(false);
		inOrder.verify(queryMock).list();
		inOrder.verifyNoMoreInteractions();

		verifyHistoricBatchListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullHistoricBatchQuery()
	  public virtual void testFullHistoricBatchQuery()
	  {
		Response response = given().queryParams(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);

		verifyQueryParameterInvocations();
		verify(queryMock).list();
		verifyNoMoreInteractions(queryMock);

		verifyHistoricBatchListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_BATCH_QUERY_COUNT_URL);

		verify(queryMock).count();
		verifyNoMoreInteractions(queryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQueryCount()
	  public virtual void testFullQueryCount()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_BATCH_QUERY_COUNT_URL);

		verifyQueryParameterInvocations();
		verify(queryMock).count();
		verifyNoMoreInteractions(queryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("batchId", "desc", Status.OK);
		inOrder.verify(queryMock).orderById();
		inOrder.verify(queryMock).desc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("batchId", "asc", Status.OK);
		inOrder.verify(queryMock).orderById();
		inOrder.verify(queryMock).asc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("startTime", "desc", Status.OK);
		inOrder.verify(queryMock).orderByStartTime();
		inOrder.verify(queryMock).desc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("startTime", "asc", Status.OK);
		inOrder.verify(queryMock).orderByStartTime();
		inOrder.verify(queryMock).asc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("endTime", "desc", Status.OK);
		inOrder.verify(queryMock).orderByEndTime();
		inOrder.verify(queryMock).desc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("endTime", "asc", Status.OK);
		inOrder.verify(queryMock).orderByEndTime();
		inOrder.verify(queryMock).asc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(queryMock).orderByTenantId();
		inOrder.verify(queryMock).desc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(queryMock).orderByTenantId();
		inOrder.verify(queryMock).asc();
	  }

	  private void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_BATCH_RESOURCE_URL);
	  }

	  protected internal virtual IDictionary<string, object> CompleteQueryParameters
	  {
		  get
		  {
			IDictionary<string, object> parameters = new Dictionary<string, object>();
    
			parameters["batchId"] = MockProvider.EXAMPLE_BATCH_ID;
			parameters["type"] = MockProvider.EXAMPLE_BATCH_TYPE;
			parameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID + "," + MockProvider.ANOTHER_EXAMPLE_TENANT_ID;
			parameters["withoutTenantId"] = true;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyQueryParameterInvocations()
	  {
		verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		verify(queryMock).type(MockProvider.EXAMPLE_BATCH_TYPE);
		verify(queryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(queryMock).withoutTenantId();
	  }

	  protected internal virtual void verifyHistoricBatchListJson(string historicBatchListJson)
	  {
		IList<object> batches = from(historicBatchListJson).get();
		assertEquals("There should be one historic batch returned.", 1, batches.Count);

		HistoricBatchDto historicBatch = from(historicBatchListJson).getObject("[0]", typeof(HistoricBatchDto));
		assertNotNull("The returned historic batch should not be null.", historicBatch);
		assertEquals(MockProvider.EXAMPLE_BATCH_ID, historicBatch.Id);
		assertEquals(MockProvider.EXAMPLE_BATCH_TYPE, historicBatch.Type);
		assertEquals(MockProvider.EXAMPLE_BATCH_TOTAL_JOBS, historicBatch.TotalJobs);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOBS_PER_SEED, historicBatch.BatchJobsPerSeed);
		assertEquals(MockProvider.EXAMPLE_INVOCATIONS_PER_BATCH_JOB, historicBatch.InvocationsPerBatchJob);
		assertEquals(MockProvider.EXAMPLE_SEED_JOB_DEFINITION_ID, historicBatch.SeedJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_MONITOR_JOB_DEFINITION_ID, historicBatch.MonitorJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOB_DEFINITION_ID, historicBatch.BatchJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_TENANT_ID, historicBatch.TenantId);
		assertEquals(MockProvider.EXAMPLE_USER_ID, historicBatch.CreateUserId);
		assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_BATCH_START_TIME), historicBatch.StartTime);
		assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_BATCH_END_TIME), historicBatch.EndTime);
		assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_BATCH_REMOVAL_TIME), historicBatch.RemovalTime);
	  }

	}

}