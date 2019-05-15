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
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder;
	using SetRemovalTimeToHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricProcessInstancesBuilder;
	using HistoricProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using JsonPathUtil = org.camunda.bpm.engine.rest.util.JsonPathUtil;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Mockito = org.mockito.Mockito;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_DECISION_INSTANCE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.RETURNS_DEEP_STUBS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
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

	public class HistoricProcessInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal const string DELETE_REASON = "deleteReason";
	  protected internal const string TEST_DELETE_REASON = "test";
	  protected internal const string FAIL_IF_NOT_EXISTS = "failIfNotExists";
	  protected internal static readonly string HISTORIC_PROCESS_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/process-instance";
	  protected internal static readonly string HISTORIC_SINGLE_PROCESS_INSTANCE_URL = HISTORIC_PROCESS_INSTANCE_URL + "/{id}";
	  protected internal static readonly string DELETE_HISTORIC_PROCESS_INSTANCES_ASYNC_URL = HISTORIC_PROCESS_INSTANCE_URL + "/delete";
	  protected internal static readonly string SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL = HISTORIC_PROCESS_INSTANCE_URL + "/set-removal-time";
	  protected internal static readonly string HISTORIC_SINGLE_PROCESS_INSTANCE_VARIABLES_URL = HISTORIC_PROCESS_INSTANCE_URL + "/{id}/variable-instances";

	  private HistoryService historyServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		historyServiceMock = mock(typeof(HistoryService));

		// runtime service
		when(processEngine.HistoryService).thenReturn(historyServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleInstance()
	  public virtual void testGetSingleInstance()
	  {
		HistoricProcessInstance mockInstance = MockProvider.createMockHistoricProcessInstance();
		HistoricProcessInstanceQuery sampleInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));

		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.processInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.singleResult()).thenReturn(mockInstance);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_PROCESS_INSTANCE_URL);

		string content = response.asString();

		string returnedProcessInstanceId = from(content).getString("id");
		string returnedProcessInstanceBusinessKey = from(content).getString("businessKey");
		string returnedProcessDefinitionId = from(content).getString("processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("processDefinitionKey");
		string returnedStartTime = from(content).getString("startTime");
		string returnedEndTime = from(content).getString("endTime");
		long returnedDurationInMillis = from(content).getLong("durationInMillis");
		string returnedStartUserId = from(content).getString("startUserId");
		string returnedStartActivityId = from(content).getString("startActivityId");
		string returnedDeleteReason = from(content).getString(DELETE_REASON);
		string returnedSuperProcessInstanceId = from(content).getString("superProcessInstanceId");
		string returnedSuperCaseInstanceId = from(content).getString("superCaseInstanceId");
		string returnedCaseInstanceId = from(content).getString("caseInstanceId");
		string returnedTenantId = from(content).getString("tenantId");
		string returnedState = from(content).getString("state");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY, returnedProcessInstanceBusinessKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_TIME, returnedStartTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_END_TIME, returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_DURATION_MILLIS, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_USER_ID, returnedStartUserId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_ACTIVITY_ID, returnedStartActivityId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON, returnedDeleteReason);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_PROCESS_INSTANCE_ID, returnedSuperProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_CASE_INSTANCE_ID, returnedSuperCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STATE, returnedState);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingProcessInstance()
	  public virtual void testGetNonExistingProcessInstance()
	  {
		HistoricProcessInstanceQuery sampleInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));

		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.processInstanceId(anyString())).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingInstanceId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic process instance with id aNonExistingInstanceId does not exist")).when().get(HISTORIC_SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstance()
	  public virtual void testDeleteProcessInstance()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(HISTORIC_SINGLE_PROCESS_INSTANCE_URL);

		verify(historyServiceMock).deleteHistoricProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingProcessInstance()
	  public virtual void testDeleteNonExistingProcessInstance()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(historyServiceMock).deleteHistoricProcessInstance(anyString());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic process instance with id " + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID + " does not exist")).when().delete(HISTORIC_SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingProcessInstanceIfExists()
	  public virtual void testDeleteNonExistingProcessInstanceIfExists()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParam("failIfNotExists", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(HISTORIC_SINGLE_PROCESS_INSTANCE_URL);

		verify(historyServiceMock).deleteHistoricProcessInstanceIfExists(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceThrowsAuthorizationException()
	  public virtual void testDeleteProcessInstanceThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(historyServiceMock).deleteHistoricProcessInstance(anyString());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(HISTORIC_SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsync()
	  public virtual void testDeleteAsync()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		Batch batchEntity = MockProvider.createMockBatch();
		when(historyServiceMock.deleteHistoricProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(HistoricProcessInstanceQuery)), anyString())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["historicProcessInstanceIds"] = ids;
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(historyServiceMock, times(1)).deleteHistoricProcessInstancesAsync(eq(ids), eq((HistoricProcessInstanceQuery) null), eq(TEST_DELETE_REASON));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithQuery()
	  public virtual void testDeleteAsyncWithQuery()
	  {
		Batch batchEntity = MockProvider.createMockBatch();
		when(historyServiceMock.deleteHistoricProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(HistoricProcessInstanceQuery)), anyString())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		messageBodyJson["historicProcessInstanceQuery"] = query;

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(historyServiceMock, times(1)).deleteHistoricProcessInstancesAsync(eq((IList<string>) null), any(typeof(HistoricProcessInstanceQuery)), Mockito.eq(TEST_DELETE_REASON));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithBadRequestQuery()
	  public virtual void testDeleteAsyncWithBadRequestQuery()
	  {
		doThrow(new BadUserRequestException("process instance ids are empty")).when(historyServiceMock).deleteHistoricProcessInstancesAsync(eq((IList<string>) null), eq((HistoricProcessInstanceQuery) null), anyString());

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(DELETE_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAllVariablesByProcessInstanceId()
	  public virtual void testDeleteAllVariablesByProcessInstanceId()
	  {
		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(HISTORIC_SINGLE_PROCESS_INSTANCE_VARIABLES_URL);

		verify(historyServiceMock).deleteHistoricVariableInstancesByProcessInstanceId(EXAMPLE_PROCESS_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAllVariablesForNonExistingProcessInstance()
	  public virtual void testDeleteAllVariablesForNonExistingProcessInstance()
	  {
		doThrow(new NotFoundException("No historic process instance found with id: 'NON_EXISTING_ID'")).when(historyServiceMock).deleteHistoricVariableInstancesByProcessInstanceId("NON_EXISTING_ID");

		given().pathParam("id", "NON_EXISTING_ID").expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("No historic process instance found with id: 'NON_EXISTING_ID'")).when().delete(HISTORIC_SINGLE_PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByIds()
	  public virtual void shouldSetRemovalTime_ByIds()
	  {
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicProcessInstanceIds"] = Collections.singletonList(EXAMPLE_PROCESS_INSTANCE_ID);
		payload["calculatedRemovalTime"] = true;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricProcessInstances();

		verify(builder).calculatedRemovalTime();
		verify(builder).byIds(EXAMPLE_PROCESS_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByQuery()
	  public virtual void shouldSetRemovalTime_ByQuery()
	  {
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		HistoricProcessInstanceQuery query = mock(typeof(HistoricProcessInstanceQueryImpl), RETURNS_DEEP_STUBS);
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(query);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["calculatedRemovalTime"] = true;
		payload["historicProcessInstanceQuery"] = Collections.singletonMap("processDefinitionId", EXAMPLE_PROCESS_DEFINITION_ID);

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricProcessInstances();

		verify(query).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID);

		verify(builder).calculatedRemovalTime();
		verify(builder).byIds(null);
		verify(builder).byQuery(query);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Absolute()
	  public virtual void shouldSetRemovalTime_Absolute()
	  {
		DateTime removalTime = DateTime.Now;

		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicProcessInstanceIds"] = Collections.singletonList(EXAMPLE_PROCESS_INSTANCE_ID);
		payload["absoluteRemovalTime"] = removalTime;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricProcessInstances();

		verify(builder).absoluteRemovalTime(removalTime);
		verify(builder).byIds(EXAMPLE_PROCESS_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTime_Absolute()
	  public virtual void shouldNotSetRemovalTime_Absolute()
	  {
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicProcessInstanceIds"] = Collections.singletonList(EXAMPLE_PROCESS_INSTANCE_ID);
		payload["absoluteRemovalTime"] = null;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		SetRemovalTimeToHistoricProcessInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricProcessInstances();

		verify(builder).byIds(EXAMPLE_PROCESS_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTime()
	  public virtual void shouldClearRemovalTime()
	  {
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicProcessInstanceIds"] = Collections.singletonList(EXAMPLE_PROCESS_INSTANCE_ID);
		payload["clearedRemovalTime"] = true;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricProcessInstances();

		verify(builder).clearedRemovalTime();
		verify(builder).byIds(EXAMPLE_PROCESS_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Response()
	  public virtual void shouldSetRemovalTime_Response()
	  {
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		Batch batchEntity = MockProvider.createMockBatch();
		when(builderMock.executeAsync()).thenReturn(batchEntity);

		Response response = given().contentType(ContentType.JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);

		verifyBatchJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ThrowBadUserException()
	  public virtual void shouldSetRemovalTime_ThrowBadUserException()
	  {
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricProcessInstances()).thenReturn(builderMock);

		doThrow(typeof(BadUserRequestException)).when(builderMock).executeAsync();

		given().contentType(ContentType.JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_PROCESS_INSTANCES_ASYNC_URL);
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


	}

}