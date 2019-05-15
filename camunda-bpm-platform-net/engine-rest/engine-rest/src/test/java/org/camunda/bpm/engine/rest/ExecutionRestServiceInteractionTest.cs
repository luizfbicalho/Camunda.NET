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
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.DATE_FORMAT_WITH_TIMEZONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EqualsList = org.camunda.bpm.engine.rest.helper.EqualsList;
	using EqualsMap = org.camunda.bpm.engine.rest.helper.EqualsMap;
	using ErrorMessageHelper = org.camunda.bpm.engine.rest.helper.ErrorMessageHelper;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsNullValue = org.camunda.bpm.engine.rest.helper.variable.EqualsNullValue;
	using EqualsObjectValue = org.camunda.bpm.engine.rest.helper.variable.EqualsObjectValue;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EqualsUntypedValue = org.camunda.bpm.engine.rest.helper.variable.EqualsUntypedValue;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using TypeFactory = com.fasterxml.jackson.databind.type.TypeFactory;
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class ExecutionRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string EXECUTION_URL = TEST_RESOURCE_ROOT_PATH + "/execution/{id}";
	  protected internal static readonly string SIGNAL_EXECUTION_URL = EXECUTION_URL + "/signal";
	  protected internal static readonly string EXECUTION_LOCAL_VARIABLES_URL = EXECUTION_URL + "/localVariables";
	  protected internal static readonly string SINGLE_EXECUTION_LOCAL_VARIABLE_URL = EXECUTION_LOCAL_VARIABLES_URL + "/{varId}";
	  protected internal static readonly string SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL = SINGLE_EXECUTION_LOCAL_VARIABLE_URL + "/data";
	  protected internal static readonly string MESSAGE_SUBSCRIPTION_URL = EXECUTION_URL + "/messageSubscriptions/{messageName}";
	  protected internal static readonly string TRIGGER_MESSAGE_SUBSCRIPTION_URL = EXECUTION_URL + "/messageSubscriptions/{messageName}/trigger";
	  protected internal static readonly string CREATE_INCIDENT_URL = EXECUTION_URL + "/create-incident";

	  private RuntimeServiceImpl runtimeServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeServiceImpl));
		when(runtimeServiceMock.getVariablesLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, true)).thenReturn(EXAMPLE_VARIABLES);
		mockEventSubscriptionQuery();

		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);
	  }

	  private void mockEventSubscriptionQuery()
	  {
		EventSubscription mockSubscription = MockProvider.createMockEventSubscription();
		EventSubscriptionQuery mockQuery = mock(typeof(EventSubscriptionQuery));
		when(runtimeServiceMock.createEventSubscriptionQuery()).thenReturn(mockQuery);
		when(mockQuery.executionId(eq(MockProvider.EXAMPLE_EXECUTION_ID))).thenReturn(mockQuery);
		when(mockQuery.eventType(eq(MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_TYPE))).thenReturn(mockQuery);
		when(mockQuery.eventName(eq(MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_NAME))).thenReturn(mockQuery);
		when(mockQuery.singleResult()).thenReturn(mockSubscription);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleExecution()
	  public virtual void testGetSingleExecution()
	  {
		Execution mockExecution = MockProvider.createMockExecution();
		ExecutionQuery sampleExecutionQuery = mock(typeof(ExecutionQuery));
		when(runtimeServiceMock.createExecutionQuery()).thenReturn(sampleExecutionQuery);
		when(sampleExecutionQuery.executionId(MockProvider.EXAMPLE_EXECUTION_ID)).thenReturn(sampleExecutionQuery);
		when(sampleExecutionQuery.singleResult()).thenReturn(mockExecution);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("ended", equalTo(MockProvider.EXAMPLE_EXECUTION_IS_ENDED)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingExecution()
	  public virtual void testGetNonExistingExecution()
	  {
		ExecutionQuery sampleExecutionQuery = mock(typeof(ExecutionQuery));
		when(runtimeServiceMock.createExecutionQuery()).thenReturn(sampleExecutionQuery);
		when(sampleExecutionQuery.executionId(anyString())).thenReturn(sampleExecutionQuery);
		when(sampleExecutionQuery.singleResult()).thenReturn(null);

		string nonExistingExecutionId = "aNonExistingInstanceId";

		given().pathParam("id", nonExistingExecutionId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Execution with id " + nonExistingExecutionId + " does not exist")).when().get(EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalExecution()
	  public virtual void testSignalExecution()
	  {
		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SIGNAL_EXECUTION_URL);

		IDictionary<string, object> expectedSignalVariables = new Dictionary<string, object>();
		expectedSignalVariables[variableKey] = variableValue;

		verify(runtimeServiceMock).signal(eq(MockProvider.EXAMPLE_EXECUTION_ID), argThat(new EqualsMap(expectedSignalVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithUnparseableIntegerVariable()
	  public virtual void testSignalWithUnparseableIntegerVariable()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Integer";


		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot signal execution anExecutionId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Integer)))).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithUnparseableShortVariable()
	  public virtual void testSignalWithUnparseableShortVariable()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Short";


		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot signal execution anExecutionId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Short)))).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithUnparseableLongVariable()
	  public virtual void testSignalWithUnparseableLongVariable()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Long";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot signal execution anExecutionId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Long)))).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithUnparseableDoubleVariable()
	  public virtual void testSignalWithUnparseableDoubleVariable()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Double";


		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot signal execution anExecutionId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(Double)))).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithUnparseableDateVariable()
	  public virtual void testSignalWithUnparseableDateVariable()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "Date";

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot signal execution anExecutionId: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, variableType, typeof(DateTime)))).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithNotSupportedVariableType()
	  public virtual void testSignalWithNotSupportedVariableType()
	  {
		string variableKey = "aKey";
		string variableValue = "1abc";
		string variableType = "X";


		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey, variableValue, variableType).Variables;
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot signal execution anExecutionId: Unsupported value type 'X'")).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalNonExistingExecution()
	  public virtual void testSignalNonExistingExecution()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).signal(anyString(), any(typeof(System.Collections.IDictionary)));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot signal execution " + MockProvider.EXAMPLE_EXECUTION_ID + ": expected exception")).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalThrowsAuthorizationException()
	  public virtual void testSignalThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).signal(anyString(), any(typeof(System.Collections.IDictionary)));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SIGNAL_EXECUTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariables()
	  public virtual void testGetLocalVariables()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_VARIABLE_KEY, notNullValue()).body(EXAMPLE_VARIABLE_KEY + ".value", equalTo(EXAMPLE_VARIABLE_VALUE.Value)).body(EXAMPLE_VARIABLE_KEY + ".type", equalTo(typeof(string).Name)).when().get(EXECUTION_LOCAL_VARIABLES_URL);

		Assert.assertEquals("Should return exactly one variable", 1, response.jsonPath().getMap("").size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariablesForNonExistingExecution()
	  public virtual void testGetLocalVariablesForNonExistingExecution()
	  {
		when(runtimeServiceMock.getVariablesLocalTyped(anyString(), eq(true))).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", "aNonExistingExecutionId").then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().get(EXECUTION_LOCAL_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalObjectVariables()
	  public virtual void testGetLocalObjectVariables()
	  {
		// given
		string variableKey = "aVariableId";

		IList<string> payload = Arrays.asList("a", "b");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = MockObjectValue.fromObjectValue(Variables.objectValue(payload).serializationDataFormat("application/json").create()).objectTypeName(typeof(List<object>).FullName).serializedValue("a serialized value"); // this should differ from the serialized json

		when(runtimeServiceMock.getVariablesLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo(payload)).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(EXECUTION_LOCAL_VARIABLES_URL);

		// then
		verify(runtimeServiceMock).getVariablesLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalObjectVariablesSerialized()
	  public virtual void testGetLocalObjectVariablesSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(runtimeServiceMock.getVariablesLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).queryParam("deserializeValues", false).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo("a serialized value")).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(EXECUTION_LOCAL_VARIABLES_URL);

		// then
		verify(runtimeServiceMock).getVariablesLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariablesThrowsAuthorizationException()
	  public virtual void testGetLocalVariablesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).getVariablesLocalTyped(anyString(), anyBoolean());

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(EXECUTION_LOCAL_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLocalVariableModification()
	  public virtual void testLocalVariableModification()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		IList<string> deletions = new List<string>();
		deletions.Add("deleteKey");
		messageBodyJson["deletions"] = deletions;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTION_LOCAL_VARIABLES_URL);

		IDictionary<string, object> expectedModifications = new Dictionary<string, object>();
		expectedModifications[variableKey] = variableValue;
		verify(runtimeServiceMock).updateVariablesLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), argThat(new EqualsMap(expectedModifications)), argThat(new EqualsList(deletions)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLocalVariableModificationForNonExistingExecution()
	  public virtual void testLocalVariableModificationForNonExistingExecution()
	  {
		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).updateVariablesLocal(anyString(), any(typeof(System.Collections.IDictionary)), any(typeof(System.Collections.IList)));

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;
		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot modify variables for execution " + MockProvider.EXAMPLE_EXECUTION_ID + ": expected exception")).when().post(EXECUTION_LOCAL_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyLocalVariableModification()
	  public virtual void testEmptyLocalVariableModification()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(EXECUTION_LOCAL_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLocalVariableModificationThrowsAuthorizationException()
	  public virtual void testLocalVariableModificationThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).updateVariablesLocal(anyString(), any(typeof(System.Collections.IDictionary)), any(typeof(System.Collections.IList)));

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;
		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(EXECUTION_LOCAL_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariable()
	  public virtual void testGetSingleLocalVariable()
	  {
		string variableKey = "aVariableKey";
		int variableValue = 123;

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(Variables.integerValue(variableValue));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body("value", @is(123)).body("type", @is("Integer")).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariableData()
	  public virtual void testGetSingleLocalVariableData()
	  {

		when(runtimeServiceMock.getVariableLocalTyped(anyString(), eq(EXAMPLE_BYTES_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE_BYTES);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", EXAMPLE_BYTES_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, EXAMPLE_BYTES_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariableDataNonExisting()
	  public virtual void testGetSingleLocalVariableDataNonExisting()
	  {

		when(runtimeServiceMock.getVariableLocalTyped(anyString(), eq("nonExisting"), eq(false))).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", "nonExisting").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("execution variable with name " + "nonExisting" + " does not exist")).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, "nonExisting", false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalVariabledataNotBinary()
	  public virtual void testGetSingleLocalVariabledataNotBinary()
	  {

		when(runtimeServiceMock.getVariableLocalTyped(anyString(), eq(EXAMPLE_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, EXAMPLE_VARIABLE_KEY, false);
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

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo(payload)).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		// then
		verify(runtimeServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, variableKey, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleLocalObjectVariableSerialized()
	  public virtual void testGetSingleLocalObjectVariableSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo("a serialized value")).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		// then
		verify(runtimeServiceMock).getVariableLocalTyped(MockProvider.EXAMPLE_EXECUTION_ID, variableKey, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingLocalVariable()
	  public virtual void testNonExistingLocalVariable()
	  {
		string variableKey = "aVariableKey";

		when(runtimeServiceMock.getVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey))).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("execution variable with name " + variableKey + " does not exist")).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariableForNonExistingExecution()
	  public virtual void testGetLocalVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), eq(true))).thenThrow(new ProcessEngineException("expected exception"));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot get execution variable " + variableKey + ": expected exception")).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetLocalVariableThrowsAuthorizationException()
	  public virtual void testGetLocalVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).getVariableLocalTyped(anyString(), anyString(), anyBoolean());

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
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

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON.ToString()).and().body("valueInfo.mimeType", equalTo(mimeType)).body("valueInfo.filename", equalTo(filename)).body("value", nullValue()).when().get(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNullFileVariable()
	  public virtual void testGetNullFileVariable()
	  {
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimeType = "text/plain";
		FileValue variableValue = Variables.fileValue(filename).mimeType(mimeType).create();

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(""))).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
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

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
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

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

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

		when(runtimeServiceMock.getVariableLocalTyped(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).header("Content-Disposition", containsString(filename)).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotDownloadVariableOtherThanFile()
	  public virtual void testCannotDownloadVariableOtherThanFile()
	  {
		string variableKey = "aVariableKey";
		BooleanValue variableValue = Variables.booleanValue(true);

		when(runtimeServiceMock.getVariableLocalTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariable()
	  public virtual void testPutSingleLocalVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsUntypedValue.matcher().value(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeInteger()
	  public virtual void testPutSingleVariableWithTypeInteger()
	  {
		string variableKey = "aVariableKey";
		int? variableValue = 123;
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.integerValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableInteger()
	  public virtual void testPutSingleVariableWithUnparseableInteger()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Integer)))).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeShort()
	  public virtual void testPutSingleVariableWithTypeShort()
	  {
		string variableKey = "aVariableKey";
		short? variableValue = 123;
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.shortValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableShort()
	  public virtual void testPutSingleVariableWithUnparseableShort()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Short)))).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeLong()
	  public virtual void testPutSingleVariableWithTypeLong()
	  {
		string variableKey = "aVariableKey";
		long? variableValue = Convert.ToInt64(123);
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.longValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableLong()
	  public virtual void testPutSingleVariableWithUnparseableLong()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Long)))).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeDouble()
	  public virtual void testPutSingleVariableWithTypeDouble()
	  {
		string variableKey = "aVariableKey";
		double? variableValue = 123.456;
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.doubleValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDouble()
	  public virtual void testPutSingleVariableWithUnparseableDouble()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Double)))).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeBoolean()
	  public virtual void testPutSingleVariableWithTypeBoolean()
	  {
		string variableKey = "aVariableKey";
		bool? variableValue = true;
		string type = "Boolean";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.booleanValue(variableValue)));
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

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.dateValue(expectedValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDate()
	  public virtual void testPutSingleVariableWithUnparseableDate()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(DateTime)))).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNotSupportedType()
	  public virtual void testPutSingleVariableWithNotSupportedType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: Unsupported value type 'X'")).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
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
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).setVariableLocal(anyString(), anyString(), any());

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariable()
	  public virtual void testPutSingleLocalBinaryVariable()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableWithValueType()
	  public virtual void testPutSingleLocalBinaryVariableWithValueType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "Bytes", "text/plain").expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableWithUnknownValueType()
	  public virtual void testPutSingleLocalBinaryVariableWithUnknownValueType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "SomeUnknownType", "text/plain").expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Unsupported value type 'SomeUnknownType'")).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock, never()).setVariableLocal(anyString(), anyString(), any(typeof(object)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableWithValueTypeOfWrongMimeType()
	  public virtual void testPutSingleLocalBinaryVariableWithValueTypeOfWrongMimeType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "{ \"type\": \"Bytes\"", "application/json").expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Form part with name 'valueType' must have a text/plain value")).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock, never()).setVariableLocal(anyString(), anyString(), any(typeof(object)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableWithNoValue()
	  public virtual void testPutSingleLocalBinaryVariableWithNoValue()
	  {
		sbyte[] bytes = new sbyte[0];

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalSerializableVariableFromJson() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalSerializableVariableFromJson()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, MediaType.APPLICATION_JSON).multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().Deserialized.value(serializable)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalSerializableVariableUnsupportedMediaType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleLocalSerializableVariableUnsupportedMediaType()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, "unsupported").multiPart("type", typeName, MediaType.TEXT_PLAIN).expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Unrecognized content type for serialized java type: unsupported")).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		verify(runtimeServiceMock, never()).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), eq(serializable));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalBinaryVariableThrowsAuthorizationException()
	  public virtual void testPutSingleLocalBinaryVariableThrowsAuthorizationException()
	  {
		sbyte[] bytes = "someContent".GetBytes();
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).setVariableLocal(anyString(), anyString(), any());

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", "unspecified", bytes).expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableFromSerialized()
	  public virtual void testPutSingleLocalVariableFromSerialized()
	  {
		string serializedValue = "{\"prop\" : \"value\"}";
		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(serializedValue, ValueType.OBJECT.Name, "aDataFormat", "aRootType");

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().serializationFormat("aDataFormat").objectTypeName("aRootType").serializedValue(serializedValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableFromInvalidSerialized()
	  public virtual void testPutSingleLocalVariableFromInvalidSerialized()
	  {
		string serializedValue = "{\"prop\" : \"value\"}";

		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(serializedValue, "aNonExistingType", null, null);

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put execution variable aVariableKey: Unsupported value type 'aNonExistingType'")).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableFromSerializedWithNoValue()
	  public virtual void testPutSingleLocalVariableFromSerializedWithNoValue()
	  {
		string variableKey = "aVariableKey";

		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(null, ValueType.OBJECT.Name, null, null);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().serializationFormat(null).objectTypeName(null).serializedValue(null)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleLocalVariableWithNoValue()
	  public virtual void testPutSingleLocalVariableWithNoValue()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), argThat(EqualsNullValue.matcher()));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutLocalVariableForNonExistingExecution()
	  public virtual void testPutLocalVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		doThrow(new BadUserRequestException("expected exception")).when(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), any());

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot put execution variable " + variableKey + ": expected exception")).when().put(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleLocalVariable()
	  public virtual void testDeleteSingleLocalVariable()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);

		verify(runtimeServiceMock).removeVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableWithEncodingAndMimeType()
	  public virtual void testPostSingleLocalFileVariableWithEncodingAndMimeType()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype + "; encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(encoding));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableWithMimeType()
	  public virtual void testPostSingleLocalFileVariableWithMimeType()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(mimetype));
		assertThat(IoUtil.readInputStream(captured.Value, null), @is(value));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableWithEncoding()
	  public virtual void testPostSingleLocalFileVariableWithEncoding()
	  {

		sbyte[] value = "some text".GetBytes();
		string variableKey = "aVariableKey";
		string encoding = "utf-8";
		string filename = "test.txt";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, value, "encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleLocalFileVariableOnlyFilename() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleLocalFileVariableOnlyFilename()
	  {

		string variableKey = "aVariableKey";
		string filename = "test.txt";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).multiPart("data", filename, new sbyte[0]).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_EXECUTION_LOCAL_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(runtimeServiceMock).setVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(MediaType.APPLICATION_OCTET_STREAM));
		assertThat(captured.Value.available(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteLocalVariableForNonExistingExecution()
	  public virtual void testDeleteLocalVariableForNonExistingExecution()
	  {
		string variableKey = "aVariableKey";

		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).removeVariableLocal(eq(MockProvider.EXAMPLE_EXECUTION_ID), eq(variableKey));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot delete execution variable " + variableKey + ": expected exception")).when().delete(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteLocalVariableThrowsAuthorizationException()
	  public virtual void testDeleteLocalVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).removeVariableLocal(anyString(), anyString());

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().delete(SINGLE_EXECUTION_LOCAL_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetMessageEventSubscription()
	  public virtual void testGetMessageEventSubscription()
	  {
		string messageName = MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_NAME;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("messageName", messageName).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_ID)).body("eventType", equalTo(MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_TYPE)).body("eventName", equalTo(MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_NAME)).body("executionId", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("activityId", equalTo(MockProvider.EXAMPLE_ACTIVITY_ID)).body("createdDate", equalTo(MockProvider.EXAMPLE_EVENT_SUBSCRIPTION_CREATION_DATE)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).when().get(MESSAGE_SUBSCRIPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingMessageEventSubscription()
	  public virtual void testGetNonExistingMessageEventSubscription()
	  {
		EventSubscriptionQuery sampleEventSubscriptionQuery = mock(typeof(EventSubscriptionQuery));
		when(runtimeServiceMock.createEventSubscriptionQuery()).thenReturn(sampleEventSubscriptionQuery);
		when(sampleEventSubscriptionQuery.executionId(anyString())).thenReturn(sampleEventSubscriptionQuery);
		when(sampleEventSubscriptionQuery.eventName(anyString())).thenReturn(sampleEventSubscriptionQuery);
		when(sampleEventSubscriptionQuery.eventType(anyString())).thenReturn(sampleEventSubscriptionQuery);
		when(sampleEventSubscriptionQuery.singleResult()).thenReturn(null);

		string executionId = MockProvider.EXAMPLE_EXECUTION_ID;
		string nonExistingMessageName = "aMessage";

		given().pathParam("id", executionId).pathParam("messageName", nonExistingMessageName).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Message event subscription for execution " + executionId + " named " + nonExistingMessageName + " does not exist")).when().get(MESSAGE_SUBSCRIPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventTriggering()
	  public virtual void testMessageEventTriggering()
	  {
		string messageName = "aMessageName";
		string variableKey1 = "aVarName";
		string variableValue1 = "aVarValue";
		string variableKey2 = "anotherVarName";
		string variableValue2 = "anotherVarValue";

		IDictionary<string, object> variables = VariablesBuilder.create().variable(variableKey1, variableValue1).variable(variableKey2, variableValue2).Variables;

		IDictionary<string, object> variablesJson = new Dictionary<string, object>();
		variablesJson["variables"] = variables;

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("messageName", messageName).contentType(ContentType.JSON).body(variablesJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TRIGGER_MESSAGE_SUBSCRIPTION_URL);

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables[variableKey1] = variableValue1;
		expectedVariables[variableKey2] = variableValue2;

		verify(runtimeServiceMock).messageEventReceived(eq(messageName), eq(MockProvider.EXAMPLE_EXECUTION_ID), argThat(new EqualsMap(expectedVariables)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventTriggeringWithoutVariables()
	  public virtual void testMessageEventTriggeringWithoutVariables()
	  {
		string messageName = "aMessageName";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("messageName", messageName).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TRIGGER_MESSAGE_SUBSCRIPTION_URL);

		verify(runtimeServiceMock).messageEventReceived(eq(messageName), eq(MockProvider.EXAMPLE_EXECUTION_ID), argThat(new EqualsMap(null)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingMessageEventTriggering()
	  public virtual void testFailingMessageEventTriggering()
	  {
		string messageName = "someMessage";
		doThrow(new ProcessEngineException("expected exception")).when(runtimeServiceMock).messageEventReceived(anyString(), anyString(), any(typeof(System.Collections.IDictionary)));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("messageName", messageName).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot trigger message " + messageName + " for execution " + MockProvider.EXAMPLE_EXECUTION_ID + ": expected exception")).when().post(TRIGGER_MESSAGE_SUBSCRIPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageEventTriggeringThrowsAuthorizationException()
	  public virtual void testMessageEventTriggeringThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(runtimeServiceMock).messageEventReceived(anyString(), anyString(), any(typeof(System.Collections.IDictionary)));

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("messageName", "someMessage").contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().post(TRIGGER_MESSAGE_SUBSCRIPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateIncident()
	  public virtual void testCreateIncident()
	  {
		when(runtimeServiceMock.createIncident(anyString(), anyString(), anyString(), anyString())).thenReturn(mock(typeof(Incident)));
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["incidentType"] = "incidentType";
		json["configuration"] = "configuration";
		json["message"] = "message";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CREATE_INCIDENT_URL);

		verify(runtimeServiceMock).createIncident("incidentType", MockProvider.EXAMPLE_EXECUTION_ID, "configuration", "message");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateIncidentWithNullIncidentType()
	  public virtual void testCreateIncidentWithNullIncidentType()
	  {
		doThrow(new BadUserRequestException()).when(runtimeServiceMock).createIncident(anyString(), anyString(), anyString(), anyString());
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["configuration"] = "configuration";
		json["message"] = "message";

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).contentType(ContentType.JSON).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(CREATE_INCIDENT_URL);
	  }
	}
}