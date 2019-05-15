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
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchQuery = org.camunda.bpm.engine.batch.BatchQuery;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class BatchRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string BATCH_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/batch";
	  protected internal static readonly string SINGLE_BATCH_RESOURCE_URL = BATCH_RESOURCE_URL + "/{id}";
	  protected internal static readonly string SUSPENDED_BATCH_RESOURCE_URL = SINGLE_BATCH_RESOURCE_URL + "/suspended";

	  protected internal ManagementService managementServiceMock;
	  protected internal BatchQuery queryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpBatchQueryMock()
	  public virtual void setUpBatchQueryMock()
	  {
		Batch batchMock = MockProvider.createMockBatch();

		queryMock = mock(typeof(BatchQuery));
		when(queryMock.batchId(eq(MockProvider.EXAMPLE_BATCH_ID))).thenReturn(queryMock);
		when(queryMock.singleResult()).thenReturn(batchMock);

		managementServiceMock = mock(typeof(ManagementService));
		when(managementServiceMock.createBatchQuery()).thenReturn(queryMock);

		when(processEngine.ManagementService).thenReturn(managementServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBatch()
	  public virtual void testGetBatch()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_BATCH_RESOURCE_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		inOrder.verify(queryMock).singleResult();
		inOrder.verifyNoMoreInteractions();

		verifyBatchJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingBatch()
	  public virtual void testGetNonExistingBatch()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;
		BatchQuery batchQuery = mock(typeof(BatchQuery));
		when(batchQuery.batchId(nonExistingId)).thenReturn(batchQuery);
		when(batchQuery.singleResult()).thenReturn(null);
		when(managementServiceMock.createBatchQuery()).thenReturn(batchQuery);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Batch with id '" + nonExistingId + "' does not exist")).when().get(SINGLE_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteBatch()
	  public virtual void deleteBatch()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_BATCH_RESOURCE_URL);

		verify(managementServiceMock).deleteBatch(eq(MockProvider.EXAMPLE_BATCH_ID), eq(false));
		verifyNoMoreInteractions(managementServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteBatchNotCascade()
	  public virtual void deleteBatchNotCascade()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).queryParam("cascade", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_BATCH_RESOURCE_URL);

		verify(managementServiceMock).deleteBatch(eq(MockProvider.EXAMPLE_BATCH_ID), eq(false));
		verifyNoMoreInteractions(managementServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteBatchCascade()
	  public virtual void deleteBatchCascade()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).queryParam("cascade", true).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_BATCH_RESOURCE_URL);

		verify(managementServiceMock).deleteBatch(eq(MockProvider.EXAMPLE_BATCH_ID), eq(true));
		verifyNoMoreInteractions(managementServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteNonExistingBatch()
	  public virtual void deleteNonExistingBatch()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;

		doThrow(new BadUserRequestException("Batch for id '" + nonExistingId + "' cannot be found")).when(managementServiceMock).deleteBatch(eq(nonExistingId), eq(false));

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Unable to delete batch with id '" + nonExistingId + "'")).when().delete(SINGLE_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteNonExistingBatchNotCascade()
	  public virtual void deleteNonExistingBatchNotCascade()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;

		doThrow(new BadUserRequestException("Batch for id '" + nonExistingId + "' cannot be found")).when(managementServiceMock).deleteBatch(eq(nonExistingId), eq(false));

		given().pathParam("id", nonExistingId).queryParam("cascade", false).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Unable to delete batch with id '" + nonExistingId + "'")).when().delete(SINGLE_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteNonExistingBatchCascade()
	  public virtual void deleteNonExistingBatchCascade()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;

		doThrow(new BadUserRequestException("Batch for id '" + nonExistingId + "' cannot be found")).when(managementServiceMock).deleteBatch(eq(nonExistingId), eq(true));

		given().pathParam("id", nonExistingId).queryParam("cascade", true).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Unable to delete batch with id '" + nonExistingId + "'")).when().delete(SINGLE_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendBatch()
	  public virtual void suspendBatch()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).contentType(ContentType.JSON).body(singletonMap("suspended", true)).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SUSPENDED_BATCH_RESOURCE_URL);

		verify(managementServiceMock).suspendBatchById(eq(MockProvider.EXAMPLE_BATCH_ID));
		verifyNoMoreInteractions(managementServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendNonExistingBatch()
	  public virtual void suspendNonExistingBatch()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;

		doThrow(new BadUserRequestException("Batch for id '" + nonExistingId + "' cannot be found")).when(managementServiceMock).suspendBatchById(eq(nonExistingId));

		given().pathParam("id", nonExistingId).contentType(ContentType.JSON).body(singletonMap("suspended", true)).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Unable to suspend batch with id '" + nonExistingId + "'")).when().put(SUSPENDED_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendBatchUnauthorized()
	  public virtual void suspendBatchUnauthorized()
	  {
		string batchId = MockProvider.EXAMPLE_BATCH_ID;
		string expectedMessage = "The user with id 'userId' does not have 'UPDATE' permission on resource '" + batchId + "' of type 'Batch'.";

		doThrow(new AuthorizationException(expectedMessage)).when(managementServiceMock).suspendBatchById(eq(batchId));

		given().pathParam("id", batchId).contentType(ContentType.JSON).body(singletonMap("suspended", true)).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(expectedMessage)).when().put(SUSPENDED_BATCH_RESOURCE_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateBatch()
	  public virtual void activateBatch()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).contentType(ContentType.JSON).body(singletonMap("suspended", false)).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SUSPENDED_BATCH_RESOURCE_URL);

		verify(managementServiceMock).activateBatchById(eq(MockProvider.EXAMPLE_BATCH_ID));
		verifyNoMoreInteractions(managementServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateNonExistingBatch()
	  public virtual void activateNonExistingBatch()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;

		doThrow(new BadUserRequestException("Batch for id '" + nonExistingId + "' cannot be found")).when(managementServiceMock).activateBatchById(eq(nonExistingId));

		given().pathParam("id", nonExistingId).contentType(ContentType.JSON).body(singletonMap("suspended", false)).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Unable to activate batch with id '" + nonExistingId + "'")).when().put(SUSPENDED_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateBatchUnauthorized()
	  public virtual void activateBatchUnauthorized()
	  {
		string batchId = MockProvider.EXAMPLE_BATCH_ID;
		string expectedMessage = "The user with id 'userId' does not have 'UPDATE' permission on resource '" + batchId + "' of type 'Batch'.";

		doThrow(new AuthorizationException(expectedMessage)).when(managementServiceMock).activateBatchById(eq(batchId));

		given().pathParam("id", batchId).contentType(ContentType.JSON).body(singletonMap("suspended", false)).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(expectedMessage)).when().put(SUSPENDED_BATCH_RESOURCE_URL);

	  }

	  protected internal virtual void verifyBatchJson(string batchJson)
	  {
		BatchDto batch = from(batchJson).getObject("", typeof(BatchDto));
		assertNotNull("The returned batch should not be null.", batch);
		assertEquals(MockProvider.EXAMPLE_BATCH_ID, batch.Id);
		assertEquals(MockProvider.EXAMPLE_BATCH_TYPE, batch.Type);
		assertEquals(MockProvider.EXAMPLE_BATCH_TOTAL_JOBS, batch.TotalJobs);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOBS_PER_SEED, batch.BatchJobsPerSeed);
		assertEquals(MockProvider.EXAMPLE_INVOCATIONS_PER_BATCH_JOB, batch.InvocationsPerBatchJob);
		assertEquals(MockProvider.EXAMPLE_SEED_JOB_DEFINITION_ID, batch.SeedJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_MONITOR_JOB_DEFINITION_ID, batch.MonitorJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOB_DEFINITION_ID, batch.BatchJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_TENANT_ID, batch.TenantId);
		assertEquals(MockProvider.EXAMPLE_USER_ID, batch.CreateUserId);
	  }

	}

}