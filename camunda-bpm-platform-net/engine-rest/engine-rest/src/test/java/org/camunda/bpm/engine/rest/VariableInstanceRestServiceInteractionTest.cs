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
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.either;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using MockVariableInstanceBuilder = org.camunda.bpm.engine.rest.helper.MockVariableInstanceBuilder;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class VariableInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string SERVICE_URL = TEST_RESOURCE_ROOT_PATH + "/variable-instance";
	  protected internal static readonly string VARIABLE_INSTANCE_URL = SERVICE_URL + "/{id}";
	  protected internal static readonly string VARIABLE_INSTANCE_BINARY_DATA_URL = VARIABLE_INSTANCE_URL + "/data";

	  protected internal RuntimeService runtimeServiceMock;

	  protected internal VariableInstanceQuery variableInstanceQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupTestData()
	  public virtual void setupTestData()
	  {
		runtimeServiceMock = mock(typeof(RuntimeService));
		variableInstanceQueryMock = mock(typeof(VariableInstanceQuery));

		// mock runtime service.
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);
		when(runtimeServiceMock.createVariableInstanceQuery()).thenReturn(variableInstanceQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstance()
	  public virtual void testGetSingleVariableInstance()
	  {

		MockVariableInstanceBuilder builder = MockProvider.mockVariableInstance();
		VariableInstance variableInstanceMock = builder.build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("name", equalTo(builder.Name)).body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo(builder.TypedValue.Value)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("executionId", equalTo(builder.ExecutionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("taskId", equalTo(builder.TaskId)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("tenantId", equalTo(builder.TenantId)).body("errorMessage", equalTo(builder.ErrorMessage)).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceDeserialized()
	  public virtual void testGetSingleVariableInstanceDeserialized()
	  {
		ObjectValue serializedValue = MockObjectValue.fromObjectValue(Variables.objectValue("a value").serializationDataFormat("aDataFormat").create()).objectTypeName("aTypeName");

		MockVariableInstanceBuilder builder = MockProvider.mockVariableInstance().typedValue(serializedValue);
		VariableInstance variableInstanceMock = builder.build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("name", equalTo(builder.Name)).body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo("a value")).body("valueInfo.serializationDataFormat", equalTo("aDataFormat")).body("valueInfo.objectTypeName", equalTo("aTypeName")).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("executionId", equalTo(builder.ExecutionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("taskId", equalTo(builder.TaskId)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("tenantId", equalTo(builder.TenantId)).body("errorMessage", equalTo(builder.ErrorMessage)).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
		verify(variableInstanceQueryMock, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceSerialized()
	  public virtual void testGetSingleVariableInstanceSerialized()
	  {
		ObjectValue serializedValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("aDataFormat").objectTypeName("aTypeName").create();

		MockVariableInstanceBuilder builder = MockProvider.mockVariableInstance().typedValue(serializedValue);
		VariableInstance variableInstanceMock = builder.build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("name", equalTo(builder.Name)).body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo("a serialized value")).body("valueInfo.serializationDataFormat", equalTo("aDataFormat")).body("valueInfo.objectTypeName", equalTo("aTypeName")).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("executionId", equalTo(builder.ExecutionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("taskId", equalTo(builder.TaskId)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("tenantId", equalTo(builder.TenantId)).body("errorMessage", equalTo(builder.ErrorMessage)).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
		verify(variableInstanceQueryMock, times(1)).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceForBinaryVariable()
	  public virtual void testGetSingleVariableInstanceForBinaryVariable()
	  {
		MockVariableInstanceBuilder builder = MockProvider.mockVariableInstance();
		VariableInstance variableInstanceMock = builder.typedValue(Variables.byteArrayValue(null)).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(ValueType.BYTES))).body("value", nullValue()).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingVariableInstance()
	  public virtual void testGetNonExistingVariableInstance()
	  {

		string nonExistingId = "nonExistingId";

		when(variableInstanceQueryMock.variableId(nonExistingId)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Variable instance with Id 'nonExistingId' does not exist.")).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForBinaryVariable()
	  public virtual void testBinaryDataForBinaryVariable()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();

		VariableInstance variableInstanceMock = MockProvider.mockVariableInstance().typedValue(Variables.byteArrayValue(byteContent)).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.BINARY.ToString()).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		sbyte[] responseBytes = response.Body.asByteArray();
		Assert.assertEquals(StringHelper.NewString(byteContent), StringHelper.NewString(responseBytes));
		verify(variableInstanceQueryMock, never()).disableBinaryFetching();
		verify(variableInstanceQueryMock).disableCustomObjectDeserialization();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForNonBinaryVariable()
	  public virtual void testBinaryDataForNonBinaryVariable()
	  {
		VariableInstance variableInstanceMock = MockProvider.createMockVariableInstance();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Value of variable with id aVariableInstanceId is not a binary value")).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		verify(variableInstanceQueryMock, never()).disableBinaryFetching();
		verify(variableInstanceQueryMock).disableCustomObjectDeserialization();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForNonExistingVariableInstance()
	  public virtual void testGetBinaryDataForNonExistingVariableInstance()
	  {

		string nonExistingId = "nonExistingId";

		when(variableInstanceQueryMock.variableId(nonExistingId)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Variable instance with Id 'nonExistingId' does not exist.")).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		verify(variableInstanceQueryMock, never()).disableBinaryFetching();
		verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForFileVariable()
	  public virtual void testGetBinaryDataForFileVariable()
	  {
		string filename = "test.txt";
		sbyte[] byteContent = "test".GetBytes();
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();

		MockVariableInstanceBuilder builder = MockProvider.mockVariableInstance();
		VariableInstance variableInstanceMock = builder.typedValue(variableValue).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().header("Content-Disposition", "attachment; filename=" + filename).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);
		//due to some problems with wildfly we gotta check this separately
		string contentType = response.ContentType;
		assertThat(contentType, @is(either(CoreMatchers.equalTo<object>(ContentType.TEXT.ToString() + "; charset=UTF-8")).or(CoreMatchers.equalTo<object>(ContentType.TEXT.ToString() + ";charset=UTF-8"))));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForNullFileVariable()
	  public virtual void testGetBinaryDataForNullFileVariable()
	  {
		string filename = "test.txt";
		sbyte[] byteContent = null;
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).create();

		MockVariableInstanceBuilder builder = MockProvider.mockVariableInstance();
		VariableInstance variableInstanceMock = builder.typedValue(variableValue).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().contentType(ContentType.TEXT).and().body(@is(equalTo(""))).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

	  }

	}

}