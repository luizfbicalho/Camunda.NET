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
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.NON_EXISTING_ID;
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



	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using EqualsList = org.camunda.bpm.engine.rest.helper.EqualsList;
	using EqualsMap = org.camunda.bpm.engine.rest.helper.EqualsMap;
	using ErrorMessageHelper = org.camunda.bpm.engine.rest.helper.ErrorMessageHelper;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using EqualsNullValue = org.camunda.bpm.engine.rest.helper.variable.EqualsNullValue;
	using EqualsObjectValue = org.camunda.bpm.engine.rest.helper.variable.EqualsObjectValue;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using EqualsUntypedValue = org.camunda.bpm.engine.rest.helper.variable.EqualsUntypedValue;
	using VariablesBuilder = org.camunda.bpm.engine.rest.util.VariablesBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
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

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TaskVariableRestResourceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string TASK_SERVICE_URL = TEST_RESOURCE_ROOT_PATH + "/task";
	  protected internal static readonly string SINGLE_TASK_URL = TASK_SERVICE_URL + "/{id}";

	  protected internal static readonly string SINGLE_TASK_VARIABLES_URL = SINGLE_TASK_URL + "/variables";
	  protected internal static readonly string SINGLE_TASK_SINGLE_VARIABLE_URL = SINGLE_TASK_VARIABLES_URL + "/{varId}";
	  protected internal static readonly string SINGLE_TASK_PUT_SINGLE_VARIABLE_URL = SINGLE_TASK_SINGLE_VARIABLE_URL;
	  protected internal static readonly string SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL = SINGLE_TASK_PUT_SINGLE_VARIABLE_URL + "/data";
	  protected internal static readonly string SINGLE_TASK_DELETE_SINGLE_VARIABLE_URL = SINGLE_TASK_SINGLE_VARIABLE_URL;
	  protected internal static readonly string SINGLE_TASK_MODIFY_VARIABLES_URL = SINGLE_TASK_VARIABLES_URL;

	  protected internal TaskService taskServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		taskServiceMock = mock(typeof(TaskService));
		when(processEngine.TaskService).thenReturn(taskServiceMock);
	  }

	  private TaskServiceImpl mockTaskServiceImpl()
	  {
		TaskServiceImpl taskServiceMock = mock(typeof(TaskServiceImpl));
		when(processEngine.TaskService).thenReturn(taskServiceMock);
		return taskServiceMock;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariables()
	  public virtual void testGetVariables()
	  {

		when(taskServiceMock.getVariablesTyped(EXAMPLE_TASK_ID, true)).thenReturn(EXAMPLE_VARIABLES);

		Response response = given().pathParam("id", EXAMPLE_TASK_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body(EXAMPLE_VARIABLE_KEY, notNullValue()).body(EXAMPLE_VARIABLE_KEY + ".value", equalTo(EXAMPLE_VARIABLE_VALUE.Value)).body(EXAMPLE_VARIABLE_KEY + ".type", equalTo(VariableTypeHelper.toExpectedValueTypeName(EXAMPLE_VARIABLE_VALUE.Type))).when().get(SINGLE_TASK_VARIABLES_URL);

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

		when(taskServiceMock.getVariablesTyped(eq(EXAMPLE_TASK_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo(payload)).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_TASK_VARIABLES_URL);

		// then
		verify(taskServiceMock).getVariablesTyped(EXAMPLE_TASK_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetObjectVariablesSerialized()
	  public virtual void testGetObjectVariablesSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(taskServiceMock.getVariablesTyped(eq(EXAMPLE_TASK_ID), anyBoolean())).thenReturn(Variables.createVariables().putValueTyped(variableKey, variableValue));

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_TASK_ID).queryParam("deserializeValues", false).then().expect().statusCode(Status.OK.StatusCode).body(variableKey + ".value", equalTo("a serialized value")).body(variableKey + ".type", equalTo("Object")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body(variableKey + ".valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_TASK_VARIABLES_URL);

		// then
		verify(taskServiceMock).getVariablesTyped(EXAMPLE_TASK_ID, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesForNonExistingTaskId()
	  public virtual void testGetVariablesForNonExistingTaskId()
	  {
		when(taskServiceMock.getVariablesTyped(NON_EXISTING_ID, true)).thenThrow(new ProcessEngineException("task " + NON_EXISTING_ID + " doesn't exist"));

		given().pathParam("id", NON_EXISTING_ID).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("task " + NON_EXISTING_ID + " doesn't exist")).when().get(SINGLE_TASK_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariablesThrowsAuthorizationException()
	  public virtual void testGetVariablesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(taskServiceMock.getVariablesTyped(anyString(), anyBoolean())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", EXAMPLE_TASK_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(SINGLE_TASK_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModification()
	  public virtual void testVariableModification()
	  {
		TaskServiceImpl taskServiceMock = mockTaskServiceImpl();

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;

		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		IList<string> deletions = new List<string>();
		deletions.Add("deleteKey");
		messageBodyJson["deletions"] = deletions;

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(ContentType.JSON).body(messageBodyJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_MODIFY_VARIABLES_URL);

		IDictionary<string, object> expectedModifications = new Dictionary<string, object>();
		expectedModifications[variableKey] = variableValue;
		verify(taskServiceMock).updateVariables(eq(EXAMPLE_TASK_ID), argThat(new EqualsMap(expectedModifications)), argThat(new EqualsList(deletions)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableModificationForNonExistingTaskId()
	  public virtual void testVariableModificationForNonExistingTaskId()
	  {
		TaskServiceImpl taskServiceMock = mockTaskServiceImpl();
		doThrow(new ProcessEngineException("Cannot find task with id " + NON_EXISTING_ID)).when(taskServiceMock).updateVariables(anyString(), any(typeof(System.Collections.IDictionary)), any(typeof(System.Collections.IList)));

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();

		string variableKey = "aKey";
		int variableValue = 123;
		IDictionary<string, object> modifications = VariablesBuilder.create().variable(variableKey, variableValue).Variables;
		messageBodyJson["modifications"] = modifications;

		given().pathParam("id", NON_EXISTING_ID).contentType(ContentType.JSON).body(messageBodyJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Cannot modify variables for task " + NON_EXISTING_ID + ": Cannot find task with id " + NON_EXISTING_ID)).when().post(SINGLE_TASK_MODIFY_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyVariableModification()
	  public virtual void testEmptyVariableModification()
	  {
		mockTaskServiceImpl();

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_MODIFY_VARIABLES_URL);
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

		TaskServiceImpl taskServiceMock = mockTaskServiceImpl();
		string message = "excpected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).updateVariables(anyString(), any(typeof(System.Collections.IDictionary)), any(typeof(System.Collections.IList)));

		given().pathParam("id", EXAMPLE_TASK_ID).contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().post(SINGLE_TASK_MODIFY_VARIABLES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariable()
	  public virtual void testGetSingleVariable()
	  {
		string variableKey = "aVariableKey";
		int variableValue = 123;

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(Variables.integerValue(variableValue));

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).body("value", @is(123)).body("type", @is("Integer")).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableData()
	  public virtual void testGetSingleVariableData()
	  {

		when(taskServiceMock.getVariableTyped(anyString(), eq(EXAMPLE_BYTES_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE_BYTES);

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", EXAMPLE_BYTES_VARIABLE_KEY).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).getVariableTyped(MockProvider.EXAMPLE_TASK_ID, EXAMPLE_BYTES_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableDataNonExisting()
	  public virtual void testGetSingleVariableDataNonExisting()
	  {

		when(taskServiceMock.getVariableTyped(anyString(), eq("nonExisting"), eq(false))).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", "nonExisting").then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("task variable with name " + "nonExisting" + " does not exist")).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).getVariableTyped(MockProvider.EXAMPLE_TASK_ID, "nonExisting", false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariabledataNotBinary()
	  public virtual void testGetSingleVariabledataNotBinary()
	  {

		when(taskServiceMock.getVariableTyped(anyString(), eq(EXAMPLE_VARIABLE_KEY), eq(false))).thenReturn(EXAMPLE_VARIABLE_VALUE);

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", EXAMPLE_VARIABLE_KEY).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).getVariableTyped(MockProvider.EXAMPLE_TASK_ID, EXAMPLE_VARIABLE_KEY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleObjectVariable()
	  public virtual void testGetSingleObjectVariable()
	  {
		// given
		string variableKey = "aVariableId";

		IList<string> payload = Arrays.asList("a", "b");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = MockObjectValue.fromObjectValue(Variables.objectValue(payload).serializationDataFormat("application/json").create()).objectTypeName(typeof(List<object>).FullName).serializedValue("a serialized value"); // this should differ from the serialized json

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo(payload)).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);

		// then
		verify(taskServiceMock).getVariableTyped(EXAMPLE_TASK_ID, variableKey, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleObjectVariableSerialized()
	  public virtual void testGetSingleObjectVariableSerialized()
	  {
		// given
		string variableKey = "aVariableId";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue variableValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("application/json").objectTypeName(typeof(List<object>).FullName).create();

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		// when
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).body("value", equalTo("a serialized value")).body("type", equalTo("Object")).body("valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("application/json")).body("valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(List<object>).FullName)).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);

		// then
		verify(taskServiceMock).getVariableTyped(EXAMPLE_TASK_ID, variableKey, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingVariable()
	  public virtual void testNonExistingVariable()
	  {
		string variableKey = "aVariableKey";

		when(taskServiceMock.getVariable(eq(EXAMPLE_TASK_ID), eq(variableKey))).thenReturn(null);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is("task variable with name " + variableKey + " does not exist")).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetVariableForNonExistingTaskId()
	  public virtual void testGetVariableForNonExistingTaskId()
	  {
		string variableKey = "aVariableKey";

		when(taskServiceMock.getVariableTyped(eq(NON_EXISTING_ID), eq(variableKey), anyBoolean())).thenThrow(new ProcessEngineException("task " + NON_EXISTING_ID + " doesn't exist"));

		given().pathParam("id", NON_EXISTING_ID).pathParam("varId", variableKey).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot get task variable " + variableKey + ": task " + NON_EXISTING_ID + " doesn't exist")).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableThrowsAuthorizationException()
	  public virtual void testGetSingleVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";

		string message = "excpected exception";
		when(taskServiceMock.getVariableTyped(anyString(), anyString(), anyBoolean())).thenThrow(new AuthorizationException(message));

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);
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

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON.ToString()).and().body("valueInfo.mimeType", equalTo(mimeType)).body("valueInfo.filename", equalTo(filename)).body("value", nullValue()).when().get(SINGLE_TASK_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNullFileVariable()
	  public virtual void testGetNullFileVariable()
	  {
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimeType = "text/plain";
		FileValue variableValue = Variables.fileValue(filename).mimeType(mimeType).create();

		when(taskServiceMock.getVariableTyped(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(""))).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);
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

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT.ToString()).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);
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

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		Response response = given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

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

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_OCTET_STREAM).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).header("Content-Disposition", containsString(filename)).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotDownloadVariableOtherThanFile()
	  public virtual void testCannotDownloadVariableOtherThanFile()
	  {
		string variableKey = "aVariableKey";
		BooleanValue variableValue = Variables.booleanValue(true);

		when(taskServiceMock.getVariableTyped(eq(EXAMPLE_TASK_ID), eq(variableKey), anyBoolean())).thenReturn(variableValue);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariable()
	  public virtual void testPutSingleVariable()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsUntypedValue.matcher().value(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeInteger()
	  public virtual void testPutSingleVariableWithTypeInteger()
	  {
		string variableKey = "aVariableKey";
		int? variableValue = 123;
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.integerValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableInteger()
	  public virtual void testPutSingleVariableWithUnparseableInteger()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Integer";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable " + variableKey + ": " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Integer)))).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeShort()
	  public virtual void testPutSingleVariableWithTypeShort()
	  {
		string variableKey = "aVariableKey";
		short? variableValue = 123;
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.shortValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableShort()
	  public virtual void testPutSingleVariableWithUnparseableShort()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Short";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable " + variableKey + ": " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Short)))).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeLong()
	  public virtual void testPutSingleVariableWithTypeLong()
	  {
		string variableKey = "aVariableKey";
		long? variableValue = Convert.ToInt64(123);
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.longValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableLong()
	  public virtual void testPutSingleVariableWithUnparseableLong()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Long";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", MockProvider.EXAMPLE_EXECUTION_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable " + variableKey + ": " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Long)))).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeDouble()
	  public virtual void testPutSingleVariableWithTypeDouble()
	  {
		string variableKey = "aVariableKey";
		double? variableValue = 123.456;
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.doubleValue(variableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDouble()
	  public virtual void testPutSingleVariableWithUnparseableDouble()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Double";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable " + variableKey + ": " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(Double)))).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithTypeBoolean()
	  public virtual void testPutSingleVariableWithTypeBoolean()
	  {
		string variableKey = "aVariableKey";
		bool? variableValue = true;
		string type = "Boolean";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.booleanValue(variableValue)));
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

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.dateValue(expectedValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithUnparseableDate()
	  public virtual void testPutSingleVariableWithUnparseableDate()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "Date";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable " + variableKey + ": " + ErrorMessageHelper.getExpectedFailingConversionMessage(variableValue, type, typeof(DateTime)))).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNotSupportedType()
	  public virtual void testPutSingleVariableWithNotSupportedType()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "1abc";
		string type = "X";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue, type);

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable " + variableKey + ": Unsupported value type 'X'")).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableWithNoValue()
	  public virtual void testPutSingleVariableWithNoValue()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsNullValue.matcher()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutVariableForNonExistingTaskId()
	  public virtual void testPutVariableForNonExistingTaskId()
	  {
		string variableKey = "aVariableKey";
		string variableValue = "aVariableValue";

		IDictionary<string, object> variableJson = VariablesBuilder.getVariableValueMap(variableValue);

		doThrow(new ProcessEngineException("Cannot find task with id " + NON_EXISTING_ID)).when(taskServiceMock).setVariable(eq(NON_EXISTING_ID), eq(variableKey), any());

		given().pathParam("id", NON_EXISTING_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot put task variable " + variableKey + ": Cannot find task with id " + NON_EXISTING_ID)).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
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
		doThrow(new AuthorizationException(message)).when(taskServiceMock).setVariable(anyString(), anyString(), any());

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(variableJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleBinaryVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleBinaryVariable()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleBinaryVariableWithValueType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleBinaryVariableWithValueType()
	  {
		sbyte[] bytes = "someContent".GetBytes();

		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).multiPart("valueType", "Bytes", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleBinaryVariableWithNoValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleBinaryVariableWithNoValue()
	  {
		sbyte[] bytes = new sbyte[0];

		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", null, bytes).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsPrimitiveValue.bytesValue(bytes)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleBinaryVariableThrowsAuthorizationException()
	  public virtual void testPutSingleBinaryVariableThrowsAuthorizationException()
	  {
		sbyte[] bytes = "someContent".GetBytes();
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).setVariable(anyString(), anyString(), any());

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", "unspecified", bytes).expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleSerializableVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleSerializableVariable()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, MediaType.APPLICATION_JSON).multiPart("type", typeName, MediaType.TEXT_PLAIN).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().Deserialized.value(serializable)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleSerializableVariableUnsupportedMediaType() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleSerializableVariableUnsupportedMediaType()
	  {

		List<string> serializable = new List<string>();
		serializable.Add("foo");

		ObjectMapper mapper = new ObjectMapper();
		string jsonBytes = mapper.writeValueAsString(serializable);
		string typeName = TypeFactory.defaultInstance().constructType(serializable.GetType()).toCanonical();

		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", jsonBytes, "unsupported").multiPart("type", typeName, MediaType.TEXT_PLAIN).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Unrecognized content type for serialized java type: unsupported")).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		verify(taskServiceMock, never()).setVariable(eq(EXAMPLE_TASK_ID), eq(variableKey), eq(serializable));
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

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype + "; encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(taskServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), captor.capture());
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
		string base64 = Base64.encodeBase64String(value);
		string variableKey = "aVariableKey";
		string filename = "test.txt";
		string mimetype = MediaType.TEXT_PLAIN;

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, value, mimetype).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(taskServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), captor.capture());
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

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, value, "encoding=" + encoding).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPostSingleFileVariableOnlyFilename() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPostSingleFileVariableOnlyFilename()
	  {

		string variableKey = "aVariableKey";
		string filename = "test.txt";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).multiPart("data", filename, new sbyte[0]).multiPart("valueType", "File", "text/plain").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(SINGLE_TASK_SINGLE_BINARY_VARIABLE_URL);

		ArgumentCaptor<FileValue> captor = ArgumentCaptor.forClass(typeof(FileValue));
		verify(taskServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), captor.capture());
		FileValue captured = captor.Value;
		assertThat(captured.Encoding, @is(nullValue()));
		assertThat(captured.Filename, @is(filename));
		assertThat(captured.MimeType, @is(MediaType.APPLICATION_OCTET_STREAM));
		assertThat(captured.Value.available(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableFromSerialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleVariableFromSerialized()
	  {
		string serializedValue = "{\"prop\" : \"value\"}";
		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(serializedValue, ValueType.OBJECT.Name, "aDataFormat", "aRootType");

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher().serializedValue(serializedValue).serializationFormat("aDataFormat").objectTypeName("aRootType")));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableFromInvalidSerialized() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPutSingleVariableFromInvalidSerialized()
	  {
		string serializedValue = "{\"prop\" : \"value\"}";

		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(serializedValue, "aNonExistingType", null, null);

		string variableKey = "aVariableKey";

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot put task variable aVariableKey: Unsupported value type 'aNonExistingType'")).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutSingleVariableFromSerializedWithNoValue()
	  public virtual void testPutSingleVariableFromSerializedWithNoValue()
	  {
		string variableKey = "aVariableKey";

		IDictionary<string, object> requestJson = VariablesBuilder.getObjectValueMap(null, ValueType.OBJECT.Name, null, null);

		given().pathParam("id", MockProvider.EXAMPLE_TASK_ID).pathParam("varId", variableKey).contentType(ContentType.JSON).body(requestJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_TASK_PUT_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).setVariable(eq(MockProvider.EXAMPLE_TASK_ID), eq(variableKey), argThat(EqualsObjectValue.objectValueMatcher()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleVariable()
	  public virtual void testDeleteSingleVariable()
	  {
		string variableKey = "aVariableKey";

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_TASK_DELETE_SINGLE_VARIABLE_URL);

		verify(taskServiceMock).removeVariable(eq(EXAMPLE_TASK_ID), eq(variableKey));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteVariableForNonExistingTaskId()
	  public virtual void testDeleteVariableForNonExistingTaskId()
	  {
		string variableKey = "aVariableKey";

		doThrow(new ProcessEngineException("Cannot find task with id " + NON_EXISTING_ID)).when(taskServiceMock).removeVariable(eq(NON_EXISTING_ID), eq(variableKey));

		given().pathParam("id", NON_EXISTING_ID).pathParam("varId", variableKey).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(RestException).Name)).body("message", @is("Cannot delete task variable " + variableKey + ": Cannot find task with id " + NON_EXISTING_ID)).when().delete(SINGLE_TASK_DELETE_SINGLE_VARIABLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteVariableThrowsAuthorizationException()
	  public virtual void testDeleteVariableThrowsAuthorizationException()
	  {
		string variableKey = "aVariableKey";

		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(taskServiceMock).removeVariable(anyString(), anyString());

		given().pathParam("id", EXAMPLE_TASK_ID).pathParam("varId", variableKey).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(message)).when().delete(SINGLE_TASK_DELETE_SINGLE_VARIABLE_URL);
	  }

	}

}