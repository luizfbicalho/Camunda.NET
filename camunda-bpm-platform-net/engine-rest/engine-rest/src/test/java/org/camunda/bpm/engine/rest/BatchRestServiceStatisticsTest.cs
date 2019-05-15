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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.JsonPathUtil.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
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


	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using BatchStatisticsQuery = org.camunda.bpm.engine.batch.BatchStatisticsQuery;
	using BatchStatisticsDto = org.camunda.bpm.engine.rest.dto.batch.BatchStatisticsDto;
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

	public class BatchRestServiceStatisticsTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string BATCH_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/batch";
	  protected internal static readonly string BATCH_STATISTICS_URL = BATCH_RESOURCE_URL + "/statistics";
	  protected internal static readonly string BATCH_STATISTICS_COUNT_URL = BATCH_STATISTICS_URL + "/count";

	  protected internal BatchStatisticsQuery queryMock;
	  protected internal IList<BatchStatistics> mockBatchStatisticsList;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpBatchStatisticsMock()
	  public virtual void setUpBatchStatisticsMock()
	  {
		mockBatchStatisticsList = MockProvider.createMockBatchStatisticsList();

		queryMock = mock(typeof(BatchStatisticsQuery));

		when(queryMock.list()).thenReturn(mockBatchStatisticsList);
		when(queryMock.count()).thenReturn((long) mockBatchStatisticsList.Count);

		when(processEngine.ManagementService.createBatchStatisticsQuery()).thenReturn(queryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuery()
	  public virtual void testQuery()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).when().get(BATCH_STATISTICS_URL);

		verify(queryMock).list();
		verifyNoMoreInteractions(queryMock);

		verifyBatchStatisticsListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnknownQueryParameter()
	  public virtual void testUnknownQueryParameter()
	  {
		Response response = given().queryParam("unknown", "unknown").then().expect().statusCode(Status.OK.StatusCode).when().get(BATCH_STATISTICS_URL);

		verify(queryMock, never()).batchId(anyString());
		verify(queryMock).list();
		verifyNoMoreInteractions(queryMock);

		verifyBatchStatisticsListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchQueryByBatchId()
	  public virtual void testBatchQueryByBatchId()
	  {
		Response response = given().queryParam("batchId", MockProvider.EXAMPLE_BATCH_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(BATCH_STATISTICS_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		inOrder.verify(queryMock).list();
		inOrder.verifyNoMoreInteractions();

		verifyBatchStatisticsListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryActiveBatches()
	  public virtual void testQueryActiveBatches()
	  {
		Response response = given().queryParam("suspended", false).then().expect().statusCode(Status.OK.StatusCode).when().get(BATCH_STATISTICS_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).active();
		inOrder.verify(queryMock).list();
		inOrder.verifyNoMoreInteractions();

		verifyBatchStatisticsListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullBatchQuery()
	  public virtual void testFullBatchQuery()
	  {
		Response response = given().queryParams(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(BATCH_STATISTICS_URL);

		verifyQueryParameterInvocations();
		verify(queryMock).list();
		verifyNoMoreInteractions(queryMock);

		verifyBatchStatisticsListJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(BATCH_STATISTICS_COUNT_URL);

		verify(queryMock).count();
		verifyNoMoreInteractions(queryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFullQueryCount()
	  public virtual void testFullQueryCount()
	  {
		given().@params(CompleteQueryParameters).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(BATCH_STATISTICS_COUNT_URL);

		verifyQueryParameterInvocations();
		verify(queryMock).count();
		verifyNoMoreInteractions(queryMock);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryPagination()
	  public virtual void testQueryPagination()
	  {
		when(queryMock.listPage(1, 2)).thenReturn(mockBatchStatisticsList);

		Response response = given().queryParam("firstResult", 1).queryParam("maxResults", 2).then().expect().statusCode(Status.OK.StatusCode).when().get(BATCH_STATISTICS_URL);


		verify(queryMock).listPage(1, 2);
		verifyNoMoreInteractions(queryMock);

		verifyBatchStatisticsListJson(response.asString());
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
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(queryMock).orderByTenantId();
		inOrder.verify(queryMock).asc();

		inOrder = Mockito.inOrder(queryMock);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(queryMock).orderByTenantId();
		inOrder.verify(queryMock).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "batchId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(BATCH_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(BATCH_STATISTICS_URL);
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
			parameters["suspended"] = true;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyQueryParameterInvocations()
	  {
		verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		verify(queryMock).type(MockProvider.EXAMPLE_BATCH_TYPE);
		verify(queryMock).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(queryMock).withoutTenantId();
		verify(queryMock).suspended();
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(BATCH_STATISTICS_URL);
	  }

	  protected internal virtual void verifyBatchStatisticsListJson(string batchStatisticsListJson)
	  {
		IList<object> batches = from(batchStatisticsListJson).get();
		assertEquals("There should be one batch statistics returned.", 1, batches.Count);

		BatchStatisticsDto batchStatistics = from(batchStatisticsListJson).getObject("[0]", typeof(BatchStatisticsDto));
		assertNotNull("The returned batch statistics should not be null.", batchStatistics);
		assertEquals(MockProvider.EXAMPLE_BATCH_ID, batchStatistics.Id);
		assertEquals(MockProvider.EXAMPLE_BATCH_TYPE, batchStatistics.Type);
		assertEquals(MockProvider.EXAMPLE_BATCH_TOTAL_JOBS, batchStatistics.TotalJobs);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOBS_CREATED, batchStatistics.JobsCreated);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOBS_PER_SEED, batchStatistics.BatchJobsPerSeed);
		assertEquals(MockProvider.EXAMPLE_INVOCATIONS_PER_BATCH_JOB, batchStatistics.InvocationsPerBatchJob);
		assertEquals(MockProvider.EXAMPLE_SEED_JOB_DEFINITION_ID, batchStatistics.SeedJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_MONITOR_JOB_DEFINITION_ID, batchStatistics.MonitorJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOB_DEFINITION_ID, batchStatistics.BatchJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_TENANT_ID, batchStatistics.TenantId);
		assertEquals(MockProvider.EXAMPLE_USER_ID, batchStatistics.CreateUserId);
		assertEquals(MockProvider.EXAMPLE_BATCH_REMAINING_JOBS, batchStatistics.RemainingJobs);
		assertEquals(MockProvider.EXAMPLE_BATCH_COMPLETED_JOBS, batchStatistics.CompletedJobs);
		assertEquals(MockProvider.EXAMPLE_BATCH_FAILED_JOBS, batchStatistics.FailedJobs);
		assertTrue(batchStatistics.Suspended);
	  }

	}

}