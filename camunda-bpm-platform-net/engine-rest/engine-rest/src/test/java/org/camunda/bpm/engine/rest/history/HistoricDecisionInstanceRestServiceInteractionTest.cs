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
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_DECISION_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_DECISION_INSTANCE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
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
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder;
	using SetRemovalTimeToHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricDecisionInstancesBuilder;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricDecisionInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using JsonPathUtil = org.camunda.bpm.engine.rest.util.JsonPathUtil;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricDecisionInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_DECISION_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/decision-instance";
	  protected internal static readonly string HISTORIC_SINGLE_DECISION_INSTANCE_URL = HISTORIC_DECISION_INSTANCE_URL + "/{id}";
	  protected internal static readonly string HISTORIC_DECISION_INSTANCES_DELETE_ASYNC_URL = HISTORIC_DECISION_INSTANCE_URL + "/delete";
	  protected internal static readonly string SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL = HISTORIC_DECISION_INSTANCE_URL + "/set-removal-time";

	  protected internal HistoryService historyServiceMock;
	  protected internal HistoricDecisionInstance historicInstanceMock;
	  protected internal HistoricDecisionInstanceQuery historicQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		historyServiceMock = mock(typeof(HistoryService));

		// runtime service
		when(processEngine.HistoryService).thenReturn(historyServiceMock);

		historicInstanceMock = MockProvider.createMockHistoricDecisionInstance();
		historicQueryMock = mock(typeof(HistoricDecisionInstanceQuery), RETURNS_DEEP_STUBS);

		when(historyServiceMock.createHistoricDecisionInstanceQuery()).thenReturn(historicQueryMock);
		when(historicQueryMock.decisionInstanceId(anyString())).thenReturn(historicQueryMock);
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricDecisionInstance()
	  public virtual void testGetSingleHistoricDecisionInstance()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);

		InOrder inOrder = inOrder(historicQueryMock);
		inOrder.verify(historicQueryMock).decisionInstanceId(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		inOrder.verify(historicQueryMock).singleResult();

		string content = response.asString();

		string returnedHistoricDecisionInstanceId = from(content).getString("id");
		string returnedDecisionDefinitionId = from(content).getString("decisionDefinitionId");
		string returnedDecisionDefinitionKey = from(content).getString("decisionDefinitionKey");
		string returnedDecisionDefinitionName = from(content).getString("decisionDefinitionName");
		string returnedEvaluationTime = from(content).getString("evaluationTime");
		string returnedProcessDefinitionId = from(content).getString("processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("processDefinitionKey");
		string returnedProcessInstanceId = from(content).getString("processInstanceId");
		string returnedCaseDefinitionId = from(content).getString("caseDefinitionId");
		string returnedCaseDefinitionKey = from(content).getString("caseDefinitionKey");
		string returnedCaseInstanceId = from(content).getString("caseInstanceId");
		string returnedActivityId = from(content).getString("activityId");
		string returnedActivityInstanceId = from(content).getString("activityInstanceId");
		string returnedUserId = from(content).getString("userId");
		IList<IDictionary<string, object>> returnedInputs = from(content).getList("inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("outputs");
		double? returnedCollectResultValue = from(content).getDouble("collectResultValue");
		string returnedTenantId = from(content).getString("tenantId");
		string returnedRootDecisionInstanceId = from(content).getString("rootDecisionInstanceId");
		string returnedDecisionRequirementsDefinitionId = from(content).getString("decisionRequirementsDefinitionId");
		string returnedDecisionRequirementsDefinitionKey = from(content).getString("decisionRequirementsDefinitionKey");

		assertThat(returnedHistoricDecisionInstanceId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID));
		assertThat(returnedDecisionDefinitionId, @is(EXAMPLE_DECISION_DEFINITION_ID));
		assertThat(returnedDecisionDefinitionKey, @is(MockProvider.EXAMPLE_DECISION_DEFINITION_KEY));
		assertThat(returnedDecisionDefinitionName, @is(MockProvider.EXAMPLE_DECISION_DEFINITION_NAME));
		assertThat(returnedEvaluationTime, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATION_TIME));
		assertThat(returnedProcessDefinitionId, @is(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID));
		assertThat(returnedProcessDefinitionKey, @is(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY));
		assertThat(returnedProcessInstanceId, @is(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID));
		assertThat(returnedCaseDefinitionId, @is(MockProvider.EXAMPLE_CASE_DEFINITION_ID));
		assertThat(returnedCaseDefinitionKey, @is(MockProvider.EXAMPLE_CASE_DEFINITION_KEY));
		assertThat(returnedCaseInstanceId, @is(MockProvider.EXAMPLE_CASE_INSTANCE_ID));
		assertThat(returnedActivityId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_ID));
		assertThat(returnedActivityInstanceId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_INSTANCE_ID));
		assertThat(returnedUserId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_USER_ID));
		assertThat(returnedInputs, @is(nullValue()));
		assertThat(returnedOutputs, @is(nullValue()));
		assertThat(returnedCollectResultValue, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_COLLECT_RESULT_VALUE));
		assertThat(returnedTenantId, @is(MockProvider.EXAMPLE_TENANT_ID));
		assertThat(returnedRootDecisionInstanceId, @is(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID));
		assertThat(returnedDecisionRequirementsDefinitionId, @is(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID));
		assertThat(returnedDecisionRequirementsDefinitionKey, @is(MockProvider.EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricDecisionInstanceWithInputs()
	  public virtual void testGetSingleHistoricDecisionInstanceWithInputs()
	  {
		historicInstanceMock = MockProvider.createMockHistoricDecisionInstanceWithInputs();
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID).queryParam("includeInputs", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);

		InOrder inOrder = inOrder(historicQueryMock);
		inOrder.verify(historicQueryMock).decisionInstanceId(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		inOrder.verify(historicQueryMock).includeInputs();
		inOrder.verify(historicQueryMock, never()).includeOutputs();
		inOrder.verify(historicQueryMock).singleResult();

		string content = response.asString();

		IList<IDictionary<string, object>> returnedInputs = from(content).getList("inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("outputs");
		assertThat(returnedInputs, @is(notNullValue()));
		assertThat(returnedInputs, hasSize(3));
		assertThat(returnedOutputs, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricDecisionInstanceWithOutputs()
	  public virtual void testGetSingleHistoricDecisionInstanceWithOutputs()
	  {
		historicInstanceMock = MockProvider.createMockHistoricDecisionInstanceWithOutputs();
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID).queryParam("includeOutputs", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);

		InOrder inOrder = inOrder(historicQueryMock);
		inOrder.verify(historicQueryMock).decisionInstanceId(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		inOrder.verify(historicQueryMock, never()).includeInputs();
		inOrder.verify(historicQueryMock).includeOutputs();
		inOrder.verify(historicQueryMock).singleResult();

		string content = response.asString();

		IList<IDictionary<string, object>> returnedInputs = from(content).getList("inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("outputs");
		assertThat(returnedInputs, @is(nullValue()));
		assertThat(returnedOutputs, @is(notNullValue()));
		assertThat(returnedOutputs, hasSize(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricDecisionInstanceWithInputsAndOutputs()
	  public virtual void testGetSingleHistoricDecisionInstanceWithInputsAndOutputs()
	  {
		historicInstanceMock = MockProvider.createMockHistoricDecisionInstanceWithInputsAndOutputs();
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID).queryParam("includeInputs", true).queryParam("includeOutputs", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);

		InOrder inOrder = inOrder(historicQueryMock);
		inOrder.verify(historicQueryMock).decisionInstanceId(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		inOrder.verify(historicQueryMock).includeInputs();
		inOrder.verify(historicQueryMock).includeOutputs();
		inOrder.verify(historicQueryMock).singleResult();

		string content = response.asString();

		IList<IDictionary<string, object>> returnedInputs = from(content).getList("inputs");
		IList<IDictionary<string, object>> returnedOutputs = from(content).getList("outputs");
		assertThat(returnedInputs, @is(notNullValue()));
		assertThat(returnedInputs, hasSize(3));
		assertThat(returnedOutputs, @is(notNullValue()));
		assertThat(returnedOutputs, hasSize(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricDecisionInstanceWithDisabledBinaryFetching()
	  public virtual void testGetSingleHistoricDecisionInstanceWithDisabledBinaryFetching()
	  {
		historicInstanceMock = MockProvider.createMockHistoricDecisionInstanceWithInputsAndOutputs();
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID).queryParam("disableBinaryFetching", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);

		InOrder inOrder = inOrder(historicQueryMock);
		inOrder.verify(historicQueryMock).decisionInstanceId(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		inOrder.verify(historicQueryMock).disableBinaryFetching();
		inOrder.verify(historicQueryMock).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricDecisionInstanceWithDisabledCustomObjectDeserialization()
	  public virtual void testGetSingleHistoricDecisionInstanceWithDisabledCustomObjectDeserialization()
	  {
		historicInstanceMock = MockProvider.createMockHistoricDecisionInstanceWithInputsAndOutputs();
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID).queryParam("disableCustomObjectDeserialization", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);

		InOrder inOrder = inOrder(historicQueryMock);
		inOrder.verify(historicQueryMock).decisionInstanceId(MockProvider.EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		inOrder.verify(historicQueryMock).disableCustomObjectDeserialization();
		inOrder.verify(historicQueryMock).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingHistoricCaseInstance()
	  public virtual void testGetNonExistingHistoricCaseInstance()
	  {
		when(historicQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic decision instance with id '" + MockProvider.NON_EXISTING_ID + "' does not exist")).when().get(HISTORIC_SINGLE_DECISION_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsync()
	  public virtual void testDeleteAsync()
	  {
		IList<string> ids = Arrays.asList(EXAMPLE_DECISION_INSTANCE_ID);

		Batch batchEntity = MockProvider.createMockBatch();

		when(historyServiceMock.deleteHistoricDecisionInstancesAsync(anyListOf(typeof(string)), any(typeof(HistoricDecisionInstanceQuery)), anyString())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["historicDecisionInstanceIds"] = ids;
		messageBodyJson["deleteReason"] = "a-delete-reason";

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DECISION_INSTANCES_DELETE_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(historyServiceMock, times(1)).deleteHistoricDecisionInstancesAsync(eq(ids), eq((HistoricDecisionInstanceQuery) null), eq("a-delete-reason"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithQuery()
	  public virtual void testDeleteAsyncWithQuery()
	  {
		Batch batchEntity = MockProvider.createMockBatch();

		when(historyServiceMock.deleteHistoricDecisionInstancesAsync(anyListOf(typeof(string)), any(typeof(HistoricDecisionInstanceQuery)), anyString())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		HistoricDecisionInstanceQueryDto query = new HistoricDecisionInstanceQueryDto();
		query.DecisionDefinitionKey = "decision";
		messageBodyJson["historicDecisionInstanceQuery"] = query;
		messageBodyJson["deleteReason"] = "a-delete-reason";

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DECISION_INSTANCES_DELETE_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(historyServiceMock, times(1)).deleteHistoricDecisionInstancesAsync(eq((IList<string>) null), any(typeof(HistoricDecisionInstanceQuery)), eq("a-delete-reason"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithIdsAndQuery()
	  public virtual void testDeleteAsyncWithIdsAndQuery()
	  {
		Batch batchEntity = MockProvider.createMockBatch();

		when(historyServiceMock.deleteHistoricDecisionInstancesAsync(anyListOf(typeof(string)), any(typeof(HistoricDecisionInstanceQuery)), anyString())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		HistoricDecisionInstanceQueryDto query = new HistoricDecisionInstanceQueryDto();
		query.DecisionDefinitionKey = "decision";
		messageBodyJson["historicDecisionInstanceQuery"] = query;

		IList<string> ids = Arrays.asList(EXAMPLE_DECISION_INSTANCE_ID);
		messageBodyJson["historicDecisionInstanceIds"] = ids;
		messageBodyJson["deleteReason"] = "a-delete-reason";

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_DECISION_INSTANCES_DELETE_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(historyServiceMock, times(1)).deleteHistoricDecisionInstancesAsync(eq(ids), any(typeof(HistoricDecisionInstanceQuery)), eq("a-delete-reason"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithBadRequestQuery()
	  public virtual void testDeleteAsyncWithBadRequestQuery()
	  {
		doThrow(new BadUserRequestException("process instance ids are empty")).when(historyServiceMock).deleteHistoricDecisionInstancesAsync(eq((IList<string>) null), eq((HistoricDecisionInstanceQuery) null), anyString());

		given().contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(HISTORIC_DECISION_INSTANCES_DELETE_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByIds()
	  public virtual void shouldSetRemovalTime_ByIds()
	  {
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicDecisionInstanceIds"] = Collections.singletonList(EXAMPLE_DECISION_INSTANCE_ID);
		payload["calculatedRemovalTime"] = true;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricDecisionInstances();

		verify(builder).calculatedRemovalTime();
		verify(builder).byIds(EXAMPLE_DECISION_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByQuery()
	  public virtual void shouldSetRemovalTime_ByQuery()
	  {
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["calculatedRemovalTime"] = true;
		payload["historicDecisionInstanceQuery"] = Collections.singletonMap("decisionInstanceId", EXAMPLE_DECISION_INSTANCE_ID);

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricDecisionInstances();

		verify(historicQueryMock).decisionInstanceId(EXAMPLE_DECISION_INSTANCE_ID);

		verify(builder).calculatedRemovalTime();
		verify(builder).byIds(null);
		verify(builder).byQuery(historicQueryMock);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Absolute()
	  public virtual void shouldSetRemovalTime_Absolute()
	  {
		DateTime removalTime = DateTime.Now;

		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicDecisionInstanceIds"] = Collections.singletonList(EXAMPLE_DECISION_INSTANCE_ID);
		payload["absoluteRemovalTime"] = removalTime;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricDecisionInstances();

		verify(builder).absoluteRemovalTime(removalTime);
		verify(builder).byIds(EXAMPLE_DECISION_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTime_Absolute()
	  public virtual void shouldNotSetRemovalTime_Absolute()
	  {
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicDecisionInstanceIds"] = Collections.singletonList(EXAMPLE_DECISION_INSTANCE_ID);
		payload["absoluteRemovalTime"] = null;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);

		SetRemovalTimeToHistoricDecisionInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricDecisionInstances();

		verify(builder).byIds(EXAMPLE_DECISION_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTime()
	  public virtual void shouldClearRemovalTime()
	  {
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["historicDecisionInstanceIds"] = Collections.singletonList(EXAMPLE_DECISION_INSTANCE_ID);
		payload["clearedRemovalTime"] = true;

		given().contentType(ContentType.JSON).body(payload).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);

		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyServiceMock.setRemovalTimeToHistoricDecisionInstances();

		verify(builder).clearedRemovalTime();
		verify(builder).byIds(EXAMPLE_DECISION_INSTANCE_ID);
		verify(builder).byQuery(null);
		verify(builder).executeAsync();
		verifyNoMoreInteractions(builder);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Response()
	  public virtual void shouldSetRemovalTime_Response()
	  {
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		Batch batchEntity = MockProvider.createMockBatch();
		when(builderMock.executeAsync()).thenReturn(batchEntity);

		Response response = given().contentType(ContentType.JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);

		verifyBatchJson(response.asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ThrowBadUserException()
	  public virtual void shouldSetRemovalTime_ThrowBadUserException()
	  {
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builderMock = mock(typeof(SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder), RETURNS_DEEP_STUBS);

		when(historyServiceMock.setRemovalTimeToHistoricDecisionInstances()).thenReturn(builderMock);

		doThrow(typeof(BadUserRequestException)).when(builderMock).executeAsync();

		given().contentType(ContentType.JSON).body(Collections.emptyMap()).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_REMOVAL_TIME_HISTORIC_DECISION_INSTANCES_ASYNC_URL);
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