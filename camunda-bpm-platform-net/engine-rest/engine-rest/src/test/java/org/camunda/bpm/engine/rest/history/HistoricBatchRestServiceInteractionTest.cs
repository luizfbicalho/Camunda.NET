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
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_BATCH_ID;
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
//	import static org.mockito.Mockito.RETURNS_DEEP_STUBS;
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

	using ContentType = io.restassured.http.ContentType;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using SetRemovalTimeSelectModeForHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricBatchesBuilder;
	using SetRemovalTimeToHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricBatchesBuilder;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricBatchDto = org.camunda.bpm.engine.rest.dto.history.batch.HistoricBatchDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using JsonPathUtil = org.camunda.bpm.engine.rest.util.JsonPathUtil;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using Response = io.restassured.response.Response;


	public class HistoricBatchRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_BATCH_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/batch";
	  protected internal static readonly string HISTORIC_SINGLE_BATCH_RESOURCE_URL = HISTORIC_BATCH_RESOURCE_URL + "/{id}";
	  protected internal static readonly string SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL = HISTORIC_BATCH_RESOURCE_URL + "/set-removal-time";

	  protected internal HistoryService historyServiceMock;
	  protected internal HistoricBatchQuery queryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpHistoricBatchQueryMock()
	  public virtual void setUpHistoricBatchQueryMock()
	  {
		HistoricBatch historicBatchMock = MockProvider.createMockHistoricBatch();

		queryMock = mock(typeof(HistoricBatchQuery));
		when(queryMock.batchId(eq(MockProvider.EXAMPLE_BATCH_ID))).thenReturn(queryMock);
		when(queryMock.singleResult()).thenReturn(historicBatchMock);

		historyServiceMock = mock(typeof(HistoryService));
		when(historyServiceMock.createHistoricBatchQuery()).thenReturn(queryMock);

		when(processEngine.HistoryService).thenReturn(historyServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetHistoricBatch()
	  public virtual void testGetHistoricBatch()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_BATCH_RESOURCE_URL);

		InOrder inOrder = inOrder(queryMock);
		inOrder.verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		inOrder.verify(queryMock).singleResult();
		inOrder.verifyNoMoreInteractions();

		verifyHistoricBatchJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingHistoricBatch()
	  public virtual void testGetNonExistingHistoricBatch()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;
		HistoricBatchQuery historicBatchQuery = mock(typeof(HistoricBatchQuery));
		when(historicBatchQuery.batchId(nonExistingId)).thenReturn(historicBatchQuery);
		when(historicBatchQuery.singleResult()).thenReturn(null);
		when(historyServiceMock.createHistoricBatchQuery()).thenReturn(historicBatchQuery);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic batch with id '" + nonExistingId + "' does not exist")).when().get(HISTORIC_SINGLE_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteHistoricBatch()
	  public virtual void deleteHistoricBatch()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_BATCH_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(HISTORIC_SINGLE_BATCH_RESOURCE_URL);

		verify(historyServiceMock).deleteHistoricBatch(eq(MockProvider.EXAMPLE_BATCH_ID));
		verifyNoMoreInteractions(historyServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteNonExistingHistoricBatch()
	  public virtual void deleteNonExistingHistoricBatch()
	  {
		string nonExistingId = MockProvider.NON_EXISTING_ID;

		doThrow(new BadUserRequestException("Historic batch for id '" + nonExistingId + "' cannot be found")).when(historyServiceMock).deleteHistoricBatch(eq(nonExistingId));

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Unable to delete historic batch with id '" + nonExistingId + "'")).when().delete(HISTORIC_SINGLE_BATCH_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByIds()
	  public virtual void shouldSetRemovalTime_ByIds()
	  {
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicBatchIds"] = Collections.singletonList(EXAMPLE_BATCH_ID);
		payload["calculatedRemovalTime"] = true;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyServiceMock.setRemovalTimeToHistoricBatches();

		verify(builder).calculatedRemovalTime();
		verify(builder).byIds(EXAMPLE_BATCH_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByQuery()
	  public virtual void shouldSetRemovalTime_ByQuery()
	  {
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["calculatedRemovalTime"] = true;
		payload["historicBatchQuery"] = Collections.singletonMap("batchId", EXAMPLE_BATCH_ID);

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyServiceMock.setRemovalTimeToHistoricBatches();

		verify(queryMock).batchId(EXAMPLE_BATCH_ID);

		verify(builder).calculatedRemovalTime();
		verify(builder).byIds(null);
		verify(builder).byQuery(queryMock);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Absolute()
	  public virtual void shouldSetRemovalTime_Absolute()
	  {
		DateTime removalTime = DateTime.Now;

		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicBatchIds"] = Collections.singletonList(EXAMPLE_BATCH_ID);
		payload["absoluteRemovalTime"] = removalTime;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyServiceMock.setRemovalTimeToHistoricBatches();

		verify(builder).absoluteRemovalTime(removalTime);
		verify(builder).byIds(EXAMPLE_BATCH_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTime_Absolute()
	  public virtual void shouldNotSetRemovalTime_Absolute()
	  {
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicBatchIds"] = Collections.singletonList(EXAMPLE_BATCH_ID);
		payload["absoluteRemovalTime"] = null;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);

		SetRemovalTimeToHistoricBatchesBuilder builder = historyServiceMock.setRemovalTimeToHistoricBatches();

		verify(builder).byIds(EXAMPLE_BATCH_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTime()
	  public virtual void shouldClearRemovalTime()
	  {
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicBatchIds"] = Collections.singletonList(EXAMPLE_BATCH_ID);
		payload["clearedRemovalTime"] = true;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyServiceMock.setRemovalTimeToHistoricBatches();

		verify(builder).clearedRemovalTime();
		verify(builder).byIds(EXAMPLE_BATCH_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Response()
	  public virtual void shouldSetRemovalTime_Response()
	  {
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		Batch batchEntity = MockProvider.createMockBatch();
		when(builderMock.executeAsync()).thenReturn(batchEntity);

		Response response = given().contentType(ContentType.JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);

		verifyBatchJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ThrowBadUserException()
	  public virtual void shouldSetRemovalTime_ThrowBadUserException()
	  {
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricBatchesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricBatches()).thenReturn(builderMock);

		doThrow(typeof(BadUserRequestException)).when(builderMock).executeAsync();

		given().contentType(ContentType.JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_BATCHES_ASYNC_URL);
	  }

	  protected internal virtual void verifyBatchJson(string batchJson)
	  {
		BatchDto batch = JsonPathUtil.from(batchJson).getObject("", typeof(BatchDto));
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
	  }

	  protected internal virtual void verifyHistoricBatchJson(string historicBatchJson)
	  {
		HistoricBatchDto historicBatch = from(historicBatchJson).getObject("", typeof(HistoricBatchDto));
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