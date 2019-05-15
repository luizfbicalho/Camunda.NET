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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.createMockBatch;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.DATE_FORMAT_WITH_TIMEZONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;



	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricProcessInstanceQueryImpl = org.camunda.bpm.engine.impl.HistoricProcessInstanceQueryImpl;
	using HistoryServiceImpl = org.camunda.bpm.engine.impl.HistoryServiceImpl;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricProcessInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;
	using ProcessInstanceSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceSuspensionStateDto;
	using SetJobRetriesByProcessDto = org.camunda.bpm.engine.rest.dto.runtime.SetJobRetriesByProcessDto;
	using DeleteProcessInstancesDto = org.camunda.bpm.engine.rest.dto.runtime.batch.DeleteProcessInstancesDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EqualsList = org.camunda.bpm.engine.rest.helper.EqualsList;
	using EqualsMap = org.camunda.bpm.engine.rest.helper.EqualsMap;
	using ErrorMessageHelper = org.camunda.bpm.engine.rest.helper.ErrorMessageHelper;
	using ExampleVariableObject = org.camunda.bpm.engine.rest.helper.ExampleVariableObject;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using EqualsNullValue = org.camunda.bpm.engine.rest.helper.variable.EqualsNullValue;
	using EqualsObjectValue = org.camunda.bpm.engine.rest.helper.variable.EqualsObjectValue;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EqualsUntypedValue = org.camunda.bpm.engine.rest.helper.variable.EqualsUntypedValue;
	using JsonPathUtil = org.camunda.bpm.engine.rest.util.JsonPathUtil;
	using ModificationInstructionBuilder = org.camunda.bpm.engine.rest.util.ModificationInstructionBuilder;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceModificationInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationInstantiationBuilder;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using UpdateProcessInstanceSuspensionStateSelectBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateSelectBuilder;
	using UpdateProcessInstanceSuspensionStateTenantBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateTenantBuilder;
	using UpdateProcessInstancesSuspensionStateBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstancesSuspensionStateBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using LongValue = org.camunda.bpm.engine.variable.value.LongValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using InOrder = org.mockito.InOrder;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using TypeFactory = com.fasterxml.jackson.databind.type.TypeFactory;
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using Mockito = org.mockito.Mockito;

	public class ProcessInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

	  protected internal const string TEST_DELETE_REASON = "test";
	  protected internal const string RETRIES = "retries";
	  protected internal const string FAIL_IF_NOT_EXISTS = "failIfNotExists";
	  protected internal const string DELETE_REASON = "deleteReason";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/process-instance";
	  protected internal static readonly string SINGLE_PROCESS_INSTANCE_URL = PROCESS_INSTANCE_URL + "/{id}";
	  protected internal static readonly string PROCESS_INSTANCE_VARIABLES_URL = SINGLE_PROCESS_INSTANCE_URL + "/variables";
	  protected internal static readonly string DELETE_PROCESS_INSTANCES_ASYNC_URL = PROCESS_INSTANCE_URL + "/delete";
	  protected internal static readonly string DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL = PROCESS_INSTANCE_URL + "/delete-historic-query-based";
	  protected internal static readonly string SET_JOB_RETRIES_ASYNC_URL = PROCESS_INSTANCE_URL + "/job-retries";
	  protected internal static readonly string SET_JOB_RETRIES_ASYNC_HIST_QUERY_URL = PROCESS_INSTANCE_URL + "/job-retries-historic-query-based";
	  protected internal static readonly string SINGLE_PROCESS_INSTANCE_VARIABLE_URL = PROCESS_INSTANCE_VARIABLES_URL + "/{varId}";
	  protected internal static readonly string SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL = SINGLE_PROCESS_INSTANCE_VARIABLE_URL + "/data";
	  protected internal static readonly string PROCESS_INSTANCE_ACTIVIY_INSTANCES_URL = SINGLE_PROCESS_INSTANCE_URL + "/activity-instances";
	  protected internal const string EXAMPLE_PROCESS_INSTANCE_ID_WITH_NULL_VALUE_AS_VARIABLE = "aProcessInstanceWithNullValueAsVariable";
	  protected internal static readonly string SINGLE_PROCESS_INSTANCE_SUSPENDED_URL = SINGLE_PROCESS_INSTANCE_URL + "/suspended";
	  protected internal static readonly string PROCESS_INSTANCE_SUSPENDED_URL = PROCESS_INSTANCE_URL + "/suspended";
	  protected internal static readonly string PROCESS_INSTANCE_SUSPENDED_ASYNC_URL = PROCESS_INSTANCE_URL + "/suspended-async";
	  protected internal static readonly string PROCESS_INSTANCE_MODIFICATION_URL = SINGLE_PROCESS_INSTANCE_URL + "/modification";
	  protected internal static readonly string PROCESS_INSTANCE_MODIFICATION_ASYNC_URL = SINGLE_PROCESS_INSTANCE_URL + "/modification-async";

	  protected internal static readonly VariableMap EXAMPLE_OBJECT_VARIABLES = Variables.createVariables();
	  static ProcessInstanceRestServiceInteractionTest()
	  {
		ExampleVariableObject variableValue = new ExampleVariableObject();
		variableValue.Property1 = "aPropertyValue";
		variableValue.Property2 = true;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		EXAMPLE_OBJECT_VARIABLES.putValueTyped(EXAMPLE_VARIABLE_KEY, MockObjectValue.fromObjectValue(Variables.objectValue(variableValue).serializationDataFormat("application/json").create()).objectTypeName(typeof(ExampleVariableObject).FullName));
	  }

	  private RuntimeServiceImpl runtimeServiceMock;
	  private ManagementServiceImpl mockManagementService;
	  private HistoryServiceImpl historyServiceMock;

	  private UpdateProcessInstanceSuspensionStateTenantBuilder mockUpdateSuspensionStateBuilder;
	  private UpdateProcessInstanceSuspensionStateSelectBuilder mockUpdateSuspensionStateSelectBuilder;
	  private UpdateProcessInstancesSuspensionStateBuilder mockUpdateProcessInstancesSuspensionStateBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeServiceImpl));
		mockManagementService = mock(typeof(ManagementServiceImpl));
		historyServiceMock = mock(typeof(HistoryServiceImpl));

		// variables
		when(runtimeServiceMock.getVariablesTyped(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, true)).thenReturn(EXAMPLE_VARIABLES);
		when(runtimeServiceMock.getVariablesTyped(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID, true)).thenReturn(EXAMPLE_OBJECT_VARIABLES);
		when(runtimeServiceMock.getVariablesTyped(EXAMPLE_PROCESS_INSTANCE_ID_WITH_NULL_VALUE_AS_VARIABLE, true)).thenReturn(EXAMPLE_VARIABLES_WITH_NULL_VALUE);

		// activity instances
		when(runtimeServiceMock.getActivityInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).thenReturn(EXAMPLE_ACTIVITY_INSTANCE);

		mockUpdateSuspensionStateSelectBuilder = mock(typeof(UpdateProcessInstanceSuspensionStateSelectBuilder));
		when(runtimeServiceMock.updateProcessInstanceSuspensionState()).thenReturn(mockUpdateSuspensionStateSelectBuilder);

		mockUpdateSuspensionStateBuilder = mock(typeof(UpdateProcessInstanceSuspensionStateTenantBuilder));
		when(mockUpdateSuspensionStateSelectBuilder.byProcessInstanceId(anyString())).thenReturn(mockUpdateSuspensionStateBuilder);
		when(mockUpdateSuspensionStateSelectBuilder.byProcessDefinitionId(anyString())).thenReturn(mockUpdateSuspensionStateBuilder);
		when(mockUpdateSuspensionStateSelectBuilder.byProcessDefinitionKey(anyString())).thenReturn(mockUpdateSuspensionStateBuilder);

		mockUpdateProcessInstancesSuspensionStateBuilder = mock(typeof(UpdateProcessInstancesSuspensionStateBuilder));
		when(mockUpdateSuspensionStateSelectBuilder.byProcessInstanceIds(anyList())).thenReturn(mockUpdateProcessInstancesSuspensionStateBuilder);
		when(mockUpdateSuspensionStateSelectBuilder.byProcessInstanceQuery(any(typeof(ProcessInstanceQuery)))).thenReturn(mockUpdateProcessInstancesSuspensionStateBuilder);
		when(mockUpdateSuspensionStateSelectBuilder.byHistoricProcessInstanceQuery(any(typeof(HistoricProcessInstanceQuery)))).thenReturn(mockUpdateProcessInstancesSuspensionStateBuilder);

		// runtime service
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);
		when(processEngine.ManagementService).thenReturn(mockManagementService);
		when(processEngine.HistoryService).thenReturn(historyServiceMock);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetActivityInstanceTree()
	  public virtual void testGetActivityInstanceTree()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(EXAMPLE_ACTIVITY_INSTANCE_ID)).body("parentActivityInstanceId", equalTo(EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID)).body("activityId", equalTo(EXAMPLE_ACTIVITY_ID)).body("processInstanceId", equalTo(EXAMPLE_PROCESS_INSTANCE_ID)).body("processDefinitionId", equalTo(EXAMPLE_PROCESS_DEFINITION_ID)).body("executionIds", not(empty())).body("executionIds[0]", equalTo(EXAMPLE_EXECUTION_ID)).body("activityName", equalTo(EXAMPLE_ACTIVITY_NAME)).body("name", equalTo(EXAMPLE_ACTIVITY_NAME)).body("childActivityInstances", not(empty())).body("childActivityInstances[0].id", equalTo(CHILD_EXAMPLE_ACTIVITY_INSTANCE_ID)).body("childActivityInstances[0].parentActivityInstanceId", equalTo(CHILD_EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID)).body("childActivityInstances[0].activityId", equalTo(CHILD_EXAMPLE_ACTIVITY_ID)).body("childActivityInstances[0].activityType", equalTo(CHILD_EXAMPLE_ACTIVITY_TYPE)).body("childActivityInstances[0].activityName", equalTo(CHILD_EXAMPLE_ACTIVITY_NAME)).body("childActivityInstances[0].name", equalTo(CHILD_EXAMPLE_ACTIVITY_NAME)).body("childActivityInstances[0].processInstanceId", equalTo(CHILD_EXAMPLE_PROCESS_INSTANCE_ID)).body("childActivityInstances[0].processDefinitionId", equalTo(CHILD_EXAMPLE_PROCESS_DEFINITION_ID)).body("childActivityInstances[0].executionIds", not(empty())).body("childActivityInstances[0].childActivityInstances", empty()).body("childActivityInstances[0].childTransitionInstances", empty()).body("childTransitionInstances", not(empty())).body("childTransitionInstances[0].id", equalTo(CHILD_EXAMPLE_ACTIVITY_INSTANCE_ID)).body("childTransitionInstances[0].parentActivityInstanceId", equalTo(CHILD_EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID)).body("childTransitionInstances[0].activityId", equalTo(CHILD_EXAMPLE_ACTIVITY_ID)).body("childTransitionInstances[0].activityName", equalTo(CHILD_EXAMPLE_ACTIVITY_NAME)).body("childTransitionInstances[0].activityType", equalTo(CHILD_EXAMPLE_ACTIVITY_TYPE)).body("childTransitionInstances[0].targetActivityId", equalTo(CHILD_EXAMPLE_ACTIVITY_ID)).body("childTransitionInstances[0].processInstanceId", equalTo(CHILD_EXAMPLE_PROCESS_INSTANCE_ID)).body("childTransitionInstances[0].processDefinitionId", equalTo(CHILD_EXAMPLE_PROCESS_DEFINITION_ID)).body("childTransitionInstances[0].executionId", equalTo(EXAMPLE_EXECUTION_ID)).when().get(PROCESS_INSTANCE_ACTIVIY_INSTANCES_URL);

		Assert.assertEquals("Should return right number of properties", 11, response.jsonPath().getMap("").size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetActivityInstanceTreeForNonExistingProcessInstance()
	  public virtual void testGetActivityInstanceTreeForNonExistingProcessInstance()
	  {
		when(runtimeServiceMock.getActivityInstance(anyString())).thenReturn(null);

		given().pathParam("id", "aNonExistingProcessInstanceId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Process instance with id aNonExistingProcessInstanceId does not exist")).when().get(PROCESS_INSTANCE_ACTIVIY_INSTANCES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetActivityInstanceTreeWithInternalError()
	  public virtual void testGetActivityInstanceTreeWithInternalError()
	  {
		when(runtimeServiceMock.getActivityInstance(anyString())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", "aNonExistingProcessInstanceId").then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("expected exception")).when().get(PROCESS_INSTANCE_ACTIVIY_INSTANCES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetActivityInstanceTreeThrowsAuthorizationException()
	  public virtual void testGetActivityInstanceTreeThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(runtimeServiceMock.getActivityInstance(anyString())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(PROCESS_INSTANCE_ACTIVIY_INSTANCES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariables()
	  public virtual void testGetVariables()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_VARIABLE_KEY, notNullValue()).body(EXAMPLE_VARIABLE_KEY + ".value", equalTo(EXAMPLE_VARIABLE_VALUE.Value)).body(EXAMPLE_VARIABLE_KEY + ".type", equalTo(typeof(string).Name)).when().get(PROCESS_INSTANCE_VARIABLES_URL);

		Assert.assertEquals("Should return exactly one variable", 1, response.jsonPath().getMap("").size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsync()
	  public virtual void testDeleteAsync()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(ids, null, TEST_DELETE_REASON, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithQuery()
	  public virtual void testDeleteAsyncWithQuery()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		messageBodyJson["processInstanceQuery"] = query;
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(anyListOf(typeof(string)), Mockito.any(typeof(ProcessInstanceQuery)), Mockito.eq(TEST_DELETE_REASON), Mockito.eq(false), Mockito.eq(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithBadRequestQuery()
	  public virtual void testDeleteAsyncWithBadRequestQuery()
	  {
		doThrow(new BadUserRequestException("process instance ids are empty")).when(runtimeServiceMock).deleteProcessInstancesAsync(eq((IList<string>) null), eq((ProcessInstanceQuery) null), anyString(), anyBoolean(), anyBoolean());

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithSkipCustomListeners()
	  public virtual void testDeleteAsyncWithSkipCustomListeners()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;
		messageBodyJson["processInstanceIds"] = Arrays.asList("processInstanceId1", "processInstanceId2");
		messageBodyJson["skipCustomListeners"] = true;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_URL);

		verify(runtimeServiceMock).deleteProcessInstancesAsync(anyListOf(typeof(string)), Mockito.any(typeof(ProcessInstanceQuery)), Mockito.eq(TEST_DELETE_REASON), Mockito.eq(true), Mockito.eq(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncWithSkipSubprocesses()
	  public virtual void testDeleteAsyncWithSkipSubprocesses()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[DELETE_REASON] = TEST_DELETE_REASON;
		messageBodyJson["processInstanceIds"] = Arrays.asList("processInstanceId1", "processInstanceId2");
		messageBodyJson["skipSubprocesses"] = true;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_URL);

		verify(runtimeServiceMock).deleteProcessInstancesAsync(anyListOf(typeof(string)), Mockito.any(typeof(ProcessInstanceQuery)), Mockito.eq(TEST_DELETE_REASON), Mockito.eq(false), Mockito.eq(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithQuery()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithQuery()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		HistoricProcessInstanceQuery mockedHistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQueryImpl));
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(mockedHistoricProcessInstanceQuery);
		IList<HistoricProcessInstance> historicProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		when(mockedHistoricProcessInstanceQuery.list()).thenReturn(historicProcessInstances);

		DeleteProcessInstancesDto body = new DeleteProcessInstancesDto();
		body.HistoricProcessInstanceQuery = new HistoricProcessInstanceQueryDto();

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), null, null, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithProcessInstanceIds()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithProcessInstanceIds()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		DeleteProcessInstancesDto body = new DeleteProcessInstancesDto();
		body.ProcessInstanceIds = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), null, null, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithQueryAndProcessInstanceIds()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithQueryAndProcessInstanceIds()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		HistoricProcessInstanceQuery mockedHistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQueryImpl));
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(mockedHistoricProcessInstanceQuery);
		IList<HistoricProcessInstance> historicProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		when(mockedHistoricProcessInstanceQuery.list()).thenReturn(historicProcessInstances);

		DeleteProcessInstancesDto body = new DeleteProcessInstancesDto();
		body.HistoricProcessInstanceQuery = new HistoricProcessInstanceQueryDto();
		body.ProcessInstanceIds = Arrays.asList(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID);

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID), null, null, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithoutQueryAndWithoutProcessInstanceIds()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithoutQueryAndWithoutProcessInstanceIds()
	  {
		doThrow(new BadUserRequestException("processInstanceIds is empty")).when(runtimeServiceMock).deleteProcessInstancesAsync(anyListOf(typeof(string)), eq((ProcessInstanceQuery) null), anyString(), anyBoolean(), anyBoolean());

		given().contentType(ContentType.JSON).body(new DeleteProcessInstancesDto()).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(new List<string>(), null, null, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithDeleteReason()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithDeleteReason()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		DeleteProcessInstancesDto body = new DeleteProcessInstancesDto();
		body.DeleteReason = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON;

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(new List<string>(), null, MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithSkipCustomListenerTrue()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithSkipCustomListenerTrue()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		DeleteProcessInstancesDto body = new DeleteProcessInstancesDto();
		body.SkipCustomListeners = true;

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(new List<string>(), null, null, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAsyncHistoricQueryBasedWithSkipSubprocesses()
	  public virtual void testDeleteAsyncHistoricQueryBasedWithSkipSubprocesses()
	  {
		when(runtimeServiceMock.deleteProcessInstancesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyString(), anyBoolean(), anyBoolean())).thenReturn(new BatchEntity());

		DeleteProcessInstancesDto body = new DeleteProcessInstancesDto();
		body.setSkipSubprocesses(true);

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(DELETE_PROCESS_INSTANCES_ASYNC_HIST_QUERY_URL);

		verify(runtimeServiceMock, times(1)).deleteProcessInstancesAsync(new List<string>(), null, null, false, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesWithNullValue()
	  public virtual void testGetVariablesWithNullValue()
	  {
		Response response = given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID_WITH_NULL_VALUE_AS_VARIABLE).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_ANOTHER_VARIABLE_KEY, notNullValue()).body(EXAMPLE_ANOTHER_VARIABLE_KEY + ".value", nullValue()).body(EXAMPLE_ANOTHER_VARIABLE_KEY + ".type", equalTo("Null")).when().get(PROCESS_INSTANCE_VARIABLES_URL);

		Assert.assertEquals("Should return exactly one variable", 1, response.jsonPath().getMap("").size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariable()
	  public virtual void testGetFileVariable()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		string mimeType = "text/plain";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(mimeType).create();

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), eq(true))).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON.ToString()).and().body("valueInfo.mimeType", equalTo(mimeType)).body("valueInfo.filename", equalTo(filename)).body("value", nullValue()).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNullFileVariable()
	  public virtual void testGetNullFileVariable()
	  {
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimeType = "text/plain";
		FileValue variableValue = Variables.fileValue(filename).mimeType(mimeType).create();

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(""))).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariableDownloadWithType()
	  public virtual void testGetFileVariableDownloadWithType()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).create();

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

	  given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariableDownloadWithTypeAndEncoding()
	  public virtual void testGetFileVariableDownloadWithTypeAndEncoding()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		string contentType = response.contentType().replaceAll(" ", "");
		assertThat(contentType, @is(ContentType.TEXT + ";charset=" + encoding));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFileVariableDownloadWithoutType()
	  public virtual void testGetFileVariableDownloadWithoutType()
	  {
		string variableKey = "aVariableKey";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		string filename = "test.txt";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).create();

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

	   given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).header("Content-Disposition", containsString(filename)).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotDownloadVariableOtherThanFile()
	  public virtual void testCannotDownloadVariableOtherThanFile()
	  {
		string variableKey = "aVariableKey";
		LongValue variableValue = Variables.longValue(123L);

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

	   given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(MediaType.APPLICATION_JSON).and().when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJavaObjectVariableSerialization()
	  public virtual void testJavaObjectVariableSerialization()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Response response = given().pathParam("id", MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_VARIABLE_KEY, notNullValue()).body(EXAMPLE_VARIABLE_KEY + ".value.property1", equalTo("aPropertyValue")).body(EXAMPLE_VARIABLE_KEY + ".value.property2", equalTo(true)).body(EXAMPLE_VARIABLE_KEY + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(ValueType.OBJECT))).body(EXAMPLE_VARIABLE_KEY + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(ExampleVariableObject).FullName)).body(EXAMPLE_VARIABLE_KEY + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).when().get(PROCESS_INSTANCE_VARIABLES_URL);

		Assert.assertEquals("Should return exactly one variable", 1, response.jsonPath().getMap("").size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetObjectVariables()
	  public virtual void testGetObjectVariables()
	  {
		// given
		string variableKey = "aVariableId";

		IList<string> payload = Arrays.asList("a", "b");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = MockObjectValue.fromObjectValue(Variables.objectValue(payload).serializationDataFormat("application/json").create()).objectTypeName(typeof(List<object>).FullName).serializedValue("a serialized value"); // this should differ from the serialized json

		when(runtimeServiceMock.getVariablesTyped(eq(EXAMPLE_PROCESS_INSTANCE_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo(payload)).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(PROCESS_INSTANCE_VARIABLES_URL);

		// then
		verify(runtimeServiceMock).getVariablesTyped(EXAMPLE_PROCESS_INSTANCE_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetObjectVariablesSerialized()
	  public virtual void testGetObjectVariablesSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(runtimeServiceMock.getVariablesTyped(eq(EXAMPLE_PROCESS_INSTANCE_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).queryParam("deserializeValues", false).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo("a serialized value")).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(PROCESS_INSTANCE_VARIABLES_URL);

		// then
		verify(runtimeServiceMock).getVariablesTyped(EXAMPLE_PROCESS_INSTANCE_ID, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesForNonExistingProcessInstance()
	  public virtual void testGetVariablesForNonExistingProcessInstance()
	  {
		when(runtimeServiceMock.getVariablesTyped(anyString(), anyBoolean())).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", "aNonExistingProcessInstanceId").then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().get(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesThrowsAuthorizationException()
	  public virtual void testGetVariablesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(runtimeServiceMock.getVariablesTyped(anyString(), anyBoolean())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleInstance()
	  public virtual void testGetSingleInstance()
	  {
		ProcessInstance mockInstance = MockProvider.createMockInstance();
		ProcessInstanceQuery sampleInstanceQuery = mock(typeof(ProcessInstanceQuery));
		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.processInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.singleResult()).thenReturn(mockInstance);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("ended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED)).body("definitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("businessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("suspended", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingProcessInstance()
	  public virtual void testGetNonExistingProcessInstance()
	  {
		ProcessInstanceQuery sampleInstanceQuery = mock(typeof(ProcessInstanceQuery));
		when(runtimeServiceMock.createProcessInstanceQuery()).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.processInstanceId(anyString())).thenReturn(sampleInstanceQuery);
		when(sampleInstanceQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingInstanceId").then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Process instance with id aNonExistingInstanceId does not exist")).when().get(SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstance()
	  public virtual void testDeleteProcessInstance()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingProcessInstance()
	  public virtual void testDeleteNonExistingProcessInstance()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).deleteProcessInstance(anyString(), anyString(), anyBoolean(), anyBoolean(), anyBoolean(),anyBoolean());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Process instance with id " + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID + " does not exist")).when().delete(SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingProcessInstanceIfExists()
	  public virtual void testDeleteNonExistingProcessInstanceIfExists()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParam("failIfNotExists", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstanceIfExists(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceThrowsAuthorizationException()
	  public virtual void testDeleteProcessInstanceThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).deleteProcessInstance(anyString(), anyString(), anyBoolean(), anyBoolean(), anyBoolean(), anyBoolean());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(SINGLE_PROCESS_INSTANCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoGivenDeleteReason1()
	  public virtual void testNoGivenDeleteReason1()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceSkipCustomListeners()
	  public virtual void testDeleteProcessInstanceSkipCustomListeners()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParams("skipCustomListeners", true).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, true, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceWithCustomListeners()
	  public virtual void testDeleteProcessInstanceWithCustomListeners()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParams("skipCustomListeners", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceSkipIoMappings()
	  public virtual void testDeleteProcessInstanceSkipIoMappings()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParams("skipIoMappings", true).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, true, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceWithoutSkipingIoMappings()
	  public virtual void testDeleteProcessInstanceWithoutSkipingIoMappings()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParams("skipIoMappings", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceSkipSubprocesses()
	  public virtual void testDeleteProcessInstanceSkipSubprocesses()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParams("skipSubprocesses", true).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessInstanceWithoutSkipSubprocesses()
	  public virtual void testDeleteProcessInstanceWithoutSkipSubprocesses()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).queryParams("skipSubprocesses", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_URL);

		verify(runtimeServiceMock).deleteProcessInstance(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, null, false, true, false, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModification()
	  public virtual void testVariableModification()
	  {
		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		IList<string> deletions = new List<string>();
		deletions.Add("deleteKey");
		messageBodyJson["deletions"] = deletions;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(PROCESS_INSTANCE_VARIABLES_URL);

		IDictionary<string, object> expectedModifications = new Dictionary<string, object>();
		expectedModifications[variableKey] = variableValue;
		verify(runtimeServiceMock).updateVariables(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), argThat(new EqualsMap(expectedModifications)), argThat(new EqualsList(deletions)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationWithUnparseableInteger()
	  public virtual void testVariableModificationWithUnparseableInteger()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Integer";

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot modify variables for process instance: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationWithUnparseableShort()
	  public virtual void testVariableModificationWithUnparseableShort()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Short";

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot modify variables for process instance: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationWithUnparseableLong()
	  public virtual void testVariableModificationWithUnparseableLong()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot modify variables for process instance: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationWithUnparseableDouble()
	  public virtual void testVariableModificationWithUnparseableDouble()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Double";

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot modify variables for process instance: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationWithUnparseableDate()
	  public virtual void testVariableModificationWithUnparseableDate()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot modify variables for process instance: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationWithNotSupportedType()
	  public virtual void testVariableModificationWithNotSupportedType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string variableType = "X";

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot modify variables for process instance: Unsupported value type 'X'")).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationForNonExistingProcessInstance()
	  public virtual void testVariableModificationForNonExistingProcessInstance()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).updateVariables(anyString(), anyMapOf(typeof(string), typeof(object)), anyCollectionOf(typeof(string)));

		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;

		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot modify variables for process instance " + MockProvider.EXAMPLE_PROCESS_INSTANCE_ID + ": expected exception")).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyVariableModification()
	  public virtual void testEmptyVariableModification()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationThrowsAuthorizationException()
	  public virtual void testVariableModificationThrowsAuthorizationException()
	  {
		string variableKey = "aKey";
		int variableValue = 123;
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		string message = "excpected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).updateVariables(anyString(), anyMapOf(typeof(string), typeof(object)), anyCollectionOf(typeof(string)));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().post(PROCESS_INSTANCE_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariable()
	  public virtual void testGetSingleVariable()
	  {
		string variableKey = "aVariableKey";
		int variableValue = 123;

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), eq(true))).thenReturn(Variables.integerValue(variableValue));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body("value", @is(123)).body("type", @is("Integer")).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingVariable()
	  public virtual void testNonExistingVariable()
	  {
		string variableKey = "aVariableKey";

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("process instance variable with name " + variableKey + " does not exist")).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableThrowsAuthorizationException()
	  public virtual void testGetSingleVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";

		string message = "excpected exception";
		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariableData()
	  public virtual void testGetSingleLocalVariableData()
	  {

		when(runtimeServiceMock.getVariableTyped(anyString(), eq(EXAMPLE_BYTES_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE_BYTES);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", EXAMPLE_BYTES_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).getVariableTyped(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, EXAMPLE_BYTES_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariableDataNonExisting()
	  public virtual void testGetSingleLocalVariableDataNonExisting()
	  {

		when(runtimeServiceMock.getVariableTyped(anyString(), eq("nonExisting"), eq(false))).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", "nonExisting").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("process instance variable with name " + "nonExisting" + " does not exist")).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).getVariableTyped(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, "nonExisting", false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariabledataNotBinary()
	  public virtual void testGetSingleLocalVariabledataNotBinary()
	  {

		when(runtimeServiceMock.getVariableTyped(anyString(), eq(EXAMPLE_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).getVariableTyped(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, EXAMPLE_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalObjectVariable()
	  public virtual void testGetSingleLocalObjectVariable()
	  {
		// given
		string variableKey = "aVariableId";

		IList<string> payload = Arrays.asList("a", "b");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = MockObjectValue.fromObjectValue(Variables.objectValue(payload).serializationDataFormat("application/json").create()).objectTypeName(typeof(List<object>).FullName).serializedValue("a serialized value"); // this should differ from the serialized json

		when(runtimeServiceMock.getVariableTyped(eq(EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo(payload)).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		// then
		verify(runtimeServiceMock).getVariableTyped(EXAMPLE_PROCESS_INSTANCE_ID, variableKey, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalObjectVariableSerialized()
	  public virtual void testGetSingleLocalObjectVariableSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(runtimeServiceMock.getVariableTyped(eq(EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo("a serialized value")).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		// then
		verify(runtimeServiceMock).getVariableTyped(EXAMPLE_PROCESS_INSTANCE_ID, variableKey, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableForNonExistingInstance()
	  public virtual void testGetVariableForNonExistingInstance()
	  {
		string variableKey = "aVariableKey";

		when(runtimeServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), eq(true))).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot get process instance variable " + variableKey + ": expected exception")).when().get(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariable()
	  public virtual void testPutSingleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsUntypedValue.matcher().value(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeString()
	  public virtual void testPutSingleVariableWithTypeString()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";
		string type = "String";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.stringValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeInteger()
	  public virtual void testPutSingleVariableWithTypeInteger()
	  {
		string variableKey = "aVariableKey";
		int? variableValue = 123;
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.integerValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableInteger()
	  public virtual void testPutSingleVariableWithUnparseableInteger()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Integer)))).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeShort()
	  public virtual void testPutSingleVariableWithTypeShort()
	  {
		string variableKey = "aVariableKey";
		short? variableValue = 123;
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.shortValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableShort()
	  public virtual void testPutSingleVariableWithUnparseableShort()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Short)))).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeLong()
	  public virtual void testPutSingleVariableWithTypeLong()
	  {
		string variableKey = "aVariableKey";
		long? variableValue = Convert.ToInt64(123);
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.longValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableLong()
	  public virtual void testPutSingleVariableWithUnparseableLong()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Long)))).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeDouble()
	  public virtual void testPutSingleVariableWithTypeDouble()
	  {
		string variableKey = "aVariableKey";
		double? variableValue = 123.456;
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.doubleValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDouble()
	  public virtual void testPutSingleVariableWithUnparseableDouble()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Double)))).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeBoolean()
	  public virtual void testPutSingleVariableWithTypeBoolean()
	  {
		string variableKey = "aVariableKey";
		bool? variableValue = true;
		string type = "Boolean";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.booleanValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeDate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleVariableWithTypeDate()
	  {
		DateTime now = DateTime.Now;

		string variableKey = "aVariableKey";
		string variableValue = DATE_FORMAT_WITH_TIMEZONE.format(now);
		string type = "Date";

		DateTime expectedValue = DATE_FORMAT_WITH_TIMEZONE.parse(variableValue);

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.dateValue(expectedValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDate()
	  public virtual void testPutSingleVariableWithUnparseableDate()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(DateTime)))).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNotSupportedType()
	  public virtual void testPutSingleVariableWithNotSupportedType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: Unsupported value type 'X'")).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableThrowsAuthorizationException()
	  public virtual void testPutSingleVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "String";
		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).setVariable(anyString(), anyString(), any());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleBinaryVariable()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariableWithValueType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleBinaryVariableWithValueType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "Bytes", "text/plain").expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariableWithNoValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleBinaryVariableWithNoValue()
	  {
		sbyte[] bytes = new sbyte[0];

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariableThrowsAuthorizationException()
	  public virtual void testPutSingleBinaryVariableThrowsAuthorizationException()
	  {
		sbyte[] bytes = "someContent".GetBytes();
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).setVariable(anyString(), anyString(), any());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).multiPart("data", "unspecified", bytes).expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleSerializableVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleSerializableVariable()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, MediaType.APPLICATION_JSON).multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().Deserialized.value(serializable)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleSerializableVariableUnsupportedMediaType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleSerializableVariableUnsupportedMediaType()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, "unsupported").multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Unrecognized content type for serialized java type: unsupported")).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock, never()).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), eq(serializable));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableFromSerialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleVariableFromSerialized()
	  {
		string serializedValue = "{\"prop\" : \"value\"}";
		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(serializedValue, ValueType.OBJECT.Name, "aDataFormat", "aRootType");

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().serializedValue(serializedValue).serializationFormat("aDataFormat").objectTypeName("aRootType")));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableFromInvalidSerialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleVariableFromInvalidSerialized()
	  {
		string serializedValue = "{\"prop\" : \"value\"}";

		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(serializedValue, "aNonExistingType", null, null);

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put process instance variable aVariableKey: Unsupported value type 'aNonExistingType'")).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableFromSerializedWithNoValue()
	  public virtual void testPutSingleVariableFromSerializedWithNoValue()
	  {
		string variableKey = "aVariableKey";
		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(null, ValueType.OBJECT.Name, null, null);

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNoValue()
	  public virtual void testPutSingleVariableWithNoValue()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), argThat(EqualsNullValue.matcher()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutVariableForNonExistingInstance()
	  public virtual void testPutVariableForNonExistingInstance()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey), any());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot put process instance variable " + variableKey + ": expected exception")).when().put(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleFileVariableWithEncodingAndMimeType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleFileVariableWithEncodingAndMimeType()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype + "; encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(encoding));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleFileVariableWithMimeType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleFileVariableWithMimeType()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleFileVariableWithEncoding() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleFileVariableWithEncoding()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, value, "encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleFileVariableOnlyFilename() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleFileVariableOnlyFilename()
	  {

		string variableKey = "aVariableKey";
		string filename = "test.txt";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, new sbyte[0]).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_PROCESS_INSTANCE_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(runtimeServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(MediaType.APPLICATION_OCTET_STREAM));
		assertThat(captured.Value.available(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleVariable()
	  public virtual void testDeleteSingleVariable()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);

		verify(runtimeServiceMock).removeVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteVariableForNonExistingInstance()
	  public virtual void testDeleteVariableForNonExistingInstance()
	  {
		string variableKey = "aVariableKey";

		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).removeVariable(eq(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID), eq(variableKey));

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot delete process instance variable " + variableKey + ": expected exception")).when().delete(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteVariableThrowsAuthorizationException()
	  public virtual void testDeleteVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).removeVariable(anyString(), anyString());

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().delete(SINGLE_PROCESS_INSTANCE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateInstance()
	  public virtual void testActivateInstance()
	  {
		ProcessInstanceSuspensionStateDto dto = new ProcessInstanceSuspensionStateDto();
		dto.Suspended = false;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		verify(mockUpdateSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowsProcessEngineException()
	  public virtual void testActivateThrowsProcessEngineException()
	  {
		ProcessInstanceSuspensionStateDto dto = new ProcessInstanceSuspensionStateDto();
		dto.Suspended = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockUpdateSuspensionStateBuilder).activate();

		given().pathParam("id", MockProvider.EXAMPLE_NON_EXISTENT_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowsAuthorizationException()
	  public virtual void testActivateThrowsAuthorizationException()
	  {
		ProcessInstanceSuspensionStateDto dto = new ProcessInstanceSuspensionStateDto();
		dto.Suspended = false;

		string message = "expectedMessage";

		doThrow(new AuthorizationException(message)).when(mockUpdateSuspensionStateBuilder).activate();

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendInstance()
	  public virtual void testSuspendInstance()
	  {
		ProcessInstanceSuspensionStateDto dto = new ProcessInstanceSuspensionStateDto();
		dto.Suspended = true;

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		verify(mockUpdateSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendThrowsProcessEngineException()
	  public virtual void testSuspendThrowsProcessEngineException()
	  {
		ProcessInstanceSuspensionStateDto dto = new ProcessInstanceSuspensionStateDto();
		dto.Suspended = true;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockUpdateSuspensionStateBuilder).suspend();

		given().pathParam("id", MockProvider.EXAMPLE_NON_EXISTENT_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendWithMultipleByParameters()
	  public virtual void testSuspendWithMultipleByParameters()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "Only one of processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendThrowsAuthorizationException()
	  public virtual void testSuspendThrowsAuthorizationException()
	  {
		ProcessInstanceSuspensionStateDto dto = new ProcessInstanceSuspensionStateDto();
		dto.Suspended = true;

		string message = "expectedMessage";

		doThrow(new AuthorizationException(message)).when(mockUpdateSuspensionStateBuilder).suspend();

		given().pathParam("id", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(SINGLE_PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionKey()
	  public virtual void testActivateProcessInstanceByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockUpdateSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionKeyWithException()
	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockUpdateSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionKeyThrowsAuthorizationException()
	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "expectedMessage";

		doThrow(new AuthorizationException(message)).when(mockUpdateSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionKey()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockUpdateSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionKeyWithException()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockUpdateSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionKeyThrowsAuthorizationException()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "expectedMessage";

		doThrow(new AuthorizationException(message)).when(mockUpdateSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionKeyAndTenantId()
	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyAndTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionTenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockUpdateSuspensionStateBuilder).processDefinitionTenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockUpdateSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionKeyWithoutTenantId()
	  public virtual void testActivateProcessInstanceByProcessDefinitionKeyWithoutTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockUpdateSuspensionStateBuilder).processDefinitionWithoutTenantId();
		verify(mockUpdateSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionKeyAndTenantId()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyAndTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionTenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockUpdateSuspensionStateBuilder).processDefinitionTenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockUpdateSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionKeyWithoutTenantId()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionKeyWithoutTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockUpdateSuspensionStateBuilder).processDefinitionWithoutTenantId();
		verify(mockUpdateSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionId()
	  public virtual void testActivateProcessInstanceByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockUpdateSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionIdWithException()
	  public virtual void testActivateProcessInstanceByProcessDefinitionIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockUpdateSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByProcessDefinitionIdThrowsAuthorizationException()
	  public virtual void testActivateProcessInstanceByProcessDefinitionIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string message = "expectedMessage";

		doThrow(new AuthorizationException(message)).when(mockUpdateSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionId()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockUpdateSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionIdWithException()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockUpdateSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByProcessDefinitionIdThrowsAuthorizationException()
	  public virtual void testSuspendProcessInstanceByProcessDefinitionIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string message = "expectedMessage";

		doThrow(new AuthorizationException(message)).when(mockUpdateSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceByIdShouldThrowException()
	  public virtual void testActivateProcessInstanceByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		string message = "Either processDefinitionId or processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByIdShouldThrowException()
	  public virtual void testSuspendProcessInstanceByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		string message = "Either processDefinitionId or processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceByNothing()
	  public virtual void testSuspendProcessInstanceByNothing()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "Either processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(PROCESS_INSTANCE_SUSPENDED_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendInstances()
	  public virtual void testSuspendInstances()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["suspended"] = true;
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspend();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateInstances()
	  public virtual void testActivateInstances()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["suspended"] = false;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendInstancesMultipleGroupOperations()
	  public virtual void testSuspendInstancesMultipleGroupOperations()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["processInstanceQuery"] = query;
		messageBodyJson["suspended"] = true;


		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).byProcessInstanceQuery(query.toQuery(processEngine));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspend();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendProcessInstanceQuery()
	  public virtual void testSuspendProcessInstanceQuery()
	  {
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceQuery"] = query;
		messageBodyJson["suspended"] = true;
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceQuery(query.toQuery(processEngine));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspend();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateProcessInstanceQuery()
	  public virtual void testActivateProcessInstanceQuery()
	  {
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceQuery"] = query;
		messageBodyJson["suspended"] = false;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceQuery(query.toQuery(processEngine));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).activate();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendHistoricProcessInstanceQuery()
	  public virtual void testSuspendHistoricProcessInstanceQuery()
	  {
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["historicProcessInstanceQuery"] = query;
		messageBodyJson["suspended"] = true;
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byHistoricProcessInstanceQuery(any(typeof(HistoricProcessInstanceQuery)));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspend();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateHistoricProcessInstanceQuery()
	  public virtual void testActivateHistoricProcessInstanceQuery()
	  {
		HistoricProcessInstanceDto query = new HistoricProcessInstanceDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["historicProcessInstanceQuery"] = query;
		messageBodyJson["suspended"] = false;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(PROCESS_INSTANCE_SUSPENDED_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byHistoricProcessInstanceQuery(any(typeof(HistoricProcessInstanceQuery)));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendAsyncWithProcessInstances()
	  public virtual void testSuspendAsyncWithProcessInstances()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["suspended"] = true;

		when(mockUpdateProcessInstancesSuspensionStateBuilder.suspendAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspendAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateAsyncWithProcessInstances()
	  public virtual void testActivateAsyncWithProcessInstances()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["suspended"] = false;

		when(mockUpdateProcessInstancesSuspensionStateBuilder.activateAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).activateAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendAsyncWithProcessInstanceQuery()
	  public virtual void testSuspendAsyncWithProcessInstanceQuery()
	  {
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceQuery"] = query;
		messageBodyJson["suspended"] = true;


		when(mockUpdateProcessInstancesSuspensionStateBuilder.suspendAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceQuery(query.toQuery(processEngine));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspendAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateAsyncWithProcessInstanceQuery()
	  public virtual void testActivateAsyncWithProcessInstanceQuery()
	  {
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceQuery"] = query;
		messageBodyJson["suspended"] = false;

		when(mockUpdateProcessInstancesSuspensionStateBuilder.activateAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceQuery(query.toQuery(processEngine));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).activateAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendAsyncWithHistoricProcessInstanceQuery()
	  public virtual void testSuspendAsyncWithHistoricProcessInstanceQuery()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["suspended"] = true;

		when(mockUpdateProcessInstancesSuspensionStateBuilder.suspendAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspendAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateAsyncWithHistoricProcessInstanceQuery()
	  public virtual void testActivateAsyncWithHistoricProcessInstanceQuery()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["suspended"] = false;

		when(mockUpdateProcessInstancesSuspensionStateBuilder.activateAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).activateAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendAsyncWithMultipleGroupOperations()
	  public virtual void testSuspendAsyncWithMultipleGroupOperations()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		ProcessInstanceQueryDto query = new ProcessInstanceQueryDto();
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstanceIds"] = ids;
		messageBodyJson["processInstanceQuery"] = query;
		messageBodyJson["suspended"] = true;

		when(mockUpdateProcessInstancesSuspensionStateBuilder.suspendAsync()).thenReturn(new BatchEntity());
		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

		verify(mockUpdateSuspensionStateSelectBuilder).byProcessInstanceIds(ids);
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).byProcessInstanceQuery(query.toQuery(processEngine));
		verify(mockUpdateProcessInstancesSuspensionStateBuilder).suspendAsync();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendAsyncWithNothing()
	  public virtual void testSuspendAsyncWithNothing()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["suspended"] = true;

		string message = "Either processInstanceIds, processInstanceQuery or historicProcessInstanceQuery should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().post(PROCESS_INSTANCE_SUSPENDED_ASYNC_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceModification()
	  public virtual void testProcessInstanceModification()
	  {
		ProcessInstanceModificationInstantiationBuilder mockModificationBuilder = setUpMockModificationBuilder();
		when(runtimeServiceMock.createProcessInstanceModification(anyString())).thenReturn(mockModificationBuilder);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["skipCustomListeners"] = true;
		json["skipIoMappings"] = true;

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.cancellation().activityInstanceId("activityInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.cancellation().transitionInstanceId("transitionInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").ancestorActivityInstanceId("ancestorActivityInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").ancestorActivityInstanceId("ancestorActivityInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").ancestorActivityInstanceId("ancestorActivityInstanceId").Json);

		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

		verify(runtimeServiceMock).createProcessInstanceModification(eq(EXAMPLE_PROCESS_INSTANCE_ID));

		InOrder inOrder = inOrder(mockModificationBuilder);
		inOrder.verify(mockModificationBuilder).cancelAllForActivity("activityId");
		inOrder.verify(mockModificationBuilder).cancelActivityInstance("activityInstanceId");
		inOrder.verify(mockModificationBuilder).cancelTransitionInstance("transitionInstanceId");
		inOrder.verify(mockModificationBuilder).startBeforeActivity("activityId");
		inOrder.verify(mockModificationBuilder).startBeforeActivity("activityId", "ancestorActivityInstanceId");
		inOrder.verify(mockModificationBuilder).startAfterActivity("activityId");
		inOrder.verify(mockModificationBuilder).startAfterActivity("activityId", "ancestorActivityInstanceId");
		inOrder.verify(mockModificationBuilder).startTransition("transitionId");
		inOrder.verify(mockModificationBuilder).startTransition("transitionId", "ancestorActivityInstanceId");

		inOrder.verify(mockModificationBuilder).execute(true, true);

		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceModificationWithVariables()
	  public virtual void testProcessInstanceModificationWithVariables()
	  {
		ProcessInstanceModificationInstantiationBuilder mockModificationBuilder = setUpMockModificationBuilder();
		when(runtimeServiceMock.createProcessInstanceModification(anyString())).thenReturn(mockModificationBuilder);

		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").variables(VariablesBuilder.create().variable("var", "value", "String", false).variable("varLocal", "valueLocal", "String", true).Variables).Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").variables(VariablesBuilder.create().variable("var", 52, "Integer", false).variable("varLocal", 74, "Integer", true).Variables).Json);

		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

		verify(runtimeServiceMock).createProcessInstanceModification(eq(EXAMPLE_PROCESS_INSTANCE_ID));

		InOrder inOrder = inOrder(mockModificationBuilder);
		inOrder.verify(mockModificationBuilder).startBeforeActivity("activityId");

		verify(mockModificationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.stringValue("valueLocal")));
		verify(mockModificationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.stringValue("value")));

		inOrder.verify(mockModificationBuilder).startAfterActivity("activityId");

		verify(mockModificationBuilder).setVariable(eq("var"), argThat(EqualsPrimitiveValue.integerValue(52)));
		verify(mockModificationBuilder).setVariableLocal(eq("varLocal"), argThat(EqualsPrimitiveValue.integerValue(74)));

		inOrder.verify(mockModificationBuilder).execute(false, false);

		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidModification()
	  public virtual void testInvalidModification()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		// start before: missing activity id
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'activityId' must be set")).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

		// start after: missing ancestor activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'activityId' must be set")).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

		// start transition: missing ancestor activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'transitionId' must be set")).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

		// cancel: missing activity id and activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.cancellation().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("For instruction type 'cancel': exactly one, " + "'activityId', 'activityInstanceId', or 'transitionInstanceId', is required")).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

		// cancel: both, activity id and activity instance id, set
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("anActivityId").activityInstanceId("anActivityInstanceId").Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("For instruction type 'cancel': exactly one, " + "'activityId', 'activityInstanceId', or 'transitionInstanceId', is required")).when().post(PROCESS_INSTANCE_MODIFICATION_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testModifyProcessInstanceThrowsAuthorizationException()
	  public virtual void testModifyProcessInstanceThrowsAuthorizationException()
	  {
		ProcessInstanceModificationInstantiationBuilder mockModificationBuilder = setUpMockModificationBuilder();
		when(runtimeServiceMock.createProcessInstanceModification(anyString())).thenReturn(mockModificationBuilder);

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockModificationBuilder).execute(anyBoolean(), anyBoolean());

		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").variables(VariablesBuilder.create().variable("var", "value", "String", false).variable("varLocal", "valueLocal", "String", true).Variables).Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").variables(VariablesBuilder.create().variable("var", 52, "Integer", false).variable("varLocal", 74, "Integer", true).Variables).Json);

		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(PROCESS_INSTANCE_MODIFICATION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsync()
	  public virtual void testSetRetriesByProcessAsync()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyInt())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstances"] = ids;
		messageBodyJson[RETRIES] = 5;

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq(ids), eq((ProcessInstanceQuery) null), eq(5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsyncWithQuery()
	  public virtual void testSetRetriesByProcessAsyncWithQuery()
	  {
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), anyInt())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[RETRIES] = 5;
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		messageBodyJson["processInstanceQuery"] = query;

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_URL);

		verifyBatchJson(response.asString());

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq((IList<string>) null), any(typeof(ProcessInstanceQuery)), Mockito.eq(5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessWithBadRequestQuery()
	  public virtual void testSetRetriesByProcessWithBadRequestQuery()
	  {
		doThrow(new BadUserRequestException("job ids are empty")).when(mockManagementService).setJobRetriesAsync(eq((IList<string>) null), eq((ProcessInstanceQuery) null), anyInt());

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[RETRIES] = 5;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessWithoutRetries()
	  public virtual void testSetRetriesByProcessWithoutRetries()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["processInstances"] = null;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessWithNegativeRetries()
	  public virtual void testSetRetriesByProcessWithNegativeRetries()
	  {
		doThrow(new BadUserRequestException("retries are negative")).when(mockManagementService).setJobRetriesAsync(anyListOf(typeof(string)), any(typeof(ProcessInstanceQuery)), eq(-1));

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[RETRIES] = -1;
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		messageBodyJson["processInstanceQuery"] = query;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsyncHistoricQueryBasedWithQuery()
	  public virtual void testSetRetriesByProcessAsyncHistoricQueryBasedWithQuery()
	  {
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), eq((ProcessInstanceQuery) null), anyInt())).thenReturn(batchEntity);

		HistoricProcessInstanceQuery mockedHistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(mockedHistoricProcessInstanceQuery);
		IList<HistoricProcessInstance> historicProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		when(mockedHistoricProcessInstanceQuery.list()).thenReturn(historicProcessInstances);

		SetJobRetriesByProcessDto body = new SetJobRetriesByProcessDto();
		body.Retries = MockProvider.EXAMPLE_JOB_RETRIES;
		body.HistoricProcessInstanceQuery = new HistoricProcessInstanceQueryDto();

		Response response = given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_HIST_QUERY_URL);

		verifyBatchJson(response.asString());

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq(Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)), eq((ProcessInstanceQuery) null), eq(MockProvider.EXAMPLE_JOB_RETRIES));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsyncHistoricQueryBasedWithProcessInstanceIds()
	  public virtual void testSetRetriesByProcessAsyncHistoricQueryBasedWithProcessInstanceIds()
	  {
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), eq((ProcessInstanceQuery) null), anyInt())).thenReturn(batchEntity);

		SetJobRetriesByProcessDto body = new SetJobRetriesByProcessDto();
		body.Retries = MockProvider.EXAMPLE_JOB_RETRIES;
		body.ProcessInstances = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_HIST_QUERY_URL);

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq(Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)), eq((ProcessInstanceQuery) null), eq(MockProvider.EXAMPLE_JOB_RETRIES));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsyncHistoricQueryBasedWithQueryAndProcessInstanceIds()
	  public virtual void testSetRetriesByProcessAsyncHistoricQueryBasedWithQueryAndProcessInstanceIds()
	  {
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), eq((ProcessInstanceQuery) null), anyInt())).thenReturn(batchEntity);

		HistoricProcessInstanceQuery mockedHistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(mockedHistoricProcessInstanceQuery);
		IList<HistoricProcessInstance> historicProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		when(mockedHistoricProcessInstanceQuery.list()).thenReturn(historicProcessInstances);

		SetJobRetriesByProcessDto body = new SetJobRetriesByProcessDto();
		body.Retries = MockProvider.EXAMPLE_JOB_RETRIES;
		body.ProcessInstances = Arrays.asList(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID);
		body.HistoricProcessInstanceQuery = new HistoricProcessInstanceQueryDto();

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_HIST_QUERY_URL);

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq(Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID)), eq((ProcessInstanceQuery) null), eq(MockProvider.EXAMPLE_JOB_RETRIES));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsyncHistoricQueryBasedWithBadRequestQuery()
	  public virtual void testSetRetriesByProcessAsyncHistoricQueryBasedWithBadRequestQuery()
	  {
		doThrow(new BadUserRequestException("jobIds is empty")).when(mockManagementService).setJobRetriesAsync(eq(new List<string>()), eq((ProcessInstanceQuery) null), anyInt());

		SetJobRetriesByProcessDto body = new SetJobRetriesByProcessDto();
		body.Retries = MockProvider.EXAMPLE_JOB_RETRIES;

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_HIST_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesByProcessAsyncHistoricQueryBasedWithNegativeRetries()
	  public virtual void testSetRetriesByProcessAsyncHistoricQueryBasedWithNegativeRetries()
	  {
		doThrow(new BadUserRequestException("retries are negative")).when(mockManagementService).setJobRetriesAsync(anyListOf(typeof(string)), eq((ProcessInstanceQuery) null), eq(MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES));

		HistoricProcessInstanceQuery mockedHistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(historyServiceMock.createHistoricProcessInstanceQuery()).thenReturn(mockedHistoricProcessInstanceQuery);
		IList<HistoricProcessInstance> historicProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		when(mockedHistoricProcessInstanceQuery.list()).thenReturn(historicProcessInstances);

		SetJobRetriesByProcessDto body = new SetJobRetriesByProcessDto();
		body.Retries = MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES;
		body.HistoricProcessInstanceQuery = new HistoricProcessInstanceQueryDto();

		given().contentType(ContentType.JSON).body(body).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SET_JOB_RETRIES_ASYNC_HIST_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceModificationAsync()
	  public virtual void testProcessInstanceModificationAsync()
	  {
		ProcessInstanceModificationInstantiationBuilder mockModificationBuilder = setUpMockModificationBuilder();
		when(runtimeServiceMock.createProcessInstanceModification(anyString())).thenReturn(mockModificationBuilder);
		Batch batchMock = createMockBatch();
		when(mockModificationBuilder.executeAsync(anyBoolean(), anyBoolean())).thenReturn(batchMock);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["skipCustomListeners"] = true;
		json["skipIoMappings"] = true;

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.cancellation().activityInstanceId("activityInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.cancellation().transitionInstanceId("transitionInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").ancestorActivityInstanceId("ancestorActivityInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").ancestorActivityInstanceId("ancestorActivityInstanceId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").Json);
		instructions.Add(ModificationInstructionBuilder.startTransition().transitionId("transitionId").ancestorActivityInstanceId("ancestorActivityInstanceId").Json);

		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);

		verify(runtimeServiceMock).createProcessInstanceModification(eq(EXAMPLE_PROCESS_INSTANCE_ID));

		InOrder inOrder = inOrder(mockModificationBuilder);
		inOrder.verify(mockModificationBuilder).cancelAllForActivity("activityId");
		inOrder.verify(mockModificationBuilder).cancelActivityInstance("activityInstanceId");
		inOrder.verify(mockModificationBuilder).cancelTransitionInstance("transitionInstanceId");
		inOrder.verify(mockModificationBuilder).startBeforeActivity("activityId");
		inOrder.verify(mockModificationBuilder).startBeforeActivity("activityId", "ancestorActivityInstanceId");
		inOrder.verify(mockModificationBuilder).startAfterActivity("activityId");
		inOrder.verify(mockModificationBuilder).startAfterActivity("activityId", "ancestorActivityInstanceId");
		inOrder.verify(mockModificationBuilder).startTransition("transitionId");
		inOrder.verify(mockModificationBuilder).startTransition("transitionId", "ancestorActivityInstanceId");

		inOrder.verify(mockModificationBuilder).executeAsync(true, true);

		inOrder.verifyNoMoreInteractions();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidModificationAsync()
	  public virtual void testInvalidModificationAsync()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		// start before: missing activity id
		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startBefore().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'activityId' must be set")).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);

		// start after: missing ancestor activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startAfter().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'activityId' must be set")).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);

		// start transition: missing ancestor activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.startTransition().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("'transitionId' must be set")).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);

		// cancel: missing activity id and activity instance id
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.cancellation().Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("For instruction type 'cancel': exactly one, " + "'activityId', 'activityInstanceId', or 'transitionInstanceId', is required")).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);

		// cancel: both, activity id and activity instance id, set
		instructions = new List<IDictionary<string, object>>();
		instructions.Add(ModificationInstructionBuilder.cancellation().activityId("anActivityId").activityInstanceId("anActivityInstanceId").Json);
		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", containsString("For instruction type 'cancel': exactly one, " + "'activityId', 'activityInstanceId', or 'transitionInstanceId', is required")).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testModifyProcessInstanceAsyncThrowsAuthorizationException()
	  public virtual void testModifyProcessInstanceAsyncThrowsAuthorizationException()
	  {
		ProcessInstanceModificationInstantiationBuilder mockModificationBuilder = setUpMockModificationBuilder();
		when(runtimeServiceMock.createProcessInstanceModification(anyString())).thenReturn(mockModificationBuilder);

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockModificationBuilder).executeAsync(anyBoolean(), anyBoolean());

		IDictionary<string, object> json = new Dictionary<string, object>();

		IList<IDictionary<string, object>> instructions = new List<IDictionary<string, object>>();

		instructions.Add(ModificationInstructionBuilder.startBefore().activityId("activityId").Json);
		instructions.Add(ModificationInstructionBuilder.startAfter().activityId("activityId").Json);

		json["instructions"] = instructions;

		given().pathParam("id", EXAMPLE_PROCESS_INSTANCE_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(PROCESS_INSTANCE_MODIFICATION_ASYNC_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.camunda.bpm.engine.runtime.ProcessInstanceModificationInstantiationBuilder setUpMockModificationBuilder()
	  protected internal virtual ProcessInstanceModificationInstantiationBuilder setUpMockModificationBuilder()
	  {
		ProcessInstanceModificationInstantiationBuilder mockModificationBuilder = mock(typeof(ProcessInstanceModificationInstantiationBuilder));

		when(mockModificationBuilder.cancelActivityInstance(anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.cancelAllForActivity(anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.startAfterActivity(anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.startAfterActivity(anyString(), anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.startBeforeActivity(anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.startBeforeActivity(anyString(), anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.startTransition(anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.startTransition(anyString(), anyString())).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.setVariables(any(typeof(System.Collections.IDictionary)))).thenReturn(mockModificationBuilder);
		when(mockModificationBuilder.setVariablesLocal(any(typeof(System.Collections.IDictionary)))).thenReturn(mockModificationBuilder);

		return mockModificationBuilder;

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